using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LastIRead.resources {
	public static class ResourceLoader {
		public static string[] LoadTextResource(string path) {
			// Determine path
			var assembly = Assembly.GetExecutingAssembly();
			var resourceNames = assembly.GetManifestResourceNames();
			path = $"{assembly.GetName().Name}.{path}";
			var resourcePath = resourceNames.Where(x => x.StartsWith(path)).OrderBy(x => x).ToArray();

			var result = new string[resourcePath.Length];
			for (var i = 0; i < result.Length; i++) {
				using var stream = assembly.GetManifestResourceStream(resourcePath[i]);
				using var reader = new StreamReader(stream ?? throw new NullReferenceException());
				result[i] = reader.ReadToEnd();
			}

			return result;
		}
	}
}