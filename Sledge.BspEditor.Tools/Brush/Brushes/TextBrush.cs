using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Poly2Tri;
using Poly2Tri.Triangulation.Polygon;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Tools.Brush.Brushes.Controls;
using Sledge.Common;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Hooks;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Plane = Sledge.DataStructures.Geometric.Plane;
using Polygon = Poly2Tri.Triangulation.Polygon.Polygon;

namespace Sledge.BspEditor.Tools.Brush.Brushes
{
    [Export(typeof(IBrush))]
    [Export(typeof(IInitialiseHook))]
    [OrderHint("T")]
    [AutoTranslate]
    public class TextBrush : IBrush, IInitialiseHook
    {
        private FontChooserControl _fontChooser;
        private NumericControl _flattenFactor;
        private TextControl _text;
        
        public string Font { get; set; }
        public string AliasingFactor { get; set; }
        public string Text { get; set; }
        public string EnteredText { get; set; }

        public Task OnInitialise()
        {
            _fontChooser = new FontChooserControl(this) { LabelText = Font };
            _flattenFactor = new NumericControl(this) { LabelText = AliasingFactor, Minimum = 0.1m, Maximum = 10m, Value = 1, Precision = 1, Increment = 0.1m };
            _text = new TextControl(this) { EnteredText = EnteredText, LabelText = Text };
            return Task.CompletedTask;
        }

        public string Name { get; set; } = "Text";
        public bool CanRound => true;

        public IEnumerable<BrushControl> GetControls()
        {
            yield return _fontChooser;
            yield return _flattenFactor;
            yield return _text;
        }

        public IEnumerable<IMapObject> Create(UniqueNumberGenerator generator, Box box, string texture, int roundfloats)
        {
            var length = Math.Max(1, Math.Abs((int) box.Length));
            var height = box.Height;
            var flatten = (float) _flattenFactor.Value;
            var text = _text.GetValue();

            var family = _fontChooser.GetFontFamily();
            var style = Enum.GetValues(typeof (FontStyle)).OfType<FontStyle>().FirstOrDefault(fs => family.IsStyleAvailable(fs));
            if (!family.IsStyleAvailable(style)) family = FontFamily.GenericSansSerif;

            var set = new List<Polygon>();

            var sizes = new List<RectangleF>();
            using (var bmp = new Bitmap(1,1))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    using (var font = new Font(family, length, style, GraphicsUnit.Pixel))
                    {
                        for (var i = 0; i < text.Length; i += 32)
                        {
                            using (var sf = new StringFormat(StringFormat.GenericTypographic))
                            {
                                var rem = Math.Min(text.Length, i + 32) - i;
                                var range = Enumerable.Range(0, rem).Select(x => new CharacterRange(x, 1)).ToArray();
                                sf.SetMeasurableCharacterRanges(range);
                                var reg = g.MeasureCharacterRanges(text.Substring(i, rem), font, new RectangleF(0, 0, float.MaxValue, float.MaxValue), sf);
                                sizes.AddRange(reg.Select(x => x.GetBounds(g)));
                            }
                        }
                    }
                }
            }

            var xOffset = box.Start.X;
            var yOffset = box.End.Y;

            for (var ci = 0; ci < text.Length; ci++)
            {
                var c = text[ci];
                var size = sizes[ci];

                var gp = new GraphicsPath();
                gp.AddString(c.ToString(CultureInfo.InvariantCulture), family, (int)style, length, new PointF(0, 0), StringFormat.GenericTypographic);
                gp.Flatten(new System.Drawing.Drawing2D.Matrix(), flatten);

                var polygons = new List<Polygon>();
                var poly = new List<PolygonPoint>();

                for (var i = 0; i < gp.PointCount; i++)
                {
                    var type = gp.PathTypes[i];
                    var point = gp.PathPoints[i];

                    poly.Add(new PolygonPoint(point.X + xOffset, -point.Y + yOffset));

                    if ((type & 0x80) == 0x80)
                    {
                        polygons.Add(new Polygon(poly));
                        poly.Clear();
                    }
                }

                var tri = new List<Polygon>();
                Polygon polygon = null;
                foreach (var p in polygons)
                {
                    if (polygon == null)
                    {
                        polygon = p;
                        tri.Add(p);
                    }
                    else if (p.CalculateWindingOrder() != polygon.CalculateWindingOrder())
                    {
                        polygon.AddHole(p);
                    }
                    else
                    {
                        polygon = null;
                        tri.Add(p);
                    }
                }

                foreach (var pp in tri)
                {
                    try
                    {
                        P2T.Triangulate(pp);
                        set.Add(pp);
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                xOffset += size.Width;
            }

            var zOffset = box.Start.Z;

            foreach (var polygon in set)
            {
                foreach (var t in polygon.Triangles)
                {
                    var points = t.Points.Select(x => new Vector3((float) x.X, (float) x.Y, zOffset).Round(roundfloats)).ToList();

                    var faces = new List<Vector3[]>();

                    // Add the vertical faces
                    var z = new Vector3(0, 0, height).Round(roundfloats);
                    for (var j = 0; j < points.Count; j++)
                    {
                        var next = (j + 1) % points.Count;
                        faces.Add(new[] {points[j], points[j] + z, points[next] + z, points[next]});
                    }
                    // Add the top and bottom faces
                    faces.Add(points.ToArray());
                    faces.Add(points.Select(x => x + z).Reverse().ToArray());

                    // Nothing new here, move along
                    var solid = new Solid(generator.Next("MapObject"));
                    solid.Data.Add(new ObjectColor(Colour.GetRandomBrushColour()));

                    foreach (var arr in faces)
                    {
                        var face = new Face(generator.Next("Face"))
                        {
                            Plane = new Plane(arr[0], arr[1], arr[2]),
                            Texture = { Name = texture }
                        };
                        face.Vertices.AddRange(arr.Select(x => x.Round(roundfloats)));
                        solid.Data.Add(face);
                    }
                    solid.DescendantsChanged();
                    yield return solid;
                }
            }
        }
    }
}
