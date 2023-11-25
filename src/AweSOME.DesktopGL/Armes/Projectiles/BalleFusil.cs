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
    class BalleFusil : Droite
    {
        public float LongueurBalle { get; protected set; }

        public Vector2 Vitesse { get; protected set; }
        public Fusil FusilOrigine { get; protected set; }
        public Personnage Propriétaire { get; protected set; }
        public int Dégats { get; protected set; }
        public int Puissance { get; protected set; }//Sert à la destruction de l'environnement

        protected List<IntersectionAvecNormale> ListeIntersections = new List<IntersectionAvecNormale>();

        public BalleFusil(Vector2 position, float angle, int dégats, int puissance,float longueurBalle, Color couleurBalle,Fusil fusil,Personnage propriétaire)
            : base(position, position + Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(angle)) * longueurBalle)
        {
            LongueurBalle = longueurBalle;
            FusilOrigine = fusil;
            Dégats = dégats;
            Puissance = puissance;
            Propriétaire = propriétaire;
            Couleur = couleurBalle;
            Dimensions.Y = 4;
            Vitesse = (P2 - P1) * 0.5f;

            ArrangerSprite(GestionTexture.GetTexture("Particules/GlowBack"));
        }

        public override void Update()
        {
            VérifierHorsNiveau();

            Sommet1.Position += Vitesse;
            Sommet2.Position += Vitesse;
            base.Update();
            CalculerAngle();
        }

        private void VérifierHorsNiveau()
        {
            if (Position.X < 0
                || Position.Y > 0
                || Position.X > GestionNiveaux.NiveauActif.DimensionsNiveau.X
                || Position.Y < -GestionNiveaux.NiveauActif.DimensionsNiveau.Y)
            {
                GestionNiveaux.NiveauActif.DétruireBalleFusil(this);
            }
        }

        public virtual void ChoisirIntersection()
        {
            //Si nous avons des intersections, nous prenons la plus près du centre de la balle (donc la première visuellement)
            IntersectionAvecNormale intersection = null;
            if (ListeIntersections.Count > 0)
            {
                ListeIntersections.Sort();
                intersection = ListeIntersections[0];
                ListeIntersections.Clear();

                //Créer un impact
                GestionImpactsBalle.AjouterImpact(intersection.Position, intersection.Normale, this, intersection.Corps);
                GestionNiveaux.NiveauActif.DétruireBalleFusil(this);
            }
        }
        
        public virtual void VérifierCollisions()
        {
            VérifierCollisionsBlocs();
            VéfifierCollissionPersonnages();
            VéfifierCollissionPolygones();
        }

        public void VéfifierCollissionPersonnages()
        {
            //CalculerPente_mx_b();

            IntersectionAvecNormale intersection;
            foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            {
                if (p != Propriétaire && Maths.IntersectionRayons(p.Position,p.Rayon,Position,Longueur))
                {
                    foreach (Bone b in p.ListeBones)
                    {
                        if (!b.EstBrisé)
                        {
                            b.CalculerPentes();
                            intersection = Intersection.TrouverPremièreIntersectionAvecNormale(this, b);

                            if (intersection != null)
                            {
                                ListeIntersections.Add(intersection);
                            }
                        }
                    }
                }
            }
        }
        public void VéfifierCollissionPolygones()
        {
            IntersectionAvecNormale intersection;
            foreach (PolygonePhysique p in GestionNiveaux.NiveauActif.ListePolygonesPhysiquesLibres)
            {
                if (Maths.IntersectionRayons(p.Position, p.Rayon, Position, Longueur))
                {
                    intersection = p.VérifierCollisionDroite(this);

                    if (intersection != null)
                    {
                        ListeIntersections.Add(intersection);
                    }
                }
            }
        }
        public void VérifierCollisionsBlocs()
        {          

            Point p1 = Bloc.CoordVersIndex(P1);
            Point p2 = Bloc.CoordVersIndex(P2);

            //bornes inférieures
            int min_i = Math.Min(p1.X, p2.X)-1;
            int min_j = Math.Min(p1.Y, p2.Y)-1;

            //bornes supérieures
            int max_i = Math.Max(p1.X, p2.X)+1;
            int max_j = Math.Max(p1.Y, p2.Y)+1;

            Bloc bloc = null;
            IntersectionAvecNormale intersection = null;

            for (int a = min_i; a < max_i; ++a)
            {
                for (int b = min_j; b < max_j; ++b)
                {
                    bloc = GestionNiveaux.NiveauActif.GetBloc(a, b);
                    if (bloc == null || bloc.EstTunel) { continue; }

                    intersection = Intersection.TrouverPremièreIntersectionAvecNormale(this, bloc);

                    if (intersection != null)
                    {
                        ListeIntersections.Add(intersection);
                    }
                }
            }
            
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(MoteurJeu.SpriteBatchAdditive);
            //spriteBatch.Draw(BanqueContent.Pixel, (P1), null, Couleur, Angle, Vector2.UnitY / 2f, Dimensions, SpriteEffects.None, );
        }

        
    }
}
