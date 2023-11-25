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
    static class GestionImpactsBalle
    {
        static List<ImpactBalle> ListeImpacts = new List<ImpactBalle>();

        public static void AjouterImpact(ImpactBalle impact)
        {
            ListeImpacts.Add(impact);
        }
        public static void AjouterImpact(Vector2 position, Vector2 normale, BalleFusil balle, PolygonePhysique polyTouché)
        {
            ListeImpacts.Add(new ImpactBalle(position,normale,balle,polyTouché));
        }

        public static void Résoudre()
        {
            foreach (ImpactBalle i in ListeImpacts)
            {
                i.Résoudre();
            }
            ListeImpacts.Clear();
        }
    }
}
