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
    class EffetTexteFlotant:TextBox,IEffet
    {
        public float Alpha = 1;
        public int TempsRestant;
        public int TempsFin;


        public EffetTexteFlotant(Vector2 position, Vector2 dimensions, int tempsRestant = 300, int tempsFin = 60)
            : base(position, dimensions)
        {
            TempsRestant = tempsRestant;
            TempsFin = tempsFin;

            Couleur = Color.Transparent;

        }

        public void Update()
        {
            if (TempsRestant > 0)
            {
                this.EffetSurTexte.Update();


                if (TempsRestant < TempsFin)
                {
                    Alpha = (float)TempsRestant / (float)TempsFin;
                    if (TempsRestant <= 0)
                    {
                        Détruire();
                    }

                }
                --TempsRestant;
            }
            else
            {
                Détruire();
            }
        }

        public void Détruire()
        {
            GestionEffets.DétruireEffet(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, null, Couleur * Alpha, Angle, OrigineSprite, Scale, SpriteEffect, Profondeur);
            if (TexteCentréY)
            {
                Vector2 dimensionsY = new Vector2(DistancePositionTexteOrigine.X, Font.MeasureString(Texte).Y * -0.5f * ScaleTexte.Y);
                Vector2 deltaPosition;

                Vector2.Transform(ref dimensionsY, ref Matrice, out deltaPosition);
                PositionTexte = Position + deltaPosition;
            }
            if (TexteCentréX)
            {
                Vector2 dimensions;
                Vector2 deltaPosition;
                string texte;
                for (int i = 0; i < ListeLignes.Count; ++i)
                {
                    texte = ListeLignes[i];
                    dimensions = Font.MeasureString(texte);
                    deltaPosition.X = -dimensions.X * 0.5f * ScaleTexte.X;
                    deltaPosition.Y = i * Font.LineSpacing * ScaleTexte.Y;

                    Vector2.Transform(ref deltaPosition, ref Matrice, out deltaPosition);
                    spriteBatch.DrawString(Font, texte, PositionTexte + deltaPosition, CouleurTexte*Alpha, Angle, OrigineTexte, ScaleTexte, SpriteEffects.None, ProfondeurTexte);
                }
            }
            else
            {
                spriteBatch.DrawString(Font, Texte, PositionTexte, CouleurTexte*Alpha, Angle, OrigineTexte, ScaleTexte, SpriteEffects.None, ProfondeurTexte);
            }
        }
    }
}
