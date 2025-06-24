using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.PSI;
using Game.Routes;
using Game.Simulation;
using Game.Tools;
using Game.Triggers;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Policies;

[CompilerGenerated]
public class ModifiedSystem : GameSystemBase
{
	public enum PolicyRange
	{
		None,
		Building,
		District,
		City,
		Route
	}

	public struct PolicyEventInfo
	{
		public bool m_Activated;

		public Entity m_Entity;

		public PolicyRange m_PolicyRange;
	}

	[BurstCompile]
	private struct ModifyPolicyJob : IJobChunk
	{
		[ReadOnly]
		public DistrictModifierInitializeSystem.DistrictModifierRefreshData m_DistrictModifierRefreshData;

		[ReadOnly]
		public BuildingModifierInitializeSystem.BuildingModifierRefreshData m_BuildingModifierRefreshData;

		[ReadOnly]
		public RouteModifierInitializeSystem.RouteModifierRefreshData m_RouteModifierRefreshData;

		[ReadOnly]
		public CityModifierUpdateSystem.CityModifierRefreshData m_CityModifierRefreshData;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_EffectProviderChunks;

		[ReadOnly]
		public ComponentTypeHandle<Modify> m_ModifyType;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public Entity m_TicketPricePolicy;

		public ComponentLookup<District> m_DistrictData;

		public ComponentLookup<Building> m_BuildingData;

		public ComponentLookup<Extension> m_ExtensionData;

		public ComponentLookup<Route> m_RouteData;

		public ComponentLookup<Game.City.City> m_CityData;

		public BufferLookup<DistrictModifier> m_DistrictModifiers;

		public BufferLookup<BuildingModifier> m_BuildingModifiers;

		public BufferLookup<RouteModifier> m_RouteModifiers;

		public BufferLookup<CityModifier> m_CityModifiers;

		public BufferLookup<Policy> m_Policies;

