using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Net;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class AvailabilityInfoToGridSystem : CellMapSystem<AvailabilityInfoCell>, IJobSerializable
{
	private struct NetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public AvailabilityInfoCell m_TotalWeight;

		public AvailabilityInfoCell m_Result;

		public float m_CellSize;

		public Bounds3 m_Bounds;

		public BufferLookup<ResourceAvailability> m_Availabilities;

		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
		}

		private void AddData(float2 attractiveness2, float2 uneducated2, float2 educated2, float2 services2, float2 workplaces2, float2 t, float3 curvePos, float weight)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			float num = math.lerp(attractiveness2.x, attractiveness2.y, t.y);
			float num2 = 0.5f * math.lerp(uneducated2.x + educated2.x, uneducated2.y + educated2.y, t.y);
			float num3 = math.lerp(services2.x, services2.y, t.y);
			float num4 = math.lerp(workplaces2.x, workplaces2.y, t.y);
			m_Result.AddAttractiveness(weight * num);
			m_TotalWeight.AddAttractiveness(weight);
			m_Result.AddConsumers(weight * num2);
			m_TotalWeight.AddConsumers(weight);
			m_Result.AddServices(weight * num3);
			m_TotalWeight.AddServices(weight);
			m_Result.AddWorkplaces(weight * num4);
			m_TotalWeight.AddWorkplaces(weight);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0244: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			if (MathUtils.Intersect(bounds.m_Bounds, m_Bounds) && m_Availabilities.HasBuffer(entity) && m_EdgeGeometryData.HasComponent(entity))
			{
				DynamicBuffer<ResourceAvailability> val = m_Availabilities[entity];
				float2 availability = val[18].m_Availability;
				float2 availability2 = val[2].m_Availability;
				float2 availability3 = val[3].m_Availability;
				float2 availability4 = val[1].m_Availability;
				float2 availability5 = val[0].m_Availability;
				EdgeGeometry edgeGeometry = m_EdgeGeometryData[entity];
				int num = (int)math.ceil(edgeGeometry.m_Start.middleLength * 0.05f);
				int num2 = (int)math.ceil(edgeGeometry.m_End.middleLength * 0.05f);
				float3 val2 = 0.5f * (m_Bounds.min + m_Bounds.max);
				for (int i = 1; i <= num; i++)
				{
					float2 val3 = (float)i / new float2((float)num, (float)(num + num2));
					float3 curvePos = math.lerp(MathUtils.Position(edgeGeometry.m_Start.m_Left, val3.x), MathUtils.Position(edgeGeometry.m_Start.m_Right, val3.x), 0.5f);
					float weight = math.max(0f, 1f - math.distance(((float3)(ref val2)).xz, ((float3)(ref curvePos)).xz) / (1.5f * m_CellSize));
					AddData(availability, availability2, availability3, availability4, availability5, val3, curvePos, weight);
				}
				for (int j = 1; j <= num2; j++)
				{
					float2 val4 = new float2((float)j, (float)(num + j)) / new float2((float)num2, (float)(num + num2));
					float3 curvePos2 = math.lerp(MathUtils.Position(edgeGeometry.m_End.m_Left, val4.x), MathUtils.Position(edgeGeometry.m_End.m_Right, val4.x), 0.5f);
					float weight2 = math.max(0f, 1f - math.distance(((float3)(ref val2)).xz, ((float3)(ref curvePos2)).xz) / (1.5f * m_CellSize));
					AddData(availability, availability2, availability3, availability4, availability5, val4, curvePos2, weight2);
				}
			}
		}
	}

	[BurstCompile]
	private struct AvailabilityInfoToGridJob : IJobParallelFor
	{
		public NativeArray<AvailabilityInfoCell> m_AvailabilityInfoMap;

		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> m_AvailabilityData;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> m_EdgeGeometryData;

		public float m_CellSize;

		public void Execute(int index)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			float3 cellCenter = CellMapSystem<AvailabilityInfoCell>.GetCellCenter(index, kTextureSize);
			NetIterator netIterator = new NetIterator
			{
				m_TotalWeight = default(AvailabilityInfoCell),
				m_Result = default(AvailabilityInfoCell),
				m_Bounds = new Bounds3(cellCenter - new float3(1.5f * m_CellSize, 10000f, 1.5f * m_CellSize), cellCenter + new float3(1.5f * m_CellSize, 10000f, 1.5f * m_CellSize)),
				m_CellSize = m_CellSize,
				m_EdgeGeometryData = m_EdgeGeometryData,
				m_Availabilities = m_AvailabilityData
			};
			m_NetSearchTree.Iterate<NetIterator>(ref netIterator, 0);
			AvailabilityInfoCell availabilityInfoCell = m_AvailabilityInfoMap[index];
			availabilityInfoCell.m_AvailabilityInfo = math.select(netIterator.m_Result.m_AvailabilityInfo / netIterator.m_TotalWeight.m_AvailabilityInfo, float4.op_Implicit(0f), netIterator.m_TotalWeight.m_AvailabilityInfo == 0f);
			m_AvailabilityInfoMap[index] = availabilityInfoCell;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
		}
	}

	public static readonly int kTextureSize = 128;

	public static readonly int kUpdatesPerDay = 32;

	private SearchSystem m_NetSearchSystem;

	private TypeHandle __TypeHandle;

	public int2 TextureSize => new int2(kTextureSize, kTextureSize);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / kUpdatesPerDay;
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CellMapSystem<AvailabilityInfoCell>.GetCellCenter(index, kTextureSize);
	}

	public static AvailabilityInfoCell GetAvailabilityInfo(float3 position, NativeArray<AvailabilityInfoCell> AvailabilityInfoMap)
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
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		AvailabilityInfoCell result = default(AvailabilityInfoCell);
		int2 cell = CellMapSystem<AvailabilityInfoCell>.GetCell(position, CellMapSystem<AvailabilityInfoCell>.kMapSize, kTextureSize);
		float2 cellCoords = CellMapSystem<AvailabilityInfoCell>.GetCellCoords(position, CellMapSystem<AvailabilityInfoCell>.kMapSize, kTextureSize);
		if (cell.x < 0 || cell.x >= kTextureSize || cell.y < 0 || cell.y >= kTextureSize)
		{
			return default(AvailabilityInfoCell);
		}
		float4 availabilityInfo = AvailabilityInfoMap[cell.x + kTextureSize * cell.y].m_AvailabilityInfo;
		float4 val = ((cell.x < kTextureSize - 1) ? AvailabilityInfoMap[cell.x + 1 + kTextureSize * cell.y].m_AvailabilityInfo : float4.op_Implicit(0));
		float4 val2 = ((cell.y < kTextureSize - 1) ? AvailabilityInfoMap[cell.x + kTextureSize * (cell.y + 1)].m_AvailabilityInfo : float4.op_Implicit(0));
		float4 val3 = ((cell.x < kTextureSize - 1 && cell.y < kTextureSize - 1) ? AvailabilityInfoMap[cell.x + 1 + kTextureSize * (cell.y + 1)].m_AvailabilityInfo : float4.op_Implicit(0));
		result.m_AvailabilityInfo = math.lerp(math.lerp(availabilityInfo, val, cellCoords.x - (float)cell.x), math.lerp(val2, val3, cellCoords.x - (float)cell.x), cellCoords.y - (float)cell.y);
		return result;
	}

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		CreateTextures(kTextureSize);
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		AvailabilityInfoToGridJob availabilityInfoToGridJob = new AvailabilityInfoToGridJob
		{
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_AvailabilityInfoMap = m_Map,
			m_AvailabilityData = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CellSize = (float)CellMapSystem<AvailabilityInfoCell>.kMapSize / (float)kTextureSize
		};
		((SystemBase)this).Dependency = IJobParallelForExtensions.Schedule<AvailabilityInfoToGridJob>(availabilityInfoToGridJob, kTextureSize * kTextureSize, kTextureSize, JobHandle.CombineDependencies(dependencies, JobHandle.CombineDependencies(m_WriteDependencies, m_ReadDependencies, ((SystemBase)this).Dependency)));
		AddWriter(((SystemBase)this).Dependency);
		m_NetSearchSystem.AddNetSearchTreeReader(((SystemBase)this).Dependency);
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
	public AvailabilityInfoToGridSystem()
	{
	}
}
