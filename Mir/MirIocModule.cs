using System;
using Ninject.Modules;
using Protogame;

namespace Mir
{
    public class MirIocModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IEventBinder<IGameContext>>().To<MirStaticEventBinder>();
        }
    }
}

