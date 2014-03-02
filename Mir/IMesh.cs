namespace Mir
{
    using Microsoft.Xna.Framework;

    public interface IMesh
    {
        short[] MeshIndicies { get; }

        Vector3[] MeshVertexPositions { get; }
    }
}