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
    class AnimationFrame
    {
        public Vector2[] ListePositions;
        int index;

        public AnimationFrame(int nbPositions)
        {
            index = 0;
            ListePositions = new Vector2[nbPositions];
        }

        public void AjouterPosition(Vector2 pos)
        {
            ListePositions[index] = pos;
            ++index;
        }
        public void AjouterPosition(Vector2 pos,float grosseurÀNeutraliser)
        {
            ListePositions[index] = pos / grosseurÀNeutraliser;
            ++index;
        }
        public void AjouterPosition(float x, float y)
        {
            ListePositions[index] = new Vector2(x, y);
            ++index;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(ListePositions.Length);
            for (int i = 0; i < ListePositions.Length; ++i)
            {
                writer.Write(ListePositions[i].X);
                writer.Write(ListePositions[i].Y);
            }
        }
        public static AnimationFrame Load(BinaryReader reader)
        {
            AnimationFrame frame = new AnimationFrame(reader.ReadInt32());
            for (int i = 0; i < frame.ListePositions.Length; ++i)
            {
                frame.AjouterPosition(reader.ReadSingle(), reader.ReadSingle());
            }
            return frame;
        }
    }
}
