using System;
using Cysharp.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;

namespace com.ktgame.save.core
{
	public class NewtonsoftJsonSerializationProvider : ISerializationProvider
	{
		private readonly JsonSerializerSettings _serializerSettings;

		public NewtonsoftJsonSerializationProvider(JsonSerializerSettings serializerSettings)
		{
			_serializerSettings = serializerSettings;
		}

		public byte[] Serialize<TData>(TData data)
		{
			return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, _serializerSettings));
		}

		public UniTask<byte[]> SerializeAsync<TData>(TData data)
		{
			return UniTask.FromResult(Serialize(data));
		}

		public TData Deserialize<TData>(byte[] data)
		{
			return JsonConvert.DeserializeObject<TData>(Encoding.UTF8.GetString(data));
		}

		public object Deserialize(byte[] data, Type dataType)
		{
			return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(data), dataType);
		}

		public UniTask<TData> DeserializeAsync<TData>(byte[] data)
		{
			return UniTask.FromResult(Deserialize<TData>(data));
		}

		public UniTask<object> DeserializeAsync(byte[] data, Type dataType)
		{
			return UniTask.FromResult(Deserialize(data, dataType));
		}
	}
}
