using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AweSOME
{
    class ParticuleDoubleTexture:Particule
    {
        public Texture2D Image2;
        public float Profondeur2;

        public ParticuleDoubleTexture(int tempsMax, int tempsFin)
            : base(tempsMax, tempsFin)
        {
            Profondeur2 = 0.0199f;
            Image = GestionTexture.GetTexture("Particules/GlowBack");
            Image2 = GestionTexture.GetTexture("Particules/GlowFront");
        }


        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(Image2, Position, null, Color.DarkGray * Alpha, Angle, OrigineSprite, Scale, SpriteEffect, Profondeur2);
        }
    }
}
