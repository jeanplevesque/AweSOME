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
    class BoneÀunJoint:Bone
    {
        public BoneÀunJoint(Joint joint1, Vector2 dimensions,int nbSommets, Personnage parent)
            : base(joint1, new Joint(joint1.Position + Vector2.UnitX * dimensions.X), dimensions.Y, parent, PartiesCorps.Tête)
        {
            Rayon=Dimensions.X/2;
            Dimensions = dimensions;
            GénérerRégulier(nbSommets);
            RelierSommets(1);
        }

        public override void UpdateParJoint()
        {
            AnciennePosition = Position;
            AncienAngle = Angle;

            Position = Joint1.Position;

            Vitesse = Position - AnciennePosition;
            VitesseAngulaire = Angle - AncienAngle;
            VitesseAngulaire %= MathHelper.TwoPi;

            Orienter();
        }
    }
}
