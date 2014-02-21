using System;
using Microsoft.Xna.Framework;

namespace Mir
{
    using System.Collections.Generic;

    public interface IMeshCollider
    {
        bool Collides(Ray testRay, IEnumerable<IMesh> mesh, out Vector3 position, out IMesh hitMesh);
    }
}

