using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

namespace CCC.IO
{
    public static class PathX
    {
        public static string RemoveExtension(string path)
        {
            return path.TrimEnd(path.LastIndexOf('.'));
        }
    }
}
