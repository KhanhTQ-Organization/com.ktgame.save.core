using System;
using Cysharp.Threading.Tasks;

namespace com.ktgame.save.core
{
	public interface IGameSaver
	{
		bool Save(Type saveModelType);

		bool Save<TSaveModel>() where TSaveModel : class, ISaveModel;

		UniTask<bool> SaveAsync(Type saveModelType);

		UniTask<bool> SaveAsync<TSaveModel>() where TSaveModel : class, ISaveModel;

		void SaveAll();

		UniTask SaveAllAsync();

		string GetRawData();
	}
}
