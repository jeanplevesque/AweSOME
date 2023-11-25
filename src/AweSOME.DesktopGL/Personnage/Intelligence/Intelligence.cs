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
using AwesomeAnimation;


namespace AweSOME
{
    abstract class Intelligence
    {       
        protected Personnage Perso;
        protected float VitesseXVoulue;

        public Intelligence(Personnage perso)
        {
            Perso = perso;
        }

        public void Update()
        {
            if (Perso.EstEnVie && !Perso.Spawning)
            {
                ChoisirPositionCible();              
                ChangerÉtats();
                GérerDéplacements();
            }
        }

        public virtual void ChangerÉtats()
        {
            if (Perso.Cible.Position.X > Perso.Position.X)
            {
                Perso.Direction = Directions.ÀDroite;
            }
            else
            {
                Perso.Direction = Directions.ÀGauche;
            }
        }
        protected abstract void GérerDéplacements();
        protected abstract void ChoisirPositionCible();

    }
}
