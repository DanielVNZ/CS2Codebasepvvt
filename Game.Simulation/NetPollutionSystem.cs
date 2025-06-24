using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Common;
using Game.Net;
using Game.Prefabs;
using Game.Tools;
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
public class NetPollutionSystem : GameSystemBase
{
	[BurstCompile]
	private struct UpdateNetPollutionJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Node> m_NodeType;

		[ReadOnly]
		public ComponentTypeHandle<Curve> m_CurveType;

		[ReadOnly]
		public ComponentTypeHandle<Upgraded> m_UpgradedType;

		[ReadOnly]
		public ComponentTypeHandle<Elevation> m_ElevationType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> m_ConnectedEdgeType;

		public ComponentTypeHandle<Game.Net.Pollution> m_PollutionType;

		[ReadOnly]
		public ComponentLookup<Edge> m_EdgeData;

		[ReadOnly]
		public ComponentLookup<Upgraded> m_UpgradedData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<NetPollutionData> m_NetPollutionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		public int m_MapSize;

		public int m_AirPollutionTextureSize;

		public int m_NoisePollutionTextureSize;

		public NativeArray<AirPollution> m_AirPollutionMap;

		public NativeArray<NoisePollution> m_NoisePollutionMap;

		[ReadOnly]
		public PollutionParameterData m_PollutionParameters;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_030c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0322: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_032d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0339: Unknown result type (might be due to invalid IL or missing references)
			//IL_0347: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_040e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0375: Unknown result type (might be due to invalid IL or missing references)
			//IL_0397: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_042a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0278: Unknown result type (might be due to invalid IL or missing references)
			//IL_027d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			PollutionParameterData pollutionParameters = m_PollutionParameters;
			float num = 4f / (float)kUpdatesPerDay;
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Node> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Node>(ref m_NodeType);
			NativeArray<Game.Net.Pollution> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Net.Pollution>(ref m_PollutionType);
			NativeArray<Elevation> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Elevation>(ref m_ElevationType);
			if (nativeArray2.Length != 0)
			{
				NativeArray<Entity> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
				BufferAccessor<ConnectedEdge> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ConnectedEdge>(ref m_ConnectedEdgeType);
				NetPollutionData netPollutionData = default(NetPollutionData);
				Elevation elevation = default(Elevation);
				bool2 val5 = default(bool2);
				Composition composition = default(Composition);
				Upgraded upgraded = default(Upgraded);
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					PrefabRef prefabRef = nativeArray[i];
					ref Game.Net.Pollution reference = ref CollectionUtils.ElementAt<Game.Net.Pollution>(nativeArray3, i);
					reference.m_Accumulation = math.lerp(reference.m_Accumulation, reference.m_Pollution, num);
					reference.m_Pollution = default(float2);
					if (!m_NetPollutionData.TryGetComponent(prefabRef.m_Prefab, ref netPollutionData))
					{
						continue;
					}
					Entity val = nativeArray5[i];
					Node node = nativeArray2[i];
					float2 val2 = reference.m_Accumulation * netPollutionData.m_Factors;
					float4 val3 = float4.op_Implicit(0f);
					float num2 = pollutionParameters.m_NetNoiseRadius;
					bool flag = CollectionUtils.TryGet<Elevation>(nativeArray4, i, ref elevation) && math.all(elevation.m_Elevation < 0f);
					DynamicBuffer<ConnectedEdge> val4 = bufferAccessor[i];
					for (int j = 0; j < val4.Length; j++)
					{
						ConnectedEdge connectedEdge = val4[j];
						Edge edge = m_EdgeData[connectedEdge.m_Edge];
						((bool2)(ref val5))._002Ector(edge.m_Start == val, edge.m_End == val);
						if (!math.any(val5))
						{
							continue;
						}
						float3 noisePollution = float3.op_Implicit(val2.x);
						if (m_CompositionData.TryGetComponent(connectedEdge.m_Edge, ref composition))
						{
							NetCompositionData netCompositionData = m_NetCompositionData[val5.x ? composition.m_StartNode : composition.m_EndNode];
							num2 = math.max(num2, netCompositionData.m_Width * 0.5f);
							if (flag)
							{
								flag = (netCompositionData.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0;
							}
						}
						if (m_UpgradedData.TryGetComponent(connectedEdge.m_Edge, ref upgraded))
						{
							CheckUpgrades(ref noisePollution, upgraded);
						}
						val3 += new float4(noisePollution, 1f);
					}
					if (!flag)
					{
						if (val3.w != 0f)
						{
							val3 /= val3.w;
							val3.x = (val3.x + val3.z) * 0.5f;
						}
						ApplyPollution(node.m_Position, num2, ((float4)(ref val3)).xy, val2.y, ref pollutionParameters);
					}
				}
				return;
			}
			NativeArray<Curve> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Curve>(ref m_CurveType);
			NativeArray<Upgraded> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Upgraded>(ref m_UpgradedType);
			NativeArray<Composition> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
			NetPollutionData netPollutionData2 = default(NetPollutionData);
			Composition composition2 = default(Composition);
			Elevation elevation2 = default(Elevation);
			Upgraded upgraded2 = default(Upgraded);
			for (int k = 0; k < nativeArray6.Length; k++)
			{
				PrefabRef prefabRef2 = nativeArray[k];
				ref Game.Net.Pollution reference2 = ref CollectionUtils.ElementAt<Game.Net.Pollution>(nativeArray3, k);
				reference2.m_Accumulation = math.lerp(reference2.m_Accumulation, reference2.m_Pollution, num);
				reference2.m_Pollution = default(float2);
				if (!m_NetPollutionData.TryGetComponent(prefabRef2.m_Prefab, ref netPollutionData2))
				{
					continue;
				}
				float num3 = pollutionParameters.m_NetNoiseRadius;
				if (CollectionUtils.TryGet<Composition>(nativeArray8, k, ref composition2))
				{
					NetCompositionData netCompositionData2 = m_NetCompositionData[composition2.m_Edge];
					num3 = math.max(num3, netCompositionData2.m_Width * 0.5f);
					if (CollectionUtils.TryGet<Elevation>(nativeArray4, k, ref elevation2) && math.all(elevation2.m_Elevation < 0f) && (netCompositionData2.m_Flags.m_General & CompositionFlags.General.Tunnel) != 0)
					{
						continue;
					}
				}
				Curve curve = nativeArray6[k];
				float2 val6 = reference2.m_Accumulation * netPollutionData2.m_Factors;
				float3 noisePollution2 = float3.op_Implicit(val6.x);
				noisePollution2.y *= 2f;
				if (CollectionUtils.TryGet<Upgraded>(nativeArray7, k, ref upgraded2))
				{
					CheckUpgrades(ref noisePollution2, upgraded2);
				}
				ApplyPollution(curve, num3, noisePollution2, val6.y, ref pollutionParameters);
			}
		}

		private void CheckUpgrades(ref float3 noisePollution, Upgraded upgraded)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0256: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			if ((upgraded.m_Flags.m_Left & upgraded.m_Flags.m_Right & CompositionFlags.Side.SoundBarrier) != 0)
			{
				noisePollution *= new float3(0f, 0.5f, 0f);
			}
			else if ((upgraded.m_Flags.m_Left & CompositionFlags.Side.SoundBarrier) != 0)
			{
				noisePollution *= new float3(0f, 0.5f, 1.5f);
			}
			else if ((upgraded.m_Flags.m_Right & CompositionFlags.Side.SoundBarrier) != 0)
			{
				noisePollution *= new float3(1.5f, 0.5f, 0f);
			}
			if ((upgraded.m_Flags.m_Left & upgraded.m_Flags.m_Right & CompositionFlags.Side.PrimaryBeautification) != 0)
			{
				noisePollution *= new float3(0.5f, 0.5f, 0.5f);
			}
			else if ((upgraded.m_Flags.m_Left & CompositionFlags.Side.PrimaryBeautification) != 0)
			{
				noisePollution *= new float3(0.5f, 0.75f, 1f);
			}
			else if ((upgraded.m_Flags.m_Right & CompositionFlags.Side.PrimaryBeautification) != 0)
			{
				noisePollution *= new float3(1f, 0.75f, 0.5f);
			}
			if ((upgraded.m_Flags.m_Left & upgraded.m_Flags.m_Right & CompositionFlags.Side.SecondaryBeautification) != 0)
			{
				noisePollution *= new float3(0.5f, 0.5f, 0.5f);
			}
			else if ((upgraded.m_Flags.m_Left & CompositionFlags.Side.SecondaryBeautification) != 0)
			{
				noisePollution *= new float3(0.5f, 0.75f, 1f);
			}
			else if ((upgraded.m_Flags.m_Right & CompositionFlags.Side.SecondaryBeautification) != 0)
			{
				noisePollution *= new float3(1f, 0.75f, 0.5f);
			}
			if ((upgraded.m_Flags.m_General & CompositionFlags.General.PrimaryMiddleBeautification) != 0)
			{
				noisePollution *= new float3(0.875f, 0.5f, 0.875f);
			}
			if ((upgraded.m_Flags.m_General & CompositionFlags.General.SecondaryMiddleBeautification) != 0)
			{
				noisePollution *= new float3(0.875f, 0.5f, 0.875f);
			}
		}

		private void ApplyPollution(float3 position, float radius, float2 noisePollution, float airPollution, ref PollutionParameterData pollutionParameters)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			if (airPollution != 0f)
			{
				short amount = (short)(pollutionParameters.m_NetAirMultiplier * airPollution);
				AddAirPollution(position, amount);
			}
			if (math.any(noisePollution != 0f))
			{
				int2 val = (int2)(pollutionParameters.m_NetNoiseMultiplier * noisePollution / 8f);
				if (radius > pollutionParameters.m_NetNoiseRadius)
				{
					AddNoise(position + new float3(radius * -0.33333f, 0f, radius * -0.33333f), (short)val.y);
					AddNoise(position + new float3(radius * 0.33333f, 0f, radius * -0.33333f), (short)val.y);
					AddNoise(position + new float3(radius * -0.33333f, 0f, radius * 0.33333f), (short)val.y);
					AddNoise(position + new float3(radius * 0.33333f, 0f, radius * 0.33333f), (short)val.y);
				}
				else
				{
					AddNoise(position, (short)(4 * val.y));
				}
				AddNoise(position + new float3(0f - radius, 0f, 0f), (short)val.x);
				AddNoise(position + new float3(radius, 0f, 0f), (short)val.x);
				AddNoise(position + new float3(0f, 0f, radius), (short)val.x);
				AddNoise(position + new float3(0f, 0f, 0f - radius), (short)val.x);
			}
		}

		private void ApplyPollution(Curve curve, float radius, float3 noisePollution, float airPollution, ref PollutionParameterData pollutionParameters)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			if (airPollution != 0f)
			{
				float num = (float)m_MapSize / (float)m_AirPollutionTextureSize;
				int num2 = Mathf.CeilToInt(2f * curve.m_Length / num);
				short amount = (short)(pollutionParameters.m_NetAirMultiplier * airPollution / (float)num2);
				for (int i = 1; i <= num2; i++)
				{
					float3 position = MathUtils.Position(curve.m_Bezier, (float)i / ((float)num2 + 1f));
					AddAirPollution(position, amount);
				}
			}
			if (!math.any(noisePollution != 0f))
			{
				return;
			}
			float num3 = (float)m_MapSize / (float)m_NoisePollutionTextureSize;
			int num4 = Mathf.CeilToInt(2f * curve.m_Length / num3);
			int3 val = (int3)(pollutionParameters.m_NetNoiseMultiplier * noisePollution / (4f * (float)num4));
			if (radius > pollutionParameters.m_NetNoiseRadius)
			{
				val.y >>= 1;
			}
			for (int j = 1; j <= num4; j++)
			{
				float num5 = (float)j / ((float)num4 + 1f);
				float3 val2 = MathUtils.Position(curve.m_Bezier, num5);
				float3 val3 = MathUtils.Tangent(curve.m_Bezier, num5);
				val3 = math.normalize(new float3(0f - val3.z, 0f, val3.x));
				if (radius > pollutionParameters.m_NetNoiseRadius)
				{
					AddNoise(val2 + radius * 0.33333f * val3, (short)val.y);
					AddNoise(val2 - radius * 0.33333f * val3, (short)val.y);
				}
				else
				{
					AddNoise(val2, (short)val.y);
				}
				if (val.x != 0)
				{
					AddNoise(val2 + radius * val3, (short)val.x);
				}
				if (val.z != 0)
				{
					AddNoise(val2 - radius * val3, (short)val.z);
				}
			}
		}

		private void AddAirPollution(float3 position, short amount)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			int2 cell = CellMapSystem<AirPollution>.GetCell(position, m_MapSize, m_AirPollutionTextureSize);
			if (math.all((cell >= 0) & (cell < m_AirPollutionTextureSize)))
			{
				int num = cell.x + cell.y * m_AirPollutionTextureSize;
				AirPollution airPollution = m_AirPollutionMap[num];
				airPollution.Add(amount);
				m_AirPollutionMap[num] = airPollution;
			}
		}

		private void AddNoise(float3 position, short amount)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			float2 cellCoords = CellMapSystem<NoisePollution>.GetCellCoords(position, m_MapSize, m_NoisePollutionTextureSize);
			float2 val = math.frac(cellCoords);
			float2 val2 = ((val.x < 0.5f) ? new float2(0f, 1f) : new float2(1f, 0f));
			float2 val3 = ((val.y < 0.5f) ? new float2(0f, 1f) : new float2(1f, 0f));
			int2 cell = default(int2);
			((int2)(ref cell))._002Ector(Mathf.FloorToInt(cellCoords.x - val2.y), Mathf.FloorToInt(cellCoords.y - val3.y));
			AddNoiseSingle(cell, (short)((0.5 + (double)val2.x - (double)val.x) * (0.5 + (double)val3.x - (double)val.y) * (double)amount));
			cell.x++;
			AddNoiseSingle(cell, (short)((-0.5 + (double)val2.y + (double)val.x) * (0.5 + (double)val3.x - (double)val.y) * (double)amount));
			cell.y++;
			AddNoiseSingle(cell, (short)((-0.5 + (double)val2.y + (double)val.x) * (-0.5 + (double)val3.y + (double)val.y) * (double)amount));
			cell.x--;
			AddNoiseSingle(cell, (short)((0.5 + (double)val2.x - (double)val.x) * (-0.5 + (double)val3.y + (double)val.y) * (double)amount));
		}

		private void AddNoiseSingle(int2 cell, short amount)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (math.all((cell >= 0) & (cell < m_NoisePollutionTextureSize)))
			{
				int num = cell.x + cell.y * m_NoisePollutionTextureSize;
				NoisePollution noisePollution = m_NoisePollutionMap[num];
				noisePollution.Add(amount);
				m_NoisePollutionMap[num] = noisePollution;
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
		public ComponentTypeHandle<Node> __Game_Net_Node_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Curve> __Game_Net_Curve_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Upgraded> __Game_Net_Upgraded_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Elevation> __Game_Net_Elevation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<ConnectedEdge> __Game_Net_ConnectedEdge_RO_BufferTypeHandle;

		public ComponentTypeHandle<Game.Net.Pollution> __Game_Net_Pollution_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Edge> __Game_Net_Edge_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Upgraded> __Game_Net_Upgraded_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetPollutionData> __Game_Prefabs_NetPollutionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_Node_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Node>(true);
			__Game_Net_Curve_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Curve>(true);
			__Game_Net_Upgraded_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Upgraded>(true);
			__Game_Net_Elevation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Elevation>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Net_ConnectedEdge_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<ConnectedEdge>(true);
			__Game_Net_Pollution_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Net.Pollution>(false);
			__Game_Net_Edge_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Edge>(true);
			__Game_Net_Upgraded_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Upgraded>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Prefabs_NetPollutionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetPollutionData>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
		}
	}

	public static readonly int kUpdatesPerDay = 128;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_PollutionQuery;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private EntityQuery m_PollutionParameterQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 262144 / (kUpdatesPerDay * 16);
	}

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
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_PollutionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadWrite<Game.Net.Pollution>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_PollutionParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_PollutionQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_PollutionParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrame = SimulationUtils.GetUpdateFrame(m_SimulationSystem.frameIndex, kUpdatesPerDay, 16);
		((EntityQuery)(ref m_PollutionQuery)).ResetFilter();
		((EntityQuery)(ref m_PollutionQuery)).SetSharedComponentFilter<UpdateFrame>(new UpdateFrame(updateFrame));
		JobHandle dependencies;
		JobHandle dependencies2;
		JobHandle val = JobChunkExtensions.Schedule<UpdateNetPollutionJob>(new UpdateNetPollutionJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NodeType = InternalCompilerInterface.GetComponentTypeHandle<Node>(ref __TypeHandle.__Game_Net_Node_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurveType = InternalCompilerInterface.GetComponentTypeHandle<Curve>(ref __TypeHandle.__Game_Net_Curve_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedType = InternalCompilerInterface.GetComponentTypeHandle<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationType = InternalCompilerInterface.GetComponentTypeHandle<Elevation>(ref __TypeHandle.__Game_Net_Elevation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_ConnectedEdgeType = InternalCompilerInterface.GetBufferTypeHandle<ConnectedEdge>(ref __TypeHandle.__Game_Net_ConnectedEdge_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PollutionType = InternalCompilerInterface.GetComponentTypeHandle<Game.Net.Pollution>(ref __TypeHandle.__Game_Net_Pollution_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EdgeData = InternalCompilerInterface.GetComponentLookup<Edge>(ref __TypeHandle.__Game_Net_Edge_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_UpgradedData = InternalCompilerInterface.GetComponentLookup<Upgraded>(ref __TypeHandle.__Game_Net_Upgraded_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetPollutionData = InternalCompilerInterface.GetComponentLookup<NetPollutionData>(ref __TypeHandle.__Game_Prefabs_NetPollutionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_NetCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AirPollutionMap = m_AirPollutionSystem.GetMap(readOnly: false, out dependencies),
			m_NoisePollutionMap = m_NoisePollutionSystem.GetMap(readOnly: false, out dependencies2),
			m_AirPollutionTextureSize = AirPollutionSystem.kTextureSize,
			m_NoisePollutionTextureSize = NoisePollutionSystem.kTextureSize,
			m_MapSize = CellMapSystem<AirPollution>.kMapSize,
			m_PollutionParameters = ((EntityQuery)(ref m_PollutionParameterQuery)).GetSingleton<PollutionParameterData>()
		}, m_PollutionQuery, JobHandle.CombineDependencies(dependencies, dependencies2, ((SystemBase)this).Dependency));
		m_AirPollutionSystem.AddWriter(val);
		m_NoisePollutionSystem.AddWriter(val);
		((SystemBase)this).Dependency = val;
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
	public NetPollutionSystem()
	{
	}
}
