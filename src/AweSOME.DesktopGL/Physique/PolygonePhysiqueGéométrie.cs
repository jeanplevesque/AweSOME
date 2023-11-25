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
using Krypton;

namespace AweSOME
{
    partial class PolygonePhysique
    {
        public int NbSommets;
       
        public ShadowHull Ombre;
        public bool PossèdeOmbre { get; protected set; }

        public Matrix Matrice;

        public Vector2[] OrientationSommetsInitiales;
        public Vector2[] OrientationSommets;

        //public PolygonePhysique(Vector2[] listePositionSommets, PolygonePhysique parent)
        //{
        //    Vitesse = parent.Vitesse;
        //    VitesseAngulaire = parent.VitesseAngulaire;
        //    NbSommets = listePositionSommets.Length;
        //    //Dimensions = Vector2.One * 45;
        //    //Générer();
        //    for (int i = 0; i < NbSommets; ++i)
        //    {
        //        ListeSommets.Add(new Sommet(listePositionSommets[i], Vector2.Zero, this));
        //    }
        //    RelierSommets(1);
        //    CalculerPosition();
        //    CalculerDistanceCentrePolygoneInitiale(ref parent.Angle);
        //    Rayon = TrouverPlusGrandRayon();
        //    Dimensions = Vector2.One * ((float)Math.Sqrt(Rayon * Rayon * 4));

        //    int nbPixelsCouleurs = 0;
        //    Image = AwesomeBatyTexture(parent.Image, parent.Angle, parent.Position, parent.Scale, out nbPixelsCouleurs);
        //    //ImagesOrigines[0] = parent.ImagesOrigines[0];
        //    //ImagesOrigines[1] = parent.ImagesOrigines[1];

        //    Masse = nbPixelsCouleurs*parent.Scale.X*parent.Scale.Y;
        //    InverseInertie = 1f / (1f / 12f * Masse * (Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y));

        //    ArrangerSprite(Image, parent.Dimensions, Vector2.One / 2f, 0, Color.White);
        //}

        public void CréerOmbre(byte darkness)
        {
            Ombre = ShadowHull.CreateRectangle(Dimensions);
            Ombre.Color.A = darkness;
            MoteurJeu.EnginKrypton.Hulls.Add(Ombre);

            PossèdeOmbre = true;
        }
        public void DétruireOmbre()
        {
            if (PossèdeOmbre)
            {
                MoteurJeu.EnginKrypton.Hulls.Remove(Ombre);
                Ombre.Enabled = false;
                Ombre = null;

                PossèdeOmbre = false;
            }
        }

        public void Générer()
        {
            ListeDroites.Clear();
            ListeSommets.Clear();

            NbSommets = 4;
            //OrientationSommets = new Vector2[NbSommets];
            //OrientationSommetsInitiales = new Vector2[NbSommets];

            Vector2 dist1 = new Vector2(Dimensions.X / 2f, Dimensions.Y / 2);
            Vector2 dist2 = new Vector2(Dimensions.X / 2f, -Dimensions.Y / 2);
            Vector2 dist3 = new Vector2(-Dimensions.X / 2f, -Dimensions.Y / 2);
            Vector2 dist4 = new Vector2(-Dimensions.X / 2f, Dimensions.Y / 2);
            ListeSommets.Add(new Sommet(Position + dist1, dist1, this));
            ListeSommets.Add(new Sommet(Position + dist2, dist2, this));
            ListeSommets.Add(new Sommet(Position + dist3, dist3, this));
            ListeSommets.Add(new Sommet(Position + dist4, dist4, this));

            //for (int i = 0; i < ListeSommets.Count; ++i)
            //{
            //    OrientationSommetsInitiales[i] = ListeSommets[i].DistanceCentrePolygoneInitiale;
            //}           
        }
        public void GénérerRégulier(int nbSommets)
        {
            ListeDroites.Clear();
            ListeSommets.Clear();

            NbSommets = nbSommets;
            //OrientationSommets = new Vector2[NbSommets];
            //OrientationSommetsInitiales = new Vector2[NbSommets];

            float deltaAngle = MathHelper.TwoPi / NbSommets;
            float angle = 0;
            for (int i = 0; i < NbSommets; ++i)
            {
                float x = (float)(Position.X + Dimensions.X /2 * Math.Cos(angle));
                float y = (float)(Position.Y + Dimensions.Y /2 * Math.Sin(angle));
                Vector2 pos = new Vector2(x, y);
                ListeSommets.Add(new Sommet(pos, pos - Position, this));

                angle -= deltaAngle;
            }

            //for (int i = 0; i < ListeSommets.Count; ++i)
            //{
            //    OrientationSommetsInitiales[i] = ListeSommets[i].DistanceCentrePolygoneInitiale;
            //}
        }
        public void GénérerIrrégulierConvexe(List<Vector2> listeDistances)
        {
            ListeDroites.Clear();
            ListeSommets.Clear();

            NbSommets = listeDistances.Count;

            for (int i = 0; i < NbSommets; ++i)
            {
                ListeSommets.Add(new Sommet(Position + listeDistances[i], listeDistances[i], this));
            }
        }

