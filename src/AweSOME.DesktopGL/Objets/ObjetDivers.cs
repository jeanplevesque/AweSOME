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
using Krypton;

namespace AweSOME
{
    class ObjetDivers:PolygonePhysique,IRamassable
    {
        //========Implémentation de l'interface===========================
        public Vector2 GetPosition() { return Position; }
        public void SetPosition(Vector2 position) { Position = position; }
        public void SetAngle(float angle) { Angle = angle; }
        public void SetSpriteEffet(SpriteEffects effect) { SpriteEffect = effect; }
        public void SetProfondeur(float profondeur) { Profondeur = profondeur; }

        public virtual void SePlacerEnMain(PersonnageArmé propriétaire)
        {
            propriétaire.PlacerItemEnMain();
        }
        public virtual void EntréeInventaire()
        {
            SetAngle(0);
            SetSpriteEffet(SpriteEffects.None);
        }
        public virtual void SortieInventaire()
        {
        }
        public virtual void Utiliser(bool nouveauClic=false)
        {
            throw new NotImplementedException();
        }
        public virtual void UtiliserAlternatif()
        {
            throw new NotImplementedException();
        }
        public virtual void SeFaireRamasser(PersonnageArmé persoArmé)
        {
            persoArmé.ObtenirObjetDivers(this);
            EstFixe = true;
            GestionNiveaux.DétruireObjetRamassable(this);
        }
        public virtual void SeFaireLaisser()
        {
            EstFixe = false;
            NbCollisions = 0;
            GestionNiveaux.AjouterObjetRamassable(this);
        }
        public new void VérifierCollisionsDécors()
        {
            base.VérifierCollisionsDécors();
        }
        public new void VérifierCollisionsPersonnages()
        {
            base.VérifierCollisionsPersonnages();
        }
        //========Implémentation de l'interface===========================


        public ObjetDivers(Vector2 position, Vector2 dimensions)
            : base(position, dimensions) { }

    }
}
