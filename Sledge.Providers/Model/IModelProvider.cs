using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sledge.FileSystem;
using Sledge.Rendering.Interfaces;

namespace Sledge.Providers.Model
{
    public interface IModelProvider
    {
        bool CanLoadModel(IFile file);
        Task<IModel> LoadModel(IFile file);

        bool IsProvider(IModel model);
        IModelRenderable CreateRenderable(IModel model);
    }
}
