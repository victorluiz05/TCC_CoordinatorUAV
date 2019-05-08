namespace Coordinator
{
    partial class Bases
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.btnEnterCoordinates = new System.Windows.Forms.Button();
            this.btnStartCoord = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(153, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Number of Distribution Centers:";
            // 
            // txtNumber
            // 
            this.txtNumber.Location = new System.Drawing.Point(159, 8);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(66, 20);
            this.txtNumber.TabIndex = 1;
            this.txtNumber.Text = "3";
            // 
            // btnEnterCoordinates
            // 
            this.btnEnterCoordinates.Location = new System.Drawing.Point(231, 6);
            this.btnEnterCoordinates.Name = "btnEnterCoordinates";
            this.btnEnterCoordinates.Size = new System.Drawing.Size(106, 23);
            this.btnEnterCoordinates.TabIndex = 2;
            this.btnEnterCoordinates.Text = "Enter Coordinates";
            this.btnEnterCoordinates.UseVisualStyleBackColor = true;
            this.btnEnterCoordinates.Click += new System.EventHandler(this.btnEnterCoordinates_Click);
            // 
            // btnStartCoord
            // 
            this.btnStartCoord.Location = new System.Drawing.Point(3, 226);
            this.btnStartCoord.Name = "btnStartCoord";
            this.btnStartCoord.Size = new System.Drawing.Size(312, 23);
            this.btnStartCoord.TabIndex = 3;
            this.btnStartCoord.Text = "Start Coordinator";
            this.btnStartCoord.UseVisualStyleBackColor = true;
            this.btnStartCoord.Click += new System.EventHandler(this.btnStartCoord_Click);
            // 
            // Bases
            // 
            this.ClientSize = new System.Drawing.Size(339, 261);
            this.Controls.Add(this.btnStartCoord);
            this.Controls.Add(this.btnEnterCoordinates);
            this.Controls.Add(this.txtNumber);
            this.Controls.Add(this.label2);
            this.Name = "Bases";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnEnter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNumberBases;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNumber;
        private System.Windows.Forms.Button btnEnterCoordinates;
        private System.Windows.Forms.Button btnStartCoord;
    }
}