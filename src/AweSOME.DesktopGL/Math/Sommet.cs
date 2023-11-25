using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AweSOME
{
    class Sommet : Sprite
    {
        //public List<Droite> ListeDroites = new List<Droite>();
        public Vector2 DistanceCentrePolygone { get; set; }
        public Vector2 DistanceCentrePolygoneInitiale { get; set; }
        public PolygonePhysique CorpsParent;

        public Sommet(Vector2 position, Vector2 distanceCentrePolygone, PolygonePhysique parent)
            : base(position, Vector2.One * 10)
        {
            Couleur = Color.SlateBlue;
            CorpsParent = parent;
            DistanceCentrePolygone = distanceCentrePolygone;
            DistanceCentrePolygoneInitiale = distanceCentrePolygone;
            Profondeur = 0;
        }
        public Sommet(Vector2 position)
            : base(position, Vector2.One * 10)
        {
            DistanceCentrePolygone = Vector2.Zero;
            DistanceCentrePolygoneInitiale = Vector2.Zero;
            Profondeur = 0;
        }
        public Sommet(Vector2 position,Vector2 distanceCentrePolygone)
            : base(position, Vector2.One * 10)
        {
            DistanceCentrePolygone = distanceCentrePolygone;
            DistanceCentrePolygoneInitiale = distanceCentrePolygone;
            Profondeur = 0;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(BanqueContent.Pixel, Caméra.Transform(Position), null, Couleur, 0, Vector2.One * 0.5f, 3 * Caméra.Zoom, SpriteEffects.None, Profondeur);
            spriteBatch.Draw(BanqueContent.Pixel,Position, null, Couleur, 0, Vector2.One * 0.5f, 3, SpriteEffects.None, Profondeur);
        }
    }
}


