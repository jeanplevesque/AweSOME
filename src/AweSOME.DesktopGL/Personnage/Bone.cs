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
    enum PartiesCorps { Jambe, Bras, Torse, Tête}
    class Bone:PolygonePhysique
    {
        public Personnage Parent;
        public PartiesCorps PartieCorps
        {
            get { return partiesCorps_; }
            set
            {
                partiesCorps_ = value;
                switch (partiesCorps_)
                {
                    case PartiesCorps.Bras:
                        CréerOmbre(30);
                        break;
                    case PartiesCorps.Torse:
                        CréerOmbre(60);
                        break;
                    case PartiesCorps.Jambe:
                        CréerOmbre(50);
                        break;
                    case PartiesCorps.Tête:
                        CréerOmbre(40);
                        break;
                }
            }
        }
        protected PartiesCorps partiesCorps_;
        
        public bool EstBrisé;

        public Joint Joint1;
        public Joint Joint2;

        public float Longueur;
        public float LongueurInitiale;
        public Vector2 VecteurPrincipal
        {
            get { return Joint2.Position - Joint1.Position; }
        }

        public Bone(Joint joint1, Joint joint2, float largeur,Personnage parent,PartiesCorps partieCorps)
            : base(joint1.Position + (joint2.Position - joint1.Position) / 2, new Vector2((joint2.Position - joint1.Position).Length(), largeur))
        {
            Matériel = MatérielPolygone.Organes;
            EstBrisé = false;
            EstFixe = true;
            PartieCorps = partieCorps;
            Parent = parent;

            Joint1 = joint1;
            Joint2 = joint2;
            ArrangerSprite(BanqueContent.Pixel);

            Longueur = LongueurInitiale = Dimensions.X;
        }

        public virtual void UpdateParJoint()
        {
            AnciennePosition = Position;
            AncienAngle = Angle;

            Position = Joint1.Position + (Joint2.Position - Joint1.Position) / 2;
            Angle = (float)Math.Atan2((double)(Joint2.Position.Y - Joint1.Position.Y), (double)(Joint2.Position.X - Joint1.Position.X));

            Vitesse = Position - AnciennePosition;
            VitesseAngulaire = Angle - AncienAngle;
            VitesseAngulaire %= MathHelper.TwoPi;

            Orienter();
        }
        public void PlacerSurPremierJoint()
        {
            Vector2 vecteur = Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(Angle))*LongueurInitiale;
            //Vector2 force = (Joint1.Position + vecteur - Joint2.Position) * Masse * -0.1f;
            Vector2 force = Parent.Vitesse * Masse * -0.1f + GRAVITÉ * 0.8f;
            
            Joint2.Position = Joint1.Position + vecteur;
            UpdateParJoint();

            AddForceAuPoint(force, Joint2.Position);
        }


        public void Briser(Vector2 vitesseBalle, Vector2 positionImpact)
        {
            if (Maths.Random.Next(2) == 0)
            {
                GestionEffets.CréerJetSang(positionImpact, vitesseBalle, 40, 30, 8, 14, 200);
            }
            else //Démembrement
            {
                PolygonePhysique clone = this.Clone();
                clone.TempsRestant = 60 * 30;
                clone.Disparaitra = true;

                clone.AddForceAuPoint(vitesseBalle * clone.Masse * 0.2f, positionImpact);
                clone.EstFixe = false;

                GestionNiveaux.AjouterPolygonePhysique(clone);
            }

            EstBrisé = true;
            Couleur *= 0;
            DétruireOmbre();


        }


        public void Heal(int pointsVie)
        {
            if (EstBrisé)
            {
                EstBrisé = false;
                Couleur = Color.White;
                PartieCorps = partiesCorps_; //Recrée l'ombre
            }

            DégatsAccumulés = Maths.Clamp(DégatsAccumulés - pointsVie, DégatsMax, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Maths.MixerCouleurs(Color.Red, Color.White, MathHelper.Clamp((float)DégatsAccumulés / (float)DégatsMax, 0f, 1f), out Couleur);
            base.Draw(spriteBatch);
        }
    }
}
