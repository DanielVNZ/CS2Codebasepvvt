using System;
using Colossal.Serialization.Entities;
using Game.Rendering;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

public class WindSystem : CellMapSystem<Wind>, IJobSerializable
{
	[BurstCompile]
	private struct WindCopyJob : IJobFor
	{
		public NativeArray<Wind> m_WindMap;

		[ReadOnly]
		public NativeArray<WindSimulationSystem.WindCell> m_Source;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public void Execute(int index)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			float3 cellCenter = WindSimulationSystem.GetCellCenter(index);
			cellCenter.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cellCenter) + 25f;
			float num = math.max(0f, (float)WindSimulationSystem.kResolution.z * (cellCenter.y - TerrainUtils.ToWorldSpace(ref m_TerrainHeightData, 0f)) / TerrainUtils.ToWorldSpace(ref m_TerrainHeightData, 65535f) - 0.5f);
			int3 val = default(int3);
			((int3)(ref val))._002Ector(index % kTextureSize, index / kTextureSize, Math.Min(Mathf.FloorToInt(num), WindSimulationSystem.kResolution.z - 1));
			int3 cell = new int3(val.x, val.y, Math.Min(val.z + 1, WindSimulationSystem.kResolution.z - 1));
			float3 centerVelocity = WindSimulationSystem.GetCenterVelocity(val, m_Source);
			float2 xy = ((float3)(ref centerVelocity)).xy;
			centerVelocity = WindSimulationSystem.GetCenterVelocity(cell, m_Source);
			float2 xy2 = ((float3)(ref centerVelocity)).xy;
			float2 wind = math.lerp(xy, xy2, math.frac(num));
			m_WindMap[index] = new Wind
			{
				m_Wind = wind
			};
		}
	}

	public static readonly int kTextureSize = 64;

	public static readonly int kUpdateInterval = 512;

	public WindSimulationSystem m_WindSimulationSystem;

	public WindTextureSystem m_WindTextureSystem;

	public TerrainSystem m_TerrainSystem;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		if (phase != SystemUpdatePhase.GameSimulation)
		{
			return 1;
		}
		return kUpdateInterval;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<Wind>.GetCellCenter(index, kTextureSize);
	}

	public static Wind GetWind(float3 position, NativeArray<Wind> windMap)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		int2 cell = CellMapSystem<Wind>.GetCell(position, CellMapSystem<Wind>.kMapSize, kTextureSize);
		cell = math.clamp(cell, int2.op_Implicit(0), int2.op_Implicit(kTextureSize - 1));
		float2 cellCoords = CellMapSystem<Wind>.GetCellCoords(position, CellMapSystem<Wind>.kMapSize, kTextureSize);
		int num = math.min(kTextureSize - 1, cell.x + 1);
		int num2 = math.min(kTextureSize - 1, cell.y + 1);
		return new Wind
		{
			m_Wind = math.lerp(math.lerp(windMap[cell.x + kTextureSize * cell.y].m_Wind, windMap[num + kTextureSize * cell.y].m_Wind, cellCoords.x - (float)cell.x), math.lerp(windMap[cell.x + kTextureSize * num2].m_Wind, windMap[num + kTextureSize * num2].m_Wind, cellCoords.x - (float)cell.x), cellCoords.y - (float)cell.y)
		};
	}

	public override JobHandle Deserialize<TReader>(EntityReaderData readerData, JobHandle inputDeps)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		m_WindTextureSystem.RequireUpdate();
		Context context = ((IReader)((EntityReaderData)(ref readerData)).GetReader<TReader>()).context;
		if (((Context)(ref context)).version > Version.cellMapLengths)
		{
			return base.Deserialize<TReader>(readerData, inputDeps);
		}
		m_Map.Dispose();
		m_Map = new NativeArray<Wind>(65536, (Allocator)4, (NativeArrayOptions)1);
		inputDeps = base.Deserialize<TReader>(readerData, inputDeps);
		((JobHandle)(ref inputDeps)).Complete();
		m_Map.Dispose();
		m_Map = new NativeArray<Wind>(kTextureSize * kTextureSize, (Allocator)4, (NativeArrayOptions)1);
		return inputDeps;
	}

	public override JobHandle SetDefaults(Context context)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		m_WindTextureSystem.RequireUpdate();
		for (int i = 0; i < m_Map.Length; i++)
		{
			m_Map[i] = new Wind
			{
				m_Wind = m_WindSimulationSystem.constantWind
			};
		}
		return default(JobHandle);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_WindSimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSimulationSystem>();
		m_WindTextureSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindTextureSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		CreateTextures(kTextureSize);
		for (int i = 0; i < m_Map.Length; i++)
		{
			m_Map[i] = new Wind
			{
				m_Wind = m_WindSimulationSystem.constantWind
			};
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightData heightData = m_TerrainSystem.GetHeightData();
		if (heightData.isCreated)
		{
			JobHandle deps;
			WindCopyJob windCopyJob = new WindCopyJob
			{
				m_WindMap = m_Map,
				m_Source = m_WindSimulationSystem.GetCells(out deps),
				m_TerrainHeightData = heightData
			};
			((SystemBase)this).Dependency = IJobForExtensions.Schedule<WindCopyJob>(windCopyJob, m_Map.Length, JobHandle.CombineDependencies(deps, JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies, ((SystemBase)this).Dependency)));
			AddWriter(((SystemBase)this).Dependency);
			m_TerrainSystem.AddCPUHeightReader(((SystemBase)this).Dependency);
			m_WindSimulationSystem.AddReader(((SystemBase)this).Dependency);
			m_WindTextureSystem.RequireUpdate();
		}
	}

	[Preserve]
	public WindSystem()
	{
	}
}
