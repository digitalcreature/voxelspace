using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VoxelSpace.UI;

namespace VoxelSpace.Assets {

    public class UISkinInfo : AssetInfo<UI.Skin> {

        BoxStyle _button;
        TextBoxStyle _textBox;

        public UISkinInfo(string name) : base(name) {}

        public UISkinInfo Button(BoxStyle style) {
            _button = style;
            return this;
        }

        public UISkinInfo Button(UIBoxStyleInfo style) {
            return Button(Module.Add(style));
        }

        public UISkinInfo TextBox(TextBoxStyle style) {
            _textBox = style;
            return this;
        }

        public UISkinInfo TextBox(UITextBoxStyleInfo style) {
            return TextBox(Module.Add(style));
        }

        protected override Skin Create() {
            return new Skin() {
                Button = _button,
                TextBox = _textBox
            };
        }
    }

    public class UIStyleInfo : AssetInfo<Style> {

        UI.IDrawable _background;
        TileFont _font;
        Color _textColor = Color.White;
        Color _backgroundColor = Color.White;

        HorizontalAlign _horizontalAlign = UI.HorizontalAlign.Left;
        VerticalAlign _verticalAlign = UI.VerticalAlign.Top;
        Padding _padding;

        public UIStyleInfo(string name) : base(name) {}

        public UIStyleInfo TextColor(Color color) {
            _textColor = color;
            return this;
        }

        public UIStyleInfo Background(UI.IDrawable bg) {
            _background = bg;
            return this;
        }

        public UIStyleInfo Background<T>(UIDrawableInfo<T> bg) where T : class, UI.IDrawable {
            return Background(Module.Add(bg));
        }

        public UIStyleInfo Font(TileFont font) {
            _font = font;
            return this;
        }

        public UIStyleInfo Font(TileFontInfo font) {
            return Font(Module.Add(font));
        }

        public UIStyleInfo BackgroundColor(Color color) {
            _backgroundColor = color;
            return this;
        }

        public UIStyleInfo HorizontalAlign(HorizontalAlign align) {
            _horizontalAlign = align;
            return this;
        }

        public UIStyleInfo VerticalAlign(VerticalAlign align) {
            _verticalAlign = align;
            return this;
        }

        public UIStyleInfo Align(HorizontalAlign halign, VerticalAlign valign) {
            _horizontalAlign = halign;
            _verticalAlign = valign;
            return this;
        }

        public UIStyleInfo Padding(Padding padding) {
            _padding = padding;
            return this;
        }

        protected override Style Create() {
            return new Style() {
                Background = _background,
                Font = _font,
                TextColor = _textColor,
                BackgroundColor = _backgroundColor,
                HorizontalAlign = _horizontalAlign,
                VerticalAlign = _verticalAlign,
                Padding = _padding
            };
        }
    }

    public class UIBoxStyleInfo : AssetInfo<BoxStyle> {

        Style _normal;
        Style _hover;
        Style _disabled;

        public UIBoxStyleInfo(string name) : base(name) {}

        public UIBoxStyleInfo Normal(Style style) {
            _normal = style;
            return this;
        }

        public UIBoxStyleInfo Normal(UIStyleInfo style) {
            return Normal(Module.Add(style));
        }

        public UIBoxStyleInfo Hover(Style style) {
            _hover = style;
            return this;
        }

        public UIBoxStyleInfo Hover(UIStyleInfo style) {
            return Hover(Module.Add(style));
        }

        public UIBoxStyleInfo Disabled(Style style) {
            _disabled = style;
            return this;
        }

        public UIBoxStyleInfo Disabled(UIStyleInfo style) {
            return Disabled(Module.Add(style));
        }

        protected override BoxStyle Create() {
            return new BoxStyle() {
                Normal = _normal,
                Hover = _hover,
                Disabled = _disabled
            };
        }

    }

    public class UITextBoxStyleInfo : AssetInfo<TextBoxStyle> {

        Style _normal;
        Style _hover;
        Style _disabled;
        Style _active;
        UI.IDrawable _cursor;
        Color _cursorColor = Color.White;

        public UITextBoxStyleInfo(string name) : base(name) {}

        public UITextBoxStyleInfo Normal(Style style) {
            _normal = style;
            return this;
        }

        public UITextBoxStyleInfo Normal(UIStyleInfo style) {
            return Normal(Module.Add(style));
        }

        public UITextBoxStyleInfo Hover(Style style) {
            _hover = style;
            return this;
        }

        public UITextBoxStyleInfo Hover(UIStyleInfo style) {
            return Hover(Module.Add(style));
        }

        public UITextBoxStyleInfo Disabled(Style style) {
            _disabled = style;
            return this;
        }

        public UITextBoxStyleInfo Disabled(UIStyleInfo style) {
            return Disabled(Module.Add(style));
        }

        public UITextBoxStyleInfo Active(Style style) {
            _active = style;
            return this;
        }

        public UITextBoxStyleInfo Active(UIStyleInfo style) {
            return Active(Module.Add(style));
        }

        public UITextBoxStyleInfo Cursor(UI.IDrawable cursor) {
            _cursor = cursor;
            return this;
        }

        public UITextBoxStyleInfo Cursor<T>(UIDrawableInfo<T> cursor) where T : class, UI.IDrawable {
            return Cursor(Module.Add(cursor));
        }

        public UITextBoxStyleInfo CursorColor(Color color) {
            _cursorColor = color;
            return this;
        }

        protected override TextBoxStyle Create() {
            return new TextBoxStyle() {
                Normal = _normal,
                Hover = _hover,
                Disabled = _disabled,
                Active = _active,
                Cursor = _cursor,
                CursorColor = _cursorColor
            };
        }

    }

    public class TileFontInfo : AssetInfo<TileFont> {

        Texture2D texture;
        float _charSpacing = 1;
        float _lineSpacing = 1;
        float _spaceWidth = 8;
        float _baseLine;

        public TileFontInfo(string name) : base(name) {
            texture = Module.LoadResource<Texture2D>(QualifiedName);
        }

        public TileFontInfo CharSpacing(float spacing) {
            _charSpacing = spacing;
            return this;
        }

        public TileFontInfo LineSpacing(float spacing) {
            _lineSpacing = spacing;
            return this;
        }

        public TileFontInfo SpaceWidth(float spaceWidth) {
            _spaceWidth = spaceWidth;
            return this;
        }

        public TileFontInfo Baseline(float baseLine) {
            _baseLine = baseLine;
            return this;
        }

        protected override TileFont Create() {
            return new TileFont(texture, _charSpacing, _lineSpacing, _spaceWidth, _baseLine);
        }


    }

    public abstract class UIDrawableInfo<T> : AssetInfo<T> where T : class, UI.IDrawable {

        public UIDrawableInfo(string name) : base(name) {}

    }

    public class NinePatchInfo : UIDrawableInfo<NinePatch> {

        NinePatch _ninePatch;

        public NinePatchInfo(string name, float left, float top, float right, float bottom) : base(name) {
            _ninePatch = new NinePatch(Module.LoadResource<Texture2D>(QualifiedName), left, top, right, bottom);
        }

        protected override NinePatch Create() {
            return _ninePatch;
        }

    }

}