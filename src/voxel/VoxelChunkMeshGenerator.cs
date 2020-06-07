using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunkMeshGenerator {

        VoxelChunk chunk;

        List<VoxelVertex> verts;
        List<uint> tris;

        public VoxelChunkMeshGenerator(VoxelChunk chunk) {
            this.chunk = chunk;
            verts = new List<VoxelVertex>();
            tris = new List<uint>();
        }

        public VoxelChunkMesh ToVoxelChunkMesh(GraphicsDevice graphics) {
            return new VoxelChunkMesh(graphics, verts.ToArray(), tris.ToArray());
        }

        public void Generate() {
            var size = VoxelChunk.chunkSize;
            for (int i = 0; i < size; i ++) {
                for (int j = 0; j < size; j ++) {
                    for (int k = 0; k < size; k ++) {
                        if (chunk[i,j,k].isMeshable) {
                            // -x face
                            if (i == 0 || !chunk[i - 1, j, k].isMeshable) {
                                AddVoxelFace(
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i, j + 1, k + 1),
                                    new Vector3(i, j, k),
                                    new Vector3(i, j, k + 1),
                                    Vector3.Left
                                );
                            }
                            // +x face
                            if (i == (size - 1) || !chunk[i + 1, j, k].isMeshable) {
                                AddVoxelFace(
                                    new Vector3(i + 1, j + 1, k + 1),
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i + 1, j, k + 1),
                                    new Vector3(i + 1, j, k),
                                    Vector3.Right
                                );
                            }
                            // -y face
                            if (j == 0 || !chunk[i, j - 1, k].isMeshable) {
                                AddVoxelFace(
                                    new Vector3(i + 1, j, k),
                                    new Vector3(i, j, k),
                                    new Vector3(i + 1, j, k + 1),
                                    new Vector3(i, j, k + 1),
                                    Vector3.Down
                                );
                            }
                            // +y face
                            if (j == (size - 1) || !chunk[i, j + 1, k].isMeshable) {
                                AddVoxelFace(
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i, j + 1, k + 1),
                                    new Vector3(i + 1, j + 1, k + 1),
                                    Vector3.Up
                                );
                            }
                            // -z face
                            if (k == 0 || !chunk[i, j, k - 1].isMeshable) {
                                AddVoxelFace(
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i + 1, j, k),
                                    new Vector3(i, j, k),
                                    Vector3.Forward
                                );
                            }
                            // +z face
                            if (k == (size - 1) || !chunk[i, j, k + 1].isMeshable) {
                                AddVoxelFace(
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
        void AddVoxelFace(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal) {
            uint i = (uint) verts.Count;
            verts.Add(new VoxelVertex(a, normal, new Vector2(0, 0)));
            verts.Add(new VoxelVertex(b, normal, new Vector2(1, 0)));
            verts.Add(new VoxelVertex(c, normal, new Vector2(0, 1)));
            verts.Add(new VoxelVertex(d, normal, new Vector2(1, 1)));
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