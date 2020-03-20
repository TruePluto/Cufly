#nullable enable
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;

namespace CefSharp.MinimalExample.Wpf
{
	public class Test
	{
		public string NID { get; set; } = string.Empty;

		public string ID { get; set; } = string.Empty;
		public string? Name { get; set; }

		private const string PreUri = @"https://ks.etest8.com/Members/lianxi?DBTpye=&cateid=";
		[JsonIgnore]
		public string URI { get => PreUri + ID; }

		public void AnalyseTest(string html)
		{
			var questions = QueryTest(html);
			PatchQuestions(questions);

			string path = Helper.baseUrl + "\\" + NID + ".json";
			var s = JsonConvert.SerializeObject(questions, Formatting.Indented, Helper.defaultJsonsettings);
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.Write(s);
			}
		}

		public List<Question> QueryTest(string html)
		{
			HtmlParser parser = new HtmlParser();
			var doc = parser.ParseDocument(html);
			var lis = doc.GetElementsByTagName("li");
			//var lis = doc.All.Where(m => m.TagName == "li");
			List<Question> questions = new List<Question>();
			foreach (var i in lis)
			{
				//<li lang="ABCDE" class="card_NoAnswer" question_id="3660" nid="101" Tlang="" typeid="16"  id="Quer_101">101</li>
				if (string.Compare(i.GetAttribute("class"), "card_NoAnswer") == 0 ||
					string.Compare(i.GetAttribute("class"), "card_NoAnswer xuanzhong") == 0)//class="card_NoAnswer"
				{
					Question question = new Question
					{
						Answer = i.GetAttribute("lang"),
						TAnswer = i.GetAttribute("Tlang"),
						ID = i.GetAttribute("question_id"),
						NID = i.GetAttribute("nid"),
						QuestionType = i.GetAttribute("typeid"),
					};
					questions.Add(question);
				}
			}
			return questions;
		}
		public List<Question> PatchQuestions(List<Question> questions)
		{
			var cookieContainer = new CookieContainer();
			var cookies = Helper.GetCookieCollection();
			cookieContainer.Add(cookies);
			foreach (var item in questions)
			{
			}
			return questions;
		}
	}
}
