using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CSProject
{
    internal class Tiles
    {
        #region fields
        protected Texture2D texture;
        public const int maxhealth = 50;
        protected int health;
        private Rectangle rectangle;
      


        public Rectangle Rectangle
        {
            get { return rectangle; }
            protected set { rectangle = value; }
        }

        private static ContentManager content;
        public static ContentManager Content
        {
            protected  get { return content; }
            set { content = value; }
        }

        #endregion

        #region methods
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
        #endregion



    }
}
