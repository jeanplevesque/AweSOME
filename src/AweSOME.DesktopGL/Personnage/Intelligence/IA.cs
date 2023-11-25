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
using AwesomeAnimation;

namespace AweSOME
{
    enum TypesIA { Zombie, Grimpeur, Tireur, Boomer }
    class IA:Intelligence
    {
        public static int MaxPathfinding=1;
        public static int PathfindingRestants=1;
        public static void ResetPathFinding() { PathfindingRestants = MaxPathfinding; }


        public Player Joueur;
        public Niveau Monde;

        public Stack<Tuile> Itinéraire = new Stack<Tuile>();
        public Stack<Tuile> AncientItinéraire = new Stack<Tuile>();
        public Tuile TuileActive { get { return Perso.TuileActive; } set { Perso.TuileActive = value; } }
        public Tuile Destination;
        public Tuile ProchaineTuile;
        public Tuile TuilePrécédente;

        public bool SuitPathfinding;
        public bool DoitPathFinder;
        public Point GrosseurPathFinding;

        public int NbTuilesHauts = 3;

        public int MaxNbPathfinding = 5;
        public int NbPathfinding;

        public IA(Personnage perso)
            :base(perso)
        {
            Joueur = GestionNiveaux.NiveauActif.Joueur;
            Monde = GestionNiveaux.NiveauActif;

            Perso.CalculerTuileActive();
            GrosseurPathFinding = new Point(60, 35);
            PathFinding(Joueur.Position);
        }

        protected override void ChoisirPositionCible()
        {
            Perso.Cible.Position = Joueur.Tête.Position;
        }
        protected override void GérerDéplacements()
        {
            if (SuitPathfinding)
            {
                UtiliserItinéraire();
            }
            else
            {
                SeRapprocherDeDestination();
            }
            
            if (TuileActive!= TuilePrécédente || DoitPathFinder)//Joueur.TuileÀChangé || 
            {
                PathFinding(Joueur.Position);
            }
            
                      
        }
        public override void ChangerÉtats()
        {
            base.ChangerÉtats();

            if (Maths.IntersectionRayons(Joueur.Position, Joueur.Dimensions.X*2, Perso.Position, Perso.Dimensions.X))
            {
                Perso.Frapper();
            }
        }


        protected void UtiliserItinéraire()
        {
            if (Itinéraire.Count > 0)
            {
                if (AllerVersProchaineTuile(ProchaineTuile))
                {
                    TuilePrécédente = ProchaineTuile;
                    ProchaineTuile = Itinéraire.Pop();
                    ProchaineTuile.Couleur = Color.White;
                }
            }
        }
        protected void SeRapprocherDeDestination()
        {
            Perso.DéplacerEnX((int)Perso.Direction);
            //if (!TuileActive.VoisinDroite.VoisinBas || Perso.CollisionMurDroite || !TuileActive.VoisinGauche.VoisinBas || Perso.CollisionMurGauche)
            if (Perso.CollisionMurDroite || Perso.CollisionMurGauche)
            {
                Perso.Sauter();
            }
            
        }

        /// <summary>
        /// Retourne vrai si nous sommes sur la prochaine tuile
        /// </summary>
        protected virtual bool AllerVersProchaineTuile(Tuile prochaineTuile)
        {
            if (prochaineTuile.X > TuilePrécédente.X)
            {
                Perso.DéplacerEnX(1);
            }
            else if (prochaineTuile.X < TuilePrécédente.X)
            {
                Perso.DéplacerEnX(-1);
            }

            if (prochaineTuile.Y > TuilePrécédente.Y ||
                (TuileActive && TuileActive.VoisinPlusBas))// ||prochaineTuile.VoisinPlusBas)
            {
                Perso.Sauter();

                //if (prochaineTuile.VoisinPlusBas)
                //{
                //    Perso.DéplacerEnX(Perso.Vitesse.X * 4);
                //}
            }

            return TuileActive==ProchaineTuile;
        }

