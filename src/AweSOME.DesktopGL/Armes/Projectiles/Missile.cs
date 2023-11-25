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
    class Missile:Arme
    {
        protected SourceFeu Feu;
        protected float ForceMoteur;
        protected float RayonExplosion;

        public Vector2 DistanceMoteurInitiale;
        public Vector2 DistanceMoteur;
        public Vector2 PositionMoteur;

        public int TempsMax=180;

        public Missile(ref Vector2 position, ref Vector2 dimensions, int dégats, float rayonExplosion, float forceMoteur, ref Color couleurFeu, PersonnageArmé propriétaire)
            : base(position,dimensions,dégats,propriétaire)
        {
            DégatsMax = 50;
            DernierPropriétaire = Propriétaire;
            Feu = new SourceFeu(couleurFeu);
            Feu.DuréeFlame = 30;
            Feu.Update(ref Position);
            RayonExplosion = rayonExplosion;
            ForceMoteur = forceMoteur;

            DistanceMoteurInitiale = new Vector2(-Dimensions.X / 2, 0);

            ArrangerSprite(GestionTexture.GetTexture("Bloc/CaseMétal0"));
        }

        public override void Update()
        {
            if (TempsMax <= 0)
            {
                Exploser();
            }
            else
            {
                AppliquerForces();

                base.Update();

                --TempsMax;

                VérifierCollisionsPolygones();
            }
        }

        public virtual void AppliquerForces()
        {
            Vector2.Transform(ref DistanceMoteurInitiale, ref Matrice, out DistanceMoteur);

            PositionMoteur = Position + DistanceMoteur;
            AddForceAuPoint(DistanceMoteur * -ForceMoteur, PositionMoteur);

            Feu.Direction = DistanceMoteur;
            Feu.Update(ref PositionMoteur);
        }

        public void Exploser()
        {
            Explosion explosion = new Explosion(Position, RayonExplosion, RayonExplosion * RayonExplosion, Dégats, 10, Propriétaire,false);
            explosion.Affecter();
            Feu.Détruire();
            GestionNiveaux.DétruirePolygonePhysique(this);
            GestionEffets.CréerFeu(Position, -Vector2.UnitY, 50, 360, 16, 24, ref Feu.CouleurBase, 30, 100);
            GestionEffets.CréerParticuleDoubleTextureFeu(Position, -Vector2.UnitY, 50, 360, 16, 24, ref Feu.CouleurBase, 30, 100);
            GestionEffets.CréerFumée(ref Position, ref DistanceMoteur, 50, 360, 28, 40, ref Feu.CouleurBaseFumée1,ref Feu.CouleurBaseFumée2, 3, 180, 120);
            GestionEffets.AjouterLumiereTempo(new LumièreTemporaire(120, (int)RayonExplosion * 8, ref Feu.CouleurBase, ref Position));
        }
        public override void Détruire()
        {
            base.Détruire();
            Exploser();
        }
        


        //--------------Fabrique-------------

        public static Missile CréerMissileBasique(ref Vector2 position, float angle, ref Matrix matriceRotation, PersonnageArmé propriétaire)
        {
            Vector2 dimensions = new Vector2(60, 20);
            Color couleur = Color.OrangeRed;
            Missile missile = new Missile(ref position, ref dimensions, 60, 175, 10f, ref couleur, propriétaire);

            Vector2 unitx = Vector2.UnitX*2;
            Vector2.Transform(ref unitx, ref matriceRotation, out missile.Vitesse);
            missile.Orienter(ref matriceRotation, angle);
            missile.ArrangerSprite(GestionTexture.GetTexture("Armes/Missile0"));

            GestionNiveaux.AjouterPolygonePhysique(missile);

            return missile;
        }

        public static SeekingMissile CréerSeekingMissile(ref Vector2 position, float angle, ref Matrix matriceRotation, PersonnageArmé propriétaire, PolygonePhysique target)
        {
            Vector2 dimensions = new Vector2(70, 20);
            Color couleur = Color.Turquoise;
            SeekingMissile missile = new SeekingMissile(ref position, ref dimensions, 60, 175, 3f, ref couleur, propriétaire, target);

            Vector2 unitx = Vector2.UnitX * 3;
            Vector2.Transform(ref unitx, ref matriceRotation, out missile.Vitesse);
            missile.Orienter(ref matriceRotation, angle);
            missile.ArrangerSprite(GestionTexture.GetTexture("Armes/Missile0"));

            GestionNiveaux.AjouterPolygonePhysique(missile);

            return missile;
        }

    }
}
