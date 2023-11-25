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
    class Spell : ISpell
    {
        public int TempsRestant = 360;


        public virtual void Déclancher()
        {
            GestionNiveaux.NiveauActif.AjouterISpell(this);
        }

        public virtual void Updater()
        {
            --TempsRestant;
            if (TempsRestant == 0)
            {
                Terminer();
            }
        }

        public virtual void Terminer()
        {
            GestionNiveaux.NiveauActif.DétruireISpell(this);
        }
    }
}
