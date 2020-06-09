using System;
using Microsoft.Xna.Framework;

namespace VoxelSpace {

    public static class MatrixExtensions {

        public static string ToStringPretty(this Matrix m) {
            return string.Format("[{0} {1} {2} {3}]\n[{4} {5} {6} {7}]\n[{8} {9} {10} {11}]\n[{12} {13} {14} {15}]",
                m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44);
        }

    }
    
}