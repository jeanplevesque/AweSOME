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

namespace AweSOME
{
    enum TypeNiveau { Normal, Infinite }
    class Partie:Écran
    {
        

        public Partie(Écran écranPrécédent,TypeNiveau typeNiveau)
            :base(écranPrécédent, "Current Game")
        {
            switch (typeNiveau)
            {
                case TypeNiveau.Infinite:
                    GestionNiveaux.GénérerInfinite(5, 5);
                    break;
                default:
                    GestionNiveaux.Générer(75, 50);
                    break;
            }
            
            GestionRounds.Reset();
            GestionRounds.PasserAuProchainRound();
            GestionNiveaux.NiveauActif.AvancerTempsJusqua(TimeSpan.FromHours(19));
        }

        public override void Update()
        {
            Caméra.Update();

            #region TEST
            //-------------

            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.Enter)) { GestionNiveaux.GénérerInfinite(5, 5); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.NumPad0)) { GestionNiveaux.GénérerInfinite(20, 10); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.Back)) { GestionNiveaux.Générer(50, 50); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.I)) { GestionRounds.Reset(); GestionRounds.PasserAuProchainRound(); }
            //if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.C))
            //{
            //    Fusil gun = GestionArmes.GénérerFusilTexte("Chat");
            //    gun.Position = GestionIntrants.PositionSourisCaméra;
            //    gun.SeFaireLaisser();
            //}
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.K))
            {
                Flare arme = new Flare(GestionIntrants.PositionSourisCaméra);
                arme.SeFaireLaisser();
            }
            //if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.V))
            //{

            //    ArmeMelee arme = ArmeMelee.CréerBâton(GestionIntrants.PositionSourisCaméra, null);

            //    arme.SeFaireLaisser();
            //}
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.P))
            {

                ArmeLancée arme = GestionArmes.GénérerArmeLancéeTexte("Pear", 1);
                arme.Position = GestionIntrants.PositionSourisCaméra;
                arme.SeFaireLaisser();
            }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.M))
            {
                GestionNiveaux.NiveauActif.AjouterPolygonePhysique(new Caisse(GestionIntrants.PositionSourisCaméra, TypesCaisse.TypeRandom));
            }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.B))
            {
                Explosion explosion = new Explosion(GestionIntrants.PositionSourisCaméra, 150, 150 * 150, 150, 10, null, true);
                explosion.Affecter();
            }
            //if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.Z)) { GestionNiveaux.NiveauActif.AjouterBloc(GestionIntrants.PositionSourisCaméra, MatérielPolygone.Métal); }
            //if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.X)) { GestionNiveaux.NiveauActif.DétruireBloc(GestionIntrants.PositionSourisCaméra); }
            if (GestionIntrants.EstToucheEnfoncée(Keys.T)) { GestionNiveaux.NiveauActif.CréerTunel(GestionIntrants.PositionSourisCaméra); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.G)) { GestionNiveaux.NiveauActif.ListePersonnages.Add(new Zombie(GestionSpawn.GetRandomSpawnLoinDe(GestionNiveaux.NiveauActif.Joueur.TuileActive), Bloc.DIMENSION_BLOC)); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.J)) { GestionNiveaux.NiveauActif.AvancerTempsJusqua(TimeSpan.FromHours(6.9)); }
            if (GestionIntrants.EstNouvelleToucheEnfoncée(Keys.N)) { GestionNiveaux.NiveauActif.AvancerTempsJusqua(TimeSpan.FromHours(19)); }
            //-------------
            #endregion


            GestionEffets.Update();

            GestionContacts.RésoudreContacts();
            GestionImpactsBalle.Résoudre();
            GestionNiveaux.Update();
            GestionRounds.Update();
            IA.ResetPathFinding();

        }

        public override void Draw()
        {
            MoteurJeu.This.DrawPartie();
        }
    }
}
