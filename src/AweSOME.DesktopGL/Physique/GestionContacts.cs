using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace AweSOME
{
    class GestionContacts
    {
        public static List<Contact> ListeContact = new List<Contact>();

        public static int NbContacts { get; private set; }

        static GestionContacts()
        {

        }

        public static void RésoudreContacts()
        {
            if (ListeContact.Count > 0)
            {
                Résoudre();
            }
            NbContacts = ListeContact.Count;
            ListeContact.Clear();
        }

        private static void Résoudre()
        {
            foreach (Contact c in ListeContact)
            {
                c.Résoudre();
            }
        }

        
        public static void Ajouter(PolygonePhysique polyIncident, PolygonePhysique polyQuiSeFaitFrapper, Vector2 position, Vector2 normale, Vector2 vecteurDirecteur, float pénétration)
        {
            ListeContact.Add(new Contact(polyIncident, polyQuiSeFaitFrapper, position, normale,vecteurDirecteur, pénétration));
        }
        public static void Ajouter(Contact contact, PolygonePhysique polyIncident)
        {
            ListeContact.Add(new Contact(polyIncident,contact.PolyQuiSeFaitFrapper,contact.Position,contact.Normal,contact.VecteurDirecteur,contact.Pénétration));
        }
        public static void Ajouter(Contact contact)
        {
            if (contact != null)
            {
                ListeContact.Add(contact);
            }
        }

        
    }
}
