using System;
using System.Numerics;
using Raylib_cs;

namespace VectorGraphics;

using RlColor = Raylib_cs.Color;
using RlRectangle = Raylib_cs.Rectangle;

/// <summary>
/// Raylib-native primitive drawing helpers for lines, circles, rectangles, triangles, and arrows.
/// </summary>
public class PrimitiveBatch
{
    public PrimitiveBatch(float circleRadius = 75f) { }

    public void CreateTextures(float circleRadius = 75f) { }

    public abstract class Shape
    {
        public Vector2 position;
        public RlColor color;
        public bool filled;

        protected Shape(Vector2 position, RlColor color, bool filled = true)
        {
            this.position = position;
            this.color = color;
            this.filled = filled;
        }

        public abstract void Draw();
    }

    public class Line : Shape
    {
        public Vector2 end;
        public float width;

        public Line(Vector2 start, Vector2 end, RlColor color, float width)
            : base(start, color, false)
        {
            this.end = end;
            this.width = width;
        }

        public Line(Vector2 start, float angle, float distance, RlColor color, float width)
            : base(start, color, false)
        {
            end = new Vector2(
                position.X + (float)Math.Cos(angle) * distance,
                position.Y + (float)Math.Sin(angle) * distance
            );
            this.width = width;
        }

        public override void Draw()
        {
            Raylib.DrawLineEx(position, end, width, color);
        }

        public static void Draw(Vector2 start, Vector2 end, RlColor color, float width)
        {
            Raylib.DrawLineEx(start, end, width, color);
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
            {
                return position;
            }

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

        public Circle(Vector2 position, float radius, RlColor color, bool filled = true)
            : base(position, color, filled)
        {
            this.radius = radius;
        }

        public override void Draw()
        {
            if (filled)
            {
                Raylib.DrawCircleV(position, radius, color);
            }
            else
            {
                Raylib.DrawCircleLinesV(position, radius, color);
            }
        }
    }

    public class Rectangle : Shape
    {
        public Vector2 size;
        public float edgeWidth;
        public RlColor edgeColor;
        public float rotation;

        public Rectangle(Vector2 position, Vector2 size, RlColor color, bool filled = true)
            : base(position, color, filled)
        {
            this.size = size;
        }
        

        public override void Draw()
        {
            var rect = new RlRectangle(position.X, position.Y, size.X, size.Y);

            if (filled)
            {
                if (Math.Abs(rotation) > 0.0001f)
                {
                    Raylib.DrawRectanglePro(
                        rect,
                        new Vector2(size.X / 2f, size.Y / 2f),
                        rotation * 57.29578f,
                        color
                    );
                }
                else
                {
                    Raylib.DrawRectangleV(position, size, color);
                }
            }

            if (edgeWidth > 0)
            {
                Raylib.DrawRectangleLinesEx(rect, edgeWidth, edgeColor);
            }
        }
    }

    public class RoundedRectangle : Rectangle
    {
        public float cornerRadius;

        public RoundedRectangle(Vector2 position, Vector2 size, float cornerRadius, RlColor color)
            : base(position, size, color)
        {
            this.cornerRadius = cornerRadius;
        }

        public override void Draw()
        {
            var rect = new RlRectangle(position.X, position.Y, size.X, size.Y);
            float roundness = cornerRadius / MathF.Max(1f, MathF.Min(size.X, size.Y) / 2f);
            roundness = Math.Clamp(roundness, 0f, 1f);

            if (filled)
            {
                Raylib.DrawRectangleRounded(rect, roundness, 12, color);
            }

            if (edgeWidth > 0)
            {
                Raylib.DrawRectangleRoundedLines(rect, roundness, 12, edgeColor);
            }
        }
    }

    public class Pixel : Shape
    {
        public Pixel(Vector2 position, RlColor color)
            : base(position, color, false) { }

        public override void Draw()
        {
            Raylib.DrawPixel((int)position.X, (int)position.Y, color);
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
            RlColor color,
            bool filled = true
        )
            : base(point1, color, filled)
        {
            this.point1 = point1;
            this.point2 = point2;
            this.point3 = point3;
        }

        public override void Draw()
        {
            if (filled)
            {
                Raylib.DrawTriangle(point1, point2, point3, color);
            }
            else
            {
                Raylib.DrawLineEx(point1, point2, 1f, color);
                Raylib.DrawLineEx(point2, point3, 1f, color);
                Raylib.DrawLineEx(point3, point1, 1f, color);
            }
        }
    }

    public class Arrow : Shape
    {
        public Vector2 Start;
        public Vector2 End;
        public float Width;

        public Arrow(Vector2 start, Vector2 end, RlColor color, float width = 2.0f)
            : base(start, color, false)
        {
            Start = start;
            End = end;
            Width = width;
        }

        public override void Draw()
        {
            Line line = new Line(Start, End, color, Width);
            line.Draw();

            Vector2 direction = Vector2.Normalize(End - Start);
            Vector2 perpendicular = new Vector2(-direction.Y, direction.X);
            float arrowHeadSize = 10f;

            Vector2 arrowPoint1 =
                End - direction * arrowHeadSize + perpendicular * (arrowHeadSize / 2);
            Vector2 arrowPoint2 =
                End - direction * arrowHeadSize - perpendicular * (arrowHeadSize / 2);

            Triangle triangle = new Triangle(End, arrowPoint1, arrowPoint2, color);
            triangle.Draw();
        }
    }
}
