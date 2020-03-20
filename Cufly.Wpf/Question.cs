#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;

namespace CefSharp.MinimalExample.Wpf
{
	public class Question
	{
		public Question()
		{
			Options = new List<Option>();
		}
		protected static readonly Random ran = new Random();
		[JsonProperty("id")]
		public string? ID { get; set; }
		[JsonProperty("nid")]
		public string? NID { get; set; }
		[JsonIgnore]
		public string QuerID { get => "Quer_" + NID; }
		[JsonProperty("type")]
		public string? QuestionType { get; set; }
		[JsonProperty("title")]
		public string? Title { get; set; }
		[JsonProperty("is_store_up")]
		public string? IsStoreUp { get; set; }
		[JsonIgnore]
		public string? OptionsString { get; set; }
		[JsonProperty("options")]
		public List<Option> Options { get; set; }
		[JsonProperty("user_explain")]
		public string? UserExplain { get; set; }
		[JsonProperty("answer_explain")]
		public string? AnswerExplain { get; set; }
		[JsonProperty("answer")]
		public string? Answer { get; set; }
		[JsonProperty("temp_answer")]
		public string? TAnswer { get; set; }

		public override string ToString() =>
			"ID:" + ID + "\tnID.;" + NID + "\tQuerID;" + QuerID + "\ttype:" + QuestionType + "\n";


		/// <summary>
		///https://ks.etest8.com/Members/maonikao_ajax.asp?action=question_list&question_id=463&noid=1&m=0.6
		///"maonikao_ajax.asp?action=question_list&question_id="+question_id+"&noid="+noid+"&m="+Math.random()
		/// </summary>
		/// 
		[JsonIgnore]
		public string QuerString
		{
			get => @"https://ks.etest8.com/Members/maonikao_ajax.asp?action=question_list&question_id=" + ID + "&noid=" + NID + "&m=" + ran.NextDouble();
		}

		//public string GetHtml()
		//{
		//	HttpWebRequest myReq =
		//	(HttpWebRequest)WebRequest.Create(QuerString);
		//	HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();
		//	// Get the stream associated with the response.
		//	Stream receiveStream = response.GetResponseStream();

		//	// Pipes the stream to a higher level stream reader with the required encoding format. 
		//	StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

		//	return readStream.ReadToEnd();
		//}



		public void AnalyseTitle(
			string CurrentTestID,
			CookieCollection cookies)
		{

			HtmlParser parser = new HtmlParser();
			var doc = parser.ParseDocument(Title);
			ModifyImage(doc, CurrentTestID, cookies);
			var list = doc.GetElementsByTagName("font");
			StringBuilder sb = new StringBuilder();
			foreach (var i in list)
			{
				sb.Append(@"<p>");
				sb.Append(i.InnerHtml);
				sb.Append(@"</p>");
			}
			Title = sb.ToString();
		}

		public void AnalyseContent(
			string CurrentTestID,
			CookieCollection cookies)
		{
			HtmlParser parser = new HtmlParser();
			var doc = parser.ParseDocument(OptionsString);
			ModifyImage(doc, CurrentTestID,cookies);
			var list = doc.GetElementsByTagName("li");
			foreach (var i in list)
			{
				Options.Add(new Option(i.InnerHtml));
			}
		}

		private void ModifyImage(
			IDocument doc,
			string CurrentTestID,
			CookieCollection cookies)
		{
			var list = doc.GetElementsByTagName("img");
			foreach (var i in list)
			{
				string url = i.GetAttribute("src");
				Helper.HttpDownload(url, Helper.baseUrl + "\\" + Helper.baseImageUrl + "\\" + CurrentTestID, cookies);
				string fileName = Path.GetFileName(url);
				i.SetAttribute("src", Helper.baseImageUrl + "/" + CurrentTestID + @"/" + fileName);
			}
		}

		public void QueryQuestion(Test test,CookieCollection cookies)
		{
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(cookies);
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(QuerString);
			req.Method = "GET";
			req.CookieContainer = cookieContainer;
			using (WebResponse wr = req.GetResponse())
			{
				StreamReader sr = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding("GB2312"));
				string ht = sr.ReadToEnd();
				//htr = ht.Replace("\"", "\\\"").Replace("'", "\"");
				var wq = JsonConvert.DeserializeObject<WebResQueation>(ht, Helper.defaultJsonsettings);
				if (wq != null)
				{
					Title = wq.Title;
					OptionsString = wq.OptionsString;
					IsStoreUp = wq.IsStoreUp;
					QuestionType = wq.QuestionType;
					UserExplain = wq.UserExplain;
					AnswerExplain = wq.AnswerExplain;
					AnalyseTitle(test.NID, cookies);
					AnalyseContent(test.NID, cookies);
				}
			}
		}
	}
}
