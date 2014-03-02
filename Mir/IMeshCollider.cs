namespace Mir
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    public interface IMeshCollider
    {
        bool Collides(
            Ray testRay, 
            IEnumerable<IMesh> mesh, 
            out Vector3 position, 
            out IMesh hitMesh, 
            bool furthest = false);
    }
}