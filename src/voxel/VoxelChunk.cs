using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunk : IDisposable {

        public const int chunkSize = 32;

        public VoxelVolume volume { get; private set; }
        public Coords coords { get; private set; }
        public VoxelChunkMesh mesh { get; private set; }

        Voxel[,,] voxels;

        public Voxel this[int x, int y, int z] {
            get => voxels[x,y,z];
            set => voxels[x, y, z] = value;
        }

        public Voxel this[Coords c] {
            get => voxels[c.x, c.y, c.z];
            set => voxels[c.x, c.y, c.z] = value;
        }

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

        public Coords LocalToGlobalCoords(Coords c) {
            return coords * chunkSize + c;
        }

        public Coords GlobalToLocalCoords(Coords c) {
            return c - coords * chunkSize;
        }

    }
}