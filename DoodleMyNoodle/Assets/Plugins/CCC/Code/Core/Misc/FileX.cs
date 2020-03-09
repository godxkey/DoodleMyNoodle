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
        public static void MakeSureExits(string fileFolder, string fileNameWithExtension)
        {
            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }

            string completePath = fileFolder + '/' + fileNameWithExtension;

            if (!File.Exists(completePath))
            {
                File.Create(completePath).Close();
            }
        }
        public static void MakeSureExits(string fullPath)
        {
            MakeSureExits(Path.GetDirectoryName(fullPath), Path.GetFileName(fullPath));
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
            MakeSureExits(fullPath);

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