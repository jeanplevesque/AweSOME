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
    class SegmentÉclaire:Droite
    {
        public float Alpha = 1f;
        public float AlphaOrigine=1f;
        public Glow Brillance;

        //public int IndexDansListe;
        public int Index;
        public int IndexDivision;

        public SegmentÉclaire(Vector2 p1, Vector2 p2, ref Color couleurGlow)
            :base(p1,p2)
        {
            Couleur = Color.White;
            Dimensions.Y = 2;
            Brillance = new Glow(CalculerPointMilieu(), ref couleurGlow, this);
        }
        public SegmentÉclaire(Vector2 p1, Vector2 p2, float alphaOrigine, int index, int indexDivision, ref Color couleurGlow)//, int indexDansListe )
            : base(p1, p2)
        {
            //IndexDansListe = indexDansListe;
            Index = index;
            IndexDivision = indexDivision;

            AlphaOrigine = alphaOrigine;
            Alpha = AlphaOrigine;
            Couleur = Color.White;
            Dimensions.Y = 2;
            Brillance = new Glow(CalculerPointMilieu(), ref couleurGlow, this);

            ArrangerSprite(GestionTexture.GetTexture("Textures/Lazer0"));
        }

        public bool VérifierCollisionsBlocs()
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
            //IntersectionAvecNormale intersection = null;

            for (int a = min_i; a < max_i; ++a)
            {
                for (int b = min_j; b < max_j; ++b)
                {
                    bloc = GestionNiveaux.NiveauActif.GetBloc(a, b);
                    if (bloc == null || bloc.EstTunel) { continue; }

                    //intersection = Intersection.TrouverPremièreIntersectionAvecNormale(this, bloc);

                    //if (intersection != null)
                    //{
                    //    ListeIntersections.Add(intersection);
                    //}
                    if (bloc.EstÀIntérieur(P1) || bloc.EstÀIntérieur(P2))
                    {
                        return true;
                    }
                }
            }
            return false;

        }

        //public bool VérifierCollisionsPersonnages(PersonnageArmé propriétaire, out Bone boneTouché)
        //{
        //    bool collision = false;
        //    boneTouché = null;

        //    foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
        //    {
        //        if (p != propriétaire && Maths.IntersectionRayons(p.Position, p.Rayon, Position, Longueur))
        //        {
        //            foreach (Bone b in p.ListeBones)
        //            {
        //                if (!b.EstBrisé)
        //                {
        //                    //b.CalculerPentes();

        //                    if (b.EstÀIntérieur(P1) || b.EstÀIntérieur(P2))
        //                    {
        //                        collision = true;

        //                        boneTouché = b;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return collision;
        //}

        public override void Update()
        {
            base.Update();
            Brillance.Update(CalculerPointMilieu(), Alpha);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, null, Couleur * Alpha, Angle, OrigineSprite, Scale, SpriteEffect, Profondeur);
            Brillance.Draw(MoteurJeu.SpriteBatchAdditive);

            //spriteBatch.DrawString(BanqueContent.GetFont("Arial"), Index.ToString() + " - " + IndexDivision.ToString(), Position, Couleur);
        }


        public void UpdateAlpha(float alpha)
        {
            Alpha = AlphaOrigine * alpha;
            Brillance.UpdateAlphaOnly(Alpha);
        }
    }
}
