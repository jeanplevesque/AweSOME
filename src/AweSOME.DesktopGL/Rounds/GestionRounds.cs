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
    static class GestionRounds
    {
        public static Round RoundPrésent;
        public static int IndexRound;

        public static void Reset()
        {
            IndexRound = 0;
        }

        public static void Update()
        {
            RoundPrésent.Update();
        }

        public static void PasserAuProchainRound()
        {
            ++IndexRound;
            RoundPrésent = new Round(IndexRound);
            RoundPrésent.Start();
        }

    }
}
