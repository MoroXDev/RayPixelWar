using Raylib_cs;
using static Raylib_cs.Raylib;
using static Code;
using System.Numerics;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml;

public class Program
{
  public static void Main()
  {
    Game game = new Game();

    while (!WindowShouldClose())
    {
      game.Update();
      game.Draw();
    }

    game.End();
  }
}

public class Game
{
  RenderTexture2D screen;
  Pixel[,] pixels;
  Vector2 mousePos;
  Color[] colors;
  Color currentColor;
  Color backgroundColor;
  Stopwatch chooseColorTimer = new Stopwatch();
  bool isColorSelected = false;
  bool isSaved = false;
  Rectangle selectedColorRect;

  public Game()
  {
    SetConfigFlags(ConfigFlags.ResizableWindow);

    InitWindow(10, 10, "Pixel War");
    SetWindowSize((int)(GetMonitorWidth(0) * 0.75f), (int)(GetMonitorHeight(0) * 0.75f));
    CenterWindow();

    SetTargetFPS(60);
    SetWindowIcon(LoadImage("PixelWarIcon.png"));

    //Initialize
    screen = LoadRenderTexture((int)FULL_HD.X, (int)FULL_HD.Y);
    pixels = new Pixel[40, 40];

    //initialize pixels
    int pixelSize = 20;
    Vector2 offset = new Vector2(FULL_HD.X / 2 - pixels.GetLength(0) * pixelSize / 2, FULL_HD.Y / 2 - pixels.GetLength(1) * pixelSize / 2);
    for (int x = 0; x < pixels.GetLength(0); x++)
    {
      for (int y = 0; y < pixels.GetLength(1); y++)
      {
        pixels[x, y] = new Pixel(new Rectangle(offset.X + x * pixelSize, offset.Y + y * pixelSize, pixelSize, pixelSize), Color.White);
      }
    }

    colors = new Color[16];
    colors[0] = Color.Green;
    colors[1] = Color.Blue;
    colors[2] = Color.Orange;
    colors[3] = Color.Purple;
    colors[4] = Color.Yellow;
    colors[5] = Color.Pink;
    colors[6] = Color.Red;
    colors[7] = Color.SkyBlue;
    colors[8] = Color.Brown;
    colors[9] = Color.Lime;
    colors[10] = Color.Magenta;
    colors[11] = Color.Maroon;
    colors[12] = Color.Black;
    colors[13] = Color.Gold;
    colors[14] = Color.White;
    colors[15] = Color.Violet;

    currentColor = Color.Maroon;

    chooseColorTimer.Start();

    backgroundColor = Color.Purple;
  }

  public void Update()
  {
    //Update mousePos
    mousePos.X = GetMousePosition().X * FULL_HD.X / GetScreenWidth();
    mousePos.Y = GetMousePosition().Y * FULL_HD.Y / GetScreenHeight();

    UpdatePixels();
    Save();
    Clear();
  }

  public void Draw()
  {
    //Draw on screen
    BeginTextureMode(screen);
    ClearBackground(backgroundColor);

    for (int x = 0; x < pixels.GetLength(0); x++)
    {
      for (int y = 0; y < pixels.GetLength(1); y++)
      {
        DrawRectangleRec(pixels[x, y].rect, pixels[x, y].color);
        DrawRectangleLinesEx(pixels[x, y].rect, 2, Color.Black);
      }
    }

    UpdateColors();

    EndTextureMode();

    //Draw screen
    BeginDrawing();
    ClearBackground(backgroundColor);

    DrawTexturePro(screen.Texture, new Rectangle(0, 0, screen.Texture.Width, -screen.Texture.Height), new Rectangle(0, 0, GetScreenWidth(), GetScreenHeight()), new Vector2(0, 0), 0, Color.White);

    EndDrawing();
  }

  public void End()
  {
    UnloadRenderTexture(screen);
  }

  public void UpdatePixels()
  {
    for (int x = 0; x < pixels.GetLength(0); x++)
    {
      for (int y = 0; y < pixels.GetLength(1); y++)
      {
        if (IsMouseButtonDown(MouseButton.Left))
        {
          if (CheckCollisionPointRec(mousePos, pixels[x, y].rect))
          {
            pixels[x, y].Draw(currentColor);
          }
        }
      }
    }
  }

  private void UpdateColors()
  {
    Rectangle colorRect;
    int colorSize = 80;
    int y = 0;
    int x = 0;
    const float offset = 120;
    for (int i = 0; i < colors.Length; i++)
    {
      colorRect = new Rectangle(offset + colorSize * x, offset + colorSize * y, colorSize, colorSize);

      DrawRectangleRec(colorRect, colors[i]);
      DrawRectangleLinesEx(colorRect, 2, Color.Black);

      if (CheckCollisionPointRec(mousePos, colorRect))
      {
        if (chooseColorTimer.Elapsed.TotalSeconds > 0.1f)
        {
          isColorSelected = false;
        }
        if (IsMouseButtonPressed(MouseButton.Left))
        {
          currentColor = colors[i];
          if (chooseColorTimer.Elapsed.TotalSeconds > 0.1f)
          {
            isColorSelected = true;
            selectedColorRect = colorRect;
            chooseColorTimer.Restart();
          }
        }
        if (isColorSelected)
        {
          DrawRectangleRec(selectedColorRect, Fade(Color.White, 0.3f));
        }
      }

      if (x >= 3)
      {
        x = 0;
        y++;
      }
      else
      {
        x++;
      }
    }
  }

  public void Save()
  {
    if (IsKeyDown(KeyboardKey.LeftControl) && IsKeyDown(KeyboardKey.S))
    {
      if (!isSaved)
      {
        RenderTexture2D screenshotTexture = LoadRenderTexture(screen.Texture.Width, screen.Texture.Height);

        BeginTextureMode(screenshotTexture);

        DrawTexture(screen.Texture, 0, 0, Color.White);

        EndTextureMode();

        Image screenshot = LoadImageFromTexture(screenshotTexture.Texture);

        ExportImage(screenshot, "screenshot.png");

        isSaved = true;
        Console.WriteLine("SAVED!");
      }
    }
    else
    {
      isSaved = false;
    }
    //Image LoadImageFromTexture(Texture2D texture); 
    // bool ExportImage(Image image, const char *fileName);
  }

  private void Clear()
  {
    if (IsKeyPressed(KeyboardKey.Delete))
    {
      for (int x = 0; x < pixels.GetLength(0); x++)
      {
        for (int y = 0; y < pixels.GetLength(1); y++)
        {
          pixels[x, y].color = Color.White;
        }
      }
    }
  }
}

public class Pixel
{
  public Rectangle rect;
  public Color color;

  public Pixel(Rectangle rect, Color color)
  {
    this.rect = rect;
    this.color = color;
  }

  public void Draw(Color color)
  {
    this.color = color;
  }
}