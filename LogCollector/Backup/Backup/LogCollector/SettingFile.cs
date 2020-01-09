using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace LogCollector
{
    /// <summary>
    /// INI�t�@�C�����[�_
    /// </summary>
    class IniFileHandler {
        [DllImport("KERNEL32.DLL")]
        public static extern uint
            GetPrivateProfileString(string lpAppName,
                                    string lpKeyName, 
                                    string lpDefault,
                                    StringBuilder lpReturnedString, 
                                    uint nSize,
                                    string lpFileName);

        [DllImport("KERNEL32.DLL", EntryPoint="GetPrivateProfileStringA")]
        public static extern uint
            GetPrivateProfileStringByByteArray(string lpAppName,
                                               string lpKeyName, 
                                               string lpDefault,
                                               byte[] lpReturnedString, 
                                               uint nSize,
                                               string lpFileName);

        [DllImport("KERNEL32.DLL")]
        public static extern uint
            GetPrivateProfileInt( string lpAppName,
                                  string lpKeyName, 
                                  int nDefault, 
                                  string lpFileName );

        [DllImport("KERNEL32.DLL")]
        public static extern uint 
            WritePrivateProfileString( string lpAppName,
                                       string lpKeyName,
                                       string lpString,
                                       string lpFileName);

        static public string GetPrivateProfileString(string appName, 
                                                        string keyName, 
                                                        string defaultString,
                                                        string fileName)
        {
            StringBuilder sb = new StringBuilder(1024);
            GetPrivateProfileString(appName, keyName, defaultString, sb, (uint)sb.Capacity, fileName);
            return sb.ToString();
        }
    }

    class IniFile
    {
        private string filePath = "";

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        /// <param name="argFilePath">Ini�t�@�C��Path</param>
        public IniFile(string argFilePath)
        {
            filePath = argFilePath;
        }

        public string IniFilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// Ini�t�@�C�����當����Ǎ���
        /// </summary>
        /// <param name="section">�Z�N�V������</param>
        /// <param name="key">�L�[��</param>
        /// <param name="defaultStr">�f�t�H���g�l</param>
        /// <returns>�擾������</returns>
        public string ReadIniString(string section, string key, string defaultStr)
        {
            if ( !File.Exists(filePath) )
            {
                return defaultStr;
            }
            return IniFileHandler.GetPrivateProfileString( section , key, defaultStr, filePath );
        }
        public string ReadIniString(string section, string key)
        {
            return ReadIniString( section, key, "" );
        }

        public int WriteIniString(string section, string key, string writeStr)
        {
            return (int)IniFileHandler.WritePrivateProfileString( section, key, writeStr, filePath );
        }

        /// <summary>Ini�t�@�C�����琔�l�Ǎ���
        /// </summary>
        /// <param name="section">�Z�N�V������</param>
        /// <param name="key">�L�[��</param>
        /// <param name="defaultVal">�f�t�H���g�l</param>
        /// <returns>�擾�l</returns>
        public int ReadIniInt(string section, string key, int defaultVal)
        {
            if ( !File.Exists( filePath ) )
            {
                return defaultVal;
            }
            return (int)IniFileHandler.GetPrivateProfileInt( section, key, defaultVal, filePath );
        }
        public int ReadIniInt(string section, string key)
        {
            return ReadIniInt( section, key, 0 );
        }

        public string[] GetKeysInSection(string section, string defaultSection)
        {
            // �w��Z�N�V�����̃L�[�̈ꗗ�𓾂�
            byte [] retArray = new byte[1024];
            
            uint resultSize1 = IniFileHandler.GetPrivateProfileStringByByteArray(
                section , null, defaultSection, retArray, (uint)retArray.Length, filePath);

            string result1 = Encoding.Default.GetString( retArray, 0, (int)resultSize1-1);
            string [] keys = result1.Split('\0');
            return keys;
        }

        public string[] GetSectionsInInifile()
        {
            // �w��t�@�C���̃Z�N�V�����̈ꗗ�𓾂�
            byte[] retArray = new byte[1024];
            uint resultSize2 = IniFileHandler.GetPrivateProfileStringByByteArray(
                        null, null, "default", retArray, (uint)retArray.Length, filePath );

            string result2 = Encoding.Default.GetString( retArray, 0, (int)resultSize2 - 1 );
            string[] sections = result2.Split( '\0' );
            return sections;
        }

        public void DeleteKey(string section, string key)
        {
            // 1�̃L�[�ƒl�̃y�A���폜����
            IniFileHandler.WritePrivateProfileString( section, key, null, filePath );
        }

        public void DeleteSection(string section)
        {
            // �w��Z�N�V�������̑S�ẴL�[�ƒl�̃y�A���폜����
            IniFileHandler.WritePrivateProfileString( section, null, null, filePath );
        }
    }


}
