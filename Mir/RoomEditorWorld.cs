namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Jitter;
    using Jitter.Collision;
    using Jitter.Collision.Shapes;
    using Jitter.LinearMath;
    using Jitter.Dynamics;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class RoomEditorWorld : IWorld
    {
        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly IAssetManager m_AssetManager;

        private readonly ITool[] m_Tools;

        private readonly IPhysicsEngine m_PhysicsEngine;

        private readonly RoomEditorEntity m_RoomEditorEntity;

        private readonly FontAsset m_DefaultFont;

        private readonly EffectAsset m_LightingEffect;

        private readonly Room m_Room;

        private int m_ActiveTool;

        private JitterWorld m_JitterWorld;

        private RigidBody m_RigidBody;

        public RoomEditorWorld(
            IFactory factory, 
            I2DRenderUtilities twoDRenderUtilities, 
            IAssetManagerProvider assetManagerProvider, 
            IPhysicsEngine physicsEngine,
            ITool[] tools)
        {
            this.Entities = new List<IEntity>();

            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_Tools = tools;
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");
            this.m_LightingEffect = this.m_AssetManager.Get<EffectAsset>("effect.Light");

            this.m_LightingEffect.Effect.Parameters["Ambient"].SetValue(new Vector3(0.2f, 0.2f, 0.2f));
            this.m_LightingEffect.Effect.Parameters["AmbientRemaining"].SetValue(new Vector3(0.8f, 0.8f, 0.8f));

            var ship = factory.CreateShipEntity();
            var player = factory.CreatePlayerEntity();
            player.ParentArea = ship;
            player.X = 20;
            player.Z = 30;

            var dcpu = factory.CreateDCPUEntity();
            dcpu.X = 20;
            dcpu.Y = 6;
            dcpu.Z = 20;

            this.m_Room = factory.CreateRoom();

            this.m_RoomEditorEntity = factory.CreateRoomEditorEntity(this.m_Room);

            this.Entities.Add(ship);
            this.Entities.Add(player);
            this.Entities.Add(dcpu);

            this.Entities.Add(this.m_RoomEditorEntity);

            this.m_PhysicsEngine = physicsEngine;

            this.m_JitterWorld = new JitterWorld(new CollisionSystemSAP() { EnableSpeculativeContacts = true });
            this.m_JitterWorld.Gravity = new JVector(0, -50, 0);
            this.m_JitterWorld.ContactSettings.MaterialCoefficientMixing = ContactSettings.MaterialCoefficientMixingType.TakeMinimum;

            var shape = new BoxShape(new JVector(2, 2, 2));
            var body = new RigidBody(shape);
            this.m_JitterWorld.AddBody(body);
            body.Position = new JVector(20, 10, 20);
            body.IsActive = true;
            this.m_RigidBody = body;

            this.SetupRoomPhysics();
        }

        public IList<IEntity> Entities { get; private set; }

        public JitterWorld JitterWorld
        {
            get
            {
                return this.m_JitterWorld;
            }
        }

        public ITool ActiveTool
        {
            get
            {
                return this.m_Tools[this.m_ActiveTool];
            }
        }

        public void Dispose()
        {
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                renderContext.PopEffect();

                return;
            }

            for (var i = 0; i < this.m_Tools.Length; i++)
            {
                var tool = this.m_Tools[i];

                var texture = this.m_AssetManager.Get<TextureAsset>(tool.TextureName);

                this.m_2DRenderUtilities.RenderRectangle(
                    renderContext, 
                    new Rectangle(16, 16 + (i * 32), 24, 24), 
                    this.m_ActiveTool == i ? Color.Orange : Color.Black, 
                    true);

                if (this.m_ActiveTool == i)
                {
                    this.m_2DRenderUtilities.RenderText(
                        renderContext,
                        new Vector2(16 + 32, 16 + (i * 32) + 5),
                        this.m_Tools[this.m_ActiveTool].Name,
                        this.m_DefaultFont);
                }

                this.m_2DRenderUtilities.RenderTexture(renderContext, new Vector2(20, 20 + (i * 32)), texture);
            }
        }

        public void RenderBelow(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            renderContext.GraphicsDevice.Clear(Color.Black);

            var player = this.Entities.OfType<PlayerEntity>().First();
            var playerPos = new Vector3(player.X, player.Y, player.Z);

            var emptyLight = new EmptyLight();
            var allLights = this.Entities.OfType<ILight>().OrderBy(x => (playerPos - x.LightPosition).LengthSquared()).ToArray();
            var lights = new ILight[]
            {
                allLights.Length > 0 ? allLights[0] : emptyLight, allLights.Length > 1 ? allLights[1] : emptyLight,
                allLights.Length > 2 ? allLights[2] : emptyLight
            };

            this.m_LightingEffect.Effect.Parameters["LightColours"].SetValue(new Matrix(
                lights[0].LightColor.R / 255f, lights[0].LightColor.G / 255f, lights[0].LightColor.B / 255f, 1,
                lights[1].LightColor.R / 255f, lights[1].LightColor.G / 255f, lights[1].LightColor.B / 255f, 1,
                lights[2].LightColor.R / 255f, lights[2].LightColor.G / 255f, lights[2].LightColor.B / 255f, 1,
                1, 1, 1, 1));

            this.m_LightingEffect.Effect.Parameters["Lights"].SetValue(new Matrix(
                lights[0].LightPosition.X, lights[0].LightPosition.Y, lights[0].LightPosition.Z, lights[0].LightDistance,
                lights[1].LightPosition.X, lights[1].LightPosition.Y, lights[1].LightPosition.Z, lights[1].LightDistance,
                lights[2].LightPosition.X, lights[2].LightPosition.Y, lights[2].LightPosition.Z, lights[2].LightDistance,
                1, 1, 1, 1));

            renderContext.PushEffect(this.m_LightingEffect.Effect);

            player.SetCamera(renderContext);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var dcpu = this.Entities.OfType<DCPUEntity>().First();
            dcpu.X = this.m_RigidBody.Position.X;
            dcpu.Y = this.m_RigidBody.Position.Y;
            dcpu.Z = this.m_RigidBody.Position.Z;
            dcpu.Rotation = this.m_RigidBody.Orientation.ToXNAMatrix();

            this.m_PhysicsEngine.UpdateWorld(this.m_JitterWorld, gameContext, updateContext);

            // TODO: Do this through the event system
            var mouse = Mouse.GetState();
            var value = mouse.ScrollWheelValue;
            while (value < 0)
            {
                value += 120 * this.m_Tools.Length;
            }

            this.m_ActiveTool = this.m_Tools.Length - ((value / 120) % this.m_Tools.Length);
            if (this.m_ActiveTool == this.m_Tools.Length)
            {
                this.m_ActiveTool = 0;
            }
        }

        public void UseTool(IGameContext gameContext, bool secondaryAlt)
        {
            this.m_RoomEditorEntity.SelectCurrentHover(gameContext, secondaryAlt);
        }

        public void ReleaseTool()
        {
            this.m_RoomEditorEntity.ReleaseCurrentSelection();
        }

        public void ActivateAlternate()
        {
            this.m_RoomEditorEntity.UseAlternative = true;
        }

        public void DeactivateAlternate()
        {
            this.m_RoomEditorEntity.UseAlternative = false;
        }

        public void TestPhysics()
        {
            this.m_RigidBody.ApplyImpulse(new JVector(0, 15, 0));
        }

        private void SetupRoomPhysics()
        {
            // Set up the static physics bodies for each wall.
            var floorShape = new BoxShape(new JVector(this.m_Room.Width, 1, this.m_Room.Depth));
            var floorBody = new RigidBody(floorShape);
            this.m_JitterWorld.AddBody(floorBody);
            floorBody.Position = new JVector(
                this.m_Room.X + (this.m_Room.Width / 2f),
                this.m_Room.Y - 0.5f,
                this.m_Room.Z + (this.m_Room.Depth / 2f));
            floorBody.IsStatic = true;

            var roofShape = new BoxShape(new JVector(this.m_Room.Width, 1, this.m_Room.Depth));
            var roofBody = new RigidBody(roofShape);
            this.m_JitterWorld.AddBody(roofBody);
            roofBody.Position = new JVector(
                this.m_Room.X + (this.m_Room.Width / 2f),
                this.m_Room.Y + this.m_Room.Height + 0.5f,
                this.m_Room.Z + (this.m_Room.Depth / 2f));
            roofBody.IsStatic = true;

            var leftShape = new BoxShape(new JVector(1, this.m_Room.Height, this.m_Room.Depth));
            var leftBody = new RigidBody(leftShape);
            this.m_JitterWorld.AddBody(leftBody);
            leftBody.Position = new JVector(
                this.m_Room.X - 0.5f,
                this.m_Room.Y + (this.m_Room.Height / 2f),
                this.m_Room.Z + (this.m_Room.Depth / 2f));
            leftBody.IsStatic = true;

            var rightShape = new BoxShape(new JVector(1, this.m_Room.Height, this.m_Room.Depth));
            var rightBody = new RigidBody(rightShape);
            this.m_JitterWorld.AddBody(rightBody);
            rightBody.Position = new JVector(
                this.m_Room.X + this.m_Room.Width + 0.5f,
                this.m_Room.Y + (this.m_Room.Height / 2f),
                this.m_Room.Z + (this.m_Room.Depth / 2f));
            rightBody.IsStatic = true;

            var backShape = new BoxShape(new JVector(this.m_Room.Width, this.m_Room.Height, 1));
            var backBody = new RigidBody(backShape);
            this.m_JitterWorld.AddBody(backBody);
            backBody.Position = new JVector(
                this.m_Room.X + (this.m_Room.Width / 2f),
                this.m_Room.Y + (this.m_Room.Height / 2f),
                this.m_Room.Z - 0.5f);
            backBody.IsStatic = true;

            var frontShape = new BoxShape(new JVector(this.m_Room.Width, this.m_Room.Height, 1));
            var frontBody = new RigidBody(frontShape);
            this.m_JitterWorld.AddBody(frontBody);
            frontBody.Position = new JVector(
                this.m_Room.X + (this.m_Room.Width / 2f),
                this.m_Room.Y + (this.m_Room.Height / 2f),
                this.m_Room.Z + this.m_Room.Depth + 0.5f);
            frontBody.IsStatic = true;
        }
    }
}
