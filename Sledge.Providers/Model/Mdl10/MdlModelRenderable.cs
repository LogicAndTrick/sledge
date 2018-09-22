using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sledge.Rendering.Engine;
using Sledge.Rendering.Pipelines;
using Sledge.Rendering.Renderables;
using Sledge.Rendering.Viewports;
using Veldrid;

namespace Sledge.Providers.Model.Mdl10
{
    public class MdlModelRenderable : IModelRenderable
    {
        public MdlModel Model { get; }

        private DeviceBuffer _transformsBuffer;
        private ResourceSet _transformsResourceSet;
        private Matrix4x4[] _transforms;

        private int _currentFrame;
        private long _lastFrameMillis;
        private float _interframePercent;

        public MdlModelRenderable(MdlModel model)
        {
            Model = model;

            _transforms = new Matrix4x4[128];
            for (var i = 0; i < _transforms.Length; i++)
            {
                _transforms[i] = Matrix4x4.Identity;
            }

            _currentFrame = 0;
            _interframePercent = 0;
        }

        public void Update(long milliseconds)
        {
            const int currentSequence = 0;
            if (currentSequence >= Model.Model.Sequences.Count) return;

            var seq = Model.Model.Sequences[currentSequence];
            var targetFps = 1000 / seq.Framerate;
            var diff = milliseconds - _lastFrameMillis;

            _interframePercent += diff / targetFps;
            var skip = (int)_interframePercent;
            _interframePercent -= skip;

            _currentFrame = (_currentFrame + skip) % seq.NumFrames;
            _lastFrameMillis = milliseconds;

            CalculateFrame(currentSequence, _currentFrame, _interframePercent);
        }

        private void CalculateFrame(int currentSequence, int currentFrame, float interFramePercent)
        {
            _currentFrame = currentFrame;

            var seq = Model.Model.Sequences[currentSequence];
            var blend = seq.Blends[0];
            var cFrame = blend.Frames[currentFrame % seq.NumFrames];
            var nFrame = blend.Frames[(currentFrame + 1) % seq.NumFrames];

            var indivTransforms = new Matrix4x4[128];
            for (var i = 0; i < Model.Model.Bones.Count; i++)
            {
                var bone = Model.Model.Bones[i];
                var cPos = bone.Position + cFrame.Positions[i] * bone.PositionScale;
                var nPos = bone.Position + nFrame.Positions[i] * bone.PositionScale;
                var cRot = bone.Rotation + cFrame.Rotations[i] * bone.RotationScale;
                var nRot = bone.Rotation + nFrame.Rotations[i] * bone.RotationScale;

                var cQtn = Quaternion.CreateFromYawPitchRoll(cRot.X, cRot.Y, cRot.Z);
                var nQtn = Quaternion.CreateFromYawPitchRoll(nRot.X, nRot.Y, nRot.Z);

                // MDL angles have Y as the up direction
                cQtn = new Quaternion(cQtn.Y, cQtn.X, cQtn.Z, cQtn.W);
                nQtn = new Quaternion(nQtn.Y, nQtn.X, nQtn.Z, nQtn.W);

                var mat = Matrix4x4.CreateFromQuaternion(Quaternion.Slerp(cQtn, nQtn, interFramePercent));
                mat.Translation = cPos * (1 - interFramePercent) + nPos * interFramePercent;

                indivTransforms[i] = mat;
            }

            for (var i = 0; i < Model.Model.Bones.Count; i++)
            {
                var mat = indivTransforms[i];
                var parent = Model.Model.Bones[i].Parent;
                while (parent >= 0)
                {
                    var parMat = indivTransforms[parent];
                    mat = mat * parMat;
                    parent = Model.Model.Bones[parent].Parent;
                }
                _transforms[i] = mat;
            }
        }

        public void CreateResources(EngineInterface engine, RenderContext context)
        {
            _transformsBuffer = context.Device.ResourceFactory.CreateBuffer(
                new BufferDescription((uint) Unsafe.SizeOf<Matrix4x4>() * 128, BufferUsage.UniformBuffer)
            );

            _transformsResourceSet = context.Device.ResourceFactory.CreateResourceSet(
                new ResourceSetDescription(context.ResourceLoader.ProjectionLayout, _transformsBuffer)
            );
        }

        public IEnumerable<ILocation> GetLocationObjects(IPipeline pipeline, IViewport viewport)
        {
            yield break;
        }

        public bool ShouldRender(IPipeline pipeline, IViewport viewport)
        {
            return pipeline.Type == PipelineType.WireframeModel || pipeline.Type == PipelineType.TexturedModel;
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl)
        {
            cl.UpdateBuffer(_transformsBuffer, 0, _transforms);
            cl.SetGraphicsResourceSet(2, _transformsResourceSet);
            Model.Render(context, pipeline, viewport, cl);
        }

        public void Render(RenderContext context, IPipeline pipeline, IViewport viewport, CommandList cl, ILocation locationObject)
        {
            //
        }

        public void Dispose()
        {
            _transformsResourceSet?.Dispose();
            _transformsBuffer?.Dispose();
        }
    }
}