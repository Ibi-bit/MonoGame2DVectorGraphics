using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VectorGraphics
{
    public class PrimitiveBatch
    {
        public Texture2D whitePixel;
        public GraphicsDevice graphicsDevice;

        public PrimitiveBatch(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            CreateTextures();
        }

        public void CreateTextures()
        {
            whitePixel = new Texture2D(graphicsDevice, 1, 1);
            whitePixel.SetData(new[] { Color.White });
        }

        public abstract class Shape
        {
            public Vector2 position;
            public Color color;
            public bool filled;

            public Shape(Vector2 position, Color color, bool filled = true)
            {
                this.position = position;
                this.color = color;
                this.filled = filled;
            }

            public abstract void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch);
        }

        public class Line : Shape
        {
            public Vector2 end;
            public float width;

            public Line(Vector2 start, Vector2 end, Color color, float width)
                : base(start, color, false)
            {
                this.end = end;
                this.width = width;
            }

            public Line(Vector2 start, float angle, float distance, Color color, float width)
                : base(start, color, false)
            {
                this.end = new Vector2(
                    (float)Math.Cos(angle) * distance,
                    (float)Math.Sin(angle) * distance
                );
                this.width = width;
            }

            public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
            {
                Vector2 edge = end - position;
                float angle = (float)Math.Atan2(edge.Y, edge.X);
                spriteBatch.Draw(
                    primitiveBatch.whitePixel,
                    new Microsoft.Xna.Framework.Rectangle(
                        (int)(position.X),
                        (int)(position.Y),
                        (int)edge.Length(),
                        (int)width
                    ),
                    null,
                    color,
                    angle,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0
                );
            }

            public static void Draw(
                SpriteBatch spriteBatch,
                PrimitiveBatch primitiveBatch,
                Vector2 start,
                Vector2 end,
                Color color,
                float width
            )
            {
                Vector2 edge = end - start;
                float angle = (float)Math.Atan2(edge.Y, edge.X);
                spriteBatch.Draw(
                    primitiveBatch.whitePixel,
                    new Microsoft.Xna.Framework.Rectangle(
                        (int)(start.X),
                        (int)(start.Y),
                        (int)edge.Length(),
                        (int)width
                    ),
                    null,
                    color,
                    angle,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0
                );
            }

            public static Vector2 ConstrainToSide(
                Vector2 position,
                Vector2 start,
                Vector2 end,
                float particleRadius = 5f
            )
            {
                Vector2 lineDir = end - start;
                float lineLength = lineDir.Length();

                if (lineLength < 0.001f)
                    return position;

                lineDir /= lineLength;

                Vector2 lineNormal = new Vector2(-lineDir.Y, lineDir.X);
                Vector2 toParticle = position - start;

                float side = Vector2.Dot(toParticle, lineNormal);
                float projection = Vector2.Dot(toParticle, lineDir);
                projection = Math.Clamp(projection, 0, lineLength);

                Vector2 closestPoint = start + lineDir * projection;

                if (side < particleRadius)
                {
                    position = closestPoint + lineNormal * particleRadius;
                }

                return position;
            }
        }

        public class Circle : Shape
        {
            public float radius;

            public Circle(Vector2 position, float radius, Color color, bool filled = true)
                : base(position, color, filled)
            {
                this.radius = radius;
            }

            public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
            {
                int cx = (int)position.X;
                int cy = (int)position.Y;
                int r = (int)radius;
                int x = 0;
                int y = r;
                int d = 3 - 2 * r;

                void PlotCirclePoints(int xc, int yc, int x, int y)
                {
                    spriteBatch.Draw(primitiveBatch.whitePixel, new Vector2(xc + x, yc + y), color);
                    spriteBatch.Draw(primitiveBatch.whitePixel, new Vector2(xc - x, yc + y), color);
                    spriteBatch.Draw(primitiveBatch.whitePixel, new Vector2(xc + x, yc - y), color);
                    spriteBatch.Draw(primitiveBatch.whitePixel, new Vector2(xc - x, yc - y), color);
                    spriteBatch.Draw(primitiveBatch.whitePixel, new Vector2(xc + y, yc + x), color);
                    spriteBatch.Draw(primitiveBatch.whitePixel, new Vector2(xc - y, yc + x), color);
                    spriteBatch.Draw(primitiveBatch.whitePixel, new Vector2(xc + y, yc - x), color);
                    spriteBatch.Draw(primitiveBatch.whitePixel, new Vector2(xc - y, yc - x), color);
                }
                void FillCircle(int x, int y, int r)
                {
                    for (int i = -r; i <= r; i++)
                    {
                        for (int j = -r; j <= r; j++)
                        {
                            if (i * i + j * j <= r * r)
                            {
                                spriteBatch.Draw(
                                    primitiveBatch.whitePixel,
                                    new Vector2(x + i, y + j),
                                    color
                                );
                            }
                        }
                    }
                }

                while (y >= x)
                {
                    PlotCirclePoints(cx, cy, x, y);
                    x++;
                    if (filled)
                        FillCircle(cx, cy, r);
                    if (d > 0)
                    {
                        y--;
                        d = d + 4 * (x - y) + 10;
                    }
                    else
                    {
                        d = d + 4 * x + 6;
                    }
                }
            }
        }

        public class Rectangle : Shape
        {
            public Vector2 size;
            public float edgeWidth;
            public Color edgeColor;

            public Rectangle(Vector2 position, Vector2 size, Color color, bool filled = true)
                : base(position, color, filled)
            {
                this.size = size;
            }

            public Rectangle(
                Microsoft.Xna.Framework.Rectangle rectangle,
                Color color,
                bool filled = true,
                float edgeWidth = 0,
                Color? edgeColor = null
            )
                : base(new Vector2(rectangle.X, rectangle.Y), color, filled)
            {
                this.size = new Vector2(rectangle.Width, rectangle.Height);
                this.edgeWidth = edgeWidth;
                this.edgeColor = edgeColor ?? color;
            }

            public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
            {
                if (filled)
                {
                    spriteBatch.Draw(
                        primitiveBatch.whitePixel,
                        new Microsoft.Xna.Framework.Rectangle(
                            (int)(position.X),
                            (int)(position.Y),
                            (int)size.X,
                            (int)size.Y
                        ),
                        color
                    );
                }
                if (edgeWidth > 0)
                {
                    float halfWidth = edgeWidth / 2f;
                    Line.Draw(
                        spriteBatch,
                        primitiveBatch,
                        position,
                        new Vector2(position.X + size.X, position.Y),
                        edgeColor,
                        edgeWidth
                    );
                    Line.Draw(
                        spriteBatch,
                        primitiveBatch,
                        new Vector2(position.X + size.X, position.Y),
                        new Vector2(position.X + size.X, position.Y + size.Y),
                        edgeColor,
                        edgeWidth
                    );
                    Line.Draw(
                        spriteBatch,
                        primitiveBatch,
                        new Vector2(position.X, position.Y + size.Y),
                        new Vector2(position.X + size.X, position.Y + size.Y),
                        edgeColor,
                        edgeWidth
                    );
                    Line.Draw(
                        spriteBatch,
                        primitiveBatch,
                        position,
                        new Vector2(position.X, position.Y + size.Y),
                        edgeColor,
                        edgeWidth
                    );
                }
            }
        }

        public class RoundedRectangle : Rectangle
        {
            public float cornerRadius;

            public RoundedRectangle(Vector2 position, Vector2 size, float cornerRadius, Color color)
                : base(position, size, color)
            {
                this.cornerRadius = cornerRadius;
            }

            public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
            {
                float innerWidth = size.X - 2 * cornerRadius;
                float innerHeight = size.Y - 2 * cornerRadius;
                var centerRect = new PrimitiveBatch.Rectangle(
                    position + new Vector2(cornerRadius, cornerRadius),
                    new Vector2(innerWidth, innerHeight),
                    color
                );
                centerRect.Draw(spriteBatch, primitiveBatch);
                var topRect = new PrimitiveBatch.Rectangle(
                    position + new Vector2(cornerRadius, 0),
                    new Vector2(innerWidth, cornerRadius),
                    color
                );
                topRect.Draw(spriteBatch, primitiveBatch);
                var bottomRect = new PrimitiveBatch.Rectangle(
                    position + new Vector2(cornerRadius, size.Y - cornerRadius),
                    new Vector2(innerWidth, cornerRadius),
                    color
                );
                bottomRect.Draw(spriteBatch, primitiveBatch);
                var leftRect = new PrimitiveBatch.Rectangle(
                    position + new Vector2(0, cornerRadius),
                    new Vector2(cornerRadius, innerHeight),
                    color
                );
                leftRect.Draw(spriteBatch, primitiveBatch);
                var rightRect = new PrimitiveBatch.Rectangle(
                    position + new Vector2(size.X - cornerRadius, cornerRadius),
                    new Vector2(cornerRadius, innerHeight),
                    color
                );
                rightRect.Draw(spriteBatch, primitiveBatch);
                var topLeft = new PrimitiveBatch.Circle(
                    position + new Vector2(cornerRadius, cornerRadius),
                    cornerRadius,
                    color
                );
                topLeft.Draw(spriteBatch, primitiveBatch);
                var topRight = new PrimitiveBatch.Circle(
                    position + new Vector2(size.X - cornerRadius, cornerRadius),
                    cornerRadius,
                    color
                );
                topRight.Draw(spriteBatch, primitiveBatch);
                var bottomLeft = new PrimitiveBatch.Circle(
                    position + new Vector2(cornerRadius, size.Y - cornerRadius),
                    cornerRadius,
                    color
                );
                bottomLeft.Draw(spriteBatch, primitiveBatch);
                var bottomRight = new PrimitiveBatch.Circle(
                    position + new Vector2(size.X - cornerRadius, size.Y - cornerRadius),
                    cornerRadius,
                    color
                );
                bottomRight.Draw(spriteBatch, primitiveBatch);
            }
        }

        public class RectangleTexture
        {
            public Vector2 size;
            private Texture2D texture;

            public Texture2D CreateTexture(Vector2 size, PrimitiveBatch primitiveBatch)
            {
                this.size = size;
                texture = new Texture2D(primitiveBatch.graphicsDevice, (int)size.X, (int)size.Y);
                Color[] data = new Color[(int)size.X * (int)size.Y];
                for (int i = 0; i < data.Length; ++i)
                    data[i] = Color.White;
                texture.SetData(data);
                return texture;
            }
        }

        public class Pixel : Shape
        {
            public Pixel(Vector2 position, Color color)
                : base(position, color, false) { }

            public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
            {
                spriteBatch.Draw(primitiveBatch.whitePixel, position, color);
            }
        }

        public class Triangle : Shape
        {
            public Vector2 point1;
            public Vector2 point2;
            public Vector2 point3;

            public Triangle(
                Vector2 point1,
                Vector2 point2,
                Vector2 point3,
                Color color,
                bool filled = true
            )
                : base(point1, color, filled)
            {
                this.point1 = point1;
                this.point2 = point2;
                this.point3 = point3;
            }

            public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
            {
                if (filled)
                {
                    // Barycentric method for filling triangle
                    Vector2 v0 = point2 - position;
                    Vector2 v1 = point3 - position;
                    float area = v0.X * v1.Y - v0.Y * v1.X;

                    int minX = (int)Math.Min(position.X, Math.Min(point2.X, point3.X));
                    int maxX = (int)Math.Max(position.X, Math.Max(point2.X, point3.X));
                    int minY = (int)Math.Min(position.Y, Math.Min(point2.Y, point3.Y));
                    int maxY = (int)Math.Max(position.Y, Math.Max(point2.Y, point3.Y));

                    for (int y = minY; y <= maxY; y++)
                    {
                        for (int x = minX; x <= maxX; x++)
                        {
                            Vector2 p = new Vector2(x, y);
                            Vector2 v2 = p - position;

                            float d00 = Vector2.Dot(v0, v0);
                            float d01 = Vector2.Dot(v0, v1);
                            float d11 = Vector2.Dot(v1, v1);
                            float d20 = Vector2.Dot(v2, v0);
                            float d21 = Vector2.Dot(v2, v1);

                            float denom = d00 * d11 - d01 * d01;
                            float a = (d11 * d20 - d01 * d21) / denom;
                            float b = (d00 * d21 - d01 * d20) / denom;

                            if (a >= 0 && b >= 0 && (a + b) <= 1)
                            {
                                spriteBatch.Draw(primitiveBatch.whitePixel, p, color);
                            }
                        }
                    }
                }
                else
                {
                    var line1 = new Line(position, point2, color, 1);
                    var line2 = new Line(point2, point3, color, 1);
                    var line3 = new Line(point3, position, color, 1);
                    line1.Draw(spriteBatch, primitiveBatch);
                    line2.Draw(spriteBatch, primitiveBatch);
                    line3.Draw(spriteBatch, primitiveBatch);
                }
            }
        }

        public class Arrow : Shape
        {
            public Vector2 Start;
            public Vector2 End;
            public float Width;

            public Arrow(Vector2 start, Vector2 end, Color color, float width = 2.0f)
                : base(start, color, false)
            {
                Start = start;
                End = end;
                Width = width;
            }

            public override void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
            {
                PrimitiveBatch.Line line = new PrimitiveBatch.Line(Start, End, color, Width);
                line.Draw(spriteBatch, primitiveBatch);

                Vector2 direction = Vector2.Normalize(End - Start);
                Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
                float arrowHeadSize = 10f;

                Vector2 arrowPoint1 =
                    End - direction * arrowHeadSize + perpendicular * (arrowHeadSize / 2);
                Vector2 arrowPoint2 =
                    End - direction * arrowHeadSize - perpendicular * (arrowHeadSize / 2);

                PrimitiveBatch.Triangle triangle = new PrimitiveBatch.Triangle(
                    End,
                    arrowPoint1,
                    arrowPoint2,
                    color
                );
                triangle.Draw(spriteBatch, primitiveBatch);
            }
        }
    }
}
