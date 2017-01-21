using Gnn.Visual.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Linq;

namespace Gnn.Visual {

    public class MainGame : Game {
        private const int InitScreenWidth = 1280;
        private const int InitScreenHeight = 768;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private World World { get; }

        private Keys[] PrevTickPressedKeys = new Keys[0];
        private Point? MouseDownPos = null;

        #region FPS counter
        private int CurSecondNr = -1;
        private int CurFrameCount = 1;
        private int FPS = -1;

        private int UpdateTimeMs = 0;
        private int DrawTimeMs = 0;
        private bool IsCpuThrottled = false;
        private bool IsGpuThrottled = false;
        #endregion FPS counter

        public Cam2D Cam { get; private set; }
        public MainGameContent Res { get; private set; }

        public bool FastMode { get; set; }
        public int TicksPerUpdate { get; set; } = 1;

        private bool ShowHelp { get; set; }

        public MainGame() {
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = InitScreenWidth,
                PreferredBackBufferHeight = InitScreenHeight,
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
            var startUpdate = Environment.TickCount;
            var mState = Mouse.GetState();
            var kState = Keyboard.GetState();

            HandleInput(mState, kState);

            var mPosRelative = Vector2.Transform(mState.Position.ToVector2(), Matrix.Invert(Cam.Transform)).ToPoint();

            if(!FastMode) {
                for(int t = 0; t < TicksPerUpdate; t++) {
                    World.Update(mState, kState, mPosRelative, (float)gameTime.ElapsedGameTime.TotalSeconds / 1.0f);
                }
            } else {
                const float inputElapsedTimeSec = 1f / 35f;
                const float preferredUpdateMethodDurationSec = 1f / 10f;

                float start = Stopwatch.GetTimestamp() / (float)Stopwatch.Frequency;
                var updateCount = 0;
                do {
                    World.Update(mState, kState, mPosRelative, inputElapsedTimeSec);
                    updateCount++;
                } while((Stopwatch.GetTimestamp() / (float)Stopwatch.Frequency) - start <= preferredUpdateMethodDurationSec);

                TicksPerUpdate = updateCount;
            }

            base.Update(gameTime);
            UpdateTimeMs += (Environment.TickCount - startUpdate);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            var startDraw = Environment.TickCount;
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: Cam.Transform);
            DrawRelative(gameTime);
            spriteBatch.End();

            spriteBatch.Begin();
            DrawStatic(gameTime);
            spriteBatch.End();

            base.Draw(gameTime);
            DrawTimeMs += (Environment.TickCount - startDraw);
        }

        private void DrawStatic(GameTime gameTime) {
            DrawInfo(gameTime);
            World.DrawStatic(spriteBatch);
        }

        private void DrawRelative(GameTime gameTime) {
            if(!FastMode) {
                World.DrawRelative(spriteBatch);
            }
        }

        private void DrawInfo(GameTime gt) {
            if(gt.TotalGameTime.Seconds == CurSecondNr) {
                CurFrameCount++;
            } else {
                IsCpuThrottled = UpdateTimeMs / CurFrameCount > 1000f / 60f;
                IsGpuThrottled = DrawTimeMs / CurFrameCount > 1000f / 60f;

                CurSecondNr = gt.TotalGameTime.Seconds;
                FPS = CurFrameCount;
                CurFrameCount = 1;
                DrawTimeMs = 0;
                UpdateTimeMs = 0;
            }

            var col = FPS >= 59 || FastMode ? Color.Black : Color.DarkRed;

            spriteBatch.DrawString(Res.FConsolas, $"(Help: F1) - {FPS:D2} FPS x{TicksPerUpdate}{(FastMode ? " [FASTMODE]" : "")}, rT={(int)gt.TotalGameTime.TotalSeconds}s", Vector2.Zero, col);

            if(IsCpuThrottled || IsGpuThrottled) {
                spriteBatch.DrawString(Res.FConsolas, $"{(IsCpuThrottled ? "[CPU] " : string.Empty)}{(IsGpuThrottled ? "[GPU]" : string.Empty)}", new Vector2(0, 10), col);
            }

            if(ShowHelp) {
                DrawHelper.DrawRect(spriteBatch, 0, 0, InitScreenWidth, InitScreenHeight, Color.LightGray);
                spriteBatch.DrawString(Res.FTrebuchet, "Plus / Minus to control speed\nCtrl + Plus / Minus for superspeed\n\nCam:\n    Drag w/ mouse to move cam\n    Scroll for zoom\n    Ctrl + Scroll for rotation\n    Ctrl + Space to reset\n\nClick a creature to follow it around\nSelect next creatures w\\ Ctrl + RightArrow", new Vector2(50, 50), Color.Black);
            }
        }

        private void HandleInput(MouseState mState, KeyboardState kState) {
            //This func returns true if key 'k' was pressed since last tick. Triggers once per key-up-down-up.
            Func<Keys, bool> keyWentDown = (k) => kState.IsKeyDown(k) && !PrevTickPressedKeys.Any(prev => prev == k);

            //This bool indicates LMB was pressed and is now released
            bool click = false;

            if(!MouseDownPos.HasValue && mState.LeftButton == ButtonState.Pressed) {
                MouseDownPos = mState.Position;
            }
            if(MouseDownPos.HasValue && mState.LeftButton == ButtonState.Released) {
                MouseDownPos = null;
                click = true;
            }

            if(keyWentDown(Keys.Escape)) {
                Exit();
            }

            Cam.HandleInput(mState, kState, click, keyWentDown, World);

            //Update speed control
            if(keyWentDown(Keys.Add)) {
                if(kState.IsKeyDown(Keys.LeftControl) || kState.IsKeyDown(Keys.RightControl)) {
                    FastMode = true;
                    TicksPerUpdate = 1;
                } else {
                    TicksPerUpdate++;
                }
            } else if(keyWentDown(Keys.Subtract)) {
                if(kState.IsKeyDown(Keys.LeftControl) || kState.IsKeyDown(Keys.RightControl)) {
                    FastMode = false;
                    TicksPerUpdate = 1;
                } else {
                    TicksPerUpdate = TicksPerUpdate > 0 ? TicksPerUpdate - 1 : 0;
                }
            }

            if(keyWentDown(Keys.F1)) {
                ShowHelp = !ShowHelp;
            }

            PrevTickPressedKeys = kState.GetPressedKeys();
        }
    }
}