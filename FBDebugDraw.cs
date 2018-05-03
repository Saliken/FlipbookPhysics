﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlipbookPhysics
{
    public static class FBDebugDraw
    {

        public static SpriteBatch SpriteBatch { get; private set; }
        public static Texture2D PixelTexture { get; set; }
        public static SpriteFont Font { get; set; }

        public static void Initialize(GraphicsDevice graphicsDevice, SpriteFont font)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
            PixelTexture = new Texture2D(graphicsDevice, 1, 1);
            PixelTexture.SetData<Color>(new Color[] { Color.White });
            Font = font;
        }

        public static void Begin()
        {
            SpriteBatch.Begin();
        }

        public static void End()
        {
            SpriteBatch.End();
        }

        public static void Point(Vector2 at, Color color)
        {
            SpriteBatch.Draw(PixelTexture, at, color);
        }

        public static void Line(Vector2 startPosition, Vector2 endPosition, Color color, float thickness = 1)
        {
            var angle = (float)Math.Atan2(endPosition.Y - startPosition.Y, endPosition.X - startPosition.X);
            Line(startPosition, angle, Vector2.Distance(startPosition, endPosition), color, thickness);
        }

        public static void Line(Vector2 startPosition, float angle, float length, Color color, float thickness = 1)
        {
            SpriteBatch.Draw(PixelTexture, startPosition, null, color, angle, new Vector2(0f, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 0f);
        }

        public static void Circle(Vector2 position, float radius, Color color, float thickness = 1, int resolution = 5)
        {
            Vector2 last = Vector2.UnitX * radius;
            Vector2 lastP = last.Perpendicular();
            for (int i = 1; i <= resolution; i++)
            {
                Vector2 at = (i * MathHelper.PiOver2 / resolution).RadiansToVector(radius);
                Vector2 atP = at.Perpendicular();

                Line(position + last, position + at, color, thickness);
                Line(position - last, position - at, color, thickness);
                Line(position + lastP, position + atP, color, thickness);
                Line(position - lastP, position - atP, color, thickness);

                last = at;
                lastP = atP;
            }
        }

        public static void Rectangle(Vector2 position, float width, float height, Color color, bool hollow = false, int thickness = 1)
        {
            Rectangle(new Rectangle((int)position.X, (int)position.Y, (int)width, (int)height), color);
        }

        public static void Rectangle(Rectangle rectangle, Color color, bool hollow = false, int thickness = 1)
        {
            if (hollow)
            {
                Line(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Right, rectangle.Top), color);
                Line(new Vector2(rectangle.Left, rectangle.Top), new Vector2(rectangle.Left, rectangle.Bottom), color);
                Line(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Bottom), color);
                Line(new Vector2(rectangle.Right, rectangle.Bottom), new Vector2(rectangle.Right, rectangle.Top), color);
            }
            else
                SpriteBatch.Draw(PixelTexture, rectangle, color);
        }

        public static void Polygon(FBPolygon polygon, Color color)
        {
            foreach (var line in polygon.MovedLines)
            {
                Line(line.StartPosition, line.EndPosition, color);
            }
        }

        public static void Text(string text, Vector2 position, Color color)
        {
            SpriteBatch.DrawString(Font, text, position, color);
        }
        public static void TextCentered(string text, Vector2 position, Color color)
        {
            Text(text, position - Font.MeasureString(text) * .5f, color);
        }

    }
}
