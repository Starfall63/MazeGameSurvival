using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CSProject
{

     class Phantom : monster
     {



        Vector2 newLocation;

        /// <summary>
        /// Constructor for the phantom.
        /// Spawns the monster in a random location in the maze making sure that it does not spawn in the walls.
        /// </summary>
        /// <param name="maze"></param>
        public Phantom(int[,] maze) : base(maze)
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
            _alive = true;
            currentcolour = Color.Blue;
            Location = new Vector2(xspawn * GameState.pixelsize, yspawn * GameState.pixelsize);

        }

        /// <summary>
        /// Loads the sprite texture of the phantom.
        /// </summary>
        /// <param name="Content"></param>
        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            Texture = Content.Load<Texture2D>(@"ghost64");
        }


        public override void Update(GameTime gameTime)
        {

            Edge = new Rectangle((int)Location.X, (int)Location.Y, 32, 32);


            if (_moving)
                Move();




            if (path.Any() && !_moving)
            {
                setDestination();
                _moving = true;
            }

            if (health <= 0) _alive = false;
            currentcolour = Color.Blue;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Location, currentcolour);
        }



        private void Move()
        {
            if (Location.X < newLocation.X) Location.X += _speed;
            else if (Location.X > newLocation.X) Location.X -= _speed;
            else if (Location.Y < newLocation.Y) Location.Y += _speed;
            else if (Location.Y > newLocation.Y) Location.Y -= _speed;
            else _moving = false;
        }

        private void setDestination()
        {
            newLocation.X = path.Peek().X * 64;
            newLocation.Y = path.Pop().Y * 64;
        }


        public override void AStar(Player player, int[,] maze)
        {
            if (!_playerSeen) return;
            path.Clear();
            var start = new AStarNode();
            start.X = (int)Location.X / 64;
            start.Y = (int)Location.Y / 64;

            var target = new AStarNode();
            target.X = (int)player.Location.X / 64;
            target.Y = (int)player.Location.Y / 64;

            start.HeuristicDistance(target.X, target.Y);

            var activenodes = new List<AStarNode>();
            activenodes.Add(start);
            var visitednodes = new List<AStarNode>();

            while (activenodes.Any())
            {
                var checkNode = activenodes.OrderBy(x => x.CostDistance).First();


                if (checkNode.X == target.X && checkNode.Y == target.Y)
                {
                    var node = checkNode;

                    while (true)
                    {
                       
                       path.Push(node);
                        
                        node = node.Parent;
                        if (node == null)
                            return;
                    }
                }


                visitednodes.Add(checkNode);
                activenodes.Remove(checkNode);

                var validnodes = GetValidNodes(maze, checkNode, target);

                foreach (var validnode in validnodes)
                {
                  

                    if (activenodes.Any(x => x.X == validnode.X && x.Y == validnode.Y))
                    {
                        var existingnode = activenodes.First(x => x.X == validnode.X && x.Y == validnode.Y);
                        if (existingnode.CostDistance > checkNode.CostDistance)
                        {
                            activenodes.Remove(existingnode);
                            activenodes.Add(validnode);
                        }
                    }
                    else
                    {
                        activenodes.Add(validnode);
                    }
                }
            }

        }




        /// <summary>
        /// Helper method for the A star algorithm.
        /// Takes in an A star node and the map.
        /// Checks every node around to make sure it is a valid node to be travelled to.
        /// Checks by checking whether the nodes around it are outside the maze.
        /// Will not check for walls as phantoms can move through walls.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="currentnode"></param>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        private static List<AStarNode> GetValidNodes(int[,] map, AStarNode currentnode, AStarNode targetNode)
        {
            var possiblenodes = new List<AStarNode>()
            {
                new AStarNode{X = currentnode.X, Y = currentnode.Y-1, Parent = currentnode, Cost = currentnode.Cost+1},
                new AStarNode{X = currentnode.X, Y = currentnode.Y+1, Parent = currentnode, Cost = currentnode.Cost+1},
                new AStarNode{X = currentnode.X-1, Y = currentnode.Y, Parent = currentnode, Cost = currentnode.Cost+1},
                new AStarNode{X = currentnode.X+1, Y = currentnode.Y, Parent = currentnode, Cost = currentnode.Cost+1},
            };

            var maxX = map.GetLength(1) - 1;
            var maxY = map.GetLength(0) - 1;

            return possiblenodes
                .Where(node => node.X >= 0 && node.X <= maxX)
                .Where(node => node.Y >= 0 && node.Y <= maxY)
                .ToList();

        }

        /// <summary>
        /// Checks every tile around the phantom up to 5 away to see if a player is within range to be attacked.
        /// Will stop checking after going out of range and will not stop if it reaches a wall because a phantom can move through walls.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="maze"></param>
        public override void sensePlayer(Player player, int[,] maze)
        {
            if (_playerSeen) return;
            int playerx = (int)player.Location.X / 64;
            int playery = (int)player.Location.Y / 64;

            int currentx = (int)Location.X / 64;
            int currenty = (int)Location.Y / 64;


            for (int i = currentx-5; i < currentx + 5; i++)
            {
                for (int j = currenty -5 ; j < currenty + 5; j++)
                {
                    if (i == currentx && j == currenty) continue;
                    else if (i <= 0) continue;
                    if(i == playerx && j == playery)
                    {
                        _playerSeen = true;
                        break;
                    }
                }
            }
            
            
            
            
            


        }
    }
}
