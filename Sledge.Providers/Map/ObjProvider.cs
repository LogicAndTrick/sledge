using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        private void Read(DataStructures.MapObjects.Map map, StreamReader reader)
        {
            var points = new List<Coordinate>();

            string line;
            while ((line = CleanLine(reader.ReadLine())) != null)
            {
                string keyword, values;
                SplitLine(line, out keyword, out values);
                switch (keyword.ToLower())
                {
                    // Vertex data
                    case "v": // geometric vertices
                        break;
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
                    case "f": // face
                        break;
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
                    case "g": // group name
                        break;
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
                }
            }
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
            throw new NotImplementedException();
        }
    }
}
