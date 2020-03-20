#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;

namespace CefSharp.MinimalExample.Wpf
{
	public class Subject
	{
		public Test? CurrentTest { get; set; }
		public List<Test> TestList { get; set; } = new List<Test>();

		public void ExtractSubject(string html)
		{
			List<string> UID = new List<string>();
			List<string> Names = new List<string>();
			HtmlParser parser = new HtmlParser();
			var doc = parser.ParseDocument(html);
			var list = doc.GetElementsByTagName("a");
			foreach (var i in list)
			{
				if (string.Compare(i.GetAttribute("onclick"), "ks_tishi();") == 0 ||
					string.Compare(i.GetAttribute("onclick"), "return STchongkao();") == 0)
				{
					//maonikao_action? action = chongkao & cateid = DB A8DD8 4D9E7 5CFE7 4D002 5D809D6042
					string herf = i.GetAttribute("href");
					string TID = herf.Substring(herf.Length - 32, 32);
					UID.Add(TID);
				}
			}
			foreach (var i in list)
			{
				if (string.Compare(i.GetAttribute("href"), "#") == 0 && i.ChildElementCount == 0)
				{
					string name = i.Text();
					Names.Add(name);
				}
			}
			if (UID.Count == Names.Count)
			{
				TestList.Clear();
				for (int i = 0; i < UID.Count; i++)
				{
					TestList.Add(new Test() { NID = (i + 1).ToString(), ID = UID[i], Name = Names[i] }); ;
				}
			}
		}

		public void AnalyseSubject()
		{
			string html;
			using (StreamReader sr = new StreamReader(Helper.baseUrl + @"\SubjectsRaw.txt"))
			{
				html = sr.ReadToEnd();
			}
			ExtractSubject(html);
			Save();
		}

		public void Load()
		{
			using (StreamReader sr = new StreamReader(Helper.baseUrl + @"\SubjectList.json"))
			{
				string json = sr.ReadToEnd();
				var tlist = JsonConvert.DeserializeObject<List<Test>>(json, Helper.defaultJsonsettings);
				if (tlist != null)
					TestList = tlist;
			}
		}

		public void Save()
		{
			using (StreamWriter sw = new StreamWriter(Helper.baseUrl + @"\SubjectList.json"))
			{
				sw.Write(JsonConvert.SerializeObject(TestList, Formatting.Indented, Helper.defaultJsonsettings));
			}
		}

	}
}
