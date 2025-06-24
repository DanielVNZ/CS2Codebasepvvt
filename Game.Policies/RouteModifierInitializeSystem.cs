using System;
using System.Runtime.CompilerServices;
using Game.Common;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Policies;

[CompilerGenerated]
public class RouteModifierInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeRouteModifiersJob : IJobChunk
	{
		[ReadOnly]
		public RouteModifierRefreshData m_RouteModifierRefreshData;

		[ReadOnly]
		public BufferTypeHandle<Policy> m_PolicyType;

		public ComponentTypeHandle<Route> m_RouteType;

		public BufferTypeHandle<RouteModifier> m_RouteModifierType;

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
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Route> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Route>(ref m_RouteType);
			BufferAccessor<RouteModifier> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<RouteModifier>(ref m_RouteModifierType);
			BufferAccessor<Policy> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Policy>(ref m_PolicyType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				DynamicBuffer<Policy> policies = bufferAccessor2[i];
				if (policies.Length != 0)
				{
					Route route = nativeArray[i];
					m_RouteModifierRefreshData.RefreshRouteOptions(ref route, policies);
					nativeArray[i] = route;
					if (bufferAccessor.Length != 0)
					{
						DynamicBuffer<RouteModifier> modifiers = bufferAccessor[i];
						m_RouteModifierRefreshData.RefreshRouteModifiers(modifiers, policies);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public struct RouteModifierRefreshData
	{
		public ComponentLookup<PolicySliderData> m_PolicySliderData;

		public ComponentLookup<RouteOptionData> m_RouteOptionData;

		public BufferLookup<RouteModifierData> m_RouteModifierData;

		public RouteModifierRefreshData(SystemBase system)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			m_PolicySliderData = system.GetComponentLookup<PolicySliderData>(true);
			m_RouteOptionData = system.GetComponentLookup<RouteOptionData>(true);
			m_RouteModifierData = system.GetBufferLookup<RouteModifierData>(true);
		}

		public void Update(SystemBase system)
		{
			m_PolicySliderData.Update(system);
			m_RouteOptionData.Update(system);
			m_RouteModifierData.Update(system);
		}

		public void RefreshRouteOptions(ref Route route, DynamicBuffer<Policy> policies)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			route.m_OptionMask = 0u;
			for (int i = 0; i < policies.Length; i++)
			{
				Policy policy = policies[i];
				if ((policy.m_Flags & PolicyFlags.Active) != 0 && m_RouteOptionData.HasComponent(policy.m_Policy))
				{
					RouteOptionData routeOptionData = m_RouteOptionData[policy.m_Policy];
					route.m_OptionMask |= routeOptionData.m_OptionMask;
				}
			}
		}

		public void RefreshRouteModifiers(DynamicBuffer<RouteModifier> modifiers, DynamicBuffer<Policy> policies)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			modifiers.Clear();
			for (int i = 0; i < policies.Length; i++)
			{
				Policy policy = policies[i];
				if ((policy.m_Flags & PolicyFlags.Active) != 0 && m_RouteModifierData.HasBuffer(policy.m_Policy))
				{
					DynamicBuffer<RouteModifierData> val = m_RouteModifierData[policy.m_Policy];
					for (int j = 0; j < val.Length; j++)
					{
						RouteModifierData modifierData = val[j];
						float modifierDelta = GetModifierDelta(modifierData, policy.m_Adjustment, policy.m_Policy, m_PolicySliderData);
						AddModifier(modifiers, modifierData, modifierDelta);
					}
				}
			}
		}

		public static float GetModifierDelta(RouteModifierData modifierData, float policyAdjustment, Entity policy, ComponentLookup<PolicySliderData> policySliderData)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (policySliderData.HasComponent(policy))
			{
				PolicySliderData policySliderData2 = policySliderData[policy];
				float num = (policyAdjustment - policySliderData2.m_Range.min) / (policySliderData2.m_Range.max - policySliderData2.m_Range.min);
				num = math.select(num, 0f, policySliderData2.m_Range.min == policySliderData2.m_Range.max);
				num = math.saturate(num);
				return math.lerp(modifierData.m_Range.min, modifierData.m_Range.max, num);
			}
			return modifierData.m_Range.min;
		}

		public static float GetPolicyAdjustmentFromModifierDelta(RouteModifierData modifierData, float modifierDelta, PolicySliderData sliderData)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			return math.clamp(math.remap(modifierData.m_Range.min, modifierData.m_Range.max, sliderData.m_Range.min, sliderData.m_Range.max, modifierDelta), sliderData.m_Range.min, sliderData.m_Range.max);
		}

		public static void AddModifierData(ref RouteModifier modifier, RouteModifierData modifierData, float delta)
		{
			switch (modifierData.m_Mode)
			{
			case ModifierValueMode.Relative:
				modifier.m_Delta.y = modifier.m_Delta.y * (1f + delta) + delta;
				break;
			case ModifierValueMode.Absolute:
				modifier.m_Delta.x += delta;
				break;
			case ModifierValueMode.InverseRelative:
				delta = 1f / math.max(0.001f, 1f + delta) - 1f;
				modifier.m_Delta.y = modifier.m_Delta.y * (1f + delta) + delta;
				break;
			}
		}

		public static float GetDeltaFromModifier(RouteModifier modifier, RouteModifierData modifierData)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			return modifierData.m_Mode switch
			{
				ModifierValueMode.Relative => modifier.m_Delta.y, 
				ModifierValueMode.Absolute => modifier.m_Delta.x, 
				ModifierValueMode.InverseRelative => (0f - modifier.m_Delta.y) / (1f + modifier.m_Delta.y), 
				_ => throw new ArgumentException(), 
			};
		}

		private static void AddModifier(DynamicBuffer<RouteModifier> modifiers, RouteModifierData modifierData, float delta)
		{
			while (modifiers.Length <= (int)modifierData.m_Type)
			{
				modifiers.Add(default(RouteModifier));
			}
			RouteModifier modifier = modifiers[(int)modifierData.m_Type];
			AddModifierData(ref modifier, modifierData, delta);
			modifiers[(int)modifierData.m_Type] = modifier;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Policy> __Game_Policies_Policy_RO_BufferTypeHandle;

		public ComponentTypeHandle<Route> __Game_Routes_Route_RW_ComponentTypeHandle;

		public BufferTypeHandle<RouteModifier> __Game_Routes_RouteModifier_RW_BufferTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Policies_Policy_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<Policy>(true);
			__Game_Routes_Route_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Route>(false);
			__Game_Routes_RouteModifier_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<RouteModifier>(false);
		}
	}

	private EntityQuery m_CreatedQuery;

	private RouteModifierRefreshData m_RouteModifierRefreshData;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_RouteModifierRefreshData = new RouteModifierRefreshData((SystemBase)(object)this);
		m_CreatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadWrite<Route>(),
			ComponentType.ReadOnly<Policy>(),
			ComponentType.Exclude<Temp>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_CreatedQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		m_RouteModifierRefreshData.Update((SystemBase)(object)this);
		InitializeRouteModifiersJob initializeRouteModifiersJob = new InitializeRouteModifiersJob
		{
			m_RouteModifierRefreshData = m_RouteModifierRefreshData,
			m_PolicyType = InternalCompilerInterface.GetBufferTypeHandle<Policy>(ref __TypeHandle.__Game_Policies_Policy_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteType = InternalCompilerInterface.GetComponentTypeHandle<Route>(ref __TypeHandle.__Game_Routes_Route_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_RouteModifierType = InternalCompilerInterface.GetBufferTypeHandle<RouteModifier>(ref __TypeHandle.__Game_Routes_RouteModifier_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<InitializeRouteModifiersJob>(initializeRouteModifiersJob, m_CreatedQuery, ((SystemBase)this).Dependency);
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
	public RouteModifierInitializeSystem()
	{
	}
}
