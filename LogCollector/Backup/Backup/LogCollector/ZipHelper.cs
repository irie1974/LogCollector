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
            // arg1: Zip�t�@�C����
            // arg2: �A�[�J�C�u�Ɋ܂߂�t�@�C��
            // arg3: �ċA
            // arg4: ��A�[�J�C�u�Ɋ܂߂�t�@�C���̃p�^�[����𐳋K�\���ŋL�q����B
            // arg5: ��A�[�J�C�u�Ɋ܂߂�f�B���N�g���̃p�^�[����𐳋K�\���ŋL�q����B
            zip.CreateZip( zipFileName, srcFolder, true, fileFilter, folderFilter );
        }

        public void ExtractZipArchive(string zipFileName, string extFolder, string fileFilter)
        {
            FastZip zip = new FastZip();
            zip.ExtractZip( zipFileName, extFolder, fileFilter );
        }
    }
}
