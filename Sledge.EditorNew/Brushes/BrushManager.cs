using System.Collections.Generic;
using Sledge.EditorNew.Brushes.Controls;

namespace Sledge.EditorNew.Brushes
{
    public static class BrushManager
    {
        public delegate void BrushAddedEventHandler(IBrush brush);

        public static event BrushAddedEventHandler BrushAdded;

        private static void OnBrushAdded(IBrush brush)
        {
            if (BrushAdded != null)
            {
                BrushAdded(brush);
            }
        }

        public static List<IBrush> Brushes { get; private set; }
        
        static BrushManager()
        {
            Brushes = new List<IBrush>();
        }

        public static void Register(IBrush brush)
        {
            Brushes.Add(brush);
            OnBrushAdded(brush);
        }
    }
}