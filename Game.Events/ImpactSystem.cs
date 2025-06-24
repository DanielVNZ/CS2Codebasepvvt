using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Simulation;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Events;

[CompilerGenerated]
public class ImpactSystem : GameSystemBase
{
	[BurstCompile]
	private struct AddImpactJob : IJob
	{
		[ReadOnly]
		public ComponentTypeHandle<Impact> m_ImpactType;

		[ReadOnly]
		public ComponentLookup<Stopped> m_StoppedData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Car> m_CarData;

		[ReadOnly]
		public ComponentLookup<CarTrailer> m_CarTrailerData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<CarTrailerLane> m_CarTrailerLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> m_PersonalCarData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> m_TaxiData;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> m_ParkingLaneData;

		[ReadOnly]
		public ComponentLookup<GarageLane> m_GarageLaneData;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> m_CarLaneData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public ComponentLookup<Moving> m_MovingData;

		public ComponentLookup<Controller> m_ControllerData;

		public ComponentLookup<InvolvedInAccident> m_InvolvedInAccidentData;

		public BufferLookup<TargetElement> m_TargetElements;

		public BufferLookup<LayoutElement> m_LayoutElements;

		[ReadOnly]
		public NativeList<ArchetypeChunk> m_Chunks;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingCarRemoveTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingPersonalCarAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingTaxiAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingServiceCarAddTypes;

		[ReadOnly]
		public ComponentTypeSet m_ParkedToMovingTrailerAddTypes;

