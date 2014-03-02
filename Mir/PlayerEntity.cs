namespace Mir
{
    using System;
    using System.Windows.Forms.VisualStyles;
    using Jitter.Collision.Shapes;
    using Jitter.Dynamics;
    using Jitter.LinearMath;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Protogame;

    public class PlayerEntity : IPhysicsEntity, IEntity, ILight
    {
        private readonly IPhysicsEngine m_PhysicsEngine;

        private float m_RelativeX;

        private float m_RelativeY;

        private float m_RelativeZ;

        private bool m_PhysicsSpawned;

        private RigidBody m_RigidBody;

        private PhysicsCharacterController m_PhysicsCharacterController;

        private Vector3 m_PendingMovement;

        private bool m_JumpRequested;

        public PlayerEntity(
            IPhysicsEngine physicsEngine)
        {
            this.m_PhysicsEngine = physicsEngine;
        }
        
        public Vector3 ForwardVector
        {
            get
            {
                return Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(this.Yaw));
            }
        }

        public Vector3 LeftVector
        {
            get
            {
                return Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(this.Yaw + MathHelper.PiOver2));
            }
        }

        public Color LightColor
        {
            get
            {
                return new Color(0.5f, 0.5f, 0.5f);
            }
        }

        public float LightDistance
        {
            get
            {
                return 15;
            }
        }

        public Vector3 LightPosition
        {
            get
            {
                return new Vector3(this.X, this.Y, this.Z);
            }
        }

        public float MovementSpeed
        {
            get
            {
                return 0.3f;
            }
        }

        public IArea ParentArea { get; set; }

        public float Pitch { get; set; }

        public Vector3 RightVector
        {
            get
            {
                return Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(this.Yaw - MathHelper.PiOver2));
            }
        }

        public Matrix Rotation { get; set; }

        public float TargetPitch { get; set; }

        public float TargetYaw { get; set; }

        public float WalkCounter { get; set; }

        public bool Walked { get; set; }

        public float X
        {
            get
            {
                return this.ParentArea.X + this.m_RelativeX;
            }

            set
            {
                this.m_RelativeX = value - this.ParentArea.X;
            }
        }

        public float Y
        {
            get
            {
                return this.ParentArea.Y + this.m_RelativeY;
            }

            set
            {
                this.m_RelativeY = value - this.ParentArea.Y;
            }
        }

        public float Yaw { get; set; }

        public float Z
        {
            get
            {
                return this.ParentArea.Z + this.m_RelativeZ;
            }

            set
            {
                this.m_RelativeZ = value - this.ParentArea.Z;
            }
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void SetCamera(IRenderContext renderContext)
        {
            this.Yaw = MathHelper.Lerp(this.Yaw, this.TargetYaw, 0.5f);
            this.Pitch = MathHelper.Lerp(this.Pitch, this.TargetPitch, 0.5f);

            var bob = (float)Math.Sin(this.WalkCounter / 4f);

            var pos = new Vector3(this.X, this.Y, this.Z);
            var headAdjust = new Vector3(0, 8 + bob / 5f, 0);

            var reference = Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(this.Yaw, this.Pitch, 0));

            renderContext.View = Matrix.CreateLookAt(pos + headAdjust, pos + headAdjust + reference, Vector3.Up);

            var viewport = renderContext.GraphicsDevice.Viewport;
            renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, 
                viewport.Width / (float)viewport.Height, 
                1f, 
                5000f);
        }

        public void ApplyDirection(Vector3 vector)
        {
            this.m_PendingMovement += vector;
        }

        public void InitiateJump()
        {
            this.m_JumpRequested = true;
            this.m_PhysicsCharacterController.JumpVelocity = 6f;
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.Walked && this.m_PhysicsCharacterController.OnFloor)
            {
                this.WalkCounter++;
                this.Walked = false;
            }

            var roomEditorWorld = gameContext.World as RoomEditorWorld;

            if (!this.m_PhysicsSpawned)
            {
                this.m_RigidBody = new RigidBody(new CapsuleShape(10.0f, 1.5f));
                this.m_RigidBody.SetMassProperties(JMatrix.Zero, 1.0f, true);
                this.m_RigidBody.AllowDeactivation = false;
                this.m_RigidBody.EnableSpeculativeContacts = true;
                this.m_RigidBody.EnableDebugDraw = true;
                this.m_RigidBody.Material.Restitution = 0;

                this.m_RigidBody.Position = new JVector(
                    this.X,
                    this.Y + 5f,
                    this.Z);

                var jitterWorld = roomEditorWorld.JitterWorld;

                this.m_PhysicsCharacterController = new PhysicsCharacterController(jitterWorld, this.m_RigidBody);
                this.m_PhysicsCharacterController.Stiffness = 0.05f;
                jitterWorld.AddBody(this.m_RigidBody);
                jitterWorld.AddConstraint(this.m_PhysicsCharacterController);

                this.m_PhysicsSpawned = true;
            }

            this.m_PhysicsCharacterController.TryJump = this.m_JumpRequested;
            if (this.m_PendingMovement != Vector3.Zero)
            {
                this.m_PendingMovement = Vector3.Normalize(this.m_PendingMovement) * 15.0f;
            }

            this.m_PhysicsCharacterController.TargetVelocity = this.m_PendingMovement.ToJitterVector();

            this.X = this.m_RigidBody.Position.X;
            this.Y = this.m_RigidBody.Position.Y - 5f;
            this.Z = this.m_RigidBody.Position.Z;

            this.m_JumpRequested = false;
            this.m_PendingMovement = Vector3.Zero;
        }
    }
}