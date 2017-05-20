using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Grid;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.BspEditor.Rendering.Viewport;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Shell.Hooks;

namespace Sledge.BspEditor.Tools
{
    [Export(typeof(IInitialiseHook))]
    public class ToolInitialiser : IInitialiseHook
    {
        [Import] private SquareGridFactory _squareGridFactory;

        public async Task OnInitialise()
        {
            Oy.Subscribe<MapViewport>("MapViewport:Created", MapViewportCreated);
            Oy.Subscribe<IDocument>("Document:Opened", DocumentOpened);
        }

        private async Task MapViewportCreated(MapViewport viewport)
        {
            var itl = new ToolViewportListener(viewport);
            viewport.Listeners.Add(itl);
        }

        private async Task DocumentOpened(IDocument doc)
        {
            var md = doc as MapDocument;
            if (md == null) return;

            MapDocumentOperation.Perform(md, new TrivialOperation(
                async d =>
                {
                    if (!d.Map.Data.Any(x => x is GridData))
                    {
                        var grid = await _squareGridFactory.Create(md);
                        d.Map.Data.Add(new GridData(grid));
                    }
                    if (!d.Map.Data.Any(x => x is ActiveTexture))
                    {
                        var tc = await d.Environment.GetTextureCollection();
                        var first = tc.GetAllTextures()
                            .OrderBy(t => t, StringComparer.CurrentCultureIgnoreCase)
                            .Where(item => item.Length > 0)
                            .Select(item => new {item, c = Char.ToLower(item[0])})
                            .Where(t => t.c >= 'a' && t.c <= 'z')
                            .Select(t => t.item)
                            .FirstOrDefault();
                        d.Map.Data.Add(new ActiveTexture {Name = first});
                    }

                    var gd = await d.Environment.GetGameData();
                    d.Map.Root.Data.Replace(new PointEntityGameDataBoundingBoxProvider(gd));
                    d.Map.Root.Invalidate();
                },
                x => x.Update(x.Document.Map.Data))
            );
        }
    }
}
