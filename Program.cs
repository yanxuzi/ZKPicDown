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
            var hrefs = GetHref();

            var srcs = GetSrcs(hrefs);

            for (var i = 0; i < srcs.Count; i++)
            {
                CopyToLocal(srcs[i], i);
            }
        }

        static void CopyToLocal(string picUrl, int num)
        {
            var picture = picUrl.AsHttpClient()
                .Get()
                .Response
                .GetResponseStream();

            const string path = @"E:\站酷图片\";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using var fs = new FileStream($"{path}{num}.jpg", FileMode.Create);
            picture.CopyTo(fs);
            fs.Flush();
        }

        static List<string> GetSrcs(List<string> hrefs)
        {
            var srcLists = new List<string>();
            foreach (var href in hrefs)
            {
                var content = href.AsHttpClient()
                    .Get()
                    .GetResponseContent();

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);
                HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes(
                    "//div[@class='work-show-box mt-40 js-work-content']/div[1]/div['photo-information-content']");

                foreach (var htmlNode in htmlNodeCollection)
                {
                    var picUrl = htmlNode.SelectSingleNode(".//img").Attributes["src"].Value;
                    srcLists.Add(picUrl);
                }
            }

            return srcLists;
        }

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