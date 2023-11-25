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
using AwesomeAnimation;
namespace AweSOME
{
    class Inventaire
    {
        public bool EstPlein
        {
            get
            {
                bool plein=true;
                for (int i = 0; i < Items.Length && plein; ++i)
                {
                    plein &= Items[i] != null;
                }
                return plein;
            }
        }
        public IRamassable[] Items = new IRamassable[5];
        public Sprite[] SpriteDessins = new Sprite[5];

        public PersonnageArmé Propriétaire;

        public Inventaire(PersonnageArmé propriétaire)
        {
            Propriétaire = propriétaire;

            Texture2D RectangleItem = GestionTexture.GetTexture("RectangleInventaire");
            Vector2 départ = new Vector2(Caméra.DimensionsFenêtreSurDeux.X - 300,Caméra.DimensionsFenêtre.Y -50);
            for (int i = 0; i < SpriteDessins.Length; ++i)
            {
                SpriteDessins[i] = new Sprite(départ,new Vector2(150,75));
                SpriteDessins[i].Profondeur = 0;
                SpriteDessins[i].ArrangerSprite(RectangleItem);
                départ.X += 150;

            }
        }

        public void AjouterItem(IRamassable item)
        {
            int index = ObtenirIndexLibre();
            if (index < Items.Length)
            {               
                Items[index] = item;
                item.EntréeInventaire();
            }
        }
        public void AjouterItem(IRamassable item, int index)
        {
            if (index < Items.Length)
            {
                Items[index] = item;
                if (item != null)
                {
                    item.EntréeInventaire();
                }
            }
        }
        public bool PossèdeDéjàFusil(Fusil nouveauFusil, out Fusil fusilDéjàPossédé)
        {
            fusilDéjàPossédé = null;
            IRamassable item = null;
            Fusil fusil = null;

            for (int i = 0; i < Items.Length; ++i)
            {
                item = Items[i];
                if (item != null && item is Fusil)
                {
                    fusil = (Fusil)item;
                    if (fusil.Nom == nouveauFusil.Nom)
                    {
                        fusilDéjàPossédé = fusil;
                        return true;
                    }
                }
            }
            return false;
        }
        public int ObtenirIndexLibre()
        {
            for(int i=0;i<Items.Length;++i)
            {
                if (Items[i] == null)
                {
                    return i;
                }
            }
            return Items.Length;
        }
        public IRamassable ObtenirMeilleurItemLancable(IRamassable itemActuel)
        {
            IRamassable item = itemActuel;
            IRamassable iterateur = null;

            for (int i = 0; i < Items.Length && item == itemActuel; ++i)
            {
                iterateur = Items[i];
                if (iterateur != null)
                {
                    if (iterateur is ArmeLancée)
                    {
                        item = ÉchangerItem(i, itemActuel);
                    }
                    else if (iterateur is Flare)
                    {
                        item = ÉchangerItem(i, itemActuel);
                    }
                }
            }
            return item;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Items.Length; ++i)
            {
                SpriteDessins[i].Draw(spriteBatch);
                if (Items[i] != null)
                {                   
                    Items[i].Draw(spriteBatch, SpriteDessins[i].Position);
                }
            }
        }

        public IRamassable ÉchangerItem(int index, IRamassable itemActuel)
        {
            IRamassable item=Items[index];           
            if (item != null)
            {
                AjouterItem(itemActuel, index);
                item.SortieInventaire();
                return item;
            }
            return itemActuel;
        }
    }
}
