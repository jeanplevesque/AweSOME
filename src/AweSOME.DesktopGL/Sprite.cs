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
    public class Sprite
    {
        public SpriteEffects SpriteEffect;
        public Vector2 Position;
        public Vector2 Dimensions;
        public Vector2 Origine = Vector2.One / 2;
        public Vector2 OrigineSprite;
        public Vector2 Scale;
        public float Angle;
        public float Profondeur = 0.5f;

        public Color Couleur;
        public Texture2D Image = BanqueContent.Pixel;



        public Sprite()
        {          
            Couleur = Color.White;
            Scale = Vector2.One;
        }
        public Sprite(Vector2 position)
        {
            Couleur = Color.White;
            Position = position;
            Scale = Vector2.One;
        }
        public Sprite(Vector2 position,Vector2 dimensions)
        {
            Couleur = Color.White;
            Position = position;
            Dimensions = dimensions;
            Scale = Vector2.One;
        }

        protected Sprite(Sprite àCloner)
        {
            Position = àCloner.Position;
            OrigineSprite = àCloner.OrigineSprite;
            //Centre = àCloner.Centre;
            Scale = àCloner.Scale;
            Angle = àCloner.Angle;
            Profondeur = àCloner.Profondeur;
            Couleur = àCloner.Couleur;
            Origine = àCloner.Origine;
            Image = àCloner.Image;
        }

        public virtual Sprite Clone()
        {
            return new Sprite(this);
        }

        public Rectangle GetRectangle()
        {
            return new Rectangle((int)(Position.X-Image.Width * Scale.X/2), (int)(Position.Y-Image.Width * Scale.X/2), (int)(Image.Width * Scale.X), (int)(Image.Width * Scale.X));
        }

        /// <param name="origine">par rapport à un carré de 1 x 1 (pixel)</param>
        public void ArrangerSprite(Texture2D image,Vector2 dimensions,Vector2 origine, float profondeur,Color couleur)
        {
            Origine = origine;
            Couleur = couleur;
            Profondeur = profondeur;
            Image = image;
            Dimensions = dimensions;
            Scale = new Vector2(Dimensions.X / Image.Width, Dimensions.Y / Image.Height);
            OrigineSprite = new Vector2(Origine.X * Image.Width, Origine.Y * Image.Height);
        }
        public void ArrangerSprite(Texture2D image)
        {
            ArrangerSprite(image, Dimensions, Origine, Profondeur, Couleur);
        }
        public void ArrangerSprite()
        {
            ArrangerSprite(Image, Dimensions, Origine, Profondeur, Couleur);
        }

        public void ResizePourTexture(Texture2D image)
        {
            Dimensions.X = image.Width;
            Dimensions.Y = image.Height;
            ArrangerSprite(image);
        }
        public void ResizePourTexture()
        {
            Dimensions.X = Image.Width;
            Dimensions.Y = Image.Height;
            ArrangerSprite();
        }

        /*public virtual void Save(BinaryWriter writer)
        {
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Dimensions.X);
            writer.Write(Dimensions.Y);
            writer.Write(Angle);
            writer.Write(GestionTexture.GetNom(Image));
            writer.Write(GestionTexture.GetNom(NormalMap));
            writer.Write(Profondeur);
        }
        public static Sprite Load(BinaryReader reader)
        {
            Sprite sprite;

            sprite = new Sprite(new Vector2(reader.ReadSingle(), reader.ReadSingle()), new Vector2(reader.ReadSingle(), reader.ReadSingle()));
            
            sprite.Angle = reader.ReadSingle();
            sprite.ArrangerSprite(GestionTexture.GetTexture(reader.ReadString()));
            sprite.NormalMap = GestionTexture.GetTexture(reader.ReadString());
            sprite.Profondeur = reader.ReadSingle();

            return sprite;
        }*/

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //if (Image != null)
            {
                spriteBatch.Draw(Image,(Position), null, Couleur, Angle, OrigineSprite, Scale , SpriteEffect, Profondeur);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch,Vector2 position)
        {
            //if (Image != null)
            {
                spriteBatch.Draw(Image, (position), null, Couleur, Angle, OrigineSprite, Scale , SpriteEffect, Profondeur);
            }
        }
        //public virtual void DrawAvecParalax(SpriteBatch spriteBatch,float atténuation)
        //{
        //    if (Image != null)
        //    {
        //        spriteBatch.Draw(Image, Caméra.Transform(Position, atténuation), null, Couleur, Angle, OrigineSprite, Scale * Caméra.Zoom, SpriteEffect, Profondeur);
        //    }
        //}
        //public virtual void DrawNormalMap(SpriteBatch spriteBatch)
        //{
        //    if (NormalMap !=null)
        //    {
        //        spriteBatch.Draw(NormalMap, Caméra.Transform(Position), null, Color.White, Angle, OrigineSprite, Scale * Caméra.Zoom, SpriteEffect, Profondeur);
        //    }
        //}

        public static implicit operator Vector2(Sprite sprite)
        {
            return sprite.Position;
        }
    }
}
