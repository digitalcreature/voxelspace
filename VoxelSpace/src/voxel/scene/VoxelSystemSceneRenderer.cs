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
            terrainMaterial.TextureAtlas = G.Assets.VoxelTextureAtlas.AtlasTexture;
            terrainMaterial.SunlightColor = new Color(255, 255, 192);
            terrainMaterial.StarlightColor = new Color(0, 20, 70);
            VoxelBodyRenderer = new VoxelBodyRenderer(terrainMaterial);

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), G.Graphics.Viewport.AspectRatio, 0.01f, 1000);

            // selection wireframe
            _selectionWireframe = new SelectionWireframe(new BasicEffect(G.Graphics));
            _selectionWireframe.Effect.DiffuseColor = Vector3.Zero;
            _selectionWireframe.Effect.Projection = ProjectionMatrix;
        }

        public override void Render(VoxelSystemScene scene) {
            var _player = scene.Player;
            var planet = scene.Planet;
            ViewMatrix = scene.Player.ViewMatrix;
            VoxelBodyRenderer.Material.SunDirection = scene.SunDirection.Normalized();
            VoxelBodyRenderer.Render(planet, ProjectionMatrix, ViewMatrix);
            if (_player.IsAimValid) {
                _selectionWireframe.Effect.View = _player.ViewMatrix;
                _selectionWireframe.Effect.Projection = ProjectionMatrix;
                _selectionWireframe.Draw(_player.AimedVoxel.Coords, G.Graphics);
            }
        }

        public override void Dispose() {
            _selectionWireframe?.Effect?.Dispose();
            VoxelBodyRenderer?.Material?.Dispose();
            VoxelBodyRenderer?.Dispose();
        }

        public override void OnScreenResize(int width, int height) {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), G.Graphics.Viewport.AspectRatio, 0.01f, 1000);
        }
    }

}