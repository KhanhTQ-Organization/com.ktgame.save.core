using System.Security.Cryptography;
using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public class AesEncryptionDataTransform : IDataTransform
	{
		private readonly byte[] _key;
		private readonly byte[] _initializationVector;

		public AesEncryptionDataTransform(string secretKey)
		{
			var aes = Aes.Create();
			_key = System.Convert.FromBase64String(secretKey);
			_initializationVector = new byte[aes.IV.Length];
		}

		public byte[] Apply(byte[] data)
		{
			return AesEncryption.Encrypt(data, _key, _initializationVector);
		}

		public UniTask<byte[]> ApplyAsync(byte[] data)
		{
			return AesEncryption.EncryptAsync(data, _key, _initializationVector).AsUniTask();
		}

		public byte[] Reverse(byte[] data)
		{
			return AesEncryption.Decrypt(data, _key, _initializationVector);
		}

		public UniTask<byte[]> ReverseAsync(byte[] data)
		{
			return AesEncryption.DecryptAsync(data, _key, _initializationVector).AsUniTask();
		}
	}
}
