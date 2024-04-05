namespace OUVoiceBanker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            txtAcoCkpt = new TextBox();
            btnOpenAcoCkpt = new Button();
            btnOpenVarCkpt = new Button();
            label2 = new Label();
            txtVarCkpt = new TextBox();
            txtVoiceName = new TextBox();
            label6 = new Label();
            button7 = new Button();
            button8 = new Button();
            button9 = new Button();
            fdCkeckPoint = new OpenFileDialog();
            fdCommon = new OpenFileDialog();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(144, 15);
            label1.TabIndex = 0;
            label1.Text = "Acoustic Checkpoint Path";
            // 
            // txtAcoCkpt
            // 
            txtAcoCkpt.Location = new Point(12, 27);
            txtAcoCkpt.Name = "txtAcoCkpt";
            txtAcoCkpt.Size = new Size(421, 23);
            txtAcoCkpt.TabIndex = 1;
            // 
            // btnOpenAcoCkpt
            // 
            btnOpenAcoCkpt.Location = new Point(439, 27);
            btnOpenAcoCkpt.Name = "btnOpenAcoCkpt";
            btnOpenAcoCkpt.Size = new Size(75, 23);
            btnOpenAcoCkpt.TabIndex = 2;
            btnOpenAcoCkpt.Text = "Open";
            btnOpenAcoCkpt.UseVisualStyleBackColor = true;
            btnOpenAcoCkpt.Click += btnOpenAcoCkpt_Click;
            // 
            // btnOpenVarCkpt
            // 
            btnOpenVarCkpt.Location = new Point(439, 71);
            btnOpenVarCkpt.Name = "btnOpenVarCkpt";
            btnOpenVarCkpt.Size = new Size(75, 23);
            btnOpenVarCkpt.TabIndex = 3;
            btnOpenVarCkpt.Text = "Open";
            btnOpenVarCkpt.UseVisualStyleBackColor = true;
            btnOpenVarCkpt.Click += btnOpenVarCkpt_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 53);
            label2.Name = "label2";
            label2.Size = new Size(142, 15);
            label2.TabIndex = 4;
            label2.Text = "Variance Checkpoint Path";
            // 
            // txtVarCkpt
            // 
            txtVarCkpt.Location = new Point(12, 71);
            txtVarCkpt.Name = "txtVarCkpt";
            txtVarCkpt.Size = new Size(421, 23);
            txtVarCkpt.TabIndex = 5;
            // 
            // txtVoiceName
            // 
            txtVoiceName.Location = new Point(12, 115);
            txtVoiceName.Name = "txtVoiceName";
            txtVoiceName.Size = new Size(502, 23);
            txtVoiceName.TabIndex = 12;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 97);
            label6.Name = "label6";
            label6.Size = new Size(70, 15);
            label6.TabIndex = 13;
            label6.Text = "Voice Name";
            // 
            // button7
            // 
            button7.Location = new Point(12, 144);
            button7.Name = "button7";
            button7.Size = new Size(502, 23);
            button7.TabIndex = 22;
            button7.Text = "Start";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // button8
            // 
            button8.Location = new Point(12, 173);
            button8.Name = "button8";
            button8.Size = new Size(502, 23);
            button8.TabIndex = 23;
            button8.Text = "Open Temp Folder";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button9
            // 
            button9.Location = new Point(12, 202);
            button9.Name = "button9";
            button9.Size = new Size(502, 23);
            button9.TabIndex = 24;
            button9.Text = "Open Output Folder";
            button9.UseVisualStyleBackColor = true;
            button9.Click += button9_Click;
            // 
            // fdCkeckPoint
            // 
            fdCkeckPoint.Filter = "ckpt file|*.ckpt";
            fdCkeckPoint.RestoreDirectory = true;
            // 
            // fdCommon
            // 
            fdCommon.Filter = "ckpt file|*.ckpt|yaml file|*.yaml|text file|*.txt";
            fdCommon.RestoreDirectory = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(526, 235);
            Controls.Add(button9);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(label6);
            Controls.Add(txtVoiceName);
            Controls.Add(txtVarCkpt);
            Controls.Add(label2);
            Controls.Add(btnOpenVarCkpt);
            Controls.Add(btnOpenAcoCkpt);
            Controls.Add(txtAcoCkpt);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Text = "OUVoiceBanker";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtAcoCkpt;
        private Button btnOpenAcoCkpt;
        private Button btnOpenVarCkpt;
        private Label label2;
        private TextBox txtVarCkpt;
        private TextBox txtVoiceName;
        private Label label6;
        private Button button7;
        private Button button8;
        private Button button9;
        private OpenFileDialog fdCkeckPoint;
        private OpenFileDialog fdCommon;
    }
}
