#nullable enable
using System;
using System.IO;
using System.Net;

using Newtonsoft.Json;

namespace CefSharp.MinimalExample.Wpf
{
	public static class Helper
	{
		public static string baseUrl = @"D:\Kaoshi";
		public static string baseImageUrl = @"Images";
		public readonly static JsonSerializerSettings defaultJsonsettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Auto,
			TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
			PreserveReferencesHandling = PreserveReferencesHandling.None,
			Formatting = Formatting.Indented,
		};

		public static bool HttpDownload(string url, string foldPath, CookieCollection cookies)
		{
			if (url == null)
				return false;
			try
			{
				string fileName = Path.GetFileName(url);
				string filePath = foldPath + "\\" + fileName;
				if (!Directory.Exists(foldPath))
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(foldPath);
					directoryInfo.Create();
				}
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
				var cookieContainer = new CookieContainer();
				cookieContainer.Add(cookies);
				if (!(WebRequest.Create(url) is HttpWebRequest request)) return false;
				request.CookieContainer = cookieContainer;

				if (!(request.GetResponse() is HttpWebResponse response)) return false;
				using (Stream responseStream = response.GetResponseStream())
				{
					using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
					{
						byte[] bArr = new byte[1024];
						int iTotalSize = 0;
						int size = responseStream.Read(bArr, 0, (int)bArr.Length);
						while (size > 0)
						{
							iTotalSize += size;
							fs.Write(bArr, 0, size);
							size = responseStream.Read(bArr, 0, (int)bArr.Length);
						}
					}
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		public static string cookies = "";

		public static CookieCollection GetCookieCollection()
		{
			CookieCollection cookieCollection = new CookieCollection();
			string[] arr = cookies.Split('$');
			foreach (string s in arr)
			{
				if (string.IsNullOrEmpty(s))
					continue;
				string[] car = s.Split('^');
				System.Net.Cookie cookie = new System.Net.Cookie();
				cookie.Domain = car[0];
				cookie.Name = car[1];
				cookie.Value = car[2];
				cookieCollection.Add(cookie);
			}
			return cookieCollection;
		}



	}
}
