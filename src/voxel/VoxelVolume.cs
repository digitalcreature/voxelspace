using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public delegate void ModifyVoxelCallback(VoxelVolume volume, VoxelChunk chunk, Coords global, Voxel voxel);

    public class VoxelVolume : IDisposable, IEnumerable<VoxelChunk>, ICollisionGrid {

        Dictionary<Coords, VoxelChunk> chunks;
        HashSet<VoxelChunk> dirtyChunks;
        public int chunkCount => chunks.Count;

        public event ModifyVoxelCallback onModifyVoxel;

        public VoxelChunk this[Coords c]
            => chunks.ContainsKey(c) ? chunks[c] : null;

        public VoxelVolume() {
            chunks = new Dictionary<Coords, VoxelChunk>();
            dirtyChunks = new HashSet<VoxelChunk>();
        }

        public VoxelChunk AddChunk(Coords coords) {
            var chunk = new VoxelChunk(this, coords);
            chunks.Add(coords, chunk);
            return chunk;
        }

        public bool RemoveChunk(Coords coords) {
            if (chunks.ContainsKey(coords)) {
                var chunk = chunks[coords];
                chunk.Dispose();
                chunks.Remove(coords);
                return true;
            }
            else {
                return false;
            }
        }

        public void SetChunkDirty(VoxelChunk chunk) {
            if (chunk.volume == this) {
                dirtyChunks.Add(chunk);
            }
        }

        public void SetChunkClean(VoxelChunk chunk) {
            if (chunk.volume == this) {
                dirtyChunks.Remove(chunk);
            }
        }

        public void Dispose() {
            foreach (var chunk in chunks.Values) {
                chunk.Dispose();
            }
        }

        // get the coords of the chunk containing a set of global coordsinates
        public Coords GlobalToChunkCoords(Coords c) {
            Vector3 v = c;
            v /= VoxelChunk.chunkSize;
            return (Coords) v;
        }

        // get the voxel at a specific set of global coords
        public Voxel GetVoxel(Coords c) {
            var chunk = GetChunkContainingGlobalCoords(c);
            if (chunk != null) {
                return chunk[chunk.GlobalToLocalCoords(c)];
            }
            else {
                return Voxel.empty;
            }
        }

        // set the voxel at a specific set of coords
        // calls the onModifyVoxel callback
        public void SetVoxel(Coords c, Voxel v) {
            var chunk = GetChunkContainingGlobalCoords(c);
            if (chunk == null) {
                chunk = AddChunk(GlobalToChunkCoords(c));
            }
            var localCoords = chunk.GlobalToLocalCoords(c);
            chunk[localCoords] = v;
            if (onModifyVoxel != null) {
                onModifyVoxel(this, chunk, c, v);
            }
        }

        // return the chunk containing the voxel at a set of coords
        public VoxelChunk GetChunkContainingGlobalCoords(Coords c) {
            c = GlobalToChunkCoords(c);
            if (chunks.ContainsKey(c)) {
                return chunks[c];
            }
            else {
                return null;
            }
        }

        public IEnumerable<VoxelChunk> GetDirtyChunks() {
            foreach (var chunk in dirtyChunks) {
                yield return chunk;
            }
        }

        public IEnumerator<VoxelChunk> GetEnumerator() {
            foreach (var chunk in chunks.Values) {
                yield return chunk;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public bool CellIsSolid(Coords c) {
            return GetVoxel(c).isSolid;
        }

        public bool Raycast(Vector3 origin, Vector3 dir, float range, Predicate<Voxel> pred, out VoxelRaycastResult result) {            
            dir.Normalize();
            Coords current = (Coords) origin;
            var voxel = GetVoxel(current);
            if (pred(voxel)) {
                result = new VoxelRaycastResult() {
                    volume = this,
                    chunk = GetChunkContainingGlobalCoords(current),
                    coords = current,
                    normal = Vector3.Zero,
                    voxel = voxel
                };
                return true;
            }
            Coords step = (Coords) dir.Sign();
            var tDelta = new Vector3(
                MathF.Sqrt(1 + (dir.Y * dir.Y + dir.Z * dir.Z) / (dir.X * dir.X)),
                MathF.Sqrt(1 + (dir.X * dir.X + dir.Z * dir.Z) / (dir.Y * dir.Y)),
                MathF.Sqrt(1 + (dir.X * dir.X + dir.Y * dir.Y) / (dir.Z * dir.Z))
            );
            var tMax = new Vector3(
                (step.x > 0 ? (current.x + 1 - origin.X) : origin.X - current.x) * tDelta.X,
                (step.y > 0 ? (current.y + 1 - origin.Y) : origin.Y - current.y) * tDelta.Y,
                (step.z > 0 ? (current.z + 1 - origin.Z) : origin.Z - current.z) * tDelta.Z
            );
            if (dir.X == 0) tMax.X = float.PositiveInfinity;
            if (dir.Y == 0) tMax.Y = float.PositiveInfinity;
            if (dir.Z == 0) tMax.Z = float.PositiveInfinity;
            float distance = 0;
            while (distance < range) {
                Vector3 normal = Vector3.Zero;
                var min = tMax.Min();
                if (min == tMax.X) {
                    current.x += step.x;
                    tMax.X += tDelta.X;
                    distance = (current.x - origin.X + (1 - step.x) / 2f) / dir.X;
                    normal.X = -step.x;
                }
                else if (min == tMax.Y) {
                    current.y += step.y;
                    tMax.Y += tDelta.Y;
                    distance = (current.y - origin.Y + (1 - step.y) / 2f) / dir.Y;
                    normal.Y = -step.y;
                }
                else if (min == tMax.Z) {
                    current.z += step.z;
                    tMax.Z += tDelta.Z;
                    distance = (current.z - origin.Z + (1 - step.z) / 2f) / dir.Z;
                    normal = Vector3.Backward;
                    normal.Z = -step.z;
                }
                voxel = GetVoxel(current);
                if (distance < range && pred(voxel)) {
                    result = new VoxelRaycastResult() {
                        voxel = voxel,
                        coords = current,
                        volume = this,
                        normal = normal,
                        chunk = GetChunkContainingGlobalCoords(current)
                    };
                    return true;
                }
            }
            result = new VoxelRaycastResult();
            return false;
        }
    }

}