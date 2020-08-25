using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoxelSpace {

    public class VoxelChunk : IDisposable {

        public const int SIZE = 32;

        public VoxelVolume Volume { get; private set; }
        public Coords Coords { get; private set; }
        public VoxelChunkMesh Mesh { get; private set; }

        public Array3<Voxel> Voxels { get; private set; }
        public VoxelChunkLightData LightData { get; private set; }

        public VoxelChunk(VoxelVolume volume, Coords coords) {
            Volume = volume;
            Coords = coords;
            Voxels = new Voxel[SIZE, SIZE, SIZE];
            LightData = new VoxelChunkLightData();
        }

        ~VoxelChunk() {
            Dispose();
        }

        public override string ToString() {
            return $"Chunk {Coords}";
        }

        public void SetMesh(VoxelChunkMesh mesh) {
            if (!mesh.AreBuffersReady) {
                throw new ArgumentException($"Cannot update chunk mesh for chunk at {Coords}: Mesh buffers aren't ready!");
            }
            if (Mesh != null && Mesh != mesh) {
                Mesh.Dispose();
            }
            Mesh = mesh;
        }

        public void Dispose() {
            Mesh?.Dispose();
            LightData?.Dispose();
            Mesh = null;
            LightData = null;
        }

        public static bool AreLocalCoordsInBounds(Coords c) {
            var min = 0;
            var max = SIZE - 1;
            return c.X >= min && c.X < max
                && c.Y >= min && c.Y < max
                && c.Z >= min && c.Z < max;
        }

        public Coords LocalToVolumeCoords(Coords c) 
            => Coords * SIZE + c;
        public Coords VolumeToLocalCoords(Coords c)
            => c - (Coords * SIZE);
        public Vector3 LocalToVolumeVector(Vector3 c) 
            => Coords * SIZE + c;
        public Vector3 VolumeToLocalVector(Vector3 c)
            => c - (Coords * SIZE);
        
        public Voxel GetVoxelIncludingNeighbors(Coords c) {
            if (AreLocalCoordsInBounds(c)) {
                return Voxels[c];
            }
            else {
                return Volume?.GetVoxel(LocalToVolumeCoords(c)) ?? Voxel.Empty;
            }
        }
        public Voxel GetVoxelIncludingNeighbors(int i, int j, int k)
            => GetVoxelIncludingNeighbors(new Coords(i, j, k));

        public VoxelLight GetVoxelLightIncludingNeighbors(Coords c) {
            if (AreLocalCoordsInBounds(c)) {
                return LightData.GetVoxelLight(c);
            }
            else {
                return Volume?.GetVoxelLight(LocalToVolumeCoords(c)) ?? VoxelLight.NULL;
            }
        }
        public VoxelLight GetVoxelLightIncludingNeighbors(int i, int j, int k)
            => GetVoxelLightIncludingNeighbors(new Coords(i, j, k));
        
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(Coords c, int channel) {
            if (AreLocalCoordsInBounds(c)) {
                return LightData[channel][c];
            }
            else {
                return Volume.GetVoxelLightData(LocalToVolumeCoords(c), channel);
            }
        }
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(Coords c, VoxelLightChannel channel)
            => GetVoxelLightDataIncludingNeighbors(c, (int) channel);
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(int i, int j, int k, int channel)
            => GetVoxelLightDataIncludingNeighbors(new Coords(i, j, k), channel);
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(int i, int j, int k, VoxelLightChannel channel)
            => GetVoxelLightDataIncludingNeighbors(new Coords(i, j, k), (int) channel);


        public unsafe class UnmanagedArray3<T> : IDisposable where T : unmanaged {

            T* data;

            public T* this[int x, int y, int z]
                => &data[x + y * SIZE + z * SIZE * SIZE];
            public T* this[Coords c]
                => &data[c.X + c.Y * SIZE + c.Z * SIZE * SIZE];
            public T* this[int offset]
                => &data[offset];

            public UnmanagedArray3() {
                int size = Marshal.SizeOf<T>() * SIZE * SIZE * SIZE;
                data = (T*) Marshal.AllocHGlobal(size);
                while (size > 0) {
                    ((byte*) data)[--size] = 0;
                }
            }

            ~UnmanagedArray3() {
                Dispose();
            }

            public void Dispose() {
                if (data != null) {
                    Marshal.FreeHGlobal((IntPtr) data);
                    data = null;
                }
            }

            public static int GetIndex(int x, int y, int z) => x + y * SIZE + z * SIZE * SIZE;
            public static int GetIndex(Coords c) => c.X + c.Y * SIZE + c.Z * SIZE * SIZE;

        }
    }

}