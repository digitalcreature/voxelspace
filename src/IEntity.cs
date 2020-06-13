using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public interface IEntity {

        VoxelWorld world { get; }

        // only call from VoxelWorld
        void _SetVoxelWorld(VoxelWorld world);
        
        void Update(GameTime time);

    }

}