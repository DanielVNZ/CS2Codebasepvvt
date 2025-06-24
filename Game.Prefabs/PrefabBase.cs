using System;
using System.Collections.Generic;
using Colossal.IO.AssetDatabase;
using Colossal.Json;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Prefabs;

public abstract class PrefabBase : ComponentBase, ISerializationCallbackReceiver, IPrefabBase
{
	public List<ComponentBase> components = new List<ComponentBase>();

	[NonSerialized]
	public bool isDirty = true;

	public string thumbnailUrl { get; private set; }

	public bool builtin
	{
		get
		{
			string text = default(string);
			if (!AssetDatabase.global.resources.prefabsMap.TryGetGuid((Object)(object)this, ref text))
			{
				PrefabAsset obj = asset;
				return ((obj != null) ? ((AssetData)obj).database : null) is AssetDatabase<Game>;
			}
			return true;
		}
	}

	public PrefabAsset asset { get; set; }

	public virtual bool canIgnoreUnlockDependencies => true;

	public virtual string uiTag => GetPrefabID().ToString();

	public void OnBeforeSerialize()
	{
	}

	public virtual void OnAfterDeserialize()
	{
		base.prefab = this;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		base.prefab = this;
		foreach (ComponentBase component in components)
		{
			if ((Object)(object)component != (Object)null || (Object)(object)component.prefab == (Object)(object)component)
			{
				component.prefab = this;
				continue;
			}
			if ((Object)(object)component == (Object)null)
			{
				ComponentBase.baseLog.ErrorFormat((Object)(object)base.prefab, "Null component on prefab: {0}", (object)((Object)base.prefab).name);
			}
			if ((Object)(object)component.prefab != (Object)(object)component)
			{
				ComponentBase.baseLog.ErrorFormat((Object)(object)base.prefab, "Component on prefab {0} is referenced from another prefab prefab: {1}", (object)((Object)base.prefab).name, (object)((Object)component.prefab).name);
			}
		}
		components.RemoveAll((ComponentBase x) => (Object)(object)x == (Object)null);
		thumbnailUrl = "thumbnail://ThumbnailCamera/" + Uri.EscapeDataString(((object)this).GetType().Name) + "/" + Uri.EscapeDataString(((Object)this).name);
	}

	public virtual void Reset()
	{
		isDirty = true;
	}

	public T AddOrGetComponent<T>() where T : ComponentBase
	{
		if (!TryGetExactly<T>(out var component))
		{
			return AddComponent<T>();
		}
		return component;
	}

	public ComponentBase AddOrGetComponent(Type type)
	{
		if (!TryGetExactly(type, out var component))
		{
			return AddComponent(type);
		}
		return component;
	}

	public T AddComponent<T>() where T : ComponentBase
	{
		return (T)AddComponent(typeof(T));
	}

	public T AddComponentFrom<T>(T from) where T : ComponentBase
	{
		T val = AddOrGetComponent<T>();
		JsonUtility.FromJsonOverwrite(JsonUtility.ToJson((object)from), (object)val);
		return val;
	}

	public ComponentBase AddComponentFrom(ComponentBase from)
	{
		Type type = ((object)from).GetType();
		ComponentBase componentBase = AddOrGetComponent(type);
		JsonUtility.FromJsonOverwrite(JsonUtility.ToJson((object)from), (object)componentBase);
		return componentBase;
	}

	public ComponentBase AddComponent(Type type)
	{
		if (Has(type))
		{
			throw new InvalidOperationException("Component already exists");
		}
		ComponentBase componentBase = (ComponentBase)(object)ScriptableObject.CreateInstance(type);
		((Object)componentBase).name = type.Name;
		componentBase.prefab = this;
		components.Add(componentBase);
		isDirty = true;
		return componentBase;
	}

