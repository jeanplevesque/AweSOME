using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AweSOME
{
    partial class InfiniteLevel
    {
        public override Bloc GetBloc(int indexX, int indexY)
        {
            indexX -= IndexMap.X * DimensionsAreas.X;
            indexY -= IndexMap.Y * DimensionsAreas.Y;

            return base.GetBloc(indexX, indexY);
        }
        public override Tuile GetTuile(int indexX, int indexY)
        {
            indexX -= IndexMap.X * DimensionsAreas.X;
            indexY -= IndexMap.Y * DimensionsAreas.Y;

            return base.GetTuile(indexX, indexY);
        }


        public override void CollerBloc(Bloc bloc)
        {
            Point index = Bloc.CoordVersIndex(bloc.Position);
            int i = Maths.Clamp(index.X, MaxIndex.X - 1, MinIndex.X);
            int j = Maths.Clamp(index.Y, MaxIndex.Y - 1, MinIndex.Y);

            Bloc blocDeTrop = GetBloc(i, j);

            //i = TransformIndexX(i);
            //j = TransformIndexY(j);

            if (blocDeTrop != null && blocDeTrop != bloc)
            {
                ListeBlocÀDessiner.Remove(blocDeTrop);
            }

            ChangerGrille(TransformIndexX(i), TransformIndexY(j), bloc);

            //Ajustement de la position
            bloc.Placer(i, j);
            bloc.VérifierSiChute();
            bloc.VérifierSiCréerOmbre();

            bloc.ÉtatPrésent = ÉtatsBloc.Statique;
            ListeBlocÀColler.Add(bloc);

            GestionEffets.CréerPoussièreAuSol(bloc.Position + Vector2.UnitY * Bloc.DIMENSION_BLOC / 2, bloc.Matériel, 3);
        }
        public override void FaireTomberBloc(int i, int j)
        {
            Bloc bloc = GetBloc(i, j);
            if (bloc == null || bloc.ÉtatPrésent == ÉtatsBloc.TombeVerticalement) { return; }

            if (bloc.EstTunel)
            {
                bloc.AbimerPourDétruire();
            }
            else
            {
                ChangerGrille(TransformIndexX(i), TransformIndexY(j), null);
                bloc.ÉtatPrésent = ÉtatsBloc.TombeVerticalement;
                ListeBlocQuiTombe.Add(bloc);
            }
            bloc.ÉtatPrésent = ÉtatsBloc.TombeVerticalement;
            DécrocherBlocAutour(i, j);
        }
        public override void DétruireBloc(Bloc b)
        {
            if (b == null) { return; }
            if (b.PossèdeOmbre) { b.DétruireOmbre(); }
            int i = b.IndexX;
            int j = b.IndexY;
            ChangerGrille(TransformIndexX(i), TransformIndexY(j), null);
            ListeBlocÀDessiner.Remove(b);

            DécrocherBlocAutour(i, j);
        }


        public override int TransformIndexX(int x)
        {
            return x - IndexMap.X * DimensionsAreas.X;
        }
        public override int TransformIndexY(int y)
        {
            return y - IndexMap.Y * DimensionsAreas.Y;
        }
        public override Point TransformIndex(Point index)
        {
            return (Point2D)index - IndexMap * DimensionsAreas;
        }

        public virtual int UnTransformIndexX(int x)
        {
            return x + IndexMap.X * DimensionsAreas.X;
        }
        public virtual int UnTransformIndexY(int y)
        {
            return y + IndexMap.Y * DimensionsAreas.Y;
        }
        public virtual Point UnTransformIndex(Point index)
        {
            return (Point2D)index + IndexMap * DimensionsAreas;
        }
    }
}
