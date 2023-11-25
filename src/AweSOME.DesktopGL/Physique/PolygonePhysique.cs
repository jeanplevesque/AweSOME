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
    enum Mat�rielPolygone { Terre, Pierre, M�tal, Organes, Bois, Inconnu }
    partial class PolygonePhysique:Sprite
    {
        public Vector2 GRAVIT� = Vector2.UnitY * 0.09f;


        public List<Droite> ListeDroites = new List<Droite>();
        public List<Sommet> ListeSommets = new List<Sommet>();

        public int Solidit�;
        public int D�gatsAccumul�s;
        public int D�gatsMax=100;
        public bool SeraD�truit;

        public int TempsRestant = 60;
        public int TempsFin = 60;
        public float Alpha = 1;
        public bool Disparaitra;

        public Mat�rielPolygone Mat�riel
        {
            get { return mat�riel_; }
            set
            {
                mat�riel_ = value;
                switch (mat�riel_)
                {
                    case Mat�rielPolygone.M�tal:
                        D�gatsMax = 500;
                        Solidit� = 10;
                        break;
                    case Mat�rielPolygone.Pierre:
                        D�gatsMax = 300;
                        Solidit� = 8;
                        break;
                    case Mat�rielPolygone.Terre:
                        D�gatsMax = 200;
                        Solidit� = 4;
                        break;
                    case Mat�rielPolygone.Bois:
                        D�gatsMax = 150;
                        Solidit� = 5;
                        break;
                    case Mat�rielPolygone.Organes:
                        D�gatsMax = 100;
                        Solidit� = 1;
                        break;
                    case Mat�rielPolygone.Inconnu:
                        D�gatsMax = 200;
                        Solidit� = 5;
                        break;
                }
            }
        }
        private Mat�rielPolygone mat�riel_;

        public bool EstFixe;
        public bool EstLibre { get { return !EstFixe; } }

        public float DampingLin�aire = 0.998f;
        public float DampingAngulaire = 0.998f;

        public float Rayon;//pour d�tection de collisions grossi�res

        public float AncienAngle;
        public Vector2 AnciennePosition;
        
        public Vector2 Vitesse;
        public Vector2 Acc�l�ration;

        public Vector2 ForceAccumul�e;
        public float TorqueAccumul�e;

        public float VitesseAngulaire;
        public float Acc�l�rationAngulaire;

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

        public Personnage DernierPropri�taire;

        public bool Tuile�Chang�;
        public Tuile TuileActive
        {
            get { return tuileActive_; }
            set 
            {
                if (tuileActive_ != value)
                {
                    Tuile�Chang� = true;
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
            
            G�n�rer();
            RelierSommets(1);
            Rayon = (float)Math.Sqrt((Dimensions.X * Dimensions.X) + (Dimensions.Y * Dimensions.Y));

            GRAVIT� *= Masse;

            //Cr�erOmbre();
        }
        public PolygonePhysique(Vector2 position, float rayon, int nbSommets)
            : base(position, Vector2.One*rayon*2)
        {

            Masse = Dimensions.X * Dimensions.Y;
            Inertie = (1f / 12f * Masse * (Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y)) * 2;
            InverseInertie = 1f / Inertie;
            
            Rayon = rayon;
            G�n�rerR�gulier(nbSommets);
            RelierSommets(1);
            

            GRAVIT� *= Masse;
        }
        public PolygonePhysique()
            : base() { }


        protected PolygonePhysique(PolygonePhysique �Cloner)
            :base()
        {
            this.AnciennePosition = �Cloner.AnciennePosition;
            this.AncienAngle = �Cloner.AncienAngle;
            this.Acc�l�ration = �Cloner.Acc�l�ration;
            this.Acc�l�rationAngulaire = �Cloner.Acc�l�rationAngulaire;
            this.Angle = �Cloner.Angle;
            this.Couleur = �Cloner.Couleur;
            this.EstFixe = �Cloner.EstFixe;
            this.ForceAccumul�e = �Cloner.ForceAccumul�e;
            this.GRAVIT� = �Cloner.GRAVIT�;
            this.Inertie = �Cloner.Inertie;
            this.Matrice = �Cloner.Matrice;
            this.Repositionnement = �Cloner.Repositionnement;
            this.Restitution = �Cloner.Restitution;
            this.Solidit� = �Cloner.Solidit�;
            this.SpriteEffect = �Cloner.SpriteEffect;
            this.TorqueAccumul�e = �Cloner.TorqueAccumul�e;
            this.Vitesse = �Cloner.Vitesse;
            this.VitesseAngulaire = �Cloner.VitesseAngulaire;
            this.Position = �Cloner.Position;
            this.Dimensions = �Cloner.Dimensions;
            this.Profondeur = �Cloner.Profondeur;
            this.NbSommets=�Cloner.NbSommets;
            this.D�gatsMax = �Cloner.D�gatsMax;
            this.D�gatsAccumul�s = �Cloner.D�gatsAccumul�s;
            this.Mat�riel = �Cloner.Mat�riel;

            Masse = �Cloner.Masse;
            InverseInertie = �Cloner.InverseInertie;
            Rayon = �Cloner.Rayon;

            if (NbSommets == 4)
            {
                G�n�rer();
            }
            else
            {
                G�n�rerR�gulier(NbSommets);
            }
            RelierSommets(1);

            ArrangerSprite(�Cloner.Image,�Cloner.Dimensions,Vector2.One/2,Profondeur,Couleur);

            Orienter();
        }

        public new PolygonePhysique Clone()
        {
            return new PolygonePhysique(this);
        }

        public virtual void ForceDelete()
        {
            if (Poss�deOmbre)
            {
                D�truireOmbre();
            }
            GestionNiveaux.NiveauActif.D�truirePolygonePhysique(this);
        }
        public virtual bool V�rifierHorsNiveau()
        {
            return Position.Y > 0 && EstLibre;
        }

        public void Integrate()
        {
            AnciennePosition = Position;
            AncienAngle = Angle;

            Acc�l�ration = Vector2.Zero;// Acc�l�ration;
            Acc�l�ration += ForceAccumul�e * MasseInverse;

            Acc�l�rationAngulaire = TorqueAccumul�e * InverseInertie;

            Vitesse += Acc�l�ration;
            VitesseAngulaire += Acc�l�rationAngulaire;

            //---Clamping---
            VitesseAngulaire = MathHelper.Clamp(VitesseAngulaire, -MathHelper.PiOver4 / 4, MathHelper.PiOver4 / 4);
            //--------------


            Vitesse *= DampingLin�aire;
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
            ForceAccumul�e += force;
        }
        public void AddTorque(float torque)
        {
            TorqueAccumul�e += torque;           
        }
        public void ClearAccumulators()
        {
            ForceAccumul�e = Vector2.Zero;
            TorqueAccumul�e = 0;
        }
        public void AddForceAuPoint(Vector2 force,Vector2 point)
        {
            if (force.LengthSquared() > Masse)
            {
                // Convert to coordinates relative to center of mass.
                Vector2 distanceCentrePoint = point - Position;

                ForceAccumul�e += force;

                float torque = Maths.ProduitVectoriel(ref distanceCentrePoint, ref force);

                TorqueAccumul�e += torque;
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
            writer.Write(EstD�coupable);
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
            corps.EstD�coupable = reader.ReadBoolean();
            corps.Profondeur = reader.ReadSingle();

            corps.Orienter();
            return corps;
        }*/

        public virtual void Update()
        {
            AppliquerGravit�();
            Integrate();
            Orienter();
            Disparaitre();
            if (V�rifierHorsNiveau())
            {
                ForceDelete();
            }
        }
        public void Update(bool appliquerGravit�)
        {
            if (appliquerGravit�)
            {
                AppliquerGravit�();
            }
            Integrate();
            Orienter();
            if (V�rifierHorsNiveau())
            {
                ForceDelete();
            }
        }

        private void AppliquerGravit�()
        {
            if (EstLibre)
            {
                AddForce(GRAVIT�);
            }
        }

        public void AbimerPourD�truire()
        {
            Abimer(D�gatsMax);
        }
        public virtual void Abimer(int d�gats)
        {
            D�gatsAccumul�s += d�gats;

            if (!(this is Bone))
            {
                if (D�gatsAccumul�s >= D�gatsMax && !SeraD�truit)
                {
                    D�truire();
                }
            }
        }

        public virtual void D�truire()
        {
            SeraD�truit = true;
            ForceDelete();
            GestionEffets.Cr�erEffetDestructionPolygone(this);
        }

        public void CalculerTuileActive()
        {
            Tuile�Chang� = false;
            TuileActive = GestionNiveaux.NiveauActif.GetTuile(Position);
        }
    }
}
