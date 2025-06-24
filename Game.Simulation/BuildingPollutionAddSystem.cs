using System;
using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Zones;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class BuildingPollutionAddSystem : GameSystemBase
{
	private struct PollutionItem
	{
		public int amount;

		public float2 position;
	}

	[BurstCompile]
	private struct ApplyBuildingPollutionJob<T> : IJob where T : struct, IPollution
	{
		public NativeArray<T> m_PollutionMap;

		public NativeQueue<PollutionItem> m_PollutionQueue;

		public int m_MapSize;

		public int m_TextureSize;

		public float m_MaxRadiusSq;

		public float m_Radius;

		public float m_Multiplier;

		public NativeArray<float> m_WeightCache;

		[ReadOnly]
		public NativeArray<float> m_DistanceWeightCache;

		public PollutionParameterData m_PollutionParameters;

		private float GetWeight(int2 cell, float2 position, float radiusSq, float offset, int cellSize)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			((float2)(ref val))._002Ector(0f - offset + ((float)cell.x + 0.5f) * (float)cellSize, 0f - offset + ((float)cell.y + 0.5f) * (float)cellSize);
			float num = math.lengthsq(position - val);
			if (num < radiusSq)
			{
				float num2 = 255f * num / m_MaxRadiusSq;
				int num3 = Mathf.FloorToInt(num2);
				return math.lerp(m_DistanceWeightCache[num3], m_DistanceWeightCache[num3 + 1], math.frac(num2));
			}
			return 0f;
		}

		private void AddSingle(int pollution, int mapSize, int textureSize, float2 position, float radius, NativeArray<T> map)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			int num = mapSize / textureSize;
			float num2 = (float)mapSize / 2f;
			float radiusSq = radius * radius;
			int2 val = default(int2);
			((int2)(ref val))._002Ector(math.max(0, Mathf.FloorToInt((position.x + num2 - radius) / (float)num)), math.max(0, Mathf.FloorToInt((position.y + num2 - radius) / (float)num)));
			int2 val2 = default(int2);
			((int2)(ref val2))._002Ector(math.min(textureSize - 1, Mathf.CeilToInt((position.x + num2 + radius) / (float)num)), math.min(textureSize - 1, Mathf.CeilToInt((position.y + num2 + radius) / (float)num)));
			float num3 = 0f;
			int num4 = 0;
			int2 val3 = default(int2);
			val3.x = val.x;
			while (val3.x <= val2.x)
			{
				val3.y = val.y;
				while (val3.y <= val2.y)
				{
					float weight = GetWeight(val3, position, radiusSq, 0.5f * (float)mapSize, num);
					num3 += weight;
					m_WeightCache[num4] = weight;
					num4++;
					val3.y++;
				}
				val3.x++;
			}
			num4 = 0;
			float num5 = 1f / (num3 * (float)kUpdatesPerDay);
			val3.x = val.x;
			while (val3.x <= val2.x)
			{
				int num6 = val3.x + textureSize * val.y;
				val3.y = val.y;
				while (val3.y <= val2.y)
				{
					float num7 = (float)pollution * num5 * m_WeightCache[num4];
					num4++;
					if (num7 > 0.2f)
					{
						int num8 = Mathf.CeilToInt(num7);
						T val4 = map[num6];
						val4.Add((short)num8);
						map[num6] = val4;
					}
					num6 += textureSize;
					val3.y++;
				}
				val3.x++;
			}
		}

		public void Execute()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			PollutionItem pollutionItem = default(PollutionItem);
			while (m_PollutionQueue.TryDequeue(ref pollutionItem))
			{
				AddSingle((int)(m_Multiplier * (float)pollutionItem.amount), m_MapSize, m_TextureSize, pollutionItem.position, m_Radius, m_PollutionMap);
			}
		}
	}

	[BurstCompile]
	private struct BuildingPolluteJob : IJobChunk
	{
		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> m_DestroyedType;

		[ReadOnly]
		public ComponentTypeHandle<Abandoned> m_AbandonedType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Park> m_ParkType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_BuildingEfficiencyType;

		[ReadOnly]
		public BufferTypeHandle<Renter> m_RenterType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_BuildingDatas;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> m_SpawnableDatas;

		[ReadOnly]
		public ComponentLookup<PollutionData> m_PollutionDatas;

		[ReadOnly]
		public ComponentLookup<PollutionModifierData> m_PollutionModifierDatas;

		[ReadOnly]
		public ComponentLookup<ZoneData> m_ZoneDatas;

		[ReadOnly]
		public BufferLookup<Employee> m_Employees;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public ComponentLookup<Citizen> m_Citizens;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public PollutionParameterData m_PollutionParameters;

		public ParallelWriter<PollutionItem> m_GroundPollutionQueue;

		public ParallelWriter<PollutionItem> m_AirPollutionQueue;

		public ParallelWriter<PollutionItem> m_NoisePollutionQueue;

		public Entity m_City;

		public uint m_UpdateFrameIndex;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			bool destroyed = ((ArchetypeChunk)(ref chunk)).Has<Destroyed>(ref m_DestroyedType);
			bool abandoned = ((ArchetypeChunk)(ref chunk)).Has<Abandoned>(ref m_AbandonedType);
			bool isPark = ((ArchetypeChunk)(ref chunk)).Has<Game.Buildings.Park>(ref m_ParkType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_BuildingEfficiencyType);
			BufferAccessor<Renter> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Renter>(ref m_RenterType);
			BufferAccessor<InstalledUpgrade> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				Entity prefab = nativeArray[i].m_Prefab;
				float3 position = nativeArray2[i].m_Position;
				float efficiency = BuildingUtils.GetEfficiency(bufferAccessor, i);
				DynamicBuffer<Renter> renters = ((bufferAccessor2.Length != 0) ? bufferAccessor2[i] : default(DynamicBuffer<Renter>));
				DynamicBuffer<InstalledUpgrade> installedUpgrades = ((bufferAccessor3.Length != 0) ? bufferAccessor3[i] : default(DynamicBuffer<InstalledUpgrade>));
				PollutionData buildingPollution = GetBuildingPollution(prefab, destroyed, abandoned, isPark, efficiency, renters, installedUpgrades, m_PollutionParameters, cityModifiers, ref m_Prefabs, ref m_BuildingDatas, ref m_SpawnableDatas, ref m_PollutionDatas, ref m_PollutionModifierDatas, ref m_ZoneDatas, ref m_Employees, ref m_HouseholdCitizens, ref m_Citizens);
				if (buildingPollution.m_GroundPollution > 0f)
				{
					m_GroundPollutionQueue.Enqueue(new PollutionItem
					{
						amount = (int)buildingPollution.m_GroundPollution,
						position = ((float3)(ref position)).xz
					});
				}
				if (buildingPollution.m_AirPollution > 0f)
				{
					m_AirPollutionQueue.Enqueue(new PollutionItem
					{
						amount = (int)buildingPollution.m_AirPollution,
						position = ((float3)(ref position)).xz
					});
				}
				if (buildingPollution.m_NoisePollution > 0f)
				{
					m_NoisePollutionQueue.Enqueue(new PollutionItem
					{
						amount = (int)buildingPollution.m_NoisePollution,
						position = ((float3)(ref position)).xz
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
		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Destroyed> __Game_Common_Destroyed_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Abandoned> __Game_Buildings_Abandoned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.Park> __Game_Buildings_Park_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Renter> __Game_Buildings_Renter_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PollutionData> __Game_Prefabs_PollutionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PollutionModifierData> __Game_Prefabs_PollutionModifierData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Employee> __Game_Companies_Employee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Common_Destroyed_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Destroyed>(true);
			__Game_Buildings_Abandoned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Abandoned>(true);
			__Game_Buildings_Park_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.Park>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Buildings_Renter_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Renter>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_PollutionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionData>(true);
			__Game_Prefabs_PollutionModifierData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PollutionModifierData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Companies_Employee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Employee>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 128;

	private SimulationSystem m_SimulationSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_PolluterQuery;

	private NativeArray<float> m_GroundWeightCache;

	private NativeArray<float> m_AirWeightCache;

	private NativeArray<float> m_NoiseWeightCache;

	private NativeArray<float> m_DistanceWeightCache;

	private NativeQueue<PollutionItem> m_GroundPollutionQueue;

	private NativeQueue<PollutionItem> m_AirPollutionQueue;

	private NativeQueue<PollutionItem> m_NoisePollutionQueue;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_985639355_0;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (16 * kUpdatesPerDay);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_GroundPollutionQueue = new NativeQueue<PollutionItem>(AllocatorHandle.op_Implicit((Allocator)4));
		m_AirPollutionQueue = new NativeQueue<PollutionItem>(AllocatorHandle.op_Implicit((Allocator)4));
		m_NoisePollutionQueue = new NativeQueue<PollutionItem>(AllocatorHandle.op_Implicit((Allocator)4));
		m_PolluterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Placeholder>()
		});
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if (m_GroundWeightCache.IsCreated)
		{
			m_GroundWeightCache.Dispose();
		}
		if (m_AirWeightCache.IsCreated)
		{
			m_AirWeightCache.Dispose();
		}
		if (m_NoiseWeightCache.IsCreated)
		{
			m_NoiseWeightCache.Dispose();
		}
		if (m_DistanceWeightCache.IsCreated)
		{
			m_DistanceWeightCache.Dispose();
		}
		m_GroundPollutionQueue.Dispose();
		m_AirPollutionQueue.Dispose();
		m_NoisePollutionQueue.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		PollutionParameterData singleton = ((EntityQuery)(ref __query_985639355_0)).GetSingleton<PollutionParameterData>();
		float num = math.max(math.max(singleton.m_GroundRadius, singleton.m_AirRadius), singleton.m_NoiseRadius);
		num *= num;
		if (!m_GroundWeightCache.IsCreated)
		{
			int num2 = 3 + Mathf.CeilToInt(2f * singleton.m_GroundRadius * (float)GroundPollutionSystem.kTextureSize / (float)CellMapSystem<GroundPollution>.kMapSize);
			m_GroundWeightCache = new NativeArray<float>(num2 * num2, (Allocator)4, (NativeArrayOptions)1);
			num2 = 3 + Mathf.CeilToInt(2f * singleton.m_AirRadius * (float)AirPollutionSystem.kTextureSize / (float)CellMapSystem<AirPollution>.kMapSize);
			m_AirWeightCache = new NativeArray<float>(num2 * num2, (Allocator)4, (NativeArrayOptions)1);
			num2 = 3 + Mathf.CeilToInt(2f * singleton.m_NoiseRadius * (float)NoisePollutionSystem.kTextureSize / (float)CellMapSystem<NoisePollution>.kMapSize);
			m_NoiseWeightCache = new NativeArray<float>(num2 * num2, (Allocator)4, (NativeArrayOptions)1);
			m_DistanceWeightCache = new NativeArray<float>(256, (Allocator)4, (NativeArrayOptions)1);
			for (int i = 0; i < 256; i++)
			{
				m_DistanceWeightCache[i] = GetWeight(math.sqrt(num * (float)i / 256f), singleton.m_DistanceExponent);
			}
		}
		uint updateFrameWithInterval = SimulationUtils.GetUpdateFrameWithInterval(m_SimulationSystem.frameIndex, (uint)GetUpdateInterval(SystemUpdatePhase.GameSimulation), 16);
		JobHandle val = JobChunkExtensions.ScheduleParallel<BuildingPolluteJob>(new BuildingPolluteJob
		{
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedType = InternalCompilerInterface.GetComponentTypeHandle<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AbandonedType = InternalCompilerInterface.GetComponentTypeHandle<Abandoned>(ref __TypeHandle.__Game_Buildings_Abandoned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ParkType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.Park>(ref __TypeHandle.__Game_Buildings_Park_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingEfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RenterType = InternalCompilerInterface.GetBufferTypeHandle<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SpawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionDatas = InternalCompilerInterface.GetComponentLookup<PollutionData>(ref __TypeHandle.__Game_Prefabs_PollutionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionModifierDatas = InternalCompilerInterface.GetComponentLookup<PollutionModifierData>(ref __TypeHandle.__Game_Prefabs_PollutionModifierData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ZoneDatas = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Employees = InternalCompilerInterface.GetBufferLookup<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Citizens = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionParameters = singleton,
			m_GroundPollutionQueue = m_GroundPollutionQueue.AsParallelWriter(),
			m_AirPollutionQueue = m_AirPollutionQueue.AsParallelWriter(),
			m_NoisePollutionQueue = m_NoisePollutionQueue.AsParallelWriter(),
			m_City = m_CitySystem.City,
			m_UpdateFrameIndex = updateFrameWithInterval
		}, m_PolluterQuery, ((SystemBase)this).Dependency);
		JobHandle dependencies;
		JobHandle val2 = IJobExtensions.Schedule<ApplyBuildingPollutionJob<GroundPollution>>(new ApplyBuildingPollutionJob<GroundPollution>
		{
			m_PollutionMap = m_GroundPollutionSystem.GetMap(readOnly: false, out dependencies),
			m_MapSize = CellMapSystem<GroundPollution>.kMapSize,
			m_TextureSize = GroundPollutionSystem.kTextureSize,
			m_PollutionParameters = singleton,
			m_MaxRadiusSq = num,
			m_Radius = singleton.m_GroundRadius,
			m_PollutionQueue = m_GroundPollutionQueue,
			m_WeightCache = m_GroundWeightCache,
			m_DistanceWeightCache = m_DistanceWeightCache,
			m_Multiplier = singleton.m_GroundMultiplier
		}, JobHandle.CombineDependencies(val, dependencies));
		m_GroundPollutionSystem.AddWriter(val2);
		JobHandle dependencies2;
		JobHandle val3 = IJobExtensions.Schedule<ApplyBuildingPollutionJob<AirPollution>>(new ApplyBuildingPollutionJob<AirPollution>
		{
			m_PollutionMap = m_AirPollutionSystem.GetMap(readOnly: false, out dependencies2),
			m_MapSize = CellMapSystem<AirPollution>.kMapSize,
			m_TextureSize = AirPollutionSystem.kTextureSize,
			m_PollutionParameters = singleton,
			m_MaxRadiusSq = num,
			m_Radius = singleton.m_AirRadius,
			m_PollutionQueue = m_AirPollutionQueue,
			m_WeightCache = m_AirWeightCache,
			m_DistanceWeightCache = m_DistanceWeightCache,
			m_Multiplier = singleton.m_AirMultiplier
		}, JobHandle.CombineDependencies(dependencies2, val));
		m_AirPollutionSystem.AddWriter(val3);
		JobHandle dependencies3;
		JobHandle val4 = IJobExtensions.Schedule<ApplyBuildingPollutionJob<NoisePollution>>(new ApplyBuildingPollutionJob<NoisePollution>
		{
			m_PollutionMap = m_NoisePollutionSystem.GetMap(readOnly: false, out dependencies3),
			m_MapSize = CellMapSystem<NoisePollution>.kMapSize,
			m_TextureSize = NoisePollutionSystem.kTextureSize,
			m_PollutionParameters = singleton,
			m_MaxRadiusSq = num,
			m_Radius = singleton.m_NoiseRadius,
			m_PollutionQueue = m_NoisePollutionQueue,
			m_WeightCache = m_NoiseWeightCache,
			m_DistanceWeightCache = m_DistanceWeightCache,
			m_Multiplier = singleton.m_NoiseMultiplier
		}, JobHandle.CombineDependencies(dependencies3, val));
		m_NoisePollutionSystem.AddWriter(val4);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val2, val3, val4);
	}

	private static float GetWeight(float distance, float exponent)
	{
		return 1f / math.max(20f, math.pow(distance, exponent));
	}

	public static PollutionData GetBuildingPollution(Entity prefab, bool destroyed, bool abandoned, bool isPark, float efficiency, DynamicBuffer<Renter> renters, DynamicBuffer<InstalledUpgrade> installedUpgrades, PollutionParameterData pollutionParameters, DynamicBuffer<CityModifier> cityModifiers, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<BuildingData> buildingDatas, ref ComponentLookup<SpawnableBuildingData> spawnableDatas, ref ComponentLookup<PollutionData> pollutionDatas, ref ComponentLookup<PollutionModifierData> pollutionModifierDatas, ref ComponentLookup<ZoneData> zoneDatas, ref BufferLookup<Employee> employees, ref BufferLookup<HouseholdCitizen> householdCitizens, ref ComponentLookup<Citizen> citizens)
	{
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		PollutionData data = default(PollutionData);
		if (!(destroyed || abandoned))
		{
			if (efficiency > 0f && pollutionDatas.TryGetComponent(prefab, ref data))
			{
				if (installedUpgrades.IsCreated)
				{
					UpgradeUtils.CombineStats<PollutionData>(ref data, installedUpgrades, ref prefabRefs, ref pollutionDatas);
				}
				SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
				if (data.m_ScaleWithRenters && !isPark && renters.IsCreated)
				{
					CountRenters(out var count, out var education, renters, ref employees, ref householdCitizens, ref citizens, ignoreEmployees: false);
					float num = (spawnableDatas.TryGetComponent(prefab, ref spawnableBuildingData) ? ((float)(int)spawnableBuildingData.m_Level) : 5f);
					float num2 = ((count > 0) ? (5f * (float)count / (num + 0.5f * (float)(education / count))) : 0f);
					data.m_GroundPollution *= num2;
					data.m_AirPollution *= num2;
					data.m_NoisePollution *= num2;
				}
				if (cityModifiers.IsCreated && spawnableDatas.TryGetComponent(prefab, ref spawnableBuildingData))
				{
					ZoneData zoneData = zoneDatas[spawnableBuildingData.m_ZonePrefab];
					if (zoneData.m_AreaType == AreaType.Industrial && (zoneData.m_ZoneFlags & ZoneFlags.Office) == 0)
					{
						CityUtils.ApplyModifier(ref data.m_GroundPollution, cityModifiers, CityModifierType.IndustrialGroundPollution);
						CityUtils.ApplyModifier(ref data.m_AirPollution, cityModifiers, CityModifierType.IndustrialAirPollution);
					}
				}
				if (installedUpgrades.IsCreated)
				{
					PollutionModifierData data2 = default(PollutionModifierData);
					UpgradeUtils.CombineStats<PollutionModifierData>(ref data2, installedUpgrades, ref prefabRefs, ref pollutionModifierDatas);
					data.m_GroundPollution *= math.max(0f, 1f + data2.m_GroundPollutionMultiplier);
					data.m_AirPollution *= math.max(0f, 1f + data2.m_AirPollutionMultiplier);
					data.m_NoisePollution *= math.max(0f, 1f + data2.m_NoisePollutionMultiplier);
				}
			}
			else
			{
				data = default(PollutionData);
			}
		}
		else
		{
			BuildingData buildingData = buildingDatas[prefab];
			data = new PollutionData
			{
				m_GroundPollution = 0f,
				m_AirPollution = 0f,
				m_NoisePollution = (destroyed ? 0f : (5f * (float)(buildingData.m_LotSize.x * buildingData.m_LotSize.y) * pollutionParameters.m_AbandonedNoisePollutionMultiplier))
			};
		}
		if ((abandoned || isPark) && renters.IsCreated)
		{
			CountRenters(out var count2, out var _, renters, ref employees, ref householdCitizens, ref citizens, ignoreEmployees: true);
			data.m_NoisePollution += count2 * pollutionParameters.m_HomelessNoisePollution;
		}
		return data;
	}

	private static void CountRenters(out int count, out int education, DynamicBuffer<Renter> renters, ref BufferLookup<Employee> employees, ref BufferLookup<HouseholdCitizen> householdCitizens, ref ComponentLookup<Citizen> citizens, bool ignoreEmployees)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		count = 0;
		education = 0;
		Enumerator<Renter> enumerator = renters.GetEnumerator();
		try
		{
			DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
			Citizen citizen = default(Citizen);
			DynamicBuffer<Employee> val2 = default(DynamicBuffer<Employee>);
			Citizen citizen2 = default(Citizen);
			while (enumerator.MoveNext())
			{
				Renter current = enumerator.Current;
				if (householdCitizens.TryGetBuffer((Entity)current, ref val))
				{
					Enumerator<HouseholdCitizen> enumerator2 = val.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							HouseholdCitizen current2 = enumerator2.Current;
							if (citizens.TryGetComponent((Entity)current2, ref citizen))
							{
								education += citizen.GetEducationLevel();
								count++;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
					}
				}
				else
				{
					if (ignoreEmployees || !employees.TryGetBuffer((Entity)current, ref val2))
					{
						continue;
					}
					Enumerator<Employee> enumerator3 = val2.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							if (citizens.TryGetComponent(enumerator3.Current.m_Worker, ref citizen2))
							{
								education += citizen2.GetEducationLevel();
								count++;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator3/*cast due to .constrained prefix*/).Dispose();
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<PollutionParameterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_985639355_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public BuildingPollutionAddSystem()
	{
	}
}
