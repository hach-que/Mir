using System;
using Protogame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mir
{
    public class ShipLayout
    {
        private readonly LayoutCell[] m_Layout;

        private readonly LayoutWire[] m_Wiring;

        private readonly int m_Size;

        private readonly IFactory m_Factory;

        private readonly TextureAsset m_TextureAsset;

        public ShipLayout(
            IFactory factory,
            IAssetManagerProvider assetManagerProvider,
            int size)
        {
            this.m_Size = size;
            this.m_Layout = new LayoutCell[size * size * size];
            this.m_Wiring = new LayoutWire[(size + 1) * (size + 1) * (size + 1)];
            this.m_Factory = factory;

            this.m_TextureAsset = assetManagerProvider.GetAssetManager().Get<TextureAsset>("ship");
        }

        public void FillCell(int x, int y, int z)
        {
            var idx = this.LookupIndex(x, y, z);

            if (this.m_Layout[idx] == null)
            {
                this.m_Layout[idx] = this.m_Factory.CreateStandardCell();
            }
        }

        public void FillArea(int x, int y, int z, int width, int height, int depth)
        {
            for (var i = x; i < x + width; i++)
            {
                for (var j = y; j < y + height; j++)
                {
                    for (var k = z; k < z + depth; k++)
                    {
                        this.FillCell(i, j, k);

                        var idx = this.LookupIndex(i, j, k);

                        this.m_Layout[idx].FrontOpen = k != z + depth - 1;
                        this.m_Layout[idx].BackOpen = k != z;

                        this.m_Layout[idx].AboveOpen = j != y + height - 1;
                        this.m_Layout[idx].BelowOpen = j != y;

                        this.m_Layout[idx].RightOpen = i != x + width - 1;
                        this.m_Layout[idx].LeftOpen = i != x;
                    }
                }
            }
        }

        public void ClearCell(int x, int y, int z)
        {
            var idx = this.LookupIndex(x, y, z);

            if (this.m_Layout[idx] != null)
            {
                this.m_Layout[idx] = null;
            }
        }

        public void ClearArea(int x, int y, int z, int width, int height, int depth)
        {
            for (var i = x; i < x + width; i++)
            {
                for (var j = y; j < y + height; j++)
                {
                    for (var k = z; k < z + depth; k++)
                    {
                        this.ClearCell(i, j, k);
                    }
                }
            }
        }

        public void RunCorridor(int x, int y, int z, LayoutDirection direction, int amount)
        {
            int dirX, dirY, dirZ;
            this.GetDirectionVector(direction, out dirX, out dirY, out dirZ);

            for (var i = 0; i < amount; i++)
            {
                var current = this.m_Layout[this.LookupIndex(x, y, z)];
                var next = this.m_Layout[this.LookupIndex(x + dirX, y + dirY, z + dirZ)];

                if (current != null)
                {
                    switch (direction)
                    {
                        case LayoutDirection.Front:
                            current.FrontOpen = true;
                            break;
                        case LayoutDirection.Back:
                            current.BackOpen = true;
                            break;
                        case LayoutDirection.Above:
                            current.AboveOpen = true;
                            break;
                        case LayoutDirection.Below:
                            current.BelowOpen = true;
                            break;
                        case LayoutDirection.Left:
                            current.LeftOpen = true;
                            break;
                        case LayoutDirection.Right:
                            current.RightOpen = true;
                            break;
                    }
                }

                if (next != null)
                {
                    switch (direction)
                    {
                        case LayoutDirection.Front:
                            next.BackOpen = true;
                            break;
                        case LayoutDirection.Back:
                            next.FrontOpen = true;
                            break;
                        case LayoutDirection.Above:
                            next.BelowOpen = true;
                            break;
                        case LayoutDirection.Below:
                            next.AboveOpen = true;
                            break;
                        case LayoutDirection.Left:
                            next.RightOpen = true;
                            break;
                        case LayoutDirection.Right:
                            next.LeftOpen = true;
                            break;
                    }
                }
            }
        }

        public void GetDirectionVector(LayoutDirection direction, out int x, out int y, out int z)
        {
            switch (direction)
            {
                case LayoutDirection.Front:
                    x = 0;
                    y = 0;
                    z = 1;
                    break;
                case LayoutDirection.Back:
                    x = 0;
                    y = 0;
                    z = -1;
                    break;
                case LayoutDirection.Above:
                    x = 0;
                    y = 1;
                    z = 0;
                    break;
                case LayoutDirection.Below:
                    x = 0;
                    y = -1;
                    z = 0;
                    break;
                case LayoutDirection.Left:
                    x = -1;
                    y = 0;
                    z = 0;
                    break;
                case LayoutDirection.Right:
                    x = 1;
                    y = 0;
                    z = 0;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        public void Render(IRenderContext renderContext, Matrix transform)
        {
            var oldWorld = renderContext.World;

            renderContext.World = transform;

            renderContext.EnableTextures();
            renderContext.SetActiveTexture(this.m_TextureAsset.Texture);

            renderContext.GraphicsDevice.BlendState = BlendState.Opaque;

            foreach (var pass in renderContext.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                for (var i = 0; i < this.m_Size; i++)
                {
                    for (var j = 0; j < this.m_Size; j++)
                    {
                        for (var k = 0; k < this.m_Size; k++)
                        {
                            var idx = this.LookupIndex(i, j, k);
                            var cell = this.m_Layout[idx];
                            if (cell != null)
                            {
                                cell.Render(renderContext, i, j, k);
                            }
                        }
                    }
                }
            }

            renderContext.World = oldWorld;
        }

        private int LookupIndex(int x, int y, int z)
        {
            return x + y * this.m_Size + z * this.m_Size * this.m_Size;
        }
    }
}