        public void Reconstruire(Vector2 position, Vector2 dimensions)
        {
            ListeDroites.Clear();
            ListeSommets.Clear();
            Dimensions = dimensions;
            Position = position;
            Générer();
            RelierSommets(1);
            Rayon = (float)Math.Sqrt((Dimensions.X * Dimensions.X) + (Dimensions.Y * Dimensions.Y));

            Masse = Dimensions.X * Dimensions.Y;
            Inertie = (1f / 12f * Masse * (Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y)) * 2;
            InverseInertie = 1f / Inertie;

            GRAVITÉ *= Masse;
        }
        public void Reconstruire(Vector2 position,int nbSommets)//générer un cercle dans un rectangle
        {
            //NbSommets = nbSommets;
            ListeDroites.Clear();
            ListeSommets.Clear();
            //Dimensions = dimensions;
            Position = position;
            Rayon = Dimensions.X/2;
            
            GénérerRégulier(nbSommets);
            RelierSommets(1);
            
            Masse = Dimensions.X * Dimensions.Y;
            Inertie = (1f / 12f * Masse * (Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y)) * 2;
            InverseInertie = 1f / Inertie;

            GRAVITÉ *= Masse;
        }
        public void Reconstruire(Vector2 position, List<Vector2> listeDistancesSommets)
        {
            //NbSommets = nbSommets;
            ListeDroites.Clear();
            ListeSommets.Clear();
            //Dimensions = dimensions;
            Position = position;
            Rayon = Dimensions.X / 2;

            GénérerIrrégulierConvexe(listeDistancesSommets);
            RelierSommets(1);

            Masse = Dimensions.X * Dimensions.Y;
            Inertie = (1f / 12f * Masse * (Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y)) * 2;
            InverseInertie = 1f / Inertie;

            GRAVITÉ *= Masse;
        }
        
        public void CalculerPosition()
        {
            Position = Vector2.Zero;
            for (int i = 0; i < NbSommets; ++i)
            {
                Vector2 p1 = ListeSommets[i].Position;
                Vector2 p2 = ListeSommets[(i + NbSommets / 2) % NbSommets].Position;
                Position += p1 + (p2 - p1) / 2;
            }
            Position /= NbSommets;
        }
        public void CalculerDistanceCentrePolygoneInitiale(ref float angle)
        {
            Matrix matrice = Matrix.CreateRotationZ(-angle);
            for (int i = 0; i < NbSommets; ++i)
            {
                Sommet sommet = ListeSommets[i];
                sommet.DistanceCentrePolygoneInitiale = Vector2.Transform((sommet.Position - Position), matrice); ;
            }
        }
        public float TrouverPlusGrandRayon()
        {
            float maxSquared = Vector2.DistanceSquared(Position, ListeSommets[0].Position);
            for (int i = 1; i < NbSommets; ++i)
            {
                float dist = Vector2.DistanceSquared(Position, ListeSommets[i].Position);
                if (dist > maxSquared)
                {
                    maxSquared = dist;
                }
            }
            return (float)Math.Sqrt(maxSquared);
        }
        public void RelierSommets(int delta)
        {
            for (int i = 0; i < NbSommets; ++i)
            {
                ListeDroites.Add(new Droite(ListeSommets[i], ListeSommets[(i + delta) % NbSommets], this));
            }

            OrientationSommets = new Vector2[NbSommets];
            OrientationSommetsInitiales = new Vector2[NbSommets];
            for (int i = 0; i < ListeSommets.Count; ++i)
            {
                OrientationSommetsInitiales[i] = ListeSommets[i].DistanceCentrePolygoneInitiale;
            } 
        }

