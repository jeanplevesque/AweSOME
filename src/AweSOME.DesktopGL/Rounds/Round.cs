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
    class Round
    {
        public const int TEMPS_MIN_ENTRE_GÉNÉRATIONS = 60 * 5;
        public const int HEURE_MORT_ZOMBIE = 7;

        public DateTime HeureDébut { get; private set; }
        public DateTime HeureFin { get; private set; }
        public TimeSpan DuréeDuRound { get; private set; }

        public bool PrêtPourProchainRound { get; private set; }
        public bool EstTerminé { get; private set; }
        public bool GénérationZombieTerminé { get; private set; }
        //public bool EstEnCours { get; private set; }
        public bool ModeNuit{get;private set;}
        public int Numéro { get; private set; }
        public int NbZombiesÀFaire { get; private set; }
        public int NbZombiesCrées { get; private set; }
        public StatistiquesPersonnage StatsDébut { get; private set; }
        public StatistiquesPersonnage StatsDuRound { get; private set; }

        public int NbZombiesEnVies { get; set; }

        public Niveau Monde;

        private int TempsAvantNextGénération;

        public Round(int numéro)
        {
            Monde = GestionNiveaux.NiveauActif;
            Numéro = numéro;
            CalculerNbZombiesÀFaire();

            MoteurJeu.AwesomeBox.WriteLine("Round  "+ Numéro.ToString() + " : " + NbZombiesÀFaire.ToString());
        }

        public void Update()
        {

            if (ModeNuit)
            {
                CréerZombies();
                VérifierSiFin();
            }
            else
            {
                if (!PrêtPourProchainRound && Monde.ÉtatJournée == ÉtatsJournée.Day)
                {
                    if (GénérationZombieTerminé)
                    {
                        PrêtPourProchainRound = true;
                    }                  
                }
                if (Monde.ÉtatJournée == ÉtatsJournée.Dusk && Monde.MomentPrésent >= Niveau.HeureDébutRound)
                {
                    if (PrêtPourProchainRound)
                    {
                        GestionRounds.PasserAuProchainRound();
                    }
                    else if(!GénérationZombieTerminé)
                    {
                        ModeNuit = true;
                    }
                }
            }

        }

        private void VérifierSiFin()
        {
            if (GénérationZombieTerminé && NbZombiesEnVies == 0)
            {
                Terminer();
            }
            if (GestionNiveaux.NiveauActif.MomentPrésent.Hours >= HEURE_MORT_ZOMBIE && GestionNiveaux.NiveauActif.ÉtatJournée == ÉtatsJournée.Dawn)
            {
                TuerZombiesRestant();

                PrêtPourProchainRound = true;
                

                Terminer();
            }
        }

        private void TuerZombiesRestant()
        {
            foreach (Personnage p in GestionNiveaux.NiveauActif.ListePersonnages)
            {
                if (p is Zombie)
                {
                    Explosion e = new Explosion(p.Position, 200, 40000, 100, 11, null, true);
                    e.Affecter();

                    p.VérifierÉtatSanté();
                    if (p.EstEnVie)
                    {
                        p.TuerEtDétruire();
                    }
                }
            }
        }

        //public void Pause()
        //{
        //    EstEnCours = false;
        //}
        //public void UnPause()
        //{
        //    EstEnCours = true;
        //}
        
        
        public void Start()
        {
            HeureDébut = DateTime.Now;
            StatsDébut = GestionNiveaux.NiveauActif.Joueur.Stats.Clone();


            AfficherMessage("Round " + Numéro.ToString());
        }
        
        public void Stop()
        {
            HeureFin = DateTime.Now;
            ModeNuit = false;

            //CalculerTemps et autres statistiques
            DuréeDuRound = (HeureFin - HeureDébut);
            StatsDuRound = GestionNiveaux.NiveauActif.Joueur.Stats - StatsDébut;
        }
        public void Terminer()
        {
            Stop();
            NbZombiesEnVies = 0;
            

            AfficherStatistiqueFinRound();
        }

        private void CalculerNbZombiesÀFaire()
        {
            NbZombiesÀFaire = (int)(Numéro * 2 + Math.Pow(Numéro,2) + 2);
            NbZombiesCrées = 0;
        }

        private void CréerZombies()
        {
            if (NbZombiesEnVies < 10)
            {
                if (!GénérationZombieTerminé && NbZombiesCrées < NbZombiesÀFaire)
                {
                    --TempsAvantNextGénération;
                    if (TempsAvantNextGénération <= 0)
                    {
                        TempsAvantNextGénération = TEMPS_MIN_ENTRE_GÉNÉRATIONS;

                        GénérerVague();

                        AjouterObjets();
                    }
                }
                else if (!GénérationZombieTerminé)
                {
                    GénérationZombieTerminé = true;
                    //AfficherMessage("End of Generation");
                }
            }
        }

        private void AjouterObjets()
        {
            if (!GestionNiveaux.NiveauActif.Joueur.Inventaire.EstPlein)
            {
                if (Maths.UneChanceSur(2))
                {
                    GestionNiveaux.AjouterPolygonePhysique(new Caisse(GestionSpawn.GetRandomSpawnPosition(), TypesCaisse.TotalRandom));
                }
            }
        }

        private void GénérerVague()
        {
            float scale;
            for (int i = 0; i < 5 && NbZombiesCrées < NbZombiesÀFaire; ++i)
            {
                scale=Maths.RandomFloat(0.95f, 1.05f);
                Zombie zombie = new Zombie(GestionSpawn.GetRandomSpawnLoinDe(GestionNiveaux.NiveauActif.Joueur.TuileActive), Bloc.DIMENSION_BLOC * scale);
                zombie.ChangerVieMax((int)(zombie.VieMax * scale));
                GestionNiveaux.NiveauActif.ListePersonnages.Add(zombie);
                ++NbZombiesCrées;
                ++NbZombiesEnVies;
            }
        }

        private void AfficherMessage(string message)
        {
            EffetTexteFlotant texte = new EffetTexteFlotant(Caméra.DimensionsFenêtreSurDeux-Vector2.UnitY*200, Bouton.DIMENSIONS_BOUTONS);

            texte.Font = BanqueContent.GetFont("Copperplate");
            texte.Write(message);
            texte.TexteCentréX = true;
            texte.EffetSurTexte.TypeEffet = TypesEffetsTexte.FadeCouleur;
            texte.EffetSurTexte.Couleur1 = new Color(187, 21, 21);
            texte.EffetSurTexte.VitesseProgression = 0.004f;
            GestionEffets.AjouterEffetHUD(texte);
        }
        private void AfficherStatistiqueFinRound()
        {
            EffetTexteFlotant texte = new EffetTexteFlotant(Caméra.DimensionsFenêtreSurDeux, new Vector2(Bouton.DIMENSIONS_BOUTONS.X * 1.4f, Caméra.DimensionsFenêtre.Y), 600, 120);

            texte.Font = BanqueContent.GetFont("Copperplate");
            texte.WriteLine("End of Round : " + Numéro);
            texte.WriteLine(" ");
            texte.WriteLine("Total Time : " + (int)DuréeDuRound.TotalMinutes + "m " + DuréeDuRound.Seconds + "s ");
            texte.WriteLine("Zombies  Generated   : " + NbZombiesCrées);
            texte.WriteLine("Zombies you   Killed   : " + StatsDuRound.NbKills);
            texte.WriteLine("Zombies you headshoted : " + StatsDuRound.NbHeadShots);

            //texte.TexteCentréX = true;
            //texte.Update();
            GestionEffets.AjouterEffetHUD(texte);
        }

        
    }
}
