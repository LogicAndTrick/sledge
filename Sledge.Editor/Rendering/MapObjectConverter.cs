using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Sledge.Common;
using Sledge.DataStructures.GameData;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.MapObjects;
using Sledge.DataStructures.Transformations;
using Sledge.Editor.Documents;
using Sledge.Editor.Extensions;
using Sledge.Providers.Texture;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Materials;
using Sledge.Rendering.Scenes;
using Sledge.Rendering.Scenes.Renderables;
using Face = Sledge.Rendering.Scenes.Renderables.Face;
using Vertex = Sledge.Rendering.Scenes.Renderables.Vertex;

namespace Sledge.Editor.Rendering
{
    public static class MapObjectConverter
    {
        public static SceneMapObject Convert(this MapObject obj, Document document)
        {
            if (obj.IsCodeHidden || obj.IsVisgroupHidden) return null;
            if (obj is Solid) return Convert((Solid)obj, document);
            if (obj is Entity) return Convert((Entity) obj, document);
            return null;
        }

        public static bool Update(this MapObject obj, SceneMapObject smo, Document document)
        {
            if (obj.IsCodeHidden || obj.IsVisgroupHidden) return false;
            if (obj is Solid) return Update((Solid)obj, smo, document);
            if (obj is Entity) return Update((Entity)obj, smo, document);
            return false;
        }

        #region Convert Solid
        public static SceneMapObject Convert(this Solid solid, Document document)
        {
            var smo = new SceneMapObject(solid);
            foreach (var face in solid.Faces.Where(x => !x.IsHidden))
            {
                var f = Convert(face, document);
                smo.SceneObjects.Add(face, f);
            }
            return smo;
        }

        public static bool Update(this Solid solid, SceneMapObject smo, Document document)
        {
            if (smo.SceneObjects.Count != solid.Faces.Count(x => !x.IsHidden)) return false;
            var values = smo.SceneObjects.Values.ToList();
            var objs = new Dictionary<object, SceneObject>();
            for (int i = 0; i < solid.Faces.Count; i++)
            {
                var face = solid.Faces[i];
                if (!Update(face, (Face)values[i], document)) return false;
                objs.Add(face, values[i]);
            }
            smo.SceneObjects.Clear();
            foreach (var kv in objs) smo.SceneObjects.Add(kv.Key, kv.Value);
            return true;
        }
        #endregion

        #region Convert Entity
        public static SceneMapObject Convert(this Entity entity, Document document)
        {
            var smo = new SceneMapObject(entity);
            var flags = CameraFlags.All;
            if (entity.ShouldHaveDecal())
            {
                var decalName = entity.GetDecalName();
                var tex = document.TextureCollection.GetItem(decalName);
                if (tex != null)
                {
                    var geo = CalculateDecalGeometry(entity, tex, document);
                    foreach (var face in geo)
                    {
                        var f = Convert(face, document);
                        f.RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe;
                        smo.SceneObjects.Add(face, f);
                    }
                }
            }
            else if (false) // todo: should have model
            {
                
            }
            else if (entity.ShouldHaveSprite())
            {
                var spriteName = entity.GetSpriteName();
                var tex = document.TextureCollection.GetItem(spriteName);
                if (tex != null)
                {
                    var spr = CreateSpriteData(entity, tex, spriteName);
                    smo.SceneObjects.Add(entity, spr);
                    flags = CameraFlags.Orthographic;
                }
                else
                {
                    ClearSpriteData(entity);
                }
            }
            else
            {
                ClearSpriteData(entity);
            }

            foreach (var face in entity.GetBoxFaces())
            {
                var f = Convert(face, document);
                f.RenderFlags = RenderFlags.Polygon | RenderFlags.Wireframe;
                f.CameraFlags = flags;
                smo.SceneObjects.Add(face, f);
            }
            return smo;
        }

        public static bool Update(this Entity entity, SceneMapObject smo, Document document)
        {
            return false;
            // if (smo.SceneObjects.Count != entity.Faces.Count) return false;
            // var values = smo.SceneObjects.Values.ToList();
            // var objs = new Dictionary<object, SceneObject>();
            // for (int i = 0; i < entity.Faces.Count; i++)
            // {
            //     var face = entity.Faces[i];
            //     if (!Update(face, (Face)values[i], document)) return false;
            //     objs.Add(face, values[i]);
            // }
            // smo.SceneObjects.Clear();
            // foreach (var kv in objs) smo.SceneObjects.Add(kv.Key, kv.Value);
            // return true;
        }
        #endregion

        #region Entity - Sprites
        private static bool ShouldHaveSprite(this Entity entity)
        {
            return GetSpriteName(entity) != null;
        }

        private static string GetSpriteName(this Entity entity)
        {
            if (entity.GameData == null) return null;
            var spr = entity.GameData.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase))
                ?? entity.GameData.Behaviours.FirstOrDefault(x => String.Equals(x.Name, "iconsprite", StringComparison.InvariantCultureIgnoreCase));
            if (spr == null) return null;

