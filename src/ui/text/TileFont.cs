using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.UI {

    public class TileFont : IDisposable {

        public Texture2D Texture { get; private set; }
        public TileFontMaterial Material { get; private set; }

        public float CharSpacing { get; private set; }
        public float SpaceWidth { get; private set; }
        public float Baseline { get; private set; }
        public float LineSpacing {get; private set; }

        public int _tileWidth;
        public int _tileHeight;
        int[] _charWidth;
        Vector2 _tileSize;

        static readonly Vector2 _tileUVSize = new Vector2(1 / 16f, 1 / 8f);

        public TileFont(Configuration config) {
            Texture = G.Content.Load<Texture2D>(config._textureName);
            Material = new TileFontMaterial();
            Material.Texture = Texture;
            _charWidth = new int[128];
            _tileSize.X = _tileWidth = Texture.Width / 16;
            _tileSize.Y = _tileHeight = Texture.Height / 8;
            Material.Size = _tileSize;
            SpaceWidth = config._spaceWidth;
            CharSpacing = config._charSpacing;
            Baseline = config._baseLine;
            LineSpacing = config._lineSpacing;
            var pixels = new Color[Texture.Width * Texture.Height];
            Texture.GetData(pixels);
            int j = _tileHeight - 1;
            for (int ti = 0; ti < 16; ti ++) {
                for (int tj = 0; tj < 8; tj ++) {
                    int charWidth = 0;
                    for (int i = 0; i < _tileWidth; i ++) {
                        var color = pixels[ti * _tileWidth + i + (tj * _tileHeight + j) * Texture.Width];
                        if (color.A == 0) {
                            break;
                        }
                        else {
                            charWidth ++;
                        }
                    }
                    _charWidth[ti + tj * 16] = charWidth;
                }
            }
        }

        public void DrawString(UI ui, Matrix projection, Vector2 position, string text, HorizontalAlign halign = HorizontalAlign.Left, VerticalAlign valign = VerticalAlign.Top) {
            Material.ProjectionMatrix = projection;
            if (valign != VerticalAlign.Top) {
                float height = getTextHeight(text);
                if (valign == VerticalAlign.Middle) {
                    height = (int) height / 2;
                }
                position.Y -= height;
            }
            int i = 0;
            while (i < text.Length) {
                drawLine(ui, projection, position, text, ref i, halign);
                position.Y += Baseline + LineSpacing;
            }
        }

        void drawLine(UI ui, Matrix matrix, Vector2 position, string text, ref int i, HorizontalAlign halign) {
            if (halign != HorizontalAlign.Left) {
                int wi = i;
                var width = getLineWidth(text, ref wi);
                if (halign == HorizontalAlign.Center) {
                    width = (int) width / 2;
                }
                position.X -= width;
            }
            while (i < text.Length) {
                var c = GetCharacterIndex(text[i]);
                if (c == -1) {
                    i ++;
                    break;
                }
                else {
                    if (c == 32) {
                        // space
                        position.X += SpaceWidth;
                    }
                    else {
                        Material.Position = position;
                        var uv = new Vector2(1/16f, 1/8f);
                        uv.X *= c % 16;
                        uv.Y *= c / 16;
                        Material.UVOffset = uv;
                        position.X += CharSpacing + _charWidth[c];
                        Material.Bind();
                        Primitives.DrawQuad();
                    }
                }
                i ++;
            }
        }

        float getTextHeight(string text) {
            float height = 1;
            for (int i = 0; i < text.Length; i ++) {
                if (text[i] == '\n'){
                    height ++;
                }
            }
            return height * Baseline + (height - 1) * LineSpacing;
        }

        float getLineWidth(string text, ref int i) {
            float width = 0;
            while (i < text.Length) {
                var c = GetCharacterIndex(text[i]);
                if (c == -1) {
                    i ++;
                    break;
                }
                else {
                    if (c == 32) {
                        width += SpaceWidth;
                    }
                    else {
                        width += CharSpacing + _charWidth[c];
                    }
                }
                i ++;
            }
            return width;
        }

        /// <summary>
        /// Get the index that will be drawn for a specific character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>ASCII value of <c>c</c> if it is a visible ASCII character. 0 if unicode or invisible, -1 if newline</returns>
        public int GetCharacterIndex(char c) {
            if (c == '\n') {
                return -1;
            }
            else {
                var i = Convert.ToUInt32(c);
                if (i > 127 || i < 32) {
                    return 0;
                }
                else {
                    return (int) i;
                }
            }
        }

        public void Dispose() {
            Texture.Dispose();
        }

        public class Configuration {

            public string _textureName;
            public float _charSpacing = 1;
            public float _lineSpacing = 1;
            public float _spaceWidth = 8;
            public float _baseLine;

            public Configuration(string textureName) {
                _textureName = textureName;
            }

            public Configuration CharSpacing(float spacing) {
                _charSpacing = spacing;
                return this;
            }

            public Configuration LineSpacing(float spacing) {
                _lineSpacing = spacing;
                return this;
            }

            public Configuration SpaceWidth(float spaceWidth) {
                _spaceWidth = spaceWidth;
                return this;
            }

            public Configuration Baseline(float baseLine) {
                _baseLine = baseLine;
                return this;
            }


        }

    }

    public enum HorizontalAlign {
        Left, Center, Right
    }

    public enum VerticalAlign {
        Top, Middle, Bottom
    }

}