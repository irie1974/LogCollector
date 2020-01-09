using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Threading;
using System.Diagnostics;

namespace LogCollector
{
    [XmlRoot( "�ʐݒ�" )]
    public class LogSetting
    {
        public bool IsEnable = false;
        public string Path = "";
        public string FileName = "";
        public string exceptPattern = "";

        public LogSetting(bool argIsEnable, string argPath, string argFileName)
        {
            IsEnable = argIsEnable;
            Path = argPath;
            FileName = argFileName;
        }

        //xml�V���A��������ׂɂ́A�����̖����R���X�g���N�^���K�v�I
        public LogSetting()
        {
        }
    }

    [XmlRoot( "���O���W�ݒ�" )]
    public class LogCollector
    {
        private List<LogSetting> logSettings = new List<LogSetting>();
        private DateTime _logCollectFrom; //���O�擾�J�n����
        private DateTime _logCollectTo;
        private Thread procCollectLog;
        private bool isProcCollect = false; // ���W���t���O

        public LogCollector()
        {
            _logCollectFrom = DateTime.Now;
            _logCollectTo = DateTime.Now;
        }

        public DateTime LogCollectFrom
        {
            get { return _logCollectFrom; }
            set { _logCollectFrom = value; }
        }

        public DateTime LogCollectTo
        {
            get { return _logCollectTo; }
            set { _logCollectTo = value; }
        }

        public List<LogSetting> LogSettings
        {
            get { return logSettings; }
            set { logSettings = value;  }
        }

        [XmlIgnore()]
        public bool IsProcCollect
        {
            get { return isProcCollect; }
            set { isProcCollect = value; }
        }

        public void AddCollectSetting( bool isEnable, string path , string fileName  )
        {
            logSettings.Add( new LogSetting( isEnable, path, fileName ) );
        }

        public void SetCollectSetting(int index, bool isEnable, string path, string fileName)
        {
            if ( logSettings.Count <= index )
            {
                AddCollectSetting( isEnable, path, fileName );
                return;
            }
            logSettings[index].IsEnable = isEnable;
            logSettings[index].Path = path;
            logSettings[index].FileName = fileName;
        }

        public void SaveToInifile(string filePath)
        {
            IniFile iniFile = new IniFile( filePath );
            string section = "";
            string key = "";
            string writeStr = "";

            

            section = "Common";
            key = "CollectFromWhen";
            writeStr = Convert.ToString( this._logCollectFrom );
            iniFile.WriteIniString( section, key, writeStr );
            
            key = "CollectToWhen";
            writeStr = Convert.ToString( this._logCollectTo );
            iniFile.WriteIniString( section, key, writeStr );

            for ( int i = 0; i < this.logSettings.Count; i++ )
            {
                section = "Setting_" + i.ToString( "0#" );
                
                key = "FileName";
                writeStr = logSettings[i].FileName;
                iniFile.WriteIniString( section, key, writeStr );

                key = "Path    ";
                writeStr = logSettings[i].Path;
                iniFile.WriteIniString( section, key, writeStr );

                key = "Enable  ";
                writeStr = Convert.ToString( logSettings[i].IsEnable );
                iniFile.WriteIniString( section, key, writeStr );
            }
        }

        public void LoadFromInifile(string filePath)
        {
            IniFile iniFile = new IniFile( filePath );
            string cmnSection = "";
            string key = "";
            string retStr = "";
            DateTime dt = DateTime.Now;

            cmnSection = "Common";

            key = "CollectFromWhen";
            retStr = iniFile.ReadIniString( cmnSection, key, DateTime.Now.ToString() );
            if ( !DateTime.TryParse( retStr, out dt ) )
            {
                dt = DateTime.Now;
            }
            this._logCollectFrom = dt;

            key = "CollectFromTo";
            retStr = iniFile.ReadIniString( cmnSection, key, DateTime.Now.ToString() );
            if ( !DateTime.TryParse( retStr, out dt ) )
            {
                dt = DateTime.Now;
            }
            this._logCollectTo = dt;

            //Setting�Z�N�V������ �擾
            string[] sections = iniFile.GetSectionsInInifile();
            
            foreach ( string section in sections )
            {
                if ( section == "Common" ) 
                    continue;

                string path = iniFile.ReadIniString( section, "Path" );
                string fileName = iniFile.ReadIniString( section, "FileName" );
                bool isEnable = Convert.ToBoolean( iniFile.ReadIniString( section, "Enable") ) ;

                LogSetting ls = new LogSetting( isEnable, path, fileName );
                logSettings.Add( ls );
            }
        }

        ///<summary>�p�����[�^���t�@�C���ɃZ�[�u</summary>
        public void Save(string filePath)
        {
            FileStream fs = new FileStream( filePath, FileMode.Create );

            // Construct a BinaryFormatter and use it to serialize the data to the stream.
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize( fs, this );
            }
            catch ( SerializationException e )
            {
                Console.WriteLine( "Failed to serialize. Reason: " + e.Message );
                throw;
            }
            finally
            {
                fs.Close();
            }

        }

