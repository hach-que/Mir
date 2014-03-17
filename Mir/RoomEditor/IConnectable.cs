namespace Mir
{
    using System.Collections.Generic;
    using Tomato.Hardware;

    public interface IConnectable
    {
        List<IConnectable> Connections { get; set; }

        Device TomatoDevice { get; }

        void ConnectionsUpdated();

        void PrepareDevice();
    }
}