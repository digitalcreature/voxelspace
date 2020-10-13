using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelType {

        public string Identifier { get; private set; }

        public bool IsSolid { get; private set; }
        public bool IsOpaque => IsSolid;    // temporary. will change when we add transparency
        public bool IsMeshable => IsSolid;

        public IVoxelSkin Skin { get; private set; }
        public UI.VoxelIconMesh VoxelIconMesh { get; private set; }

        public VoxelType(string id, bool isSolid, IVoxelSkin skin) {
            Identifier = id;
            Skin = skin;
            IsSolid = isSolid;
        }

        public void CreateVoxelIconMesh(UI.VoxelIconMaterial material) {
            VoxelIconMesh = new UI.VoxelIconMesh(this, material);
        }

    }

}