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
    class ArmeMelee:Arme
    {
        public bool Holding;
        public bool MouvementAmorcé;
        public bool CoupDonné;
        public bool AnimationTerminé;
        public bool MaxHolding
        {
            get { return maxHolding_; }
            set
            {
                if (value != maxHolding_ && value)
                {
                    SurMaxHolding();
                }
                maxHolding_ = value;
            }
        }
        protected bool maxHolding_;

        public Vector2 VecteurPrincipal { get { return ListeDroites[3].VecteurPrincipal; } }
        public Vector2 PositionBout; //{ get { return ListeSommets[1].Position; } }
        public Vector2 VitesseBout;

        public Vector2 DeltaPosition;
        public Vector2 DeltaPositionOrigine;

        public float HoldingPower;
        public float DeltaHolding;

        public ArmeMelee()
            : base(Vector2.Zero, new Vector2(80, 10), 1, null)
        {
            DeltaHolding = Dégats / 120f;
            DeltaPositionOrigine = Vector2.UnitX * Dimensions.X * 0.45f;
            ArrangerSprite(GestionTexture.GetTexture("Armes/WoodenStick0"), Dimensions, new Vector2(0.5f, 0.5f), Profondeur, Color.White);
        }

        public ArmeMelee(Vector2 position, Vector2 dimensions, int dégats, PersonnageArmé propriétaire)
            : base(position,dimensions,dégats,propriétaire)
        {
        }

        public ArmeMelee(Vector2 position, PersonnageArmé propriétaire)
            : base(position, new Vector2(80, 10), 1, propriétaire)
        {
            DeltaHolding = Dégats / 150;
            DeltaPositionOrigine = Vector2.UnitX * Dimensions.X * 0.45f;
            ArrangerSprite(GestionTexture.GetTexture("Armes/WoodenStick0"), Dimensions, new Vector2(0.5f, 0.5f), Profondeur, Color.White);
        }

        public override void Utiliser(bool nouveauClic)
        {
            if (nouveauClic)
            {
                DébuterFrappe();
            }
            else if (Propriétaire.AnimationMelee.Index <= 4)
            {
                Holding = true;
                HoldingPower = MathHelper.Clamp(HoldingPower + DeltaHolding, 0, Dégats);
                if (HoldingPower == Dégats)
                {
                    MaxHolding = true;
                }
            }
        }
        public override void SeFaireRamasser(PersonnageArmé persoArmé)
        {
            persoArmé.ObtenirArmeMelee(this);
            base.SeFaireRamasser(persoArmé);
        }
        public override void SeFaireLaisser()
        {
            MouvementAmorcé = false;
            base.SeFaireLaisser();
        }
        public override void SePlacerEnMain(PersonnageArmé propriétaire)
        {
            Vector2.Transform(ref DeltaPositionOrigine, ref Matrice, out DeltaPosition);
            Position = propriétaire.MainDroite.Position + DeltaPosition;
            Angle = propriétaire.BrasDroite.Angle - 1.15f * (int)propriétaire.Direction;
        }

        protected virtual void DébuterFrappe()
        {
            MaxHolding = false;
            HoldingPower = 0;
            MouvementAmorcé = true;
            CoupDonné = false;

            Propriétaire.Frapper();
        }
        protected void Cogner()
        {
            AnimationTerminé = false;
            CoupDonné = false;
            if (MouvementAmorcé && !CoupDonné && Propriétaire.AnimationMelee.Index > 4)
            {
                Bone boneTouché = null;
                foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
                {
                    if (p != this.Propriétaire)
                    {
                        if (IntersectionPersonnage(p, out boneTouché))
                        {
                            p.Blesser(this, boneTouché);
                            GestionEffets.CréerJetSang(PositionBout, VitesseBout, 3, 90, 6, 14, 50);

                            MouvementAmorcé = false;
                            CoupDonné = true;                       

                            //----------------------------------------------------
                            MoteurJeu.AwesomeBox.WriteLine("Coup: " + ((HoldingPower + Dégats) * VitesseBout.Length()).ToString());
                            MoteurJeu.AwesomeBox.WriteLine(HoldingPower.ToString() );
                            //----------------------------------------------------

                            HoldingPower = 0;
                        }
                    }
                }
                if (Propriétaire.AnimationMelee.EstTerminée)
                {
                    MouvementAmorcé = false;
                    AnimationTerminé = true;
                    //HoldingPower = 0;
                }
            }           
        }
        protected virtual void SurMaxHolding()
        {

        }

        public override void Update()
        {
            base.Update();

            VitesseBout = PositionBout - ListeSommets[1].Position;
            PositionBout = ListeSommets[1].Position;

            if (Propriétaire != null)
            {
                if (Holding)
                {
                    if (Propriétaire.AnimationMelee.Index >= 4)
                    {
                        Propriétaire.AnimationMelee.Pause = true;
                    }
                }
                else
                {
                    Propriétaire.AnimationMelee.Pause = false;
                    Cogner();
                }
            }

            Holding = false;
        }


        //---------------Classe Statique---------------------------

        public static ArmeMelee CréerBâton(Vector2 position, PersonnageArmé propriétaire)
        {
            ArmeMelee arme = new ArmeMelee(position,propriétaire);

            //arme.Dimensions = new Vector2(80,8);
            //Vector2 dist1 = new Vector2(arme.Dimensions.X *0.9f, arme.Dimensions.Y / 2);
            //Vector2 dist2 = new Vector2(arme.Dimensions.X * 0.9f, -arme.Dimensions.Y / 2);
            //Vector2 dist3 = new Vector2(-arme.Dimensions.X *0.1f, -arme.Dimensions.Y / 2);
            //Vector2 dist4 = new Vector2(-arme.Dimensions.X *0.1f, arme.Dimensions.Y / 2);
            //List<Vector2> listeVecteurs = new List<Vector2>();
            //listeVecteurs.Add(dist1);
            //listeVecteurs.Add(dist2); 
            //listeVecteurs.Add(dist3);
            //listeVecteurs.Add(dist4);
            //arme.Reconstruire(position,arme.Dimensions);
            //arme.Dégats = 1;
            //arme.DeltaHolding = arme.Dégats / 180f;
            //arme.DeltaPositionOrigine = Vector2.UnitX * arme.Dimensions.X * 0.45f;
            //arme.ArrangerSprite(GestionTexture.GetTexture("Armes/WoodenStick0"),arme.Dimensions,new Vector2(0.5f,0.5f),arme.Profondeur,Color.Brown);

            return arme;
        }
    }
}
