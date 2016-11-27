﻿using Gnn.Visual.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gnn.Visual {

    public class MainGame : Game {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private World World { get; }

        #region FPS counter
        private int CurSecondNr = -1;
        private int CurFrameCount = 1;
        private int FPS = -1;
        #endregion FPS counter

        public Cam2D Cam { get; private set; }
        public MainGameContent Res { get; private set; }
        public Random Rand = new Random(4);

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

            World = new World(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            Res = new MainGameContent();
            DrawHelper.Init(GraphicsDevice);
            Cam = new Cam2D(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

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
            World.Initialize();
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
            var mState = Mouse.GetState();
            var kState = Keyboard.GetState();

            if(kState.IsKeyDown(Keys.Escape)) {
                Exit();
            }

            Cam.Move(mState, kState);
            var mPosRelative = Vector2.Transform(mState.Position.ToVector2(), Matrix.Invert(Cam.Transform)).ToPoint();

            World.Update(mState, kState, mPosRelative, (float)gameTime.ElapsedGameTime.TotalSeconds / 1.0f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            DrawStatic(gameTime);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: Cam.Transform);
            DrawRelative(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawStatic(GameTime gameTime) {
            DrawFps(gameTime);
        }

        private void DrawRelative(GameTime gameTime) {
            World.Draw(spriteBatch);
        }

        private void DrawFps(GameTime gt) {
            if(gt.TotalGameTime.Seconds == CurSecondNr) {
                CurFrameCount++;
            } else {
                CurSecondNr = gt.TotalGameTime.Seconds;
                FPS = CurFrameCount;
                CurFrameCount = 1;
            }

            var col = FPS >= 59 ? Color.Black : Color.DarkRed;

            spriteBatch.DrawString(Res.FConsolas, $"{FPS} FPS", Vector2.Zero, col);
        }
    }
}