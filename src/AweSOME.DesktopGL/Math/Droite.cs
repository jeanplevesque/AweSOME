using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AweSOME
{
    class Droite:Sprite
    {
        public Sommet Sommet1;
        public Sommet Sommet2;
        public Vector2 P1;
        public Vector2 P2;
        public Vector2 VecteurNormal;// { get { return Vector2.Transform(VecteurPrincipal, Maths.MatriceRotation90); } }
        public Vector2 VecteurPrincipal;
        public float Pente_m;
        public float Pente_b;
        public float Longueur;
        public PolygonePhysique Parent;

        public Droite(Sommet s1, Sommet s2, PolygonePhysique parent)
            :base(s1.Position,Vector2.One)
        {
            Parent = parent;
            Sommet1 = s1;
            Sommet2 = s2;
            P1 = s1.Position;
            P2 = s2.Position;
            UpdateDroiteSansHéritage();
            CalculerPente_mx_b();
            Longueur = Vector2.Distance(P1, P2);
            Dimensions.X = Longueur;
            Dimensions.Y = 2;

            ArrangerSprite(BanqueContent.Pixel, Dimensions, Vector2.UnitY / 2f, parent.Profondeur - 0.01f, Color.Black);
        }
        public Droite(Sommet s1, Sommet s2)
            : base(s1.Position, Vector2.One)
        {
            Couleur = Color.Black;
            Sommet1 = s1;
            Sommet2 = s2;
            P1 = s1.Position;
            P2 = s2.Position;
            UpdateDroiteSansHéritage();
            CalculerPente_mx_b();
            Longueur = Vector2.Distance(P1, P2);
            Dimensions.X = Longueur;
            Dimensions.Y = 2;

            ArrangerSprite(BanqueContent.Pixel, Dimensions, Vector2.UnitY / 2f, 0.5f, Couleur);
        }
        public Droite(Vector2 s1, Vector2 s2)
            : base(s1, Vector2.One)
        {
            Sommet1 = new Sommet(s1);
            Sommet2 = new Sommet(s2);
            Couleur = Color.Black;
            P1 = s1;
            P2 = s2;
            UpdateDroiteSansHéritage();
            CalculerPente_mx_b();
            Longueur = Vector2.Distance(P1, P2);
            Dimensions.X = Longueur;
            Dimensions.Y = 2;

            ArrangerSprite(BanqueContent.Pixel, Dimensions, Vector2.UnitY / 2f, 0.5f, Couleur);
        }
        public Droite()
        {
            Sommet1 = new Sommet(Vector2.Zero);
            Sommet2 = new Sommet(Vector2.Zero);
            Dimensions.Y = 2;
        }
        public void UpdateDroiteSansHéritage()
        {
            P1 = Sommet1.Position;
            P2 = Sommet2.Position;

            Position = P1;

            VecteurPrincipal = P2 - P1;

            CalculerAngle();
        }
        public virtual void Update()
        {
            P1 = Sommet1.Position;
            P2 = Sommet2.Position;

            Position = P1;

            VecteurPrincipal = P2 - P1;
            
        }
        public void CalculerLongueur()
        {
            Dimensions.X = Longueur = Vector2.Distance(P1, P2);
        }
        public void CalculerAngle()
        {
            Angle = (float)Math.Atan2((double)(P2.Y - P1.Y), (double)(P2.X - P1.X));
        }
        public void CalculerPente_mx_b()
        {             
            Pente_m = (P2.Y - P1.Y) / (P2.X - P1.X);
            Pente_b = P1.Y - Pente_m * P1.X;

            Vector2.Transform(ref VecteurPrincipal, ref Maths.MatriceRotation90, out VecteurNormal);
        }
        public float CalculerY(float x)
        {
            return Pente_b + Pente_m * x;
        }
        public Vector2 CalculerPointMilieu()
        {
            return P1 + VecteurPrincipal * 0.5f;
        }
        public Vector2 CalculerPointFraction(float fraction)
        {
            return P1 + VecteurPrincipal * fraction;
        }


        public virtual Vector2 ObtenirIntersection(float pente_m,float pente_b,Vector2 unPoint)
        {
            float intersectionX;
            float intersectionY;
            if (float.IsInfinity(Pente_m))
            {
                intersectionX = Sommet1.Position.X;
            }
            else if (float.IsInfinity(pente_m))
            {
                intersectionX = unPoint.X;
            }
            else
            {
                intersectionX = (pente_b - Pente_b) / (Pente_m - pente_m);
            }
            if (Pente_m == 0)
            {
                intersectionY = Sommet1.Position.Y;
            }
            else if (pente_m == 0)
            {
                intersectionY = unPoint.Y;
            }
            else
            {
                intersectionY = intersectionX * Pente_m + Pente_b;
            }
            //bool EntreEnX = Maths.EstEntre(ref intersectionX, ref droite.Sommet1.Position.X, ref droite.Sommet2.Position.X) && Maths.EstEntre(ref intersectionX, ref Sommet1.Position.X, ref Sommet2.Position.X);
            //bool EntreEnY = Maths.EstEntre(ref intersectionY, ref droite.Sommet1.Position.Y, ref droite.Sommet2.Position.Y) && Maths.EstEntre(ref intersectionY, ref Sommet1.Position.Y, ref Sommet2.Position.Y);
            //intersection = new Vector2(intersectionX, intersectionY);

            //return EntreEnX && EntreEnY;
            return new Vector2(intersectionX, intersectionY);
        }
        public virtual bool VérifierCollisions(Droite droite, out Vector2 intersection)
        {
            intersection = Vector2.Zero;
            float intersectionX;
            float intersectionY;
            if (float.IsInfinity(Pente_m))
            {
                intersectionX = Sommet1.Position.X;
            }
            else if (float.IsInfinity(droite.Pente_m))
            {
                intersectionX = droite.Sommet1.Position.X;
            }
            else if (Pente_m != droite.Pente_m)
            {
                intersectionX = (droite.Pente_b - Pente_b) / (Pente_m - droite.Pente_m);
            }
            else
            {
                intersectionX = 0;
                return false;
                //throw new Exception("wtf le pentes sont les memes");
            }
            if (Pente_m == 0)
            {
                intersectionY = Sommet1.Position.Y;
            }
            else if (droite.Pente_m == 0)
            {
                intersectionY = droite.Sommet1.Position.Y;
            }
            else if (float.IsInfinity(Pente_m))
            {
                intersectionY = intersectionX * droite.Pente_m + droite.Pente_b;
            }
            else
            {
                intersectionY = intersectionX * Pente_m + Pente_b;
            }
            bool EntreEnX = Maths.EstEntre(ref intersectionX, ref droite.Sommet1.Position.X, ref droite.Sommet2.Position.X) && Maths.EstEntre(ref intersectionX, ref Sommet1.Position.X, ref Sommet2.Position.X);
            bool EntreEnY = Maths.EstEntre(ref intersectionY, ref droite.Sommet1.Position.Y, ref droite.Sommet2.Position.Y) && Maths.EstEntre(ref intersectionY, ref Sommet1.Position.Y, ref Sommet2.Position.Y);
            intersection = new Vector2(intersectionX, intersectionY);

            return EntreEnX && EntreEnY;
        }
        public virtual bool VérifierCollisions(Droite droite)
        {
            float intersectionX;
            float intersectionY;
            if (float.IsInfinity(Pente_m))
            {
                intersectionX = Sommet1.Position.X;
            }
            else if (float.IsInfinity(droite.Pente_m))
            {
                intersectionX = droite.Sommet1.Position.X;
            }
            else
            {
                intersectionX = (droite.Pente_b - Pente_b) / (Pente_m - droite.Pente_m);
            }
            if (Pente_m == 0)
            {
                intersectionY = Sommet1.Position.Y;
            }
            else if (droite.Pente_m == 0)
            {
                intersectionY = droite.Sommet1.Position.Y;
            }
            else if (float.IsInfinity(Pente_m))
            {
                intersectionY = intersectionX * droite.Pente_m + droite.Pente_b;
            }
            else
            {
                intersectionY = intersectionX * Pente_m + Pente_b;
            }
            bool EntreEnX = Maths.EstEntre(ref intersectionX, ref droite.Sommet1.Position.X, ref droite.Sommet2.Position.X) && Maths.EstEntre(ref intersectionX, ref Sommet1.Position.X, ref Sommet2.Position.X);
            bool EntreEnY = Maths.EstEntre(ref intersectionY, ref droite.Sommet1.Position.Y, ref droite.Sommet2.Position.Y) && Maths.EstEntre(ref intersectionY, ref Sommet1.Position.Y, ref Sommet2.Position.Y);
            //intersection = new Vector2(intersectionX, intersectionY);

            return EntreEnX && EntreEnY;
        }

        //public override void Draw(SpriteBatch spriteBatch)
        //{
        //    Couleur = Color.Black;
        //    spriteBatch.Draw(BanqueContent.Pixel, (P1), null, Couleur, Angle, Vector2.UnitY / 2f, Dimensions , SpriteEffects.None, Parent.Profondeur-0.001f);            
        //}
        public void DrawDroite(SpriteBatch spriteBatch)
        {
            CalculerAngle();
            spriteBatch.Draw(BanqueContent.Pixel, (P1), null, Couleur, Angle, Vector2.UnitY / 2f, Dimensions, SpriteEffects.None, Parent.Profondeur - 0.001f);
        }
    }
}
