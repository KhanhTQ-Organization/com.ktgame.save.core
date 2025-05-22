using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public interface ISerializationProvider
	{
		byte[] Serialize<TData>(TData data);

		UniTask<byte[]> SerializeAsync<TData>(TData data);

		TData Deserialize<TData>(byte[] data);

		object Deserialize(byte[] data, System.Type dataType);

		UniTask<TData> DeserializeAsync<TData>(byte[] data);

		UniTask<object> DeserializeAsync(byte[] data, System.Type dataType);
	}
}
