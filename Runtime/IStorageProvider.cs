using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public interface IStorageProvider
	{
		bool Exists(string key);

		void Save<TData>(string key, TData data);

		UniTask SaveAsync<TData>(string key, TData data);

		TData Load<TData>(string key);

		object Load(string key, System.Type dataType);

		UniTask<TData> LoadAsync<TData>(string key);

		UniTask<object> LoadAsync(string key, System.Type dataType);

		void Copy(string fromKey, string toKey);

		bool Delete(string key);

		void DeleteAll();
	}
}
