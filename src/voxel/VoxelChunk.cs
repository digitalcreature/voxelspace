using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public interface IVoxelChunk {

        VoxelVolume volume { get; }
        Coords coords { get; }

        ref Voxel this[int i, int j, int k] { get; }
        ref Voxel this[Coords c] { get; }

    }

    public class VoxelChunk : IVoxelChunk, IDisposable {

        public const int chunkSize = 32;

        public VoxelVolume volume { get; private set; }
        public Coords coords { get; private set; }
        public VoxelChunkMesh mesh { get; private set; }

        Voxel[,,] voxels;

        public ref Voxel this[int x, int y, int z] => ref voxels[x,y,z];

        public ref Voxel this[Coords c] => ref voxels[c.x, c.y, c.z];

        public VoxelChunk(VoxelVolume volume, Coords coords) {
            this.volume = volume;
            voxels = new Voxel[chunkSize, chunkSize, chunkSize];
            this.coords = coords;
        }

        public void UpdateMesh(VoxelChunkMesh mesh) {
            if (this.mesh != null) {
                this.mesh.Dispose();
            }
            this.mesh = mesh;
        }

        public void Dispose() {
            if (mesh != null) {
                mesh.Dispose();
            }
        }
        
    }

    public static class VoxelChunkExtentions {

        public static Coords LocalToVolumeCoords(this IVoxelChunk chunk, Coords c) 
            => chunk.coords * VoxelChunk.chunkSize + c;
        public static Coords VolumeToLocalCoords(this IVoxelChunk chunk, Coords c)
            => c - chunk.coords * VoxelChunk.chunkSize;
        public static Vector3 LocalToVolumeVector(this IVoxelChunk chunk, Vector3 c) 
            => chunk.coords * VoxelChunk.chunkSize + c;
        public static Vector3 VolumeToLocalVector(this IVoxelChunk chunk, Vector3 c)
            => c - (chunk.coords * VoxelChunk.chunkSize);
        public static Voxel GetVoxelIncludingNeighbors(this IVoxelChunk chunk, Coords c)
            => chunk.GetVoxelIncludingNeighbors(c.x, c.y, c.z);
        public static Voxel GetVoxelIncludingNeighbors(this IVoxelChunk chunk, int i, int j, int k) {
            if (i >= 0 && i < VoxelChunk.chunkSize &&
                j >= 0 && j < VoxelChunk.chunkSize &&
                k >= 0 && k < VoxelChunk.chunkSize) {
                    return chunk[i, j, k];
            }
            else {
                return chunk.volume?.GetVoxel(chunk.LocalToVolumeCoords(new Coords(i, j, k))) ?? Voxel.empty;
            }
        }
    }

    public class SingleVoxelChunk : IVoxelChunk {

        public VoxelVolume volume { get; private set; }
        public Coords coords { get; private set; }

        Voxel voxel;

        public ref Voxel this[Coords c] => ref voxel;
        public ref Voxel this[int i, int j, int k] => ref voxel;

        public SingleVoxelChunk(VoxelVolume volume, Coords coords, Voxel? voxel) {
            this.volume = volume;
            this.coords = coords;
            this.voxel = voxel ?? Voxel.empty;
        }

    }
}