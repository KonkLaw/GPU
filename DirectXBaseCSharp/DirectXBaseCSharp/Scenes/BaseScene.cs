using System.Drawing;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;

namespace DirectXBaseCSharp.Scenes
{
	abstract class PreDrawHandler
	{
		public abstract void PreDraw(DeviceContext context, DepthStencilView depthStencilView);
	}

	// TODO: Base class is faster that interfaces. (?)
	abstract class BaseScene
	{
		public readonly PreDrawHandler PreDrawHandler;

		protected BaseScene(PreDrawHandler preDrawHandler = null)
		{
			PreDrawHandler = preDrawHandler;
		}

		public abstract void Dispose();

		public abstract void InitShadersAndLayout(Device device);

		public abstract void InitDrawingData(Device device);

		public abstract void Render(DeviceContext context);


		// TODO: Better interface for custom setup.
		public virtual bool TryGetCustomDepthStencilView(Device device, SampleDescription sampleDescription, Size size, out DepthStencilView depthStencilView)
		{
			depthStencilView = default;
			return false;
		}
	}
}