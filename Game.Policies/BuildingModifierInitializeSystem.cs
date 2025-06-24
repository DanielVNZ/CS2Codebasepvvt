using System.Runtime.CompilerServices;
using Game.Buildings;
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
public class BuildingModifierInitializeSystem : GameSystemBase
{
	[BurstCompile]
	private struct InitializeBuildingModifiersJob : IJobChunk
	{
		[ReadOnly]
		public BuildingModifierRefreshData m_BuildingModifierRefreshData;

		[ReadOnly]
		public BufferTypeHandle<Policy> m_PolicyType;

		public ComponentTypeHandle<Building> m_BuildingType;

		public BufferTypeHandle<BuildingModifier> m_BuildingModifierType;

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
			NativeArray<Building> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Building>(ref m_BuildingType);
			BufferAccessor<BuildingModifier> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<BuildingModifier>(ref m_BuildingModifierType);
			BufferAccessor<Policy> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Policy>(ref m_PolicyType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				DynamicBuffer<Policy> policies = bufferAccessor2[i];
				if (policies.Length != 0)
				{
					Building building = nativeArray[i];
					m_BuildingModifierRefreshData.RefreshBuildingOptions(ref building, policies);
					nativeArray[i] = building;
					if (bufferAccessor.Length != 0)
					{
						DynamicBuffer<BuildingModifier> modifiers = bufferAccessor[i];
						m_BuildingModifierRefreshData.RefreshBuildingModifiers(modifiers, policies);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	public struct BuildingModifierRefreshData
	{
		public ComponentLookup<PolicySliderData> m_PolicySliderData;

		public ComponentLookup<BuildingOptionData> m_BuildingOptionData;

		public BufferLookup<BuildingModifierData> m_BuildingModifierData;

		public BuildingModifierRefreshData(SystemBase system)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			m_PolicySliderData = system.GetComponentLookup<PolicySliderData>(true);
			m_BuildingOptionData = system.GetComponentLookup<BuildingOptionData>(true);
			m_BuildingModifierData = system.GetBufferLookup<BuildingModifierData>(true);
		}

		public void Update(SystemBase system)
		{
			m_PolicySliderData.Update(system);
			m_BuildingOptionData.Update(system);
			m_BuildingModifierData.Update(system);
		}

		public void RefreshBuildingOptions(ref Building building, DynamicBuffer<Policy> policies)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			building.m_OptionMask = 0u;
			for (int i = 0; i < policies.Length; i++)
			{
				Policy policy = policies[i];
				if ((policy.m_Flags & PolicyFlags.Active) != 0 && m_BuildingOptionData.HasComponent(policy.m_Policy))
				{
					BuildingOptionData buildingOptionData = m_BuildingOptionData[policy.m_Policy];
					building.m_OptionMask |= buildingOptionData.m_OptionMask;
				}
			}
		}

		public void RefreshBuildingModifiers(DynamicBuffer<BuildingModifier> modifiers, DynamicBuffer<Policy> policies)
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
				if ((policy.m_Flags & PolicyFlags.Active) == 0 || !m_BuildingModifierData.HasBuffer(policy.m_Policy))
				{
					continue;
				}
				DynamicBuffer<BuildingModifierData> val = m_BuildingModifierData[policy.m_Policy];
				for (int j = 0; j < val.Length; j++)
				{
					BuildingModifierData modifierData = val[j];
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

		private static void AddModifier(DynamicBuffer<BuildingModifier> modifiers, BuildingModifierData modifierData, float delta)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			while (modifiers.Length <= (int)modifierData.m_Type)
			{
				modifiers.Add(default(BuildingModifier));
			}
			BuildingModifier buildingModifier = modifiers[(int)modifierData.m_Type];
			switch (modifierData.m_Mode)
			{
			case ModifierValueMode.Relative:
				buildingModifier.m_Delta.y = buildingModifier.m_Delta.y * (1f + delta) + delta;
				break;
			case ModifierValueMode.Absolute:
				buildingModifier.m_Delta.x += delta;
				break;
			case ModifierValueMode.InverseRelative:
				delta = 1f / math.max(0.001f, 1f + delta) - 1f;
				buildingModifier.m_Delta.y = buildingModifier.m_Delta.y * (1f + delta) + delta;
				break;
			}
			modifiers[(int)modifierData.m_Type] = buildingModifier;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<Policy> __Game_Policies_Policy_RO_BufferTypeHandle;

		public ComponentTypeHandle<Building> __Game_Buildings_Building_RW_ComponentTypeHandle;

		public BufferTypeHandle<BuildingModifier> __Game_Buildings_BuildingModifier_RW_BufferTypeHandle;

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
			__Game_Buildings_Building_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Building>(false);
			__Game_Buildings_BuildingModifier_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<BuildingModifier>(false);
		}
	}

	private EntityQuery m_CreatedQuery;

	private BuildingModifierRefreshData m_BuildingModifierRefreshData;

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
		m_BuildingModifierRefreshData = new BuildingModifierRefreshData((SystemBase)(object)this);
		m_CreatedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadWrite<Building>(),
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
		m_BuildingModifierRefreshData.Update((SystemBase)(object)this);
		InitializeBuildingModifiersJob initializeBuildingModifiersJob = new InitializeBuildingModifiersJob
		{
			m_BuildingModifierRefreshData = m_BuildingModifierRefreshData,
			m_PolicyType = InternalCompilerInterface.GetBufferTypeHandle<Policy>(ref __TypeHandle.__Game_Policies_Policy_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingType = InternalCompilerInterface.GetComponentTypeHandle<Building>(ref __TypeHandle.__Game_Buildings_Building_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingModifierType = InternalCompilerInterface.GetBufferTypeHandle<BuildingModifier>(ref __TypeHandle.__Game_Buildings_BuildingModifier_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef)
		};
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<InitializeBuildingModifiersJob>(initializeBuildingModifiersJob, m_CreatedQuery, ((SystemBase)this).Dependency);
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
	public BuildingModifierInitializeSystem()
	{
	}
}
