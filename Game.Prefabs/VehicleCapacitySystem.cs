using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Common;
using Game.Serialization;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class VehicleCapacitySystem : GameSystemBase, IPostDeserialize
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<DeliveryTruckData> __Game_Prefabs_DeliveryTruckData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarTrailerData> __Game_Prefabs_CarTrailerData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CarTractorData> __Game_Prefabs_CarTractorData_RO_ComponentTypeHandle;

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
			__Game_Prefabs_DeliveryTruckData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DeliveryTruckData>(true);
			__Game_Prefabs_CarTrailerData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarTrailerData>(true);
			__Game_Prefabs_CarTractorData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarTractorData>(true);
		}
	}

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_UpdatedQuery;

	private EntityQuery m_DeliveryTruckQuery;

	private NativeList<DeliveryTruckSelectItem> m_DeliveryTruckItems;

	private JobHandle m_WriteDependency;

	private VehicleSelectRequirementData m_VehicleSelectRequirementData;

	private bool m_RequireUpdate;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_VehicleSelectRequirementData = new VehicleSelectRequirementData((SystemBase)(object)this);
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<VehicleData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_UpdatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<DeliveryTruckData>(),
			ComponentType.ReadOnly<CarData>(),
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.None = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Locked>() };
		array2[0] = val;
		m_DeliveryTruckQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		m_DeliveryTruckItems = new NativeList<DeliveryTruckSelectItem>(10, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_DeliveryTruckItems.Dispose();
		base.OnDestroy();
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		if ((int)((Context)(ref context)).purpose == 1 || (int)((Context)(ref context)).purpose == 2)
		{
			m_RequireUpdate = true;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (m_RequireUpdate || !((EntityQuery)(ref m_UpdatedQuery)).IsEmptyIgnoreFilter)
		{
			m_RequireUpdate = false;
			JobHandle val = default(JobHandle);
			NativeList<ArchetypeChunk> prefabChunks = ((EntityQuery)(ref m_DeliveryTruckQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
			m_VehicleSelectRequirementData.Update((SystemBase)(object)this, m_CityConfigurationSystem);
			JobHandle val2 = IJobExtensions.Schedule<UpdateDeliveryTruckSelectJob>(new UpdateDeliveryTruckSelectJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_DeliveryTruckDataType = InternalCompilerInterface.GetComponentTypeHandle<DeliveryTruckData>(ref __TypeHandle.__Game_Prefabs_DeliveryTruckData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarTrailerDataType = InternalCompilerInterface.GetComponentTypeHandle<CarTrailerData>(ref __TypeHandle.__Game_Prefabs_CarTrailerData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CarTractorDataType = InternalCompilerInterface.GetComponentTypeHandle<CarTractorData>(ref __TypeHandle.__Game_Prefabs_CarTractorData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabChunks = prefabChunks,
				m_RequirementData = m_VehicleSelectRequirementData,
				m_DeliveryTruckItems = m_DeliveryTruckItems
			}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
			prefabChunks.Dispose(val2);
			m_WriteDependency = val2;
			((SystemBase)this).Dependency = val2;
		}
	}

	public DeliveryTruckSelectData GetDeliveryTruckSelectData()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_WriteDependency)).Complete();
		return new DeliveryTruckSelectData(m_DeliveryTruckItems.AsArray());
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
	public VehicleCapacitySystem()
	{
	}
}
