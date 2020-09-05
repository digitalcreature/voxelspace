using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.UI {

    public class Skin {

        public BoxStyle Button;
        public TextBoxStyle TextBox;

    }

    public class BoxStyle {

        public Style Normal;
        public Style Hover;
        public Style Disabled;
    }

    public class Style {

        public IDrawable Background;
        public TileFont Font;
        public Color TextColor = Color.White;
        public Color BackgroundColor = Color.White;

        public HorizontalAlign HorizontalAlign = HorizontalAlign.Left;
        public VerticalAlign VerticalAlign = VerticalAlign.Top;
        public Padding Padding;

    }

    public class TextBoxStyle : BoxStyle {

    }

}