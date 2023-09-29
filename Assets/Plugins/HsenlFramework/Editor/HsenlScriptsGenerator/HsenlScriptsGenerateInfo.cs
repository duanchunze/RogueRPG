namespace Hsenl {
    public class HsenlScriptsGenerateInfo {
        // 这三个参数可以设置也不设置, 不设置的话, 就会使用Scheme里的通用的参数
        public string templatePath; // 模板路径
        public string templateReplaceOrigianl; // 被取代源
        public string outputDirectory; // 输出目录


        public string templateReplaceContent { get; private set; } // 替代内容
        public string outputFileName { get; private set; } // 输出的文件名(记得带后缀名)

        private HsenlScriptsGenerateInfo() { }

        public HsenlScriptsGenerateInfo(string replaceContent, string fileName) {
            // 这两个参数是必须给的
            this.templateReplaceContent = replaceContent;
            this.outputFileName = fileName;
        }
    }
}