using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Providers.Map
{
    // Parts of this code is borrowed from the Helix Toolkit: http://helixtoolkit.codeplex.com, license: MIT
    // http://www.martinreddy.net/gfx/3d/OBJ.spec
    /// <summary>
    /// OBJ is a model format and does not have the intersecting planes limitation.
    /// Detection of a convex solid is performed and on the off chance it is okay a solid is generated,
    /// but in most cases the loader must fall back to tetrehedrons.
    /// OBJ also embeds textures which doesn't work the the standard resource model of a Sledge map:-
    /// all texture data is ignored and untextured faces are generated.
    /// </summary>
    public class ObjProvider : MapProvider
    {
        protected override IEnumerable<MapFeature> GetFormatFeatures()
        {
            return new[]
            {
                MapFeature.Solids
            };
        }

        protected override bool IsValidForFileName(string filename)
        {
            return filename.EndsWith(".obj");
        }

        private string CleanLine(string line)
        {
            if (line == null) return null;
            return line.StartsWith("#") ? "" : line.Trim();
        }

        protected override DataStructures.MapObjects.Map GetFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var map = new DataStructures.MapObjects.Map();
                Read(map, reader);
                /*
                var allentities = ReadAllEntities(reader, map.IDGenerator);
                var worldspawn = allentities.FirstOrDefault(x => x.EntityData.Name == "worldspawn")
                                 ?? new Entity(0) { EntityData = { Name = "worldspawn" } };
                allentities.Remove(worldspawn);
                map.WorldSpawn.EntityData = worldspawn.EntityData;
                allentities.ForEach(x => x.SetParent(map.WorldSpawn));
                foreach (var obj in worldspawn.Children.ToArray())
                {
                    obj.SetParent(map.WorldSpawn);
                }
                map.WorldSpawn.UpdateBoundingBox(false);
                 */
                return map;
            }
        }

        private struct ObjFace
        {
            public string Group { get; set; }
            public List<int> Vertices { get; set; }

            public ObjFace(string group, IEnumerable<int> vertices) : this()
            {
                Group = group;
                Vertices = vertices.ToList();
            }
        }

        private void Read(DataStructures.MapObjects.Map map, StreamReader reader)
        {
            var points = new List<Coordinate>();
            var faces = new List<ObjFace>();
            var currentGroup = "default";
            var scale = 100m;

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("# Scale: "))
                {
                    var num = line.Substring(9);
                    decimal s;
                    if (decimal.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out s))
                    {
                        scale = s;
                    }
                }

                line = CleanLine(line);
                string keyword, values;
                SplitLine(line, out keyword, out values);
                if (String.IsNullOrWhiteSpace(keyword)) continue;

                var vals = (values ?? "").Split(' ').Where(x => !String.IsNullOrWhiteSpace(x)).ToArray();
                switch (keyword.ToLower())
                {
                    // Things I care about
                    case "v": // geometric vertices
                        points.Add(Coordinate.Parse(vals[0], vals[1], vals[2]) * scale);
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
                    face.Colour = solid.Colour;
                    face.AlignTextureToFace();
                }
                solid.SetParent(map.WorldSpawn);
            }
        }

        private IEnumerable<Solid> CreateSolids(DataStructures.MapObjects.Map map, List<Coordinate> points, IEnumerable<ObjFace> objFaces)
        {
            var faces = objFaces.Select(x => CreateFace(map, points, x)).ToList();

            // See if the solid is valid
            var solid = new Solid(map.IDGenerator.GetNextObjectID());
            solid.Colour = Colour.GetRandomBrushColour();
            solid.Faces.AddRange(faces);
            faces.ForEach(x => x.Parent = solid);
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
                var polygon = new Polygon(face.Vertices.Select(x => x.Location));
                if (!polygon.IsValid() || !polygon.IsConvex())
                {
                    // tetrahedrons
                    foreach (var triangle in face.GetTriangles())
                    {
                        var tf = new Face(map.IDGenerator.GetNextFaceID());
                        tf.Plane = new Plane(triangle[0].Location, triangle[1].Location, triangle[2].Location);
                        tf.Vertices.AddRange(triangle.Select(x => new Vertex(x.Location, tf)));
                        tf.UpdateBoundingBox();
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

        private Solid SolidifyFace(DataStructures.MapObjects.Map map, Face face)
        {
            var solid = new Solid(map.IDGenerator.GetNextObjectID());
            solid.Colour = Colour.GetRandomBrushColour();
            solid.Faces.Add(face);
            face.Parent = solid;
            var center = face.Vertices.Aggregate(Coordinate.Zero, (sum, v) => sum + v.Location) / face.Vertices.Count;
            var offset = center - face.Plane.Normal * 5;
            for (var i = 0; i < face.Vertices.Count; i++)
            {
                var v1 = face.Vertices[i];
                var v2 = face.Vertices[(i + 1) % face.Vertices.Count];
                var f = new Face(map.IDGenerator.GetNextFaceID());
                f.Parent = solid;
                f.Plane = new Plane(v1.Location, offset, v2.Location);
                f.Parent = solid;
                f.Vertices.Add(new Vertex(offset, f));
                f.Vertices.Add(new Vertex(v2.Location, f));
                f.Vertices.Add(new Vertex(v1.Location, f));
                f.UpdateBoundingBox();

                solid.Faces.Add(f);
            }
            return solid;
        }


        private Face CreateFace(DataStructures.MapObjects.Map map, List<Coordinate> points, ObjFace objFace)
        {
            var verts = objFace.Vertices.Select(x => points[x]).ToList();
            var f = new Face(map.IDGenerator.GetNextFaceID());
            f.Plane = new Plane(verts[2], verts[1], verts[0]);
            f.Vertices.AddRange(verts.Select(x => new Vertex(x, f)).Reverse());
            f.UpdateBoundingBox();
            return f;
        }

        private int ParseFaceIndex(List<Coordinate> list, string index)
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

        public static IEnumerable<string> SplitValues(string values)
        {
            return values.Split(' ', '\t').Where(x => !String.IsNullOrWhiteSpace(x));
        }

        protected override void SaveToStream(Stream stream, DataStructures.MapObjects.Map map)
        {
            /*
            // Csg version
            
            var csg = new CsgSolid();
            foreach (var mo in map.WorldSpawn.Find(x => x is Solid).OfType<Solid>())
            {
                csg = csg.Union(new CsgSolid(mo.Faces.Select(x => new Polygon(x.Vertices.Select(v => v.Location)))));
            }

            using (var sw = new StreamWriter(stream))
            {
                foreach (var polygon in csg.Polygons)
                {
                    foreach (var v in polygon.Vertices)
                    {
                        sw.Write("v ");
                        sw.Write(v.X.ToString("0.0000", CultureInfo.InvariantCulture));
                        sw.Write(' ');
                        sw.Write(v.Y.ToString("0.0000", CultureInfo.InvariantCulture));
                        sw.Write(' ');
                        sw.Write(v.Z.ToString("0.0000", CultureInfo.InvariantCulture));
                        sw.WriteLine();
                    }
                    
                    sw.Write("f ");
                    for (int i = polygon.Vertices.Count; i > 0; i--)
                    {
                        sw.Write(-i);
                        sw.Write(' ');
                    }
                    sw.WriteLine();
                    sw.WriteLine();
                }
            }
            */

            // Semi-recoverable version
            using (var sw = new StreamWriter(stream))
            {
                sw.WriteLine("# Sledge Object Export");
                sw.WriteLine("# Scale: 1");
                sw.WriteLine();

                foreach (var solid in map.WorldSpawn.Find(x => x is Solid).OfType<Solid>())
                {
                    sw.Write("g solid_");
                    sw.Write(solid.ID);
                    sw.WriteLine();

                    foreach (var face in solid.Faces)
                    {
                        foreach (var v in face.Vertices)
                        {
                            sw.Write("v ");
                            sw.Write(v.Location.X.ToString("0.0000", CultureInfo.InvariantCulture));
                            sw.Write(' ');
                            sw.Write(v.Location.Y.ToString("0.0000", CultureInfo.InvariantCulture));
                            sw.Write(' ');
                            sw.Write(v.Location.Z.ToString("0.0000", CultureInfo.InvariantCulture));
                            sw.WriteLine();
                        }

                        sw.Write("f ");
                        for (var i = 1; i <= face.Vertices.Count; i++)
                        {
                            sw.Write(-i);
                            sw.Write(' ');
                        }
                        sw.WriteLine();
                        sw.WriteLine();
                    }
                }
            }
        }
    }
}
