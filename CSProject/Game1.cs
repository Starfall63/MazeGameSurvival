using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//gitgubmactest6
namespace CSProject
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public const int screenwidth = 1366;
        public const int screenheight = 768;
        SpriteFont Font;

        private State _currentState;
        private State _nextState;

        public void ChangeState(State state)
        {
            _nextState = state;
        }


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Initializes the window width and height of the game screen.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = screenwidth;
            graphics.PreferredBackBufferHeight = screenheight;
            graphics.ApplyChanges();
            base.Initialize();
        }

        /// <summary>
        /// Sets the starting gamestate the to menustate.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _currentState = new MenuState(this, graphics.GraphicsDevice, Content);
            _currentState.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Changes the current gamestate the the nextstate and initializes all the content in the next state.
            if(_nextState != null)
            {
                _currentState = _nextState;
                _currentState.Initialize();
                _currentState.LoadContent();
                _nextState = null;
            }


            _currentState.Update(gameTime);



            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _currentState.Draw(gameTime,spriteBatch);
            base.Draw(gameTime);
        }
    }
}
