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
    static class GestionNiveaux
    {
        public static Niveau NiveauActif;


        public static void AjouterObjetRamassable(IRamassable objet)
        {
            NiveauActif.AjouterObjetRamassable(objet);
        }
        public static void DétruireObjetRamassable(IRamassable objet)
        {
            NiveauActif.DétruireObjetRamassable(objet);
        }

        public static void AjouterPolygonePhysique(PolygonePhysique poly)
        {
            NiveauActif.AjouterPolygonePhysique(poly);
        }
        public static void DétruirePolygonePhysique(PolygonePhysique poly)
        {
            NiveauActif.DétruirePolygonePhysique(poly);
        }

        public static void Générer(int x, int y)
        {
            NiveauActif = new Niveau(x, y);
            NiveauActif.Initialiser();
        }
        public static void GénérerInfinite(int tailleAreaX, int tailleAreaY)
        {
            NiveauActif = new InfiniteLevel(tailleAreaX, tailleAreaY);
            NiveauActif.Initialiser();
        }
        public static void Update()
        {
            NiveauActif.Update();
        }
        public static void Draw(SpriteBatch spriteBatch)
        {
            NiveauActif.Draw(spriteBatch);
        }
    }
}
