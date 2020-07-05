using System;

namespace VoxelSpace {

    // drop in wrapper that lets us index using Coords
    public struct Array3<T> {

        T[,,] data;

        public ref T this[int i, int j, int k] => ref data[i, j, k];
        public ref T this[(int, int, int) v] => ref data[v.Item1, v.Item2, v.Item3];
        public ref T this[Coords c] => ref data[c.x, c.y, c.z];

        public Array3(int w, int h, int d) {
            data = new T[w, h, d];
        }

        public Array3(T[,,] data) {
            this.data = data;
        }

        public static implicit operator T[,,](Array3<T> a) => a.data;
        public static implicit operator Array3<T>(T[,,] data) => new Array3<T>(data);


    }


}