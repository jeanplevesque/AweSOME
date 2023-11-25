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
    class Coupeuse:Droite
    {
        public uint NbCoupesMax;
        //public uint NbCoupes;
        public int Puissance;
        public List<DoubleIntersection> ListeDoubleIntersections = new List<DoubleIntersection>();

        public Coupeuse(Vector2 position, Vector2 direction, float longueur)
            : base(position, position + direction * longueur)
        {
            NbCoupesMax = 1;
        }
        public Coupeuse(Vector2 position, Vector2 direction, float longueur,uint nbCoupesMax,int puissance)
            : base(position, position + direction * longueur)
        {
            NbCoupesMax = nbCoupesMax;
            Puissance = puissance;
        }

        public void Couper(List<PolygonePhysique> listeCorps)
        {
            throw new NotImplementedException("Fonction Désactivée");

            //ListeDoubleIntersections.Clear();
            //DoubleIntersection lesIntersections;
            //foreach (PolygonePhysique p in listeCorps)
            //{
            //    lesIntersections = Intersection.TrouverLesDeuxIntersection(this, p);
            //    if (lesIntersections != null)
            //    {
            //        ListeDoubleIntersections.Add(lesIntersections);
            //    }
            //}
            //if (ListeDoubleIntersections.Count > 0)
            //{
            //    ListeDoubleIntersections.Sort();
            //    for (int i = 0;i<ListeDoubleIntersections.Count&& i < NbCoupesMax; ++i)
            //    {
            //        DoubleIntersection doubleIntersection = ListeDoubleIntersections[i];
            //        if (Puissance < doubleIntersection.PolyMobile.Solidité)
            //        {
            //            return;
            //        }
                    
            //        //doubleIntersection.PolyMobile.CréerDeuxMorceaux(this, doubleIntersection);
            //        //GestionNiveaux.NiveauActif.DétruirePolygonePhysique(doubleIntersection.PolyMobile);
            //    }
            //}
        }
    }
}
