using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System.Drawing;
using System;
using DirectXBaseCSharp.Scenes;
using Device = SharpDX.Direct3D11.Device;

namespace DirectXBaseCSharp
{
	class Renderer : IDisposable
    {
        private static readonly Format textureFormat = Format.R8G8B8A8_UNorm;
        private readonly SampleDescription sampleDescription = new SampleDescription(1, 0);
        private readonly RawColor4 backColor = new RawColor4(1, 1, 1, 1);

		private readonly Size size;
		private readonly IntPtr handle;

		private Device device;
        private DeviceContext context;
        private SwapChain swapChain;

        private RenderTargetView renderTargetView;
        private DepthStencilView depthStencilView;
        private RawViewportF viewPort;

        private readonly BaseScene scene;

	    public Renderer(WinFormsArgs winFormsArgs)
		{
			size = winFormsArgs.Size;
			handle = winFormsArgs.ControlHandle;

			scene = new StencilTest();
			Init();
		}

		private void Init()
        {
            InitDeviceAndContext();
            InitSwapChain();
            InitRenderTargetAndDepthStencil();
            InitViewPort();
            context.OutputMerger.SetRenderTargets(depthStencilView, renderTargetView);

            scene.InitShadersAndLayout(device);
            scene.InitDrawingData(device);
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
	        using (var dxgiDevice = device.QueryInterface<SharpDX.DXGI.Device>())
	        {
		        using (var adapter = dxgiDevice.GetParent<Adapter>())
		        {
			        using (var factory = adapter.GetParent<Factory>())
			        {
				        swapChain = new SwapChain(factory, device, new SwapChainDescription
				        {
					        BufferCount = 1,
					        ModeDescription = new ModeDescription(
						        size.Width, size.Height,
						        new Rational(60, 1),
						        textureFormat),
					        IsWindowed = true,
					        OutputHandle = handle,
					        SampleDescription = sampleDescription,
					        SwapEffect = SwapEffect.Discard,
					        Usage = Usage.RenderTargetOutput
				        });
			        }
		        }
	        }
        }

        private void InitRenderTargetAndDepthStencil()
        {
            using (var backBuffer = swapChain.GetBackBuffer<Texture2D>(0))
            {
                renderTargetView = new RenderTargetView(device, backBuffer);
            }

            if (!scene.TryGetCustomDepthStencilView(device, sampleDescription, size, out depthStencilView))
            {
	            var depthTextureDescription = new Texture2DDescription
	            {
		            BindFlags = BindFlags.DepthStencil,
		            Format = Format.D32_Float,
		            Width = size.Width,
		            Height = size.Width,
		            MipLevels = 1,
		            SampleDescription = sampleDescription,
		            Usage = ResourceUsage.Default,
		            OptionFlags = ResourceOptionFlags.None,
		            CpuAccessFlags = CpuAccessFlags.None,
		            ArraySize = 1
	            };
	            using (var depthTexture = new Texture2D(device, depthTextureDescription))
	            {
		            depthStencilView = new DepthStencilView(device, depthTexture);
	            }
			}
        }

        private void InitViewPort()
        {
            viewPort = new RawViewportF
            {
                Width = size.Width,
                Height = size.Height,
                X = 0, 
                Y = 0,
                MinDepth = 0,
                MaxDepth = 1,
            };
        }

        public void RenderFrame()
        {
			// Base view setup.
	        context.Rasterizer.SetViewport(viewPort);
	        context.ClearRenderTargetView(renderTargetView, backColor);
	        if (scene.PreDrawHandler == null)
				context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1f, 0);
	        else
	        {
				scene.PreDrawHandler.PreDraw(context, depthStencilView);
			}
			// Custom render.
			scene.Render(context);
			// Present.
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

            scene.Dispose();
		}
    }
}