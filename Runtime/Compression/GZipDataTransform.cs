using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public class GZipDataTransform : IDataTransform
	{
		public byte[] Apply(byte[] data)
		{
			return WriteGZip(data);
		}

		public UniTask<byte[]> ApplyAsync(byte[] data)
		{
			return WriteGZipAsync(data).AsUniTask();
		}

		public byte[] Reverse(byte[] data)
		{
			return ReadGZip(data);
		}

		public UniTask<byte[]> ReverseAsync(byte[] data)
		{
			return ReadGZipAsync(data).AsUniTask();
		}

		private static byte[] WriteGZip(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			using var dataStream = new MemoryStream();
			using (var compressionStream = new GZipStream(dataStream, CompressionMode.Compress))
			{
				compressionStream.Write(data, 0, data.Length);
			}

			return dataStream.ToArray();
		}

		private static Task<byte[]> WriteGZipAsync(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var dataStream = new MemoryStream();
			var compressionStream = new GZipStream(dataStream, CompressionMode.Compress);

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

		private static byte[] ReadGZip(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			using var dataStream = new MemoryStream(data);
			using var compressionStream = new GZipStream(dataStream, CompressionMode.Decompress);
			using var decompressedStream = new MemoryStream();
			compressionStream.CopyTo(decompressedStream);

			return decompressedStream.ToArray();
		}

		private static Task<byte[]> ReadGZipAsync(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			var dataStream = new MemoryStream(data);
			var compressionStream = new GZipStream(dataStream, CompressionMode.Decompress);
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
