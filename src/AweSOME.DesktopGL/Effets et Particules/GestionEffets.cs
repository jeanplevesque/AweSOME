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
using Krypton;
using Krypton.Lights;

namespace AweSOME
{
    static class GestionEffets
    {
        public static int NbParticules { get { return ListeIEffets.Count + ListeIEffetsAdditiveBlend.Count; } }

        static LinkedList<IEffet> ListeIEffetsAdditiveBlend = new LinkedList<IEffet>();
        static LinkedList<IEffet> ListeIEffetsHUD = new LinkedList<IEffet>();
        
        static LinkedList<LumièreTemporaire> ListeLumièreTempo = new LinkedList<LumièreTemporaire>();
        static LinkedList<LumièreTemporaire> ListeLumièreTempoÀDétruire = new LinkedList<LumièreTemporaire>();

        static LinkedList<IEffet> ListeIEffets = new LinkedList<IEffet>();
        static LinkedList<IEffet> ListeIEffetsÀDétruire = new LinkedList<IEffet>();      
        
        static LinkedList<IEffet> ListeIEffetsÀAjouterAdditive = new LinkedList<IEffet>();
        static LinkedList<IEffet> ListeIEffetsÀAjouter = new LinkedList<IEffet>();
        static LinkedList<IEffet> ListeIEffetsÀAjouterHUD = new LinkedList<IEffet>();
        
        

        public static List<Color> CouleursTerres = new List<Color>();
        public static List<Color> CouleursSang = new List<Color>();
        public static List<Color> CouleursPierres = new List<Color>();
        public static List<Color> CouleursFeu = new List<Color>();
        

        static Matrix Matrice;
        static float cptFloat;

        public static Vector2 Vent;

        static GestionEffets()
        {
            Vent = new Vector2(0.04f, 0);

            CouleursTerres.Add(new Color(159, 109, 31));
            CouleursTerres.Add(new Color(213, 185, 151));
            CouleursTerres.Add(new Color(184, 152, 124));
            CouleursTerres.Add(new Color(144, 108, 52));
            CouleursTerres.Add(new Color(240, 214, 176));

            CouleursTerres.Add(new Color(162, 118, 82));
            CouleursTerres.Add(new Color(192, 141, 96));
            CouleursTerres.Add(new Color(212, 184, 160));
            CouleursTerres.Add(new Color(201, 153, 125));
            CouleursTerres.Add(new Color(184, 123, 91));

            CouleursSang.Add(new Color(111, 13, 0));
            CouleursSang.Add(new Color(136, 16, 0));
            CouleursSang.Add(new Color(153, 11, 2));
            CouleursSang.Add(new Color(153, 0, 17));
            CouleursSang.Add(new Color(153, 0, 0));

            CouleursPierres.Add(new Color(255, 255, 255));
            CouleursPierres.Add(new Color(200, 203, 214));
            CouleursPierres.Add(new Color(206, 206, 210));
            CouleursPierres.Add(new Color(230, 234, 246));
            CouleursPierres.Add(new Color(230, 231, 236));

            CouleursFeu.Add(new Color(208, 80, 17));
            CouleursFeu.Add(new Color(234, 160, 13));
            CouleursFeu.Add(new Color(240, 96, 0));
            CouleursFeu.Add(new Color(240, 153, 0));
            CouleursFeu.Add(new Color(208, 174, 17));
            CouleursFeu.Add(new Color(208, 117, 17));
            CouleursFeu.Add(new Color(208, 140, 17));
            CouleursFeu.Add(new Color(208, 100, 17));


        }
        public static void AjouterLumiereTempo(LumièreTemporaire lumiere)
        {
            ListeLumièreTempo.AddLast(lumiere);
        }
        public static void DétruireLumiereTempo(LumièreTemporaire lumiere)
        {
            ListeLumièreTempoÀDétruire.AddLast(lumiere);
        }
        public static void AjouterEffetAdditiveBlend(IEffet effet)
        {
            ListeIEffetsÀAjouterAdditive.AddLast(effet);
        }
        public static void AjouterEffetHUD(IEffet effet)
        {
            ListeIEffetsÀAjouterHUD.AddLast(effet);
        }
        public static void AjouterEffet(IEffet effet)
        {
            ListeIEffetsÀAjouter.AddLast(effet);
        }
        public static void DétruireEffet(IEffet effet)
        {
            ListeIEffetsÀDétruire.AddLast(effet);
        }
        private static void Nettoyer()
        {
            foreach (LumièreTemporaire l in ListeLumièreTempoÀDétruire)
            {
                ListeLumièreTempo.Remove(l);
            }
            ListeLumièreTempoÀDétruire.Clear();

            foreach (IEffet p in ListeIEffetsÀDétruire)
            {
                ListeIEffets.Remove(p);
                ListeIEffetsAdditiveBlend.Remove(p);
                ListeIEffetsHUD.Remove(p);
            }
            ListeIEffetsÀDétruire.Clear();

            foreach (IEffet p in ListeIEffetsÀAjouter)
            {
                ListeIEffets.AddLast(p);
            }
            ListeIEffetsÀAjouter.Clear();

            foreach (IEffet p in ListeIEffetsÀAjouterAdditive)
            {
                ListeIEffetsAdditiveBlend.AddLast(p);
            }
            ListeIEffetsÀAjouterAdditive.Clear();

            foreach (IEffet p in ListeIEffetsÀAjouterHUD)
            {
                ListeIEffetsHUD.AddLast(p);
            }
            ListeIEffetsÀAjouterHUD.Clear();
        }


