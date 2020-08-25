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

        VoxelChunkMesh GenerateChunkMesh(VoxelChunk chunk) {
            var mesh = new VoxelChunkMesh(chunk);
            mesh.GenerateGeometryAndLighting();
            return mesh;
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
            var tasks = new Task[Volume.ChunkCount];
            int i = 0;
            foreach (var chunk in Input) {
                tasks[i++] = (Task.Factory.StartNew(() => {
                    var mesh = GenerateChunkMesh(chunk);
                    _dirtyMeshes.Enqueue(mesh);
                }));
            }
            int count = 0;
            while (count < Volume.ChunkCount) {
                _onChunkFinished.WaitOne();
                while (_finishedChunks.TryDequeue(out var chunk)) {
                    Produce(chunk);
                    count ++;
                }
            }
            Task.WaitAll(tasks);
        }
    }

}
