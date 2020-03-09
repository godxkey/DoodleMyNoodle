using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCC.IO
{
    public static class PathX
    {
        public static string RemoveExtension(string path)
        {
            return path.RemoveFrom(path.LastIndexOf('.'));
        }
    }
}
