using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TextMood
{
	public static class DebugServices
	{
		#region Constant Fields
		static Lazy<Dictionary<PathType, string>> _pathTypeSeparatorDictionary = new Lazy<Dictionary<PathType, string>>(() =>
			 new Dictionary<PathType, string>{
				{ PathType.Linux, "/" },
				{ PathType.Windows, @"\" }
			 });
		#endregion

		#region Enums
		enum PathType { Windows, Linux };
		#endregion

		#region Properties
		static Dictionary<PathType, string> PathTypeSeparatorDictionary => _pathTypeSeparatorDictionary.Value;
		#endregion

		#region Methods
		public static void Report(Exception exception,
								  IDictionary<string, string> properties = null,
								  [CallerMemberName] string callerMemberName = "",
								  [CallerLineNumber] int lineNumber = 0,
								  [CallerFilePath] string filePath = "")
		{
			PrintException(exception, callerMemberName, lineNumber, filePath);
		}

		[Conditional("DEBUG")]
		static void PrintException(Exception exception, string callerMemberName, int lineNumber, string filePath)
		{
			var fileName = GetFileNameFromFilePath(filePath);

			Debug.WriteLine(exception.GetType());
			Debug.WriteLine($"Error: {exception.Message}");
			Debug.WriteLine($"Line Number: {lineNumber}");
			Debug.WriteLine($"Caller Name: {callerMemberName}");
			Debug.WriteLine($"File Name: {fileName}");
		}

		static string GetFileNameFromFilePath(string filePath)
		{
			string fileName;

			var pathType = filePath.Contains(PathTypeSeparatorDictionary[PathType.Linux]) ? PathType.Linux : PathType.Windows;

			while (true)
			{
				if (!(filePath.Contains(PathTypeSeparatorDictionary[pathType])))
				{
					fileName = filePath;
					break;
				}

				var indexOfDirectorySeparator = filePath.IndexOf(PathTypeSeparatorDictionary[pathType], StringComparison.Ordinal);
				var newStringStartIndex = indexOfDirectorySeparator + 1;

				filePath = filePath.Substring(newStringStartIndex, filePath.Length - newStringStartIndex);
			}

			return fileName;
		}
		#endregion
	}
}