            // First see if the studio behaviour forces a model...
            if (spr.Values.Count == 1 && !String.IsNullOrWhiteSpace(spr.Values[0]))
            {
                return spr.Values[0].Trim();
            }

            // Find the first property that is a studio type, or has a name of "sprite"...
            var prop = entity.GameData.Properties.FirstOrDefault(x => x.VariableType == VariableType.Sprite);
            if (prop == null) prop = entity.GameData.Properties.FirstOrDefault(x => String.Equals(x.Name, "sprite", StringComparison.InvariantCultureIgnoreCase));
            if (prop != null)
            {
                var val = entity.EntityData.GetPropertyValue(prop.Name);
                if (!String.IsNullOrWhiteSpace(val)) return val;
            }
            return null;
        }

        private static void ClearSpriteData(Entity entity)
        {
            if (!entity.MetaData.Has<Box>("BoundingBox") && !entity.MetaData.Has<bool>("RotateBoundingBox")) return;
            entity.MetaData.Unset("BoundingBox");
            entity.MetaData.Unset("RotateBoundingBox");
            entity.UpdateBoundingBox();
        }

        private static Sprite CreateSpriteData(Entity entity, TextureItem tex, string spriteName)
        {
            var scale = 1m;
            if (entity.GameData != null && entity.GameData.Properties.Any(x => String.Equals(x.Name, "scale", StringComparison.CurrentCultureIgnoreCase)))
            {
                var scaleStr = entity.GetEntityData().GetPropertyValue("scale");
                if (!Decimal.TryParse(scaleStr, out scale)) scale = 1;
                if (scale <= 0.1m) scale = 1;
            }
            var bb = new Coordinate(tex.Width, tex.Width, tex.Height) * scale;

            // Don't set the bounding box if the sprite comes from the iconsprite gamedata
            if (entity.GameData == null || !entity.GameData.Behaviours.Any(x => String.Equals(x.Name, "iconsprite", StringComparison.CurrentCultureIgnoreCase)))
            {
                entity.MetaData.Set("BoundingBox", new Box(-bb / 2, bb / 2));
                entity.MetaData.Set("RotateBoundingBox", false); // todo rotations
                entity.UpdateBoundingBox();
            }

            var spr = new Sprite(entity.Origin.ToVector3(), Material.Texture(spriteName, true), (float) bb.X, (float) bb.Z);
            return spr;
        }
        #endregion

        #region Entity - Decals

        private static bool ShouldHaveDecal(this Entity entity)
        {
            return GetDecalName(entity) != null;
        }

        private static string GetDecalName(this Entity entity)
        {
            if (entity.EntityData.Name != "infodecal") return null;

            var decal = entity.EntityData.Properties.FirstOrDefault(x => x.Key == "texture");
            if (decal == null) return null;

            return decal.Value;
        }

        private static IEnumerable<DataStructures.MapObjects.Face> CalculateDecalGeometry(Entity entity, TextureItem decal, Document document)
        {
            var decalGeometry = new List<DataStructures.MapObjects.Face>();
            if (decal == null || entity.Parent == null) return decalGeometry; // Texture not found

            var boxRadius = Coordinate.One * 4;
            // Decals apply to all faces that intersect within an 8x8x8 bounding box
            // centered at the origin of the decal
            var box = new Box(entity.Origin - boxRadius, entity.Origin + boxRadius);
            var root = MapObject.GetRoot(entity.Parent);
            // Get the faces that intersect with the decal's radius
            var faces = root.GetAllNodesIntersectingWith(box).OfType<Solid>()
                    .SelectMany(x => x.Faces).Where(x => x.IntersectsWithBox(box));
            var idg = new IDGenerator(); // Dummy generator
            foreach (var face in faces)
            {
                // Project the decal onto the face
                var center = face.Plane.Project(entity.Origin);
                var texture = face.Texture.Clone();
                texture.Name = decal.Name;
                texture.Texture = document.GetTexture(decal.Name);
                texture.XShift = -decal.Width / 2m;
                texture.YShift = -decal.Height / 2m;
                var decalFace = new DataStructures.MapObjects.Face(idg.GetNextFaceID())
                {
                    Colour = entity.Colour,
                    IsSelected = entity.IsSelected,
                    IsHidden = entity.IsCodeHidden,
                    Plane = face.Plane,
                    Texture = texture
                };
                // Re-project the vertices in case the texture axes are not on the face plane
                var xShift = face.Texture.UAxis * face.Texture.XScale * decal.Width / 2;
                var yShift = face.Texture.VAxis * face.Texture.YScale * decal.Height / 2;
                var verts = new[]
                                {
                                    new DataStructures.MapObjects.Vertex(face.Plane.Project(center + xShift - yShift), decalFace), // Bottom Right
                                    new DataStructures.MapObjects.Vertex(face.Plane.Project(center + xShift + yShift), decalFace), // Top Right
                                    new DataStructures.MapObjects.Vertex(face.Plane.Project(center - xShift + yShift), decalFace), // Top Left
                                    new DataStructures.MapObjects.Vertex(face.Plane.Project(center - xShift - yShift), decalFace)  // Bottom Left
                                };

                // Because the texture axes don't have to align to the face, we might have a reversed face here
                // If so, reverse the points to get a valid face for the plane.
                // TODO: Is there a better way to do this?
                var vertPlane = new Plane(verts[0].Location, verts[1].Location, verts[2].Location);
                if (!face.Plane.Normal.EquivalentTo(vertPlane.Normal))
                {
                    Array.Reverse(verts);
                }

                decalFace.Vertices.AddRange(verts);
                decalFace.UpdateBoundingBox();

                // Calculate the X and Y shift bases on the first vertex location (assuming U/V of first vertex is zero) - we dont want these to change
                var vtx = decalFace.Vertices[0];
                decalFace.Texture.XShift = -(vtx.Location.Dot(decalFace.Texture.UAxis)) / decalFace.Texture.XScale;
                decalFace.Texture.YShift = -(vtx.Location.Dot(decalFace.Texture.VAxis)) / decalFace.Texture.YScale;
                decalFace.CalculateTextureCoordinates(true);

                // Next, the decal geometry needs to be clipped to the face so it doesn't spill into the void
                // Create a fake solid out of the decal geometry and clip it against all the brush planes
                var fake = CreateFakeDecalSolid(decalFace);

                foreach (var f in face.Parent.Faces.Except(new[] { face }))
                {
                    Solid back, front;
                    fake.Split(f.Plane, out back, out front, idg);
                    fake = back ?? fake;
                }

                // Extract out the original face
                decalFace = fake.Faces.FirstOrDefault(x => x.Plane.EquivalentTo(face.Plane, 0.05m));
                if (decalFace == null) continue;

                // Add a tiny bit to the normal axis to ensure the decal is rendered in front of the face
                var normalAdd = face.Plane.Normal * 0.2m;
                decalFace.Transform(new UnitTranslate(normalAdd), TransformFlags.TextureLock);

                decalFace.IsSelected = entity.IsSelected;
                decalGeometry.Add(decalFace);
            }
            return decalGeometry;
        }

        private static Solid CreateFakeDecalSolid(DataStructures.MapObjects.Face face)
        {
            var s = new Solid(0)
            {
                Colour = face.Colour,
                IsVisgroupHidden = face.IsHidden,
                IsSelected = face.IsSelected
            };
            s.Faces.Add(face);
            var p = face.BoundingBox.Center - face.Plane.Normal * 10; // create a new point underneath the face
            var p1 = face.Vertices[0].Location;
            var p2 = face.Vertices[1].Location;
            var p3 = face.Vertices[2].Location;
            var p4 = face.Vertices[3].Location;
            var faces = new[]
                            {
                                new[] { p2, p1, p},
                                new[] { p3, p2, p},
                                new[] { p4, p3, p},
                                new[] { p1, p4, p}
                            };
            foreach (var ff in faces)
            {
                var f = new DataStructures.MapObjects.Face(-1)
                {
                    Colour = face.Colour,
                    IsSelected = face.IsSelected,
                    IsHidden = face.IsHidden,
                    Plane = new Plane(ff[0], ff[1], ff[2])
                };
                f.Vertices.AddRange(ff.Select(x => new DataStructures.MapObjects.Vertex(x, f)));
                f.UpdateBoundingBox();
                s.Faces.Add(f);
            }
            s.UpdateBoundingBox();
            return s;
        }
        #endregion

        #region Convert Face
        public static Face Convert(this DataStructures.MapObjects.Face face, Document document)
        {
            var tex = document.TextureCollection.GetItem(face.Texture.Name);
            var mat = tex == null ? Material.Flat(face.Colour) : Material.Texture(tex.Name, tex.Flags.HasFlag(TextureFlags.Transparent));
            var sel = face.IsSelected || (face.Parent != null && face.Parent.IsSelected);
            return new Face(mat, face.Vertices.Select(x => new Vertex(x.Location.ToVector3(), (float)x.TextureU, (float)x.TextureV)).ToList())
            {
                AccentColor = sel ? Color.Red : face.Colour,
                TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White,
                IsSelected = sel
            };
        }

        public static bool Update(this DataStructures.MapObjects.Face face, Face sceneFace, Document document)
        {
            var tex = document.TextureCollection.GetItem(face.Texture.Name);
            var mat = tex == null ? Material.Flat(face.Colour) : Material.Texture(tex.Name, tex.Flags.HasFlag(TextureFlags.Transparent));
            var sel = face.IsSelected || (face.Parent != null && face.Parent.IsSelected);

            sceneFace.Material = mat;
            sceneFace.Vertices = face.Vertices.Select(x => new Vertex(x.Location.ToVector3(), (float) x.TextureU, (float) x.TextureV)).ToList();
            sceneFace.AccentColor = sel ? Color.Red : face.Colour;
            sceneFace.TintColor = sel ? Color.FromArgb(128, Color.Red) : Color.White;
            sceneFace.IsSelected = sel;

            return true;
        }
        #endregion
    }
}