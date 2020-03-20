#nullable enable
using Newtonsoft.Json;

namespace CefSharp.MinimalExample.Wpf
{
	public class WebResQueation
	{
		[JsonProperty("title")]
		public string? Title { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("content")]
		public string? OptionsString { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("questio_type")]//questioN_type ???
		public string? QuestionType { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("is_storeUp")]
		public string? IsStoreUp { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("user_explain")]
		public string? UserExplain { get; set; }
		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("answer_explain")]
		public string? AnswerExplain { get; set; }
	}
}
