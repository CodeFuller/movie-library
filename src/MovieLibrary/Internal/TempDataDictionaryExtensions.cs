using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MovieLibrary.Internal
{
	internal static class TempDataDictionaryExtensions
	{
		public static bool GetBooleanValue(this ITempDataDictionary tempData, string key)
		{
			if (!tempData.TryGetValue(key, out var value))
			{
				return false;
			}

			return value is bool boolValue && boolValue;
		}
	}
}
