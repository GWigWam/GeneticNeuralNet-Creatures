using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Gnn.Visual {

    public class Cam2D {
        private const int BaseArrowScrollSpeedPx = 20;

        private int previousScrollValue = 0;

        public Point Center;

        private float zoom = 1;
        public float Zoom {
            get { return zoom; }
            set { zoom = value <= 0 ? 0.01f : value; }
        }

        private float rotation = 0;
        public float Rotation {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }

        public int ViewWidth { get; set; }
        public int ViewHeight { get; set; }

        public Matrix Transform {
            get {
                return
                    Matrix.CreateTranslation(new Vector3(-Center.X, -Center.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new Vector3(ViewWidth * 0.5f, ViewHeight * 0.5f, 0));
            }
        }

        public Rectangle VisibleArea {
            get {
                var inverseViewMatrix = Matrix.Invert(Transform);
                var tl = Vector2.Transform(Vector2.Zero, inverseViewMatrix);
                var tr = Vector2.Transform(new Vector2(ViewWidth, 0), inverseViewMatrix);
                var bl = Vector2.Transform(new Vector2(0, ViewHeight), inverseViewMatrix);
                var br = Vector2.Transform(new Vector2(ViewWidth, ViewHeight), inverseViewMatrix);
                var min = new Vector2(
                    MathHelper.Min(tl.X, MathHelper.Min(tr.X, MathHelper.Min(bl.X, br.X))),
                    MathHelper.Min(tl.Y, MathHelper.Min(tr.Y, MathHelper.Min(bl.Y, br.Y))));
                var max = new Vector2(
                    MathHelper.Max(tl.X, MathHelper.Max(tr.X, MathHelper.Max(bl.X, br.X))),
                    MathHelper.Max(tl.Y, MathHelper.Max(tr.Y, MathHelper.Max(bl.Y, br.Y))));
                return new Rectangle((int)min.X, (int)min.Y, (int)(max.X - min.X), (int)(max.Y - min.Y));
            }
        }

        public Point? MouseDownPosTransformed { get; private set; }

        private GameObjects.Base.GameObject Following;

        public Cam2D(int viewWidth, int viewHeight, Point? centerPos = null) {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
            Center = centerPos ?? new Point(ViewWidth / 2, ViewHeight / 2);
        }

        public void HandleInput(MouseState mState, KeyboardState kState, bool click, Func<Keys, bool> keyWentDown, World world) {
            var mouseTransformed = Vector2.Transform(mState.Position.ToVector2(), Matrix.Invert(Transform)).ToPoint();

            if(!MouseDownPosTransformed.HasValue && mState.LeftButton == ButtonState.Pressed) {
                MouseDownPosTransformed = mouseTransformed;
            }
            if(MouseDownPosTransformed.HasValue && mState.LeftButton == ButtonState.Released) {
                MouseDownPosTransformed = null;
            }

            HandleSelection(click, mouseTransformed, kState, keyWentDown, world);
            HandleZoom(mState, kState);
            HandleClickDrag(mouseTransformed);
            HandleArrowScrolling(kState);
            HandleReset(kState);
        }

        private void HandleClickDrag(Point mouseTransformed) {
            if(MouseDownPosTransformed.HasValue) {
                Center += (MouseDownPosTransformed.Value - mouseTransformed);
            }
        }

        private void HandleReset(KeyboardState kState) {
            if(kState.IsKeyDown(Keys.Space) && kState.IsKeyDown(Keys.LeftControl)) {
                Rotation = 0;
                Zoom = 1;
                Center = new Point(ViewWidth / 2, ViewHeight / 2);
            }
        }

        private void HandleArrowScrolling(KeyboardState kState) {
            int pxPerDownTick = (int)(BaseArrowScrollSpeedPx / zoom);
            if(kState.IsKeyDown(Keys.Down)) {
                Center += new Point(0, pxPerDownTick);
            }
            if(kState.IsKeyDown(Keys.Up)) {
                Center += new Point(0, -pxPerDownTick);
            }
            if(kState.IsKeyDown(Keys.Right)) {
                Center += new Point(pxPerDownTick, 0);
            }
            if(kState.IsKeyDown(Keys.Left)) {
                Center += new Point(-pxPerDownTick, 0);
            }
        }

        private void HandleZoom(MouseState mState, KeyboardState kState) {
            if(mState.ScrollWheelValue < previousScrollValue) {
                if(kState.IsKeyDown(Keys.LeftControl) || kState.IsKeyDown(Keys.RightControl)) {
                    Rotation += 0.5f;
                } else {
                    Zoom *= 0.8f;
                }
                previousScrollValue = mState.ScrollWheelValue;
            } else if(mState.ScrollWheelValue > previousScrollValue) {
                if(kState.IsKeyDown(Keys.LeftControl) || kState.IsKeyDown(Keys.RightControl)) {
                    Rotation -= 0.5f;
                } else {
                    Zoom *= 1.25f;
                }
                previousScrollValue = mState.ScrollWheelValue;
            }
        }

        private void HandleSelection(bool click, Point mouseTransformed, KeyboardState kState, Func<Keys, bool> keyWentDown, World world) {
            if(Following != null) {
                if(!Following.Active) {
                    Following = null;
                } else {
                    Center = Following.CenterPosition.ToPoint();
                }
            }

            if(kState.IsKeyDown(Keys.LeftControl) || kState.IsKeyDown(Keys.RightControl)) {
                if(keyWentDown(Keys.Right)) {
                    var actives = world.CreatureObjs.Where(c => c.Active).ToArray();
                    var ndx = ThreadSafeRandom.Instance.Next(0, actives.Length);
                    Following = actives[ndx];
                }
            }

            if(click) {
                var clicked = world.ActiveGameObjs
                    .OfType<GameObjects.CreatureComponents.Creature>()
                    .FirstOrDefault(g => Vector2.Distance(g.CenterPosition, mouseTransformed.ToVector2()) < g.Radius);

                if(clicked != null) {
                    if(clicked != Following) {
                        if(Following != null) {
                            Following.ShowInfo = false;
                        }
                        Following = clicked;
                        Following.ShowInfo = true;
                    } else {
                        Following.ShowInfo = false;
                        Following = null;
                    }
                }
            }
        }
    }
}