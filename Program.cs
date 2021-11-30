using System;
using System.Collections.Generic;
using System.IO;
using AgileHttp;
using HtmlAgilityPack;

namespace PictureDownLoadProject
{
    class Program
    {
        static void Main(string[] args)
        {
            // 主页
            var hrefs = GetHref();
            // 详情页
            var result = GetSrcs(hrefs);

            // 所有主页
            foreach (var dictionary in result)
            {
                // 单个详情页
                foreach (var itemUrl in dictionary)
                {
                    foreach (var url in itemUrl.Value)
                    {
                        CopyToLocal(url, itemUrl.Key);
                    }
                }
            }


            Console.WriteLine("下载完成");
        }

        static void CopyToLocal(string picUrl, string title)
        {
            var picture = picUrl.AsHttpClient()
                .Get()
                .Response
                .GetResponseStream();

            const string basePath = @"E:\站酷图片\";

            var path = Path.Combine(basePath, title.Replace("|", ""));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var dt = DateTime.Now;
            var fileName = $"{dt.Year}-{dt.Month}-{dt.Day}-{dt.Hour}-{dt.Minute}-{dt.Second}.jpg";

            using var fs = new FileStream(@$"{path}\{fileName}", FileMode.Create);
            picture.CopyTo(fs);
            fs.Flush();
        }

        /// <summary>
        /// 图片链接
        /// </summary>
        /// <param name="hrefs"></param>
        /// <returns></returns>
        static List<Dictionary<string, List<string>>> GetSrcs(List<string> hrefs)
        {
            var result = new List<Dictionary<string, List<string>>>();
            foreach (var href in hrefs)
            {
                var srcLists = new List<string>();
                var content = href.AsHttpClient()
                    .Get()
                    .GetResponseContent();

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);
                var documentNode = htmlDocument.DocumentNode;

                // 网站标题作为文件夹名称
                var title = documentNode.SelectSingleNode("//h2").InnerText.Trim().Replace(" ", "");

                var htmlNodeCollection = documentNode.SelectNodes("//div[@class='photo-information-content']");

                foreach (var htmlNode in htmlNodeCollection)
                {
                    var picUrl = htmlNode.SelectSingleNode(".//img").Attributes["src"].Value;
                    srcLists.Add(picUrl);
                }

                // 一个主页对应一个详情页多张图片
                var data = new Dictionary<string, List<string>>
                {
                    {
                        title, srcLists
                    }
                };
                result.Add(data);
            }


            return result;
        }


        /// <summary>
        /// 外层链接
        /// </summary>
        /// <returns></returns>
        static List<string> GetHref()
        {
            var hrefList = new List<string>();
            var url = "https://www.zcool.com.cn/collection/ZMzc2MzczNDg=";

            var html = url.AsHttpClient()
                .Get()
                .GetResponseContent();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            HtmlNodeCollection htmlNodeCollection =
                htmlDocument.DocumentNode.SelectNodes(
                    "//div[@class='left p-relative js-item']/div[1]/div[@class='card-img']");

            foreach (HtmlNode htmlNode in htmlNodeCollection)
            {
                var hrefStr = htmlNode.SelectSingleNode(".//a").Attributes["href"].Value;
                hrefList.Add(hrefStr);
            }

            return hrefList;
        }
    }
}