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
    static class Maths
    {
        public const float UN_SUR_2PI = 1f / MathHelper.TwoPi;

        public static Random Random = new Random(0);
        public static Color RandomColor
        {
            get
            {
                return new Color(Random.Next(256), Random.Next(256), Random.Next(256));
            }
        }
        public static Matrix MatriceRotation90 = Matrix.CreateRotationZ(MathHelper.PiOver2);
        public static Matrix MatriceRotation270 = Matrix.CreateRotationZ(-MathHelper.PiOver2);

        public static bool EstEntre(float valeur, float borne1, float borne2)
        {
            return (valeur <= borne1 && valeur >= borne2) || (valeur <= borne2 && valeur >= borne1);
        }
        public static bool EstEntre(ref float valeur,ref float borne1,ref  float borne2)
        {
            return (valeur <= borne1 && valeur >= borne2) || (valeur <= borne2 && valeur >= borne1);
        }
        public static bool EstEntre(ref float valeur, float borne1, float borne2)
        {
            return (valeur <= borne1 && valeur >= borne2) || (valeur <= borne2 && valeur >= borne1);
        }

        /// <summary>
        /// Vous avez une chance sur diviseurChance que la fonction retourne vrai
        /// </summary>
        public static bool UneChanceSur(int diviseurChance)
        {
            return Random.Next(diviseurChance) == 0;
        }

        public static int PuissanceDeDeux(int exposant)
        {
            return 1 << exposant;
        }

        /// <summary>
        /// Retourne Vrai si la position se trouve à l'intérieur du rectangle
        /// </summary>
        /// <param name="position">La Position à vérifier</param>
        /// <param name="rectangle">Le Rectangle</param>
        public static bool EstDansRectangle(Vector2 position, Rectangle rectangle)
        {
            return EstEntre(ref position.X, rectangle.X,rectangle.X + rectangle.Width) && EstEntre(ref position.Y, rectangle.Y, rectangle.Y + rectangle.Height);
        }
        
        /// <summary>
        /// Retourne la longueur approximative du Vecteur
        /// </summary>
        /// <param name="vecteur">Le vecteur à mesurer</param>
        public static float LongueurVecteurApprox(Vector2 vecteur)
        {
            return ApproxDistance(Vector2.Zero, vecteur);
        }
        
        public static Vector2 ProjectionOrthogonale(Vector2 vecteur,Vector2 vecteurDirecteur)
        {
            return Vector2.Dot(vecteur, vecteurDirecteur) / Vector2.Dot(vecteurDirecteur, vecteurDirecteur) * vecteurDirecteur;            
        }
        
        /// <summary>
        /// Retourne la Composante Z obtenue par un produit vectoriel où z1 et z2 sont nuls
        /// </summary>
        /// <param name="v1">Premier Vecteur</param>
        /// <param name="v2">Deuxième Vecteur</param>
        public static float ProduitVectoriel(Vector2 v1, Vector2 v2)
        {
            //v2.Y *= -1;
            //v1.Y *= -1;
            return v1.X * v2.Y - v1.Y * v2.X;
        }
        /// <summary>
        /// Retourne la Composante Z obtenue par un produit vectoriel où z1 et z2 sont nuls
        /// </summary>
        /// <param name="v1">Premier Vecteur</param>
        /// <param name="v2">Deuxième Vecteur</param>
        public static float ProduitVectoriel(ref Vector2 v1, ref Vector2 v2)
        {
            //v2.Y *= -1;
            //v1.Y *= -1;
            return v1.X * v2.Y - v1.Y * v2.X;
        }
        
        public static Vector2 ProduitVectoriel(float z1, Vector2 v2)
        {
            //v2.Y *= -1;
            //v1.Y *= -1;
            return new Vector2(-z1 * v2.Y, -z1 * v2.X);
        }
        
        /// <summary>
        /// Calcule la composante m d'une pente y = mx + b en fonction d'un Vector2
        /// </summary>
        public static float ObtenirPente_m(Vector2 vecteur)
        {
            return vecteur.Y/vecteur.X;
        }
        
        /// <summary>
        /// Retourne une distance approximative entre 2 points
        /// </summary>
        /// <param name="v1">Premier Vecteur (Position)</param>
        /// <param name="v2">Deuxième Vecteur (Position)</param>
        public static int ApproxDistance(Vector2 v1, Vector2 v2)
        {
            int dx = (int)Math.Abs(v1.X - v2.X);
            int dy = (int)Math.Abs(v1.Y - v2.Y);
            int min, max; if (dx < 0) dx = -dx;
            if (dy < 0) dy = -dy; if (dx < dy)
            {
                min = dx;
                max = dy;
            }
            else
            {
                min = dy;
                max = dx;
            }   // coefficients equivalent to ( 123/128 * max ) and ( 51/128 * min )
            return (((max << 8) + (max << 3) - (max << 4) - (max << 1) +
                     (min << 7) - (min << 5) + (min << 3) - (min << 1)) >> 8);
        }

        /// <summary>
        /// Retourne la valeur restreinte de val entre min et max
        /// </summary>
        /// <param name="val">La valeur à Restreindre</param>
        public static int Clamp(int val, int max, int min)
        {
            if (val > max)
            {
                val = max;
            }
            else if (val < min)
            {
                val = min;
            }
            return val;
        }

        public static float CalculerAngleEntreDeuxPosition(Vector2 pos1, Vector2 pos2)
        {
            return (float)Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X);
        }
        
        public static float CalculerAngleDunVecteur(Vector2 vecteur)
        {
            return (float)Math.Atan2(vecteur.Y, vecteur.X);
        }

        /// <summary>
        /// Vérifie l'intersection des rayons de 2 PolygonePhysique
        /// </summary>
        public static bool IntersectionRayons(PolygonePhysique poly1, PolygonePhysique poly2)
        {
            return IntersectionRayons(poly1.Position, poly1.Rayon, poly2.Position, poly2.Rayon);
        }

        /// <summary>
        /// Retourne vrai si les rayons se croisent
        /// </summary>
        public static bool IntersectionRayons(Vector2 position1, float rayon1, Vector2 position2, float rayon2)
        {
            float distanceCarré = Vector2.DistanceSquared(position1, position2);
            float distanceCentre=(rayon1+rayon2);
            return distanceCarré <= distanceCentre * distanceCentre;
        }
        public static bool IntersectionRayons(Vector2 position1, Vector2 position2, float rayonAuCarré)
        {
            return Vector2.DistanceSquared(position1, position2) <= rayonAuCarré;
        }

        public static Vector2 MultiplierY(Vector2 vecteur, float nombre)
        {
            vecteur.Y *= nombre;
            return vecteur;
        }

        /// <summary>
        /// Change les composantes RGB d'une couleur en ajoutant un int entre
        /// -différenceMax et différenceMax à chacune de ces composantes
        /// </summary>
        /// <param name="différenceMax">int de 0 à 256</param>
        public static void ModifierCouleur(ref Color source, int différenceMax, out Color destination)
        {
            destination = new Color(source.R + Random.Next(2 * différenceMax) - différenceMax,
                                    source.G + Random.Next(2 * différenceMax) - différenceMax,
                                    source.B + Random.Next(2 * différenceMax) - différenceMax);
                                    
        }
        public static Color ModifierCouleur(Color source, int différenceMax)
        {
            return new Color(source.R + Random.Next(2 * différenceMax) - différenceMax,
                             source.G + Random.Next(2 * différenceMax) - différenceMax,
                             source.B + Random.Next(2 * différenceMax) - différenceMax);

        }
        
        /// <summary>
        /// Mélange deux Color selon un certain rapport donné par float ratio 
        /// </summary>
        /// <param name="ratio">Entre 0f et 1f, représente le pourcentage de couleur1 dans la couleur finale</param>
        public static void MixerCouleurs(ref Color couleur1, ref Color couleur2, float ratio, out Color résultat)
        {
            float autreRatio = 1f - ratio;
            Vector4 color1 = couleur1.ToVector4() * ratio;
            Vector4 color2 = couleur2.ToVector4() * autreRatio;
            résultat = new Color(color1.X + color2.X,
                                 color1.Y + color2.Y,
                                 color1.Z + color2.Z,
                                 color1.W + color2.W);
        }


        public static void MixerCouleurs(Color couleur1, Color couleur2, float ratio, out Color résultat)
        {
            float autreRatio = 1f - ratio;
            Vector4 color1 = couleur1.ToVector4() * ratio;
            Vector4 color2 = couleur2.ToVector4() * autreRatio;
            résultat = new Color(color1.X + color2.X,
                                 color1.Y + color2.Y,
                                 color1.Z + color2.Z,
                                 color1.W + color2.W);
        }

        /// <summary>
        /// Reourne aléatoirement 1f ou -1f
        /// </summary>
        public static float RandomInverseur()
        {
            if (Random.Next(2) == 0)
            {
                return -1f;
            }
            else
            {
                return 1f;
            }
        }
        
        /// <summary>
        /// Retourne un float entre -1 et 1
        /// </summary>
        public static float RandomMultiplicateur()
        {
            float result = (Random.Next(65536) - 32768) * 0.000030517578125f;

            return result;
        }

        /// <summary>
        /// Retourne un float entre -bornes et borne
        /// </summary>
        public static float RandomMultiplicateur(float bornes)
        {
            float result = bornes * (Random.Next(65536) - 32768) * 0.000030517578125f;

            return result;
        }

        /// <summary>
        /// Retourne un float entre 0 et max
        /// </summary>
        public static float RandomFloat(float max)
        {
            return (float)Random.NextDouble() * max;
        }

        /// <summary>
        /// Retourne un float entre min et max
        /// </summary>
        public static float RandomFloat(float min, float max)
        {
            float result = Random.Next((int)(min * 65536f), (int)(max * 65536f)) * 0.0000152587890625f;
            //if (result > max || result < min-1)
            //{

            //}
            return result;
        }

        /// <summary>
        /// Retourne un Vector2 de longueur 1f orienté dans un angle aléatoire
        /// </summary>
        public static Vector2 RandomVecteurUnitaire()
        {
            return Vector2.Transform(Vector2.UnitX, Matrix.CreateRotationZ(MathHelper.ToRadians(Random.Next(360))));
        }

        /// <summary>
        /// Retourne un Vector2 de longueur 1f orienté dans un angle aléatoire
        /// </summary>
        public static void RandomVecteurUnitaire(out Vector2 vecteurUnitaire)
        {
            vecteurUnitaire = Vector2.UnitX;
            Matrix matrice = Matrix.CreateRotationZ(MathHelper.ToRadians(Random.Next(360)));
            Vector2.Transform(ref vecteurUnitaire, ref matrice, out vecteurUnitaire);
        }

        /// <summary>
        /// Retourne un Vector2 unitaire avec l'angle désiré
        /// </summary>
        public static Vector2 VecteurUnitaire(float angle)
        {
            Vector2 vecteur = Vector2.UnitX;
            Matrix matrice = Matrix.CreateRotationZ(angle);
            Vector2.Transform(ref vecteur, ref matrice, out vecteur);
            return vecteur;
        }

        /// <summary>
        /// Retourne un Vector2 unitaire avec l'angle désiré
        /// </summary>
        public static void VecteurUnitaire(float angle, out Vector2 vecteur)
        {
            vecteur = Vector2.UnitX;
            Matrix matrice = Matrix.CreateRotationZ(angle);
            Vector2.Transform(ref vecteur, ref matrice, out vecteur);
        }

        /// <summary>
        /// Retourne un Vector2 avec l'angle et la longueur désirée
        /// </summary>
        public static Vector2 Vecteur(float angle, float longueur)
        {
            Vector2 vecteur = Vector2.UnitX * longueur;
            Matrix matrice = Matrix.CreateRotationZ(angle);
            Vector2.Transform(ref vecteur, ref matrice, out vecteur);
            return vecteur;
        }

        /// <summary>
        /// Retourne un Vector2 avec l'angle et la longueur désirée 
        /// </summary>
        public static void Vecteur(float angle, float longueur, out Vector2 vecteur)
        {
            vecteur = Vector2.UnitX * longueur;
            Matrix matrice = Matrix.CreateRotationZ(angle);
            Vector2.Transform(ref vecteur, ref matrice, out vecteur);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valeurPrécédente"></param>
        /// <param name="vitesseOscillation"></param>
        /// <returns></returns>
        public static float ValeurOscillante(float valeurPrécédente,float vitesseOscillation)
        {
            return (float)Math.Sin(Math.Asin(valeurPrécédente) + vitesseOscillation);
            
        }

        /// <summary>
        /// Retourne le sin de (la valeur de référence * TwoPi)
        /// </summary>
        /// <param name="valeurRéférence">de 0f à 1f</param>
        public static float ValeurOscillante(float valeurRéférence)
        {
            return (float)Math.Sin(MathHelper.TwoPi * valeurRéférence);

        }

        /// <summary>
        /// Retourne une valeur oscillant entre 0 et 1f
        /// </summary>
        /// <param name="valeurRéférence">de 0f à 1f</param>
        public static float ValeurOscillanteEntre0et1(float valeurRéférence)
        {
            //float val = (float)Math.Sin(MathHelper.TwoPi * valeurRéférence + 3 * MathHelper.PiOver2) * 0.5f + 0.5f;
            float val = (float)Math.Cos(MathHelper.TwoPi * valeurRéférence) * -0.5f + 0.5f;
            //MoteurJeu.AwesomeBox.WriteLine(val.ToString());
            return val;

        }

        /// <summary>
        /// Cette Fonction Retourne 1 si le bool est vrai et 0 s'il est faut
        /// </summary>
        public static int BoolVersInt(bool unbool)
        {
            if (unbool) { return 1; }
            return 0;
        }
    }
}
