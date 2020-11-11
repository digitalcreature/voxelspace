using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelType {

        public string Identifier { get; private set; }

        public bool IsSolid { get; private set; }
        public bool IsOpaque { get; private set; }    // temporary. will change when we add transparency
        public bool IsMeshable => IsSolid;

        public byte? PointLightLevel { get; private set; }

        public IVoxelSkin Skin { get; private set; }
        public UI.VoxelIconMesh VoxelIconMesh { get; private set; }

        public VoxelType(string id, bool isSolid, bool isOpaque, IVoxelSkin skin, byte? pointLightLevel = null) {
            Identifier = id;
            Skin = skin;
            IsSolid = isSolid;
            IsOpaque = isOpaque;
            PointLightLevel = pointLightLevel;
        }

        public void CreateVoxelIconMesh(UI.VoxelIconMaterial material) {
            VoxelIconMesh = new UI.VoxelIconMesh(this, material);
        }

    }

}