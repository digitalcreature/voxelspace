using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunkMeshGenerator {

        public VoxelChunk chunk { get; private set; }

        List<VoxelVertex> verts;
        List<uint> tris;

        public VoxelChunkMeshGenerator(VoxelChunk chunk) {
            this.chunk = chunk;
            verts = new List<VoxelVertex>();
            tris = new List<uint>();
        }

        public VoxelChunkMesh ToVoxelChunkMesh(GraphicsDevice graphics) {
            if (verts.Count == 0) {
                return null;
            }
            else {
                return new VoxelChunkMesh(graphics, verts.ToArray(), tris.ToArray());
            }
        }

        // get a voxel based on local coordinates to this chunk, while also checking
        // other chunks in the volume if this is out of bounds
        Voxel GetVoxelIncludingNeighbors(int i, int j, int k) {
            if (i >= 0 && i < VoxelChunk.chunkSize &&
                j >= 0 && j < VoxelChunk.chunkSize &&
                k >= 0 && k < VoxelChunk.chunkSize) {
                    return chunk[i, j, k];
            }
            else {
                if (chunk.volume != null) {
                    var c = chunk.LocalToGlobalCoords(new Coords(i, j, k));
                    var neighbor = chunk.volume.GetChunkContainingGlobalCoords(c);
                    if (neighbor != null) {
                        return neighbor[neighbor.GlobalToLocalCoords(c)];
                    }
                    else {
                        return Voxel.empty;
                    }
                }
                else {
                    return Voxel.empty;
                }
            }
        }

        public void Generate() {
            var size = VoxelChunk.chunkSize;
            for (int i = 0; i < size; i ++) {
                for (int j = 0; j < size; j ++) {
                    for (int k = 0; k < size; k ++) {
                        var voxel = chunk[i,j,k];
                        if (voxel.isMeshable) {
                            // -x face
                            if (!GetVoxelIncludingNeighbors(i - 1, j, k).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i, j + 1, k + 1),
                                    new Vector3(i, j, k),
                                    new Vector3(i, j, k + 1),
                                    Vector3.Left
                                );
                            }
                            // +x face
                            if (!GetVoxelIncludingNeighbors(i + 1, j, k).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    new Vector3(i + 1, j + 1, k + 1),
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i + 1, j, k + 1),
                                    new Vector3(i + 1, j, k),
                                    Vector3.Right
                                );
                            }
                            // -y face
                            if (!GetVoxelIncludingNeighbors(i, j - 1, k).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    new Vector3(i + 1, j, k),
                                    new Vector3(i, j, k),
                                    new Vector3(i + 1, j, k + 1),
                                    new Vector3(i, j, k + 1),
                                    Vector3.Down
                                );
                            }
                            // +y face
                            if (!GetVoxelIncludingNeighbors(i, j + 1, k).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i, j + 1, k + 1),
                                    new Vector3(i + 1, j + 1, k + 1),
                                    Vector3.Up
                                );
                            }
                            // -z face
                            if (!GetVoxelIncludingNeighbors(i, j, k - 1).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i + 1, j, k),
                                    new Vector3(i, j, k),
                                    Vector3.Forward
                                );
                            }
                            // +z face
                            if (!GetVoxelIncludingNeighbors(i, j, k + 1).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    new Vector3(i, j + 1, k + 1),
                                    new Vector3(i + 1, j + 1, k + 1),
                                    new Vector3(i, j, k + 1),
                                    new Vector3(i + 1, j, k + 1),
                                    Vector3.Backward
                                );
                            }
                        }
                    }
                }
            }
        }

        // add a square
        // verts in order:
        // 
        //      a --- b
        //      |   / |
        //      |  /  |
        //      | /   |
        //      c --- d
        // 
        void AddVoxelFace(Voxel voxel, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal) {
            uint i = (uint) verts.Count;
            voxel.type.GetTextureCoordinates(out var uv00, out var uv01, out var uv10, out var uv11);
            verts.Add(new VoxelVertex(a, normal, uv00));
            verts.Add(new VoxelVertex(b, normal, uv01));
            verts.Add(new VoxelVertex(c, normal, uv10));
            verts.Add(new VoxelVertex(d, normal, uv11));
            // tri a b c
            tris.Add(i);
            tris.Add(i + 1);
            tris.Add(i + 2);
            // tri b d c
            tris.Add(i + 1);
            tris.Add(i + 3);
            tris.Add(i + 2);
        }

    }

}