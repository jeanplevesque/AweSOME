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
using Krypton;
using Krypton.Lights;

namespace AweSOME
{
    class MagicWand:ArmeMelee
    {
        ISpell SpellDeBaguette
        {
            get { return spellDeBaguette_; }
            set
            {
                if (spellDeBaguette_ != null)
                {
                    spellDeBaguette_.Terminer();
                }
                spellDeBaguette_ = value;
            }
        }
        ISpell spellDeBaguette_;

        public override void Update()
        {
            if (MouvementAmorcé)
            {
                Color couleur;

                if (Maths.UneChanceSur((int)(10f * (Dégats - HoldingPower))))
                {
                    couleur = Maths.RandomColor;

                    GestionEffets.CréerMagie(PositionBout, VecteurPrincipal, 0.787978f, 180, 16, 32, ref couleur, 30 + (int)(20 * HoldingPower), 40);
                    if (MaxHolding && Maths.UneChanceSur(10))
                    {
                        GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(PositionBout, Maths.RandomVecteurUnitaire(), Dimensions.X * 2, 4, couleur, 0.3f));
                    }
                }

                couleur = new Color(100, 0, 100);
                GestionEffets.CréerCloneLumineux(this, 30, ref couleur);

            }

            base.Update();

            if (CoupDonné || AnimationTerminé)
            {
                CastSpell();
                FaireEffetVisuel();
            }

        }
        public override void UtiliserAlternatif()
        {
            if (SpellDeBaguette != null)
            {
                SpellDeBaguette.Terminer();
                SpellDeBaguette = null;
            }
        }

