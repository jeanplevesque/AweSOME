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
    enum MatérielPolygone { Terre, Pierre, Métal, Organes, Bois, Inconnu }
    partial class PolygonePhysique:Sprite
    {
        public Vector2 GRAVITÉ = Vector2.UnitY * 0.09f;


        public List<Droite> ListeDroites = new List<Droite>();
        public List<Sommet> ListeSommets = new List<Sommet>();

        public int Solidité;
        public int DégatsAccumulés;
        public int DégatsMax=100;
        public bool SeraDétruit;

        public int TempsRestant = 60;
        public int TempsFin = 60;
        public float Alpha = 1;
        public bool Disparaitra;

        public MatérielPolygone Matériel
        {
            get { return matériel_; }
            set
            {
                matériel_ = value;
                switch (matériel_)
                {
                    case MatérielPolygone.Métal:
                        DégatsMax = 500;
                        Solidité = 10;
                        break;
                    case MatérielPolygone.Pierre:
                        DégatsMax = 300;
                        Solidité = 8;
                        break;
                    case MatérielPolygone.Terre:
                        DégatsMax = 200;
                        Solidité = 4;
                        break;
                    case MatérielPolygone.Bois:
                        DégatsMax = 150;
                        Solidité = 5;
                        break;
                    case MatérielPolygone.Organes:
                        DégatsMax = 100;
                        Solidité = 1;
                        break;
                    case MatérielPolygone.Inconnu:
                        DégatsMax = 200;
                        Solidité = 5;
                        break;
                }
            }
        }
        private MatérielPolygone matériel_;

        public bool EstFixe;
        public bool EstLibre { get { return !EstFixe; } }

        public float DampingLinéaire = 0.998f;
        public float DampingAngulaire = 0.998f;

        public float Rayon;//pour détection de collisions grossières

        public float AncienAngle;
        public Vector2 AnciennePosition;
        
        public Vector2 Vitesse;
        public Vector2 Accélération;

        public Vector2 ForceAccumulée;
        public float TorqueAccumulée;

        public float VitesseAngulaire;
        public float AccélérationAngulaire;

        public float Masse
        {
            get { return masse_; }
            set
            {
                masse_ = value;
                masseInverse_ = 1f / masse_;
            }
        }
        float masse_;
        public float MasseInverse
        {
            get { return masseInverse_; }
            set
            {
                masseInverse_ = value;
                masse_ = 1f / masseInverse_;
            }
        }
        float masseInverse_;
        public float InverseInertie;
        public float Inertie;

        public int NbCollisions;
        public int NbCollisionsMax=3;

        public float Restitution=0.5f;
        public float Repositionnement = 1f;

        public Personnage DernierPropriétaire;

        public bool TuileÀChangé;
        public Tuile TuileActive
        {
            get { return tuileActive_; }
            set 
            {
                if (tuileActive_ != value)
                {
                    TuileÀChangé = true;
                }
                tuileActive_ = value;
            }
        }
        Tuile tuileActive_;

        public PolygonePhysique(Vector2 position, Vector2 dimensions)
            : base(position, dimensions)
        {
            
            Masse = Dimensions.X * Dimensions.Y;
            Inertie = (1f / 12f * Masse * (Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y))*2;
            InverseInertie = 1f / Inertie;
            
            Générer();
            RelierSommets(1);
            Rayon = (float)Math.Sqrt((Dimensions.X * Dimensions.X) + (Dimensions.Y * Dimensions.Y));

            GRAVITÉ *= Masse;

            //CréerOmbre();
        }
        public PolygonePhysique(Vector2 position, float rayon, int nbSommets)
            : base(position, Vector2.One*rayon*2)
        {

            Masse = Dimensions.X * Dimensions.Y;
            Inertie = (1f / 12f * Masse * (Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y)) * 2;
            InverseInertie = 1f / Inertie;
            
            Rayon = rayon;
            GénérerRégulier(nbSommets);
            RelierSommets(1);
            

            GRAVITÉ *= Masse;
        }
        public PolygonePhysique()
            : base() { }


        protected PolygonePhysique(PolygonePhysique àCloner)
            :base()
        {
            this.AnciennePosition = àCloner.AnciennePosition;
            this.AncienAngle = àCloner.AncienAngle;
            this.Accélération = àCloner.Accélération;
            this.AccélérationAngulaire = àCloner.AccélérationAngulaire;
            this.Angle = àCloner.Angle;
            this.Couleur = àCloner.Couleur;
            this.EstFixe = àCloner.EstFixe;
            this.ForceAccumulée = àCloner.ForceAccumulée;
            this.GRAVITÉ = àCloner.GRAVITÉ;
            this.Inertie = àCloner.Inertie;
            this.Matrice = àCloner.Matrice;
            this.Repositionnement = àCloner.Repositionnement;
            this.Restitution = àCloner.Restitution;
            this.Solidité = àCloner.Solidité;
            this.SpriteEffect = àCloner.SpriteEffect;
            this.TorqueAccumulée = àCloner.TorqueAccumulée;
            this.Vitesse = àCloner.Vitesse;
            this.VitesseAngulaire = àCloner.VitesseAngulaire;
            this.Position = àCloner.Position;
            this.Dimensions = àCloner.Dimensions;
            this.Profondeur = àCloner.Profondeur;
            this.NbSommets=àCloner.NbSommets;
            this.DégatsMax = àCloner.DégatsMax;
            this.DégatsAccumulés = àCloner.DégatsAccumulés;
            this.Matériel = àCloner.Matériel;

            Masse = àCloner.Masse;
            InverseInertie = àCloner.InverseInertie;
            Rayon = àCloner.Rayon;

            if (NbSommets == 4)
            {
                Générer();
            }
            else
            {
                GénérerRégulier(NbSommets);
            }
            RelierSommets(1);

            ArrangerSprite(àCloner.Image,àCloner.Dimensions,Vector2.One/2,Profondeur,Couleur);

            Orienter();
        }

        public new PolygonePhysique Clone()
        {
            return new PolygonePhysique(this);
        }

        public virtual void ForceDelete()
        {
            if (PossèdeOmbre)
            {
                DétruireOmbre();
            }
            GestionNiveaux.NiveauActif.DétruirePolygonePhysique(this);
        }
        public virtual bool VérifierHorsNiveau()
        {
            return Position.Y > 0 && EstLibre;
        }

        public void Integrate()
        {
            AnciennePosition = Position;
            AncienAngle = Angle;

            Accélération = Vector2.Zero;// Accélération;
            Accélération += ForceAccumulée * MasseInverse;

            AccélérationAngulaire = TorqueAccumulée * InverseInertie;

            Vitesse += Accélération;
            VitesseAngulaire += AccélérationAngulaire;

            //---Clamping---
            VitesseAngulaire = MathHelper.Clamp(VitesseAngulaire, -MathHelper.PiOver4 / 4, MathHelper.PiOver4 / 4);
            //--------------


            Vitesse *= DampingLinéaire;
            VitesseAngulaire *= DampingAngulaire;

            Position += Vitesse;
            Angle += VitesseAngulaire;


            Vitesse = Position - AnciennePosition;
            VitesseAngulaire = Angle - AncienAngle;
            VitesseAngulaire %= MathHelper.TwoPi;

            ClearAccumulators();
        }
        public void Disparaitre()
        {
            if (Disparaitra)
            {
                --TempsRestant;
                if (TempsRestant < TempsFin)
                {
                    Alpha = (float)TempsRestant / (float)TempsFin;
                    if (TempsRestant <= 0)
                    {
                        ForceDelete();
                    }
                }
            }
        }

        public void AddForce(Vector2 force)
        {
            ForceAccumulée += force;
        }
        public void AddTorque(float torque)
        {
            TorqueAccumulée += torque;           
        }
        public void ClearAccumulators()
        {
            ForceAccumulée = Vector2.Zero;
            TorqueAccumulée = 0;
        }
        public void AddForceAuPoint(Vector2 force,Vector2 point)
        {
            if (force.LengthSquared() > Masse)
            {
                // Convert to coordinates relative to center of mass.
                Vector2 distanceCentrePoint = point - Position;

                ForceAccumulée += force;

                float torque = Maths.ProduitVectoriel(ref distanceCentrePoint, ref force);

                TorqueAccumulée += torque;
            }
        }

        /*public override void Save(BinaryWriter writer)
        {
            writer.Write(EstFixe);
            writer.Write(Position.X);
            writer.Write(Position.Y);
            writer.Write(Dimensions.X);
            writer.Write(Dimensions.Y);
            writer.Write(Angle);
            writer.Write(GestionTexture.GetNom(Image));
            writer.Write(GestionTexture.GetNom(NormalMap));
            writer.Write(EstDécoupable);
            writer.Write(Profondeur);
        }
        
        public static new PolygonePhysique Load(BinaryReader reader)
        {
            PolygonePhysique corps;
            if (reader.ReadBoolean())
            {
                corps = new PolygoneFixe(new Vector2(reader.ReadSingle(), reader.ReadSingle()), new Vector2(reader.ReadSingle(), reader.ReadSingle()));
            }
            else
            {
                corps = new PolygoneMobile(new Vector2(reader.ReadSingle(), reader.ReadSingle()), new Vector2(reader.ReadSingle(), reader.ReadSingle()));
            }
            corps.Angle=reader.ReadSingle();
            corps.ArrangerSprite(GestionTexture.GetTexture(reader.ReadString()));
            corps.NormalMap = GestionTexture.GetTexture(reader.ReadString());
            corps.EstDécoupable = reader.ReadBoolean();
            corps.Profondeur = reader.ReadSingle();

            corps.Orienter();
            return corps;
        }*/

        public virtual void Update()
        {
            AppliquerGravité();
            Integrate();
            Orienter();
            Disparaitre();
            if (VérifierHorsNiveau())
            {
                ForceDelete();
            }
        }
        public void Update(bool appliquerGravité)
        {
            if (appliquerGravité)
            {
                AppliquerGravité();
            }
            Integrate();
            Orienter();
            if (VérifierHorsNiveau())
            {
                ForceDelete();
            }
        }

        private void AppliquerGravité()
        {
            if (EstLibre)
            {
                AddForce(GRAVITÉ);
            }
        }

        public void AbimerPourDétruire()
        {
            Abimer(DégatsMax);
        }
        public virtual void Abimer(int dégats)
        {
            DégatsAccumulés += dégats;

            if (!(this is Bone))
            {
                if (DégatsAccumulés >= DégatsMax && !SeraDétruit)
                {
                    Détruire();
                }
            }
        }

        public virtual void Détruire()
        {
            SeraDétruit = true;
            ForceDelete();
            GestionEffets.CréerEffetDestructionPolygone(this);
        }

        public void CalculerTuileActive()
        {
            TuileÀChangé = false;
            TuileActive = GestionNiveaux.NiveauActif.GetTuile(Position);
        }
    }
}
