using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public interface IGameLoader
	{
		bool Load(System.Type saveModelType);

		bool Load<TSaveModel>() where TSaveModel : ISaveModel, new();

		UniTask<bool> LoadAsync(System.Type saveModelType);

		UniTask<bool> LoadAsync<TDataModel>() where TDataModel : ISaveModel, new();

		bool LoadFromRawData(string rawData);
	}
}
