using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AgileHttp;
using HtmlAgilityPack;

namespace ZhanKuImgDownLoadProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 主页
            var hrefs = Utils.GetHref();
            // 详情页
            var result = Utils.GetImgSrc(await hrefs);
            // 主页 -> 详情页
            foreach (Dictionary<string, List<string>> dictionary in result)
            {
                // 单个详情页
                foreach (var (key, value) in dictionary)
                {
                    var tasks = value.Select(url =>
                        Utils.CopyToLocalAsync(url, key)
                    ).ToList();
                    await Task.WhenAll(tasks);
                    Console.WriteLine($"{key} 下载完成");
                }
            }
        }
    }
}