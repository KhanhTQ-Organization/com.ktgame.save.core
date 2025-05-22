using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.ktgame.save.core
{
	public abstract class BaseGameSave : IGameSave
	{
		[ShowInInspector, InlineProperty] private readonly Dictionary<string, ISaveModel> _models = new();

		public IReadOnlyCollection<string> Keys => _models.Keys;

		public IReadOnlyCollection<ISaveModel> Models => _models.Values;

		public void AddSaveModel<TSaveModel>() where TSaveModel : ISaveModel, new()
		{
			var type = typeof(TSaveModel);
			if (_models.ContainsKey(type.Name))
			{
				Debug.LogError($"Save model has already been added {type.Name}");
				return;
			}

			_models.Add(type.Name, new TSaveModel());
		}

		public void AddSaveModel(ISaveModel saveModel)
		{
			var type = saveModel.GetType();
			if (_models.ContainsKey(type.Name))
			{
				Debug.LogError($"Save model has already been added {type.Name}");
				return;
			}

			_models.Add(type.Name, saveModel);
		}

		public void RemoveSaveModel<TSaveModel>() where TSaveModel : ISaveModel
		{
			var type = typeof(TSaveModel);
			if (_models.ContainsKey(type.Name))
			{
				_models.Remove(type.Name);
			}
		}

		public void RemoveSaveModel(Type saveModelType)
		{
			if (_models.ContainsKey(saveModelType.Name))
			{
				_models.Remove(saveModelType.Name);
			}
		}

		public void RemoveSaveModel(string saveModelName)
		{
			if (_models.ContainsKey(saveModelName))
			{
				_models.Remove(saveModelName);
			}
		}

		public ISaveModel GetSaveModel(Type saveModelType)
		{
			return _models.TryGetValue(saveModelType.Name, out var model) ? model : default;
		}

		public TSaveModel GetSaveModel<TSaveModel>() where TSaveModel : ISaveModel
		{
			var type = typeof(TSaveModel);
			return _models.TryGetValue(type.Name, out var model) ? (TSaveModel)model : default;
		}

		public bool TryGetSaveModel(Type saveModelType, out ISaveModel saveModel)
		{
			var result = _models.TryGetValue(saveModelType.Name, out var value);
			saveModel = result ? value : default;
			return result;
		}

		public bool TryGetSaveModel<TSaveModel>(out TSaveModel saveModel) where TSaveModel : ISaveModel
		{
			var type = typeof(TSaveModel);
			var result = _models.TryGetValue(type.Name, out var value);
			saveModel = result ? (TSaveModel)value : default;
			return result;
		}

		public void Clear()
		{
			_models.Clear();
		}
	}
}
