using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Common;
using Game.Notifications;
using Game.Prefabs;
using Game.Tools;
using Game.UI.Localization;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class TempNotificationTooltipSystem : TooltipSystemBase
{
	private struct ItemInfo : IComparable<ItemInfo>
	{
		public Entity m_Prefab;

		public IconPriority m_Priority;

		public int CompareTo(ItemInfo other)
		{
			return -m_Priority.CompareTo(other.m_Priority);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Icon> __Game_Notifications_Icon_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Notifications_Icon_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Icon>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_TempQuery;

	private NativeParallelHashMap<Entity, IconPriority> m_Priorities;

	private NativeList<ItemInfo> m_Items;

	private List<StringTooltip> m_Tooltips;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<Icon>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>()
		});
		m_Priorities = new NativeParallelHashMap<Entity, IconPriority>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_Items = new NativeList<ItemInfo>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_Tooltips = new List<StringTooltip>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Priorities.Dispose();
		m_Items.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		((SystemBase)this).CompleteDependency();
		m_Priorities.Clear();
		m_Items.Clear();
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_TempQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			ComponentTypeHandle<Temp> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Owner> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<Icon> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Icon>(ref __TypeHandle.__Game_Notifications_Icon_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<PrefabRef> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			Enumerator<ArchetypeChunk> enumerator = val.GetEnumerator();
			try
			{
				Temp temp2 = default(Temp);
				IconPriority iconPriority = default(IconPriority);
				while (enumerator.MoveNext())
				{
					ArchetypeChunk current = enumerator.Current;
					NativeArray<Temp> nativeArray = ((ArchetypeChunk)(ref current)).GetNativeArray<Temp>(ref componentTypeHandle);
					NativeArray<Icon> nativeArray2 = ((ArchetypeChunk)(ref current)).GetNativeArray<Icon>(ref componentTypeHandle3);
					NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref current)).GetNativeArray<Owner>(ref componentTypeHandle2);
					NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref current)).GetNativeArray<PrefabRef>(ref componentTypeHandle4);
					for (int i = 0; i < nativeArray.Length; i++)
					{
						Temp temp = nativeArray[i];
						Icon icon = nativeArray2[i];
						PrefabRef prefabRef = nativeArray4[i];
						if (icon.m_ClusterLayer == IconClusterLayer.Marker)
						{
							continue;
						}
						if (nativeArray3.Length != 0)
						{
							Owner owner = nativeArray3[i];
							if (EntitiesExtensions.TryGetComponent<Temp>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref temp2) && HasIcon(temp2.m_Original, prefabRef.m_Prefab, icon.m_Priority))
							{
								continue;
							}
						}
						if ((temp.m_Flags & (TempFlags.Dragging | TempFlags.Select)) == TempFlags.Select)
						{
							continue;
						}
						if (m_Priorities.TryGetValue(prefabRef.m_Prefab, ref iconPriority))
						{
							if ((int)icon.m_Priority > (int)iconPriority)
							{
								m_Priorities[prefabRef.m_Prefab] = icon.m_Priority;
							}
						}
						else
						{
							m_Priorities.TryAdd(prefabRef.m_Prefab, icon.m_Priority);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			val.Dispose();
		}
		Enumerator<Entity, IconPriority> enumerator2 = m_Priorities.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				KeyValue<Entity, IconPriority> current2 = enumerator2.Current;
				ref NativeList<ItemInfo> reference = ref m_Items;
				ItemInfo itemInfo = new ItemInfo
				{
					m_Prefab = current2.Key,
					m_Priority = current2.Value
				};
				reference.Add(ref itemInfo);
			}
		}
		finally
		{
			((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
		}
		NativeSortExtension.Sort<ItemInfo>(m_Items);
		for (int j = 0; j < m_Items.Length; j++)
		{
			ItemInfo itemInfo2 = m_Items[j];
			NotificationIconPrefab prefab = m_PrefabSystem.GetPrefab<NotificationIconPrefab>(itemInfo2.m_Prefab);
			if (m_Tooltips.Count <= j)
			{
				m_Tooltips.Add(new StringTooltip
				{
					path = $"notification{j}"
				});
			}
			StringTooltip stringTooltip = m_Tooltips[j];
			if (prefab.TryGet<UIObject>(out var component) && !string.IsNullOrEmpty(component.m_Icon))
			{
				stringTooltip.icon = component.m_Icon;
			}
			else
			{
				stringTooltip.icon = null;
			}
			stringTooltip.value = LocalizedString.Id("Notifications.TITLE[" + ((Object)prefab).name + "]");
			stringTooltip.color = NotificationTooltip.GetColor(itemInfo2.m_Priority);
			AddMouseTooltip(stringTooltip);
		}
	}

	private bool HasIcon(Entity entity, Entity prefab, IconPriority minPriority)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<IconElement> val = default(DynamicBuffer<IconElement>);
		if (EntitiesExtensions.TryGetBuffer<IconElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val))
		{
			for (int i = 0; i < val.Length; i++)
			{
				Entity icon = val[i].m_Icon;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				Icon componentData = ((EntityManager)(ref entityManager)).GetComponentData<Icon>(icon);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(icon).m_Prefab == prefab && (int)componentData.m_Priority >= (int)minPriority)
				{
					return true;
				}
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public TempNotificationTooltipSystem()
	{
	}
}
