using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Events;
using Game.Prefabs;
using Game.Serialization;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class SoilWaterSystem : CellMapSystem<SoilWater>, IJobSerializable, IPostDeserialize
{
	[BurstCompile]
	private struct SoilWaterTickJob : IJob
	{
		public NativeArray<SoilWater> m_SoilWaterMap;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public NativeArray<float> m_SoilWaterTextureData;

		public SoilWaterParameterData m_SoilWaterParameters;

		public ComponentLookup<WaterLevelChange> m_Changes;

		public ComponentLookup<FloodCounterData> m_FloodCounterDatas;

		[ReadOnly]
		public ComponentLookup<EventData> m_Events;

		[ReadOnly]
		public NativeList<Entity> m_FloodEntities;

		[ReadOnly]
		public NativeList<Entity> m_FloodPrefabEntities;

		public EntityCommandBuffer m_CommandBuffer;

		public Entity m_FloodCounterEntity;

		public float m_Weather;

		public int m_ShaderUpdatesPerSoilUpdate;

		public int m_LoadDistributionIndex;

		private void HandleInterface(int index, int otherIndex, NativeArray<int> tmp, ref SoilWaterParameterData soilWaterParameters)
		{
			SoilWater soilWater = m_SoilWaterMap[index];
			SoilWater soilWater2 = m_SoilWaterMap[otherIndex];
			int num = tmp[index];
			int num2 = tmp[otherIndex];
			float num3 = soilWater2.m_Surface - soilWater.m_Surface;
			float num4 = (float)soilWater2.m_Amount / (float)soilWater2.m_Max - (float)soilWater.m_Amount / (float)soilWater.m_Max;
			float num5 = soilWaterParameters.m_HeightEffect * num3 / (float)(CellMapSystem<SoilWater>.kMapSize / kTextureSize) + 0.25f * num4;
			num5 = ((!(num5 >= 0f)) ? math.max(0f - soilWaterParameters.m_MaxDiffusion, num5) : math.min(soilWaterParameters.m_MaxDiffusion, num5));
			int num6 = Mathf.RoundToInt(num5 * (float)((num5 > 0f) ? soilWater2.m_Amount : soilWater.m_Amount));
			num += num6;
			num2 -= num6;
			tmp[index] = num;
			tmp[otherIndex] = num2;
		}

		private void StartFlood()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			if (m_FloodPrefabEntities.Length > 0)
			{
				EntityArchetype archetype = m_Events[m_FloodPrefabEntities[0]].m_Archetype;
				Entity val = ((EntityCommandBuffer)(ref m_CommandBuffer)).CreateEntity(archetype);
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<PrefabRef>(val, new PrefabRef
				{
					m_Prefab = m_FloodPrefabEntities[0]
				});
				((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<WaterLevelChange>(val, new WaterLevelChange
				{
					m_DangerHeight = 0f,
					m_Direction = new float2(0f, 0f),
					m_Intensity = 0f,
					m_MaxIntensity = 0f
				});
			}
		}

		private void StopFlood()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Deleted>(m_FloodEntities[0]);
		}

		public void Execute()
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0255: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_037c: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<int> tmp = default(NativeArray<int>);
			tmp._002Ector(m_SoilWaterMap.Length, (Allocator)2, (NativeArrayOptions)1);
			for (int i = 0; i < m_SoilWaterMap.Length; i++)
			{
				int num = i % kTextureSize;
				int num2 = i / kTextureSize;
				if (num < kTextureSize - 1)
				{
					HandleInterface(i, i + 1, tmp, ref m_SoilWaterParameters);
				}
				if (num2 < kTextureSize - 1)
				{
					HandleInterface(i, i + kTextureSize, tmp, ref m_SoilWaterParameters);
				}
			}
			float num3 = math.max(0f, math.pow(2f * math.max(0f, m_Weather - 0.5f), 2f));
			float num4 = 1f / (2f * m_SoilWaterParameters.m_MaximumWaterDepth);
			int3 resolution = m_WaterSurfaceData.resolution;
			int2 val = ((int3)(ref resolution)).xz / kTextureSize;
			FloodCounterData floodCounterData = m_FloodCounterDatas[m_FloodCounterEntity];
			floodCounterData.m_FloodCounter = math.max(0f, 0.98f * floodCounterData.m_FloodCounter + 2f * num3 - 0.1f);
			if (floodCounterData.m_FloodCounter > 20f && m_FloodEntities.Length == 0)
			{
				StartFlood();
			}
			else if (m_FloodEntities.Length > 0)
			{
				if (floodCounterData.m_FloodCounter == 0f)
				{
					StopFlood();
				}
				else
				{
					WaterLevelChange waterLevelChange = m_Changes[m_FloodEntities[0]];
					waterLevelChange.m_Intensity = math.max(0f, (floodCounterData.m_FloodCounter - 20f) / 80f);
					m_Changes[m_FloodEntities[0]] = waterLevelChange;
				}
			}
			m_FloodCounterDatas[m_FloodCounterEntity] = floodCounterData;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = m_LoadDistributionIndex * kTextureSize / kLoadDistribution;
			int num9 = num8 + kTextureSize / kLoadDistribution;
			for (int j = num8 * kTextureSize; j < num9 * kTextureSize; j++)
			{
				SoilWater soilWater = m_SoilWaterMap[j];
				soilWater.m_Amount = (short)math.max(0, soilWater.m_Amount + tmp[j] + Mathf.RoundToInt(m_SoilWaterParameters.m_RainMultiplier * num3));
				float surface = TerrainUtils.SampleHeight(ref m_TerrainHeightData, GetCellCenter(j));
				soilWater.m_Surface = surface;
				short num10 = (short)Mathf.RoundToInt(math.max(0f, 0.1f * (0.5f * (float)soilWater.m_Max - (float)soilWater.m_Amount)));
				float num11 = (float)num10 * m_SoilWaterParameters.m_WaterPerUnit / (float)soilWater.m_Max;
				int num12 = 0;
				int num13 = 0;
				float num14 = 0f;
				float num15 = 0f;
				int num16 = j % kTextureSize * val.x + j / kTextureSize * m_WaterSurfaceData.resolution.x * val.y;
				for (int k = 0; k < val.x; k += 4)
				{
					for (int l = 0; l < val.y; l += 4)
					{
						float depth = m_WaterSurfaceData.depths[num16 + k + l * m_WaterSurfaceData.resolution.z].m_Depth;
						if (depth > 0.01f)
						{
							num12++;
							num14 += math.min(m_SoilWaterParameters.m_MaximumWaterDepth, depth);
							num15 += math.min(num11, depth);
						}
						num13++;
					}
				}
				num10 = (short)Math.Min(num10, Mathf.RoundToInt((float)soilWater.m_Max * 10f * num15));
				num11 = (float)num10 * m_SoilWaterParameters.m_WaterPerUnit / (float)soilWater.m_Max;
				float num17 = (1f - num4 * num14 / (float)num13) * (float)soilWater.m_Max;
				short num18 = (short)Mathf.RoundToInt(math.max(0f, m_SoilWaterParameters.m_OverflowRate * ((float)soilWater.m_Amount - num17)));
				float num19 = 0f;
				if ((float)num18 > 0f)
				{
					num19 = (float)soilWater.m_Amount / (float)soilWater.m_Max;
					num11 = 0f;
				}
				if (num12 == 0)
				{
					num11 = 0f;
				}
				soilWater.m_Amount += num10;
				soilWater.m_Amount -= num18;
				short num20 = (short)Mathf.RoundToInt((float)math.sign(soilWater.m_Max / 8 - soilWater.m_Amount));
				soilWater.m_Amount += num20;
				num6 += num10 + Math.Max((short)0, num20);
				num5 += num18 + Math.Max(0, -num20);
				num7 += soilWater.m_Amount;
				m_SoilWaterTextureData[j] = (0f - num11) / (float)m_ShaderUpdatesPerSoilUpdate + num19;
				m_SoilWaterMap[j] = soilWater;
			}
			tmp.Dispose();
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<WaterLevelChange> __Game_Events_WaterLevelChange_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EventData> __Game_Prefabs_EventData_RO_ComponentLookup;

		public ComponentLookup<FloodCounterData> __Game_Simulation_FloodCounterData_RW_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_WaterLevelChange_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterLevelChange>(false);
			__Game_Prefabs_EventData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EventData>(true);
			__Game_Simulation_FloodCounterData_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<FloodCounterData>(false);
		}
	}

	public static readonly int kTextureSize = 128;

	public static readonly int kUpdatesPerDay = 1024;

	public static readonly int kLoadDistribution = 8;

	private SimulationSystem m_SimulationSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private ClimateSystem m_ClimateSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private Texture2D m_SoilWaterTexture;

	private EntityQuery m_SoilWaterParameterQuery;

	private EntityQuery m_FloodQuery;

	private EntityQuery m_FloodPrefabQuery;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_336595330_0;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public Texture soilTexture => (Texture)(object)m_SoilWaterTexture;

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<SoilWater>.GetCellCenter(index, kTextureSize);
	}

	public static SoilWater GetSoilWater(float3 position, NativeArray<SoilWater> soilWaterMap)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		SoilWater result = default(SoilWater);
		int2 cell = CellMapSystem<SoilWater>.GetCell(position, CellMapSystem<SoilWater>.kMapSize, kTextureSize);
		float2 cellCoords = CellMapSystem<SoilWater>.GetCellCoords(position, CellMapSystem<SoilWater>.kMapSize, kTextureSize);
		if (cell.x < 0 || cell.x >= kTextureSize || cell.y < 0 || cell.y >= kTextureSize)
		{
			return result;
		}
		float num = soilWaterMap[cell.x + kTextureSize * cell.y].m_Amount;
		float num2 = ((cell.x < kTextureSize - 1) ? soilWaterMap[cell.x + 1 + kTextureSize * cell.y].m_Amount : 0);
		float num3 = ((cell.y < kTextureSize - 1) ? soilWaterMap[cell.x + kTextureSize * (cell.y + 1)].m_Amount : 0);
		float num4 = ((cell.x < kTextureSize - 1 && cell.y < kTextureSize - 1) ? soilWaterMap[cell.x + 1 + kTextureSize * (cell.y + 1)].m_Amount : 0);
		result.m_Amount = (short)Mathf.RoundToInt(math.lerp(math.lerp(num, num2, cellCoords.x - (float)cell.x), math.lerp(num3, num4, cellCoords.x - (float)cell.x), cellCoords.y - (float)cell.y));
		return result;
	}

	private void CreateFloodCounter()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).CreateEntity(((EntityManager)(ref entityManager2)).CreateArchetype((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<FloodCounterData>() }));
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SoilWaterParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SoilWaterParameterData>() });
		m_FloodQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Flood>() });
		m_FloodPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FloodData>() });
		CreateFloodCounter();
		CreateTextures(kTextureSize);
		m_SoilWaterTexture = new Texture2D(kTextureSize, kTextureSize, (TextureFormat)18, false, true)
		{
			name = "SoilWaterTexture",
			hideFlags = (HideFlags)61
		};
		NativeArray<float> rawTextureData = m_SoilWaterTexture.GetRawTextureData<float>();
		for (int i = 0; i < m_Map.Length; i++)
		{
			_ = (float)(i % kTextureSize) / (float)kTextureSize;
			_ = (float)(i / kTextureSize) / (float)kTextureSize;
			SoilWater soilWater = new SoilWater
			{
				m_Amount = 1024,
				m_Max = 8192
			};
			m_Map[i] = soilWater;
			rawTextureData[i] = 0f;
		}
		m_SoilWaterTexture.Apply();
		((ComponentSystemBase)this).RequireForUpdate(m_SoilWaterParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightData heightData = m_TerrainSystem.GetHeightData();
		if (heightData.isCreated)
		{
			m_SoilWaterTexture.Apply();
			float value = m_ClimateSystem.precipitation.value;
			int shaderUpdatesPerSoilUpdate = 262144 / (kUpdatesPerDay / kLoadDistribution) / m_WaterSystem.SimulationCycleSteps;
			int loadDistributionIndex = (int)(m_SimulationSystem.frameIndex / (262144 / kUpdatesPerDay) % kLoadDistribution);
			JobHandle deps;
			JobHandle val = default(JobHandle);
			JobHandle val2 = default(JobHandle);
			SoilWaterTickJob soilWaterTickJob = new SoilWaterTickJob
			{
				m_SoilWaterMap = m_Map,
				m_TerrainHeightData = heightData,
				m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
				m_SoilWaterTextureData = m_SoilWaterTexture.GetRawTextureData<float>(),
				m_SoilWaterParameters = ((EntityQuery)(ref m_SoilWaterParameterQuery)).GetSingleton<SoilWaterParameterData>(),
				m_FloodEntities = ((EntityQuery)(ref m_FloodQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_FloodPrefabEntities = ((EntityQuery)(ref m_FloodPrefabQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
				m_Changes = InternalCompilerInterface.GetComponentLookup<WaterLevelChange>(ref __TypeHandle.__Game_Events_WaterLevelChange_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Events = InternalCompilerInterface.GetComponentLookup<EventData>(ref __TypeHandle.__Game_Prefabs_EventData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_FloodCounterDatas = InternalCompilerInterface.GetComponentLookup<FloodCounterData>(ref __TypeHandle.__Game_Simulation_FloodCounterData_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
				m_FloodCounterEntity = ((EntityQuery)(ref __query_336595330_0)).GetSingletonEntity(),
				m_Weather = value,
				m_ShaderUpdatesPerSoilUpdate = shaderUpdatesPerSoilUpdate,
				m_LoadDistributionIndex = loadDistributionIndex
			};
			((SystemBase)this).Dependency = IJobExtensions.Schedule<SoilWaterTickJob>(soilWaterTickJob, JobUtils.CombineDependencies(m_WriteDependencies, m_ReadDependencies, val, val2, deps, ((SystemBase)this).Dependency));
			AddWriter(((SystemBase)this).Dependency);
			m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
			m_TerrainSystem.AddCPUHeightReader(((SystemBase)this).Dependency);
			m_WaterSystem.AddSurfaceReader(((SystemBase)this).Dependency);
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies, ((SystemBase)this).Dependency);
		}
	}

	public void PostDeserialize(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<FloodCounterData>() });
		try
		{
			if (((EntityQuery)(ref val)).CalculateEntityCount() == 0)
			{
				CreateFloodCounter();
			}
		}
		finally
		{
			((EntityQuery)(ref val)).Dispose();
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
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<FloodCounterData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_336595330_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
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
	public SoilWaterSystem()
	{
	}
}
