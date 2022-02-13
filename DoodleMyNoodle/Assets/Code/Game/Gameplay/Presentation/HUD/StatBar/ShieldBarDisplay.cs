
public class ShieldBarDisplay : StatBarDisplay
{
    protected override float GetStatBarMaxValue() { return (float)Cache.GroupMaxShield; }
    protected override float GetStatBarValue() { return (float)Cache.GroupMaxHealth; }
}