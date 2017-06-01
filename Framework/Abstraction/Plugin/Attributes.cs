using System;

namespace HakeQuick.Abstraction.Plugin
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class IdentityAttribute : Attribute
    {
        private readonly string name;

        public IdentityAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }


    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ActionAttribute : Attribute
    {
        private readonly string name;

        public ActionAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }


    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class IgnoreIdentityAttribute : Attribute
    {
        private readonly string name;

        public IgnoreIdentityAttribute(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ExplicitCallAttribute : Attribute
    {
        public ExplicitCallAttribute()
        {
        }
        
    }
}
