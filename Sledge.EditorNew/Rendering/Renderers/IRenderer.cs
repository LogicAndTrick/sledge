using System;
using System.Collections.Generic;
using OpenTK;
using Sledge.DataStructures.MapObjects;
using Sledge.EditorNew.Documents;
using Sledge.EditorNew.UI.Viewports;

namespace Sledge.EditorNew.Rendering.Renderers
{
    public interface IRenderer : IDisposable
    {
        string Name { get; }
        Document Document { get; set; }

        // Render interface
        void UpdateGrid(decimal gridSpacing, bool showIn2D, bool showIn3D, bool force);
        void SetSelectionTransform(Matrix4 selectionTransform);
        void Draw2D(IViewport2D context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView);
        void Draw3D(IViewport3D context, Matrix4 viewport, Matrix4 camera, Matrix4 modelView);

        // Update interface
        void Update();
        void UpdatePartial(IEnumerable<MapObject> objects);
        void UpdatePartial(IEnumerable<Face> faces);
        void UpdateSelection(IEnumerable<MapObject> objects);

        void UpdateDocumentToggles();
    }
}