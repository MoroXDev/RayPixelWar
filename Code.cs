using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;
using static Raylib_cs.Raylib;

public static class Code
{
  public static void CenterWindow()
  {
    SetWindowPosition(GetMonitorWidth(0) / 2 - GetScreenWidth() / 2, GetMonitorHeight(0) / 2 - GetScreenHeight() / 2);    
  }

  public static readonly Vector2 FULL_HD = new Vector2(1920, 1080);

}