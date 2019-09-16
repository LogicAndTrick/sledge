using System;
using System.Drawing;
using System.Numerics;
using ImGuiNET;
using Sledge.Common;
using Sledge.Rendering.Viewports;

namespace Sledge.Rendering.Overlay
{
    internal class ImGui2DRenderer : I2DRenderer, IDisposable
    {
        private readonly IViewport _viewport;
        private readonly ImGuiController _controller;

        private int _numObjects;
        private int _viewportId;
        private ImDrawListPtr _currentList;

        private const int MaxObjects = 1000;

        public ImGui2DRenderer(IViewport viewport, ImGuiController controller)
        {
            _viewport = viewport;
            _controller = controller;
            _numObjects = 0;
            _viewportId = 0;
            
            ImGui.SetCurrentContext(_controller.Context);
            ImGui.PushFont(_controller.DefaultFont);

            BeginWindow();
        }

        private void BeginWindow()
        {
            ImGui.SetNextWindowPos(new Vector2(0, 0), ImGuiCond.Always, Vector2.Zero);
            ImGui.SetNextWindowSizeConstraints(new Vector2(_viewport.Width, _viewport.Height), new Vector2(_viewport.Width, _viewport.Height));
            ImGui.SetNextWindowBgAlpha(0);
            ImGui.Begin("Viewport" + _viewportId++, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav);
            _currentList = ImGui.GetWindowDrawList();
        }

        private void EndWindow()
        {
            _currentList = null;
            ImGui.End();
        }

        private ImDrawListPtr GetList(int numObjects = 1)
        {
            if (_numObjects >= MaxObjects)
            {
                EndWindow();
                _numObjects = 0;
                BeginWindow();
            }

            _numObjects += numObjects;
            return _currentList;
        }

        public void AddRect(Vector2 start, Vector2 end, Color color, bool antiAliased = false)
        {
            var list = GetList();
            list.Flags = antiAliased ? ImDrawListFlags.AntiAliasedLines : 0;
            list.AddRect(start, end, color.ToImGuiColor());
            list.Flags = 0;
        }

        public void AddRectFilled(Vector2 start, Vector2 end, Color color, bool antiAliased = false)
        {
            var list = GetList();
            list.Flags = antiAliased ? ImDrawListFlags.AntiAliasedLines : 0;
            list.AddRectFilled(start, end, color.ToImGuiColor());
            list.Flags = 0;
        }

        public void AddRectOutlineOpaque(Vector2 start, Vector2 end, Color outlineColor, Color fillColor, float outlineWidth = 1, bool antiAliased = false)
        {
            var list = GetList(2);
            list.Flags = antiAliased ? ImDrawListFlags.AntiAliasedLines : 0;
            var outl = new Vector2(outlineWidth, outlineWidth);
            list.AddRectFilled(start, end, outlineColor.ToImGuiColor());
            list.AddRectFilled(start + outl, end - outl, fillColor.ToImGuiColor());
            list.Flags = 0;
        }

        public void AddCircle(Vector2 center, float radius, Color color, bool antiAliased = true)
        {
            var list = GetList();
            list.Flags = antiAliased ? ImDrawListFlags.AntiAliasedLines : 0;
            list.AddCircle(center, radius, color.ToImGuiColor());
            list.Flags = 0;
        }

        public void AddCircleFilled(Vector2 center, float radius, Color color, bool antiAliased)
        {
            var list = GetList();
            list.Flags = antiAliased ? ImDrawListFlags.AntiAliasedFill : 0;
            list.AddCircleFilled(center, radius, color.ToImGuiColor());
            list.Flags = 0;
        }

        public Vector2 CalcTextSize(FontType type, string text)
        {
            switch (type)
            {
                case FontType.Normal:
                    return ImGui.CalcTextSize(text);
                case FontType.Bold:
                    ImGui.PushFont(_controller.BoldFont);
                    var bs = ImGui.CalcTextSize(text);
                    ImGui.PopFont();
                    return bs;
                case FontType.Large:
                    ImGui.PushFont(_controller.LargeFont);
                    var ls = ImGui.CalcTextSize(text);
                    ImGui.PopFont();
                    return ls;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void AddText(Vector2 position, Color color, FontType type, string text)
        {
            var list = GetList();
            switch (type)
            {
                case FontType.Normal:
                    list.AddText(position, color.ToImGuiColor(), text);
                    break;
                case FontType.Bold:
                    ImGui.PushFont(_controller.BoldFont);
                    list.AddText(position, color.ToImGuiColor(), text);
                    ImGui.PopFont();
                    break;
                case FontType.Large:
                    ImGui.PushFont(_controller.LargeFont);
                    list.AddText(position, color.ToImGuiColor(), text);
                    ImGui.PopFont();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void AddLine(Vector2 start, Vector2 end, Color color, float width = 1, bool antiAliased = true)
        {
            var list = GetList();
            list.Flags = antiAliased ? ImDrawListFlags.AntiAliasedLines : 0;
            list.AddLine(start, end, color.ToImGuiColor(), width);
            list.Flags = 0;
        }

        public void Dispose()
        {
            EndWindow();
        }
    }
}
