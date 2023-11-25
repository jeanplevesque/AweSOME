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
    enum BlocGunMode { Create=0, Teleport=1, Tunel=2, Shoot=3}

    class BlocGun:Fusil
    {
        public BlocGunMode ModePrésent;
        public BlocGunMode ModePrécédent; 
        public Bloc BlocTenu;
        

        public BlocGun()
        {
            //ModePrésent = BlocGunMode.Tunel;
            ModePrésent = BlocGunMode.Teleport;
            Flash.Image = BanqueContent.Pixel;
            Flash.Couleur = Color.Black;
        }

        public override void Update()
        {
            base.Update();

            UpdateBlocTenu();

            if (CptTempsTir != 0)
            {
                CptTempsTir += (int)ModePrésent;
            }
        }

        public void ChangerMode()
        {
            ++ModePrésent;
            ModePrésent = (BlocGunMode)((int)ModePrésent%(int)BlocGunMode.Shoot);
        }

        public void RecevoirBloc(Bloc bloc)
        {
            BlocTenu = bloc;
            GestionNiveaux.NiveauActif.DétruireBloc(bloc);

            ModePrécédent = ModePrésent;
            ModePrésent = BlocGunMode.Shoot;
        }

        public override void Tirer(bool nouveauTir)
        {
            if ((nouveauTir || Type == TypeFusil.Automatique || ModePrésent== BlocGunMode.Shoot) && CptTempsTir == 0 && NbBallesDansChargeur > 0 && !GestionNiveaux.NiveauActif.BlocPrésentNonTunel(Position))
            {
                ++CptTempsTir;
                TirerSelonMode();
            }
        }

        protected void TirerSelonMode()
        {
            switch (ModePrésent)
            {
                case BlocGunMode.Create:
                    TirerEnCreate();
                    break;
                case BlocGunMode.Teleport:
                    TirerEnTeleport();
                    break;
                case BlocGunMode.Tunel:
                    TirerEnTunel();
                    break;
                case BlocGunMode.Shoot:
                    EnvoyerBloc();
                    break;
            }
        }
        protected void TirerEnCreate()
        {
            //int ballesRequises = BlocTenu.Solidité * 2;
            int ballesRequises = 10;
            if (NbBallesDansChargeur >= ballesRequises)
            {
                NbBallesDansChargeur -= ballesRequises;

                BlocTenu = new Bloc(PositionEmbout, MatérielPolygone.Métal);

                GestionEffets.CréerÉtincelles(PositionEmbout, Vector2.UnitX, 15, 360, 20, 24, "ÉtincelleBleue", 30);
                Flash.Allumer(4);

                ModePrécédent = ModePrésent;
                ModePrésent = BlocGunMode.Shoot;
            }
        }
        protected void TirerEnTeleport()
        {
            if (NbBallesDansChargeur >= 5)
            {
                NbBallesDansChargeur -= 5;

                BalleBlocGun balle = new BalleBlocGun(PositionEmbout, Angle + AngleImprécision, Dégats, Puissance, LongueurBalles, CouleurBalles, this, Propriétaire);
                GestionNiveaux.NiveauActif.AjouterBalleFusil(balle);
                Flash.Allumer(4);
            }
        }
        protected void TirerEnTunel()
        {
            if (NbBallesDansChargeur >= 5)
            {
                NbBallesDansChargeur -= 5;

                BalleBlocGun balle = new BalleBlocGun(PositionEmbout, Angle + AngleImprécision, Dégats, Puissance, LongueurBalles, CouleurBalles, this, Propriétaire);
                GestionNiveaux.NiveauActif.AjouterBalleFusil(balle);
                Flash.Allumer(4);
            }
        }
        protected void UpdateBlocTenu()
        {
            if (BlocTenu != null)
            {
                BlocTenu.Position = PositionEmbout;
                BlocTenu.Angle += 0.03f;
            }
        }
        protected void EnvoyerBloc()
        {
            if (BlocTenu != null)
            {
                --NbBallesDansChargeur;

                BlocTenu.Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * 10;
                BlocTenu.AddTorque(300000);
                //BlocTenu.ÉtatPrésent = ÉtatsBloc.PhysiquePourColler;
                GestionNiveaux.NiveauActif.AjouterBlocPhysiqueCollante(BlocTenu);
                BlocTenu = null;

                ModePrésent = ModePrécédent;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (BlocTenu != null)
            {
                BlocTenu.Draw(spriteBatch);
            }

            spriteBatch.DrawString(BanqueContent.Font1, this.ModePrésent.ToString(), PositionLazer + new Vector2(50, -50), Color.Green,0,Vector2.Zero,1.5f, SpriteEffects.None,0);
        }
    }
}
