using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Sledge.Rendering.Interfaces;

namespace Sledge.Rendering.Materials
{
    public class Material : IUpdatable
    {
        public MaterialType Type
        {
            get { return _type; }
        }

        public IList<string> TextureFrames
        {
            get { return _textureFrames; }
        }

        public Color Color { get; set; }
        public string CurrentFrame { get { return TextureFrames[CurrentFrameIndex]; } }
        public int CurrentFrameIndex { get; set; }

        public bool TransparentTexture { get; set; }

        public bool HasTransparency
        {
            get
            {
                return Color.A < 255 || TransparentTexture;
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
            get { return _uniqIdentifier; }
        }

        public static Material Flat(Color color)
        {
            return new Material(MaterialType.Flat, color, "Internal::White");
        }

        public static Material Texture(string name, bool transparent)
        {
            return new Material(MaterialType.Textured, Color.White, name) { TransparentTexture = transparent };
        }

        public static Material Texture(string name, float opacity)
        {
            return new Material(MaterialType.Textured, Color.FromArgb((int)(opacity * 255), Color.White), name);
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
            _type = type;
            Color = color;
            _textureFrames = new ReadOnlyCollection<string>(textureFrames);
            CurrentFrameIndex = 0;
            TransparentTexture = false;
            _uniqIdentifier = Type + ":" + String.Join("|", TextureFrames);
        }

        public bool IsUpdatable()
        {
            return Type == MaterialType.Animated && _millisecondsPerFrame > 0 && TextureFrames.Count > 1;
        }

        private long _lastFrame = -1;
        private readonly MaterialType _type;
        private readonly ReadOnlyCollection<string> _textureFrames;
        private readonly string _uniqIdentifier;

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