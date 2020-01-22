using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;

namespace DirectXBaseCSharp.Scenes
{
	class DraftScene : BaseScene
	{
		private VertexShader vertexShader;
		private PixelShader pixelShader;
		private InputLayout pos4Col3Layout;

		public override void InitShadersAndLayout(Device device)
		{
			byte[] signature = Helper.GetShaderByteCode("VertexShader");
			vertexShader = new VertexShader(device, signature);
			pixelShader = new PixelShader(device, Helper.GetShaderByteCode("PixelShader"));
			pos4Col3Layout = new InputLayout(device, signature, new[]
			{
				new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
				new InputElement("COLOR", 0, Format.R32G32B32_Float, 0, 1),
				new InputElement("TEXTCOORD", 0, Format.R32G32_Float, 0, 2),
			});
		}

		public override void InitDrawingData(Device device)
		{
			//RawVector4[] points = new RandomFloatHelper().GenInCube(-1f, 1f, -1f, 1f, 1000000);

			//RawVector4[] points = Helper.CreatePoints().ToArray();

			RawVector4[] points = new RawVector4[]
			{
				new RawVector4(-0.5f, -0.8f, 0.6f, 1f),
				new RawVector4(+0.00f, +0.8f, 0.6f, 1f),
				new RawVector4(+0.5f, -0.5f, 0.6f, 1f),

				new RawVector4(-0.6f, -0.5f, 0.7f, 1f),
				new RawVector4(+0.0f, +0.8f, 0.7f, 1f),
				new RawVector4(+0.6f, -0.5f, 0.7f, 1f),
			};

			RawVector3[] colors = Helper.CreateTriangleColors(points.Length);
			//RawVector3[] colors = Helper.CreateColors(points.Length);

			//RawVector2[] textCoord = new []
			//{
			//	new RawVector2(0.0f, 0.0f),
			//	new RawVector2(1.0f, 0.0f),
			//	new RawVector2(0.0f, 1.0f),
			//
			//	new RawVector2(0.0f, 0.0f),
			//	new RawVector2(1.0f, 0.0f),
			//	new RawVector2(0.0f, 1.0f),
			//};

			var pointsCount = points.Length;

			Buffer pointsBuffer = Buffer.Create(device, BindFlags.VertexBuffer, points);
			var positionsBufferBinding = new VertexBufferBinding(pointsBuffer, 16, 0);

			Buffer colorsBuffer = Buffer.Create(device, BindFlags.VertexBuffer, colors);
			var colorsBufferBinding = new VertexBufferBinding(colorsBuffer, 12, 0);

			//Buffer textCoordBuffer = Buffer.Create(device, BindFlags.VertexBuffer, textCoord);
			//textCoordBufferBinding = new VertexBufferBinding(textCoordBuffer, 8, 0);
			//var texture2D = new Texture2D(device, new Texture2DDescription
			//{
			//    ArraySize = 1,
			//    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
			//    CpuAccessFlags = CpuAccessFlags.None,
			//    Format = Format.B8G8R8A8_UNorm,
			//    Width = 400,
			//    Height = 400,
			//    MipLevels = 1,
			//    SampleDescription = new SampleDescription(1, 0),
			//    Usage = ResourceUsage.Default,
			//});
			//shaderResource = new ShaderResourceView(device, texture2D);
		}

		public override void Dispose()
		{
			vertexShader.Dispose();
			pixelShader.Dispose();
			pos4Col3Layout.Dispose();
			//positionsBufferBinding.Buffer.Dispose();
			//colorsBufferBinding.Buffer.Dispose();
			//colorsBufferBinding.Buffer.Dispose();
			//textCoordBufferBinding.Buffer.Dispose();
			//shaderResource.Dispose();
		}

		public override void Render(DeviceContext context)
		{
			// IA.
			context.InputAssembler.InputLayout = pos4Col3Layout;
			context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
			//context.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
			// Data.
			//context.InputAssembler.SetVertexBuffers(0, positionsBufferBinding);
			//context.InputAssembler.SetVertexBuffers(1, colorsBufferBinding);
			//context.InputAssembler.SetVertexBuffers(2, textCoordBufferBinding);
			//context.PixelShader.SetShaderResource(0, shaderResource);
			// Draw.
			context.VertexShader.Set(vertexShader);
			context.PixelShader.Set(pixelShader);
			//context.Draw(pointsCount, 0);
		}
	}
}