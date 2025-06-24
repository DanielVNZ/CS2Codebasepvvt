using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TerrainAttractivenessSystem : CellMapSystem<TerrainAttractiveness>, IJobSerializable
{
	[BurstCompile]
	private struct TerrainAttractivenessPrepareJob : IJobParallelForBatch
	{
		[ReadOnly]
		public TerrainHeightData m_TerrainData;

		[ReadOnly]
		public WaterSurfaceData m_WaterData;

		[ReadOnly]
		public CellMapData<ZoneAmbienceCell> m_ZoneAmbienceData;

		public NativeArray<float3> m_AttractFactorData;

		public void Execute(int startIndex, int count)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			for (int i = startIndex; i < startIndex + count; i++)
			{
				float3 cellCenter = GetCellCenter(i);
				m_AttractFactorData[i] = new float3(WaterUtils.SampleDepth(ref m_WaterData, cellCenter), TerrainUtils.SampleHeight(ref m_TerrainData, cellCenter), ZoneAmbienceSystem.GetZoneAmbience(GroupAmbienceType.Forest, cellCenter, m_ZoneAmbienceData.m_Buffer, 1f));
			}
		}
	}

	[BurstCompile]
	private struct TerrainAttractivenessJob : IJobParallelForBatch
	{
		[ReadOnly]
		public NativeArray<float3> m_AttractFactorData;

		[ReadOnly]
		public float m_Scale;

		public NativeArray<TerrainAttractiveness> m_AttractivenessMap;

		public AttractivenessParameterData m_AttractivenessParameters;

		public void Execute(int startIndex, int count)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			for (int i = startIndex; i < startIndex + count; i++)
			{
				float3 cellCenter = GetCellCenter(i);
				float2 val = float2.op_Implicit(0);
				int num = Mathf.CeilToInt(math.max(m_AttractivenessParameters.m_ForestDistance, m_AttractivenessParameters.m_ShoreDistance) / m_Scale);
				for (int j = -num; j <= num; j++)
				{
					for (int k = -num; k <= num; k++)
					{
						int num2 = math.min(kTextureSize - 1, math.max(0, i % kTextureSize + j));
						int num3 = math.min(kTextureSize - 1, math.max(0, i / kTextureSize + k));
						int num4 = num2 + num3 * kTextureSize;
						float3 val2 = m_AttractFactorData[num4];
						float num5 = math.distance(GetCellCenter(num4), cellCenter);
						val.x = math.max(val.x, math.saturate(1f - num5 / m_AttractivenessParameters.m_ForestDistance) * val2.z);
						val.y = math.max(val.y, math.saturate(1f - num5 / m_AttractivenessParameters.m_ShoreDistance) * ((val2.x > 2f) ? 1f : 0f));
					}
				}
				m_AttractivenessMap[i] = new TerrainAttractiveness
				{
					m_ForestBonus = val.x,
					m_ShoreBonus = val.y
				};
			}
		}
	}

	public static readonly int kTextureSize = 128;

	public static readonly int kUpdatesPerDay = 16;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private ZoneAmbienceSystem m_ZoneAmbienceSystem;

	private EntityQuery m_AttractivenessParameterGroup;

	private NativeArray<float3> m_AttractFactorData;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<TerrainAttractiveness>.GetCellCenter(index, kTextureSize);
	}

	public static float EvaluateAttractiveness(float terrainHeight, TerrainAttractiveness attractiveness, AttractivenessParameterData parameters)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		float num = parameters.m_ForestEffect * attractiveness.m_ForestBonus;
		float num2 = parameters.m_ShoreEffect * attractiveness.m_ShoreBonus;
		float num3 = math.min(parameters.m_HeightBonus.z, math.max(0f, terrainHeight - parameters.m_HeightBonus.x) * parameters.m_HeightBonus.y);
		return num + num2 + num3;
	}

	public static float EvaluateAttractiveness(float3 position, CellMapData<TerrainAttractiveness> data, TerrainHeightData heightData, AttractivenessParameterData parameters, NativeArray<int> factors)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainUtils.SampleHeight(ref heightData, position);
		TerrainAttractiveness attractiveness = GetAttractiveness(position, data.m_Buffer);
		float num2 = parameters.m_ForestEffect * attractiveness.m_ForestBonus;
		AttractionSystem.SetFactor(factors, AttractionSystem.AttractivenessFactor.Forest, num2);
		float num3 = parameters.m_ShoreEffect * attractiveness.m_ShoreBonus;
		AttractionSystem.SetFactor(factors, AttractionSystem.AttractivenessFactor.Beach, num3);
		float num4 = math.min(parameters.m_HeightBonus.z, math.max(0f, num - parameters.m_HeightBonus.x) * parameters.m_HeightBonus.y);
		AttractionSystem.SetFactor(factors, AttractionSystem.AttractivenessFactor.Height, num4);
		return num2 + num3 + num4;
	}

	public static TerrainAttractiveness GetAttractiveness(float3 position, NativeArray<TerrainAttractiveness> attractivenessMap)
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
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		TerrainAttractiveness result = default(TerrainAttractiveness);
		int2 cell = CellMapSystem<TerrainAttractiveness>.GetCell(position, CellMapSystem<TerrainAttractiveness>.kMapSize, kTextureSize);
		float2 cellCoords = CellMapSystem<TerrainAttractiveness>.GetCellCoords(position, CellMapSystem<TerrainAttractiveness>.kMapSize, kTextureSize);
		if (cell.x < 0 || cell.x >= kTextureSize || cell.y < 0 || cell.y >= kTextureSize)
		{
			return result;
		}
		TerrainAttractiveness terrainAttractiveness = attractivenessMap[cell.x + kTextureSize * cell.y];
		TerrainAttractiveness terrainAttractiveness2 = ((cell.x < kTextureSize - 1) ? attractivenessMap[cell.x + 1 + kTextureSize * cell.y] : default(TerrainAttractiveness));
		TerrainAttractiveness terrainAttractiveness3 = ((cell.y < kTextureSize - 1) ? attractivenessMap[cell.x + kTextureSize * (cell.y + 1)] : default(TerrainAttractiveness));
		TerrainAttractiveness terrainAttractiveness4 = ((cell.x < kTextureSize - 1 && cell.y < kTextureSize - 1) ? attractivenessMap[cell.x + 1 + kTextureSize * (cell.y + 1)] : default(TerrainAttractiveness));
		result.m_ForestBonus = (short)Mathf.RoundToInt(math.lerp(math.lerp(terrainAttractiveness.m_ForestBonus, terrainAttractiveness2.m_ForestBonus, cellCoords.x - (float)cell.x), math.lerp(terrainAttractiveness3.m_ForestBonus, terrainAttractiveness4.m_ForestBonus, cellCoords.x - (float)cell.x), cellCoords.y - (float)cell.y));
		result.m_ShoreBonus = (short)Mathf.RoundToInt(math.lerp(math.lerp(terrainAttractiveness.m_ShoreBonus, terrainAttractiveness2.m_ShoreBonus, cellCoords.x - (float)cell.x), math.lerp(terrainAttractiveness3.m_ShoreBonus, terrainAttractiveness4.m_ShoreBonus, cellCoords.x - (float)cell.x), cellCoords.y - (float)cell.y));
		return result;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		CreateTextures(kTextureSize);
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_ZoneAmbienceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneAmbienceSystem>();
		m_AttractivenessParameterGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AttractivenessParameterData>() });
		m_AttractFactorData = new NativeArray<float3>(m_Map.Length, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_AttractFactorData.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightData heightData = m_TerrainSystem.GetHeightData();
		JobHandle deps;
		JobHandle dependencies;
		TerrainAttractivenessPrepareJob obj = new TerrainAttractivenessPrepareJob
		{
			m_AttractFactorData = m_AttractFactorData,
			m_TerrainData = heightData,
			m_WaterData = m_WaterSystem.GetSurfaceData(out deps),
			m_ZoneAmbienceData = m_ZoneAmbienceSystem.GetData(readOnly: true, out dependencies)
		};
		TerrainAttractivenessJob terrainAttractivenessJob = new TerrainAttractivenessJob
		{
			m_Scale = heightData.scale.x * (float)kTextureSize,
			m_AttractFactorData = m_AttractFactorData,
			m_AttractivenessMap = m_Map,
			m_AttractivenessParameters = ((EntityQuery)(ref m_AttractivenessParameterGroup)).GetSingleton<AttractivenessParameterData>()
		};
		JobHandle val = IJobParallelForBatchExtensions.ScheduleBatch<TerrainAttractivenessPrepareJob>(obj, m_Map.Length, 4, JobHandle.CombineDependencies(deps, dependencies, ((SystemBase)this).Dependency));
		m_TerrainSystem.AddCPUHeightReader(val);
		m_ZoneAmbienceSystem.AddReader(val);
		m_WaterSystem.AddSurfaceReader(val);
		((SystemBase)this).Dependency = IJobParallelForBatchExtensions.ScheduleBatch<TerrainAttractivenessJob>(terrainAttractivenessJob, m_Map.Length, 4, JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies, val));
		AddWriter(((SystemBase)this).Dependency);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies, ((SystemBase)this).Dependency);
	}

	[Preserve]
	public TerrainAttractivenessSystem()
	{
	}
}
