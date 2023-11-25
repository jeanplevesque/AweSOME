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
    public enum TextBoxWarpTypes { KeepAspectRatio, WarpYOnly, WarpXOnly, WarpBoth }
    public class TextBox:Sprite
    {
        public bool TexteCentréX
        {
            get { return texteCentréX_; }
            set
            {
                texteCentréX_ = value;
                if (texteCentréX_)
                {
                    //PositionTexte.X = Position.X;
                    DistancePositionTexteOrigine.X = 0;
                }
                else
                {
                    //PositionTexte.X = Position.X - Dimensions.X / 2;
                    DistancePositionTexteOrigine.X = -Dimensions.X / 2;
                }
                Vector2.Transform(ref DistancePositionTexteOrigine, ref Matrice, out DistancePositionTexte);
                PositionTexte = Position + DistancePositionTexte;
            }
        }
        private bool texteCentréX_;
        public bool TexteCentréY
        {
            get { return texteCentréY_; }
            set
            {
                texteCentréY_ = value;
                //if (texteCentréY_)
                //{
                //    //PositionTexte.X = Position.X;
                //    //DistancePositionTexteOrigine.Y = 0;
                //}
                //else
                //{
                //    //PositionTexte.X = Position.X - Dimensions.X / 2;
                //    //DistancePositionTexteOrigine.Y = -Dimensions.Y / 2;
                //}
            }
        }
        private bool texteCentréY_;

        public EffetTexte EffetSurTexte;

        public SpriteFont Font;
        public Color CouleurTexte;
        public string Texte="";
        public Vector2 PositionTexte;
        public Vector2 DistancePositionTexte;
        public Vector2 DistancePositionTexteOrigine;
        public Vector2 OrigineTexte = Vector2.Zero;
        public Vector2 ScaleTexte = Vector2.One;
        public float ProfondeurTexte;

        protected string DernièreLigne
        {
            get
            {
                //if (ListeLignes.Count > 0)
                //{
                    return ListeLignes[ListeLignes.Count - 1];
                //}
                //else
                //{
                //    return "";
                //}
            }
            set
            {
                ListeLignes[ListeLignes.Count-1] = value;
            }
        }
        protected List<string> ListeLignes = new List<string>();

        protected Matrix Matrice=Matrix.Identity;

        public TextBox(Vector2 position, Vector2 dimensions)
            : base(position, dimensions)
        {
            ProfondeurTexte = Profondeur - 0.001f;

            Couleur = Color.DarkGray * 0.5f;

            //CouleurTexte = new Color(0, 255, 0);
            CouleurTexte = Color.LightGray * 0.5f;
            //CouleurTexte.A = 50;

            Font = BanqueContent.Font1;
            ArrangerSprite(GestionTexture.GetTexture("Pixel"));

            //PositionTexte = Position - Dimensions / 2;
            DistancePositionTexteOrigine = -Dimensions / 2;
            DistancePositionTexte = DistancePositionTexteOrigine;
            
            Clear();

            TexteCentréX = false;
            TexteCentréY = false;

            //WriteLine("AwesomeConsole");
            //WriteLine("is Awesome");
            //WarpTexte(TextBoxWarpTypes.KeepAspectRatio);
            
            //CentrerTexteXY();
            //ChangerAngle(MathHelper.PiOver2);
            EffetSurTexte = new EffetTexte(TypesEffetsTexte.OscillationCouleur, this, Color.LightGray , CouleurTexte);
        }

        public void WriteLine(Vector2 vecteur)
        {
            WriteLine(vecteur.ToString());
        }
        public void WriteLine(string texte)
        {
            if (DernièreLigne != "")
            {
                ListeLignes.Add(texte);
                FormaterTexte();
                CréerLignes();
                RestreindreHauteurTexte();
            }
            else
            {
                Write(texte);
            }
            
        }
        public void Write(string texte)
        {
            DernièreLigne += texte;
            FormaterTexte();
            CréerLignes();
            RestreindreHauteurTexte();
        }

        public void Clear()
        {
            ListeLignes.Clear();
            ListeLignes.Add("");
            Texte = "";
        }

        public void ChangerAngle(float angle)
        {
            //float deltaAngle = angle - Angle;
            Angle = angle;
            Matrix.CreateRotationZ(Angle, out Matrice);

            Vector2.Transform(ref DistancePositionTexteOrigine, ref Matrice, out DistancePositionTexte);
            PositionTexte = Position + DistancePositionTexte;
        }

        public void CentrerTexteXY()
        {
            TexteCentréX = true;
            TexteCentréY = true;

            //Vector2 dimensions = Font.MeasureString(Texte);
            //OrigineTexte = dimensions / 2;
            //PositionTexte = Position;
        }
        public void ForcerCentrerTexteXY(ref Vector2 dimensions)
        {
            OrigineTexte = dimensions / 2;
            PositionTexte = Position;
        }
        //public void AllignerTexte()
        //{
        //    OrigineTexte = Vector2.Zero;
        //    PositionTexte = Position+DistancePositionTexte;
        //}
        public void WarpTexte(TextBoxWarpTypes warpType)
        {
            WarpTexte(warpType, ref Dimensions);
        }
        public void WarpTexte(TextBoxWarpTypes warpType, ref Vector2 dimensionsVoulues)
        {
            Vector2 dimensions = Font.MeasureString(Texte);
            switch (warpType)
            {
                case TextBoxWarpTypes.KeepAspectRatio:
                    ScaleTexte.X = dimensionsVoulues.X / dimensions.X;
                    ScaleTexte.Y = dimensionsVoulues.Y / dimensions.Y;

                    if (ScaleTexte.X < ScaleTexte.Y)
                    {
                        ScaleTexte.Y = ScaleTexte.X;
                    }
                    else
                    {
                        ScaleTexte.X = ScaleTexte.Y;
                    }
                    break;
                case TextBoxWarpTypes.WarpXOnly:
                    ScaleTexte.X = dimensionsVoulues.X / dimensions.X;
                    break;
                case TextBoxWarpTypes.WarpYOnly:
                    ScaleTexte.Y = dimensionsVoulues.Y / dimensions.Y;
                    break;
                case TextBoxWarpTypes.WarpBoth:
                    ScaleTexte.X = dimensionsVoulues.X / dimensions.X;
                    ScaleTexte.Y = dimensionsVoulues.Y / dimensions.Y;
                    break;
            }
        }

        protected void FormaterTexte()
        {
            string texteDeTrop = "";
            DernièreLigne = CouperTexteTrop(DernièreLigne, out texteDeTrop);
            if (texteDeTrop != "")
            {
                WriteLine(texteDeTrop);
            }
        }
        protected void CréerLignes()
        {
            Texte = "";
            for (int i = 0; i < ListeLignes.Count; ++i)
            {
                Texte += ListeLignes[i];
                if (i < ListeLignes.Count - 1)
                {
                    Texte += "\n";
                }
            }

        }
        protected void RestreindreHauteurTexte()
        {
            while (TexteTropLong(ref Texte))
            {
                EnleverPremiereLigne();
                CréerLignes();
            }
        }
        protected void EnleverPremiereLigne()
        {
            ListeLignes.RemoveAt(0);
        }
        
        protected string CouperTexteTrop(string texte, out string texteDeTrop)
        {
            texteDeTrop="";
            for (int longueurTexte = texte.Length-1; longueurTexte>0 && LigneTropLongue(ref texte); --longueurTexte )
            {
                texteDeTrop += texte[longueurTexte];
                texte = texte.Remove(longueurTexte, 1);
            }
            texteDeTrop = new string(texteDeTrop.Reverse().ToArray());
            return texte;
        }
        protected void CouperTexteTrop(ref string texte, out string texteDeTrop)
        {
            texteDeTrop = "";
            for (int longueurTexte = texte.Length - 1; longueurTexte > 0 && LigneTropLongue(ref texte); --longueurTexte)
            {
                texteDeTrop += texte[longueurTexte];
                texte = texte.Remove(longueurTexte, 1);
            }
            texteDeTrop = new string(texteDeTrop.Reverse().ToArray());
        }
        protected bool LigneTropLongue(string ligne)
        {
            Vector2 dimensions = Font.MeasureString(ligne);
            return dimensions.X * ScaleTexte.X > Dimensions.X;
        }
        protected bool LigneTropLongue(ref string ligne)
        {
            Vector2 dimensions = Font.MeasureString(ligne);
            return dimensions.X * ScaleTexte.X > Dimensions.X;
        }
        protected bool TexteTropLong(string texte)
        {
            Vector2 dimensions = Font.MeasureString(texte);
            return dimensions.Y * ScaleTexte.Y > Dimensions.Y;
        }
        protected bool TexteTropLong(ref string texte)
        {
            Vector2 dimensions = Font.MeasureString(texte);
            return dimensions.Y * ScaleTexte.Y > Dimensions.Y;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            if (TexteCentréY)
            {
                Vector2 dimensionsY = new Vector2(DistancePositionTexteOrigine.X, Font.MeasureString(Texte).Y * -0.5f * ScaleTexte.Y);
                Vector2 deltaPosition;

                Vector2.Transform(ref dimensionsY, ref Matrice, out deltaPosition);
                PositionTexte = Position + deltaPosition;
            }
            if (TexteCentréX)
            {
                Vector2 dimensions;
                Vector2 deltaPosition;
                string texte;
                for (int i = 0; i < ListeLignes.Count; ++i)
                {
                    texte = ListeLignes[i];
                    dimensions = Font.MeasureString(texte);
                    deltaPosition.X = -dimensions.X * 0.5f * ScaleTexte.X;
                    deltaPosition.Y = i * Font.LineSpacing * ScaleTexte.Y;

                    Vector2.Transform(ref deltaPosition, ref Matrice, out deltaPosition);
                    spriteBatch.DrawString(Font, texte, PositionTexte + deltaPosition, CouleurTexte, Angle, OrigineTexte, ScaleTexte, SpriteEffects.None, ProfondeurTexte);
                }
            }
            else
            {
                spriteBatch.DrawString(Font, Texte, PositionTexte, CouleurTexte, Angle, OrigineTexte, ScaleTexte, SpriteEffects.None, ProfondeurTexte);
            }
        }
        public void DrawDirect(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.DrawString(Font, Texte, PositionTexte, CouleurTexte, Angle, OrigineTexte, ScaleTexte, SpriteEffects.None, ProfondeurTexte);
        }
    }
}
