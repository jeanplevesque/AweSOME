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
    class BalleBlocGun:BalleFusil
    {
        BlocGunMode Mode;

        public BalleBlocGun(Vector2 position, float angle, int dégats, int puissance, float longueurBalle, Color couleurBalle, Fusil fusil, Personnage propriétaire)
            :base(position,angle,dégats,puissance,longueurBalle,couleurBalle,fusil,propriétaire)
        {
            Mode = ((BlocGun)fusil).ModePrésent;
            Dimensions.Y = 5;
            ArrangerSprite();
        }

        public override void Update()
        {
            base.Update();

            //GestionEffets.CréerÉtincelles(Position, -Vitesse, 2, 120, 20, 24, "ÉtincelleBleue", 30);
        }
        public override void ChoisirIntersection()
        {
            //Si nous avons des intersections, nous prenons la plus près du centre de la balle (donc la première visuellement)
            IntersectionAvecNormale intersection = null;
            if (ListeIntersections.Count > 0)
            {
                ListeIntersections.Sort();
                intersection = ListeIntersections[0];
                ListeIntersections.Clear();

                AffecterBloc(intersection);

                GestionNiveaux.NiveauActif.DétruireBalleFusil(this);
            }
        }
        public override void VérifierCollisions()
        {
            VérifierCollisionsBlocs();
        }

        private void AffecterBloc(Intersection intersection)
        {
            Bloc bloc = ((Bloc)intersection.Corps);

            switch (Mode)
            {
                case BlocGunMode.Tunel:
                    GestionNiveaux.NiveauActif.CréerTunel(bloc.Position);
                    GestionEffets.CréerEffetDestructionPolygone(bloc);
                    break;
                case BlocGunMode.Teleport:
                    ((BlocGun)FusilOrigine).RecevoirBloc(bloc);

                    GestionEffets.CréerTourbillonPoussière(bloc.Position, 10, 40, 70, GestionEffets.CouleursPierres, 2, 40);
                    GestionEffets.CréerÉtincelles(FusilOrigine.PositionEmbout,Vector2.UnitX,10,360,20,24,"ÉtincelleBleue",30);
                    GestionEffets.CréerÉtincelles(bloc.Position, Vector2.UnitX, 10, 360, 20, 24, "ÉtincelleBleue", 30);
                    GestionEffets.CréerMagie(bloc.Position, Vector2.UnitX, 10, 360, 12, 16, ref Couleur, 30,60);
                    GestionEffets.CréerMagie(FusilOrigine.PositionEmbout, Vector2.UnitX, 10, 360, 12, 16, ref Couleur, 30, 60);
                    break;
            }
        }


    }
}
