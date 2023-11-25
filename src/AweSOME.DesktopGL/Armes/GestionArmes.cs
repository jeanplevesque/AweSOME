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
    //public enum CouleursBalles{ Jaune, Blanc ,Rouge, Random}
    public enum VariétésFusils { Vanille, Shotgun, Blocgun }
    static class GestionArmes
    {
        public static Dictionary<string, InfoFusil> InfosFusils = new Dictionary<string, InfoFusil>();
        public static List<string> NomsFusils = new List<string>();

        public static List<string> NomsArmesLancées = new List<string>();

        static GestionArmes()
        {
            InfosFusils.Add("Chat", new InfoFusil("Chat", 0.75f, VariétésFusils.Vanille));
            InfosFusils.Add("Plastic M4", new InfoFusil("Plastic M4", 0.2f, VariétésFusils.Vanille));
            InfosFusils.Add("GoldM1014", new InfoFusil("GoldM1014", 0.4f, VariétésFusils.Shotgun));
            InfosFusils.Add("Blok", new InfoFusil("Blok", 1f, VariétésFusils.Blocgun));

            NomsArmesLancées.Add("Pomegranate");
            NomsArmesLancées.Add("Orange");
            NomsArmesLancées.Add("Pear");
        }
        
        public static ArmeLancée GénérerArmeLancéeTexte(string nom, float grosseur)
        {
            StreamReader reader = new StreamReader("Data/ArmesLancées/" + nom + ".al");
            ArmeLancée arme = LoadArmeLancéeTexte(reader, grosseur);
            reader.Close();
            return arme;
        }
        private static ArmeLancée LoadArmeLancéeTexte(StreamReader reader, float grosseur)
        {
            ArmeLancée arme = new ArmeLancée()
            {
                Nom = reader.ReadLine(),
                NomTexture = reader.ReadLine(),

                Type = (TypeArmesLancées)int.Parse(reader.ReadLine()),
                RayonDAction = int.Parse(reader.ReadLine()),
                Dégats = int.Parse(reader.ReadLine()),
                TicsRestants = int.Parse(reader.ReadLine()),

                Puissance = int.Parse(reader.ReadLine()),
                Dimensions = BanqueContent.LoadVector2(reader) * grosseur,
            };
            arme.ArrangerSprite(GestionTexture.GetTexture(arme.NomTexture));
            arme.Reconstruire(Vector2.Zero, 8);

            return arme;
        } 
        
        
        public static Shotgun GénérerShotgunTexte(string nom)
        {
            return GénérerShotgunTexte(nom, InfosFusils[nom].Grosseur);
        }
        public static Shotgun GénérerShotgunTexte(string nom, float grosseur)
        {
            StreamReader reader = new StreamReader("Data/Fusils/" + nom + ".Gun");
            Shotgun fusil = LoadShotGunTexte(reader, grosseur);
            reader.Close();
            return fusil;
        }
        private static Shotgun LoadShotGunTexte(StreamReader reader, float grosseur)
        {
            Shotgun fusil = new Shotgun()
            {
                Nom = reader.ReadLine(),
                NomTexture = reader.ReadLine(),

                TempsEntreTirs = int.Parse(reader.ReadLine()),
                Dégats = int.Parse(reader.ReadLine()),
                NbBallesTotal = int.Parse(reader.ReadLine()),
                CapacitéChargeur = int.Parse(reader.ReadLine()),
                Type = (TypeFusil)int.Parse(reader.ReadLine()),
                LongueurBalles = int.Parse(reader.ReadLine()),
                AngleImprécisionMaxDegrées = int.Parse(reader.ReadLine()),
                Puissance = int.Parse(reader.ReadLine()),
                CouleurBalles = new Color(int.Parse(reader.ReadLine()),
                                          int.Parse(reader.ReadLine()),
                                          int.Parse(reader.ReadLine())),

                DistanceEmboutInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                DistanceLazerInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                DistanceFlashLightInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                Dimensions = BanqueContent.LoadVector2(reader) * grosseur,

                AngleOuvertureBallesDegrées = int.Parse(reader.ReadLine()),
                NbBallesDansCartouche = int.Parse(reader.ReadLine()),
            };
            fusil.ArrangerSprite(GestionTexture.GetTexture(fusil.NomTexture));
            fusil.NbBallesDansChargeur = fusil.CapacitéChargeur;
            fusil.Reconstruire(Vector2.Zero, fusil.Dimensions);

            fusil.DétruireOmbre();

            return fusil;
        }
        
        public static Fusil GénérerFusilTexte(string nom)
        {
            return GénérerFusilTexte(nom, InfosFusils[nom].Grosseur);
        }
        public static Fusil GénérerFusilTexte(string nom, float grosseur)
        {
            StreamReader reader = new StreamReader("Data/Fusils/" + nom + ".Gun");
            Fusil fusil = LoadFusilTexte(reader, grosseur);
            reader.Close();
            return fusil;
        }
        private static Fusil LoadFusilTexte(StreamReader reader, float grosseur)
        {
            Fusil fusil = new Fusil()
            {
                Nom = reader.ReadLine(),
                NomTexture = reader.ReadLine(),

                TempsEntreTirs = int.Parse(reader.ReadLine()),
                Dégats = int.Parse(reader.ReadLine()),
                NbBallesTotal = int.Parse(reader.ReadLine()),
                CapacitéChargeur = int.Parse(reader.ReadLine()),
                Type = (TypeFusil)int.Parse(reader.ReadLine()),               
                LongueurBalles = int.Parse(reader.ReadLine()),
                AngleImprécisionMaxDegrées = int.Parse(reader.ReadLine()),
                Puissance = int.Parse(reader.ReadLine()),
                CouleurBalles = new Color(int.Parse(reader.ReadLine()),
                                          int.Parse(reader.ReadLine()),
                                          int.Parse(reader.ReadLine())),

                DistanceEmboutInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                DistanceLazerInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                DistanceFlashLightInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                Dimensions = BanqueContent.LoadVector2(reader) * grosseur,
            };
            fusil.ArrangerSprite(GestionTexture.GetTexture(fusil.NomTexture));
            fusil.NbBallesDansChargeur = fusil.CapacitéChargeur;
            fusil.Reconstruire(Vector2.Zero, fusil.Dimensions);

            fusil.DétruireOmbre();

            return fusil;
        }

        public static BlocGun GénérerBlocGunTexte(string nom, float grosseur)
        {
            StreamReader reader = new StreamReader("Data/Fusils/" + nom + ".Gun");
            BlocGun fusil = LoadBlocGunTexte(reader, grosseur);
            reader.Close();
            return fusil;
        }
        private static BlocGun LoadBlocGunTexte(StreamReader reader, float grosseur)
        {
            BlocGun fusil = new BlocGun()
            {
                Nom = reader.ReadLine(),
                NomTexture = reader.ReadLine(),

                TempsEntreTirs = int.Parse(reader.ReadLine()),
                Dégats = int.Parse(reader.ReadLine()),
                NbBallesTotal = int.Parse(reader.ReadLine()),
                CapacitéChargeur = int.Parse(reader.ReadLine()),
                Type = (TypeFusil)int.Parse(reader.ReadLine()),
                LongueurBalles = int.Parse(reader.ReadLine()),
                AngleImprécisionMaxDegrées = int.Parse(reader.ReadLine()),
                Puissance = int.Parse(reader.ReadLine()),
                CouleurBalles = new Color(int.Parse(reader.ReadLine()),
                                          int.Parse(reader.ReadLine()),
                                          int.Parse(reader.ReadLine())),

                DistanceEmboutInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                DistanceLazerInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                DistanceFlashLightInitiale = BanqueContent.LoadVector2(reader) * grosseur,
                Dimensions = BanqueContent.LoadVector2(reader) * grosseur,
            };
            fusil.ArrangerSprite(GestionTexture.GetTexture(fusil.NomTexture));
            fusil.NbBallesDansChargeur = fusil.CapacitéChargeur;
            fusil.Reconstruire(Vector2.Zero, fusil.Dimensions);

            fusil.DétruireOmbre();

            return fusil;
        }

        public static Fusil GetRandomFusil()
        {
            return GénérerFusil(NomsFusils[Maths.Random.Next(NomsFusils.Count)]);
        }
        public static Fusil GénérerFusil(string nom)
        {
            Fusil fusil = null;
            InfoFusil infos=InfosFusils[nom];
            switch (infos.Variété)
            {
                case VariétésFusils.Blocgun:
                    fusil = GénérerBlocGunTexte(nom, infos.Grosseur);
                    break;
                case VariétésFusils.Shotgun:
                    fusil = GénérerShotgunTexte(nom, infos.Grosseur);
                    break;
                case VariétésFusils.Vanille:
                    fusil = GénérerFusilTexte(nom, infos.Grosseur);
                    break;
            }
            return fusil;
        }

        public static ArmeLancée GetRandomArmeLancée()
        {
            return GénérerArmeLancéeTexte(NomsArmesLancées[Maths.Random.Next(NomsArmesLancées.Count)], 1);
        }
    }

    class InfoFusil
    {
        public VariétésFusils Variété;
        public float Grosseur;

        public InfoFusil(string nom, float grosseur, VariétésFusils variété)
        {
            GestionArmes.NomsFusils.Add(nom);
            Grosseur = grosseur;
            Variété = variété;
        }
    }
}
