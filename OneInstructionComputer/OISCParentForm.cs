using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OneInstructionComputer
{
    public partial class OISCParentForm : Form
    {
        
        public OISCParentForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// User wants to open a script.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                ScriptEditor editor = new ScriptEditor();
                editor.MdiParent = this;
                editor.Open(openFileDialog.FileName);
                editor.Show();
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild.GetType() == typeof(ScriptEditor))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    ((ScriptEditor)this.ActiveMdiChild).SaveAs(saveFileDialog.FileName);
                }
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void OISCParentForm_Load(object sender, EventArgs e)
        {
            
        }

        private void captureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form captureForm = new Capture();
            captureForm.MdiParent = this;
            captureForm.Text = "Capture";
            captureForm.Show();
        }

        private void registersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form captureForm = new Registers();
            captureForm.MdiParent = this;
            captureForm.Text = "Registers";
            captureForm.Show();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild.GetType() == typeof(ScriptEditor))
            {
                ((ScriptEditor)this.ActiveMdiChild).Save();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild.GetType() == typeof(ScriptEditor))
            {
                ((ScriptEditor)this.ActiveMdiChild).Save();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createNewScript();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            createNewScript();
        }

        private void createNewScript()
        {
            ScriptEditor editor = new ScriptEditor();
            editor.MdiParent = this;
            editor.Show();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild.GetType() == typeof(ScriptEditor))
            {
                if (!OISC.Running)
                {
                    ((ScriptEditor)this.ActiveMdiChild).Save();
                    OISC.Script = ((ScriptEditor)this.ActiveMdiChild).ScriptBody;
                    OISC.SetBreakpoints(((ScriptEditor)this.ActiveMdiChild).Breakpoints);
                    OISC.Run();
                }
                else
                {
                    OISC.Continue();
                }
            }
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild.GetType() == typeof(ScriptEditor))
            {
                ((ScriptEditor)this.ActiveMdiChild).Save();
                OISC.Script = ((ScriptEditor)this.ActiveMdiChild).ScriptBody;
                OISC.Break();
                OISC.Run();
            }
        }

        private void stepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OISC.Step();
        }

        /// <summary>
        /// User is opening the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolsMenu_DropDownOpening(object sender, EventArgs e)
        {
            runToolStripMenuItem.Enabled = (this.ActiveMdiChild !=null && this.ActiveMdiChild.GetType() == typeof(ScriptEditor));
            debugToolStripMenuItem.Enabled = (this.ActiveMdiChild != null && this.ActiveMdiChild.GetType() == typeof(ScriptEditor));
        }

        /// <summary>
        /// User wants to toggle a breakpoint
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toggleBreakpointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild.GetType() == typeof(ScriptEditor))
            {
                ((ScriptEditor)this.ActiveMdiChild).ToggleBreakpoint();
            }
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

    }
}
