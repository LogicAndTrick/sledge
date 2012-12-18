using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Sledge.Common;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;

namespace Sledge.Providers.Map
{
    public class VmfProvider : MapProvider
    {
        protected override bool IsValidForFileName(string filename)
        {
            return filename.EndsWith(".vmf");
        }

        private static EntityData ReadEntityData(GenericStructure structure)
        {
            var ret = new EntityData();
            foreach (var kv in structure.Properties)
            {
                ret.Properties.Add(new Property {Key = kv.Key, Value = kv.Value});
            }
            return ret;
        }

        private static Displacement ReadDisplacement(GenericStructure dispinfo)
        {
            var disp = new Displacement();
            // power, startposition, flags, elevation, subdiv, normals{}, distances{},
            // offsets{}, offset_normals{}, alphas{}, triangle_tags{}, allowed_verts{}
            disp.SetPower(dispinfo.PropertyInteger("power", 3));
            disp.StartPosition = dispinfo.PropertyCoordinate("startposition");
            disp.Elevation = dispinfo.PropertyDecimal("elevation");
            disp.SubDiv = dispinfo.PropertyInteger("subdiv") > 0;
            var size = disp.Resolution + 1;
            var normals = dispinfo.GetChildren("normals").FirstOrDefault();
            var distances = dispinfo.GetChildren("distances").FirstOrDefault();
            var offsets = dispinfo.GetChildren("offsets").FirstOrDefault();
            var offsetNormals = dispinfo.GetChildren("offset_normals").FirstOrDefault();
            var alphas = dispinfo.GetChildren("alphas").FirstOrDefault();
            //var triangleTags = dispinfo.GetChildren("triangle_tags").First();
            //var allowedVerts = dispinfo.GetChildren("allowed_verts").First();
            for (var i = 0; i < size; i++)
            {
                var row = "row" + i;
                var norm = normals != null ? normals.PropertyCoordinateArray(row, size) : Enumerable.Range(0, size).Select(x => Coordinate.Zero).ToArray();
                var dist = distances != null ? distances.PropertyDecimalArray(row, size) : Enumerable.Range(0, size).Select(x => 0m).ToArray();
                var offn = offsetNormals != null ? offsetNormals.PropertyCoordinateArray(row, size) : Enumerable.Range(0, size).Select(x => Coordinate.Zero).ToArray();
                var offs = offsets != null ? offsets.PropertyDecimalArray(row, size) : Enumerable.Range(0, size).Select(x => 0m).ToArray();
                var alph = alphas != null ? alphas.PropertyDecimalArray(row, size) : Enumerable.Range(0, size).Select(x => 0m).ToArray();
                for (var j = 0; j < size; j++)
                {
                    disp.Points[i, j].Displacement = new Vector(norm[j], dist[j]);
                    disp.Points[i, j].OffsetDisplacement = new Vector(offn[j], offs[j]);
                    disp.Points[i, j].Alpha = alph[j];
                }
            }
            return disp;
        }

        private static Face ReadFace(GenericStructure side)
        {
            var dispinfo = side.GetChildren("dispinfo").FirstOrDefault();
            var ret = dispinfo != null ? ReadDisplacement(dispinfo) : new Face();
            // id, plane, material, uaxis, vaxis, rotation, lightmapscale, smoothing_groups
            var uaxis = side.PropertyTextureAxis("uaxis");
            var vaxis = side.PropertyTextureAxis("vaxis");
            ret.Texture.Name = side["material"];
            ret.Texture.UAxis = uaxis.Item1;
            ret.Texture.XShift = uaxis.Item2;
            ret.Texture.XScale = uaxis.Item3;
            ret.Texture.VAxis = vaxis.Item1;
            ret.Texture.YShift = vaxis.Item2;
            ret.Texture.YScale = vaxis.Item3;
            ret.Texture.Rotation = side.PropertyDecimal("rotation");
            ret.Plane = side.PropertyPlane("plane");
            return ret;
        }

        private static Solid ReadSolid(GenericStructure solid)
        {
            var editor = solid.GetChildren("editor").FirstOrDefault() ?? new GenericStructure("editor");
            var faces = solid.GetChildren("side").Select(ReadFace).ToList();

            var ret = Solid.CreateFromIntersectingPlanes(faces.Select(x => x.Plane));
            ret.Colour = editor.PropertyColour("color", Colour.GetRandomBrushColour());

            for (var i = 0; i < ret.Faces.Count; i++)
            {
                var face = ret.Faces[i];
                var f = faces.FirstOrDefault(x => x.Plane.Normal.EquivalentTo(ret.Faces[i].Plane.Normal));
                if (f == null)
                {
                    // TODO: Report invalid solids
                    Debug.WriteLine("Invalid solid! ID: " + solid["id"]);
                    return null;
                }
                if (f is Displacement)
                {
                    var disp = (Displacement) f;
                    disp.Plane = face.Plane;
                    disp.Vertices = face.Vertices;
                    disp.UpdateBoundingBox();
                    disp.AlignTextureToWorld();
                    disp.CalculatePoints();
                    ret.Faces[i] = disp;
                    face = disp;
                }
                face.Texture = f.Texture;
                face.Parent = ret;
                face.Colour = ret.Colour;
            }

            if (ret.Faces.Any(x => x is Displacement))
            {
                ret.Faces.ForEach(x => x.IsHidden = !(x is Displacement));
            }

            ret.UpdateBoundingBox(false);

            return ret;
        }

        private static Entity ReadEntity(GenericStructure entity)
        {
            var ret = new Entity
                          {
                              ClassName = entity["classname"],
                              EntityData = ReadEntityData(entity)
                          };
            foreach (var solid in entity.GetChildren("solid"))
            {
                var s = ReadSolid(solid);
                if (s != null) ret.Children.Add(s);
            }
            return ret;
        }

        private static World ReadWorld(GenericStructure world)
        {
            var ret = new World
                          {
                              ClassName = "worldspawn",
                              EntityData = ReadEntityData(world)
                          };
            foreach (var solid in world.GetChildren("solid"))
            {
                var s = ReadSolid(solid);
                if (s != null) ret.Children.Add(s);
            }
            foreach (var group in world.GetChildren("group"))
            {
                // TODO: load groups
            }
            return ret;
        }

        protected override DataStructures.MapObjects.Map GetFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var parent = new GenericStructure("Root");
                parent.Children.AddRange(GenericStructure.Parse(reader));
                // Sections from a Hammer map:
                // versioninfo
                // visgroups
                // viewsettings
                // world
                // entity
                // cameras
                // cordon
                // (why does hammer save these in reverse alphabetical?)

                var map = new DataStructures.MapObjects.Map();

                var world = parent.GetChildren("world").FirstOrDefault();
                var entities = parent.GetChildren("entity");
                var visgroups = parent.GetChildren("visgroups").FirstOrDefault();
                var cameras = parent.GetChildren("cameras").FirstOrDefault();

                if (world != null) map.WorldSpawn = ReadWorld(world);
                foreach (var entity in entities)
                {
                    map.WorldSpawn.Children.Add(ReadEntity(entity));
                }

                return map;
            }
        }

        protected override void SaveToStream(Stream stream, DataStructures.MapObjects.Map map)
        {
            throw new NotImplementedException();
        }
    }
}
