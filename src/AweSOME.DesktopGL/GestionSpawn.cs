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
    static class GestionSpawn
    {
        public static Niveau NiveauActif;

        public static List<Tuile> ListeTuileSpawnable = new List<Tuile>();

        public static void CalculerTuilesSpawnable()
        {
            ListeTuileSpawnable.Clear();
            foreach (Tuile t in NiveauActif.GrilleTuiles)
            {
                if (t.X > 0 && t.X < NiveauActif.DimensionsGrille.Y && !t.VoisinPlusBas && t.EstPassable(3))
                {
                    ListeTuileSpawnable.Add(t);
                }
            }
        }
        public static Vector2 GetRandomSpawnPosition()
        {
            return ListeTuileSpawnable[Maths.Random.Next(ListeTuileSpawnable.Count)];
        }

        public static Vector2 GetRandomSpawnLoinDe(Tuile tuileÀÉviter)
        {
            Tuile spawn = null;

            do
            {
                spawn = ListeTuileSpawnable[Maths.Random.Next(ListeTuileSpawnable.Count)];
            }
            while (Vector2.DistanceSquared(spawn, tuileÀÉviter) <  36864);

            return spawn;
        }
    }
}
