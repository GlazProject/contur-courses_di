﻿using System.Drawing;

namespace TagCloud.App.CloudCreatorDriver.RectanglesLayouters.SpiralCloudLayouters;

public class SpiralCloudLayouter : ICloudLayouter
{
    private SpiralCloudLayouterSettings? settings;
    private readonly List<Rectangle> setRectangles = new();
    private double rotationAngle;
        
    public void SetSettings(ICloudLayouterSettings layouterSettings)
    {
        if (layouterSettings == null)
            throw new NullReferenceException("Layouter settings can not be null");
        
        if (layouterSettings is not SpiralCloudLayouterSettings spiralLayouterSettings)
            throw new Exception("Incorrect layouter settings type. " +
                                             $"Expected {typeof(SpiralCloudLayouterSettings)}, " +
                                             $"found {layouterSettings.GetType()}");
        settings = spiralLayouterSettings;
    }

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (settings == null)
            throw new NullReferenceException("Layouter settings can not be null");
        
        Rectangle rectangle;
        if (setRectangles.Count == 0)
            rectangle = new Rectangle(
                settings.Center.X - rectangleSize.Width / 2,
                settings.Center.Y - rectangleSize.Height / 2,
                rectangleSize.Width,
                rectangleSize.Height);
        else rectangle = FindNextRectangleOnSpiral(rectangleSize);
        setRectangles.Add(rectangle);
        return rectangle;
    }

    private Rectangle FindNextRectangleOnSpiral(Size rectangleSize)
    {
        rotationAngle = 0d;
        while (true)
        {
            var position = GetNextPositionOnSpiral();
            position.X += settings!.Center.X;
            position.Y += settings!.Center.Y;
            var newRectangle = GetPositionedRectangle_DependedOnAngle(position, rectangleSize);
            if (setRectangles.All(rectangle => !newRectangle.IntersectsWith(rectangle)))
            {
                return newRectangle;
            }
        }
    }
    
    private Rectangle GetPositionedRectangle_DependedOnAngle(Point position, Size rectangleSize)
    {
        var angle = rotationAngle % (2 * Math.PI);
        var x = position.X;
        var y = position.Y;
        int left, top;
        if (Math.Abs(angle - Math.PI / 2) < 1e-4)
        {
            left = x - rectangleSize.Width / 2;
            top = y - rectangleSize.Height;
            return new Rectangle(left, top, rectangleSize.Width, rectangleSize.Height);
        }
            
        if (Math.Abs(angle - 3 * Math.PI / 2) < 1e-4)
        {
            left = x - rectangleSize.Width / 2;
            top = y;
            return new Rectangle(left, top, rectangleSize.Width, rectangleSize.Height);
        }
            
        var quarterOfPlane = (int)Math.Ceiling(2 * angle / Math.PI);
        switch (quarterOfPlane)
        {
            case 1:
                left = x;
                top = y - rectangleSize.Height;
                break;
            case 2:
                left = x - rectangleSize.Width;
                top = y - rectangleSize.Height;
                break;
            case 3:
                left = x - rectangleSize.Width;
                top = y;
                break;
            default:
                left = x;
                top = y;
                break;
        }
        return new Rectangle(left, top, rectangleSize.Width, rectangleSize.Height);
    }

    private Point GetNextPositionOnSpiral()
    {
        var radius = GetPolarRadiusByAngleOnSpiral(rotationAngle);
        var x = radius * Math.Cos(rotationAngle);
        var y = radius * Math.Sin(rotationAngle);
        var intX = (int)Math.Round(x);
        var intY = (int)Math.Round(y);
        rotationAngle += settings!.RotationStep;
        return new Point(intX, intY);
    }

    private double GetPolarRadiusByAngleOnSpiral(double angle)
    {
        if (angle < 0)
            throw new ArgumentException("angle should be not negative");
        return (settings!.SpiralStep / (2 * Math.PI)) * angle;
    }
}