        protected void PathFinding(Vector2 positionVoulue)
        {

            Tuile tuile = Monde.GetTuile(positionVoulue);
            if (tuile != null && Monde.EstDansNiveau(Perso.Position))
            {
                Destination = tuile;

                if (NbPathfinding <= MaxNbPathfinding && VérifierProximitéPathfinding(Destination))
                {
                    DoitPathFinder = true;
                    if (PathfindingRestants > 0)//car c'est long a calculé
                    {
                        --PathfindingRestants;
                        DoitPathFinder = false;

                        AncientItinéraire = Itinéraire;
                        Itinéraire.Clear();

                        CalculerPoids();

                        SuitPathfinding = TrouverPlusCourtChemain(Destination);

                        AppliquerPathfinding();

                        ++NbPathfinding;
                        MoteurJeu.AwesomeBox.WriteLine("Pathfinder calculé  " + Maths.Random.Next(10));
                        if (NbPathfinding >= MaxNbPathfinding)
                        {
                            MoteurJeu.AwesomeBox.WriteLine("Pathfinder MAAAAAX  " + Maths.Random.Next(10));
                        }
                    }
                }
                else
                {
                    AppliquerNonPathfinding();
                }
            }
        }

        private bool TrouverPlusCourtChemain(Tuile destination)
        {
            //Color Couleur = Color.Red;
            //foreach (Tuile c in Monde.GrilleTuiles)
            //{
            //    if (c.Couleur == Couleur)
            //    {
            //        c.Couleur = Color.White;
            //    }
            //}
            Tuile[,] Grille = Monde.GrilleTuiles;
            Itinéraire.Push(destination);
            //destination.Couleur = Couleur;
            Tuile partielle = destination;
            bool cheminPossible;
            do
            {
                cheminPossible = false;
                Tuile bas = Monde.GetTuile(partielle.X, partielle.Y - 1);
                Tuile haut = Monde.GetTuile(partielle.X, partielle.Y + 1);
                Tuile gauche = Monde.GetTuile(partielle.X - 1, partielle.Y);
                Tuile droite = Monde.GetTuile(partielle.X + 1, partielle.Y);

                if (bas != null && bas.Poids == partielle.Poids - 1)
                {
                    Itinéraire.Push(partielle);
                    partielle = bas;
                    //partielle.Couleur = Couleur;
                    cheminPossible = true;
                }
                else if (droite != null && droite.Poids == partielle.Poids - 1)
                {
                    Itinéraire.Push(partielle);
                    partielle = droite;
                    //partielle.Couleur = Couleur;
                    cheminPossible = true;
                }
                else if (gauche != null && gauche.Poids == partielle.Poids - 1)
                {
                    Itinéraire.Push(partielle);
                    partielle = gauche;
                    //partielle.Couleur = Couleur;
                    cheminPossible = true;
                }
                //else if (bas != null && bas.Poids == partielle.Poids - 1)
                //{
                //    Itinéraire.Push(partielle);
                //    partielle = bas;
                //    //partielle.Couleur = Couleur;
                //    cheminPossible = true;
                //}
                else if (haut != null && haut.Poids == partielle.Poids - 1)
                {
                    Itinéraire.Push(partielle);
                    partielle = haut;
                    //partielle.Couleur = Couleur;
                    cheminPossible = true;
                }
            }
            while (partielle != TuileActive && cheminPossible);
            //if (!cheminPossible)
            //{
                //Itinéraire.Clear();
            //}
            //InfoJeu.LaCarte.RéinitialiserPoids();
            return cheminPossible;
        }

