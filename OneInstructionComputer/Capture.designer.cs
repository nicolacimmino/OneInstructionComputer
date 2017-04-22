namespace OneInstructionComputer
{
    partial class Capture
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
            this.textBoxSignals = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.interruptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxSignals
            // 
            this.textBoxSignals.AllowDrop = true;
            this.textBoxSignals.ContextMenuStrip = this.contextMenuStrip1;
            this.textBoxSignals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSignals.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSignals.Location = new System.Drawing.Point(0, 0);
            this.textBoxSignals.Multiline = true;
            this.textBoxSignals.Name = "textBoxSignals";
            this.textBoxSignals.Size = new System.Drawing.Size(433, 171);
            this.textBoxSignals.TabIndex = 4;
            this.textBoxSignals.WordWrap = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.interruptToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(121, 26);
            // 
            // interruptToolStripMenuItem
            // 
            this.interruptToolStripMenuItem.Name = "interruptToolStripMenuItem";
            this.interruptToolStripMenuItem.Size = new System.Drawing.Size(120, 22);
            this.interruptToolStripMenuItem.Text = "Interrupt";
            // 
            // Capture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 171);
            this.Controls.Add(this.textBoxSignals);
            this.Name = "Capture";
            this.ShowIcon = false;
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxSignals;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem interruptToolStripMenuItem;
    }
}

