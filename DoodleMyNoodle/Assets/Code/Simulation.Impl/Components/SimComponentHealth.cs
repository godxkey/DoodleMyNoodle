using System.Collections;
using System.Collections.Generic;

public class SimComponentHealth : SimComponent
{
    public float maxHealth;
    public float minHealth;
    public float currentHealth;

    private float cachedValue;

    [SimViewGenerationIgnore]
    public float runtimeOnlyValue;
}
