using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.SceneGraph;

namespace VoxelSpace {

    // represents a physical body made of voxels, like a planet, ship, asteroid, etc
    public class VoxelBody : SceneObject, IDisposable, IO.IBinaryWritable {

        public VoxelVolume Volume { get; private set; }
        public VoxelVolumeChangeManager ChangeManager { get; private set; }

        public VoxelBody() : base() {
            Volume = new VoxelVolume();
            ChangeManager = new VoxelVolumeChangeManager(Volume);
        }

        public VoxelBody(BinaryReader reader) {
            Volume = new VoxelVolume(reader);
            ChangeManager = new VoxelVolumeChangeManager(Volume);
        }

        public void StartThreads() {
            ChangeManager.StartThread();
        }

        public void StopThreads() {
            ChangeManager.StopThread();
        }

        public void Dispose() {
            Volume?.Dispose();
            Volume = null;
            StopThreads();
        }

        public virtual void WriteBinary(BinaryWriter writer) {
            Volume.WriteBinary(writer);
        }
    }
    
}