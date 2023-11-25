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
    class ObjetCollable:ObjetDivers
    {
        //public Bloc BlocSoutien;


        public ObjetCollable(Vector2 position, Vector2 dimensions)
            : base(position, dimensions) { }
    }
}
