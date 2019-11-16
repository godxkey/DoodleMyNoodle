using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is used to help serialization
//      fbessette: maybe this can be moved to CCC.Core
public interface IDType
{
    object GetValue();
    void SetValue(object obj);
}
