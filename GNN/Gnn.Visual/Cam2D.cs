using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gnn.Visual {

    public class Cam2D {
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
        }
    }
}