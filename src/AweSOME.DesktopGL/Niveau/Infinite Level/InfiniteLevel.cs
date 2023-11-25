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
using System.Threading;
using System.IO;

namespace AweSOME
{
    partial class InfiniteLevel:Niveau
    {
        public static Point2D IndexAreaJoueurVoulu = new Point2D(1, 1);

        public Point2D IndexMap;
        public Point2D DeltaIndexMap;

        public Area[,] Areas;
        public Point2D DimensionsAreas;
        public Vector2 ConversionArea;

        //public Vector2 MinPositions;
        //public Vector2 MaxPositions;

        public Point2D MinIndex;
        public Point2D MaxIndex;


        public InfiniteLevel(int dimAreaX, int dimAreaY)
            : base(dimAreaX * 3, dimAreaY * 3)
        {
            DimensionsAreas = new Point(dimAreaX,dimAreaY);
            ConversionArea = new Vector2(1f / dimAreaX, -1f / dimAreaY)/Bloc.DIMENSION_BLOC;
            CréerAreas();
        }

        private void CréerAreas()
        {
            Areas = new Area[3,3];

            Point index;
            for (index.X = 0; index.X < 3; ++index.X)
            {
                for (index.Y = 0; index.Y < 3; ++index.Y)
                {
                    Areas[index.X, index.Y] = new Area(this, (Point2D)index + IndexMap, DimensionsAreas);
                }
            }
        }

        public override void Initialiser()
        {
            base.Initialiser();

            foreach (Area a in Areas)
            {
                //a.RefreshBlocs();
                a.Save();


                a.SupprimerBlocsDuNiveau();//useless, seulement pour tester
                a.MettreLesBlocsÀNull();//useless, seulement pour tester
                a.LoadOuGénérer();//useless, seulement pour tester
                //a.Générer();
                a.InsérerBlocsDansNiveau(); //useless, seulement pour tester
                a.CalculerGazonEtOmbre();
            }
            ListeBlocVérifierChute.Clear();//useless, seulement pour tester
        }


        public override void Update()
        {
            //base.Update();

            CalculerEtGérerDeltaMap();

            base.Update();
        }

        private void CalculerEtGérerDeltaMap()
        {
            Point2D indexJoueur = (Point2D)(Joueur.Position * ConversionArea);
            
            if (Joueur.Position.X < 0)
            {
                indexJoueur.X -= 1;
            }
            if (Joueur.Position.Y > 0)
            {
                indexJoueur.Y -= 1;
            }

            MoteurJeu.AwesomeBox.WriteLine("Index joueur : " + indexJoueur.ToString());
            indexJoueur -= IndexMap;


            
            if (indexJoueur != IndexAreaJoueurVoulu)//on devra déplacer la map
            {
                DeltaIndexMap =  indexJoueur - IndexAreaJoueurVoulu;
                

                //MoteurJeu.AwesomeBox.WriteLine("Index joueur : " + indexJoueur.ToString());
                //MoteurJeu.AwesomeBox.WriteLine("Index Bloc   : " + Bloc.CoordVersIndex(Joueur.Position).ToString());
                //MoteurJeu.AwesomeBox.WriteLine("DeltaIndexMap : " + DeltaIndexMap.ToString());
                //MoteurJeu.AwesomeBox.WriteLine("IndexMap : " + IndexMap.ToString());

                //Thread t = new Thread(new ThreadStart(DéplacerMap));
                //t.Start();
                DéplacerMap();
            }
        }

        private void DéplacerMap()
        {
            //IndexMap += DeltaIndexMap;
            //if (DeltaIndexMap.X == 1)
            {
                //Point2D coinBasDroite = new Point2D(2, 0);
                //Point2D coinBasGauche = new Point2D(0, 0);

                foreach (Area a in Areas)
                {
                    //a.RefreshBlocs();
                    a.Save();
                    a.SupprimerBlocsDuNiveau();
                }
                IndexMap += DeltaIndexMap;
                CréerAreas();
                foreach (Area a in Areas)
                {
                    a.LoadOuGénérer();
                    a.InsérerBlocsDansNiveau();
                    a.CalculerGazonEtOmbre();
                }
                ListeBlocVérifierChute.Clear();


            }
            //else if (DeltaIndexMap.X == -1)
            //{
            //}

            MinIndex = (IndexMap * DimensionsAreas);
            MaxIndex = MinIndex + (Point2D)DimensionsGrille;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            AfficherTuile();
        }
             
    }
}
