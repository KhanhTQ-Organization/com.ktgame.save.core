using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace com.ktgame.save.core
{
	public abstract class BaseGameData : IGameData
	{
		[ShowInInspector, InlineProperty] private readonly Dictionary<string, IObscuredModel> _models = new();

		public IReadOnlyCollection<string> Keys => _models.Keys;

		public IReadOnlyCollection<IObscuredModel> Models => _models.Values;

		public void AddLoadModel<TObscuredModel>() where TObscuredModel : IObscuredModel, new()
		{
			var type = typeof(TObscuredModel);
			if (_models.ContainsKey(type.Name))
			{
				Debug.LogError($"Obscured model has already been added {type.Name}");
				return;
			}

			_models.Add(type.Name, new TObscuredModel());
		}

		public void AddLoadModel(IObscuredModel obscuredModel)
		{
			var type = obscuredModel.GetType();
			if (_models.ContainsKey(type.Name))
			{
				Debug.LogError($"Obscured model has already been added {type.Name}");
				return;
			}

			_models.Add(type.Name, obscuredModel);
		}

		public void RemoveDataModel<TObscuredModel>() where TObscuredModel : IObscuredModel
		{
			var type = typeof(TObscuredModel);
			if (_models.ContainsKey(type.Name))
			{
				_models.Remove(type.Name);
			}
		}

		public void RemoveDataModel(Type dataModelType)
		{
			if (_models.ContainsKey(dataModelType.Name))
			{
				_models.Remove(dataModelType.Name);
			}
		}

		public void RemoveDataModel(string dataModelName)
		{
			if (_models.ContainsKey(dataModelName))
			{
				_models.Remove(dataModelName);
			}
		}

		public IObscuredModel GetDataModel(Type dataModelType)
		{
			var result = _models.TryGetValue(dataModelType.Name, out var value);
			return result ? value : null;
		}

		public TDataModel GetDataModel<TDataModel>() where TDataModel : IObscuredModel
		{
			var type = typeof(TDataModel);
			return _models.TryGetValue(type.Name, out var model) ? (TDataModel)model : default;
		}

		public bool TryGetDataModel(Type dataModelType, out IObscuredModel dataModel)
		{
			var result = _models.TryGetValue(dataModelType.Name, out var value);
			dataModel = result ? value : null;
			return result;
		}

		public bool TryGetDataModel<TDataModel>(out TDataModel dataModel) where TDataModel : IObscuredModel
		{
			var type = typeof(TDataModel);
			var result = _models.TryGetValue(type.Name, out var value);
			dataModel = result ? (TDataModel)value : default;
			return result;
		}

		public void Clear()
		{
			_models.Clear();
		}
	}
}
