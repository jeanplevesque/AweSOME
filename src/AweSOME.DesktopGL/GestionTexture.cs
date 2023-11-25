using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AweSOME
{
    static class GestionTexture
    {
        public static ContentManager Content;
        //static List<ÉlémentTexture> ListeTextures = new List<ÉlémentTexture>();
        static Dictionary<string, ÉlémentTexture> BanqueTextures = new Dictionary<string, ÉlémentTexture>();

        public static void Load(string nom)
        {
            AddTexture(Content.Load<Texture2D>(nom), nom);            
        }
        public static void AddTexture(Texture2D texture,string nom)
        {
            if (!Contient(nom))
            {
                //ListeTextures.Add(new ÉlémentTexture(texture, nom, ListeTextures.Count));
                BanqueTextures.Add(nom, new ÉlémentTexture(texture, nom, BanqueTextures.Count));
            }
        }
        //public static int GetIndex(Texture2D texture)
        //{
        //    foreach (ÉlémentTexture é in ListeTextures)
        //    {
        //        if (é.Texture == texture)
        //        {
        //            return é.Index;
        //        }
        //    }
        //    return 0;
        //}
        public static Texture2D GetTexture(string nom)
        {
            ÉlémentTexture élémentTexture=BanqueTextures["Pixel"];
            BanqueTextures.TryGetValue(nom, out élémentTexture);
            return élémentTexture.Texture;

            //foreach (ÉlémentTexture é in ListeTextures)
            //{
            //    if (é.Nom == nom)
            //    {
            //        return é.Texture;
            //    }
            //}
            //return ListeTextures[0].Texture;
        }
        //public static Texture2D GetTexture(int index)
        //{
        //    return ListeTextures[(int)Math.Abs(index) % ListeTextures.Count].Texture;
        //}
        //public static ÉlémentTexture GetÉlémentTexture(int index)
        //{
        //    return ListeTextures[(int)Math.Abs(index) % ListeTextures.Count];
        //}
        //public static string GetNom(Texture2D texture)
        //{
        //    foreach (ÉlémentTexture é in ListeTextures)
        //    {
        //        if (é.Texture == texture)
        //        {
        //            return é.Nom;
        //        }
        //    }
        //    return ListeTextures[0].Nom;
        //}
        public static bool Contient(string nom)
        {
            return BanqueTextures.ContainsKey(nom);
            //foreach (ÉlémentTexture é in ListeTextures)
            //{
            //    if (é.Nom == nom)
            //    {
            //        return true;
            //    }
            //}
            //return false;
        }
    }
    class ÉlémentTexture
    {
        public Texture2D Texture;
        public string Nom;
        public int Index;

        public ÉlémentTexture(Texture2D texture, string nom, int index)
        {
            Texture = texture;
            Nom = nom;
            Index = index;
        }
        public static implicit operator string(ÉlémentTexture é)
        {
            return é.Nom;
        }

    }
}
