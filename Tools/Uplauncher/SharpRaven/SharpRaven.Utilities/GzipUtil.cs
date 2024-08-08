using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace SharpRaven.Utilities
{
	internal class GzipUtil
	{
		public static void Write(string json, Stream stream)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
			{
				gZipStream.Write(bytes, 0, bytes.Length);
			}
		}

		public static async Task WriteAsync(string json, Stream stream)
		{
			byte[] data = Encoding.UTF8.GetBytes(json);
			using (GZipStream gzip = new GZipStream(stream, CompressionMode.Compress))
			{
				await gzip.WriteAsync(data, 0, data.Length);
			}
		}
	}
}
