using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Common;
using Game.Prefabs;
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
public class DistrictModifierInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeDistrictModifiersJob : IJobChunk
	{
		[ReadOnly]
		public DistrictModifierRefreshData m_DistrictModifierRefreshData;

		[ReadOnly]
		public BufferTypeHandle<Policy> m_PolicyType;

		public ComponentTypeHandle<District> m_DistrictType;

		public BufferTypeHandle<DistrictModifier> m_DistrictModifierType;

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
			NativeArray<District> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<District>(ref m_DistrictType);
			BufferAccessor<DistrictModifier> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<DistrictModifier>(ref m_DistrictModifierType);
			BufferAccessor<Policy> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Policy>(ref m_PolicyType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				DynamicBuffer<Policy> policies = bufferAccessor2[i];
				if (policies.Length != 0)
				{
					District district = nativeArray[i];
					m_DistrictModifierRefreshData.RefreshDistrictOptions(ref district, policies);
					nativeArray[i] = district;
					if (bufferAccessor.Length != 0)
					{
						DynamicBuffer<DistrictModifier> modifiers = bufferAccessor[i];
						m_DistrictModifierRefreshData.RefreshDistrictModifiers(modifiers, policies);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public struct DistrictModifierRefreshData
	{
		public ComponentLookup<PolicySliderData> m_PolicySliderData;

		public ComponentLookup<DistrictOptionData> m_DistrictOptionData;

		public BufferLookup<DistrictModifierData> m_DistrictModifierData;

		public DistrictModifierRefreshData(SystemBase system)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			m_PolicySliderData = system.GetComponentLookup<PolicySliderData>(true);
			m_DistrictOptionData = system.GetComponentLookup<DistrictOptionData>(true);
			m_DistrictModifierData = system.GetBufferLookup<DistrictModifierData>(true);
		}

		public void Update(SystemBase system)
		{
			m_PolicySliderData.Update(system);
			m_DistrictOptionData.Update(system);
			m_DistrictModifierData.Update(system);
		}

		public void RefreshDistrictOptions(ref District district, DynamicBuffer<Policy> policies)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			district.m_OptionMask = 0u;
			for (int i = 0; i < policies.Length; i++)
			{
				Policy policy = policies[i];
				if ((policy.m_Flags & PolicyFlags.Active) != 0 && m_DistrictOptionData.HasComponent(policy.m_Policy))
				{
					DistrictOptionData districtOptionData = m_DistrictOptionData[policy.m_Policy];
					district.m_OptionMask |= districtOptionData.m_OptionMask;
				}
			}
		}

		public void RefreshDistrictModifiers(DynamicBuffer<DistrictModifier> modifiers, DynamicBuffer<Policy> policies)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			modifiers.Clear();
			for (int i = 0; i < policies.Length; i++)
			{
				Policy policy = policies[i];
				if ((policy.m_Flags & PolicyFlags.Active) == 0 || !m_DistrictModifierData.HasBuffer(policy.m_Policy))
				{
					continue;
				}
				DynamicBuffer<DistrictModifierData> val = m_DistrictModifierData[policy.m_Policy];
				for (int j = 0; j < val.Length; j++)
				{
					DistrictModifierData modifierData = val[j];
					float delta;
					if (m_PolicySliderData.HasComponent(policy.m_Policy))
					{
						PolicySliderData policySliderData = m_PolicySliderData[policy.m_Policy];
						float num = (policy.m_Adjustment - policySliderData.m_Range.min) / (policySliderData.m_Range.max - policySliderData.m_Range.min);
						num = math.select(num, 0f, policySliderData.m_Range.min == policySliderData.m_Range.max);
						num = math.saturate(num);
						delta = math.lerp(modifierData.m_Range.min, modifierData.m_Range.max, num);
					}
					else
					{
						delta = modifierData.m_Range.min;
					}
					AddModifier(modifiers, modifierData, delta);
				}
			}
		}

		private static void AddModifier(DynamicBuffer<DistrictModifier> modifiers, DistrictModifierData modifierData, float delta)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			while (modifiers.Length <= (int)modifierData.m_Type)
			{
				modifiers.Add(default(DistrictModifier));
			}
			DistrictModifier districtModifier = modifiers[(int)modifierData.m_Type];
			switch (modifierData.m_Mode)
			{
			case ModifierValueMode.Relative:
				districtModifier.m_Delta.y = districtModifier.m_Delta.y * (1f + delta) + delta;
				break;
			case ModifierValueMode.Absolute:
				districtModifier.m_Delta.x += delta;
				break;
			case ModifierValueMode.InverseRelative:
				delta = 1f / math.max(0.001f, 1f + delta) - 1f;
				districtModifier.m_Delta.y = districtModifier.m_Delta.y * (1f + delta) + delta;
				break;
			}
			modifiers[(int)modifierData.m_Type] = districtModifier;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Policy> __Game_Policies_Policy_RO_BufferTypeHandle;

		public ComponentTypeHandle<District> __Game_Areas_District_RW_ComponentTypeHandle;

		public BufferTypeHandle<DistrictModifier> __Game_Areas_DistrictModifier_RW_BufferTypeHandle;

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
			__Game_Areas_District_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<District>(false);
			__Game_Areas_DistrictModifier_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<DistrictModifier>(false);
		}
	}

	private EntityQuery m_CreatedQuery;

	private DistrictModifierRefreshData m_DistrictModifierRefreshData;

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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_DistrictModifierRefreshData = new DistrictModifierRefreshData((SystemBase)(object)this);
		m_CreatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadWrite<District>(),
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
		m_DistrictModifierRefreshData.Update((SystemBase)(object)this);
		InitializeDistrictModifiersJob initializeDistrictModifiersJob = new InitializeDistrictModifiersJob
		{
			m_DistrictModifierRefreshData = m_DistrictModifierRefreshData,
			m_PolicyType = InternalCompilerInterface.GetBufferTypeHandle<Policy>(ref __TypeHandle.__Game_Policies_Policy_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictType = InternalCompilerInterface.GetComponentTypeHandle<District>(ref __TypeHandle.__Game_Areas_District_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictModifierType = InternalCompilerInterface.GetBufferTypeHandle<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<InitializeDistrictModifiersJob>(initializeDistrictModifiersJob, m_CreatedQuery, ((SystemBase)this).Dependency);
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
	public DistrictModifierInitializeSystem()
	{
	}
}
