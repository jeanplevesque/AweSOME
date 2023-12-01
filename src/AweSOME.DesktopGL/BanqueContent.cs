using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Globalization;


namespace AweSOME
{
    static class BanqueContent
    {
        static Dictionary<string, SpriteFont> MapStringFont = new System.Collections.Generic.Dictionary<string, SpriteFont>();

        public static ContentManager Content;
        
        public static Texture2D Pixel;
        //public static Texture2D KryptonTexture;
        public static SpriteFont Font1;

        public static void LoadFont(string nom)
        {
            AjouterFont(nom, Content.Load<SpriteFont>("Fonts/" + nom));
        }
        public static void AjouterFont(string nom, SpriteFont font)
        {
            MapStringFont.Add(nom, font);
        }

        public static SpriteFont GetFont(ref string nom)
        {
            return MapStringFont[nom];
        }
        public static SpriteFont GetFont(string nom)
        {
            return MapStringFont[nom];
        }




        public static void SaveVector2(BinaryWriter writer, Vector2 vecteur)
        {
            writer.Write(vecteur.X);
            writer.Write(vecteur.Y);
        }
        public static void SaveVector2(StreamWriter writer, Vector2 vecteur)
        {
            writer.WriteLine(vecteur.X);
            writer.WriteLine(vecteur.Y);
        }
        public static Vector2 LoadVector2(BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }
        public static Vector2 LoadVector2(StreamReader reader)
        {
            return new Vector2(float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture), float.Parse(reader.ReadLine(), CultureInfo.InvariantCulture));
        }
    }
}
