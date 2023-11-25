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
    class Écran:IÉcran
    {
        public Écran ÉcranPrécédent;
        public string Nom;

        public Écran(Écran écranPrécédent,string nom)
        {
            Nom = nom;
            ÉcranPrécédent = écranPrécédent;
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
