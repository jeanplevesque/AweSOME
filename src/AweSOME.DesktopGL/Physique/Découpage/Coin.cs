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
    class Coin:IComparable
    {
        public Vector2 Position;
        public Vector2 DistanceCentre;
        public float AngleCentre;
        public bool EngendréParCoupeuse;

        public Coin(Vector2 position, Vector2 positionCentre,bool engendréParCoupeuse)
        {
            Position = position;
            DistanceCentre = Position - positionCentre;
            AngleCentre = (float)Math.Atan2(DistanceCentre.X, DistanceCentre.Y) + MathHelper.TwoPi;
            EngendréParCoupeuse = engendréParCoupeuse;
        }

        public int CompareTo(object obj)
        {
            return AngleCentre.CompareTo(((Coin)obj).AngleCentre);
        }
    }
}
