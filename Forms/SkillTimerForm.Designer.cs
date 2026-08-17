namespace _4RTools.Forms
{
    partial class SkillTimerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.headerDelay = new System.Windows.Forms.Label();
            this.rowLabel1 = new System.Windows.Forms.Label();
            this.rowLabel2 = new System.Windows.Forms.Label();
            this.rowLabel3 = new System.Windows.Forms.Label();
            this.txtSkillTimerKey = new System.Windows.Forms.TextBox();
            this.txtSkillTimerKey2 = new System.Windows.Forms.TextBox();
            this.txtSkillTimerKey3 = new System.Windows.Forms.TextBox();
            this.txtAutoRefreshDelay = new System.Windows.Forms.NumericUpDown();
            this.txtAutoRefreshDelay2 = new System.Windows.Forms.NumericUpDown();
            this.txtAutoRefreshDelay3 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.txtAutoRefreshDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAutoRefreshDelay2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAutoRefreshDelay3)).BeginInit();
            this.SuspendLayout();
            // 
            // headerDelay
            // 
            this.headerDelay.AutoSize = true;
            this.headerDelay.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.headerDelay.Location = new System.Drawing.Point(127, 8);
            this.headerDelay.Name = "headerDelay";
            this.headerDelay.Size = new System.Drawing.Size(66, 15);
            this.headerDelay.TabIndex = 1;
            this.headerDelay.Text = "Delay (ms)";
            // 
            // rowLabel1
            // 
            this.rowLabel1.AutoSize = true;
            this.rowLabel1.Location = new System.Drawing.Point(24, 33);
            this.rowLabel1.Name = "rowLabel1";
            this.rowLabel1.Size = new System.Drawing.Size(19, 13);
            this.rowLabel1.TabIndex = 2;
            this.rowLabel1.Text = "#1";
            // 
            // rowLabel2
            // 
            this.rowLabel2.AutoSize = true;
            this.rowLabel2.Location = new System.Drawing.Point(24, 58);
            this.rowLabel2.Name = "rowLabel2";
            this.rowLabel2.Size = new System.Drawing.Size(19, 13);
            this.rowLabel2.TabIndex = 3;
            this.rowLabel2.Text = "#2";
            // 
            // rowLabel3
            // 
            this.rowLabel3.AutoSize = true;
            this.rowLabel3.Location = new System.Drawing.Point(24, 83);
            this.rowLabel3.Name = "rowLabel3";
            this.rowLabel3.Size = new System.Drawing.Size(19, 13);
            this.rowLabel3.TabIndex = 4;
            this.rowLabel3.Text = "#3";
            // 
            // txtSkillTimerKey
            // 
            this.txtSkillTimerKey.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtSkillTimerKey.Location = new System.Drawing.Point(66, 27);
            this.txtSkillTimerKey.Name = "txtSkillTimerKey";
            this.txtSkillTimerKey.Size = new System.Drawing.Size(56, 23);
            this.txtSkillTimerKey.TabIndex = 8;
            this.txtSkillTimerKey.Text = "None";
            // 
            // txtSkillTimerKey2
            // 
            this.txtSkillTimerKey2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtSkillTimerKey2.Location = new System.Drawing.Point(66, 52);
            this.txtSkillTimerKey2.Name = "txtSkillTimerKey2";
            this.txtSkillTimerKey2.Size = new System.Drawing.Size(56, 23);
            this.txtSkillTimerKey2.TabIndex = 9;
            this.txtSkillTimerKey2.Text = "None";
            // 
            // txtSkillTimerKey3
            // 
            this.txtSkillTimerKey3.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtSkillTimerKey3.Location = new System.Drawing.Point(66, 77);
            this.txtSkillTimerKey3.Name = "txtSkillTimerKey3";
            this.txtSkillTimerKey3.Size = new System.Drawing.Size(56, 23);
            this.txtSkillTimerKey3.TabIndex = 10;
            this.txtSkillTimerKey3.Text = "None";
            // 
            // txtAutoRefreshDelay
            // 
            this.txtAutoRefreshDelay.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtAutoRefreshDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtAutoRefreshDelay.Location = new System.Drawing.Point(128, 27);
            this.txtAutoRefreshDelay.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.txtAutoRefreshDelay.Name = "txtAutoRefreshDelay";
            this.txtAutoRefreshDelay.Size = new System.Drawing.Size(108, 23);
            this.txtAutoRefreshDelay.TabIndex = 11;
            this.txtAutoRefreshDelay.ThousandsSeparator = true;
            this.txtAutoRefreshDelay.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // txtAutoRefreshDelay2
            // 
            this.txtAutoRefreshDelay2.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtAutoRefreshDelay2.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtAutoRefreshDelay2.Location = new System.Drawing.Point(128, 52);
            this.txtAutoRefreshDelay2.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.txtAutoRefreshDelay2.Name = "txtAutoRefreshDelay2";
            this.txtAutoRefreshDelay2.Size = new System.Drawing.Size(108, 23);
            this.txtAutoRefreshDelay2.TabIndex = 12;
            this.txtAutoRefreshDelay2.ThousandsSeparator = true;
            this.txtAutoRefreshDelay2.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // txtAutoRefreshDelay3
            // 
            this.txtAutoRefreshDelay3.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtAutoRefreshDelay3.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.txtAutoRefreshDelay3.Location = new System.Drawing.Point(128, 77);
            this.txtAutoRefreshDelay3.Maximum = new decimal(new int[] {
            60000,
            0,
            0,
            0});
            this.txtAutoRefreshDelay3.Name = "txtAutoRefreshDelay3";
            this.txtAutoRefreshDelay3.Size = new System.Drawing.Size(108, 23);
            this.txtAutoRefreshDelay3.TabIndex = 13;
            this.txtAutoRefreshDelay3.ThousandsSeparator = true;
            this.txtAutoRefreshDelay3.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // SkillTimerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(241)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(242, 106);
            this.Controls.Add(this.txtAutoRefreshDelay3);
            this.Controls.Add(this.txtAutoRefreshDelay2);
            this.Controls.Add(this.txtAutoRefreshDelay);
            this.Controls.Add(this.txtSkillTimerKey3);
            this.Controls.Add(this.txtSkillTimerKey2);
            this.Controls.Add(this.txtSkillTimerKey);
            this.Controls.Add(this.rowLabel3);
            this.Controls.Add(this.rowLabel2);
            this.Controls.Add(this.rowLabel1);
            this.Controls.Add(this.headerDelay);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SkillTimerForm";
            this.Text = "SkillTimerForm";
            ((System.ComponentModel.ISupportInitialize)(this.txtAutoRefreshDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAutoRefreshDelay2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAutoRefreshDelay3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label headerDelay;
        private System.Windows.Forms.Label rowLabel1;
        private System.Windows.Forms.Label rowLabel2;
        private System.Windows.Forms.Label rowLabel3;
        private System.Windows.Forms.TextBox txtSkillTimerKey;
        private System.Windows.Forms.TextBox txtSkillTimerKey2;
        private System.Windows.Forms.TextBox txtSkillTimerKey3;
        private System.Windows.Forms.NumericUpDown txtAutoRefreshDelay;
        private System.Windows.Forms.NumericUpDown txtAutoRefreshDelay2;
        private System.Windows.Forms.NumericUpDown txtAutoRefreshDelay3;
    }
}
