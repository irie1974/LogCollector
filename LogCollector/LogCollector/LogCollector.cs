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
    [XmlRoot( "個別設定" )]
    public class LogSetting
    {
        public bool IsEnable = false;
        public string Path = "";
        public string SearchPattern = "";
        public string exceptPattern = "";

        public LogSetting(bool argIsEnable, string argPath, string argSearchPattern)
        {
            IsEnable = argIsEnable;
            Path = argPath;
            SearchPattern = argSearchPattern;
        }

        //xmlシリアル化する為には、引数の無いコンストラクタが必要！
        public LogSetting()
        {
        }
    }

    [XmlRoot( "ファイル収集設定" )]
    public class LogCollector
    {
        private List<LogSetting> logSettings = new List<LogSetting>();
        private DateTime _logCollectFrom; //ログ取得開始日時
        private DateTime _logCollectTo;
        private Thread procCollectLog;
        private bool isProcCollect = false; // 収集中フラグ

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

        public void SetCollectSetting(int index, bool isEnable, string path, string searchPattern)
        {
            if ( logSettings.Count <= index )
            {
                AddCollectSetting( isEnable, path, searchPattern );
                return;
            }
            logSettings[index].IsEnable = isEnable;
            logSettings[index].Path = path;
            logSettings[index].SearchPattern = searchPattern;
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
                
                key = "SearchPattern";
                writeStr = logSettings[i].SearchPattern;
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

            //Settingセクション数 取得
            string[] sections = iniFile.GetSectionsInInifile();
            
            foreach ( string section in sections )
            {
                if ( section == "Common" ) 
                    continue;

                string path = iniFile.ReadIniString( section, "Path" );
                string fileName = iniFile.ReadIniString( section, "SearchPattern" );
                bool isEnable = Convert.ToBoolean( iniFile.ReadIniString( section, "Enable") ) ;

                LogSetting ls = new LogSetting( isEnable, path, fileName );
                logSettings.Add( ls );
            }
        }

        ///<summary>パラメータをファイルにセーブ</summary>
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

        ///<summary>ファイルから読取り</summary>
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
        ///<summary>ファイルから読取り</summary>
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
                Console.WriteLine( "すでに実行中プロセスがあります" );
                return false;
            }
            procCollectLog = new Thread( CollectLogFiles );
            procCollectLog.Start();

            return true;
        }

        private void CollectLogFiles()
        {
            isProcCollect = true;

            //収集先フォルダ名
            string saveFolderName = "File_" + _logCollectFrom.ToString("yyyy-MM-dd_HHmmss" )
                                  + "_" + _logCollectTo.ToString( "yyyy-MM-dd_HHmmss" );

            // 一次保存フォルダPath
            string tempPath = Path.GetTempPath();

            //収集先ベースフォルダPath
            string saveBasePath = Path.Combine( tempPath, saveFolderName );
            // CurrentFolder.Get()
            // System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            //収集先ベースフォルダ内が既に有る場合はを削除
            if ( Directory.Exists( saveBasePath ) )
            {
                // 読取り専用フラグを除去して削除
                DirectoryInfo di = new DirectoryInfo( saveBasePath );
                RemoveReadonlyAttribute(di);
                Directory.Delete( saveBasePath, true );
            }

            //設定分ループ
            foreach ( LogSetting logSet in logSettings )
            {
                //有効設定されていないものは処理しない。
                if ( !logSet.IsEnable )
                {
                    continue;
                }

                //ファイル名指定無し→すべて
                if ( logSet.SearchPattern.Length == 0 )
                {
                    logSet.SearchPattern = "*";
                }

                //収集設定Path内の保存該当ファイル一覧取得
                string[] files = Directory.GetFiles( logSet.Path, logSet.SearchPattern );

                //ファイル一覧からファイルを一つ一つ処理
                foreach ( string file in files )
                {
                    string filepath = Path.Combine( logSet.Path, file );
                    DateTime lastWrite = File.GetLastWriteTime( filepath );

                    // 収集条件（日時）チェック
                    if ( lastWrite < _logCollectFrom || _logCollectTo < lastWrite )
                    {
                        continue;
                    }

                    try
                    {
                        // 一時保存フォルダ作成
                        string savePath = Path.Combine( saveBasePath, filepath.Replace( ":", "" ) );
                        string parentPath = Directory.GetParent( savePath ).FullName;
                        Directory.CreateDirectory( parentPath );

                        // ファイルコピー
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
                // Zipアーカイブ作成
                ZipHelper zip = new ZipHelper();
                zip.CreateZipArchive( saveFolderName + ".zip", saveBasePath, "", "" );

                // 一時保存フォルダ削除
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
            //基のフォルダの属性を変更
            if ( (dirInfo.Attributes & FileAttributes.ReadOnly) ==
                FileAttributes.ReadOnly )
                dirInfo.Attributes = FileAttributes.Normal;
            //フォルダ内のすべてのファイルの属性を変更
            foreach ( FileInfo fi in dirInfo.GetFiles() )
                if ( (fi.Attributes & FileAttributes.ReadOnly) ==
                    FileAttributes.ReadOnly )
                    fi.Attributes = FileAttributes.Normal;
            //サブフォルダの属性を回帰的に変更
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
