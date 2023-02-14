using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CSProject
{
    class Weapon:GameObject
    {
        public int damage;
        public bool equipped = false;
        protected KeyboardState previousKey;
        protected KeyboardState currentKey;
        public string type;
       

        public Weapon(int[,] maze, string t)
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
            Edge = new Rectangle((int)Location.X, (int)Location.Y, 32, 25);
            damage = RNG.Next(5, 15);
            type = t;

        }


        public override void LoadContent(ContentManager Content)
        {
            
            Texture = Content.Load<Texture2D>(type);

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
