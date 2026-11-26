using System;
using Microsoft.Xna.Framework;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;
namespace MonoGameLibrary.Physics;

public sealed class CollisionBody
{
    private Point _size;

    public Vector2 Position { get; private set; }

    public Rectangle Bounds => new(
        (int)Position.X,
        (int)Position.Y,
        _size.X,
        _size.Y);

   public CollisionBody(IEnumerable<Animation> animations, Vector2 position)
{
    Position = position;
    _size = GetSize(animations);
}
    public void Move(Vector2 movement, params CollisionBody[] others)
{
    TryMove(movement.X, true, others);
    TryMove(movement.Y, false, others);
}

private void TryMove(float amount, bool horizontal, CollisionBody[] others)
{
    if (amount == 0f)
    {
        return;
    }

    Vector2 nextPosition = Position;

    if (horizontal)
    {
        nextPosition.X += amount;
    }
    else
    {
        nextPosition.Y += amount;
    }

    Rectangle nextBounds = new Rectangle(
        (int)nextPosition.X,
        (int)nextPosition.Y,
        _size.X,
        _size.Y
    );

    foreach (CollisionBody other in others)
    {
        if (other != null && nextBounds.Intersects(other.Bounds))
        {
            return;
        }
    }

    Position = nextPosition;
}

   private static Point GetSize(IEnumerable<Animation> animations)
{
    int width = 0;
    int height = 0;

    foreach (Animation animation in animations)
    {
        foreach (TextureRegion frame in animation.Frames)
        {
            width = Math.Max(width, frame.Width);
            height = Math.Max(height, frame.Height);
        }
    }

    return new Point(width * 3, height * 3);
}

    public void KeepInside(Rectangle screenBounds)
    {
        float maxX = Math.Max(screenBounds.Left, screenBounds.Right - _size.X);
        float maxY = Math.Max(screenBounds.Top, screenBounds.Bottom - _size.Y);

        Position = new Vector2(
            Math.Clamp(Position.X, screenBounds.Left, maxX),
            Math.Clamp(Position.Y, screenBounds.Top, maxY)
        );
    }

    public void SetSize(int width, int height)
    {
        _size = new Point(width, height);
    }

    public void SetHeightFromBottom(int height)
{
    float pieY = Position.Y + _size.Y;

    _size = new Point(_size.X, height);
    Position = new Vector2(Position.X, pieY - height);
}
public CollisionBody(Point size, Vector2 position)
{
    Position = position;
    _size = size;
}
public void SetPosition(Vector2 position)
{
    Position = position;
}
public static bool Intersectan(Rectangle cuerpo1, Rectangle cuerpo2)
{
    return cuerpo1.Intersects(cuerpo2);
}

public static bool Intersectan(Rectangle hitboxAtaque, CollisionBody enemigo)
{
    return hitboxAtaque.Intersects(enemigo.Bounds);
}
}
