using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;

namespace VoxelSpace {

    public partial class VoxelChunkMesh : Mesh {

        List<VoxelVertex> _verts;
        List<uint> _tris;
        VoxelLightVertex[] _lights;

        public void GenerateGeometryAndLighting() {
            var size = VoxelChunk.SIZE;
            _verts = new List<VoxelVertex>();
            _tris = new List<uint>();
            for (int i = 0; i < size; i ++) {
                for (int j = 0; j < size; j ++) {
                    for (int k = 0; k < size; k ++) {
                        ref readonly var voxel = ref Chunk.Voxels[i,j,k];
                        if (voxel.IsMeshable) {
                            var coords = new Coords(i, j, k);
                            var orientation = Chunk.Volume.GetVoxelOrientation(Chunk.LocalToVolumeCoords(coords));
                            // -x face
                            if (!Chunk.GetVoxelIncludingNeighbors(i - 1, j, k).IsMeshable) {
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
                            if (!Chunk.GetVoxelIncludingNeighbors(i + 1, j, k).IsMeshable) {
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
                            if (!Chunk.GetVoxelIncludingNeighbors(i, j - 1, k).IsMeshable) {
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
                            if (!Chunk.GetVoxelIncludingNeighbors(i, j + 1, k).IsMeshable) {
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
                            if (!Chunk.GetVoxelIncludingNeighbors(i, j, k - 1).IsMeshable) {
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
                            if (!Chunk.GetVoxelIncludingNeighbors(i, j, k + 1).IsMeshable) {
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
            GenerateLighting();
        }

        public unsafe void GenerateLighting() {
            var lights = new VoxelLightVertex[_verts.Count];
            for (int v = 0; v < _verts.Count; v ++) {
                var vert = _verts[v];
                var vc = vert.Coords;
                var pc = (Coords) vert.Position;
                Orientation normal = vert.Normal.ToAxisAlignedOrientation();
                Coords n = vc + (Coords) vert.Normal;
                var t = Coords.ZERO;
                var bt = Coords.ZERO;
                switch (normal) {
                    case Orientation.Xn:
                    case Orientation.Xp:
                        t = new Coords(0, pc.Y == vc.Y ? -1 : 1, 0);
                        bt = new Coords(0, 0, pc.Z == vc.Z ? -1 : 1);
                        break;
                    case Orientation.Yn:
                    case Orientation.Yp:
                        t = new Coords(pc.X == vc.X ? -1 : 1, 0, 0);
                        bt = new Coords(0, 0, pc.Z == vc.Z ? -1 : 1);
                        break;
                    case Orientation.Zn:
                    case Orientation.Zp:
                        t = new Coords(pc.X == vc.X ? -1 : 1, 0, 0);
                        bt = new Coords(0, pc.Y == vc.Y ? -1 : 1, 0);
                        break;
                }
                // ao calculation with help from:
                // https://0fps.net/2013/07/03/ambient-occlusion-for-minecraft-like-worlds/
                Voxel top = Chunk.GetVoxelIncludingNeighbors(n);
                Voxel corner = Chunk.GetVoxelIncludingNeighbors(n + t + bt);
                Voxel sideA = Chunk.GetVoxelIncludingNeighbors(n + t);
                Voxel sideB = Chunk.GetVoxelIncludingNeighbors(n + bt);
                VoxelLight topLight = Chunk.GetVoxelLightIncludingNeighbors(n);
                VoxelLight cornerLight = Chunk.GetVoxelLightIncludingNeighbors(n + t + bt);
                VoxelLight sideALight = Chunk.GetVoxelLightIncludingNeighbors(n + t);
                VoxelLight sideBLight = Chunk.GetVoxelLightIncludingNeighbors(n + bt);
                var light = VoxelLightVertex.zero;
                float count = 0;
                if (!top.IsOpaque && topLight.IsNonNull) {
                    light.AddLight(new VoxelLightVertex(topLight));
                    count ++;
                }
                float ao = 0;
                if (sideA.IsOpaque && sideB.IsOpaque) {
                    ao = 1;
                }
                else {
                    if (corner.IsOpaque) ao ++;
                    else {
                        if (cornerLight.IsNonNull) {
                            light.AddLight(new VoxelLightVertex(cornerLight));
                            count ++;
                        }
                    }
                    if (sideA.IsOpaque) ao ++;
                    else {
                        if (sideALight.IsNonNull) {
                            light.AddLight(new VoxelLightVertex(sideALight));
                            count ++;
                        }
                    }
                    if (sideB.IsOpaque) ao ++;
                    else {
                        if (sideBLight.IsNonNull) {
                            light.AddLight(new VoxelLightVertex(sideBLight));
                            count ++;
                        }
                    }
                    ao /= 3;
                }
                light.DivideLight(count);
                light.AO = 1 - ao;
                lights[v] = light;
            }
            // go back and flip any faces we need to for ao reasons
            for (int i = 0; i < _verts.Count; i += 4) {
                 var aoA = lights[i  ].AO;
                 var aoB = lights[i+1].AO;
                 var aoC = lights[i+2].AO;
                 var aoD = lights[i+3].AO;
                if (aoA + aoD > aoB + aoC) {
                    var tmp = _verts[i];
                    _verts[i] = _verts[i+1];
                    _verts[i+1] = _verts[i+3];
                    _verts[i+3] = _verts[i+2];
                    _verts[i+2] = tmp;
                    var tmp2 = lights[i];
                    lights[i] = lights[i+1];
                    lights[i+1] = lights[i+3];
                    lights[i+3] = lights[i+2];
                    lights[i+2] = tmp2;
                }
            }
            _lights = lights;
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
        void AddQuad(VoxelVertex a, VoxelVertex b, VoxelVertex c, VoxelVertex d) {
            uint i = (uint) _verts.Count;
            _verts.Add(a);
            _verts.Add(b);
            _verts.Add(c);
            _verts.Add(d);
            // tri a b c
            _tris.Add(i);
            _tris.Add(i + 1);
            _tris.Add(i + 2);
            // tri b d c
            _tris.Add(i + 1);
            _tris.Add(i + 3);
            _tris.Add(i + 2);
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
            var uv = voxel.Type.Skin.GetFaceUVs(voxel, orientation, n, u, r);
            var normal = n.ToNormal();
            AddQuad(
                new VoxelVertex(a, normal, uv.A, coords),
                new VoxelVertex(b, normal, uv.B, coords),
                new VoxelVertex(c, normal, uv.C, coords),
                new VoxelVertex(d, normal, uv.D, coords)
            );
        }

    }

}