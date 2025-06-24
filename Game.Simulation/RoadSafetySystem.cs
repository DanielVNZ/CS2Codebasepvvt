using System.Runtime.CompilerServices;
using Game.Areas;
using Game.City;
using Game.Common;
using Game.Events;
using Game.Net;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class RoadSafetySystem : GameSystemBase
{
	[BurstCompile]
	private struct RoadSafetyJob : IJobChunk
	{
		[ReadOnly]
		public NativeList<ArchetypeChunk> m_FirePrefabChunks;

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<NetCondition> m_NetConditionType;

		[ReadOnly]
		public ComponentTypeHandle<Road> m_RoadType;

		[ReadOnly]
		public ComponentTypeHandle<Composition> m_CompositionType;

		[ReadOnly]
		public ComponentTypeHandle<BorderDistrict> m_BorderDistrictType;

		[ReadOnly]
		public ComponentTypeHandle<EventData> m_PrefabEventType;

		[ReadOnly]
		public ComponentTypeHandle<TrafficAccidentData> m_PrefabTrafficAccidentType;

		[ReadOnly]
		public ComponentTypeHandle<Locked> m_LockedType;

		[ReadOnly]
		public ComponentLookup<RoadComposition> m_RoadCompositionData;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		[ReadOnly]
		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public float4 m_TimeFactors;

		[ReadOnly]
		public float m_Brightness;

		public ParallelWriter m_CommandBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_024e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			if (((Random)(ref random)).NextInt(64) != 0)
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<NetCondition> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<NetCondition>(ref m_NetConditionType);
			NativeArray<Road> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Road>(ref m_RoadType);
			NativeArray<Composition> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Composition>(ref m_CompositionType);
			NativeArray<BorderDistrict> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<BorderDistrict>(ref m_BorderDistrictType);
			DynamicBuffer<CityModifier> modifiers = m_CityModifiers[m_City];
			RoadComposition roadComposition = default(RoadComposition);
			DynamicBuffer<DistrictModifier> modifiers2 = default(DynamicBuffer<DistrictModifier>);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				NetCondition netCondition = nativeArray2[i];
				Road road = nativeArray3[i];
				Composition composition = nativeArray4[i];
				float duration = math.dot(road.m_TrafficFlowDuration0 + road.m_TrafficFlowDuration1, m_TimeFactors) * 2.6666667f;
				float num = math.dot(road.m_TrafficFlowDistance0 + road.m_TrafficFlowDistance1, m_TimeFactors) * 2.6666667f;
				if (num < 0.01f || !m_RoadCompositionData.TryGetComponent(composition.m_Edge, ref roadComposition))
				{
					continue;
				}
				float trafficFlowSpeed = NetUtils.GetTrafficFlowSpeed(duration, num);
				float num2 = math.select(0.7f, 0.9f, (roadComposition.m_Flags & Game.Prefabs.RoadFlags.HasStreetLights) != 0 && (road.m_Flags & Game.Net.RoadFlags.LightsOff) == 0);
				float num3 = 500f / math.sqrt(num);
				num3 *= math.lerp(0.5f, 1f, trafficFlowSpeed);
				num3 *= math.lerp(1f, 0.75f, math.csum(netCondition.m_Wear) * 0.05f);
				num3 *= math.lerp(num2, 1f, math.min(1f, m_Brightness * 2f));
				if ((roadComposition.m_Flags & Game.Prefabs.RoadFlags.SeparatedCarriageways) != 0)
				{
					num3 *= 1.1f;
				}
				if ((roadComposition.m_Flags & Game.Prefabs.RoadFlags.UseHighwayRules) == 0 && nativeArray5.Length != 0)
				{
					float2 val = float2.op_Implicit(num3);
					BorderDistrict borderDistrict = nativeArray5[i];
					if (m_DistrictModifiers.TryGetBuffer(borderDistrict.m_Left, ref modifiers2))
					{
						AreaUtils.ApplyModifier(ref val.x, modifiers2, DistrictModifierType.StreetTrafficSafety);
					}
					if (m_DistrictModifiers.TryGetBuffer(borderDistrict.m_Right, ref modifiers2))
					{
						AreaUtils.ApplyModifier(ref val.y, modifiers2, DistrictModifierType.StreetTrafficSafety);
					}
					num3 = ((!(math.cmax(val) >= num3)) ? math.min(num3, math.cmax(val)) : math.max(num3, math.cmin(val)));
				}
				if ((roadComposition.m_Flags & Game.Prefabs.RoadFlags.UseHighwayRules) != 0)
				{
					CityUtils.ApplyModifier(ref num3, modifiers, CityModifierType.HighwayTrafficSafety);
				}
				TryStartAccident(unfilteredChunkIndex, ref random, entity, num3, EventTargetType.Road);
			}
		}

		private void TryStartAccident(int jobIndex, ref Random random, Entity entity, float roadSafety, EventTargetType targetType)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_FirePrefabChunks.Length; i++)
			{
				ArchetypeChunk val = m_FirePrefabChunks[i];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val)).GetNativeArray(m_EntityType);
				NativeArray<EventData> nativeArray2 = ((ArchetypeChunk)(ref val)).GetNativeArray<EventData>(ref m_PrefabEventType);
				NativeArray<TrafficAccidentData> nativeArray3 = ((ArchetypeChunk)(ref val)).GetNativeArray<TrafficAccidentData>(ref m_PrefabTrafficAccidentType);
				EnabledMask enabledMask = ((ArchetypeChunk)(ref val)).GetEnabledMask<Locked>(ref m_LockedType);
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					TrafficAccidentData trafficAccidentData = nativeArray3[j];
					if (trafficAccidentData.m_RandomSiteType != targetType)
					{
						continue;
					}
					SafeBitRef enableBit = ((EnabledMask)(ref enabledMask)).EnableBit;
					if (!((SafeBitRef)(ref enableBit)).IsValid || !((EnabledMask)(ref enabledMask))[j])
					{
						float num = trafficAccidentData.m_OccurenceProbability / math.max(1f, roadSafety);
						if (((Random)(ref random)).NextFloat(1f) < num)
						{
							CreateAccidentEvent(jobIndex, entity, nativeArray[j], nativeArray2[j], trafficAccidentData);
							return;
						}
					}
				}
			}
		}

		private void CreateAccidentEvent(int jobIndex, Entity targetEntity, Entity eventPrefab, EventData eventData, TrafficAccidentData trafficAccidentData)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, eventData.m_Archetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PrefabRef>(jobIndex, val, new PrefabRef(eventPrefab));
			((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<TargetElement>(jobIndex, val).Add(new TargetElement(targetEntity));
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
		public ComponentTypeHandle<NetCondition> __Game_Net_NetCondition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Road> __Game_Net_Road_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Composition> __Game_Net_Composition_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<BorderDistrict> __Game_Areas_BorderDistrict_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<EventData> __Game_Prefabs_EventData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TrafficAccidentData> __Game_Prefabs_TrafficAccidentData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Locked> __Game_Prefabs_Locked_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<RoadComposition> __Game_Prefabs_RoadComposition_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<DistrictModifier> __Game_Areas_DistrictModifier_RO_BufferLookup;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Net_NetCondition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NetCondition>(true);
			__Game_Net_Road_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Road>(true);
			__Game_Net_Composition_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Composition>(true);
			__Game_Areas_BorderDistrict_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<BorderDistrict>(true);
			__Game_Prefabs_EventData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<EventData>(true);
			__Game_Prefabs_TrafficAccidentData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TrafficAccidentData>(true);
			__Game_Prefabs_Locked_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Locked>(true);
			__Game_Prefabs_RoadComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadComposition>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Areas_DistrictModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DistrictModifier>(true);
		}
	}

	private const int UPDATES_PER_DAY = 64;

	private TimeSystem m_TimeSystem;

	private CitySystem m_CitySystem;

	private LightingSystem m_LightingSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private EntityQuery m_RoadQuery;

	private EntityQuery m_AccidentPrefabQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 4096;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_LightingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LightingSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Edge>(),
			ComponentType.ReadOnly<Composition>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Road>() };
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_RoadQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_AccidentPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<EventData>(),
			ComponentType.ReadOnly<TrafficAccidentData>(),
			ComponentType.Exclude<Locked>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_RoadQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_AccidentPrefabQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		float num = m_TimeSystem.normalizedTime * 4f;
		float4 val = default(float4);
		((float4)(ref val))._002Ector(math.max(num - 3f, 1f - num), 1f - math.abs(num - new float3(1f, 2f, 3f)));
		val = math.saturate(val);
		JobHandle val2 = default(JobHandle);
		RoadSafetyJob roadSafetyJob = new RoadSafetyJob
		{
			m_FirePrefabChunks = ((EntityQuery)(ref m_AccidentPrefabQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_NetConditionType = InternalCompilerInterface.GetComponentTypeHandle<NetCondition>(ref __TypeHandle.__Game_Net_NetCondition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadType = InternalCompilerInterface.GetComponentTypeHandle<Road>(ref __TypeHandle.__Game_Net_Road_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CompositionType = InternalCompilerInterface.GetComponentTypeHandle<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BorderDistrictType = InternalCompilerInterface.GetComponentTypeHandle<BorderDistrict>(ref __TypeHandle.__Game_Areas_BorderDistrict_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabEventType = InternalCompilerInterface.GetComponentTypeHandle<EventData>(ref __TypeHandle.__Game_Prefabs_EventData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTrafficAccidentType = InternalCompilerInterface.GetComponentTypeHandle<TrafficAccidentData>(ref __TypeHandle.__Game_Prefabs_TrafficAccidentData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LockedType = InternalCompilerInterface.GetComponentTypeHandle<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RoadCompositionData = InternalCompilerInterface.GetComponentLookup<RoadComposition>(ref __TypeHandle.__Game_Prefabs_RoadComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictModifiers = InternalCompilerInterface.GetBufferLookup<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_City = m_CitySystem.City,
			m_RandomSeed = RandomSeed.Next(),
			m_TimeFactors = val,
			m_Brightness = m_LightingSystem.dayLightBrightness
		};
		EntityCommandBuffer val3 = m_EndFrameBarrier.CreateCommandBuffer();
		roadSafetyJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val3)).AsParallelWriter();
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<RoadSafetyJob>(roadSafetyJob, m_RoadQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val2));
		m_EndFrameBarrier.AddJobHandleForProducer(val4);
		((SystemBase)this).Dependency = val4;
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
	public RoadSafetySystem()
	{
	}
}
