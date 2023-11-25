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
    class PolygoneComplexe:PolygonePhysique
    {
        List<PolygonePhysique> ListePolygones = new List<PolygonePhysique>();
        Vector2[] DistancePolygonesInitiale;
        Vector2[] DistancePolygones;
        float[] AnglesInitiaux;

        public PolygoneComplexe(Vector2 position, int nbPolygones)
        {
            Position = position;

            DistancePolygonesInitiale = new Vector2[nbPolygones];
            DistancePolygones = new Vector2[nbPolygones];
            AnglesInitiaux = new float[nbPolygones];
        }

        public void AjouterPolygone(PolygonePhysique poly, float angleInitiale, Vector2 distancePolyInitiale, int index)
        {
            ListePolygones.Add(poly);
            DistancePolygonesInitiale[index] = distancePolyInitiale;

            Matrix matrice=Matrix.CreateRotationZ(angleInitiale);
            Vector2.Transform(poly.OrientationSommetsInitiales, ref matrice, poly.OrientationSommetsInitiales);
            AnglesInitiaux[index] = angleInitiale;

            poly.ArrangerSprite();
        }
        public void TerminerCréation()
        {
            Rayon = 1;
            float rayon;
            for (int i = 0; i < ListePolygones.Count; ++i)
            {
                Masse += ListePolygones[i].Masse;
                Inertie += ListePolygones[i].Inertie;
                foreach (Sommet s in ListePolygones[i].ListeSommets)
                {
                    rayon=Vector2.Distance(Position,s.Position);
                    if (rayon > Rayon)
                    {
                        Rayon = rayon;
                    }
                }
            }
            InverseInertie = 1f / Inertie;
            GRAVITÉ *= Masse;
        }

        public override void Orienter()
        {
            Matrice = Matrix.CreateRotationZ(Angle);

            Vector2.Transform(DistancePolygonesInitiale, ref Matrice, DistancePolygones);
            for (int i = 0; i < ListePolygones.Count; ++i)
            {
                ListePolygones[i].Position = Position + DistancePolygones[i];
                ListePolygones[i].Orienter(ref Matrice, AnglesInitiaux[i] + Angle);//et on l'envoie aux autres polygones
            }
        }

        public override IntersectionAvecNormale VérifierCollisionDroite(Droite droite)
        {
            List<IntersectionAvecNormale> intersections = new List<IntersectionAvecNormale>();
            IntersectionAvecNormale intersection=null;
            for (int i = 0; i < ListePolygones.Count; ++i)
            {
                intersection=Intersection.TrouverPremièreIntersectionAvecNormale(droite, ListePolygones[i]);
                if (intersection != null)
                {
                    intersection.Corps = this;
                    intersections.Add(intersection);
                }
            }
            if (intersections.Count > 0)
            {
                intersections.Sort();
                intersection = intersections[0];
            }
            return intersection;
        }
        public override Contact VérifierCollisionsPersonnages()
        {
            Contact contact = null;
            if (NbCollisions < NbCollisionsMax)
            {
                for (int i = 0; i < ListePolygones.Count; ++i)
                {
                    contact = ListePolygones[i].VérifierCollisionsPersonnages();
                    if (contact != null)
                    {
                        contact.PolyIncident = this;
                    }
                }
            }
            return contact;
        }
        public override List<Contact> VérifierCollisionsDécors()
        {
            List<Contact> contacts = null;
            for (int i = 0; i < ListePolygones.Count; ++i)
            {
                contacts = ListePolygones[i].VérifierCollisionsDécors();
                for (int a = 0; a < contacts.Count; ++a)
                {
                    if (contacts[a] != null)
                    {
                        contacts[a].PolyIncident = this;
                    }
                }
            }
            return contacts;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            foreach (PolygonePhysique p in ListePolygones)
            {
                p.Draw(spriteBatch);
            }
        }
    }
}
