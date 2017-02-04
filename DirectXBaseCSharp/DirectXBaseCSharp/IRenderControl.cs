using System;

namespace DirectXBaseCSharp
{
    interface IRenderControl
    {
        event Action Render;

        IntPtr Handle { get; }
        int Width { get; }
        int Height { get; }
    }
}
