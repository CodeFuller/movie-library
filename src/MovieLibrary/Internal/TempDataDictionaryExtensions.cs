using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MovieLibrary.Internal
{
	internal static class TempDataDictionaryExtensions
	{
		public static bool GetBooleanValue(this ITempDataDictionary tempData, string key)
		{
			return tempData.GetValue<bool>(key, default);
		}

		public static string GetStringValue(this ITempDataDictionary tempData, string key)
		{
			return tempData.GetValue<string>(key, default);
		}

		private static T GetValue<T>(this ITempDataDictionary tempData, string key, T defaultValue)
		{
			if (!tempData.TryGetValue(key, out var value))
			{
				return defaultValue;
			}

			return value is T typedValue ? typedValue : defaultValue;
		}
	}
}
