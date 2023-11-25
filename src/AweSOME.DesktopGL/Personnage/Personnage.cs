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

    public enum Directions { ÀDroite = 1, ÀGauche = -1 }
    public enum ÉtatsPossibles { Immobile, Marche, Course, TeaBagging }
    
    class Personnage:PolygonePhysique
    {
        public Vector2 DIMENSIONS = new Vector2(Bloc.DIMENSION_BLOC * 0.8f, Bloc.DIMENSION_BLOC * 2.5f);
        
        public Vector2 ImpulsionSaut;

        public Intelligence Cerveau;

        public float Grosseur;// = Bloc.DIMENSION_BLOC;
        //public string Nom;
        public Cible Cible = new Cible();

        public bool ÉtaitSurSol;
        public virtual bool EstSurSol
        {
            get { return estSurSol_;}// && TuileActive != null && !TuileActive.VoisinPlusBas.Passable; }
            set { estSurSol_ = value; }
        }
        protected bool estSurSol_;
        public bool CollisionMurDroite;
        public bool CollisionMurGauche;
        public bool CollisionPlafond;
        public bool Sprint;
        public bool Frappe
        {
            get { return frappe_; }
            set
            {
                frappe_ = value;
                if (frappe_)
                {
                    if (Rampe)
                    {
                        AnimationMarche = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Rampe"), AnimationTypes.Jambes);
                    }
                    else
                    {
                        AnimationMarche = AnimationMarchePieds;
                    }
                }
                else
                {
                    if (Rampe)
                    {
                        AnimationMarche = GestionAnimation.Load("Rampe");
                    }
                    else
                    {
                        AnimationMarche = AnimationMarchePieds;
                    }
                }
            }
        }
        private bool frappe_;
        public bool EstEnVie
        {
            get { return estEnVie_; }
            set
            {                
                if (estEnVie_ && !value)
                {
                    if (DerniereSourceDégats != null)
                    {
                        DerniereSourceDégats.Stats.AjouterKill(this);
                    }
                    //if (estEnVie_)
                    //{
                        Mourrir();
                        Position = Bassin;
                    //}
                }
                estEnVie_ = value;
            }
        }
        private bool estEnVie_;
        public bool Spawning;

        protected bool Rampe { get { return !PeutSauter; } }

        public bool PeutSauter {
            get { return peutSauter_; }
            set
            {
                peutSauter_ = value;
                if (!peutSauter_)
                {
                    AnimationMelee = GestionAnimation.Load("MeleeBas");
                }
            }
        }
        private bool peutSauter_;
        public bool PeutBougerEnX;
        public bool PeutFrapper;

        public List<Bone> ListeBones = new List<Bone>();
        public List<Joint> ListeJoints = new List<Joint>();
        public Joint[] ListeJointsJambes = new Joint[5];
        public Joint[] ListeJointsBras = new Joint[4];
        public Joint[] ListeJointsMains = new Joint[2];
        public Joint[] ListeJointsPieds = new Joint[2];

        public const float VITESSE_X_MAX = 3;
        public const float VITESSE_X_MAX_SPRINT = 5;

        public ÉtatsPossibles ÉtatPrésent;

        public int VieRestante { get; protected set; }
        public int VieMax { get; protected set; }
        public int ForceMelee=15;


        public Directions Direction
        {
            get { return direction_; }
            set
            {
                if (direction_ != value)
                {
                    InverserPositionsX();
                    if (value == Directions.ÀDroite)
                    {
                        Tête.SpriteEffect = SpriteEffects.None;
                        foreach (Bone b in ListeBones)
                        {
                            b.SpriteEffect = SpriteEffects.None;
                        }
                    }
                    else
                    {
                        Tête.SpriteEffect = SpriteEffects.FlipVertically;
                        foreach (Bone b in ListeBones)
                        {
                            b.SpriteEffect = SpriteEffects.FlipVertically;
                        }
                    }
                    direction_ = value;
                    ArrangerOrdreAffichage(0.6f);
                }

            }
        }
        Directions direction_ = Directions.ÀGauche;

        //protected Animation AnimationMelee = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Melee"),AnimationTypes.Mains);
        public Animation AnimationMelee = GestionAnimation.Load("Melee");
        protected Animation AnimationCourse = GestionAnimation.Load("Course");
        protected Animation AnimationMarche = GestionAnimation.Load("Marche");
        protected Animation AnimationMarchePieds;

        public StatistiquesPersonnage Stats;

        public Personnage DerniereSourceDégats;

        #region Bones et Joints

        public BoneÀunJoint Tête { get; protected set; }
        public Bone Torse;
        public Bone JambeDroite;
        public Bone JambeGauche;
        public Bone BrasDroite;
        public Bone BrasGauche;

        protected Joint CentreTête;
        protected Joint Épaules;
        protected Joint Bassin;
        protected Joint PiedGauche;
        protected Joint PiedDroite;
        protected Joint MainGauche;
        public Joint MainDroite;
        #endregion

        public Personnage(Vector2 position,float grosseur)
            : base(position, new Vector2(grosseur * 0.8f, grosseur * 2.5f))
        {
            Stats = new StatistiquesPersonnage(this);

            //GestionAnimation.Save(AnimationMelee);
            //AnimationMelee = GestionAnimation.Load("Melee");
            Grosseur = grosseur;
            
            GRAVITÉ *= 1.2f;
            ImpulsionSaut = -GRAVITÉ * 38;
            //EstFixe = true;
            
            EstEnVie = true;           
            VieMax = 100;
            VieRestante = VieMax;
            PeutBougerEnX = true;
            PeutSauter = true;
            PeutFrapper = true;

            NbCollisionsMax = 10;

            AnimationMarchePieds = Animation.CréerÀPartirDeBody(AnimationMarche, AnimationTypes.Pieds);
            
            ArrangerSprite(BanqueContent.Pixel);
            //Couleur = Color.Turquoise*0.5f;

            Vector2 dist0 = new Vector2(0, Grosseur * 0.25f);//Bassin
            Vector2 dist1 = new Vector2(0, Grosseur * 1.25f);//Pieds
            Vector2 dist2 = new Vector2(0, Grosseur * -0.75f);//Épaules
            Vector2 dist3 = new Vector2(0, Grosseur * -1f);//Centre Tête
            Vector2 dist4 = new Vector2(Grosseur, Grosseur * -0.75f);//Bras Gauche
            Vector2 dist5 = new Vector2(-Grosseur, Grosseur * -0.75f);//Bras Droit

            Bassin = new Joint(Position + dist0, dist0, this);
            PiedDroite = new Joint(Position + dist1, dist1, this);
            PiedGauche = new Joint(Position + dist1, dist1, this);
            Épaules = new Joint(Position + dist2, dist2, this);
            CentreTête = new Joint(Position + dist3, dist3, this);
            MainGauche = new Joint(Position + dist4, dist4, this);
            MainDroite = new Joint(Position + dist5, dist5, this);

            ListeJoints.Add(Bassin);
            ListeJoints.Add(Épaules);
            ListeJoints.Add(PiedDroite);
            ListeJoints.Add(PiedGauche);
            ListeJoints.Add(MainDroite);
            ListeJoints.Add(MainGauche);
            ListeJoints.Add(CentreTête);

            Tête = new BoneÀunJoint(CentreTête,Vector2.One*Grosseur/1.5f,8,this);

            Torse = new Bone(Épaules,Bassin , Grosseur * 0.5f, this, PartiesCorps.Torse);
            JambeDroite = new Bone(Bassin, PiedDroite, Grosseur * 0.5f, this, PartiesCorps.Jambe);
            JambeGauche = new Bone(Bassin, PiedGauche, Grosseur * 0.5f, this, PartiesCorps.Jambe);
            BrasDroite = new Bone(Épaules, MainDroite, Grosseur * 0.5f, this, PartiesCorps.Bras);
            BrasGauche = new Bone(Épaules, MainGauche, Grosseur * 0.5f, this, PartiesCorps.Bras);

            
            ListeBones.Add(Torse);
            ListeBones.Add(JambeDroite);
            ListeBones.Add(JambeGauche);
            ListeBones.Add(BrasDroite);
            ListeBones.Add(BrasGauche);
            ListeBones.Add(Tête);

            ListeJointsJambes[0] = Bassin;
            ListeJointsJambes[1] = PiedDroite;
            ListeJointsJambes[2] = PiedGauche;
            ListeJointsJambes[3] = Épaules;
            ListeJointsJambes[4] = CentreTête;

            ListeJointsBras[0] = Épaules;
            ListeJointsBras[1] = MainDroite;
            ListeJointsBras[2] = MainGauche;
            ListeJointsBras[3] = CentreTête;

            ListeJointsMains[0] = MainDroite;
            ListeJointsMains[1] = MainGauche;

            ListeJointsPieds[0] = PiedDroite;
            ListeJointsPieds[1] = PiedGauche;

            ÉtatPrésent = ÉtatsPossibles.Immobile;
            Orienter();

            //GénérerRandomZombie();

            Direction = Directions.ÀDroite;
            ArrangerOrdreAffichage(0.6f);
            //CalculerTuileActive();
        }

        public void AjusterPosition(Vector2 delta)
        {
            Position += delta;
            foreach (Joint j in ListeJoints)
            {
                j.Position += delta;
                j.PositionVoulue += delta;
                j.AnciennePositionVoulue += delta;
            }
        }

        public void Frapper()
        {
            if (!Frappe && PeutFrapper)
            {
                Frappe = true;
                AnimationMelee.Reset();
            }
        }
        private void UpdateFrapper()
        {
            if (Frappe)
            {
                AnimationMelee.Animer(this, 0.2f, AnimationSens.Normal);
                if (AnimationMelee.EstTerminée)
                {
                    Frappe = false;
                    DonnerCoupsBras();
                }
            }
        }
        protected virtual void DonnerCoupsBras()
        {
            foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            {
                if (p != this)
                {
                    if (!BrasDroite.EstBrisé && BrasDroite.IntersectionPersonnage(p))
                    {
                        p.Blesser(ForceMelee,this);
                        GestionEffets.CréerJetSang(BrasGauche.Position, MainGauche.Vitesse, 3, 90, 6, 14, 50);
                    } 
                    if (!BrasGauche.EstBrisé && BrasGauche.IntersectionPersonnage(p))
                    {
                        p.Blesser(ForceMelee,this);
                        GestionEffets.CréerJetSang(BrasGauche.Position, MainGauche.Vitesse, 3, 90, 6, 14, 50);
                    }
                }
            }
        }

        public void Blesser(BouleEnergie bouleEnergie, Bone boneTouché)
        {
            DerniereSourceDégats = bouleEnergie.Propriétaire;

            int dégats = bouleEnergie.Dégats;

            BlesserBone(boneTouché, dégats, boneTouché.Position, bouleEnergie.Vitesse * dégats);
            
            BlesserEnFonctionPartieCorps(boneTouché, dégats);
            
            if (VieRestante <= 0)
            {
                AddForceAuPoint(bouleEnergie.Vitesse * Masse * 0.05f * dégats, boneTouché.Position);
            }
        }
        public void Blesser(ArmeMelee armeMelee, Bone boneTouché)
        {
            int dégats = (int)((armeMelee.HoldingPower + armeMelee.Dégats) * MathHelper.Clamp(armeMelee.VitesseBout.Length(), armeMelee.Dégats, armeMelee.Dégats * 10));
            Blesser(dégats, armeMelee.Propriétaire);
            BlesserBone(boneTouché, dégats, boneTouché.Position, armeMelee.VitesseBout);
        }
        public void Blesser(ÉclaireQuiBlesse éclaire, Bone boneTouché)
        {
            DerniereSourceDégats = éclaire.Propriétaire;

            int dégats = éclaire.Dégats;

            BlesserBone(boneTouché, dégats, boneTouché.Position, éclaire.VecteurPrincipal * 0.00002f * dégats);

            BlesserEnFonctionPartieCorps(boneTouché, dégats);

            if (VieRestante <= 0)
            {
                AddForceAuPoint(éclaire.VecteurPrincipal * Masse * 0.00008f * dégats, boneTouché.Position);
            }
        }
        public void Blesser(Explosion explosion)
        {
            DerniereSourceDégats = explosion.Propriétaire;
            float distance = 0;
            int dégats = 0;
            foreach (Bone b in ListeBones)
            {
                distance = Vector2.Distance(b.Position, explosion.Position);
                dégats = Maths.Clamp((int)((explosion.RayonDAction-distance) * explosion.Dégats * explosion.UnSurRayon*0.25f), explosion.Dégats, 0);
                //dégats = Maths.Clamp((int)(explosion.Dégats / (distance*explosion.UnSurRayon)), explosion.Dégats, 0);


                BlesserEnFonctionPartieCorps(b,dégats);
                BlesserBone(b, dégats, b.Position,b.Position - explosion.Position);
            }
            if (VieRestante <= 0)
            {
                distance=Vector2.Distance(Position, explosion.Position);
                float multiplication = (explosion.RayonDAction - distance);
                AddForce((Position - explosion.Position) * multiplication  * explosion.UnSurRayon * Masse * 0.002f * explosion.Dégats);
                //AddForce((Position - explosion.Position) / distance * Masse * 0.02f * explosion.Dégats);
            
            }
        }
        public void Blesser(ImpactBalle impact)
        {
            DerniereSourceDégats = impact.Balle.Propriétaire;

            Bone boneTouché=(Bone)impact.PolyTouché;
            BalleFusil balle=impact.Balle;

            BlesserBone(boneTouché,impact);

            BlesserEnFonctionPartieCorps(boneTouché, balle.Dégats);

            if (VieRestante <= 0)
            {
                AddForceAuPoint(balle.Vitesse * Masse * 0.002f * balle.Dégats, impact.Position);
            }
        }
        public void Blesser(PolygonePhysique poly, Bone boneTouché, Vector2 positionContact)
        {
            int dégats=0;
            if (poly.EstFixe)//Collision contre le décors
            {
                if (!ÉtaitSurSol)//On s'assure que le personnage tombait
                {
                    dégats = (int)(Vitesse.Y * Vitesse.Y * Vitesse.Y / 16);
                    if (dégats > 25)
                    {
                        Blesser(dégats);
                        BlesserBone(JambeDroite, dégats, positionContact, Vitesse);
                        BlesserBone(JambeGauche, dégats, positionContact, Vitesse);
                    }
                    GestionEffets.CréerPoussièreAuSol(positionContact, poly.Matériel,(int) (Vitesse.Y / 2));
                }
            }
            else // le perso se fait frapper par un polygone libre
            {
                if (poly.DernierPropriétaire != null)
                {
                    DerniereSourceDégats = poly.DernierPropriétaire;
                }

                int degats = (int)(poly.Vitesse.LengthSquared())/4;
                if (degats >= 10)//on s'assure que le polygone se déplace assez rapidement
                {
                    Blesser(degats);
                    BlesserBone(boneTouché, degats, positionContact, poly.Vitesse); // Hé oui on peut se faire arracher le bras par un autre bras XD

                    GestionEffets.CréerJetSang(positionContact, poly.Vitesse, 3, 90, 6, 14, 50);
                }
            }
        }
        public void Blesser(int dégats)
        {
            VieRestante -= dégats;
        }
        public void Blesser(int dégats, Personnage persoQuiFrappe)
        {
            VieRestante -= dégats;
            DerniereSourceDégats = persoQuiFrappe;
        }
        protected void BlesserBone(Bone bone, int dégats, Vector2 positionImpact, Vector2 vitesseObject)
        {
            bone.DégatsAccumulés += dégats;

            if (!bone.EstBrisé && bone.PartieCorps != PartiesCorps.Torse && bone.DégatsAccumulés >= VieMax/4)//On ne veut pas détaché le torse :)
            {
                int random = Maths.Random.Next(1);
                switch (random)
                {
                    //On Détache le membre 
                    case 0:
                        bone.Briser(vitesseObject, positionImpact);
                        AjouterMembreDétruitsStats();
                        VérifierPossibilitésDéplacement();
                        break;
                    default:
                        break;
                }
            }
            if (bone == Tête && bone.DégatsAccumulés >= VieMax / 4 && !Tête.EstBrisé)
            {
                bone.Briser(vitesseObject, positionImpact);
                AjouterMembreDétruitsStats();
                VérifierPossibilitésDéplacement();
            }
        }
        protected void BlesserBone(Bone bone, ImpactBalle impact)
        {
            BlesserBone(bone, impact.Balle.Dégats, impact.Position, impact.Balle.Vitesse);
        }
        protected void BlesserEnFonctionPartieCorps(Bone boneTouché, int dégats)
        {
            switch (boneTouché.PartieCorps)
            {
                case PartiesCorps.Bras:
                    Blesser(dégats);
                    break;
                case PartiesCorps.Jambe:
                    Blesser(dégats / 2);
                    break;
                case PartiesCorps.Torse:
                    Blesser((int)(dégats * 1.5f));
                    break;
                case PartiesCorps.Tête:
                    Blesser((int)(dégats * 2.5f));
                    break;
            }
        }

        public void Heal(int pointsVie)
        {
            VieRestante += pointsVie;
        }
        public void HealBone(Bone bone, int pointsVie)
        {
            bone.Heal(pointsVie);

            VérifierPossibilitésDéplacement();
        }
        public void HealBones(int pointsVie)
        {
            foreach (Bone bone in ListeBones)
            {
                bone.Heal(pointsVie);
            }

            VérifierPossibilitésDéplacement();
        }
        public virtual void ChangerAnimationPourRamper()
        {
            AnimationMarche = GestionAnimation.Load("Rampe");
            AnimationMarchePieds = Animation.CréerÀPartirDeBody(AnimationMarche, AnimationTypes.Jambes);

            AnimationMelee = GestionAnimation.Load("MeleeBas");
        }
        public virtual void ChangerAnimationPourMarcher()
        {
            AnimationMarche = (GestionAnimation.Load("Marche"));
            AnimationMarchePieds = Animation.CréerÀPartirDeBody(AnimationMarche, AnimationTypes.Pieds);

            AnimationMelee = GestionAnimation.Load("Melee");
        }
        public virtual void VérifierPossibilitésDéplacement()
        {
            if (JambeDroite.EstBrisé && JambeGauche.EstBrisé)
            {
                PeutBougerEnX = false;
                PeutSauter = false;

                //ChangerAnimation
                ChangerAnimationPourRamper();
                //AnimationMarcheJambe.PlacerSurFrame(ListeJointsJambes, Position, Grosseur, 0);
            }
            else
            {
                PeutBougerEnX = true;
                PeutSauter = true;

                //ChangerAnimation
                //AnimationMarche = Animation.CréerÀPartirDeBody(GestionAnimation.Load("Marche"), AnimationTypes.Pieds);
                ChangerAnimationPourMarcher();
                //AnimationMarche = (GestionAnimation.Load("Marche"));
                //AnimationMarchePieds = Animation.CréerÀPartirDeBody(AnimationMarche, AnimationTypes.Pieds);

                //AnimationMarcheJambe.PlacerSurFrame(ListeJointsJambes, Position, Grosseur, 0);
            }
            if (BrasDroite.EstBrisé && BrasGauche.EstBrisé)
            {
                PeutFrapper = false;
            }
            else
            {
                PeutFrapper = true;
            }
        }

        protected void AjouterMembreDétruitsStats()
        {
            if (DerniereSourceDégats != null)
            {
                ++DerniereSourceDégats.Stats.NbMembresDétruits;
            }
        }

        protected virtual void Mourrir()
        {
            
        }

        public void TuerEtDétruire()
        {
            VieRestante = 0;
            CréerExplosionDeSang();

            EstEnVie = false;

            DétruireCorps();
        }
        public void Sauter()
        {
            if (PeutSauter)
            {
                if (EstSurSol && !CollisionPlafond)
                {
                    Vitesse.Y = 0;
                    AddForce(ImpulsionSaut);
                    EstSurSol = false;
                }
                else if (Vitesse.Y < 0)//on ne veut pas rallentir la déscente
                {
                    AddForce(-GRAVITÉ * 0.5f);
                }
            }
        }
        public void DéplacerEnX(float vitesseXVoulue)
        {
            if (PeutBougerEnX)
            {
                if (!CollisionMurDroite && vitesseXVoulue > 0 )
                {
                    Vitesse.X += vitesseXVoulue / 64;
                    Vitesse.X = MathHelper.Clamp(Vitesse.X, 0, vitesseXVoulue);
                }
                else if (!CollisionMurGauche && vitesseXVoulue < 0)
                {
                    Vitesse.X += vitesseXVoulue / 64;
                    Vitesse.X = MathHelper.Clamp(Vitesse.X, vitesseXVoulue, 0);
                }
                else// if (vitesseXVoulue == 0)
                {
                    Vitesse.X *= 0.9f;
                }

                if (Sprint)
                {
                    Vitesse.X = MathHelper.Clamp(Vitesse.X, -VITESSE_X_MAX_SPRINT, VITESSE_X_MAX_SPRINT);
                }
                else
                {
                    Vitesse.X = MathHelper.Clamp(Vitesse.X, -VITESSE_X_MAX, VITESSE_X_MAX);
                }
            }
        }


        public override void Update()
        {
            if (EstEnVie)
            {
                CalculerTuileActive();
                Cerveau.Update();
            }

            VérifierÉtatSanté();

            base.Update(!EstSurSol);

            if (EstEnVie)
            {
                Animer();
                UpdateFrapper();               
            }

            UpdateBones();

            ÉtaitSurSol = EstSurSol;
            EstSurSol = false;
            CollisionMurDroite = false;
            CollisionMurGauche = false;
            CollisionPlafond = false;
        }

        public new void VérifierCollisionsDécors()
        {
            if (EstEnVie)
            {
                base.VérifierCollisionsDécorsV2();
            }
            else
            {
                if (NbCollisions < NbCollisionsMax)
                {
                    CollisionSelonPolygone(Torse);
                    //CollisionSelonPolygone(Tête);
                }
                else
                {
                    TuerEtDétruire();
                }
            }
        }

        public bool VérifierCollisionsÉclaire(ÉclaireQuiBlesse éclaire, out Bone boneTouché)
        {
            bool collision = false;
            boneTouché = null;
            SegmentÉclaire s = null;

            //foreach (SegmentÉclaire s in éclaire.ListeSegments)
            //{
            for (int i = 0; i < éclaire.ListeSegments.Count; i += 3)//on en saute un car on vérifie les deux sommets
            {
                s = éclaire.ListeSegments[i];
                if (Maths.IntersectionRayons(s.Position, s.Longueur, Position, Rayon))
                {
                    foreach (Bone b in ListeBones)
                    {
                        if (!b.EstBrisé)
                        {
                            //b.CalculerPentes();

                            if (b.EstÀIntérieur(s.P1) || b.EstÀIntérieur(s.P2))
                            {
                                boneTouché = b;

                                return true;
                            }
                        }
                    }
                }
            }

            return collision;
        }

        public void ChangerVieMax(int vieMax)
        {
            VieMax = vieMax;
            VieRestante = VieMax;
        }
        protected void GénérerRandomZombie()
        {
            Tête.ArrangerSprite(GestionTexture.GetTexture("Zombie/TêteZombie" + Maths.Random.Next(5).ToString()), Vector2.One * Tête.Dimensions.X * 1.15f, Vector2.One / 2, Profondeur, Color.White);
            Torse.ArrangerSprite(GestionTexture.GetTexture("Zombie/TorseZombie" + Maths.Random.Next(5).ToString()), Torse.Dimensions + new Vector2(Grosseur / 4, Grosseur / 6), Vector2.One / 2, Profondeur, Color.White);

            JambeGauche.ArrangerSprite(GestionTexture.GetTexture("Zombie/JambeZombie" + Maths.Random.Next(5).ToString()));
            JambeDroite.ArrangerSprite(GestionTexture.GetTexture("Zombie/JambeZombie" + Maths.Random.Next(5).ToString()));

            BrasDroite.ArrangerSprite(GestionTexture.GetTexture("Zombie/BrasZombie" + Maths.Random.Next(5).ToString()), BrasDroite.Dimensions + Vector2.UnitX * Grosseur / 3, new Vector2(0.4f, 0.5f), Profondeur, Color.White);
            BrasGauche.ArrangerSprite(GestionTexture.GetTexture("Zombie/BrasZombie" + Maths.Random.Next(5).ToString()), BrasGauche.Dimensions + Vector2.UnitX * Grosseur / 3, new Vector2(0.4f, 0.5f), Profondeur, Color.White);
        }

        protected virtual void Animer()
        {
            AnimationSens sens = AnimationSens.Normal;
            if (Vitesse.X * (int)Direction < 0)
            {
                sens = AnimationSens.Reverse;
            }
            switch (ÉtatPrésent)
            {
                case ÉtatsPossibles.Immobile:
                    AnimationMarche.Animer(this, Math.Abs(Vitesse.X / 16f), sens);
                    break;

                case ÉtatsPossibles.Marche:
                    AnimationMarche.Animer(this, Math.Abs(Vitesse.X / 16f), sens);
                    //AnimationMarche.Animer(this, Math.Abs(Vitesse.X / 16f), sens);
                    break;

                case ÉtatsPossibles.Course:
                    AnimationCourse.Animer(this, Math.Abs(Vitesse.X / 16f), sens);
                    break;
                case ÉtatsPossibles.TeaBagging:
                    break;
            }
        }
        protected virtual void ArrangerOrdreAffichage(float profondeur)
        {
            Profondeur = profondeur;
            Torse.Profondeur = Profondeur;
            Tête.Profondeur = Profondeur - 0.025f;
            BrasDroite.Profondeur = Profondeur - (int)Direction * 0.02f;
            BrasGauche.Profondeur = Profondeur + (int)Direction * 0.02f;
            JambeDroite.Profondeur = Profondeur - (int)Direction * 0.01f;
            JambeGauche.Profondeur = Profondeur + (int)Direction * 0.01f;
            //Profondeur += 0.1f;
        }
        
        void CollisionSelonPolygone(PolygonePhysique bone)
        {
            List<Contact> contacts = bone.VérifierCollisionsDécors();
            foreach (Contact c in contacts)
            {
                if (c != null)
                {
                    GestionContacts.Ajouter(c, this);
                }
            }

        }
        void OrienterJointCentralsSelonAngle()
        {
            Matrix matrice = Matrix.CreateRotationZ(Angle);

            //foreach (Joint j in ListeJoints)
            //{
            //    j.DistanceCentrePolygone = Vector2.Transform(j.DistanceCentrePolygoneInitiale, matrice);
            //    j.Position = Position + j.DistanceCentrePolygone;
            //}

            Bassin.DistanceCentrePolygone = Vector2.Transform(Bassin.DistanceCentrePolygoneInitiale, matrice);
            Bassin.Position = Position + Bassin.DistanceCentrePolygone;

            Épaules.DistanceCentrePolygone = Vector2.Transform(Épaules.DistanceCentrePolygoneInitiale, matrice);
            Épaules.Position = Position + Épaules.DistanceCentrePolygone;

            CentreTête.DistanceCentrePolygone = Vector2.Transform(CentreTête.DistanceCentrePolygoneInitiale, matrice);
            CentreTête.Position = Position + CentreTête.DistanceCentrePolygone;
        }
        public void VérifierÉtatSanté()
        {
            VieRestante = Maths.Clamp(VieRestante, VieMax, 0);
            if (VieRestante <= 0 && EstEnVie)
            {
                EstEnVie = false;
            }
            if (Tête.EstBrisé) { GestionEffets.CréerJetSang(Épaules.Position, -Torse.VecteurPrincipal, 0.41f, 30, 6, 14, 70); }
            if (BrasDroite.EstBrisé) { GestionEffets.CréerJetSang(Épaules.Position, Vector2.UnitX * ((int)Direction), 0.23f, 30, 6, 14, 70); }
            if (BrasGauche.EstBrisé) { GestionEffets.CréerJetSang(Épaules.Position, -Vector2.UnitX * ((int)Direction), 0.23f, 30, 6, 14, 70); }
            if (JambeDroite.EstBrisé) { GestionEffets.CréerJetSang(Bassin.Position, Vector2.UnitY, 0.18f, 30, 6, 14, 70); }
            if (JambeGauche.EstBrisé) { GestionEffets.CréerJetSang(Bassin.Position, Vector2.UnitY, 0.18f, 30, 6, 14, 70); }

            if (Position.Y > 100) { TuerEtDétruire(); }//Si on se trouve sous la carte, on disparait
        }
        
        void InverserPositionsX()
        {
            for (int i = 0; i < ListeJoints.Count; ++i)
            {
                ListeJoints[i].Position.X -= (ListeJoints[i].Position.X - Position.X) * 2;
                ListeJoints[i].PositionVoulue.X -= (ListeJoints[i].PositionVoulue.X - Position.X) * 2;
                ListeJoints[i].AnciennePositionVoulue.X -= (ListeJoints[i].AnciennePositionVoulue.X - Position.X) * 2;
                ListeJoints[i].Vitesse *= 0;
            }
        }
        
        protected void UpdateBones()
        {
            DistribuerVitesseAuxJoints();
            
            if (EstEnVie)
            {
                Tête.Angle = Maths.CalculerAngleEntreDeuxPosition(CentreTête.Position, Cible.Position);
                foreach (Bone b in ListeBones)
                {
                    b.UpdateParJoint();
                }
            }
            else
            {
                foreach (Bone b in ListeBones)
                {
                    b.Update();
                }

                OrienterJointCentralsSelonAngle();

                Tête.UpdateParJoint();
                Torse.UpdateParJoint();
                BrasDroite.PlacerSurPremierJoint();
                BrasGauche.PlacerSurPremierJoint();
                JambeDroite.PlacerSurPremierJoint();
                JambeGauche.PlacerSurPremierJoint();
            }
        }
        protected void DistribuerVitesseAuxJoints()
        {
           
            for (int i = 0; i < ListeJoints.Count ; ++i)
            {
                ListeJoints[i].Position += Vitesse;
                ListeJoints[i].AnciennePositionVoulue += Vitesse;
                ListeJoints[i].PositionVoulue += Vitesse;
            }
            for (int i = 0; i < ListeBones.Count; ++i)
            {
                //ListeBones[i].Vitesse += Vitesse;
            }
        }

        protected void CréerExplosionDeSang()
        {
            GestionEffets.CréerJetSang(Bassin.Position, Vector2.UnitY, 25, 360, 6, 14, 90);
            GestionEffets.CréerJetSang(JambeDroite.Position, Vector2.UnitY, 25, 360, 6, 14, 90);
            GestionEffets.CréerJetSang(JambeDroite.Position, Vector2.UnitY, 25, 360, 6, 14, 90);
            GestionEffets.CréerJetSang(BrasDroite.Position, Vector2.UnitY, 25, 360, 6, 14, 90);
            GestionEffets.CréerJetSang(BrasGauche.Position, Vector2.UnitY, 25, 360, 6, 14, 90);
            GestionEffets.CréerJetSang(Torse.Position, Vector2.UnitY, 25, 360, 6, 14, 90);
            GestionEffets.CréerJetSang(Tête.Position, -Vector2.UnitY, 25, 30, 6, 14, 180);

            GestionEffets.CréerOrganes(Tête.Position, -Vector2.UnitY);
            GestionEffets.CréerOrganes(Torse.Position, -Vector2.UnitY);
            GestionEffets.CréerOrganes(Bassin.Position, -Vector2.UnitY);
            GestionEffets.CréerOrganes(BrasDroite.Position, -Vector2.UnitY);
            GestionEffets.CréerOrganes(BrasGauche.Position, -Vector2.UnitY);
        }
        protected void DétruireCorps()
        {
            foreach (Bone b in ListeBones)
            {
                b.DétruireOmbre();
            }

            GestionNiveaux.NiveauActif.DétruirePolygonePhysique(this);
        }

        public override void ForceDelete()
        {
            //base.ForceDelete(); // On ne veut pas que la fonction s'exécute
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //base.Draw(spriteBatch);
            foreach (Bone b in ListeBones)
            {
                b.Draw(spriteBatch);
            }
            
            //MoteurJeu.SpriteBatchScène.Draw(Image, Position, null, Couleur, 0, Vector2.One / 2, Vector2.One * 8, this.SpriteEffect, 0);
            //Tête.Draw(spriteBatch);
            //foreach (Joint j in ListeJoints)
            //{
            //    j.Draw(spriteBatch);
            //}
            //spriteBatch.DrawString(BanqueContent.Font1, VieRestante.ToString(),(Position - Vector2.UnitY * Dimensions.Y), Color.Green, 0, Vector2.One / 2f, 3, SpriteEffects.None, 0);
            //spriteBatch.DrawString(BanqueContent.Font1, Vitesse.ToString(), (Position - Vector2.UnitY * (Dimensions.Y+50)), Color.Green, 0, Vector2.One / 2f, 3, SpriteEffects.None, 0);
            //spriteBatch.DrawString(BanqueContent.Font1, "Vie : " + VieRestante.ToString() + "\n" +
            //                                            "Tête : " + Tête.DégatsAccumulés.ToString() + "\n" +
            //                                            "Torse : " + Torse.DégatsAccumulés.ToString() + "\n" +
            //                                            "BrasGauche : " + BrasGauche.DégatsAccumulés.ToString() + "\n" +
            //                                            "BrasDroite : " + BrasDroite.DégatsAccumulés.ToString() + "\n" +
            //                                            "JambeGauche : " + JambeGauche.DégatsAccumulés.ToString() + "\n" +
            //                                            "JambeDroite : " + JambeDroite.DégatsAccumulés.ToString() + "\n",
            //    Position + new Vector2(Dimensions.X * 4, -Dimensions.Y), Color.Green, 0, Vector2.One / 2f, 1f, SpriteEffects.None, 0);
            /*spriteBatch.DrawString(BanqueContent.Font1,"Vie : " + VieRestante.ToString() + "\n" + 
                                                        "Kills : " + Stats.NbKills.ToString() + "\n" +
                                                            "HeadShots : " + Stats.NbHeadShots.ToString() + "\n" +
                                                            "Tirs : " + Stats.NbTirs.ToString() + "\n",
                    Position + new Vector2(Dimensions.X, -Dimensions.Y), Color.White, 0, Vector2.One / 2f, 1f, SpriteEffects.None, 0);*/
        }


        
    }

    class Cible:Sprite
    {
        public Bloc GetBlocPointé()
        {
            throw new NotImplementedException();
        }
    }
}
