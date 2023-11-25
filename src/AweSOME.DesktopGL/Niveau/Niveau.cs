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
        public Player Joueur;

        public int NbPolygones { get { return ListePolygonesPhysiques.Count; } }
        
        public List<PolygonePhysique> ListePolygonesPhysiques = new List<PolygonePhysique>();
        public List<PolygonePhysique> ListePolygonesPhysiquesLibres = new List<PolygonePhysique>();
        public List<PolygonePhysique> ListePolyÀDétruire = new List<PolygonePhysique>();
        public List<PolygonePhysique> ListePolyÀAjouter = new List<PolygonePhysique>();

        public List<Personnage> ListePersonnages = new List<Personnage>();
        public List<BalleFusil> ListeBallesFusil = new List<BalleFusil>();
        public List<BalleFusil> ListeBallesFusilÀDétruire = new List<BalleFusil>();

        public List<IRamassable> ListeObjetsRamassable = new List<IRamassable>();
        public List<IRamassable> ListeObjetsRamassableÀDétruire = new List<IRamassable>();
        public List<IRamassable> ListeObjetsRamassableÀAjouter = new List<IRamassable>();

        public List<ISpell> ListeISpell = new List<ISpell>();
        public List<ISpell> ListeISpellÀDétruire = new List<ISpell>();

        int minNbBlocs_;
        //Vector2 positionCentre_;
        //Sprite BackGround;


        public Niveau(int dimX, int dimY)
        {
            GestionSpawn.NiveauActif = this;

            MoteurJeu.EnginKrypton.Hulls.Clear();
            MoteurJeu.EnginKrypton.Lights.Clear();   
                        
            DimensionsGrille = new Point(dimX,dimY);
            DimensionsNiveau = new Vector2(dimX,dimY) * Bloc.DIMENSION_BLOC + Bloc.DIMENSIONS_BLOC;


            Grille = new Bloc[dimX, dimY];
            GrilleTuiles = new Tuile[dimX, dimY];
        }

        public virtual void Initialiser()
        {
            Construire();
            GestionSpawn.CalculerTuilesSpawnable();

            InitialiserEnvironnement();

            Joueur = new Player(GestionSpawn.GetRandomSpawnPosition());
            ListePersonnages.Add(Joueur);

            //ListePersonnages.Add(new Zombie(GestionSpawn.GetRandomSpawnPosition(), Bloc.DIMENSION_BLOC));
        }
        
        private void Construire()
        {
            //Random random = new Random(3);
            Random random = new Random();

            int nbBlocsY = (int)DimensionsGrille.Y / 2;
            minNbBlocs_ = nbBlocsY;
            int nombreRandom = 0;
            for (int i = 0; i < DimensionsGrille.X; ++i)
            {
                for (int j = 0; j < DimensionsGrille.Y; ++j)
                {
                    GrilleTuiles[i, j] = new Tuile(i, j, null);
                }
            }
            for (int i = 0; i < DimensionsGrille.X; ++i)
            {
                nombreRandom = random.Next(0, 10);
                if (nombreRandom < 6) { }
                else if (nombreRandom < 8) { nbBlocsY = Maths.Clamp(random.Next(nbBlocsY - 2, nbBlocsY + 3), (int)DimensionsGrille.Y, 3); }
                else if (nombreRandom < 10) { nbBlocsY = Maths.Clamp(random.Next(nbBlocsY - 3, nbBlocsY + 3), (int)DimensionsGrille.Y, 3); }

                for (int j = 0; j < nbBlocsY; ++j)
                {
                    Bloc dessous = GetBloc(i, j - 1);
                    if ((dessous != null && dessous.Matériel == MatérielPolygone.Terre) || (j > nbBlocsY / 2 + random.Next(10)))
                    {
                        AjouterBloc(i, j, MatérielPolygone.Terre);
                    }
                    else
                    {
                        AjouterBloc(i, j, MatérielPolygone.Pierre);
                    }
                }
            }
            //on smooth la surface
            foreach (Bloc b in Grille)
            {
                if (b != null)
                {
                    if (!BlocPrésent(b.TransformedIndexX + 1, b.TransformedIndexY) && !BlocPrésent(b.TransformedIndexX-1, b.TransformedIndexY))
                    {
                        DétruireBloc(b);
                    }
                }
            }
            for (int i = 0; i < DimensionsGrille.X; ++i)
            {
                for (int j = 0; j < DimensionsGrille.Y; ++j)
                {
                    if (!BlocPrésent(i, j) && BlocPrésent(i + 1, j) && BlocPrésent(i - 1, j))
                    {
                        AjouterBloc(i, j, MatérielPolygone.Terre);
                    }
                }
            }

            // Section des Ombres / Gazons
            foreach (Bloc b in Grille)
            {
                if (b != null)
                {
                    if (!b.EstEntouréNonTunel)
                    {
                        b.CréerOmbre(127);
                    }
                    if (b.Matériel == MatérielPolygone.Terre && !BlocPrésent(b.TransformedIndexX, b.TransformedIndexY + 1))
                    {
                        b.ArrangerSprite(GestionTexture.GetTexture("Bloc/CaseGazon" + random.Next(0, 5).ToString()), new Vector2(64, 68.2666667f), Vector2.One / 2, b.Profondeur, b.Couleur);
                        b.OrigineSprite = new Vector2(30, 34);
                        //b.Dimensions = new Vector2(60, 64);
                    }
                }
            }
            // Calculs des Tuiles
            foreach (Tuile t in GrilleTuiles)
            {
                t.CalculerVoisins(false);
            }
        }

        public void AjouterISpell(ISpell iSpell)
        {
            ListeISpell.Add(iSpell);
        }
        public void DétruireISpell(ISpell iSpell)
        {
            ListeISpellÀDétruire.Add(iSpell);
        }

        public void AjouterObjetRamassable(IRamassable objet)
        {
            ListeObjetsRamassableÀAjouter.Add(objet);
        }
        public void DétruireObjetRamassable(IRamassable objet)
        {
            ListeObjetsRamassableÀDétruire.Add(objet);
        }

        public void AjouterBalleFusil(BalleFusil balle)
        {
            ListeBallesFusil.Add(balle);
        }
        public void DétruireBalleFusil(BalleFusil balle)
        {
            ListeBallesFusilÀDétruire.Add(balle);
        }

        public void AjouterPolygonePhysique(PolygonePhysique poly)
        {
            ListePolyÀAjouter.Add(poly);
            //ListePolygonesPhysiques.Add(poly);
            //ListePolygonesPhysiquesLibres.Add(poly);
        }       
        public void DétruirePolygonePhysique(PolygonePhysique poly)
        {
            ListePolyÀDétruire.Add(poly);
        }
        
        public void Nettoyer()
        {

            foreach (IRamassable p in ListeObjetsRamassableÀAjouter)
            {
                ListeObjetsRamassable.Add(p);
            }
            ListeObjetsRamassableÀAjouter.Clear();

            foreach (PolygonePhysique p in ListePolyÀAjouter)
            {
                ListePolygonesPhysiques.Add(p);
                ListePolygonesPhysiquesLibres.Add(p);
            }
            ListePolyÀAjouter.Clear();

            foreach (PolygonePhysique p in ListePolyÀDétruire)
            {
                EnleverPolygonePhysique(p);

                if(p is Bloc)
                {
                    Bloc bloc = GetBloc(((Bloc)p).Index);
                    if (bloc == ((Bloc)p))
                    {
                        ChangerGrille(bloc.TransformedIndexX, bloc.TransformedIndexY, null);   
                        ListeBlocÀDessiner.Remove(bloc);
                        DécrocherBlocAutour(bloc.IndexX, bloc.IndexX);
                    }                    
                }
            }
            foreach (IRamassable i in ListeObjetsRamassableÀDétruire)
            {
                ListeObjetsRamassable.Remove(i);
            }
            foreach (ISpell s in ListeISpellÀDétruire)
            {
                ListeISpell.Remove(s);
            }
            foreach (BalleFusil b in ListeBallesFusilÀDétruire)
            {
                ListeBallesFusil.Remove(b);
            }
            foreach (Bloc b in ListeBlocÀColler)
            {
                ListeBlocQuiTombe.Remove(b);
            }
            ListeObjetsRamassableÀDétruire.Clear();
            ListeBallesFusilÀDétruire.Clear();
            ListeISpellÀDétruire.Clear();
            ListePolyÀDétruire.Clear();
            ListeBlocÀColler.Clear();
        }

        private void EnleverPolygonePhysique(PolygonePhysique poly)
        {
            ListePolygonesPhysiques.Remove(poly);
            ListePolygonesPhysiquesLibres.Remove(poly);
            if (poly is Personnage)
            {
                ListePersonnages.Remove((Personnage)poly);
            }
        }

        

        public virtual void Update()
        {
            UpdateÉtatJour();

            Nettoyer();
            foreach (ISpell i in ListeISpell)
            {
                i.Updater();
            }
            foreach (IRamassable i in ListeObjetsRamassable)
            {
                i.Update();
                i.VérifierCollisionsDécors();
                i.VérifierCollisionsPersonnages();
            }
            foreach (PolygonePhysique p in ListePolygonesPhysiques)
            {
                p.Update();
            }
            foreach (PolygonePhysique p in ListeBlocQuiTombe)
            {
                p.Update();
                p.VérifierCollisionsPersonnages();//Les blocs qui tombes en lignes droite tuent les personnages qui sont dessous
            }
            foreach (BalleFusil b in ListeBallesFusil)
            {
                
                b.VérifierCollisions();
                b.ChoisirIntersection();
                b.Update();
            }
            foreach (PolygonePhysique p in ListePolygonesPhysiquesLibres)
            {
                p.VérifierCollisionsDécors();
                p.VérifierCollisionsPersonnages();
            }
            foreach (Personnage p in ListePersonnages)
            {
                p.VérifierCollisionsDécors();//On effectue le test de collision Avant pour savoir si                
                p.Update();                  //le personnage est sur le sol pour les autres calculs               
            }
            DécrocherBlocs();
            FaireTomberBlocs();
            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            //BackGround.Draw(spriteBatch);

            DrawPoly(spriteBatch);
            DessinerAstres();
            
        }

        
        private void DrawPoly(SpriteBatch spriteBatch)
        {
            foreach (Bloc p in ListeBlocÀDessiner)
            {
                p.Draw(spriteBatch);              
            }
            foreach (PolygonePhysique p in ListePolygonesPhysiques)
            {
                p.Draw(spriteBatch);
            }
            foreach (IRamassable i in ListeObjetsRamassable)
            {
                i.Draw(spriteBatch);
            }
            foreach (Personnage p in ListePersonnages)
            {
                p.Draw(spriteBatch);
            }
            foreach (BalleFusil b in ListeBallesFusil)
            {
                b.Draw(spriteBatch);
            }
        }





        
    }
}
