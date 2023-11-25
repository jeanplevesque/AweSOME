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
    static class MagicHelper
    {
        public static BoulePlasma CastBoulePlasma(ref Vector2 position, ref Vector2 vitesse, PersonnageArmé Propriétaire, PlasmaTypes typePlasma)
        {
            BoulePlasma boule;
            if (typePlasma == PlasmaTypes.Magma)
            {
                boule = new BouleMagma(ref position, ref vitesse, Propriétaire, typePlasma);
            }
            else
            {
                boule = new BoulePlasma(ref position, ref vitesse, Propriétaire, typePlasma);
            }
            GestionNiveaux.NiveauActif.AjouterISpell(boule);
            return boule;
        }

        public static SpellLumièreBaguette CastLumiereBaguette(PersonnageArmé propriétaire)
        {
            SpellLumièreBaguette spell = new SpellLumièreBaguette(propriétaire);
            GestionNiveaux.NiveauActif.AjouterISpell(spell);
            GestionEffets.CréerÉtincellesDoubleTexture(propriétaire.MainDroite.Position, -Vector2.UnitY, 20, 230, 24, 32, ref spell.CouleurChangeante.CouleurBase, 60);
            return spell;
        }

        public static ÉclaireQuiBlesse CastÉclaires(ref Vector2 position, ref Vector2 direction, PersonnageArmé propriéraire, float bonusPuissance)
        {
            Color couleur = new Color(bonusPuissance, 0, 3f * (1f - bonusPuissance));
            ÉclaireQuiBlesse éclaire = new ÉclaireQuiBlesse(position, direction, 256f + 256f * bonusPuissance, 6, couleur, propriéraire, 0.25f - 0.1f * bonusPuissance, false, true);
            éclaire.Dégats = (int)(50f + 75f * bonusPuissance);
            éclaire.VérifierLesCollisions();
            GestionEffets.AjouterEffetAdditiveBlend(éclaire);
            GestionEffets.CréerÉtincellesDoubleTexture(position, direction, 20, 230, 24, 32, ref couleur, 60);
            GestionEffets.CréerMagie(position, direction, 20, 360, 24, 32, ref couleur, 60, 60);
            return éclaire;
        }

        public static void CastCercleProtection(PersonnageArmé Propriétaire, float rayon = 100f)
        {
            Color couleur;
            Maths.MixerCouleurs(Color.DarkGreen,Color.DarkTurquoise,Maths.RandomFloat(0.3f,0.7f),out couleur);
            MagicHelper.CastCercleÉlectrique(Propriétaire, 6, couleur, rayon);

            GestionEffets.CréerCercleExplosion(rayon, Propriétaire.Position, ref couleur);
            Explosion explosion = new Explosion(Propriétaire, rayon, rayon * rayon, 30, 3, Propriétaire, false);
            explosion.Affecter(false);

            GestionEffets.AjouterLumiereTempo(new LumièreTemporaire(90, (int)(rayon * 4), ref couleur, ref Propriétaire.Position));
        }

        public static void CastCercleÉlectrique(PersonnageArmé Propriétaire, int nbCôtés, Color couleurÉclaires,float rayon=100f)
        {
            Vector2[] sommets = new Vector2[nbCôtés+1];
            Vector2 distance = Vector2.UnitY * -rayon;
            Matrix matrice;
            Matrix.CreateRotationZ(MathHelper.TwoPi / nbCôtés, out matrice);

            sommets[0] = distance;
            for (int i = 1; i <= nbCôtés; ++i)
            {
                Vector2.Transform(ref sommets[i - 1], ref matrice, out sommets[i]);
                GestionEffets.AjouterEffetAdditiveBlend(new ÉclaireQuiBlesse(Propriétaire + sommets[i - 1], Propriétaire + sommets[i], 3, couleurÉclaires, Propriétaire, 0.2f, true,false));
            }
        }

        public static void CastTeleport(ref Vector2 positionBout, Vector2 positionVoulue, PersonnageArmé propriétaire, bool maxHolding)
        {
            Color couleur = Color.DarkViolet;
            Maths.ModifierCouleur(ref couleur, 100, out couleur);

            Tuile tuileVoulue = GestionNiveaux.NiveauActif.GetTuile(positionVoulue);

            GestionEffets.CréerÉtincellesDoubleTexture(positionBout, Vector2.UnitY, 15, 360, 24, 32, ref couleur, 60);


            if (tuileVoulue != null && tuileVoulue.EstPassable(3) || maxHolding)
            {
                Vector2 posHaut = propriétaire.Position + Vector2.UnitY * propriétaire.Grosseur * -2.25f;

                GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(posHaut, Vector2.UnitY, propriétaire.Grosseur * 4, 5, couleur));//, 0.25f, true, false, false));
                GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(posHaut, Vector2.UnitY, propriétaire.Grosseur * 4, 5, couleur));//, 0.25f, true, false, false));

                GestionEffets.CréerMagie(propriétaire.Position, Vector2.UnitY, 15, 360, 24, 32, ref couleur, 60, 60);
                GestionEffets.CréerFumée(propriétaire.Position + Vector2.UnitY * propriétaire.Grosseur * 1.1f, -Vector2.UnitY, 15, 120, 25, 32, ref couleur, 2);

                GestionEffets.CréerCloneLumineux(propriétaire, 60, ref couleur);

                propriétaire.AjusterPosition(positionVoulue - propriétaire);

                Vector2 posHaut2 = propriétaire.Position + Vector2.UnitY * propriétaire.Grosseur * -2.25f;

                GestionEffets.CréerÉtincellesDoubleTexture(positionVoulue, Vector2.UnitY, 15, 360, 24, 32, ref couleur, 60);
                GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(posHaut, posHaut2, 6, couleur, 0.25f, true, false, false));

                GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(posHaut2, Vector2.UnitY, propriétaire.Grosseur * 8, 5, couleur));//, 0.25f, true, false, false));
                GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(posHaut2, Vector2.UnitY, propriétaire.Grosseur * 8, 5, couleur));//, 0.25f, true, false, false));

            }
            if (maxHolding && tuileVoulue!=null)
            {
                for (int i = -1; i < 2; ++i)
                {
                    for (int j = -1; j < 2; ++j)
                    {
                        GestionNiveaux.NiveauActif.CréerTunel(new Point(tuileVoulue.X + i, tuileVoulue.Y + j));
                    }
                }
            }
        }

        public static void CastHeal(PersonnageArmé propriétaire, float holdingPower)
        {
            int pointsVie = (int)(holdingPower * propriétaire.VieMax);

            propriétaire.Heal(pointsVie);
            propriétaire.HealBones(pointsVie);

            Color couleur = Color.Red;
            GestionEffets.CréerCroixHeal(propriétaire.Position + Vector2.UnitY * -25, -Vector2.UnitY, 5, 270, 16, 24, ref couleur, 50, 45);
            GestionEffets.CréerCroixHeal(propriétaire.Position + Vector2.UnitY * 25, -Vector2.UnitY, 5, 270, 16, 24, ref couleur, 50, 45);
            GestionEffets.CréerCroixHeal(propriétaire, -Vector2.UnitY, 5, 270, 16, 24, ref couleur, 50, 45);
            GestionEffets.AjouterLumiereTempo(new LumièreTemporaire(45, 1024, ref couleur, ref propriétaire.Position));
        }
    }
}
