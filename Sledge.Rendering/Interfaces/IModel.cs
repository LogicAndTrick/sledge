using System;
using System.Collections.Generic;
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
        /// Get a list of sequences for this model.
        /// </summary>
        /// <returns>List of sequences</returns>
        List<string> GetSequences();
    }
}