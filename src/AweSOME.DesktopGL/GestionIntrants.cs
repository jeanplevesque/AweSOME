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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace AweSOME
{

    static class GestionIntrants
    {
        public delegate void ÉvénementSouris();
        public delegate void ÉvénementClavier();
        public static event ÉvénementSouris ClickGauche = new ÉvénementSouris(surclique);
        public static event ÉvénementSouris ClickGaucheInverse;// = new ÉvénementSouris(surclique);
        public static event ÉvénementSouris ClickDroite = new ÉvénementSouris(surclique);
        public static event ÉvénementSouris ClickDroiteInverse = new ÉvénementSouris(surclique);
        public static event ÉvénementSouris ClickMilieu = new ÉvénementSouris(surclique);
        public static event ÉvénementSouris ClickMilieuInverse = new ÉvénementSouris(surclique);
        public static event ÉvénementClavier ToucheDelete;
        public static event ÉvénementClavier ToucheEscape;
        //public static event ÉvénementClavier ToucheF2;

        public static void surclique(){}

        public static bool NouvellesTouchesDispo { get; private set; }
        public static KeyboardState ÉtatClavier { get; private set; }
        public static KeyboardState AncienÉtatClavier { get; private set; }
        public static Keys[] AnciennesTouches { get; private set; }
        public static List<Keys> NouvellesTouches { get; private set; }
        public static MouseState ÉtatSouris { get; private set; }
        public static MouseState AncienÉtatSouris { get; private set; }
        public static Vector2 PositionSouris
        {
            get
            {
                return positionsouris_;
            }
            set
            {
                Mouse.SetPosition((int)value.X, (int)value.Y);
                positionsouris_ = value;
            }
        }
        static Vector2 positionsouris_;
        public static Vector2 PositionSourisCaméra
        {
            get
            {
                //return Caméra.Transform(new Vector2(ÉtatSouris.X, ÉtatSouris.Y));
                //return Caméra.Untransform(new Vector2(ÉtatSouris.X, ÉtatSouris.Y));
                Vector2 résultat = Vector2.Zero;              

                Vector2.Transform(ref positionsouris_, ref Caméra.MatriceInverse, out résultat);
                return résultat;
            }
        }
        static Keys[] touches_;

        static GestionIntrants()
        {
            AnciennesTouches = new Keys[0];
            NouvellesTouches = new List<Keys>();

            ToucheEscape+= new ÉvénementClavier(GestionÉcran.AllerÀÉcranPrécédent);
        }

        private static void CalculerPositionSouris()
        {
            //PositionSouris = new Vector2(ÉtatSouris.X, ÉtatSouris.Y);
            positionsouris_.X = ÉtatSouris.X;
            positionsouris_.Y = ÉtatSouris.Y;
        }

        public static bool ClicGauche;
        //{
        //    get
        //    {
        //        return ÉtatSouris.LeftButton == ButtonState.Released && AncienÉtatSouris.LeftButton == ButtonState.Pressed;
        //    }
        //}
        public static bool ClicGaucheInverse;
        //{
        //    get
        //    {
        //        return ÉtatSouris.LeftButton == ButtonState.Pressed && AncienÉtatSouris.LeftButton == ButtonState.Released;
        //    }
        //}
        public static bool ClicDroit;
        //{
        //    get
        //    {
        //        return ÉtatSouris.RightButton == ButtonState.Released && AncienÉtatSouris.RightButton == ButtonState.Pressed;
        //    }
        //}
        public static bool ClicDroitInverse;
        //{
        //    get
        //    {
        //        return ÉtatSouris.RightButton == ButtonState.Pressed && AncienÉtatSouris.RightButton == ButtonState.Released;
        //    }
        //}
        public static bool ClicMilieu;
        //{
        //    get
        //    {
        //        return ÉtatSouris.MiddleButton == ButtonState.Released && AncienÉtatSouris.MiddleButton == ButtonState.Pressed;
        //    }
        //}
        public static bool ClicMilieuInverse;
        //{
        //    get
        //    {
        //        return ÉtatSouris.MiddleButton == ButtonState.Pressed && AncienÉtatSouris.MiddleButton == ButtonState.Released;
        //    }
        //}
        public static bool BoutonSourisGaucheEnfoncé
        {
            get
            {
                return ÉtatSouris.LeftButton == ButtonState.Pressed;
            }
        }
        public static bool BoutonSourisDroitEnfoncé
        {
            get
            {
                return ÉtatSouris.RightButton == ButtonState.Pressed;
            }
        }
        public static bool BoutonSourisMilieuEnfoncé
        {
            get
            {
                return ÉtatSouris.MiddleButton == ButtonState.Pressed;
            }
        }

        public static int XSourisCaméra
        {
            get { return (int)PositionSourisCaméra.X; }
        }
        public static int YSourisCaméra
        {
            get { return (int)PositionSourisCaméra.Y; }
        }

        public static string InputString;

        ///// <summary>
        ///// Lit le texte écrit au clavier et l'ajoute au texte envoyé
        ///// </summary>
        ///// <param name="texte">Le texte à modifier</param>
        ///// <returns></returns>
        //public static string AjouterTexteTapé(string texte)
        //{
        //    Keys[] nouvellesTouches = ÉtatClavier.GetPressedKeys();
        //    foreach (Keys k in nouvellesTouches)
        //    {
        //        if (EstNouvelleToucheEnfoncée(k))
        //        {
        //            string touche = k.ToString().ToLower();
        //            if (EstToucheEnfoncée(Keys.LeftShift) || EstToucheEnfoncée(Keys.RightShift))
        //            {
        //                touche = touche.ToUpper();
        //            }
        //            if (touche.Length == 1)
        //            {
        //                texte += touche;
        //            }
        //            else
        //            {
        //                switch (k)
        //                {
        //                    case Keys.Space:
        //                        texte += ' ';
        //                        break;
        //                    case Keys.Subtract:
        //                        texte += '-';
        //                        break;
        //                    case Keys.Back:
        //                        if (texte.Length > 0)
        //                        {
        //                            texte=texte.Remove(texte.Length - 1, 1);
        //                        }
        //                        break;
        //                }
        //            }

        //        }
        //    }
        //    return texte;
        //}
        ///// <summary>
        ///// Lit le texte écrit au clavier et l'ajoute au texte envoyé
        ///// </summary>
        ///// <param name="texte">Le texte à modifier</param>
        //public static void AjouterTexteTapé(ref string texte)
        //{
        //    Keys[] nouvellesTouches = ÉtatClavier.GetPressedKeys();
        //    foreach (Keys k in nouvellesTouches)
        //    {
        //        if (EstNouvelleToucheEnfoncée(k))
        //        {
        //            string touche = k.ToString().ToLower();
        //            if (EstToucheEnfoncée(Keys.LeftShift) || EstToucheEnfoncée(Keys.RightShift))
        //            {
        //                touche = touche.ToUpper();
        //            }
        //            if (touche.Length == 1)
        //            {
        //                texte += touche;
        //            }
        //            else
        //            {
        //                switch (k)
        //                {
        //                    case Keys.Space:
        //                        texte += ' ';
        //                        break;
        //                    case Keys.Subtract:
        //                        texte += '-';
        //                        break;
        //                    case Keys.Back:
        //                        if (texte.Length > 0)
        //                        {
        //                            texte = texte.Remove(texte.Length - 1, 1);
        //                        }
        //                        break;
        //                }
        //            }

        //        }
        //    }
        //}

        public static string GetString()
        {
            string texte = "";

            foreach (Keys k in NouvellesTouches)
            {
                string touche = k.ToString().ToLower();
                if (EstToucheEnfoncée(Keys.LeftShift) || EstToucheEnfoncée(Keys.RightShift))
                {
                    touche = touche.ToUpper();
                }
                if (touche.Length == 1)
                {
                    texte += touche;
                }
                else
                {
                    switch (k)
                    {
                        case Keys.Space:
                            texte += ' ';
                            break;
                        case Keys.Subtract:
                            texte += '-';
                            break;
                        //case Keys.Back:
                        //    if (texte.Length > 0)
                        //    {
                        //        texte = texte.Remove(texte.Length - 1, 1);
                        //    }
                        //    break;
                    }
                }
            }
            return texte;
        }

        public static bool EstToucheEnfoncée(Keys touche)
        {
            return ÉtatClavier.IsKeyDown(touche);
        }
        public static bool EstNouvelleToucheEnfoncée(Keys touche)
        {
            //bool estEnfoncée = EstToucheEnfoncée(touche);
            //bool estNouvelleTouche = true;
            //if (estEnfoncée)
            //{
            //    for (int i = 0; i < AnciennesTouches.Length && estNouvelleTouche; ++i)
            //    {
            //        estNouvelleTouche = AnciennesTouches[i] != touche;//dès qu'on retrouve la meme touche, on sait qu'on l'avait déjà
            //    }
            //}
            //return estNouvelleTouche&&estEnfoncée;

            return NouvellesTouches.Contains(touche);
        }

        

        public static void Update()
        {
            AncienÉtatClavier = ÉtatClavier;
            AncienÉtatSouris = ÉtatSouris;
            AnciennesTouches = touches_;

            CalculerNouvellesTouches();
            CalculerInputString();

            ÉtatClavier = Keyboard.GetState();
            ÉtatSouris = Mouse.GetState();
            CalculerPositionSouris();
            CalculerClicsSouris();
            //PositionSouris = new Vector2(ÉtatSouris.X, ÉtatSouris.Y);
        }

        private static void CalculerClicsSouris()
        {
            ClicGauche = ÉtatSouris.LeftButton == ButtonState.Released && AncienÉtatSouris.LeftButton == ButtonState.Pressed;
            if (ClicGauche) { ClickGauche.Invoke(); }
            
            ClicGaucheInverse = ÉtatSouris.LeftButton == ButtonState.Pressed && AncienÉtatSouris.LeftButton == ButtonState.Released;
            if (ClicGaucheInverse) { ClickGaucheInverse.Invoke(); }
            
            ClicDroit = ÉtatSouris.RightButton == ButtonState.Released && AncienÉtatSouris.RightButton == ButtonState.Pressed;
            if (ClicDroit) { ClickDroite.Invoke(); }
            
            ClicDroitInverse = ÉtatSouris.RightButton == ButtonState.Pressed && AncienÉtatSouris.RightButton == ButtonState.Released;
            if (ClicDroitInverse) { ClickDroiteInverse.Invoke(); }
            
            ClicMilieu = ÉtatSouris.MiddleButton == ButtonState.Released && AncienÉtatSouris.MiddleButton == ButtonState.Pressed;
            if (ClicMilieu) { ClickMilieu.Invoke(); }
            
            ClicMilieuInverse = ÉtatSouris.MiddleButton == ButtonState.Pressed && AncienÉtatSouris.MiddleButton == ButtonState.Released;
            if (ClicMilieuInverse) { ClickMilieuInverse.Invoke(); }
        }

        private static void CalculerNouvellesTouches()
        {
            touches_ = ÉtatClavier.GetPressedKeys();
            NouvellesTouches.Clear();
            NouvellesTouchesDispo = false;
            foreach (Keys k in touches_)
            {
                if (!AnciennesTouches.Contains(k))
                {
                    NouvellesTouches.Add(k);
                    NouvellesTouchesDispo = true;

                    CalculerTouchesSpéciales(k);
                }
            }
        }

        private static void CalculerTouchesSpéciales(Keys k)
        {
            switch (k)
            {
                case Keys.Delete:
                    ToucheDelete.Invoke();
                    break;
                case Keys.Escape:
                    ToucheEscape.Invoke();
                    break;
                //case Keys.F2:
                //    ToucheF2.Invoke();
                //    break;
            }
        }
        private static void CalculerInputString()
        {
            //if (EstNouvelleToucheEnfoncée(Keys.Space))
            //{
            //    InputString = "";
            //}
            if(NouvellesTouchesDispo)
            {
                InputString += GetString();
            }
        }

        public static void ResetInputString()
        {
            InputString = "";
        }
    }
}
