using System;
using Protogame;
using Microsoft.Xna.Framework.Input;

namespace Mir
{
    public class MirStaticEventBinder : StaticEventBinder<IGameContext>
    {
        public override void Configure()
        {
            this.Bind<MouseMoveEvent>(x => true).To<MouseMoveAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.W).To<MoveForwardAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.S).To<MoveBackwardAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.A).To<StrafeLeftAction>();
            this.Bind<KeyHeldEvent>(x => x.Key == Keys.D).To<StrafeRightAction>();
            this.Bind<MousePressEvent>(x => x.Button == MouseButton.Right).To<ToggleMouseAction>();
        }
    }
}

