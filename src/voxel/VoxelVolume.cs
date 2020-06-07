using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolume : IDisposable, IEnumerable<VoxelChunk> {

        Dictionary<Coords, VoxelChunk> chunks;
        public int chunkCount => chunks.Count;

        public VoxelVolume() {
            chunks = new Dictionary<Coords, VoxelChunk>();
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

        public void Dispose() {
            foreach (var chunk in chunks.Values) {
                chunk.Dispose();
            }
        }

        // return the chunk containing the voxel at a set of coords
        public VoxelChunk GetChunkForVoxelCoords(Coords c) {
            Vector3 v = c;
            v /= VoxelChunk.chunkSize;
            c = (Coords) v;
            if (chunks.ContainsKey(c)) {
                return chunks[c];
            }
            else {
                return null;
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
    }

}