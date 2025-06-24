using System.Runtime.CompilerServices;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Events;
using Game.Notifications;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Game.Triggers;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class HealthProblemSystem : GameSystemBase
{
	[BurstCompile]
	private struct HealthProblemJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentBuilding> m_CurrentBuildingType;

		[ReadOnly]
		public ComponentTypeHandle<CurrentTransport> m_CurrentTransportType;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> m_CitizenType;

		[ReadOnly]
		public ComponentTypeHandle<TravelPurpose> m_TravelPurposeType;

		[ReadOnly]
		public SharedComponentTypeHandle<UpdateFrame> m_UpdateFrameType;

		public ComponentTypeHandle<HealthProblem> m_HealthProblemType;

		public BufferTypeHandle<TripNeeded> m_TripNeededType;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> m_HouseholdMemberType;

		[ReadOnly]
		public ComponentLookup<HealthcareRequest> m_HealthcareRequestData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> m_HospitalData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.DeathcareFacility> m_DeathcareFacilityData;

		[ReadOnly]
		public ComponentLookup<OnFire> m_OnFireData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Deleted> m_DeletedData;

		[ReadOnly]
		public ComponentLookup<Dispatched> m_DispatchedData;

		[ReadOnly]
		public ComponentLookup<Target> m_TargetData;

		[ReadOnly]
		public ComponentLookup<Static> m_StaticData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> m_OutsideConnectionData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Ambulance> m_AmbulanceData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Hearse> m_HearseData;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> m_CurrentVehicleData;

		[ReadOnly]
		public ComponentLookup<Divert> m_DivertData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<BuildingData> m_PrefabBuildingData;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> m_HouseholdCitizens;

		[ReadOnly]
		public uint m_UpdateFrameIndex;

		[ReadOnly]
		public RandomSeed m_RandomSeed;

		[ReadOnly]
		public EntityArchetype m_HealthcareRequestArchetype;

		[ReadOnly]
		public EntityArchetype m_JournalDataArchetype;

		[ReadOnly]
		public EntityArchetype m_ResetTripArchetype;

		[ReadOnly]
		public EntityArchetype m_HandleRequestArchetype;

		[ReadOnly]
		public HealthcareParameterData m_HealthcareParameterData;

		[ReadOnly]
		public FireConfigurationData m_FireConfigurationData;

		public IconCommandBuffer m_IconCommandBuffer;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<StatisticsEvent> m_StatisticsEventQueue;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0427: Unknown result type (might be due to invalid IL or missing references)
			//IL_042c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_043b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0440: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_0444: Unknown result type (might be due to invalid IL or missing references)
			//IL_0340: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c19: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c1e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c24: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c29: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c2d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c34: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c36: Unknown result type (might be due to invalid IL or missing references)
			//IL_0743: Unknown result type (might be due to invalid IL or missing references)
			//IL_0748: Unknown result type (might be due to invalid IL or missing references)
			//IL_074e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0753: Unknown result type (might be due to invalid IL or missing references)
			//IL_0757: Unknown result type (might be due to invalid IL or missing references)
			//IL_075c: Unknown result type (might be due to invalid IL or missing references)
			//IL_075e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d38: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d3d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d43: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d48: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d4c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d51: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d53: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c7c: Unknown result type (might be due to invalid IL or missing references)
			//IL_099a: Unknown result type (might be due to invalid IL or missing references)
			//IL_099f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0555: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d9f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c91: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_09fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0867: Unknown result type (might be due to invalid IL or missing references)
			//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_077f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0612: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_0482: Unknown result type (might be due to invalid IL or missing references)
			//IL_048c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dae: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d74: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cfe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ca0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c73: Unknown result type (might be due to invalid IL or missing references)
			//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
			//IL_09d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0875: Unknown result type (might be due to invalid IL or missing references)
			//IL_087d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0793: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0620: Unknown result type (might be due to invalid IL or missing references)
			//IL_0628: Unknown result type (might be due to invalid IL or missing references)
			//IL_0508: Unknown result type (might be due to invalid IL or missing references)
			//IL_050c: Unknown result type (might be due to invalid IL or missing references)
			//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0e16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dbf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0dc3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d88: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d92: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d97: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d0c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d14: Unknown result type (might be due to invalid IL or missing references)
			//IL_0d16: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0cb4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a2f: Unknown result type (might be due to invalid IL or missing references)
			//IL_09ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0964: Unknown result type (might be due to invalid IL or missing references)
			//IL_096c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0973: Unknown result type (might be due to invalid IL or missing references)
			//IL_0979: Unknown result type (might be due to invalid IL or missing references)
			//IL_088d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0892: Unknown result type (might be due to invalid IL or missing references)
			//IL_0819: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_070f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0717: Unknown result type (might be due to invalid IL or missing references)
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0724: Unknown result type (might be due to invalid IL or missing references)
			//IL_0638: Unknown result type (might be due to invalid IL or missing references)
			//IL_063d: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_05c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_058b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0593: Unknown result type (might be due to invalid IL or missing references)
			//IL_059a: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_027e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ae7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
			//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0651: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bd6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bde: Unknown result type (might be due to invalid IL or missing references)
			//IL_0be5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0beb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b04: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a8f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a5a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
			//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
			//IL_0539: Unknown result type (might be due to invalid IL or missing references)
			//IL_0541: Unknown result type (might be due to invalid IL or missing references)
			//IL_0543: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0300: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_0de7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0def: Unknown result type (might be due to invalid IL or missing references)
			//IL_0df1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ce9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ceb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b18: Unknown result type (might be due to invalid IL or missing references)
			//IL_08db: Unknown result type (might be due to invalid IL or missing references)
			//IL_08e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0929: Unknown result type (might be due to invalid IL or missing references)
			//IL_0931: Unknown result type (might be due to invalid IL or missing references)
			//IL_0938: Unknown result type (might be due to invalid IL or missing references)
			//IL_093e: Unknown result type (might be due to invalid IL or missing references)
			//IL_084a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0852: Unknown result type (might be due to invalid IL or missing references)
			//IL_0854: Unknown result type (might be due to invalid IL or missing references)
			//IL_0686: Unknown result type (might be due to invalid IL or missing references)
			//IL_068e: Unknown result type (might be due to invalid IL or missing references)
			//IL_069c: Unknown result type (might be due to invalid IL or missing references)
			//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b4d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b55: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b63: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b69: Unknown result type (might be due to invalid IL or missing references)
			//IL_0b9b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ba3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0baa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0bb0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0abc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0ac6: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).GetSharedComponent<UpdateFrame>(m_UpdateFrameType).m_Index != m_UpdateFrameIndex)
			{
				return;
			}
			Random random = m_RandomSeed.GetRandom(unfilteredChunkIndex);
			int num = (int)(m_HealthcareParameterData.m_TransportWarningTime * (15f / 64f));
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Citizen> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Citizen>(ref m_CitizenType);
			NativeArray<HealthProblem> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthProblem>(ref m_HealthProblemType);
			NativeArray<CurrentBuilding> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentBuilding>(ref m_CurrentBuildingType);
			NativeArray<TravelPurpose> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<TravelPurpose>(ref m_TravelPurposeType);
			NativeArray<CurrentTransport> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CurrentTransport>(ref m_CurrentTransportType);
			BufferAccessor<TripNeeded> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TripNeeded>(ref m_TripNeededType);
			NativeArray<HouseholdMember> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HouseholdMember>(ref m_HouseholdMemberType);
			for (int i = 0; i < nativeArray3.Length; i++)
			{
				HealthProblem healthProblem = nativeArray3[i];
				CurrentBuilding currentBuilding = default(CurrentBuilding);
				TravelPurpose travelPurpose = default(TravelPurpose);
				CurrentTransport currentTransport = default(CurrentTransport);
				if (nativeArray4.Length != 0)
				{
					currentBuilding = nativeArray4[i];
				}
				if (nativeArray5.Length != 0)
				{
					travelPurpose = nativeArray5[i];
				}
				if (nativeArray6.Length != 0)
				{
					currentTransport = nativeArray6[i];
				}
				if ((healthProblem.m_Flags & ~HealthProblemFlags.NoHealthcare) == 0)
				{
					Entity val = nativeArray[i];
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<HealthProblem>(unfilteredChunkIndex, val);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(unfilteredChunkIndex, val);
					continue;
				}
				if ((healthProblem.m_Flags & (HealthProblemFlags.InDanger | HealthProblemFlags.Trapped)) != HealthProblemFlags.None)
				{
					if ((healthProblem.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
					{
						healthProblem.m_Flags &= ~(HealthProblemFlags.InDanger | HealthProblemFlags.Trapped);
						nativeArray3[i] = healthProblem;
					}
					else
					{
						Entity val2 = nativeArray[i];
						Citizen citizen = nativeArray2[i];
						DynamicBuffer<TripNeeded> tripNeededs = bufferAccessor[i];
						if (m_OnFireData.HasComponent(currentBuilding.m_CurrentBuilding))
						{
							if ((healthProblem.m_Flags & HealthProblemFlags.InDanger) != HealthProblemFlags.None)
							{
								OnFire onFire = m_OnFireData[currentBuilding.m_CurrentBuilding];
								float num2 = (float)(int)citizen.m_Health - onFire.m_Intensity * 0.5f;
								if (((Random)(ref random)).NextFloat(100f) < num2)
								{
									if ((healthProblem.m_Flags & HealthProblemFlags.Trapped) == 0)
									{
										healthProblem.m_Flags &= ~HealthProblemFlags.InDanger;
										nativeArray3[i] = healthProblem;
										GoToSafety(unfilteredChunkIndex, val2, currentBuilding, travelPurpose, currentTransport, tripNeededs);
									}
								}
								else if ((healthProblem.m_Flags & HealthProblemFlags.Trapped) == 0 && ((Random)(ref random)).NextFloat() < m_FireConfigurationData.m_DeathRateOfFireAccident)
								{
									if ((healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None)
									{
										m_IconCommandBuffer.Remove(val2, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
										healthProblem.m_Timer = 0;
									}
									healthProblem.m_Flags &= ~(HealthProblemFlags.InDanger | HealthProblemFlags.Trapped);
									healthProblem.m_Flags |= HealthProblemFlags.Dead | HealthProblemFlags.RequireTransport;
									nativeArray3[i] = healthProblem;
									AddJournalData(unfilteredChunkIndex, healthProblem);
									Entity household = ((nativeArray7.Length != 0) ? nativeArray7[i].m_Household : Entity.Null);
									DeathCheckSystem.PerformAfterDeathActions(nativeArray[i], household, m_TriggerBuffer, m_StatisticsEventQueue, ref m_HouseholdCitizens);
								}
								else
								{
									healthProblem.m_Flags |= HealthProblemFlags.Trapped;
									nativeArray3[i] = healthProblem;
								}
							}
						}
						else if (m_DestroyedData.HasComponent(currentBuilding.m_CurrentBuilding))
						{
							if ((healthProblem.m_Flags & HealthProblemFlags.InDanger) != HealthProblemFlags.None)
							{
								healthProblem.m_Flags &= ~HealthProblemFlags.InDanger;
								nativeArray3[i] = healthProblem;
							}
							if ((healthProblem.m_Flags & HealthProblemFlags.Trapped) != HealthProblemFlags.None)
							{
								Destroyed destroyed = m_DestroyedData[currentBuilding.m_CurrentBuilding];
								if (((Random)(ref random)).NextFloat(1f) < destroyed.m_Cleared)
								{
									healthProblem.m_Flags &= ~HealthProblemFlags.Trapped;
									nativeArray3[i] = healthProblem;
									GoToSafety(unfilteredChunkIndex, val2, currentBuilding, travelPurpose, currentTransport, tripNeededs);
								}
							}
							else
							{
								GoToSafety(unfilteredChunkIndex, val2, currentBuilding, travelPurpose, currentTransport, tripNeededs);
							}
						}
						else
						{
							healthProblem.m_Flags &= ~(HealthProblemFlags.InDanger | HealthProblemFlags.Trapped);
							nativeArray3[i] = healthProblem;
						}
					}
				}
				if ((healthProblem.m_Flags & HealthProblemFlags.RequireTransport) != HealthProblemFlags.None)
				{
					if ((healthProblem.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
					{
						Entity val3 = nativeArray[i];
						DynamicBuffer<TripNeeded> tripNeededs2 = bufferAccessor[i];
						Entity val4 = currentBuilding.m_CurrentBuilding;
						if (val4 == Entity.Null && (travelPurpose.m_Purpose == Purpose.Deathcare || travelPurpose.m_Purpose == Purpose.Hospital) && m_TargetData.HasComponent(currentTransport.m_CurrentTransport))
						{
							val4 = m_TargetData[currentTransport.m_CurrentTransport].m_Target;
						}
						if (m_DeathcareFacilityData.HasComponent(val4))
						{
							if ((m_DeathcareFacilityData[val4].m_Flags & (DeathcareFacilityFlags.CanProcessCorpses | DeathcareFacilityFlags.CanStoreCorpses)) != 0)
							{
								if (healthProblem.m_Timer > 0)
								{
									m_IconCommandBuffer.Remove(val3, m_HealthcareParameterData.m_HearseNotificationPrefab);
									healthProblem.m_Timer = 0;
									nativeArray3[i] = healthProblem;
								}
								HandleRequest(unfilteredChunkIndex, healthProblem);
								if (val4 == currentBuilding.m_CurrentBuilding && travelPurpose.m_Purpose != Purpose.Deathcare && travelPurpose.m_Purpose != Purpose.InDeathcare)
								{
									GoToDeathcare(unfilteredChunkIndex, val3, currentBuilding, travelPurpose, currentTransport, tripNeededs2, val4);
								}
								continue;
							}
						}
						else if (m_HospitalData.HasComponent(val4) && (m_HospitalData[val4].m_Flags & HospitalFlags.CanProcessCorpses) != 0)
						{
							if (healthProblem.m_Timer > 0)
							{
								m_IconCommandBuffer.Remove(val3, m_HealthcareParameterData.m_HearseNotificationPrefab);
								healthProblem.m_Timer = 0;
								nativeArray3[i] = healthProblem;
							}
							HandleRequest(unfilteredChunkIndex, healthProblem);
							if (val4 == currentBuilding.m_CurrentBuilding && travelPurpose.m_Purpose != Purpose.Hospital && travelPurpose.m_Purpose != Purpose.InHospital)
							{
								GoToHospital(unfilteredChunkIndex, val3, currentBuilding, travelPurpose, currentTransport, tripNeededs2, val4, immediate: true);
							}
							continue;
						}
						if (m_OutsideConnectionData.HasComponent(val4))
						{
							continue;
						}
						if (RequestVehicleIfNeeded(unfilteredChunkIndex, val3, currentBuilding, travelPurpose, currentTransport, tripNeededs2, healthProblem))
						{
							if (currentTransport.m_CurrentTransport != Entity.Null || m_StaticData.HasComponent(currentBuilding.m_CurrentBuilding))
							{
								if (healthProblem.m_Timer < num)
								{
									if (++healthProblem.m_Timer == num)
									{
										m_IconCommandBuffer.Add(val3, m_HealthcareParameterData.m_HearseNotificationPrefab, IconPriority.MajorProblem);
									}
									nativeArray3[i] = healthProblem;
								}
							}
							else if (healthProblem.m_Timer > 0)
							{
								m_IconCommandBuffer.Remove(val3, m_HealthcareParameterData.m_HearseNotificationPrefab);
								healthProblem.m_Timer = 0;
								nativeArray3[i] = healthProblem;
							}
						}
						else
						{
							m_IconCommandBuffer.Remove(val3, m_HealthcareParameterData.m_HearseNotificationPrefab);
						}
					}
					else if ((healthProblem.m_Flags & HealthProblemFlags.Injured) != HealthProblemFlags.None)
					{
						Entity val5 = nativeArray[i];
						DynamicBuffer<TripNeeded> tripNeededs3 = bufferAccessor[i];
						Entity val6 = currentBuilding.m_CurrentBuilding;
						if (val6 == Entity.Null && travelPurpose.m_Purpose == Purpose.Hospital && m_TargetData.HasComponent(currentTransport.m_CurrentTransport))
						{
							val6 = m_TargetData[currentTransport.m_CurrentTransport].m_Target;
						}
						if (m_HospitalData.HasComponent(val6) && (m_HospitalData[val6].m_Flags & HospitalFlags.CanCureInjury) != 0)
						{
							if (healthProblem.m_Timer > 0)
							{
								m_IconCommandBuffer.Remove(val5, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
								healthProblem.m_Timer = 0;
								nativeArray3[i] = healthProblem;
							}
							HandleRequest(unfilteredChunkIndex, healthProblem);
							if (val6 == currentBuilding.m_CurrentBuilding && travelPurpose.m_Purpose != Purpose.Hospital && travelPurpose.m_Purpose != Purpose.InHospital)
							{
								GoToHospital(unfilteredChunkIndex, val5, currentBuilding, travelPurpose, currentTransport, tripNeededs3, val6, immediate: true);
							}
						}
						else
						{
							if (m_OutsideConnectionData.HasComponent(val6))
							{
								continue;
							}
							if (RequestVehicleIfNeeded(unfilteredChunkIndex, val5, currentBuilding, travelPurpose, currentTransport, tripNeededs3, healthProblem))
							{
								if (currentTransport.m_CurrentTransport != Entity.Null || m_StaticData.HasComponent(currentBuilding.m_CurrentBuilding))
								{
									if (healthProblem.m_Timer < num)
									{
										if (++healthProblem.m_Timer == num)
										{
											m_IconCommandBuffer.Add(val5, m_HealthcareParameterData.m_AmbulanceNotificationPrefab, IconPriority.MajorProblem);
										}
										nativeArray3[i] = healthProblem;
									}
								}
								else if (healthProblem.m_Timer > 0)
								{
									m_IconCommandBuffer.Remove(val5, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
									healthProblem.m_Timer = 0;
									nativeArray3[i] = healthProblem;
								}
							}
							else
							{
								m_IconCommandBuffer.Remove(val5, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
							}
						}
					}
					else
					{
						if ((healthProblem.m_Flags & (HealthProblemFlags.Sick | HealthProblemFlags.NoHealthcare)) != HealthProblemFlags.Sick)
						{
							continue;
						}
						Entity val7 = nativeArray[i];
						DynamicBuffer<TripNeeded> tripNeededs4 = bufferAccessor[i];
						Entity val8 = currentBuilding.m_CurrentBuilding;
						if (val8 == Entity.Null && travelPurpose.m_Purpose == Purpose.Hospital && m_TargetData.HasComponent(currentTransport.m_CurrentTransport))
						{
							val8 = m_TargetData[currentTransport.m_CurrentTransport].m_Target;
						}
						if (val8 == Entity.Null)
						{
							HandleRequest(unfilteredChunkIndex, healthProblem);
						}
						else if (m_HospitalData.HasComponent(val8) && (m_HospitalData[val8].m_Flags & HospitalFlags.CanCureDisease) != 0)
						{
							if (healthProblem.m_Timer > 0)
							{
								m_IconCommandBuffer.Remove(val7, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
								healthProblem.m_Timer = 0;
								nativeArray3[i] = healthProblem;
							}
							HandleRequest(unfilteredChunkIndex, healthProblem);
							if (val8 == currentBuilding.m_CurrentBuilding && travelPurpose.m_Purpose != Purpose.Hospital && travelPurpose.m_Purpose != Purpose.InHospital)
							{
								GoToHospital(unfilteredChunkIndex, val7, currentBuilding, travelPurpose, currentTransport, tripNeededs4, val8, immediate: true);
							}
						}
						else
						{
							if (m_OutsideConnectionData.HasComponent(val8))
							{
								continue;
							}
							if (RequestVehicleIfNeeded(unfilteredChunkIndex, val7, currentBuilding, travelPurpose, currentTransport, tripNeededs4, healthProblem))
							{
								if (currentTransport.m_CurrentTransport != Entity.Null || m_StaticData.HasComponent(currentBuilding.m_CurrentBuilding))
								{
									if (healthProblem.m_Timer < num)
									{
										if (++healthProblem.m_Timer == num)
										{
											m_IconCommandBuffer.Add(val7, m_HealthcareParameterData.m_AmbulanceNotificationPrefab, IconPriority.MajorProblem);
										}
										nativeArray3[i] = healthProblem;
									}
								}
								else if (healthProblem.m_Timer > 0)
								{
									m_IconCommandBuffer.Remove(val7, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
									healthProblem.m_Timer = 0;
									nativeArray3[i] = healthProblem;
								}
							}
							else
							{
								m_IconCommandBuffer.Remove(val7, m_HealthcareParameterData.m_AmbulanceNotificationPrefab);
							}
						}
					}
				}
				else
				{
					if ((healthProblem.m_Flags & (HealthProblemFlags.Dead | HealthProblemFlags.NoHealthcare)) != HealthProblemFlags.None)
					{
						continue;
					}
					if ((healthProblem.m_Flags & HealthProblemFlags.Sick) != HealthProblemFlags.None)
					{
						Entity entity = nativeArray[i];
						DynamicBuffer<TripNeeded> tripNeededs5 = bufferAccessor[i];
						Entity val9 = currentBuilding.m_CurrentBuilding;
						if (val9 == Entity.Null && travelPurpose.m_Purpose == Purpose.Hospital && m_TargetData.HasComponent(currentTransport.m_CurrentTransport))
						{
							val9 = m_TargetData[currentTransport.m_CurrentTransport].m_Target;
						}
						if (val9 == Entity.Null)
						{
							continue;
						}
						if (m_HospitalData.HasComponent(val9) && (m_HospitalData[val9].m_Flags & HospitalFlags.CanCureDisease) != 0)
						{
							if (val9 == currentBuilding.m_CurrentBuilding && travelPurpose.m_Purpose != Purpose.Hospital && travelPurpose.m_Purpose != Purpose.InHospital)
							{
								GoToHospital(unfilteredChunkIndex, entity, currentBuilding, travelPurpose, currentTransport, tripNeededs5, val9, immediate: true);
							}
						}
						else if (!m_OutsideConnectionData.HasComponent(val9))
						{
							GoToHospital(unfilteredChunkIndex, entity, currentBuilding, travelPurpose, currentTransport, tripNeededs5, Entity.Null, immediate: false);
						}
					}
					else
					{
						if ((healthProblem.m_Flags & HealthProblemFlags.Injured) == 0)
						{
							continue;
						}
						Entity entity2 = nativeArray[i];
						DynamicBuffer<TripNeeded> tripNeededs6 = bufferAccessor[i];
						Entity val10 = currentBuilding.m_CurrentBuilding;
						if (val10 == Entity.Null && travelPurpose.m_Purpose == Purpose.Hospital && m_TargetData.HasComponent(currentTransport.m_CurrentTransport))
						{
							val10 = m_TargetData[currentTransport.m_CurrentTransport].m_Target;
						}
						if (m_HospitalData.HasComponent(val10) && (m_HospitalData[val10].m_Flags & HospitalFlags.CanCureInjury) != 0)
						{
							if (val10 == currentBuilding.m_CurrentBuilding && travelPurpose.m_Purpose != Purpose.Hospital && travelPurpose.m_Purpose != Purpose.InHospital)
							{
								GoToHospital(unfilteredChunkIndex, entity2, currentBuilding, travelPurpose, currentTransport, tripNeededs6, val10, immediate: true);
							}
						}
						else if (!m_OutsideConnectionData.HasComponent(val10))
						{
							GoToHospital(unfilteredChunkIndex, entity2, currentBuilding, travelPurpose, currentTransport, tripNeededs6, Entity.Null, immediate: true);
						}
					}
				}
			}
		}

		private void HandleRequest(int jobIndex, HealthProblem healthProblem)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (m_HealthcareRequestData.HasComponent(healthProblem.m_HealthcareRequest))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HandleRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HandleRequest>(jobIndex, val, new HandleRequest(healthProblem.m_HealthcareRequest, Entity.Null, completed: true));
			}
		}

		private bool RequestVehicleIfNeeded(int jobIndex, Entity entity, CurrentBuilding currentBuilding, TravelPurpose travelPurpose, CurrentTransport currentTransport, DynamicBuffer<TripNeeded> tripNeededs, HealthProblem healthProblem)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0295: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_037f: Unknown result type (might be due to invalid IL or missing references)
			//IL_038c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0308: Unknown result type (might be due to invalid IL or missing references)
			//IL_030d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0335: Unknown result type (might be due to invalid IL or missing references)
			//IL_033a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0348: Unknown result type (might be due to invalid IL or missing references)
			//IL_0356: Unknown result type (might be due to invalid IL or missing references)
			//IL_035b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0362: Unknown result type (might be due to invalid IL or missing references)
			//IL_0367: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			if (m_HealthcareRequestData.HasComponent(healthProblem.m_HealthcareRequest))
			{
				if (m_DispatchedData.HasComponent(healthProblem.m_HealthcareRequest))
				{
					Dispatched dispatched = m_DispatchedData[healthProblem.m_HealthcareRequest];
					if ((healthProblem.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None)
					{
						if (m_HearseData.HasComponent(dispatched.m_Handler))
						{
							Game.Vehicles.Hearse hearse = m_HearseData[dispatched.m_Handler];
							if (hearse.m_TargetCorpse == entity && (hearse.m_State & HearseFlags.AtTarget) != 0)
							{
								GoToDeathcare(jobIndex, entity, currentBuilding, travelPurpose, currentTransport, tripNeededs, dispatched.m_Handler);
								return false;
							}
						}
						else if (m_AmbulanceData.HasComponent(dispatched.m_Handler))
						{
							Game.Vehicles.Ambulance ambulance = m_AmbulanceData[dispatched.m_Handler];
							if (ambulance.m_TargetPatient == entity && (ambulance.m_State & AmbulanceFlags.AtTarget) != 0)
							{
								GoToHospital(jobIndex, entity, currentBuilding, travelPurpose, currentTransport, tripNeededs, dispatched.m_Handler, immediate: true);
								return false;
							}
						}
					}
					else if (m_AmbulanceData.HasComponent(dispatched.m_Handler))
					{
						Game.Vehicles.Ambulance ambulance2 = m_AmbulanceData[dispatched.m_Handler];
						if (ambulance2.m_TargetPatient == entity && (ambulance2.m_State & AmbulanceFlags.AtTarget) != 0)
						{
							GoToHospital(jobIndex, entity, currentBuilding, travelPurpose, currentTransport, tripNeededs, dispatched.m_Handler, immediate: true);
							return false;
						}
					}
				}
				if (m_CurrentVehicleData.HasComponent(currentTransport.m_CurrentTransport))
				{
					return false;
				}
				if (m_TargetData.HasComponent(currentTransport.m_CurrentTransport) && !m_DeletedData.HasComponent(currentTransport.m_CurrentTransport) && (m_TargetData[currentTransport.m_CurrentTransport].m_Target != Entity.Null || m_DivertData.HasComponent(currentTransport.m_CurrentTransport)))
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val, new ResetTrip
					{
						m_Creature = currentTransport.m_CurrentTransport,
						m_Target = Entity.Null
					});
				}
				return true;
			}
			if (m_CurrentVehicleData.HasComponent(currentTransport.m_CurrentTransport))
			{
				return false;
			}
			HealthcareRequestType healthcareRequestType = (((healthProblem.m_Flags & HealthProblemFlags.Dead) != HealthProblemFlags.None) ? HealthcareRequestType.Hearse : HealthcareRequestType.Ambulance);
			bool flag = true;
			if (healthcareRequestType == HealthcareRequestType.Hearse)
			{
				PrefabRef prefabRef = default(PrefabRef);
				BuildingData buildingData = default(BuildingData);
				flag = m_PrefabRefData.TryGetComponent(currentBuilding.m_CurrentBuilding, ref prefabRef) && m_PrefabBuildingData.TryGetComponent(prefabRef.m_Prefab, ref buildingData) && (buildingData.m_Flags & Game.Prefabs.BuildingFlags.HasInsideRoom) != 0;
			}
			if (flag)
			{
				Entity val2 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_HealthcareRequestArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HealthcareRequest>(jobIndex, val2, new HealthcareRequest(entity, healthcareRequestType));
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<RequestGroup>(jobIndex, val2, new RequestGroup(16u));
			}
			if (m_TargetData.HasComponent(currentTransport.m_CurrentTransport) && !m_DeletedData.HasComponent(currentTransport.m_CurrentTransport))
			{
				if (m_TargetData[currentTransport.m_CurrentTransport].m_Target != Entity.Null || m_DivertData.HasComponent(currentTransport.m_CurrentTransport))
				{
					Entity val3 = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val3, new ResetTrip
					{
						m_Creature = currentTransport.m_CurrentTransport,
						m_Target = Entity.Null
					});
				}
			}
			else if (!flag)
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<CurrentBuilding>(jobIndex, entity);
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<TravelPurpose>(jobIndex, entity, new TravelPurpose
				{
					m_Purpose = Purpose.GoingHome
				});
			}
			return true;
		}

		private void GoToHospital(int jobIndex, Entity entity, CurrentBuilding currentBuilding, TravelPurpose travelPurpose, CurrentTransport currentTransport, DynamicBuffer<TripNeeded> tripNeededs, Entity ambulance, bool immediate)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			if (currentBuilding.m_CurrentBuilding != Entity.Null)
			{
				if (immediate)
				{
					tripNeededs.Clear();
				}
				else
				{
					for (int i = 0; i < tripNeededs.Length; i++)
					{
						if (tripNeededs[i].m_Purpose == Purpose.Hospital)
						{
							return;
						}
					}
				}
				tripNeededs.Add(new TripNeeded
				{
					m_Purpose = Purpose.Hospital,
					m_TargetAgent = ambulance
				});
				if (immediate)
				{
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBuyer>(jobIndex, entity);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, entity);
					((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(jobIndex, entity);
				}
			}
			else if (immediate && ambulance != Entity.Null && m_TargetData.HasComponent(currentTransport.m_CurrentTransport) && !m_DeletedData.HasComponent(currentTransport.m_CurrentTransport) && (!m_CurrentVehicleData.HasComponent(currentTransport.m_CurrentTransport) || !(m_CurrentVehicleData[currentTransport.m_CurrentTransport].m_Vehicle == ambulance)) && (travelPurpose.m_Purpose != Purpose.Hospital || m_TargetData[currentTransport.m_CurrentTransport].m_Target != ambulance || m_DivertData.HasComponent(currentTransport.m_CurrentTransport)))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val, new ResetTrip
				{
					m_Creature = currentTransport.m_CurrentTransport,
					m_Target = ambulance,
					m_TravelPurpose = Purpose.Hospital
				});
			}
		}

		private void GoToDeathcare(int jobIndex, Entity entity, CurrentBuilding currentBuilding, TravelPurpose travelPurpose, CurrentTransport currentTransport, DynamicBuffer<TripNeeded> tripNeededs, Entity hearse)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			if (currentBuilding.m_CurrentBuilding != Entity.Null)
			{
				tripNeededs.Clear();
				tripNeededs.Add(new TripNeeded
				{
					m_Purpose = Purpose.Deathcare,
					m_TargetAgent = hearse
				});
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBuyer>(jobIndex, entity);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, entity);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(jobIndex, entity);
			}
			else if (hearse != Entity.Null && m_TargetData.HasComponent(currentTransport.m_CurrentTransport) && !m_DeletedData.HasComponent(currentTransport.m_CurrentTransport) && (!m_CurrentVehicleData.HasComponent(currentTransport.m_CurrentTransport) || !(m_CurrentVehicleData[currentTransport.m_CurrentTransport].m_Vehicle == hearse)) && (travelPurpose.m_Purpose != Purpose.Deathcare || m_TargetData[currentTransport.m_CurrentTransport].m_Target != hearse || m_DivertData.HasComponent(currentTransport.m_CurrentTransport)))
			{
				Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(jobIndex, m_ResetTripArchetype);
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<ResetTrip>(jobIndex, val, new ResetTrip
				{
					m_Creature = currentTransport.m_CurrentTransport,
					m_Target = hearse,
					m_TravelPurpose = Purpose.Deathcare
				});
			}
		}

		private void GoToSafety(int jobIndex, Entity entity, CurrentBuilding currentBuilding, TravelPurpose travelPurpose, CurrentTransport currentTransport, DynamicBuffer<TripNeeded> tripNeededs)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			if (currentBuilding.m_CurrentBuilding != Entity.Null)
			{
				tripNeededs.Clear();
				tripNeededs.Add(new TripNeeded
				{
					m_Purpose = Purpose.Safety
				});
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<ResourceBuyer>(jobIndex, entity);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<TravelPurpose>(jobIndex, entity);
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<AttendingMeeting>(jobIndex, entity);
			}
		}

		private void AddJournalData(int chunkIndex, HealthProblem problem)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(chunkIndex, m_JournalDataArchetype);
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AddEventJournalData>(chunkIndex, val, new AddEventJournalData(problem.m_Event, EventDataTrackingType.Casualties));
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
		public ComponentTypeHandle<CurrentBuilding> __Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CurrentTransport> __Game_Citizens_CurrentTransport_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Citizen> __Game_Citizens_Citizen_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<TravelPurpose> __Game_Citizens_TravelPurpose_RO_ComponentTypeHandle;

		public SharedComponentTypeHandle<UpdateFrame> __Game_Simulation_UpdateFrame_SharedComponentTypeHandle;

		public ComponentTypeHandle<HealthProblem> __Game_Citizens_HealthProblem_RW_ComponentTypeHandle;

		public BufferTypeHandle<TripNeeded> __Game_Citizens_TripNeeded_RW_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<HouseholdMember> __Game_Citizens_HouseholdMember_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<HealthcareRequest> __Game_Simulation_HealthcareRequest_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.Hospital> __Game_Buildings_Hospital_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.DeathcareFacility> __Game_Buildings_DeathcareFacility_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<OnFire> __Game_Events_OnFire_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Deleted> __Game_Common_Deleted_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Dispatched> __Game_Simulation_Dispatched_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Target> __Game_Common_Target_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Static> __Game_Objects_Static_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.OutsideConnection> __Game_Objects_OutsideConnection_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Ambulance> __Game_Vehicles_Ambulance_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Hearse> __Game_Vehicles_Hearse_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CurrentVehicle> __Game_Creatures_CurrentVehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Divert> __Game_Creatures_Divert_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<HouseholdCitizen> __Game_Citizens_HouseholdCitizen_RO_BufferLookup;

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
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentBuilding>(true);
			__Game_Citizens_CurrentTransport_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CurrentTransport>(true);
			__Game_Citizens_Citizen_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Citizen>(true);
			__Game_Citizens_TravelPurpose_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<TravelPurpose>(true);
			__Game_Simulation_UpdateFrame_SharedComponentTypeHandle = ((SystemState)(ref state)).GetSharedComponentTypeHandle<UpdateFrame>();
			__Game_Citizens_HealthProblem_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HealthProblem>(false);
			__Game_Citizens_TripNeeded_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<TripNeeded>(false);
			__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<HouseholdMember>(true);
			__Game_Simulation_HealthcareRequest_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthcareRequest>(true);
			__Game_Buildings_Hospital_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.Hospital>(true);
			__Game_Buildings_DeathcareFacility_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.DeathcareFacility>(true);
			__Game_Events_OnFire_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<OnFire>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Common_Deleted_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Deleted>(true);
			__Game_Simulation_Dispatched_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Dispatched>(true);
			__Game_Common_Target_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Target>(true);
			__Game_Objects_Static_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Static>(true);
			__Game_Objects_OutsideConnection_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.OutsideConnection>(true);
			__Game_Vehicles_Ambulance_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Ambulance>(true);
			__Game_Vehicles_Hearse_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Hearse>(true);
			__Game_Creatures_CurrentVehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CurrentVehicle>(true);
			__Game_Creatures_Divert_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Divert>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Citizens_HouseholdCitizen_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<HouseholdCitizen>(true);
		}
	}

	private const uint SYSTEM_UPDATE_INTERVAL = 16u;

	private EndFrameBarrier m_EndFrameBarrier;

	private SimulationSystem m_SimulationSystem;

	private IconCommandSystem m_IconCommandSystem;

	private EntityArchetype m_HealthcareRequestArchetype;

	private EntityArchetype m_JournalDataArchetype;

	private EntityArchetype m_ResetTripArchetype;

	private EntityArchetype m_HandleRequestArchetype;

	private EntityQuery m_HealthProblemQuery;

	private EntityQuery m_HealthcareSettingsQuery;

	private EntityQuery m_FireSettingsQuery;

	private TriggerSystem m_TriggerSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_IconCommandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconCommandSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_HealthProblemQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.ReadOnly<UpdateFrame>(),
			ComponentType.ReadWrite<HealthProblem>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_HealthcareSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_FireSettingsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<FireConfigurationData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HealthcareRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<ServiceRequest>(),
			ComponentType.ReadWrite<HealthcareRequest>(),
			ComponentType.ReadWrite<RequestGroup>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_JournalDataArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<AddEventJournalData>(),
			ComponentType.ReadWrite<Game.Common.Event>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_ResetTripArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<ResetTrip>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		m_HandleRequestArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<HandleRequest>(),
			ComponentType.ReadWrite<Game.Common.Event>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_HealthProblemQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_HealthcareSettingsQuery);
		Assert.IsTrue(true);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		uint updateFrameIndex = (m_SimulationSystem.frameIndex / 16) & 0xF;
		JobHandle deps;
		HealthProblemJob healthProblemJob = new HealthProblemJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentBuildingType = InternalCompilerInterface.GetComponentTypeHandle<CurrentBuilding>(ref __TypeHandle.__Game_Citizens_CurrentBuilding_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentTransportType = InternalCompilerInterface.GetComponentTypeHandle<CurrentTransport>(ref __TypeHandle.__Game_Citizens_CurrentTransport_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CitizenType = InternalCompilerInterface.GetComponentTypeHandle<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TravelPurposeType = InternalCompilerInterface.GetComponentTypeHandle<TravelPurpose>(ref __TypeHandle.__Game_Citizens_TravelPurpose_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UpdateFrameType = InternalCompilerInterface.GetSharedComponentTypeHandle<UpdateFrame>(ref __TypeHandle.__Game_Simulation_UpdateFrame_SharedComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HealthProblemType = InternalCompilerInterface.GetComponentTypeHandle<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TripNeededType = InternalCompilerInterface.GetBufferTypeHandle<TripNeeded>(ref __TypeHandle.__Game_Citizens_TripNeeded_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdMemberType = InternalCompilerInterface.GetComponentTypeHandle<HouseholdMember>(ref __TypeHandle.__Game_Citizens_HouseholdMember_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_HealthcareRequestData = InternalCompilerInterface.GetComponentLookup<HealthcareRequest>(ref __TypeHandle.__Game_Simulation_HealthcareRequest_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HospitalData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.Hospital>(ref __TypeHandle.__Game_Buildings_Hospital_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeathcareFacilityData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.DeathcareFacility>(ref __TypeHandle.__Game_Buildings_DeathcareFacility_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OnFireData = InternalCompilerInterface.GetComponentLookup<OnFire>(ref __TypeHandle.__Game_Events_OnFire_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DeletedData = InternalCompilerInterface.GetComponentLookup<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DispatchedData = InternalCompilerInterface.GetComponentLookup<Dispatched>(ref __TypeHandle.__Game_Simulation_Dispatched_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetData = InternalCompilerInterface.GetComponentLookup<Target>(ref __TypeHandle.__Game_Common_Target_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StaticData = InternalCompilerInterface.GetComponentLookup<Static>(ref __TypeHandle.__Game_Objects_Static_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OutsideConnectionData = InternalCompilerInterface.GetComponentLookup<Game.Objects.OutsideConnection>(ref __TypeHandle.__Game_Objects_OutsideConnection_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AmbulanceData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Ambulance>(ref __TypeHandle.__Game_Vehicles_Ambulance_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HearseData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Hearse>(ref __TypeHandle.__Game_Vehicles_Hearse_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentVehicleData = InternalCompilerInterface.GetComponentLookup<CurrentVehicle>(ref __TypeHandle.__Game_Creatures_CurrentVehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DivertData = InternalCompilerInterface.GetComponentLookup<Divert>(ref __TypeHandle.__Game_Creatures_Divert_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabBuildingData = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HouseholdCitizens = InternalCompilerInterface.GetBufferLookup<HouseholdCitizen>(ref __TypeHandle.__Game_Citizens_HouseholdCitizen_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TriggerBuffer = m_TriggerSystem.CreateActionBuffer().AsParallelWriter(),
			m_StatisticsEventQueue = m_CityStatisticsSystem.GetStatisticsEventQueue(out deps).AsParallelWriter(),
			m_UpdateFrameIndex = updateFrameIndex,
			m_RandomSeed = RandomSeed.Next(),
			m_HealthcareRequestArchetype = m_HealthcareRequestArchetype,
			m_JournalDataArchetype = m_JournalDataArchetype,
			m_ResetTripArchetype = m_ResetTripArchetype,
			m_HandleRequestArchetype = m_HandleRequestArchetype,
			m_HealthcareParameterData = ((EntityQuery)(ref m_HealthcareSettingsQuery)).GetSingleton<HealthcareParameterData>(),
			m_FireConfigurationData = ((EntityQuery)(ref m_FireSettingsQuery)).GetSingleton<FireConfigurationData>(),
			m_IconCommandBuffer = m_IconCommandSystem.CreateCommandBuffer()
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		healthProblemJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		HealthProblemJob healthProblemJob2 = healthProblemJob;
		((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<HealthProblemJob>(healthProblemJob2, m_HealthProblemQuery, JobHandle.CombineDependencies(((SystemBase)this).Dependency, deps));
		m_CityStatisticsSystem.AddWriter(((SystemBase)this).Dependency);
		m_TriggerSystem.AddActionBufferWriter(((SystemBase)this).Dependency);
		m_IconCommandSystem.AddCommandBufferWriter(((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
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
	public HealthProblemSystem()
	{
	}
}
