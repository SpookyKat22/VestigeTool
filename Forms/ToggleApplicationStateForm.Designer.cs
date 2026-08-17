using System.Windows.Forms;

namespace _4RTools.Forms
{
    partial class ToggleApplicationStateForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToggleApplicationStateForm));
            this.txtStatusToggleKey = new System.Windows.Forms.TextBox();
            this.btnStatusToggle = new System.Windows.Forms.Button();
            this.lblStatusToggle = new System.Windows.Forms.Label();
            this.notifyIconTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbAudio = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtStatusToggleKey
            // 
            this.txtStatusToggleKey.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtStatusToggleKey.Location = new System.Drawing.Point(74, 51);
            this.txtStatusToggleKey.Name = "txtStatusToggleKey";
            this.txtStatusToggleKey.Size = new System.Drawing.Size(61, 23);
            this.txtStatusToggleKey.TabIndex = 23;
            this.txtStatusToggleKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnStatusToggle
            // 
            this.btnStatusToggle.BackColor = System.Drawing.Color.Red;
            this.btnStatusToggle.FlatAppearance.BorderSize = 0;
            this.btnStatusToggle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatusToggle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStatusToggle.ForeColor = System.Drawing.SystemColors.Window;
            this.btnStatusToggle.Location = new System.Drawing.Point(72, 16);
            this.btnStatusToggle.Margin = new System.Windows.Forms.Padding(0);
            this.btnStatusToggle.Name = "btnStatusToggle";
            this.btnStatusToggle.Size = new System.Drawing.Size(64, 32);
            this.btnStatusToggle.TabIndex = 21;
            this.btnStatusToggle.UseVisualStyleBackColor = false;
            this.btnStatusToggle.Click += new System.EventHandler(this.btnToggleStatusHandler);
            // 
            // lblStatusToggle
            // 
            this.lblStatusToggle.AllowDrop = true;
            this.lblStatusToggle.AutoSize = false;
            this.lblStatusToggle.Location = new System.Drawing.Point(18, 78);
            this.lblStatusToggle.MaximumSize = new System.Drawing.Size(188, 32);
            this.lblStatusToggle.Name = "lblStatusToggle";
            this.lblStatusToggle.Size = new System.Drawing.Size(188, 30);
            this.lblStatusToggle.TabIndex = 22;
            this.lblStatusToggle.Text = "Click or press the hotkey to start";
            this.lblStatusToggle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // notifyIconTray
            // 
            this.notifyIconTray.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconTray.Icon")));
            this.notifyIconTray.Text = "CookieCutter";
            this.notifyIconTray.Visible = true;
            this.notifyIconTray.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconDoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbAudio);
            this.groupBox1.Controls.Add(this.txtStatusToggleKey);
            this.groupBox1.Controls.Add(this.btnStatusToggle);
            this.groupBox1.Controls.Add(this.lblStatusToggle);
            this.groupBox1.Location = new System.Drawing.Point(1, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(222, 118);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // cbAudio
            // 
            this.cbAudio.AutoSize = true;
            this.cbAudio.Checked = true;
            this.cbAudio.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAudio.Location = new System.Drawing.Point(156, 23);
            this.cbAudio.Name = "cbAudio";
            this.cbAudio.Size = new System.Drawing.Size(57, 17);
            this.cbAudio.TabIndex = 24;
            this.cbAudio.Text = "Sound";
            this.cbAudio.UseVisualStyleBackColor = true;
            // 
            // ToggleApplicationStateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(241)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(226, 122);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ToggleApplicationStateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ToggleApplicationStateForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnStatusToggle;
        private System.Windows.Forms.Label lblStatusToggle;
        private System.Windows.Forms.TextBox txtStatusToggleKey;
        private System.Windows.Forms.NotifyIcon notifyIconTray;
        private GroupBox groupBox1;
        private CheckBox cbAudio;
    }
}
