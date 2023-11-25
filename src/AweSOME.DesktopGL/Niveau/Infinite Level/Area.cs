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
    class Area
    {
        public InfiniteLevel Level;
        public Point2D IndexÉcran;
        public Point2D IndexRéel;
        public Point2D Dimensions;


        public Bloc[,] GrilleBlocs;

        public Area(InfiniteLevel level, Point2D indexRéel, Point2D dimensions)
        {
            
            Level = level;           
            IndexRéel = indexRéel;
            IndexÉcran = indexRéel - Level.IndexMap;
            Dimensions = dimensions;

            GrilleBlocs = new Bloc[Dimensions.X, Dimensions.Y];
        }

        public void RefreshBlocs()
        {
            for (int i = 0; i < Dimensions.X; ++i)
            {
                for (int j = 0; j < Dimensions.Y; ++j)
                {
                    GrilleBlocs[i, j] = Level.Grille[IndexÉcran.X * Dimensions.X + i, IndexÉcran.Y * Dimensions.Y + j];
                    
                    //---------------TEST------------------
                    //if (GrilleBlocs[i, j] != null)
                    //{
                    //    EffetTexteFlotant texte = new EffetTexteFlotant(GrilleBlocs[i, j].Position, Vector2.One * 64, 1000);
                    //    texte.Write(IndexRéel.ToString());
                    //    GestionEffets.AjouterEffetAdditiveBlend(texte);
                    //}
                    //----------END--TEST------------------
                }
            }
        }

        public void Save()
        {
            //Obtenir les blocs
            RefreshBlocs();
            //Sauvegarder Chaque bloc
            FileStream fileStream = new FileStream("Maps/" + IndexRéel.X + " " + IndexRéel.Y, FileMode.Create, FileAccess.ReadWrite);
            BinaryWriter writer = new BinaryWriter(fileStream);

            writer.Write(Dimensions.X);
            writer.Write(Dimensions.Y);

            Bloc bloc = null;
            for (int i = 0; i < Dimensions.X; ++i)
            {
                for (int j = 0; j < Dimensions.Y; ++j)
                {
                    bloc = GrilleBlocs[i, j] ?? new Bloc(i, j, MatérielPolygone.Inconnu);
                    
                    bloc.Save(writer);
                }
            }

            writer.Close();
            fileStream.Close();
        }

        public void LoadOuGénérer()
        {
            if (!GénérerFromFile())
            {
                Générer();
            }
        }

        public void Générer()
        {
            int nbBlocsY = (int)Dimensions.Y/2;
            Random random = Maths.Random;
            int nombreRandom;

            for (int i = 0; i < this.Dimensions.X; ++i)
            {
                nombreRandom = random.Next(0, 10);
                if (nombreRandom < 6) { }
                else if (nombreRandom < 8) { nbBlocsY = Maths.Clamp(random.Next(nbBlocsY - 2, nbBlocsY + 3), (int)Dimensions.Y, 3); }
                else if (nombreRandom < 10) { nbBlocsY = Maths.Clamp(random.Next(nbBlocsY - 3, nbBlocsY + 3), (int)Dimensions.Y, 3); }

                for (int j = 0; j < nbBlocsY; ++j)
                {
                    Bloc dessous = GetBloc(i, j - 1);
                    if ((dessous != null && dessous.Matériel == MatérielPolygone.Terre) || (j > nbBlocsY / 2 + random.Next(10)))
                    {
                        GrilleBlocs[i, j] = new Bloc(IndexRéel.X * Dimensions.X + i, IndexRéel.Y * Dimensions.Y + j, MatérielPolygone.Terre);
                        //GrilleBlocs[i, j].CalculerPosition(IndexRéel.X * Dimensions.X + i, IndexRéel.Y * Dimensions.Y + j);
                    }
                    else
                    {
                        GrilleBlocs[i, j] = new Bloc(IndexRéel.X * Dimensions.X + i, IndexRéel.Y * Dimensions.Y + j, MatérielPolygone.Pierre);
                        //GrilleBlocs[i, j].CalculerPosition(IndexRéel.X * Dimensions.X + i, IndexRéel.Y * Dimensions.Y + j);
                    }
                }
            }
        }

        public bool GénérerFromFile()
        {
            try
            {
                FileStream fileStream = new FileStream("Maps/" + IndexRéel.X + " " + IndexRéel.Y, FileMode.Open, FileAccess.ReadWrite);
                BinaryReader reader = new BinaryReader(fileStream);

                Point2D dimensionsLues;
                dimensionsLues.X = reader.ReadInt32();
                dimensionsLues.Y = reader.ReadInt32();
                if (Dimensions != dimensionsLues)
                {
                    fileStream.Close();
                    reader.Close(); 
                    return false;
                }

                Bloc bloc = null;


                for (int i = 0; i < Dimensions.X; ++i)
                {
                    for (int j = 0; j < Dimensions.Y; ++j)
                    {
                        bloc = new Bloc(IndexRéel.X * Dimensions.X + i, IndexRéel.Y * Dimensions.Y + j, (MatérielPolygone)reader.ReadByte(),reader.ReadByte());
                        bloc.DégatsAccumulés = reader.ReadInt16();
                        bloc.EstTunel = reader.ReadBoolean();
                        //bloc.CalculerPosition(IndexRéel.X * Dimensions.X + i, IndexRéel.Y * Dimensions.Y + j);

                        GrilleBlocs[i, j] = bloc.Matériel == MatérielPolygone.Inconnu ? null : bloc; // si le matériel est inconnu, il n'y a pas de bloc
                    }
                }

                fileStream.Close();
                reader.Close();

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public void InsérerBlocsDansNiveau()
        {
            for (int i = 0; i < Dimensions.X; ++i)
            {
                for (int j = 0; j < Dimensions.Y; ++j)
                {
                    Bloc bloc = GrilleBlocs[i, j];
                    Level.ChangerGrille(IndexÉcran.X * Dimensions.X + i, IndexÉcran.Y * Dimensions.Y + j, bloc);
                    if (bloc != null)
                    {
                        Level.ListeBlocÀDessiner.Add(bloc);
                    }
                }
            }
        }

        public Bloc GetBloc(int indexX, int indexY)
        {
            if (indexX < 0 || indexY < 0 || indexX >= Dimensions.X || indexY >= Dimensions.Y)
            {
                return null;
            }
            return GrilleBlocs[indexX, indexY];
        }

        public void SupprimerBlocsDuNiveau()
        {
            foreach (Bloc b in GrilleBlocs)
            {
                Level.DétruireBloc(b);
            }
        }
        public void MettreLesBlocsÀNull()
        {
            GrilleBlocs = new Bloc[Dimensions.X, Dimensions.Y];
        }

        public void CalculerGazonEtOmbre()
        {
            foreach (Bloc b in GrilleBlocs)
            {
                if (b != null)
                {
                    if (!b.EstEntouréNonTunel)
                    {
                        b.CréerOmbre(127);
                    }
                    if (b.Matériel == MatérielPolygone.Terre && !Level.BlocPrésent(b.IndexX, b.IndexY + 1))
                    {
                        b.ArrangerSprite(GestionTexture.GetTexture("Bloc/CaseGazon" + b.NoImages), new Vector2(64, 68.2666667f), Vector2.One / 2, b.Profondeur, b.Couleur);
                        b.OrigineSprite = new Vector2(30, 34);
                    }
                }
            }
        }
    }

    
}
