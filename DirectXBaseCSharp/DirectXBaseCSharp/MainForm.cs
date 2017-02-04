using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace DirectXBaseCSharp
{
    public partial class MainForm : Form
    {
        private Renderer renderer;

        public MainForm()
        {
            InitializeComponent();
            renderer = new Renderer(renderControl);
        }

        private void buttonRunTest_Click(object sender, System.EventArgs e)
        {
            Text = "!!! TEST ON RUN !!!";
            GC.Collect();
            GC.WaitForPendingFinalizers();
            renderer.RenderFrame();

            const int count = 1000;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < count; i++)
            {
                renderer.RenderFrame();
            }

            stopwatch.Stop();

            Text = "MainWindow";
            MessageBox.Show("FPS: " + 
                Math.Round(
                    count / (stopwatch.ElapsedMilliseconds / TimeSpan.FromSeconds(1).TotalMilliseconds))
                .ToString());
        }
    }
}
