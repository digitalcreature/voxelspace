using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Scene;

namespace VoxelSpace {

    public abstract class GravityField {

        public Vector3 GetGravity(Vector3 position) {
            return GetGravityDirection(position) * GetGravityStrength(position);
        }

        public abstract Vector3 GetGravityDirection(Vector3 position);

        public abstract float GetGravityStrength(Vector3 position);

        public void AlignToGravity(Transform t) {
            var g = GetGravityDirection(t.LocalPosition);
            var f = t.LocalForward.ProjectPlane(g);
            t.LocalRotation = Quaternion.CreateFromRotationMatrix(f.CreateLookMatrix(-g));
        }

    }

}