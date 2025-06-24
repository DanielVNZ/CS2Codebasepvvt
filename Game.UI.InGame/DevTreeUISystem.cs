using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class DevTreeUISystem : UISystemBase
{
	private struct DevTreeNodeInfo
	{
		public Entity entity;

		public PrefabData prefabData;

		public DevTreeNodeData devTreeNodeData;

		public bool locked;
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<DevTreeNodeData> __Game_Prefabs_DevTreeNodeData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Locked> __Game_Prefabs_Locked_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_DevTreeNodeData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DevTreeNodeData>(true);
			__Game_Prefabs_Locked_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Locked>(true);
		}
	}

	private const string kGroup = "devTree";

	private PrefabSystem m_PrefabSystem;

	private PrefabUISystem m_PrefabUISystem;

	private DevTreeSystem m_DevTreeSystem;

	private ImageSystem m_ImageSystem;

	private EntityQuery m_DevTreePointsQuery;

	private EntityQuery m_DevTreeNodeQuery;

	private EntityQuery m_UnlockedServiceQuery;

	private EntityQuery m_ModifiedDevTreeNodeQuery;

	private EntityQuery m_LockedDevTreeNodeQuery;

	private GetterValueBinding<int> m_PointsBinding;

	private RawValueBinding m_ServicesBinding;

	private RawMapBinding<Entity> m_ServiceDetailsBinding;

	private RawMapBinding<Entity> m_NodesBinding;

	private RawMapBinding<Entity> m_NodeDetailsBinding;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Expected O, but got Unknown
		//IL_0232: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_DevTreeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DevTreeSystem>();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		m_DevTreePointsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DevTreePoints>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<DevTreeNodeData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_DevTreeNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_UnlockedServiceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<DevTreeNodeData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Updated>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Temp>() };
		array2[0] = val;
		m_ModifiedDevTreeNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<DevTreeNodeData>(),
			ComponentType.ReadOnly<Locked>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array3[0] = val;
		m_LockedDevTreeNodeQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array3);
		AddBinding((IBinding)(object)(m_PointsBinding = new GetterValueBinding<int>("devTree", "points", (Func<int>)GetDevTreePoints, (IWriter<int>)null, (EqualityComparer<int>)null)));
		RawValueBinding val2 = new RawValueBinding("devTree", "services", (Action<IJsonWriter>)BindServices);
		RawValueBinding binding = val2;
		m_ServicesBinding = val2;
		AddBinding((IBinding)(object)binding);
		AddBinding((IBinding)(object)(m_ServiceDetailsBinding = new RawMapBinding<Entity>("devTree", "serviceDetails", (Action<IJsonWriter, Entity>)BindServiceDetails, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_NodesBinding = new RawMapBinding<Entity>("devTree", "nodes", (Action<IJsonWriter, Entity>)BindNodes, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)(m_NodeDetailsBinding = new RawMapBinding<Entity>("devTree", "nodeDetails", (Action<IJsonWriter, Entity>)BindNodeDetails, (IReader<Entity>)null, (IWriter<Entity>)null)));
		AddBinding((IBinding)(object)new TriggerBinding<Entity>("devTree", "purchaseNode", (Action<Entity>)PurchaseNode, (IReader<Entity>)null));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		m_PointsBinding.Update();
		if (!((EntityQuery)(ref m_ModifiedDevTreeNodeQuery)).IsEmptyIgnoreFilter || PrefabUtils.HasUnlockedPrefab<DevTreeNodeData>(((ComponentSystemBase)this).EntityManager, m_UnlockedServiceQuery))
		{
			((MapBindingBase<Entity>)(object)m_NodesBinding).UpdateAll();
			((MapBindingBase<Entity>)(object)m_NodeDetailsBinding).UpdateAll();
		}
		if (PrefabUtils.HasUnlockedPrefab<ServiceData>(((ComponentSystemBase)this).EntityManager, m_UnlockedServiceQuery))
		{
			m_ServicesBinding.Update();
			((MapBindingBase<Entity>)(object)m_ServiceDetailsBinding).UpdateAll();
		}
	}

	private void PurchaseNode(Entity node)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_DevTreeSystem.Purchase(node);
	}

	private int GetDevTreePoints()
	{
		return Mathf.Min((!((EntityQuery)(ref m_DevTreePointsQuery)).IsEmptyIgnoreFilter) ? ((EntityQuery)(ref m_DevTreePointsQuery)).GetSingleton<DevTreePoints>().m_Points : 0, GetMaxDevTreePoints());
	}

	private int GetMaxDevTreePoints()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<DevTreeNodeData> val = ((EntityQuery)(ref m_LockedDevTreeNodeQuery)).ToComponentDataArray<DevTreeNodeData>(AllocatorHandle.op_Implicit((Allocator)2));
		int num = 0;
		Enumerator<DevTreeNodeData> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				num += enumerator.Current.m_Cost;
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
		return num;
	}

	private void BindServices(IJsonWriter writer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		NativeList<UIObjectInfo> sortedDevTreeServices = GetSortedDevTreeServices((Allocator)3);
		JsonWriterExtensions.ArrayBegin(writer, sortedDevTreeServices.Length);
		for (int i = 0; i < sortedDevTreeServices.Length; i++)
		{
			UIObjectInfo uIObjectInfo = sortedDevTreeServices[i];
			ServicePrefab prefab = m_PrefabSystem.GetPrefab<ServicePrefab>(uIObjectInfo.prefabData);
			writer.TypeBegin("devTree.Service");
			writer.PropertyName("entity");
			UnityWriters.Write(writer, uIObjectInfo.entity);
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("icon");
			writer.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
			writer.PropertyName("locked");
			writer.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, uIObjectInfo.entity));
			writer.PropertyName("uiTag");
			writer.Write(prefab.uiTag);
			writer.PropertyName("requirements");
			m_PrefabUISystem.BindPrefabRequirements(writer, uIObjectInfo.entity);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
		sortedDevTreeServices.Dispose();
	}

	private NativeList<UIObjectInfo> GetSortedDevTreeServices(Allocator allocator)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<DevTreeNodeData> val = ((EntityQuery)(ref m_DevTreeNodeQuery)).ToComponentDataArray<DevTreeNodeData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashSet<Entity> val2 = default(NativeParallelHashSet<Entity>);
		val2._002Ector(16, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<UIObjectInfo> val3 = default(NativeList<UIObjectInfo>);
		val3._002Ector(16, AllocatorHandle.op_Implicit(allocator));
		UIObjectData uIObjectData = default(UIObjectData);
		for (int i = 0; i < val.Length; i++)
		{
			Entity service = val[i].m_Service;
			if (val2.Add(service) && EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, service, ref uIObjectData))
			{
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabData>(service);
				UIObjectInfo uIObjectInfo = new UIObjectInfo(service, componentData, uIObjectData.m_Priority);
				val3.Add(ref uIObjectInfo);
			}
		}
		val.Dispose();
		val2.Dispose();
		NativeSortExtension.Sort<UIObjectInfo>(val3);
		return val3;
	}

	private void BindServiceDetails(IJsonWriter binder, Entity service)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (service != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			PrefabData prefabData = default(PrefabData);
			if (((EntityManager)(ref entityManager)).HasComponent<ServiceData>(service) && EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, service, ref prefabData))
			{
				ServicePrefab prefab = m_PrefabSystem.GetPrefab<ServicePrefab>(prefabData);
				binder.TypeBegin("devTree.ServiceDetails");
				binder.PropertyName("entity");
				UnityWriters.Write(binder, service);
				binder.PropertyName("name");
				binder.Write(((Object)prefab).name);
				binder.PropertyName("icon");
				binder.Write(ImageSystem.GetIcon(prefab) ?? m_ImageSystem.placeholderIcon);
				binder.PropertyName("locked");
				binder.Write(EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, service));
				binder.PropertyName("milestoneRequirement");
				binder.Write(ProgressionUtils.GetRequiredMilestone(((ComponentSystemBase)this).EntityManager, service));
				binder.TypeEnd();
				return;
			}
		}
		binder.WriteNull();
	}

	private void BindNodes(IJsonWriter binder, Entity service)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (service != Entity.Null)
		{
			NativeList<DevTreeNodeInfo> devTreeNodes = GetDevTreeNodes(service, (Allocator)3);
			JsonWriterExtensions.ArrayBegin(binder, devTreeNodes.Length);
			DynamicBuffer<DevTreeNodeRequirement> val = default(DynamicBuffer<DevTreeNodeRequirement>);
			for (int i = 0; i < devTreeNodes.Length; i++)
			{
				DevTreeNodeInfo devTreeNodeInfo = devTreeNodes[i];
				DevTreeNodePrefab prefab = m_PrefabSystem.GetPrefab<DevTreeNodePrefab>(devTreeNodeInfo.prefabData);
				binder.TypeBegin("devTree.Node");
				binder.PropertyName("entity");
				UnityWriters.Write(binder, devTreeNodeInfo.entity);
				binder.PropertyName("name");
				binder.Write(((Object)prefab).name);
				binder.PropertyName("icon");
				binder.Write(GetDevTreeIcon(prefab));
				binder.PropertyName("cost");
				binder.Write(devTreeNodeInfo.devTreeNodeData.m_Cost);
				binder.PropertyName("locked");
				binder.Write(devTreeNodeInfo.locked);
				binder.PropertyName("position");
				MathematicsWriters.Write(binder, new float2((float)prefab.m_HorizontalPosition, prefab.m_VerticalPosition));
				if (EntitiesExtensions.TryGetBuffer<DevTreeNodeRequirement>(((ComponentSystemBase)this).EntityManager, devTreeNodeInfo.entity, true, ref val))
				{
					bool flag = val.Length > 0;
					binder.PropertyName("requirements");
					JsonWriterExtensions.ArrayBegin(binder, val.Length);
					for (int j = 0; j < val.Length; j++)
					{
						UnityWriters.Write(binder, val[j].m_Node);
						if (EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, val[j].m_Node))
						{
							flag = false;
						}
					}
					binder.ArrayEnd();
					binder.PropertyName("unlockable");
					binder.Write(flag);
				}
				else
				{
					binder.PropertyName("requirements");
					JsonWriterExtensions.WriteEmptyArray(binder);
					binder.PropertyName("unlockable");
					binder.Write(!devTreeNodeInfo.locked);
				}
				binder.TypeEnd();
			}
			binder.ArrayEnd();
			devTreeNodes.Dispose();
		}
		else
		{
			JsonWriterExtensions.WriteEmptyArray(binder);
		}
	}

	private NativeList<DevTreeNodeInfo> GetDevTreeNodes(Entity service, Allocator allocator)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		NativeList<DevTreeNodeInfo> result = default(NativeList<DevTreeNodeInfo>);
		result._002Ector(16, AllocatorHandle.op_Implicit(allocator));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<DevTreeNodeData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<DevTreeNodeData>(ref __TypeHandle.__Game_Prefabs_DevTreeNodeData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Locked> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_DevTreeNodeQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
				NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
				NativeArray<DevTreeNodeData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<DevTreeNodeData>(ref componentTypeHandle2);
				EnabledMask enabledMask = ((ArchetypeChunk)(ref val2)).GetEnabledMask<Locked>(ref componentTypeHandle3);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					if (nativeArray3[j].m_Service == service)
					{
						DevTreeNodeInfo devTreeNodeInfo = new DevTreeNodeInfo
						{
							entity = nativeArray[j],
							prefabData = nativeArray2[j],
							devTreeNodeData = nativeArray3[j]
						};
						SafeBitRef enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
						devTreeNodeInfo.locked = ((SafeBitRef)(ref enableBit)).IsValid && ((EnabledMask)(ref enabledMask))[j];
						result.Add(ref devTreeNodeInfo);
					}
				}
			}
			return result;
		}
		finally
		{
			val.Dispose();
		}
	}

	private void BindNodeDetails(IJsonWriter binder, Entity node)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		DevTreeNodeData devTreeNodeData = default(DevTreeNodeData);
		PrefabData prefabData = default(PrefabData);
		if (node != Entity.Null && EntitiesExtensions.TryGetComponent<DevTreeNodeData>(((ComponentSystemBase)this).EntityManager, node, ref devTreeNodeData) && EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, node, ref prefabData))
		{
			DevTreeNodePrefab prefab = m_PrefabSystem.GetPrefab<DevTreeNodePrefab>(prefabData);
			bool flag = EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, node);
			binder.TypeBegin("devTree.NodeDetails");
			binder.PropertyName("entity");
			UnityWriters.Write(binder, node);
			binder.PropertyName("name");
			binder.Write(((Object)prefab).name);
			binder.PropertyName("icon");
			binder.Write(GetDevTreeIcon(prefab));
			binder.PropertyName("cost");
			binder.Write(devTreeNodeData.m_Cost);
			binder.PropertyName("locked");
			binder.Write(flag);
			int num = 0;
			bool flag2 = false;
			DynamicBuffer<DevTreeNodeRequirement> val = default(DynamicBuffer<DevTreeNodeRequirement>);
			if (EntitiesExtensions.TryGetBuffer<DevTreeNodeRequirement>(((ComponentSystemBase)this).EntityManager, node, true, ref val))
			{
				num = val.Length;
				flag2 = val.Length > 0;
				for (int i = 0; i < val.Length; i++)
				{
					if (EntitiesExtensions.HasEnabledComponent<Locked>(((ComponentSystemBase)this).EntityManager, val[i].m_Node))
					{
						flag2 = false;
					}
				}
			}
			binder.PropertyName("unlockable");
			binder.Write(flag2);
			binder.PropertyName("requirementCount");
			binder.Write(num);
			binder.PropertyName("milestoneRequirement");
			binder.Write(ProgressionUtils.GetRequiredMilestone(((ComponentSystemBase)this).EntityManager, devTreeNodeData.m_Service));
			binder.TypeEnd();
		}
		else
		{
			binder.WriteNull();
		}
	}

	private string GetDevTreeIcon(DevTreeNodePrefab prefab)
	{
		if (!string.IsNullOrEmpty(prefab.m_IconPath))
		{
			return prefab.m_IconPath;
		}
		if ((Object)(object)prefab.m_IconPrefab != (Object)null)
		{
			return ImageSystem.GetThumbnail(prefab.m_IconPrefab) ?? m_ImageSystem.placeholderIcon;
		}
		return m_ImageSystem.placeholderIcon;
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
	public DevTreeUISystem()
	{
	}
}