	public ComponentBase ReplaceComponentWith(ComponentBase target, Type type)
	{
		ComponentBase componentBase = (ComponentBase)(object)ScriptableObject.CreateInstance(type);
		componentBase.prefab = this;
		int index = components.IndexOf(target);
		components[index] = componentBase;
		isDirty = true;
		return componentBase;
	}

	public void Remove<T>() where T : ComponentBase
	{
		Remove(typeof(T));
	}

	public void Remove(Type type)
	{
		int num = -1;
		for (int i = 0; i < components.Count; i++)
		{
			if (((object)components[i]).GetType() == type)
			{
				num = i;
				break;
			}
		}
		if (num >= 0)
		{
			components.RemoveAt(num);
			isDirty = true;
		}
	}

	public bool Has<T>() where T : ComponentBase
	{
		return Has(typeof(T));
	}

	public bool Has(Type type)
	{
		if (((object)this).GetType() == type)
		{
			return true;
		}
		foreach (ComponentBase component in components)
		{
			if (((object)component).GetType() == type)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasSubclassOf(Type type)
	{
		if (((object)this).GetType().IsSubclassOf(type))
		{
			return true;
		}
		foreach (ComponentBase component in components)
		{
			if (((object)component).GetType().IsSubclassOf(type))
			{
				return true;
			}
		}
		return false;
	}

	public bool TryGet<T>(out T component) where T : ComponentBase
	{
		ComponentBase component2;
		bool result = TryGet(typeof(T), out component2);
		component = (T)component2;
		return result;
	}

	public bool TryGet(Type type, out ComponentBase component)
	{
		Type type2 = ((object)this).GetType();
		component = null;
		if (type2 == type || type2.IsSubclassOf(type))
		{
			component = this;
			return true;
		}
		foreach (ComponentBase component2 in components)
		{
			Type type3 = ((object)component2).GetType();
			if (type3 == type || type3.IsSubclassOf(type))
			{
				component = component2;
				return true;
			}
		}
		return false;
	}

	public bool TryGetExactly<T>(out T component) where T : ComponentBase
	{
		ComponentBase component2;
		bool result = TryGetExactly(typeof(T), out component2);
		component = (T)component2;
		return result;
	}

	public bool TryGetExactly(Type type, out ComponentBase component)
	{
		component = null;
		if (((object)this).GetType() == type)
		{
			component = this;
			return true;
		}
		foreach (ComponentBase component2 in components)
		{
			if (((object)component2).GetType() == type)
			{
				component = component2;
				return true;
			}
		}
		return false;
	}

	public bool TryGet<T>(List<T> result)
	{
		Assert.IsNotNull<List<ComponentBase>>(components);
		int count = result.Count;
		if (this is T item)
		{
			result.Add(item);
		}
		foreach (ComponentBase component in components)
		{
			if (component.active && component is T item2)
			{
				result.Add(item2);
			}
		}
		return count != result.Count;
	}

	public override void GetPrefabComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PrefabData>());
		components.Add(ComponentType.ReadWrite<LoadedIndex>());
	}

	public override void GetArchetypeComponents(HashSet<ComponentType> components)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		components.Add(ComponentType.ReadWrite<PrefabRef>());
	}

	public PrefabID GetPrefabID()
	{
		return new PrefabID(this);
	}

	public PrefabBase Clone(string newName = null)
	{
		PrefabBase prefabBase = (PrefabBase)(object)ScriptableObject.CreateInstance(((object)this).GetType());
		Variant obj = JSON.Load(JsonUtility.ToJson((object)this));
		ProxyObject val = (ProxyObject)(object)((obj is ProxyObject) ? obj : null);
		if (val != null)
		{
			val.Remove("components");
			val.Remove("m_NameOverride");
		}
		JsonUtility.FromJsonOverwrite(((Variant)val).ToJSON(), (object)prefabBase);
		((Object)prefabBase).name = newName ?? (((Object)this).name + " (copy)");
		foreach (ComponentBase component in components)
		{
			prefabBase.AddComponentFrom(component);
		}
		return prefabBase;
	}
}
