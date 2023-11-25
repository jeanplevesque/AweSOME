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
    class Particule:Sprite,IEffet
    {
        public Vector2 Vitesse;
        public float AtténuationVitesse=1;
        public Vector2 Accélération;

        public float VitesseRotation;
        public float AtténuationVitesseRotation=1;

        public float MutiplicateurDimensions=1;
        public float AtténuationMutiplicateurDimensions = 1;
        public float Alpha=1;
        public int TempsRestant;
        public int TempsFin;

        public bool AngleÉgalDirection;
        public bool ClampAtténuationScale;
        public bool AffecterParVent;

        public Particule(Vector2 position,Vector2 dimensions, Vector2 vitesse, Vector2 accélération,float atténuationVitesse,int tempsMax, int tempsFin,Texture2D image)
            :base(position,dimensions)
        {
            Vitesse = vitesse;
            Accélération = accélération;
            AtténuationVitesse = atténuationVitesse;
            TempsRestant = tempsMax;
            TempsFin = tempsFin;

            ArrangerSprite(image);
        }

        public Particule(int tempsMax, int tempsFin)
        {
            ClampAtténuationScale = true;
            TempsRestant = tempsMax;
            TempsFin = tempsFin;
            Profondeur = 0.2f;
        }
        public Particule(int tempsMax, int tempsFin, Sprite spriteDeBase)
        {
            ClampAtténuationScale = true;
            TempsRestant = tempsMax;
            TempsFin = tempsFin;
            Profondeur = 0.2f;

            Position = spriteDeBase.Position;
            ArrangerSprite(spriteDeBase.Image, spriteDeBase.Dimensions, spriteDeBase.Origine, spriteDeBase.Profondeur, spriteDeBase.Couleur);
            Angle = spriteDeBase.Angle;
            SpriteEffect = spriteDeBase.SpriteEffect;
        }
        public Particule(int tempsMax, int tempsFin, Sprite spriteDeBase, ref Color couleur)
        {
            ClampAtténuationScale = true;
            TempsRestant = tempsMax;
            TempsFin = tempsFin;
            Profondeur = 0.2f;

            Position = spriteDeBase.Position;
            ArrangerSprite(spriteDeBase.Image, spriteDeBase.Dimensions, spriteDeBase.Origine, spriteDeBase.Profondeur, couleur);
            Angle = spriteDeBase.Angle;
            SpriteEffect = spriteDeBase.SpriteEffect;
        }

        
        public virtual void Update()
        {
            --TempsRestant;
            Position += Vitesse;

            Vitesse *= AtténuationVitesse;

            Vitesse += Accélération;

            if (AffecterParVent)
            {
                Vitesse += GestionEffets.Vent;
            }
            if (AngleÉgalDirection)
            {
                Angle = Maths.CalculerAngleDunVecteur(Vitesse);
            }
            else
            {
                Angle += VitesseRotation;
                VitesseRotation *= AtténuationVitesseRotation;
            }        

            Dimensions *= MutiplicateurDimensions;
            MutiplicateurDimensions *= AtténuationMutiplicateurDimensions;
            if (ClampAtténuationScale)
            {
                MutiplicateurDimensions = MathHelper.Clamp(MutiplicateurDimensions, 1, MutiplicateurDimensions);
            }

            if (TempsRestant < TempsFin)
            {
                Alpha = (float)TempsRestant / (float)TempsFin;
                if (TempsRestant <= 0)
                {
                    Détruire();
                }
            }
            if (MutiplicateurDimensions != 1)//on ne divisera pas pour rien
            {
                ArrangerSprite();
            }
        }
        public virtual void Détruire()
        {
            GestionEffets.DétruireEffet(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Image, Position, null, Couleur*Alpha, Angle, OrigineSprite, Scale, SpriteEffect, Profondeur);           
        }
    }
}
