using System.Collections.Generic;
using UnityEngine;

namespace com.ktgame.save.core
{
	public abstract class BaseLoadStrategy<TSaveModel, TDataModel> : ILoadStrategy<TSaveModel, TDataModel>
		where TSaveModel : ISaveModel
		where TDataModel : IObscuredModel
	{
		private readonly Dictionary<int, IVersionConverter<TSaveModel, TSaveModel>> _converters = new();

		protected void AddConverter(IVersionConverter<TSaveModel, TSaveModel> converter)
		{
			_converters.Add(converter.FromVersion, converter);
		}

		public void Load(ISaveModel saveModel, IObscuredModel dataModel, bool firstLoad)
		{
			if (saveModel is TSaveModel typedSaveModel && dataModel is TDataModel typedDataModel)
			{
				Load(typedSaveModel, typedDataModel, firstLoad);
			}
			else
			{
				Debug.LogError("Type mismatch: Cannot cast ISaveModel or IObscuredModel to the expected types.");
			}
		}

		public void Load(TSaveModel saveModel, TDataModel dataModel, bool firstLoad)
		{
			switch (firstLoad)
			{
				case true:
					OnFirstLoad(dataModel);
					break;
				case false:
					var rawSaveModel = saveModel;
					var currentVersion = rawSaveModel.Version;
					var targetVersion = dataModel.Version;
					if (currentVersion < targetVersion)
					{
						while (currentVersion < targetVersion)
						{
							if (_converters.TryGetValue(currentVersion, out var converter))
							{
								rawSaveModel = converter.Convert(rawSaveModel);
								currentVersion = converter.ToVersion;
							}
							else
							{
								Debug.LogError($"Missing save version converter from version {currentVersion} to {targetVersion}. Save data might be corrupted or incompatible!");
								break;
							}
						}
					}

					OnLoad(rawSaveModel, dataModel);
					break;
			}
		}

		protected abstract void OnFirstLoad(TDataModel dataModel);

		protected abstract void OnLoad(TSaveModel saveModel, TDataModel dataModel);
	}
}
