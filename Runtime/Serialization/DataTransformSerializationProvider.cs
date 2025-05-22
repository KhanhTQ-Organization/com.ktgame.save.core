using System;
using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public class DataTransformSerializationProvider : ISerializationProvider
	{
		private readonly ISerializationProvider _provider;
		private readonly IDataTransform _dataTransform;

		public DataTransformSerializationProvider(ISerializationProvider baseProvider, IDataTransform dataTransform)
		{
			_provider = baseProvider ?? throw new ArgumentNullException(nameof(baseProvider));
			_dataTransform = dataTransform ?? throw new ArgumentNullException(nameof(dataTransform));
		}

		public byte[] Serialize<TData>(TData data)
		{
			return _dataTransform.Apply(_provider.Serialize(data));
		}

		public UniTask<byte[]> SerializeAsync<TData>(TData data)
		{
			return _provider.SerializeAsync(data).ContinueWith(bytes => _dataTransform.ApplyAsync(bytes));
		}

		public TData Deserialize<TData>(byte[] data)
		{
			return _provider.Deserialize<TData>(_dataTransform.Reverse(data));
		}

		public object Deserialize(byte[] data, Type dataType)
		{
			return _provider.Deserialize(_dataTransform.Reverse(data), dataType);
		}

		public UniTask<TData> DeserializeAsync<TData>(byte[] data)
		{
			return _dataTransform.ReverseAsync(data).ContinueWith(bytes => _provider.DeserializeAsync<TData>(bytes));
		}

		public UniTask<object> DeserializeAsync(byte[] data, Type dataType)
		{
			return _dataTransform.ReverseAsync(data).ContinueWith(bytes => _provider.DeserializeAsync(bytes, dataType));
		}
	}
}
