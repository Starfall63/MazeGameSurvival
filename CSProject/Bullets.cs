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
        #region fields
        public int damage;
        public Vector2 Velocity;

        public string type;
        public bool isVisible;

        #endregion

        #region methods
        /// <summary>
        /// Constructor assigning the texture and type of bullet being created.
        /// </summary>
        /// <param name="newTexture"></param>
        /// <param name="t"></param>
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
            //Moves the bullet and the rectangle of the bullet.
            Edge = new Rectangle((int)Location.X, (int)Location.Y, 16, 16);
            Location += Velocity;


        }


        
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, Color.White);
        }

        #endregion



    }
}
