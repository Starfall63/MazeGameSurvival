using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSProject
{
    class SpeedBuff:GameObject
    {
        #region fields
        public int amount;
        public int bufftime;
        public bool touched = false;
        #endregion

        #region methods
        /// <summary>
        /// Constructor that will spawn the speed item in a random place in the maze making sure it isn't inside a wall.
        /// Assigns a random amount of speed a player will gain when picking up a speed item between 1 and 8.
        /// Assigns the time the speed buff will last after the player has picked up the speed item.
        /// </summary>
        /// <param name="maze"></param>
        public SpeedBuff(int[,] maze)
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

            Location = new Vector2((xspawn * GameState.pixelsize) + 16, (yspawn * GameState.pixelsize) + 16);
            Edge = new Rectangle((int)Location.X, (int)Location.Y, 32, 44);
            amount = RNG.Next(1, 8);

            bufftime = RNG.Next(5000, 20000);
        }


        public override void LoadContent(ContentManager Content)
        {
            Texture = Content.Load<Texture2D>("Speed");

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
