using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace VoxelSpace {

    public class TextureAtlas : IDisposable {

        public Texture2D atlasTexture { get; private set; }
        // what is the pixel width of each tile?
        public int tilePixelWidth { get; private set; }
        // what is the width of each tile's uv space mapping?
        public float tileUVWidth { get; private set; }
        // how many tiles wide is the atlas?
        public int atlasTileWidth { get; private set; }

        Dictionary<string, TileTexture> tiles;

        public TextureAtlas() {
            tiles = new Dictionary<string, TileTexture>();
            tilePixelWidth = -1;
        }

        public void AddTileTexture(TileTexture tile) {
            var t = tile.texture;
            if (t.Width != t.Height) {
                Logger.ErrorFormat(this, "Can't add texture {0} to atlas: Not square {1}x{2}", t.Name, t.Width, t.Height);
                return;
            }
            if (tilePixelWidth == -1) {
                tilePixelWidth = t.Width;
            }
            else if (tilePixelWidth != t.Width) {
                Logger.ErrorFormat(this, "Can't add texture {0} to atlas: Width {1} is not expected {2}", t.Name, t.Width, tilePixelWidth);
                return;
            }
            tiles.Add(t.Name, tile);
        }

        public Texture2D CreateAtlasTexture(GraphicsDevice graphics) {
            if (atlasTexture != null) atlasTexture.Dispose();
            int tileCount = tiles.Count;
            int atlasTileWidth = (int) MathF.Ceiling(MathF.Sqrt(tileCount));
            int atlasPixelWidth = atlasTileWidth * tilePixelWidth;
            atlasTexture = new Texture2D(graphics, atlasPixelWidth, atlasPixelWidth, false, SurfaceFormat.Color);
            var atlasData = new Color[atlasPixelWidth * atlasPixelWidth];
            var tileData = new Color[tilePixelWidth * tilePixelWidth];
            int tileIndex = 0;
            tileUVWidth = 1f / atlasTileWidth;
            foreach (var tile in tiles.Values) {
                int ti = tileIndex % atlasTileWidth;
                int tj = tileIndex / atlasTileWidth;
                tile.texture.GetData<Color>(tileData);
                var tileUV = new Vector2(ti * tileUVWidth, tj * tileUVWidth);
                tile.AddToAtlas(this, tileUV);
                for (int i = 0; i < tilePixelWidth; i ++) {
                    for (int j = 0; j < tilePixelWidth; j ++) {
                        int ai = ti * tilePixelWidth + i;
                        int aj = tj * tilePixelWidth + j;
                        atlasData[aj * atlasPixelWidth + ai] = tileData[j * tilePixelWidth + i];
                    }
                }
                tileIndex ++;
            }
            atlasTexture.SetData(atlasData);
            return atlasTexture;
        }

        public void Dispose() {
            if (atlasTexture != null) {
                atlasTexture.Dispose();
            }
        }

    }

}