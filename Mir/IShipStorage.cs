namespace Mir
{
    using System.Collections.Generic;

    public interface IShipStorage
    {
        IEnumerable<string> List();

        string Read(string name);

        void Write(string name, string data);
    }
}