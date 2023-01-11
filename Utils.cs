using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AgileHttp;
using Flurl.Http;
using HtmlAgilityPack;

namespace ZhanKuImgDownLoadProject
{
    public static class Utils
    {
        public static async Task CopyToLocalAsync(string picUrl, string title)
        {
            // var picture = (await picUrl.AsHttpClient().GetAsync())
            //     .Response
            //     .GetResponseStream();

            var picture = await picUrl.GetStreamAsync();

            var basePath = Path.Combine(Environment.CurrentDirectory, "壁纸");

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
        public static async Task<List<string>> GetHref()
        {
            const string url = "https://www.zcool.com.cn/collection/ZMzc2MzczNDg=";

            var html = await url
                .WithHeader("referer", "https://www.zcool.com.cn/collection/ZMzc2MzczNDg=")
                .WithHeader("user-agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36")
                .GetStringAsync();


            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            HtmlNodeCollection htmlNodeCollection =
                htmlDocument.DocumentNode.SelectNodes(
                    "//div[@class='sc-16jldha-0 gACMOV']/div[1]");

            var cardImg = htmlNodeCollection
                .Select(x => x.SelectNodes("//div[@class='cardImg']"))
                .ToList();

            var cardImgData = new List<string>();
            foreach (HtmlNodeCollection nodeCollection in cardImg)
            {
                var result = nodeCollection.Select(htmlNode =>
                        htmlNode.SelectSingleNode(".//a").Attributes["href"].Value)
                    .ToList();
                cardImgData.AddRange(result);
            }

            return cardImgData;
        }
    }
}