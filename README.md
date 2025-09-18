
⸻



# VectorGraphics Library Documentation

A simple vector drawing library for MonoGame, featuring utilities to draw lines, circles, rectangles, and pixels.

---

## 📦 Namespace: `VectorGraphics`

### 🔧 Class: `PrimitiveBatch`

Handles the creation of common drawing textures and acts as a shared resource container.

#### Constructor

```csharp
PrimitiveBatch(GraphicsDevice graphicsDevice)

Initializes WhitePixel and WhiteCircle textures for rendering shapes.

Properties
	•	Texture2D WhitePixel — 1×1 white pixel texture for rectangle/line drawing.
	•	Texture2D WhiteCircle — Large white circle texture with transparency outside radius.
	•	GraphicsDevice graphicsDevice — Reference to the graphics device.

Method

void CreateTextures()

Creates a white pixel and a 1000×1000 white circle texture used for drawing shapes.

⸻

🔹 Nested Classes

⸻

🟦 PrimitiveBatch.Line

Represents a drawable line.

Constructors

Line(Vector2 start, Vector2 end, Color color, float width)
Line(Vector2 start, float angle, float distance, Color color, float width)

	•	First version defines a line with start and end points.
	•	Second version defines a line with angle and length from the start point.

Method

void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)

Draws the line using the WhitePixel texture.

⸻

⚪ PrimitiveBatch.Circle

Represents a drawable circle.

Constructor

Circle(Vector2 position, float radius, Color color)

Method

void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)

Draws a circle using the pre-generated WhiteCircle texture and scales it based on radius.

⸻

▫ PrimitiveBatch.Rectangle

Represents a drawable rectangle.

Constructors

Rectangle(Vector2 position, Vector2 size, Color color)
Rectangle(Microsoft.Xna.Framework.Rectangle rectangle, Color color)

Method

void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)

Draws a rectangle using the WhitePixel texture.

⸻

📏 Class: RectangleTexture

Utility class for creating filled rectangle textures.

Method

Texture2D CreateTexture(Vector2 size, PrimitiveBatch primitiveBatch)

	•	Creates and returns a white texture of the given size.

⸻

🧵 Class: Pixel

Represents a drawable pixel.

Constructor

Pixel(Vector2 position, Color color)

Method

void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)

Draws a single pixel using WhitePixel.

⸻

📌 Usage Example

PrimitiveBatch primitives = new PrimitiveBatch(GraphicsDevice);

var line = new PrimitiveBatch.Line(new Vector2(10, 10), new Vector2(100, 100), Color.Red, 2f);
var circle = new PrimitiveBatch.Circle(new Vector2(200, 200), 50f, Color.Blue);
var rect = new PrimitiveBatch.Rectangle(new Vector2(300, 300), new Vector2(60, 40), Color.Green);
var pixel = new Pixel(new Vector2(400, 400), Color.Yellow);

spriteBatch.Begin();
line.Draw(spriteBatch, primitives);
circle.Draw(spriteBatch, primitives);
rect.Draw(spriteBatch, primitives);
pixel.Draw(spriteBatch, primitives);
spriteBatch.End();



⸻

📁 Notes
	•	This library is optimized for ease of use, not performance.
	•	All shapes are rendered using simple 2D textures.
	•	WhiteCircle is pre-generated at 1000×1000 resolution—adjust diameter in CreateTextures() if needed.

⸻

📜 License

MIT or custom license as defined by the developer.

---