        public void PlacerSommets()
        {
            PlacerSommets(Angle);
        }
        public void PlacerSommets(float angle)
        {
            Matrice = Matrix.CreateRotationZ(angle);

            Vector2.Transform(OrientationSommetsInitiales, ref Matrice, OrientationSommets);
            for (int i = 0; i < ListeSommets.Count; ++i)
            {
                ListeSommets[i].Position = Position + OrientationSommets[i];
            }           
        }
        public void PlacerSommets(ref Matrix matrice, float angle)
        {
            Angle = angle;
            Matrice = matrice;

            Vector2.Transform(OrientationSommetsInitiales, ref Matrice, OrientationSommets);
            for (int i = 0; i < ListeSommets.Count; ++i)
            {
                ListeSommets[i].Position = Position + OrientationSommets[i];
            }
        }

        public virtual void CalculerPentes()
        {
            foreach (Droite d in ListeDroites)
            {
                d.CalculerPente_mx_b();
            }
        }
        public virtual void UpdateDroites()
        {
            foreach (Droite d in ListeDroites)
            {
                d.Update();
                d.CalculerPente_mx_b();
            }
        }
        //public void CréerDeuxMorceaux(Coupeuse coupeuse, DoubleIntersection doubleIntersection)
        //{
        //    List<Coin> ListeCoins = new List<Coin>();
        //    List<Vector2> ListePositions = new List<Vector2>();
        //    int cpt = 0;

        //    foreach (Sommet s in ListeSommets)
        //    {
        //        ListeCoins.Add(new Coin(s.Position, Position, false));
        //    }
        //    ListeCoins.Add(new Coin(doubleIntersection.Positions[0], Position, true));
        //    ListeCoins.Add(new Coin(doubleIntersection.Positions[1], Position, true));

        //    ListeCoins.Sort();
        //    //cpt=0;
        //    Coin coinActif;
        //    bool débuté = false;
        //    for (int i = 0; cpt < 2; ++i)
        //    {
        //        i %= ListeCoins.Count;
        //        coinActif = ListeCoins[i];
        //        if (débuté)
        //        {
        //            if (coinActif.EngendréParCoupeuse)//on ferme le polygone et on recommence
        //            {
        //                ++cpt;
        //                ListePositions.Add(coinActif.Position);
        //                PolygonePhysique poly = new PolygonePhysique(ListePositions.ToArray(), this);
        //                if (AssezGros(poly))
        //                {
        //                    GestionNiveaux.AjouterPolygonePhysique(poly);
        //                }
        //                ListePositions.Clear();
        //            }
        //            ListePositions.Add(coinActif.Position);
        //        }
        //        else if (coinActif.EngendréParCoupeuse)
        //        {
        //            débuté = true;
        //            ListePositions.Add(coinActif.Position);
        //        }

        //    }
        //    coupeuse.NbCoupes++;
        //    //SeraDétruit = true;
        //}
        
        //private bool AssezGros(PolygonePhysique poly)
        //{
        //    int cpt1 = 0;
        //    foreach (Droite d in ListeDroites)
        //    {
        //        if (Maths.LongueurVecteurApprox(d.VecteurPrincipal) < 10)
        //        {
        //            ++cpt1;
        //        }
        //    }
        //    return (poly.Masse > 50 && cpt1 < 2);
        //}
        //private Texture2D AwesomeBatyTexture(Texture2D imageParent, float angleParent, Vector2 positionParent, Vector2 scaleParent, out int nbPixelCouleurs)
        //{
        //    nbPixelCouleurs = 0;
        //    Texture2D nouvelleTexture = new Texture2D(MoteurJeu.GraphicDevice, (int)MathHelper.Clamp(imageParent.Width, 1, imageParent.Width),
        //                                                                    (int)MathHelper.Clamp(imageParent.Height, 1, imageParent.Height),
        //                                                                    false, SurfaceFormat.Color);

        //    Angle = 0;
        //    PlacerSommets();
        //    UpdateDroites();

