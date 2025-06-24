using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Debug;
using Game.Prefabs;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class CountResidentialPropertySystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public struct ResidentialPropertyData : IAccumulable<ResidentialPropertyData>, ISerializable
	{
		public int3 m_FreeProperties;

		public int3 m_TotalProperties;

		public int m_FreeShelterCapacity;

		public int m_TotalShelterCapacity;

		public void Accumulate(ResidentialPropertyData other)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			m_FreeProperties += other.m_FreeProperties;
			m_TotalProperties += other.m_TotalProperties;
			m_FreeShelterCapacity += other.m_FreeShelterCapacity;
			m_TotalShelterCapacity += other.m_TotalShelterCapacity;
		}

		public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			int3 val = m_FreeProperties;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
			int3 val2 = m_TotalProperties;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
			int num = m_FreeShelterCapacity;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
			int num2 = m_TotalShelterCapacity;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		}

		public void Deserialize<TReader>(TReader reader) where TReader : IReader
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			ref int3 reference = ref m_FreeProperties;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref int3 reference2 = ref m_TotalProperties;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
			Context context = ((IReader)reader).context;
			ContextFormat format = ((Context)(ref context)).format;
			if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.HomelessAndWorkerFix))
			{
				ref int reference3 = ref m_FreeShelterCapacity;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
				ref int reference4 = ref m_TotalShelterCapacity;
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference4);
			}
		}
	}

	[BurstCompile]
	private struct CountResidentialPropertyJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableBuildingDatas;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneDatas;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> m_ZonePropertiesDatas;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> m_BuildingPropertyDatas;

		[ReadOnly]
		public ComponentLookup<Household> m_Households;

		public ParallelWriter<ResidentialPropertyData> m_ResidentialPropertyData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			ResidentialPropertyData residentialPropertyData = default(ResidentialPropertyData);
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Renter> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			if (((ArchetypeChunk)(ref chunk)).Has<Abandoned>() || ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Park>())
			{
				for (int i = 0; i < nativeArray.Length; i++)
				{
					int shelterHomelessCapacity = BuildingUtils.GetShelterHomelessCapacity(nativeArray[i].m_Prefab, ref m_BuildingDatas, ref m_BuildingPropertyDatas);
					residentialPropertyData.m_TotalShelterCapacity += shelterHomelessCapacity;
					DynamicBuffer<Renter> val = bufferAccessor[i];
					int num = 0;
					for (int j = 0; j < val.Length; j++)
					{
						if (m_Households.HasComponent(val[j].m_Renter))
						{
							num++;
						}
					}
					residentialPropertyData.m_FreeShelterCapacity = math.max(0, shelterHomelessCapacity - num);
				}
			}
			else
			{
				ZoneData zoneData = default(ZoneData);
				ZonePropertiesData zonePropertiesData = default(ZonePropertiesData);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Entity prefab = nativeArray[k].m_Prefab;
					if (!m_BuildingPropertyDatas.HasComponent(prefab))
					{
						continue;
					}
					SpawnableBuildingData spawnableBuildingData = m_SpawnableBuildingDatas[prefab];
					if ((!m_ZoneDatas.TryGetComponent(spawnableBuildingData.m_ZonePrefab, ref zoneData) && zoneData.m_AreaType != AreaType.Residential) || !m_ZonePropertiesDatas.TryGetComponent(spawnableBuildingData.m_ZonePrefab, ref zonePropertiesData))
					{
						continue;
					}
					ZoneDensity zoneDensity = PropertyUtils.GetZoneDensity(zoneData, zonePropertiesData);
					BuildingPropertyData propertyData = m_BuildingPropertyDatas[prefab];
					DynamicBuffer<Renter> val2 = bufferAccessor[k];
					int residentialProperties = PropertyUtils.GetResidentialProperties(propertyData);
					int num2 = 0;
					for (int l = 0; l < val2.Length; l++)
					{
						if (m_Households.HasComponent(val2[l].m_Renter))
						{
							num2++;
						}
					}
					switch (zoneDensity)
					{
					case ZoneDensity.Low:
						residentialPropertyData.m_TotalProperties.x += residentialProperties;
						if (((ArchetypeChunk)(ref chunk)).Has<PropertyOnMarket>() || ((ArchetypeChunk)(ref chunk)).Has<PropertyToBeOnMarket>())
						{
							residentialPropertyData.m_FreeProperties.x += residentialProperties - num2;
						}
						break;
					case ZoneDensity.Medium:
						residentialPropertyData.m_TotalProperties.y += residentialProperties;
						if (((ArchetypeChunk)(ref chunk)).Has<PropertyOnMarket>() || ((ArchetypeChunk)(ref chunk)).Has<PropertyToBeOnMarket>())
						{
							residentialPropertyData.m_FreeProperties.y += residentialProperties - num2;
						}
						break;
					default:
						residentialPropertyData.m_TotalProperties.z += residentialProperties;
						if (((ArchetypeChunk)(ref chunk)).Has<PropertyOnMarket>() || ((ArchetypeChunk)(ref chunk)).Has<PropertyToBeOnMarket>())
						{
							residentialPropertyData.m_FreeProperties.z += residentialProperties - num2;
						}
						break;
					}
				}
			}
			m_ResidentialPropertyData.Accumulate(residentialPropertyData);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZonePropertiesData> __Game_Prefabs_ZonePropertiesData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Household> __Game_Citizens_Household_RO_ComponentLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZonePropertiesData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Citizens_Household_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Household>(true);
		}
	}

	private NativeAccumulator<ResidentialPropertyData> m_ResidentialPropertyData;

	private ResidentialPropertyData m_LastResidentialPropertyData;

	private EntityQuery m_ResidentialPropertyQuery;

	private TypeHandle __TypeHandle;

	[DebugWatchValue]
	public int3 FreeProperties => m_LastResidentialPropertyData.m_FreeProperties;

	[DebugWatchValue]
	public int3 TotalProperties => m_LastResidentialPropertyData.m_TotalProperties;

	[DebugWatchValue]
	public int FreeShelterCapacity => m_LastResidentialPropertyData.m_FreeShelterCapacity;

	[DebugWatchValue]
	public int TotalShelterCapacity => m_LastResidentialPropertyData.m_TotalShelterCapacity;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public ResidentialPropertyData GetResidentialPropertyData()
	{
		return m_LastResidentialPropertyData;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		EntityQueryDesc[] array = new EntityQueryDesc[2];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Building>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Abandoned>(),
			ComponentType.ReadOnly<Game.Buildings.Park>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Condemned>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ResidentialProperty>() };
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Condemned>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[1] = val;
		m_ResidentialPropertyQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_ResidentialPropertyData = new NativeAccumulator<ResidentialPropertyData>(AllocatorHandle.op_Implicit((Allocator)4));
		((ComponentSystemBase)this).RequireForUpdate(m_ResidentialPropertyQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_ResidentialPropertyData.Dispose();
		base.OnDestroy();
	}

	public void SetDefaults(Context context)
	{
		m_LastResidentialPropertyData = default(ResidentialPropertyData);
		m_ResidentialPropertyData.Clear();
	}

	[Preserve]
	protected override void OnUpdate()
	{
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
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		m_LastResidentialPropertyData = m_ResidentialPropertyData.GetResult(0);
		m_ResidentialPropertyData.Clear();
		CountResidentialPropertyJob countResidentialPropertyJob = new CountResidentialPropertyJob
		{
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableBuildingDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZonePropertiesDatas = InternalCompilerInterface.GetComponentLookup<ZonePropertiesData>(ref __TypeHandle.__Game_Prefabs_ZonePropertiesData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneDatas = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Households = InternalCompilerInterface.GetComponentLookup<Household>(ref __TypeHandle.__Game_Citizens_Household_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResidentialPropertyData = m_ResidentialPropertyData.AsParallelWriter()
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<CountResidentialPropertyJob>(countResidentialPropertyJob, m_ResidentialPropertyQuery, ((SystemBase)this).Dependency);
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		((IWriter)writer/*cast due to .constrained prefix*/).Write<ResidentialPropertyData>(m_LastResidentialPropertyData);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.homelessFix)
		{
			ref ResidentialPropertyData reference = ref m_LastResidentialPropertyData;
			((IReader)reader/*cast due to .constrained prefix*/).Read<ResidentialPropertyData>(ref reference);
		}
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
	public CountResidentialPropertySystem()
	{
	}
}
