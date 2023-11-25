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
    enum PlasmaTypes { Fire, Magma, Blue, Green, Violet, _NB_TYPES }

    class BoulePlasma:BouleEnergie
    {
        public SourceFeu EffetPlasma;
        public PlasmaTypes TypePlasma;

        public BoulePlasma(ref Vector2 position, ref Vector2 vitesse, PersonnageArmé propriétaire, PlasmaTypes typePlasma)
        {
            Position = position;
            Vitesse = vitesse;
            Propriétaire = propriétaire;
            TypePlasma = typePlasma;

            Générer();
        }

        public BoulePlasma(Vector2 position, Vector2 vitesse, float rayon, float rayonExplosion, int dégats, PersonnageArmé propriétaire, bool doitExploser)
            : base(position, vitesse, rayon, rayonExplosion, dégats, propriétaire, doitExploser)
        {

        }

        protected void Générer()
        {
            switch (TypePlasma)
            {
                case PlasmaTypes.Fire:
                default:
                    EffetPlasma = new SourceFeu(Color.OrangeRed);
                    EffetPlasma.Direction = Vitesse;
                    Dégats = 15;
                    RayonExplosion = Bloc.DIMENSION_BLOC;
                    Puissance = 5;
                    DoitExploser = true;
                    Vitesse *= 3;

                    break;
                case PlasmaTypes.Magma:
                    EffetPlasma = new SourceFeu(Color.DarkRed);
                    EffetPlasma.AngleOuverture = 90;
                    EffetPlasma.Direction = Vitesse;
                    EffetPlasma.VitesseParticuleX10 = (int)(Vitesse.Length() * 30f);
                    EffetPlasma.NbParticuleParFrame = 3f;

                    Dégats = 10;
                    RayonExplosion = 30;
                    Puissance = 10;
                    DoitExploser = false;
                    Vitesse *= 2.75f;

                    break;
                case PlasmaTypes.Blue:
                    EffetPlasma = new SourceFeu(Color.Turquoise);
                    EffetPlasma.DésactiverFumée();
                    EffetPlasma.AngleOuverture = 30;
                    EffetPlasma.Direction = Vitesse;
                    EffetPlasma.VitesseParticuleX10 = (int)(Vitesse.Length() * 37.5f);
                    
                    Dégats = 30;
                    RayonExplosion = Bloc.DIMENSION_BLOC * 4;
                    Puissance = 6;
                    DoitExploser = true;
                    Vitesse *= 3.75f;

                    break;
                case PlasmaTypes.Green:
                    EffetPlasma = new SourceFeu(Color.Chartreuse);
                    EffetPlasma.Direction = -Vitesse;
                    EffetPlasma.CouleurBaseFumée1 = Color.LightGray;
                    EffetPlasma.VitesseParticuleX10 = (int)(Vitesse.Length() * 30f);
                    
                    Dégats = 40;
                    RayonExplosion = Bloc.DIMENSION_BLOC * 2;
                    Puissance = 9;
                    DoitExploser = true;
                    Vitesse *= 4.50f;

                    break;
                case PlasmaTypes.Violet:
                    EffetPlasma = new SourceFeu(Color.Purple);
                    EffetPlasma.DésactiverFumée();
                    EffetPlasma.AngleOuverture = 60;
                    EffetPlasma.Direction = Vitesse;
                    EffetPlasma.VitesseParticuleX10 = (int)(Vitesse.Length() * 30f);
                    EffetPlasma.NbParticuleParFrame = 3f;
                    EffetPlasma.MinGrosseur = 20;
                    EffetPlasma.MaxGrosseur = 28;

                    Dégats = 50;
                    RayonExplosion = Bloc.DIMENSION_BLOC * 2.5f;
                    Puissance = 10;
                    DoitExploser = true;
                    Vitesse *= 3;

                    break;
            }
        }

        public override void Updater()
        {
            base.Updater();

            //switch (TypePlasma)
            //{
            //    case PlasmaTypes.Fire:
            //    default:

            //        break;
            //    case PlasmaTypes.Magma:

            //        break;
            //    case PlasmaTypes.Blue:

            //        break;
            //    case PlasmaTypes.Green:
            //        Matrix matrice = Matrix.CreateRotationZ(0.05f);
            //        Vector2.Transform(ref EffetPlasma.Direction, ref matrice, out EffetPlasma.Direction);
            //        break;
            //    case PlasmaTypes.Violet:

            //        break;
            //}
        }

        protected override void ÉmettreParticules()
        {
            EffetPlasma.Update(ref Position);
        }

        public override void Terminer()
        {
            EffetPlasma.Détruire();

            base.Terminer();
        }

        protected override void Exploser()
        {
            base.Exploser();

            GestionEffets.CréerFeu(Position, -Vector2.UnitY, 40, 360, 16, 24, ref EffetPlasma.CouleurBase, 30, 100);
            GestionEffets.CréerParticuleDoubleTextureFeu(Position, -Vector2.UnitY, 20, 360, 12, 20, ref EffetPlasma.CouleurBase, 30, 100);
            GestionEffets.AjouterLumiereTempo(new LumièreTemporaire(120, (int)RayonExplosion * 8, ref EffetPlasma.CouleurBase, ref Position));

            if (TypePlasma != PlasmaTypes.Blue && TypePlasma != PlasmaTypes.Violet)
            {
                GestionEffets.CréerFumée(ref Position, ref Vitesse, 50, 360, 28, 40, ref EffetPlasma.CouleurBaseFumée1, ref EffetPlasma.CouleurBaseFumée2, 3, 180, 120);
            }

            CréerCercleExplosion();
        }
        protected void CréerCercleExplosion()
        {
            Vector2 vecteur = Vector2.Zero;
            Vector2 vecteurPerpendiculaire=Vector2.Zero;
            Color couleur;
            int tempsMax;
            ParticuleDoubleTexture particule; 
            float unSurRayon = 1f / RayonExplosion;

            for (int i = 0; i <= 360; i += 15)
            {
                tempsMax = Maths.Random.Next(40, 60);                
                
                vecteur.X = RayonExplosion * (float)Math.Cos(MathHelper.ToRadians(i));
                vecteur.Y = RayonExplosion * (float)Math.Sin(MathHelper.ToRadians(i));
                
                Vector2.Transform(ref vecteur,ref Maths.MatriceRotation90, out vecteurPerpendiculaire);
                Maths.ModifierCouleur(ref EffetPlasma.CouleurBase, 50, out couleur);

                particule = new ParticuleDoubleTexture(tempsMax, tempsMax / 4)
                {
                    Dimensions = new Vector2(20f),
                    Position = this.Position + vecteur,
                    Vitesse = vecteur * -Maths.RandomFloat(0.04f, 0.07f),
                    AtténuationVitesse = 0.94f,

                    Accélération = vecteurPerpendiculaire * 0.001f,
                    Couleur = couleur
                };
                particule.ArrangerSprite();
                GestionEffets.AjouterEffetAdditiveBlend(particule);

                particule = new ParticuleDoubleTexture(tempsMax, tempsMax / 4)
                {
                    Dimensions = new Vector2(20f),
                    Position = this.Position,
                    Vitesse = vecteur * Maths.RandomFloat(0.02f,0.04f),
                    AtténuationVitesse = 0.965f,

                    Accélération = vecteurPerpendiculaire * 0.001f,
                    Couleur = couleur
                };
                particule.ArrangerSprite();
                GestionEffets.AjouterEffetAdditiveBlend(particule);
                //GestionEffets.CréerPoussière(Position + vecteur, vecteur, 2, 120, 5, 8, GestionEffets.CouleursPierres, 3);
                //GestionEffets.CréerPoussière(Position, vecteur, 2, 15, 5, 20, GestionEffets.CouleursPierres, (int)(RayonDAction * 0.15f));
            }
        }
    }
}
