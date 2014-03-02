namespace Mir
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Protogame;

    public class MeshCollider : IMeshCollider
    {
        private readonly ICollision m_Collision;

        public MeshCollider(ICollision collision)
        {
            this.m_Collision = collision;
        }

        public bool Collides(
            Ray testRay, 
            IEnumerable<IMesh> meshes, 
            out Vector3 position, 
            out IMesh hitMesh, 
            bool furthest = false)
        {
            var distance = furthest ? 0f : 10000f;
            var closestPosition = Vector3.Zero;
            IMesh closestMesh = null;

            foreach (var mesh in meshes)
            {
                if (mesh.MeshVertexPositions == null)
                {
                    continue;
                }

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
                        if (!furthest)
                        {
                            if (tempDistance < distance && tempDistance > 0)
                            {
                                distance = tempDistance;
                                closestPosition = point.Value;
                                closestMesh = mesh;
                            }
                        }
                        else
                        {
                            if (tempDistance > distance && tempDistance > 0)
                            {
                                distance = tempDistance;
                                closestPosition = point.Value;
                                closestMesh = mesh;
                            }
                        }
                    }
                }
            }

            position = closestPosition;
            hitMesh = closestMesh;
            return closestPosition != Vector3.Zero;
        }
    }
}