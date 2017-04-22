namespace OneInstructionComputer
{
    partial class ScriptEditor
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
            this.textBoxScript = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // textBoxScript
            // 
            this.textBoxScript.AcceptsTab = true;
            this.textBoxScript.AutoWordSelection = true;
            this.textBoxScript.CausesValidation = false;
            this.textBoxScript.DetectUrls = false;
            this.textBoxScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxScript.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxScript.Location = new System.Drawing.Point(0, 0);
            this.textBoxScript.Margin = new System.Windows.Forms.Padding(20, 3, 3, 3);
            this.textBoxScript.Name = "textBoxScript";
            this.textBoxScript.ShowSelectionMargin = true;
            this.textBoxScript.Size = new System.Drawing.Size(284, 264);
            this.textBoxScript.TabIndex = 1;
            this.textBoxScript.Text = "";
            this.textBoxScript.WordWrap = false;
            this.textBoxScript.TextChanged += new System.EventHandler(this.textBoxScript_TextChanged);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.textBoxScript);
            this.DoubleBuffered = true;
            this.Name = "ScriptEditor";
            this.ShowIcon = false;
            this.Text = "ScriptEditor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox textBoxScript;

    }
}