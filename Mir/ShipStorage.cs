namespace Mir
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class ShipStorage : IShipStorage
    {
        public IEnumerable<string> List()
        {
            this.EnsureExists();

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                ".mir", 
                "ships");

            return
                new DirectoryInfo(path).GetFiles("*.ship").Select(file => file.Name.Substring(0, file.Name.Length - 5));
        }

        public string Read(string name)
        {
            this.EnsureExists();

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                ".mir", 
                "ships");

            using (var reader = new StreamReader(Path.Combine(path, name + ".ship")))
            {
                return reader.ReadToEnd();
            }
        }

        public void Write(string name, string data)
        {
            this.EnsureExists();

            var path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                ".mir", 
                "ships");

            using (var writer = new StreamWriter(Path.Combine(path, name + ".ship")))
            {
                writer.Write(data);
            }
        }

        private void EnsureExists()
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".mir");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, "ships");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}