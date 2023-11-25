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
    static class GestionÉcran
    {
        public static Partie PartieEnCours
        {
            get { return (Partie)LesÉcrans[0]; }
            private set { LesÉcrans[0] = value; }
        }

        public static Menu ÉcranTitre { get { return (Menu)LesÉcrans[1]; } private set { LesÉcrans[1] = value; } }
        public static Menu MenuPrincipale { get { return (Menu)LesÉcrans[2]; } private set { LesÉcrans[2] = value; } }

        public static Menu MenuMultijoueur { get { return (Menu)LesÉcrans[3]; } private set { LesÉcrans[3] = value; } }


        public static Menu MenuSolo { get { return (Menu)LesÉcrans[4]; } private set { LesÉcrans[4] = value; } }
        public static Menu MenuSoloZombie { get { return (Menu)LesÉcrans[5]; } private set { LesÉcrans[5] = value; } }
        public static Menu MenuSoloWizard { get { return (Menu)LesÉcrans[6]; } private set { LesÉcrans[6] = value; } }


        public static Écran ÉcranActif
        {
            get { return écranActif_; }
            private set
            {
                écranActif_ = value;
                MoteurJeu.ÉcranPrésent = écranActif_;
            }
        }
        static Écran écranActif_;

        public static Écran[] LesÉcrans = new Écran[7];

        public static void CréerMenus()
        {
            CréerÉcranTitre();
            CréerMenuPrincipale();
            CréerPartieZombie();

            ÉcranActif = ÉcranTitre;

        }

        public static Partie CréerPartieZombie()
        {
            PartieEnCours = new Partie(MenuPrincipale, TypeNiveau.Normal);
            return PartieEnCours;
        }

        public static IÉcran AllerÀÉcranPartieEnCours()
        {
            ÉcranActif = PartieEnCours;
            return ÉcranActif;
        }
        public static IÉcran AllerÀÉcranMenuPrincipale()
        {
            ÉcranActif = MenuPrincipale;
            return ÉcranActif;
        }
        public static IÉcran AllerÀÉcranTitre()
        {
            ÉcranActif = ÉcranTitre;
            return ÉcranActif;
        }
        public static void AllerÀÉcranPrécédent()
        {
            ÉcranActif = ÉcranActif.ÉcranPrécédent;
            //return ÉcranActif;
        }
        public static Écran AllerÀÉcran(int noÉcran)
        {
            Écran écran = LesÉcrans[noÉcran];
            if (écran != null)
            {
                ÉcranActif = écran;
            }
            return écran;
        }

        private static void CréerÉcranTitre()
        {
            ÉcranTitre = new Menu(null, "Title Screen",false);
            BoutonVersÉcran boutonStart = new BoutonVersÉcran(Caméra.DimensionsFenêtreSurDeux, new Vector2(200,100), ÉcranTitre, 2, "START");
            //boutonStart.CliquéPourÉcran += new BoutonVersÉcran.EventVersÉcran(AllerÀÉcran);
            boutonStart.Couleur = Color.Transparent;
            //boutonStart.CouleurTexte = Color.LightGray;
            boutonStart.WarpTexte(TextBoxWarpTypes.KeepAspectRatio);
            boutonStart.CentrerTexteXY();
            boutonStart.Update();
            ÉcranTitre.ÉcranPrécédent = ÉcranTitre;

            ÉcranTitre.ListeBoutons.Add(boutonStart);
        }
        private static void CréerMenuPrincipale()
        {
            MenuPrincipale = new Menu(ÉcranTitre, "Main Menu");
            BoutonVersÉcran boutonMulti = new BoutonVersÉcran(Caméra.DimensionsFenêtreSurDeux + 
                            new Vector2(Caméra.DimensionsFenêtreSurDeux.X * 0.25f, 
                                        Caméra.DimensionsFenêtreSurDeux.X * -0.125f), 
                            Bouton.DIMENSIONS_BOUTONS, MenuPrincipale, 3, "Multiplayer");
            
            MenuPrincipale.ListeBoutons.Add(boutonMulti);

            BoutonVersÉcran boutonSingle = new BoutonVersÉcran(Caméra.DimensionsFenêtreSurDeux + 
                            new Vector2(Caméra.DimensionsFenêtreSurDeux.X * 0.25f, 
                                        Caméra.DimensionsFenêtreSurDeux.X * 0.125f), 
                            Bouton.DIMENSIONS_BOUTONS, MenuPrincipale, 0, "Singleplayer");

            MenuPrincipale.ListeBoutons.Add(boutonSingle);
        }
    }
}
