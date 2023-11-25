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
    /// <summary>
    /// Définit les 2 points d'intersection entre une droite coupeuse et un polygone mobile
    /// </summary>
    class DoubleIntersection : IComparable
    {
        public Vector2 Position
        {
            get{return Positions[0];}
        }
        public PolygonePhysique PolyMobile;
        public Vector2[] Positions = new Vector2[2];
        public float DistanceSquared;

        public DoubleIntersection(Vector2[] positions, Vector2 positionCoupeuse, PolygonePhysique poly)
        {
            PolyMobile = poly;
            Positions = positions;
            float dist1 = Vector2.DistanceSquared(Positions[0], positionCoupeuse);
            float dist2 = Vector2.DistanceSquared(Positions[1], positionCoupeuse);
            if (dist1 < dist2)
            {
                DistanceSquared = dist1;
            }
            else
            {
                DistanceSquared = dist2;
            }
        }
        public int CompareTo(object obj)
        {
            return DistanceSquared.CompareTo(((DoubleIntersection)obj).DistanceSquared);
        }
    }
    /// <summary>
    /// Hérite de Intersection
    /// Possède comme attribut le vecteur normal à la droite croisée ainsi que la droite croisée
    /// </summary>
    class IntersectionAvecNormale:Intersection
    {
        public Droite DroiteCoisée;
        public Vector2 Normale { get { return DroiteCoisée.VecteurNormal; } }

        public IntersectionAvecNormale(Vector2 position, Vector2 positionParent, PolygonePhysique corps,Droite droiteCroisée)
            :base(position, positionParent, corps)
        {
            Position = position;
            DroiteCoisée = droiteCroisée;
        }

    }
    /// <summary>
    /// Représente le croisement de deux droite
    /// </summary>
    class Intersection:IComparable
    {
        public Vector2 Position;
        public PolygonePhysique Corps;
        public float DistanceSquaredDeOrigineDroite;


        public Intersection(Vector2 position, Vector2 positionParent, PolygonePhysique corps)
        {
            Corps = corps;
            Position = position;
            DistanceSquaredDeOrigineDroite = Vector2.DistanceSquared(position, positionParent);
        }

        public int CompareTo(object obj)
        {
            return DistanceSquaredDeOrigineDroite.CompareTo(((Intersection)obj).DistanceSquaredDeOrigineDroite);
        }

        //------------Classe Statique----------------
        public static Intersection TrouverPremièreIntersection(Droite droiteRayon, PolygonePhysique polygone)
        {
            Intersection[] intersections = new Intersection[2];
            Intersection intersection = null;

            int cpt = 0;
            Vector2 pos;
            foreach (Droite d in polygone.ListeDroites)
            {
                pos = Vector2.Zero;
                if (d.VérifierCollisions(droiteRayon, out pos))
                {
                    intersections[cpt] = new Intersection(pos, droiteRayon.Position, polygone);
                    cpt++;
                }
            }
            if (intersections[0] != null & intersections[1] != null)
            {
                if (intersections[0].DistanceSquaredDeOrigineDroite > intersections[1].DistanceSquaredDeOrigineDroite)
                {
                    intersection = intersections[1];
                }
                else
                {
                    intersection = intersections[0];
                }
            }
            return intersection;
        }
        public static IntersectionAvecNormale TrouverPremièreIntersectionAvecNormale(Droite droiteRayon, PolygonePhysique polygone)
        {
            IntersectionAvecNormale[] intersections = new IntersectionAvecNormale[4];
            IntersectionAvecNormale intersection = null;

            int cpt = 0;
            Vector2 pos;
            foreach (Droite d in polygone.ListeDroites)
            {
                pos = Vector2.Zero;
                if (d.VérifierCollisions(droiteRayon, out pos))
                {
                    intersections[cpt] = new IntersectionAvecNormale(pos, droiteRayon.Position, polygone,d);
                    cpt++;
                }
            }
            if (intersections[0] != null & intersections[1] != null)
            {
                if (intersections[0].DistanceSquaredDeOrigineDroite > intersections[1].DistanceSquaredDeOrigineDroite)
                {
                    intersection = intersections[1];
                }
                else
                {
                    intersection = intersections[0];
                }
            }
            else if (intersections[0] != null)
            {
                intersection = intersections[0];
            }
            return intersection;
        }
        public static DoubleIntersection TrouverLesDeuxIntersection(Coupeuse coupeuse, PolygonePhysique polygone)
        {
            DoubleIntersection intersections;

            int cpt = 0;
            Vector2[] positions = new Vector2[2];

            foreach (Droite d in polygone.ListeDroites)
            {
                if (cpt < 2)
                {
                    Vector2 pos;
                    if (d.VérifierCollisions(coupeuse, out pos))
                    {
                        positions[cpt] = pos;
                        cpt++;
                    }
                }
            }
            intersections = new DoubleIntersection(positions, coupeuse.Position, polygone);

            if (positions[0] != Vector2.Zero & positions[1] != Vector2.Zero)
            {
                return intersections;
            }
            else
            {
                return null;
            }
            
        }
    }

}
