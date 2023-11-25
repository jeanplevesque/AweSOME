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
    class AstreLumineux:Sprite
    {
        public SourceLumière Lumière;

        public Vector2 DistanceCentre;
        public Vector2 Rayons;
        public float ProgressionAngle;
        public float MaxIntensité;
        
        

        public void ChangerPosition(Vector2 position)
        {         
            Position = position;
            Lumière.Position = position;
        }
        public void ChangerPositionY(float y)
        {
            Position.Y = y;
            Lumière.Position = Position;
        }

        public void Update(float moment)
        {
            ProgressionAngle = moment * MathHelper.TwoPi;

            DistanceCentre.X = Rayons.X * (float)Math.Sin(ProgressionAngle);
            DistanceCentre.Y = Rayons.Y * (float)Math.Cos(ProgressionAngle);

            Position = GestionNiveaux.NiveauActif.CentreRotationAstres + DistanceCentre;
            Lumière.Position = Position;
        }


        //-------Classe Statique-------------

        public static AstreLumineux CréerSoleil(ref Vector2 dimensionsNiveau)
        {
            AstreLumineux astre = new AstreLumineux();
            astre.Lumière = SourceLumière.CréerSoleil();
            astre.MaxIntensité = astre.Lumière.BackLight.Intensity;
            astre.ArrangerSprite(GestionTexture.GetTexture("Particules/GlowFront"), new Vector2(1024), Vector2.One * 0.5f, 0.9f, Color.Orange);
            astre.Rayons = dimensionsNiveau * 1.1f;
            astre.Rayons.X *= -1f;
            //astre.ChangerPosition(new Vector2(positionX, 0));

            return astre;
        }
        public static AstreLumineux CréerLune(ref Vector2 dimensionsNiveau)
        {
            AstreLumineux astre = new AstreLumineux();
            astre.Lumière = SourceLumière.CréerLune();
            astre.MaxIntensité = astre.Lumière.BackLight.Intensity;
            astre.ArrangerSprite(GestionTexture.GetTexture("Particules/GlowFront"), new Vector2(512), Vector2.One * 0.5f, 0.9f, Color.GhostWhite);
            astre.Rayons = dimensionsNiveau * 1.1f;
            astre.Rayons.Y *= -1f;
            //astre.ChangerPosition(new Vector2(positionX, 0));

            return astre;
        }
    }
}