        //    Rectangle lecteur = new Rectangle(0, 0, 1, 1);
        //    Rectangle writer = new Rectangle(0, 0, nouvelleTexture.Width, nouvelleTexture.Height);
        //    Color[] couleurs = new Color[nouvelleTexture.Height * nouvelleTexture.Width];
        //    Color[] origine = new Color[1];

        //    Vector2 delta = positionParent - Position;
        //    delta = Vector2.Transform(delta, Matrix.CreateRotationZ(-angleParent));

        //    int deltaX = (int)(delta.X / scaleParent.X);
        //    int deltaY = (int)(delta.Y / scaleParent.Y);

        //    for (int j = 0; j < nouvelleTexture.Height; ++j)
        //    {
        //        for (int i = 0; i < nouvelleTexture.Width; ++i)
        //        {
        //            if (EstÀIntérieur(new Vector2((i - imageParent.Width / 2) * scaleParent.X + Position.X,
        //                                          (j - imageParent.Height / 2) * scaleParent.Y + Position.Y)))
        //            {
        //                lecteur.X = (int)MathHelper.Clamp((i - deltaX), 0, imageParent.Width - 1);
        //                lecteur.Y = (int)MathHelper.Clamp((j - deltaY), 0, imageParent.Height - 1);
        //                imageParent.GetData<Color>(0, lecteur, origine, 0, 1);
        //                couleurs[j * nouvelleTexture.Width + i] = origine[0];
        //                nbPixelCouleurs++;
        //            }
        //            else
        //            {
        //                couleurs[j * nouvelleTexture.Width + i] = Color.Transparent;
        //                if (i == 0 || j == 0)
        //                {
        //                    //couleurs[j * nouvelleTexture.Width + i] = Color.Purple;//TEST
        //                }
        //            }
        //        }
        //    }
        //    nouvelleTexture.SetData<Color>(0, writer, couleurs, 0, couleurs.Length);

        //    Angle = angleParent;
        //    PlacerSommets();
        //    UpdateDroites();

        //    return nouvelleTexture;
        //}
        //private Texture2D AwesomeBatyTexture1(Texture2D imageParent, float angleParent, Vector2 positionParent, Vector2 scaleParent, out int nbPixelCouleurs)
        //{
        //    nbPixelCouleurs = 0;
        //    Texture2D nouvelleTexture = new Texture2D(MoteurJeu.GraphicDevice, (int)MathHelper.Clamp(imageParent.Width * scaleParent.X, 1, imageParent.Width * scaleParent.X),
        //                                                                    (int)MathHelper.Clamp(imageParent.Height * scaleParent.Y, 1, imageParent.Height * scaleParent.Y),
        //                                                                    false, SurfaceFormat.Color);

        //    Angle = 0;
        //    PlacerSommets();
        //    UpdateDroites();

        //    Rectangle lecteur = new Rectangle(0, 0, 1, 1);
        //    Rectangle writer = new Rectangle(0, 0, nouvelleTexture.Width, nouvelleTexture.Height);
        //    Color[] couleurs = new Color[nouvelleTexture.Height * nouvelleTexture.Width];
        //    Color[] origine = new Color[1];

        //    Vector2 delta = positionParent - Position;
        //    delta = Vector2.Transform(delta,Matrix.CreateRotationZ(-angleParent));

        //    int deltaX = (int)(delta.X);
        //    int deltaY = (int)(delta.Y);

        //    for (int j = 0; j < nouvelleTexture.Height; ++j)
        //    {
        //        for (int i = 0; i < nouvelleTexture.Width; ++i)
        //        {
        //            if (EstÀIntérieur(new Vector2(i + Position.X - imageParent.Width / 2 * scaleParent.X,
        //                                          j + Position.Y - imageParent.Height / 2 * scaleParent.Y)))
        //            {
        //                lecteur.X = (int)MathHelper.Clamp((i - deltaX) / scaleParent.X, 0, imageParent.Width - 1);
        //                lecteur.Y = (int)MathHelper.Clamp((j - deltaY) / scaleParent.Y, 0, imageParent.Height - 1);
        //                imageParent.GetData<Color>(0, lecteur, origine, 0, 1);
        //                couleurs[j * nouvelleTexture.Width + i] = origine[0];
        //                nbPixelCouleurs++;
        //            }
        //            else
        //            {
        //                couleurs[j * nouvelleTexture.Width + i] = Color.Transparent;
        //                if (i == 0 || j == 0)
        //                {
        //                    //couleurs[j * nouvelleTexture.Width + i] = Color.Purple;//TEST
        //                }
        //            }
        //        }
        //    }
        //    nouvelleTexture.SetData<Color>(0, writer, couleurs, 0, couleurs.Length);

