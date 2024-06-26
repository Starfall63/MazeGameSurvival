﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CSProject
{

    class HowToPlayState:State
    {
        SpriteFont Font;
        Button backButton;
        string howtoplay;
        string controls;
        string weapons;
        public HowToPlayState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _game = game;
        }

        public override void Initialize()
        {

        }

        /// <summary>
        /// Loads the fonts to be used.
        /// Loads the text that will be shown onscreen.
        /// Initilizes any buttons that will be used.
        /// </summary>
        public override void LoadContent()
        {
            Font = _content.Load<SpriteFont>("myfont");

            //Initialzes the back button with its font, text and positon.
            backButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(10, 10),
                Text = "Back",
            };

            //Calls the event that will happen when the back button is clicked.
            backButton.Click += backButton_Click;


            howtoplay = "To play the game you will have to navigate the maze and kill all the monsters in each wave.\n" +
                "There are 3 different monsters that are able to spawn and each wave will spawn in more monsters.\n\n" +
                "The Regular Monster: Cannot break walls and will only move when it has direct sight of you. (White)\n" +
                "The Phantom: Can move through walls and can detect you through walls.(Blue)\n" +
                "The WallEater: Can break walls and can detect you through walls.(Yellow)";
            controls = "Controls:\n" +
                "Up : W\n" +
                "Left: A\n" +
                "Down: S\n" +
                "Right: D\n" +
                "Pick Up Weapon: E\n" +
                "Shoot: Spacebar";
            weapons = "Weapons:\n" +
                "Pistol: Regular Weapon does not shoot through walls\n" +
                "Ray Gun: Can break walls\n" +
                "Laser: Can shoot through walls";

        }

        /// <summary>
        /// Goes back to the starting menu screen when the back button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new MenuState(_game, _graphicsDevice, _content));
        }



        public override void Update(GameTime gameTime)
        {
            backButton.Update(gameTime);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            backButton.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(Font, "How To Play", new Vector2(Game1.screenwidth / 3+75, 15), Color.WhiteSmoke);
            spriteBatch.DrawString(Font, howtoplay,new Vector2( Game1.screenwidth/9, Game1.screenheight / 6),Color.White);
            spriteBatch.DrawString(Font, controls, new Vector2(Game1.screenwidth / 8, Game1.screenheight / 3+75), Color.White);
            spriteBatch.DrawString(Font, weapons, new Vector2(Game1.screenwidth / 2, Game1.screenheight / 3 + 75), Color.White);
            spriteBatch.End();
        }
    }
}
