using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunk {

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

        public void UpdateMesh(GraphicsDevice graphics) {
            if (mesh != null) {
                mesh.Dispose();
            }
            var generator = new VoxelChunkMeshGenerator(this);
            generator.Generate();
            this.mesh = generator.ToVoxelChunkMesh(graphics);
        }


    }
}