using System;
using System.Collections.Generic;
using System.Linq;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Unity.Entities;
using UnityEngine;

namespace Game.Prefabs;

[Serializable]
public abstract class ComponentBase : ScriptableObject, IComponentBase, IComparable
{
	protected static ILog baseLog;

	public bool active = true;

	public virtual bool ignoreUnlockDependencies => false;

	public PrefabBase prefab { get; set; }

	public virtual IEnumerable<string> modTags => Enumerable.Empty<string>();

	public virtual string GetDebugString()
	{
		return ((object)this).GetType().Name;
	}

	protected virtual void OnEnable()
	{
		baseLog = LogManager.GetLogger("SceneFlow");
	}

	protected virtual void OnDisable()
	{
	}

	public T GetComponent<T>() where T : ComponentBase
	{
		if ((Object)(object)prefab == (Object)null)
		{
			throw new NullReferenceException($"GetComponent<{typeof(T)}>() -> prefab is null");
		}
		if (prefab.TryGet<T>(out var component))
		{
			return component;
		}
		return null;
	}

	public ComponentBase GetComponentExactly(Type type)
	{
		if ((Object)(object)prefab == (Object)null)
		{
			throw new NullReferenceException($"GetComponentExactly<{type}>() -> prefab is null");
		}
		if (prefab.TryGetExactly(type, out var component))
		{
			return component;
		}
		return null;
	}

	public bool GetComponents<T>(List<T> list) where T : ComponentBase
	{
		Type typeFromHandle = typeof(T);
		if ((Object)(object)prefab == (Object)null)
		{
			throw new NullReferenceException($"GetComponents<{typeFromHandle}>() -> prefab is null");
		}
		return prefab.TryGet(list);
	}

	public virtual void GetDependencies(List<PrefabBase> prefabs)
	{
	}

	public virtual void Initialize(EntityManager entityManager, Entity entity)
	{
	}

	public virtual void LateInitialize(EntityManager entityManager, Entity entity)
	{
	}

	public abstract void GetPrefabComponents(HashSet<ComponentType> components);

	public abstract void GetArchetypeComponents(HashSet<ComponentType> components);

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		ComponentBase componentBase = obj as ComponentBase;
		if ((Object)(object)componentBase != (Object)null)
		{
			return ((Object)this).name.CompareTo(((Object)componentBase).name);
		}
		throw new ArgumentException("Object is not a ComponentBase");
	}
}
