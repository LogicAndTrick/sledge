using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;

namespace Sledge.BspEditor.Editing.Problems
{
    public class Problem
    {
        public Type Type { get; set; }
        public MapDocument Document { get; set; }
        public List<IMapObject> Objects { get; set; }
        public List<Face> Faces { get; set; }
        public Func<Problem, IOperation> Fix { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }

        public Problem(Type type, MapDocument document, Func<Problem, IOperation> fix, string message, string description)
        {
            Type = type;
            Document = document;
            Faces = new List<Face>();
            Objects = new List<IMapObject>();
            Fix = fix;
            Message = message;
            Description = description;
        }

        public Problem(Type type, MapDocument document, IEnumerable<IMapObject> objects, Func<Problem, IOperation> fix, string message, string description)
        {
            Type = type;
            Document = document;
            Faces = new List<Face>();
            Objects = (objects ?? new IMapObject[0]).ToList();
            Fix = fix;
            Message = message;
            Description = description;
        }

        public Problem(Type type, MapDocument document, IEnumerable<Face> faces, Func<Problem, IOperation> fix, string message, string description)
        {
            Type = type;
            Document = document;
            Objects = new List<IMapObject>();
            Faces = (faces ?? new Face[0]).ToList();
            Fix = fix;
            Message = message;
            Description = description;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}