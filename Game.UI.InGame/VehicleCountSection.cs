using System;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Pathfind;
using Game.Policies;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class VehicleCountSection : InfoSectionBase
{
	private enum Result
	{
		VehicleCount,
		ActiveVehicles,
		VehicleCountMin,
		VehicleCountMax,
		Count
	}

	[BurstCompile]
	private struct CalculateVehicleCountJob : IJob
	{
		[ReadOnly]
		public Entity m_SelectedEntity;

		[ReadOnly]
		public Entity m_SelectedPrefab;

		[ReadOnly]
		public Entity m_Policy;

		[ReadOnly]
		public ComponentLookup<VehicleTiming> m_VehicleTimings;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformations;

		[ReadOnly]
		public ComponentLookup<TransportLineData> m_TransportLineDatas;

		[ReadOnly]
		public ComponentLookup<PolicySliderData> m_PolicySliderDatas;

		[ReadOnly]
		public BufferLookup<RouteVehicle> m_RouteVehicles;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> m_RouteWaypoints;

		[ReadOnly]
		public BufferLookup<RouteSegment> m_RouteSegments;

		[ReadOnly]
		public BufferLookup<RouteModifier> m_RouteModifiers;

		[ReadOnly]
		public BufferLookup<RouteModifierData> m_RouteModifierDatas;

		public NativeArray<int> m_IntResults;

		public NativeReference<float> m_Duration;

		public void Execute()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			TransportLineData transportLineData = m_TransportLineDatas[m_SelectedPrefab];
			DynamicBuffer<RouteVehicle> val = m_RouteVehicles[m_SelectedEntity];
			DynamicBuffer<RouteModifier> modifiers = m_RouteModifiers[m_SelectedEntity];
			PolicySliderData policySliderData = m_PolicySliderDatas[m_Policy];
			float defaultVehicleInterval = transportLineData.m_DefaultVehicleInterval;
			float value = defaultVehicleInterval;
			RouteUtils.ApplyModifier(ref value, modifiers, RouteModifierType.VehicleInterval);
			float num = CalculateStableDuration(transportLineData);
			m_Duration.Value = num;
			m_IntResults[0] = TransportLineSystem.CalculateVehicleCount(value, num);
			m_IntResults[1] = val.Length;
			m_IntResults[2] = CalculateVehicleCountFromAdjustment(policySliderData.m_Range.min, defaultVehicleInterval, num);
			m_IntResults[3] = CalculateVehicleCountFromAdjustment(policySliderData.m_Range.max, defaultVehicleInterval, num);
		}

		private int CalculateVehicleCountFromAdjustment(float policyAdjustment, float interval, float duration)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			RouteModifier modifier = default(RouteModifier);
			Enumerator<RouteModifierData> enumerator = m_RouteModifierDatas[m_Policy].GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RouteModifierData current = enumerator.Current;
					if (current.m_Type == RouteModifierType.VehicleInterval)
					{
						float modifierDelta = RouteModifierInitializeSystem.RouteModifierRefreshData.GetModifierDelta(current, policyAdjustment, m_Policy, m_PolicySliderDatas);
						RouteModifierInitializeSystem.RouteModifierRefreshData.AddModifierData(ref modifier, current, modifierDelta);
						break;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			interval += modifier.m_Delta.x;
			interval += interval * modifier.m_Delta.y;
			return TransportLineSystem.CalculateVehicleCount(interval, duration);
		}

		public static float CalculateAdjustmentFromVehicleCount(int vehicleCount, float originalInterval, float duration, DynamicBuffer<RouteModifierData> modifierDatas, PolicySliderData sliderData)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			float num = TransportLineSystem.CalculateVehicleInterval(duration, vehicleCount);
			RouteModifier modifier = default(RouteModifier);
			Enumerator<RouteModifierData> enumerator = modifierDatas.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					RouteModifierData current = enumerator.Current;
					if (current.m_Type == RouteModifierType.VehicleInterval)
					{
						if (current.m_Mode == ModifierValueMode.Absolute)
						{
							modifier.m_Delta.x = num - originalInterval;
						}
						else
						{
							modifier.m_Delta.y = (0f - originalInterval + num) / originalInterval;
						}
						float deltaFromModifier = RouteModifierInitializeSystem.RouteModifierRefreshData.GetDeltaFromModifier(modifier, current);
						return RouteModifierInitializeSystem.RouteModifierRefreshData.GetPolicyAdjustmentFromModifierDelta(current, deltaFromModifier, sliderData);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
			return -1f;
		}

		public float CalculateStableDuration(TransportLineData transportLineData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<RouteWaypoint> val = m_RouteWaypoints[m_SelectedEntity];
			DynamicBuffer<RouteSegment> val2 = m_RouteSegments[m_SelectedEntity];
			int num = 0;
			for (int i = 0; i < val.Length; i++)
			{
				if (m_VehicleTimings.HasComponent(val[i].m_Waypoint))
				{
					num = i;
					break;
				}
			}
			float num2 = 0f;
			PathInformation pathInformation = default(PathInformation);
			for (int j = 0; j < val.Length; j++)
			{
				int2 val3 = int2.op_Implicit(num + j);
				val3.y++;
				val3 = math.select(val3, val3 - val.Length, val3 >= val.Length);
				Entity waypoint = val[val3.y].m_Waypoint;
				Entity segment = val2[val3.x].m_Segment;
				if (m_PathInformations.TryGetComponent(segment, ref pathInformation))
				{
					num2 += pathInformation.m_Duration;
				}
				if (m_VehicleTimings.HasComponent(waypoint))
				{
					num2 += transportLineData.m_StopDuration;
				}
			}
			return num2;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<TransportLineData> __Game_Prefabs_TransportLineData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PolicySliderData> __Game_Prefabs_PolicySliderData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<VehicleTiming> __Game_Routes_VehicleTiming_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<RouteVehicle> __Game_Routes_RouteVehicle_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteWaypoint> __Game_Routes_RouteWaypoint_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteSegment> __Game_Routes_RouteSegment_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteModifier> __Game_Routes_RouteModifier_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<RouteModifierData> __Game_Prefabs_RouteModifierData_RO_BufferLookup;

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
			__Game_Prefabs_TransportLineData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TransportLineData>(true);
			__Game_Prefabs_PolicySliderData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PolicySliderData>(true);
			__Game_Routes_VehicleTiming_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<VehicleTiming>(true);
			__Game_Pathfind_PathInformation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PathInformation>(true);
			__Game_Routes_RouteVehicle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteVehicle>(true);
			__Game_Routes_RouteWaypoint_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteWaypoint>(true);
			__Game_Routes_RouteSegment_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteSegment>(true);
			__Game_Routes_RouteModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteModifier>(true);
			__Game_Prefabs_RouteModifierData_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteModifierData>(true);
		}
	}

	private PoliciesUISystem m_PoliciesUISystem;

	private Entity m_VehicleCountPolicy;

	private EntityQuery m_ConfigQuery;

	private NativeArray<int> m_IntResults;

	private NativeReference<float> m_DurationResult;

	private TypeHandle __TypeHandle;

	protected override string group => "VehicleCountSection";

	private int vehicleCountMin { get; set; }

	private int vehicleCountMax { get; set; }

	private int vehicleCount { get; set; }

	private int activeVehicles { get; set; }

	private float stableDuration { get; set; }

	protected override void Reset()
	{
		vehicleCountMin = 0;
		vehicleCountMax = 0;
		vehicleCount = 0;
		activeVehicles = 0;
		stableDuration = 0f;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PoliciesUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PoliciesUISystem>();
		m_ConfigQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UITransportConfigurationData>() });
		AddBinding((IBinding)(object)new TriggerBinding<float>(group, "setVehicleCount", (Action<float>)OnSetVehicleCount, (IReader<float>)null));
		m_IntResults = new NativeArray<int>(4, (Allocator)4, (NativeArrayOptions)1);
		m_DurationResult = new NativeReference<float>(0f, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_IntResults.Dispose();
		m_DurationResult.Dispose();
		base.OnDestroy();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_ConfigQuery)).IsEmptyIgnoreFilter)
		{
			UITransportConfigurationPrefab singletonPrefab = m_PrefabSystem.GetSingletonPrefab<UITransportConfigurationPrefab>(m_ConfigQuery);
			m_VehicleCountPolicy = m_PrefabSystem.GetEntity(singletonPrefab.m_VehicleCountPolicy);
		}
	}

	private void OnSetVehicleCount(float newVehicleCount)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<RouteModifierData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<RouteModifierData>(m_VehicleCountPolicy, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		PolicySliderData componentData = ((EntityManager)(ref entityManager)).GetComponentData<PolicySliderData>(m_VehicleCountPolicy);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		float adjustment = CalculateVehicleCountJob.CalculateAdjustmentFromVehicleCount((int)newVehicleCount, ((EntityManager)(ref entityManager)).GetComponentData<TransportLineData>(selectedPrefab).m_DefaultVehicleInterval, stableDuration, buffer, componentData);
		m_PoliciesUISystem.SetPolicy(selectedEntity, m_VehicleCountPolicy, active: true, adjustment);
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<RouteWaypoint>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					return ((EntityManager)(ref entityManager)).HasComponent<Policy>(selectedEntity);
				}
			}
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		base.visible = Visible();
		if (base.visible)
		{
			JobHandle val = IJobExtensions.Schedule<CalculateVehicleCountJob>(new CalculateVehicleCountJob
			{
				m_SelectedEntity = selectedEntity,
				m_SelectedPrefab = selectedPrefab,
				m_Policy = m_VehicleCountPolicy,
				m_TransportLineDatas = InternalCompilerInterface.GetComponentLookup<TransportLineData>(ref __TypeHandle.__Game_Prefabs_TransportLineData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PolicySliderDatas = InternalCompilerInterface.GetComponentLookup<PolicySliderData>(ref __TypeHandle.__Game_Prefabs_PolicySliderData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_VehicleTimings = InternalCompilerInterface.GetComponentLookup<VehicleTiming>(ref __TypeHandle.__Game_Routes_VehicleTiming_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PathInformations = InternalCompilerInterface.GetComponentLookup<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteVehicles = InternalCompilerInterface.GetBufferLookup<RouteVehicle>(ref __TypeHandle.__Game_Routes_RouteVehicle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteWaypoints = InternalCompilerInterface.GetBufferLookup<RouteWaypoint>(ref __TypeHandle.__Game_Routes_RouteWaypoint_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteSegments = InternalCompilerInterface.GetBufferLookup<RouteSegment>(ref __TypeHandle.__Game_Routes_RouteSegment_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteModifiers = InternalCompilerInterface.GetBufferLookup<RouteModifier>(ref __TypeHandle.__Game_Routes_RouteModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_RouteModifierDatas = InternalCompilerInterface.GetBufferLookup<RouteModifierData>(ref __TypeHandle.__Game_Prefabs_RouteModifierData_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IntResults = m_IntResults,
				m_Duration = m_DurationResult
			}, ((SystemBase)this).Dependency);
			((JobHandle)(ref val)).Complete();
		}
	}

	protected override void OnProcess()
	{
		vehicleCountMin = m_IntResults[2];
		vehicleCountMax = m_IntResults[3];
		vehicleCount = m_IntResults[0];
		activeVehicles = m_IntResults[1];
		stableDuration = m_DurationResult.Value;
		base.tooltipTags.Add("TransportLine");
		base.tooltipTags.Add("CargoRoute");
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("vehicleCountMin");
		writer.Write(vehicleCountMin);
		writer.PropertyName("vehicleCountMax");
		writer.Write(vehicleCountMax);
		writer.PropertyName("vehicleCount");
		writer.Write(vehicleCount);
		writer.PropertyName("activeVehicles");
		writer.Write(activeVehicles);
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
	public VehicleCountSection()
	{
	}
}
