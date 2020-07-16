using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public interface IEntity {

        VoxelBody voxelBody { get; }

        // only call from VoxelWorld
        void _SetVoxelWorld(VoxelBody world);
        
        void Update(GameTime time);

    }

}