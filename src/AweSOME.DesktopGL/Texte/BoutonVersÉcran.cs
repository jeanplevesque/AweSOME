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
    class BoutonVersÉcran:Bouton
    {
        public delegate Écran EventVersÉcran(int écranSouhaité);
        public event EventVersÉcran CliquéPourÉcran;

        public int ÉcranÀAppeler;// { get; private set; }

        public BoutonVersÉcran(Vector2 position, Vector2 dimensions, Écran écranParent,int écranÀAppeler, string texte = "DEFAULT_TEXT")
            : base(position, dimensions, écranParent, texte)
        {
            ÉcranÀAppeler = écranÀAppeler;

            Cliqué += new EventDeClic(BoutonVersÉcran_Cliqué);

            CliquéPourÉcran += new BoutonVersÉcran.EventVersÉcran(GestionÉcran.AllerÀÉcran);
            //boutonMulti.Couleur = Color.Transparent;
            //boutonStart.CouleurTexte = Color.LightGray;
            //boutonMulti.CentrerTexteXY();
            Update();
        }

        void BoutonVersÉcran_Cliqué()
        {
            CliquéPourÉcran.Invoke(ÉcranÀAppeler);
        }
    }
}
