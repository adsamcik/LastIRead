using System;
using System.Collections.Generic;
using System.Linq;

namespace LastIRead.tools {
	public static class ReflectionTools {
		public static List<T> GetImplementors<T>() =>
			AppDomain.CurrentDomain
			         .GetAssemblies()
			         .SelectMany(x => x.GetTypes())
			         .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
			         .Select(CreateInstance<T>)
			         .ToList();

		private static T CreateInstance<T>(Type type) =>
			(T) (Activator.CreateInstance(type) ??
			     throw new NullReferenceException($"Failed to create instance of type {type.FullName}"));
	}
}