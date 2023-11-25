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
using AwesomeAnimation;

namespace AweSOME
{
    class PersonnageArmé:Personnage
    {
        public Inventaire Inventaire;

        //public PolygonePhysique PolyCiblé;
        public Fusil Fusil;
        public PolygonePhysique PolygoneEnMain { get { return (PolygonePhysique)ItemEnMain; } }
        public IRamassable ItemEnMain
        {
            get { return itemEnMain_; }
            set
            {
                itemEnMain_ = value;
                if (itemEnMain_ is Fusil)
                {
                    Fusil = (Fusil)itemEnMain_;
                }
                else
                {
                    Fusil = null;
                }
                if (itemEnMain_ == null)
                {
                    AnimationMarche = GestionAnimation.Load("Marche");
                    AnimationCourse = GestionAnimation.Load("Course");
                    Fusil = null;                   
                }
                else
                {
                    AnimationMarche = Animation.CréerÀPartirDeBody(AnimationMarche, AnimationTypes.Pieds);
                    AnimationCourse = Animation.CréerÀPartirDeBody(AnimationCourse, AnimationTypes.Pieds);
                    ArrangerOrdreAffichage(Profondeur);
                }
            }
        }
        protected IRamassable itemEnMain_;

        protected Matrix MatriceBras;
        protected Vector2 OrientationBrasOrigine;
        protected Vector2 OrientationFusilOrigine;

        public bool PeutRecharger;
        public bool PeutLancer;
        public bool Recharge;
        public bool Lance;

        public bool PeutTenirItem
        {
            get { return peutTenirItem_; }
            set
            {
                peutTenirItem_ = value;
                if (!peutTenirItem_ && ItemEnMain!=null)
                {
                    FaireTomberItem();
                }
            }
 
        }
        protected bool peutTenirItem_;

        public float AngleBras;
        //public int NbKills;
        //public int NbHeadShots;

        public Animation AnimationRecharge = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Recharge"), AnimationTypes.Bras);
        public Animation AnimationLancer = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Lancer"), AnimationTypes.Bras);


        public PersonnageArmé(Vector2 position)
            :base(position,Bloc.DIMENSION_BLOC)
        {
            Inventaire = new Inventaire(this);
            //Cerveau = new IA(this);

            ObtenirFusil(GestionArmes.GetRandomFusil());

            OrientationBrasOrigine = Vector2.UnitX * BrasDroite.LongueurInitiale;
            OrientationFusilOrigine = Vector2.UnitX * Grosseur;

            PeutRecharger = true;
            PeutTenirItem = true;
            PeutLancer = true;

            AnimationMelee = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Melee2"), AnimationTypes.Bras);
        }

        public override void Update()
        {

            base.Update();

            AnalyserÉtat();

            PlacerBras();

            UpdateItemEnMain();


            //if (PolyCiblé != null && Fusil!=null)
            //{
            //    GestionEffets.CréerMagie(PolyCiblé.Position, -Vector2.UnitY, 1, 60, 12, 20, ref Fusil.PointeurLazer.Couleur, 20, 30);
            //}
        }

        protected override void Mourrir()
        {
            FaireTomberItem();
            //base.Mourrir();
        }

        


