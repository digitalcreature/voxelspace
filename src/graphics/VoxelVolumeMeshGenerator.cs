using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading.Tasks;

namespace VoxelSpace {
    
    public class VoxelVolumeMeshGenerator : VoxelVolumeProcessor {

        GraphicsDevice _graphics;

        ConcurrentQueue<VoxelChunkMesh> _dirtyMeshes;

        public VoxelVolumeMeshGenerator(GraphicsDevice graphics) {
            _dirtyMeshes = new ConcurrentQueue<VoxelChunkMesh>();
            _graphics = graphics;
        }

        protected override Task StartTask() {
            return Task.Factory.StartNew(() => {
                var tasks = new Task[Volume.ChunkCount];
                int i = 0;
                while (Input.WaitForChunk(out var chunk)) {
                    tasks[i++] = (Task.Factory.StartNew(() => {
                        var mesh = GenerateChunkMesh(chunk);
                        _dirtyMeshes.Enqueue(mesh);
                    }));
                }
                Task.WaitAll(tasks);
            });
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
                EmitChunk(mesh.Chunk);
            }
        }

    }

}
