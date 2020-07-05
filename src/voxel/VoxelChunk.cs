using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunk : IDisposable {

        public const int chunkSize = 32;

        public VoxelVolume volume { get; private set; }
        public Coords coords { get; private set; }
        public VoxelChunkMesh mesh { get; private set; }

        public Array3<Voxel> voxels { get; private set; }
        public Array3<VoxelLight> lights { get; private set; }

        public VoxelChunk(VoxelVolume volume, Coords coords) {
            this.volume = volume;
            this.coords = coords;
            voxels = new Voxel[chunkSize, chunkSize, chunkSize];
            lights = new VoxelLight[chunkSize, chunkSize, chunkSize];
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
                return lights[c];
            }
            else {
                return volume?.GetVoxelLight(LocalToVolumeCoords(c)) ?? VoxelLight.NODATA;
            }
        }
        public VoxelLight GetVoxelLightIncludingNeighbors(int i, int j, int k)
            => GetVoxelLightIncludingNeighbors(new Coords(i, j, k));
    }

}