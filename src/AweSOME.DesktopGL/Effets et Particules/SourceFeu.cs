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
using Krypton.Lights;
using Krypton;

namespace AweSOME
{
    class SourceFeu
    {
        public Vector2 Position;
        public Vector2 Direction = -Vector2.UnitY;

        public Color CouleurBaseFumée1 = Color.Black;
        public Color CouleurBaseFumée2 = Color.Gray;
        public Color CouleurBase;
        public Color NouvelleCouleur;

        public SourceLumière Lumière;
        public int MinGrosseur = 12;
        public int MaxGrosseur = 20;
        public int AngleOuverture = 200;
        public int VitesseParticuleX10=30;
        public float NbParticuleParFrame=2f;
        public int DuréeFlame = 60;

        public bool EstActif { get; private set; }
        public bool ÉmetFumée { get; private set; }

        public SourceFeu(Color couleurBase)
        {
            EstActif = true;
            ÉmetFumée = true;

            CouleurBase = couleurBase;

            Lumière = SourceLumière.CréerFeu(CouleurBase);
        }

        public void Détruire()
        {
            Lumière.Supprimer();
        }
        public void Désactiver(bool désactiverFumée)
        {
            EstActif = false;
            ÉmetFumée = !désactiverFumée;
        }
        public void Activer(bool activerFumée)
        {
            EstActif = true;
            ÉmetFumée = activerFumée;
        }
        public void ActiverFumée()
        {
            ÉmetFumée = true;
        }
        public void DésactiverFumée()
        {
            ÉmetFumée = false;
        }

        public void Update(ref Vector2 position)
        {
            Position = position;
            Lumière.Position = Position;
            if (EstActif)
            {
                Maths.ModifierCouleur(ref CouleurBase, 50, out NouvelleCouleur);

                GestionEffets.CréerFeu(Position, Direction, NbParticuleParFrame * 0.51718f, AngleOuverture, MinGrosseur, MaxGrosseur, ref NouvelleCouleur, VitesseParticuleX10, DuréeFlame);
                GestionEffets.CréerParticuleDoubleTextureFeu(Position, Direction, NbParticuleParFrame * 0.51123f, AngleOuverture, MinGrosseur, MaxGrosseur, ref NouvelleCouleur, VitesseParticuleX10, DuréeFlame);
                Lumière.BackLight.Color = NouvelleCouleur;

                if (ÉmetFumée)
                {
                    Maths.MixerCouleurs(ref CouleurBaseFumée1, ref CouleurBaseFumée2, (float)Maths.Random.Next(100)/100f, out NouvelleCouleur);

                    GestionEffets.CréerFumée(ref Position, ref Direction, 1, 120, 12, 24, ref NouvelleCouleur, 2, 120, 80);
                }
            }
        }
    }
}
