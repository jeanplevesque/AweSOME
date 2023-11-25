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

namespace AweSOME
{
    class Éclaire : SegmentÉclaire,IEffet
    {
        public List<SegmentÉclaire> ListeSegments = new List<SegmentÉclaire>();
        public List<SegmentÉclaire> ListeSegmentsCollisions = new List<SegmentÉclaire>();
        //public List<Sommet> ListeSommets = new List<Sommet>();

        Matrix Matrice = Matrix.Identity;

        public int TempsRestant = 31;
        public int TempsFin = 30;

        public float OffsetOrigine = 0.25f; //entre 0.125f et 0.25f = ze best
        public float Offset;

        public bool PossèdeDivisions = true;

        protected int NbGénérations;

        public bool DoitVérifierCollisions;
        public bool DoitVérifierCollisionsDécors;

        public Éclaire(Vector2 posDébut, Vector2 directionUnitaire, float longueur, int nbGénérations, Color couleurGlow,
            float offsetOrigine = 0.25f,
            bool CréerMaintenant = true,
            bool doitVérifierCollisions = true,
            bool doitVérifierCollisionsDécors = true)
            : base(posDébut, posDébut + directionUnitaire * longueur, ref couleurGlow)
        {
            OffsetOrigine = offsetOrigine;
            NbGénérations = nbGénérations;
            DoitVérifierCollisions = doitVérifierCollisions;
            DoitVérifierCollisionsDécors = doitVérifierCollisionsDécors;
            
            if (CréerMaintenant)
            {
                Créer();
            }
            //Générer(nbGénérations);
            //CréerLumièreTempo();
        }
        public Éclaire(Vector2 posDébut, Vector2 posFin, int nbGénérations, Color couleurGlow, 
            float offsetOrigine = 0.25f, 
            bool CréerMaintenant = true, 
            bool doitVérifierCollisions = true,
            bool doitVérifierCollisionsDécors = true)
            :base(posDébut,posFin, ref couleurGlow)
        {
            OffsetOrigine = offsetOrigine;
            NbGénérations = nbGénérations;
            DoitVérifierCollisions = doitVérifierCollisions;
            DoitVérifierCollisionsDécors = doitVérifierCollisionsDécors;

            if (CréerMaintenant)
            {
                Créer();
            }
            //Générer(nbGénérations);
            //CréerLumièreTempo();
        }
        public void Créer()
        {
            Générer();
            CréerLumièreTempo();
        }

        protected void Générer()
        {
            ListeSegments.Clear();

            int nbSegmentsAjoutés = 0;
            int nbSegmentsÀEnlever = 1;

            ListeSegments.Add(this);

            Offset = OffsetOrigine;
            Vector2 nouveauPoint;

            int nbDroites = 0;
            SegmentÉclaire droite = null;
            float fraction;

            for (int i = 1; i <= NbGénérations; ++i)
            {
                nbDroites = ListeSegments.Count;
                for (int d = 0; d < nbDroites; ++d)//(Droite d in ListeSegments)
                {
                    droite = ListeSegments[d];
                    fraction = Maths.RandomFloat(0.40f, 0.61f);
                    nouveauPoint = droite.CalculerPointFraction(fraction);
                    nouveauPoint += droite.VecteurNormal * Offset * Maths.RandomMultiplicateur();

                    ListeSegments.Add(new SegmentÉclaire(droite.P1, nouveauPoint, droite.AlphaOrigine, droite.Index, droite.IndexDivision, ref Brillance.Couleur));
                    ListeSegments.Add(new SegmentÉclaire(nouveauPoint, droite.P2, droite.AlphaOrigine, droite.Index + Maths.PuissanceDeDeux(NbGénérations - i), droite.IndexDivision, ref Brillance.Couleur));
                    nbSegmentsAjoutés += 2;

                    //Ajouter les divisions
                    if (PossèdeDivisions && Maths.Random.Next(Maths.Clamp(nbDroites - d, 16, 4)) == 0)
                    {
                        ListeSegments.Add(new SegmentÉclaire(nouveauPoint, nouveauPoint + Vector2.Transform(droite.VecteurPrincipal * 0.5f,
                                                             Matrix.CreateRotationZ(MathHelper.ToRadians(Maths.Random.Next(140) - 70) * Offset)),
                                                             droite.AlphaOrigine * 0.7f, droite.Index + Maths.PuissanceDeDeux(NbGénérations - i), droite.IndexDivision + 1, ref Brillance.Couleur));
                        nbSegmentsAjoutés += 1;
                    }
                }

                ListeSegments.RemoveRange(0, nbSegmentsÀEnlever);
                nbSegmentsÀEnlever = nbSegmentsAjoutés;
                nbSegmentsAjoutés = 0;


                Offset *= 0.95f;
            }
            if (DoitVérifierCollisions)
            {
                VérifierLesCollisions();
            }
            OrienterSegments();

            //MoteurJeu.AwesomeBox.WriteLine("Éclaire : " + ListeSegments.Count.ToString());
        }

