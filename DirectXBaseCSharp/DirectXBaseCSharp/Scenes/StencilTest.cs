using System.Drawing;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace DirectXBaseCSharp.Scenes
{
	class StencilTest : BaseScene
	{
		class StencilPreDraw : PreDrawHandler
		{
			public override void PreDraw(DeviceContext context, DepthStencilView depthStencilView)
			{
				context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);
			}
		}

		private VertexShader vertexShader;
		private PixelShader pixelShader;
		private InputLayout pos4Col3Layout;

		private VertexBufferBinding positionsBufferBinding;
		private VertexBufferBinding colorsBufferBinding;
		private int pointsCount;

		private DepthStencilState writeMask;
		private DepthStencilState applyMask;

		public StencilTest() : base(new StencilPreDraw()) { }

		public override bool TryGetCustomDepthStencilView(Device device, SampleDescription sampleDescription, Size size, out DepthStencilView depthStencilView)
		{
			var depthTextureDescription = new Texture2DDescription
			{
				BindFlags = BindFlags.DepthStencil,
				Format = Format.D24_UNorm_S8_UInt,
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
			return true;
		}



		public override void InitShadersAndLayout(Device device)
		{
			byte[] signature = Helper.GetShaderByteCode("SimpleTriangleVS");
			vertexShader = new VertexShader(device, signature);
			pixelShader = new PixelShader(device, Helper.GetShaderByteCode("SimpleTrianglePS"));
			pos4Col3Layout = new InputLayout(device, signature, new[]
			{
				new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
				new InputElement("COLOR", 0, Format.R32G32B32_Float, 0, 1),
			});




			writeMask = new DepthStencilState(device, new DepthStencilStateDescription
			{
				IsDepthEnabled = false,
				DepthWriteMask = DepthWriteMask.Zero,
				DepthComparison = Comparison.Never,

				IsStencilEnabled = true,
				StencilReadMask = byte.MaxValue,
				StencilWriteMask = byte.MaxValue,

				FrontFace = new DepthStencilOperationDescription
				{
					Comparison = Comparison.Always,
					FailOperation = StencilOperation.Keep,
					DepthFailOperation = StencilOperation.Keep,
					PassOperation = StencilOperation.IncrementAndClamp,
				},

				BackFace = new DepthStencilOperationDescription
				{
					Comparison = Comparison.Always,
					FailOperation = StencilOperation.Keep,
					DepthFailOperation = StencilOperation.Keep,
					PassOperation = StencilOperation.IncrementAndClamp,
				},
			});


			applyMask = new DepthStencilState(device, new DepthStencilStateDescription
			{
				IsDepthEnabled = true,
				DepthWriteMask = DepthWriteMask.All,
				DepthComparison = Comparison.LessEqual,
				

				IsStencilEnabled = true,
				StencilReadMask = byte.MaxValue,
				StencilWriteMask = byte.MaxValue,

				FrontFace = new DepthStencilOperationDescription
				{
					Comparison = Comparison.Less,
					FailOperation = StencilOperation.Keep,
					DepthFailOperation = StencilOperation.Keep,
					PassOperation = StencilOperation.Keep,
				},

				BackFace = new DepthStencilOperationDescription
				{
					Comparison = Comparison.Less,
					FailOperation = StencilOperation.Keep,
					DepthFailOperation = StencilOperation.Keep,
					PassOperation = StencilOperation.Keep,
				},

			});
		}

		public override void InitDrawingData(Device device)
		{
			var points = new RawVector4[]
			{
				new RawVector4(-0.60f, -0.5f, 0.6f, 1f),
				new RawVector4(-0.10f, +0.8f, 0.6f, 1f),
				new RawVector4(+0.40f, -0.5f, 0.6f, 1f),
			
				new RawVector4(-0.2f, -0.5f, 0.7f, 1f),
				new RawVector4(+0.3f, +0.8f, 0.7f, 1f),
				new RawVector4(+0.8f, -0.5f, 0.7f, 1f),
			};

			RawVector3[] colors = Helper.CreateTriangleColors(points.Length);

			pointsCount = points.Length;

			Buffer pointsBuffer = Buffer.Create(device, BindFlags.VertexBuffer, points);
			positionsBufferBinding = new VertexBufferBinding(pointsBuffer, 16, 0);

			Buffer colorsBuffer = Buffer.Create(device, BindFlags.VertexBuffer, colors);
			colorsBufferBinding = new VertexBufferBinding(colorsBuffer, 12, 0);
		}

		public override void Dispose()
		{
			vertexShader.Dispose();
			pixelShader.Dispose();
			pos4Col3Layout.Dispose();
			positionsBufferBinding.Buffer.Dispose();
			colorsBufferBinding.Buffer.Dispose();

			writeMask.Dispose();
			applyMask.Dispose();
		}

		public override void Render(DeviceContext context)
		{
			// IA.
			context.InputAssembler.InputLayout = pos4Col3Layout;
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			// Data.
			context.InputAssembler.SetVertexBuffers(0, positionsBufferBinding);
			context.InputAssembler.SetVertexBuffers(1, colorsBufferBinding);
			// Draw.
			context.VertexShader.Set(vertexShader);
			context.PixelShader.Set(pixelShader);

			context.OutputMerger.DepthStencilState = writeMask;
			context.Draw(3, 0);
			
			context.OutputMerger.DepthStencilState = applyMask;
			context.Draw(3, 3);
		}
	}
}