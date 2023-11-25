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
using System.IO;

namespace AweSOME
{
    class Glow:Sprite
    {
        Sprite Parent;

        public Vector2 AjoutDimensions = new Vector2(6,12);
        public Vector2 MultiplicateurDimensions = new Vector2(2, 2);

        public float Alpha = 1f;

        public Glow(Vector2 position, ref Color couleur, Sprite parent)
            : base(position)
        {
            Couleur = couleur;
            Parent = parent;
            Dimensions = Parent.Dimensions * MultiplicateurDimensions + AjoutDimensions;
            Angle = parent.Angle;
            Origine = Vector2.One / 2f;
            ArrangerSprite(GestionTexture.GetTexture("Particules/GlowBack"));
        }

        public void Update(ref Vector2 position)
        {
            Angle = Parent.Angle;
            Position = position;
            Dimensions = Parent.Dimensions * MultiplicateurDimensions + AjoutDimensions;
            ArrangerSprite();
        }
        public void Update(Vector2 position,float alpha)
        {
            Alpha = alpha;
            Angle = Parent.Angle;
            Position = position;
            Dimensions = Parent.Dimensions * MultiplicateurDimensions + AjoutDimensions;
            ArrangerSprite();
        }
        public void UpdateAlphaOnly(float alpha)
        {
            Alpha = alpha;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, null, Couleur * Alpha, Angle, OrigineSprite, Scale, SpriteEffect, Profondeur);
        }

    }
}
