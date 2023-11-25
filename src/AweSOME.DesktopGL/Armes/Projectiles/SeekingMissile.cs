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
    class SeekingMissile:Missile
    {
        public PolygonePhysique Target;
        public float AngleEntreTarget;
        public float DifférenceDAngle;
        public float DifférenceAngle;
        public Vector2 DistanceTournée;

        public float RayonCarré;

        public int TempsDépart=60;

        public SeekingMissile(ref Vector2 position, ref Vector2 dimensions, int dégats, float rayonExplosion, float forceMoteur, ref Color couleurFeu, PersonnageArmé propriétaire, PolygonePhysique target)
            : base(ref position, ref dimensions, dégats, rayonExplosion, forceMoteur, ref couleurFeu, propriétaire)
        {
            Target = target;
            Feu.DésactiverFumée();
            Feu.DuréeFlame = 15;
            Feu.NbParticuleParFrame = 1;
            Feu.AngleOuverture = 30;
            Feu.CouleurBaseFumée2 = Color.White;
            Feu.CouleurBaseFumée1 = Color.DarkGray;

            TempsMax = 1800;
            RayonCarré = RayonExplosion * RayonExplosion;
            //DampingLinéaire = 0.97f;
        }

        public override void Update()
        {
            --TempsDépart;

            //Target.Position = GestionIntrants.PositionSourisCaméra;

            base.Update();
        }

        public override void AppliquerForces()
        {

            Vector2.Transform(ref DistanceMoteurInitiale, ref Matrice, out DistanceMoteur);
            Vector2.Transform(ref DistanceMoteur, ref Maths.MatriceRotation90, out DistanceTournée);
            PositionMoteur = Position + DistanceMoteur;
            AngleEntreTarget = Maths.CalculerAngleEntreDeuxPosition(Position, Target.Position);
            DifférenceAngle = AngleEntreTarget - Angle;
            DifférenceDAngle = (DifférenceAngle + MathHelper.TwoPi) % MathHelper.TwoPi;

            if (TempsDépart <= 0)
            {
                if (Math.Abs(DifférenceDAngle) > MathHelper.PiOver4 * 0.1f)
                {
                    if (DifférenceDAngle < MathHelper.Pi)
                    {
                        Angle += MathHelper.Clamp(0.01f * Vitesse.Length(), 0, 0.08f);
                        Vector2.Transform(ref DistanceMoteur, ref Maths.MatriceRotation270, out Feu.Direction);
                    }
                    else
                    {
                        Angle -= MathHelper.Clamp(0.01f * Vitesse.Length(), 0, 0.08f);
                        Vector2.Transform(ref DistanceMoteur, ref Maths.MatriceRotation90, out Feu.Direction);
                    }

                    AddForce(-Vitesse * ForceMoteur*12.5f);

                }
                else
                {
                    AddForceAuPoint(DistanceMoteur * -ForceMoteur, PositionMoteur);
                    Feu.Direction = DistanceMoteur;
                }
            }
            else
            {
                AddForceAuPoint(DistanceMoteur * -ForceMoteur*2, PositionMoteur);
            }
            Feu.Update(ref PositionMoteur);
            AddForceAuPoint(DistanceMoteur * -ForceMoteur, PositionMoteur);

            if (Vector2.DistanceSquared(Target.Position, Position) < RayonCarré*0.3f)
            {
                Exploser();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Target.Position);
            base.Draw(spriteBatch);
        }
    }
}
