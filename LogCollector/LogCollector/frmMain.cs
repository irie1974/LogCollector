using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ERDHelper;

namespace LogCollector
{
    public partial class frmMain : Form
    {
        private BindingSource bindingSrc = new BindingSource();
        private DataTable dataTbl = new DataTable();
        private LogCollector lc = new LogCollector();
        private const string PATH_SETTING = ".\\LogCollectSetting.ini";
        private const string PATH_SETTING_XML = ".\\LogCollectSetting.xml";
        private Int32 _procCnt = 0;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Shown += new EventHandler(frmMain_Shown);
            this.FormClosing += new FormClosingEventHandler( frmMain_FormClosing );

            dgvCollectData.CellEndEdit += new DataGridViewCellEventHandler( dgvCollectData_CellEndEdit );
            dgvCollectData.CellBeginEdit += new DataGridViewCellCancelEventHandler( dgvCollectData_CellBeginEdit );
            dgvCollectData.CellMouseUp += new DataGridViewCellMouseEventHandler( dgvCollectData_CellMouseUp );
            dgvCollectData.MouseUp += new MouseEventHandler( dgvCollectData_MouseUp );
            dgvCollectData.SizeChanged += new EventHandler( dgvCollectData_SizeChanged );

            dtpCollectFrom.Format = DateTimePickerFormat.Custom;
            dtpCollectFrom.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            dtpCollectTo.Format = DateTimePickerFormat.Custom;
            dtpCollectTo.CustomFormat = "yyyy-MM-dd HH:mm:ss";

            //bindingSrc.DataSource = dataTbl;
            //dgvCollectData.DataSource = bindingSrc;

            lblMsg.Text = "";

            this.MinimumSize = this.Size;
            //lc = LogCollector.Load( PATH_SETTING );
            //lc.LoadFromInifile( PATH_SETTING );
            lc = LogCollector.LoadFromXml( PATH_SETTING_XML );

            timer.Enabled = true;
            timer.Interval = 1000;
        }

