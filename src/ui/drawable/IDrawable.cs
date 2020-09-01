using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VoxelSpace.UI {

    public interface IDrawable {

        void DrawUI(UI ui, Matrix projection, Rect rect);

    }

}