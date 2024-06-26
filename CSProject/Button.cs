﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace CSProject
{
    class Button
    {
        #region fields
        private MouseState _currentMouse;

        private MouseState _previousMouse;

        private SpriteFont _font;

        private bool _isHovering;

        private Texture2D _texture;


        public event EventHandler Click;

        public bool Clicked { get; private set; }

        public Color TextColour { get; set; }

        public Vector2 Position { get; set; }


        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);    
            }
        }

        public string Text { get; set; }

        #endregion

        #region methods
        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
            TextColour = Color.Black;
        }




        public void Update(GameTime gameTime)
        {
            _previousMouse = _currentMouse;
            _currentMouse = Mouse.GetState();

            Rectangle mouseRectangle = new Rectangle(_currentMouse.X, _currentMouse.Y, 1, 1);

            _isHovering= false;

            //Checks whether a mouse is hovering over the button and whether it had been clicked.
            if (mouseRectangle.Intersects(Rectangle))
            {
                _isHovering= true;

                if(_currentMouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }



        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
           Color colour = Color.White;

            //highlights the button if user is hovering over it.
            if (_isHovering)
                colour = Color.Gray;

            spriteBatch.Draw(_texture,Rectangle,colour);

            //Adds the text to the button and centres it.
            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width/ 2) - (_font.MeasureString(Text).X/2));
                var y = (Rectangle.Y + (Rectangle.Height / 2) - (_font.MeasureString(Text).Y / 2));

                spriteBatch.DrawString(_font, Text, new Vector2(x, y), TextColour);
            }
        }

        #endregion
    }
}
