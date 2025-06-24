using System.Runtime.CompilerServices;
using Game.Common;
using Game.Objects;
using Game.Routes;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class RouteInitializeSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<RouteData> __Game_Prefabs_RouteData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkRouteData> __Game_Prefabs_WorkRouteData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TransportStopData> __Game_Prefabs_TransportStopData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<MailBoxData> __Game_Prefabs_MailBoxData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<WorkStopData> __Game_Prefabs_WorkStopData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle;

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
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_RouteData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<RouteData>(false);
			__Game_Prefabs_TransportLineData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportLineData>(true);
			__Game_Prefabs_WorkRouteData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkRouteData>(true);
			__Game_Prefabs_TransportStopData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TransportStopData>(true);
			__Game_Prefabs_MailBoxData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<MailBoxData>(true);
			__Game_Prefabs_WorkStopData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<WorkStopData>(true);
			__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ObjectGeometryData>(false);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_PrefabQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<RouteData>(),
			ComponentType.ReadOnly<TransportStopData>(),
			ComponentType.ReadOnly<MailBoxData>(),
			ComponentType.ReadOnly<WorkStopData>()
		};
		array[0] = val;
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_PrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<RouteData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<RouteData>(ref __TypeHandle.__Game_Prefabs_RouteData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<TransportLineData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<WorkRouteData> componentTypeHandle4 = InternalCompilerInterface.GetComponentTypeHandle<WorkRouteData>(ref __TypeHandle.__Game_Prefabs_WorkRouteData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<TransportStopData> componentTypeHandle5 = InternalCompilerInterface.GetComponentTypeHandle<TransportStopData>(ref __TypeHandle.__Game_Prefabs_TransportStopData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<MailBoxData> componentTypeHandle6 = InternalCompilerInterface.GetComponentTypeHandle<MailBoxData>(ref __TypeHandle.__Game_Prefabs_MailBoxData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<WorkStopData> componentTypeHandle7 = InternalCompilerInterface.GetComponentTypeHandle<WorkStopData>(ref __TypeHandle.__Game_Prefabs_WorkStopData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ObjectGeometryData> componentTypeHandle8 = InternalCompilerInterface.GetComponentTypeHandle<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
			NativeArray<RouteData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<RouteData>(ref componentTypeHandle2);
			if (nativeArray2.Length != 0)
			{
				float num = 0f;
				RouteType type = RouteType.None;
				if (((ArchetypeChunk)(ref val2)).Has<TransportLineData>(ref componentTypeHandle3))
				{
					num = 8f;
					type = RouteType.TransportLine;
				}
				if (((ArchetypeChunk)(ref val2)).Has<WorkRouteData>(ref componentTypeHandle4))
				{
					num = 8f;
					type = RouteType.WorkRoute;
				}
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					RoutePrefab prefab = m_PrefabSystem.GetPrefab<RoutePrefab>(nativeArray[j]);
					RouteData routeData = nativeArray2[j];
					routeData.m_SnapDistance = math.max(num, prefab.m_Width);
					routeData.m_Type = type;
					routeData.m_Color = Color32.op_Implicit(prefab.m_Color);
					routeData.m_Width = prefab.m_Width;
					routeData.m_SegmentLength = prefab.m_SegmentLength;
					nativeArray2[j] = routeData;
				}
			}
			if (((ArchetypeChunk)(ref val2)).Has<TransportStopData>(ref componentTypeHandle5) || ((ArchetypeChunk)(ref val2)).Has<MailBoxData>(ref componentTypeHandle6) || ((ArchetypeChunk)(ref val2)).Has<WorkStopData>(ref componentTypeHandle7))
			{
				NativeArray<ObjectGeometryData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ObjectGeometryData>(ref componentTypeHandle8);
				for (int k = 0; k < nativeArray3.Length; k++)
				{
					ObjectGeometryData objectGeometryData = nativeArray3[k];
					objectGeometryData.m_Flags &= ~(GeometryFlags.Overridable | GeometryFlags.Brushable);
					nativeArray3[k] = objectGeometryData;
				}
			}
		}
		val.Dispose();
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
	public RouteInitializeSystem()
	{
	}
}
