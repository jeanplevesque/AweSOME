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
using Krypton;
using Krypton.Lights;
using System.IO;

namespace AweSOME
{
    enum TypeFusil { Automatique=0, SemiAuto=1 }
    class Fusil:Arme
    {
        public Directions Direction;
        public TypeFusil Type;
        public int TempsEntreTirs;
        protected int CptTempsTir;

        public int NbBallesMaximum;
        public int NbBallesTotal;
        public int CapacitéChargeur
        {
            get { return capacitéChargeur_; }
            set
            {
                capacitéChargeur_ = value;
                NbBallesMaximum = capacitéChargeur_ * 10;
            }
        }
        private int capacitéChargeur_;

        public int NbBallesDansChargeur;
        public int AngleImprécisionMaxDegrées;
        public float AngleImprécision
        {
            get
            {
                return MathHelper.ToRadians(Maths.Random.Next(AngleImprécisionMaxDegrées) - AngleImprécisionMaxDegrées / 2);
            }
        }
        public float LongueurBalles;
        public Color CouleurBalles;

        public Vector2 DistanceEmbout;
        public Vector2 DistanceEmboutInitiale;
        public Vector2 PositionEmbout { get { return Position + DistanceEmbout; } }
        public Vector2 DistanceFlashLight;
        public Vector2 DistanceFlashLightInitiale;
        public Vector2 PositionFlashLight { get { return Position + DistanceFlashLight; } }
        public Vector2 DistanceLazer;
        public Vector2 DistanceLazerInitiale;
        public Vector2 PositionLazer { get { return Position + DistanceLazer; } }

        public SourceLumière FlashLight;
        public LazerPointer PointeurLazer;

        protected MuzzleFlash Flash;

        public Fusil(Vector2 position, Vector2 dimensions, int dégats, PersonnageArmé propriétaire, TypeFusil type, int tempsEntreTirs)
            : base(position, dimensions, dégats, propriétaire)
        {
            Type = type;
            TempsEntreTirs = tempsEntreTirs;

            EstFixe = true;
            FlashLight = SourceLumière.CréerFlashLight();
            PointeurLazer = new LazerPointer(this);
            Flash = new MuzzleFlash(this);
        }
        public Fusil()
        {
            EstFixe = true;
            FlashLight = SourceLumière.CréerFlashLight();
            PointeurLazer = new LazerPointer(this);
            Flash = new MuzzleFlash(this);
        }

        public override void Update()
        {
            base.Update();

            if (Propriétaire != null)
            {
                Direction= Propriétaire.Direction;
            }
            DistanceEmbout = Vector2.Transform(Maths.MultiplierY(DistanceEmboutInitiale, ((int)Direction)), Matrice);
            DistanceFlashLight = Vector2.Transform(Maths.MultiplierY(DistanceFlashLightInitiale, ((int)Direction)), Matrice);
            DistanceLazer = Vector2.Transform(Maths.MultiplierY(DistanceLazerInitiale, ((int)Direction)), Matrice);


            if (CptTempsTir != 0)
            {
                ++CptTempsTir;
                if (CptTempsTir >= TempsEntreTirs) { CptTempsTir = 0; }
            }
            UpdateLumiere();
            PointeurLazer.Update();
            Flash.Update();
        }
        protected void UpdateLumiere()
        {
            FlashLight.Position = PositionFlashLight;
            FlashLight.Angle = Angle;
        }

        public void AjouterBalles(Fusil fusil)
        {
            NbBallesTotal += fusil.NbBallesTotal + fusil.NbBallesDansChargeur;
            NbBallesTotal = Maths.Clamp(NbBallesTotal + NbBallesDansChargeur, NbBallesMaximum - NbBallesDansChargeur, 0);
        }
        public virtual void Tirer(bool nouveauTir)
        {
            if (nouveauTir || Type == TypeFusil.Automatique)
            {
                if (CptTempsTir == 0 && NbBallesDansChargeur > 0 && !GestionNiveaux.NiveauActif.BlocPrésentNonTunel(Position))
                {
                    --NbBallesDansChargeur;
                    ++CptTempsTir;
                    BalleFusil balle = new BalleFusil(PositionEmbout, Angle + AngleImprécision, Dégats, Puissance, LongueurBalles, CouleurBalles, this, Propriétaire);
                    GestionNiveaux.NiveauActif.AjouterBalleFusil(balle);

                    //--------------Créer un effet de flare-------------
                    CréerEffetsTirs();
                    
                    ++Propriétaire.Stats.NbTirs;
                    
                    //----LoL---
                    //Missile.CréerSeekingMissile(ref Position, Angle, ref Matrice, Propriétaire, PointeurLazer.DernierPolyPointé);
                    //Missile.CréerMissileBasique(ref Position, Angle, ref Matrice, Propriétaire);
                }
            }
        }
        public virtual void CréerEffetsTirs()
        {
            GestionEffets.CréerPoussière(PositionEmbout, DistanceEmbout, 3, 15, 5, 10, GestionEffets.CouleursPierres, 3);

            Flash.Allumer(3);
        }

        public void Recharger()
        {
            if (NbBallesTotal > 0)
            {
                NbBallesTotal -= CapacitéChargeur-NbBallesDansChargeur;
                NbBallesDansChargeur = CapacitéChargeur;
                if (NbBallesTotal < 0)
                {
                    NbBallesDansChargeur += NbBallesTotal;
                }
                NbBallesTotal = Maths.Clamp(NbBallesTotal, 10*CapacitéChargeur, 0);
            }
        }

        public override bool VérifierHorsNiveau()
        {
            return base.VérifierHorsNiveau() ||
                (NbBallesTotal <= 0 && NbCollisions >= NbCollisionsMax && EstLibre) ||
                (GestionNiveaux.NiveauActif.ListeObjetsRamassable.Count > 15 && EstLibre && DernierPropriétaire == null);
        }
        
        public override void SeFaireRamasser(PersonnageArmé persoArmé)
        {
            persoArmé.RamasserFusil(this);
            base.SeFaireRamasser(persoArmé); //Enleve l'objet de la liste d'objets ramassable
        }
        public override void SeFaireLaisser()
        {
            PointeurLazer.Désactiver();
            base.SeFaireLaisser();
        }
        public override void EntréeInventaire()
        {
            base.EntréeInventaire();
            FlashLight.Fermer();
        }
        public override void SortieInventaire()
        {
            base.SortieInventaire();
            FlashLight.Ouvrir();
            PointeurLazer.Activer();
        }
        public override void ForceDelete()
        {
            FlashLight.Supprimer();
            base.ForceDelete();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            PointeurLazer.Draw(spriteBatch);
            Flash.Draw();
        }
    }
}
