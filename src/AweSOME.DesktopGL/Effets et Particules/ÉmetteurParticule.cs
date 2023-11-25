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
    class ÉmetteurParticule:Particule
    {
        public Particule ParticuleÀÉmettre;
        public float cptFloat;
        public float NbÉmissions;
        public int VitesseMaxX10;

        Matrix Matrice;

        public ÉmetteurParticule(int tempsMax, int tempsFin, Particule particuleÀÉmettre, int vitesseMaxX10)
            : base(tempsMax, tempsFin)
        {
            ParticuleÀÉmettre = particuleÀÉmettre;
            VitesseMaxX10 = vitesseMaxX10;
            Couleur = Color.Red;
            Dimensions *= 10;
        }

        public override void Update()
        {
            EnvoyerParticules();
            base.Update();           
        }

        public virtual void EnvoyerParticules()
        {
            cptFloat += NbÉmissions;
            if (cptFloat > 1)
            {
                cptFloat -= (int)cptFloat;

                float grosseur;
                float deltaAngle;

                for (int i = 0; i < NbÉmissions; ++i)
                {
                    grosseur = Maths.Random.Next((int)ParticuleÀÉmettre.Dimensions.X - 2, (int)ParticuleÀÉmettre.Dimensions.X + 2);
                    deltaAngle = MathHelper.ToRadians(Maths.Random.Next(360));

                    Matrice = Matrix.CreateRotationZ(deltaAngle);

                    if (ParticuleÀÉmettre is ParticuleDoubleTexture)
                    {

                        ParticuleDoubleTexture p = new ParticuleDoubleTexture(Maths.Random.Next(ParticuleÀÉmettre.TempsRestant / 2, ParticuleÀÉmettre.TempsRestant), ParticuleÀÉmettre.TempsRestant / 4);

                        //Angle = angleNormal + deltaAngle,
                        //AngleÉgalDirection = true,

                        p.Position = this.Position;
                        p.Dimensions = new Vector2(grosseur, grosseur);

                        p.Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(9, VitesseMaxX10) / 10f;
                        p.VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f);
                        p.Accélération = Vector2.UnitY * 0.05f;

                        p.AtténuationVitesse = ParticuleÀÉmettre.AtténuationVitesse;
                        p.AtténuationVitesseRotation = ParticuleÀÉmettre.AtténuationVitesseRotation;

                        p.MutiplicateurDimensions = ParticuleÀÉmettre.MutiplicateurDimensions;
                        p.AtténuationMutiplicateurDimensions = ParticuleÀÉmettre.AtténuationMutiplicateurDimensions;

                        p.Couleur = Maths.ModifierCouleur(ParticuleÀÉmettre.Couleur, 75);



                        //p.Profondeur -= ListeParticules.Count * 0.0001f;
                        p.ArrangerSprite();

                        GestionEffets.AjouterEffetAdditiveBlend(p);
                    }
                    //else
                    //{
                    //    Particule p = new Particule(Maths.Random.Next(ParticuleÀÉmettre.TempsRestant / 2, ParticuleÀÉmettre.TempsRestant), ParticuleÀÉmettre.TempsRestant / 4)
                    //    {
                    //        //Angle = angleNormal + deltaAngle,
                    //        //AngleÉgalDirection = true,

                    //        Position = this.Position,
                    //        Dimensions = new Vector2(grosseur, grosseur),

                    //        Vitesse = Vector2.Transform(Vector2.UnitX, Matrice) * Maths.Random.Next(9, VitesseMaxX10) / 10f,
                    //        VitesseRotation = MathHelper.ToRadians(Maths.Random.Next(30) - 15f),
                    //        Accélération = Vector2.UnitY * 0.05f,

                    //        AtténuationVitesse = ParticuleÀÉmettre.AtténuationVitesse,
                    //        AtténuationVitesseRotation = ParticuleÀÉmettre.AtténuationVitesseRotation,

                    //        MutiplicateurDimensions = ParticuleÀÉmettre.MutiplicateurDimensions,
                    //        AtténuationMutiplicateurDimensions = ParticuleÀÉmettre.AtténuationMutiplicateurDimensions,

                    //        Couleur = ParticuleÀÉmettre.Couleur

                    //    };

                    //    //p.Profondeur -= ListeParticules.Count * 0.0001f;
                    //    p.ArrangerSprite();

                    //    GestionEffets.AjouterParticule(p);
                    //}

                }

            }
        }
    }
}
