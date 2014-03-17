namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
    using Jitter;
    using Jitter.Collision;
    using Jitter.Collision.Shapes;
    using Jitter.Dynamics;
    using Jitter.LinearMath;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class RoomEditorWorld : IWorld
    {
        public ShipEditorWorld PreviousWorld { get; set; }

        private readonly I2DRenderUtilities m_2DRenderUtilities;

        private readonly IAssetManager m_AssetManager;

        private readonly FontAsset m_DefaultFont;

        private readonly JitterWorld m_JitterWorld;

        private readonly EffectAsset m_LightingEffect;

        private readonly IPhysicsEngine m_PhysicsEngine;

        private readonly RigidBody m_RigidBody;

        private readonly Room m_Room;

        private readonly RoomEditorEntity m_RoomEditorEntity;

        private readonly IRoomTool[] m_RoomTools;

        private int m_ActiveTool;

        public RoomEditorWorld(
            IFactory factory, 
            I2DRenderUtilities twoDRenderUtilities, 
            IAssetManagerProvider assetManagerProvider, 
            IPhysicsEngine physicsEngine, 
            IRoomTool[] roomTools,
            ShipEditorWorld previousWorld,
            Room room)
        {
            this.PreviousWorld = previousWorld;
            this.Entities = new List<IEntity>();

            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_RoomTools = roomTools;
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

            this.m_Room = room;
            foreach (var obj in this.m_Room.Objects)
            {
                obj.Reinitalize();
            }

            this.m_RoomEditorEntity = factory.CreateRoomEditorEntity(this.m_Room);

            this.Entities.Add(ship);
            this.Entities.Add(player);
            this.Entities.Add(dcpu);

            this.Entities.Add(this.m_RoomEditorEntity);

            this.m_PhysicsEngine = physicsEngine;

            this.m_JitterWorld = new JitterWorld(
                new CollisionSystemSAP
                {
                    EnableSpeculativeContacts = true
                })
            {
                Gravity = new JVector(0, -50, 0)
            };

            this.m_JitterWorld.ContactSettings.MaterialCoefficientMixing =
                ContactSettings.MaterialCoefficientMixingType.TakeMinimum;

            var shape = new BoxShape(new JVector(2, 2, 2));
            var body = new RigidBody(shape);
            this.m_JitterWorld.AddBody(body);
            body.Position = new JVector(20, 10, 20);
            body.IsActive = true;
            this.m_RigidBody = body;

            this.SetupRoomPhysics();
        }

        public IRoomTool ActiveRoomTool
        {
            get
            {
                return this.m_RoomTools[this.m_ActiveTool];
            }
        }

        public IList<IEntity> Entities { get; private set; }

        public JitterWorld JitterWorld
        {
            get
            {
                return this.m_JitterWorld;
            }
        }

        public void ActivateAlternate()
        {
            this.m_RoomEditorEntity.UseAlternative = true;
        }

        public void DeactivateAlternate()
        {
            this.m_RoomEditorEntity.UseAlternative = false;
        }

        public void Dispose()
        {
        }

        public void ReleaseTool()
        {
            this.m_RoomEditorEntity.ReleaseCurrentSelection();
        }

        public void RenderAbove(IGameContext gameContext, IRenderContext renderContext)
        {
            if (renderContext.Is3DContext)
            {
                renderContext.PopEffect();

                return;
            }

            for (var i = 0; i < this.m_RoomTools.Length; i++)
            {
                var tool = this.m_RoomTools[i];

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
                        this.m_RoomTools[this.m_ActiveTool].Name, 
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
            var allLights =
                this.Entities.OfType<ILight>().OrderBy(x => (playerPos - x.LightPosition).LengthSquared()).ToArray();
            var lights = new[]
            {
                allLights.Length > 0 ? allLights[0] : emptyLight, allLights.Length > 1 ? allLights[1] : emptyLight, 
                allLights.Length > 2 ? allLights[2] : emptyLight, allLights.Length > 3 ? allLights[3] : emptyLight, 
                allLights.Length > 4 ? allLights[4] : emptyLight, allLights.Length > 5 ? allLights[5] : emptyLight, 
                allLights.Length > 6 ? allLights[6] : emptyLight, allLights.Length > 7 ? allLights[7] : emptyLight, 
                allLights.Length > 8 ? allLights[8] : emptyLight, allLights.Length > 9 ? allLights[9] : emptyLight, 
                allLights.Length > 10 ? allLights[10] : emptyLight, allLights.Length > 11 ? allLights[11] : emptyLight, 
                allLights.Length > 12 ? allLights[12] : emptyLight, allLights.Length > 13 ? allLights[13] : emptyLight, 
                allLights.Length > 14 ? allLights[14] : emptyLight, allLights.Length > 15 ? allLights[15] : emptyLight
            };

            this.m_LightingEffect.Effect.Parameters["LightColours1"].SetValue(
                new Matrix(
                    lights[0].LightColor.R / 255f, 
                    lights[0].LightColor.G / 255f, 
                    lights[0].LightColor.B / 255f, 
                    1, 
                    lights[1].LightColor.R / 255f, 
                    lights[1].LightColor.G / 255f, 
                    lights[1].LightColor.B / 255f, 
                    1, 
                    lights[2].LightColor.R / 255f, 
                    lights[2].LightColor.G / 255f,
                    lights[2].LightColor.B / 255f,
                    1,
                    lights[3].LightColor.R / 255f,
                    lights[3].LightColor.G / 255f,
                    lights[3].LightColor.B / 255f, 
                    1));

            this.m_LightingEffect.Effect.Parameters["LightColours2"].SetValue(
                new Matrix(
                    lights[4].LightColor.R / 255f,
                    lights[4].LightColor.G / 255f,
                    lights[4].LightColor.B / 255f,
                    1,
                    lights[5].LightColor.R / 255f,
                    lights[5].LightColor.G / 255f,
                    lights[5].LightColor.B / 255f,
                    1,
                    lights[6].LightColor.R / 255f,
                    lights[6].LightColor.G / 255f,
                    lights[6].LightColor.B / 255f,
                    1,
                    lights[7].LightColor.R / 255f,
                    lights[7].LightColor.G / 255f,
                    lights[7].LightColor.B / 255f,
                    1));

            this.m_LightingEffect.Effect.Parameters["LightColours3"].SetValue(
                new Matrix(
                    lights[8].LightColor.R / 255f,
                    lights[8].LightColor.G / 255f,
                    lights[8].LightColor.B / 255f,
                    1,
                    lights[9].LightColor.R / 255f,
                    lights[9].LightColor.G / 255f,
                    lights[9].LightColor.B / 255f,
                    1,
                    lights[10].LightColor.R / 255f,
                    lights[10].LightColor.G / 255f,
                    lights[10].LightColor.B / 255f,
                    1,
                    lights[11].LightColor.R / 255f,
                    lights[11].LightColor.G / 255f,
                    lights[11].LightColor.B / 255f,
                    1));

            this.m_LightingEffect.Effect.Parameters["LightColours4"].SetValue(
                new Matrix(
                    lights[12].LightColor.R / 255f,
                    lights[12].LightColor.G / 255f,
                    lights[12].LightColor.B / 255f,
                    1,
                    lights[13].LightColor.R / 255f,
                    lights[13].LightColor.G / 255f,
                    lights[13].LightColor.B / 255f,
                    1,
                    lights[14].LightColor.R / 255f,
                    lights[14].LightColor.G / 255f,
                    lights[14].LightColor.B / 255f,
                    1,
                    lights[15].LightColor.R / 255f,
                    lights[15].LightColor.G / 255f,
                    lights[15].LightColor.B / 255f,
                    1));

            this.m_LightingEffect.Effect.Parameters["Lights1"].SetValue(
                new Matrix(
                    lights[0].LightPosition.X, 
                    lights[0].LightPosition.Y, 
                    lights[0].LightPosition.Z, 
                    lights[0].LightDistance, 
                    lights[1].LightPosition.X, 
                    lights[1].LightPosition.Y, 
                    lights[1].LightPosition.Z, 
                    lights[1].LightDistance, 
                    lights[2].LightPosition.X, 
                    lights[2].LightPosition.Y, 
                    lights[2].LightPosition.Z, 
                    lights[2].LightDistance, 
                    lights[3].LightPosition.X, 
                    lights[3].LightPosition.Y, 
                    lights[3].LightPosition.Z,
                    lights[3].LightDistance));

            this.m_LightingEffect.Effect.Parameters["Lights2"].SetValue(
                new Matrix(
                    lights[4].LightPosition.X,
                    lights[4].LightPosition.Y,
                    lights[4].LightPosition.Z,
                    lights[4].LightDistance,
                    lights[5].LightPosition.X,
                    lights[5].LightPosition.Y,
                    lights[5].LightPosition.Z,
                    lights[5].LightDistance,
                    lights[6].LightPosition.X,
                    lights[6].LightPosition.Y,
                    lights[6].LightPosition.Z,
                    lights[6].LightDistance,
                    lights[7].LightPosition.X,
                    lights[7].LightPosition.Y,
                    lights[7].LightPosition.Z,
                    lights[7].LightDistance));

            this.m_LightingEffect.Effect.Parameters["Lights3"].SetValue(
                new Matrix(
                    lights[8].LightPosition.X,
                    lights[8].LightPosition.Y,
                    lights[8].LightPosition.Z,
                    lights[8].LightDistance,
                    lights[9].LightPosition.X,
                    lights[9].LightPosition.Y,
                    lights[9].LightPosition.Z,
                    lights[9].LightDistance,
                    lights[10].LightPosition.X,
                    lights[10].LightPosition.Y,
                    lights[10].LightPosition.Z,
                    lights[10].LightDistance,
                    lights[11].LightPosition.X,
                    lights[11].LightPosition.Y,
                    lights[11].LightPosition.Z,
                    lights[11].LightDistance));

            this.m_LightingEffect.Effect.Parameters["Lights4"].SetValue(
                new Matrix(
                    lights[12].LightPosition.X,
                    lights[12].LightPosition.Y,
                    lights[12].LightPosition.Z,
                    lights[12].LightDistance,
                    lights[13].LightPosition.X,
                    lights[13].LightPosition.Y,
                    lights[13].LightPosition.Z,
                    lights[13].LightDistance,
                    lights[14].LightPosition.X,
                    lights[14].LightPosition.Y,
                    lights[14].LightPosition.Z,
                    lights[14].LightDistance,
                    lights[15].LightPosition.X,
                    lights[15].LightPosition.Y,
                    lights[15].LightPosition.Z,
                    lights[15].LightDistance));

            renderContext.PushEffect(this.m_LightingEffect.Effect);

            player.SetCamera(renderContext);
        }

        public void TestPhysics()
        {
            this.m_RigidBody.ApplyImpulse(new JVector(0, 15, 0));
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var dcpu = this.Entities.OfType<DCPUEntity>().First();
            dcpu.X = this.m_RigidBody.Position.X;
            dcpu.Y = this.m_RigidBody.Position.Y;
            dcpu.Z = this.m_RigidBody.Position.Z;
            dcpu.Rotation = this.m_RigidBody.Orientation.ToXNAMatrix();

            this.m_PhysicsEngine.UpdateWorld(this.m_JitterWorld, gameContext, updateContext);

            var mouse = Mouse.GetState();
            var value = mouse.ScrollWheelValue;
            while (value < 0)
            {
                value += 120 * this.m_RoomTools.Length;
            }

            this.m_ActiveTool = this.m_RoomTools.Length - ((value / 120) % this.m_RoomTools.Length);
            if (this.m_ActiveTool == this.m_RoomTools.Length)
            {
                this.m_ActiveTool = 0;
            }
        }

        public void UseTool(IGameContext gameContext, bool secondaryAlt)
        {
            this.m_RoomEditorEntity.SelectCurrentHover(gameContext, secondaryAlt);
        }

        private void SetupRoomPhysics()
        {
            // Set up the static physics bodies for each wall.
            var floorShape = new BoxShape(new JVector(this.m_Room.Width, 1, this.m_Room.Depth));
            var floorBody = new RigidBody(floorShape);
            this.m_JitterWorld.AddBody(floorBody);
            floorBody.Position = new JVector(
                /*this.m_Room.X +*/ (this.m_Room.Width / 2f), 
                /*this.m_Room.Y*/ - 0.5f,
                /*this.m_Room.Z +*/ (this.m_Room.Depth / 2f));
            floorBody.IsStatic = true;

            var roofShape = new BoxShape(new JVector(this.m_Room.Width, 1, this.m_Room.Depth));
            var roofBody = new RigidBody(roofShape);
            this.m_JitterWorld.AddBody(roofBody);
            roofBody.Position = new JVector(
                /*this.m_Room.X*/ + (this.m_Room.Width / 2f), 
                /*this.m_Room.Y*/ + this.m_Room.Height + 0.5f, 
                /*this.m_Room.Z*/ + (this.m_Room.Depth / 2f));
            roofBody.IsStatic = true;

            var leftShape = new BoxShape(new JVector(1, this.m_Room.Height, this.m_Room.Depth));
            var leftBody = new RigidBody(leftShape);
            this.m_JitterWorld.AddBody(leftBody);
            leftBody.Position = new JVector(
                /*this.m_Room.X*/ - 0.5f, 
                /*this.m_Room.Y*/ + (this.m_Room.Height / 2f), 
                /*this.m_Room.Z*/ + (this.m_Room.Depth / 2f));
            leftBody.IsStatic = true;

            var rightShape = new BoxShape(new JVector(1, this.m_Room.Height, this.m_Room.Depth));
            var rightBody = new RigidBody(rightShape);
            this.m_JitterWorld.AddBody(rightBody);
            rightBody.Position = new JVector(
                /*this.m_Room.X*/ + this.m_Room.Width + 0.5f, 
                /*this.m_Room.Y*/ + (this.m_Room.Height / 2f), 
                /*this.m_Room.Z*/ + (this.m_Room.Depth / 2f));
            rightBody.IsStatic = true;

            var backShape = new BoxShape(new JVector(this.m_Room.Width, this.m_Room.Height, 1));
            var backBody = new RigidBody(backShape);
            this.m_JitterWorld.AddBody(backBody);
            backBody.Position = new JVector(
                /*this.m_Room.X*/ + (this.m_Room.Width / 2f), 
                /*this.m_Room.Y*/ + (this.m_Room.Height / 2f), 
                /*this.m_Room.Z*/ - 0.5f);
            backBody.IsStatic = true;

            var frontShape = new BoxShape(new JVector(this.m_Room.Width, this.m_Room.Height, 1));
            var frontBody = new RigidBody(frontShape);
            this.m_JitterWorld.AddBody(frontBody);
            frontBody.Position = new JVector(
                /*this.m_Room.X*/ + (this.m_Room.Width / 2f), 
                /*this.m_Room.Y*/ + (this.m_Room.Height / 2f),
                /*this.m_Room.Z*/ +this.m_Room.Depth + 0.5f);
            frontBody.IsStatic = true;
        }
    }
}