        private void CalculerPoids()
        {
            GestionNiveaux.NiveauActif.ResetPoidsTuiles();

            Tuile[,] Grille = Monde.GrilleTuiles;
            //TuileActive = Monde.GetTuile(Perso.Position);
            TuileActive.Poids = 1;

            Tuile tuile = null;
            Tuile t = null;

            int min_x = TuileActive.X;
            int min_y = TuileActive.Y;
            int max_x = min_x + 1;
            int max_y = min_y + 1;

            int min_x_clamp = Maths.Clamp(TuileActive.X - GrosseurPathFinding.X / 2, GestionNiveaux.NiveauActif.DimensionsGrille.X, 0);
            int min_y_clamp = Maths.Clamp(TuileActive.Y - GrosseurPathFinding.Y / 2, GestionNiveaux.NiveauActif.DimensionsGrille.Y, 0);
            int max_x_clamp = Maths.Clamp(max_x+ GrosseurPathFinding.X / 2, GestionNiveaux.NiveauActif.DimensionsGrille.X, 0);
            int max_y_clamp = Maths.Clamp(max_y + GrosseurPathFinding.Y / 2, GestionNiveaux.NiveauActif.DimensionsGrille.Y, 0); 

            bool progres;
            do
            {
                progres = false;
                for (int y = min_y; y < max_y; ++y)
                {
                    for (int x = min_x; x < max_x; ++x)
                    {
                        t = Grille[x, y];

                        
                        //foreach (Tuile t in Grille)
                        //{
                        
                        if (t.Poids != int.MaxValue)
                        {
                            tuile = Monde.GetTuile(t.X, t.Y + 1);
                            if (tuile != null && tuile.EstPassable(NbTuilesHauts) && tuile.Poids > t.Poids + 1)
                            {
                                tuile.Poids = t.Poids + 1;
                                progres = true;
                            }
                            tuile = Monde.GetTuile(t.X, t.Y - 1);
                            if (tuile != null && tuile.EstPassable(NbTuilesHauts) && tuile.Poids > t.Poids + 1)
                            {
                                tuile.Poids = t.Poids + 1;
                                progres = true;
                            }
                            tuile = Monde.GetTuile(t.X - 1, t.Y);
                            if (tuile != null && tuile.EstPassable(NbTuilesHauts) && tuile.Poids > t.Poids + 1)
                            {
                                tuile.Poids = t.Poids + 1;
                                progres = true;
                            }
                            tuile = Monde.GetTuile(t.X + 1, t.Y);
                            if (tuile != null && tuile.EstPassable(NbTuilesHauts) && tuile.Poids > t.Poids + 1)
                            {
                                tuile.Poids = t.Poids + 1;
                                progres = true;
                            }
                        }
                    }
                    //min_y = Maths.Clamp(min_y - 1, (int)GestionNiveaux.NiveauActif.DimensionsGrille.Y, 0);
                    //max_y = Maths.Clamp(max_y + 1, (int)GestionNiveaux.NiveauActif.DimensionsGrille.Y, 0);
                }
                min_x = Maths.Clamp(min_x - 1, max_x_clamp, min_x_clamp);
                max_x = Maths.Clamp(max_x + 1, max_x_clamp, min_x_clamp);
                min_y = Maths.Clamp(min_y - 1, max_y_clamp, min_y_clamp);
                max_y = Maths.Clamp(max_y + 1, max_y_clamp, min_y_clamp);
            }
            while (progres);
        }

        private bool VérifierProximitéPathfinding(Tuile destination)
        {
            return Math.Abs(TuileActive.X - destination.X) < GrosseurPathFinding.X/2 &&
                   Math.Abs(TuileActive.Y - destination.Y) < GrosseurPathFinding.Y/2;
        }
        private void AppliquerNonPathfinding()
        {
            if (Itinéraire.Count > 0)
            {
                SuitPathfinding = true;
                //ProchaineTuile = Itinéraire.Pop();
                TuilePrécédente = TuileActive;

            }
            else
            {
                ProchaineTuile = Destination;
                TuilePrécédente = TuileActive;
                SuitPathfinding = false;
            }
        }
        private void AppliquerPathfinding()
        {
            if (SuitPathfinding)
            {
                ProchaineTuile = Itinéraire.Pop();
                TuilePrécédente = TuileActive;
            }
            else
            {
                Itinéraire = AncientItinéraire;
                //ProchaineTuile = Itinéraire.Pop();
                TuilePrécédente = TuileActive;
            }
        }

        public void Draw()
        {
            //if (TuileActive != null)
            //{
            //    MoteurJeu.SpriteBatchScène.Draw(BanqueContent.Pixel, TuileActive.CalculerPosition(), null, Color.Green, 0, Vector2.One / 2, 20, SpriteEffects.None, 0);
            //}
            ////MoteurJeu.SpriteBatchScène.Draw(BanqueContent.Pixel, this.TuilePrécédente.CalculerPosition(), null, Color.Yellow, 0, Vector2.One / 2, 30, SpriteEffects.None, 0.1f);
            //if (ProchaineTuile != null)
            //{
            //    MoteurJeu.SpriteBatchScène.Draw(BanqueContent.Pixel, this.ProchaineTuile.CalculerPosition(), null, Color.Red, 0, Vector2.One / 2, 40, SpriteEffects.None, 0.2f);
            //}
        }
    }
}
