using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------------------------

// ValueSetting - Add this interface to your ValueSettings, 
// each setting should have a function to evaluate the value and ajust it according to the setting

//-----------------------------------------------------------------------

public interface ValueSetting<T>
{
    T EvaluateValue(T currentValue);
}
