namespace LogCollector
{
    partial class frmMain
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if ( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpCollectFrom = new System.Windows.Forms.DateTimePicker();
            this.lblCollectTo = new System.Windows.Forms.Label();
            this.dtpCollectTo = new System.Windows.Forms.DateTimePicker();
            this.dgvCollectData = new System.Windows.Forms.DataGridView();
            this.btnCollect = new System.Windows.Forms.Button();
            this.lblMsg = new System.Windows.Forms.Label();
            this.lblSettingTitle = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCollectData)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "取得対象 開始日時";
            // 
            // dtpCollectFrom
            // 
            this.dtpCollectFrom.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpCollectFrom.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpCollectFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCollectFrom.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dtpCollectFrom.Location = new System.Drawing.Point(124, 5);
            this.dtpCollectFrom.Name = "dtpCollectFrom";
            this.dtpCollectFrom.Size = new System.Drawing.Size(183, 22);
            this.dtpCollectFrom.TabIndex = 1;
            // 
            // lblCollectTo
            // 
            this.lblCollectTo.AutoSize = true;
            this.lblCollectTo.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblCollectTo.Location = new System.Drawing.Point(10, 44);
            this.lblCollectTo.Name = "lblCollectTo";
            this.lblCollectTo.Size = new System.Drawing.Size(105, 12);
            this.lblCollectTo.TabIndex = 2;
            this.lblCollectTo.Text = "取得対象 終了日時";
            // 
            // dtpCollectTo
            // 
            this.dtpCollectTo.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpCollectTo.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dtpCollectTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpCollectTo.Location = new System.Drawing.Point(123, 37);
            this.dtpCollectTo.Name = "dtpCollectTo";
            this.dtpCollectTo.Size = new System.Drawing.Size(184, 22);
            this.dtpCollectTo.TabIndex = 3;
            // 
            // dgvCollectData
            // 
            this.dgvCollectData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCollectData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCollectData.Location = new System.Drawing.Point(12, 85);
            this.dgvCollectData.Name = "dgvCollectData";
            this.dgvCollectData.RowTemplate.Height = 21;
            this.dgvCollectData.Size = new System.Drawing.Size(477, 250);
            this.dgvCollectData.TabIndex = 4;
            // 
            // btnCollect
            // 
            this.btnCollect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCollect.Location = new System.Drawing.Point(407, 5);
            this.btnCollect.Name = "btnCollect";
            this.btnCollect.Size = new System.Drawing.Size(82, 22);
            this.btnCollect.TabIndex = 5;
            this.btnCollect.Text = "ログ収集";
            this.btnCollect.UseVisualStyleBackColor = true;
            this.btnCollect.Click += new System.EventHandler(this.btnCollect_Click);
            // 
            // lblMsg
            // 
            this.lblMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMsg.ForeColor = System.Drawing.Color.Navy;
            this.lblMsg.Location = new System.Drawing.Point(9, 338);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(47, 13);
            this.lblMsg.TabIndex = 6;
            this.lblMsg.Text = "lblMsg";
            // 
            // lblSettingTitle
            // 
            this.lblSettingTitle.AutoSize = true;
            this.lblSettingTitle.Location = new System.Drawing.Point(10, 70);
            this.lblSettingTitle.Name = "lblSettingTitle";
            this.lblSettingTitle.Size = new System.Drawing.Size(81, 12);
            this.lblSettingTitle.TabIndex = 7;
            this.lblSettingTitle.Text = "収集設定 一覧";
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 360);
            this.Controls.Add(this.lblSettingTitle);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.btnCollect);
            this.Controls.Add(this.dgvCollectData);
            this.Controls.Add(this.dtpCollectTo);
            this.Controls.Add(this.lblCollectTo);
            this.Controls.Add(this.dtpCollectFrom);
            this.Controls.Add(this.label1);
            this.Name = "frmMain";
            this.Text = "Log Collector";
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCollectData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpCollectFrom;
        private System.Windows.Forms.Label lblCollectTo;
        private System.Windows.Forms.DateTimePicker dtpCollectTo;
        private System.Windows.Forms.DataGridView dgvCollectData;
        private System.Windows.Forms.Button btnCollect;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Label lblSettingTitle;
        private System.Windows.Forms.Timer timer;
    }
}