        public static void CréerOrganes(Vector2 position, Vector2 direction)
        {
            CréerParticulesLourdes(position, direction, 3, 120, 24, 48, "Particules/Organe");
        }
        public static void CréerSang(Vector2 position, Vector2 direction)
        {
            CréerPetitesGoutesSang(position, direction, 6, 40, 6, 12, CouleursSang);
            CréerJetSang(position, direction, 8, 60, 8, 14,50);
        }
        public static void CréerPoussièreEtTerre(Vector2 position, Vector2 direction)
        {
            CréerPoussière(position, direction, 6, 90, 5, 20,CouleursTerres,2);
            CréerParticulesLourdes(position, direction, 1, 120, 12, 24, "Particules/ParticuleTerre");
        }
        public static void CréerPoussièreDeTerre(Vector2 position, Vector2 direction)
        {
            CréerPoussière(position, direction, 6, 90, 5, 20, CouleursTerres,2);
        }
        public static void CréerPoussièreDePierre(Vector2 position, Vector2 direction)
        {
            CréerPoussière(position, direction, 6, 90, 5, 20, CouleursPierres,2);
        }
        public static void CréerPoussièreEtPierres(Vector2 position, Vector2 direction)
        {
            CréerPoussière(position, direction, 6, 90, 5, 20, CouleursPierres,2);
            CréerParticulesLourdes(position, direction, 1, 120, 12, 24, "Particules/ParticulePierre");
        }
        public static void CréerPoussièreAuSol(Vector2 position, MatérielPolygone matériel, int force)
        {
            switch (matériel)
            {
                case MatérielPolygone.Terre:
                    CréerPoussière(position, Vector2.UnitX, 2 * force, 30, 5, 20, CouleursTerres,force);
                    CréerPoussière(position, -Vector2.UnitX, 2 * force, 30, 5, 20, CouleursTerres,force);
                    break;
                default:
                    CréerPoussière(position, Vector2.UnitX, 2 * force, 30, 5, 20, CouleursPierres,force);
                    CréerPoussière(position, -Vector2.UnitX, 2 * force, 30, 5, 20, CouleursPierres, force);
                    break;
            }
        }
        public static void CréerÉtincellesBleues(Vector2 position, Vector2 direction)
        {
            CréerÉtincelles(position, direction, 4, 90, 20, 25, "ÉtincelleBleue", 50);
        }
        public static void CréerÉtincellesJaunes(Vector2 position, Vector2 direction)
        {
            CréerÉtincelles(position, direction, 4, 90, 20, 25, "ÉtincelleJaune", 50);
        }
        
        public static void CréerFumée(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur, int force)
        {
            CréerFumée(ref position, ref direction, nbParticules, angleOuvertureDegrees, grosseurMin, grosseurMax, ref couleur, force, 180, 120);
        }
        public static void CréerFumée(ref Vector2 position, ref Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur, int force, int duréeMax, int duréeFin)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;
                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new Particule(duréeMax, duréeFin)
                    {
                        Position = position,
                        Dimensions = Vector2.One * grosseur,

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * force,
                        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(20) - 10f),
                        Accélération = Vent + new Vector2(0.0f, -0.02f),

