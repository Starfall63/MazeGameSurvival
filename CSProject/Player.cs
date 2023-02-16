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
    class Player : GameObject
    {

        public enum Direction
        {
            down,
            left,
            right,
            up,
        }
        private Vector2 Movement; 
        private float FrameTime = 100; 
        private int Frame = 0; 
        public Direction direction = Direction.down; 
        private float timeExpired;
        private float timeExpiredSinceBuff;
        private const int baseSpeed = 5;
        public int killcount;
        public int speed;
        public int bufftime;
        private const int maxSpeed = 12;
        private const int maxHealth = 100;
        public int health;
        public Weapon equippedWeapon;

        /// <summary>
        /// Spawns the player in a random place in the maze making sure it isn't inside a wall.
        /// </summary>
        /// <param name="maze"></param>
        public Player(int[,] maze)
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
            speed = baseSpeed;
            Location = new Vector2(xspawn * GameState.pixelsize, yspawn * GameState.pixelsize);
            Movement = new Vector2(0, 0);
            currentcolour = Color.White;
            health = maxHealth;
        }

        /// <summary>
        /// Loads the texture of the player.
        /// </summary>
        /// <param name="Content"></param>
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            Texture = Content.Load<Texture2D>(@"SpriteMapHero");
        }

        public void Update(GameTime gameTime, List<CollisionTiles> tiles)
        {
            currentcolour = Color.White;
            Edge = new Rectangle((int)Location.X, (int)Location.Y, 32, 54);

            //Resets the speed of the player back to normal when the speed buff has worn out.
            if (bufftime == 0) speed = baseSpeed;

            KeyboardState ks = Keyboard.GetState();

            //User controls for moving the player.
            //Will stop the movement of the player if it will hit a wall.
            if (ks.IsKeyDown(Keys.A))
            {
                Movement.X = -speed;
                Movement.Y = 0;
                direction = Direction.left;
                foreach (CollisionTiles t in tiles)
                    if (RightCollision(t)) Movement.X = 0;
                Location = Location + Movement;
            }
            else if (ks.IsKeyDown(Keys.D))
            {
                Movement.X = speed;
                Movement.Y = 0;
                direction = Direction.right;
                foreach (CollisionTiles t in tiles)
                    if (LeftCollision(t)) Movement.X = 0;
                Location = Location + Movement;
            }
            else if (ks.IsKeyDown(Keys.S))
            {
                Movement.X = 0;
                Movement.Y = speed;
                direction = Direction.down;
                foreach (CollisionTiles t in tiles)
                    if (TopCollision(t)) Movement.Y = 0;
                Location = Location + Movement;
            }
            else if (ks.IsKeyDown(Keys.W))
            {
                Movement.X = 0;
                Movement.Y = -speed;
                direction = Direction.up;
                foreach (CollisionTiles t in tiles)
                    if (BottomCollision(t)) Movement.Y = 0;
                Location = Location + Movement;
            }

            Vector2 boundary1 = new Vector2(0, 0);
            Vector2 boundary2 = new Vector2(1366, 768);
            if (Location.X <= 0) Location.X = 0;
            if (Location.Y <= 0) Location.Y = 0;

            //Resets the speed of the player back to normal when the speed buff has worn out.
            if (bufftime > 0) timeExpiredSinceBuff += gameTime.ElapsedGameTime.Milliseconds;
            if (timeExpiredSinceBuff >= bufftime)
            {
                speed = baseSpeed;
                bufftime = 0;
                timeExpiredSinceBuff = 0;

            }
            timeExpired += gameTime.ElapsedGameTime.Milliseconds;

            //Changes the frame for animation when moving.
            if (timeExpired > FrameTime)
            {
                Frame = (Frame + 1) % 4; 
                timeExpired = 0;
            }
            if (!ks.IsKeyDown(Keys.A) && !ks.IsKeyDown(Keys.S) && !ks.IsKeyDown(Keys.W) && !ks.IsKeyDown(Keys.D)) { Frame = 0; };
        }

        //Draws the player making sure to select the correct frame from the spritesheet.
        public override void Draw(SpriteBatch spriteBatch)
        {

            int X = Frame * 32;
            int Y = ((int)direction * 54);
            spriteBatch.Draw(Texture, Location, new Rectangle(X, Y, 32, 54), currentcolour);
        }

        /// <summary>
        /// Function to check whether the player will collide with the left hand side of a wall when walking right.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        protected bool LeftCollision(CollisionTiles T)
        {
            return this.Edge.Right + this.Movement.X > T.Rectangle.Left &&
                    this.Edge.Left < T.Rectangle.Left &&
                    this.Edge.Bottom > T.Rectangle.Top &&
                    this.Edge.Top < T.Rectangle.Bottom;
        }

        /// <summary>
        /// Function to check whether the player will collide with the right hand side of a wall when walking left.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        protected bool RightCollision(CollisionTiles T)
        {
            return this.Edge.Left + this.Movement.X < T.Rectangle.Right &&
                    this.Edge.Right > T.Rectangle.Right &&
                    this.Edge.Bottom > T.Rectangle.Top &&
                    this.Edge.Top < T.Rectangle.Bottom;
        }

        /// <summary>
        /// Function to check whether the player will collide with the top of a wall when walking downwards.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        protected bool TopCollision(CollisionTiles T)
        {
            return this.Edge.Bottom + this.Movement.Y > T.Rectangle.Top &&
                   this.Edge.Top < T.Rectangle.Top &&
                   this.Edge.Right > T.Rectangle.Left &&
                   this.Edge.Left < T.Rectangle.Right;
        }

        /// <summary>
        /// Function to check whether the player will collide with the bottom of the wall when walking upwards.
        /// </summary>
        /// <param name="T"></param>
        /// <returns></returns>
        protected bool BottomCollision(CollisionTiles T)
        {
            return this.Edge.Top + this.Movement.Y < T.Rectangle.Bottom &&
                   this.Edge.Bottom > T.Rectangle.Bottom &&
                   this.Edge.Right > T.Rectangle.Left &&
                   this.Edge.Left < T.Rectangle.Right;
        }



        /// <summary>
        /// Recovers the health of the player when picking up a health item making sure it doesn't go past 100 hp.
        /// </summary>
        /// <param name="h"></param>
        public void gainhealth(HealthBuff h)
        {
            if (Edge.Intersects(h.Edge))
                health += h.amount;
            if (health > maxHealth)
                health = maxHealth;


        }

        /// <summary>
        /// Applies a speed boost for a set amount of time when picking up a speed item.
        /// </summary>
        /// <param name="s"></param>
        public void gainspeed(SpeedBuff s)
        {
            if (Edge.Intersects(s.Edge))
                speed += s.amount;
            if (speed >= maxSpeed)
                speed = maxSpeed;
            bufftime = s.bufftime;
        }

        /// <summary>
        /// Makes player take damage.
        /// </summary>
        /// <param name="damage"></param>
        public void takeDamage(int damage)
        {
            health -= damage;
            currentcolour = Color.Red;
        }


    }
}
