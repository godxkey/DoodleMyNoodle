
namespace CCC.ConfigVarInterals
{
    public abstract class ConfigVarBase
    {
        public static int DirtyFlagsRaw = 0;

        public string name;
        public string description;
        public int rawFlags;
        public bool changed;

        string _stringValue;
        float _floatValue;
        int _intValue;

        public ConfigVarBase(string name, string description, int flags = 0)
        {
            this.name = name;
            this.rawFlags = flags;
            this.description = description;
        }

        public virtual string Value
        {
            get { return _stringValue; }
            set
            {
                if (_stringValue == value)
                    return;

                // change flags
                DirtyFlagsRaw |= rawFlags;

                _stringValue = value;

                if (!int.TryParse(value, out _intValue))
                {
                    // failed to parse int
                    _intValue = 0;
                }

                if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _floatValue))
                {
                    //failed to parse float
                    _floatValue = 0;
                }

                changed = true;
            }
        }

        public int IntValue => _intValue;
        public float FloatValue => _floatValue;

        public bool ChangeCheck()
        {
            if (!changed)
                return false;
            changed = false;
            return true;
        }
    }
}