using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunk : IDisposable {

        public const int chunkSize = 32;
        Voxel[,,] voxels;
        public Coords coords { get; private set; }

        public VoxelChunkMesh mesh { get; private set; }

        public Voxel this[int x, int y, int z] {
            get => voxels[x,y,z];
            set => voxels[x, y, z] = value;
        }

        public VoxelChunk(Coords coords) {
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

        public Coords LocalToVolume(Coords c) {
            return coords * chunkSize + c;
        }

    }
}