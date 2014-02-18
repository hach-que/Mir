using System;
using Ninject.Modules;
using Protogame;
using Ninject.Extensions.Factory;

namespace Mir
{
    public class MirIocModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IEventBinder<IGameContext>>().To<MirStaticEventBinder>();
            this.Bind<IFactory>().ToFactory();
        }
    }
}

