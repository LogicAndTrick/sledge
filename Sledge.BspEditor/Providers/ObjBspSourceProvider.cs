using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common;
using Sledge.Common.Shell.Documents;
using Sledge.DataStructures.Geometric;
using Plane = Sledge.DataStructures.Geometric.Plane;

namespace Sledge.BspEditor.Providers
{
    [Export(typeof(IBspSourceProvider))]
    public class ObjBspSourceProvider : IBspSourceProvider
    {
        private static readonly IEnumerable<Type> SupportedTypes = new List<Type>
        {
            // Sledge only supports solids in the OBJ format
            typeof(Solid),
        };

        public IEnumerable<Type> SupportedDataTypes => SupportedTypes;

        public IEnumerable<FileExtensionInfo> SupportedFileExtensions { get; } = new[]
        {
            new FileExtensionInfo("Wavefront model format", ".obj")
        };

        public Task<BspFileLoadResult> Load(Stream stream, IEnvironment environment)
        {
            return Task.Run(() =>
            {
                using (var reader = new StreamReader(stream, Encoding.ASCII, true, 1024, false))
                {
                    var result = new BspFileLoadResult();

                    var map = new Map();

                    Read(map, reader);

                    result.Map = map;
                    return result;
                }
            });
        }

        public Task Save(Stream stream, Map map)
        {
            return Task.Run(() =>
            {
                using (var writer = new StreamWriter(stream, Encoding.ASCII, 1024, true))
                {
                    Write(map, writer);
                }
            });
        }

        #region Reading

        private struct ObjFace
        {
            public string Group { get; }
            public List<int> Vertices { get; }

            public ObjFace(string group, IEnumerable<int> vertices) : this()
            {
                Group = group;
                Vertices = vertices.ToList();
            }
        }

        private string CleanLine(string line)
        {
            if (line == null) return null;
            return line.StartsWith("#") ? "" : line.Trim();
        }

        private void Read(Map map, StreamReader reader)
        {
            const NumberStyles ns = NumberStyles.Float;

            var points = new List<Vector3>();
            var faces = new List<ObjFace>();
            var currentGroup = "default";
            var scale = 100f;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("# Scale: "))
                {
                    var num = line.Substring(9);
                    if (float.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out var s))
                    {
                        scale = s;
                    }
                }

                line = CleanLine(line);
                SplitLine(line, out var keyword, out var values);
                if (String.IsNullOrWhiteSpace(keyword)) continue;

