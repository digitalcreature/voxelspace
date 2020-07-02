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

        public Coords LocalToVolumeCoords(Coords c) 
            => coords * chunkSize + c;
        public Coords VolumeToLocalCoords(Coords c)
            => c - coords * chunkSize;
        public Vector3 LocalToVolumeVector(Vector3 c) 
            => coords * chunkSize + c;
        public Vector3 VolumeToLocalVector(Vector3 c)
            => c - (coords * chunkSize);
        

    }
}