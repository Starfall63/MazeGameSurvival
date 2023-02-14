using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CSProject
{
    internal class Bullets:GameObject
    {

        public int damage;
        public Vector2 Velocity;

        public string type;
        public bool isVisible;


        public Bullets(Texture2D newTexture, string t)
        {
            Random RNG = new Random();
            Texture = newTexture;
            isVisible = false;
            damage = RNG.Next(1, 20);
            type = t;
        }


        public override void Update(GameTime gameTime)
        {
            Edge = new Rectangle((int)Location.X, (int)Location.Y, 16, 16);
            Location += Velocity;


        }


        
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, Color.White);
        }



        

    }
}
