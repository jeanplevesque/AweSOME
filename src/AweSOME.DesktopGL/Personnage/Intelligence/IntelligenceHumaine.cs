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
    class IntelligenceHumaine:Intelligence
    {
        Player PersoArmé;

        Keys ToucheRecharge = Keys.R;
        Keys ToucheSaut = Keys.Space;
        Keys ToucheGauche = Keys.A;
        Keys ToucheDroite = Keys.D;
        Keys ToucheSprint = Keys.LeftShift;
        Keys ToucheRamasser = Keys.E;
        Keys ToucheÉchapper = Keys.Q;
        Keys ToucheFlashLight = Keys.F;
        Keys ToucheLazerPointer = Keys.L;

        public IntelligenceHumaine(Player perso)
            :base(perso)
        {
            PersoArmé = perso;
            Caméra.Position = Perso.Position;
        }


        public override void ChangerÉtats()
        { 
            base.ChangerÉtats();
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.NumPad0)) { Perso.Blesser(Perso.VieMax); }//Suicide

            if (GestionIntrants.BoutonSourisGaucheEnfoncé) { PersoArmé.UtiliserItemEnMain(GestionIntrants.ClicGaucheInverse); }
            if (GestionIntrants.ClicMilieu) { PersoArmé.Lancer(); }
            if (GestionIntrants.BoutonSourisDroitEnfoncé) { PersoArmé.UtiliserItemEnMainAlternatif(GestionIntrants.ClicDroitInverse); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(ToucheRecharge)) { PersoArmé.Recharger(); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(ToucheRamasser)) { PersoArmé.RamasserObjetÀProximité(); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(ToucheÉchapper)) { PersoArmé.FaireTomberItem(); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(ToucheFlashLight)) { PersoArmé.ToggleFlashLight(); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(ToucheLazerPointer)) { PersoArmé.ToggleLazerPointer(); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.D1)) { PersoArmé.PrendreItemDeInventaire(0); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.D2)) { PersoArmé.PrendreItemDeInventaire(1); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.D3)) { PersoArmé.PrendreItemDeInventaire(2); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.D4)) { PersoArmé.PrendreItemDeInventaire(3); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.D5)) { PersoArmé.PrendreItemDeInventaire(4); }

            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.W)) { PersoArmé.BoutonFonction1(); }
        }

        protected override void GérerDéplacements()
        {
            if ((GestionIntrants.EstNouvelleToucheEnfoncée(ToucheSaut) && Perso.EstSurSol) ||
                (GestionIntrants.EstToucheEnfoncée(ToucheSaut) && !Perso.EstSurSol))
            {
                Perso.Sauter();
            }
            //if (GestionIntrants.EstToucheEnfoncée(ToucheSaut)) { Perso.Sauter(); }

            if(GestionIntrants.EstToucheEnfoncée(ToucheGauche))
            {
                VitesseXVoulue = -Personnage.VITESSE_X_MAX;
                if (GestionIntrants.EstToucheEnfoncée(ToucheSprint))
                {
                    Perso.Sprint = true;
                    VitesseXVoulue = -Personnage.VITESSE_X_MAX_SPRINT;
                    Perso.ÉtatPrésent = ÉtatsPossibles.Course;
                    //--------------------------------------------------------
                    Color couleur = new Color(6, 3, 0);
                    GestionEffets.CréerCloneLumineux(Perso, 60, ref couleur);
                    //--------------------------------------------------------
                }
                else
                {
                    VitesseXVoulue = -Personnage.VITESSE_X_MAX;
                    Perso.ÉtatPrésent = ÉtatsPossibles.Marche;
                    Perso.Sprint = false;
                }
            }
            else if (GestionIntrants.EstToucheEnfoncée(ToucheDroite))
            {
                VitesseXVoulue = Personnage.VITESSE_X_MAX;
                if (GestionIntrants.EstToucheEnfoncée(ToucheSprint))
                {
                    VitesseXVoulue = Personnage.VITESSE_X_MAX_SPRINT;
                    Perso.ÉtatPrésent = ÉtatsPossibles.Course;
                    Perso.Sprint = true;

                    //--------------------------------------------------------
                    Color couleur = new Color(6, 3, 0);
                    GestionEffets.CréerCloneLumineux(Perso, 60, ref couleur);
                    //--------------------------------------------------------
                }
                else
                {
                    VitesseXVoulue = Personnage.VITESSE_X_MAX;
                    Perso.ÉtatPrésent = ÉtatsPossibles.Marche;
                    Perso.Sprint = false;
                }
            }
            else
            {
                VitesseXVoulue = 0;
                Perso.ÉtatPrésent = ÉtatsPossibles.Immobile;
            }
            Perso.DéplacerEnX(VitesseXVoulue);
        }

        protected override void ChoisirPositionCible()
        {
            PersoArmé.Cible.Position = GestionIntrants.PositionSourisCaméra;

            Vector2 positionCaméraVoulue = PersoArmé.Position + (PersoArmé.Cible.Position - PersoArmé.Position) * 0.5f;
            if (Vector2.DistanceSquared(Caméra.Position, positionCaméraVoulue) > 1024)
            {
                Caméra.Position += (positionCaméraVoulue - Caméra.Position) * 0.025f;
            }
        }
    }
}