        private void CastSpell()
        {
            string texte = GestionIntrants.InputString;

            Vector2 distanceCibleBout = Propriétaire.Cible.Position - PositionBout;
            Vector2 vitesseBoule;
            Color couleur;
            Maths.Vecteur(Maths.CalculerAngleDunVecteur(Propriétaire.Cible-PositionBout), Dégats + HoldingPower,out vitesseBoule);
            
            texte = texte.ToUpper();

            if (texte.Contains("ADAD"))
            {
                SpellDeBaguette = MagicHelper.CastLumiereBaguette(Propriétaire);
                
            }
            else if(texte.Contains(" W W"))//Jump
            {
                Propriétaire.AddForce(Propriétaire.ImpulsionSaut*2f);
                Maths.MixerCouleurs(Color.Blue, Color.DarkTurquoise, Maths.RandomFloat(0.3f,0.5f), out couleur);
                GestionEffets.CréerMagie(Propriétaire.JambeDroite.ListeSommets[1].Position, Vector2.UnitX, 10, 30, 16, 24, ref couleur, 50, 60);
                GestionEffets.CréerMagie(Propriétaire.JambeGauche.ListeSommets[1].Position, -Vector2.UnitX, 10, 30, 16, 24, ref couleur, 50, 60);
                Maths.MixerCouleurs(Color.Blue, Color.DarkTurquoise, Maths.RandomFloat(0.5f,0.9f), out couleur);
                GestionEffets.CréerMagie(PositionBout, Vector2.UnitY, 10, 360, 20, 28, ref couleur, 20, 40);
                GestionEffets.CréerÉtincellesDoubleTexture(Propriétaire.Position+Vector2.UnitY*Propriétaire.Grosseur, Vector2.UnitY, 10, 90, 24, 32, ref couleur, 50);
            }
            else if (texte.Contains("AWDSA"))
            {
                MagicHelper.CastCercleProtection(Propriétaire, 110 * (Dégats + HoldingPower));
            }
            else if (texte.Contains("WSWSADDA"))
            {
                MagicHelper.CastTeleport(ref PositionBout, GestionIntrants.PositionSourisCaméra, Propriétaire, MaxHolding);
            }
            else if (texte.Contains("ASDWA"))
            {
                MagicHelper.CastHeal(Propriétaire, HoldingPower);
            }
            else if (texte.Contains("SUICIDE"))
            {
                Explosion explosion = new Explosion(PositionBout, 300, 90000, 130, 11, Propriétaire, true);
                explosion.Affecter(true);
                couleur = Color.Chartreuse;
                GestionEffets.CréerÉtincellesDoubleTexture(PositionBout, VecteurPrincipal, 30, 360, 24, 32, ref couleur, 90);
                GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(PositionBout, -VecteurPrincipal, 4f, 5, couleur));
                GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(PositionBout, -VecteurPrincipal, 4f, 5, couleur));
                GestionEffets.AjouterEffetAdditiveBlend(new Éclaire(PositionBout, -VecteurPrincipal, 5f, 5, couleur));
            }

            texte = texte.Replace(" ", "");
            texte = texte.Replace("A", "");
            texte = texte.Replace("W", "");
            texte = texte.Replace("S", "");
            texte = texte.Replace("D", "");

            int longueurTexte = texte.Length;

            if (longueurTexte > 1)//&&longueurTexte < 6)
            {
                switch (texte)
                {
                    //default:
                    //    vitesseBoule *= 0.125f;
                    //    MagicHelper.CastBoulePlasma(ref PositionBout, ref vitesseBoule, Propriétaire, PlasmaTypes.Fire);
                    //    break;
                    case "ZC":
                    case "CZ":
                        MagicHelper.CastBoulePlasma(ref PositionBout, ref vitesseBoule, Propriétaire, PlasmaTypes.Fire);
                        break;
                    case "ZCX":
                    case "CZX":
                        MagicHelper.CastBoulePlasma(ref PositionBout, ref vitesseBoule, Propriétaire, PlasmaTypes.Blue);
                        break;
                    case "ZCXX":
                    case "CZXX":
                        MagicHelper.CastBoulePlasma(ref PositionBout, ref vitesseBoule, Propriétaire, PlasmaTypes.Magma);
                        break;
                    case "ZCCC":
                    case "CZZZ":
                        MagicHelper.CastBoulePlasma(ref PositionBout, ref vitesseBoule, Propriétaire, PlasmaTypes.Green);
                        break;
                    case "ZCXC":
                    case "CZXZ":
                        MagicHelper.CastBoulePlasma(ref PositionBout, ref vitesseBoule, Propriétaire, PlasmaTypes.Violet);
                        break;
                    case "ZXCXZ":
                    case "CXZXC":
                        MagicHelper.CastÉclaires(ref PositionBout, ref vitesseBoule, Propriétaire, HoldingPower);
                        break;
                }
            }
            
            //if (longueurTexte < 3)
            //{
            //    Explosion e = new Explosion(PositionBout, 60, 3600, 300, 10, Propriétaire, true);
            //    e.Affecter();
            //}
            //else
            //{
            //    Missile m = Missile.CréerMissileBasique(ref PositionBout, Propriétaire.AngleBras, ref Propriétaire.BrasDroite.Matrice, Propriétaire);
            //    GestionNiveaux.AjouterPolygonePhysique(m);
            //}
        }

        private void FaireEffetVisuel()
        {

        }

        public override void SeFaireLaisser()
        {
            base.SeFaireLaisser();
            if (MaxHolding)
            {
                Explosion explosion = new Explosion(Position, 200, 40000, 100, 11, null, true);
                explosion.Affecter();
                Color couleur = Maths.RandomColor;
                GestionEffets.AjouterLumiereTempo(new LumièreTemporaire(60, 512, ref couleur, ref Position));
            }
        }
        

        protected override void DébuterFrappe()
        {
            base.DébuterFrappe();
            GestionIntrants.ResetInputString();
        }

        protected override void SurMaxHolding()
        {
            Color couleur = Color.Red;
            GestionEffets.CréerMagie(PositionBout, -Vector2.UnitY, 20, 60, 16, 32, ref couleur, 100, 40);
        }
    }
}
