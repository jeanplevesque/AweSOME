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
    //public enum TypeBloc { Terre, TunelDeTerre, Pierre, Métal, Eau }
    public enum ÉtatsBloc { Statique, TombeVerticalement, PhysiquePourColler}

    class Bloc : PolygonePhysique
    {
        public const int DIMENSION_BLOC = 64;
        public static Vector2 DIMENSIONS_BLOC = Vector2.One*DIMENSION_BLOC;

        public Bloc[] BlocsAutour = new Bloc[4];

        public bool EstEntouré
        {
            get
            {
                return GestionNiveaux.NiveauActif.BlocPrésent(IndexX - 1, IndexY)
                    && GestionNiveaux.NiveauActif.BlocPrésent(IndexX + 1, IndexY)
                    && GestionNiveaux.NiveauActif.BlocPrésent(IndexX, IndexY - 1)
                    && GestionNiveaux.NiveauActif.BlocPrésent(IndexX, IndexY + 1);
            }
        }
        public bool EstEntouréNonTunel
        {
            get
            {
                return GestionNiveaux.NiveauActif.BlocPrésentNonTunel(IndexX - 1, IndexY)
                    && GestionNiveaux.NiveauActif.BlocPrésentNonTunel(IndexX + 1, IndexY)
                    && GestionNiveaux.NiveauActif.BlocPrésentNonTunel(IndexX, IndexY - 1)
                    && GestionNiveaux.NiveauActif.BlocPrésentNonTunel(IndexX, IndexY + 1);
            }
        }
        public ÉtatsBloc ÉtatPrésent
        {
            get { return étatPrésent_; }
            set
            {
                étatPrésent_ = value;
                EstFixe = étatPrésent_ == ÉtatsBloc.Statique;
            }
        }
        private ÉtatsBloc étatPrésent_;

        public bool EstTunel
        {
            get { return estTunel_; }
            set
            {
                estTunel_ = value;
                if (estTunel_)
                {
                    Couleur = Color.White*0.4f;
                    DétruireOmbre();
                    CalculerBlocsAutour();
                    CréerOmbreBlocsAutour();                 
                }
                else
                {
                    Couleur = Color.White;
                }
            }
        }
        private bool estTunel_;

        public int IndexX;
        public int IndexY;
        public int TransformedIndexX
        {
            get { return GestionNiveaux.NiveauActif.TransformIndexX(IndexX); }
            set { IndexX = GestionNiveaux.NiveauActif.UnTransformIndexX(value); }
        }
        public int TransformedIndexY
        {
            get { return GestionNiveaux.NiveauActif.TransformIndexY(IndexY); }
            set { IndexY = GestionNiveaux.NiveauActif.UnTransformIndexY(value); }
        }
        public Point TransformedIndex { get { return GestionNiveaux.NiveauActif.TransformIndex(new Point(IndexX, IndexY)); } }
        public Point Index { get { return new Point(IndexX, IndexY); } }
        public byte NoImages; 

        public void Save(BinaryWriter writer)
        {
            writer.Write((byte)Matériel);
            writer.Write(NoImages);
            writer.Write((Int16)DégatsAccumulés);                      
            writer.Write(EstTunel);
        }

        public Bloc(int indexXTransformé, int indexYTransformé,MatérielPolygone matériel,byte noImage=255)
            : base(new Vector2(indexXTransformé * DIMENSION_BLOC, indexYTransformé * -DIMENSION_BLOC), DIMENSIONS_BLOC)
        {
            TransformedIndexX = indexXTransformé;
            TransformedIndexY = indexYTransformé;            
            Profondeur = 0.5f;

            ÉtatPrésent = ÉtatsBloc.Statique;
            Matériel = matériel;
            ChoisirTexture(noImage);
            
            ArrangerSprite();
        }
        
        
        /// <summary>
        /// Constructeur pour le Blocgun
        /// </summary>
        public Bloc(Vector2 position, MatérielPolygone matériel)
            : base(position, DIMENSIONS_BLOC)
        {
            Profondeur = 0.5f;

            ÉtatPrésent = ÉtatsBloc.Statique;
            Matériel = matériel;
            ChoisirTexture(255);

            ArrangerSprite();
            //CréerOmbre(120);
        }
        
        public void VérifierSiCréerOmbre()
        {
            if (!EstTunel && !PossèdeOmbre && !EstEntouréNonTunel)
            {
                CréerOmbre(150);
            }
        }
        public void CréerOmbreBlocsAutour()
        {
            Bloc b = null;
            for (int i = 0; i < BlocsAutour.Length; ++i)
            {
                b = BlocsAutour[i];
                if (b != null)
                {
                    b.VérifierSiCréerOmbre();
                }
            }
        }
       
        public void CalculerBlocsAutour()
        {
            //BlocsAutour = new Bloc[4];
            BlocsAutour[0] = GestionNiveaux.NiveauActif.GetBloc(IndexX - 1, IndexY);
            BlocsAutour[1] = GestionNiveaux.NiveauActif.GetBloc(IndexX + 1, IndexY);
            BlocsAutour[2] = GestionNiveaux.NiveauActif.GetBloc(IndexX, IndexY - 1);
            BlocsAutour[3] = GestionNiveaux.NiveauActif.GetBloc(IndexX, IndexY + 1);
        }
        public void Placer(int iRéel, int jRéel)
        {
            Vitesse = Vector2.Zero;
            VitesseAngulaire = 0;
            Angle = 0;
            IndexX = iRéel;
            IndexY = jRéel;
            Position.X = IndexX * DIMENSION_BLOC;
            Position.Y = IndexY * -DIMENSION_BLOC;
            Orienter();
        }

        public override void Update()
        {
            switch (ÉtatPrésent)
            {
                case ÉtatsBloc.TombeVerticalement:
                    base.Update();
                    Point index = CoordVersIndex(Position);
                    IndexX = index.X;
                    IndexY = index.Y;
                    Bloc bloc = GestionNiveaux.NiveauActif.GetBloc(IndexX, IndexY - 1);
                    if (bloc != null && bloc != this)
                    {
                        if (bloc.EstTunel)// && bloc.DégatsAccumulés<bloc.DégatsMax)
                        {
                            bloc.AbimerPourDétruire();
                        }
                        else
                        {
                            GestionNiveaux.NiveauActif.CollerBloc(this);
                        }
                    }
                    
                    break;

                case ÉtatsBloc.PhysiquePourColler:
                    base.Update();
                    if (CollisionContreAutresBlocs())
                    {
                        GestionNiveaux.NiveauActif.CollerBloc(this);
                    }
                    break;
                case ÉtatsBloc.Statique:
                    break;
            }           
        }

        public override bool EstÀIntérieur(Vector2 position)
        {
            return position.X >= Position.X - Dimensions.X / 2 &&
                   position.X <= Position.X + Dimensions.X / 2 &&
                   position.Y >= Position.Y - Dimensions.Y / 2 &&
                   position.Y <= Position.Y + Dimensions.Y / 2;
        }

        public void VérifierSiChute()
        {
            if (GestionNiveaux.NiveauActif == null) { return; }

            int nbVoisinsCollés = 0;

            //Bloc de Gauche
            Bloc voisin = GestionNiveaux.NiveauActif.GetBloc(IndexX - 1, IndexY);
            nbVoisinsCollés += BoolVersInt(voisin != null && voisin.EstFixe)*2;

            //Bloc de Droite
            voisin = GestionNiveaux.NiveauActif.GetBloc(IndexX + 1, IndexY);
            nbVoisinsCollés += BoolVersInt(voisin != null && voisin.EstFixe)*2;

            //Bloc du Bas
            voisin = GestionNiveaux.NiveauActif.GetBloc(IndexX, IndexY - 1);
            nbVoisinsCollés += BoolVersInt(voisin != null && voisin.EstFixe) * 4; //si un bloc est dessous, on ne tombe pas

            //Bloc du Bas à Droite
            voisin = GestionNiveaux.NiveauActif.GetBloc(IndexX + 1, IndexY - 1);
            nbVoisinsCollés += BoolVersInt(voisin != null && voisin.EstFixe)*2;

            //Bloc du Bas à Gauche
            voisin = GestionNiveaux.NiveauActif.GetBloc(IndexX - 1, IndexY - 1);
            nbVoisinsCollés += BoolVersInt(voisin != null && voisin.EstFixe)*2;

            //Bloc du Haut à Gauche
            //voisin = GestionNiveaux.NiveauActif.GetBloc(IndexX - 1, IndexY + 1);
            //nbVoisinsCollés += BoolVersInt(voisin != null && voisin.EstFixe);
            
            //Bloc du Haut à Droite
            //voisin = GestionNiveaux.NiveauActif.GetBloc(IndexX + 1, IndexY + 1);
            //nbVoisinsCollés += BoolVersInt(voisin != null && voisin.EstFixe);
            
            //Bloc du Haut
            voisin = GestionNiveaux.NiveauActif.GetBloc(IndexX, IndexY + 1);
            nbVoisinsCollés += BoolVersInt(voisin != null && voisin.EstFixe);

            if (nbVoisinsCollés < 4)
            {
                GestionNiveaux.NiveauActif.AjouterBlocÀFaireTomber(this);
            }
        }

        public override Contact VérifierCollisionsPersonnages()
        {
            Contact contact = null;

            foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            {
                foreach (Sommet s in p.ListeSommets)
                {
                    if (EstÀIntérieur(s.Position))
                    {
                        contact = CréerContact(p, this, s, true);
                        if (contact != null && contact.Normal.Y == 1 && Maths.EstEntre(ref p.Position.X, Position.X - Dimensions.X/2, Position.X + Dimensions.X/2))
                        {
                            p.TuerEtDétruire();
                        }                     
                        break;
                    }
                }
            }
            return contact;
        }

        public override void Détruire()
        {
            EstTunel = true;//plutot que de faire ca:
            //DétruireOmbre();
            //CalculerBlocsAutour();
            //CréerOmbreBlocsAutour();
            GestionNiveaux.NiveauActif.DétruireBloc(this);
            base.Détruire();
        }

        private void ChoisirTexture(byte noImage)
        {

            NoImages = noImage == 255 ? (byte)Maths.Random.Next(0, 5) : noImage;

            switch (Matériel)
            {
                case MatérielPolygone.Métal:
                    Image = GestionTexture.GetTexture("Bloc/CaseMétal" + NoImages.ToString());
                    break;
                case MatérielPolygone.Pierre:
                    Image = GestionTexture.GetTexture("Bloc/CasePierre" + NoImages.ToString());
                    break;
                case MatérielPolygone.Terre:
                    Image = GestionTexture.GetTexture("Bloc/CaseTerre" + NoImages.ToString());
                    break;
            }
        }
        private bool CollisionContreAutresBlocs()
        {
            Point index = Bloc.CoordVersIndex(Position);

            int i = index.X;// (int)((Position.X + Bloc.DIMENSION_BLOC / 2) / Bloc.DIMENSION_BLOC);
            int j = index.Y;// (int)((Position.Y - Bloc.DIMENSION_BLOC / 2) / -Bloc.DIMENSION_BLOC);

            //bornes inférieures
            int min_i = i - 1;
            int min_j = j - 1;

            //bornes supérieures
            int max_i = i + 1;
            int max_j = j + 1;

            for (int a = min_i; a <= max_i; ++a)
            {
                for (int b = min_j; b <= max_j; ++b)
                {
                    Bloc bloc = GestionNiveaux.NiveauActif.GetBloc(a, b);
                    if (bloc != null)
                    {
                        if (bloc.EstTunel)
                        {
                            bloc.AbimerPourDétruire();
                        }
                        else
                        {
                            //on vérifie si un de nos sommet entre dans un bloc de la grille
                            foreach (Sommet s in ListeSommets)
                            {
                                if (bloc.EstÀIntérieur(s.Position))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                }
            }
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //base.Draw(spriteBatch, IndexVersCoord(CoordVersIndex(Position)));
            //spriteBatch.Draw(Image, Position, new Rectangle((int)(Position.X % (float)Image.Width), (int)(-Position.Y % (float)Image.Height), (int)Dimensions.X, (int)Dimensions.Y), Couleur, Angle, Dimensions/2, Vector2.One, SpriteEffect, Profondeur);

            base.Draw(spriteBatch);
            //Couleur = Color.White;
            //spriteBatch.DrawString(BanqueContent.Font1, ((Point2D)TransformedIndex).ToString(), Position, Color.White);
        }

        //------------Classe Statique------------------------------------------------------------------------------------------
        public static implicit operator Tuile(Bloc bloc)
        {
            Tuile tuile = new Tuile()
            {
                X = bloc.TransformedIndexX,
                Y = bloc.TransformedIndexY
            };
            if(bloc!=null)
            {
                tuile.Passable = bloc.EstTunel;
            }
            return tuile;

            //return new Tuile()
            //{
            //    Passable = bloc == null || bloc.EstTunel,
            //    X = bloc.IndexX,
            //    Y = bloc.IndexY
            //};
        }

        public static Point CoordVersIndex(Vector2 position)
        {
            //return new Point((int)((position.X+DIMENSION_BLOC/2) / DIMENSION_BLOC), (int)((position.Y-DIMENSION_BLOC/2) / -DIMENSION_BLOC));
            return new Point((int)(position.X + DIMENSION_BLOC / 2) >> 6, (int)-(position.Y - DIMENSION_BLOC / 2) >> 6);
        }
        public static Point CoordVersIndex(ref Vector2 position)
        {
            //return new Point((int)((position.X+DIMENSION_BLOC/2) / DIMENSION_BLOC), (int)((position.Y-DIMENSION_BLOC/2) / -DIMENSION_BLOC));
            return new Point((int)(position.X + DIMENSION_BLOC / 2) >> 6, (int)-(position.Y - DIMENSION_BLOC / 2) >> 6);
        }
        public static Vector2 IndexVersCoord(int i, int j)
        {
            return new Vector2(i * DIMENSION_BLOC, j * -DIMENSION_BLOC);
        }
        public static Vector2 IndexVersCoord(Point point)
        {
            return new Vector2(point.X * DIMENSION_BLOC, point.Y * -DIMENSION_BLOC);
        }

        /// <summary>
        /// Cette Fonction Retourne 1 si le bool est vrai et 0 s'il est faut
        /// </summary>
        private static int BoolVersInt(bool unbool)
        {
            if (unbool) { return 1; }
            return 0;
        }

        
    }
}