        void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            SetCollectSettingFromGrid();
            //設定を保存
            //lc.SaveToInifile( PATH_SETTING );
            lc.SaveToXml( PATH_SETTING_XML );
        }

        void dgvCollectData_SizeChanged(object sender, EventArgs e)
        {
            dgvCollectData.Columns["Main"].Width = dgvCollectData.Width
                                        - dgvCollectData.Margin.Left
                                        - dgvCollectData.Columns["IsEnable"].Width
                                        - dgvCollectData.Columns["FileName"].Width
                                        - dgvCollectData.Columns["Sub"].Width;
        }

        /// <summary>グリッドMouseUpイベント</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgvCollectData_MouseUp(object sender, MouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Right && dgvCollectData.CurrentRow != null)
            {
                //新規追加行では処理しない。
                if ( dgvCollectData.CurrentRow.IsNewRow )
                {
                    return;
                }

                //コンテキストメニュー表示
                dgvCollectData.ContextMenu.Show( dgvCollectData, new Point( e.X, e.Y ) );
            }
        }

        /// <summary>グリッドセルMouseUpイベント</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgvCollectData_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if ( e.Button == MouseButtons.Right )
            {
                //クリックしたセルのIndexがグリッド範囲内か
                if ( 0 <= e.ColumnIndex && e.ColumnIndex < dgvCollectData.Columns.Count
                    && 0 <= e.RowIndex && e.RowIndex < dgvCollectData.Rows.Count )
                {
                    //カレントセルをクリックしたセルへ移動
                    dgvCollectData.CurrentCell = dgvCollectData[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        /// <summary>コンテキストメニュー(行削除）クリック</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuDelRow_Click(object sender, EventArgs e)
        {
            
            // 行未選択 or 新規追加行 なら処理しない。
            if ( dgvCollectData.CurrentRow == null || dgvCollectData.CurrentRow.IsNewRow )
            {
                return;
            }
            // カレント行を削除
            dgvCollectData.Rows.RemoveAt( dgvCollectData.CurrentRow.Index );
        }

        /// <summary>コンテキストメニュー（行追加）クリック</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuAddRow_Click(object sender, EventArgs e)
        {
            // カレント行に一行追加
            dgvCollectData.Rows.Insert( dgvCollectData.CurrentRow.Index, 1 );
        }

        /// <summary>セル編集開始</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgvCollectData_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //
            // 指定Pathが無い場合は「有効」設定できないようにする。
            //

            DataGridView dgv = (DataGridView)sender;
            DataGridViewCell cell = dgv.CurrentCell;

            if ( cell.OwningColumn.Name == "IsEnable" )
            {
                //メインパスの取得
                string path = "";
                for (int i = 0; i <= cell.RowIndex; i++)
                {
                    if(Convert.ToString(dgvCollectData["Main", i].Value) != "")
                        path = Convert.ToString(dgvCollectData["Main", i].Value);
                }

                path = System.IO.Path.Combine(path, Convert.ToString( dgvCollectData["Sub", cell.RowIndex].Value ));
                if ( !Directory.Exists( path ) )
                {
                    cell.Value = false;
                    e.Cancel = true;
                    return;
                }   
            }
        }

        /// <summary>セル編集終了</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgvCollectData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //
            // パスが無い場合は有効フラグをOff
            //
            DataGridView dgv = (DataGridView)sender;
            DataGridViewCell cell = dgv.CurrentCell;

            if ( cell.OwningColumn.Name != "Path" )
            {
                return;
            }
            
            string path = Convert.ToString( cell.Value );

            if ( Directory.Exists( path ) )
            {
                cell.Style.ForeColor = Color.Black;
            }
            else {
                cell.Style.ForeColor = Color.Crimson;
                dgvCollectData["IsEnable", cell.RowIndex].Value = false;
            }


        }


        private void frmMain_Shown(object sender, EventArgs e)
        {
            SetupGrid();
        }

        private void SetupGrid()
        {
            // カラム設定
            GridHelper.AddCol_TextBox(dgvCollectData, "メインフォルダ", "Main", "Main", false);
            GridHelper.AddCol_TextBox(dgvCollectData, "サブフォルダ", "Sub", "Sub", false);
            GridHelper.AddCol_TextBox(dgvCollectData, "ファイル名 または検索パターン", "FileName", "FileName", false);
            GridHelper.AddCol_ChkBox( dgvCollectData, "有効", "IsEnable", "IsEnable", true, false, false, false );

            // カラムサイズ設定
            //dgvCollectData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCollectData.Columns["IsEnable"].Width = (int)(dgvCollectData.Width * 0.1);
            dgvCollectData.Columns["FileName"].Width = (int)(dgvCollectData.Width * 0.2);
            dgvCollectData.Columns["Sub"].Width = (int)(dgvCollectData.Width * 0.2);
            dgvCollectData.Columns["Main"].Width = dgvCollectData.Width 
                                                    - dgvCollectData.Margin.Left 
                                                    - dgvCollectData.Columns["IsEnable"].Width 
                                                    - dgvCollectData.Columns["FileName"].Width
                                                    - dgvCollectData.Columns["Sub"].Width;
            
            // ユーザーリサイズ可・不可
            dgvCollectData.AllowUserToResizeColumns = true;
            dgvCollectData.AllowUserToResizeRows = false;
            
            // その他設定
            dgvCollectData.AlternatingRowsDefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvCollectData.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            dgvCollectData.RowHeadersVisible = false;
            dgvCollectData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            Font f = new System.Drawing.Font("Meiryo UI", 9);
            dgvCollectData.Font = f;

            // コンテキストメニュー設定
            MenuItem menuDelRow = new MenuItem( "この行を削除", new EventHandler( menuDelRow_Click ) );
            MenuItem menuAddRow = new MenuItem( "ここに行を追加", new EventHandler( menuAddRow_Click ) );
            MenuItem[] menuItems = new MenuItem[] { menuDelRow, menuAddRow };
            dgvCollectData.ContextMenu = new ContextMenu( menuItems );
            dgvCollectData.ContextMenu.Name = "dgvMenu";
            
            // 設定をグリッドに反映
            SetLogCollectSetting();
        }

        private void SetLogCollectSetting()
        {
            int i = 0;
            foreach ( LogSetting ls in lc )
            {
                dgvCollectData.Rows.Add();
                dgvCollectData.Rows[i].Cells["IsEnable"].Value = ls.IsEnable;
                dgvCollectData.Rows[i].Cells["FileName"].Value = ls.SearchPattern;
                dgvCollectData.Rows[i].Cells["Sub"].Value = ls.SubPath;
                dgvCollectData.Rows[i].Cells["Main"].Value = ls.MainPath;
                i++;
            }

            dtpCollectFrom.Value = lc.LogCollectFrom;
            dtpCollectTo.Value = lc.LogCollectTo;
        }

        private void btnCollect_Click(object sender, EventArgs e)
        {
            CollectStart();
        }

        private void SetCollectSettingFromGrid()
        {
            lc.LogCollectFrom = dtpCollectFrom.Value;
            lc.LogCollectTo = dtpCollectTo.Value;

            int settingIdx = 0;
            for ( int rowIdx = 0; rowIdx < dgvCollectData.Rows.Count; rowIdx++ )
            {
                if (( dgvCollectData["Main", rowIdx].Value == null ) &&
                    (dgvCollectData["Sub", rowIdx].Value == null))
                {
                    continue;
                }

                if ( dgvCollectData["IsEnable", rowIdx].Value == null )
                {
                    dgvCollectData["IsEnable", rowIdx].Value = false;
                }

                lc.SetCollectSetting( settingIdx,
                    (bool)dgvCollectData["IsEnable", rowIdx].Value,
                    Convert.ToString( dgvCollectData["Main", rowIdx].Value ),
                    Convert.ToString(dgvCollectData["Sub", rowIdx].Value),
                    Convert.ToString(dgvCollectData["FileName", rowIdx].Value));
                
                settingIdx++;
            }
        }

        public void CollectStart()
        {
            _procCnt = 1;
            
            SetCollectSettingFromGrid();
            if ( !lc.CollectLogStart() )
            {
                MessageBox.Show( "現在処理中の為、開始できません。", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ( lc.IsProcCollect )
            {
                //収集中
                btnCollect.Enabled = false;
                Int32 chrCnt = _procCnt % 50;
                lblMsg.Text = "収集中" + new string( '.', chrCnt );
                _procCnt++;
            }
            else if ( 0 < _procCnt )
            {
                //収集終了
                _procCnt = 0;
                btnCollect.Enabled = true;
                lblMsg.Text = "";
                MessageBox.Show( "収集完了しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information );
                this.Close();
            }
        }
    }
}