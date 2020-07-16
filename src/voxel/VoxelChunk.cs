using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;

namespace VoxelSpace {

    public class VoxelChunk : IDisposable {

        public const int chunkSize = 32;

        public VoxelVolume volume { get; private set; }
        public Coords coords { get; private set; }
        public VoxelChunkMesh mesh { get; private set; }

        public Array3<Voxel> voxels { get; private set; }
        public Array3<VoxelLight> lights { get; private set; }
        public VoxelChunkLightData lightData { get; private set; }

        public VoxelChunk(VoxelVolume volume, Coords coords) {
            this.volume = volume;
            this.coords = coords;
            voxels = new Voxel[chunkSize, chunkSize, chunkSize];
            lights = new VoxelLight[chunkSize, chunkSize, chunkSize];
            lightData = new VoxelChunkLightData();
        }

        ~VoxelChunk() {
            Dispose();
        }

        public void UpdateMesh(VoxelChunkMesh mesh) {
            if (!mesh.areBuffersReady) {
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
            var max = chunkSize - 1;
            return c.x >= min && c.x < max
                && c.y >= min && c.y < max
                && c.z >= min && c.z < max;
        }

        public Coords LocalToVolumeCoords(Coords c) 
            => coords * chunkSize + c;
        public Coords VolumeToLocalCoords(Coords c)
            => c - (coords * chunkSize);
        public Vector3 LocalToVolumeVector(Vector3 c) 
            => coords * chunkSize + c;
        public Vector3 VolumeToLocalVector(Vector3 c)
            => c - (coords * chunkSize);
        
        public Voxel GetVoxelIncludingNeighbors(Coords c) {
            if (AreLocalCoordsInBounds(c)) {
                return voxels[c];
            }
            else {
                return volume?.GetVoxel(LocalToVolumeCoords(c)) ?? Voxel.empty;
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
                => &data[x + y * chunkSize + z * chunkSize * chunkSize];
            public T* this[Coords c]
                => &data[c.x + c.y * chunkSize + c.z * chunkSize * chunkSize];
            public T* this[int offset]
                => &data[offset];

            public UnmanagedArray3() {
                int size = Marshal.SizeOf<T>() * chunkSize * chunkSize * chunkSize;
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

            public static int GetIndex(int x, int y, int z) => x + y * chunkSize + z * chunkSize * chunkSize;
            public static int GetIndex(Coords c) => c.x + c.y * chunkSize + c.z * chunkSize * chunkSize;

        }
    }

}