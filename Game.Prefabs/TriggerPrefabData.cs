using System;
using System.Runtime.CompilerServices;
using Game.Triggers;
using Unity.Collections;
using Unity.Entities;

namespace Game.Prefabs;

public struct TriggerPrefabData : IDisposable
{
	public struct PrefabKey : IEquatable<PrefabKey>
	{
		public TriggerType m_TriggerType;

		public Entity m_TriggerEntity;

		public bool Equals(PrefabKey other)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (m_TriggerType == other.m_TriggerType)
			{
				return m_TriggerEntity == other.m_TriggerEntity;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)m_TriggerType * 31 + ((object)System.Runtime.CompilerServices.Unsafe.As<Entity, Entity>(ref m_TriggerEntity)/*cast due to .constrained prefix*/).GetHashCode();
		}
	}

	public struct PrefabValue
	{
		public TargetType m_TargetTypes;

		public Entity m_Prefab;
	}

	public struct Iterator
	{
		public NativeParallelMultiHashMapIterator<PrefabKey> m_Iterator;
	}

	private NativeParallelMultiHashMap<PrefabKey, PrefabValue> m_PrefabMap;

	public TriggerPrefabData(Allocator allocator)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabMap = new NativeParallelMultiHashMap<PrefabKey, PrefabValue>(100, AllocatorHandle.op_Implicit(allocator));
	}

	public void Dispose()
	{
		m_PrefabMap.Dispose();
	}

	public void AddPrefab(Entity prefab, TriggerData triggerData)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		PrefabKey prefabKey = new PrefabKey
		{
			m_TriggerType = triggerData.m_TriggerType,
			m_TriggerEntity = triggerData.m_TriggerPrefab
		};
		PrefabValue prefabValue = new PrefabValue
		{
			m_TargetTypes = triggerData.m_TargetTypes,
			m_Prefab = prefab
		};
		m_PrefabMap.Add(prefabKey, prefabValue);
	}

	public void RemovePrefab(Entity prefab, TriggerData triggerData)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		PrefabKey prefabKey = new PrefabKey
		{
			m_TriggerType = triggerData.m_TriggerType,
			m_TriggerEntity = triggerData.m_TriggerPrefab
		};
		PrefabValue prefabValue = default(PrefabValue);
		NativeParallelMultiHashMapIterator<PrefabKey> val = default(NativeParallelMultiHashMapIterator<PrefabKey>);
		if (!m_PrefabMap.TryGetFirstValue(prefabKey, ref prefabValue, ref val))
		{
			return;
		}
		do
		{
			if (prefabValue.m_TargetTypes == triggerData.m_TargetTypes && prefabValue.m_Prefab == prefab)
			{
				m_PrefabMap.Remove(val);
				break;
			}
		}
		while (m_PrefabMap.TryGetNextValue(ref prefabValue, ref val));
	}

	public bool HasAnyPrefabs(TriggerType triggerType, Entity triggerPrefab)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		PrefabKey prefabKey = new PrefabKey
		{
			m_TriggerType = triggerType,
			m_TriggerEntity = triggerPrefab
		};
		PrefabValue prefabValue = default(PrefabValue);
		NativeParallelMultiHashMapIterator<PrefabKey> val = default(NativeParallelMultiHashMapIterator<PrefabKey>);
		return m_PrefabMap.TryGetFirstValue(prefabKey, ref prefabValue, ref val);
	}

	public bool TryGetFirstPrefab(TriggerType triggerType, TargetType targetType, Entity triggerPrefab, out Entity prefab, out Iterator iterator)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		PrefabKey prefabKey = new PrefabKey
		{
			m_TriggerType = triggerType,
			m_TriggerEntity = triggerPrefab
		};
		PrefabValue prefabValue = default(PrefabValue);
		if (m_PrefabMap.TryGetFirstValue(prefabKey, ref prefabValue, ref iterator.m_Iterator))
		{
			do
			{
				if (targetType == TargetType.Nothing || (prefabValue.m_TargetTypes & targetType) != TargetType.Nothing)
				{
					prefab = prefabValue.m_Prefab;
					return true;
				}
			}
			while (m_PrefabMap.TryGetNextValue(ref prefabValue, ref iterator.m_Iterator));
		}
		prefab = Entity.Null;
		return false;
	}

	public bool TryGetNextPrefab(TriggerType triggerType, TargetType targetType, Entity triggerPrefab, out Entity prefab, ref Iterator iterator)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		PrefabKey prefabKey = new PrefabKey
		{
			m_TriggerType = triggerType,
			m_TriggerEntity = triggerPrefab
		};
		PrefabValue prefabValue = default(PrefabValue);
		while (m_PrefabMap.TryGetNextValue(ref prefabValue, ref iterator.m_Iterator))
		{
			if (targetType == TargetType.Nothing || (prefabValue.m_TargetTypes & targetType) != TargetType.Nothing)
			{
				prefab = prefabValue.m_Prefab;
				return true;
			}
		}
		prefab = Entity.Null;
		return false;
	}
}
