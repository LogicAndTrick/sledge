using System;
using System.Runtime.Serialization;

namespace Sledge.DataStructures.MapObjects
{
    [Serializable]
    public class Output : ISerializable
    {
        public string Name { get; set; }
        public string Target { get; set; }
        public string Input { get; set; }
        public string Parameter { get; set; }
        public decimal Delay { get; set; }
        public bool OnceOnly { get; set; }

        public Output()
        {
        }

        protected Output(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString("Name");
            Target = info.GetString("Target");
            Input = info.GetString("Input");
            Parameter = info.GetString("Parameter");
            Delay = info.GetDecimal("Delay");
            OnceOnly = info.GetBoolean("OnceOnly");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", Name);
            info.AddValue("Target", Target);
            info.AddValue("Input", Input);
            info.AddValue("Parameter", Parameter);
            info.AddValue("Delay", Delay);
            info.AddValue("OnceOnly", OnceOnly);
        }

        public Output Clone()
        {
            return new Output
                       {
                           Name = Name,
                           Target = Target,
                           Input = Input,
                           Parameter = Parameter,
                           Delay = Delay,
                           OnceOnly = OnceOnly
                       };
        }
    }
}
