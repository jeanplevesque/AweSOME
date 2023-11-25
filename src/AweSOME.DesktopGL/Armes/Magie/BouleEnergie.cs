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
    abstract class BouleEnergie : Spell
    {
        public PersonnageArmé Propriétaire;

        public int Dégats;
        public int Puissance;

        public float Rayon=30;
        public float RayonExplosion;

        public Vector2 Position;
        public Vector2 Vitesse;

        public bool DoitExploser;

        public BouleEnergie()
        {
        }
        public BouleEnergie(Vector2 position,Vector2 vitesse,float rayon,float rayonExplosion,int dégats,PersonnageArmé propriétaire,bool doitExploser)
        {
            Position = position;
            Vitesse = vitesse;
            Rayon = rayon;
            RayonExplosion = rayonExplosion;
            Dégats = dégats;
            Propriétaire = propriétaire;
            DoitExploser = doitExploser;
        }

        public override void Déclancher()
        {
            base.Déclancher();
        }

        public override void Updater()
        {
            Bouger();
            ÉmettreParticules();
            if (VérifierCollisions() && DoitExploser)
            {
                Terminer();
                Position -= Vitesse;
            }

            base.Updater();
        }

        public override void Terminer()
        {
            Exploser();
            
            base.Terminer();
        }


        protected void Bouger()
        {
            Position += Vitesse;
        }
        protected virtual void ÉmettreParticules()
        {
            
        }
        protected bool VérifierCollisions()
        {
            bool collision = false;

            PolygonePhysique poly;
            if (VérifierCollisionsDécors(out poly))
            {
                collision = true;
                SurCollisionDécors(poly);               
            }

            Bone boneTouché;
            if (VérifierCollisionsPersonnages(out boneTouché))
            {
                collision = true;
                SurCollisionPerso(boneTouché);               
            }
            return collision;
        }
        protected bool VérifierCollisionsDécors(out PolygonePhysique poly)
        {
            poly = GestionNiveaux.NiveauActif.GetBloc(Position);
            return poly != null && !((Bloc)poly).EstTunel;
        }
        protected bool VérifierCollisionsPersonnages(out Bone boneTouché)
        {
            boneTouché = null;

            foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            {
                if (p != Propriétaire && Maths.IntersectionRayons(Position,Rayon,p.Position,p.Rayon))
                {
                    foreach (Bone b in p.ListeBones)
                    {
                        if (!b.EstBrisé)
                        {
                            if(b.EstÀIntérieur(Position))
                            {
                                boneTouché = b;

                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        protected virtual void SurCollisionDécors(PolygonePhysique polyTouché)
        {
            if (polyTouché != null && Puissance >= polyTouché.Solidité)
            {
                polyTouché.Abimer(Dégats * 2);
            }
        }
        protected virtual void SurCollisionPerso(Bone boneTouché)
        {
            boneTouché.Parent.Blesser(this, boneTouché);
        }

        protected virtual void Exploser()
        {
            Explosion explosion = new Explosion(Position, RayonExplosion, RayonExplosion * RayonExplosion, Dégats, Puissance, Propriétaire, false);
            explosion.Affecter(false);
        }
    }
}
