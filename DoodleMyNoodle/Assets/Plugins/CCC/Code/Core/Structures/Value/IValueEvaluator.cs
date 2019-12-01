using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------------------------

// ValueEvaluator - Add this interface to your ValueSettings, 
// each setting should have a function to evaluate the value and ajust it according to the setting

//-----------------------------------------------------------------------

public interface IValueEvaluator<T>
{
    T EvaluateValue(T currentValue);
}
