using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Mir
{
    public interface IMesh
    {
        Vector3[] MeshVertexPositions { get; }

        short[] MeshIndicies { get; }
    }
}