        public void RamasserObjetÀProximité()
        {
            if (Inventaire.EstPlein&&ItemEnMain!=null) { return; }

            float distance;
            float distanceMin=BrasDroite.LongueurInitiale*BrasDroite.LongueurInitiale*2.5f;
            IRamassable objetPlusPrès = null;

            float deltaX;
            foreach (IRamassable i in GestionNiveaux.NiveauActif.ListeObjetsRamassable)
            {
                deltaX=(Position.X-i.GetPosition().X)*(int)Direction;//On vérifie que l'objet se trouve Devant le personnage
                if (deltaX < distanceMin && deltaX < 0)
                {
                    distance = Vector2.DistanceSquared(i.GetPosition(), Position);
                    if (distanceMin > distance)
                    {
                        distanceMin = distance;
                        objetPlusPrès = i;
                    }
                }
            }
            if (objetPlusPrès != null)
            {
                objetPlusPrès.SeFaireRamasser(this);
            }
        }
        public void Lancer()
        {
            if (!Lance && PeutLancer)
            {
                if (ItemEnMain is ArmeLancée || ItemEnMain is Flare)
                {
                    ItemEnMain.Utiliser();//on active la bombe si nous en avons une
                    Lance = true;
                    AnimationLancer.Reset();
                }
                else
                {
                    ItemEnMain = Inventaire.ObtenirMeilleurItemLancable(ItemEnMain);
                    if (ItemEnMain != null)//On ne veut pas lancer du vide
                    {
                        ItemEnMain.Utiliser();//Dans le cas d'une bombe, elle s'active
                        Lance = true;
                        AnimationLancer.Reset();
                    }
                }
            }
        }
        public void Tirer(bool nouveauTir)
        {
            if (!Recharge && Fusil != null)
            {
                Fusil.Tirer(nouveauTir);
            }
        }
        public void Recharger()
        {
            if (!Recharge && PeutRecharger && !Lance && Fusil != null)
            {
                AnimationRecharge.Reset();
                Recharge = true;
            }
        }
        public void UtiliserItemEnMain(bool nouveauClic)
        {
            if (ItemEnMain != null)
            {
                if (ItemEnMain is Fusil)
                {
                    Tirer(nouveauClic);
                }
                else
                {
                    ItemEnMain.Utiliser(nouveauClic);
                }
            }
        }
        public void UtiliserItemEnMainAlternatif(bool nouveauClic)
        {
            //if (Fusil != null)
            //{
            //    PolyCiblé = Fusil.PointeurLazer.DernierPolyPointé;
            //}
            if (ItemEnMain != null)
            {
                ItemEnMain.UtiliserAlternatif();
            }
        }

        public void ToggleLazerPointer()
        {
            if (Fusil != null)
            {
                Fusil.PointeurLazer.ToggleActivation();
            }
        }
        public void ToggleFlashLight()
        {
            if (Fusil != null)
            {
                Fusil.FlashLight.ToggleOnOff();
            }
        }
        public void PrendreItemDeInventaire(int index)
        {
            ItemEnMain = Inventaire.ÉchangerItem(index,ItemEnMain);
        }
        public void MettreItemEnMainDansInventaire()
        {
            if (ItemEnMain != null)
            {
                Inventaire.AjouterItem(ItemEnMain);
            }
        }

        public void ObtenirArmeMelee(ArmeMelee armeMelee)
        {
            armeMelee.Propriétaire = this;
            if (Inventaire.EstPlein && ItemEnMain == null)
            {
                ItemEnMain = armeMelee;
            }
            else
            {
                Inventaire.AjouterItem(armeMelee);
            }
        }
        public void ObtenirFusil(Fusil fusil)
        {
            if (ItemEnMain is Fusil && Fusil.Nom == fusil.Nom)
            {
                Fusil.AjouterBalles(fusil);
                fusil.FlashLight.Supprimer();
            }
            else
            {
                Fusil fusilDéjàPossédé=null;
                if (Inventaire.PossèdeDéjàFusil(fusil, out fusilDéjàPossédé))
                {
                    fusilDéjàPossédé.AjouterBalles(fusil);
                    fusil.FlashLight.Supprimer();
                }
                else
                {
                    MettreItemEnMainDansInventaire();//Si nous en avons déja un

                    ItemEnMain = fusil;
                    Fusil.Propriétaire = this;
                    Fusil.PointeurLazer.Propriétaire = this;
                    Fusil.PointeurLazer.Activer();
                }
            }
        }
        public void ObtenirArmeLancée(ArmeLancée armeLancée)
        {
            armeLancée.Propriétaire = this;
            if (Inventaire.EstPlein && ItemEnMain == null)
            {
                ItemEnMain = armeLancée;
            }
            else
            {
                Inventaire.AjouterItem(armeLancée);
            }
        }
        public void ObtenirObjetDivers(ObjetDivers objet)
        {
            objet.DernierPropriétaire = this;
            if (Inventaire.EstPlein && ItemEnMain == null)
            {
                ItemEnMain = objet;
            }
            else
            {
                Inventaire.AjouterItem(objet);
            }
        }
        public void FaireTomberItem()
        {
            if (ItemEnMain != null)
            {
                ItemEnMain.SeFaireLaisser();
                ItemEnMain = null;
            }
        }
        public void RamasserFusil(Fusil fusil)
        {
            if (PeutTenirItem)
            {
                ObtenirFusil(fusil);
                GestionNiveaux.DétruireObjetRamassable(fusil);
            }
        }
        public void RamasserArmeLancée(ArmeLancée armeLancée)
        {
            if (PeutTenirItem)
            {
                ObtenirArmeLancée(armeLancée);
                GestionNiveaux.DétruireObjetRamassable(armeLancée);
            }
        }

