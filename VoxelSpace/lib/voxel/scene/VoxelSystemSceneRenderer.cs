using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using VoxelSpace.Graphics;
using VoxelSpace.SceneGraph;

namespace VoxelSpace {

    public class VoxelSystemSceneRenderer : SceneRenderer<VoxelSystemScene> {
        
        public VoxelBodyRenderer VoxelBodyRenderer { get; private set; }

        SelectionWireframe _selectionWireframe;

        public VoxelSystemSceneRenderer() {
             // terrain material
            var terrainMaterial = new VoxelTerrainMaterial();
            terrainMaterial.DiffuseIntensity = 0.1f;
            terrainMaterial.AmbientIntensity = 0.8f;
            terrainMaterial.MainTexture = G.Assets.VoxelTextureAtlas.AtlasTexture;
            terrainMaterial.SunlightColor = new Color(255, 255, 192);
            terrainMaterial.StarlightColor = new Color(0, 20, 70);
            VoxelBodyRenderer = new VoxelBodyRenderer(terrainMaterial);

            Camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), G.Graphics.Viewport.AspectRatio, 0.01f, 1000);

            // selection wireframe
            _selectionWireframe = new SelectionWireframe(new BasicEffect(G.Graphics));
            _selectionWireframe.Effect.DiffuseColor = Vector3.Zero;
            _selectionWireframe.Effect.Projection = Camera.ProjectionMatrix;
            ShadowMap = new ShadowMap(4096);
            ShadowMap.SetCenter(Vector3.Zero);
            ShadowMap.SetRadius(200);
            ShadowMap.SetNearClip(-200);
            ShadowMap.SetFarClip(200);
        }

        protected override void PreRender(VoxelSystemScene scene) {
            Camera.ViewMatrix = scene.Player.ViewMatrix;
            ShadowMap.SetLightDirection(scene.SunDirection);
            VoxelBodyRenderer.SetCamera(Camera);
            VoxelBodyRenderer.SetShadowMap(ShadowMap);
        }

        public override void Dispose() {
            base.Dispose();
            _selectionWireframe?.Effect?.Dispose();
            VoxelBodyRenderer?.Material?.Dispose();
            VoxelBodyRenderer?.Dispose();
        }

        public override void OnScreenResize(int width, int height) {
            Camera.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), G.Graphics.Viewport.AspectRatio, 0.01f, 1000);
        }

        protected override void Render(VoxelSystemScene scene, RenderPass pass) {
            var player = scene.Player;
            var planet = scene.Planet;
            VoxelBodyRenderer.Material.SunDirection = scene.SunDirection.Normalized();
            VoxelBodyRenderer.Render(planet, pass);
            if (pass == RenderPass.Geometry && player.IsAimValid) {
                _selectionWireframe.Effect.View = Camera.ViewMatrix;
                _selectionWireframe.Effect.Projection = Camera.ProjectionMatrix;
                _selectionWireframe.Draw(player.AimedVoxel.Coords, G.Graphics);
            }
        }
    }

}