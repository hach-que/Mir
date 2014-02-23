namespace Mir
{
    using System;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Protogame;

    public class RoomEditorEntity : IEntity
    {
        private readonly IMeshCollider m_MeshCollider;

        private readonly Room m_Room;

        private Vector3 m_HoveredMouseStartPosition;

        private RoomObject m_HoveredRoomObject;

        private int m_HoveredRoomObjectFace;

        private int m_HoveredRoomObjectVerticalEdge;

        private RoomEditorMode m_RoomEditorMode;

        private Vector3 m_SelectedMouseStartPosition;

        private Vector3 m_SelectedMouseStartPositionRelative;

        private RoomObject m_SelectedRoomObject;

        private float m_SelectedRoomObjectDistanceToPlayer;

        private int m_SelectedRoomObjectFace;

        private int m_SelectedRoomObjectStartValue1;

        private int m_SelectedRoomObjectStartValue2;

        private int m_SelectedRoomObjectVerticalEdge;

        private bool m_RenderSelectionTransparently;

        private bool m_IsAlternateMode;

        private bool m_IsSecondaryAlternateMode;

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

        public bool UseAlternative { get; set; }

        public void ReleaseCurrentSelection()
        {
            this.m_RoomEditorMode = RoomEditorMode.Hovering;
            this.m_SelectedRoomObject = null;
            this.m_SelectedRoomObjectFace = -1;
            this.m_SelectedRoomObjectVerticalEdge = -1;
            this.m_SelectedMouseStartPosition = Vector3.Zero;
            this.m_SelectedRoomObjectDistanceToPlayer = -1;
            this.m_SelectedMouseStartPositionRelative = Vector3.Zero;
        }

        public void Render(IGameContext gameContext, IRenderContext renderContext)
        {
            if (!renderContext.Is3DContext)
            {
                return;
            }

            this.m_Room.Render(
                renderContext,
                this.m_RoomEditorMode == RoomEditorMode.Hovering ? this.m_HoveredRoomObject : this.m_SelectedRoomObject,
                this.m_RenderSelectionTransparently);

            var world = (RoomEditorWorld)gameContext.World;

            if (world.ActiveTool is NewTool && this.m_HoveredMouseStartPosition != Vector3.Zero)
            {
                var tempObject = new RoomObject();
                tempObject.X = (int)Math.Round(this.m_HoveredMouseStartPosition.X);
                tempObject.Y = (int)Math.Round(this.m_HoveredMouseStartPosition.Y);
                tempObject.Z = (int)Math.Round(this.m_HoveredMouseStartPosition.Z);

                if (tempObject.X >= this.X + this.m_Room.Width)
                {
                    tempObject.X = (int)(this.X + this.m_Room.Width - tempObject.Width);
                }

                if (tempObject.Y >= this.Y + this.m_Room.Height)
                {
                    tempObject.Y = (int)(this.Y + this.m_Room.Height - tempObject.Height);
                }

                if (tempObject.Z >= this.Z + this.m_Room.Depth)
                {
                    tempObject.Z = (int)(this.Z + this.m_Room.Depth - tempObject.Depth);
                }

                tempObject.Render(renderContext);
                tempObject.RenderSelection(renderContext, 0);
                tempObject.RenderSelection(renderContext, 1);
                tempObject.RenderSelection(renderContext, 2);
                tempObject.RenderSelection(renderContext, 3);
                tempObject.RenderSelection(renderContext, 4);
                tempObject.RenderSelection(renderContext, 5);
            }
            else if (world.ActiveTool is SizeTool || world.ActiveTool is TextureTool)
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
                if (this.m_RoomEditorMode == RoomEditorMode.Hovering && this.m_HoveredRoomObject != null)
                {
                    this.m_HoveredRoomObject.RenderVerticalEdge(renderContext, this.m_HoveredRoomObjectVerticalEdge);
                }

                if (this.m_RoomEditorMode == RoomEditorMode.Selected && this.m_SelectedRoomObject != null)
                {
                    this.m_SelectedRoomObject.RenderVerticalEdge(renderContext, this.m_SelectedRoomObjectVerticalEdge);
                }
            }
        }

        public void SelectCurrentHover(IGameContext gameContext, bool secondaryAlt)
        {
            var world = (RoomEditorWorld)gameContext.World;

            if (this.m_HoveredRoomObject == null && !(world.ActiveTool is NewTool))
            {
                return;
            }

            this.m_IsAlternateMode = this.UseAlternative;
            this.m_IsSecondaryAlternateMode = secondaryAlt;
            this.m_RoomEditorMode = RoomEditorMode.Selected;
            this.m_SelectedRoomObject = this.m_HoveredRoomObject;
            this.m_SelectedRoomObjectFace = this.m_HoveredRoomObjectFace;
            this.m_SelectedRoomObjectVerticalEdge = this.m_HoveredRoomObjectVerticalEdge;
            this.m_SelectedMouseStartPosition = this.m_HoveredMouseStartPosition;
            this.m_SelectedRoomObjectDistanceToPlayer = -1;

            if (this.m_HoveredRoomObject != null)
            {
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
        }

        public void Update(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_RenderSelectionTransparently = false;

            var world = (RoomEditorWorld)gameContext.World;

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
                            this.m_HoveredRoomObjectVerticalEdge = this.m_HoveredRoomObject.GetVerticalEdge(position);
                            this.m_HoveredMouseStartPosition = position;

                            if (this.UseAlternative)
                            {
                                this.m_RenderSelectionTransparently = true;
                                
                                this.m_MeshCollider.Collides(
                                        gameContext.MouseRay, 
                                        new IMesh[] { mesh }, 
                                        out position, 
                                        out mesh,
                                        furthest: true);

                                this.m_HoveredRoomObjectFace = this.m_HoveredRoomObject.GetFaceForTouchPosition(position);
                                this.m_HoveredRoomObjectVerticalEdge = this.m_HoveredRoomObject.GetVerticalEdge(position);
                            }
                        }
                        else if (roomObject == null && mesh == this.m_Room && world.ActiveTool is NewTool)
                        {
                            this.m_HoveredRoomObject = null;
                            this.m_HoveredRoomObjectFace = -1;
                            this.m_HoveredRoomObjectVerticalEdge = -1;
                            this.m_HoveredMouseStartPosition = position;
                        }
                    }

                    break;
                }

                case RoomEditorMode.Selected:
                {
                    if (this.m_IsAlternateMode)
                    {
                        this.m_RenderSelectionTransparently = true;
                    }

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
                        this.ReleaseCurrentSelection();
                    }
                    else if (world.ActiveTool is TextureTool)
                    {
                        this.HandleTexture(gameContext, updateContext);
                        this.ReleaseCurrentSelection();
                    }
                    else if (world.ActiveTool is NewTool)
                    {
                        this.HandleNew(gameContext, updateContext);
                        this.ReleaseCurrentSelection();
                    }
                    else if (world.ActiveTool is DeleteTool)
                    {
                        this.HandleDelete(gameContext, updateContext);
                        this.ReleaseCurrentSelection();
                    }

                    break;
                }
            }
        }

        private void HandleAngle(IGameContext gameContext, IUpdateContext updateContext)
        {
            switch (this.m_SelectedRoomObjectVerticalEdge)
            {
                case 0:
                {
                    this.m_SelectedRoomObject.LeftBackEdgeMode++;
                    if (this.m_SelectedRoomObject.LeftBackEdgeMode > 2)
                    {
                        this.m_SelectedRoomObject.LeftBackEdgeMode = 0;
                    }

                    break;
                }

                case 1:
                {
                    this.m_SelectedRoomObject.RightBackEdgeMode++;
                    if (this.m_SelectedRoomObject.RightBackEdgeMode > 2)
                    {
                        this.m_SelectedRoomObject.RightBackEdgeMode = 0;
                    }

                    break;
                }

                case 2:
                {
                    this.m_SelectedRoomObject.LeftFrontEdgeMode++;
                    if (this.m_SelectedRoomObject.LeftFrontEdgeMode > 2)
                    {
                        this.m_SelectedRoomObject.LeftFrontEdgeMode = 0;
                    }

                    break;
                }

                case 3:
                {
                    this.m_SelectedRoomObject.RightFrontEdgeMode++;
                    if (this.m_SelectedRoomObject.RightFrontEdgeMode > 2)
                    {
                        this.m_SelectedRoomObject.RightFrontEdgeMode = 0;
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

            if (this.m_SelectedRoomObject.X < this.X)
            {
                this.m_SelectedRoomObject.X = (int)(this.X);
            }

            if (this.m_SelectedRoomObject.Y < this.Y)
            {
                this.m_SelectedRoomObject.Y = (int)(this.Y);
            }

            if (this.m_SelectedRoomObject.Z < this.Z)
            {
                this.m_SelectedRoomObject.Z = (int)(this.Z);
            }

            if (this.m_SelectedRoomObject.X >= this.X + this.m_Room.Width)
            {
                this.m_SelectedRoomObject.X = (int)(this.X + this.m_Room.Width - this.m_SelectedRoomObject.Width);
            }

            if (this.m_SelectedRoomObject.Y >= this.Y + this.m_Room.Height)
            {
                this.m_SelectedRoomObject.Y = (int)(this.Y + this.m_Room.Height - this.m_SelectedRoomObject.Height);
            }

            if (this.m_SelectedRoomObject.Z >= this.Z + this.m_Room.Depth)
            {
                this.m_SelectedRoomObject.Z = (int)(this.Z + this.m_Room.Depth - this.m_SelectedRoomObject.Depth);
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

        private void HandleTexture(IGameContext gameContext, IUpdateContext updateContext)
        {
            var adj = this.m_IsSecondaryAlternateMode ? -1 : 1;

            switch (this.m_SelectedRoomObjectFace)
            {
                case 0:
                {
                    this.m_SelectedRoomObject.LeftTextureIndex += adj;
                    if (this.m_SelectedRoomObject.LeftTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.LeftTextureIndex = 0;
                    }
                    if (this.m_SelectedRoomObject.LeftTextureIndex < 0)
                    {
                        this.m_SelectedRoomObject.LeftTextureIndex = 80;
                    }

                    break;
                }

                case 1:
                {
                    this.m_SelectedRoomObject.RightTextureIndex += adj;
                    if (this.m_SelectedRoomObject.RightTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.RightTextureIndex = 0;
                    }
                    if (this.m_SelectedRoomObject.RightTextureIndex < 0)
                    {
                        this.m_SelectedRoomObject.RightTextureIndex = 80;
                    }

                    break;
                }

                case 2:
                {
                    this.m_SelectedRoomObject.BackTextureIndex += adj;
                    if (this.m_SelectedRoomObject.BackTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.BackTextureIndex = 0;
                    }
                    if (this.m_SelectedRoomObject.BackTextureIndex < 0)
                    {
                        this.m_SelectedRoomObject.BackTextureIndex = 80;
                    }

                    break;
                }

                case 3:
                {
                    this.m_SelectedRoomObject.FrontTextureIndex += adj;
                    if (this.m_SelectedRoomObject.FrontTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.FrontTextureIndex = 0;
                    }
                    if (this.m_SelectedRoomObject.FrontTextureIndex < 0)
                    {
                        this.m_SelectedRoomObject.FrontTextureIndex = 80;
                    }

                    break;
                }

                case 4:
                {
                    this.m_SelectedRoomObject.BelowTextureIndex += adj;
                    if (this.m_SelectedRoomObject.BelowTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.BelowTextureIndex = 0;
                    }
                    if (this.m_SelectedRoomObject.BelowTextureIndex < 0)
                    {
                        this.m_SelectedRoomObject.BelowTextureIndex = 80;
                    }

                    break;
                }

                case 5:
                {
                    this.m_SelectedRoomObject.AboveTextureIndex += adj;
                    if (this.m_SelectedRoomObject.AboveTextureIndex >= 81)
                    {
                        this.m_SelectedRoomObject.AboveTextureIndex = 0;
                    }
                    if (this.m_SelectedRoomObject.AboveTextureIndex < 0)
                    {
                        this.m_SelectedRoomObject.AboveTextureIndex = 80;
                    }

                    break;
                }
            }
        }

        private void HandleNew(IGameContext gameContext, IUpdateContext updateContext)
        {
            var roundedX = (int)Math.Round(this.m_SelectedMouseStartPosition.X);
            var roundedY = (int)Math.Round(this.m_SelectedMouseStartPosition.Y);
            var roundedZ = (int)Math.Round(this.m_SelectedMouseStartPosition.Z);

            var tempObject = new RoomObject { X = roundedX, Y = roundedY, Z = roundedZ };

            if (tempObject.X >= this.X + this.m_Room.Width)
            {
                tempObject.X = (int)(this.X + this.m_Room.Width - tempObject.Width);
            }

            if (tempObject.Y >= this.Y + this.m_Room.Height)
            {
                tempObject.Y = (int)(this.Y + this.m_Room.Height - tempObject.Height);
            }

            if (tempObject.Z >= this.Z + this.m_Room.Depth)
            {
                tempObject.Z = (int)(this.Z + this.m_Room.Depth - tempObject.Depth);
            }

            this.m_Room.Objects.Add(tempObject);
        }

        private void HandleDelete(IGameContext gameContext, IUpdateContext updateContext)
        {
            this.m_Room.Objects.Remove(this.m_SelectedRoomObject);
        }
    }
}