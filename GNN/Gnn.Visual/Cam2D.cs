using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gnn.Visual {

    public class Cam2D {
        private bool clicked = false;
        private Vector2 clickPos;
        private int previousScrollValue = 0;

        public Point Center;

        private float zoom = 1;
        public float Zoom {
            get { return zoom; }
            set { zoom = value <= 0 ? 0.01f : value; }
        }

        public int ViewWidth { get; set; }
        public int ViewHeight { get; set; }

        public Matrix Transform {
            get {
                return
                    Matrix.CreateTranslation(new Vector3(-Center.X, -Center.Y, 0)) *
                    Matrix.CreateScale(Zoom) *
                    Matrix.CreateTranslation(new Vector3(ViewWidth * 0.5f, ViewHeight * 0.5f, 0));
            }
        }

        public Cam2D(int viewWidth, int viewHeight, Point? centerPos = null) {
            ViewWidth = viewWidth;
            ViewHeight = viewHeight;
            Center = centerPos ?? new Point(ViewWidth / 2, ViewHeight / 2);
        }

        public void Move(MouseState mState) {
            if(!clicked && mState.LeftButton == ButtonState.Pressed) {
                clickPos = new Vector2(mState.X, mState.Y);
                clicked = true;
            }

            if(clicked) {
                int xMovement = (int)((clickPos.X - mState.X) / Zoom);
                int yMovement = (int)((clickPos.Y - mState.Y) / Zoom);

                Center.X += xMovement;
                Center.Y += yMovement;

                clickPos = new Vector2(mState.X, mState.Y);

                if(mState.LeftButton == ButtonState.Released) {
                    clicked = false;
                }
            }

            if(mState.ScrollWheelValue < previousScrollValue) {
                Zoom *= 0.8f;
                previousScrollValue = mState.ScrollWheelValue;
            } else if(mState.ScrollWheelValue > previousScrollValue) {
                Zoom *= 1.25f;
                previousScrollValue = mState.ScrollWheelValue;
            }
        }
    }
}