using Microsoft.Xna.Framework;

namespace VoxelSpace.Graphics {

    public struct Camera {
        
        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;

        public Camera(Matrix viewMatrix, Matrix projectionMatrix) {
            ViewMatrix = viewMatrix;
            ProjectionMatrix = projectionMatrix;
        }

    }

}