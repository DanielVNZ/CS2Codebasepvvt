using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.UI.Localization;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Tooltip;

[CompilerGenerated]
public class TempWaterPumpingTooltipSystem : TooltipSystemBase
{
	private struct TempResult
	{
		public AllowedWaterTypes m_Types;

		public int m_Production;

		public int m_MaxCapacity;
	}

	[BurstCompile]
	private struct TempJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> m_SubObjectType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<WaterPumpingStationData> m_PumpDatas;

		[ReadOnly]
		public ComponentLookup<Transform> m_Transforms;

		[ReadOnly]
		public ComponentLookup<Game.Simulation.WaterSourceData> m_WaterSources;

		[ReadOnly]
		public NativeArray<GroundWater> m_GroundWaterMap;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		public NativeReference<TempResult> m_Result;

		public WaterPipeParameterData m_Parameters;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			ref TempResult reference = ref CollectionUtils.ValueAsRef<TempResult>(m_Result);
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<Game.Objects.SubObject> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Objects.SubObject>(ref m_SubObjectType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			WaterPumpingStationData data = default(WaterPumpingStationData);
			Transform transform = default(Transform);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				if ((nativeArray2[i].m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Upgrade)) == 0)
				{
					continue;
				}
				m_PumpDatas.TryGetComponent(nativeArray[i].m_Prefab, ref data);
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<WaterPumpingStationData>(ref data, bufferAccessor2[i], ref m_Prefabs, ref m_PumpDatas);
				}
				int num = 0;
				if (data.m_Types != AllowedWaterTypes.None)
				{
					if ((data.m_Types & AllowedWaterTypes.Groundwater) != AllowedWaterTypes.None)
					{
						int num2 = Mathf.RoundToInt(math.clamp((float)GroundWaterSystem.GetGroundWater(nativeArray3[i].m_Position, m_GroundWaterMap).m_Max / m_Parameters.m_GroundwaterPumpEffectiveAmount, 0f, 1f) * (float)data.m_Capacity);
						num += num2;
					}
					if ((data.m_Types & AllowedWaterTypes.SurfaceWater) != AllowedWaterTypes.None && bufferAccessor.Length != 0)
					{
						DynamicBuffer<Game.Objects.SubObject> val = bufferAccessor[i];
						for (int j = 0; j < val.Length; j++)
						{
							Entity subObject = val[j].m_SubObject;
							if (m_WaterSources.HasComponent(subObject) && m_Transforms.TryGetComponent(subObject, ref transform))
							{
								float surfaceWaterAvailability = WaterPumpingStationAISystem.GetSurfaceWaterAvailability(transform.m_Position, data.m_Types, m_WaterSurfaceData, m_Parameters.m_SurfaceWaterPumpEffectiveDepth);
								num += Mathf.RoundToInt(surfaceWaterAvailability * (float)data.m_Capacity);
							}
						}
					}
				}
				else
				{
					num = data.m_Capacity;
				}
				reference.m_Types |= data.m_Types;
				reference.m_Production += math.min(num, data.m_Capacity);
				reference.m_MaxCapacity += data.m_Capacity;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct GroundWaterPumpJob : IJobChunk
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_Prefabs;

		[ReadOnly]
		public ComponentLookup<WaterPumpingStationData> m_PumpDatas;

		[ReadOnly]
		public NativeArray<GroundWater> m_GroundWaterMap;

		public NativeParallelHashMap<int2, int> m_PumpCapacityMap;

		public NativeList<int2> m_TempGroundWaterPumpCells;

		public WaterPipeParameterData m_Parameters;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			BufferAccessor<InstalledUpgrade> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			bool flag = nativeArray2.Length != 0;
			WaterPumpingStationData data = default(WaterPumpingStationData);
			for (int i = 0; i < ((ArchetypeChunk)(ref chunk)).Count; i++)
			{
				if (flag && (nativeArray2[i].m_Flags & (TempFlags.Create | TempFlags.Modify | TempFlags.Upgrade)) == 0)
				{
					continue;
				}
				m_PumpDatas.TryGetComponent(nativeArray[i].m_Prefab, ref data);
				if (bufferAccessor.Length != 0)
				{
					UpgradeUtils.CombineStats<WaterPumpingStationData>(ref data, bufferAccessor[i], ref m_Prefabs, ref m_PumpDatas);
				}
				if ((data.m_Types & AllowedWaterTypes.Groundwater) != AllowedWaterTypes.None && GroundWaterSystem.TryGetCell(nativeArray3[i].m_Position, out var cell))
				{
					int num = Mathf.CeilToInt(math.clamp((float)GroundWaterSystem.GetGroundWater(nativeArray3[i].m_Position, m_GroundWaterMap).m_Max / m_Parameters.m_GroundwaterPumpEffectiveAmount, 0f, 1f) * (float)data.m_Capacity);
					if (!m_PumpCapacityMap.ContainsKey(cell))
					{
						m_PumpCapacityMap.Add(cell, num);
					}
					else
					{
						ref NativeParallelHashMap<int2, int> reference = ref m_PumpCapacityMap;
						int2 val = cell;
						reference[val] += num;
					}
					if (flag)
					{
						m_TempGroundWaterPumpCells.Add(ref cell);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public struct GroundWaterReservoirResult
	{
		public int m_PumpCapacity;

		public int m_Volume;
	}

	[BurstCompile]
	public struct GroundWaterReservoirJob : IJob
	{
		[ReadOnly]
		public NativeArray<GroundWater> m_GroundWaterMap;

		[ReadOnly]
		public NativeParallelHashMap<int2, int> m_PumpCapacityMap;

		[ReadOnly]
		public NativeList<int2> m_TempGroundWaterPumpCells;

		public NativeQueue<int2> m_Queue;

		public NativeReference<GroundWaterReservoirResult> m_Result;

		public void Execute()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			NativeParallelHashSet<int2> processedCells = default(NativeParallelHashSet<int2>);
			processedCells._002Ector(128, AllocatorHandle.op_Implicit((Allocator)2));
			ref GroundWaterReservoirResult reference = ref CollectionUtils.ValueAsRef<GroundWaterReservoirResult>(m_Result);
			Enumerator<int2> enumerator = m_TempGroundWaterPumpCells.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int2 current = enumerator.Current;
					EnqueueIfUnprocessed(current, processedCells);
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			int2 val = default(int2);
			int num2 = default(int);
			while (m_Queue.TryDequeue(ref val))
			{
				int num = val.x + val.y * GroundWaterSystem.kTextureSize;
				GroundWater groundWater = m_GroundWaterMap[num];
				if (m_PumpCapacityMap.TryGetValue(val, ref num2))
				{
					reference.m_PumpCapacity += num2;
				}
				if (groundWater.m_Max > 500)
				{
					reference.m_Volume += groundWater.m_Max;
					EnqueueIfUnprocessed(new int2(val.x - 1, val.y), processedCells);
					EnqueueIfUnprocessed(new int2(val.x + 1, val.y), processedCells);
					EnqueueIfUnprocessed(new int2(val.x, val.y - 1), processedCells);
					EnqueueIfUnprocessed(new int2(val.x, val.y + 2), processedCells);
				}
				else if (reference.m_Volume > 0)
				{
					reference.m_Volume += groundWater.m_Max;
				}
			}
		}

		private void EnqueueIfUnprocessed(int2 cell, NativeParallelHashSet<int2> processedCells)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (GroundWaterSystem.IsValidCell(cell) && processedCells.Add(cell))
			{
				m_Queue.Enqueue(cell);
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WaterPumpingStationData> __Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Simulation.WaterSourceData> __Game_Simulation_WaterSourceData_RO_ComponentLookup;

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
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Objects_SubObject_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Objects.SubObject>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WaterPumpingStationData>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Simulation_WaterSourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Simulation.WaterSourceData>(true);
		}
	}

	private GroundWaterSystem m_GroundWaterSystem;

	private WaterSystem m_WaterSystem;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_ErrorQuery;

	private EntityQuery m_TempQuery;

	private EntityQuery m_PumpQuery;

	private EntityQuery m_ParameterQuery;

	private ProgressTooltip m_Capacity;

	private IntTooltip m_ReservoirUsage;

	private StringTooltip m_OverRefreshCapacityWarning;

	private StringTooltip m_AvailabilityWarning;

	private LocalizedString m_GroundWarning;

	private LocalizedString m_SurfaceWarning;

	private NativeReference<TempResult> m_TempResult;

	private NativeReference<GroundWaterReservoirResult> m_ReservoirResult;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_ErrorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Error>()
		});
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Game.Buildings.WaterPumpingStation>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Temp>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Error>(),
			ComponentType.Exclude<Deleted>()
		});
		m_PumpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Buildings.WaterPumpingStation>(),
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Hidden>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<WaterPipeParameterData>() });
		m_Capacity = new ProgressTooltip
		{
			path = "groundWaterCapacity",
			icon = "Media/Game/Icons/Water.svg",
			label = LocalizedString.Id("Tools.WATER_OUTPUT_LABEL"),
			unit = "volume",
			omitMax = true
		};
		m_ReservoirUsage = new IntTooltip
		{
			path = "groundWaterReservoirUsage",
			label = LocalizedString.Id("Tools.GROUND_WATER_RESERVOIR_USAGE"),
			unit = "percentage"
		};
		m_OverRefreshCapacityWarning = new StringTooltip
		{
			path = "groundWaterOverRefreshCapacityWarning",
			value = LocalizedString.Id("Tools.WARNING[OverRefreshCapacity]"),
			color = TooltipColor.Warning
		};
		m_AvailabilityWarning = new StringTooltip
		{
			path = "waterAvailabilityWarning",
			color = TooltipColor.Warning
		};
		m_GroundWarning = LocalizedString.Id("Tools.WARNING[NotEnoughGroundWater]");
		m_SurfaceWarning = LocalizedString.Id("Tools.WARNING[NotEnoughFreshWater]");
		m_TempResult = new NativeReference<TempResult>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_ReservoirResult = new NativeReference<GroundWaterReservoirResult>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_TempResult.Dispose();
		m_ReservoirResult.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ErrorQuery)).IsEmptyIgnoreFilter || ((EntityQuery)(ref m_TempQuery)).IsEmptyIgnoreFilter)
		{
			m_TempResult.Value = default(TempResult);
			m_ReservoirResult.Value = default(GroundWaterReservoirResult);
			return;
		}
		ProcessResults();
		m_TempResult.Value = default(TempResult);
		m_ReservoirResult.Value = default(GroundWaterReservoirResult);
		JobHandle dependencies;
		NativeArray<GroundWater> map = m_GroundWaterSystem.GetMap(readOnly: true, out dependencies);
		WaterPipeParameterData singleton = ((EntityQuery)(ref m_ParameterQuery)).GetSingleton<WaterPipeParameterData>();
		JobHandle deps;
		JobHandle val = JobChunkExtensions.Schedule<TempJob>(new TempJob
		{
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjectType = InternalCompilerInterface.GetBufferTypeHandle<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PumpDatas = InternalCompilerInterface.GetComponentLookup<WaterPumpingStationData>(ref __TypeHandle.__Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Transforms = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WaterSources = InternalCompilerInterface.GetComponentLookup<Game.Simulation.WaterSourceData>(ref __TypeHandle.__Game_Simulation_WaterSourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroundWaterMap = map,
			m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
			m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
			m_Result = m_TempResult,
			m_Parameters = singleton
		}, m_TempQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies, deps));
		m_WaterSystem.AddSurfaceReader(val);
		m_TerrainSystem.AddCPUHeightReader(val);
		NativeParallelHashMap<int2, int> pumpCapacityMap = default(NativeParallelHashMap<int2, int>);
		pumpCapacityMap._002Ector(8, AllocatorHandle.op_Implicit((Allocator)3));
		NativeList<int2> tempGroundWaterPumpCells = default(NativeList<int2>);
		tempGroundWaterPumpCells._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle val2 = JobChunkExtensions.Schedule<GroundWaterPumpJob>(new GroundWaterPumpJob
		{
			m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PumpDatas = InternalCompilerInterface.GetComponentLookup<WaterPumpingStationData>(ref __TypeHandle.__Game_Prefabs_WaterPumpingStationData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GroundWaterMap = map,
			m_PumpCapacityMap = pumpCapacityMap,
			m_TempGroundWaterPumpCells = tempGroundWaterPumpCells,
			m_Parameters = singleton
		}, m_PumpQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
		GroundWaterReservoirJob groundWaterReservoirJob = new GroundWaterReservoirJob
		{
			m_GroundWaterMap = map,
			m_PumpCapacityMap = pumpCapacityMap,
			m_TempGroundWaterPumpCells = tempGroundWaterPumpCells,
			m_Queue = new NativeQueue<int2>(AllocatorHandle.op_Implicit((Allocator)3)),
			m_Result = m_ReservoirResult
		};
		JobHandle val3 = IJobExtensions.Schedule<GroundWaterReservoirJob>(groundWaterReservoirJob, val2);
		groundWaterReservoirJob.m_Queue.Dispose(val3);
		pumpCapacityMap.Dispose(val3);
		tempGroundWaterPumpCells.Dispose(val3);
		((SystemBase)this).Dependency = JobHandle.CombineDependencies(val, val3);
		m_GroundWaterSystem.AddReader(((SystemBase)this).Dependency);
	}

	private void ProcessResults()
	{
		TempResult value = m_TempResult.Value;
		GroundWaterReservoirResult value2 = m_ReservoirResult.Value;
		if (value.m_MaxCapacity <= 0)
		{
			return;
		}
		if ((value.m_Types & AllowedWaterTypes.Groundwater) != AllowedWaterTypes.None)
		{
			ProcessProduction(value);
			if (value2.m_Volume > 0)
			{
				ProcessReservoir(value2);
			}
			ProcessAvailabilityWarning(value, m_GroundWarning);
		}
		else if ((value.m_Types & AllowedWaterTypes.SurfaceWater) != AllowedWaterTypes.None)
		{
			ProcessProduction(value);
			ProcessAvailabilityWarning(value, m_SurfaceWarning);
		}
		else
		{
			ProcessProduction(value);
		}
	}

	private void ProcessReservoir(GroundWaterReservoirResult reservoir)
	{
		WaterPipeParameterData singleton = ((EntityQuery)(ref m_ParameterQuery)).GetSingleton<WaterPipeParameterData>();
		float num = singleton.m_GroundwaterReplenish / singleton.m_GroundwaterUsageMultiplier * (float)reservoir.m_Volume;
		float num2 = ((num > 0f && reservoir.m_PumpCapacity > 0) ? math.clamp(100f * (float)reservoir.m_PumpCapacity / num, 1f, 999f) : 0f);
		m_ReservoirUsage.value = Mathf.RoundToInt(num2);
		m_ReservoirUsage.color = ((num2 > 100f) ? TooltipColor.Warning : TooltipColor.Info);
		AddMouseTooltip(m_ReservoirUsage);
		if (num2 > 100f)
		{
			AddMouseTooltip(m_OverRefreshCapacityWarning);
		}
	}

	private void ProcessProduction(TempResult temp)
	{
		if (temp.m_Production > 0)
		{
			m_Capacity.value = temp.m_Production;
			m_Capacity.max = temp.m_MaxCapacity;
			ProgressTooltip.SetCapacityColor(m_Capacity);
			AddMouseTooltip(m_Capacity);
		}
	}

	private void ProcessAvailabilityWarning(TempResult temp, LocalizedString warningText)
	{
		if (temp.m_Production > 0 && (float)temp.m_Production < (float)temp.m_MaxCapacity * 0.75f)
		{
			m_AvailabilityWarning.value = warningText;
			AddMouseTooltip(m_AvailabilityWarning);
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
	public TempWaterPumpingTooltipSystem()
	{
	}
}
