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
    partial class Niveau
    {
        public Bloc[,] Grille;
        public Tuile[,] GrilleTuiles;
        public Point DimensionsGrille;
        public Vector2 DimensionsNiveau;

        public List<Bloc> ListeBlocQuiTombe = new List<Bloc>();
        public List<Bloc> ListeBlocÀColler = new List<Bloc>();
        public List<Bloc> ListeBlocÀDessiner = new List<Bloc>();

        public List<Bloc> ListeBlocVérifierChute = new List<Bloc>();
        public List<Bloc> ListeBlocÀFaireTomber = new List<Bloc>();

        public void ChangerGrille(int i, int j, Bloc bloc)
        {
            Grille[i, j] = bloc;
            GrilleTuiles[i, j].Modifier(bloc);
            //GrilleTuiles[i, j] = new Tuile(i,j,bloc);
            //GrilleTuiles[i, j].CalculerVoisins();
            GestionSpawn.CalculerTuilesSpawnable();
        }

        public virtual void CollerBloc(Bloc bloc)
        {
            Point index = Bloc.CoordVersIndex(bloc.Position);
            int i = Maths.Clamp(index.X, (int)DimensionsGrille.X - 1, 0);// (int)(bloc.Position.X / Bloc.DIMENSION_BLOC);
            int j = Maths.Clamp(index.Y, (int)DimensionsGrille.Y - 1, 0);//(int)(bloc.Position.Y / -Bloc.DIMENSION_BLOC);

            Bloc blocDeTrop = Grille[i, j];
            if (blocDeTrop != null && blocDeTrop != bloc)
            {
                ListeBlocÀDessiner.Remove(blocDeTrop);
            }

            ChangerGrille(i, j, bloc);

            //Ajustement de la position
            bloc.Placer(i, j);
            bloc.VérifierSiChute();
            bloc.VérifierSiCréerOmbre();

            bloc.ÉtatPrésent = ÉtatsBloc.Statique;
            ListeBlocÀColler.Add(bloc);

            GestionEffets.CréerPoussièreAuSol(bloc.Position + Vector2.UnitY * Bloc.DIMENSION_BLOC / 2, bloc.Matériel, 3);
        }

        public void FaireTomberBlocs()
        {
            foreach (Bloc b in ListeBlocÀFaireTomber)
            {
                FaireTomberBloc(b);
            }
            ListeBlocÀFaireTomber.Clear();
        }
        public virtual void FaireTomberBloc(int i, int j)
        {
            Bloc bloc = GetBloc(i, j);
            if (bloc == null || bloc.ÉtatPrésent == ÉtatsBloc.TombeVerticalement) { return; }

            if (bloc.EstTunel)
            {
                bloc.AbimerPourDétruire();
            }
            else
            {
                ChangerGrille(i, j, null);
                bloc.ÉtatPrésent = ÉtatsBloc.TombeVerticalement;
                ListeBlocQuiTombe.Add(bloc);
            }
            bloc.ÉtatPrésent = ÉtatsBloc.TombeVerticalement;
            DécrocherBlocAutour(i, j);
        }
        public void FaireTomberBloc(Bloc bloc)
        {
            FaireTomberBloc(bloc.IndexX, bloc.IndexY);
        }
        public void AjouterBlocÀFaireTomber(Bloc bloc)
        {
            ListeBlocÀFaireTomber.Add(bloc);
        }

        //public void RendreBlocPhysique(int i, int j)
        //{
        //    Bloc bloc = GetBloc(i, j);
        //    Grille[i, j] = null;
        //    bloc.ÉtatPrésent = ÉtatsBloc.Physique;
        //    ListePolygonesPhysiquesLibres.Add(bloc);
        //    if (j + 1 == TopColonnes[i])
        //    {
        //        --TopColonnes[i];
        //    }
        //    DécrocherBlocAutour(i, j);
        //}
        //public void RendreBlocPhysique(Bloc bloc)
        //{
        //    RendreBlocPhysique(bloc.IndexX, bloc.IndexY);
        //}

        //public void RendreBlocPhysiqueCollante(int i, int j)
        //{
        //    Bloc bloc = GetBloc(i, j);
        //    if (bloc == null || bloc.ÉtatPrésent == ÉtatsBloc.TombeVerticalement) { return; }

        //    if (bloc.EstTunel)
        //    {
        //        bloc.AbimerPourDétruire();
        //    }
        //    else
        //    {
        //        Grille[i, j] = null;
        //        bloc.ÉtatPrésent = ÉtatsBloc.PhysiquePourColler;
        //        ListeBlocQuiTombe.Add(bloc);
        //        if (j + 1 == TopColonnes[i])
        //        {
        //            --TopColonnes[i];
        //        }
        //    }
        //    bloc.ÉtatPrésent = ÉtatsBloc.PhysiquePourColler;
        //    DécrocherBlocAutour(i, j);
        //}
        public void AjouterBlocPhysiqueCollante(Bloc bloc)
        {
            ListeBlocÀDessiner.Add(bloc);
            if (bloc.EstTunel)
            {
                bloc.AbimerPourDétruire();
            }
            else
            {
                bloc.ÉtatPrésent = ÉtatsBloc.PhysiquePourColler;
                ListeBlocQuiTombe.Add(bloc);
            }
            bloc.ÉtatPrésent = ÉtatsBloc.PhysiquePourColler;
        }

        public void AjouterBloc(Vector2 position)
        {
            int i = (int)((position.X + Bloc.DIMENSION_BLOC / 2) / Bloc.DIMENSION_BLOC);
            int j = (int)((position.Y - Bloc.DIMENSION_BLOC / 2) / -Bloc.DIMENSION_BLOC);
            if (i >= 0 && j >= 0 && i < DimensionsGrille.X && j < DimensionsGrille.Y)
            {
                if (!BlocPrésent(i, j))
                {
                    AjouterBloc(i, j, MatérielPolygone.Terre);
                    GetBloc(i, j).VérifierSiChute();
                }
            }
        }
        public void AjouterBloc(Vector2 position, MatérielPolygone matériel)
        {
            int i = (int)((position.X + Bloc.DIMENSION_BLOC / 2) / Bloc.DIMENSION_BLOC);
            int j = (int)((position.Y - Bloc.DIMENSION_BLOC / 2) / -Bloc.DIMENSION_BLOC);
            if (i >= 0 && j >= 0 && i < DimensionsGrille.X && j < DimensionsGrille.Y)
            {
                if (!BlocPrésent(i, j))
                {
                    AjouterBloc(i, j, matériel);
                    GetBloc(i, j).VérifierSiChute();
                }
            }
        }
        protected void AjouterBloc(int i, int j, MatérielPolygone matériel)
        {
            Bloc nouveauBloc = new Bloc(i, j, matériel);
            ChangerGrille(i, j, nouveauBloc);
            ListeBlocÀDessiner.Add(nouveauBloc);
        }

        protected void DécrocherBlocAutour(int i, int j)
        {
            //Bloc de Droite
            Bloc bloc = GetBloc(i + 1, j);
            if (bloc != null) { ListeBlocVérifierChute.Add(bloc); }
            //Bloc de Gauche
            bloc = GetBloc(i - 1, j);
            if (bloc != null) { ListeBlocVérifierChute.Add(bloc); }
            //Bloc du Haut
            bloc = GetBloc(i, j + 1);
            if (bloc != null) { ListeBlocVérifierChute.Add(bloc); }
            //Bloc du Haut à Droite
            bloc = GetBloc(i + 1, j + 1);
            if (bloc != null) { ListeBlocVérifierChute.Add(bloc); }
            //Bloc du Haut à Droite
            bloc = GetBloc(i - 1, j + 1);
            if (bloc != null) { ListeBlocVérifierChute.Add(bloc); }

        }
        private void DécrocherBlocs()
        {
            foreach (Bloc b in ListeBlocVérifierChute)
            {
                b.VérifierSiChute();
            }
            ListeBlocVérifierChute.Clear();
        }

        public virtual void DétruireBloc(Bloc b)
        {
            if (b == null) { return; }
            if (b.PossèdeOmbre) { b.DétruireOmbre(); }
            int i = b.IndexX;
            int j = b.IndexY;
            ChangerGrille(i, j, null);
            ListeBlocÀDessiner.Remove(b);

            DécrocherBlocAutour(i, j);
        }
        //public void DétruireBloc(Vector2 position)
        //{
        //    Point point = Bloc.CoordVersIndex(position);
        //    int i = point.X;//= (int)((position.X + Bloc.DIMENSION_BLOC / 2) / Bloc.DIMENSION_BLOC);
        //    int j = point.Y;//= (int)((position.Y - Bloc.DIMENSION_BLOC / 2) / -Bloc.DIMENSION_BLOC);
        //    if (i >= 0 && j >= 0 && i < DimensionsGrille.X && j < DimensionsGrille.Y)
        //    {
        //        if (BlocPrésent(i, j))
        //        {
        //            DétruireBloc(GetBloc(i, j));
        //        }
        //    }
        //}

        public Bloc GetBloc(Vector2 position)
        {
            return GetBloc(Bloc.CoordVersIndex(position));
        }
        public Bloc GetBloc(ref Vector2 position)
        {
            return GetBloc(Bloc.CoordVersIndex(ref position));
        }
        public Bloc GetBloc(Point point)
        {
            return GetBloc(point.X, point.Y);
        }
        public virtual Bloc GetBloc(int indexX, int indexY)
        {
            if (indexX < 0 || indexY < 0 || indexX >= DimensionsGrille.X || indexY >= DimensionsGrille.Y)
            {
                return null;
            }
            return Grille[indexX, indexY];
        }

        public bool BlocPrésent(int indexX, int indexY)
        {
            return GetBloc(indexX, indexY) != null;
        }
        public bool BlocPrésentNonTunel(int indexX, int indexY)
        {
            Bloc bloc = GetBloc(indexX, indexY);
            return bloc != null && !bloc.EstTunel;
        }
        public bool BlocPrésent(Vector2 position)
        {
            Point point = Bloc.CoordVersIndex(position);
            int i =point.X;//= (int)((position.X + Bloc.DIMENSION_BLOC / 2) / Bloc.DIMENSION_BLOC);
            int j =point.Y;//= (int)((position.Y - Bloc.DIMENSION_BLOC / 2) / -Bloc.DIMENSION_BLOC);
            if (i >= 0 && j >= 0 && i < DimensionsGrille.X && j < DimensionsGrille.Y)
            {
                return BlocPrésent(i, j);
            }
            return false;
        }
        public bool BlocPrésentNonTunel(Vector2 position)
        {
            Point point = Bloc.CoordVersIndex(position);
            int i = point.X;//= (int)((position.X + Bloc.DIMENSION_BLOC / 2) / Bloc.DIMENSION_BLOC);
            int j = point.Y;//= (int)((position.Y - Bloc.DIMENSION_BLOC / 2) / -Bloc.DIMENSION_BLOC);
            if (i >= 0 && j >= 0 && i < DimensionsGrille.X && j < DimensionsGrille.Y)
            {
                Bloc bloc = GetBloc(i, j);
                return (bloc != null && !bloc.EstTunel);

            }
            return false;
        }

        public virtual bool EstDansNiveau(Vector2 position)
        {
            return (position.X > 0 && position.Y < 0 && position.X <= DimensionsNiveau.X && position.Y <= DimensionsNiveau.Y);
        }

        public void CréerTunel(Point index)
        {
            Bloc bloc = GetBloc(index);
            if (bloc != null)
            {
                bloc.EstTunel = true;
                GetTuile(bloc).Passable = true;
            }
        }
        public void CréerTunel(Vector2 position)
        {
            CréerTunel(Bloc.CoordVersIndex(position));
        }

        public Tuile GetTuile(Bloc blocCorrespondant)
        {
            return GrilleTuiles[blocCorrespondant.TransformedIndexX, blocCorrespondant.TransformedIndexY];
        }
        public virtual Tuile GetTuile(int indexX, int indexY)
        {
            if (indexX < 0 || indexY < 0 || indexX >= DimensionsGrille.X || indexY >= DimensionsGrille.Y)
            {
                return null;
            }
            return GrilleTuiles[indexX, indexY];
        }
        public Tuile GetTuile(Point index)
        {
            return GetTuile(index.X,index.Y);
        }
        public Tuile GetTuile(Vector2 position)
        {
            return GetTuile(Bloc.CoordVersIndex(position));
        }

        public virtual int TransformIndexX(int x)
        {
            return x;
        }
        public virtual int TransformIndexY(int y)
        {
            return y;
        }
        public virtual Point TransformIndex(Point index)
        {
            return index;
        }
        public virtual int UnTransformIndexX(int x)
        {
            return x;
        }
        public virtual int UnTransformIndexY(int y)
        {
            return y;
        }
        public virtual Point UnTransformIndex(Point index)
        {
            return index;
        }

        public void ResetPoidsTuiles()
        {
            foreach (Tuile t in GrilleTuiles)
            {
                t.ResetPoids();
            }
        }

        public void AfficherTuile()
        {
            foreach (Tuile t in GrilleTuiles)
            {
                t.Draw();
            }
        }
    }
}
