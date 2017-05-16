using System;
using System.Drawing;
using System.Windows.Forms;

namespace DirectXBaseCSharp
{
    public partial class RenderControl : UserControl
    {
		private Renderer renderer;

        public RenderControl()
        {
            InitializeComponent();
        }

		internal void RequestRender() => renderer.RenderFrame();

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			RequestRender();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			renderer = new Renderer(new WinFormsArgs(Size, Handle));
		}
		
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
				renderer.Dispose();
			}
			base.Dispose(disposing);
		}
	}

	public struct WinFormsArgs
	{
		public readonly Size Size;
		public readonly IntPtr ControlHandle;

		public WinFormsArgs(Size size, IntPtr controlHandle)
		{
			Size = size;
			ControlHandle = controlHandle;
		}
	}
}
