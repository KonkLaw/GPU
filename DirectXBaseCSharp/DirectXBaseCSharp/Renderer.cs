using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System.Drawing;
using System;

using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace DirectXBaseCSharp
{
    class Renderer : IDisposable
    {
        private static readonly Format textureFormat = Format.R8G8B8A8_UNorm;
        private readonly SampleDescription sampleDescription = new SampleDescription(1, 0);
        private readonly RawColor4 backColor = new RawColor4(1, 1, 1, 1);

		private readonly Size Size;
		private readonly IntPtr Handle;

		private Device device;
        private DeviceContext context;
        private SwapChain swapChain;

        private RenderTargetView renderTargetView;
        private DepthStencilView depthStencilView;
        private RawViewportF viewPort;

        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout pos4Col3Layout;

        private VertexBufferBinding positionsBufferBinding;
        private VertexBufferBinding colorsBufferBinding;
        private int pointsCount;

		public Renderer(WinFormsArgs winFormsArgs)
		{
			Size = winFormsArgs.Size;
			Handle = winFormsArgs.ControlHandle;
			InitDirectX();
		}

		private void InitDirectX()
        {
            InitDeviceAndContext();
            InitSwapChain();
            InitRenderTargetAndDepthStencil();
            InitViewPort();
            InitShadersAndLayout();
            InitDrawingData();
            context.OutputMerger.SetRenderTargets(depthStencilView, renderTargetView);
        }

        private void InitDeviceAndContext()
        {
            device = new Device(DriverType.Hardware, DeviceCreationFlags.None, FeatureLevel.Level_11_0);
            context = device.ImmediateContext;

            //var arr = new int[]
            //{
            //device.CheckMultisampleQualityLevels(textureFormat, 1),
            //device.CheckMultisampleQualityLevels(textureFormat, 2),
            //device.CheckMultisampleQualityLevels(textureFormat, 4),
            //device.CheckMultisampleQualityLevels(textureFormat, 8),
            //};
            //MessageBox.Show($"{arr[0]}{arr[1]}{arr[2]}{arr[3]}");
        }

        private void InitSwapChain()
        {
            SharpDX.DXGI.Device dxgiDevice;
            Adapter adapret;
            Factory factory;
            using (new DisposableContainer(
                dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device>(),
                adapret = dxgiDevice.GetParent<Adapter>(),
                factory = adapret.GetParent<Factory>()))
            {
                swapChain = new SwapChain(factory, device, new SwapChainDescription
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(
						Size.Width, Size.Height,
                        new Rational(60, 1),
                        textureFormat),
                    IsWindowed = true,
                    OutputHandle = Handle,
                    SampleDescription = sampleDescription,
                    SwapEffect = SwapEffect.Discard,
                    Usage = Usage.RenderTargetOutput
                });
            }
        }

        private void InitRenderTargetAndDepthStencil()
        {
            using (var backBuffer = swapChain.GetBackBuffer<Texture2D>(0))
            {
                renderTargetView = new RenderTargetView(device, backBuffer);
            }

            var depthЕextureDescription = new Texture2DDescription
            {
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float,
                Width = Size.Width,
                Height = Size.Width,
                MipLevels = 1,
                SampleDescription = sampleDescription,
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1,
            };
            using (var depthTexture = new Texture2D(device, depthЕextureDescription))
            {
                depthStencilView = new DepthStencilView(device, depthTexture);
            }
        }

        private void InitViewPort()
        {
            viewPort = new RawViewportF
            {
                Width = Size.Width,
                Height = Size.Height,
                X = 0, 
                Y = 0,
                MinDepth = 0,
                MaxDepth = 1,
            };
        }

        private void InitShadersAndLayout()
        {
            byte[] signature = Helper.GetShaderByteCode("VertexShader");
            vertexShader = new VertexShader(device, signature);
            pixelShader = new PixelShader(device, Helper.GetShaderByteCode("PixelShader"));
            pos4Col3Layout = new InputLayout(device, signature, new[]
                {
                    new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                    new InputElement("COLOR", 0, Format.R32G32B32_Float, 0, 1),
                });
        }

        private void InitDrawingData()
        {
            RawVector4[] points = Helper.CreatePoints().ToArray();
            Buffer pointsBuffer = Buffer.Create(device, BindFlags.VertexBuffer, points);
            positionsBufferBinding = new VertexBufferBinding(pointsBuffer, 16, 0);
            RawVector3[] colors = Helper.CreateColors(points.Length);
            Buffer colorsBuffer = Buffer.Create(device, BindFlags.VertexBuffer, colors);
            colorsBufferBinding = new VertexBufferBinding(colorsBuffer, 12, 0);
            pointsCount = points.Length;
        }

        public void RenderFrame()
        {
            // View.
            context.Rasterizer.SetViewport(viewPort);
            context.ClearRenderTargetView(renderTargetView, backColor);
            context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1f, 0);
            // IA.
            context.InputAssembler.InputLayout = pos4Col3Layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            // Data.
            context.InputAssembler.SetVertexBuffers(0, positionsBufferBinding);
            context.InputAssembler.SetVertexBuffers(1, colorsBufferBinding);
            // Draw.
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);
            context.Draw(pointsCount, 0);

            swapChain.Present(0, PresentFlags.None);
        }

        public void Dispose()
        {
            device.Dispose();
            context.Dispose();
            swapChain.Dispose();

            context.OutputMerger.SetTargets(depthStencilView: null, renderTargetView: null);
            renderTargetView.Dispose();
            depthStencilView.Dispose();

            vertexShader.Dispose();
            pixelShader.Dispose();
            pos4Col3Layout.Dispose();
            positionsBufferBinding.Buffer.Dispose();
            colorsBufferBinding.Buffer.Dispose();
            colorsBufferBinding.Buffer.Dispose();
        }
    }
}