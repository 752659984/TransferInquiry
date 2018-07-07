using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TransferInquiry.Command
{
    public class HtmlHelp
    {
        public static string cookieString = "";

        public static string GetHtmlString(string url, Encoding encoding)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream stream = null;
            StreamReader sr = null;
            Task<WebResponse> resultAsync = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Host = "kyfw.12306.cn";
                request.Accept = "*/*";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
                //request.Headers["Upgrade-Insecure-Requests"] = "1";
                request.Referer = "https://kyfw.12306.cn/otn/leftTicket/init";
                //request.Headers["X-Requested-With"] = "XMLHttpRequest";
                //request.Headers["Cache-Control"] = "no-cache";

                request.Headers["Cookie"] = cookieString;
                //request.Headers["Cookie"] = "JSESSIONID=7A33CB91E6591D8D586793DBA0DEA552; RAIL_EXPIRATION=1516363783227; RAIL_DEVICEID=s6W3fOgzj_MmB1owwagwhmSiQztK-oRQGNyU8dcdG-TH5z1eZVQk_hOihQj16rckcMjAYOOwVZKKxZkQwRnz_WpLSsqTb5w6Ki4LEYLzgGkcV5NgosTXqjHiJQCUcB_bEMLPCkGCkHcYMOudRNTCiFfS9B1INZsJ; route=6f50b51faa11b987e576cdb301e545c4; BIGipServerotn=1088946442.64545.0000; BIGipServerpassport=803733770.50215.0000; _jc_save_fromStation=%u5E7F%u5DDE%2CGZQ; _jc_save_toStation=%u8D35%u6E2F%2CGGZ; _jc_save_fromDate=2018-02-15; _jc_save_toDate=2018-01-17; _jc_save_wfdc_flag=dc";
                request.CookieContainer = new CookieContainer();

                response = (HttpWebResponse)request.GetResponse();
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                }
                else
                {
                    stream = response.GetResponseStream();
                }

                resultAsync = request.GetResponseAsync();
                if (resultAsync.Result.Headers.AllKeys.Contains("Set-Cookie"))
                {
                    var cs3 = resultAsync.Result.Headers.GetValues("Set-Cookie").ToList();
                    foreach (var s in cs3)
                    {
                        cookieString += s.Replace("Path=/", "").Replace("path=/", "");
                    }
                }

                sr = new StreamReader(stream, encoding);
                var result = sr.ReadToEnd();

                return result;
            }
            catch (Exception e)
            {
                return "";
            }
            finally
            {
                if (sr != null)
                {
                    sr.Dispose();
                }
                if (stream != null)
                {
                    stream.Dispose();
                }
                if (resultAsync != null)
                {
                    resultAsync.Dispose();
                }
                if (response != null)
                {
                    response.Dispose();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        /// <summary>
        /// 下载指定url的文件到指定的路径
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filepath"></param>
        /// <param name="maxsize"></param>
        /// <param name="allowAutoRedirect"></param>
        /// <param name="dic"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool DownFile(string url, string filepath, int maxsize, bool allowAutoRedirect)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            FileStream filestream = null;
            Stream stream = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.AllowAutoRedirect = allowAutoRedirect;

                response = (HttpWebResponse)request.GetResponse();

                filestream = new FileStream(filepath, FileMode.Create);

                stream = response.GetResponseStream();

                byte[] buff = new byte[maxsize];

                int n = 0;
                while ((n = stream.Read(buff, 0, buff.Length)) > 0)
                {
                    filestream.Write(buff, 0, n);
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                if (request != null)
                    request.Abort();

                if (response != null)
                    response.Dispose();

                if (filestream != null)
                    filestream.Dispose();

                if (stream != null)
                    stream.Dispose();
            }
        }

        /// <summary>
        /// 查看url是否需要host
        /// </summary>
        /// <param name="url">http://www.baidu.com/abc</param>
        /// <param name="currUrl">/bcd</param>
        /// <returns></returns>
        public static string NeedHost(string url, string currUrl)
        {
            try
            {
                url = url[url.Length - 1] != '/' ? url + "/" : url;
                url = url.Replace("\\", "/");
                currUrl = currUrl.Replace("\\", "/");

                if (currUrl[0].ToString() == "/")
                {
                    Regex reg = new Regex("http[s]{0,1}://.*?/");
                    url = reg.Match(url).Value;
                }

                var resultUrl = "";

                if (!(currUrl.Contains("http://") || currUrl.Contains("https://")) && !currUrl.Contains("www."))
                    resultUrl = url.Substring(0, url.LastIndexOf("/")) + "/" + currUrl;
                else
                    resultUrl = currUrl;

                return resultUrl.Replace("//", "/").Replace(":/", "://");
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("url：{0}，currUrl：{1}，message：{2}", url, currUrl, e.Message));
            }
        }

        /// <summary>
        /// 保存文本到指定的路径
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="txt"></param>
        /// <param name="isAppend"></param>
        public static void SaveTxtFile(string filePath, string txt, bool isAppend)
        {
            using (var sw = new StreamWriter(filePath, isAppend))
            {
                sw.Write(txt);
            }
        }

        /// <summary>
        /// 读取指定的文本文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string LoadTxtFile(string filePath)
        {
            using (var fs = new StreamReader(filePath))
            {
                return fs.ReadToEnd();
            }
        }
    }
}
