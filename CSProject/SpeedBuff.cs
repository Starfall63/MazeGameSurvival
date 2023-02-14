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
        public int amount;
        public int bufftime;
        public bool touched = false;
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
            //bufftime = RNG.Next(5, 20);
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


    }
}