                        AtténuationVitesse = Maths.Random.Next(95, 99) * 0.01f,
                        AtténuationVitesseRotation = 0.99f,

                        MutiplicateurDimensions = 1.012f,
                        //AtténuationMutiplicateurDimensions = 0.9f,
                        AffecterParVent=true,
                        Couleur = couleur


                    };

                    //p.Profondeur += ListeParticules.Count * 0.01f;
                    p.ArrangerSprite(GestionTexture.GetTexture("Particules/Poussiere" + Maths.Random.Next(5)));

                    AjouterEffet(p);

                }
            }
        }
        public static void CréerFumée(ref Vector2 position, ref Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur1, ref Color couleur2, int force, int duréeMax, int duréeFin)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;
                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new Particule(duréeMax, duréeFin)
                    {
                        Position = position,
                        Dimensions = Vector2.One * grosseur,

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * force,
                        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(20) - 10f),
                        Accélération = Vent + new Vector2(0.0f, -0.02f),

                        AtténuationVitesse = Maths.Random.Next(95, 99) * 0.01f,
                        AtténuationVitesseRotation = 0.99f,

                        MutiplicateurDimensions = 1.012f,
                        //AtténuationMutiplicateurDimensions = 0.9f,
                        AffecterParVent = true,
                        //Couleur = Maths.MixerCouleurs(ref cou


                    };

                    //p.Profondeur += ListeParticules.Count * 0.01f;
                    Maths.MixerCouleurs(ref couleur1, ref couleur2, Maths.Random.Next(255) / 255f, out p.Couleur);
                    p.ArrangerSprite(GestionTexture.GetTexture("Particules/Poussiere" + Maths.Random.Next(5)));

                    AjouterEffet(p);

                }
            }
        }
        public static void CréerTourbillonPoussière(Vector2 position, int nbParticules, int grosseurMin, int grosseurMax, List<Color> couleurs, int force, int rayon)
        {            
            float deltaAngle;
            int grosseur;

            for (int i = 0; i < nbParticules; ++i)
            {               
                grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                deltaAngle = MathHelper.ToRadians(Maths.Random.Next(360));

                Matrice = Matrix.CreateRotationZ(deltaAngle);

                Particule p = new Particule(180, 120)
                {
                    Position = position + Vector2.Transform(Vector2.UnitX * Maths.Random.Next(rayon), Matrice),
                    Dimensions = Vector2.One * grosseur,

                    Vitesse = Vector2.Transform(Vector2.UnitY, Matrice) * (1 + force),
                    VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(20) - 10f),
                    Accélération = Vector2.Transform(-Vector2.UnitX, Matrice) * 0.2f,

                    AtténuationVitesse = Maths.Random.Next(90, 99) * 0.01f,
                    AtténuationVitesseRotation = 0.99f,

                    MutiplicateurDimensions = 0.98f,
                    AtténuationMutiplicateurDimensions = 0.99f,
                    ClampAtténuationScale = false,

                    Couleur = couleurs[Maths.Random.Next(couleurs.Count)]
                };

                //p.Profondeur += ListeParticules.Count * 0.01f;
                p.ArrangerSprite(GestionTexture.GetTexture("Particules/Poussiere" + Maths.Random.Next(5)));

                AjouterEffet(p);
                
            }
        }
        public static void CréerPoussière(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, List<Color> couleurs, int force)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;
                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new Particule(180, 120)
                    {
                        Position = position,
                        Dimensions = Vector2.One * grosseur,

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * (3 + force),
                        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(20) - 10f),
                        Accélération = Vector2.UnitY * 0.02f,

                        AtténuationVitesse = Maths.Random.Next(50, 93) * 0.01f,
                        AtténuationVitesseRotation = 0.95f,

                        MutiplicateurDimensions = 1.5f,
                        AtténuationMutiplicateurDimensions = 0.9f,

                        Couleur = couleurs[Maths.Random.Next(couleurs.Count)]


                    };

                    //p.Profondeur += ListeParticules.Count * 0.01f;
                    p.ArrangerSprite(GestionTexture.GetTexture("Particules/Poussiere" + Maths.Random.Next(5)));

                    AjouterEffet(p);

                }
            }
        }
        public static void CréerParticulesLourdes(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, string nomFichier)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;
                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new Particule(120, 30)
                    {
                        Position = position,
                        Dimensions = Vector2.One * grosseur,

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * 3,
                        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(20) - 10f),
                        Accélération = Vector2.UnitY * 0.2f,

                        AtténuationVitesseRotation = 0.99f,
                    };

                    //p.Profondeur -= ListeParticules.Count * 0.01f;
                    p.ArrangerSprite(GestionTexture.GetTexture(nomFichier + Maths.Random.Next(5)));

                    AjouterEffet(p);

                }
            }
        }
        public static void CréerPetitesGoutesSang(Vector2 position, Vector2 direction, int nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, List<Color> couleurs)
        {
            float angleNormal = Maths.CalculerAngleDunVecteur(direction);
            float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

            float deltaAngle;
            int grosseur;

            for (int i = 0; i < nbParticules; ++i)
            {
                grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                Particule p = new Particule(40, 30)
                {
                    Position = position,
                    Dimensions = Vector2.One * grosseur,

                    Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(3,7),
                    //VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(20) - 10f),
                    Accélération = Vector2.UnitY * 0.02f,

                    AtténuationVitesse = Maths.Random.Next(50, 93) * 0.01f,
                    //AtténuationVitesseRotation = 0.95f,

                    MutiplicateurDimensions = 1.5f,
                    AtténuationMutiplicateurDimensions = 0.9f,

                    Couleur = couleurs[Maths.Random.Next(couleurs.Count)]

                };

                //p.Profondeur += ListeParticules.Count * 0.01f;
                p.ArrangerSprite(GestionTexture.GetTexture("Particules/Poussiere" + Maths.Random.Next(5)));

                AjouterEffet(p);

            }
        }
        public static void CréerJetSang(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, int forceJetX10)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;

                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new Particule(120, 30)
                    {
                        Position = position,
                        Dimensions = Vector2.One * grosseur,

                        Vitesse = Vector2.Transform(Vector2.UnitX * ((float)(grosseurMax - grosseur) / (grosseurMax - grosseurMin)), Matrice) * Maths.Random.Next(10, forceJetX10) / 10f,
                        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                        Accélération = Vector2.UnitY * 0.0025f * grosseur * grosseur,

                        AtténuationVitesse = Maths.Random.Next(97, 99) * 0.01f,
                        AtténuationVitesseRotation = 0.99f,

                        //MutiplicateurDimensions = 1.5f,
                        //AtténuationMutiplicateurDimensions = 0.9f,

                        //Couleur = couleurs[Maths.Random.Next(couleurs.Count)]

                    };

                    //p.Profondeur -= ListeParticules.Count * 0.0001f;
                    p.ArrangerSprite(GestionTexture.GetTexture("Particules/Sang" + Maths.Random.Next(5)));

                    AjouterEffet(p);

                }
            }
        }
        public static void CréerÉtincelles(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax,string nomFichier, int forceJet)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;

                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new Particule(Maths.Random.Next(10, 20), 4)
                    {
                        Angle=angleNormal+deltaAngle,
                        AngleÉgalDirection=true,

                        Position = position,
                        Dimensions = new Vector2(grosseur,3),

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(25, forceJet) / 10f,
                        //VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                        Accélération = Vector2.UnitY * 0.2f,

                        //AtténuationVitesse = Maths.Random.Next(97, 99) * 0.01f,
                        //AtténuationVitesseRotation = 0.99f,

                        //MutiplicateurDimensions = 1.5f,
                        //AtténuationMutiplicateurDimensions = 0.9f,

                        //Couleur = couleurs[Maths.Random.Next(couleurs.Count)]

                    };

                    //p.Profondeur -= ListeParticules.Count * 0.0001f;
                    p.ArrangerSprite(GestionTexture.GetTexture("Particules/" + nomFichier));

                    AjouterEffetAdditiveBlend(p);

                }
            }
        }
        public static void CréerÉtincellesDoubleTexture(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur, int vitesseX10min25)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat -= (int)cptFloat;

                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new ParticuleDoubleTexture(Maths.Random.Next(16, 32), 8)
                    {
                        Angle = angleNormal + deltaAngle,
                        AngleÉgalDirection = true,

                        Position = position,
                        Dimensions = new Vector2(grosseur, 7),

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(25, vitesseX10min25) / 10f,
                        //VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                        Accélération = Vector2.UnitY * 0.2f,

                        //AtténuationVitesse = Maths.Random.Next(97, 99) * 0.01f,
                        //AtténuationVitesseRotation = 0.99f,

                        //MutiplicateurDimensions = 1.5f,
                        //AtténuationMutiplicateurDimensions = 0.9f,

                        Couleur = couleur

                    };

                    //p.Profondeur -= ListeParticules.Count * 0.0001f;
                    p.ArrangerSprite();

                    AjouterEffetAdditiveBlend(p);

                }
            }
        }

        public static void CréerFeu(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur, int vitesseMaxX10, int duréeMax)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;

                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new Particule(Maths.Random.Next(duréeMax / 2, duréeMax), duréeMax / 4)
                    {
                        //Angle = angleNormal + deltaAngle,
                        //AngleÉgalDirection = true,

                        Position = position,
                        Dimensions = new Vector2(grosseur, grosseur),

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(9, vitesseMaxX10) / 10f,
                        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                        Accélération = Vector2.UnitY * -0.04f,

                        AtténuationVitesse = Maths.Random.Next(95, 99) * 0.01f,
                        AtténuationVitesseRotation = 0.99f,

                        MutiplicateurDimensions = 1.015f,
                        //AtténuationMutiplicateurDimensions = 0.9f,
                        AffecterParVent=true,
                        Couleur = couleur

                    };
                    p.Accélération.X -= p.Vitesse.X * 0.0333f;
                    p.Vitesse.X *= 1.5f;
                    p.Profondeur -=0.01f;
                    p.ArrangerSprite(GestionTexture.GetTexture("Particules/Feu" + Maths.Random.Next(5).ToString()));

                    AjouterEffetAdditiveBlend(p);

                }
            }
        }
        public static void CréerParticuleDoubleTextureFeu(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur, int vitesseMaxX10, int duréeMax)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat -= (int)cptFloat;

                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new ParticuleDoubleTexture(Maths.Random.Next(duréeMax / 2, duréeMax), duréeMax / 4)
                    {
                        //Angle = angleNormal + deltaAngle,
                        //AngleÉgalDirection = true,

                        Position = position,
                        Dimensions = new Vector2(grosseur, grosseur),

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(9, vitesseMaxX10) / 10f,
                        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                        Accélération = Vector2.UnitY * -0.04f,

                        AtténuationVitesse = Maths.Random.Next(95, 99) * 0.01f,
                        AtténuationVitesseRotation = 0.99f,

                        MutiplicateurDimensions = 1.015f,
                        //AtténuationMutiplicateurDimensions = 0.9f,
                        AffecterParVent = true,
                        Couleur = couleur

                    };
                    p.Accélération.X -= p.Vitesse.X * 0.0333f;
                    p.Vitesse.X *= 1.5f;
                    p.Profondeur -= 0.01f;
                    p.ArrangerSprite();

                    AjouterEffetAdditiveBlend(p);

                }
            }
        }
        public static void CréerFeu(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, List<Color> couleurs, int vitesseMaxX10, int duréeMax)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;

                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    Particule p = new Particule(Maths.Random.Next(duréeMax / 2, duréeMax), duréeMax / 4)
                    {
                        //Angle = angleNormal + deltaAngle,
                        //AngleÉgalDirection = true,

                        Position = position,
                        Dimensions = new Vector2(grosseur, grosseur),

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(9, vitesseMaxX10) / 10f,
                        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                        Accélération = Vector2.UnitY * -0.03f,

                        AtténuationVitesse = Maths.Random.Next(95, 99) * 0.01f,
                        AtténuationVitesseRotation = 0.99f,

                        MutiplicateurDimensions = 1.01f,
                        //AtténuationMutiplicateurDimensions = 0.9f,
                        AffecterParVent = true,
                        Couleur = couleurs[Maths.Random.Next(couleurs.Count)]

                    };
                    p.Accélération.X -= p.Vitesse.X * 0.0333f;
                    p.Vitesse.X *= 1.5f;
                    p.Profondeur -= 0.01f;
                    p.ArrangerSprite(GestionTexture.GetTexture("Particules/Feu" + Maths.Random.Next(5).ToString()));

                    AjouterEffetAdditiveBlend(p);

                }
            }
        }
        public static void CréerMagie(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur, int vitesseMaxX10, int duréeMax)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat-=(int)cptFloat;

                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    ParticuleDoubleTexture p = new ParticuleDoubleTexture(Maths.Random.Next(duréeMax / 2, duréeMax), duréeMax/4)
                    {
                        //Angle = angleNormal + deltaAngle,
                        //AngleÉgalDirection = true,

                        Position = position,
                        Dimensions = new Vector2(grosseur, grosseur),

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(9, vitesseMaxX10) / 10f,
                        //VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                        Accélération = Vector2.UnitY * 0.05f,

                        AtténuationVitesse = Maths.Random.Next(95, 99) * 0.01f,
                        //AtténuationVitesseRotation = 0.99f,

                        //MutiplicateurDimensions = 1.5f,
                        //AtténuationMutiplicateurDimensions = 0.9f,

                        Couleur = couleur

                    };

                    //p.Profondeur -= ListeParticules.Count * 0.0001f;
                    p.ArrangerSprite();

                    AjouterEffetAdditiveBlend(p);

                }
            }
        }
        public static void CréerCroixHeal(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur, int vitesseMaxX10, int duréeMax)
        {
            cptFloat += nbParticules;
            if (cptFloat > 1)
            {
                cptFloat -= (int)cptFloat;

                float angleNormal = Maths.CalculerAngleDunVecteur(direction);
                float angleOuvertureSurDeux = angleOuvertureDegrees / 2;

                float deltaAngle;
                int grosseur;

                for (int i = 0; i < nbParticules; ++i)
                {
                    grosseur = Maths.Random.Next(grosseurMin, grosseurMax);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(angleOuvertureDegrees) - angleOuvertureSurDeux);

                    Matrice = Matrix.CreateRotationZ(angleNormal + deltaAngle);

                    ParticuleDoubleTexture p = new ParticuleDoubleTexture(Maths.Random.Next(duréeMax / 2, duréeMax), duréeMax / 4)
                    {
                        //Angle = angleNormal + deltaAngle,
                        //AngleÉgalDirection = true,

                        Position = position,
                        Dimensions = new Vector2(grosseur, grosseur),

                        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(9, vitesseMaxX10) / 10f,
                        //VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                        Accélération = Maths.RandomVecteurUnitaire() * 0.05f,

                        AtténuationVitesse = Maths.Random.Next(98, 99) * 0.01f,
                        //AtténuationVitesseRotation = 0.99f,

                        //MutiplicateurDimensions = 1.5f,
                        //AtténuationMutiplicateurDimensions = 0.9f,

                        Couleur = couleur

                    };

                    //p.Profondeur -= ListeParticules.Count * 0.0001f;
                    //p.ArrangerSprite();
                    p.ArrangerSprite(GestionTexture.GetTexture("Particules/CroixGlow0"));
                    p.Image2 = GestionTexture.GetTexture("Particules/Croix0");

                    AjouterEffetAdditiveBlend(p);

                }
            }
        }

        public static void CréerÉmetteurMagique(Vector2 position, Vector2 direction, float nbParticules, int angleOuvertureDegrees, int grosseurMin, int grosseurMax, ref Color couleur, int vitesseMaxX10, int duréeMax)
        {
            ParticuleDoubleTexture particule = new ParticuleDoubleTexture(Maths.Random.Next(60 / 2, 60), 60 / 4)
            {
                //Angle = angleNormal + deltaAngle,
                //AngleÉgalDirection = true,

                Position = position,
                Dimensions = new Vector2((grosseurMax+grosseurMin)/2),

                Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(9, vitesseMaxX10) / 10f,
                //VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                Accélération = Vector2.UnitY * 0.05f,

                AtténuationVitesse = Maths.Random.Next(95, 99) * 0.01f,
                //AtténuationVitesseRotation = 0.99f,

                //MutiplicateurDimensions = 1.5f,
                //AtténuationMutiplicateurDimensions = 0.9f,

                Couleur = couleur

            };

            //p.Profondeur -= ListeParticules.Count * 0.0001f;
            particule.ArrangerSprite();


            ÉmetteurParticule émetteur = new ÉmetteurParticule(duréeMax, duréeMax / 4, particule, vitesseMaxX10)
            {
                NbÉmissions = nbParticules,
                Position = position,

                Vitesse = direction * 0.5f,
                AtténuationVitesse = 0.98f,

                Accélération = Vector2.UnitY * 0.05f,
            };

            AjouterEffetAdditiveBlend(émetteur);
        }

        public static void CréerEffetDestructionPolygone(PolygonePhysique poly)
        {
            switch (poly.Matériel)
            {
                case MatérielPolygone.Organes:
                    GestionEffets.CréerJetSang(poly.Position, -Vector2.UnitY, 30, 270, 8, 16, 50);
                    GestionEffets.CréerOrganes(poly.Position, -Vector2.UnitY);
                    break;
                case MatérielPolygone.Pierre:
                    GestionEffets.CréerPoussière(poly.Position, -Vector2.UnitY, 20, 270, 5, 30, GestionEffets.CouleursPierres, 1);
                    GestionEffets.CréerParticulesLourdes(poly.Position, -Vector2.UnitY, 8, 360, 12, 24, "Particules/ParticulePierre");
                    break;
                case MatérielPolygone.Terre:
                    GestionEffets.CréerPoussière(poly.Position, -Vector2.UnitY, 20, 270, 5, 30, GestionEffets.CouleursTerres, 1);
                    GestionEffets.CréerParticulesLourdes(poly.Position, -Vector2.UnitY, 8, 360, 12, 24, "Particules/ParticuleTerre");
                    break;
                case MatérielPolygone.Bois:
                    GestionEffets.CréerPoussière(poly.Position, -Vector2.UnitY, 20, 270, 5, 30, GestionEffets.CouleursTerres, 1);
                    break;
                default:
                    GestionEffets.CréerPoussière(poly.Position, -Vector2.UnitY, 10, 270, 5, 30, GestionEffets.CouleursPierres, 1);
                    break;
            }
        }

        public static void CréerCercleExplosion(float rayon, Vector2 centre, ref Color couleurBase)
        {
            Vector2 vecteur = Vector2.Zero;
            Vector2 vecteurPerpendiculaire = Vector2.Zero;
            Color couleur;
            int tempsMax;
            ParticuleDoubleTexture particule;
            float unSurRayon = 1f / rayon;

            for (int i = 0; i <= 360; i += 15)
            {
                tempsMax = Maths.Random.Next(40, 60);

                vecteur.X = rayon * (float)Math.Cos(MathHelper.ToRadians(i));
                vecteur.Y = rayon * (float)Math.Sin(MathHelper.ToRadians(i));

                Vector2.Transform(ref vecteur, ref Maths.MatriceRotation90, out vecteurPerpendiculaire);
                Maths.ModifierCouleur(ref couleurBase, 50, out couleur);

                particule = new ParticuleDoubleTexture(tempsMax, tempsMax / 4)
                {
                    Dimensions = new Vector2(20f),
                    Position = centre + vecteur,
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
                    Position = centre,
                    Vitesse = vecteur * Maths.RandomFloat(0.02f, 0.04f),
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
        public static void CréerCloneLumineux(Personnage perso, int tempsMax)
        {
            foreach (Bone b in perso.ListeBones)
            {
                AjouterEffetAdditiveBlend(new Particule(tempsMax, tempsMax / 2, b));
            }
        }
        public static void CréerCloneLumineux(Personnage perso, int tempsMax, ref Color couleur)
        {
            foreach (Bone b in perso.ListeBones)
            {
                AjouterEffetAdditiveBlend(new Particule(tempsMax, tempsMax / 2, b, ref couleur));
            }
        }
        public static void CréerCloneLumineux(PolygonePhysique polygone, int tempsMax, ref Color couleur)
        {
            AjouterEffetAdditiveBlend(new Particule(tempsMax, tempsMax / 2, polygone, ref couleur));
        }
        public static void CréerCloneLumineux(PolygonePhysique polygone, int tempsMax)
        {
            AjouterEffetAdditiveBlend(new Particule(tempsMax, tempsMax / 2, polygone));
        }

        public static void Update()
        {
            Nettoyer();

            foreach (LumièreTemporaire l in ListeLumièreTempo)
            {
                l.Update();
            }
            foreach (IEffet p in ListeIEffets)
            {
                p.Update();
            }
            foreach (IEffet p in ListeIEffetsAdditiveBlend)
            {
                p.Update();
            }
            foreach (IEffet p in ListeIEffetsHUD)
            {
                p.Update();
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (IEffet p in ListeIEffets)
            {
                p.Draw(spriteBatch);
            }
        }
        public static void DrawAdditive(SpriteBatch spriteBatch)
        {
            foreach (IEffet p in ListeIEffetsAdditiveBlend)
            {
                p.Draw(spriteBatch);
            }
        }
        public static void DrawHUD(SpriteBatch spriteBatch)
        {
            foreach (IEffet p in ListeIEffetsHUD)
            {
                p.Draw(spriteBatch);
            }
        }
    }

    class LumièreTemporaire
    {
        public int TempMax;
        public SourceLumière Lumiere;

        int deltaRange1;
        int deltaRange2;

        public LumièreTemporaire(int tempsMax, int range, ref Color couleur, ref Vector2 position)
        {
            TempMax = tempsMax;
            Lumiere = SourceLumière.CréerLumièreExplosion(couleur);
            Lumiere.Position = position;

            Lumiere.BackLight.Range = range;
            Lumiere.SpotLight.Range = range * 0.75f;
            
            deltaRange1 = (int)(Lumiere.BackLight.Range / TempMax);
            deltaRange2 = (int)(Lumiere.SpotLight.Range / TempMax);
        }
        public LumièreTemporaire(int tempsMax, int range, ref Color couleur,ref Color couleurCentre, ref Vector2 position)
        {
            TempMax = tempsMax;
            Lumiere = SourceLumière.CréerLumièreExplosion(couleur);
            Lumiere.SpotLight.Color = couleurCentre;
            Lumiere.Position = position;

            Lumiere.BackLight.Range = range;
            Lumiere.SpotLight.Range = range * 0.75f;

            deltaRange1 = (int)(Lumiere.BackLight.Range / TempMax);
            deltaRange2 = (int)(Lumiere.SpotLight.Range / TempMax);
        }

        public virtual void Update()
        {
            if (TempMax <= 0)
            {
                GestionEffets.DétruireLumiereTempo(this);
                Lumiere.Supprimer();
            }
            else
            {
                Lumiere.BackLight.Range -= deltaRange1;
                Lumiere.SpotLight.Range -= deltaRange2;

                --TempMax;
            }
        }
    }
    class LumièreTemporaireIntensityFade : LumièreTemporaire
    {
        float deltaIntensity1;
        float deltaIntensity2;

        public LumièreTemporaireIntensityFade(int tempsMax, int range, ref Color couleur, ref Color couleurCentre, ref Vector2 position, float intensity=1f)
            :base(tempsMax, range, ref couleur,ref couleurCentre, ref position)
        {
            deltaIntensity1 = (Lumiere.BackLight.Intensity / TempMax);
            deltaIntensity2 = (Lumiere.SpotLight.Intensity / TempMax);

            Lumiere.BackLight.Intensity = intensity;
            Lumiere.SpotLight.Intensity = intensity * 0.8f;
        }

        public LumièreTemporaireIntensityFade(int tempsMax, int range, ref Color couleur, ref Vector2 position, float intensity = 1f)
            : base(tempsMax, range, ref couleur, ref position)
        {
            deltaIntensity1 = (Lumiere.BackLight.Intensity / TempMax);
            deltaIntensity2 = (Lumiere.SpotLight.Intensity / TempMax);

            Lumiere.BackLight.Intensity = intensity;
            Lumiere.SpotLight.Intensity = intensity * 0.8f;
        }

        public override void Update()
        {
            if (TempMax <= 0)
            {
                GestionEffets.DétruireLumiereTempo(this);
                Lumiere.Supprimer();
            }
            else
            {
                Lumiere.BackLight.Intensity -= deltaIntensity1;
                Lumiere.SpotLight.Intensity -= deltaIntensity2;

                --TempMax;
            }
        }
    }
}
