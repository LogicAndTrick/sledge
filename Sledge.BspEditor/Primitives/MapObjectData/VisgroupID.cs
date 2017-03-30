namespace Sledge.BspEditor.Primitives.MapObjectData
{
    public class VisgroupID : IMapObjectData
    {
        public long ID { get; set; }

        public VisgroupID(long id)
        {
            ID = id;
        }

        public IMapObjectData Clone()
        {
            return new VisgroupID(ID);
        }
    }
}