using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelType {

        public bool IsSolid { get; private set; }
        public bool IsOpaque => IsSolid;    // temporary. will change when we add transparency
        public bool IsMeshable => IsSolid;

        public IVoxelSkin Skin { get; private set; }
        public UI.UIVoxelMesh UIVoxelMesh { get; private set; }

        public VoxelType(bool isSolid, IVoxelSkin skin) {
            Skin = skin;
            IsSolid = isSolid;
        }

        public void CreateUIVoxelMesh() {
            UIVoxelMesh = new UI.UIVoxelMesh(this);
        }

    }

}