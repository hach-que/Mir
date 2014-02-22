using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mir
{
    using Microsoft.Xna.Framework;
    using Protogame;

    public class RoomEditorEntity : IEntity
    {
        private readonly Room m_Room;

        private readonly IMeshCollider m_MeshCollider;

        private RoomObject m_SelectedRoomObject;

        private int m_SelectedRoomObjectFace;

        private Vector3 m_SelectedMouseStartPosition;

        private Vector3 m_SelectedMouseStartPositionRelative;

        private int m_SelectedRoomObjectStartValue1;

        private int m_SelectedRoomObjectStartValue2;

        private float m_SelectedRoomObjectDistanceToPlayer;

        private RoomObject m_HoveredRoomObject;

        private int m_HoveredRoomObjectFace;

        private Vector3 m_HoveredMouseStartPosition;

        private RoomEditorMode m_RoomEditorMode;

        public RoomEditorEntity(IMeshCollider meshCollider, Room room)
        {
            this.m_MeshCollider = meshCollider;
            this.m_Room = room;

            this.m_RoomEditorMode = RoomEditorMode.Hovering;
        }

        private enum RoomEditorMode
        {
            Hovering,
            Selected
        }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public void SelectCurrentHover()
        {
            if (this.m_HoveredRoomObject == null)
            {
                return;
            }

            this.m_RoomEditorMode = RoomEditorMode.Selected;
            this.m_SelectedRoomObject = this.m_HoveredRoomObject;
            this.m_SelectedRoomObjectFace = this.m_HoveredRoomObjectFace;
            this.m_SelectedMouseStartPosition = this.m_HoveredMouseStartPosition;
            this.m_SelectedRoomObjectDistanceToPlayer = -1;
            this.m_SelectedMouseStartPositionRelative = this.m_HoveredMouseStartPosition
                                                        - new Vector3(
                                                              this.m_HoveredRoomObject.X,
                                                              this.m_HoveredRoomObject.Y,
                                                              this.m_HoveredRoomObject.Z);

            switch (this.m_SelectedRoomObjectFace)
            {
                case 0:
                    this.m_SelectedRoomObjectStartValue1 = this.m_SelectedRoomObject.X;
                    this.m_SelectedRoomObjectStartValue2 = this.m_SelectedRoomObject.Width;
                    break;
                case 1:
                    this.m_SelectedRoomObjectStartValue1 = this.m_SelectedRoomObject.Width;
                    break;
                case 2:
                    this.m_SelectedRoomObjectStartValue1 = this.m_SelectedRoomObject.Z;
                    this.m_SelectedRoomObjectStartValue2 = this.m_SelectedRoomObject.Depth;
                    break;
                case 3:
                    this.m_SelectedRoomObjectStartValue1 = this.m_SelectedRoomObject.Depth;
                    break;
                case 4:
                    this.m_SelectedRoomObjectStartValue1 = this.m_SelectedRoomObject.Y;
                    this.m_SelectedRoomObjectStartValue2 = this.m_SelectedRoomObject.Height;
                    break;
                case 5:
                    this.m_SelectedRoomObjectStartValue1 = this.m_SelectedRoomObject.Height;
                    break;
            }
        }

        public void ReleaseCurrentSelection()
        {
            this.m_RoomEditorMode = RoomEditorMode.Hovering;
            this.m_SelectedRoomObject = null;
            this.m_SelectedRoomObjectFace = -1;
            this.m_SelectedMouseStartPosition = Vector3.Zero;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            this.m_Room.Render(renderContext);

            var world = (RoomEditorWorld)gameContext.World;

            if (world.ActiveTool is SizeTool || world.ActiveTool is TextureTool)
            {
                if (this.m_RoomEditorMode == RoomEditorMode.Hovering && this.m_HoveredRoomObject != null)
                {
                    this.m_HoveredRoomObject.RenderSelection(renderContext, this.m_HoveredRoomObjectFace);
                }

                if (this.m_RoomEditorMode == RoomEditorMode.Selected && this.m_SelectedRoomObject != null)
                {
                    this.m_SelectedRoomObject.RenderSelection(renderContext, this.m_SelectedRoomObjectFace);
                }
            }
            else if (world.ActiveTool is MoveTool || world.ActiveTool is DeleteTool)
            {
                if (this.m_RoomEditorMode == RoomEditorMode.Hovering && this.m_HoveredRoomObject != null)
                {
                    this.m_HoveredRoomObject.RenderSelection(renderContext, 0);
                    this.m_HoveredRoomObject.RenderSelection(renderContext, 1);
                    this.m_HoveredRoomObject.RenderSelection(renderContext, 2);
                    this.m_HoveredRoomObject.RenderSelection(renderContext, 3);
                    this.m_HoveredRoomObject.RenderSelection(renderContext, 4);
                    this.m_HoveredRoomObject.RenderSelection(renderContext, 5);
                }

                if (this.m_RoomEditorMode == RoomEditorMode.Selected && this.m_SelectedRoomObject != null)
                {
                    this.m_SelectedRoomObject.RenderSelection(renderContext, 0);
                    this.m_SelectedRoomObject.RenderSelection(renderContext, 1);
                    this.m_SelectedRoomObject.RenderSelection(renderContext, 2);
                    this.m_SelectedRoomObject.RenderSelection(renderContext, 3);
                    this.m_SelectedRoomObject.RenderSelection(renderContext, 4);
                    this.m_SelectedRoomObject.RenderSelection(renderContext, 5);
                }
            }
            else if (world.ActiveTool is AngleTool)
            {
                
            }
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            switch (this.m_RoomEditorMode)
            {
                case RoomEditorMode.Hovering:
                {
                    this.m_HoveredRoomObject = null;
                    this.m_HoveredRoomObjectFace = -1;

                    Vector3 position;
                    IMesh mesh;
                    if (this.m_MeshCollider.Collides(
                        gameContext.MouseRay,
                        this.m_Room.Objects.Concat(new IMesh[] { this.m_Room }),
                        out position,
                        out mesh))
                    {
                        var roomObject = mesh as RoomObject;
                        if (roomObject != null)
                        {
                            this.m_HoveredRoomObject = roomObject;
                            this.m_HoveredRoomObjectFace = this.m_HoveredRoomObject.GetFaceForTouchPosition(position);
                            this.m_HoveredMouseStartPosition = position;
                        }
                    }

                    break;
                }

                case RoomEditorMode.Selected:
                {
                    var world = (RoomEditorWorld)gameContext.World;

                    if (this.m_SelectedRoomObjectDistanceToPlayer == -1f)
                    {
                        var player = world.Entities.OfType<PlayerEntity>().First();
                        this.m_SelectedRoomObjectDistanceToPlayer =
                            (this.m_SelectedMouseStartPosition - new Vector3(player.X, player.Y, player.Z)).Length();
                    }

                    if (world.ActiveTool is SizeTool)
                    {
                        this.HandleResize(gameContext, updateContext);
                    }
                    else if (world.ActiveTool is MoveTool)
                    {
                        this.HandleMove(gameContext, updateContext);
                    }
                    else if (world.ActiveTool is AngleTool)
                    {
                        this.HandleAngle(gameContext, updateContext);
                    }
                    else if (world.ActiveTool is TextureTool)
                    {
                        this.HandleTexture(gameContext, updateContext);
                        this.ReleaseCurrentSelection();
                    }

                    break;
                }
            }
        }

        private void HandleMove(IGameContext gameContext, IUpdateContext updateContext)
        {
            var targetLoc = gameContext.MouseRay.Position
                            + (gameContext.MouseRay.Direction * this.m_SelectedRoomObjectDistanceToPlayer);

            this.m_SelectedRoomObject.X = (int)Math.Round(targetLoc.X - this.m_SelectedMouseStartPositionRelative.X);
            this.m_SelectedRoomObject.Y = (int)Math.Round(targetLoc.Y - this.m_SelectedMouseStartPositionRelative.Y);
            this.m_SelectedRoomObject.Z = (int)Math.Round(targetLoc.Z - this.m_SelectedMouseStartPositionRelative.Z);
        }

        private void HandleAngle(IGameContext gameContext, IUpdateContext updateContext)
        {
        }

        private void HandleTexture(IGameContext gameContext, IUpdateContext updateContext)
        {
            switch (this.m_SelectedRoomObjectFace)
            {
                case 0:
                {
                    this.m_SelectedRoomObject.LeftTextureIndex++;
                    if (this.m_SelectedRoomObject.LeftTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.LeftTextureIndex = 0;
                    }
                    break;
                }
                case 1:
                {
                    this.m_SelectedRoomObject.RightTextureIndex++;
                    if (this.m_SelectedRoomObject.RightTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.RightTextureIndex = 0;
                    }
                    break;
                }
                case 2:
                {
                    this.m_SelectedRoomObject.BackTextureIndex++;
                    if (this.m_SelectedRoomObject.BackTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.BackTextureIndex = 0;
                    }
                    break;
                }
                case 3:
                {
                    this.m_SelectedRoomObject.FrontTextureIndex++;
                    if (this.m_SelectedRoomObject.FrontTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.FrontTextureIndex = 0;
                    }
                    break;
                }
                case 4:
                {
                    this.m_SelectedRoomObject.BelowTextureIndex++;
                    if (this.m_SelectedRoomObject.BelowTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.BelowTextureIndex = 0;
                    }
                    break;
                }
                case 5:
                {
                    this.m_SelectedRoomObject.AboveTextureIndex++;
                    if (this.m_SelectedRoomObject.AboveTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.AboveTextureIndex = 0;
                    }
                    break;
                }
            }
        }

        private void HandleResize(IGameContext gameContext, IUpdateContext updateContext)
        {
            switch (this.m_SelectedRoomObjectFace)
            {
                case 0:
                {
                    var dir = new Vector3(-1, 0, 0);
                    var ray = new Ray(this.m_SelectedMouseStartPosition, dir);

                    var intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                    if (intersectionDistance != null)
                    {
                        var newWidth = this.m_SelectedRoomObjectStartValue2
                                        + (int)Math.Round(intersectionDistance.Value);
                        if (newWidth >= 1)
                        {
                            this.m_SelectedRoomObject.X = this.m_SelectedRoomObjectStartValue1
                                                            - (int)Math.Round(intersectionDistance.Value);
                            this.m_SelectedRoomObject.Width = newWidth;
                        }
                    }
                    else
                    {
                        dir = new Vector3(1, 0, 0);
                        ray = new Ray(this.m_SelectedMouseStartPosition, dir);
                        intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                        if (intersectionDistance != null)
                        {
                            var newWidth = this.m_SelectedRoomObjectStartValue2
                                            - (int)Math.Round(intersectionDistance.Value);
                            if (newWidth >= 1)
                            {
                                this.m_SelectedRoomObject.X = this.m_SelectedRoomObjectStartValue1
                                                                + (int)Math.Round(intersectionDistance.Value);
                                this.m_SelectedRoomObject.Width = newWidth;
                            }
                        }
                    }

                    break;
                }

                case 1:
                {
                    var dir = new Vector3(1, 0, 0);
                    var ray = new Ray(this.m_SelectedMouseStartPosition, dir);

                    var intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                    if (intersectionDistance != null)
                    {
                        var newWidth = this.m_SelectedRoomObjectStartValue1
                                        + (int)Math.Round(intersectionDistance.Value);
                        if (newWidth >= 1)
                        {
                            this.m_SelectedRoomObject.Width = newWidth;
                        }
                    }
                    else
                    {
                        dir = new Vector3(-1, 0, 0);
                        ray = new Ray(this.m_SelectedMouseStartPosition, dir);
                        intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                        if (intersectionDistance != null)
                        {
                            var newWidth = this.m_SelectedRoomObjectStartValue1
                                            - (int)Math.Round(intersectionDistance.Value);
                            if (newWidth >= 1)
                            {
                                this.m_SelectedRoomObject.Width = newWidth;
                            }
                        }
                    }

                    break;
                }

                case 2:
                {
                    var dir = new Vector3(0, 0, -1);
                    var ray = new Ray(this.m_SelectedMouseStartPosition, dir);

                    var intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                    if (intersectionDistance != null)
                    {
                        var newDepth = this.m_SelectedRoomObjectStartValue2
                                        + (int)Math.Round(intersectionDistance.Value);
                        if (newDepth >= 1)
                        {
                            this.m_SelectedRoomObject.Z = this.m_SelectedRoomObjectStartValue1
                                                            - (int)Math.Round(intersectionDistance.Value);
                            this.m_SelectedRoomObject.Depth = newDepth;
                        }
                    }
                    else
                    {
                        dir = new Vector3(0, 0, 1);
                        ray = new Ray(this.m_SelectedMouseStartPosition, dir);
                        intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                        if (intersectionDistance != null)
                        {
                            var newDepth = this.m_SelectedRoomObjectStartValue2
                                            - (int)Math.Round(intersectionDistance.Value);
                            if (newDepth >= 1)
                            {
                                this.m_SelectedRoomObject.Z = this.m_SelectedRoomObjectStartValue1
                                                                + (int)Math.Round(intersectionDistance.Value);
                                this.m_SelectedRoomObject.Depth = newDepth;
                            }
                        }
                    }

                    break;
                }

                case 3:
                {
                    var dir = new Vector3(0, 0, 1);
                    var ray = new Ray(this.m_SelectedMouseStartPosition, dir);

                    var intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                    if (intersectionDistance != null)
                    {
                        var newDepth = this.m_SelectedRoomObjectStartValue1
                                        + (int)Math.Round(intersectionDistance.Value);
                        if (newDepth >= 1)
                        {
                            this.m_SelectedRoomObject.Depth = newDepth;
                        }
                    }
                    else
                    {
                        dir = new Vector3(0, 0, -1);
                        ray = new Ray(this.m_SelectedMouseStartPosition, dir);
                        intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                        if (intersectionDistance != null)
                        {
                            var newDepth = this.m_SelectedRoomObjectStartValue1
                                            - (int)Math.Round(intersectionDistance.Value);
                            if (newDepth >= 1)
                            {
                                this.m_SelectedRoomObject.Depth = newDepth;
                            }
                        }
                    }

                    break;
                }

                case 4:
                {
                    var dir = new Vector3(0, -1, 0);
                    var ray = new Ray(this.m_SelectedMouseStartPosition, dir);

                    var intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                    if (intersectionDistance != null)
                    {
                        var newHeight = this.m_SelectedRoomObjectStartValue2
                                        + (int)Math.Round(intersectionDistance.Value);
                        if (newHeight >= 1)
                        {
                            this.m_SelectedRoomObject.Y = this.m_SelectedRoomObjectStartValue1
                                                            - (int)Math.Round(intersectionDistance.Value);
                            this.m_SelectedRoomObject.Height = newHeight;
                        }
                    }
                    else
                    {
                        dir = new Vector3(0, 1, 0);
                        ray = new Ray(this.m_SelectedMouseStartPosition, dir);
                        intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                        if (intersectionDistance != null)
                        {
                            var newHeight = this.m_SelectedRoomObjectStartValue2
                                            - (int)Math.Round(intersectionDistance.Value);
                            if (newHeight >= 1)
                            {
                                this.m_SelectedRoomObject.Y = this.m_SelectedRoomObjectStartValue1
                                                                + (int)Math.Round(intersectionDistance.Value);
                                this.m_SelectedRoomObject.Height = newHeight;
                            }
                        }
                    }

                    break;
                }

                case 5:
                {
                    var dir = new Vector3(0, 1, 0);
                    var ray = new Ray(this.m_SelectedMouseStartPosition, dir);

                    var intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                    if (intersectionDistance != null)
                    {
                        var newHeight = this.m_SelectedRoomObjectStartValue1
                                        + (int)Math.Round(intersectionDistance.Value);
                        if (newHeight >= 1)
                        {
                            this.m_SelectedRoomObject.Height = newHeight;
                        }
                    }
                    else
                    {
                        dir = new Vector3(0, -1, 0);
                        ray = new Ray(this.m_SelectedMouseStartPosition, dir);
                        intersectionDistance = ray.Intersects(gameContext.MouseVerticalPlane);
                        if (intersectionDistance != null)
                        {
                            var newHeight = this.m_SelectedRoomObjectStartValue1
                                            - (int)Math.Round(intersectionDistance.Value);
                            if (newHeight >= 1)
                            {
                                this.m_SelectedRoomObject.Height = newHeight;
                            }
                        }
                    }

                    break;
                }
            }
        }
    }
}
