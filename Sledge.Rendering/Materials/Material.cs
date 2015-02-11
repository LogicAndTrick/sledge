using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Materials
{
    public class Material : IUpdatable
    {
        public MaterialType Type { get; set; }
        public Color Color { get; set; }
        public string CurrentFrame { get { return TextureFrames[CurrentFrameIndex]; } }
        public int CurrentFrameIndex { get; set; }
        public List<string> TextureFrames { get; set; } // ??? 

        public bool HasTransparency
        {
            get
            {
                // todo: check for texture transparency
                return Color.A < 255;
            }
        }

        private int _millisecondsPerFrame;

        public int FramesPerSecond
        {
            get { return 1000 / _millisecondsPerFrame; }
            set { _millisecondsPerFrame = 1000 / value; }
        }

        public string UniqueIdentifier
        {
            get { return Type + ":" + String.Join("|", TextureFrames); }
        }

        public static Material Flat(Color color)
        {
            return new Material(MaterialType.Flat, color, "WhitePixel");
        }

        public static Material Texture(string name)
        {
            return new Material(MaterialType.Textured, Color.White, name);
        }

        public static Material Animated(int fps, params string[] names)
        {
            return new Material(MaterialType.Animated, Color.White, names) { FramesPerSecond = fps };
        }

        public static Material Blended(params string[] names)
        {
            return new Material(MaterialType.Blended, Color.White, names);
        }

        public Material(MaterialType type, Color color, params string[] textureFrames)
        {
            Type = type;
            Color = color;
            TextureFrames = textureFrames.ToList();
            CurrentFrameIndex = 0;
        }

        public bool IsUpdatable()
        {
            return Type == MaterialType.Animated && _millisecondsPerFrame > 0 && TextureFrames.Count > 1;
        }

        private long _lastFrame = -1;
        public void Update(Frame frame)
        {
            if (!IsUpdatable()) return;

            if (_lastFrame >= 0 && frame.Milliseconds - _lastFrame >= _millisecondsPerFrame)
            {
                CurrentFrameIndex = (CurrentFrameIndex + 1) % TextureFrames.Count;
                _lastFrame = frame.Milliseconds;
            }
            if (_lastFrame < 0) _lastFrame = frame.Milliseconds;
        }
    }
}