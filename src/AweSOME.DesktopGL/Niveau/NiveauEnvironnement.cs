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
    enum ÉtatsJournée { Day, Dusk, Night, Dawn}

    partial class Niveau
    {
        public const float HEURE_DAWN = 5f;
        public const float HEURE_DUSK = 17f;
        public const float DURÉE_DAWN = 3f;
        public const float DURÉE_DUSK = 3f;

        public static TimeSpan HeureDébutRound = TimeSpan.FromHours(19);
        public static TimeSpan HeureFinRound = TimeSpan.FromHours(7);

        public static float UnSurNbSecondesJour = 1f / (float)TimeSpan.FromHours(24).TotalSeconds;

        public static float SoustracteurDawn = (float)TimeSpan.FromHours(HEURE_DAWN + DURÉE_DAWN).TotalSeconds;
        public static float SoustracteurDusk = (float)TimeSpan.FromHours(HEURE_DUSK + DURÉE_DUSK).TotalSeconds;
        public static float DiviseurDawn = 1f / (float)TimeSpan.FromHours(DURÉE_DAWN).TotalSeconds;
        public static float DiviseurDusk = 1f / (float)TimeSpan.FromHours(DURÉE_DUSK).TotalSeconds;

        public static TimeSpan DuréeJourMax = TimeSpan.FromMinutes(5);

        public static Color CouleurCielNuit = new Color(25, 49, 107);
        public static Color CouleurCielJour = new Color(74, 142, 255);
        public static Color LumièreAmbianteNuit = new Color(5, 5, 5);
        public static Color LumièreAmbianteJour = new Color(200, 200, 200);

        public float MinVitesseTempsSecondes = 1;
        public float MaxVitesseTempsSecondes = 180;

        public Vector2 CentreRotationAstres;

        public Color CouleurCiel;
        public Color LumièreAmbiante;

        public AstreLumineux Soleil;
        public AstreLumineux Lune;
        
        public float MomentPrésentFloat;
        public TimeSpan MomentPrésent;

        public TimeSpan TempsMaxAvanceRapide;
        public TimeSpan TempsAvanceRapide;

        public ÉtatsJournée ÉtatJournée
        {
            get { return étatJournée_; }
            private set
            {
                //if (étatJournée_ != value)
                //{
                //    AvanceRapide = false;
                //}
                étatJournée_ = value;
            }
        }
        private ÉtatsJournée étatJournée_;

        public float TransitionneurJournée
        {
            get { return transitionneurJournée_; }
            private set
            {
                transitionneurJournée_ = value;

                if (transitionneurJournée_ >= 1.0f)
                {
                    transitionneurJournée_ = 1;
                    ÉtatJournée = ÉtatsJournée.Day;
                }
                else if (transitionneurJournée_ <= 0.0f)
                {
                    transitionneurJournée_ = 0;
                    ÉtatJournée = ÉtatsJournée.Night;
                }
                Lune.Lumière.BackLight.Intensity = (1 - transitionneurJournée_) * Lune.MaxIntensité;
                Soleil.Lumière.BackLight.Intensity = transitionneurJournée_;

                Maths.MixerCouleurs(ref CouleurCielJour, ref CouleurCielNuit, transitionneurJournée_, out CouleurCiel);
                Maths.MixerCouleurs(ref LumièreAmbianteJour, ref LumièreAmbianteNuit, transitionneurJournée_, out LumièreAmbiante);
            }
        }
        private float transitionneurJournée_;

        public bool AvanceRapide
        {
            get { return avanceRapide_; }
            set
            {
                avanceRapide_ = value;
                if (avanceRapide_)
                {
                    VitesseTemps = TimeSpan.FromSeconds(MaxVitesseTempsSecondes);
                }
                else
                {
                    VitesseTemps = TimeSpan.FromSeconds(MinVitesseTempsSecondes);
                }
            }
        }
        private bool avanceRapide_;

        public TimeSpan VitesseTemps;

        public void InitialiserEnvironnement()
        {
            VitesseTemps = TimeSpan.FromSeconds(MinVitesseTempsSecondes);
            //MomentPrésent = new TimeSpan(0,0,0);
            CentreRotationAstres = new Vector2(DimensionsNiveau.X / 2, 0);

            Soleil = AstreLumineux.CréerSoleil(ref DimensionsNiveau);
            Lune = AstreLumineux.CréerLune(ref DimensionsNiveau);

            //ÉtatJournée = ÉtatsJournée.Night;
            //TransitionneurJournée = 0;

            MomentPrésent = new TimeSpan(12, 0, 0);

            ÉtatJournée = ÉtatsJournée.Day;
            TransitionneurJournée = 1;
        }

        //public void FaireTomberJour()
        //{
        //    if (ÉtatJournée == ÉtatsJournée.Night)
        //    {
        //        ÉtatJournée = ÉtatsJournée.Dawn;
        //    }
        //}
        //public void FaireTomberNuit()
        //{
        //    if (ÉtatJournée == ÉtatsJournée.Day)
        //    {
        //        ÉtatJournée = ÉtatsJournée.Dusk;
        //    }
        //}
         
        private void UpdateÉtatJour()
        {           
            MomentPrésent += VitesseTemps;
            MomentPrésentFloat = (float)MomentPrésent.TotalSeconds * UnSurNbSecondesJour;

            Soleil.Update(MomentPrésentFloat);
            Lune.Update(MomentPrésentFloat);

            if (AvanceRapide)
            {
                TempsAvanceRapide += VitesseTemps;
                if (TempsAvanceRapide >= TempsMaxAvanceRapide)
                {
                    AvanceRapide = false;
                }
            }


            switch (ÉtatJournée)
            {
                case ÉtatsJournée.Day:
                    if (MomentPrésent.Hours == HEURE_DUSK)
                    {
                        ÉtatJournée = ÉtatsJournée.Dusk;
                    }
                    break;
                case ÉtatsJournée.Dusk:
                    TransitionneurJournée = ((SoustracteurDusk - (float)MomentPrésent.TotalSeconds) * DiviseurDusk);
                    break;
                case ÉtatsJournée.Night:
                    if (MomentPrésent.Days == 1) { MomentPrésent -= TimeSpan.FromDays(1); }
                    if (MomentPrésent.Hours == HEURE_DAWN)
                    {
                        ÉtatJournée = ÉtatsJournée.Dawn;
                    }
                    break;
                case ÉtatsJournée.Dawn:
                    TransitionneurJournée = 1f - ((SoustracteurDawn - (float)MomentPrésent.TotalSeconds) * DiviseurDawn);
                    break;
            }


            //Changer force du Vent
            if (Maths.Random.Next(100) == 0) { GestionEffets.Vent = Vector2.UnitX * Maths.Random.Next(-50, 50) * 0.001f; }
        }
        public void AvancerTempsJusqua(TimeSpan moment)
        {
            AvanceRapide = true;
            TempsAvanceRapide = TimeSpan.Zero;
            TempsMaxAvanceRapide = TimeSpan.FromHours((moment - MomentPrésent + TimeSpan.FromHours(24)).TotalHours % 24);
            //if (TempsMaxAvanceRapide < TimeSpan.Zero)
            //{
            //    TempsMaxAvanceRapide = TimeSpan.FromSeconds(TempsMaxAvanceRapide.TotalSeconds * -1);
            //}
        }

        private void DessinerAstres()
        {
            //MoteurJeu.SpriteBatchAdditive.DrawString(BanqueContent.Font1, (DateTime.Parse(MomentPrésent.ToString())).ToShortTimeString(), Vector2.UnitY * 50, Color.White, 0, Vector2.Zero, 3, SpriteEffects.None, 0);
            MoteurJeu.SpriteBatchAdditive.DrawString(BanqueContent.Font1, (MomentPrésent.ToString()), Vector2.UnitY * 50, Color.White, 0, Vector2.Zero, 3, SpriteEffects.None, 0);


            Soleil.Draw(MoteurJeu.SpriteBatchScène);
            Lune.Draw(MoteurJeu.SpriteBatchAdditive);
        }
    }
}
