using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelWorld {

        public VoxelVolume volume { get; private set; }
        public VoxelVolumeRenderer volumeRenderer { get; private set; }
        public GravityField gravity { get; private set; }

        HashSet<IEntity> entities;

        public VoxelWorld(GravityField gravity = null, VoxelVolumeRenderer volumeRenderer = null) {
            volume = new VoxelVolume();
            this.volumeRenderer = volumeRenderer;
            this.gravity = gravity;
            entities = new HashSet<IEntity>();
        }

        public void Update(GameTime time) {
            foreach (var entity in entities) {
                entity.Update(time);
            }
        }

        public void AddEntity(IEntity entity) {
            if (entity.world != null) {
                entity.world.RemoveEntity(entity);
            }
            entities.Add(entity);
            entity._SetVoxelWorld(this);
        }

        public bool RemoveEntity(IEntity entity) {
            if (entity.world == this) {
                entity._SetVoxelWorld(null);
                return entities.Remove(entity);
            }
            return false;
        }

        public void Render(GraphicsDevice graphics) {
            if (volumeRenderer != null) {
                volumeRenderer.Render(graphics, volume);
            }
            else {
                Logger.Error(this, "No renderer assigned!");
            }
        }

    }
    
}