using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Simulation;

public struct HealthcarePathfindSetup
{
	[BurstCompile]
	private struct SetupAmbulancesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Hospital> m_HospitalType;

		[ReadOnly]
		public ComponentTypeHandle<Ambulance> m_AmbulanceType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		[ReadOnly]
		public ComponentLookup<Game.City.City> m_CityData;

		[ReadOnly]
		public Entity m_City;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_018e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_023d: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>() && !CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Hospital> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Hospital>(ref m_HospitalType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					Hospital hospital = nativeArray2[i];
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var targetSeeker);
						RoadTypes roadTypes = RoadTypes.None;
						if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
						{
							if ((hospital.m_Flags & HospitalFlags.HasAvailableAmbulances) != 0)
							{
								roadTypes |= RoadTypes.Car;
							}
							if ((hospital.m_Flags & HospitalFlags.HasAvailableMedicalHelicopters) != 0)
							{
								roadTypes |= RoadTypes.Helicopter;
							}
						}
						roadTypes &= targetSeeker.m_SetupQueueTarget.m_RoadTypes | targetSeeker.m_SetupQueueTarget.m_FlyingTypes;
						if (roadTypes != RoadTypes.None)
						{
							float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
							RoadTypes roadTypes2 = targetSeeker.m_SetupQueueTarget.m_RoadTypes;
							RoadTypes flyingTypes = targetSeeker.m_SetupQueueTarget.m_FlyingTypes;
							targetSeeker.m_SetupQueueTarget.m_RoadTypes &= roadTypes;
							targetSeeker.m_SetupQueueTarget.m_FlyingTypes &= roadTypes;
							targetSeeker.FindTargets(val, cost);
							targetSeeker.m_SetupQueueTarget.m_RoadTypes = roadTypes2;
							targetSeeker.m_SetupQueueTarget.m_FlyingTypes = flyingTypes;
						}
					}
				}
				return;
			}
			NativeArray<Ambulance> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Ambulance>(ref m_AmbulanceType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<PathOwner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Ambulance ambulance = nativeArray3[k];
				if (nativeArray4.Length != 0)
				{
					if ((ambulance.m_State & (AmbulanceFlags.Returning | AmbulanceFlags.Dispatched | AmbulanceFlags.Transporting | AmbulanceFlags.Disabled)) != AmbulanceFlags.Returning)
					{
						continue;
					}
				}
				else if ((ambulance.m_State & AmbulanceFlags.Disabled) != 0)
				{
					continue;
				}
				Entity entity2 = nativeArray[k];
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out var entity3, out var targetSeeker2);
					if (nativeArray5.Length == 0 || AreaUtils.CheckServiceDistrict(entity3, nativeArray5[k].m_Owner, m_ServiceDistricts))
					{
						targetSeeker2.FindTargets(entity2, 0f);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SetupHospitalsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Hospital> m_HospitalType;

		[ReadOnly]
		public ComponentLookup<Citizen> m_CitizenData;

		[ReadOnly]
		public ComponentLookup<HealthProblem> m_HealthProblemData;

		[ReadOnly]
		public ComponentLookup<Ambulance> m_AmbulanceData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Hospital> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Hospital>(ref m_HospitalType);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var entity, out var owner, out var targetSeeker);
				int num = 0;
				HealthProblemFlags healthProblemFlags = HealthProblemFlags.None;
				Entity val = owner;
				Entity val2 = Entity.Null;
				if (m_AmbulanceData.HasComponent(owner))
				{
					val = m_AmbulanceData[owner].m_TargetPatient;
					if (targetSeeker.m_Owner.HasComponent(owner))
					{
						val2 = targetSeeker.m_Owner[owner].m_Owner;
					}
				}
				if (m_CitizenData.HasComponent(val))
				{
					num = m_CitizenData[val].m_Health;
				}
				if (m_HealthProblemData.HasComponent(val))
				{
					healthProblemFlags = m_HealthProblemData[val].m_Flags;
				}
				for (int j = 0; j < nativeArray2.Length; j++)
				{
					Entity val3 = nativeArray[j];
					Hospital hospital = nativeArray2[j];
					if (((healthProblemFlags & HealthProblemFlags.Sick) != HealthProblemFlags.None && (hospital.m_Flags & HospitalFlags.CanCureDisease) == 0) || ((healthProblemFlags & HealthProblemFlags.Injured) != HealthProblemFlags.None && (hospital.m_Flags & HospitalFlags.CanCureInjury) == 0) || num < hospital.m_MinHealth || num > hospital.m_MaxHealth)
					{
						continue;
					}
					PathMethod pathMethod = targetSeeker.m_SetupQueueTarget.m_Methods;
					RoadTypes roadTypes = targetSeeker.m_SetupQueueTarget.m_RoadTypes;
					if (!AreaUtils.CheckServiceDistrict(entity, val3, m_ServiceDistricts))
					{
						pathMethod &= ~PathMethod.Pedestrian;
					}
					if ((pathMethod & PathMethod.Pedestrian) != 0 || roadTypes != RoadTypes.None)
					{
						float num2 = (255f - (float)(int)hospital.m_TreatmentBonus) * 200f / (20f + (float)num);
						if (val3 != val2)
						{
							num2 += 10f;
						}
						if ((hospital.m_Flags & HospitalFlags.HasRoomForPatients) == 0)
						{
							num2 += 120f;
						}
						PathMethod methods = targetSeeker.m_SetupQueueTarget.m_Methods;
						RoadTypes roadTypes2 = targetSeeker.m_SetupQueueTarget.m_RoadTypes;
						targetSeeker.m_SetupQueueTarget.m_Methods = pathMethod;
						targetSeeker.m_SetupQueueTarget.m_RoadTypes = roadTypes;
						targetSeeker.FindTargets(val3, num2);
						targetSeeker.m_SetupQueueTarget.m_Methods = methods;
						targetSeeker.m_SetupQueueTarget.m_RoadTypes = roadTypes2;
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SetupHearsesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<DeathcareFacility> m_DeathcareFacilityType;

		[ReadOnly]
		public ComponentLookup<Game.City.City> m_CityData;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public ComponentTypeHandle<Hearse> m_HearseType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0193: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			if (((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>() && !CityUtils.CheckOption(m_CityData[m_City], CityOption.ImportOutsideServices))
			{
				return;
			}
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<DeathcareFacility> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<DeathcareFacility>(ref m_DeathcareFacilityType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Entity val = nativeArray[i];
					if ((nativeArray2[i].m_Flags & (DeathcareFacilityFlags.HasAvailableHearses | DeathcareFacilityFlags.HasRoomForBodies)) != (DeathcareFacilityFlags.HasAvailableHearses | DeathcareFacilityFlags.HasRoomForBodies))
					{
						continue;
					}
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var targetSeeker);
						if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
						{
							float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
							targetSeeker.FindTargets(val, cost);
						}
					}
				}
				return;
			}
			NativeArray<Hearse> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Hearse>(ref m_HearseType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<PathOwner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Hearse hearse = nativeArray3[k];
				if (nativeArray4.Length != 0)
				{
					if ((hearse.m_State & (HearseFlags.Returning | HearseFlags.Dispatched | HearseFlags.Transporting | HearseFlags.Disabled)) != HearseFlags.Returning)
					{
						continue;
					}
				}
				else if ((hearse.m_State & HearseFlags.Disabled) != 0)
				{
					continue;
				}
				Entity entity2 = nativeArray[k];
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out var entity3, out var targetSeeker2);
					if (nativeArray5.Length == 0 || AreaUtils.CheckServiceDistrict(entity3, nativeArray5[k].m_Owner, m_ServiceDistricts))
					{
						targetSeeker2.FindTargets(entity2, 0f);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct HealthcareRequestsJob : IJobChunk
	{
		private struct DistrictIterator : INativeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<AreaSearchItem, QuadTreeBoundsXZ>
		{
			public float2 m_Position;

			public ComponentLookup<District> m_DistrictData;

			public BufferLookup<Game.Areas.Node> m_Nodes;

			public BufferLookup<Triangle> m_Triangles;

			public Entity m_Result;

			public bool Intersect(QuadTreeBoundsXZ bounds)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				return MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Position);
			}

			public void Iterate(QuadTreeBoundsXZ bounds, AreaSearchItem areaItem)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_003f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0047: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0062: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				if (MathUtils.Intersect(((Bounds3)(ref bounds.m_Bounds)).xz, m_Position) && m_DistrictData.HasComponent(areaItem.m_Area))
				{
					DynamicBuffer<Game.Areas.Node> nodes = m_Nodes[areaItem.m_Area];
					DynamicBuffer<Triangle> val = m_Triangles[areaItem.m_Area];
					float2 val2 = default(float2);
					if (val.Length > areaItem.m_Triangle && MathUtils.Intersect(AreaUtils.GetTriangle2(nodes, val[areaItem.m_Triangle]), m_Position, ref val2))
					{
						m_Result = areaItem.m_Area;
					}
				}
			}
		}

		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentTypeHandle<HealthcareRequest> m_HealthcareRequestType;

		[ReadOnly]
		public ComponentLookup<HealthcareRequest> m_HealthcareRequestData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<District> m_DistrictData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		[ReadOnly]
		public NativeQuadTree<AreaSearchItem, QuadTreeBoundsXZ> m_AreaTree;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0253: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_0298: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<HealthcareRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<HealthcareRequest>(ref m_HealthcareRequestType);
			HealthcareRequest healthcareRequest = default(HealthcareRequest);
			Owner owner2 = default(Owner);
			CurrentBuilding currentBuilding = default(CurrentBuilding);
			CurrentTransport currentTransport = default(CurrentTransport);
			Transform transform = default(Transform);
			Transform transform2 = default(Transform);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_HealthcareRequestData.TryGetComponent(owner, ref healthcareRequest))
				{
					continue;
				}
				Entity service = Entity.Null;
				if (m_VehicleData.HasComponent(healthcareRequest.m_Citizen))
				{
					if (targetSeeker.m_Owner.TryGetComponent(healthcareRequest.m_Citizen, ref owner2))
					{
						service = owner2.m_Owner;
					}
				}
				else
				{
					if (!targetSeeker.m_PrefabRef.HasComponent(healthcareRequest.m_Citizen))
					{
						continue;
					}
					service = healthcareRequest.m_Citizen;
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						continue;
					}
					HealthcareRequest healthcareRequest2 = nativeArray3[j];
					if (healthcareRequest2.m_Type != healthcareRequest.m_Type)
					{
						continue;
					}
					Entity district = Entity.Null;
					if (targetSeeker.m_CurrentBuilding.TryGetComponent(healthcareRequest2.m_Citizen, ref currentBuilding))
					{
						if (m_CurrentDistrictData.HasComponent(currentBuilding.m_CurrentBuilding))
						{
							district = m_CurrentDistrictData[currentBuilding.m_CurrentBuilding].m_District;
						}
					}
					else if (targetSeeker.m_CurrentTransport.TryGetComponent(healthcareRequest2.m_Citizen, ref currentTransport) && targetSeeker.m_Transform.TryGetComponent(currentTransport.m_CurrentTransport, ref transform))
					{
						DistrictIterator districtIterator = new DistrictIterator
						{
							m_Position = ((float3)(ref transform.m_Position)).xz,
							m_DistrictData = m_DistrictData,
							m_Nodes = targetSeeker.m_AreaNode,
							m_Triangles = targetSeeker.m_AreaTriangle
						};
						m_AreaTree.Iterate<DistrictIterator>(ref districtIterator, 0);
						district = districtIterator.m_Result;
					}
					if (!AreaUtils.CheckServiceDistrict(district, service, m_ServiceDistricts))
					{
						continue;
					}
					targetSeeker.FindTargets(nativeArray[j], healthcareRequest2.m_Citizen, 0f, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
					Entity val = healthcareRequest2.m_Citizen;
					if (targetSeeker.m_CurrentTransport.HasComponent(val))
					{
						val = targetSeeker.m_CurrentTransport[val].m_CurrentTransport;
					}
					else if (targetSeeker.m_CurrentBuilding.HasComponent(val))
					{
						val = targetSeeker.m_CurrentBuilding[val].m_CurrentBuilding;
					}
					if (targetSeeker.m_Transform.TryGetComponent(val, ref transform2) && (targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Flying) != 0 && (targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Helicopter) != RoadTypes.None)
					{
						Entity lane = Entity.Null;
						float curvePos = 0f;
						float distance = float.MaxValue;
						targetSeeker.m_AirwayData.helicopterMap.FindClosestLane(transform2.m_Position, targetSeeker.m_Curve, ref lane, ref curvePos, ref distance);
						if (lane != Entity.Null)
						{
							targetSeeker.m_Buffer.Enqueue(new PathTarget(nativeArray[j], lane, curvePos, 0f));
						}
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private EntityQuery m_AmbulanceQuery;

	private EntityQuery m_HospitalQuery;

	private EntityQuery m_HearseQuery;

	private EntityQuery m_HealthcareRequestQuery;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<Owner> m_OwnerType;

	private ComponentTypeHandle<PathOwner> m_PathOwnerType;

	private ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

	private ComponentTypeHandle<HealthcareRequest> m_HealthcareRequestType;

	private ComponentTypeHandle<Hospital> m_HospitalType;

	private ComponentTypeHandle<DeathcareFacility> m_DeathcareFacilityType;

	private ComponentTypeHandle<Hearse> m_HearseType;

	private ComponentTypeHandle<Ambulance> m_AmbulanceType;

	private ComponentLookup<HealthcareRequest> m_HealthcareRequestData;

	private ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

	private ComponentLookup<District> m_DistrictData;

	private ComponentLookup<Citizen> m_CitizenData;

	private ComponentLookup<HealthProblem> m_HealthProblemData;

	private ComponentLookup<Vehicle> m_VehicleData;

	private ComponentLookup<Ambulance> m_AmbulanceData;

	private BufferLookup<ServiceDistrict> m_ServiceDistricts;

	private ComponentLookup<Game.City.City> m_CityData;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private CitySystem m_CitySystem;

	public HealthcarePathfindSetup(PathfindSetupSystem system)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Hospital>(),
			ComponentType.ReadOnly<Ambulance>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_AmbulanceQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		m_HospitalQuery = system.GetSetupQuery(ComponentType.ReadOnly<Hospital>(), ComponentType.ReadOnly<ServiceDispatch>(), ComponentType.Exclude<Temp>(), ComponentType.Exclude<Destroyed>(), ComponentType.Exclude<Deleted>());
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<DeathcareFacility>(),
			ComponentType.ReadOnly<Hearse>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		m_HearseQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array2);
		m_HealthcareRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<HealthcareRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_OwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<Owner>(true);
		m_PathOwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<PathOwner>(true);
		m_ServiceRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<ServiceRequest>(true);
		m_HealthcareRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<HealthcareRequest>(true);
		m_HospitalType = ((ComponentSystemBase)system).GetComponentTypeHandle<Hospital>(true);
		m_DeathcareFacilityType = ((ComponentSystemBase)system).GetComponentTypeHandle<DeathcareFacility>(true);
		m_HearseType = ((ComponentSystemBase)system).GetComponentTypeHandle<Hearse>(true);
		m_AmbulanceType = ((ComponentSystemBase)system).GetComponentTypeHandle<Ambulance>(true);
		m_HealthcareRequestData = ((SystemBase)system).GetComponentLookup<HealthcareRequest>(true);
		m_CurrentDistrictData = ((SystemBase)system).GetComponentLookup<CurrentDistrict>(true);
		m_DistrictData = ((SystemBase)system).GetComponentLookup<District>(true);
		m_CitizenData = ((SystemBase)system).GetComponentLookup<Citizen>(true);
		m_HealthProblemData = ((SystemBase)system).GetComponentLookup<HealthProblem>(true);
		m_VehicleData = ((SystemBase)system).GetComponentLookup<Vehicle>(true);
		m_AmbulanceData = ((SystemBase)system).GetComponentLookup<Ambulance>(true);
		m_ServiceDistricts = ((SystemBase)system).GetBufferLookup<ServiceDistrict>(true);
		m_CityData = ((SystemBase)system).GetComponentLookup<Game.City.City>(true);
		m_AreaSearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_CitySystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<CitySystem>();
	}

	public JobHandle SetupAmbulances(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_HospitalType.Update((SystemBase)(object)system);
		m_AmbulanceType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		m_CityData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupAmbulancesJob>(new SetupAmbulancesJob
		{
			m_EntityType = m_EntityType,
			m_HospitalType = m_HospitalType,
			m_AmbulanceType = m_AmbulanceType,
			m_OwnerType = m_OwnerType,
			m_PathOwnerType = m_PathOwnerType,
			m_ServiceDistricts = m_ServiceDistricts,
			m_CityData = m_CityData,
			m_City = m_CitySystem.City,
			m_SetupData = setupData
		}, m_AmbulanceQuery, inputDeps);
	}

	public JobHandle SetupHospitals(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_HospitalType.Update((SystemBase)(object)system);
		m_CitizenData.Update((SystemBase)(object)system);
		m_HealthProblemData.Update((SystemBase)(object)system);
		m_AmbulanceData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupHospitalsJob>(new SetupHospitalsJob
		{
			m_EntityType = m_EntityType,
			m_HospitalType = m_HospitalType,
			m_CitizenData = m_CitizenData,
			m_HealthProblemData = m_HealthProblemData,
			m_AmbulanceData = m_AmbulanceData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_HospitalQuery, inputDeps);
	}

	public JobHandle SetupHearses(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_DeathcareFacilityType.Update((SystemBase)(object)system);
		m_HearseType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		m_CityData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupHearsesJob>(new SetupHearsesJob
		{
			m_EntityType = m_EntityType,
			m_DeathcareFacilityType = m_DeathcareFacilityType,
			m_HearseType = m_HearseType,
			m_CityData = m_CityData,
			m_OwnerType = m_OwnerType,
			m_PathOwnerType = m_PathOwnerType,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData,
			m_City = m_CitySystem.City
		}, m_HearseQuery, inputDeps);
	}

	public JobHandle SetupHealthcareRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
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
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_HealthcareRequestType.Update((SystemBase)(object)system);
		m_HealthcareRequestData.Update((SystemBase)(object)system);
		m_CurrentDistrictData.Update((SystemBase)(object)system);
		m_DistrictData.Update((SystemBase)(object)system);
		m_VehicleData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		JobHandle dependencies;
		JobHandle val = JobChunkExtensions.ScheduleParallel<HealthcareRequestsJob>(new HealthcareRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_HealthcareRequestType = m_HealthcareRequestType,
			m_HealthcareRequestData = m_HealthcareRequestData,
			m_CurrentDistrictData = m_CurrentDistrictData,
			m_DistrictData = m_DistrictData,
			m_VehicleData = m_VehicleData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_AreaTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies),
			m_SetupData = setupData
		}, m_HealthcareRequestQuery, JobHandle.CombineDependencies(inputDeps, dependencies));
		m_AreaSearchSystem.AddSearchTreeReader(val);
		return val;
	}
}
