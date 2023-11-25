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
    public enum TypesEffetsTexte { Aucun, OscillationCouleur, FadeCouleur }
    public class EffetTexte
    {
        /// <summary>
        /// Valeur entre 0f et 1f
        /// </summary>
        public float Progression
        {
            get { return progression_; }
            set
            {
                progression_ = value;
                if (progression_>=1f)
                {
                    if (Répéter)
                    {
                        progression_ = 0f;
                    }
                    else
                    {
                        Terminé = true;
                    }
                }
            }
        }
        float progression_;

        public float VitesseProgression;

        public Color Couleur1;
        public Color Couleur2;
        public bool Répéter;
        public bool Terminé;

        public TypesEffetsTexte TypeEffet
        {
            get { return typeEffet_; }
            set
            {
                typeEffet_ = value;
                switch (typeEffet_)
                {
                    case TypesEffetsTexte.FadeCouleur:
                        Répéter = false;
                        break;
                    case TypesEffetsTexte.OscillationCouleur:
                        Répéter = true;
                        break;
                }
            }
        }
        TypesEffetsTexte typeEffet_;

        public TextBox TexteBox;

        public EffetTexte(TypesEffetsTexte typeEffet, TextBox texteBox, Color couleur1, Color couleur2, float vitesseProgression = 0.01f)
        {
            TypeEffet = typeEffet;
            TexteBox = texteBox;
            VitesseProgression = vitesseProgression;
            Couleur1 = couleur1;
            Couleur2 = couleur2;
        }

        public void Update()
        {
            switch(TypeEffet)
            {
                case TypesEffetsTexte.OscillationCouleur:

                    Maths.MixerCouleurs(ref Couleur1, ref Couleur2, Maths.ValeurOscillanteEntre0et1(Progression), out TexteBox.CouleurTexte);
                    Progression += VitesseProgression;
                    break;

                case TypesEffetsTexte.FadeCouleur:
                    
                    if (!Terminé)
                    {
                        Maths.MixerCouleurs(ref Couleur1, ref Couleur2, Progression, out TexteBox.CouleurTexte);
                        Progression+= VitesseProgression;
                    }
                    break;
            }
        }
        public void Reset()
        {
            Terminé = false;
            Progression = 0;

            Update();
        }
    }
}
