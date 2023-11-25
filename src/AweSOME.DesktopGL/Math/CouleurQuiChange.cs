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
    class CouleurQuiChange
    {
        public Color Couleur
        {
            get
            {
                Color couleur;
                Maths.ModifierCouleur(ref CouleurBase, MaxChangement, out couleur);
                return couleur; 
            }
        }
        public Color CouleurBase;
        public int MaxChangement;

        public CouleurQuiChange(Color couleur, int maxChangement)
        {
            CouleurBase = couleur;
            MaxChangement = maxChangement;
        }

        public static implicit operator Color(CouleurQuiChange couleurQuiChange)
        {
            return couleurQuiChange.Couleur;
        }

        //public static explicit operator Color(CouleurQuiChange couleurQuiChange)
        //{
        //    return couleurQuiChange.Couleur;
        //}

        public static implicit operator CouleurQuiChange(Color couleur)
        {
            return new CouleurQuiChange(couleur, 50);
        }
    }
}
