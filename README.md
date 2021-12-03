# ZKPicDown
站酷网图片设置主页地址下载该主页下所有图片地址

* .Net 6.0 控制台应用程序实现的一个站酷网图片下载工具

1. zk网输入某个主页地址 如

   [zk]: https://www.zcool.com.cn/collection/ZMzc2MzczNDg=	"主页地址"

2. 主页里面的相册列表名称 作为该合集文件夹名称

3. 图片名称使用 Guid.NewGuid生成

4. 使用了HtmlAgilityPack 解析Html中节点

5. AgileHttp 发送http请求

   虽说是.Net 6 但没有多少.Net 6的东西，.Net 5也可以跑，3.1亦可，2.0也不是不行

   **^__^**
