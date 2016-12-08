using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gnn.Visual {

    public static class DrawHelper {
        public static Texture2D Pixel { get; set; }

        public static void Init(GraphicsDevice graphicsDevice) {
            Pixel = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Pixel.SetData(new Color[] { Color.White });
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, int width, Color color) {
            var length = (int)Math.Round(Vector2.Distance(start, end));
            var rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            var rect = new Rectangle((int)Math.Round(start.X), (int)Math.Round(start.Y), length, width);
            spriteBatch.Draw(Pixel, rect, null, color, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }

        public static void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color) {
            spriteBatch.Draw(Pixel, rect, color);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Point start, Point end, int width, Color color) => DrawLine(spriteBatch, new Vector2(start.X, start.Y), new Vector2(end.X, end.Y), width, color);

        public static void DrawLine(SpriteBatch spriteBatch, float startX, float startY, float endX, float endY, int width, Color color) => DrawLine(spriteBatch, new Vector2(startX, startY), new Vector2(endX, endY), width, color);

        public static void DrawRect(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color) => DrawRect(spriteBatch, new Rectangle(x, y, width, height), color);
    }
}