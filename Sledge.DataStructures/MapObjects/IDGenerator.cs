using System;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class IDGenerator : ISerializable
    {
        private long _lastObjectId;
        private long _lastFaceId;

        public IDGenerator()
        {
            _lastFaceId = 0;
            _lastObjectId = 0;
        }

        protected IDGenerator(SerializationInfo info, StreamingContext context)
        {
            _lastObjectId = info.GetInt32("LastObjectID");
            _lastFaceId = info.GetInt32("LastFaceID");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LastObjectID", _lastObjectId);
            info.AddValue("LastFaceID", _lastFaceId);
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