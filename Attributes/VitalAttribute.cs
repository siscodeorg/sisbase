using sisbase.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using sisbase.Systems;

namespace sisbase.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class VitalAttribute : Attribute
	{

	}

	public static class VitalExtensions
	{
		public static bool IsVital(this ISystem t)
		 => t.GetType().GetCustomAttribute<VitalAttribute>() != null;
	}
}
