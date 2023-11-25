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

namespace AwesomeAnimation
{
    static class GestionAnimation
    {
        public static Animation Load(string nom)
        {
            FileStream file = new FileStream("Animations/"+ nom + ".aa", FileMode.Open);
            BinaryReader reader = new BinaryReader(file);

            Animation animation = Animation.Load(reader);

            reader.Close();
            file.Close();

            return animation;
        }
        public static void Save(Animation animation)
        {
            FileStream file = new FileStream("Animations/" + animation.Nom + ".aa00", FileMode.Create);
            BinaryWriter writer = new BinaryWriter(file);

            animation.Save(writer);

            writer.Close();
            file.Close();
        }
    }
}
