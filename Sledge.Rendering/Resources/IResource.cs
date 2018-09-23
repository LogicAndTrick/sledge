using System;
using Sledge.Rendering.Engine;

namespace Sledge.Rendering.Resources
{
    public interface IResource : IDisposable
    {
        /// <summary>
        /// Create resources needed by this resource class.
        /// Don't call this method directly, instead use <see cref="EngineInterface.CreateResource"/>.
        /// </summary>
        /// <param name="engine">The engine interface</param>
        /// <param name="context">The render context</param>
        void CreateResources(EngineInterface engine, RenderContext context);
        
        /// <summary>
        /// Destroy resources that were created by this resource class.
        /// Don't call this method directly, instead use <see cref="EngineInterface.DestroyResource"/>.
        /// </summary>
        void DestroyResources();
    }
}