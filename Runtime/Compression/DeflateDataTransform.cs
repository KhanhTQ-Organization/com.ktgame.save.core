using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public class DeflateDataTransform : IDataTransform
	{
		public byte[] Apply(byte[] data)
		{
			return WriteDeflate(data);
		}

		public UniTask<byte[]> ApplyAsync(byte[] data)
		{
			return WriteDeflateAsync(data).AsUniTask();
		}

		public byte[] Reverse(byte[] data)
		{
			return ReadDeflate(data);
		}

		public UniTask<byte[]> ReverseAsync(byte[] data)
		{
			return ReadDeflateAsync(data).AsUniTask();
		}

		private static byte[] WriteDeflate(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			using var dataStream = new MemoryStream();
			using (var compressionStream = new DeflateStream(dataStream, CompressionMode.Compress))
			{
				compressionStream.Write(data, 0, data.Length);
			}

			return dataStream.ToArray();
		}

		private static Task<byte[]> WriteDeflateAsync(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var dataStream = new MemoryStream();
			var compressionStream = new DeflateStream(dataStream, CompressionMode.Compress);

			return compressionStream.WriteAsync(data, 0, data.Length)
				.ContinueWith(task =>
				{
					using (dataStream)
					{
						compressionStream.Dispose();
						return dataStream.ToArray();
					}
				});
		}

		private static byte[] ReadDeflate(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			using var dataStream = new MemoryStream(data);
			using var compressionStream = new DeflateStream(dataStream, CompressionMode.Decompress);
			using var decompressedStream = new MemoryStream();
			compressionStream.CopyTo(decompressedStream);

			return decompressedStream.ToArray();
		}

		private static Task<byte[]> ReadDeflateAsync(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var dataStream = new MemoryStream(data);
			var compressionStream = new DeflateStream(dataStream, CompressionMode.Decompress);
			var decompressedStream = new MemoryStream();

			return compressionStream.CopyToAsync(decompressedStream)
				.ContinueWith(task =>
				{
					using (dataStream)
					using (compressionStream)
					using (decompressedStream)
					{
						return decompressedStream.ToArray();
					}
				});
		}
	}
}
