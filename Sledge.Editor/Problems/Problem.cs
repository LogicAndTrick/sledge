using System;
using System.Collections.Generic;
using System.Linq;
using Sledge.DataStructures.MapObjects;
using Sledge.Editor.Actions;

namespace Sledge.Editor.Problems
{
    public class Problem
    {
        public Type Type { get; set; }
        public List<MapObject> Objects { get; set; }
        public List<Face> Faces { get; set; }
        public Func<Problem, IAction> Fix { get; set; }
        public string Message { get; set; }
        public string Description { get; set; }

        public Problem(Type type, Func<Problem, IAction> fix, string message, string description)
        {
            Type = type;
            Faces = new List<Face>();
            Objects = new List<MapObject>();
            Fix = fix;
            Message = message;
            Description = description;
        }

        public Problem(Type type, IEnumerable<MapObject> objects, Func<Problem, IAction> fix, string message, string description)
        {
            Type = type;
            Faces = new List<Face>();
            Objects = (objects ?? new MapObject[0]).ToList();
            Fix = fix;
            Message = message;
            Description = description;
        }

        public Problem(Type type, IEnumerable<Face> faces, Func<Problem, IAction> fix, string message, string description)
        {
            Type = type;
            Objects = new List<MapObject>();
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