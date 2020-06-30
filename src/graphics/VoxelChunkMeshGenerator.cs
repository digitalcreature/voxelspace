using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelChunkMeshGenerator {

        public VoxelChunk chunk { get; private set; }

        List<VoxelVertex> verts;
        List<VoxelLightVertex> lightVerts;
        List<uint> tris;

        public VoxelChunkMeshGenerator(VoxelChunk chunk) {
            this.chunk = chunk;
            verts = new List<VoxelVertex>();
            lightVerts = new List<VoxelLightVertex>();
            tris = new List<uint>();
        }

        public VoxelChunkMesh ToVoxelChunkMesh(GraphicsDevice graphics) {
            if (verts.Count == 0) {
                return null;
            }
            else {
                return new VoxelChunkMesh(graphics, verts.ToArray(), lightVerts.ToArray(), tris.ToArray());
            }
        }

        // get a voxel based on local coordinates to this chunk, while also checking
        // other chunks in the volume if this is out of bounds
        Voxel GetVoxelIncludingNeighbors(Coords c) => GetVoxelIncludingNeighbors(c.x, c.y, c.z);
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
                            var coords = new Coords(i, j, k);
                            var orientation = chunk.volume.GetVoxelOrientation(chunk.LocalToVolumeCoords(coords));
                            // -x face
                            if (!GetVoxelIncludingNeighbors(i - 1, j, k).isMeshable) {
                                AddVoxelFace(
                                    coords,
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
                                    coords,
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
                                    coords,
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
                                    coords,
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
                                    coords,
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
                                    coords,
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
            CalculateLighting();
        }

        void CalculateLighting() {
            foreach (var vert in verts) {
                var vc = vert.coords;
                var pc = (Coords) vert.position;
                Orientation normal = vert.normal.ToAxisAlignedOrientation();
                Coords neighborCoords = vc + (Coords) vert.normal;
                var t = Coords.zero;
                var bt = Coords.zero;
                switch (normal) {
                    case Orientation.Xn:
                    case Orientation.Xp:
                        t = new Coords(0, pc.y == vc.y ? -1 : 1, 0);
                        bt = new Coords(0, 0, pc.z == vc.z ? -1 : 1);
                        break;
                    case Orientation.Yn:
                    case Orientation.Yp:
                        t = new Coords(pc.x == vc.x ? -1 : 1, 0, 0);
                        bt = new Coords(0, 0, pc.z == vc.z ? -1 : 1);
                        break;
                    case Orientation.Zn:
                    case Orientation.Zp:
                        t = new Coords(pc.x == vc.x ? -1 : 1, 0, 0);
                        bt = new Coords(0, pc.y == vc.y ? -1 : 1, 0);
                        break;
                }
                // ao calculation with help from:
                // https://0fps.net/2013/07/03/ambient-occlusion-for-minecraft-like-worlds/
                bool corner = GetVoxelIncludingNeighbors(neighborCoords + t + bt).isSolid;
                bool sideA = GetVoxelIncludingNeighbors(neighborCoords + t).isSolid;
                bool sideB = GetVoxelIncludingNeighbors(neighborCoords + bt).isSolid;
                float light = 1;
                float ao = 0;
                if (sideA && sideB) {
                    ao = 1;
                }
                else {
                    if (corner) ao ++;
                    if (sideA) ao ++;
                    if (sideB) ao ++;
                    ao /= 3;
                }
                ao = 1 - ao;
                lightVerts.Add(new VoxelLightVertex(light, ao));
            }
            // go back and flip any faces we need to for ao reasons
            for (int i = 0; i < verts.Count; i += 4) {
                 var lightA = lightVerts[i  ];
                 var lightB = lightVerts[i+1];
                 var lightC = lightVerts[i+2];
                 var lightD = lightVerts[i+3];
                if (lightA.ao + lightD.ao > lightB.ao + lightC.ao) {
                    var tmp = verts[i];
                    verts[i] = verts[i+1];
                    verts[i+1] = verts[i+3];
                    verts[i+3] = verts[i+2];
                    verts[i+2] = tmp;
                    lightVerts[i  ] = lightB;
                    lightVerts[i+1] = lightD;
                    lightVerts[i+2] = lightA;
                    lightVerts[i+3] = lightC;
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
        void AddVoxelFace(
                Coords coords, Voxel voxel, Orientation orientation,
                Vector3 a, Vector3 b, Vector3 c, Vector3 d,
                Orientation n, Orientation u, Orientation r) {
            var uv = voxel.type.skin.GetFaceUVs(voxel, orientation, n, u, r);
            var normal = n.ToNormal();
            AddQuad(
                new VoxelVertex(a, normal, uv.a, coords),
                new VoxelVertex(b, normal, uv.b, coords),
                new VoxelVertex(c, normal, uv.c, coords),
                new VoxelVertex(d, normal, uv.d, coords)
            );
        }

    }

}