using System;
using Microsoft.Xna.Framework;

namespace Mir
{
    public interface IMeshCollider
    {
        bool Collides(Ray testRay, IMesh mesh, out Vector3 position);
    }
}

