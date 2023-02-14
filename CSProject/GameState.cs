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
        Random RNG = new Random();

        public static int width = 3840;
        public static int height = 2160;

        Camera2D GameCamera;
        Vector2 CameraOffSet;

        Map map;
        public static int pixelsize = 64;
        public static int mapheight = (int)(height / pixelsize) + 2;
        public static int mapwidth = (int)(width / pixelsize) + 3;

        public int[,] maze = new int[mapheight, mapwidth];

        private int timer = 0;



        SpriteFont Font;

        Player player;

        private int wave = 1;


        int multiplier = 1;
        private List<monster> monsterlist = new List<monster>();
        int monsterstobespawned = 3;
        int wavebreak = 10000;
        int timeonbreak = 0;

        int timesincelastcalc = 0;
        int movebreak = 100;

        int timesincedamaged = 1250;
        int damagewaittime = 1250;

        private List<HealthBuff> healthitems = new List<HealthBuff>();
        private List<SpeedBuff> speeditems = new List<SpeedBuff>();
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
        public void initializeMaze()
        {
            for (int x = 0; x < maze.GetLength(1); x++)
                for (int y = 0; y < maze.GetLength(0); y++)
                {
                    maze[y, x] = 1;
                }

            GenerateMaze(1, 1);
        }



        public void GenerateMaze(int x, int y)
        {
            Random RNG = new Random();
            maze[y, x] = 0;
            List<direction> options = new List<direction>();

            do
            {
                options.Clear();
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



        }





        public GameState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _game = game;
        }

        public override void Initialize()
        {
            
            map = new Map();
            initializeMaze();

            player = new Player(maze);
            SpawnMonsters();
            GameCamera = new Camera2D(_graphicsDevice.Viewport);
            CameraOffSet = new Vector2(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);

        }
        public override void LoadContent()
        {
            Font = _content.Load<SpriteFont>("myfont");
            Tiles.Content = _content;
            player.LoadContent(_content);
            map.Generate(maze, pixelsize);
        }
        public override void Update(GameTime gameTime)
        {
            timer += gameTime.ElapsedGameTime.Milliseconds;
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.P))
            {
                initializeMaze();
                map.Generate(maze, pixelsize);
            }

            IsGameFinish();

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

            foreach (monster m in monsterlist)
                m.sensePlayer(player, maze);

            timesincelastcalc += gameTime.ElapsedGameTime.Milliseconds;

            if (timesincelastcalc >= movebreak)
            {
                foreach (monster m in monsterlist)
                    if(m.GetMoveStatus() == false) 
                        m.AStar(player, maze);

                timesincelastcalc = 0;
            }



            timesincedamaged += gameTime.ElapsedGameTime.Milliseconds;
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
                if (m.NeedToBreakWall())
                    foreach (CollisionTiles t in map.CollisionTiles)
                        if (m.Edge.Intersects(t.Rectangle))
                            t.takeDamage(m.GetDamage());
            }
            monsterlist = monsterlist.Where(m => m.GetAlive() == true).ToList();

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

            foreach (HealthBuff h in healthitems)
            {
                if (h.Edge.Intersects(player.Edge))
                {
                    player.gainhealth(h);
                    h.touched = true;
                }
            }
            healthitems = healthitems.Where(h => h.touched == false).ToList();
            foreach (SpeedBuff s in speeditems)
                if (s.Edge.Intersects(player.Edge))
                {
                    player.gainspeed(s);
                    s.touched = true;
                }
            speeditems = speeditems.Where(s => s.touched == false).ToList();
            itemsonmap = healthitems.Count() + speeditems.Count();


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

            foreach (Weapon p in weapons)
                if (p.Edge.Intersects(player.Edge) && ks.IsKeyDown(Keys.E))
                {
                    player.equippedWeapon = p;
                    p.equipped = true;
                    p.Location = new Vector2(325, 0);
                }
            weapons = weapons.Where(p => p.equipped == false).ToList();
            if (player.equippedWeapon != null)
                if (ks.IsKeyDown(Keys.Space) && previouskey.IsKeyUp(Keys.Space))
                    Shoot();
            previouskey = ks;
            UpdateBullets(gameTime);
        }

        public void UpdateBullets(GameTime gameTime)
        {
            foreach (Bullets b in bullets)
            {
                b.Update(gameTime);
                if (Vector2.Distance(b.Location, player.Location) > 500)
                    b.isVisible = false;
                foreach (CollisionTiles t in map.CollisionTiles)
                    if (b.Edge.Intersects(t.Rectangle))
                    {
                        if (b.type == "RayGun")
                            t.takeDamage(b.damage);
                        else if (b.type == "Laser")
                            continue;
                        b.isVisible = false;
                    }
                foreach(monster m in monsterlist)
                    if (b.Edge.Intersects(m.Edge))
                    {
                        b.isVisible = false;
                        m.TakeDamage(b.damage);
                    }
                
            }

            for (int i = 0; i < bullets.Count; i++)
            {

                if (!bullets[i].isVisible)
                {
                    bullets.RemoveAt(i);
                    i--;
                }
            }
        }

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

            if (bullets.Count() < 20)
                bullets.Add(newBullet);


        }

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

        public void IsGameFinish()
        {
            if (player.health <= 0) _game.ChangeState(new EndState(_game, _graphicsDevice, _content, wave, timer));
        }

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

        
    }
}
