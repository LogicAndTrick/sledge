using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Resources;
using Sledge.Common.Shell.Settings;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Primitives;
using Sledge.Rendering.Resources;

namespace Sledge.BspEditor.Rendering.Converters
{
    [Export(typeof(IMapObjectGroupSceneConverter))]
    [Export(typeof(ISettingsContainer))]
    public class CenterHandlesConverter : IMapObjectGroupSceneConverter, ISettingsContainer
    {
        private readonly EngineInterface _engine;

        [ImportingConstructor]
        public CenterHandlesConverter([Import] Lazy<EngineInterface> engine)
        {
            _engine = engine.Value;
            _engine.UploadTexture(CenterHandleTextureDataSource.Name, HandleDataSource.Width, HandleDataSource.Height, HandleDataSource.Data, HandleDataSource.SampleType);
        }

        // Settings

        [Setting("DrawCenterHandles")] private bool _drawCenterHandles = true;
        // [Setting("CenterHandlesActiveViewportOnly")] private bool _centerHandlesActiveViewportOnly = false;

        string ISettingsContainer.Name => "Sledge.BspEditor.Rendering.Converters.CenterHandlesConverter";

        IEnumerable<SettingKey> ISettingsContainer.GetKeys()
        {
            yield return new SettingKey("Rendering", "DrawCenterHandles", typeof(bool));
            //yield return new SettingKey("Rendering", "CenterHandlesActiveViewportOnly", typeof(bool));
        }

        void ISettingsContainer.LoadValues(ISettingsStore store)
        {
            store.LoadInstance(this);
        }

        void ISettingsContainer.StoreValues(ISettingsStore store)
        {
            store.StoreInstance(this);
        }

        // Converter

        public MapObjectSceneConverterPriority Priority => MapObjectSceneConverterPriority.DefaultMedium;

        public Task Convert(BufferBuilder builder, MapDocument document, IEnumerable<IMapObject> objects, ResourceCollector resourceCollector)
        {
            if (!_drawCenterHandles) return Task.CompletedTask;

            var objs = (
                from mo in objects
                where mo is Solid || (mo is Entity && !mo.Hierarchy.HasChildren)
                where !mo.Data.OfType<IObjectVisibility>().Any(x => x.IsHidden)
                select mo
            ).ToList();

            var verts = (
                // Current centers
                from mo in objs
                let color = mo.IsSelected ? Color.Red : mo.Data.GetOne<ObjectColor>()?.Color ?? Color.White
                select new VertexStandard
                {
                    Position = mo.Data.GetOne<Origin>()?.Location ?? mo.BoundingBox.Center,
                    Normal = new Vector3(9, 9, 0),
                    Colour = color.ToVector4(),
                    Tint = Vector4.One
                }
            ).Union(
                // Selective transformed centers
                from mo in objs
                where mo.IsSelected
                let color = Color.Red
                select new VertexStandard
                {
                    Position = mo.Data.GetOne<Origin>()?.Location ?? mo.BoundingBox.Center,
                    Normal = new Vector3(9, 9, 0),
                    Colour = color.ToVector4(),
                    Tint = Vector4.One,
                    Flags = VertexFlags.SelectiveTransformed
                }
            ).ToList();

            builder.Append(verts, Enumerable.Range(0, verts.Count).Select(x => (uint) x), new[]
            {
                new BufferGroup(PipelineType.BillboardOpaque, CameraType.Orthographic, CenterHandleTextureDataSource.Name, 0, (uint) verts.Count)
            });

            return Task.CompletedTask;
        }

        private static readonly CenterHandleTextureDataSource HandleDataSource = new CenterHandleTextureDataSource();
        private class CenterHandleTextureDataSource
        {
            public const string Name = "DefaultSolidConverter::CenterHandle::X";
            public byte[] Data { get; }
            public TextureSampleType SampleType => TextureSampleType.Point;
            public int Width => 9;
            public int Height => 9;

            public CenterHandleTextureDataSource()
            {
                using (var img = new Bitmap(Width, Height))
                {
                    using (var g = Graphics.FromImage(img))
                    {
                        g.FillRectangle(Brushes.Transparent, 0, 0, img.Width, img.Height);
                        g.DrawLine(Pens.White, 1, 1, img.Width - 2, img.Height - 2);
                        g.DrawLine(Pens.White, img.Width - 2, 1, 1, img.Height - 2);
                    }
                    var lb = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    Data = new byte[lb.Stride * lb.Height];
                    Marshal.Copy(lb.Scan0, Data, 0, Data.Length);
                    img.UnlockBits(lb);
                }
            }
        }
    }
}