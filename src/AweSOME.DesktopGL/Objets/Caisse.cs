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
    enum TypesCaisse { Guns, ArmesLancées, ObjetsDivers, TotalRandom, TypeRandom }
    class Caisse:PolygonePhysique
    {
        IRamassable[] Objets;
        int nbObjets;

        public Caisse(Vector2 position, TypesCaisse type)
            :base(position,Vector2.One*80)
        {
            //Masse *= 2;
            //GRAVITÉ *= 2;
            if (type == TypesCaisse.TypeRandom)
            {
                type = (TypesCaisse)Maths.Random.Next((int)TypesCaisse.TypeRandom);
            }

            Matériel = MatérielPolygone.Bois;
            ArrangerSprite(GestionTexture.GetTexture("Objets/Crate0"));
            CréerOmbre(100);
            switch (type)
            {
                case TypesCaisse.ArmesLancées:
                    nbObjets = Maths.Random.Next(2, 5);
                    Objets = new IRamassable[nbObjets];
                    for (int i = 0; i < nbObjets; ++i)
                    {
                        Objets[i] = GestionArmes.GetRandomArmeLancée();
                    }
                    break;
                case TypesCaisse.Guns:
                    nbObjets = Maths.Random.Next(1, 4);
                    Objets = new IRamassable[nbObjets];
                    for (int i = 0; i < nbObjets; ++i)
                    {
                        Objets[i] = GestionArmes.GetRandomFusil();
                    }
                    break;
                case TypesCaisse.ObjetsDivers:
                    nbObjets = Maths.Random.Next(2, 6);
                    Objets = new IRamassable[nbObjets];
                    for (int i = 0; i < nbObjets; ++i)
                    {
                        Objets[i] = CréerObjetDiversRandom();
                    }
                    break;
                case TypesCaisse.TotalRandom:
                    nbObjets = Maths.Random.Next(2, 6);
                    Objets = new IRamassable[nbObjets];
                    for (int i = 0; i < nbObjets; ++i)
                    {
                        Objets[i] = CréerObjetRandom();
                    }
                    break;
            }
        }

        private IRamassable CréerObjetRandom()
        {
            int random = Maths.Random.Next(3);
            switch (random)
            {
                case 0:
                    return GestionArmes.GetRandomArmeLancée();
                case 1:
                    return GestionArmes.GetRandomFusil();
                case 2:
                    return new Flare(Vector2.Zero);
                default:
                    return GestionArmes.GetRandomArmeLancée();
            }
        }
        private IRamassable CréerObjetDiversRandom()
        {
            int random = Maths.Random.Next(1);
            switch (random)
            {
                case 0:
                    return new Flare(Vector2.Zero);
                case 1:
                    return new Flare(Vector2.Zero);
                default:
                    return new Flare(Vector2.Zero);
            }
        }

        public override void Détruire()
        {
            base.Détruire();
            LibérerContenu();
            DétruireOmbre();
        }

        public override void Update()
        {
            base.Update();
        }

        public void LibérerContenu()
        {
            for (int i = 0; i < nbObjets; ++i)
            {
                Objets[i].SetPosition(AnciennePosition);
                Objets[i].SeFaireLaisser();
            }
        }
              
        
    }
}
