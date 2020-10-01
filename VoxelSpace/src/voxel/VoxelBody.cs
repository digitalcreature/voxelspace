using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.SceneGraph;

namespace VoxelSpace {

    // represents a physical body made of voxels, like a planet, ship, asteroid, etc
    public class VoxelBody : SceneObject, IDisposable {

        public VoxelVolume Volume { get; private set; }
        public VoxelVolumeChangeManager ChangeManager { get; private set; }
        public GravityField Gravity { get; private set; }

        public VoxelBody(GravityField gravity = null) : base() {
            Volume = new VoxelVolume();
            ChangeManager = new VoxelVolumeChangeManager(Volume);
            Gravity = gravity;
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
    }
    
}