using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public class TextureAtlas : IDisposable {

        public Texture2D AtlasTexture { get; private set; }
        // what is the pixel width of each tile?
        public int TilePixelWidth { get; private set; }
        // what is the width of each tile's uv space mapping?
        public float TileUVWidth { get; private set; }
        // how many tiles wide is the atlas?
        public int AtlasTileWidth { get; private set; }

        Dictionary<string, TileTexture> _tiles;

        public TextureAtlas() {
            _tiles = new Dictionary<string, TileTexture>();
            TilePixelWidth = -1;
        }

        public void AddTileTexture(TileTexture tile) {
            var t = tile.Texture;
            if (t.Width != t.Height) {
                Logger.Error(this, $"Can't add texture {t.Name} to atlas: Not square {t.Width}x{t.Height}");
                return;
            }
            if (TilePixelWidth == -1) {
                TilePixelWidth = t.Width;
            }
            else if (TilePixelWidth != t.Width) {
                Logger.Error(this, $"Can't add texture {t.Name} to atlas: Width {t.Width} is not expected {TilePixelWidth}");
                return;
            }
            _tiles.Add(t.Name, tile);
        }

        public Texture2D CreateAtlasTexture(GraphicsDevice graphics) {
            if (AtlasTexture != null) AtlasTexture.Dispose();
            int tileCount = _tiles.Count;
            int atlasTileWidth = (int) MathF.Ceiling(MathF.Sqrt(tileCount));
            int atlasPixelWidth = atlasTileWidth * TilePixelWidth;
            AtlasTexture = new Texture2D(graphics, atlasPixelWidth, atlasPixelWidth, false, SurfaceFormat.Color);
            var atlasData = new Color[atlasPixelWidth * atlasPixelWidth];
            var tileData = new Color[TilePixelWidth * TilePixelWidth];
            int tileIndex = 0;
            TileUVWidth = 1f / atlasTileWidth;
            foreach (var tile in _tiles.Values) {
                int ti = tileIndex % atlasTileWidth;
                int tj = tileIndex / atlasTileWidth;
                tile.Texture.GetData<Color>(tileData);
                var tileUV = new Vector2(ti * TileUVWidth, tj * TileUVWidth);
                tile.AddToAtlas(this, new QuadUVs(tileUV, tileUV + Vector2.One * TileUVWidth));
                for (int i = 0; i < TilePixelWidth; i ++) {
                    for (int j = 0; j < TilePixelWidth; j ++) {
                        int ai = ti * TilePixelWidth + i;
                        int aj = tj * TilePixelWidth + j;
                        atlasData[aj * atlasPixelWidth + ai] = tileData[j * TilePixelWidth + i];
                    }
                }
                tileIndex ++;
            }
            AtlasTexture.SetData(atlasData);
            return AtlasTexture;
        }

        public void Dispose() {
            if (AtlasTexture != null) {
                AtlasTexture.Dispose();
            }
        }

    }

}