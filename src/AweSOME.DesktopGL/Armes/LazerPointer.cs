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
using Krypton.Lights;
using Krypton;

namespace AweSOME
{
    class LazerPointer:Droite
    {
        public Fusil FusilOrigine { get; protected set; }
        public Personnage Propriétaire { get; set; }
        public bool EstActif { get; set; }
        public bool PeutEtreActif { get; set; }
        public PolygonePhysique DernierPolyPointé;
        public ParticuleDoubleTexture Extrémité;

        protected List<IntersectionAvecNormale> ListeIntersections = new List<IntersectionAvecNormale>();

        private Vector2 orientationOrigine = Vector2.UnitX * 3000;//GestionNiveaux.NiveauActif.DimensionsNiveau.X;
        private Vector2 orientation;

        public LazerPointer(Fusil fusilOrigine)
        {
            FusilOrigine = fusilOrigine;
            Profondeur = FusilOrigine.Profondeur - 0.01f;
            Couleur = Color.Red;
            Extrémité = new ParticuleDoubleTexture(60, 60)
            {
                Couleur=this.Couleur,
                Dimensions=Vector2.One*20
            };
            Dimensions.Y=4;
            Extrémité.ArrangerSprite();
            ArrangerSprite(GestionTexture.GetTexture("Textures/Lazer0"));
            PeutEtreActif = true;
            //Couleur *= 0.5f;
            //Activer();
        }

        public void Activer()
        {
            EstActif = PeutEtreActif;
        }
        public void Désactiver()
        {
            EstActif = false;
        }
        public void ToggleActivation()
        {
            EstActif = !EstActif && PeutEtreActif;
        }

        public override void Update()
        {
            if (EstActif)
            {
                Sommet1.Position = FusilOrigine.PositionLazer;
                Angle = FusilOrigine.Angle;
                Vector2.Transform(ref orientationOrigine, ref FusilOrigine.Matrice, out orientation);
                Sommet2.Position = Sommet1 + orientation;

                base.Update();
                CalculerPente_mx_b();

                VérifierCollisions();
                ChoisirIntersection();

                Dimensions.X = Longueur;
                ArrangerSprite(Image,Dimensions,Vector2.UnitY*0.5f,Profondeur,Couleur);

                Extrémité.Position = P2;
                //Extrémité.Dimensions
            }
        }

        public void ChoisirIntersection()
        {
            //Si nous avons des intersections, nous prenons la plus près du centre de la balle (donc la première visuellement)
            IntersectionAvecNormale intersection = null;
            if (ListeIntersections.Count > 0)
            {
                ListeIntersections.Sort();
                intersection = ListeIntersections[0];
                ListeIntersections.Clear();

                DernierPolyPointé = intersection.Corps;
                //DernierPolyPointé.Couleur = Color.Red;
                //GestionEffets.CréerMagie(DernierPolyPointé.Position, -Vector2.UnitY, 1.21f, 360, 12, 20, ref Couleur, 20, 30);
                P2 = intersection.Position;

                //Longueur = Maths.ApproxDistance(Position, intersection.Position);
                Longueur = Vector2.Distance(Position, intersection.Position);
            }
            else
            {
                Longueur = GestionNiveaux.NiveauActif.DimensionsNiveau.X;
            }
        }
        public virtual void VérifierCollisions()
        {
            VérifierCollisionsBlocs2();
            VéfifierCollissionPersonnages();
            VéfifierCollissionPolygones();
        }

        public void VéfifierCollissionPersonnages()
        {
            //CalculerPente_mx_b();

            IntersectionAvecNormale intersection;
            foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            {
                if (p != Propriétaire && Maths.IntersectionRayons(p.Position, p.Rayon, Position, Longueur))
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
        public void VérifierCollisionsBlocs2()
        {
            Point p1 = Bloc.CoordVersIndex(P1);
            Point p2 = Bloc.CoordVersIndex(P2);

            float max_x = Math.Abs(P2.X - P1.X);

            Bloc bloc = null;
            IntersectionAvecNormale intersection = null;

            Vector2 position;
            Point index;
            float delta = MathHelper.Clamp(max_x * 0.01f, 0.1f, Bloc.DIMENSION_BLOC * 1);
            float signe = 1;
            if (P1.X > P2.X) { signe *= -1; }

            //float distanceCarréeMin = Longueur * Longueur;
            //float distanceCarrée = 0;

            for (float x = 0; x <= max_x; x += delta)
            {
                position.X = P1.X + x * signe;
                position.Y = CalculerY(position.X);
                index = Bloc.CoordVersIndex(position);

                //for (int j = -1; j < 3; ++j)
                {
                    bloc = GestionNiveaux.NiveauActif.GetBloc(index.X, index.Y + 0);
                    if (bloc == null || bloc.EstTunel) { continue; }

                    //distanceCarrée = Vector2.DistanceSquared(P1, bloc.Position);

                    //if (distanceCarrée <= distanceCarréeMin)
                    {
                        //distanceCarréeMin = distanceCarrée;

                        intersection = Intersection.TrouverPremièreIntersectionAvecNormale(this, bloc);

                        if (intersection != null)
                        {
                            ListeIntersections.Add(intersection);
                        }
                    }
                }
            }
        }
        public void VérifierCollisionsBlocs()
        {

            Point p1 = Bloc.CoordVersIndex(P1);
            Point p2 = Bloc.CoordVersIndex(P2);

            //bornes inférieures
            int min_i = Math.Min(p1.X, p2.X) - 1;
            int min_j = Math.Min(p1.Y, p2.Y) - 1;

            //bornes supérieures
            int max_i = Math.Max(p1.X, p2.X) + 1;
            int max_j = Math.Max(p1.Y, p2.Y) + 1;

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

        //CECI EST UN LAZER POINTER
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (EstActif)
            {
                base.Draw(MoteurJeu.SpriteBatchAdditive);//On le dessine en additive
                Extrémité.Draw(MoteurJeu.SpriteBatchAdditive);
            }
        }
             
    }
}
