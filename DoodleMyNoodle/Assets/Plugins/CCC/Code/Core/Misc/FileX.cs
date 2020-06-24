using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CCC.IO
{
    public static class FileX
    {
        public static bool CreateIfInexistant(string fileFolder, string fileNameWithExtension)
        {
            bool created = false;
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
                created = true;
            }

            string completePath = fileFolder + '/' + fileNameWithExtension;

            if (!File.Exists(completePath))
            {
                File.Create(completePath).Close();
                created = true;
            }

            return created;
        }

        public static bool CreateIfInexistant(string fullPath)
        {
            return CreateIfInexistant(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath));
        }

        public class AssetFileAndWriter : IDisposable
        {
            public readonly FileStream FileStream;
            public readonly StreamWriter StreamWriter;

            public AssetFileAndWriter(FileStream fileStream, StreamWriter streamWriter)
            {
                FileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream));
                StreamWriter = streamWriter ?? throw new ArgumentNullException(nameof(streamWriter));
            }

            public void Dispose()
            {
                StreamWriter.Dispose();
                FileStream.Dispose();

#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
        }

        public static AssetFileAndWriter OpenFileFlushedAndReadyToWrite(string fullPath)
        {
            CreateIfInexistant(fullPath);

            FileStream fileStream = File.Open(fullPath, FileMode.Truncate);
            StreamWriter writer = new StreamWriter(fileStream);

            return new AssetFileAndWriter(fileStream, writer);
        }

        public static AssetFileAndWriter OpenFileFlushedAndReadyToWrite(string fileFolder, string fileNameWithExtension)
        {
            return OpenFileFlushedAndReadyToWrite(fileFolder + '/' + fileNameWithExtension);
        }
    }

}