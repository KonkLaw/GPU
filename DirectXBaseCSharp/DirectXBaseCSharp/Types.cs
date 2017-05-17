using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DirectXBaseCSharp
{
    public class DisposableContainer : IDisposable
    {
        private readonly IDisposable[] disposableData;

        public DisposableContainer(params IDisposable[] disposableObjects)
        {
            disposableData = disposableObjects;
        }

        public void Dispose()
        {
            foreach (var disposable in disposableData)
            {
                disposable.Dispose();
            }
        }
    }

    public static class Helper
    {
        public static byte[] GetShaderByteCode(string shaderName)
        {
            Assembly assembley = Assembly.GetExecutingAssembly();
            using (Stream manifestResourceStream = assembley.GetManifestResourceStream(
                    $"{assembley.GetName().Name}.CompiledShaders.{shaderName}.cso"))
            {
                if (manifestResourceStream != null)
                    return new BinaryReader(manifestResourceStream).ReadBytes((int)manifestResourceStream.Length);
                throw new ArgumentException($"{shaderName} shader not founded.");
            }
        }

        internal static List<RawVector4> CreatePoints()
        {
            //return new List<RawVector4>()
            //{
            //	new RawVector4(0.0f, 0.5f, 0.5f, 1.0f),
            //	new RawVector4(0.5f, -0.5f, 0.5f, 1.0f),
            //	new RawVector4(-0.5f, -0.5f, 0.5f, 1.0f),
            //	new RawVector4(0.0f, 0.25f, 0.75f, 1.0f),
            //	new RawVector4(0.25f, -0.25f, 0.25f, 1.0f),
            //	new RawVector4(-0.25f, -0.25f, 0.25f, 1.0f),
            //};

            var result = new List<RawVector4>();

            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.90f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.85f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.80f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.75f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.70f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.65f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.60f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.55f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.50f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.45f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.40f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.35f);
            AddTraingles(result, -1.5f, 1.5f, 300, -1.5f, 1.5f, 300, 0.30f);

            return result;
        }

        private static void AddTraingles(List<RawVector4> data,
            float xBegin, float xEnd, int countX,
            float yBegin, float yEnd, int countY,
            float z)
        {
            float stepX = (xEnd - xBegin) / countX;
            float stepY = (yEnd - yBegin) / countY;

            for (int j = 0; j < countY; j++)
            {
                float currY = yBegin + j * stepY + stepY / 2;
                for (int i = 0; i < countX; i++)
                {
                    float currX = xBegin + i * stepX + stepX / 2;
                    data.Add(new RawVector4(currX - stepX / 2, currY - stepY / 2, z, 1.0f));
                    data.Add(new RawVector4(currX, currY + stepY / 2, z, 1.0f));
                    data.Add(new RawVector4(currX + stepX / 2, currY - stepY / 2, z, 1.0f));
                }
            }
        }

        internal static RawVector3[] CreateTriangleColors(int length)
        {
            var colors = new RawVector3[length];
            var randomHelper = new RandomFloatHelper();
            for (int i = 0; i < colors.Length;)
            {
                float randR = randomHelper.GetNext(0f, 1f);
                float randG = randomHelper.GetNext(0f, 1f);
                float randB = randomHelper.GetNext(0f, 1f);
                var color = new RawVector3(randR, randG, randB);
                colors[i++] = color;
                colors[i++] = color;
                colors[i++] = color;
            }
            return colors;
        }

		internal static RawVector3[] CreateColors(int length)
		{
			var colors = new RawVector3[length];
			var randomHelper = new RandomFloatHelper();
			for (int i = 0; i < colors.Length;)
			{
				float randR = randomHelper.GetNext(0f, 1f);
				float randG = randomHelper.GetNext(0f, 1f);
				float randB = randomHelper.GetNext(0f, 1f);
				var color = new RawVector3(randR, randG, randB);
				colors[i++] = color;
			}
			return colors;
		}
	}

    class RandomFloatHelper
    {
        private readonly byte[] randomBytes = new byte[sizeof(float)];
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public float GetNext()
        {
            Random.NextBytes(randomBytes);
            return BitConverter.ToSingle(randomBytes, 0);
        }

        public float GetNext(float begin, float end) => Convert.ToSingle(begin + Random.NextDouble() * (end - begin));

	    public RawVector4 GenInCube(float beginX, float endX, float beginY, float endY)
			=> new RawVector4(GetNext(beginX, endX), GetNext(beginY, endY), GetNext(0, 1), 1);

	    public RawVector4[] GenInCube(float beginX, float endX, float beginY, float endY, int count)
	    {
			var result = new RawVector4[count];
		    for (int i = 0; i < result.Length; i++)
		    {
			    result[i] = GenInCube(beginX, endX, beginY, endY);
		    }
		    return result;
	    }
    }
}
