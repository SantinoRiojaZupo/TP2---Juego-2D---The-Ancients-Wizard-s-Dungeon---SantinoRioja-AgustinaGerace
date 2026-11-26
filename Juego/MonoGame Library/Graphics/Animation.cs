using System;
using System.Collections.Generic;

namespace MonoGameLibrary.Graphics;
using Microsoft.Xna.Framework;



public class Animation 
{
    private int _currentIndex = 0;
private TimeSpan _elapsedTime = TimeSpan.Zero;

/// <summary>
/// Gets the current texture region frame to display.
/// </summary>
public TextureRegion CurrentFrame => Frames.Count > 0 ? Frames[_currentIndex] : null;

/// <summary>
/// Indicates whether the animation has finished playing (if it doesn't loop).
/// </summary>
public bool IsFinished { get; private set; } = false;

/// <summary>
/// Gets or Sets whether the animation should loop when it reaches the end.
/// </summary>
public bool IsLooping { get; set; } = true;

/// <summary>
/// Updates the animation timer and switches frames based on elapsed time.
/// </summary>
public void Update(GameTime gameTime)
{
    if (Frames.Count == 0 || IsFinished) return;

    _elapsedTime += gameTime.ElapsedGameTime;

    if (_elapsedTime >= Delay)
    {
        _elapsedTime = TimeSpan.Zero;
        _currentIndex++;

        if (_currentIndex >= Frames.Count)
        {
            if (IsLooping)
            {
                _currentIndex = 0;
            }
            else
            {
                _currentIndex = Frames.Count - 1;
                IsFinished = true;
            }
        }
    }
}

/// <summary>
/// Resets the animation back to the first frame.
/// </summary>
public void Reset()
{
    _currentIndex = 0;
    _elapsedTime = TimeSpan.Zero;
    IsFinished = false;
}
/// <summary>
/// The texture regions that make up the frames of this animation.  The order of the regions within the collection
/// are the order that the frames should be displayed in.
/// </summary>
public List<TextureRegion> Frames { get; set; }

/// <summary>
/// The amount of time to delay between each frame before moving to the next frame for this animation.
/// </summary>
public TimeSpan Delay { get; set; }

/// <summary>
/// Creates a new animation.
/// </summary>
public Animation()
{
    Frames = new List<TextureRegion>();
    Delay = TimeSpan.FromMilliseconds(100);
}

/// <summary>
/// Creates a new animation with the specified frames and delay.
/// </summary>
/// <param name="frames">An ordered collection of the frames for this animation.</param>
/// <param name="delay">The amount of time to delay between each frame of this animation.</param>
public Animation(List<TextureRegion> frames, TimeSpan delay)
{
    Frames = frames;
    Delay = delay;
}

}

