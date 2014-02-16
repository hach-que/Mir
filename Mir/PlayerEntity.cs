using System;
using Protogame;
using Microsoft.Xna.Framework;

namespace Mir
{
    public class PlayerEntity : IEntity
    {
        public PlayerEntity()
        {
        }

        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }

        public float Z
        {
            get;
            set;
        }

        public float Yaw
        {
            get;
            set;
        }

        public float Pitch
        {
            get;
            set;
        }

        public float TargetYaw
        {
            get;
            set;
        }

        public float TargetPitch
        {
            get;
            set;
        }

        public Vector3 ForwardVector
        {
            get
            {
                return Vector3.Transform(
                    Vector3.Forward,
                    Matrix.CreateRotationY(this.Yaw));
            }
        }

        public Vector3 LeftVector
        {
            get
            {
                return Vector3.Transform(
                    Vector3.Forward,
                    Matrix.CreateRotationY(this.Yaw + MathHelper.PiOver2));
            }
        }

        public Vector3 RightVector
        {
            get
            {
                return Vector3.Transform(
                    Vector3.Forward,
                    Matrix.CreateRotationY(this.Yaw - MathHelper.PiOver2));
            }
        }

        public float MovementSpeed
        {
            get
            {
                return 0.3f;
            }
        }

        public bool CaptureMouse
        {
            get;
            set;
        }

        public bool Walked
        {
            get;
            set;
        }

        public float WalkCounter
        {
            get;
            set;
        }

        public void Constrain()
        {
            this.X = MathHelper.Clamp(this.X, -12, 12);
            this.Z = MathHelper.Clamp(this.Z, -17, 17);
        }

        public void SetCamera(IRenderContext renderContext)
        {
            this.Yaw = MathHelper.Lerp(this.Yaw, this.TargetYaw, 0.5f);
            this.Pitch = MathHelper.Lerp(this.Pitch, this.TargetPitch, 0.5f);

            var bob = (float)Math.Sin(this.WalkCounter / 4f);

            var pos = new Vector3(this.X, this.Y, this.Z);
            var headAdjust = new Vector3(0, 8 + bob / 5f, 0);

            var reference = Vector3.Transform(
                Vector3.Forward,
                Matrix.CreateFromYawPitchRoll(
                    this.Yaw,
                    this.Pitch,
                    0));

            renderContext.View = Matrix.CreateLookAt(
                pos + headAdjust,
                pos + headAdjust + reference,
                Vector3.Up);

            var viewport = renderContext.GraphicsDevice.Viewport;
            renderContext.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                (float)viewport.Width / (float)viewport.Height,
                1f,
                5000f);
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            if (this.Walked)
            {
                this.WalkCounter++;
                this.Walked = false;
            }
        }
    }
}

