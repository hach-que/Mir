namespace Mir
{
    using System.Collections.Generic;
    using System.Linq;
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

        private readonly RoomEditorEntity m_RoomEditorEntity;

        private readonly FontAsset m_DefaultFont;

        private readonly Room m_Room;

        private int m_ActiveTool;

        private JitterWorld m_JitterWorld;

        private RigidBody m_RigidBody;

        public RoomEditorWorld(
            IFactory factory, 
            I2DRenderUtilities twoDRenderUtilities, 
            IAssetManagerProvider assetManagerProvider, 
            ITool[] tools)
        {
            this.Entities = new List<IEntity>();

            this.m_2DRenderUtilities = twoDRenderUtilities;
            this.m_AssetManager = assetManagerProvider.GetAssetManager();
            this.m_Tools = tools;
            this.m_DefaultFont = this.m_AssetManager.Get<FontAsset>("font.Default");

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

            this.m_JitterWorld = new JitterWorld(new CollisionSystemSAP());
            this.m_JitterWorld.Gravity = new JVector(0, -10, 0);

            var shape = new BoxShape(new JVector(2, 2, 2));
            var body = new RigidBody(shape);
            this.m_JitterWorld.AddBody(body);
            body.Position = new JVector(20, 10, 20);
            body.IsActive = true;
            this.m_RigidBody = body;

            this.SetupRoomPhysics();
        }

        public IList<IEntity> Entities { get; private set; }

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

            player.SetCamera(renderContext);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            var dcpu = this.Entities.OfType<DCPUEntity>().First();
            dcpu.X = this.m_RigidBody.Position.X;
            dcpu.Y = this.m_RigidBody.Position.Y;
            dcpu.Z = this.m_RigidBody.Position.Z;
            dcpu.Orientation = this.m_RigidBody.Orientation.ToXNAMatrix();

            this.m_JitterWorld.Step((float)gameContext.GameTime.ElapsedGameTime.TotalSeconds, true);

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
            var floorShape = new BoxShape(new JVector(this.m_Room.Width * 2, 1, this.m_Room.Depth * 2));
            var floorBody = new RigidBody(floorShape);
            this.m_JitterWorld.AddBody(floorBody);
            floorBody.Position = new JVector(this.m_Room.X, this.m_Room.Y - 1, this.m_Room.Z);
            floorBody.IsStatic = true;

            var roofShape = new BoxShape(new JVector(this.m_Room.Width * 2, 1, this.m_Room.Depth * 2));
            var roofBody = new RigidBody(roofShape);
            this.m_JitterWorld.AddBody(roofBody);
            roofBody.Position = new JVector(this.m_Room.X, this.m_Room.Y + this.m_Room.Height, this.m_Room.Z);
            roofBody.IsStatic = true;

            var leftShape = new BoxShape(new JVector(1, this.m_Room.Height * 2, this.m_Room.Depth * 2));
            var leftBody = new RigidBody(leftShape);
            this.m_JitterWorld.AddBody(leftBody);
            leftBody.Position = new JVector(this.m_Room.X - 1, this.m_Room.Y, this.m_Room.Z);
            leftBody.IsStatic = true;

            var rightShape = new BoxShape(new JVector(1, this.m_Room.Height * 2, this.m_Room.Depth * 2));
            var rightBody = new RigidBody(rightShape);
            this.m_JitterWorld.AddBody(rightBody);
            rightBody.Position = new JVector(this.m_Room.X + this.m_Room.Width, this.m_Room.Y, this.m_Room.Z);
            rightBody.IsStatic = true;

            var backShape = new BoxShape(new JVector(this.m_Room.Width * 2, this.m_Room.Height * 2, 1));
            var backBody = new RigidBody(backShape);
            this.m_JitterWorld.AddBody(backBody);
            backBody.Position = new JVector(this.m_Room.X, this.m_Room.Y, this.m_Room.Z - 1);
            backBody.IsStatic = true;

            var frontShape = new BoxShape(new JVector(this.m_Room.Width * 2, this.m_Room.Height * 2, 1));
            var frontBody = new RigidBody(frontShape);
            this.m_JitterWorld.AddBody(frontBody);
            frontBody.Position = new JVector(this.m_Room.X, this.m_Room.Y, this.m_Room.Z + this.m_Room.Depth);
            frontBody.IsStatic = true;
        }
    }
}