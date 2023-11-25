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
    class Player:PersonnageArmé
    {
        public Player(Vector2 position)
            : base(position)
        {
            Cerveau = new IntelligenceHumaine(this);

            GénérerHumainRandom();

            //ArmeMelee.CréerBâton(Vector2.Zero, this).SeFaireRamasser(this);
            MagicWand wand = new MagicWand();
            wand.SeFaireRamasser(this);
        }
        public void BoutonFonction1()
        {
            if (Fusil != null && Fusil is BlocGun)
            {
                ((BlocGun)Fusil).ChangerMode();
            }
        }

        protected void GénérerHumainRandom()
        {

            Tête.ArrangerSprite(GestionTexture.GetTexture("Humain/TêteHumain" + Maths.Random.Next(3).ToString()), Vector2.One * Grosseur * 0.75f, Vector2.One / 2, Profondeur, Color.White);
            Torse.ArrangerSprite(GestionTexture.GetTexture("Humain/TorseHumain" + Maths.Random.Next(5).ToString()), Torse.Dimensions + new Vector2(Grosseur / 2f, Grosseur / 3f), Vector2.One / 2, Profondeur, Color.White);

            JambeGauche.ArrangerSprite(GestionTexture.GetTexture("Humain/JambeHumain" + Maths.Random.Next(5).ToString()));
            JambeDroite.ArrangerSprite(GestionTexture.GetTexture("Humain/JambeHumain" + Maths.Random.Next(5).ToString()));

            BrasDroite.ArrangerSprite(GestionTexture.GetTexture("Humain/BrasHumain" + Maths.Random.Next(5).ToString()), BrasDroite.Dimensions + Vector2.UnitX * Grosseur / 3, new Vector2(0.4f, 0.5f), Profondeur, Color.White);
            BrasGauche.ArrangerSprite(GestionTexture.GetTexture("Humain/BrasHumain" + Maths.Random.Next(5).ToString()), BrasGauche.Dimensions + Vector2.UnitX * Grosseur / 3, new Vector2(0.4f, 0.5f), Profondeur, Color.White);
        }

        
    }
}
