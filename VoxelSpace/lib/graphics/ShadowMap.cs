using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.Graphics {

    public class ShadowMap : IDisposable {

        public Vector3 LightDirection { get; private set; }
        public Vector3 LightUp { get; private set; }
        public Vector3 Center { get; private set; }

        public float Radius { get; private set; }
        public float NearClip { get; private set; }
        public float FarClip { get; private set; }

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        public int TextureSize { get; private set; }

        public RenderTarget2D ShadowTarget { get; private set; }

        public ShadowMap(int textureSize) {
            TextureSize = textureSize;
            UpdateTexture();
        }

        public void SetLightDirection(Vector3 lightDirection) {
            var last = LightDirection;
            LightDirection = lightDirection.Normalized();
            if (last == Vector3.Zero) {
                CalculateUp();
            }
            else {
                var proj = LightUp.ProjectPlane(lightDirection);
                if (proj.LengthSquared() < 0.01f) {
                    // if the direction moved too far and is too close to parallel to the old up, calculate a new up
                    CalculateUp();
                }
                else {
                    // otherwise just use the new projection
                    LightUp = proj.Normalized();
                }
            }
            UpdateView();
        }

        // calculate an arbitrary up vector perpendicular to the light direction
        void CalculateUp() {
            var abs = LightDirection.Abs();
            var max = abs.Max();
            Vector3 up = Vector3.UnitY;
            if (max == abs.X) {
                up = new Vector3(0, 1, 0);
            }
            if (max == abs.Y) {
                up = new Vector3(0, 0, 1);
            }
            if (max == abs.Z) {
                up = new Vector3(1, 0, 0);
            }
            LightUp = up.ProjectPlane(LightDirection).Normalized();
        }
        
        public void SetCenter(Vector3 center) {
            Center = center;
            UpdateView();
        }

        public void SetRadius(float radius) {
            Radius = radius;
            UpdateProjection();
        }

        public void SetNearClip(float clip) {
            NearClip = clip;
            UpdateProjection();
        }

        public void SetFarClip(float clip) {
            FarClip = clip;
            UpdateProjection();
        }

        public void SetTextureSize(int size) {
            TextureSize = size;
            UpdateTexture();
        }

        void UpdateView() {
            ViewMatrix = Matrix.CreateLookAt(Center, Center + LightDirection, LightUp);
        }

        void UpdateProjection() {
            ProjectionMatrix = Matrix.CreateOrthographic(Radius * 2, Radius * 2, NearClip, FarClip);
        }

        void UpdateTexture() {
            var graphics = G.Graphics;
            ShadowTarget?.Dispose();
            ShadowTarget = new RenderTarget2D(graphics, TextureSize, TextureSize, false, SurfaceFormat.Color, DepthFormat.Depth24);
        }

        public void Dispose() {
            ShadowTarget?.Dispose();
        }


    }

}