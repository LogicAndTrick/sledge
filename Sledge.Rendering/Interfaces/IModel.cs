using System;
using System.Numerics;
using Sledge.Rendering.Resources;

namespace Sledge.Rendering.Interfaces
{
    /// <summary>
    /// A model must render itself given the standard properties of the model pipeline.
    /// The transforms will be set before the model is rendered.
    /// </summary>
    public interface IModel : IDisposable, IResource
    {
        /// <summary>
        /// The minimum bounds for this model
        /// </summary>
        Vector3 Mins { get; }

        /// <summary>
        /// The maximum bounds for this model
        /// </summary>
        Vector3 Maxs { get; }
    }
}