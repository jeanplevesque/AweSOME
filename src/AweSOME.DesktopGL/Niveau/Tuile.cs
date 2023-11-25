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
    class Tuile
    {
        public Vector2 Position;
        public bool Passable = true;
        public int Poids = int.MaxValue;
        public int X;
        public int Y;
        public Color Couleur=Color.White;

        public Tuile VoisinPlusBas;
        public Tuile VoisinBas;
        public Tuile VoisinHaut;
        public Tuile VoisinGauche;
        public Tuile VoisinDroite;

        public Tuile()
        {
            Passable = true;
            Poids = int.MaxValue;
        }

        public Tuile(int i, int j, Bloc bloc)
        {
            this.X = i;
            this.Y = j;
            this.Passable = bloc==null || bloc.EstTunel;
            Position = CalculerPosition();
            CalculerVoisins(true);
        }
        public void ResetPoids()
        {
            Poids = int.MaxValue;
        }

        public void Modifier(Bloc bloc)
        {
            this.Passable = bloc == null || bloc.EstTunel;

            //CalculerVoisins(true);
        }

        public void CalculerVoisins(bool premierAppel)
        {
            VoisinPlusBas = GestionNiveaux.NiveauActif.GetTuile(X, Y - 2);
            VoisinBas = GestionNiveaux.NiveauActif.GetTuile(X, Y - 1);
            VoisinHaut = GestionNiveaux.NiveauActif.GetTuile(X, Y + 1);
            VoisinGauche = GestionNiveaux.NiveauActif.GetTuile(X - 1, Y);
            VoisinDroite = GestionNiveaux.NiveauActif.GetTuile(X + 1, Y);

            if (premierAppel)
            {
                if (VoisinPlusBas != null) { VoisinPlusBas.CalculerVoisins(false); }
                if (VoisinBas != null) { VoisinBas.CalculerVoisins(false); }
                if (VoisinHaut != null) { VoisinHaut.CalculerVoisins(false); }
                if (VoisinGauche != null) { VoisinGauche.CalculerVoisins(false); }
                if (VoisinDroite != null) { VoisinDroite.CalculerVoisins(false); }
            }
        }
        public Vector2 CalculerPosition()
        {
            return new Vector2(X * Bloc.DIMENSION_BLOC, Y * -Bloc.DIMENSION_BLOC);
        }
        
        
        public static implicit operator Vector2(Tuile tuile)
        {
            return tuile.Position;
        }
        public static implicit operator bool(Tuile tuile)
        {
            return tuile == null || tuile.Passable;
        }
        public static implicit operator int(Tuile tuile)
        {
            return tuile.Poids;
        }

        public void Draw()
        {
            string texte = Poids.ToString();
            if (Poids == int.MaxValue)
            {
                texte = "M";
            }
            //if (Passable)
            //{
            //    Couleur = Color.Green;
            //}
            //else
            //{
            //    Couleur = Color.White;
            //}
            MoteurJeu.SpriteBatchAdditive.DrawString(BanqueContent.Font1, texte, new Vector2(X * Bloc.DIMENSION_BLOC, Y * -Bloc.DIMENSION_BLOC), Couleur);

        }

        public bool EstPassable(int nbTuilesHauts)
        {
            //Tuile haut = GestionNiveaux.NiveauActif.GetTuile(X, Y + 1);
            //Tuile bas = GestionNiveaux.NiveauActif.GetTuile(X, Y - 1);
            //CalculerVoisins(true);
            
            return VoisinHaut != null && VoisinBas != null && Passable && VoisinHaut.Passable && VoisinBas.Passable;
        }
    }
}
