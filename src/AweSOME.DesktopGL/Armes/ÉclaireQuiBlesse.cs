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
    class ÉclaireQuiBlesse:Éclaire
    {
        public PersonnageArmé Propriétaire;
        public int Dégats = 75;


        public ÉclaireQuiBlesse(Vector2 posDébut, Vector2 directionUnitaire, float longueur, int nbGénérations, Color couleurGlow, PersonnageArmé propriétaire, float offsetOrigine = 0.25f, bool vérifierCollisions = true, bool vérifierCollisionsDécors = true)
            : base(posDébut, posDébut + directionUnitaire * longueur, nbGénérations, couleurGlow, offsetOrigine, false, vérifierCollisions, vérifierCollisionsDécors)
        {
            Propriétaire = propriétaire;
            Créer();
        }
        public ÉclaireQuiBlesse(Vector2 posDébut, Vector2 posFin, int nbGénérations, Color couleurGlow, PersonnageArmé propriétaire, float offsetOrigine = 0.25f, bool vérifierCollisions = true, bool vérifierCollisionsDécors = true)
            : base(posDébut, posFin, nbGénérations, couleurGlow, offsetOrigine, false, vérifierCollisions, vérifierCollisionsDécors)
        {
            Propriétaire = propriétaire;
            Créer();
        }

        public override void VérifierLesCollisions()
        {
            base.VérifierLesCollisions();

            VérifierCollisionsPersonnages();
        }

        protected void VérifierCollisionsPersonnages()
        {
            Bone bone = null;

            foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            {
                if (p != Propriétaire && p.VérifierCollisionsÉclaire(this, out bone))
                {
                    p.Blesser(this, bone);

                    GestionEffets.CréerÉtincellesDoubleTexture(bone.Position, -Vector2.UnitY, 20, 60, 20, 28, ref Brillance.Couleur, 60);
                    GestionEffets.CréerMagie(bone.Position, -Vector2.UnitY, 20, 180, 16, 24, ref Brillance.Couleur, 60, 60);

                    GestionEffets.CréerSang(bone.Position, VecteurPrincipal);

                    GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(bone.Position, Vector2.UnitY, p.Dimensions.Y * 2, 4, Brillance.Couleur, 0.255f));


                    //GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(p.Tête.Position, p.JambeDroite.ListeSommets[1].Position, 3, Brillance.Couleur, 0.175f, true, false));
                    //GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(p.Tête.Position, p.JambeGauche.ListeSommets[1].Position, 3, Brillance.Couleur, 0.175f, true, false));
                    //GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(p.Position, p.BrasDroite.ListeSommets[1].Position, 3, Brillance.Couleur, 0.175f, true, false));
                    //GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(p.Position, p.BrasGauche.ListeSommets[1].Position, 3, Brillance.Couleur, 0.175f, true, false));
                }
            }
        }
    }
}
