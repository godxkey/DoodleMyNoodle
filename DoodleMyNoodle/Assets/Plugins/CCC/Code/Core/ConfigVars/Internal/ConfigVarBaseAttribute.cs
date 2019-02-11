using System;

namespace CCC.ConfigVarInterals
{
    public abstract class ConfigVarBaseAttribute : Attribute
    {
        protected string name = null;
        protected string defaultValue = "";
        protected int flags = 0;
        protected string description = "";

        public string Name => name;
        public string DefaultValue => defaultValue;
        public int Flags => flags;
        public string Description => description;
    }
}