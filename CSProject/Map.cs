using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CSProject
{
    internal class Map
    {
        private List<CollisionTiles> collisionTiles = new List<CollisionTiles>();
        public List<CollisionTiles> CollisionTiles
        {
            get { return collisionTiles; }

        }

        private int width,height;
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; } 
        }

        public Map() { }

        /// <summary>
        /// Goes through the map and places each collisiontile (the walls) where there is a 1.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="size"></param>
        public void Generate(int[,] map , int size)
        {
            collisionTiles.Clear();
            for (int x = 0; x <  map.GetLength(1); x++)
               for (int y = 0; y < map.GetLength(0); y++)     
                {
                    int number = map[y, x];
                    if (number > 0)
                        collisionTiles.Add(new CollisionTiles(number, new Rectangle(x * size, y * size, size, size)));

                    width = (x+1) * size;
                    height = (y+1) * size;
                }
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (CollisionTiles tile in collisionTiles)
            {
                tile.Draw(spriteBatch); 
            }
        }

    }
}
