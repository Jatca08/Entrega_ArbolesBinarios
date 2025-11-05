namespace Árbol__nombres
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
            btnAsignación = new Button();
            button1 = new Button();
            SuspendLayout();
            // 
            // btnAsignación
            // 
            btnAsignación.Location = new Point(471, 3);
            btnAsignación.Name = "btnAsignación";
            btnAsignación.Size = new Size(94, 29);
            btnAsignación.TabIndex = 0;
            btnAsignación.Text = "Asignación";
            btnAsignación.UseVisualStyleBackColor = true;
            btnAsignación.Click += btnAsignación_Click;
            // 
            // button1
            // 
            button1.Location = new Point(371, 3);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 1;
            button1.Text = "Glosario";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(btnAsignación);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnAsignación;
        private Button button1;
    }
}
