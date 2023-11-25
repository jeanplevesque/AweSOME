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
    class StatistiquesPersonnage
    {

        public int NbCrédits { get; private set; }
        public int NbKills { get; private set; }
        public int NbHeadShots { get; private set; }
        public int NbTirs { get; set; }
        public int NbMembresDétruits { get; set; }

        public Personnage PersonnageSource { get; private set; }


        public StatistiquesPersonnage(Personnage persoSource)
        {
            PersonnageSource = persoSource;
        }

        public StatistiquesPersonnage(StatistiquesPersonnage statistiquesPersonnage)
        {
            NbCrédits = statistiquesPersonnage.NbCrédits;
            NbHeadShots = statistiquesPersonnage.NbHeadShots;
            NbKills = statistiquesPersonnage.NbKills;
            NbTirs = statistiquesPersonnage.NbTirs;
            NbMembresDétruits = statistiquesPersonnage.NbMembresDétruits;

            PersonnageSource = statistiquesPersonnage.PersonnageSource;
        }

        public void AjouterKill(Personnage persoTué)
        {
            if (persoTué.EstEnVie)//on ne veut pas tuer un mort
            {
                int headshot=Maths.BoolVersInt(persoTué.Tête.EstBrisé);
                NbHeadShots += headshot;
                ++NbKills;

                NbCrédits += 10;
                NbCrédits += headshot * 20;
            }
        }
        public StatistiquesPersonnage Clone()
        {
            return new StatistiquesPersonnage(this);
        }

        public static StatistiquesPersonnage operator -(StatistiquesPersonnage left,StatistiquesPersonnage right)
        {
            StatistiquesPersonnage stats = new StatistiquesPersonnage(new Personnage(Vector2.Zero, 0));
            
            stats.NbCrédits = left.NbCrédits - right.NbCrédits;
            stats.NbHeadShots = left.NbHeadShots - right.NbHeadShots;
            stats.NbKills = left.NbKills - right.NbKills;
            stats.NbTirs = left.NbTirs - right.NbTirs;
            stats.NbMembresDétruits = left.NbMembresDétruits - right.NbMembresDétruits;

            return stats;
        }
        public static StatistiquesPersonnage operator +(StatistiquesPersonnage left, StatistiquesPersonnage right)
        {
            StatistiquesPersonnage stats = new StatistiquesPersonnage(new Personnage(Vector2.Zero, 0));

            stats.NbCrédits = left.NbCrédits + right.NbCrédits;
            stats.NbHeadShots = left.NbHeadShots + right.NbHeadShots;
            stats.NbKills = left.NbKills + right.NbKills;
            stats.NbTirs = left.NbTirs + right.NbTirs;
            stats.NbMembresDétruits = left.NbMembresDétruits + right.NbMembresDétruits;

            return stats;
        }
    }
}
