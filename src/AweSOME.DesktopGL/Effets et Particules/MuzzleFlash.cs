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
using Krypton;
using Krypton.Lights;

namespace AweSOME
{
    class MuzzleFlash:Sprite
    {
        Fusil FusilParent;
        PointLight Lumière;
        bool estPrésent;
        int ticsRestants;
        Color couleurLumiere;

        public MuzzleFlash(Fusil fusil)
        {
            FusilParent = fusil;
            Lumière = new PointLight();
            Lumière.Texture = SourceLumière.Texture360;
            Lumière.Range = 1024;
            Lumière.Intensity = 0.5f;
            Lumière.IsOn = false;

            MoteurJeu.EnginKrypton.Lights.Add(Lumière);

            ArrangerSprite(GestionTexture.GetTexture("Textures/muzzletest"),new Vector2(80,40),new Vector2(0,0.5f),0.3f,Color.White);
        }

        public void Allumer(int nbTics)
        {
            ticsRestants = nbTics;
            Lumière.IsOn = true;
            estPrésent = true;
            
            Maths.ModifierCouleur(ref FusilParent.CouleurBalles, 50, out couleurLumiere);
            Lumière.Color = couleurLumiere;
        }
        public void Éteindre()
        {
            Lumière.IsOn = false;
            estPrésent = false;
        }

        public void Update()
        {
            if (estPrésent)
            {
                --ticsRestants;

                
                Position = FusilParent.PositionEmbout;
                Angle = FusilParent.Angle;
                Lumière.Position = Position;             

                if (ticsRestants <= 0)
                {
                    Éteindre();
                }
            }
        }
        public void Draw()
        {
            if (estPrésent)
            {
                base.Draw(MoteurJeu.SpriteBatchAdditive);
            }
        }
    }
}
