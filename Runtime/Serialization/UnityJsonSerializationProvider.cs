using System;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.ktgame.save.core
{
	public class UnityJsonSerializationProvider : ISerializationProvider
	{
		private readonly bool _prettyPrint;

		public UnityJsonSerializationProvider() : this(false) { }

		public UnityJsonSerializationProvider(bool prettyPrint)
		{
			_prettyPrint = prettyPrint;
		}

		public byte[] Serialize<TData>(TData data)
		{
			return StringToBytes(JsonUtility.ToJson(data, _prettyPrint));
		}

		public UniTask<byte[]> SerializeAsync<TData>(TData data)
		{
			return UniTask.FromResult(Serialize(data));
		}

		public TData Deserialize<TData>(byte[] data)
		{
			return JsonUtility.FromJson<TData>(BytesToString(data));
		}

		public object Deserialize(byte[] data, Type dataType)
		{
			return JsonUtility.FromJson(BytesToString(data), dataType);
		}

		public UniTask<TData> DeserializeAsync<TData>(byte[] data)
		{
			return UniTask.FromResult(Deserialize<TData>(data));
		}

		public UniTask<object> DeserializeAsync(byte[] data, Type dataType)
		{
			return UniTask.FromResult(Deserialize(data, dataType));
		}

		private static byte[] StringToBytes(string str)
		{
			return Encoding.UTF8.GetBytes(str);
		}

		private static string BytesToString(byte[] bytes)
		{
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
