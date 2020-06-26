using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolumeMeshUpdater {

        GraphicsDevice graphics;

        public VoxelVolumeMeshUpdater(GraphicsDevice graphics) {
            this.graphics = graphics;
        }

        public void RegisterCallbacks(VoxelVolume volume) {
            volume.onModifyVoxel += OnModifyVoxel;
        }

        void OnModifyVoxel(VoxelVolume volume, VoxelChunk chunk, Coords global, Voxel voxel) {
            var local = chunk.VolumeToLocalCoords(global);
            UpdateChunkMesh(chunk);
            if (local.x == 0) {
                var neighbor = volume[chunk.coords + new Coords(-1, 0, 0)];
                if (neighbor != null) UpdateChunkMesh(neighbor);
            }
            if (local.x == VoxelChunk.chunkSize - 1) {
                var neighbor = volume[chunk.coords + new Coords(1, 0, 0)];
                if (neighbor != null) UpdateChunkMesh(neighbor);
            }
            if (local.y == 0) {
                var neighbor = volume[chunk.coords + new Coords(0, -1, 0)];
                if (neighbor != null) UpdateChunkMesh(neighbor);
            }
            if (local.y == VoxelChunk.chunkSize - 1) {
                var neighbor = volume[chunk.coords + new Coords(0, 1, 0)];
                if (neighbor != null) UpdateChunkMesh(neighbor);
            }
            if (local.z == 0) {
                var neighbor = volume[chunk.coords + new Coords(0, 0, -1)];
                if (neighbor != null) UpdateChunkMesh(neighbor);
            }
            if (local.z == VoxelChunk.chunkSize - 1) {
                var neighbor = volume[chunk.coords + new Coords(0, 0, 1)];
                if (neighbor != null) UpdateChunkMesh(neighbor);
            }
        }

        void UpdateChunkMesh(VoxelChunk chunk) {
            var generator = new VoxelChunkMeshGenerator(chunk);
            generator.Generate();
            chunk.UpdateMesh(generator.ToVoxelChunkMesh(graphics));
        }

    }

}