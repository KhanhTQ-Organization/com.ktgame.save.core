using System;
using System.Collections.Generic;
using com.ktgame.json.mini;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.ktgame.save.core
{
	public abstract class BaseGameSaver<TGameData> : IGameSaver where TGameData : IGameData
	{
		private readonly TGameData _gameData;
		private readonly IStorageProvider _storageProvider;
		private readonly Dictionary<Type, ISaveStrategy> _saveStrategies;
		private readonly Dictionary<Type, Type> _dataModelTypes;

		public IReadOnlyCollection<Type> Keys => _saveStrategies.Keys;

		public IReadOnlyCollection<ISaveStrategy> Strategies => _saveStrategies.Values;

		protected BaseGameSaver(TGameData gameData, IStorageProvider storageProvider)
		{
			_gameData = gameData;
			_storageProvider = storageProvider;
			_saveStrategies = new Dictionary<Type, ISaveStrategy>();
			_dataModelTypes = new Dictionary<Type, Type>();
		}

		public void AddSaveStrategy<TSaveModel, TDataModel>(ISaveStrategy<TSaveModel, TDataModel> saveStrategy)
			where TSaveModel : ISaveModel
			where TDataModel : IObscuredModel
		{
			if (_saveStrategies.ContainsKey(typeof(TSaveModel)))
			{
				Debug.LogError($"Save strategy has already been added {saveStrategy.GetType().Name}");
				return;
			}

			_dataModelTypes.Add(typeof(TSaveModel), typeof(TDataModel));
			_saveStrategies.Add(typeof(TSaveModel), saveStrategy);
		}

		public bool TryGetSaveStrategy(Type saveModelType, out ISaveStrategy saveStrategy)
		{
			return _saveStrategies.TryGetValue(saveModelType, out saveStrategy);
		}

		public bool Save(Type saveModelType)
		{
			var fileName = saveModelType.Name;
			var backupFilename = fileName + "-backup";
			var hasPrevSave = _storageProvider.Exists(fileName);

			try
			{
				if (_saveStrategies.TryGetValue(saveModelType, out var saveStrategy))
				{
					if (hasPrevSave)
					{
						if (_storageProvider.Exists(backupFilename))
						{
							_storageProvider.Delete(backupFilename);
						}

						_storageProvider.Copy(fileName, backupFilename);
					}

					var dataModel = _gameData.GetDataModel(_dataModelTypes[saveModelType]);
					var saveModel = saveStrategy.Save(dataModel);
					saveModel.Version = dataModel.Version;

					_storageProvider.Save(fileName, saveModel);

					if (_storageProvider.Exists(backupFilename))
					{
						_storageProvider.Delete(backupFilename);
					}

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
				Debug.LogError($"Save file {fileName} failed: {e.Message}");

				_storageProvider.Delete(fileName);

				if (_storageProvider.Exists(backupFilename))
				{
					_storageProvider.Copy(backupFilename, fileName);
					_storageProvider.Delete(backupFilename);
				}

				return false;
			}
		}

		public bool Save<TSaveModel>() where TSaveModel : class, ISaveModel
		{
			return Save(typeof(TSaveModel));
		}

		public async UniTask<bool> SaveAsync(Type saveModelType)
		{
			var fileName = saveModelType.Name;
			var backupFilename = fileName + "-backup";
			var hasPrevSave = _storageProvider.Exists(fileName);

			try
			{
				if (_saveStrategies.TryGetValue(saveModelType, out var saveStrategy))
				{
					if (hasPrevSave)
					{
						if (_storageProvider.Exists(backupFilename))
						{
							_storageProvider.Delete(backupFilename);
						}

						_storageProvider.Copy(fileName, backupFilename);
					}

					var dataModel = _gameData.GetDataModel(_dataModelTypes[saveModelType]);
					var saveModel = saveStrategy.Save(dataModel);
					await _storageProvider.SaveAsync(fileName, saveModel);

					if (_storageProvider.Exists(backupFilename))
					{
						_storageProvider.Delete(backupFilename);
					}

					return true;
				}
				else
				{
					Debug.LogError($"Save strategy for {saveModelType.Name} not found");

					_storageProvider.Delete(fileName);

					if (_storageProvider.Exists(backupFilename))
					{
						_storageProvider.Copy(backupFilename, fileName);
						_storageProvider.Delete(backupFilename);
					}

					return false;
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"Save file {fileName} failed: {e.Message}");
				return false;
			}
		}

		public async UniTask<bool> SaveAsync<TSaveModel>() where TSaveModel : class, ISaveModel
		{
			return await SaveAsync(typeof(TSaveModel));
		}

		public void SaveAll()
		{
			foreach (var saveStrategy in _saveStrategies)
			{
				Save(saveStrategy.Key);
			}
		}

		public async UniTask SaveAllAsync()
		{
			foreach (var saveStrategy in _saveStrategies)
			{
				await SaveAsync(saveStrategy.Key);
			}
		}

		public string GetRawData()
		{
			var data = new Dictionary<string, object>();
			foreach ((var saveModelType, var dataModelType) in _dataModelTypes)
			{
				if (_saveStrategies.TryGetValue(saveModelType, out var saveStrategy))
				{
					var dataModel = _gameData.GetDataModel(dataModelType);
					var saveModel = saveStrategy.Save(dataModel);
					data.Add(saveModelType.Name, JsonUtility.ToJson(saveModel));
				}
			}

			return MiniJson.JsonEncode(data);
		}

		public void Clear()
		{
			_saveStrategies.Clear();
			_dataModelTypes.Clear();
		}
	}
}
