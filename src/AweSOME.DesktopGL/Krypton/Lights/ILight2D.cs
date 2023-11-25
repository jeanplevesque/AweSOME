using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Krypton.Lights
{
    public interface ILight2D
    {
        bool IsOn { get; set; }

        void Draw(KryptonRenderHelper helper);
        void DrawShadows(KryptonRenderHelper helper, List<ShadowHull> hullNode);
    }
}