        ///<summary>�t�@�C������ǎ��</summary>
        static public LogCollector Load(string filePath)
        {
            LogCollector lc;

            if ( !File.Exists( filePath ) )
            {
                return new LogCollector();
            }

            FileStream fs = new FileStream( filePath, FileMode.Open );
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                lc = (LogCollector)formatter.Deserialize( fs );
            }
            catch ( SerializationException e )
            {
                lc = new LogCollector();
                Console.WriteLine( "Failed to deserialize. Reason: " + e.Message );
            }
            finally
            {
                fs.Close();
            }
            return lc;
        }


        public void SaveToXml(string filePath)
        {
            try
            {
                XmlSerializer serializer1 = new XmlSerializer( typeof( LogCollector ) );

                using ( FileStream fs = new FileStream( filePath, FileMode.Create ) )
                {
                    serializer1.Serialize( fs, this );
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( e.Message );
                throw;
            }
        }
        ///<summary>�t�@�C������ǎ��</summary>
        static public LogCollector LoadFromXml(string fileName)
        {
            LogCollector setting;
            try
            {
                XmlSerializer serializer = new XmlSerializer( typeof( LogCollector ) );

                using ( FileStream fs = new FileStream( fileName, System.IO.FileMode.Open ) )
                {
                    setting = (LogCollector)serializer.Deserialize( fs );
                }
            }
            catch ( Exception e )
            {
                setting = new LogCollector();
            }
            return setting;
        }

        public bool CollectLogStart()
        {
            if ( procCollectLog != null )
            {
                Console.WriteLine( "���łɎ��s���v���Z�X������܂�" );
                return false;
            }
            procCollectLog = new Thread( CollectLogFiles );
            procCollectLog.Start();

            return true;
        }

        private void CollectLogFiles()
        {
            isProcCollect = true;

            //���W��t�H���_��
            string saveFolderName = "Log_" + _logCollectFrom.ToString("yyyy-MM-dd_HHmmss" )
                                  + "_" + _logCollectTo.ToString( "yyyy-MM-dd_HHmmss" );

            // �ꎟ�ۑ��t�H���_Path
            string tempPath = Path.GetTempPath();

            //���W��x�[�X�t�H���_Path
            string saveBasePath = Path.Combine( tempPath, saveFolderName );
            // CurrentFolder.Get()
            // System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //���W��x�[�X�t�H���_�������ɗL��ꍇ�͂��폜
            if ( Directory.Exists( saveBasePath ) )
            {
                // �ǎ���p�t���O���������č폜
                DirectoryInfo di = new DirectoryInfo( saveBasePath );
                RemoveReadonlyAttribute(di);
                Directory.Delete( saveBasePath, true );
            }

            //�ݒ蕪���[�v
            foreach ( LogSetting logSet in logSettings )
            {
                //�L���ݒ肳��Ă��Ȃ����̂͏������Ȃ��B
                if ( !logSet.IsEnable )
                {
                    continue;
                }

                //�t�@�C�����w�薳�������ׂ�
                if ( logSet.FileName.Length == 0 )
                {
                    logSet.FileName = "*";
                }

                //���W�ݒ�Path���̕ۑ��Y���t�@�C���ꗗ�擾
                string[] files = Directory.GetFiles( logSet.Path, logSet.FileName );

                //�t�@�C���ꗗ����t�@�C����������
                foreach ( string file in files )
                {
                    string filepath = Path.Combine( logSet.Path, file );
                    DateTime lastWrite = File.GetLastWriteTime( filepath );

                    // ���W�����i�����j�`�F�b�N
                    if ( lastWrite < _logCollectFrom || _logCollectTo < lastWrite )
                    {
                        continue;
                    }

                    try
                    {
                        // �ꎞ�ۑ��t�H���_�쐬
                        string savePath = Path.Combine( saveBasePath, filepath.Replace( ":", "" ) );
                        string parentPath = Directory.GetParent( savePath ).FullName;
                        Directory.CreateDirectory( parentPath );

                        // �t�@�C���R�s�[
                        File.Copy( filepath, savePath, true );
                    }
                    catch ( Exception e )
                    {
                        Console.WriteLine( e.Message );
                    }

                }
            }

            try
            {
                // Zip�A�[�J�C�u�쐬
                ZipHelper zip = new ZipHelper();
                zip.CreateZipArchive( saveFolderName + ".zip", saveBasePath, "", "" );

                // �ꎞ�ۑ��t�H���_�폜
                Directory.Delete( saveBasePath, true );
            }
            catch
            {
            }

            isProcCollect = false;
            procCollectLog = null;
        }

        public static void RemoveReadonlyAttribute(DirectoryInfo dirInfo)
        {
            //��̃t�H���_�̑�����ύX
            if ( (dirInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly )
                dirInfo.Attributes = FileAttributes.Normal;
            //�t�H���_���̂��ׂẴt�@�C���̑�����ύX
            foreach ( FileInfo fi in dirInfo.GetFiles() )
                if ( (fi.Attributes & FileAttributes.ReadOnly) ==
                    FileAttributes.ReadOnly )
                    fi.Attributes = FileAttributes.Normal;
            //�T�u�t�H���_�̑�������A�I�ɕύX
            foreach ( DirectoryInfo di in dirInfo.GetDirectories() )
                RemoveReadonlyAttribute( di );
        }
        
        public MyEnumerator GetEnumerator() 
        {
            return new MyEnumerator(this);
        }

        // Declare the enumerator class:
        public class MyEnumerator
        {
            int nIndex;
            LogCollector collection;
            public MyEnumerator(LogCollector coll)
            {
                collection = coll;
                nIndex = -1;
            }

            public bool MoveNext()
            {
                nIndex++;
                return (nIndex < collection.logSettings.Count);
            }

            public LogSetting Current
            {
                get
                {
                    return (collection.logSettings[nIndex]);
                }
            }
        }

    }
}
