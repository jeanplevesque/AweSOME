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
    /// <summary>
    /// Il faut noter qu'il n'y a pas d'update pour l'explosion, puisque son effet n'est pas réparti sur le temps
    /// </summary>
    class Explosion:Sprite
    {
        public float RayonAuCarré;
        public float RayonDAction;
        public float UnSurRayon;

        public int Dégats;
        public int Puissance;
        public Personnage Propriétaire;

        public Explosion(Vector2 position, float rayonAction, float rayonCarré, int dégats,int puissance, Personnage propriétaire, bool effetPoussiere)
            : base(position)
        {
            Propriétaire = propriétaire;
            Puissance = puissance;
            RayonAuCarré = rayonCarré;
            RayonDAction = rayonAction;
            UnSurRayon = 1f / RayonDAction;
            Dégats = dégats;

            if (effetPoussiere)
            {
                Vector2 vecteur = Vector2.Zero;
                for (int i = 0; i <= 360; i += 30)
                {
                    vecteur.X = RayonDAction * (float)Math.Cos(MathHelper.ToRadians(i));
                    vecteur.Y = RayonDAction * (float)Math.Sin(MathHelper.ToRadians(i));

                    GestionEffets.CréerPoussière(Position + vecteur, vecteur, 2, 120, 5, 8, GestionEffets.CouleursPierres, 3);
                    GestionEffets.CréerPoussière(Position, vecteur, 2, 15, 5, 20, GestionEffets.CouleursPierres, (int)(RayonDAction * 0.15f));
                }
            }
        }

        public void Affecter(bool blesserPropriétaire=true)
        {
            AffecterPersonnages(blesserPropriétaire);
            AffecterPolygonesLibres();
            AffecterBlocs();
        }

        public void AffecterPersonnages(bool blesserPropriétaire)
        {
            foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            {
                if (p == Propriétaire && blesserPropriétaire || p!=Propriétaire)
                {
                    if (Maths.IntersectionRayons(p.Position, Position, RayonAuCarré * 4))
                    {
                        p.Blesser(this);
                    }
                }
            }
        }
        public void AffecterPolygonesLibres()
        {
            foreach (PolygonePhysique p in GestionNiveaux.NiveauActif.ListePolygonesPhysiquesLibres)
            {
                if (Maths.IntersectionRayons(p.Position, Position, RayonAuCarré))
                {
                    float distance = Vector2.Distance(p.Position, Position);
                    int dégats = Maths.Clamp((int)((RayonDAction - distance) * Dégats * UnSurRayon), Dégats, 0);

                    p.Abimer(dégats*2);//on brise le polygone
                    //Et on lui ajoute une force en son centre
                    p.AddForce((p.Position - Position) * (RayonDAction - distance) * UnSurRayon * p.Masse * 0.002f * Dégats);
                }
            }
        }
        public void AffecterBlocs()
        {

            Point p1 = Bloc.CoordVersIndex(Position + Vector2.One * RayonDAction);
            Point p2 = Bloc.CoordVersIndex(Position + Vector2.One * -RayonDAction);

            //bornes inférieures
            int min_i = Math.Min(p1.X, p2.X) - 1;
            int min_j = Math.Min(p1.Y, p2.Y) - 1;

            //bornes supérieures
            int max_i = Math.Max(p1.X, p2.X) + 1;
            int max_j = Math.Max(p1.Y, p2.Y) + 1;

            Bloc bloc = null;

            for (int a = min_i; a < max_i; ++a)
            {
                for (int b = min_j; b < max_j; ++b)
                {
                    bloc = GestionNiveaux.NiveauActif.GetBloc(a, b);
                    if (bloc == null || bloc.Solidité > Puissance) { continue; }

                    float distance = Vector2.Distance(bloc.Position, Position);
                    int dégats = Maths.Clamp((int)((RayonDAction - distance) * Dégats * UnSurRayon), Dégats, 0);

                    bloc.Abimer(dégats * 2);//on brise le Bloc
                }
            }

        }
    }
}
