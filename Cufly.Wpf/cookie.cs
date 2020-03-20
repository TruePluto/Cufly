using System;
using System.Collections.Generic;
using System.Text;

namespace CefSharp.MinimalExample.Wpf
{
    public class CookieVisitor : CefSharp.ICookieVisitor
    {
        public event Action<CefSharp.Cookie> SendCookie;

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public bool Visit(CefSharp.Cookie cookie, int count, int total, ref bool deleteCookie)
        {
            deleteCookie = false;
            SendCookie?.Invoke(cookie);

            return true;
        }
    }
}