        protected void AnalyserÉtat()
        {
            if (Recharge && AnimationRecharge.EstTerminée && Fusil != null)
            {
                Fusil.Recharger();
                Recharge = false;
            }
            else if (Lance && AnimationLancer.Index == 4)
            {
                EnvoyerProjectile();
                Lance = false;
            }
        }
        protected void EnvoyerProjectile()
        {
            if (ItemEnMain != null)
            {
                PolygonePhysique poly = ((PolygonePhysique)ItemEnMain);
                Vector2 direction = Cible.Position - ItemEnMain.GetPosition();
                float force = direction.Length();
                direction /= force;
                force = MathHelper.Clamp(force, 8, 256);
                poly.AddForce(direction * poly.Masse * force * 0.0585937f);//yup [C'est 15f * (1/256)]
                poly.AddTorque(1000000);
                poly.DernierPropriétaire = this;
                FaireTomberItem();
            }
        }

        protected override void DonnerCoupsBras()
        {
            //foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            //{
                //if (p != this)
                //{
                //    if (!BrasDroite.EstBrisé && BrasDroite.IntersectionPersonnage(p))
                //    {
                //        p.Blesser(ForceMelee,this);
                //        GestionEffets.CréerJetSang(BrasGauche.Position, MainGauche.Vitesse, 5, 90, 8, 16, 50);
                //    }
                //    if (!BrasGauche.EstBrisé && BrasGauche.IntersectionPersonnage(p))
                //    {
                //        p.Blesser(ForceMelee,this);
                //        GestionEffets.CréerJetSang(BrasGauche.Position, MainGauche.Vitesse, 5, 90, 8, 16, 50);
                //    }
                //    if (PolygoneEnMain != null && PolygoneEnMain.IntersectionPersonnage(p))
                //    {
                //        p.Blesser(ForceMelee,this);
                //        GestionEffets.CréerJetSang(PolygoneEnMain.Position, PolygoneEnMain.Vitesse, 5, 90, 8, 16, 50);
                //    }
                //}
            //}
        }
       
