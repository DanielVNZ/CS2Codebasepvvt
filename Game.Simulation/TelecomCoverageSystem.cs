using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class TelecomCoverageSystem : CellMapSystem<TelecomCoverage>, IJobSerializable
{
	private struct CellDensityData
	{
		public ushort m_Density;
	}

	private struct CellFacilityData
	{
		public float m_SignalStrength;

		public float m_AccumulatedSignalStrength;

		public float m_NetworkCapacity;
	}

	[BurstCompile]
	public struct TelecomCoverageJob : IJob
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_DensityChunks;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_FacilityChunks;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public bool m_Preview;

		public NativeArray<TelecomCoverage> m_TelecomCoverage;

		public NativeArray<TelecomStatus> m_TelecomStatus;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> m_PropertyRenterType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TelecomFacility> m_TelecomFacilityType;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> m_BuildingEfficiencyType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> m_HouseholdCitizenType;

		[ReadOnly]
		public BufferTypeHandle<Employee> m_EmployeeType;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TelecomFacility> m_TelecomFacilityData;

		[ReadOnly]
		public BufferLookup<Efficiency> m_BuildingEfficiencyData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> m_ObjectGeometryData;

		[ReadOnly]
		public ComponentLookup<TelecomFacilityData> m_PrefabTelecomFacilityData;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		public void Execute()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<CellDensityData> densityData = default(NativeArray<CellDensityData>);
			densityData._002Ector(16384, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<CellFacilityData> facilityData = default(NativeArray<CellFacilityData>);
			facilityData._002Ector(16384, (Allocator)2, (NativeArrayOptions)1);
			NativeArray<float> obstructSlopes = default(NativeArray<float>);
			obstructSlopes._002Ector(16384, (Allocator)2, (NativeArrayOptions)0);
			NativeList<float> signalStrengths = default(NativeList<float>);
			signalStrengths._002Ector(16384, AllocatorHandle.op_Implicit((Allocator)2));
			DynamicBuffer<CityModifier> cityModifiers = m_CityModifiers[m_City];
			for (int i = 0; i < m_DensityChunks.Length; i++)
			{
				AddDensity(densityData, m_DensityChunks[i]);
			}
			for (int j = 0; j < m_FacilityChunks.Length; j++)
			{
				CalculateSignalStrength(facilityData, obstructSlopes, signalStrengths, m_FacilityChunks[j], cityModifiers);
			}
			int arrayIndex = 0;
			TelecomStatus status = default(TelecomStatus);
			for (int k = 0; k < m_FacilityChunks.Length; k++)
			{
				AddNetworkCapacity(densityData, facilityData, signalStrengths, m_FacilityChunks[k], ref arrayIndex, ref status, cityModifiers);
			}
			if (m_TelecomCoverage.Length != 0)
			{
				CalculateTelecomCoverage(facilityData);
			}
			if (m_TelecomStatus.Length != 0)
			{
				status.m_Quality = CalculateTelecomQuality(densityData, facilityData);
				m_TelecomStatus[0] = status;
			}
			densityData.Dispose();
			facilityData.Dispose();
			obstructSlopes.Dispose();
			signalStrengths.Dispose();
		}

		private void CalculateTelecomCoverage(NativeArray<CellFacilityData> facilityData)
		{
			int num = 0;
			TelecomCoverage telecomCoverage = default(TelecomCoverage);
			for (int i = 0; i < 128; i++)
			{
				for (int j = 0; j < 128; j++)
				{
					int num2 = num + j;
					CellFacilityData cellFacilityData = facilityData[num2];
					telecomCoverage.m_SignalStrength = (byte)math.clamp((int)(cellFacilityData.m_SignalStrength * 255f), 0, 255);
					telecomCoverage.m_NetworkLoad = (byte)math.clamp((int)(127.5f / math.max(0.0001f, cellFacilityData.m_NetworkCapacity)), 0, 255);
					m_TelecomCoverage[num2] = telecomCoverage;
				}
				num += 128;
			}
		}

		private float CalculateTelecomQuality(NativeArray<CellDensityData> densityData, NativeArray<CellFacilityData> facilityData)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			float2 val = float2.op_Implicit(0f);
			int num = 0;
			for (int i = 0; i < 128; i++)
			{
				for (int j = 0; j < 128; j++)
				{
					int num2 = num + j;
					CellDensityData cellDensityData = densityData[num2];
					CellFacilityData cellFacilityData = facilityData[num2];
					float num3 = cellFacilityData.m_SignalStrength * 2f;
					float num4 = 1f / math.max(0.0001f, cellFacilityData.m_NetworkCapacity);
					float num5 = math.min(1f, num3 / (1f + num4));
					float num6 = (int)cellDensityData.m_Density;
					val += new float2(num5 * num6, num6);
				}
				num += 128;
			}
			if (val.y != 0f)
			{
				val.x /= val.y;
			}
			return val.x;
		}

		private void AddNetworkCapacity(NativeArray<CellDensityData> densityData, NativeArray<CellFacilityData> facilityData, NativeList<float> signalStrengths, ArchetypeChunk chunk, ref int arrayIndex, ref TelecomStatus status, DynamicBuffer<CityModifier> cityModifiers)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Transform> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Game.Buildings.TelecomFacility> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.TelecomFacility>(ref m_TelecomFacilityType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_BuildingEfficiencyType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Transform transform = nativeArray[i];
				PrefabRef prefabRef = nativeArray3[i];
				TelecomFacilityData data = m_PrefabTelecomFacilityData[prefabRef.m_Prefab];
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<TelecomFacilityData>(ref data, bufferAccessor2[i], ref m_PrefabRefData, ref m_PrefabTelecomFacilityData);
				}
				float efficiencyFactor = GetEfficiencyFactor(nativeArray2, nativeArray4, bufferAccessor, i);
				CityUtils.ApplyModifier(ref data.m_NetworkCapacity, cityModifiers, CityModifierType.TelecomCapacity);
				data.m_Range *= math.sqrt(efficiencyFactor);
				data.m_NetworkCapacity *= efficiencyFactor;
				if (!(data.m_Range < 1f) && !(data.m_NetworkCapacity < 1f))
				{
					int2 val = math.max(CellMapSystem<TelecomCoverage>.GetCell(transform.m_Position - data.m_Range, CellMapSystem<TelecomCoverage>.kMapSize, 128), int2.op_Implicit(0));
					int2 val2 = math.min(CellMapSystem<TelecomCoverage>.GetCell(transform.m_Position + data.m_Range, CellMapSystem<TelecomCoverage>.kMapSize, 128) + 1, int2.op_Implicit(128));
					int2 val3 = val2 - val;
					if (!math.any(val3 <= 0))
					{
						NativeArray<float> subArray = signalStrengths.AsArray().GetSubArray(arrayIndex, val3.x * val3.y);
						arrayIndex += val3.x * val3.y;
						float num = CalculateNetworkUsers(densityData, facilityData, subArray, val, val2);
						float capacity = data.m_NetworkCapacity / math.max(1f, num);
						AddNetworkCapacity(facilityData, subArray, val, val2, capacity);
						status.m_Capacity += data.m_NetworkCapacity;
						status.m_Load += num;
					}
				}
			}
		}

		private void AddNetworkCapacity(NativeArray<CellFacilityData> facilityData, NativeArray<float> signalStrengthArray, int2 min, int2 max, float capacity)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			int2 val = max - min;
			int num = 128 * min.y;
			int num2 = -min.x;
			for (int i = min.y; i < max.y; i++)
			{
				for (int j = min.x; j < max.x; j++)
				{
					float num3 = signalStrengthArray[num2 + j];
					int num4 = num + j;
					CellFacilityData cellFacilityData = facilityData[num4];
					cellFacilityData.m_NetworkCapacity = math.select(cellFacilityData.m_NetworkCapacity, cellFacilityData.m_NetworkCapacity + capacity * (num3 / cellFacilityData.m_AccumulatedSignalStrength), cellFacilityData.m_AccumulatedSignalStrength > 0.0001f);
					facilityData[num4] = cellFacilityData;
				}
				num += 128;
				num2 += val.x;
			}
		}

		private float CalculateNetworkUsers(NativeArray<CellDensityData> densityData, NativeArray<CellFacilityData> facilityData, NativeArray<float> signalStrengthArray, int2 min, int2 max)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			int2 val = max - min;
			int num2 = 128 * min.y;
			int num3 = -min.x;
			for (int i = min.y; i < max.y; i++)
			{
				for (int j = min.x; j < max.x; j++)
				{
					float num4 = signalStrengthArray[num3 + j];
					int num5 = num2 + j;
					CellDensityData cellDensityData = densityData[num5];
					CellFacilityData cellFacilityData = facilityData[num5];
					num += math.select(0f, (float)(int)cellDensityData.m_Density * (num4 / cellFacilityData.m_AccumulatedSignalStrength), cellFacilityData.m_AccumulatedSignalStrength > 0.0001f);
				}
				num2 += 128;
				num3 += val.x;
			}
			return num;
		}

		private void CalculateSignalStrength(NativeArray<CellFacilityData> facilityData, NativeArray<float> obstructSlopes, NativeList<float> signalStrengths, ArchetypeChunk chunk, DynamicBuffer<CityModifier> cityModifiers)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0249: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0274: Unknown result type (might be due to invalid IL or missing references)
			//IL_0279: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_0504: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050d: Unknown result type (might be due to invalid IL or missing references)
			//IL_050f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0511: Unknown result type (might be due to invalid IL or missing references)
			//IL_0516: Unknown result type (might be due to invalid IL or missing references)
			//IL_0526: Unknown result type (might be due to invalid IL or missing references)
			//IL_0527: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0287: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0334: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0463: Unknown result type (might be due to invalid IL or missing references)
			//IL_046a: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02be: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0476: Unknown result type (might be due to invalid IL or missing references)
			//IL_0418: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0341: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_034c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_035c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0374: Unknown result type (might be due to invalid IL or missing references)
			//IL_0318: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0400: Unknown result type (might be due to invalid IL or missing references)
			//IL_0409: Unknown result type (might be due to invalid IL or missing references)
			//IL_0421: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_0483: Unknown result type (might be due to invalid IL or missing references)
			//IL_0485: Unknown result type (might be due to invalid IL or missing references)
			//IL_048e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0493: Unknown result type (might be due to invalid IL or missing references)
			//IL_0495: Unknown result type (might be due to invalid IL or missing references)
			//IL_049e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_045a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0383: Unknown result type (might be due to invalid IL or missing references)
			//IL_0387: Unknown result type (might be due to invalid IL or missing references)
			//IL_038e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_039e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_042f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0430: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Transform> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Game.Buildings.TelecomFacility> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.TelecomFacility>(ref m_TelecomFacilityType);
			BufferAccessor<Efficiency> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Efficiency>(ref m_BuildingEfficiencyType);
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Temp> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			BufferAccessor<InstalledUpgrade> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Transform transform = nativeArray[i];
				PrefabRef prefabRef = nativeArray3[i];
				ObjectGeometryData objectGeometryData = m_ObjectGeometryData[prefabRef.m_Prefab];
				TelecomFacilityData data = m_PrefabTelecomFacilityData[prefabRef.m_Prefab];
				if (bufferAccessor2.Length != 0)
				{
					UpgradeUtils.CombineStats<TelecomFacilityData>(ref data, bufferAccessor2[i], ref m_PrefabRefData, ref m_PrefabTelecomFacilityData);
				}
				float efficiencyFactor = GetEfficiencyFactor(nativeArray2, nativeArray4, bufferAccessor, i);
				CityUtils.ApplyModifier(ref data.m_NetworkCapacity, cityModifiers, CityModifierType.TelecomCapacity);
				data.m_Range *= math.sqrt(efficiencyFactor);
				data.m_NetworkCapacity *= efficiencyFactor;
				if (data.m_Range < 1f || data.m_NetworkCapacity < 1f)
				{
					continue;
				}
				float3 position = transform.m_Position;
				position.y += objectGeometryData.m_Size.y;
				int2 val = math.max(CellMapSystem<TelecomCoverage>.GetCell(position - data.m_Range, CellMapSystem<TelecomCoverage>.kMapSize, 128), int2.op_Implicit(0));
				int2 val2 = math.min(CellMapSystem<TelecomCoverage>.GetCell(position + data.m_Range, CellMapSystem<TelecomCoverage>.kMapSize, 128) + 1, int2.op_Implicit(128));
				int2 val3 = val2 - val;
				if (math.any(val3 <= 0))
				{
					continue;
				}
				int length = signalStrengths.Length;
				signalStrengths.Resize(length + val3.x * val3.y, (NativeArrayOptions)0);
				NativeArray<float> subArray = signalStrengths.AsArray().GetSubArray(length, val3.x * val3.y);
				if (data.m_PenetrateTerrain)
				{
					CalculateSignalStrength(subArray, val, val2, data.m_Range, position);
				}
				else
				{
					ResetObstructAngles(obstructSlopes, val, val2);
					int2 val4 = math.clamp(CellMapSystem<TelecomCoverage>.GetCell(position, CellMapSystem<TelecomCoverage>.kMapSize, 128), int2.op_Implicit(0), int2.op_Implicit(127));
					CalculateCellSignalStrength(obstructSlopes, subArray, val4, val, val2, data.m_Range, position);
					int2 val5 = val4;
					int2 val6 = val4 + 1;
					while (math.any((val5 > val) | (val6 < val2)))
					{
						if (val5.y > val.y)
						{
							val5.y--;
							for (int j = val4.x; j < val6.x; j++)
							{
								CalculateCellSignalStrength(obstructSlopes, subArray, new int2(j, val5.y), val, val2, data.m_Range, position);
							}
							for (int num = val4.x - 1; num >= val5.x; num--)
							{
								CalculateCellSignalStrength(obstructSlopes, subArray, new int2(num, val5.y), val, val2, data.m_Range, position);
							}
						}
						if (val6.y < val2.y)
						{
							for (int k = val4.x; k < val6.x; k++)
							{
								CalculateCellSignalStrength(obstructSlopes, subArray, new int2(k, val6.y), val, val2, data.m_Range, position);
							}
							for (int num2 = val4.x - 1; num2 >= val5.x; num2--)
							{
								CalculateCellSignalStrength(obstructSlopes, subArray, new int2(num2, val6.y), val, val2, data.m_Range, position);
							}
							val6.y++;
						}
						if (val5.x > val.x)
						{
							val5.x--;
							for (int l = val4.y; l < val6.y; l++)
							{
								CalculateCellSignalStrength(obstructSlopes, subArray, new int2(val5.x, l), val, val2, data.m_Range, position);
							}
							for (int num3 = val4.y - 1; num3 >= val5.y; num3--)
							{
								CalculateCellSignalStrength(obstructSlopes, subArray, new int2(val5.x, num3), val, val2, data.m_Range, position);
							}
						}
						if (val6.x < val2.x)
						{
							for (int m = val4.y; m < val6.y; m++)
							{
								CalculateCellSignalStrength(obstructSlopes, subArray, new int2(val6.x, m), val, val2, data.m_Range, position);
							}
							for (int num4 = val4.y - 1; num4 >= val5.y; num4--)
							{
								CalculateCellSignalStrength(obstructSlopes, subArray, new int2(val6.x, num4), val, val2, data.m_Range, position);
							}
							val6.x++;
						}
					}
				}
				AddSignalStrengths(facilityData, subArray, val, val2);
			}
		}

		private float GetEfficiencyFactor(NativeArray<Game.Buildings.TelecomFacility> telecomFacilities, NativeArray<Temp> temps, BufferAccessor<Efficiency> efficiencyAccessor, int i)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			float result = 1f;
			if (temps.Length != 0)
			{
				Temp temp = temps[i];
				DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
				if (m_BuildingEfficiencyData.TryGetBuffer(temp.m_Original, ref buffer))
				{
					Game.Buildings.TelecomFacility telecomFacility = m_TelecomFacilityData[temp.m_Original];
					if (!m_Preview || (telecomFacility.m_Flags & TelecomFacilityFlags.HasCoverage) != 0)
					{
						result = BuildingUtils.GetEfficiency(buffer);
					}
				}
			}
			else if (efficiencyAccessor.Length != 0)
			{
				Game.Buildings.TelecomFacility telecomFacility2 = telecomFacilities[i];
				if (!m_Preview || (telecomFacility2.m_Flags & TelecomFacilityFlags.HasCoverage) != 0)
				{
					result = BuildingUtils.GetEfficiency(efficiencyAccessor[i]);
				}
			}
			return result;
		}

		private void AddSignalStrengths(NativeArray<CellFacilityData> facilityData, NativeArray<float> signalStrengthArray, int2 min, int2 max)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			int2 val = max - min;
			int num = 128 * min.y;
			int num2 = -min.x;
			for (int i = min.y; i < max.y; i++)
			{
				for (int j = min.x; j < max.x; j++)
				{
					float num3 = signalStrengthArray[num2 + j];
					int num4 = num + j;
					CellFacilityData cellFacilityData = facilityData[num4];
					cellFacilityData.m_SignalStrength = 1f - (1f - cellFacilityData.m_SignalStrength) * (1f - num3);
					cellFacilityData.m_AccumulatedSignalStrength += num3;
					facilityData[num4] = cellFacilityData;
				}
				num += 128;
				num2 += val.x;
			}
		}

		private void CalculateSignalStrength(NativeArray<float> signalStrengthArray, int2 min, int2 max, float range, float3 position)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			int2 val = max - min;
			int num = -min.x;
			for (int i = min.y; i < max.y; i++)
			{
				for (int j = min.x; j < max.x; j++)
				{
					float3 cellCenter = CellMapSystem<TelecomCoverage>.GetCellCenter(new int2(j, i), 128);
					float3 val2 = position - cellCenter;
					float distance = math.length(((float3)(ref val2)).xz);
					signalStrengthArray[num + j] = math.max(0f, CalculateSignalStrength(distance, range));
				}
				num += val.x;
			}
		}

		private void ResetObstructAngles(NativeArray<float> obstructAngles, int2 min, int2 max)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			int2 val = max - min;
			int num = val.x * val.y;
			for (int i = 0; i < num; i++)
			{
				obstructAngles[i] = float.MaxValue;
			}
		}

		private float CalculateSignalStrength(float distance, float range)
		{
			float num = distance / range;
			num *= num;
			return 1f - num;
		}

		private void CalculateCellSignalStrength(NativeArray<float> obstructSlopes, NativeArray<float> signalStrengthArray, int2 cell, int2 min, int2 max, float range, float3 position)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			int2 val = cell - min;
			int2 val2 = max - min;
			int num = val.x + val2.x * val.y;
			float3 cellCenter = CellMapSystem<TelecomCoverage>.GetCellCenter(cell, 128);
			float3 val3 = position - cellCenter;
			float num2 = math.length(((float3)(ref val3)).xz);
			float num3 = CalculateSignalStrength(num2, range);
			if (num3 <= 0f)
			{
				signalStrengthArray[num] = 0f;
				return;
			}
			cellCenter.y = TerrainUtils.SampleHeight(ref m_TerrainHeightData, cellCenter);
			val3.y = position.y - cellCenter.y;
			float num4 = val3.y / math.max(1f, num2);
			float num5 = (float)CellMapSystem<TelecomCoverage>.kMapSize / 128f;
			float2 val4 = math.abs(((float3)(ref val3)).xz);
			int2 val5 = math.clamp(val + math.select((int2)math.sign(((float3)(ref val3)).xz), int2.op_Implicit(0), math.all(val4 < num5)), int2.op_Implicit(0), val2 - 1);
			int2 val6;
			float num6;
			if (val4.x >= val4.y)
			{
				val6 = val5.x + val2.x * new int2(val.y, val5.y);
				num6 = val4.y / math.max(1f, val4.x);
			}
			else
			{
				val6 = new int2(val.x, val5.x) + val2.x * val5.y;
				num6 = val4.x / math.max(1f, val4.y);
			}
			float2 val7 = default(float2);
			((float2)(ref val7))._002Ector(obstructSlopes[val6.x], obstructSlopes[val6.y]);
			float2 val8 = math.saturate((val7 - num4) * 20f + 1f);
			obstructSlopes[num] = math.min(math.lerp(val7.x, val7.y, num6), num4);
			signalStrengthArray[num] = num3 * math.lerp(val8.x, val8.y, num6);
		}

		private void AddDensity(NativeArray<CellDensityData> densityData, ArchetypeChunk chunk)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<PropertyRenter> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PropertyRenter>(ref m_PropertyRenterType);
			if (nativeArray.Length != 0)
			{
				BufferAccessor<HouseholdCitizen> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenType);
				BufferAccessor<Employee> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
				for (int i = 0; i < bufferAccessor.Length; i++)
				{
					PropertyRenter propertyRenter = nativeArray[i];
					DynamicBuffer<HouseholdCitizen> val = bufferAccessor[i];
					if (val.Length != 0 && m_TransformData.HasComponent(propertyRenter.m_Property))
					{
						Transform transform = m_TransformData[propertyRenter.m_Property];
						AddDensity(densityData, val.Length, transform.m_Position);
					}
				}
				for (int j = 0; j < bufferAccessor2.Length; j++)
				{
					PropertyRenter propertyRenter2 = nativeArray[j];
					DynamicBuffer<Employee> val2 = bufferAccessor2[j];
					if (val2.Length != 0 && m_TransformData.HasComponent(propertyRenter2.m_Property))
					{
						Transform transform2 = m_TransformData[propertyRenter2.m_Property];
						AddDensity(densityData, val2.Length, transform2.m_Position);
					}
				}
				return;
			}
			NativeArray<Transform> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			if (nativeArray2.Length == 0)
			{
				return;
			}
			BufferAccessor<HouseholdCitizen> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<HouseholdCitizen>(ref m_HouseholdCitizenType);
			BufferAccessor<Employee> bufferAccessor4 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Employee>(ref m_EmployeeType);
			for (int k = 0; k < bufferAccessor3.Length; k++)
			{
				Transform transform3 = nativeArray2[k];
				DynamicBuffer<HouseholdCitizen> val3 = bufferAccessor3[k];
				if (val3.Length != 0)
				{
					AddDensity(densityData, val3.Length, transform3.m_Position);
				}
			}
			for (int l = 0; l < bufferAccessor4.Length; l++)
			{
				Transform transform4 = nativeArray2[l];
				DynamicBuffer<Employee> val4 = bufferAccessor4[l];
				if (val4.Length != 0)
				{
					AddDensity(densityData, val4.Length, transform4.m_Position);
				}
			}
		}

		private void AddDensity(NativeArray<CellDensityData> densityData, int density, float3 position)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			int2 val = math.clamp(CellMapSystem<TelecomCoverage>.GetCell(position, CellMapSystem<TelecomCoverage>.kMapSize, 128), int2.op_Implicit(0), int2.op_Implicit(127));
			int num = val.x + 128 * val.y;
			CellDensityData cellDensityData = densityData[num];
			cellDensityData.m_Density = (ushort)math.min(65535, cellDensityData.m_Density + density);
			densityData[num] = cellDensityData;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.TelecomFacility> __Game_Buildings_TelecomFacility_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Efficiency> __Game_Buildings_Efficiency_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<Employee> __Game_Companies_Employee_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.TelecomFacility> __Game_Buildings_TelecomFacility_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Efficiency> __Game_Buildings_Efficiency_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TelecomFacilityData> __Game_Prefabs_TelecomFacilityData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

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
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertyRenter>(true);
			__Game_Buildings_TelecomFacility_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Buildings.TelecomFacility>(true);
			__Game_Buildings_Efficiency_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Efficiency>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<InstalledUpgrade>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<HouseholdCitizen>(true);
			__Game_Companies_Employee_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Employee>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Buildings_TelecomFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.TelecomFacility>(true);
			__Game_Buildings_Efficiency_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Efficiency>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Prefabs_TelecomFacilityData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TelecomFacilityData>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	public const int TEXTURE_SIZE = 128;

	private TerrainSystem m_TerrainSystem;

	private CitySystem m_CitySystem;

	private EntityQuery m_DensityQuery;

	private EntityQuery m_FacilityQuery;

	private NativeArray<TelecomStatus> m_Status;

	private TypeHandle __TypeHandle;

	public int2 TextureSize => new int2(128, 128);

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 4096;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.ReadOnly<Employee>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_DensityQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_FacilityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Game.Buildings.TelecomFacility>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Game.Buildings.ServiceUpgrade>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_Status = new NativeArray<TelecomStatus>(0, (Allocator)4, (NativeArrayOptions)1);
		CreateTextures(128);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Status.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		if (m_TerrainSystem.GetHeightData().isCreated)
		{
			JobHandle val = default(JobHandle);
			NativeList<ArchetypeChunk> densityChunks = ((EntityQuery)(ref m_DensityQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
			JobHandle val2 = default(JobHandle);
			NativeList<ArchetypeChunk> facilityChunks = ((EntityQuery)(ref m_FacilityQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val2);
			JobHandle dependencies;
			TelecomCoverageJob obj = new TelecomCoverageJob
			{
				m_DensityChunks = densityChunks,
				m_FacilityChunks = facilityChunks,
				m_TerrainHeightData = m_TerrainSystem.GetHeightData(),
				m_City = m_CitySystem.City,
				m_Preview = false,
				m_TelecomCoverage = GetMap(readOnly: false, out dependencies),
				m_TelecomStatus = m_Status,
				m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenterType = InternalCompilerInterface.GetComponentTypeHandle<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TelecomFacilityType = InternalCompilerInterface.GetComponentTypeHandle<Game.Buildings.TelecomFacility>(ref __TypeHandle.__Game_Buildings_TelecomFacility_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingEfficiencyType = InternalCompilerInterface.GetBufferTypeHandle<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_InstalledUpgradeType = InternalCompilerInterface.GetBufferTypeHandle<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_HouseholdCitizenType = InternalCompilerInterface.GetBufferTypeHandle<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_EmployeeType = InternalCompilerInterface.GetBufferTypeHandle<Employee>(ref __TypeHandle.__Game_Companies_Employee_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TelecomFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.TelecomFacility>(ref __TypeHandle.__Game_Buildings_TelecomFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingEfficiencyData = InternalCompilerInterface.GetBufferLookup<Efficiency>(ref __TypeHandle.__Game_Buildings_Efficiency_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabTelecomFacilityData = InternalCompilerInterface.GetComponentLookup<TelecomFacilityData>(ref __TypeHandle.__Game_Prefabs_TelecomFacilityData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			JobHandle val3 = JobHandle.CombineDependencies(val, val2, dependencies);
			JobHandle val4 = IJobExtensions.Schedule<TelecomCoverageJob>(obj, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val3));
			densityChunks.Dispose(val4);
			facilityChunks.Dispose(val4);
			m_TerrainSystem.AddCPUHeightReader(val4);
			AddWriter(val4);
			((SystemBase)this).Dependency = val4;
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
	public TelecomCoverageSystem()
	{
	}
}
