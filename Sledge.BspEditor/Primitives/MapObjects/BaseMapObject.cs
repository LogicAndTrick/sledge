using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.Common.Transport;
using Sledge.DataStructures.Geometric;

namespace Sledge.BspEditor.Primitives.MapObjects
{
    /// <summary>
    /// Nice and simple base class for all map objects. Strongly recommended but
    /// not specifically required to use this class as a base.
    /// </summary>
    public abstract class BaseMapObject : IMapObject
    {
        /// <inheritdoc />
        public long ID { get; }

        /// <inheritdoc />
        public virtual bool IsSelected { get; set; }

        /// <inheritdoc />
        public Box BoundingBox { get; private set; }

        /// <inheritdoc />
        public MapObjectDataCollection Data { get; private set; }

        /// <inheritdoc />
        public MapObjectHierarchy Hierarchy { get; private set; }

        protected BaseMapObject(long id)
        {
            ID = id;
            Data = new MapObjectDataCollection();
            Hierarchy = new MapObjectHierarchy(this);
            BoundingBox = Box.Empty;
        }

        protected BaseMapObject(SerialisedObject obj)
        {
            if (SerialisedName != obj.Name) throw new Exception($"Tried to deserialise a {obj.Name} into a {SerialisedName}.");
            ID = obj.Get<long>("ID");
            IsSelected = obj.Get<bool>("IsSelected");
            Data = new MapObjectDataCollection();
            Hierarchy = new MapObjectHierarchy(this);
            BoundingBox = Box.Empty;
            SetCustomSerialisedData(obj);
        }

        /// <inheritdoc />
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);
            info.AddValue("Data", Data);
            info.AddValue("Children", Hierarchy.ToArray());
        }

        /// <inheritdoc />
        public void DescendantsChanged()
        {
            OnDescendantsChanged();
            BoundingBox = GetBoundingBox();
            Hierarchy.Parent?.DescendantsChanged();
        }

        /// <inheritdoc />
        public void Invalidate()
        {
            foreach (var ch in Hierarchy)
            {
                ch.Invalidate();
            }
            BoundingBox = GetBoundingBox();
            OnInvalidated();
        }

        /// <summary>
        /// Fires when descendants changed is called, before updating the bounding box or bubbling to the parent.
        /// </summary>
        protected virtual void OnDescendantsChanged()
        {
            //
        }

        /// <summary>
        /// Fires when invalidate is called, after updating the bounding box and bubbling to all children.
        /// </summary>
        protected virtual void OnInvalidated()
        {
            //
        }

        protected abstract Box GetBoundingBox();

        protected void CloneBase(BaseMapObject copy)
        {
            copy.IsSelected = IsSelected;
            copy.Data = Data.Clone();
            foreach (var child in Hierarchy)
            {
                var c = (IMapObject) child.Clone();
                c.Hierarchy.Parent = copy;
            }
            copy.DescendantsChanged();
        }

        protected void CopyBase(BaseMapObject copy, UniqueNumberGenerator numberGenerator)
        {
            copy.IsSelected = IsSelected;
            copy.Data = Data.Copy(numberGenerator);
            foreach (var child in Hierarchy)
            {
                var c = (IMapObject)child.Copy(numberGenerator);
                c.Hierarchy.Parent = copy;
            }
            copy.DescendantsChanged();
        }

        protected void UncloneBase(BaseMapObject source)
        {
            IsSelected = source.IsSelected;
            Data = source.Data.Clone();
            Hierarchy.Clear();
            foreach (var obj in source.Hierarchy)
            {
                var copy = (IMapObject) obj.Clone();
                copy.Hierarchy.Parent = this;
            }
            DescendantsChanged();
        }

        /// <inheritdoc />
        public abstract IEnumerable<Polygon> GetPolygons();

        /// <inheritdoc />
        public abstract IEnumerable<IMapObject> Decompose(IEnumerable<Type> allowedTypes);

        /// <inheritdoc />
        public virtual IMapElement Clone()
        {
            var inst = (BaseMapObject) GetType().GetConstructor(new[] {typeof(long)}).Invoke(new object[] {ID});
            CloneBase(inst);
            return inst;
        }

        /// <inheritdoc />
        public virtual void Unclone(IMapObject obj)
        {
            if (obj.GetType() != GetType()) throw new ArgumentException("Cannot unclone into a different type.", nameof(obj));
            UncloneBase((BaseMapObject) obj);
        }

        /// <inheritdoc />
        public virtual IMapElement Copy(UniqueNumberGenerator numberGenerator)
        {
            var inst = (BaseMapObject)GetType().GetConstructor(new[] { typeof(long) }).Invoke(new object[] { numberGenerator.Next("MapObject") });
            CopyBase(inst, numberGenerator);
            return inst;
        }

        /// <inheritdoc />
        public virtual void Transform(Matrix4x4 matrix)
        {
            foreach (var t in Data.OfType<ITransformable>())
            {
                t.Transform(matrix);
            }
            foreach (var t in Hierarchy)
            {
                t.Transform(matrix);
            }
            DescendantsChanged();
        }

        protected abstract string SerialisedName { get; }

        public virtual SerialisedObject ToSerialisedObject()
        {
            var obj = new SerialisedObject(SerialisedName);
            obj.Set("ID", ID);
            obj.Set("IsSelected", IsSelected);
            obj.Set("ParentID", Hierarchy.Parent?.ID);
            AddCustomSerialisedData(obj);
            foreach (var data in Data)
            {
                obj.Children.Add(data.ToSerialisedObject());
            }
            foreach (var child in Hierarchy)
            {
                obj.Children.Add(child.ToSerialisedObject());
            }
            return obj;
        }

        protected virtual void AddCustomSerialisedData(SerialisedObject obj)
        {

        }

        protected virtual void SetCustomSerialisedData(SerialisedObject obj)
        {

        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IMapObject)obj);
        }

        /// <inheritdoc />
        public bool Equals(IMapObject other)
        {
            return other != null && other.ID == ID;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}