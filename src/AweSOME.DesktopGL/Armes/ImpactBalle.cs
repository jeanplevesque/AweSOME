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
    class ImpactBalle
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Normale { get; protected set; }
        public BalleFusil Balle { get; protected set; }
        public PolygonePhysique PolyTouché { get; protected set; }

        public ImpactBalle(Vector2 position, Vector2 normale, BalleFusil balle, PolygonePhysique polyTouché)
        {
            Position = position;
            Normale = normale;
            Balle = balle;
            PolyTouché = polyTouché;
        }

        public void Résoudre()
        {
            if (PolyTouché is Bone)
            {
                RésoudrePersonnage();
            }
            else
            {
                RésoudrePoly();
            }
        }

        private void RésoudrePersonnage()
        {
            Bone bone = ((Bone)PolyTouché);
            //Le Personnage se blesse
            if (bone.Parent != Balle.Propriétaire)
            {
                bone.Parent.Blesser(this);
                
            }

            //On créer un sploush de sang
            GestionEffets.CréerSang(Position, Balle.VecteurPrincipal);
            
        }

        private void RésoudrePoly()
        {
            if (PolyTouché.EstLibre)
            {
                PolyTouché.AddForceAuPoint(Balle.Vitesse * Balle.Dégats * 10, Position);
                PolyTouché.NbCollisions = 0;
            }

            if (Balle.Puissance >= PolyTouché.Solidité)
            {
                PolyTouché.Abimer(Balle.Dégats);

                //On créer un effet d'impact
                switch (PolyTouché.Matériel)
                {
                    case MatérielPolygone.Terre:
                        GestionEffets.CréerPoussièreEtTerre(Position, Normale);
                        break;
                    case MatérielPolygone.Bois:
                        GestionEffets.CréerPoussièreDeTerre(Position, Normale);
                        break;
                    case MatérielPolygone.Pierre:
                        GestionEffets.CréerPoussièreEtPierres(Position, Normale);
                        break;
                    case MatérielPolygone.Métal:
                        GestionEffets.CréerPoussièreDePierre(Position, Normale);
                        GestionEffets.CréerÉtincellesJaunes(Position, Normale);
                        break;
                    case MatérielPolygone.Organes:
                        GestionEffets.CréerSang(Position, Normale);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (PolyTouché.Matériel)
                {
                    case MatérielPolygone.Bois:
                        GestionEffets.CréerPoussièreDeTerre(Position, Normale);
                        break;
                    case MatérielPolygone.Terre:
                        GestionEffets.CréerPoussièreDeTerre(Position, Normale);
                        break;
                    case MatérielPolygone.Pierre:
                        GestionEffets.CréerPoussièreDePierre(Position, Normale);
                        break;
                    case MatérielPolygone.Métal:
                        GestionEffets.CréerÉtincellesJaunes(Position, Normale);
                        break;
                    case MatérielPolygone.Organes:
                        GestionEffets.CréerSang(Position, Normale);
                        break;
                    default:
                        break;
                }
            }

            
            
        }
    }
}
