using System;
using System.Runtime.InteropServices;

namespace VoxelSpace {

    // wrapper that lets us index with Coords
    public struct Array3<T> where T : struct {

        public readonly T[,,] Data;

        public ref T this[int i, int j, int k] => ref Data[i, j, k];
        public ref T this[(int, int, int) v] => ref Data[v.Item1, v.Item2, v.Item3];
        public ref T this[Coords c] => ref Data[c.X, c.Y, c.Z];

        public Array3(int w, int h, int d) {
            Data = new T[w, h, d];
        }

        public Array3(T[,,] data) {
            Data = data;
        }

        public static implicit operator T[,,](Array3<T> a) => a.Data;
        public static implicit operator Array3<T>(T[,,] data) => new Array3<T>(data);

    }

}