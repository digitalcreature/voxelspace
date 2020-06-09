using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace {

    public abstract class GravityField {

        public abstract Vector3 GetGravity(Vector3 position);

        public void AlignToGravity(Transform t) {
            var g = GetGravity(t.position);
            var f = t.forward.ProjectPlane(g);
            t.rotation = Quaternion.CreateFromRotationMatrix(f.CreateLookMatrix(-g));
        }

    }

}