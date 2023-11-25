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
    class Flare:ObjetDivers
    {
        public int DuréeVie;

        SourceLumière Lumière;
        Color CouleurBlanche = Color.White;
        Color CouleurNoire = Color.Black;
        Color NouvelleCouleur;
        bool EstDéclanché;
        bool EstFuckedUp;

        Vector2 distanceBoutInitiale_;
        Vector2 distanceBout_;
        Vector2 positionBout_;

        public Flare(Vector2 position)
            : base(position, new Vector2(30,8))
        {
            DuréeVie = 1800;
            Matériel = MatérielPolygone.Inconnu;
            distanceBoutInitiale_ = new Vector2(Dimensions.X / 2, 0);
            switch(Maths.Random.Next(9))
            {
                case 0:
                    Couleur = Color.Cyan;
                    break;
                case 1:
                    Couleur = Color.Red;
                    break;
                case 2:
                    Couleur = Color.Green;
                    break;
                case 3:
                    Couleur = Color.Orange;
                    break;
                case 4:
                    Couleur = Color.HotPink;
                    break;
                case 5:
                    Couleur = Color.Yellow;
                    break;
                case 6:
                    Couleur = Color.Chartreuse;
                    break;
                case 7:
                    Couleur = Color.RoyalBlue;
                    break;
                case 8:
                    Couleur = Color.Purple;
                    EstFuckedUp = true;
                    DuréeVie *= 2;
                    break;
            }
            Lumière = SourceLumière.CréerFlare(Couleur);
            Lumière.Fermer();
            ArrangerSprite(GestionTexture.GetTexture("Objets/Flare"));
        }

        public override void Update()
        {
            
            base.Update();
            if (EstDéclanché)
            {
                --DuréeVie;
                if (DuréeVie > 0)
                {
                    Vector2.Transform(ref distanceBoutInitiale_, ref Matrice, out distanceBout_);
                    positionBout_ = Position + distanceBout_;
                    Lumière.Position = positionBout_;
                    Lumière.Angle = Angle;

                    Maths.ModifierCouleur(ref Couleur, 50, out NouvelleCouleur);
                    Lumière.BackLight.Color = NouvelleCouleur;
                    Lumière.BackLight.Range += Maths.Random.Next(21) - 10;
                    Lumière.SpotLight.Range += Maths.Random.Next(21) - 10;

                    GestionEffets.CréerMagie(positionBout_, distanceBout_, 0.457f, 120, 32, 48, ref NouvelleCouleur, 50,60);
                    GestionEffets.CréerFumée(positionBout_, distanceBout_, 0.317f, 90, 16, 20, ref CouleurBlanche, 1);
                    GestionEffets.CréerFumée(positionBout_, distanceBout_, 0.335f, 90, 16, 20, ref NouvelleCouleur, 1);
                    GestionEffets.CréerÉtincelles(positionBout_, distanceBout_, 0.676f, 90, 12, 20, "ÉtincelleJaune", 50);
                    if (DuréeVie < 180)
                    {
                        Lumière.SpotLight.Range -= 3;
                        Lumière.BackLight.Range -= 3;
                    }

                    if (EstFuckedUp)
                    {
                        AddForceAuPoint(-distanceBout_ * 1.5f, positionBout_);
                        if (Maths.Random.Next(100) == 0)
                        {
                            AddForceAuPoint(-distanceBout_ * 50f, positionBout_);
                            GestionEffets.CréerFumée(positionBout_, distanceBout_, 5f, 90, 24, 32, ref CouleurNoire, 3);
                            GestionEffets.CréerMagie(positionBout_, distanceBout_, 40, 270, 36, 64, ref NouvelleCouleur, 80, 120);
                            NbCollisions = 0;
                        }
                    }
                }
                else
                {
                    Lumière.Fermer();
                    Lumière.Supprimer();
                    GestionNiveaux.DétruireObjetRamassable(this);
                }
            }
        }
        public override void SeFaireRamasser(PersonnageArmé persoArmé)
        {
            Lumière.OnOff(persoArmé.Inventaire.EstPlein);
            base.SeFaireRamasser(persoArmé);
            
        }
        public override void Utiliser(bool nouveauClic=false)
        {
            EstDéclanché = true;
            Lumière.Ouvrir();
        }
        public override void EntréeInventaire()
        {
            Scale *= 2;
            base.EntréeInventaire();
        }
        public override void SortieInventaire()
        {
            Scale /= 2;
            base.SortieInventaire();
        }
    }
}
