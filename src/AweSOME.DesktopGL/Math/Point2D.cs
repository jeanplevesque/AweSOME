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
    struct Point2D : IEquatable<Point2D>
    {
        public static readonly Point2D Zero=new Point2D();

        public Point2D(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public Point2D(float x = 0, float y = 0)
        {
            X = (int)x;
            Y = (int)y;
        }


        public int X;
        public int Y;

        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Point2D left, Point2D right)
        {
            return !left.Equals(right);
        }


        public static Point2D operator-(Point2D left, Point2D right)
        {
            return new Point2D(left.X - right.X, left.Y - right.Y);
        }
        public static Point2D operator+(Point2D left, Point2D right)
        {
            return new Point2D(left.X + right.X, left.Y + right.Y);
        }

        public static Point2D operator *(Point2D left, float right)
        {
            return new Point2D(left.X * right, left.Y * right);
        }
        public static Point2D operator *(Point2D left, int right)
        {
            return new Point2D(left.X * right, left.Y * right);
        }
        public static Point2D operator *(Point2D left, Point2D right)
        {
            return new Point2D(left.X * right.X, left.Y * right.Y);
        }

        public static implicit operator Point(Point2D point2D)
        {
            return new Point(point2D.X, point2D.Y);
        }
        public static implicit operator Point2D(Point point)
        {
            return new Point2D(point.X, point.Y);
        }

        public static explicit operator Point2D(Vector2 vecteur)
        {
            return new Point2D(vecteur.X, vecteur.Y);
        }
        public static explicit operator Vector2(Point2D point)
        {
            return new Vector2(point.X, point.Y);
        }

        public bool Equals(Point2D other)
        {
            return X == other.X && Y == other.Y;
        }
        public override string ToString()
        {
            return X.ToString() + " ; " + Y.ToString();
        }
    }
}
