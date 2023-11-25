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
    static class GestionCoupeuse
    {
        static List<Coupeuse> ListeCoupeuses = new List<Coupeuse>();

        public static void Ajouter(Coupeuse coupeuse)
        {
            ListeCoupeuses.Add(coupeuse);
        }
        public static void Ajouter(Vector2 position, Vector2 direction, float longueur)
        {
            ListeCoupeuses.Add(new Coupeuse(position, direction, longueur));
        }
        public static void Ajouter(Vector2 position, Vector2 direction, float longueur, uint nbCoupesMax, int puissance)
        {
            ListeCoupeuses.Add(new Coupeuse(position, direction, longueur, nbCoupesMax, puissance));
        }

        public static void RésoudreCoupes(List<PolygonePhysique> listeCorps)
        {
            for (int i = 0; i < ListeCoupeuses.Count; ++i)
            {
                ListeCoupeuses[i].Couper(listeCorps);
            }
            ListeCoupeuses.Clear();
        }
    }
}
