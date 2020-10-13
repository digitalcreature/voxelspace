using System;
using System.Collections;
using System.Collections.Generic;


namespace VoxelSpace {

    public class VoxelTypeIndex : ICollection<VoxelType> {

        VoxelType[] _forward;
        Dictionary<VoxelType, ushort> _reverse;

        public int Count { get; private set; }
        public bool IsReadOnly => false;

        /// <summary>
        /// Get the voxel type represented by an index value.
        /// NOTE: Does not perform any bounds-checking for performance. Make sure you're passing valid values, or use Add() instead.
        /// </summary>
        public VoxelType this[ushort index] => _forward[index];

        public ushort this[VoxelType type] {
            get {
                if (type == null) {
                    return 0;
                }
                else {
                    if (_reverse.TryGetValue(type, out var index)) {
                        return index;
                    }
                    else {
                        throw new KeyNotFoundException($"Voxel type {type.Identifier} is not listed this Voxel Type Index!");
                    }
                }
            }
        }

        public VoxelTypeIndex() {
            _reverse = new Dictionary<VoxelType, ushort>();
            Clear();
        }

        /// <summary>
        /// Add a type to the index. No change occurs if it was already indexed.
        /// </summary>
        /// <returns>The index of the type, whether it was already indexed or not.</returns>
        public ushort Add(VoxelType type) {
            Add(type, out var index);
            return index;
        }

        /// <summary>
        /// If type isn't in the index, add it. Regardless, the index of the already indexed or newly indexed type is available.
        /// </summary>
        /// <returns>true if the type wasnt already indexed, false otherwise.</returns>
        public bool Add(VoxelType type, out ushort index) {
            if (type == null) {
                index = 0;
                return false;
            }
            else if (_reverse.TryGetValue(type, out index)) {
                return false;
            }
            else {
                if (_forward.Length == Count) {
                    var forward = new VoxelType[_forward.Length + 32];
                    _forward.CopyTo(forward, 0);
                    _forward = forward;
                }
                _forward[Count] = type;
                index = (ushort) Count;
                _reverse[type] = index;
                Count ++;
                return true;
            }
        }

        void ICollection<VoxelType>.Add(VoxelType item) => Add(item);

        public void Clear() {
            _forward = new VoxelType[32];
            _forward[0] = null;
            _reverse.Clear();
            Count = 1;
        }

        public bool Contains(VoxelType item) {
            return item == null || _reverse.ContainsKey(item);
        }

        public void CopyTo(VoxelType[] array, int arrayIndex) {
            Buffer.BlockCopy(_forward, 0, array, arrayIndex, Count);
        }

        public IEnumerator<VoxelType> GetEnumerator() 
            => new Enumerator(this);

        bool ICollection<VoxelType>.Remove(VoxelType item) 
            => throw new System.NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        struct Enumerator : IEnumerator<VoxelType> {

            VoxelTypeIndex _index;
            int _cursor;

            public Enumerator(VoxelTypeIndex index) {
                _index = index;
                _cursor = -1;
            }

            public VoxelType Current => _index._forward[_cursor];

            object IEnumerator.Current => Current;

            public void Dispose() {
                throw new System.NotImplementedException();
            }

            public bool MoveNext() {
                _cursor ++;
                return _cursor < _index.Count;
            }

            public void Reset() {
                _cursor = -1;
            }
        }
    }

}