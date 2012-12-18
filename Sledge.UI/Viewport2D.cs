﻿using System;
using System.Linq;
using Sledge.DataStructures.Geometric;
using Sledge.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using Sledge.Graphics.Helpers;

namespace Sledge.UI
{
    public class Viewport2D : ViewportBase
    {
        public enum ViewDirection
        {
            /// <summary>
            /// The XY view
            /// </summary>
            Top,

            /// <summary>
            /// The YZ view
            /// </summary>
            Front,

            /// <summary>
            /// The XZ view
            /// </summary>
            Side
        }

        public static Coordinate Flatten(Coordinate c, ViewDirection direction)
        {
            switch (direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(c.Y, c.Z, 0);
                case ViewDirection.Side:
                    return new Coordinate(c.X, c.Z, 0);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        public static Coordinate Expand(Coordinate c, ViewDirection direction)
        {
            switch (direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(0, c.X, c.Y);
                case ViewDirection.Side:
                    return new Coordinate(c.X, 0, c.Y);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        public static Coordinate GetUnusedCoordinate(Coordinate c, ViewDirection direction)
        {
            switch (direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(0, 0, c.Z);
                case ViewDirection.Front:
                    return new Coordinate(c.X, 0, 0);
                case ViewDirection.Side:
                    return new Coordinate(0, c.Y, 0);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        public static Coordinate ZeroUnusedCoordinate(Coordinate c, ViewDirection direction)
        {
            switch (direction)
            {
                case ViewDirection.Top:
                    return new Coordinate(c.X, c.Y, 0);
                case ViewDirection.Front:
                    return new Coordinate(0, c.Y, c.Z);
                case ViewDirection.Side:
                    return new Coordinate(c.X, 0, c.Z);
                default:
                    throw new ArgumentOutOfRangeException("direction");
            }
        }

        public ViewDirection Direction { get; private set; }

        private Coordinate _position;
        public Coordinate Position
        {
            get { return _position; }
            set
            {
                var old = _position;
                _position = value;
                Listeners.Where(l => l is IViewport2DEventListener)
                    .Select(l => l as IViewport2DEventListener)
                    .ToList().ForEach(l => l.PositionChanged(old, _position));
            }
        }

        private decimal _zoom;
        public decimal Zoom
        {
            get { return _zoom; }
            set
            {
                var old = _zoom;
                _zoom = value;
                Listeners.Where(l => l is IViewport2DEventListener)
                    .Select(l => l as IViewport2DEventListener)
                    .ToList().ForEach(l => l.ZoomChanged(old, _zoom));
            }
        }

        protected Coordinate CenterScreen { get; set; }

        public Viewport2D(ViewDirection direction)
        {
            Zoom = 1;
            Position = new Coordinate(0, 0, 0);
            Direction = direction;
            CenterScreen = new Coordinate(Width / 2m, Height / 2m, 0);
        }
        
        public Viewport2D(ViewDirection direction, RenderContext context) : base(context)
        {
            Zoom = 1;
            Position = new Coordinate(0, 0, 0);
            Direction = direction;
            CenterScreen = new Coordinate(Width / 2m, Height / 2m, 0);
        }

        public Coordinate Flatten(Coordinate c)
        {
            return Flatten(c, Direction);
        }

        public Coordinate Expand(Coordinate c)
        {
            return Expand(c, Direction);
        }

        public Coordinate GetUnusedCoordinate(Coordinate c)
        {
            return GetUnusedCoordinate(c, Direction);
        }

        public Coordinate ZeroUnusedCoordinate(Coordinate c)
        {
            return ZeroUnusedCoordinate(c, Direction);
        }

        public override void SetViewport()
        {
            base.SetViewport();
            Viewport.Orthographic(0, 0, Width, Height, -50000, 50000);
        }

        protected override void OnResize(EventArgs e)
        {
            CenterScreen = new Coordinate(Width / 2m, Height / 2m, 0);
            base.OnResize(e);
        }

        public Coordinate ScreenToWorld(Point location)
        {
            return ScreenToWorld(location.X, location.Y);
        }

        public Coordinate ScreenToWorld(decimal x, decimal y)
        {
            return ScreenToWorld(new Coordinate(x, y, 0));
        }

        public Coordinate ScreenToWorld(Coordinate location)
        {
            return Position + ((location - CenterScreen) / Zoom);
        }

        public Coordinate WorldToScreen(Coordinate location)
        {
            return CenterScreen + ((location - Position) * Zoom);
        }

        protected override void UpdateBeforeRender()
        {
            GL.Scale(new Vector3((float) Zoom, (float) Zoom, 0));
            GL.Translate((float)-Position.X, (float)-Position.Y, 0);
            base.UpdateBeforeRender();
        }

        protected override void UpdateAfterRender()
        {
            Listeners.ForEach(x => x.Render2D());
            base.UpdateAfterRender();
        }
    }
}
