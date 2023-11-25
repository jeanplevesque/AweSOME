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
    

    class Bouton:TextBox
    {
        public static Vector2 DIMENSIONS_BOUTONS = new Vector2(500, 75);

        public delegate void EventDeClic();
        public event EventDeClic Cliqué;

        
        public Écran ÉcranParent { get; private set; }
        public bool SourisEstDessus { get; private set; }

        public Bouton(Vector2 position, Vector2 dimensions, Écran écranParent,string texte = "DEFAULT_TEXT")
            : base(position, dimensions)
        {
            ÉcranParent = écranParent;

            GestionIntrants.ClickGaucheInverse += new GestionIntrants.ÉvénementSouris(VérifierCliqueSouris);

            Font = BanqueContent.GetFont("Copperplate");

            Write(texte);
            //WarpTexte(TextBoxWarpTypes.KeepAspectRatio);
        }

        public void Update()
        {
            CalculerSiSourisEstDessus();
            if (SourisEstDessus)
            {
                EffetSurTexte.Update();
            }
            else
            {
                //EffetSurTexte.Reset();
            }
        }

        private void VérifierCliqueSouris()
        {
            if (SourisEstDessus)
            {
                Cliqué.Invoke();
            }
        }

        protected void CalculerSiSourisEstDessus()
        {
            Vector2 pos = GestionIntrants.PositionSouris;
            SourisEstDessus = (pos.X > Position.X - Dimensions.X * 0.5f &&
                                pos.Y > Position.Y - Dimensions.Y * 0.5f &&
                                pos.X < Position.X + Dimensions.X * 0.5f &&
                                pos.Y < Position.Y + Dimensions.Y * 0.5f);
        }
    }
}
