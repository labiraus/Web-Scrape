using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Web_Scrape
{
    public class Scraper
    {
        private readonly string _userAgent;
        private Dictionary<string, Cookie> _cookies;
        private string lastUrl;

        public Scraper(string userAgent)
        {
            _userAgent = userAgent;
            _cookies = new Dictionary<string, Cookie>();
            lastUrl = "";
        }

        public HttpWebResponse HttpGet(string url)
        {
            return Request(url, lastUrl, string.Empty, HttpMethod.GET);
        }

        public HttpWebResponse HttpPost(string url, string postData)
        {
            return Request(url, lastUrl, postData, HttpMethod.POST);
        }

        private HttpWebResponse Request(string url, string referer, string postData, HttpMethod method)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.AllowAutoRedirect = true;
            http.Method = method.ToString();
            http.UserAgent = _userAgent;
            http.CookieContainer = new CookieContainer();
            foreach (Cookie cookie in _cookies.Values)
            {
                cookie.Expires = DateTime.Today.AddDays(1);
                http.CookieContainer.Add(cookie);
            }
            http.Referer = referer;
            http.KeepAlive = true;
            http.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            switch (method)
            {
                case HttpMethod.POST:
                    {
                        http.ContentType = "application/x-www-form-urlencoded";
                        var dataBytes = Encoding.UTF8.GetBytes(postData);
                        http.ContentLength = dataBytes.Length;
                        using (var postStream = http.GetRequestStream())
                        {
                            postStream.Write(dataBytes, 0, dataBytes.Length);
                        }
                    }
                    break;
            }
            var httpResponse = (HttpWebResponse)http.GetResponse();
            this.lastUrl = httpResponse.ResponseUri.AbsoluteUri;
            if (httpResponse.Cookies.Count > 0)
            {
                foreach (Cookie cookie in httpResponse.Cookies)
                    _cookies[cookie.Name] = cookie;
                if (http.Headers.HasKeys() && http.Headers.AllKeys.Contains("Cookie"))
                    foreach (string cookieVal in http.Headers.GetValues("Cookie")[0].Split(';'))
                    {
                        string[] cookiePair = cookieVal.Trim().Split('=');
                        if (!_cookies.ContainsKey(cookiePair[0]))
                            _cookies[cookiePair[0]] = new Cookie(cookiePair[0], cookiePair[1], "/", http.Host);
                        else
                            _cookies[cookiePair[0]].Value = cookiePair[1];
                    }
            }

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.Redirect:
                    string redirect = httpResponse.Headers["Location"];
                    if (!redirect.Contains(':') && redirect.StartsWith("/"))
                    {
                        redirect = url.Substring(0, url.LastIndexOf("/")) + redirect;
                    }
                    return Request(redirect, url, string.Empty, HttpMethod.GET);
            }
            return httpResponse;
        }

        public void DownloadFile(string url, string downloadPath, string filename)
        {
            var http = (HttpWebRequest)WebRequest.Create(url);
            http.AllowAutoRedirect = true;
            http.Method = "GET";
            http.UserAgent = _userAgent;
            http.CookieContainer = new CookieContainer();
            foreach (Cookie cookie in _cookies.Values)
            {
                cookie.Expires = DateTime.Today.AddDays(1);
                http.CookieContainer.Add(cookie);
            }
            http.Referer = string.Empty;
            http.AllowAutoRedirect = false;
            var response = http.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                var buffer = new byte[32 * 1024]; //32Kb chunks
                using (var filestream = File.Create(Path.Combine(downloadPath, filename)))
                {
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        filestream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
    }

    public enum HttpMethod
    {
        GET,
        POST
    }
}
