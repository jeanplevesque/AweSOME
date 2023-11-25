using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace AweSOME
{
    class Contact
    {
        public Vector2 Position;
        public Vector2 Normal;
        public Vector2 VecteurDirecteur;
        public float Pénétration;

        public PolygonePhysique PolyIncident;
        public PolygonePhysique PolyQuiSeFaitFrapper;

        public float Restitution;// = 0.5f;
        public float Friction = -0.3f;

        public Contact(PolygonePhysique polyIncident, PolygonePhysique polyQuiSeFaitFrapper, Vector2 position, Vector2 normal, Vector2 vecteurDirecteur, float pénétration)
        {
            
            PolyIncident = polyIncident;
            PolyQuiSeFaitFrapper = polyQuiSeFaitFrapper;
            Position = position;
            Normal = normal;
            VecteurDirecteur = vecteurDirecteur;
            Pénétration = pénétration;

            Restitution = (PolyIncident.Restitution + PolyQuiSeFaitFrapper.Restitution)/2;
        }

        public void Résoudre()
        {
            //++PolyIncident.NbCollisions;

            if (PolyIncident is Personnage)
            {
                RésoudreContactPersonnage();
            }
            else
            {
                RésoudreContactPolyLibre();               
            }
        }

        private void RésoudreContactPersonnage()
        {
            Personnage perso = ((Personnage)PolyIncident);


            if (perso.EstEnVie)
            {

                //-----------Comment savoir si le perso est sur le sol-------
                if (Math.Abs(Normal.Y) > Math.Abs(Normal.X))
                {
                    if (Normal.Y > 0)
                    {
                        //Collision contre le sol
                        if (!perso.EstSurSol && perso.Vitesse.Y>=0)
                        {
                            perso.AjusterPosition(-Normal * Pénétration * 0.1f);//On ne fait pas sortir le personnage du sol completement pour que le bool EstSurSol n'oscille pas                           
                            perso.Blesser(PolyQuiSeFaitFrapper,null,Position);
                            
                            perso.Vitesse.Y = 0;
                            perso.EstSurSol = true;                            
                        }
                    }
                    else
                    {
                        //Collision contre un plafond
                        perso.AjusterPosition(-Normal * Pénétration * 0.5f);
                        perso.Vitesse.Y = 0;
                        perso.CollisionPlafond = true;
                    }
                }
                else
                {
                    //on Met les flags de collisions des Murs
                    if (Normal.X > 0) { perso.CollisionMurDroite = true; }
                    else { perso.CollisionMurGauche = true; }

                    //On vérifie que l'obstacle est suffisement haut (on ne cogne pas le sol)
                    if (MathHelper.Distance(perso.Position.Y, Position.Y) < perso.Dimensions.Y * 0.4f)
                    {
                        perso.AjusterPosition(-Normal * Pénétration * 0.5f);
                    }
                }
            }
            else
            {
                //Si le perso est mort, il rebondit quelque peu et s'enfonce dans le sol
                ++perso.NbCollisions;

                perso.AjusterPosition(-Normal * Pénétration * perso.Repositionnement);
                PolyIncident.AddForceAuPoint(CalculerVitesseLocale(PolyIncident) * PolyIncident.Masse * Restitution * -Normal * perso.Repositionnement, Position);

                //perso.Repositionnement *= 0.99f;
            }
        }
        private void RésoudreContactPolyLibre()
        {
            
            if (PolyQuiSeFaitFrapper.EstFixe)
            {
                float vitesseLocal = CalculerVitesseLocale(PolyIncident);

                PolyIncident.Position -= Normal * Pénétration * PolyIncident.Repositionnement;
                PolyIncident.AddForceAuPoint(vitesseLocal * PolyIncident.Masse * Restitution * -Normal, Position);
                PolyIncident.AddForceAuPoint(Maths.ProjectionOrthogonale(CalculerVitesseLocaleVecteur(PolyIncident),VecteurDirecteur) * PolyIncident.Masse * Friction, Position);

                if (vitesseLocal > 5)
                {
                    PolyIncident.Abimer((int)(vitesseLocal*vitesseLocal));
                }

                if (PolyQuiSeFaitFrapper is Bone) // un polygone fixe peut etre un personnage, dans ce cas, on le blesse
                {
                    ++PolyIncident.NbCollisions;

                    Bone bone = ((Bone)PolyQuiSeFaitFrapper);
                    bone.Parent.Blesser(PolyIncident, bone, Position);
                }
            }
            else
            {
                //++PolyQuiSeFaitFrapper.NbCollisions;

                PolyIncident.Position -= Normal * Pénétration * 0.5f;
                PolyQuiSeFaitFrapper.Position -= Normal * Pénétration * -0.5f;


                Vector2 force = (CalculerVitesseLocale(PolyIncident) * PolyIncident.Masse
                               - CalculerVitesseLocale(PolyQuiSeFaitFrapper) * PolyQuiSeFaitFrapper.Masse) * Restitution * -Normal;

                PolyIncident.AddForceAuPoint(force, Position);
                PolyQuiSeFaitFrapper.AddForceAuPoint(-force, Position);

                int dégats = (int)(force.LengthSquared()*0.000001f);
                PolyIncident.Abimer(dégats);
                PolyQuiSeFaitFrapper.Abimer(dégats);
            }
        }
        
        public float CalculerVitesseLocale(PolygonePhysique poly)
        {
            Vector2 vitesse=Vector2.Zero;
            vitesse = Maths.ProduitVectoriel(poly.VitesseAngulaire, Position-poly.Position);
            vitesse += poly.Vitesse;

            return vitesse.Length();
        }
        public Vector2 CalculerVitesseLocaleVecteur(PolygonePhysique poly)
        {
            Vector2 vitesse = Vector2.Zero;
            vitesse = Maths.ProduitVectoriel(poly.VitesseAngulaire, Position - poly.Position);
            vitesse += poly.Vitesse;

            return vitesse;
        }
    }
}
