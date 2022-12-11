﻿using System.Drawing;
using TagCloud.App.WordPreprocessorDriver.WordsPreprocessor.Words;

namespace TagCloud.App.CloudCreatorDriver.CloudDrawers.WordToDraw;

public interface IDrawingWord : IWord
{
    Font Font { get; }
    Color Color { get; }
    Rectangle Rectangle { get; }
}