        //    Angle = angleParent;
        //    PlacerSommets();
        //    UpdateDroites();

        //    return nouvelleTexture;
        //}
        
        public virtual bool EstÀIntérieur(Vector2 position)
        {
            bool résultat = true;
            Vector2 v1 = Vector2.Zero;
            Vector2 v2 = Vector2.Zero;
            float z = 0;
            for (int i = 0; i < NbSommets && résultat; ++i)
            {
                v1 = ListeDroites[i].VecteurPrincipal;
                v2 = position - ListeDroites[i].P1;
                z = Maths.ProduitVectoriel(ref v1, ref v2);
                résultat = résultat && z <= 0;
            }
            return résultat;
        }
        public bool IntersectionPersonnage(Personnage p)
        {
            if (Maths.IntersectionRayons(p, this))
            {
                foreach (Bone b in p.ListeBones)
                {
                    if (!b.EstBrisé && Maths.IntersectionRayons(b, this))
                    {
                        foreach (Sommet s in b.ListeSommets)
                        {
                            if (EstÀIntérieur(s.Position))
                            {
                                return true;
                            }
                        }
                        foreach (Sommet s in ListeSommets)
                        {
                            if (b.EstÀIntérieur(s.Position))
                            {
                                return true;
                            }
                        }                        
                    }
                }
            }
            return false;
        }
        public bool IntersectionPersonnage(Personnage p, out Bone boneTouché)
        {
            boneTouché = null;
            if (Maths.IntersectionRayons(p, this))
            {
                foreach (Bone b in p.ListeBones)
                {
                    if (!b.EstBrisé && Maths.IntersectionRayons(b, this))
                    {
                        foreach (Sommet s in b.ListeSommets)
                        {
                            if (EstÀIntérieur(s.Position))
                            {
                                boneTouché = b;
                                return true;
                            }
                        }
                        foreach (Sommet s in ListeSommets)
                        {
                            if (b.EstÀIntérieur(s.Position))
                            {
                                boneTouché = b;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public virtual IntersectionAvecNormale VérifierCollisionDroite(Droite droite)
        {
            return Intersection.TrouverPremièreIntersectionAvecNormale(droite, this);
        }

        public virtual Contact VérifierCollisionsPersonnages()
        {
            if (NbCollisions < NbCollisionsMax)
            {
                Contact contact = null;
                foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
                {
                    if (Maths.IntersectionRayons(p, this) && ((DernierPropriétaire != null && DernierPropriétaire != p) || DernierPropriétaire == null))
                    {
                        foreach (Bone b in p.ListeBones)
                        {
                            if (!b.EstBrisé && Maths.IntersectionRayons(b, this))
                            {
                                foreach (Sommet s in b.ListeSommets)
                                {
                                    if (EstÀIntérieur(s.Position))
                                    {
                                        contact = CréerContact(b, this, s, true);
                                        if (contact != null)
                                        {
                                            GestionContacts.Ajouter(contact);

                                            return contact;
                                        }
                                    }
                                }

                                foreach (Sommet s in ListeSommets)
                                {
                                    if (b.EstÀIntérieur(s.Position))
                                    {
                                        contact = CréerContact(this, b, s, false);
                                        if (contact != null)
                                        {
                                            GestionContacts.Ajouter(contact);

                                            return contact;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
            return null;
        }
        public virtual List<Contact> VérifierCollisionsDécorsV2()
        {
            bool croisementTrouvé = false;
            //index du polygone dans la grille
            Point index = Bloc.CoordVersIndex(Position);

            int i = index.X;// (int)((Position.X + Bloc.DIMENSION_BLOC / 2) / Bloc.DIMENSION_BLOC);
            int j = index.Y;// (int)((Position.Y - Bloc.DIMENSION_BLOC / 2) / -Bloc.DIMENSION_BLOC);

            //bornes inférieures
            int min_i = i - ((int)Dimensions.X >> 6) - 1;
            int min_j = j - ((int)Dimensions.Y >> 6) - 1;

            //bornes supérieures
            int max_i = i + ((int)Dimensions.X >> 6) + 1;
            int max_j = j + ((int)Dimensions.Y >> 6) + 1;

            List<Contact> tableauContacts = new List<Contact>();
            Contact contact = null;

            for (int a = min_i; a <= max_i; ++a)
            {
                for (int b = min_j; b <= max_j; ++b)
                {
                    Bloc bloc = GestionNiveaux.NiveauActif.GetBloc(a, b);
                    if (bloc == null || bloc.EstTunel) { continue; }

                    //on vérifie si un sommet d'un bloc de grille entre dans notre bloc
                    foreach (Sommet s in bloc.ListeSommets)
                    {
                        if (EstÀIntérieur(s.Position))
                        {
                            contact = CréerContact(bloc, this, s, true);
                            if (contact != null)
                            {
                                tableauContacts.Add(contact);
                                GestionContacts.Ajouter(contact);

                                croisementTrouvé = true;
                            }
                            break;
                        }
                    }
                    if (!croisementTrouvé)
                    {
                        //on vérifie si un de nos sommet entre dans un bloc de la grille
                        foreach (Sommet s in ListeSommets)
                        {
                            if (bloc.EstÀIntérieur(s.Position))
                            {
                                contact = CréerContact(this, bloc, s, false);
                                if (contact != null)
                                {
                                    tableauContacts.Add(contact);
                                    GestionContacts.Ajouter(contact);

                                    croisementTrouvé = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return tableauContacts;
        }
        public virtual List<Contact> VérifierCollisionsDécors()
        {
            List<Contact> tableauContacts = new List<Contact>();
            Contact contact = null;

            bool croisementTrouvé = false;
            Bloc bloc = null;

            //on vérifie si un de nos sommet entre dans un bloc de la grille
            foreach (Sommet s in ListeSommets)
            {
                bloc = GestionNiveaux.NiveauActif.GetBloc(ref s.Position);
                if (bloc == null || bloc.EstTunel) { continue; }

                if (bloc.EstÀIntérieur(s.Position))
                {
                    contact = CréerContact(this, bloc, s, false);
                    if (contact != null)
                    {
                        tableauContacts.Add(contact);
                        GestionContacts.Ajouter(contact);

                        croisementTrouvé = true;
                    }
                    break;
                }
            }
            if (!croisementTrouvé)
            {
                //index du polygone dans la grille
                Point index = Bloc.CoordVersIndex(Position);

                int i = index.X;// (int)((Position.X + Bloc.DIMENSION_BLOC / 2) / Bloc.DIMENSION_BLOC);
                int j = index.Y;// (int)((Position.Y - Bloc.DIMENSION_BLOC / 2) / -Bloc.DIMENSION_BLOC);

                //bornes inférieures
                int min_i = i - ((int)Rayon >> 6) - 1;
                int min_j = j - ((int)Rayon >> 6) - 1;

                //bornes supérieures
                int max_i = i + ((int)Rayon >> 6) + 1;
                int max_j = j + ((int)Rayon >> 6) + 1;

                for (int a = min_i; a <= max_i; ++a)
                {
                    for (int b = min_j; b <= max_j; ++b)
                    {
                        bloc = GestionNiveaux.NiveauActif.GetBloc(a, b);
                        if (bloc == null || bloc.EstTunel) { continue; }

                        //on vérifie si un sommet d'un bloc de grille entre dans notre bloc
                        foreach (Sommet s in bloc.ListeSommets)
                        {
                            if (EstÀIntérieur(s.Position))
                            {
                                contact = CréerContact(bloc, this, s, true);
                                if (contact != null)
                                {
                                    tableauContacts.Add(contact);
                                    GestionContacts.Ajouter(contact);

                                    croisementTrouvé = true;

                                }
                                break;
                            }
                        }
                    }
                }
            }
            return tableauContacts;
        }
        protected static Contact CréerContact(PolygonePhysique polyQuiALeSommet, PolygonePhysique autrePoly, Sommet sommet, bool sommetDeLAutre)
        {
            
            Vector2 positionContact = sommet.Position;//Plus valide que 0,0
            Vector2 positionContactMin = positionContact;
            Droite segmentCentre_Sommet = new Droite(polyQuiALeSommet.Position - (sommet.Position - polyQuiALeSommet.Position), sommet.Position);

            bool trouvé = false;

            float pénétration;
            float pénétrationMin=1000;//Valeur bidon et trop grosse pour qu'elle soit réel

            float distanceCentreIntersection;
            float distanceCentreIntersectionMin = int.MaxValue;

            Droite côté;
            Droite côtéMin = new Droite(Vector2.Zero, Vector2.Zero); //Valeur bidon

            for (int i = 0; i < autrePoly.NbSommets && !trouvé; ++i)
            {
                côté = autrePoly.ListeDroites[i];
                Vector2 intersection = Vector2.Zero;

                //côté.CalculerPente_mx_b();
                côté.VecteurNormal.Normalize();

                if (segmentCentre_Sommet.VérifierCollisions(côté, out intersection))
                {
                    trouvé = true;
                    Vector2 segmentIntersection_Sommet = sommet.Position - intersection;
                    positionContact = intersection + Maths.ProjectionOrthogonale(segmentIntersection_Sommet, côté.VecteurPrincipal);

                    pénétration = Vector2.Distance(positionContact, sommet.Position);

                    if (sommetDeLAutre)
                    {
                        distanceCentreIntersection = Vector2.DistanceSquared(autrePoly.Position, sommet.Position);
                    }
                    else
                    {
                        distanceCentreIntersection = Vector2.DistanceSquared(polyQuiALeSommet.Position, sommet.Position);
                    }


                    if (distanceCentreIntersection < distanceCentreIntersectionMin)
                    {
                        distanceCentreIntersectionMin = distanceCentreIntersection;
                        pénétrationMin = pénétration;
                        côtéMin = côté;
                        positionContactMin = positionContact;
                    }
                }
                
            }
            if (trouvé)
            {
                if (sommetDeLAutre)
                {
                    return new Contact(autrePoly, polyQuiALeSommet, positionContactMin, côtéMin.VecteurNormal,côtéMin.VecteurPrincipal, pénétrationMin);
                }
                else
                {
                    return new Contact(polyQuiALeSommet, autrePoly, positionContactMin, -côtéMin.VecteurNormal,côtéMin.VecteurPrincipal, pénétrationMin);
                }
            }
            return null;
        }

        public void VérifierCollisionsPolygones()
        {
            Contact contact = null;
            bool collision = false;
            foreach (PolygonePhysique p in GestionNiveaux.NiveauActif.ListePolygonesPhysiques)
            {
                if (p != this && Maths.IntersectionRayons(p, this))
                {
                    foreach (Sommet s in p.ListeSommets)
                    {
                        if (EstÀIntérieur(s.Position))
                        {
                            contact = CréerContact(p, this, s, true);
                            if (contact != null)
                            {
                                GestionContacts.Ajouter(contact);
                                collision = true;
                            }
                        }
                    }
                    if (!collision)
                    {
                        foreach (Sommet s in ListeSommets)
                        {
                            if (p.EstÀIntérieur(s.Position))
                            {
                                contact = CréerContact(this, p, s, false);
                                if (contact != null)
                                {
                                    GestionContacts.Ajouter(contact);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void Orienter()
        {
            PlacerSommets();
            UpdateDroites();
        }
        public void Orienter(ref Matrix matrice, float angle)
        {
            PlacerSommets(ref matrice, angle);
            UpdateDroites();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (PossèdeOmbre)
            {
                Ombre.Angle = Angle;
                Ombre.Position = Position;
            }

            if (Disparaitra)
            {
                spriteBatch.Draw(Image, Position, null, Couleur * Alpha, Angle, OrigineSprite, Scale, SpriteEffect, Profondeur);
            }
            else
            {
                base.Draw(spriteBatch);
            }

            foreach (Droite s in ListeDroites)
            {
                s.DrawDroite(spriteBatch);
            }
            foreach (Sommet s in ListeSommets)
            {
                s.Draw(spriteBatch);
            }
        }
    }
}
