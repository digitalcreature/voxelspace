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
                    var c = chunk.LocalToVolumeCoords(new Coords(i, j, k));
                    var neighbor = chunk.volume.GetChunkContainingGlobalCoords(c);
                    if (neighbor != null) {
                        return neighbor[neighbor.VolumeToLocalCoords(c)];
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
                            var c = new Coords(i, j, k);
                            var orientation = chunk.volume.GetVoxelOrientation(chunk.LocalToVolumeCoords(c));
                            // -x face
                            if (!GetVoxelIncludingNeighbors(i - 1, j, k).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    orientation,
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i, j + 1, k + 1),
                                    new Vector3(i, j, k),
                                    new Vector3(i, j, k + 1),
                                    Orientation.Xn,
                                    Orientation.Yp,
                                    Orientation.Zp
                                );
                            }
                            // +x face
                            if (!GetVoxelIncludingNeighbors(i + 1, j, k).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    orientation,
                                    new Vector3(i + 1, j + 1, k + 1),
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i + 1, j, k + 1),
                                    new Vector3(i + 1, j, k),
                                    Orientation.Xp,
                                    Orientation.Yp,
                                    Orientation.Zn
                                );
                            }
                            // -y face
                            if (!GetVoxelIncludingNeighbors(i, j - 1, k).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    orientation,
                                    new Vector3(i + 1, j, k),
                                    new Vector3(i, j, k),
                                    new Vector3(i + 1, j, k + 1),
                                    new Vector3(i, j, k + 1),
                                    Orientation.Yn,
                                    Orientation.Zn,
                                    Orientation.Xn
                                );
                            }
                            // +y face
                            if (!GetVoxelIncludingNeighbors(i, j + 1, k).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    orientation,
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i, j + 1, k + 1),
                                    new Vector3(i + 1, j + 1, k + 1),
                                    Orientation.Yp,
                                    Orientation.Zn,
                                    Orientation.Xp
                                );
                            }
                            // -z face
                            if (!GetVoxelIncludingNeighbors(i, j, k - 1).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    orientation,
                                    new Vector3(i + 1, j + 1, k),
                                    new Vector3(i, j + 1, k),
                                    new Vector3(i + 1, j, k),
                                    new Vector3(i, j, k),
                                    Orientation.Zn,
                                    Orientation.Yp,
                                    Orientation.Xn
                                );
                            }
                            // +z face
                            if (!GetVoxelIncludingNeighbors(i, j, k + 1).isMeshable) {
                                AddVoxelFace(
                                    voxel,
                                    orientation,
                                    new Vector3(i, j + 1, k + 1),
                                    new Vector3(i + 1, j + 1, k + 1),
                                    new Vector3(i, j, k + 1),
                                    new Vector3(i + 1, j, k + 1),
                                    Orientation.Zp,
                                    Orientation.Yp,
                                    Orientation.Xp
                                );
                            }
                        }
                    }
                }
            }
        }

        // add a quad
        // verts in order:
        // 
        //      a --- b
        //      |   / |
        //      |  /  |
        //      | /   |
        //      c --- d
        // 
        public void AddQuad(VoxelVertex a, VoxelVertex b, VoxelVertex c, VoxelVertex d) {
            uint i = (uint) verts.Count;
            verts.Add(a);
            verts.Add(b);
            verts.Add(c);
            verts.Add(d);
            // tri a b c
            tris.Add(i);
            tris.Add(i + 1);
            tris.Add(i + 2);
            // tri b d c
            tris.Add(i + 1);
            tris.Add(i + 3);
            tris.Add(i + 2);
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
        void AddVoxelFace(Voxel voxel, Orientation orientation, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Orientation n, Orientation u, Orientation r) {
            var uv = voxel.type.skin.GetFaceUVs(voxel, orientation, n, u, r);
            var normal = n.ToNormal();
            AddQuad(
                new VoxelVertex(a, normal, uv.a),
                new VoxelVertex(b, normal, uv.b),
                new VoxelVertex(c, normal, uv.c),
                new VoxelVertex(d, normal, uv.d)
            );
        }

    }

}