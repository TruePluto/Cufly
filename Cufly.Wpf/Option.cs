#nullable enable
using System.Text.RegularExpressions;

namespace CefSharp.MinimalExample.Wpf
{
	public class Option
	{
		public string? ID { get; set; }
		public string? Content { get; set; }
		private static Regex pat = new Regex(@"^(?<ID>\S)\.(?<Content>.*)$");

		public Option()
		{ }

		public Option(string innerHtml)
		{
			Match pathMatch = pat.Match(innerHtml);
			if (pathMatch.Success)
			{
				ID = pathMatch.Groups["ID"].Value;
				Content = pathMatch.Groups["Content"].Value;
			}
		}
	}
}
