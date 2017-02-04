using System;
using System.Windows.Forms;

namespace DirectXBaseCSharp
{
    public partial class RenderControl : UserControl, IRenderControl
    {
        public event Action Render;

        public RenderControl()
        {
            InitializeComponent();
            Paint += (o, e) => Render();
        }
    }
}
