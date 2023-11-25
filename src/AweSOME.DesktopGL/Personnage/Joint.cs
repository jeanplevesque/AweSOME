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
    class Joint : Sommet
    {
        //public float Masse
        //{
        //    get { return masse_; }
        //    set
        //    {
        //        masse_ = value;
        //        MasseInverse = 1 / masse_;
        //    }
        //}
        //float masse_;
        //public float MasseInverse { get; protected set; }
        public Vector2 Vitesse;
        //public Vector2 ForceAccumulée;
        public Vector2 PositionVoulue;
        //public Vector2 DeltaPositionVoulue;

        public Vector2 AnciennePositionVoulue;

        public Joint(Vector2 position)
            : base(position)
        {
            PositionVoulue = Position;
            AnciennePositionVoulue = Position;
        }
        public Joint(Vector2 position, Vector2 distanceCentrePolygone,PolygonePhysique parent)
            : base(position, distanceCentrePolygone,parent)
        {
            PositionVoulue = Position;
            AnciennePositionVoulue = Position;
        }

        //public void Update()
        //{
            //Vitesse = ForceAccumulée * MasseInverse;
            //Position += Vitesse;
            //ClearForceAccumulée();
        //}
    }
}
