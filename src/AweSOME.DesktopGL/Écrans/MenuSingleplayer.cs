using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AweSOME
{
    class MenuSingleplayer:Menu
    {

        public MenuSingleplayer(Écran écranPrécédent, string nom)
            : base(écranPrécédent, "SinglePlayer" + nom)
        {

        }
    }
}
