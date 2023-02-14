using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CSProject
{
    public abstract class GameObject
    {
        #region fields

        public Vector2 Location; 
        protected Texture2D Texture;
        public Rectangle Edge;
        protected Color currentcolour;
        #endregion


        #region methods

        public virtual void LoadContent(ContentManager Content)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, currentcolour);
        }







        #endregion



    }
}
