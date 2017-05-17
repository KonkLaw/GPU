using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DirectXBaseCSharp
{
    public partial class MainForm : Form
    {
	    private readonly string info;

        public MainForm()
        {
            InitializeComponent();
	        info = Environment.Is64BitProcess ? "x64" : "x86";
	        base.Text = info;
        }

        private void buttonRunTest_Click(object sender, System.EventArgs e)
        {
            Text = "!!! TEST ON RUN !!!";
            GC.Collect();
            GC.WaitForPendingFinalizers();
			renderControl.RequestRender();

            const int count = 1000;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < count; i++)
            {
				renderControl.RequestRender();
            }

            stopwatch.Stop();

            Text = info;
            MessageBox.Show("FPS: " + 
                Math.Round(
                    count / (stopwatch.ElapsedMilliseconds / TimeSpan.FromSeconds(1).TotalMilliseconds))
                .ToString());
        }
    }
}
