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
    class SpellLumièreBaguette:SpellPermanent
    {
        public static Vector2 DifférencePosition = Vector2.UnitY * -100;

        static Color couleur;

        PersonnageArmé Propriétaire;
        SourceLumière Lumière;
        public CouleurQuiChange CouleurChangeante;
        

        public SpellLumièreBaguette(PersonnageArmé propriétaire)
        {
            CouleurChangeante = new CouleurQuiChange(Maths.RandomColor,60);
            Propriétaire = propriétaire;
            Lumière = SourceLumière.CréerFlare(CouleurChangeante);
            Lumière.SupprimerSpotLight();
        }

        public override void Updater()
        {
            Lumière.Position = Propriétaire.Position + DifférencePosition;
            couleur = CouleurChangeante;
            Lumière.BackLight.Color = couleur;

            if (Maths.UneChanceSur(50))
            {
                GestionEffets.CréerMagie(Lumière.Position, -Vector2.UnitY, 1.123456f, 90, 20, 28, ref couleur, 30, 60);
            }

        }

        public override void Terminer()
        {
            Lumière.Supprimer();
            base.Terminer();
        }
    }
}
