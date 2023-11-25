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
    class Menu:Écran
    {
        public static Vector2 DimensionsBoutonsQuit = new Vector2(120, 60);
        public static Vector2 PositionBoutonQuit = new Vector2(Caméra.DimensionsFenêtre.X - DimensionsBoutonsQuit.X / 2, DimensionsBoutonsQuit.Y / 2);
        

        public Sprite Background;
        public List<Bouton> ListeBoutons = new List<Bouton>();
        public List<TextBox> ListeTextBox = new List<TextBox>();

        public bool ÉcrireNom
        {
            get { return écrireNom_; }
            set
            {
                écrireNom_ = value;
                if (écrireNom_)
                {
                    TextBox nom = new TextBox(Bouton.DIMENSIONS_BOUTONS*0.75f, Bouton.DIMENSIONS_BOUTONS);
                    nom.Font = BanqueContent.GetFont("Copperplate");
                    nom.Write(Nom);

                    ListeTextBox.Add(nom);
                }
            }
        }
        private bool écrireNom_;

        public Menu(Écran écranPrécédent,string nom, bool écrireNom=true)
            :base(écranPrécédent, nom)
        {
            Background = new Sprite(Caméra.DimensionsFenêtreSurDeux, Caméra.DimensionsFenêtre);
            Background.ArrangerSprite(GestionTexture.GetTexture("Menus/AvE Wallpaper"));
            Background.Couleur *= 0.5f;

            ÉcrireNom = écrireNom;

            Bouton boutonQuiter = new Bouton(PositionBoutonQuit, DimensionsBoutonsQuit, this, "Quit");
            boutonQuiter.Cliqué+=new Bouton.EventDeClic(MoteurJeu.QuitterJeu);
            ListeBoutons.Add(boutonQuiter);
        }

        public override void Update()
        {
            foreach (Bouton b in ListeBoutons)
            {
                b.Update();
            }
            //GestionEffets.CréerJetSang(GestionIntrants.PositionSouris, -Vector2.UnitY, 1, 20, 12, 18, 20);
            Color couleur = Maths.RandomColor;
            //GestionEffets.CréerFumée(GestionIntrants.PositionSouris, -Vector2.UnitY, 1.55115f, 360, 12, 24,ref couleur, 1);
            GestionEffets.CréerMagie(GestionIntrants.PositionSouris, -Vector2.UnitY, 1.55115f, 360, 12, 24, ref couleur, 10,30);
            GestionEffets.Update();
        }

        public override void Draw()
        {
            MoteurJeu.GraphicDevice.Clear(Color.Black);

            MoteurJeu.SpriteBatchScène.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null);
            MoteurJeu.SpriteBatchAdditive.Begin(SpriteSortMode.BackToFront, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null);
            
            Background.Draw(MoteurJeu.SpriteBatchScène);
            foreach (Bouton b in ListeBoutons)
            {
                b.Draw(MoteurJeu.SpriteBatchScène);
            }
            foreach (TextBox t in ListeTextBox)
            {
                t.Draw(MoteurJeu.SpriteBatchScène);
            }
            GestionEffets.Draw(MoteurJeu.SpriteBatchScène);
            GestionEffets.DrawAdditive(MoteurJeu.SpriteBatchAdditive);

            MoteurJeu.AwesomeBox.Draw(MoteurJeu.SpriteBatchScène);

            MoteurJeu.SpriteBatchScène.End();
            MoteurJeu.SpriteBatchAdditive.End();
        }
    }
}
