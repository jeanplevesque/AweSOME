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
using Krypton.Lights;
using System.IO;

namespace AweSOME
{
    class SourceLumière
    {
        public static Texture2D Texture360= LightTextureBuilder.CreatePointLight(MoteurJeu.GraphicDevice, 512);

        //List<PointLight> ListeLumière = new List<PointLight>();
        public float Angle
        {
            get { return angle_; }
            set
            {
                angle_ = value;
                SpotLight.Angle = angle_;
                BackLight.Angle = angle_;
            }
        }
        protected float angle_;
        
        public Vector2 Position
        {
            get { return position_; }
            set
            {
                position_ = value;
                SpotLight.Position = position_;
                BackLight.Position = position_;
            }
        }
        protected Vector2 position_;


        public PointLight SpotLight;
        public PointLight BackLight;

        public Color Couleurs
        {
            get { return couleurs_; }
            set
            {
                couleurs_ = value;
                SpotLight.Color = couleurs_;
                BackLight.Color = couleurs_;
            }
        }
        protected Color couleurs_;

        
        public SourceLumière()
        {
            SpotLight = new PointLight();
            BackLight = new PointLight();
            MoteurJeu.EnginKrypton.Lights.Add(SpotLight);
            MoteurJeu.EnginKrypton.Lights.Add(BackLight);


            //SpotLight.Texture = BanqueContent.KryptonTexture;
            SpotLight.Texture = Texture360;
            //BackLight.Texture = BanqueContent.KryptonTexture;
            BackLight.Texture = Texture360;


        }

        public void OnOff(bool ouvert)
        {
            SpotLight.IsOn = ouvert;
            BackLight.IsOn = ouvert;
        }
        public void ToggleOnOff()
        {
            SpotLight.IsOn = !SpotLight.IsOn;
            BackLight.IsOn = !BackLight.IsOn;
        }
        public void Fermer()
        {
            SpotLight.IsOn = false;
            BackLight.IsOn = false;
        }
        public void Ouvrir()
        {
            SpotLight.IsOn = true;
            BackLight.IsOn = true;
        }
        public void Supprimer()
        {
            MoteurJeu.EnginKrypton.Lights.Remove(SpotLight);
            MoteurJeu.EnginKrypton.Lights.Remove(BackLight);
        }
        public void SupprimerSpotLight()
        {
            MoteurJeu.EnginKrypton.Lights.Remove(SpotLight);
        }


        public static SourceLumière CréerFlashLight()
        {
            SourceLumière source = new SourceLumière();

            source.BackLight.Fov = MathHelper.TwoPi;
            source.BackLight.Intensity = 0.5f;
            source.BackLight.Range = 1024;

            source.SpotLight.Fov = MathHelper.ToRadians(30);
            source.SpotLight.Intensity = 0.8f;
            source.SpotLight.Range = 1536;

            return source;
        }
        public static SourceLumière CréerFeu(Color couleur)
        {
            SourceLumière source = new SourceLumière();

            source.BackLight.Fov = MathHelper.TwoPi;
            source.BackLight.Intensity = 0.9f;
            source.BackLight.Range = 768;

            source.SpotLight.Fov = MathHelper.TwoPi;
            source.SpotLight.Intensity = 0.4f;
            source.SpotLight.Range = 512;

            source.BackLight.Color = couleur;

            return source;
        }
        public static SourceLumière CréerLumièreExplosion(Color couleur)
        {
            SourceLumière source = new SourceLumière();

            source.BackLight.Fov = MathHelper.TwoPi;
            source.BackLight.Intensity = 1f;
            source.BackLight.Range = 1024;

            source.SpotLight.Fov = MathHelper.TwoPi;
            source.SpotLight.Intensity = 0.8f;
            source.SpotLight.Range = 768;

            source.BackLight.Color = couleur;

            return source;
        }
        public static SourceLumière CréerFlare(Color couleur)
        {
            SourceLumière source = new SourceLumière();

            source.BackLight.Fov = MathHelper.TwoPi;
            source.BackLight.Intensity = 1f;
            source.BackLight.Range = 512;

            source.SpotLight.Fov = MathHelper.TwoPi;
            //source.SpotLight.Intensity = 1;
            source.SpotLight.Range = 768;

            source.Couleurs = couleur;

            return source;
        }
        public static SourceLumière CréerSpotLight()
        {
            SourceLumière source = new SourceLumière();

            source.BackLight.Fov = MathHelper.TwoPi;
            source.BackLight.Intensity = 0.6f;
            source.BackLight.Range = 1024;

            source.SpotLight.Fov = MathHelper.ToRadians(100);
            //source.SpotLight.Intensity = 1;
            source.SpotLight.Range = 1024;

            return source;
        }
        public static SourceLumière CréerSoleil()
        {
            SourceLumière source = new SourceLumière();

            source.BackLight.Fov = MathHelper.TwoPi;
            source.BackLight.Intensity = 1f;
            source.BackLight.Range = 16384;

            source.SupprimerSpotLight();

            return source;
        }
        public static SourceLumière CréerLune()
        {
            SourceLumière source = new SourceLumière();

            source.BackLight.Fov = MathHelper.TwoPi;
            source.BackLight.Intensity = 0.4f;
            source.BackLight.Range = 16384;

            source.SupprimerSpotLight();

            return source;
        }

    }
}