		public EntityCommandBuffer m_CommandBuffer;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0491: Unknown result type (might be due to invalid IL or missing references)
			//IL_0496: Unknown result type (might be due to invalid IL or missing references)
			//IL_049b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0536: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04de: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0567: Unknown result type (might be due to invalid IL or missing references)
			//IL_054a: Unknown result type (might be due to invalid IL or missing references)
			//IL_054f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0554: Unknown result type (might be due to invalid IL or missing references)
			//IL_0523: Unknown result type (might be due to invalid IL or missing references)
			//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0510: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0247: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026a: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_040b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_041f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0349: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0442: Unknown result type (might be due to invalid IL or missing references)
			//IL_042e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0360: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0392: Unknown result type (might be due to invalid IL or missing references)
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0314: Unknown result type (might be due to invalid IL or missing references)
			//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_03db: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			for (int i = 0; i < m_Chunks.Length; i++)
			{
				int num2 = num;
				ArchetypeChunk val = m_Chunks[i];
				num = num2 + ((ArchetypeChunk)(ref val)).Count;
			}
			NativeParallelHashMap<Entity, InvolvedInAccident> val2 = default(NativeParallelHashMap<Entity, InvolvedInAccident>);
			val2._002Ector(num, AllocatorHandle.op_Implicit((Allocator)2));
			InvolvedInAccident involvedInAccident = default(InvolvedInAccident);
			InvolvedInAccident involvedInAccident3 = default(InvolvedInAccident);
			for (int j = 0; j < m_Chunks.Length; j++)
			{
				ArchetypeChunk val3 = m_Chunks[j];
				NativeArray<Impact> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<Impact>(ref m_ImpactType);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					Impact impact = nativeArray[k];
					if (!m_PrefabRefData.HasComponent(impact.m_Target) || (impact.m_CheckStoppedEvent && !m_MovingData.HasComponent(impact.m_Target) && m_InvolvedInAccidentData.TryGetComponent(impact.m_Target, ref involvedInAccident) && involvedInAccident.m_Event == impact.m_Event))
					{
						continue;
					}
					InvolvedInAccident involvedInAccident2 = new InvolvedInAccident(impact.m_Event, impact.m_Severity, m_SimulationFrame);
					if (val2.TryGetValue(impact.m_Target, ref involvedInAccident3))
					{
						if (involvedInAccident2.m_Severity > involvedInAccident3.m_Severity)
						{
							val2[impact.m_Target] = involvedInAccident2;
						}
					}
					else if (m_InvolvedInAccidentData.HasComponent(impact.m_Target))
					{
						involvedInAccident3 = m_InvolvedInAccidentData[impact.m_Target];
						if (involvedInAccident2.m_Severity > involvedInAccident3.m_Severity)
						{
							val2.TryAdd(impact.m_Target, involvedInAccident2);
						}
					}
					else
					{
						val2.TryAdd(impact.m_Target, involvedInAccident2);
					}
					Moving moving = default(Moving);
					if (!((float3)(ref impact.m_VelocityDelta)).Equals(default(float3)) || !((float3)(ref impact.m_AngularVelocityDelta)).Equals(default(float3)))
					{
						if (m_MovingData.HasComponent(impact.m_Target))
						{
							moving = m_MovingData[impact.m_Target];
							ref float3 velocity = ref moving.m_Velocity;
							velocity += impact.m_VelocityDelta;
							ref float3 angularVelocity = ref moving.m_AngularVelocity;
							angularVelocity += impact.m_AngularVelocityDelta;
							m_MovingData[impact.m_Target] = moving;
						}
						else
						{
							ref float3 velocity2 = ref moving.m_Velocity;
							velocity2 += impact.m_VelocityDelta;
							ref float3 angularVelocity2 = ref moving.m_AngularVelocity;
							angularVelocity2 += impact.m_AngularVelocityDelta;
						}
					}
					if (m_VehicleData.HasComponent(impact.m_Target))
					{
						if (m_CarData.HasComponent(impact.m_Target))
						{
							if (m_ParkedCarData.HasComponent(impact.m_Target))
							{
								ActivateParkedCar(impact.m_Target, moving);
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OutOfControl>(impact.m_Target, default(OutOfControl));
							}
							else if (m_CarCurrentLaneData.HasComponent(impact.m_Target))
							{
								if (m_StoppedData.HasComponent(impact.m_Target))
								{
									ActivateStoppedCar(impact.m_Target, moving);
								}
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OutOfControl>(impact.m_Target, default(OutOfControl));
							}
						}
						else
						{
							if (!m_CarTrailerData.HasComponent(impact.m_Target))
							{
								continue;
							}
							if (m_ParkedCarData.HasComponent(impact.m_Target))
							{
								ActivateParkedCarTrailer(impact.m_Target, moving, default(ParkedCar));
							}
							else if (m_CarTrailerLaneData.HasComponent(impact.m_Target))
							{
								if (m_StoppedData.HasComponent(impact.m_Target))
								{
									ActivateStoppedTrailer(impact.m_Target, moving);
								}
								if (m_ControllerData.HasComponent(impact.m_Target))
								{
									DetachVehicle(impact.m_Target);
								}
								((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<OutOfControl>(impact.m_Target, default(OutOfControl));
							}
						}
					}
					else if (m_CreatureData.HasComponent(impact.m_Target))
					{
						if (m_StoppedData.HasComponent(impact.m_Target))
						{
							ActivateStoppedCreature(impact.m_Target, moving);
						}
						((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Stumbling>(impact.m_Target, default(Stumbling));
					}
				}
			}
			if (val2.Count() == 0)
			{
				return;
			}
			NativeArray<Entity> keyArray = val2.GetKeyArray(AllocatorHandle.op_Implicit((Allocator)2));
			for (int l = 0; l < keyArray.Length; l++)
			{
				Entity val4 = keyArray[l];
				InvolvedInAccident involvedInAccident4 = val2[val4];
				if (m_InvolvedInAccidentData.HasComponent(val4))
				{
					if (m_InvolvedInAccidentData[val4].m_Event != involvedInAccident4.m_Event && m_TargetElements.HasBuffer(involvedInAccident4.m_Event))
					{
						CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[involvedInAccident4.m_Event], new TargetElement(val4));
					}
					m_InvolvedInAccidentData[val4] = involvedInAccident4;
				}
				else
				{
					if (m_TargetElements.HasBuffer(involvedInAccident4.m_Event))
					{
						CollectionUtils.TryAddUniqueValue<TargetElement>(m_TargetElements[involvedInAccident4.m_Event], new TargetElement(val4));
					}
					((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InvolvedInAccident>(val4, involvedInAccident4);
				}
			}
		}

		private void DetachVehicle(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Controller controller = m_ControllerData[entity];
			if (controller.m_Controller != Entity.Null && controller.m_Controller != entity)
			{
				DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
				if (m_LayoutElements.TryGetBuffer(controller.m_Controller, ref val))
				{
					CollectionUtils.RemoveValue<LayoutElement>(val, new LayoutElement(entity));
				}
				controller.m_Controller = Entity.Null;
				m_ControllerData[entity] = controller;
			}
		}

		private void ActivateParkedCarTrailer(Entity entity, Moving moving, ParkedCar parkedCar)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			ParkedCar parkedCar2 = m_ParkedCarData[entity];
			if (parkedCar2.m_Lane == Entity.Null)
			{
				parkedCar2.m_Lane = parkedCar.m_Lane;
				parkedCar2.m_CurvePosition = parkedCar.m_CurvePosition;
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(entity, ref m_ParkedToMovingCarRemoveTypes);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(entity, ref m_ParkedToMovingTrailerAddTypes);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Moving>(entity, moving);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<CarTrailerLane>(entity, new CarTrailerLane(parkedCar2));
			if (m_CarLaneData.HasComponent(parkedCar2.m_Lane))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(parkedCar2.m_Lane, default(PathfindUpdated));
			}
		}

		private void ActivateParkedCar(Entity entity, Moving moving)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			ParkedCar parkedCar = m_ParkedCarData[entity];
			Game.Vehicles.CarLaneFlags flags = Game.Vehicles.CarLaneFlags.EndReached | Game.Vehicles.CarLaneFlags.ParkingSpace | Game.Vehicles.CarLaneFlags.FixedLane;
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent(entity, ref m_ParkedToMovingCarRemoveTypes);
			if (m_PersonalCarData.HasComponent(entity))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(entity, ref m_ParkedToMovingPersonalCarAddTypes);
			}
			else if (m_TaxiData.HasComponent(entity))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(entity, ref m_ParkedToMovingTaxiAddTypes);
			}
			else
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent(entity, ref m_ParkedToMovingServiceCarAddTypes);
			}
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<Moving>(entity, moving);
			((EntityCommandBuffer)(ref m_CommandBuffer)).SetComponent<CarCurrentLane>(entity, new CarCurrentLane(parkedCar, flags));
			if (m_ParkingLaneData.HasComponent(parkedCar.m_Lane) || m_GarageLaneData.HasComponent(parkedCar.m_Lane))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(parkedCar.m_Lane);
			}
			DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
			if (!m_LayoutElements.TryGetBuffer(entity, ref val))
			{
				return;
			}
			for (int i = 1; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				if (m_ParkedCarData.HasComponent(vehicle))
				{
					ActivateParkedCarTrailer(vehicle, default(Moving), parkedCar);
				}
			}
		}

		private void ActivateStoppedCar(Entity entity, Moving moving)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			CarCurrentLane carCurrentLane = m_CarCurrentLaneData[entity];
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Stopped>(entity);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Moving>(entity, moving);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<TransformFrame>(entity);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(entity, default(InterpolatedTransform));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Swaying>(entity, default(Swaying));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(entity, default(Updated));
			if (m_CarLaneData.HasComponent(carCurrentLane.m_Lane))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(carCurrentLane.m_Lane, default(PathfindUpdated));
			}
			if (m_CarLaneData.HasComponent(carCurrentLane.m_ChangeLane))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(carCurrentLane.m_ChangeLane, default(PathfindUpdated));
			}
			DynamicBuffer<LayoutElement> val = default(DynamicBuffer<LayoutElement>);
			if (!m_LayoutElements.TryGetBuffer(entity, ref val))
			{
				return;
			}
			for (int i = 1; i < val.Length; i++)
			{
				Entity vehicle = val[i].m_Vehicle;
				if (m_StoppedData.HasComponent(vehicle))
				{
					ActivateStoppedTrailer(vehicle, default(Moving));
				}
			}
		}

		private void ActivateStoppedTrailer(Entity entity, Moving moving)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			CarTrailerLane carTrailerLane = m_CarTrailerLaneData[entity];
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Stopped>(entity);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Moving>(entity, moving);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<TransformFrame>(entity);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(entity, default(InterpolatedTransform));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Swaying>(entity, default(Swaying));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(entity, default(Updated));
			if (m_CarLaneData.HasComponent(carTrailerLane.m_Lane))
			{
				((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<PathfindUpdated>(carTrailerLane.m_Lane, default(PathfindUpdated));
			}
		}

		private void ActivateStoppedCreature(Entity entity, Moving moving)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			((EntityCommandBuffer)(ref m_CommandBuffer)).RemoveComponent<Stopped>(entity);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddBuffer<TransformFrame>(entity);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<InterpolatedTransform>(entity, default(InterpolatedTransform));
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Moving>(entity, moving);
			((EntityCommandBuffer)(ref m_CommandBuffer)).AddComponent<Updated>(entity, default(Updated));
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<Impact> __Game_Events_Impact_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Stopped> __Game_Objects_Stopped_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Car> __Game_Vehicles_Car_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarTrailer> __Game_Vehicles_CarTrailer_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarTrailerLane> __Game_Vehicles_CarTrailerLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PersonalCar> __Game_Vehicles_PersonalCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.Taxi> __Game_Vehicles_Taxi_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Creature> __Game_Creatures_Creature_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.ParkingLane> __Game_Net_ParkingLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<GarageLane> __Game_Net_GarageLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Net.CarLane> __Game_Net_CarLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		public ComponentLookup<Moving> __Game_Objects_Moving_RW_ComponentLookup;

		public ComponentLookup<Controller> __Game_Vehicles_Controller_RW_ComponentLookup;

		public ComponentLookup<InvolvedInAccident> __Game_Events_InvolvedInAccident_RW_ComponentLookup;

		public BufferLookup<TargetElement> __Game_Events_TargetElement_RW_BufferLookup;

		public BufferLookup<LayoutElement> __Game_Vehicles_LayoutElement_RW_BufferLookup;

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
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			__Game_Events_Impact_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Impact>(true);
			__Game_Objects_Stopped_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stopped>(true);
			__Game_Vehicles_Vehicle_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Vehicle>(true);
			__Game_Vehicles_Car_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Car>(true);
			__Game_Vehicles_CarTrailer_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailer>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(true);
			__Game_Vehicles_CarTrailerLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarTrailerLane>(true);
			__Game_Vehicles_PersonalCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.PersonalCar>(true);
			__Game_Vehicles_Taxi_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.Taxi>(true);
			__Game_Creatures_Creature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Creature>(true);
			__Game_Net_ParkingLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.ParkingLane>(true);
			__Game_Net_GarageLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<GarageLane>(true);
			__Game_Net_CarLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Net.CarLane>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Objects_Moving_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Moving>(false);
			__Game_Vehicles_Controller_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(false);
			__Game_Events_InvolvedInAccident_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<InvolvedInAccident>(false);
			__Game_Events_TargetElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<TargetElement>(false);
			__Game_Vehicles_LayoutElement_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<LayoutElement>(false);
		}
	}

	private ModificationBarrier4 m_ModificationBarrier;

	private SimulationSystem m_SimulationSystem;

	private EntityQuery m_ImpactQuery;

	private ComponentTypeSet m_ParkedToMovingCarRemoveTypes;

	private ComponentTypeSet m_ParkedToMovingPersonalCarAddTypes;

	private ComponentTypeSet m_ParkedToMovingTaxiAddTypes;

	private ComponentTypeSet m_ParkedToMovingServiceCarAddTypes;

	private ComponentTypeSet m_ParkedToMovingTrailerAddTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier4>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ImpactQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Impact>(),
			ComponentType.ReadOnly<Game.Common.Event>()
		});
		m_ParkedToMovingCarRemoveTypes = new ComponentTypeSet(ComponentType.ReadWrite<ParkedCar>(), ComponentType.ReadWrite<Stopped>());
		m_ParkedToMovingPersonalCarAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[12]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<CarNavigation>(),
			ComponentType.ReadWrite<CarNavigationLane>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_ParkedToMovingTaxiAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[13]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<CarNavigation>(),
			ComponentType.ReadWrite<CarNavigationLane>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<ServiceDispatch>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_ParkedToMovingServiceCarAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[14]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<CarNavigation>(),
			ComponentType.ReadWrite<CarNavigationLane>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.ReadWrite<Blocker>(),
			ComponentType.ReadWrite<PathElement>(),
			ComponentType.ReadWrite<PathInformation>(),
			ComponentType.ReadWrite<ServiceDispatch>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<Updated>()
		});
		m_ParkedToMovingTrailerAddTypes = new ComponentTypeSet((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadWrite<Moving>(),
			ComponentType.ReadWrite<TransformFrame>(),
			ComponentType.ReadWrite<InterpolatedTransform>(),
			ComponentType.ReadWrite<CarTrailerLane>(),
			ComponentType.ReadWrite<Swaying>(),
			ComponentType.ReadWrite<Updated>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_ImpactQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		JobHandle val = default(JobHandle);
		NativeList<ArchetypeChunk> chunks = ((EntityQuery)(ref m_ImpactQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val);
		JobHandle val2 = IJobExtensions.Schedule<AddImpactJob>(new AddImpactJob
		{
			m_ImpactType = InternalCompilerInterface.GetComponentTypeHandle<Impact>(ref __TypeHandle.__Game_Events_Impact_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_StoppedData = InternalCompilerInterface.GetComponentLookup<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleData = InternalCompilerInterface.GetComponentLookup<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarData = InternalCompilerInterface.GetComponentLookup<Car>(ref __TypeHandle.__Game_Vehicles_Car_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarTrailerData = InternalCompilerInterface.GetComponentLookup<CarTrailer>(ref __TypeHandle.__Game_Vehicles_CarTrailer_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneData = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarTrailerLaneData = InternalCompilerInterface.GetComponentLookup<CarTrailerLane>(ref __TypeHandle.__Game_Vehicles_CarTrailerLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PersonalCarData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.PersonalCar>(ref __TypeHandle.__Game_Vehicles_PersonalCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TaxiData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.Taxi>(ref __TypeHandle.__Game_Vehicles_Taxi_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureData = InternalCompilerInterface.GetComponentLookup<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkingLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.ParkingLane>(ref __TypeHandle.__Game_Net_ParkingLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_GarageLaneData = InternalCompilerInterface.GetComponentLookup<GarageLane>(ref __TypeHandle.__Game_Net_GarageLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_CarLaneData = InternalCompilerInterface.GetComponentLookup<Game.Net.CarLane>(ref __TypeHandle.__Game_Net_CarLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MovingData = InternalCompilerInterface.GetComponentLookup<Moving>(ref __TypeHandle.__Game_Objects_Moving_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InvolvedInAccidentData = InternalCompilerInterface.GetComponentLookup<InvolvedInAccident>(ref __TypeHandle.__Game_Events_InvolvedInAccident_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TargetElements = InternalCompilerInterface.GetBufferLookup<TargetElement>(ref __TypeHandle.__Game_Events_TargetElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElements = InternalCompilerInterface.GetBufferLookup<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Chunks = chunks,
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_ParkedToMovingCarRemoveTypes = m_ParkedToMovingCarRemoveTypes,
			m_ParkedToMovingPersonalCarAddTypes = m_ParkedToMovingPersonalCarAddTypes,
			m_ParkedToMovingTaxiAddTypes = m_ParkedToMovingTaxiAddTypes,
			m_ParkedToMovingServiceCarAddTypes = m_ParkedToMovingServiceCarAddTypes,
			m_ParkedToMovingTrailerAddTypes = m_ParkedToMovingTrailerAddTypes,
			m_CommandBuffer = m_ModificationBarrier.CreateCommandBuffer()
		}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, val));
		chunks.Dispose(val2);
		((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val2;
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
	public ImpactSystem()
	{
	}
}
