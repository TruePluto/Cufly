#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using AngleSharp.Dom;
using AngleSharp.Html.Parser;

using Newtonsoft.Json;

namespace CefSharp.MinimalExample.Wpf
{
	public partial class MainWindow
	{
		public Subject currentSubject = new Subject();

		public MainWindow()
		{
			InitializeComponent();
			//wb.FrameLoadEnd += WebBrowserFrameLoadEnded;
			//wb.Address = "http://www.racingpost.com/horses2/cards/card.sd?race_id=644222&r_date=2016-03-10#raceTabs=sc_";
			//progressBar.Visibility = System.Windows.Visibility.Collapsed;
			//加载开始事件
#pragma warning disable CS8622 // 参数类型中引用类型的为 Null 性与目标委托不匹配。
			browser0.FrameLoadStart += Browser_FrameLoadStart;
#pragma warning restore CS8622 // 参数类型中引用类型的为 Null 性与目标委托不匹配。

			//加载完成后事件
#pragma warning disable CS8622 // 参数类型中引用类型的为 Null 性与目标委托不匹配。
			browser0.FrameLoadEnd += Browser_FrameLoadEnd;
#pragma warning restore CS8622 // 参数类型中引用类型的为 Null 性与目标委托不匹配。
		}

		#region Browser_Frame
		private void Browser_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
		{
		}

		private void Browser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
		{
			var cookieManager = Cef.GetGlobalCookieManager();
			CookieVisitor visitor = new CookieVisitor();
			visitor.SendCookie += Visitor_SendCookie;
			cookieManager.VisitAllCookies(visitor);
		}

		private void Visitor_SendCookie(Cookie obj)
		{
			Helper.cookies += obj.Domain.TrimStart('.') + "^" + obj.Name + "^" + obj.Value + "$";
		}
		#endregion Browser_Frame
		private async void btnAnalyseTest_Click(object sender, RoutedEventArgs e)
		{
			if (currentSubject.CurrentTest == null)
			{
				MessageBox.Show("选择一个试卷");
				return;
			}
			var html = await browser0.GetSourceAsync();
			currentSubject.CurrentTest.AnalyseTest(html);
		}
		#region Subjects

		private void btnLoadSubject_Click(object sender, RoutedEventArgs e)
		{
			testList.ItemsSource = null;
			currentSubject.Load();
			testList.ItemsSource = currentSubject.TestList;
		}

		private void btnSaveSubject_Click(object sender, RoutedEventArgs e)
		{
			currentSubject.Save();
		}

		private void btnAnalyseSubject_Click(object sender, RoutedEventArgs e)
		{
			testList.ItemsSource = null;
			currentSubject.AnalyseSubject();
			testList.ItemsSource = currentSubject.TestList;
		}

		#endregion Subjects

		#region Test
		private void LoadTestList()
		{
		}
		private void AnalyseTest(string html)
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
			//"https://ks.etest8.com/Members/lianxi?DBTpye=&cateid=74FA37C90D56A63E2D3BC210ED8D7C28"
			//List<string> htmls = new List<string>(questions.Count);
			//for (int i = 0; i < questions.Count; i++)
			//{
			//	htmls.Add(string.Empty);
			//}

			var cookies = Helper.GetCookieCollection();
			var cookieContainer = new CookieContainer();
			cookieContainer.Add(cookies);
			for (int i = 8; i < 9; i++)
			{
				Question q = questions[i];
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(q.QuerString);
				req.Method = "GET";
				req.CookieContainer = cookieContainer;
				using (WebResponse wr = req.GetResponse())
				{
					StreamReader sr = new StreamReader(wr.GetResponseStream(), Encoding.GetEncoding("GB2312"));
					string ht = sr.ReadToEnd();
					//htr = ht.Replace("\"", "\\\"").Replace("'", "\"");
					var wq = JsonConvert.DeserializeObject<WebResQueation>(ht, Helper.defaultJsonsettings);
					q.Title = wq.Title;
					q.OptionsString = wq.OptionsString;
					q.IsStoreUp = wq.IsStoreUp;
					q.QuestionType = wq.QuestionType;
					q.UserExplain = wq.UserExplain;
					q.AnswerExplain = wq.AnswerExplain;
					q.AnalyseTitle(currentSubject.CurrentTest.NID, cookies);
					q.AnalyseContent(currentSubject.CurrentTest.NID, cookies);
				}
			}

			string path = Helper.baseUrl + "\\" + currentSubject.CurrentTest.NID + ".json";
			var s = JsonConvert.SerializeObject(questions, Formatting.Indented, Helper.defaultJsonsettings);
			using (StreamWriter sw = new StreamWriter(path))
			{
				sw.Write(s);
			}
		}


		#endregion Test
		#region testList
		//private Test CurrentTest;
		private void TestList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			IList newItems = e.AddedItems;
			if (newItems.Count > 0)
			currentSubject.CurrentTest = newItems[0] as Test;
			if (currentSubject.CurrentTest != null)
				browser0.Address = currentSubject.CurrentTest.URI;
		}

		#endregion testList

		private void TestButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Helper");

		}
	}
}