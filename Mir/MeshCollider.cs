using System;
using Microsoft.Xna.Framework;
using Protogame;

namespace Mir
{
    public class MeshCollider : IMeshCollider
    {
        private readonly ICollision m_Collision;

        public MeshCollider(ICollision collision)
        {
            this.m_Collision = collision;
        }

        public bool Collides(Ray testRay, IMesh mesh, out Vector3 position)
        {
            float distance = 10000f;
            Vector3 closestPosition = Vector3.Zero;

            for (var a = 0; a < mesh.MeshIndicies.Length; a += 3)
            {
                var vertexA = mesh.MeshVertexPositions[mesh.MeshIndicies[a]];
                var vertexB = mesh.MeshVertexPositions[mesh.MeshIndicies[a + 1]];
                var vertexC = mesh.MeshVertexPositions[mesh.MeshIndicies[a + 2]];

                float tempDistance;
                var point = this.m_Collision.CollidesWithTriangle(
                    testRay,
                    vertexA,
                    vertexB,
                    vertexC,
                    out tempDistance,
                    false);
                if (point != null)
                {
                    if (tempDistance < distance && tempDistance > 0)
                    {
                        distance = tempDistance;
                        closestPosition = point.Value;
                    }
                }
            }

            position = closestPosition;
            return closestPosition != Vector3.Zero;
        }
    }
}

