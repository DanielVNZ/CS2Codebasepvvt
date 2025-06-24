using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class ServiceCoverageSystem : GameSystemBase
{
	[BurstCompile]
	public struct ClearCoverageJob : IJobChunk
	{
		[ReadOnly]
		public int m_CoverageIndex;

		public BufferTypeHandle<Game.Net.ServiceCoverage> m_ServiceCoverageType;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			BufferAccessor<Game.Net.ServiceCoverage> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Game.Net.ServiceCoverage>(ref m_ServiceCoverageType);
			for (int i = 0; i < bufferAccessor.Length; i++)
			{
				DynamicBuffer<Game.Net.ServiceCoverage> val = bufferAccessor[i];
				val[m_CoverageIndex] = default(Game.Net.ServiceCoverage);
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public struct CoverageElement : IComparable<CoverageElement>
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe void* m_CoveragePtr;

		public float2 m_Coverage;

		public float m_AverageCoverage;

		public float m_DensityFactor;

		public float m_LengthFactor;

		public int CompareTo(CoverageElement other)
		{
			return math.select(0, math.select(-1, 1, m_AverageCoverage < other.m_AverageCoverage), m_AverageCoverage != other.m_AverageCoverage);
		}
	}

	private struct QueueItem
	{
		public Entity m_Entity;

		public uint m_QueueFrame;

		public uint m_ResultFrame;
	}

	public struct BuildingData
	{
		public Entity m_Entity;

		public int m_ElementIndex;

		public int m_ElementCount;

		public float m_Total;

		public float m_Remaining;
	}

	[BurstCompile]
	public struct PrepareCoverageJob : IJob
	{
		[ReadOnly]
		public CoverageService m_Service;

		[ReadOnly]
		public NativeArray<ArchetypeChunk> m_BuildingChunks;

		[ReadOnly]
		public SharedComponentTypeHandle<CoverageServiceType> m_CoverageServiceType;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public BufferTypeHandle<Game.Pathfind.CoverageElement> m_CoverageElementType;

		public NativeList<BuildingData> m_BuildingData;

		public NativeList<CoverageElement> m_Elements;

		public void Execute()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			BuildingData buildingData = default(BuildingData);
			for (int i = 0; i < m_BuildingChunks.Length; i++)
			{
				ArchetypeChunk val = m_BuildingChunks[i];
				if (((ArchetypeChunk)(ref val)).GetSharedComponent<CoverageServiceType>(m_CoverageServiceType).m_Service != m_Service)
				{
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				BufferAccessor<Game.Pathfind.CoverageElement> bufferAccessor = ((ArchetypeChunk)(ref val)).GetBufferAccessor<Game.Pathfind.CoverageElement>(ref m_CoverageElementType);
				for (int j = 0; j < ((ArchetypeChunk)(ref val)).Count; j++)
				{
					DynamicBuffer<Game.Pathfind.CoverageElement> val2 = bufferAccessor[j];
					if (val2.Length != 0)
					{
						buildingData.m_Entity = nativeArray[j];
						buildingData.m_ElementCount = val2.Length;
						m_BuildingData.Add(ref buildingData);
						buildingData.m_ElementIndex += val2.Length;
					}
				}
			}
			m_Elements.ResizeUninitialized(buildingData.m_ElementIndex);
		}
	}

	[BurstCompile]
	public struct ProcessCoverageJob : IJobParallelForDefer
	{
		[ReadOnly]
		public int m_CoverageIndex;

		[NativeDisableParallelForRestriction]
		public NativeList<BuildingData> m_BuildingData;

		[NativeDisableParallelForRestriction]
		public NativeList<CoverageElement> m_Elements;

		[ReadOnly]
		public ComponentLookup<Curve> m_CurveData;

		[ReadOnly]
		public ComponentLookup<Density> m_DensityData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<ModifiedServiceCoverage> m_ModifiedServiceCoverageData;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> m_BorderDistrictData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CoverageData> m_PrefabCoverageData;

		[ReadOnly]
		public BufferLookup<Game.Pathfind.CoverageElement> m_CoverageElements;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		[ReadOnly]
		public BufferLookup<Efficiency> m_Efficiencies;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Game.Net.ServiceCoverage> m_CoverageData;

		public unsafe void Execute(int index)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_034d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			ref BuildingData reference = ref m_BuildingData.ElementAt(index);
			PrefabRef prefabRef = m_PrefabRefData[reference.m_Entity];
			DynamicBuffer<Game.Pathfind.CoverageElement> val = m_CoverageElements[reference.m_Entity];
			CoverageData coverage = default(CoverageData);
			m_PrefabCoverageData.TryGetComponent(prefabRef.m_Prefab, ref coverage);
			DynamicBuffer<ServiceDistrict> val2 = default(DynamicBuffer<ServiceDistrict>);
			Temp temp = default(Temp);
			float efficiency;
			if (m_TempData.TryGetComponent(reference.m_Entity, ref temp))
			{
				ModifiedServiceCoverage modifiedServiceCoverage = default(ModifiedServiceCoverage);
				if (m_ModifiedServiceCoverageData.TryGetComponent(temp.m_Original, ref modifiedServiceCoverage))
				{
					modifiedServiceCoverage.ReplaceData(ref coverage);
				}
				m_ServiceDistricts.TryGetBuffer(temp.m_Original, ref val2);
				efficiency = BuildingUtils.GetEfficiency(temp.m_Original, ref m_Efficiencies);
			}
			else
			{
				ModifiedServiceCoverage modifiedServiceCoverage2 = default(ModifiedServiceCoverage);
				if (m_ModifiedServiceCoverageData.TryGetComponent(reference.m_Entity, ref modifiedServiceCoverage2))
				{
					modifiedServiceCoverage2.ReplaceData(ref coverage);
				}
				m_ServiceDistricts.TryGetBuffer(reference.m_Entity, ref val2);
				efficiency = BuildingUtils.GetEfficiency(reference.m_Entity, ref m_Efficiencies);
			}
			NativeHashSet<Entity> val3 = default(NativeHashSet<Entity>);
			if (val2.IsCreated && val2.Length != 0)
			{
				val3._002Ector(val2.Length, AllocatorHandle.op_Implicit((Allocator)2));
				for (int i = 0; i < val2.Length; i++)
				{
					val3.Add(val2[i].m_District);
				}
			}
			int num = reference.m_ElementIndex;
			DynamicBuffer<Game.Net.ServiceCoverage> val4 = default(DynamicBuffer<Game.Net.ServiceCoverage>);
			BorderDistrict borderDistrict = default(BorderDistrict);
			bool2 val5 = default(bool2);
			Density density = default(Density);
			for (int j = 0; j < val.Length; j++)
			{
				Game.Pathfind.CoverageElement coverageElement = val[j];
				if (!m_CoverageData.TryGetBuffer(coverageElement.m_Edge, ref val4))
				{
					continue;
				}
				float densityFactor = 1f;
				if (val3.IsCreated && m_BorderDistrictData.TryGetComponent(coverageElement.m_Edge, ref borderDistrict))
				{
					if (borderDistrict.m_Right == borderDistrict.m_Left)
					{
						if (!val3.Contains(borderDistrict.m_Left))
						{
							continue;
						}
					}
					else
					{
						val5.x = val3.Contains(borderDistrict.m_Left);
						val5.y = val3.Contains(borderDistrict.m_Right);
						if (!math.any(val5))
						{
							continue;
						}
						densityFactor = math.select(0.5f, 1f, math.all(val5));
					}
				}
				float num2 = 0.01f;
				if (m_DensityData.TryGetComponent(coverageElement.m_Edge, ref density))
				{
					num2 = math.max(num2, density.m_Density);
				}
				CoverageElement coverageElement2 = default(CoverageElement);
				coverageElement2.m_CoveragePtr = UnsafeUtility.AddressOf<Game.Net.ServiceCoverage>(ref val4.ElementAt(m_CoverageIndex));
				coverageElement2.m_Coverage = math.max(float2.op_Implicit(0f), 1f - coverageElement.m_Cost * coverageElement.m_Cost) * coverage.m_Magnitude * efficiency;
				coverageElement2.m_AverageCoverage = math.csum(coverageElement2.m_Coverage) * 0.5f;
				coverageElement2.m_DensityFactor = densityFactor;
				coverageElement2.m_LengthFactor = m_CurveData[coverageElement.m_Edge].m_Length * math.sqrt(num2);
				m_Elements[num++] = coverageElement2;
			}
			if (num > reference.m_ElementIndex + 1)
			{
				NativeSortExtension.Sort<CoverageElement>(m_Elements.AsArray().GetSubArray(reference.m_ElementIndex, num - reference.m_ElementIndex));
			}
			reference.m_Total = coverage.m_Capacity;
			reference.m_Remaining = coverage.m_Capacity;
			reference.m_ElementCount = num - reference.m_ElementIndex;
			if (val3.IsCreated)
			{
				val3.Dispose();
			}
		}
	}

	private struct BuildingDataComparer : IComparer<BuildingData>
	{
		public NativeList<CoverageElement> m_Elements;

		public int Compare(BuildingData x, BuildingData y)
		{
			return m_Elements[x.m_ElementIndex].CompareTo(m_Elements[y.m_ElementIndex]);
		}
	}

	[BurstCompile]
	public struct ApplyCoverageJob : IJob
	{
		public NativeList<BuildingData> m_BuildingData;

		public NativeList<CoverageElement> m_Elements;

		public unsafe void Execute()
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_BuildingData.Length; i++)
			{
				BuildingData buildingData = m_BuildingData[i];
				if (buildingData.m_ElementCount == 0 || buildingData.m_Remaining <= 0f)
				{
					m_BuildingData.RemoveAtSwapBack(i--);
				}
			}
			NativeSortExtension.Sort<BuildingData, BuildingDataComparer>(m_BuildingData, new BuildingDataComparer
			{
				m_Elements = m_Elements
			});
			int num = 0;
			while (num < m_BuildingData.Length)
			{
				BuildingData buildingData2 = m_BuildingData[num];
				CoverageElement coverageElement = m_Elements[buildingData2.m_ElementIndex++];
				ref Game.Net.ServiceCoverage reference = ref UnsafeUtility.AsRef<Game.Net.ServiceCoverage>(coverageElement.m_CoveragePtr);
				if (math.any(coverageElement.m_Coverage > reference.m_Coverage))
				{
					float num2 = 0.99f * (1f - buildingData2.m_Remaining / buildingData2.m_Total);
					num2 *= num2;
					num2 *= num2;
					num2 *= num2;
					num2 = 1f - num2;
					float2 val = coverageElement.m_Coverage * num2;
					float2 val2 = val - reference.m_Coverage;
					val2 = math.clamp(val2, float2.op_Implicit(0f), val * coverageElement.m_DensityFactor);
					ref float2 coverage = ref reference.m_Coverage;
					coverage += val2;
					val2 = math.saturate(val2 / coverageElement.m_Coverage);
					buildingData2.m_Remaining -= math.lerp(val2.x, val2.y, 0.5f) * coverageElement.m_LengthFactor * coverageElement.m_DensityFactor;
				}
				if (--buildingData2.m_ElementCount == 0 || buildingData2.m_Remaining <= 0f)
				{
					num++;
					continue;
				}
				coverageElement = m_Elements[buildingData2.m_ElementIndex];
				m_BuildingData[num] = buildingData2;
				for (int j = num + 1; j < m_BuildingData.Length; j++)
				{
					BuildingData buildingData3 = m_BuildingData[j];
					CoverageElement other = m_Elements[buildingData3.m_ElementIndex];
					if (coverageElement.CompareTo(other) <= 0)
					{
						break;
					}
					m_BuildingData[j] = buildingData2;
					m_BuildingData[j - 1] = buildingData3;
				}
			}
		}
	}

	[BurstCompile]
	public struct SetupCoverageSearchJob : IJob
	{
		[ReadOnly]
		public Entity m_Entity;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<CoverageData> m_PrefabCoverageData;

		public CoverageAction m_Action;

		public PathfindTargetSeeker<PathfindTargetBuffer> m_TargetSeeker;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			PrefabRef prefabRef = m_PrefabRefData[m_Entity];
			CoverageData coverageData = default(CoverageData);
			if (m_PrefabCoverageData.HasComponent(prefabRef.m_Prefab))
			{
				coverageData = m_PrefabCoverageData[prefabRef.m_Prefab];
			}
			m_TargetSeeker.FindTargets(m_Entity, 0f);
			m_Action.data.m_Parameters = new CoverageParameters
			{
				m_Methods = m_TargetSeeker.m_PathfindParameters.m_Methods,
				m_Range = coverageData.m_Range
			};
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		public BufferTypeHandle<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RW_BufferTypeHandle;

		public SharedComponentTypeHandle<CoverageServiceType> __Game_Net_CoverageServiceType_SharedComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Game.Pathfind.CoverageElement> __Game_Pathfind_CoverageElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Curve> __Game_Net_Curve_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Density> __Game_Net_Density_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ModifiedServiceCoverage> __Game_Buildings_ModifiedServiceCoverage_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BorderDistrict> __Game_Areas_BorderDistrict_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CoverageData> __Game_Prefabs_CoverageData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Pathfind.CoverageElement> __Game_Pathfind_CoverageElement_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> __Game_Areas_ServiceDistrict_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		public BufferLookup<Game.Net.ServiceCoverage> __Game_Net_ServiceCoverage_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_ServiceCoverage_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Net.ServiceCoverage>(false);
			__Game_Net_CoverageServiceType_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<CoverageServiceType>();
			__Game_Pathfind_CoverageElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Game.Pathfind.CoverageElement>(true);
			__Game_Net_Curve_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Curve>(true);
			__Game_Net_Density_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Density>(true);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Buildings_ModifiedServiceCoverage_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ModifiedServiceCoverage>(true);
			__Game_Areas_BorderDistrict_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BorderDistrict>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_CoverageData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CoverageData>(true);
			__Game_Pathfind_CoverageElement_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Pathfind.CoverageElement>(true);
			__Game_Areas_ServiceDistrict_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceDistrict>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Net_ServiceCoverage_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.ServiceCoverage>(false);
		}
	}

	public const uint COVERAGE_UPDATE_INTERVAL = 256u;

	private SimulationSystem m_SimulationSystem;

	private PathfindQueueSystem m_PathfindQueueSystem;

	private AirwaySystem m_AirwaySystem;

	private EntityQuery m_EdgeQuery;

	private EntityQuery m_BuildingQuery;

	private PathfindTargetSeekerData m_TargetSeekerData;

	private NativeQueue<QueueItem> m_PendingCoverages;

	private CoverageService m_LastCoverageService;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_PathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		m_AirwaySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirwaySystem>();
		m_EdgeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Net.Edge>(),
			ComponentType.ReadWrite<Game.Net.ServiceCoverage>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_BuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<CoverageServiceType>(),
			ComponentType.ReadOnly<Game.Pathfind.CoverageElement>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_TargetSeekerData = new PathfindTargetSeekerData((SystemBase)(object)this);
		m_PendingCoverages = new NativeQueue<QueueItem>(AllocatorHandle.op_Implicit((Allocator)4));
		m_LastCoverageService = CoverageService.Count;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_PendingCoverages.Dispose();
		base.OnDestroy();
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		m_PendingCoverages.Clear();
		m_LastCoverageService = CoverageService.Count;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		CoverageService frameService = GetFrameService(m_SimulationSystem.frameIndex);
		CoverageService frameService2 = GetFrameService(m_SimulationSystem.frameIndex + 1);
		if (frameService == frameService2)
		{
			if (EnqueuePendingCoverages(out var outputDeps))
			{
				((SystemBase)this).Dependency = outputDeps;
			}
			return;
		}
		NativeArray<ArchetypeChunk> buildingChunks = ((EntityQuery)(ref m_BuildingQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		BufferTypeHandle<Game.Net.ServiceCoverage> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		SharedComponentTypeHandle<CoverageServiceType> sharedComponentTypeHandle = InternalCompilerInterface.GetSharedComponentTypeHandle<CoverageServiceType>(ref __TypeHandle.__Game_Net_CoverageServiceType_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		uint queueFrame = m_SimulationSystem.frameIndex + 192;
		uint resultFrame = m_SimulationSystem.frameIndex + 256;
		for (int i = 0; i < buildingChunks.Length; i++)
		{
			ArchetypeChunk val = buildingChunks[i];
			if (((ArchetypeChunk)(ref val)).GetSharedComponent<CoverageServiceType>(sharedComponentTypeHandle).m_Service == frameService2)
			{
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(entityTypeHandle);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					m_PendingCoverages.Enqueue(new QueueItem
					{
						m_Entity = nativeArray[j],
						m_QueueFrame = queueFrame,
						m_ResultFrame = resultFrame
					});
				}
			}
		}
		EnqueuePendingCoverages(out var outputDeps2);
		if (m_LastCoverageService != CoverageService.Count)
		{
			NativeList<BuildingData> val2 = default(NativeList<BuildingData>);
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<CoverageElement> elements = default(NativeList<CoverageElement>);
			elements._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			PrepareCoverageJob prepareCoverageJob = new PrepareCoverageJob
			{
				m_Service = frameService,
				m_BuildingChunks = buildingChunks,
				m_CoverageServiceType = sharedComponentTypeHandle,
				m_EntityType = entityTypeHandle,
				m_CoverageElementType = InternalCompilerInterface.GetBufferTypeHandle<Game.Pathfind.CoverageElement>(ref __TypeHandle.__Game_Pathfind_CoverageElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingData = val2,
				m_Elements = elements
			};
			ClearCoverageJob clearCoverageJob = new ClearCoverageJob
			{
				m_CoverageIndex = (int)frameService,
				m_ServiceCoverageType = bufferTypeHandle
			};
			ProcessCoverageJob processCoverageJob = new ProcessCoverageJob
			{
				m_CoverageIndex = (int)frameService,
				m_BuildingData = val2,
				m_Elements = elements,
				m_CurveData = InternalCompilerInterface.GetComponentLookup<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_DensityData = InternalCompilerInterface.GetComponentLookup<Density>(ref __TypeHandle.__Game_Net_Density_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ModifiedServiceCoverageData = InternalCompilerInterface.GetComponentLookup<ModifiedServiceCoverage>(ref __TypeHandle.__Game_Buildings_ModifiedServiceCoverage_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BorderDistrictData = InternalCompilerInterface.GetComponentLookup<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCoverageData = InternalCompilerInterface.GetComponentLookup<CoverageData>(ref __TypeHandle.__Game_Prefabs_CoverageData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CoverageElements = InternalCompilerInterface.GetBufferLookup<Game.Pathfind.CoverageElement>(ref __TypeHandle.__Game_Pathfind_CoverageElement_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceDistricts = InternalCompilerInterface.GetBufferLookup<ServiceDistrict>(ref __TypeHandle.__Game_Areas_ServiceDistrict_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Efficiencies = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CoverageData = InternalCompilerInterface.GetBufferLookup<Game.Net.ServiceCoverage>(ref __TypeHandle.__Game_Net_ServiceCoverage_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			ApplyCoverageJob obj = new ApplyCoverageJob
			{
				m_BuildingData = val2,
				m_Elements = elements
			};
			JobHandle val3 = IJobExtensions.Schedule<PrepareCoverageJob>(prepareCoverageJob, ((SystemBase)this).Dependency);
			JobHandle val4 = JobChunkExtensions.ScheduleParallel<ClearCoverageJob>(clearCoverageJob, m_EdgeQuery, ((SystemBase)this).Dependency);
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<ProcessCoverageJob, BuildingData>(processCoverageJob, val2, 1, JobHandle.CombineDependencies(val3, val4));
			JobHandle val6 = IJobExtensions.Schedule<ApplyCoverageJob>(obj, val5);
			buildingChunks.Dispose(val3);
			val2.Dispose(val6);
			elements.Dispose(val6);
			outputDeps2 = JobHandle.CombineDependencies(outputDeps2, val6);
		}
		else
		{
			buildingChunks.Dispose();
		}
		m_LastCoverageService = frameService2;
		((SystemBase)this).Dependency = outputDeps2;
	}

	private bool EnqueuePendingCoverages(out JobHandle outputDeps)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		outputDeps = default(JobHandle);
		if (m_PendingCoverages.IsEmpty())
		{
			return false;
		}
		int count = m_PendingCoverages.Count;
		int num = 192;
		int num2 = (count + num - 1) / num;
		m_TargetSeekerData.Update((SystemBase)(object)this, m_AirwaySystem.GetAirwayData());
		PathfindParameters pathfindParameters = new PathfindParameters
		{
			m_MaxSpeed = float2.op_Implicit(111.111115f),
			m_WalkSpeed = float2.op_Implicit(5.555556f),
			m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
			m_PathfindFlags = (PathfindFlags.Stable | PathfindFlags.IgnoreFlow),
			m_IgnoredRules = (RuleFlags.HasBlockage | RuleFlags.ForbidCombustionEngines | RuleFlags.ForbidTransitTraffic | RuleFlags.ForbidHeavyTraffic | RuleFlags.ForbidPrivateTraffic | RuleFlags.ForbidSlowTraffic)
		};
		SetupQueueTarget setupQueueTarget = default(SetupQueueTarget);
		SetupCoverageSearchJob setupCoverageSearchJob = new SetupCoverageSearchJob
		{
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCoverageData = InternalCompilerInterface.GetComponentLookup<CoverageData>(ref __TypeHandle.__Game_Prefabs_CoverageData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		CoverageServiceType coverageServiceType = default(CoverageServiceType);
		for (int i = 0; i < count; i++)
		{
			QueueItem queueItem = m_PendingCoverages.Peek();
			if (--num2 < 0 && queueItem.m_QueueFrame > m_SimulationSystem.frameIndex)
			{
				break;
			}
			m_PendingCoverages.Dequeue();
			if (EntitiesExtensions.TryGetSharedComponent<CoverageServiceType>(((ComponentSystemBase)this).EntityManager, queueItem.m_Entity, ref coverageServiceType))
			{
				SetupPathfindMethods(coverageServiceType.m_Service, ref pathfindParameters, ref setupQueueTarget);
				CoverageAction action = new CoverageAction((Allocator)4);
				setupCoverageSearchJob.m_Entity = queueItem.m_Entity;
				setupCoverageSearchJob.m_TargetSeeker = new PathfindTargetSeeker<PathfindTargetBuffer>(m_TargetSeekerData, pathfindParameters, setupQueueTarget, action.data.m_Sources.AsParallelWriter(), RandomSeed.Next(), isStartTarget: true);
				setupCoverageSearchJob.m_Action = action;
				JobHandle val = IJobExtensions.Schedule<SetupCoverageSearchJob>(setupCoverageSearchJob, ((SystemBase)this).Dependency);
				outputDeps = JobHandle.CombineDependencies(outputDeps, val);
				m_PathfindQueueSystem.Enqueue(action, queueItem.m_Entity, val, queueItem.m_ResultFrame, this);
			}
		}
		return true;
	}

	public static void SetupPathfindMethods(CoverageService service, ref PathfindParameters pathfindParameters, ref SetupQueueTarget setupQueueTarget)
	{
		switch (service)
		{
		case CoverageService.PostService:
		case CoverageService.Education:
		case CoverageService.EmergencyShelter:
		case CoverageService.Welfare:
			pathfindParameters.m_Methods = PathMethod.Pedestrian;
			setupQueueTarget.m_Methods = PathMethod.Pedestrian;
			setupQueueTarget.m_RoadTypes = RoadTypes.None;
			break;
		case CoverageService.Park:
			pathfindParameters.m_Methods = PathMethod.Pedestrian;
			setupQueueTarget.m_Methods = PathMethod.Pedestrian;
			setupQueueTarget.m_RoadTypes = RoadTypes.None;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.BenchSitting).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.PullUps).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.Standing).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.GroundLaying).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.GroundSitting).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.PushUps).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.SitUps).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.JumpingJacks).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.JumpingLunges).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.Squats).m_Mask;
			setupQueueTarget.m_ActivityMask.m_Mask |= new ActivityMask(ActivityType.Yoga).m_Mask;
			break;
		default:
			pathfindParameters.m_Methods = PathMethod.Road;
			setupQueueTarget.m_Methods = PathMethod.Road;
			setupQueueTarget.m_RoadTypes = RoadTypes.Car;
			break;
		}
	}

	private static CoverageService GetFrameService(uint frame)
	{
		return (CoverageService)(frame % 256 * 8 / 256);
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
	public ServiceCoverageSystem()
	{
	}
}
