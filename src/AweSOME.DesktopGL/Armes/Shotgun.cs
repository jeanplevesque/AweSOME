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
    class Shotgun:Fusil
    {
        public int AngleOuvertureBallesDegrées;
        public int NbBallesDansCartouche;

        public Shotgun() { }

        public override void Tirer(bool nouveauTir)
        {
            if (nouveauTir || Type == TypeFusil.Automatique)
            {
                if (CptTempsTir == 0 && NbBallesDansChargeur > 0 && !GestionNiveaux.NiveauActif.BlocPrésentNonTunel(Position))//Dernier bool pour empecher de tirer a traver un mur
                {
                    --NbBallesDansChargeur;
                    ++CptTempsTir;

                    float deltaAngle = MathHelper.ToRadians(AngleOuvertureBallesDegrées) / NbBallesDansCartouche;
                    float angleDébut = Angle - MathHelper.ToRadians(AngleOuvertureBallesDegrées) / 2;

                    for (int i = 0; i < NbBallesDansCartouche; ++i)
                    {
                        angleDébut += deltaAngle;
                        BalleFusil balle = new BalleFusil(PositionEmbout, angleDébut + AngleImprécision, Dégats, Puissance, LongueurBalles, CouleurBalles, this, Propriétaire);
                        GestionNiveaux.NiveauActif.AjouterBalleFusil(balle);
                    }

                    //--------------Créer un effet de flare-------------
                    //GestionEffets.CréerPoussière(PositionEmbout, DistanceEmbout, 5, AngleOuvertureBallesDegrées, 6, 10, GestionEffets.CouleursPierres, 3);
                    
                    CréerEffetsTirs();
                    
                    
                    ++Propriétaire.Stats.NbTirs;
                }
            }
        }
    }
}
