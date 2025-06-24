using System;
using Colossal.Entities;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace Game.UI;

public readonly struct UIObjectInfo : IComparable<UIObjectInfo>
{
	public Entity entity { get; }

	public PrefabData prefabData { get; }

	public int priority { get; }

	public UIObjectInfo(Entity entity, int priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		this.entity = entity;
		prefabData = default(PrefabData);
		this.priority = priority;
	}

	public UIObjectInfo(Entity entity, PrefabData prefabData, int priority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		this.entity = entity;
		this.prefabData = prefabData;
		this.priority = priority;
	}

	public int CompareTo(UIObjectInfo other)
	{
		return priority.CompareTo(other.priority);
	}

	public static NativeList<UIObjectInfo> GetSortedObjects(EntityQuery query, Allocator allocator)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref query)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			NativeArray<PrefabData> val2 = ((EntityQuery)(ref query)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)2));
			try
			{
				NativeArray<UIObjectData> val3 = ((EntityQuery)(ref query)).ToComponentDataArray<UIObjectData>(AllocatorHandle.op_Implicit((Allocator)2));
				try
				{
					int length = val.Length;
					NativeList<UIObjectInfo> val4 = new NativeList<UIObjectInfo>(length, AllocatorHandle.op_Implicit(allocator));
					for (int i = 0; i < length; i++)
					{
						UIObjectInfo uIObjectInfo = new UIObjectInfo(val[i], val2[i], val3[i].m_Priority);
						val4.Add(ref uIObjectInfo);
					}
					NativeSortExtension.Sort<UIObjectInfo>(val4);
					return val4;
				}
				finally
				{
					((IDisposable)val3/*cast due to .constrained prefix*/).Dispose();
				}
			}
			finally
			{
				((IDisposable)val2/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			((IDisposable)val/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public static NativeList<UIObjectInfo> GetSortedObjects(EntityManager entityManager, EntityQuery query, Allocator allocator)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref query)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			NativeArray<PrefabData> val2 = ((EntityQuery)(ref query)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)2));
			try
			{
				int length = val.Length;
				NativeList<UIObjectInfo> val3 = new NativeList<UIObjectInfo>(length, AllocatorHandle.op_Implicit(allocator));
				UIObjectData uIObjectData = default(UIObjectData);
				for (int i = 0; i < length; i++)
				{
					int num = (EntitiesExtensions.TryGetComponent<UIObjectData>(entityManager, val[i], ref uIObjectData) ? uIObjectData.m_Priority : 0);
					UIObjectInfo uIObjectInfo = new UIObjectInfo(val[i], val2[i], num);
					val3.Add(ref uIObjectInfo);
				}
				NativeSortExtension.Sort<UIObjectInfo>(val3);
				return val3;
			}
			finally
			{
				((IDisposable)val2/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			((IDisposable)val/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public static NativeList<UIObjectInfo> GetSortedObjects(EntityManager entityManager, NativeList<Entity> entities, Allocator allocator)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		int length = entities.Length;
		NativeList<UIObjectInfo> val = default(NativeList<UIObjectInfo>);
		val._002Ector(length, AllocatorHandle.op_Implicit(allocator));
		UIObjectData uIObjectData = default(UIObjectData);
		for (int i = 0; i < length; i++)
		{
			Entity val2 = entities[i];
			int num = (EntitiesExtensions.TryGetComponent<UIObjectData>(entityManager, val2, ref uIObjectData) ? uIObjectData.m_Priority : 0);
			UIObjectInfo uIObjectInfo = new UIObjectInfo(val2, ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(val2), num);
			val.Add(ref uIObjectInfo);
		}
		NativeSortExtension.Sort<UIObjectInfo>(val);
		return val;
	}

	public static NativeList<UIObjectInfo> GetObjects(EntityManager entityManager, DynamicBuffer<UIGroupElement> elements, Allocator allocator)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		NativeList<UIObjectInfo> result = default(NativeList<UIObjectInfo>);
		result._002Ector(elements.Length, AllocatorHandle.op_Implicit(allocator));
		UIObjectData uIObjectData = default(UIObjectData);
		for (int i = 0; i < elements.Length; i++)
		{
			Entity prefab = elements[i].m_Prefab;
			int num = (EntitiesExtensions.TryGetComponent<UIObjectData>(entityManager, prefab, ref uIObjectData) ? uIObjectData.m_Priority : 0);
			UIObjectInfo uIObjectInfo = new UIObjectInfo(prefab, ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(prefab), num);
			result.Add(ref uIObjectInfo);
		}
		return result;
	}

	public static NativeList<UIObjectInfo> GetSortedObjects(EntityManager entityManager, DynamicBuffer<UIGroupElement> elements, Allocator allocator)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		NativeList<UIObjectInfo> val = default(NativeList<UIObjectInfo>);
		val._002Ector(elements.Length, AllocatorHandle.op_Implicit(allocator));
		UIObjectData uIObjectData = default(UIObjectData);
		for (int i = 0; i < elements.Length; i++)
		{
			Entity prefab = elements[i].m_Prefab;
			int num = (EntitiesExtensions.TryGetComponent<UIObjectData>(entityManager, prefab, ref uIObjectData) ? uIObjectData.m_Priority : 0);
			UIObjectInfo uIObjectInfo = new UIObjectInfo(prefab, ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(prefab), num);
			val.Add(ref uIObjectInfo);
		}
		NativeSortExtension.Sort<UIObjectInfo>(val);
		return val;
	}
}
