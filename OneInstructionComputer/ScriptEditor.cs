using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace OneInstructionComputer
{
    public partial class ScriptEditor : Form
    {
        public ScriptEditor()
        {
            InitializeComponent();
            this.Text = "New";
            OISC.BreakpointHit += new OISC.BreakpointHitDelegate(OISC_BreakpointHit);
        }

        private String scriptFile = "";

        public void Open(string filename)
        {
            scriptFile = filename;
            try
            {
                TextReader reader = new StreamReader(scriptFile);
                textBoxScript.Text = reader.ReadToEnd();
                doSyntaxHighlight();
                reader.Close();
                this.Text = scriptFile;
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to open script");
            }
        }

        public void SaveAs(String fileName)
        {
            scriptFile = fileName;
            Save();
        }

        public void Save()
        {
            if (scriptFile != "")
            {
                TextWriter writer = new StreamWriter(scriptFile);
                writer.Write(textBoxScript.Text);
                writer.Close();
                this.Text = scriptFile;
            }
        }

        /// <summary>
        /// Gets all breakpoints currently set.
        /// </summary>
        public List<int> Breakpoints
        {
            get
            {
                return breakpointLines;
            }
        }

        /// <summary>
        /// Toggle breakpoint on current line
        /// </summary>
        public void ToggleBreakpoint()
        {
            int lineNumber = textBoxScript.GetLineFromCharIndex(textBoxScript.SelectionStart);
            if (breakpointLines.Contains(lineNumber))
            {
                breakpointLines.Remove(lineNumber);
            }
            else
            {
                breakpointLines.Add(lineNumber);
            }

            doSyntaxHighlight();
        }

        /// <summary>
        /// Lines that have a breakpoint.
        /// </summary>
        private List<int> breakpointLines = new List<int>();

        private void textBoxScript_TextChanged(object sender, EventArgs e)
        {
            this.Text = "* " + scriptFile;
            doSyntaxHighlight();
        }

        /// <summary>
        /// Gets the whole script body.
        /// </summary>
        public String ScriptBody
        {
            get
            {
                return textBoxScript.Text;
            }
        }

        /// <summary>
        /// We have hit a breakpoint.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OISC_BreakpointHit(object sender, OISC.BreakpointHitEventArgs e)
        {
            // TODO: figure out if we are the script running, in case user has many windows open.
            currentHighlightedLine = e.OriginalSourceLine;

            doSyntaxHighlight();
        }

        /// <summary>
        /// Currently highlighted line number
        /// </summary>
        private int currentHighlightedLine = -1; 

        
        private bool syntaxHighlightOngoing = false;

        /// <summary>
        /// Delegate for void functions.
        /// </summary>
        private delegate void voidDelegate();

        private void doSyntaxHighlight()
        {
            if (syntaxHighlightOngoing)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                this.Invoke(new voidDelegate(doSyntaxHighlight));
                return;
            }

            syntaxHighlightOngoing = true;

            // Stop redrawing and sending events, this prevents flickering
            //  and to get ourself called over and over by textChanged event
            SendMessage(this.Handle, WM_SETREDRAW, 0, IntPtr.Zero);
            eventMask = SendMessage(this.Handle, EM_GETEVENTMASK, 0, IntPtr.Zero);

            int currentIndex = textBoxScript.SelectionStart;

            // Reset text color and properties
            textBoxScript.SelectAll();
            textBoxScript.SelectionColor = Color.Black;
            textBoxScript.SelectionBackColor = Color.White;

            textBoxScript.SelectionFont = new Font(textBoxScript.SelectionFont, FontStyle.Regular);
            
            int startIndex = 0;
            int endIndex = 0;
            while (startIndex >= 0)
            {
                startIndex = textBoxScript.Text.IndexOf("#", startIndex + 1);
                if (startIndex >= 0)
                {
                    endIndex = textBoxScript.Text.IndexOf('\n', startIndex);
                    if (endIndex > 0)
                    {
                        textBoxScript.Select(startIndex, endIndex - startIndex);
                        textBoxScript.SelectionColor = Color.Brown;
                    }
                }
            }

            startIndex = 0;
            endIndex = 0;
            while (startIndex >= 0)
            {
                startIndex = textBoxScript.Text.IndexOf(";", startIndex + 1);
                if (startIndex >= 0)
                {
                    endIndex = textBoxScript.Text.IndexOf('\n', startIndex);
                    textBoxScript.Select(startIndex, endIndex - startIndex);
                    textBoxScript.SelectionColor = Color.Green;
                    textBoxScript.SelectionFont = new Font(textBoxScript.SelectionFont, FontStyle.Bold);
                }
            }

            foreach (int lineNumber in breakpointLines)
            {
                startIndex = textBoxScript.GetFirstCharIndexFromLine(lineNumber);
                endIndex = textBoxScript.Text.IndexOf("\n", startIndex);
                textBoxScript.SelectionStart = startIndex;
                textBoxScript.SelectionLength = endIndex - startIndex;
                textBoxScript.SelectionBackColor = Color.Brown;
                textBoxScript.SelectionColor = Color.White;
            }

            if (currentHighlightedLine >= 0)
            {
                textBoxScript.SelectionStart = textBoxScript.GetFirstCharIndexFromLine(currentHighlightedLine);
                textBoxScript.SelectionLength = textBoxScript.Text.IndexOf("\n", textBoxScript.SelectionStart) - textBoxScript.SelectionStart;
                textBoxScript.SelectionBackColor = Color.Yellow;
            }

            textBoxScript.SelectionStart = currentIndex;
            textBoxScript.SelectionLength = 0;

            // Turn back events and refresh.
            SendMessage(this.Handle, EM_SETEVENTMASK, 0, eventMask);
            SendMessage(this.Handle, WM_SETREDRAW, 1, IntPtr.Zero);
            this.Invalidate();
            this.Refresh();

            syntaxHighlightOngoing = false;
        }

        IntPtr eventMask = IntPtr.Zero;

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 0x000B;
        private const int WM_USER = 0x400;
        private const int EM_GETEVENTMASK = (WM_USER + 59);
        private const int EM_SETEVENTMASK = (WM_USER + 69);

    }
}
