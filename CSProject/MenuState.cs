using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSProject
{
    public class MenuState : State
    {
        #region fields
        SpriteFont Font;
        SpriteFont TitleFont;
        Button startButton;
        Button HowtoPlayButton;
        Button QuitButton;
        #endregion

        #region methods
        public MenuState(Game1 game, GraphicsDevice graphicsDevice, ContentManager content) : base(game, graphicsDevice, content)
        {
            _content = content;
            _graphicsDevice = graphicsDevice;
            _game = game;
        }

        public override void Initialize()
        {
            
        }

        /// <summary>
        /// Loads all the fonts and textures for buttons and initialises the buttons.
        /// </summary>
        public override void LoadContent()
        {
            Font = _content.Load<SpriteFont>("myfont");
            TitleFont = _content.Load<SpriteFont>("TitleFont");

            //Initializes the start game button with its postion, text and font.
            startButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth/2 - 75, Game1.screenheight/2),
                Text = "Start",
            };

            //Calls the event that will happen when the start game button is clicked.
            startButton.Click += StartButton_Click;

            //Initializes the how to play button with its postion, text and font.

            HowtoPlayButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth / 2 - 75, Game1.screenheight / 2 + 75),
                Text = "Help",
            };

            //Calls the event that will happen when the how to play button is clicked.
            HowtoPlayButton.Click += HowtoPlayButton_Click;

            //Initializes the quit game button with its postion, text and font.
            QuitButton = new Button(_content.Load<Texture2D>("Button"), Font)
            {
                Position = new Vector2(Game1.screenwidth / 2 - 75, Game1.screenheight / 2 + 150),
                Text = "Quit",
            };

            //Calls the event that will happen when the quit game button is clicked.
            QuitButton.Click += QuitButton_Click;
        }

        /// <summary>
        /// Changes the to the help screen when the how to play button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HowtoPlayButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new HowToPlayState(_game, _graphicsDevice, _content));
        }

        /// <summary>
        /// Starts the game when the start game button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, EventArgs e)
        {
            _game.ChangeState(new GameState(_game, _graphicsDevice, _content));
        }

        /// <summary>
        /// Clsoes the game when the quit game button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuitButton_Click(object sender, EventArgs e)
        {
            _game.Exit();
        }



        public override void Update(GameTime gameTime)
        {
            startButton.Update(gameTime);
            HowtoPlayButton.Update(gameTime);
            QuitButton.Update(gameTime);
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            startButton.Draw(gameTime, spriteBatch);
            HowtoPlayButton.Draw(gameTime, spriteBatch);
            QuitButton.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(TitleFont, "SURVIVE THE MAZE", new Vector2(Game1.screenwidth / 5, Game1.screenheight / 4), Color.Red);
            spriteBatch.End();
        }
        #endregion

    }
}
