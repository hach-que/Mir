namespace Mir
{
    using System;
    using System.Collections.Generic;
    using Jitter.Collision.Shapes;
    using Jitter.Dynamics;
    using Jitter.Dynamics.Constraints;
    using Jitter.LinearMath;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class LightEntity : IEntity, ILight, IPhysicsEntity
    {
        private readonly I3DRenderUtilities m_3DRenderUtilities;

        private readonly LightRoomObject m_LightRoomObject;

        private readonly List<Constraint> m_PendingConstraints;

        private readonly RigidBody[] m_RopeComponents;

        private readonly TextureAsset m_TextureAsset;

        private IGameContext m_LastGameContext;

        private bool m_SetPhysics;

        public LightEntity(
            LightRoomObject lightRoomObject, 
            I3DRenderUtilities threedRenderUtilities, 
            IAssetManagerProvider assetManagerProvider)
        {
            this.m_LightRoomObject = lightRoomObject;
            this.m_LightRoomObject.Deleted += this.OnDeleted;
            this.m_3DRenderUtilities = threedRenderUtilities;
            this.m_TextureAsset = assetManagerProvider.GetAssetManager().Get<TextureAsset>("ship");

            var baseX = lightRoomObject.X + 0.5f;
            var baseY = lightRoomObject.Y + 0.5f;
            var baseZ = lightRoomObject.Z + 0.5f;

            // Set up rope physics.
            this.m_RopeComponents = new RigidBody[12];
            this.m_PendingConstraints = new List<Constraint>();
            for (var i = 0; i < 12; i++)
            {
                var shape = new BoxShape(0.5f, 0.5f, 0.5f);
                var rigidBody = new RigidBody(shape);
                rigidBody.Position = new JVector(baseX, baseY - (i * 0.6f), baseZ);
                this.m_RopeComponents[i] = rigidBody;

                if (i == 0)
                {
                    rigidBody.IsStatic = true;
                }
                else
                {
                    var constraint = new PointOnPoint(
                        rigidBody, 
                        this.m_RopeComponents[i - 1], 
                        new JVector(baseX, baseY - (i * 0.6f) + 0.3f, baseZ)) {
                                                                                 BiasFactor = 0.8f, Softness = 0.4f 
                                                                              };
                    this.m_PendingConstraints.Add(constraint);
                }
            }
        }

        public Color LightColor
        {
            get
            {
                return Color.White;
            }
        }

        public float LightDistance
        {
            get
            {
                return 60;
            }
        }

        public Vector3 LightPosition
        {
            get
            {
                return this.m_RopeComponents[this.m_RopeComponents.Length - 1].Position.ToXNAVector();
            }
        }

        public Matrix Rotation { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            for (var i = 0; i < this.m_RopeComponents.Length - 1; i++)
            {
                var firstBody = this.m_RopeComponents[i];
                var secondBody = this.m_RopeComponents[i + 1];

                this.m_3DRenderUtilities.RenderLine(
                    renderContext, 
                    firstBody.Position.ToXNAVector(), 
                    secondBody.Position.ToXNAVector(), 
                    this.m_TextureAsset, 
                    new Vector2(0.5f, 0.5f), 
                    new Vector2(0.5f, 0.5f));
            }

            var lastBody = this.m_RopeComponents[this.m_RopeComponents.Length - 1];

            this.m_3DRenderUtilities.RenderCube(
                renderContext, 
                Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(-0.25f, -0.25f, -0.25f)
                * lastBody.Orientation.ToXNAMatrix() * Matrix.CreateTranslation(lastBody.Position.ToXNAVector()), 
                new TextureAsset(renderContext.SingleWhitePixel), 
                Vector2.Zero, 
                Vector2.One);
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_LastGameContext = gameContext;

            var world = (RoomEditorWorld)gameContext.World;

            if (!this.m_SetPhysics)
            {
                foreach (var body in this.m_RopeComponents)
                {
                    world.JitterWorld.AddBody(body);
                }

                foreach (var constraint in this.m_PendingConstraints)
                {
                    world.JitterWorld.AddConstraint(constraint);
                }

                this.m_PendingConstraints.Clear();

                this.m_SetPhysics = true;
            }

            var keyboard = Keyboard.GetState();
            if (keyboard.IsKeyChanged(this, Keys.U) == KeyState.Down)
            {
                this.m_RopeComponents[5].ApplyImpulse(new JVector(5, 0, 0));
            }
        }

        private void OnDeleted(object sender, EventArgs e)
        {
            if (this.m_LastGameContext != null)
            {
                this.m_LastGameContext.World.Entities.Remove(this);
            }
        }
    }
}