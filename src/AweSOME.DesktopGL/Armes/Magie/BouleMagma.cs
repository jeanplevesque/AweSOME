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
    class BouleMagma:BoulePlasma
    {
        public BouleMagma(ref Vector2 position, ref Vector2 vitesse, PersonnageArmé propriétaire, PlasmaTypes typePlasma)
            :base(ref position, ref vitesse, propriétaire, typePlasma)
        {

        }


        protected override void SurCollisionDécors(PolygonePhysique polyTouché)
        {
            //base.SurCollisionDécors(polyTouché);
            if (polyTouché is Bloc)
            {
                GestionNiveaux.NiveauActif.CréerTunel(Position);
                GestionEffets.CréerEffetDestructionPolygone(polyTouché);
            }
        }

        protected override void SurCollisionPerso(Bone boneTouché)
        {
            base.SurCollisionPerso(boneTouché);
            Terminer();
        }
    }
}
