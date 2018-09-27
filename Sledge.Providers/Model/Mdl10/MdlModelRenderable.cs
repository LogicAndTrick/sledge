using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.DataStructures.Geometric;
using Sledge.Rendering.Cameras;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Interfaces;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Providers.Model.Mdl10
{
    public class MdlModelRenderable : IModelRenderable
    {
        private readonly MdlModel _model;
        public IModel Model => _model;

        private DeviceBuffer _transformsBuffer;
        private ResourceSet _transformsResourceSet;
        private Matrix4x4[] _transforms;

        private DeviceBuffer _frozenTransformsBuffer;
        private ResourceSet _frozenTransformsResourceSet;

        private int _currentFrame;
        private long _lastFrameMillis;
        private float _interframePercent;

        public Vector3 Origin { get; set; }
        public Vector3 Angles { get; set; }

        private int _lastSequence = -1;
        public int Sequence { get; set; }

        public MdlModelRenderable(MdlModel model)
        {
            _model = model;

            _transforms = new Matrix4x4[128];
            for (var i = 0; i < _transforms.Length; i++)
            {
                _transforms[i] = Matrix4x4.Identity;
            }

            _currentFrame = 0;
            _interframePercent = 0;
        }

        public Matrix4x4 GetModelTransformation()
        {
            return Matrix4x4.CreateFromYawPitchRoll(Angles.X, Angles.Z, Angles.Y) * Matrix4x4.CreateTranslation(Origin);
        }

        public (Vector3, Vector3) GetBoundingBox()
        {
            var (min, max) = _model.GetBoundingBox(Sequence, 0, 0);

            var tf = GetModelTransformation();
            var box = new Box(min, max);
            box = box.Transform(tf);

            return (box.Start, box.End);
        }

        public void Update(long milliseconds)
        {
            var currentSequence = Sequence;
            if (currentSequence >= _model.Model.Sequences.Count) return;

            var seq = _model.Model.Sequences[currentSequence];
            var targetFps = 1000 / seq.Framerate;
            var diff = milliseconds - _lastFrameMillis;

            _interframePercent += diff / targetFps;
            var skip = (int)_interframePercent;
            _interframePercent -= skip;

            _currentFrame = (_currentFrame + skip) % seq.NumFrames;
            _lastFrameMillis = milliseconds;
            
            _model.Model.GetTransforms(currentSequence, _currentFrame, _interframePercent, ref _transforms);
        }

        public void CreateResources(EngineInterface engine, RenderContext context)
        {
            _transformsBuffer = context.Device.ResourceFactory.CreateBuffer(
                new BufferDescription((uint) Unsafe.SizeOf<Matrix4x4>() * 128, BufferUsage.UniformBuffer)
            );

            _transformsResourceSet = context.Device.ResourceFactory.CreateResourceSet(
                new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _transformsBuffer)
            );

            _frozenTransformsBuffer = context.Device.ResourceFactory.CreateBuffer(
                new BufferDescription((uint) Unsafe.SizeOf<Matrix4x4>() * 128, BufferUsage.UniformBuffer)
            );

            _frozenTransformsResourceSet = context.Device.ResourceFactory.CreateResourceSet(
                new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _frozenTransformsBuffer)
            );
        }

        public IEnumerable<ILocation> GetLocationObjects(IPipeline pipeline, IViewport viewport)
        {
            yield break;
        }

        public bool ShouldRender(IPipeline pipeline, IViewport viewport)
        {
            if (pipeline.Type == PipelineType.WireframeModel)
            {
                if (viewport.Camera.Type != CameraType.Orthographic) return false;
                if (viewport.Camera is OrthographicCamera oc && oc.Zoom < 0.25f) return false;
                return true;
            }
            else if (pipeline.Type == PipelineType.TexturedModel)
            {
                if (viewport.Camera.Type != CameraType.Perspective) return false;
                return true;
            }
            return false;
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            if (_transformsResourceSet == null || _transformsBuffer == null) return;

            if (pipeline.Type == PipelineType.WireframeModel)
            {
                if (_lastSequence != Sequence)
                {
                    var transforms = new Matrix4x4[128];
                    _model.Model.GetTransforms(Sequence, 0, 0, ref transforms);
                    cl.UpdateBuffer(_frozenTransformsBuffer, 0, transforms);
                    _lastSequence = Sequence;
                }
                cl.SetGraphicsResourceSet(1, _frozenTransformsResourceSet);
            }
            else
            {
                cl.UpdateBuffer(_transformsBuffer, 0, _transforms);
                cl.SetGraphicsResourceSet(2, _transformsResourceSet);
            }

            _model.Render(context, pipeline, viewport, cl);
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl, ILocation locationObject)
        {
            //
        }

        public void DestroyResources()
        {
            _transformsResourceSet?.Dispose();
            _transformsBuffer?.Dispose();
            _frozenTransformsResourceSet?.Dispose();
            _frozenTransformsBuffer?.Dispose();

            _transformsResourceSet = null;
            _transformsBuffer = null;
            _frozenTransformsResourceSet = null;
            _frozenTransformsBuffer = null;
        }

        public void Dispose()
        {
            //
        }
    }
}