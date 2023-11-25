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
    interface IRamassable
    {
        Vector2 GetPosition();
        void SetPosition(Vector2 position);
        void SetAngle(float angle);
        void SetSpriteEffet(SpriteEffects effect);
        void SetProfondeur(float profondeur);

        void SePlacerEnMain(PersonnageArmé propriétaire);
        void Utiliser(bool nouveauClic=false);
        void UtiliserAlternatif();
        void SeFaireRamasser(PersonnageArmé persoArmé);
        void SeFaireLaisser();
        void Update();
        void Draw(SpriteBatch spriteBatch);
        void Draw(SpriteBatch spriteBatch, Vector2 position);
        void VérifierCollisionsDécors();
        void VérifierCollisionsPersonnages();
        void EntréeInventaire();
        void SortieInventaire();
    }

    class Arme:PolygonePhysique,IRamassable
    {
        #region Interface IRamassable
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
            //throw new NotImplementedException();
        }
        public virtual void UtiliserAlternatif()
        {
            //throw new NotImplementedException();
        }
        public virtual void SeFaireRamasser(PersonnageArmé persoArmé)
        {
            EstFixe = true;
            GestionNiveaux.DétruireObjetRamassable(this);
        }
        public virtual void SeFaireLaisser()
        {
            EstFixe = false;
            NbCollisions = 0;
            GestionNiveaux.AjouterObjetRamassable(this);
        }       
        public new virtual void VérifierCollisionsDécors()
        {
            base.VérifierCollisionsDécors();
        }
        public new void VérifierCollisionsPersonnages()
        {
            base.VérifierCollisionsPersonnages();
        }
        //========Implémentation de l'interface===========================
        #endregion

        public string Nom;
        public string NomTexture;
        public int Dégats;
        public PersonnageArmé Propriétaire;
        public int Puissance;

        public Arme(Vector2 position, Vector2 dimensions, int dégats, PersonnageArmé propriétaire)
            : base(position,dimensions)
        {
            Dégats = dégats;
            Propriétaire = propriétaire;
            Matériel = MatérielPolygone.Métal;
        }
        public Arme()
        {
            Matériel = MatérielPolygone.Métal;
        }

        //public override void Update()
        //{
        //    base.Update();
        //    if (VérifierHorsNiveau())
        //    {
        //        GestionNiveaux.NiveauActif.DétruireObjetRamassable(this);
        //    }
        //}
        public override void ForceDelete()
        {
            GestionNiveaux.NiveauActif.DétruireObjetRamassable(this);
        }

        
    }
}
