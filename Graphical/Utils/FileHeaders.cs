using System.Collections.Generic;

namespace Graphical.Utils
{
    internal class FileHeaders
    {
        public static Dictionary<string, string[]> Headers = new Dictionary<string, string[]>()
        {
            { "504B0304", new string[]{ "zip", "zip archive", "*.zip;*.aar;*.apk;*.docx;*.epub;*.ipa;*.jar;*.kmz;*.maff;*.msix;*.odp;*.ods;*.odt;*.pk3;*.pk4;*.pptx;*.usdz;*.vsdx;*.xlsx;*.xpi" } },
            { "504B0506", new string[]{ "zip", "empty zip archive", "*.zip" } }, // 空zip压缩文件
            { "504B0708", new string[]{ "z", "spanned zip archive(*.z*)(eg:xx.z01)", "*.z*" } }, // 分卷zip压缩文件(*.z*)(eg:xx.z01)
            { "FFD8FFDB", new string[]{ "jpg", "jpg image", "*.jpg;*.jpeg" } },
            { "FFD8FFE0", new string[]{ "jpg", "jpg image", "*.jpg;*.jpeg" } },
            { "FFD8FFE1", new string[]{ "jpg", "jpg image", "*.jpg;*.jpeg" } },
            { "FFD8FFEE", new string[]{ "jpg", "jpg image", "*.jpg;*.jpeg" } },
            { "89504E47", new string[]{ "png", "png image", "*.png" } },
            { "47494638", new string[]{ "gif", "gif image", "*.gif" } }
        };
    }
}
