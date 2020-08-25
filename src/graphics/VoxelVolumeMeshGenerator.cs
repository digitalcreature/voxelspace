using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace VoxelSpace {
    
    public class VoxelVolumeMeshGenerator : VoxelChunkProcessor {

        GraphicsDevice _graphics;

        ConcurrentQueue<VoxelChunkMesh> _dirtyMeshes;
        ConcurrentQueue<VoxelChunk> _finishedChunks;

        AutoResetEvent _onChunkFinished;

        public VoxelVolumeMeshGenerator(GraphicsDevice graphics) {
            _graphics = graphics;
            _dirtyMeshes = new ConcurrentQueue<VoxelChunkMesh>();
            _finishedChunks = new ConcurrentQueue<VoxelChunk>();
            _onChunkFinished = new AutoResetEvent(false);
        }

        void GenerateChunkMesh(object o) {
            var chunk = (VoxelChunk) o;
            var mesh = new VoxelChunkMesh(chunk);
            mesh.GenerateGeometryAndLighting();
            _dirtyMeshes.Enqueue(mesh);
        }

        public override void Update() {
            while (_dirtyMeshes.TryDequeue(out var mesh)) {
                mesh.ApplyChanges(_graphics);
                mesh.Chunk.SetMesh(mesh);
                _finishedChunks.Enqueue(mesh.Chunk);
                _onChunkFinished.Set();
            }
        }

        protected override void Process() {
            var tasks = new List<Task>();
            foreach (var chunk in Input) {
                var task = Task.Factory.StartNew(GenerateChunkMesh, chunk);
                tasks.Add(task);
            }
            int count = 0;
            while (count < Volume.ChunkCount) {
                _onChunkFinished.WaitOne();
                while (_finishedChunks.TryDequeue(out var chunk)) {
                    Produce(chunk);
                    count ++;
                }
            }
            Task.WaitAll(tasks.ToArray());
        }
    }

}