                var vals = (values ?? "").Split(' ').Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
                switch (keyword.ToLower())
                {
                    // Things I care about
                    case "v": // geometric vertices
                        var vec = NumericsExtensions.Parse(vals[0], vals[1], vals[2], ns, CultureInfo.InvariantCulture);
                        points.Add(vec * scale);
                        break;
                    case "f": // face
                        faces.Add(new ObjFace(currentGroup, vals.Select(x => ParseFaceIndex(points, x))));
                        break;
                    case "g": // group name
                        currentGroup = (values ?? "").Trim();
                        break;

                    // Things I don't care about
                    #region Not Implemented

                    // Vertex data
                    // "v"
                    case "vt": // texture vertices
                        break;
                    case "vn": // vertex normals
                        break;
                    case "vp": // parameter space vertices
                    case "cstype": // rational or non-rational forms of curve or surface type: basis matrix, Bezier, B-spline, Cardinal, Taylor
                    case "degree": // degree
                    case "bmat": // basis matrix
                    case "step": // step size
                        // not supported
                        break;

                    // Elements
                    // "f"
                    case "p": // point
                    case "l": // line
                    case "curv": // curve
                    case "curv2": // 2D curve
                    case "surf": // surface
                        // not supported
                        break;

                    // Free-form curve/surface body statements
                    case "parm": // parameter name
                    case "trim": // outer trimming loop (trim)
                    case "hole": // inner trimming loop (hole)
                    case "scrv": // special curve (scrv)
                    case "sp":  // special point (sp)
                    case "end": // end statement (end)
                        // not supported
                        break;

                    // Connectivity between free-form surfaces
                    case "con": // connect
                        // not supported
                        break;

                    // Grouping
                    // "g"
                    case "s": // smoothing group
                        break;
                    case "mg": // merging group
                        break;
                    case "o": // object name
                        // not supported
                        break;

                    // Display/render attributes
                    case "mtllib": // material library
                    case "usemtl": // material name
                    case "usemap": // texture map name
                    case "bevel": // bevel interpolation
                    case "c_interp": // color interpolation
                    case "d_interp": // dissolve interpolation
                    case "lod": // level of detail
                    case "shadow_obj": // shadow casting
                    case "trace_obj": // ray tracing
                    case "ctech": // curve approximation technique
                    case "stech": // surface approximation technique
                        // not relevant
                        break;

                        #endregion
                }
            }

            var solids = new List<Solid>();

            // Try and see if we have a valid solid per-group
            foreach (var g in faces.GroupBy(x => x.Group))
            {
                solids.AddRange(CreateSolids(map, points, g));
            }

            foreach (var solid in solids)
            {
                foreach (var face in solid.Faces)
                {
                    face.Texture.AlignToNormal(face.Plane.Normal);
                }

                solid.Hierarchy.Parent = map.Root;
            }

            map.Root.DescendantsChanged();
        }

        private IEnumerable<Solid> CreateSolids(Map map, List<Vector3> points, IEnumerable<ObjFace> objFaces)
        {
            var faces = objFaces.Select(x => CreateFace(map, points, x)).ToList();

            // See if the solid is valid
            var solid = new Solid(map.NumberGenerator.Next("MapObject"));
            solid.Data.Add(new ObjectColor(Colour.GetRandomBrushColour()));
            solid.Data.AddRange(faces);
            if (solid.IsValid())
            {
                // Do an additional check to ensure that all edges are shared
                var edges = solid.Faces.SelectMany(x => x.GetEdges()).ToList();
                if (edges.All(x => edges.Count(y => x.EquivalentTo(y)) == 2))
                {
                    // Valid! let's get out of here!
                    yield return solid;
                    yield break;
                }
            }

            // Not a valid solid, decompose into tetrahedrons/etc
            foreach (var face in faces)
            {
                var polygon = face.ToPolygon();
                if (!polygon.IsValid() || !polygon.IsConvex())
                {
                    // tetrahedrons
                    foreach (var triangle in GetTriangles(face))
                    {
                        var tf = new Face(map.NumberGenerator.Next("Face"))
                        {
                            Plane = new Plane(triangle[0], triangle[1], triangle[2])
                        };
                        tf.Vertices.AddRange(triangle);
                        yield return SolidifyFace(map, tf);
                    }
                }
                else
                {
                    // cone/pyramid/whatever
                    yield return SolidifyFace(map, face);
                }
            }
        }

        private IEnumerable<Vector3[]> GetTriangles(Face face)
        {
            for (var i = 1; i < face.Vertices.Count - 1; i++)
            {
                yield return new[]
                {
                    face.Vertices[0],
                    face.Vertices[i],
                    face.Vertices[i + 1]
                };
            }
        }

        private Solid SolidifyFace(Map map, Face face)
        {
            var solid = new Solid(map.NumberGenerator.Next("MapObject"));
            solid.Data.Add(new ObjectColor(Colour.GetRandomBrushColour()));
            solid.Data.Add(face);

            var center = face.Vertices.Aggregate(Vector3.Zero, (sum, v) => sum + v) / face.Vertices.Count;
            var offset = center - face.Plane.Normal * 5;
            for (var i = 0; i < face.Vertices.Count; i++)
            {
                var v1 = face.Vertices[i];
                var v2 = face.Vertices[(i + 1) % face.Vertices.Count];

                var f = new Face(map.NumberGenerator.Next("Face"))
                {
                    Plane = new Plane(v1, offset, v2)
                };

                f.Vertices.Add(offset);
                f.Vertices.Add(v2);
                f.Vertices.Add(v1);

                solid.Data.Add(f);
            }

            solid.DescendantsChanged();
            return solid;
        }


        private Face CreateFace(Map map, List<Vector3> points, ObjFace objFace)
        {
            var verts = objFace.Vertices.Select(x => points[x]).ToList();

            var f = new Face(map.NumberGenerator.Next("Face"))
            {
                Plane = new Plane(verts[2], verts[1], verts[0])
            };

            verts.Reverse();
            f.Vertices.AddRange(verts);

            return f;
        }

        private int ParseFaceIndex(List<Vector3> list, string index)
        {
            if (index.Contains('/')) index = index.Substring(0, index.IndexOf('/'));
            var idx = int.Parse(index);

            if (idx < 0)
            {
                idx = list.Count + idx;
            }
            else
            {
                idx -= 1;
            }
            //
            return idx;
        }

        private static void SplitLine(string line, out string keyword, out string arguments)
        {
            var idx = line.IndexOf(' ');
            if (idx < 0)
            {
                keyword = line;
                arguments = null;
                return;
            }

            keyword = line.Substring(0, idx);
            arguments = line.Substring(idx + 1);
        }

        #endregion

        #region Writing

        private void Write(Map map, StreamWriter writer)
        {
            writer.WriteLine("# Sledge Object Export");
            writer.WriteLine("# Scale: 1");
            writer.WriteLine();

            foreach (var solid in map.Root.Find(x => x is Solid).OfType<Solid>())
            {
                writer.Write("g solid_");
                writer.Write(solid.ID);
                writer.WriteLine();

                foreach (var face in solid.Faces)
                {
                    foreach (var v in face.Vertices)
                    {
                        writer.Write("v ");
                        writer.Write(v.X.ToString("0.0000", CultureInfo.InvariantCulture));
                        writer.Write(' ');
                        writer.Write(v.Y.ToString("0.0000", CultureInfo.InvariantCulture));
                        writer.Write(' ');
                        writer.Write(v.Z.ToString("0.0000", CultureInfo.InvariantCulture));
                        writer.WriteLine();
                    }

                    writer.Write("f ");
                    for (var i = 1; i <= face.Vertices.Count; i++)
                    {
                        writer.Write(-i);
                        writer.Write(' ');
                    }
                    writer.WriteLine();
                    writer.WriteLine();
                }
            }
        }

        #endregion
    }
}
