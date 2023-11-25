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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace AweSOME
{
    static class Caméra
    {
        static Random random = new Random();
        public static Vector2 Position
        {
            get { return position_; }
            set
            {
                position_ = value;
                CalculerMatrice();
            }
        }
        private static Vector2 position_;

        public static Vector3 Position3D;
        public static float Zoom
        {
            get { return zoom_; }
            set
            {
                zoom_ = value;
                CalculerMatrice();
            }
        }
        private static float zoom_;
        public static float VitesseScalaire { get; set; }
        public static float DistanceSourisDéplacementCaméra { get; set; }

        public static Vector2 DimensionsFenêtre;
        public static Vector2 DimensionsFenêtreSurDeux;

        public static Matrix Matrice;
        public static Matrix MatriceInverse;
        public static Matrix MatriceKrypton;
        public static Matrix MatriceOrthographic;

        static Caméra()
        {          

            Zoom = 1;
            VitesseScalaire = 5;
            DistanceSourisDéplacementCaméra = 200;

            MatriceKrypton = Matrix.Identity;
            Matrice = Matrix.Identity;
            Matrix.CreateScale(1,out Matrice);
            Matrix.CreateScale(1, -1, 1, out MatriceKrypton);
        }

        public static void Update()
        {
            #region Déplacement
            VitesseScalaire = 5 / Zoom;
            //if (GestionIntrants.EstToucheEnfoncée(Keys.A))
            //{
            //    //Position.X -= VitesseScalaire;
            //}
            //if (GestionIntrants.EstToucheEnfoncée(Keys.D))
            //{
            //    //Position.X += VitesseScalaire;
            //}
            //if (GestionIntrants.EstToucheEnfoncée(Keys.W))
            //{
            //    //Position.Y -= VitesseScalaire;
            //}
            //if (GestionIntrants.EstToucheEnfoncée(Keys.S))
            //{
            //    //Position.Y += VitesseScalaire;
            //}

            //if (GestionIntrants.PositionSouris.Y < DistanceSourisDéplacementCaméra && EstEntre(GestionIntrants.PositionSouris.X, 
            //                                                                                    DistanceSourisDéplacementCaméra, 
            //                                                                                    DimensionsFenêtre.X - DistanceSourisDéplacementCaméra))
            //{
            //    Position -= Vector2.UnitY * VitesseScalaire;
            //}
            //if (GestionIntrants.PositionSouris.X < DistanceSourisDéplacementCaméra)
            //{
            //    Position -= Vector2.UnitX * VitesseScalaire;
            //}
            //if (GestionIntrants.PositionSouris.X + DistanceSourisDéplacementCaméra > DimensionsFenêtre.X)
            //{
            //    Position += Vector2.UnitX * VitesseScalaire;
            //}
            //if (GestionIntrants.PositionSouris.Y + DistanceSourisDéplacementCaméra / 2 > DimensionsFenêtre.Y)
            //{
            //    Position += Vector2.UnitY * VitesseScalaire;
            //}
            #endregion
            #region Zoom
            if (GestionIntrants.EstToucheEnfoncée(Keys.Add))
            {
                Zoom += 0.01f;
            }
            if (GestionIntrants.ÉtatSouris.ScrollWheelValue < GestionIntrants.AncienÉtatSouris.ScrollWheelValue)
            {
                Zoom -= 0.01f;
            }
            if (GestionIntrants.EstToucheEnfoncée(Keys.Subtract))
            {
                Zoom -= 0.01f;
            }
            if (GestionIntrants.ÉtatSouris.ScrollWheelValue > GestionIntrants.AncienÉtatSouris.ScrollWheelValue)
            {
                Zoom += 0.01f;
            }
            Zoom = MathHelper.Clamp(Zoom, 0.05f, 3f);
            #endregion

            //CalculerMatrice();
        }

        /// <summary>
        /// Change les dimensions de la fenêtre de jeu
        /// </summary>
        /// <param name="dimensions">Les nouvelles dimensions de la fenêtres</param>
        public static void SetDimensionsFenêtre(Vector2 dimensions)
        {
            DimensionsFenêtre = dimensions;
            DimensionsFenêtreSurDeux = dimensions / 2;

            MoteurJeu.graphics.PreferredBackBufferWidth = (int)DimensionsFenêtre.X;
            MoteurJeu.graphics.PreferredBackBufferHeight = (int)DimensionsFenêtre.Y;
            MoteurJeu.graphics.ApplyChanges();
        }

        public static Rectangle Transform(Rectangle rect)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.X = (int)((rect.X - Position.X) * Zoom + DimensionsFenêtreSurDeux.X);
            rectangle.Y = (int)((rect.Y - Position.Y) * Zoom + DimensionsFenêtreSurDeux.Y);
            rectangle.Width = (int)((float)rect.Width * Zoom);
            rectangle.Height = (int)((float)rect.Height * Zoom);
            return rectangle;
        }
        public static Vector2 Transform(Vector2 pos)
        {
            throw new Exception("NNOOONNN Utiliser la matrice");
            //return (pos - Position) * Zoom + DimensionsFenêtreSurDeux;
        }
        public static Vector2 Transform(Vector2 pos,float atténuation)
        {
            return (pos - Position) * Zoom * atténuation + DimensionsFenêtreSurDeux;
        }
        public static Vector2 Untransform(Vector2 pos)
        {
            return (pos - DimensionsFenêtreSurDeux) / Zoom + Position;
        }
        public static bool EstVisible(Vector2 pos,float rayon)
        {
            pos=Transform(pos);
            return pos.X + rayon * Caméra.Zoom > 0 &&
                pos.X - rayon * Caméra.Zoom < DimensionsFenêtreSurDeux.X &&
                pos.Y + rayon * Caméra.Zoom > 0 &&
                pos.Y - rayon * Caméra.Zoom < DimensionsFenêtreSurDeux.Y;
        }
        public static bool EstEntre(float valeur, float borne1, float borne2)
        {
            return (valeur <= borne1 && valeur >= borne2) || (valeur <= borne2 && valeur >= borne1);
        }

        public static void Shake()
        {
            Position += Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(random.Next(360) / 360f * MathHelper.TwoPi))*random.Next(10);
        }
        public static void Shake(int force)
        {
            Position += Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(random.Next(360) / 360f * MathHelper.TwoPi)) * random.Next(force);
        }
        
        //static float zero = 0;

        private static void CalculerMatrice()
        {
            Position3D.X = -Position.X * Zoom + DimensionsFenêtreSurDeux.X;
            Position3D.Y = -Position.Y * Zoom + DimensionsFenêtreSurDeux.Y;
            //Matrice = Matrix.Identity;
            Matrix.CreateScale(Zoom, out Matrice);
            Matrice.Translation = Position3D;

            Matrix.CreateScale(1, -1, 0, out MatriceKrypton);
            MatriceKrypton.Translation=new Vector3(-Position.X, Position.Y, 0);
            Matrix.CreateOrthographic(DimensionsFenêtre.X / Zoom, DimensionsFenêtre.Y / Zoom, 0, 1, out MatriceOrthographic);

            Matrix.Multiply(ref MatriceKrypton, ref MatriceOrthographic, out MatriceKrypton);
           
            Matrix.Invert(ref Matrice, out MatriceInverse);
        }
        //public static Matrix GetMatrice()
        //{
        //    Matrix translation; 
        //    Matrix.CreateTranslation(-Position.X, -Position.Y, zero ,out translation);
        //    Matrix scale;
        //    Matrix.CreateScale(Zoom, out scale);

        //    return scale * translation;
        //}
    }
}
