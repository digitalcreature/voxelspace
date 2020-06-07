using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelVolume : IDisposable, IEnumerable<VoxelChunk> {

        Dictionary<Coords, VoxelChunk> chunks;

        public VoxelVolume() {
            chunks = new Dictionary<Coords, VoxelChunk>();
        }

        public VoxelChunk AddChunk(Coords coords) {
            var chunk = new VoxelChunk(coords);
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

        public void UpdateAllChunkMeshes(GraphicsDevice graphics) {
            var sw = Stopwatch.StartNew();
            foreach (var chunk in chunks.Values) {
                chunk.UpdateMesh(graphics);
            }
            Console.WriteLine(string.Format("updated {0} chunk meshes in {1}s", chunks.Count, sw.ElapsedMilliseconds / 1000f));
        }

        public void Dispose() {
            foreach (var chunk in chunks.Values) {
                chunk.Dispose();
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