using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoxelSpace {

    public class VoxelChunk : IDisposable {

        public const int SIZE = 32;

        public VoxelVolume volume { get; private set; }
        public Coords coords { get; private set; }
        public VoxelChunkMesh mesh { get; private set; }

        public Array3<Voxel> voxels { get; private set; }
        public Array3<VoxelLight> lights { get; private set; }
        public VoxelChunkLightData lightData { get; private set; }

        public VoxelChunk(VoxelVolume volume, Coords coords) {
            this.volume = volume;
            this.coords = coords;
            voxels = new Voxel[SIZE, SIZE, SIZE];
            lights = new VoxelLight[SIZE, SIZE, SIZE];
            lightData = new VoxelChunkLightData();
        }

        ~VoxelChunk() {
            Dispose();
        }

        public void UpdateMesh(VoxelChunkMesh mesh) {
            if (!mesh.AreBuffersReady) {
                throw new ArgumentException($"Cannot update chunk mesh for chunk at {coords}: Mesh buffers aren't ready!");
            }
            if (this.mesh != null && this.mesh != mesh) {
                this.mesh.Dispose();
            }
            this.mesh = mesh;
        }

        public void Dispose() {
            mesh?.Dispose();
            lightData?.Dispose();
            mesh = null;
            lightData = null;
        }

        public static bool AreLocalCoordsInBounds(Coords c) {
            var min = 0;
            var max = SIZE - 1;
            return c.X >= min && c.X < max
                && c.Y >= min && c.Y < max
                && c.Z >= min && c.Z < max;
        }

        public Coords LocalToVolumeCoords(Coords c) 
            => coords * SIZE + c;
        public Coords VolumeToLocalCoords(Coords c)
            => c - (coords * SIZE);
        public Vector3 LocalToVolumeVector(Vector3 c) 
            => coords * SIZE + c;
        public Vector3 VolumeToLocalVector(Vector3 c)
            => c - (coords * SIZE);
        
        public Voxel GetVoxelIncludingNeighbors(Coords c) {
            if (AreLocalCoordsInBounds(c)) {
                return voxels[c];
            }
            else {
                return volume?.GetVoxel(LocalToVolumeCoords(c)) ?? Voxel.Empty;
            }
        }
        public Voxel GetVoxelIncludingNeighbors(int i, int j, int k)
            => GetVoxelIncludingNeighbors(new Coords(i, j, k));

        public VoxelLight GetVoxelLightIncludingNeighbors(Coords c) {
            if (AreLocalCoordsInBounds(c)) {
                return lightData.GetVoxelLight(c);
            }
            else {
                return volume?.GetVoxelLight(LocalToVolumeCoords(c)) ?? VoxelLight.INVALID;
            }
        }
        public VoxelLight GetVoxelLightIncludingNeighbors(int i, int j, int k)
            => GetVoxelLightIncludingNeighbors(new Coords(i, j, k));
        
        public unsafe byte* GetVoxelLightDataIncludingNeighbors(Coords c, int channel) {
            if (AreLocalCoordsInBounds(c)) {
                return lightData[channel][c];
            }
            else {
                return volume.GetVoxelLightData(LocalToVolumeCoords(c), channel);
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