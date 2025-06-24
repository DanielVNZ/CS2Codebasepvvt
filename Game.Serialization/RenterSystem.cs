using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Serialization;

[CompilerGenerated]
public class RenterSystem : GameSystemBase
{
	[BurstCompile]
	private struct RenterJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		[ReadOnly]
		public ComponentTypeHandle<TouristHousehold> m_TouristHouseholdType;

		[ReadOnly]
		public ComponentTypeHandle<HomelessHousehold> m_HomelessHouseholdType;

		[ReadOnly]
		public ComponentLookup<CityServiceUpkeep> m_ServiceBuildings;

		[ReadOnly]
		public ComponentLookup<CompanyData> m_CompanyDatas;

		public BufferLookup<Renter> m_Renters;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PropertyRenter> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			NativeArray<TouristHousehold> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TouristHousehold>(ref m_TouristHouseholdType);
			NativeArray<HomelessHousehold> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HomelessHousehold>(ref m_HomelessHouseholdType);
			DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				PropertyRenter propertyRenter = nativeArray2[i];
				if (m_Renters.TryGetBuffer(propertyRenter.m_Property, ref val2))
				{
					if (m_CompanyDatas.HasComponent(val))
					{
						bool flag = false;
						for (int j = 0; j < val2.Length; j++)
						{
							if (m_CompanyDatas.HasComponent(val2[j].m_Renter))
							{
								Debug.LogWarning((object)$"delete duplicate company:{val.Index}");
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(val);
								flag = true;
							}
						}
						if (!flag)
						{
							val2.Add(new Renter
							{
								m_Renter = val
							});
						}
					}
					else
					{
						val2.Add(new Renter
						{
							m_Renter = val
						});
					}
				}
				else if (!m_ServiceBuildings.HasComponent(val))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<PropertyRenter>(val);
				}
			}
			DynamicBuffer<Renter> val3 = default(DynamicBuffer<Renter>);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Entity renter = nativeArray[k];
				TouristHousehold touristHousehold = nativeArray3[k];
				if (m_Renters.TryGetBuffer(touristHousehold.m_Hotel, ref val3))
				{
					val3.Add(new Renter
					{
						m_Renter = renter
					});
				}
			}
			DynamicBuffer<Renter> val4 = default(DynamicBuffer<Renter>);
			for (int l = 0; l < nativeArray4.Length; l++)
			{
				Entity renter2 = nativeArray[l];
				HomelessHousehold homelessHousehold = nativeArray4[l];
				if (m_Renters.TryGetBuffer(homelessHousehold.m_TempHome, ref val4))
				{
					val4.Add(new Renter
					{
						m_Renter = renter2
					});
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TouristHousehold> __Game_Citizens_TouristHousehold_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HomelessHousehold> __Game_Citizens_HomelessHousehold_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<CityServiceUpkeep> __Game_City_CityServiceUpkeep_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CompanyData> __Game_Companies_CompanyData_RO_ComponentLookup;

		public BufferLookup<Renter> __Game_Buildings_Renter_RW_BufferLookup;

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
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Citizens_TouristHousehold_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TouristHousehold>(true);
			__Game_Citizens_HomelessHousehold_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HomelessHousehold>(true);
			__Game_City_CityServiceUpkeep_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CityServiceUpkeep>(true);
			__Game_Companies_CompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CompanyData>(true);
			__Game_Buildings_Renter_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(false);
		}
	}

	private DeserializationBarrier m_DeserializationBarrier;

	private EntityQuery m_Query;

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
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_DeserializationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DeserializationBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<TouristHousehold>(),
			ComponentType.ReadOnly<HomelessHousehold>()
		};
		array[0] = val;
		m_Query = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireForUpdate(m_Query);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		RenterJob renterJob = new RenterJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TouristHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<TouristHousehold>(ref __TypeHandle.__Game_Citizens_TouristHousehold_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HomelessHouseholdType = InternalCompilerInterface.GetComponentTypeHandle<HomelessHousehold>(ref __TypeHandle.__Game_Citizens_HomelessHousehold_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceBuildings = InternalCompilerInterface.GetComponentLookup<CityServiceUpkeep>(ref __TypeHandle.__Game_City_CityServiceUpkeep_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompanyDatas = InternalCompilerInterface.GetComponentLookup<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CommandBuffer = m_DeserializationBarrier.CreateCommandBuffer()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.Schedule<RenterJob>(renterJob, m_Query, ((SystemBase)this).Dependency);
		((EntityCommandBufferSystem)m_DeserializationBarrier).AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public RenterSystem()
	{
	}
}
