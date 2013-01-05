namespace Sledge.DataStructures.MapObjects
{
    public class IDGenerator
    {
        private long _lastObjectId;
        private long _lastFaceId;

        public IDGenerator()
        {
            _lastFaceId = 0;
            _lastObjectId = 0;
        }

        public long GetNextObjectID()
        {
            _lastObjectId++;
            return _lastObjectId;
        }

        public long GetNextFaceID()
        {
            _lastFaceId++;
            return _lastFaceId;
        }

        public void Reset()
        {
            Reset(0, 0);
        }

        public void Reset(long maxObjectId, long maxFaceId)
        {
            _lastFaceId = maxFaceId;
            _lastObjectId = maxObjectId;
        }
    }
}