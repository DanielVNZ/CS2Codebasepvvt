using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
using Unity.Assertions;
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
public class LandValueSystem : CellMapSystem<LandValueCell>, IJobSerializable
{
	private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public int m_TotalCount;

		public float m_TotalLandValueBonus;

		public Bounds3 m_Bounds;

		public ComponentLookup<LandValue> m_LandValueData;

		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && m_LandValueData.HasComponent(entity) && m_EdgeGeometryData.HasComponent(entity))
			{
				LandValue landValue = m_LandValueData[entity];
				if (landValue.m_LandValue > 0f)
				{
					m_TotalLandValueBonus += landValue.m_LandValue;
					m_TotalCount++;
				}
			}
		}
	}

	[BurstCompile]
	private struct LandValueMapUpdateJob : IJobParallelFor
	{
		public NativeArray<LandValueCell> m_LandValueMap;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public NativeArray<TerrainAttractiveness> m_AttractiveMap;

		[ReadOnly]
		public NativeArray<GroundPollution> m_GroundPollutionMap;

		[ReadOnly]
		public NativeArray<AirPollution> m_AirPollutionMap;

		[ReadOnly]
		public NativeArray<NoisePollution> m_NoisePollutionMap;

		[ReadOnly]
		public NativeArray<AvailabilityInfoCell> m_AvailabilityInfoMap;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_TelecomCoverageMap;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public ComponentLookup<LandValue> m_LandValueData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		[ReadOnly]
		public AttractivenessParameterData m_AttractivenessParameterData;

		[ReadOnly]
		public LandValueParameterData m_LandValueParameterData;

		public float m_CellSize;

		public void Execute(int index)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			float3 cellCenter = CellMapSystem<LandValueCell>.GetCellCenter(index, kTextureSize);
			if (WaterUtils.SampleDepth(ref m_WaterSurfaceData, cellCenter) > 1f)
			{
				m_LandValueMap[index] = new LandValueCell
				{
					m_LandValue = m_LandValueParameterData.m_LandValueBaseline
				};
				return;
			}
			NetIterator netIterator = new NetIterator
			{
				m_TotalCount = 0,
				m_TotalLandValueBonus = 0f,
				m_Bounds = new Bounds3(cellCenter - new float3(1.5f * m_CellSize, 10000f, 1.5f * m_CellSize), cellCenter + new float3(1.5f * m_CellSize, 10000f, 1.5f * m_CellSize)),
				m_EdgeGeometryData = m_EdgeGeometryData,
				m_LandValueData = m_LandValueData
			};
			m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
			float num = GroundPollutionSystem.GetPollution(cellCenter, m_GroundPollutionMap).m_Pollution;
			float num2 = AirPollutionSystem.GetPollution(cellCenter, m_AirPollutionMap).m_Pollution;
			float num3 = NoisePollutionSystem.GetPollution(cellCenter, m_NoisePollutionMap).m_Pollution;
			float x = AvailabilityInfoToGridSystem.GetAvailabilityInfo(cellCenter, m_AvailabilityInfoMap).m_AvailabilityInfo.x;
			float num4 = TelecomCoverage.SampleNetworkQuality(m_TelecomCoverageMap, cellCenter);
			LandValueCell landValueCell = m_LandValueMap[index];
			float num5 = (((float)netIterator.m_TotalCount > 0f) ? (netIterator.m_TotalLandValueBonus / (float)netIterator.m_TotalCount) : 0f);
			float num6 = math.min((x - 5f) * m_LandValueParameterData.m_AttractivenessBonusMultiplier, m_LandValueParameterData.m_CommonFactorMaxBonus);
			float num7 = math.min(num4 * m_LandValueParameterData.m_TelecomCoverageBonusMultiplier, m_LandValueParameterData.m_CommonFactorMaxBonus);
			num5 += num6 + num7;
			float num8 = WaterUtils.SamplePolluted(ref m_WaterSurfaceData, cellCenter);
			float num9 = 0f;
			if (num8 <= 0f && num <= 0f)
			{
				num9 = TerrainAttractivenessSystem.EvaluateAttractiveness(TerrainUtils.SampleHeight(ref m_TerrainHeightData, cellCenter), m_AttractiveMap[index], m_AttractivenessParameterData);
				num5 += math.min(math.max(num9 - 5f, 0f) * m_LandValueParameterData.m_AttractivenessBonusMultiplier, m_LandValueParameterData.m_CommonFactorMaxBonus);
			}
			float num10 = num * m_LandValueParameterData.m_GroundPollutionPenaltyMultiplier + num2 * m_LandValueParameterData.m_AirPollutionPenaltyMultiplier + num3 * m_LandValueParameterData.m_NoisePollutionPenaltyMultiplier;
			float num11 = math.max(m_LandValueParameterData.m_LandValueBaseline, m_LandValueParameterData.m_LandValueBaseline + num5 - num10);
			if (math.abs(landValueCell.m_LandValue - num11) >= 0.1f)
			{
				landValueCell.m_LandValue = math.lerp(landValueCell.m_LandValue, num11, 0.4f);
			}
			m_LandValueMap[index] = landValueCell;
		}
	}

	[BurstCompile]
	private struct EdgeUpdateJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Edge> m_EdgeType;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.ServiceCoverage> m_ServiceCoverageType;

		[ReadOnly]
		public BufferTypeHandle<ResourceAvailability> m_AvailabilityType;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<LandValue> m_LandValues;

		[ReadOnly]
		public LandValueParameterData m_LandValueParameterData;

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
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Edge> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Edge>(ref m_EdgeType);
			BufferAccessor<Game.Net.ServiceCoverage> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.ServiceCoverage>(ref m_ServiceCoverageType);
			BufferAccessor<ResourceAvailability> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ResourceAvailability>(ref m_AvailabilityType);
			for (int i = 0; i < nativeArray2.Length; i++)
			{
				Entity val = nativeArray[i];
				float num = 0f;
				float num2 = 0f;
				float num3 = 0f;
				if (bufferAccessor.Length > 0)
				{
					DynamicBuffer<Game.Net.ServiceCoverage> val2 = bufferAccessor[i];
					Game.Net.ServiceCoverage serviceCoverage = val2[0];
					num = math.lerp(serviceCoverage.m_Coverage.x, serviceCoverage.m_Coverage.y, 0.5f) * m_LandValueParameterData.m_HealthCoverageBonusMultiplier;
					Game.Net.ServiceCoverage serviceCoverage2 = val2[5];
					num2 = math.lerp(serviceCoverage2.m_Coverage.x, serviceCoverage2.m_Coverage.y, 0.5f) * m_LandValueParameterData.m_EducationCoverageBonusMultiplier;
					Game.Net.ServiceCoverage serviceCoverage3 = val2[2];
					num3 = math.lerp(serviceCoverage3.m_Coverage.x, serviceCoverage3.m_Coverage.y, 0.5f) * m_LandValueParameterData.m_PoliceCoverageBonusMultiplier;
				}
				float num4 = 0f;
				float num5 = 0f;
				float num6 = 0f;
				if (bufferAccessor2.Length > 0)
				{
					DynamicBuffer<ResourceAvailability> val3 = bufferAccessor2[i];
					ResourceAvailability resourceAvailability = val3[1];
					num4 = math.lerp(resourceAvailability.m_Availability.x, resourceAvailability.m_Availability.y, 0.5f) * m_LandValueParameterData.m_CommercialServiceBonusMultiplier;
					ResourceAvailability resourceAvailability2 = val3[31];
					num5 = math.lerp(resourceAvailability2.m_Availability.x, resourceAvailability2.m_Availability.y, 0.5f) * m_LandValueParameterData.m_BusBonusMultiplier;
					ResourceAvailability resourceAvailability3 = val3[32];
					num6 = math.lerp(resourceAvailability3.m_Availability.x, resourceAvailability3.m_Availability.y, 0.5f) * m_LandValueParameterData.m_TramSubwayBonusMultiplier;
				}
				LandValue landValue = m_LandValues[val];
				float num7 = math.max(num + num2 + num3 + num4 + num5 + num6, 0f);
				if (math.abs(landValue.m_LandValue - num7) >= 0.1f)
				{
					float num8 = math.lerp(landValue.m_LandValue, num7, 0.6f);
					landValue.m_LandValue = math.max(num8, 0f);
					m_LandValues[val] = landValue;
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
		public ComponentTypeHandle<Edge> __Game_Net_Edge_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferTypeHandle;

		public ComponentLookup<LandValue> __Game_Net_LandValue_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LandValue> __Game_Net_LandValue_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

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
			__Game_Net_Edge_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Edge>(true);
			__Game_Net_ServiceCoverage_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.ServiceCoverage>(true);
			__Game_Net_ResourceAvailability_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ResourceAvailability>(true);
			__Game_Net_LandValue_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(false);
			__Game_Net_LandValue_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
		}
	}

	public static readonly int kTextureSize = 128;

	public static readonly int kUpdatesPerDay = 32;

	private EntityQuery m_EdgeGroup;

	private EntityQuery m_NodeGroup;

	private EntityQuery m_AttractivenessParameterQuery;

	private EntityQuery m_LandValueParameterQuery;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private AvailabilityInfoToGridSystem m_AvailabilityInfoToGridSystem;

	private SearchSystem m_NetSearchSystem;

	private TerrainAttractivenessSystem m_TerrainAttractivenessSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private TypeHandle __TypeHandle;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<LandValueCell>.GetCellCenter(index, kTextureSize);
	}

	public static int GetCellIndex(float3 pos)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		int num = CellMapSystem<LandValueCell>.kMapSize / kTextureSize;
		return Mathf.FloorToInt(((float)(CellMapSystem<LandValueCell>.kMapSize / 2) + pos.x) / (float)num) + Mathf.FloorToInt(((float)(CellMapSystem<LandValueCell>.kMapSize / 2) + pos.z) / (float)num) * kTextureSize;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		Assert.IsTrue(kTextureSize == TerrainAttractivenessSystem.kTextureSize);
		CreateTextures(kTextureSize);
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_TerrainAttractivenessSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainAttractivenessSystem>();
		m_AvailabilityInfoToGridSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AvailabilityInfoToGridSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_AttractivenessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AttractivenessParameterData>() });
		m_LandValueParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<LandValueParameterData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadWrite<LandValue>(),
			ComponentType.ReadOnly<Curve>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[0];
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_EdgeGroup = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[1] { m_EdgeGroup });
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_EdgeGroup)).IsEmptyIgnoreFilter)
		{
			EdgeUpdateJob edgeUpdateJob = new EdgeUpdateJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeType = InternalCompilerInterface.GetComponentTypeHandle<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceCoverageType = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_AvailabilityType = InternalCompilerInterface.GetBufferTypeHandle<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_LandValues = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LandValueParameterData = ((EntityQuery)(ref m_LandValueParameterQuery)).GetSingleton<LandValueParameterData>()
			};
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<EdgeUpdateJob>(edgeUpdateJob, m_EdgeGroup, ((SystemBase)this).Dependency);
		}
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle dependencies3;
		JobHandle dependencies4;
		JobHandle dependencies5;
		JobHandle dependencies6;
		JobHandle dependencies7;
		JobHandle deps;
		LandValueMapUpdateJob landValueMapUpdateJob = new LandValueMapUpdateJob
		{
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_AttractiveMap = m_TerrainAttractivenessSystem.GetMap(readOnly: true, out dependencies2),
			m_GroundPollutionMap = m_GroundPollutionSystem.GetMap(readOnly: true, out dependencies3),
			m_AirPollutionMap = m_AirPollutionSystem.GetMap(readOnly: true, out dependencies4),
			m_NoisePollutionMap = m_NoisePollutionSystem.GetMap(readOnly: true, out dependencies5),
			m_AvailabilityInfoMap = m_AvailabilityInfoToGridSystem.GetMap(readOnly: true, out dependencies6),
			m_TelecomCoverageMap = m_TelecomCoverageSystem.GetData(readOnly: true, out dependencies7),
			m_LandValueMap = m_Map,
			m_LandValueData = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttractivenessParameterData = ((EntityQuery)(ref m_AttractivenessParameterQuery)).GetSingleton<AttractivenessParameterData>(),
			m_LandValueParameterData = ((EntityQuery)(ref m_LandValueParameterQuery)).GetSingleton<LandValueParameterData>(),
			m_CellSize = (float)CellMapSystem<LandValueCell>.kMapSize / (float)kTextureSize
		};
		((SystemBase)this).Dependency = IJobParallelForExtensions.Schedule<LandValueMapUpdateJob>(landValueMapUpdateJob, kTextureSize * kTextureSize, kTextureSize, JobHandle.CombineDependencies(dependencies, dependencies2, JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps, JobHandle.CombineDependencies(dependencies3, dependencies5, JobHandle.CombineDependencies(dependencies6, dependencies4, dependencies7))))));
		AddWriter(((SystemBase)this).Dependency);
		m_NetSearchSystem.AddNetSearchTreeReader(((SystemBase)this).Dependency);
		m_WaterSystem.AddSurfaceReader(((SystemBase)this).Dependency);
		m_TerrainAttractivenessSystem.AddReader(((SystemBase)this).Dependency);
		m_GroundPollutionSystem.AddReader(((SystemBase)this).Dependency);
		m_AirPollutionSystem.AddReader(((SystemBase)this).Dependency);
		m_NoisePollutionSystem.AddReader(((SystemBase)this).Dependency);
		m_AvailabilityInfoToGridSystem.AddReader(((SystemBase)this).Dependency);
		m_TelecomCoverageSystem.AddReader(((SystemBase)this).Dependency);
		m_TerrainSystem.AddCPUHeightReader(((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies, ((SystemBase)this).Dependency);
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
	public LandValueSystem()
	{
	}
}
