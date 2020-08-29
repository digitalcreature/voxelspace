using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public interface IEntity {

        VoxelBody VoxelBody { get; }

        // only call from VoxelBody
        void _SetVoxelBody(VoxelBody world);
        
        void Update();

    }

}