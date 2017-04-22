using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace OneInstructionComputer
{
    public partial class Capture : Form
    {
        public Capture()
        {
            InitializeComponent();

            // Subscribe to events from OISC
            OISC.CaptureDone += new OISC.CaptureDoneDelegate(OISC_CaptureDone);
        }

        private void showCaptureData(Dictionary<String, String> captureData)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new showCaptureDataDelegate(showCaptureData), captureData);
                return;
            }

            textBoxSignals.Clear();
            foreach (String key in captureData.Keys.ToList<String>())
            {
                textBoxSignals.Text += captureData[key] + "\r\n";
            }
        }

        public delegate void showCaptureDataDelegate(Dictionary<String, String> captureData);

        // New capture data
        void OISC_CaptureDone(object sender, OISC.CaptureEventArgs e)
        {
            showCaptureData(e.CaptureData);
        }
    }

}
