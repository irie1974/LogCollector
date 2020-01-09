using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace ERDHelper
{

    class GridHelper
    {
        /// <summary>
        /// シンプルなグリッドに設定
        /// </summary>
        /// <param name="dgv"></param>
        static public void SetSimple(DataGridView dgv)
        {
            if(dgv == null)
                return;
            dgv.AutoGenerateColumns = false;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeColumns = true; //桁の幅は変更可
            dgv.AllowUserToResizeRows = false; //行の高さは変更不可
            dgv.AllowUserToAddRows = false;
            dgv.ShowCellToolTips = false; //ツールチップ表示しない
            NoSort(dgv);
        }

        static public void NoSort(DataGridView dgv)
        {
            for(int c = 0;c < dgv.ColumnCount;c++) {
                dgv.Columns[c].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        static public void SetSortMode(DataGridView dgv, DataGridViewColumnSortMode sortMode)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
                col.SortMode = sortMode;
        }
        /// <summary>
        /// 列タイトル設定.
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="titles"></param>
        static public void SetColTitle(DataGridView dgv, string[] titles)
        {
            dgv.ColumnCount = titles.Length;
            for(int i = 0;i < titles.Length;i++) {
                dgv.Columns[i].Name = titles[i];
            }
        }

                /// <summary>
        /// グリッドに１行データを設定
        /// </summary>
        /// <param name="dgv">グリッド</param>
        /// <param name="row">行</param>
        /// <param name="valCompo">,で区切られた文字列</param>
        static public void SetLineToGrid(DataGridView dgv, int row, string valCompo)
        {
            if(string.IsNullOrEmpty(valCompo))
                return;
            string[] parts = valCompo.Split(new char[] { ',' });
            for(int i = 0;i < parts.Length;i++) {
                if(parts[i].Length < 1)
                    continue;
                if(dgv.ColumnCount <= i)
                    break;
                string top = parts[i].Substring(0, 1); //最初の文字
                string s = parts[i];
                switch(top) {
                case "!":
                    dgv.Rows[row].Cells[i].Style.BackColor = Color.Pink;
                    s = s.Substring(1);
                    break;
                case "@":
                    dgv.Rows[row].Cells[i].Style.BackColor = Color.SkyBlue;
                    s = s.Substring(1);
                    break;
                }
                dgv.Rows[row].Cells[i].Value = s;
            }
        }

        static public void SetColHeaderText(DataGridView dgv, string[] HeaderTexts)
        {
            for (int i = 0; i < HeaderTexts.Length; i++)
            {
                dgv.Columns[i].HeaderText = HeaderTexts[i];
            }
        }

        static public void SetColHeaderText(DataGridView dgv, string HeaderText)
        {
            if (string.IsNullOrEmpty(HeaderText))
                return;
            string[] parts = HeaderText.Split(new char[] { ',' });
            SetColHeaderText(dgv, parts);
        }

        /// <summary>
        /// 列追加（テキストボックス）
        /// </summary>
        /// <param name="dgv">DataGridViewオブジェクト</param>
        /// <param name="HeaderText">表示タイトル</param>
        /// <param name="ColName" >カラム名称</param>
        /// <param name="DataPropertyName">元データ名</param>
        static public void AddCol_TextBox(DataGridView dgv, string HeaderText, string ColName, string DataPropertyName,
            bool readOnly)
        {

            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.DataPropertyName = DataPropertyName;
            col.HeaderText = HeaderText;
            col.Name = ColName;
            col.ReadOnly = readOnly;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgv.Columns.Add(col);
            col.Dispose();
        }
        /// <summary>
        /// 列追加（チェックボックス）
        /// </summary>
        /// <param name="dgv">DataGridViewオブジェクト</param>
        /// <param name="HeaderText">表示タイトル</param>
        /// <param name="DataPropertyName">元データタイトル</param>
        /// <param name="argTrueValue">Trueとする値</param>
        /// <param name="argThreeState"></param>
        static public void AddCol_ChkBox(DataGridView dgv, string HeaderText, string ColName, string DataPropertyName,
            object argTrueValue, object argFalseValue, bool argThreeState, bool readOnly)
        {
            DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
            col.DataPropertyName = DataPropertyName;
            col.HeaderText = HeaderText;
            col.Name = ColName;
            col.TrueValue = argTrueValue;
            col.FalseValue = argFalseValue;
            col.ThreeState = argThreeState;
            col.ReadOnly = readOnly;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgv.Columns.Add(col);
            col.Dispose();
        }
        /// <summary>
        /// 列追加（コンボボックス）
        /// </summary>
        /// <param name="dgv">DataGridViewオブジェクト</param>
        /// <param name="HeaderText"></param>
        /// <param name="DataPropertyName"></param>
        /// <param name="DataSrcTbl">コンボボックス用テーブル(表示タイトルと元データタイトルで構成）</param>
        /// <param name="ValueMember">元データタイトル</param>
        /// <param name="DisplayMember">表示タイトル</param>
        static public void AddCol_ComboBox(DataGridView dgv, string HeaderText, string ColName, string DataPropertyName,
            object DataSrcTbl, string ValueMember, string DisplayMember)
        {
            DataGridViewComboBoxColumn col = new DataGridViewComboBoxColumn();
            col.DataPropertyName = DataPropertyName;
            col.HeaderText = HeaderText;
            col.Name = ColName;
            col.DataSource = DataSrcTbl;
            col.ValueMember = ValueMember;
            col.DisplayMember = DisplayMember;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgv.Columns.Add(col);
            col.Dispose();
        }

        static public void AddRowTitle(DataGridView dgv, object HeaderVal)
        {
            DataGridViewRowHeaderCell row = new DataGridViewRowHeaderCell();
            row.Value = HeaderVal;
            //dgv.RowHeadersWidth = 15;
            dgv.Rows.Add(row);
        }


    }


    class GridPrinter
    {
        Graphics graph;
        DataGridView dgv;

        //コンストラクタ
        public GridPrinter(Graphics grp, DataGridView grd)
        {
            graph = grp;
            dgv = grd;
        }

        //枠付きで文字列を印字
        void prnStrWithFrame(Font fnt, string str, RectangleF rect)
        {
            graph.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
            graph.DrawString(str, fnt, Brushes.Black, rect);
        }

        void prnStrFit(PointF pt, Font fnt, string str)
        {
            SizeF sf = graph.MeasureString(str, fnt);
            RectangleF rf = new RectangleF(pt.X, pt.Y, sf.Width, sf.Height);
            prnStrWithFrame(fnt, str, rf);
        }

        //1行印刷
        void prnStrGridRow(Font fnt, RectangleF rect, float[] colWidths, int row)
        {
            RectangleF rectf = rect;
            for(int c = 0;c < dgv.ColumnCount;c++) {
                rectf.Width = colWidths[c];
                prnStrWithFrame(fnt, (string)dgv.Rows[row].Cells[c].Value.ToString(), rectf);
                rectf.X += rectf.Width;
            }
        }

        public void printGrid(PointF pt, Font fnt)
        {
            //BaseになるHeight
            float bH = 0;
            //桁の幅
            float[] colWs = new float[dgv.ColumnCount];
            //桁の幅を調べる
            SizeF sf;
            for(int c = 0;c < dgv.ColumnCount;c++) {
                sf = graph.MeasureString(dgv.Columns[c].HeaderText, fnt);
                bH = sf.Height;
                colWs[c] = sf.Width;
            }
            RectangleF rectf = new RectangleF();
            rectf.Height = bH;
            rectf.X = 0;
            rectf.Y = 0;
            //ヘッダを印刷
            for(int c = 0;c < dgv.ColumnCount;c++) {
                rectf.Width = colWs[c];
                prnStrWithFrame(fnt, dgv.Columns[c].HeaderText, rectf);
                rectf.X += rectf.Width;
            }

            rectf.X = 0;
            rectf.Y += rectf.Height;
            //本体を印刷
            for(int r = 0;r < dgv.RowCount;r++) {
                //１行印刷
                prnStrGridRow(fnt, rectf, colWs, r);
                rectf.Y += rectf.Height;
            }
        }

    }

    class GridPrinter2
    {
        #region [定数・変数]
        private DataGridView dgv;
        private int crntRowIdx = 0;
        private int page = 1;
        private PrintDocument printDoc = new PrintDocument();
        private StringFormat[] strFmt;
        private Font customFont = null;
        private Font font = new Font("ＭＳ ゴシック", 9);
        private string title = "";
        #endregion [変数・定数]

        public Font CustomFont
        {
            set { customFont = (Font)value.Clone(); }
        }

        //コンストラクタ
        public GridPrinter2(DataGridView argDgv,PrintDocument argPrnDoc,string argTitle)
        {
            title = argTitle;
            dgv = argDgv;
            printDoc = argPrnDoc;
            strFmt = new StringFormat[argDgv.Columns.Count];
            SetAlignmentFromGrid(strFmt, dgv);
        }

        private void SetAlignmentFromGrid(StringFormat[] sf, DataGridView dgv)
        {
            for (int colIdx = 0; colIdx < dgv.Columns.Count; colIdx++)
            {
                sf[colIdx] = new StringFormat();
                //Alignment設定
                switch (dgv.Columns[colIdx].DefaultCellStyle.Alignment)
                {
                    //left

                    case DataGridViewContentAlignment.BottomLeft:
                    case DataGridViewContentAlignment.MiddleLeft:
                    case DataGridViewContentAlignment.TopLeft:
                        sf[colIdx].Alignment = StringAlignment.Near;
                        break;

                    //center

                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.BottomCenter:
                    case DataGridViewContentAlignment.MiddleCenter:
                        sf[colIdx].Alignment = StringAlignment.Center;
                        break;

                    //right

                    case DataGridViewContentAlignment.BottomRight:
                    case DataGridViewContentAlignment.TopRight:
                    case DataGridViewContentAlignment.MiddleRight:
                        sf[colIdx].Alignment = StringAlignment.Far;
                        break;
                    default:
                        sf[colIdx].Alignment = StringAlignment.Center;
                        break;
                }
                sf[colIdx].LineAlignment = StringAlignment.Center;
            }
        }

        public void PrintDataGrid(PrintPageEventArgs e)
        {
            Single CurrentX = 0;
            Single CurrentY = 0;
            Single zoom = 1;
            Font fnt = new Font("ＭＳ ゴシック", 6);
            int maxRow = 0;                             //１ページの行数
            string PageTitle = "";

            //グリッドの幅が用紙に入らない場合は縮小
            Single totalDgvWidth = 0;
            for (int c = 0; c < dgv.Columns.Count; c++)
            {
                if (dgv.Columns[c].Visible)
                    totalDgvWidth += dgv.Columns[c].Width;
            }
            Single printWidth = printDoc.DefaultPageSettings.Bounds.Width
                                - printDoc.DefaultPageSettings.Margins.Left
                                - printDoc.DefaultPageSettings.Margins.Right;
            if (printWidth < totalDgvWidth)
            {
                zoom = printWidth / totalDgvWidth;
                customFont = new Font("MS UI Gothic", 5);
            }

            if (dgv.Rows.Count == 0)
                return;

            PageTitle = "Page : " + page;
            SizeF szfTitle;
            szfTitle = e.Graphics.MeasureString(PageTitle, fnt);

            maxRow = Convert.ToInt32((printDoc.DefaultPageSettings.Bounds.Height
                                      - szfTitle.Height 
                                      - dgv.ColumnHeadersHeight 
                                      - printDoc.DefaultPageSettings.Margins.Top
                                      - printDoc.DefaultPageSettings.Margins.Bottom
                                      ) 
                                      / dgv.Rows[0].Height);


            for (int rowIdx = crntRowIdx; rowIdx < dgv.Rows.Count; rowIdx++)
            {
                if (dgv.Rows[rowIdx].Visible == false)
                    continue;

                //ヘッダー描画
                if (rowIdx % maxRow == 0)
                {
                    CurrentX = printDoc.DefaultPageSettings.Margins.Left;
                    CurrentY = printDoc.DefaultPageSettings.Margins.Top;

                    //タイトル
                    e.Graphics.DrawString(title, fnt, Brushes.Black, new PointF(CurrentX, CurrentY));
                    //ページ数
                    CurrentX = printDoc.DefaultPageSettings.Bounds.Width 
                               - printDoc.DefaultPageSettings.Margins.Right  
                               - szfTitle.Width;
                    e.Graphics.DrawString(PageTitle, fnt, Brushes.Black, new PointF(CurrentX, CurrentY));

                    //次描画位置
                    CurrentX = printDoc.DefaultPageSettings.Margins.Left;
                    CurrentY += szfTitle.Height;

                    //列ヘッダー描画
                    for (int j = 0; j < dgv.Columns.Count; j++)
                    {
                        if (dgv.Columns[j].Visible == false)
                            continue;

                        //罫線描画
                        e.Graphics.DrawRectangle(Pens.Black, CurrentX, CurrentY,
                             dgv.Columns[j].Width * zoom, dgv.ColumnHeadersHeight);

                        //文字描画
                        StringFormat sf = new StringFormat();
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;

                        if (customFont == null)
                            fnt = (Font)dgv.ColumnHeadersDefaultCellStyle.Font.Clone();
                        else
                            fnt = (Font)customFont.Clone();

                        e.Graphics.DrawString(dgv.Columns[j].HeaderText.ToString(), fnt,
                             Brushes.Black, new RectangleF(CurrentX, CurrentY,
                             dgv.Columns[j].Width * zoom, dgv.ColumnHeadersHeight), sf);

                        //次の列へ
                        CurrentX += dgv.Columns[j].Width * zoom;
                    }

                    //次描画位置へ
                    CurrentX = printDoc.DefaultPageSettings.Margins.Left;
                    CurrentY = CurrentY + dgv.ColumnHeadersHeight;
                }

                //データ本体描画
                for (int colIdx = 0; colIdx < dgv.Columns.Count; colIdx++)
                {
                    if (dgv.Columns[colIdx].Visible == false)
                        continue;

                    //セル罫線描画
                    e.Graphics.DrawRectangle(Pens.Black, CurrentX, CurrentY,
                         dgv.Columns[colIdx].Width * zoom, dgv.Rows[rowIdx].Height);

                    //セル文字描画
                    if (customFont == null)
                        fnt = (Font)dgv.DefaultCellStyle.Font.Clone();
                    else
                        fnt = (Font)customFont.Clone();

                    e.Graphics.DrawString(Convert.ToString(dgv[colIdx, rowIdx].Value), fnt,
                         Brushes.Black, new RectangleF(CurrentX, CurrentY,
                         dgv.Columns[colIdx].Width * zoom, dgv.Rows[rowIdx].Height), strFmt[colIdx]);

                    //次のセルの描画開始位置へ
                    CurrentX += dgv[colIdx, rowIdx].Size.Width * zoom;
                }

                //次の行の描画開始位置へ
                CurrentX = printDoc.DefaultPageSettings.Margins.Left;
                CurrentY = CurrentY + dgv.Rows[rowIdx].Height;
                
                //次の行へ
                crntRowIdx++;

                //改ページ処理
                if (crntRowIdx % maxRow == 0)
                {
                    e.HasMorePages = true;
                    page++;
                    break;
                }
            }
        }
    }

    class FormHelper
    {
        /// <summary>
        /// フォームを画面の一番下中央に移動させる
        /// </summary>
        static public void MoveBottom(Form frm)
        {
            int screenW = Screen.PrimaryScreen.WorkingArea.Width;
            int screenH = Screen.PrimaryScreen.WorkingArea.Height;
            int x = (screenW / 2) - (frm.Width / 2);
            int y = screenH - frm.Height;
            frm.DesktopLocation = new Point(x, y);
        }

        /// <summary>
        /// フォームを画面の一番上中央に移動させる
        /// </summary>
        static public void MoveTopCenter(Form frm)
        {
            int screenW = Screen.PrimaryScreen.WorkingArea.Width;
            int x = (screenW / 2) - (frm.Width / 2);
            frm.DesktopLocation = new Point(x, 0);
        }

    }

    //ＵＩ用の、テンポラリ記憶
    class UIValRecord
    {
        private static readonly UIValRecord instance = new UIValRecord();
        public int EditRecipeNum = 0;

        private UIValRecord()
        {
        }

        static public UIValRecord GetInstance()
        {
            return instance;
        }
    }

    //class DumpException
    //{
    //    public static string Total(Exception e)
    //    {

    //    }
    //}

}
