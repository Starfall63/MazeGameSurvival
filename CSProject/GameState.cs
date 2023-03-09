using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace CSProject
{
    public class GameState : State
    {
        //ridertest
        #region fields
        Random RNG = new Random();

        //Size of maze and map
        public static int width = 3840;
        public static int height = 2160;


        Map map;
        public static int pixelsize = 64;
        public static int mapheight = (int)(height / pixelsize) + 2;
        public static int mapwidth = (int)(width / pixelsize) + 3;

        public int[,] maze = new int[mapheight, mapwidth];

        private int timer = 0;

        Camera2D GameCamera;
        Vector2 CameraOffSet;

        SpriteFont Font;

        Player player;

        private int wave = 1;



        private List<monster> monsterlist = new List<monster>();
        int monsterstobespawned = 3;
        
        //Time delay before a new wave begins after the current wave has been finished.
        int wavebreak = 10000;
        int timeonbreak = 0;

        //Time delay before a monster is able to calculate a new path.
        int timesincelastcalc = 0;
        int movebreak = 100;

        //Time delay before the player is able to take damage from a monster again.
        int timesincedamaged = 1250;
        int damagewaittime = 1250;

        private List<HealthBuff> healthitems = new List<HealthBuff>();
        private List<SpeedBuff> speeditems = new List<SpeedBuff>();
        
        //Time delay before an item is spawned on the map.
        int timesincelastitem = 0;
        int itemwaittime = 20000;
        int itemsonmap;

        private List<Weapon> weapons = new List<Weapon>();
        private List<Bullets> bullets = new List<Bullets>();
        KeyboardState previouskey;


        enum direction
        {
            north,
            east,
            south,
            west
        }
        #endregion

        #region methods
        /// <summary>
        /// Places ones in the maze array to act as walls that will be broken in the recursive maze generation algorithm.
        /// Calls the recursive maze generation algorithm after it has placed all the ones in the array.
        /// </summary>
        public void initializeMaze()
        {
            for (int x = 0; x < maze.GetLength(1); x++)
                for (int y = 0; y < maze.GetLength(0); y++)
                {
                    maze[y, x] = 1;
                }

            GenerateMaze(1, 1);
        }


        /// <summary>
        /// Recursive Maze Generation algorithm that will generate a random maze when it is called.
        /// Will leave the outer edges of the maze untouched so the player cannot go outside the maze.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GenerateMaze(int x, int y)
        {
            Random RNG = new Random();
            
            //Removes the wall of the starting point.
            maze[y, x] = 0;
            
            //List of directions that we are able to create a path by breaking walls without going outside the boundaries.
            List<direction> options = new List<direction>();

            do
            {
                options.Clear();
                
                //Checks all directions around the current position to see if we are able to make a valid path and adds to the list of valid directions.
                if (x < mapwidth - 2)
                {
                    if (maze[y, x + 2] == 1) options.Add(direction.west);
                }

                try
                { if (maze[y, x - 2] == 1) options.Add(direction.east); }
                catch { }

                try
                { if (maze[y - 2, x] == 1) options.Add(direction.north); }
                catch { }

                if (y < mapheight - 3)
                { if (maze[y + 2, x] == 1) options.Add(direction.south); }

                //If there are valid paths a random path will be created by breaking the walls in the direction selected.
                if (options.Count > 0)
                {
                    int choice = RNG.Next(0, options.Count);
                    switch (options[choice])
                    {
                        case direction.north:
                            maze[y - 1, x] = 0;
                            maze[y - 2, x] = 0;
                            GenerateMaze(x, y - 2);
                            break;
                        case direction.east:
                            maze[y, x - 1] = 0;
                            maze[y, x - 2] = 0;
                            GenerateMaze(x - 2, y);
                            break;
                        case direction.south:
                            maze[y + 1, x] = 0;
                            maze[y + 2, x] = 0;
                            GenerateMaze(x, y + 2);
                            break;
                        case direction.west:
                            maze[y, x + 1] = 0;
                            maze[y, x + 2] = 0;
                            GenerateMaze(x + 2, y);
                            break;
                        default:
                            break;
                    }
                }


            } while (options.Count > 0);
            //Algorithm will keep running until all the possible paths have been created.


        }





        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _game = game;
        }

        /// <summary>
        /// Creates the maze.
        /// Initializes the player.
        /// Spawns the monsters onto the map.
        /// Initializes the camera that will follow the player.
        /// </summary>
        public override void Initialize()
        {
            
            map = new Map();
            initializeMaze();

            player = new Player(maze);
            SpawnMonsters();
            GameCamera = new Camera2D(_graphicsDevice.Viewport);
            CameraOffSet = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);

        }

        /// <summary>
        /// Loads the textures of the player and walls.
        /// Loads the font that will be used for text.
        /// </summary>
        public override void LoadContent()
        {
            Font = _content.Load<SpriteFont>("myfont");
            Tiles.Content = _content;
            player.LoadContent(_content);
            map.Generate(maze, pixelsize);
        }

        public override void Update(GameTime gameTime)
        {
            //Increases the timer.
            timer += gameTime.ElapsedGameTime.Milliseconds;
            KeyboardState ks = Keyboard.GetState();
            
            
            if (ks.IsKeyDown(Keys.P))
            {
                initializeMaze();
                map.Generate(maze, pixelsize);
            }

            //Checks the game has not finished.
            IsGameFinish();

            //Checks and removes walls on the map where their health has fallen below zero.
            for (int i = 0; i < map.CollisionTiles.Count-1; i++)
            {
                if (map.CollisionTiles[i].GetHealth() <= 0)
                {
                    maze[map.CollisionTiles[i].Rectangle.Y / pixelsize, map.CollisionTiles[i].Rectangle.X / pixelsize] = 0;
                    map.CollisionTiles.RemoveAt(i);
                }
            }


            player.Update(gameTime,map.CollisionTiles);

            GameCamera.Pos = player.Location - CameraOffSet;
            GameCamera.Update();

            //Every monster checks whether is it able to see the player and whether a path will need to be calculated.
            foreach (monster m in monsterlist)
                m.sensePlayer(player, maze);

            timesincelastcalc += gameTime.ElapsedGameTime.Milliseconds;

            //If the time delay for calculating a path is reached then a path will be calculated for monsters that can see the player.
            if (timesincelastcalc >= movebreak)
            {
                foreach (monster m in monsterlist)
                    if(m.GetMoveStatus() == false) 
                        m.AStar(player, maze);

                timesincelastcalc = 0;
            }


            timesincedamaged += gameTime.ElapsedGameTime.Milliseconds;
            
            //Checks whether a monster has intersected with a player and whether the player needs to take damage.
            foreach (monster m in monsterlist)
            {
                m.Update(gameTime);
                if (m.Edge.Intersects(player.Edge))
                {
                    if(timesincedamaged >= damagewaittime)
                    {
                        player.takeDamage(m.GetDamage());
                        timesincedamaged = 0;
                    }
                       

                }
                //If a wallbreaker intersects with a wall then it will damage the wall and break it.
                if (m.NeedToBreakWall())
                    foreach (CollisionTiles t in map.CollisionTiles)
                        if (m.Edge.Intersects(t.Rectangle))
                            t.takeDamage(m.GetDamage());
            }
            
            //Checks the list of monsters and removes any that have been killed.
            monsterlist = monsterlist.Where(m => m.GetAlive() == true).ToList();

            //If no monsters are left and the delay before a new wave is reached then new monsters will be spawned in.
            if(monsterlist.Count == 0)
            {
                timeonbreak += gameTime.ElapsedGameTime.Milliseconds;
                if(timeonbreak == wavebreak)
                {
                    wave++;
                    monsterstobespawned++;
                    SpawnMonsters();
                    timeonbreak = 0;
                }
            }



            timesincelastitem += gameTime.ElapsedGameTime.Milliseconds;
            
            //If there are less than 10 items on the map and the time delay before an item is spawned is reached then an item will be randomly selected and spawned.
            if (itemsonmap < 10)
            {
                if (timesincelastitem >= itemwaittime)
                {
                    int choice = RNG.Next(0, 2);

                    if (choice == 0)
                    {
                        HealthBuff hp = new HealthBuff(maze);
                        hp.LoadContent(_content);
                        healthitems.Add(hp);
                    }
                    else if (choice == 1)
                    {
                        SpeedBuff speed = new SpeedBuff(maze);
                        speed.LoadContent(_content);
                        speeditems.Add(speed);
                    }
                    timesincelastitem = 0;
                }
            }

            //Checks whether the player has intersected with a health item and whether their health will need to be increased.
            foreach (HealthBuff h in healthitems)
            {
                if (h.Edge.Intersects(player.Edge))
                {
                    player.gainhealth(h);
                    h.touched = true;
                }
            }

            //Removes any health items that have been picked up and used.
            healthitems = healthitems.Where(h => h.touched == false).ToList();

            //Checks whether the player has intersected with a speed item and whether their speed will need to be increased.
            foreach (SpeedBuff s in speeditems)
                if (s.Edge.Intersects(player.Edge))
                {
                    player.gainspeed(s);
                    s.touched = true;
                }

            //Removes any speed items that have been picked up and used.
            speeditems = speeditems.Where(s => s.touched == false).ToList();
            
            //Counts how many items have currently been spawned and haven't been picked up.
            itemsonmap = healthitems.Count() + speeditems.Count();

            //If there are less than 5 weapons on the map then a weapon will be randomly selected and spawned.
            if (weapons.Count < 5)
            {
                Weapon p; 
                int randint = RNG.Next(3);
                if (randint == 1)
                    p = new Weapon(maze, "RayGun");
                else if (randint == 2)
                    p = new Weapon(maze, "pistol");
                else p = new Weapon(maze, "Laser");
                p.LoadContent(_content);
                weapons.Add(p);
            }

            //Checks whether a player has picked up and equipped a weapon and changes the position of the weapon sprite so player can see equipped weapon.
            foreach (Weapon p in weapons)
                if (p.Edge.Intersects(player.Edge) && ks.IsKeyDown(Keys.E))
                {
                    player.equippedWeapon = p;
                    p.equipped = true;
                    p.Location = new Vector2(325, 0);
                }
            
            //Removes weapons from that map that have been picked up and equipped.
            weapons = weapons.Where(p => p.equipped == false).ToList();
            
            //If the player has a weapon equipped then it will shoot if they press space.
            if (player.equippedWeapon != null)
                if (ks.IsKeyDown(Keys.Space) && previouskey.IsKeyUp(Keys.Space))
                    Shoot();
            previouskey = ks;
            UpdateBullets(gameTime);
        }

        /// <summary>
        /// Moves all the bullets that have been shot.
        /// Checks whether the bullets that have been shot have collided with a wall or monster.
        /// Checks whether the bullets have reached their max range.
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateBullets(GameTime gameTime)
        {
            foreach (Bullets b in bullets)
            {
                b.Update(gameTime);
                
                //If bullets have reached the max range then it will not be visible.
                if (Vector2.Distance(b.Location, player.Location) > 500)
                    b.isVisible = false;
                
                foreach (CollisionTiles t in map.CollisionTiles)
                    if (b.Edge.Intersects(t.Rectangle))
                    {
                        //If a raygun bullet has hit a wall then it will damage the wall.
                        if (b.type == "RayGun")
                            t.takeDamage(b.damage);
                        //Nothing will happen if a laser bullet hits a wall as it can go through them.
                        else if (b.type == "Laser")
                            continue;
                        b.isVisible = false;
                        //Any bullet that isn't from a laser will not be visible when it has hit a wall.
                    }
                
                //Checks whether any bullets have intersected with any monsters and whether they will need to take damage.
                foreach(monster m in monsterlist)
                    if (b.Edge.Intersects(m.Edge))
                    {
                        b.isVisible = false;
                        m.TakeDamage(b.damage);
                    }
                
            }

            //Removes any bullets from the bullets list that are not visible.
            for (int i = 0; i < bullets.Count; i++)
            {

                if (!bullets[i].isVisible)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// When the player shoots their weapon then a new bullet of that weapon's type is created.
        /// Assigns the direction that the bullet will be moved in.
        /// </summary>
        public void Shoot()
        {
            Bullets newBullet;
            if (player.equippedWeapon.type == "pistol")
                newBullet = new Bullets(_content.Load<Texture2D>("redbullet"), "pistol");
            else if(player.equippedWeapon.type == "RayGun")
                newBullet = new Bullets(_content.Load<Texture2D>("greenbullet"), "RayGun");
            else newBullet = new Bullets(_content.Load<Texture2D>("bluebullet"), "Laser");
            switch (player.direction)
            {
                case Player.Direction.down:
                    newBullet.Velocity = new Vector2(0, player.speed + 2);
                    break;
                case Player.Direction.left:
                    newBullet.Velocity = new Vector2(-(player.speed + 2), 0);
                    break;
                case Player.Direction.right:
                    newBullet.Velocity = new Vector2(player.speed + 2, 0);
                    break;
                case Player.Direction.up:
                    newBullet.Velocity = new Vector2(0, -(player.speed + 2));
                    break;
                default:
                    break;
            }
            newBullet.Location.X = player.Location.X + newBullet.Velocity.X * 5;
            newBullet.Location.Y = player.Location.Y + 18 + newBullet.Velocity.Y * 5;
            newBullet.isVisible = true;

            //Ensures that no more than 20 bullets can be shot at any one time.
            if (bullets.Count() < 20)
                bullets.Add(newBullet);


        }

        /// <summary>
        /// If the limit to the monsters to be spawned have not been reached then a monster will be randomly selected and spawned.
        /// </summary>
        public void SpawnMonsters()
        {
            for (int i = 0; i < monsterstobespawned; i++)
            {
                int randint = RNG.Next(3);
                if (randint == 0)
                {
                    monster newmonster = new monster(maze);
                    newmonster.LoadContent(_content);
                    monsterlist.Add(newmonster);
                }
                else if(randint == 1)
                {
                    Phantom newphantom = new Phantom(maze);
                    newphantom.LoadContent(_content);
                    monsterlist.Add(newphantom);
                }
                else if(randint == 2)
                {
                    WallBreaker newwallBreaker = new WallBreaker(maze);
                    newwallBreaker.LoadContent(_content);
                    monsterlist.Add(newwallBreaker);
                }
            }
        }

        /// <summary>
        /// Checks whether the player has not died.
        /// If the player has died then the game will go to the end screen with the game stats to be displayed.
        /// </summary>
        public void IsGameFinish()
        {
            if (player.health <= 0) _game.ChangeState(new EndState(_game, _graphicsDevice, _content, wave, timer));
        }

        /// <summary>
        /// Draws all the sprites in the game.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="spriteBatch"></param>
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: GameCamera.Transform);
            map.Draw(spriteBatch);
            foreach (HealthBuff h in healthitems)
                h.Draw(spriteBatch);
            foreach (SpeedBuff s in speeditems)
                s.Draw(spriteBatch);
            foreach (Weapon p in weapons)
                p.Draw(spriteBatch);
            foreach (Bullets b in bullets)
                b.Draw(spriteBatch);
            player.Draw(spriteBatch);
            foreach (monster m in monsterlist)
                m.Draw(spriteBatch);
            spriteBatch.End();


            spriteBatch.Begin();
            spriteBatch.DrawString(Font, "Health: " + player.health, new Vector2(1, 0), Color.White);
            spriteBatch.DrawString(Font, (timer/1000).ToString(), new Vector2(245, 0), Color.White);
            spriteBatch.DrawString(Font, "Speed: " + player.speed, new Vector2(145, 0), Color.White);
            if (player.equippedWeapon != null)
                player.equippedWeapon.Draw(spriteBatch);
            spriteBatch.DrawString(Font, "X: " + player.Location.X + " Y: " + (player.Location.Y), new Vector2(1, 20), Color.White);
            spriteBatch.DrawString(Font, "Wave: " + wave, new Vector2(400, 0), Color.White);
            if(monsterlist.Count >= 0) spriteBatch.DrawString(Font, "Monsters Remaining: " + monsterlist.Count, new Vector2(500, 0), Color.White);
            if (timeonbreak != 0)
                spriteBatch.DrawString(Font, "Time until next wave: " + (int)((wavebreak - timeonbreak)/1000), new Vector2(750, 0), Color.White);
            spriteBatch.End();


        }
        #endregion


    }
}
