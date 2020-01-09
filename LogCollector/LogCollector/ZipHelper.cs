using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;

namespace LogCollector
{
    class ZipHelper
    {
        public void CreateZipArchive(string zipFileName, string srcFolder, string fileFilter, string folderFilter )
        {
            FastZip zip = new FastZip();
            // arg1: Zipファイル名
            // arg2: アーカイブに含めるファイル
            // arg3: 再帰
            // arg4: ｢アーカイブに含めるファイルのパターン｣を正規表現で記述する。
            // arg5: ｢アーカイブに含めるディレクトリのパターン｣を正規表現で記述する。
            zip.CreateZip( zipFileName, srcFolder, true, fileFilter, folderFilter );
        }

        public void ExtractZipArchive(string zipFileName, string extFolder, string fileFilter)
        {
            FastZip zip = new FastZip();
            zip.ExtractZip( zipFileName, extFolder, fileFilter );
        }
    }
}
