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
    class Zombie:Personnage
    {
       

        protected Animation AnimationSpawn;
        protected Animation AnimationBrasZombie;
        //protected Animation AnimationAttaqueBrasZombie;

        //SourceFeu FlamesMouahahah;

        public override bool EstSurSol
        {
            get { return estSurSol_ && TuileActive != null && !TuileActive.VoisinPlusBas; }
            set { estSurSol_ = value; }
        }

        public Zombie(Vector2 position, float grosseur)
            : base(position, grosseur)
        {
            Cerveau = new IA(this);
            ChoisirAnimation();

            //Couleur = Color.Red;
            //this.GRAVITÉ *= 1.5f;

            GénérerRandomZombie();


            ArrangerOrdreAffichage(0.6f);
            Spawn();
            //FlamesMouahahah = new SourceFeu(GestionEffets.CouleursFeu[Maths.Random.Next(GestionEffets.CouleursFeu.Count)]);
            //FlamesMouahahah = new SourceFeu(Color.OrangeRed);
            //FlamesMouahahah.DésactiverFumée();
        }

        public void Spawn()
        {
            Spawning = true;
            AnimationSpawn.Reset();
            AnimationSpawn.PlacerSurFrame(ListeJoints.ToArray(), Position, Grosseur, 0);
            UpdateBones();
            Cible.Position = GestionNiveaux.NiveauActif.Joueur.Position;
            Cerveau.ChangerÉtats();
        }
        public override void Update()
        {
            base.Update();

            if (EstEnVie)
            {
                UpdateSpawn();
            }
            //FlamesMouahahah.Update(ref Tête.Position);
            //GestionEffets.CréerFeu(Tête.Position, -Vector2.UnitY, 3, 200, 12, 20, GestionEffets.CouleursFeu, 30, 40);
        }

        private void UpdateSpawn()
        {
            if (Spawning)
            {
                if (AnimationSpawn.EstTerminée)
                {
                    Spawning = false;
                }
                else
                {
                    GestionEffets.CréerParticulesLourdes(Position + Vector2.UnitY * Dimensions.Y / 2, Vector2.UnitY * -1, 0.05178f, 30, 12, 24, "Particules/ParticuleTerre");
                    GestionEffets.CréerPoussière(Position + Vector2.UnitY * Dimensions.Y / 2, Vector2.UnitY * -1, 0.05015f, 90, 5, 20, GestionEffets.CouleursTerres,3);
                }
            }
        }
        protected override void DonnerCoupsBras()
        {
            Personnage joueur = GestionNiveaux.NiveauActif.Joueur;

            if (!BrasDroite.EstBrisé && BrasDroite.IntersectionPersonnage(joueur))
            {
                joueur.Blesser(ForceMelee);
                GestionEffets.CréerJetSang(BrasDroite.Position, MainGauche.Vitesse, 3, 90, 8, 16, 50);
            }
            if (!BrasGauche.EstBrisé && BrasGauche.IntersectionPersonnage(joueur))
            {
                joueur.Blesser(ForceMelee);
                GestionEffets.CréerJetSang(BrasGauche.Position, MainGauche.Vitesse, 3, 90, 8, 16, 50);
            }


        }
        public override void VérifierPossibilitésDéplacement()
        {
            base.VérifierPossibilitésDéplacement();
            PeutBougerEnX = true;//Les zombies rampent


        }
        protected override void Mourrir()
        {
            //FlamesMouahahah.Détruire();
            --GestionRounds.RoundPrésent.NbZombiesEnVies;
            if (Spawning)
            {
                CréerExplosionDeSang();
                DétruireCorps();
            }
            base.Mourrir();
        }

        protected override void Animer()
        {
            if (Spawning)
            {
                AnimationSpawn.Animer(this, 0.03f, AnimationSens.Normal);
            }
            else
            {
                base.Animer();
                if (PeutSauter)
                {
                    AnimationBrasZombie.Animer(this, 0.01f, AnimationSens.Normal);
                }
            }
        }
        public override void ChangerAnimationPourMarcher()
        {
            AnimationMarchePieds = Animation.CréerÀPartirDeBody(AnimationMarche, AnimationTypes.Pieds);

            AnimationMelee = GestionAnimation.Load("Melee");
        }
        protected void ChoisirAnimation()
        {
            AnimationSpawn = GestionAnimation.Load("Spawn");
            AnimationMarche = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Marche"),AnimationTypes.Pieds);
            AnimationBrasZombie=Animation.CréerÀPartirDeBody(GestionAnimation.Load("Zombie"+Maths.Random.Next(2)), AnimationTypes.Bras);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            ((IA)Cerveau).Draw();
        }
    }
}