		public EntityCommandBuffer m_CommandBuffer;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		public ParallelWriter<PolicyEventInfo> m_PolicyEventInfos;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_031c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_032f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0345: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0275: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Modify> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Modify>(ref m_ModifyType);
			DynamicBuffer<Policy> policies = default(DynamicBuffer<Policy>);
			Extension extension = default(Extension);
			BuildingOptionData optionData = default(BuildingOptionData);
			Owner owner = default(Owner);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Modify modify = nativeArray[i];
				if (m_Policies.TryGetBuffer(modify.m_Entity, ref policies))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(modify.m_Entity);
					int num = 0;
					while (true)
					{
						if (num < policies.Length)
						{
							Policy policy = policies[num];
							if (policy.m_Policy == modify.m_Policy)
							{
								if ((modify.m_Flags & PolicyFlags.Active) == 0)
								{
									CheckFreeBusTicketEventTrigger(policy);
									if (!m_DistrictModifierRefreshData.m_PolicySliderData.HasComponent(policy.m_Policy))
									{
										policies.RemoveAt(num);
										RefreshEffects(modify.m_Entity, modify.m_Policy, policies);
										m_PolicyEventInfos.Enqueue(new PolicyEventInfo
										{
											m_Activated = false,
											m_Entity = modify.m_Policy,
											m_PolicyRange = GetPolicyRange(modify.m_Entity, modify.m_Policy)
										});
										break;
									}
									if (m_DistrictModifierRefreshData.m_PolicySliderData[policy.m_Policy].m_Default == policy.m_Adjustment)
									{
										policies.RemoveAt(num);
										RefreshEffects(modify.m_Entity, modify.m_Policy, policies);
										m_PolicyEventInfos.Enqueue(new PolicyEventInfo
										{
											m_Activated = false,
											m_Entity = modify.m_Policy,
											m_PolicyRange = GetPolicyRange(modify.m_Entity, modify.m_Policy)
										});
										break;
									}
								}
								policy.m_Flags = modify.m_Flags;
								policy.m_Adjustment = modify.m_Adjustment;
								policies[num] = policy;
								RefreshEffects(modify.m_Entity, modify.m_Policy, policies);
								break;
							}
							num++;
							continue;
						}
						if ((modify.m_Flags & PolicyFlags.Active) != 0)
						{
							policies.Add(new Policy(modify.m_Policy, modify.m_Flags, modify.m_Adjustment));
							RefreshEffects(modify.m_Entity, modify.m_Policy, policies);
							m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.PolicyActivated, modify.m_Policy, Entity.Null, Entity.Null));
							m_PolicyEventInfos.Enqueue(new PolicyEventInfo
							{
								m_Activated = true,
								m_Entity = modify.m_Policy,
								m_PolicyRange = GetPolicyRange(modify.m_Entity, modify.m_Policy)
							});
						}
						break;
					}
				}
				else if (m_ExtensionData.TryGetComponent(modify.m_Entity, ref extension) && m_BuildingModifierRefreshData.m_BuildingOptionData.TryGetComponent(modify.m_Policy, ref optionData) && BuildingUtils.HasOption(optionData, BuildingOption.Inactive))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(modify.m_Entity);
					if ((modify.m_Flags & PolicyFlags.Active) != 0)
					{
						extension.m_Flags |= ExtensionFlags.Disabled;
					}
					else
					{
						extension.m_Flags &= ~ExtensionFlags.Disabled;
					}
					m_ExtensionData[modify.m_Entity] = extension;
				}
				if (m_ServiceUpgradeData.HasComponent(modify.m_Entity) && m_OwnerData.TryGetComponent(modify.m_Entity, ref owner))
				{
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(owner.m_Owner);
				}
			}
		}

		private void CheckFreeBusTicketEventTrigger(Policy policy)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (m_TicketPricePolicy == policy.m_Policy)
			{
				m_TriggerBuffer.Enqueue(new TriggerAction
				{
					m_TriggerType = TriggerType.FreePublicTransport,
					m_Value = 0f,
					m_TriggerPrefab = policy.m_Policy,
					m_SecondaryTarget = Entity.Null,
					m_PrimaryTarget = Entity.Null
				});
			}
		}

		private PolicyRange GetPolicyRange(Entity entity, Entity policy)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			if (m_DistrictModifierRefreshData.m_DistrictOptionData.HasComponent(policy) && m_DistrictData.HasComponent(entity))
			{
				return PolicyRange.District;
			}
			if (m_DistrictModifierRefreshData.m_DistrictModifierData.HasBuffer(policy) && m_DistrictModifiers.HasBuffer(entity))
			{
				return PolicyRange.District;
			}
			if (m_BuildingModifierRefreshData.m_BuildingOptionData.HasComponent(policy) && m_BuildingData.HasComponent(entity))
			{
				return PolicyRange.Building;
			}
			if (m_BuildingModifierRefreshData.m_BuildingModifierData.HasBuffer(policy) && m_BuildingModifiers.HasBuffer(entity))
			{
				return PolicyRange.Building;
			}
			if (m_RouteModifierRefreshData.m_RouteOptionData.HasComponent(policy) && m_RouteData.HasComponent(entity))
			{
				return PolicyRange.Route;
			}
			if (m_RouteModifierRefreshData.m_RouteModifierData.HasBuffer(policy) && m_RouteModifiers.HasBuffer(entity))
			{
				return PolicyRange.Route;
			}
			if (m_CityModifierRefreshData.m_CityOptionData.HasComponent(policy) && m_CityData.HasComponent(entity))
			{
				return PolicyRange.City;
			}
			if (m_CityModifierRefreshData.m_CityModifierData.HasBuffer(policy) && m_CityModifiers.HasBuffer(entity))
			{
				return PolicyRange.City;
			}
			return PolicyRange.None;
		}

		private void RefreshEffects(Entity entity, Entity policy, DynamicBuffer<Policy> policies)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_019b: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_017a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018a: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0212: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0223: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			if (m_DistrictModifierRefreshData.m_DistrictOptionData.HasComponent(policy) && m_DistrictData.HasComponent(entity))
			{
				District district = m_DistrictData[entity];
				m_DistrictModifierRefreshData.RefreshDistrictOptions(ref district, policies);
				m_DistrictData[entity] = district;
			}
			if (m_DistrictModifierRefreshData.m_DistrictModifierData.HasBuffer(policy) && m_DistrictModifiers.HasBuffer(entity))
			{
				DynamicBuffer<DistrictModifier> modifiers = m_DistrictModifiers[entity];
				m_DistrictModifierRefreshData.RefreshDistrictModifiers(modifiers, policies);
			}
			if (m_BuildingModifierRefreshData.m_BuildingOptionData.HasComponent(policy) && m_BuildingData.HasComponent(entity))
			{
				Building building = m_BuildingData[entity];
				m_BuildingModifierRefreshData.RefreshBuildingOptions(ref building, policies);
				m_BuildingData[entity] = building;
			}
			if (m_BuildingModifierRefreshData.m_BuildingModifierData.HasBuffer(policy) && m_BuildingModifiers.HasBuffer(entity))
			{
				DynamicBuffer<BuildingModifier> modifiers2 = m_BuildingModifiers[entity];
				m_BuildingModifierRefreshData.RefreshBuildingModifiers(modifiers2, policies);
			}
			if (m_RouteModifierRefreshData.m_RouteOptionData.HasComponent(policy) && m_RouteData.HasComponent(entity))
			{
				Route route = m_RouteData[entity];
				m_RouteModifierRefreshData.RefreshRouteOptions(ref route, policies);
				m_RouteData[entity] = route;
			}
			if (m_RouteModifierRefreshData.m_RouteModifierData.HasBuffer(policy) && m_RouteModifiers.HasBuffer(entity))
			{
				DynamicBuffer<RouteModifier> modifiers3 = m_RouteModifiers[entity];
				m_RouteModifierRefreshData.RefreshRouteModifiers(modifiers3, policies);
			}
			if (m_CityModifierRefreshData.m_CityOptionData.HasComponent(policy) && m_CityData.HasComponent(entity))
			{
				Game.City.City city = m_CityData[entity];
				m_CityModifierRefreshData.RefreshCityOptions(ref city, policies);
				m_CityData[entity] = city;
			}
			if (m_CityModifierRefreshData.m_CityModifierData.HasBuffer(policy) && m_CityModifiers.HasBuffer(entity))
			{
				NativeList<CityModifierData> tempModifierList = default(NativeList<CityModifierData>);
				tempModifierList._002Ector(10, AllocatorHandle.op_Implicit((Allocator)2));
				DynamicBuffer<CityModifier> modifiers4 = m_CityModifiers[entity];
				m_CityModifierRefreshData.RefreshCityModifiers(modifiers4, policies, m_EffectProviderChunks, tempModifierList);
				tempModifierList.Dispose();
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(entity);
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Modify> __Game_Policies_Modify_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		public ComponentLookup<District> __Game_Areas_District_RW_ComponentLookup;

		public ComponentLookup<Building> __Game_Buildings_Building_RW_ComponentLookup;

		public ComponentLookup<Extension> __Game_Buildings_Extension_RW_ComponentLookup;

		public ComponentLookup<Route> __Game_Routes_Route_RW_ComponentLookup;

		public ComponentLookup<Game.City.City> __Game_City_City_RW_ComponentLookup;

		public BufferLookup<DistrictModifier> __Game_Areas_DistrictModifier_RW_BufferLookup;

		public BufferLookup<BuildingModifier> __Game_Buildings_BuildingModifier_RW_BufferLookup;

		public BufferLookup<RouteModifier> __Game_Routes_RouteModifier_RW_BufferLookup;

		public BufferLookup<CityModifier> __Game_City_CityModifier_RW_BufferLookup;

		public BufferLookup<Policy> __Game_Policies_Policy_RW_BufferLookup;

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
			__Game_Policies_Modify_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Modify>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Areas_District_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<District>(false);
			__Game_Buildings_Building_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(false);
			__Game_Buildings_Extension_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extension>(false);
			__Game_Routes_Route_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Route>(false);
			__Game_City_City_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.City.City>(false);
			__Game_Areas_DistrictModifier_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<DistrictModifier>(false);
			__Game_Buildings_BuildingModifier_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<BuildingModifier>(false);
			__Game_Routes_RouteModifier_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<RouteModifier>(false);
			__Game_City_CityModifier_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(false);
			__Game_Policies_Policy_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Policy>(false);
		}
	}

	private EntityQuery m_EventQuery;

	private EntityQuery m_EffectProviderQuery;

	private ModificationBarrier4 m_ModificationBarrier;

	private TriggerSystem m_TriggerSystem;

	private NativeQueue<PolicyEventInfo> m_PolicyEventInfos;

	private DistrictModifierInitializeSystem.DistrictModifierRefreshData m_DistrictModifierRefreshData;

	private BuildingModifierInitializeSystem.BuildingModifierRefreshData m_BuildingModifierRefreshData;

	private RouteModifierInitializeSystem.RouteModifierRefreshData m_RouteModifierRefreshData;

	private CityModifierUpdateSystem.CityModifierRefreshData m_CityModifierRefreshData;

	private Entity m_TicketPricePolicy;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PolicyEventInfos = new NativeQueue<PolicyEventInfo>(AllocatorHandle.op_Implicit((Allocator)4));
		m_DistrictModifierRefreshData = new DistrictModifierInitializeSystem.DistrictModifierRefreshData((SystemBase)(object)this);
		m_BuildingModifierRefreshData = new BuildingModifierInitializeSystem.BuildingModifierRefreshData((SystemBase)(object)this);
		m_RouteModifierRefreshData = new RouteModifierInitializeSystem.RouteModifierRefreshData((SystemBase)(object)this);
		m_CityModifierRefreshData = new CityModifierUpdateSystem.CityModifierRefreshData((SystemBase)(object)this);
		PrefabSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UITransportConfigurationData>() });
		UITransportConfigurationPrefab singletonPrefab = orCreateSystemManaged.GetSingletonPrefab<UITransportConfigurationPrefab>(entityQuery);
		m_TicketPricePolicy = orCreateSystemManaged.GetEntity(singletonPrefab.m_TicketPricePolicy);
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Event>(),
			ComponentType.ReadOnly<Modify>()
		});
		m_EffectProviderQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<CityEffectProvider>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Temp>()
		});
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		((ComponentSystemBase)this).RequireForUpdate(m_EventQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> effectProviderChunks = ((EntityQuery)(ref m_EffectProviderQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		m_DistrictModifierRefreshData.Update((SystemBase)(object)this);
		m_BuildingModifierRefreshData.Update((SystemBase)(object)this);
		m_RouteModifierRefreshData.Update((SystemBase)(object)this);
		m_CityModifierRefreshData.Update((SystemBase)(object)this);
		NativeQueue<TriggerAction> val2 = (((ComponentSystemBase)m_TriggerSystem).Enabled ? m_TriggerSystem.CreateActionBuffer() : new NativeQueue<TriggerAction>(AllocatorHandle.op_Implicit((Allocator)3)));
		JobHandle val3 = JobChunkExtensions.Schedule<ModifyPolicyJob>(new ModifyPolicyJob
		{
			m_DistrictModifierRefreshData = m_DistrictModifierRefreshData,
			m_BuildingModifierRefreshData = m_BuildingModifierRefreshData,
			m_RouteModifierRefreshData = m_RouteModifierRefreshData,
			m_CityModifierRefreshData = m_CityModifierRefreshData,
			m_EffectProviderChunks = effectProviderChunks,
			m_ModifyType = InternalCompilerInterface.GetComponentTypeHandle<Modify>(ref __TypeHandle.__Game_Policies_Modify_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerBuffer = val2.AsParallelWriter(),
			m_DistrictData = InternalCompilerInterface.GetComponentLookup<District>(ref __TypeHandle.__Game_Areas_District_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtensionData = InternalCompilerInterface.GetComponentLookup<Extension>(ref __TypeHandle.__Game_Buildings_Extension_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteData = InternalCompilerInterface.GetComponentLookup<Route>(ref __TypeHandle.__Game_Routes_Route_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityData = InternalCompilerInterface.GetComponentLookup<Game.City.City>(ref __TypeHandle.__Game_City_City_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DistrictModifiers = InternalCompilerInterface.GetBufferLookup<DistrictModifier>(ref __TypeHandle.__Game_Areas_DistrictModifier_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingModifiers = InternalCompilerInterface.GetBufferLookup<BuildingModifier>(ref __TypeHandle.__Game_Buildings_BuildingModifier_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RouteModifiers = InternalCompilerInterface.GetBufferLookup<RouteModifier>(ref __TypeHandle.__Game_Routes_RouteModifier_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Policies = InternalCompilerInterface.GetBufferLookup<Policy>(ref __TypeHandle.__Game_Policies_Policy_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PolicyEventInfos = m_PolicyEventInfos.AsParallelWriter(),
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer(),
			m_TicketPricePolicy = m_TicketPricePolicy
		}, m_EventQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		effectProviderChunks.Dispose(val3);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val3);
		if (((ComponentSystemBase)m_TriggerSystem).Enabled)
		{
			m_TriggerSystem.AddActionBufferWriter(val3);
		}
		else
		{
			val2.Dispose(val3);
		}
		((SystemBase)this).Dependency = val3;
		((JobHandle)(ref val3)).Complete();
		while (m_PolicyEventInfos.Count > 0)
		{
			Telemetry.Policy(m_PolicyEventInfos.Dequeue());
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_PolicyEventInfos.Dispose();
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
	public ModifiedSystem()
	{
	}
}
