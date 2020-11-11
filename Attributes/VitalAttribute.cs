using sisbase.Systems;
using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace sisbase.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VitalAttribute : Attribute
    {

    }

    public static class VitalExtensions
    {
        public static bool IsVital(this BaseSystem t)
         => t.GetType().GetCustomAttribute<VitalAttribute>() != null;
    }
}
