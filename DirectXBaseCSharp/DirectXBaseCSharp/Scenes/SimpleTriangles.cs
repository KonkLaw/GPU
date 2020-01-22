using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;

namespace DirectXBaseCSharp.Scenes
{
	class SimpleTriangles : BaseScene
	{
		private VertexShader vertexShader;
		private PixelShader pixelShader;
		private InputLayout pos4Col3Layout;

		private VertexBufferBinding positionsBufferBinding;
		private VertexBufferBinding colorsBufferBinding;
		private int pointsCount;

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
		}

		public override void InitDrawingData(Device device)
		{
			var points = new RawVector4[]
			{
				new RawVector4(-0.5f, -0.8f, 0.6f, 1f),
				new RawVector4(+0.00f, +0.8f, 0.6f, 1f),
				new RawVector4(+0.5f, -0.5f, 0.6f, 1f),
			
				new RawVector4(-0.6f, -0.5f, 0.7f, 1f),
				new RawVector4(+0.0f, +0.8f, 0.7f, 1f),
				new RawVector4(+0.6f, -0.5f, 0.7f, 1f),
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
			context.Draw(pointsCount, 0);
		}
	}
}