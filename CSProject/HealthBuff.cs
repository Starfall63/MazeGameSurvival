using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSProject
{
    class HealthBuff:GameObject
    {
        #region fields
        public int amount;
        public bool touched = false;
        #endregion

        #region methods
        /// <summary>
        /// Spawns in a health item in a random place in the maze making sure it isn't spawning inside a wall.
        /// Assigns a random amount of health that can be gained between 1 and 20.
        /// </summary>
        /// <param name="maze"></param>
        public HealthBuff(int[,] maze)
        {
            Random RNG = new Random();
            bool correctSpawn = false;
            int xspawn = 1;
            int yspawn = 1;
            while (!correctSpawn)
            {
                xspawn = RNG.Next(1, GameState.mapwidth - 2);
                yspawn = RNG.Next(1, GameState.mapheight - 2);
                if (maze[yspawn, xspawn] != 1) correctSpawn = true;
            }

            Location = new Vector2((xspawn * GameState.pixelsize) + 16 , (yspawn * GameState.pixelsize) + 16);
            Edge = new Rectangle((int)Location.X, (int)Location.Y, 32 , 30);
            amount = RNG.Next(1, 20);
        }


        public override void LoadContent(ContentManager Content)
        {
            Texture = Content.Load<Texture2D>("health");

        }



        public override void Update(GameTime gameTime)
        {
            
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, Color.White);
        }

        #endregion
    }
}