        public override void ChangerAnimationPourRamper()
        {
            base.ChangerAnimationPourRamper();
            AnimationRecharge = Animation.CréerÀPartirDeBody(GestionAnimation.Load("RechargeBas"), AnimationTypes.Bras);
        }
        public override void ChangerAnimationPourMarcher()
        {
            base.ChangerAnimationPourMarcher();

            AnimationMelee = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Melee2"), AnimationTypes.Bras);
            AnimationRecharge = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Recharge"), AnimationTypes.Bras);
            AnimationLancer = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Lancer"), AnimationTypes.Bras);
        }
        public override void VérifierPossibilitésDéplacement()
        {
            base.VérifierPossibilitésDéplacement();
            //if (!PeutSauter)//Les 2 jambes sont brisées
            //{
            //    AnimationRecharge = Animation.CréerÀPartirDeBody(GestionAnimation.Load("RechargeBas"), AnimationTypes.Bras);
            //}
            if (BrasDroite.EstBrisé)
            {
                PeutLancer = false;
                PeutRecharger = false;
                PeutFrapper = false;
                if (BrasGauche.EstBrisé)
                {
                    PeutTenirItem = false;
                }
            }
            else
            {
                PeutLancer = true;
                PeutFrapper = true;
                PeutRecharger = true;
                PeutTenirItem = true;
            }
        }
        protected override void Animer()
        {
            base.Animer();
            if (Recharge)
            {
                AnimationRecharge.Animer(this, 0.1f, AnimationSens.Normal);
            }
            else if (Lance)
            {
                AnimationLancer.Animer(this, 0.1f, AnimationSens.Normal);
            }
        }



        protected override void ArrangerOrdreAffichage(float profondeur)
        {
            base.ArrangerOrdreAffichage(profondeur);
            if (ItemEnMain != null)
            {
                if (Direction == Directions.ÀGauche)
                {
                    ItemEnMain.SetSpriteEffet(SpriteEffects.FlipVertically);
                }
                else
                {
                    ItemEnMain.SetSpriteEffet(SpriteEffects.None);
                }
                ItemEnMain.SetProfondeur(Profondeur + 0.015f);
            }
        }
        protected void PlacerBras()
        {
            if (!Frappe && !Recharge && !Lance && ItemEnMain!=null)
            {
                AngleBras = Maths.CalculerAngleEntreDeuxPosition(Épaules.Position, Cible.Position);

                MatriceBras = Matrix.CreateRotationZ(AngleBras + 0.2f * ((int)Direction));
                BrasGauche.Joint2.Position = Épaules.Position + Vector2.Transform(OrientationBrasOrigine, MatriceBras);
                BrasGauche.Joint2.PositionVoulue = BrasGauche.Joint2.Position;
                BrasGauche.Joint2.AnciennePositionVoulue = BrasGauche.Joint2.Position;

                MatriceBras = Matrix.CreateRotationZ(AngleBras);
                BrasDroite.Joint2.Position = Épaules.Position + Vector2.Transform(OrientationBrasOrigine, MatriceBras);
                BrasDroite.Joint2.PositionVoulue = BrasDroite.Joint2.Position;
                BrasDroite.Joint2.AnciennePositionVoulue = BrasDroite.Joint2.Position;

            }

        }
        
        private void UpdateItemEnMain()
        {
            if (ItemEnMain != null)
            {
                ItemEnMain.SePlacerEnMain(this);
                ItemEnMain.Update();
            }
        }
        public void PlacerItemEnMain()
        {
            if (Frappe || Recharge || Lance || Fusil == null)
            {
                ItemEnMain.SetPosition(MainDroite.Position);
            }
            else
            {
                ItemEnMain.SetPosition(Épaules.Position + Vector2.Transform(OrientationFusilOrigine, MatriceBras));
            }
            ItemEnMain.SetAngle(BrasDroite.Angle);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (ItemEnMain != null)
            {
                ItemEnMain.Draw(spriteBatch);
            }
            //if(Fusil!=null)
            //{
            //    spriteBatch.DrawString(BanqueContent.Font1, "NbBallesTotal : " + Fusil.NbBallesTotal.ToString() + "\n"+
            //                                                "NbBallesChargeur : " + Fusil.NbBallesDansChargeur.ToString()+ "\n"+
            //                                                "CapacitéChargeur : " + Fusil.CapacitéChargeur.ToString(), 
            //    Position + new Vector2(Dimensions.X,-Dimensions.Y), Color.White, 0, Vector2.One / 2f, 0.75f, SpriteEffects.None, 0);
        
            //}
            
            //Color couleur = new Color(10, 1, 10);
            
            
            
        }

        
    }
}