        protected void CréerLumièreTempo()
        {
            //Vector2 pos = CalculerPointMilieu();
            Color couleur = Color.Black ;
            //Maths.MixerCouleurs(ref Brillance.Couleur, ref Couleur, Maths.RandomFloat(0.25f, 0.75f), out couleur);
            LumièreTemporaireIntensityFade lumiere = new LumièreTemporaireIntensityFade(TempsRestant / 2, (int)Longueur * 4, ref Brillance.Couleur,ref couleur, ref Position,0.75f);
            GestionEffets.AjouterLumiereTempo(lumiere);
        }
        private void OrienterSegments()
        {
            Droite segment;
            for (int i = 0; i < ListeSegments.Count;++i )
            {
                segment = ListeSegments[i];
                segment.CalculerAngle();
            }

        }

        public virtual void VérifierLesCollisions()
        {
            if (DoitVérifierCollisionsDécors)
            {
                ListeSegmentsCollisions.AddRange(VérifierCollisionsDécors());
            }
            SegmentÉclaire segment;
            foreach (SegmentÉclaire s in ListeSegmentsCollisions)
            {
                if (ListeSegments.Contains(s))
                {
                    GestionEffets.CréerMagie(s.CalculerPointMilieu(), -s.VecteurPrincipal, 8, 180, 16, 24, ref s.Brillance.Couleur, 60, 50);
                    GestionEffets.CréerÉtincellesDoubleTexture(s.CalculerPointMilieu(), -s.VecteurPrincipal, 8, 60, 24, 48, ref s.Brillance.Couleur, 60);

                    segment = s;
                    for (int i = ListeSegments.IndexOf(s); i != -1 && segment.Index >= s.Index && i < ListeSegments.Count; i += 0)
                    {
                        segment = ListeSegments[i];
                        ListeSegments.RemoveAt(i);
                    }
                }
            }

        }
        protected List<SegmentÉclaire> VérifierCollisionsDécors()
        {
            List<SegmentÉclaire> listeSegments = new List<SegmentÉclaire>();

            foreach (SegmentÉclaire s in ListeSegments)
            {
                if (s.VérifierCollisionsBlocs())
                {
                    listeSegments.Add(s);
                }
            }
            return listeSegments;
        }
        
        public override void Update()
        {
            //Droite segment;
            //Vector2 delta;
            //Vector2 deltaOrigine = Vector2.UnitX * 0.25f;
            //for (int i = 0; i < ListeSegments.Count; ++i)
            //{
            //    segment = ListeSegments[i];

            //    Vector2.Transform(ref deltaOrigine, ref Matrice, out delta);
            //    segment.Sommet2.Position += delta;
            //    ListeSegments[Maths.Clamp(i + 1, ListeSegments.Count - 1, 0)].Sommet1.Position += delta;

            //    if (Maths.Random.Next(4) == 0)
            //    {
            //        Matrix.CreateRotationZ(MathHelper.ToRadians(Maths.Random.Next(360)), out Matrice);
            //    }
            //}

            
            //base.Update();

            if (TempsRestant < TempsFin)
            {
                Alpha = (float)TempsRestant / (float)TempsFin;
                if (TempsRestant <= 0)
                {
                    Détruire();
                }
            }
            --TempsRestant;

            foreach (SegmentÉclaire d in ListeSegments)
            {
                //d.Update();
                //d.CalculerAngle();
                //d.CalculerLongueur();
                //d.ArrangerSprite();
                d.UpdateAlpha(Alpha);
            }

            
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //base.Draw(spriteBatch);

            foreach (Droite d in ListeSegments)
            {
                //d.Couleur = Color.White;
                d.Draw(spriteBatch);
            }
            //foreach (Sommet d in ListeSommets)
            //{
            //    d.Couleur = Color.Red;
            //    d.Draw(MoteurJeu.SpriteBatchScène);
            //}
        }

        public virtual void Détruire()
        {
            GestionEffets.DétruireEffet(this);
        }

    }
}
