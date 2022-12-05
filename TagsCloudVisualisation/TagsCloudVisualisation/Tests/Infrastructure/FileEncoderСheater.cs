﻿using System;
using TagsCloudVisualisation.InputStream.FileInputStream;

namespace TagsCloudVisualisation.Tests.Infrastructure
{
    public class FileEncoderСheater : IFileEncoder
    {
        private readonly string text;
        private readonly string fileType;
        private readonly bool existFile;

        public FileEncoderСheater(string text, bool existFile, string fileType)
        {
            this.text = text;
            this.existFile = existFile;
            this.fileType = fileType;
        }

        public string GetText(string fileName)
        {
            return existFile
                ? text
                : throw new ArgumentException("File not found");
        }

        public bool IsCompatibleFileType(string fileName)
        {
            return fileName.EndsWith(fileType);
        }

        public string GetExpectedFileType()
        {
            return fileType;
        }
    }
}