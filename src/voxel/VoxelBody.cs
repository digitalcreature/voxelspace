using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    // represents a physical body made of voxels, like a planet, ship, asteroid, etc
    public class VoxelBody {

        public VoxelVolume volume { get; private set; }
        public VoxelVolumeRenderer volumeRenderer { get; private set; }
        public VoxelVolumeChangeManager changeManager { get; private set; }
        public GravityField gravity { get; private set; }

        HashSet<IEntity> entities;

        public VoxelBody(GravityField gravity = null, VoxelVolumeRenderer volumeRenderer = null) {
            volume = new VoxelVolume();
            this.volumeRenderer = volumeRenderer;
            this.changeManager = new VoxelVolumeChangeManager(volume);
            this.gravity = gravity;
            entities = new HashSet<IEntity>();
        }

        public void StartThreads() {
            changeManager.StartThread();
        }

        public void StopThreads() {
            changeManager.StopThread();
        }

        public void Update(GameTime time) {
            foreach (var entity in entities) {
                entity.Update(time);
            }
        }

        public void AddEntity(IEntity entity) {
            if (entity.voxelBody != null) {
                entity.voxelBody.RemoveEntity(entity);
            }
            entities.Add(entity);
            entity._SetVoxelWorld(this);
        }

        public bool RemoveEntity(IEntity entity) {
            if (entity.voxelBody == this) {
                entity._SetVoxelWorld(null);
                return entities.Remove(entity);
            }
            return false;
        }

        public void Render(GraphicsDevice graphics) {
            changeManager.UpdateChunkMeshes(graphics);
            if (volumeRenderer != null) {
                volumeRenderer.Render(graphics, volume);
            }
            else {
                Logger.Error(this, "No renderer assigned!");
            }
        }

    }
    
}