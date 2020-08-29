using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    // represents a physical body made of voxels, like a planet, ship, asteroid, etc
    public class VoxelBody : IDisposable {

        public VoxelVolume Volume { get; private set; }
        public VoxelVolumeRenderer VolumeRenderer { get; private set; }
        public VoxelVolumeChangeManager ChangeManager { get; private set; }
        public GravityField Gravity { get; private set; }

        HashSet<IEntity> _entities;

        public VoxelBody(GravityField gravity = null, VoxelVolumeRenderer volumeRenderer = null) {
            Volume = new VoxelVolume();
            VolumeRenderer = volumeRenderer;
            ChangeManager = new VoxelVolumeChangeManager(Volume);
            Gravity = gravity;
            _entities = new HashSet<IEntity>();
        }

        public void StartThreads() {
            ChangeManager.StartThread();
        }

        public void StopThreads() {
            ChangeManager.StopThread();
        }

        public void Update() {
            foreach (var entity in _entities) {
                entity.Update();
            }
        }

        public void AddEntity(IEntity entity) {
            if (entity.VoxelBody != null) {
                entity.VoxelBody.RemoveEntity(entity);
            }
            _entities.Add(entity);
            entity._SetVoxelBody(this);
        }

        public bool RemoveEntity(IEntity entity) {
            if (entity.VoxelBody == this) {
                entity._SetVoxelBody(null);
                return _entities.Remove(entity);
            }
            return false;
        }

        public void Render() {
            ChangeManager.UpdateChunkMeshes();
            if (VolumeRenderer != null) {
                VolumeRenderer.Render(Volume);
            }
            else {
                Logger.Error(this, "No renderer assigned!");
            }
        }

        public void Dispose() {
            Volume?.Dispose();
            Volume = null;
            StopThreads();
        }
    }
    
}