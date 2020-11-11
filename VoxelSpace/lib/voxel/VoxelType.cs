using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelType {

        public string Identifier { get; private set; }

        public bool IsSolid { get; private set; }
        public bool IsOpaque { get; private set; }    // temporary. will change when we add transparency

        public VoxelFaceMode FaceMode { get; private set; }

        public byte? PointLightLevel { get; private set; }

        public IVoxelSkin Skin { get; private set; }
        public UI.VoxelIconMesh VoxelIconMesh { get; private set; }

        public VoxelType(string id, bool isSolid, bool isOpaque, IVoxelSkin skin, VoxelFaceMode faceMode, byte? pointLightLevel = null) {
            Identifier = id;
            Skin = skin;
            IsSolid = isSolid;
            IsOpaque = isOpaque;
            FaceMode = faceMode;
            PointLightLevel = pointLightLevel;
        }

        public void CreateVoxelIconMesh(UI.VoxelIconMaterial material) {
            VoxelIconMesh = new UI.VoxelIconMesh(this, material);
        }

        public bool CanCreateFace(VoxelType neighbor) {
            if (neighbor == null) {
                return true;
            }
            else {
                switch (FaceMode) {
                    case VoxelFaceMode.Transparent:
                        return neighbor.FaceMode != VoxelFaceMode.Opaque && neighbor != this;
                    case VoxelFaceMode.Opaque:
                    case VoxelFaceMode.TransparentInner:
                        return neighbor.FaceMode != VoxelFaceMode.Opaque;
                    default:
                        return false;
                }
            }
        }


    }

    public enum VoxelFaceMode {
        Opaque,
        Transparent,
        TransparentInner
    }
}