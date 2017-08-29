using System.Collections.Generic;
using System.Threading.Tasks;
using Sledge.BspEditor.Compile;
using Sledge.BspEditor.Documents;
using Sledge.DataStructures.GameData;
using Sledge.FileSystem;
using Sledge.Providers.Texture;

namespace Sledge.BspEditor.Environment.Goldsource
{
    public class EmptyEnvironment : IEnvironment
    {
        public string Engine => "None";
        public string ID => "Empty";
        public string Name => "Empty";
        public IFile Root => null;
        public IEnumerable<string> Directories => new string[0];

        public async Task<TextureCollection> GetTextureCollection()
        {
             return new TextureCollection(new TexturePackage[0]);
        }

        public async Task<GameData> GetGameData()
        {
            return new GameData();
        }

        public Task UpdateDocumentData(MapDocument document)
        {
            return Task.FromResult(0);
        }

        public void AddData(IEnvironmentData data)
        {

        }

        public IEnumerable<T> GetData<T>() where T : IEnvironmentData
        {
            return null;
        }

        public Task<Batch> CreateBatch(IEnumerable<BatchArgument> arguments)
        {
            return Task.FromResult<Batch>(null);
        }
    }
}
