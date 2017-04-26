namespace Ants.HPA
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.x1NUD = new System.Windows.Forms.NumericUpDown();
            this.y1NUD = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.y2NUD = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.x2NUD = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.x1NUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.y1NUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.y2NUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.x2NUD)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(600, 600);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(627, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Build Setup";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(96, 86);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Find Path";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "x1:";
            // 
            // x1NUD
            // 
            this.x1NUD.Location = new System.Drawing.Point(42, 25);
            this.x1NUD.Name = "x1NUD";
            this.x1NUD.Size = new System.Drawing.Size(48, 20);
            this.x1NUD.TabIndex = 4;
            this.x1NUD.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
            // 
            // y1NUD
            // 
            this.y1NUD.Location = new System.Drawing.Point(123, 25);
            this.y1NUD.Name = "y1NUD";
            this.y1NUD.Size = new System.Drawing.Size(48, 20);
            this.y1NUD.TabIndex = 6;
            this.y1NUD.Value = new decimal(new int[] {
            45,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "y1:";
            // 
            // y2NUD
            // 
            this.y2NUD.Location = new System.Drawing.Point(123, 51);
            this.y2NUD.Name = "y2NUD";
            this.y2NUD.Size = new System.Drawing.Size(48, 20);
            this.y2NUD.TabIndex = 10;
            this.y2NUD.Value = new decimal(new int[] {
            64,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(96, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "y2:";
            // 
            // x2NUD
            // 
            this.x2NUD.Location = new System.Drawing.Point(42, 51);
            this.x2NUD.Name = "x2NUD";
            this.x2NUD.Size = new System.Drawing.Size(48, 20);
            this.x2NUD.TabIndex = 8;
            this.x2NUD.Value = new decimal(new int[] {
            64,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "x2:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.y2NUD);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.x1NUD);
            this.groupBox1.Controls.Add(this.x2NUD);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.y1NUD);
            this.groupBox1.Location = new System.Drawing.Point(627, 41);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(183, 119);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Path Finding";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 657);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Main";
            this.Text = "Hierachically Path-Finding Visualizer";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.x1NUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.y1NUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.y2NUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.x2NUD)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown x1NUD;
        private System.Windows.Forms.NumericUpDown y1NUD;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown y2NUD;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown x2NUD;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}