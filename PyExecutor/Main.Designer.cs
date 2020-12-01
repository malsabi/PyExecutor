
namespace PyExecutor
{
    partial class Main
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
            this.LogBox = new IViewCustomControls.IViewRichTextBox();
            this.InputQueueCountLabel = new System.Windows.Forms.Label();
            this.QueueStatisticsGroupBox = new IViewCustomControls.IViewGroupBox();
            this.OutputQueueCountLabel = new System.Windows.Forms.Label();
            this.QueueCounter = new System.Windows.Forms.Timer(this.components);
            this.QueueStatisticsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // LogBox
            // 
            this.LogBox.BackColor = System.Drawing.Color.Black;
            this.LogBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.LogBox.Font = new System.Drawing.Font("Verdana", 13F);
            this.LogBox.ForeColor = System.Drawing.Color.White;
            this.LogBox.Location = new System.Drawing.Point(0, 0);
            this.LogBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.LogBox.Name = "LogBox";
            this.LogBox.Selectable = false;
            this.LogBox.Size = new System.Drawing.Size(558, 528);
            this.LogBox.TabIndex = 0;
            this.LogBox.Text = "";
            // 
            // InputQueueCountLabel
            // 
            this.InputQueueCountLabel.AutoSize = true;
            this.InputQueueCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.InputQueueCountLabel.Font = new System.Drawing.Font("Verdana", 12F);
            this.InputQueueCountLabel.ForeColor = System.Drawing.Color.Black;
            this.InputQueueCountLabel.Location = new System.Drawing.Point(8, 66);
            this.InputQueueCountLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.InputQueueCountLabel.Name = "InputQueueCountLabel";
            this.InputQueueCountLabel.Size = new System.Drawing.Size(194, 18);
            this.InputQueueCountLabel.TabIndex = 1;
            this.InputQueueCountLabel.Text = "Input Queue Count:  0";
            // 
            // QueueStatisticsGroupBox
            // 
            this.QueueStatisticsGroupBox.AnimationSpeed = 30;
            this.QueueStatisticsGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(142)))), ((int)(((byte)(192)))));
            this.QueueStatisticsGroupBox.BorderColor = System.Drawing.Color.Black;
            this.QueueStatisticsGroupBox.BoxHeight = 50;
            this.QueueStatisticsGroupBox.Collapse = true;
            this.QueueStatisticsGroupBox.Controls.Add(this.OutputQueueCountLabel);
            this.QueueStatisticsGroupBox.Controls.Add(this.InputQueueCountLabel);
            this.QueueStatisticsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.QueueStatisticsGroupBox.EnableCollapsing = true;
            this.QueueStatisticsGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(220)))), ((int)(((byte)(232)))));
            this.QueueStatisticsGroupBox.Location = new System.Drawing.Point(558, 0);
            this.QueueStatisticsGroupBox.Name = "QueueStatisticsGroupBox";
            this.QueueStatisticsGroupBox.Padding = new System.Windows.Forms.Padding(3, 40, 3, 3);
            this.QueueStatisticsGroupBox.PreviousHeight = 159;
            this.QueueStatisticsGroupBox.Size = new System.Drawing.Size(367, 50);
            this.QueueStatisticsGroupBox.TabIndex = 2;
            this.QueueStatisticsGroupBox.TabStop = false;
            this.QueueStatisticsGroupBox.Text = "Queue Statistics";
            // 
            // OutputQueueCountLabel
            // 
            this.OutputQueueCountLabel.AutoSize = true;
            this.OutputQueueCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.OutputQueueCountLabel.Font = new System.Drawing.Font("Verdana", 12F);
            this.OutputQueueCountLabel.ForeColor = System.Drawing.Color.Black;
            this.OutputQueueCountLabel.Location = new System.Drawing.Point(8, 103);
            this.OutputQueueCountLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.OutputQueueCountLabel.Name = "OutputQueueCountLabel";
            this.OutputQueueCountLabel.Size = new System.Drawing.Size(207, 18);
            this.OutputQueueCountLabel.TabIndex = 2;
            this.OutputQueueCountLabel.Text = "Output Queue Count:  0";
            // 
            // QueueCounter
            // 
            this.QueueCounter.Enabled = true;
            this.QueueCounter.Interval = 1;
            this.QueueCounter.Tick += new System.EventHandler(this.QueueCounter_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(925, 528);
            this.Controls.Add(this.QueueStatisticsGroupBox);
            this.Controls.Add(this.LogBox);
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Main";
            this.Text = "Main";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.QueueStatisticsGroupBox.ResumeLayout(false);
            this.QueueStatisticsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private IViewCustomControls.IViewRichTextBox LogBox;
        private System.Windows.Forms.Label InputQueueCountLabel;
        private IViewCustomControls.IViewGroupBox QueueStatisticsGroupBox;
        private System.Windows.Forms.Label OutputQueueCountLabel;
        private System.Windows.Forms.Timer QueueCounter;
    }
}