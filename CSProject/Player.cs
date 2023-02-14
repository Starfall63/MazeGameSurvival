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

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            Texture = Content.Load<Texture2D>(@"SpriteMapHero");
        }

        public void Update(GameTime gameTime, List<CollisionTiles> tiles)
        {
            currentcolour = Color.White;
            Edge = new Rectangle((int)Location.X, (int)Location.Y, 32, 54);
            if (bufftime == 0) speed = baseSpeed;

            KeyboardState ks = Keyboard.GetState();
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

            if (bufftime > 0) timeExpiredSinceBuff += gameTime.ElapsedGameTime.Milliseconds;
            if (timeExpiredSinceBuff >= bufftime)
            {
                speed = baseSpeed;
                bufftime = 0;
                timeExpiredSinceBuff = 0;

            }
            timeExpired += gameTime.ElapsedGameTime.Milliseconds;

          
            if (timeExpired > FrameTime)
            {
                Frame = (Frame + 1) % 4; 
                timeExpired = 0;
            }
            if (!ks.IsKeyDown(Keys.A) && !ks.IsKeyDown(Keys.S) && !ks.IsKeyDown(Keys.W) && !ks.IsKeyDown(Keys.D)) { Frame = 0; };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            int X = Frame * 32;
            int Y = ((int)direction * 54);
            spriteBatch.Draw(Texture, Location, new Rectangle(X, Y, 32, 54), currentcolour);
        }


        protected bool LeftCollision(CollisionTiles T)
        {
            return this.Edge.Right + this.Movement.X > T.Rectangle.Left &&
                    this.Edge.Left < T.Rectangle.Left &&
                    this.Edge.Bottom > T.Rectangle.Top &&
                    this.Edge.Top < T.Rectangle.Bottom;
        }

        protected bool RightCollision(CollisionTiles T)
        {
            return this.Edge.Left + this.Movement.X < T.Rectangle.Right &&
                    this.Edge.Right > T.Rectangle.Right &&
                    this.Edge.Bottom > T.Rectangle.Top &&
                    this.Edge.Top < T.Rectangle.Bottom;
        }

        protected bool TopCollision(CollisionTiles T)
        {
            return this.Edge.Bottom + this.Movement.Y > T.Rectangle.Top &&
                   this.Edge.Top < T.Rectangle.Top &&
                   this.Edge.Right > T.Rectangle.Left &&
                   this.Edge.Left < T.Rectangle.Right;
        }

        protected bool BottomCollision(CollisionTiles T)
        {
            return this.Edge.Top + this.Movement.Y < T.Rectangle.Bottom &&
                   this.Edge.Bottom > T.Rectangle.Bottom &&
                   this.Edge.Right > T.Rectangle.Left &&
                   this.Edge.Left < T.Rectangle.Right;
        }




        public void gainhealth(HealthBuff h)
        {
            if (Edge.Intersects(h.Edge))
                health += h.amount;
            if (health > maxHealth)
                health = maxHealth;


        }

        public void gainspeed(SpeedBuff s)
        {
            if (Edge.Intersects(s.Edge))
                speed += s.amount;
            if (speed >= maxSpeed)
                speed = maxSpeed;
            bufftime = s.bufftime;
        }

        public void takeDamage(int damage)
        {
            health -= damage;
            currentcolour = Color.Red;
        }


    }
}
