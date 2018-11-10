using System.Drawing;
using System.Numerics;

namespace Sledge.Rendering.Overlay
{
    public interface I2DRenderer
    {
        void AddRect(Vector2 start, Vector2 end, Color color, bool antiAliased = false);
        void AddRectFilled(Vector2 start, Vector2 end, Color color, bool antiAliased = false);
        void AddRectOutlineOpaque(Vector2 start, Vector2 end, Color outlineColor, Color fillColor, float outlineWidth = 1, bool antiAliased = false);

        void AddCircle(Vector2 center, float radius, Color color, bool antiAliased = true);
        void AddCircleFilled(Vector2 center, float radius, Color color, bool antiAliased = true);

        Vector2 CalcTextSize(FontType type, string text);
        void AddText(Vector2 position, Color color, FontType type, string text);

        void AddLine(Vector2 start, Vector2 end, Color color, float width = 1, bool antiAliased = true);
    }
}