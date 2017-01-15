using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gnn.Visual {

    public class Cam2D {
        private const int BaseArrowScrollSpeedPx = 20;

        private bool clicked = false;
        private Point lastClickPos;
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

        public Cam2D(int viewWidth, int viewHeight, Point? centerPos = null) {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
            Center = centerPos ?? new Point(ViewWidth / 2, ViewHeight / 2);
        }

        public void Move(MouseState mState, KeyboardState kState) {
            var mouseTransformed = Vector2.Transform(mState.Position.ToVector2(), Matrix.Invert(Transform)).ToPoint();

            if(!clicked && mState.LeftButton == ButtonState.Pressed) {
                lastClickPos = mouseTransformed;
                clicked = true;
            }

            if(clicked) {
                Center += (lastClickPos - mouseTransformed);

                if(mState.LeftButton == ButtonState.Released) {
                    clicked = false;
                }
            }

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

            if(kState.IsKeyDown(Keys.Space) && kState.IsKeyDown(Keys.LeftControl)) {
                Rotation = 0;
                Zoom = 1;
                Center = new Point(ViewWidth / 2, ViewHeight / 2);
            }

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
    }
}