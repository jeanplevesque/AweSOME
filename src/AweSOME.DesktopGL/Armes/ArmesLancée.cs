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
    enum TypeArmesLancées { Bombe, Cluster }

    class ArmeLancée:Arme
    {
        public const int FPS = 60;

        public float RayonAuCarré{ get; set; }
        public float RayonDAction
        {
            get { return rayonAction_; }
            set
            {
                rayonAction_ = value;
                RayonAuCarré = rayonAction_ * rayonAction_;
            }
        }
        private float rayonAction_;

        public TypeArmesLancées Type { get; set; }

        public int TicsRestants { get; set; }

        private bool estActivée_;

        public ArmeLancée(Vector2 position, Vector2 dimensions, int dégats, PersonnageArmé propriétaire, TypeArmesLancées type, float rayon)
            :base(position, dimensions, dégats, propriétaire)
        {
            Type = type;
            RayonDAction = rayon;
            RayonAuCarré = RayonDAction * RayonDAction;
        }
        public ArmeLancée() { Restitution = 0.3f; }

        public override void Utiliser(bool nouveauClic=false)
        {
            Activer();
        }

        public override void Update()
        {
            base.Update();

            if (estActivée_)
            {
                --TicsRestants;
                if (TicsRestants <= 0)
                {
                    Déclancher();
                }
            }
        }
        public void Activer()
        {
            estActivée_ = true;
        }

        protected virtual void Déclancher()
        {
            //Do something
            switch (Type)
            {
                case TypeArmesLancées.Bombe:
                    Explosion explosion = new Explosion(Position, RayonDAction, RayonAuCarré, Dégats,Puissance, Propriétaire,true);
                    explosion.Affecter();
                    break;
                case TypeArmesLancées.Cluster:
                    explosion = new Explosion(Position, RayonDAction, RayonAuCarré, Dégats,Puissance, Propriétaire,true);
                    explosion.Affecter();

                    GénérerCluster();
                    break;
            }
            

            //Et disparait
            GestionNiveaux.DétruirePolygonePhysique(this);
            GestionNiveaux.DétruireObjetRamassable(this);
        }

        private void GénérerCluster()
        {
            throw new NotImplementedException();
        }

        public override void SeFaireRamasser(PersonnageArmé persoArmé)
        {
            persoArmé.RamasserArmeLancée(this);
            base.SeFaireRamasser(persoArmé); //Enleve l'objet de la liste d'objets ramassable
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(BanqueContent.Font1, TicsRestants.ToString(), Position - Vector2.UnitY * 30, Color.White);
        }
    }
}
