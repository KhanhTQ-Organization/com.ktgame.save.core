using System;

namespace com.ktgame.save.core
{
	public interface IGameData
	{
		IObscuredModel GetDataModel(Type dataModelType);

		TDataModel GetDataModel<TDataModel>() where TDataModel : IObscuredModel;

		bool TryGetDataModel(Type dataModelType, out IObscuredModel dataModel);

		bool TryGetDataModel<TDataModel>(out TDataModel saveModel) where TDataModel : IObscuredModel;
	}
}
