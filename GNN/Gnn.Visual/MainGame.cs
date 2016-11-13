using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Gnn.Visual {

    public class MainGame : Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private MainGameContent Res { get; set; }

        #region FPS counter
        private int CurSecondNr = -1;
        private int CurFrameCount = 1;
        private int FPS = -1;
        #endregion FPS counter

        public MainGame() {
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 768,
                SynchronizeWithVerticalRetrace = true,
            };
            Content.RootDirectory = "Content";

            IsFixedTimeStep = false;
            IsMouseVisible = true;

            Window.IsBorderless = false;
            Window.Title = "GNN";

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += (s1, a1) => {
                if(graphics.PreferredBackBufferWidth != Window.ClientBounds.Width || graphics.PreferredBackBufferHeight != Window.ClientBounds.Height) {
                    graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                    graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
                    graphics.ApplyChanges();
                }
            };
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            Res = new MainGameContent();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Res.Init(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            if(Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            DrawFps(gameTime, spriteBatch);

            spriteBatch.Draw(Res.Test, new Rectangle(100, 100, 150, 150), null, Color.White, 0.1f, new Vector2(), SpriteEffects.None, 1f);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawFps(GameTime gt, SpriteBatch sb) {
            if(gt.TotalGameTime.Seconds == CurSecondNr) {
                CurFrameCount++;
            } else {
                CurSecondNr = gt.TotalGameTime.Seconds;
                FPS = CurFrameCount;
                CurFrameCount = 1;
            }

            sb.DrawString(Res.Consolas, $"{FPS} FPS", new Vector2(0, 0), Color.Black);
        }
    }
}