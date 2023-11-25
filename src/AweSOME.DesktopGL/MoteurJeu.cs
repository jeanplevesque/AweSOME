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
    public class MoteurJeu : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice GraphicDevice;
        public static SpriteBatch SpriteBatchSc�ne;
        public static SpriteBatch SpriteBatchAdditive;
        public static SpriteBatch SpriteBatchHUD;

        public static KryptonEngine EnginKrypton;

        public static MoteurJeu This;
        public static GameTime TheGameTime;

        public static I�cran �cranPr�sent;

        static bool FocusFen�tre;
        public static TextBox AwesomeBox;

        public static void QuitterJeu()
        {
            This.Quitter();
        }
        public void Quitter()
        {
            Exit();
        }
        public MoteurJeu()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //IsFixedTimeStep = false;

            EnginKrypton = new KryptonEngine(this, "KryptonContent/KryptonEffect");
            Components.Add(EnginKrypton);
            This = this;

        }
        protected override void Initialize()
        {         
            base.Initialize();
            Cam�ra.SetDimensionsFen�tre(new Vector2(1600, 900));
            //graphics.IsFullScreen = true;
            //graphics.ApplyChanges();

            AwesomeBox = new TextBox(new Vector2(Cam�ra.DimensionsFen�tre.X-150,150), Vector2.One * 300);
           
            
            Gestion�cran.Cr�erMenus();
            //Gestion�cran.Cr�erPartieZombie();
            //Gestion�cran.Aller��cranPartieEnCours();

            //EnginKrypton.AmbientColor = new Color(5, 5, 5);
            //EnginKrypton.AmbientColor = new Color(150, 150, 150);
            EnginKrypton.CullMode = Krypton.CullMode.None;
            EnginKrypton.BlurFactorU = 1.0f / (GraphicsDevice.Viewport.Width * 0.075f);
            EnginKrypton.BlurFactorV = 1.0f / (GraphicsDevice.Viewport.Height * 0.075f);
            EnginKrypton.BlurEnable = true;
            EnginKrypton.Bluriness = 1.9f;
            EnginKrypton.LightMapSize = LightMapSize.Eighth;


           
        }

        protected override void LoadContent()
        {
            GraphicDevice = GraphicsDevice;
            SpriteBatchSc�ne = new SpriteBatch(GraphicsDevice);
            SpriteBatchAdditive = new SpriteBatch(GraphicsDevice);
            SpriteBatchHUD = new SpriteBatch(GraphicsDevice);

            GestionTexture.Content = Content;
            BanqueContent.Content = Content;

            BanqueContent.Pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            BanqueContent.Pixel.SetData<Color>(new Color[1] { Color.White });

            BanqueContent.Font1 = Content.Load<SpriteFont>("Fonts/Arial");
            
            BanqueContent.LoadFont("CourierNew");
            BanqueContent.LoadFont("Arial");
            BanqueContent.LoadFont("Copperplate");

            GestionTexture.AddTexture(BanqueContent.Pixel, "Pixel");
            for (int i = 0; i < 5; ++i)
            {
                GestionTexture.Load("Bloc/CaseGazon" + i.ToString());
                GestionTexture.Load("Bloc/CaseTerre" + i.ToString());
                GestionTexture.Load("Bloc/CaseM�tal" + i.ToString());
                GestionTexture.Load("Bloc/CasePierre" + i.ToString());
                GestionTexture.Load("Zombie/JambeZombie" + i.ToString());
                GestionTexture.Load("Zombie/BrasZombie" + i.ToString());
                GestionTexture.Load("Zombie/TorseZombie" + i.ToString());
                GestionTexture.Load("Zombie/T�teZombie" + i.ToString());

                GestionTexture.Load("Humain/JambeHumain" + i.ToString());
                GestionTexture.Load("Humain/BrasHumain" + i.ToString());
                GestionTexture.Load("Humain/TorseHumain" + i.ToString());
                //GestionTexture.Load("Humain/T�teHumain" + i.ToString());

                GestionTexture.Load("Particules/ParticuleTerre" + i.ToString());
                GestionTexture.Load("Particules/ParticulePierre" + i.ToString());
                GestionTexture.Load("Particules/Poussiere" + i.ToString());
                GestionTexture.Load("Particules/Sang" + i.ToString());
                GestionTexture.Load("Particules/Organe" + i.ToString());
                GestionTexture.Load("Particules/Feu" + i.ToString());
            }
            GestionTexture.Load("Humain/T�teHumain" + 0.ToString());
            GestionTexture.Load("Humain/T�teHumain" + 1.ToString());
            GestionTexture.Load("Humain/T�teHumain" + 2.ToString());
            
            GestionTexture.Load("Armes/Chat0");
            GestionTexture.Load("Armes/FisherPriceM4");
            GestionTexture.Load("Armes/FisherPriceM4FlashLight");
            GestionTexture.Load("Armes/GoldM1014");
            GestionTexture.Load("Armes/GoldM1014FlashLight");
            GestionTexture.Load("Armes/Blocgun0");
            GestionTexture.Load("Armes/Missile0");

            GestionTexture.Load("Armes/Pomegranate");
            GestionTexture.Load("Armes/Pear");
            GestionTexture.Load("Armes/Orange");

            GestionTexture.Load("Armes/WoodenStick0");

            GestionTexture.Load("Particules/�tincelleBleue");
            GestionTexture.Load("Particules/�tincelleJaune");
            GestionTexture.Load("Particules/GlowBack");
            GestionTexture.Load("Particules/GlowFront");
            GestionTexture.Load("Particules/Croix0");
            GestionTexture.Load("Particules/CroixGlow0");
            
            GestionTexture.Load("RectangleInventaire");
            GestionTexture.Load("Background0");
            GestionTexture.Load("Menus/AvE Wallpaper");

            GestionTexture.Load("Objets/Flare");
            GestionTexture.Load("Objets/Crate0");
            GestionTexture.Load("Textures/Lazer0");
            GestionTexture.Load("Textures/muzzletest");
            

        }

        protected override void UnloadContent()
        {

        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (FocusFen�tre)
            {
                TheGameTime = gameTime;

                GestionIntrants.Update();

                //AwesomeBox.WriteLine(TheGameTime.ElapsedGameTime.ToString());
                //if (GestionIntrants.EstNouvelleToucheEnfonc�e(Keys.LeftControl))
                //{
                //    AwesomeBox.WriteLine("Allo");
                //}
                //if (GestionIntrants.EstNouvelleToucheEnfonc�e(Keys.LeftAlt))
                //{
                //    AwesomeBox.Write("Allo1234567890123456789012345678901234567890");
                //}
                //if (GestionIntrants.EstNouvelleToucheEnfonc�e(Keys.LeftShift))
                //{
                //    //AwesomeBox.AllignerTexte();
                //}
                //if (GestionIntrants.EstNouvelleToucheEnfonc�e(Keys.CapsLock))
                //{
                //    AwesomeBox.CentrerTexteXY();
                //}
                //if (GestionIntrants.EstToucheEnfonc�e(Keys.Add))
                //{
                //    AwesomeBox.ChangerAngle(AwesomeBox.Angle + 0.01f);
                //}
                
                if (GestionIntrants.NouvellesTouchesDispo)
                {
                    AwesomeBox.Write(GestionIntrants.GetString());
                }
                if (GestionIntrants.EstToucheEnfonc�e(Keys.T) && gameTime.TotalGameTime.Milliseconds % 100 == 0)
                {
                    GestionEffets.AjouterEffetAdditiveBlend(new �claireQuiBlesse(GestionNiveaux.NiveauActif.Joueur.Position-Vector2.UnitY*200, GestionIntrants.PositionSourisCam�ra, 6, Maths.RandomColor,GestionNiveaux.NiveauActif.Joueur));
                }


                �cranPr�sent.Update();

                base.Update(gameTime);
            }
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            �cranPr�sent.Draw();
          
            AfficherStatsJeu(gameTime);
        }

        public void DrawPartie()
        {
            EnginKrypton.AmbientColor = GestionNiveaux.NiveauActif.Lumi�reAmbiante;
            EnginKrypton.Matrix = Cam�ra.MatriceKrypton;

            EnginKrypton.LightMapPrepare();

            GraphicsDevice.Clear(GestionNiveaux.NiveauActif.CouleurCiel);

            //On dessine la scene et les effets de bases
            //Matrix matricelol = new Matrix(1.0f, 0.2f, 0.0f, 0,
            //                               0.2f, 1.1f, 0.0f, 0,
            //                               0.0f, 0.0f, 1.2f, 0,
            //                               0.0f, 0.0f, 0.0f, 1);

            SpriteBatchSc�ne.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Cam�ra.Matrice);
            SpriteBatchAdditive.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Cam�ra.Matrice);
            //SpriteBatchHUD.Begin(SpriteSortMode.BackToFront, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null);

            GestionNiveaux.Draw(SpriteBatchSc�ne);
            GestionEffets.Draw(SpriteBatchSc�ne);

            SpriteBatchSc�ne.End();

            //Testing
            //GestionNiveaux.NiveauActif.AfficherTuile();

            base.Draw(TheGameTime);//C'est ici que l'on applique les ombres
            
            //On dessine les particules brillantes et le feu           
            GestionEffets.DrawAdditive(SpriteBatchAdditive);
            SpriteBatchAdditive.End();

            //DebugDraw();
            

            DessinerHUD();
            //GestionEffets.DrawHUD(SpriteBatchHUD);
            //SpriteBatchHUD.End();
        }

        private void DessinerHUD()
        {
            Player joueur = GestionNiveaux.NiveauActif.Joueur;

            SpriteBatchSc�ne.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null);
            joueur.Inventaire.Draw(SpriteBatchSc�ne);
            //spriteBatch.DrawString(BanqueContent.Font1, "Translation" + Cam�ra.Matrice.Translation.ToString() + "\n", Vector2.Zero, Color.White);
            /*if (joueur.Fusil != null)
            {
                SpriteBatchSc�ne.DrawString(BanqueContent.Font1, joueur.Fusil.Nom + "\n" +
                                                            joueur.Fusil.NbBallesDansChargeur.ToString()+ "\n" + 
                                                            joueur.Fusil.NbBallesTotal.ToString(), Vector2.UnitY * (Cam�ra.DimensionsFen�tre.Y - 100), Color.White);
            }
            SpriteBatchSc�ne.DrawString(BanqueContent.Font1, (GestionNiveaux.NiveauActif.MomentPr�sent.ToString())+
                                                              "\n � Faire : "+ GestionRounds.RoundPr�sent.NbZombies�Faire+
                                                              "\n Cr��s :" + GestionRounds.RoundPr�sent.NbZombiesCr�es+
                                                              "\n En Vie : " + GestionRounds.RoundPr�sent.NbZombiesEnVies, Vector2.UnitY * 50, Color.White, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

            SpriteBatchSc�ne.DrawString(BanqueContent.Font1, GestionNiveaux.NiveauActif.�tatJourn�e.ToString(), Vector2.Zero, Color.White);*/

            //AwesomeBox.Draw(SpriteBatchSc�ne);
            GestionEffets.DrawHUD(SpriteBatchSc�ne);


            
            SpriteBatchSc�ne.End();
 
        }
        private void DebugDraw()
        {
            // Clear the helpers vertices
            EnginKrypton.RenderHelper.ShadowHullVertices.Clear();
            EnginKrypton.RenderHelper.ShadowHullIndicies.Clear();

            foreach (var hull in EnginKrypton.Hulls)
            {
                EnginKrypton.RenderHelper.BufferAddShadowHull(hull);
            }

            EnginKrypton.RenderHelper.Effect.CurrentTechnique = EnginKrypton.RenderHelper.Effect.Techniques["DebugDraw"];

            foreach (var effectPass in EnginKrypton.RenderHelper.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                EnginKrypton.RenderHelper.BufferDraw();
            }
        }
        private void AfficherStatsJeu(GameTime gameTime)
        {
            if (gameTime.IsRunningSlowly)
            {
                //AwesomeBox.WriteLine("Running Slow " + (1.0 / gameTime.ElapsedGameTime.TotalSeconds).ToString() + " " + Maths.Random.Next(10).ToString());
                //GC.Collect();
            }

            //-------FPS
            float myFPS = ((int)((1.0 / gameTime.ElapsedGameTime.TotalSeconds) * 100) / 100.0f);
            string fpsString = "FPS : " + myFPS.ToString();
            Window.Title = "Test  " + fpsString + "    Polygones : " + GestionNiveaux.NiveauActif.NbPolygones +
                            "   Zoom : " + Cam�ra.Zoom.ToString() +
                            " Particules : " + GestionEffets.NbParticules.ToString() +
                            " Nb Balles : " + GestionNiveaux.NiveauActif.ListeBallesFusil.Count.ToString() +
                            " Blocs : " + GestionNiveaux.NiveauActif.ListeBloc�Dessiner.Count +
                            " Contacts : " + GestionContacts.NbContacts;
            //-------
        }
        
        protected override void OnDeactivated(object sender, EventArgs args)
        {
            base.OnDeactivated(sender, args);
            FocusFen�tre = false;
        }
        protected override void OnActivated(object sender, EventArgs args)
        {
            base.OnActivated(sender, args);
            FocusFen�tre = true;
        }
    }
}
