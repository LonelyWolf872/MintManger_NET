using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Cache;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace LowUtilities
{
    public class Net
    {
        delegate void HTTPWebRequestHandler(string response);
        event HTTPWebRequestHandler OnResponse;
        public enum HTTPMethod
        {
            GET,
            POST
        }
        private HttpWebRequest http;
        private CookieCollection cookies;
        private CookieContainer cookie_container;
        private string _response;
        public Net(string uri, HTTPMethod method = HTTPMethod.GET, string useragent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36", string contenttype = "application/x-www-form-urlencoded", bool keepalive = false)
        {
            http = WebRequest.CreateHttp(uri);
            if(method == HTTPMethod.GET) {
                http.Method = "GET";
            } else {
                http.Method = "POST";
            }
            http.UserAgent = useragent;
            http.ContentType = contenttype;
            http.KeepAlive = keepalive;
            http.UseDefaultCredentials = true;
            cookie_container = new CookieContainer();
            http.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Default);
            http.AllowAutoRedirect = true;
            http.CookieContainer = cookie_container;
            http.ContinueTimeout = 2000;
            http.Timeout = 3000;
            cookies = new CookieCollection();
        }
        public void ChangeURL(string uri, HTTPMethod method = HTTPMethod.GET, string useragent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36", string contenttype = "application/x-www-form-urlencoded", bool keepalive = false)
        {
            http.Abort();
            http = null;
            http = WebRequest.CreateHttp(uri);
            if (method == HTTPMethod.GET)
            {
                http.Method = "GET";
            }
            else
            {
                http.Method = "POST";
            }
            http.UserAgent = useragent;
            http.ContentType = contenttype;
            http.KeepAlive = keepalive;
            http.UseDefaultCredentials = true;
            cookie_container = new CookieContainer();
            http.CachePolicy = new RequestCachePolicy(RequestCacheLevel.Default);
            http.AllowAutoRedirect = true;
            http.CookieContainer = cookie_container;
            http.ContinueTimeout = 2000;
            http.Timeout = 3000;
            cookies = new CookieCollection();
        }
        public void ChangeMethod(HTTPMethod method)
        {
            if (method == HTTPMethod.GET)
            {
                http.Method = "GET";
            }
            else
            {
                http.Method = "POST";
            }
        }
        public string GetFolder(string url)
        {
            try
            {
                Uri uri = new Uri(url);
                string tmp_path = "";
                foreach (var segment in uri.Segments)
                {
                    tmp_path += segment;
                    //Debug.WriteLine(segment);
                }
                return tmp_path;
            } catch (Exception e)
            {
                return "";
            }
        }
        public bool Send(string data)
        {
            try
            {
                if (http.Method == "POST")
                {
                    byte[] dataBytes = Encoding.ASCII.GetBytes(data);
                    http.ContentLength = dataBytes.Length;
                    Stream requestStream = http.GetRequestStream();
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                    requestStream.Close();
                    HttpWebResponse response = (HttpWebResponse)http.GetResponse();
                    _response = new StreamReader(response.GetResponseStream()).ReadToEnd();
                }
                else
                {
                    using (HttpWebResponse response = (HttpWebResponse)http.GetResponse())
                    {
                        _response = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        cookies = response.Cookies;
                    }
                }
                return true;
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message + " | " + e.StackTrace);
                return false;
            }
        }
        public void CreateFolder(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch { }
        }
        public string CheckPath(string path)
        {
            try
            {
                if (path.StartsWith('/'))
                {
                    path = "." + path;
                }
                if (path.StartsWith('\\'))
                {
                    path = "." + path;
                }
                if (path.Contains("//"))
                {
                    path = path.Replace("//", "\\");
                }
                if (path.Contains('/'))
                {
                    path = path.Replace('/', '\\');
                }
                if (path.Contains("\\\\"))
                {
                    path = path.Replace("\\\\", "\\");
                }
                return path;
            }
            catch
            {
                return null;
            }
        }
        public Image DownloadImage(string imgUrl)
        {
            try
            {
                Uri uri = new Uri(imgUrl);
                var cachePath = CheckPath(Path.Join("./Cache/", GetFolder(uri.AbsoluteUri)));
                CreateFolder(Path.GetDirectoryName(cachePath));
                //string filename = ".\\Cache\\" +  System.IO.Path.GetFileName(uri.LocalPath);
                if (File.Exists(cachePath)) return Image.FromFile(cachePath);
                WebClient webClient = new WebClient();
                byte[] data = webClient.DownloadData(imgUrl);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var yourImage = Image.FromStream(mem))
                    {
                        yourImage.Save(cachePath, ImageFormat.Png);
                        return Image.FromFile(cachePath);
                    }
                }
            } catch
            {
                return null;
            }
        }
        public string GetResponse()
        {
            return _response;
        }
        public CookieCollection GetCookies()
        {
            return cookie_container.GetCookies(http.Address);
        }
    }
}
