using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolume : IDisposable, IEnumerable<VoxelChunk>, ICollisionGrid {

        Dictionary<Coords, VoxelChunk> chunks;
        HashSet<VoxelChunk> dirtyChunks;
        public int chunkCount => chunks.Count;

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
    }

}