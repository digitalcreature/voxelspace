using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public class VoxelType {

        public string Identifier { get; private set; }

        public bool IsSolid { get; private set; }
        public bool IsOpaque { get; private set; }    // temporary. will change when we add transparency

        public VoxelFaceMode FaceMode { get; private set; }
        public VoxelInitialDataMode InitialDataMode { get; private set; }

        public byte? PointLightLevel { get; private set; }

        public IVoxelSkin Skin { get; private set; }
        public UI.VoxelIconMesh VoxelIconMesh { get; private set; }

        public VoxelType(string id, bool isSolid, bool isOpaque, IVoxelSkin skin, VoxelFaceMode faceMode, VoxelInitialDataMode initialDataMode, byte? pointLightLevel = null) {
            Identifier = id;
            Skin = skin;
            IsSolid = isSolid;
            IsOpaque = isOpaque;
            FaceMode = faceMode;
            InitialDataMode = initialDataMode;
            PointLightLevel = pointLightLevel;
        }

        public void CreateVoxelIconMesh(UI.VoxelIconMaterial material) {
            var voxel = new Voxel(this);
            switch (InitialDataMode) {
                case VoxelInitialDataMode.None:
                    voxel.Data = 0;
                    break;
                case VoxelInitialDataMode.NormalOrientation:
                    voxel.Data = (ushort) Orientation.Yp;
                    break;
            }
            VoxelIconMesh = new UI.VoxelIconMesh(voxel, material);
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

        public Voxel CreateVoxel(VoxelRaycastResult result) {
            ushort data = 0;
            switch (InitialDataMode) {
                case VoxelInitialDataMode.None:
                    break;
                case VoxelInitialDataMode.NormalOrientation:
                    data = (ushort) result.Normal.ToOrientation();
                    break;
            }
            return new Voxel(this, data);
        }


    }

    public enum VoxelFaceMode {
        Opaque,
        Transparent,
        TransparentInner
    }

    public enum VoxelInitialDataMode {
        None,
        NormalOrientation
    }
}