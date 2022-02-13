
public class HealthBarDisplay : StatBarDisplay
{
    protected override float GetStatBarMaxValue() { return (float)Cache.GroupMaxHealth; }
    protected override float GetStatBarValue() { return (float)Cache.GroupHealth; }
}
