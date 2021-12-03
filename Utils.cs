using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AgileHttp;
using HtmlAgilityPack;

namespace ZhanKuImgDownLoadProject
{
    public static class Utils
    {
        public static async Task CopyToLocalAsync(string picUrl, string title)
        {
            var picture = (await picUrl.AsHttpClient().GetAsync())
                .Response
                .GetResponseStream();

            const string basePath = @"E:\站酷图片\";

            var path = Path.Combine(basePath, title.Replace("|", ""));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = Guid.NewGuid();
            await using var fs = new FileStream(@$"{path}\{fileName}.jpg", FileMode.CreateNew, FileAccess.Write);
            await picture.CopyToAsync(fs);
            Console.WriteLine($"{title}-{picUrl} complete...");
        }

        /// <summary>
        /// 图片链接
        /// </summary>
        /// <param name="hrefs"></param>
        /// <returns></returns>
        public static List<Dictionary<string, List<string>>> GetImgSrc(List<string> hrefs)
        {
            var result = new List<Dictionary<string, List<string>>>();
            foreach (var href in hrefs)
            {
                var content = href.AsHttpClient()
                    .Get()
                    .GetResponseContent();

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(content);
                var documentNode = htmlDocument.DocumentNode;

                // 网站标题 文件夹名称
                var title = documentNode.SelectSingleNode("//h2").InnerText.Trim().Replace(" ", "");

                var htmlNodeCollection = documentNode.SelectNodes("//div[@class='photo-information-content']");

                var srcLists = htmlNodeCollection
                    .Select(htmlNode =>
                        htmlNode.SelectSingleNode(".//img").Attributes["src"].Value)
                    .ToList();

                // 详情页 多张图片
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
        public static List<string> GetHref()
        {
            const string url = "https://www.zcool.com.cn/collection/ZMzc2MzczNDg=";

            var html = url.AsHttpClient()
                .Get()
                .GetResponseContent();

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            HtmlNodeCollection htmlNodeCollection =
                htmlDocument.DocumentNode.SelectNodes(
                    "//div[@class='left p-relative js-item']/div[1]/div[@class='card-img']");

            return htmlNodeCollection.Select(htmlNode =>
                    htmlNode.SelectSingleNode(".//a").Attributes["href"].Value)
                .ToList();
        }
    }
}