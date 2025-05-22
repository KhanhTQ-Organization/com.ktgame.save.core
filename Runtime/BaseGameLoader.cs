using System;
using System.Collections.Generic;
using com.ktgame.json.mini;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace com.ktgame.save.core
{
	public abstract class BaseGameLoader<TGameData> : IGameLoader where TGameData : IGameData
	{
		private readonly TGameData _gameData;
		private readonly IStorageProvider _storageProvider;
		private readonly Dictionary<Type, ILoadStrategy> _loadStrategies;
		private readonly Dictionary<Type, Type> _dataModelTypes;

		public IReadOnlyCollection<Type> Keys => _loadStrategies.Keys;

		public IReadOnlyCollection<ILoadStrategy> Strategies => _loadStrategies.Values;

		protected BaseGameLoader(TGameData gameData, IStorageProvider storageProvider)
		{
			_gameData = gameData;
			_storageProvider = storageProvider;
			_loadStrategies = new Dictionary<Type, ILoadStrategy>();
			_dataModelTypes = new Dictionary<Type, Type>();
		}

		public void AddLoadStrategy<TSaveModel, TDataModel>(ILoadStrategy<TSaveModel, TDataModel> loadStrategy)
			where TSaveModel : ISaveModel
			where TDataModel : IObscuredModel
		{
			if (_loadStrategies.ContainsKey(typeof(TSaveModel)))
			{
				Debug.LogError($"Load strategy has already been added {loadStrategy.GetType().Name}");
				return;
			}

			_dataModelTypes.Add(typeof(TSaveModel), typeof(TDataModel));
			_loadStrategies.Add(typeof(TSaveModel), loadStrategy);
		}

		public bool Load(Type saveModelType)
		{
			var fileName = saveModelType.Name;
			try
			{
				if (_loadStrategies.TryGetValue(saveModelType, out var loadStrategy))
				{
					bool firstLoad;
					ISaveModel saveModel;

					if (_storageProvider.Exists(fileName))
					{
						firstLoad = false;
						saveModel = (ISaveModel)_storageProvider.Load(fileName, saveModelType);
					}
					else
					{
						firstLoad = true;
						saveModel = (ISaveModel)Activator.CreateInstance(saveModelType);
					}

					var dataModel = _gameData.GetDataModel(_dataModelTypes[saveModelType]);
					loadStrategy.Load(saveModel, dataModel, firstLoad);
					return true;
				}
				else
				{
					Debug.LogError($"Save strategy for {saveModelType.Name} not found");
					return false;
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Load save file {fileName} failed: {e.Message}");
				return false;
			}
		}

		public bool Load<TSaveModel>() where TSaveModel : ISaveModel, new()
		{
			return Load(typeof(TSaveModel));
		}

		public async UniTask<bool> LoadAsync(Type saveModelType)
		{
			var fileName = saveModelType.Name;
			try
			{
				if (_loadStrategies.TryGetValue(saveModelType, out var loadStrategy))
				{
					bool firstLoad;
					ISaveModel saveModel;

					if (_storageProvider.Exists(fileName))
					{
						firstLoad = false;
						saveModel = (ISaveModel)await _storageProvider.LoadAsync(fileName, saveModelType);
					}
					else
					{
						firstLoad = true;
						saveModel = (ISaveModel)Activator.CreateInstance(saveModelType);
					}

					var dataModel = _gameData.GetDataModel(_dataModelTypes[saveModelType]);
					loadStrategy.Load(saveModel, dataModel, firstLoad);
					return true;
				}
				else
				{
					Debug.LogError($"Load strategy for {saveModelType.Name} not found");
					return false;
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Load save file {fileName} failed: {e.Message}");
				return false;
			}
		}

		public async UniTask<bool> LoadAsync<TSaveModel>() where TSaveModel : ISaveModel, new()
		{
			return await LoadAsync(typeof(TSaveModel));
		}

		public void LoadAll()
		{
			foreach (var key in _loadStrategies.Keys)
			{
				Load(key);
			}
		}

		public async UniTask LoadAllAsync()
		{
			foreach (var key in _loadStrategies.Keys)
			{
				await LoadAsync(key);
			}
		}

		public bool LoadFromRawData(string rawData)
		{
			if (string.IsNullOrEmpty(rawData))
			{
				Debug.LogError("Raw data is null");
				return false;
			}

			if (MiniJson.JsonDecode(rawData) is Dictionary<string, object> data)
			{
				foreach ((var key, var value) in data)
				{
					foreach ((var type, var strategy) in _loadStrategies)
					{
						if (type.Name.Equals(key))
						{
							var dataModel = _gameData.GetDataModel(_dataModelTypes[type]);
							var saveModel = (ISaveModel)JsonConvert.DeserializeObject((string)value, type);
							strategy.Load(saveModel, dataModel, false);
						}
					}
				}

				return true;
			}

			return false;
		}

		public void Clear()
		{
			_loadStrategies.Clear();
			_dataModelTypes.Clear();
		}
	}
}
