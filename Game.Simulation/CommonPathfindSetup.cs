using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Events;
using Game.Net;
using Game.Pathfind;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Simulation;

public struct CommonPathfindSetup
{
	[BurstCompile]
	private struct SetupCurrentLocationJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public ComponentLookup<PathOwner> m_PathOwnerData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(int index)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0319: Unknown result type (might be due to invalid IL or missing references)
			//IL_031a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0321: Unknown result type (might be due to invalid IL or missing references)
			//IL_0324: Unknown result type (might be due to invalid IL or missing references)
			//IL_0329: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0331: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_033d: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_035f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_036c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0371: Unknown result type (might be due to invalid IL or missing references)
			//IL_0388: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0258: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026b: Unknown result type (might be due to invalid IL or missing references)
			//IL_026d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
			m_SetupData.GetItem(index, out var entity, out var targetSeeker);
			float num = targetSeeker.m_SetupQueueTarget.m_Value2;
			if ((targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) != 0 && m_VehicleData.HasComponent(entity))
			{
				num = math.max(10f, num);
			}
			EdgeFlags edgeFlags = EdgeFlags.DefaultMask;
			if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.SecondaryPath) != SetupTargetFlags.None)
			{
				edgeFlags |= EdgeFlags.Secondary;
			}
			PathElement pathElement = default(PathElement);
			bool flag = false;
			bool flag2 = false;
			if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.PathEnd) != SetupTargetFlags.None)
			{
				flag = true;
				PathOwner pathOwner = default(PathOwner);
				DynamicBuffer<PathElement> val = default(DynamicBuffer<PathElement>);
				if (m_PathOwnerData.TryGetComponent(entity, ref pathOwner) && m_PathElements.TryGetBuffer(entity, ref val) && pathOwner.m_ElementIndex < val.Length)
				{
					pathElement = val[val.Length - 1];
					flag2 = true;
				}
			}
			if (flag2)
			{
				targetSeeker.m_Buffer.Enqueue(new PathTarget(entity, pathElement.m_Target, pathElement.m_TargetDelta.y, 0f));
			}
			else
			{
				if ((targetSeeker.FindTargets(entity, entity, 0f, edgeFlags, allowAccessRestriction: true, flag) != 0 && flag) || ((targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Flying) == 0 && num <= 0f))
				{
					return;
				}
				Entity val2 = entity;
				if (targetSeeker.m_CurrentTransport.HasComponent(val2))
				{
					val2 = targetSeeker.m_CurrentTransport[val2].m_CurrentTransport;
				}
				else if (targetSeeker.m_CurrentBuilding.HasComponent(val2))
				{
					val2 = targetSeeker.m_CurrentBuilding[val2].m_CurrentBuilding;
				}
				if (!targetSeeker.m_Transform.HasComponent(val2))
				{
					return;
				}
				float3 position = targetSeeker.m_Transform[val2].m_Position;
				Owner owner = default(Owner);
				if ((targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Road) != 0 && num > 0f && targetSeeker.m_Owner.TryGetComponent(val2, ref owner) && targetSeeker.m_Building.HasComponent(val2) && targetSeeker.m_Building.HasComponent(owner.m_Owner))
				{
					targetSeeker.FindTargets(entity, owner.m_Owner, 100f, edgeFlags, allowAccessRestriction: true, flag);
				}
				if ((targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Flying) != 0)
				{
					if ((targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Helicopter) != RoadTypes.None)
					{
						Entity lane = Entity.Null;
						float curvePos = 0f;
						float distance = float.MaxValue;
						targetSeeker.m_AirwayData.helicopterMap.FindClosestLane(position, targetSeeker.m_Curve, ref lane, ref curvePos, ref distance);
						if (lane != Entity.Null)
						{
							targetSeeker.m_Buffer.Enqueue(new PathTarget(entity, lane, curvePos, 0f));
						}
					}
					if ((targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Airplane) != RoadTypes.None)
					{
						Entity lane2 = Entity.Null;
						float curvePos2 = 0f;
						float distance2 = float.MaxValue;
						targetSeeker.m_AirwayData.airplaneMap.FindClosestLane(position, targetSeeker.m_Curve, ref lane2, ref curvePos2, ref distance2);
						if (lane2 != Entity.Null)
						{
							targetSeeker.m_Buffer.Enqueue(new PathTarget(entity, lane2, curvePos2, 0f));
						}
					}
				}
				if (num > 0f)
				{
					TargetIterator targetIterator = new TargetIterator
					{
						m_Entity = entity,
						m_Bounds = new Bounds3(position - num, position + num),
						m_Position = position,
						m_MaxDistance = num,
						m_TargetSeeker = targetSeeker,
						m_Flags = edgeFlags,
						m_CompositionData = m_CompositionData,
						m_NetCompositionData = m_NetCompositionData
					};
					m_NetSearchTree.Iterate<TargetIterator>(ref targetIterator, 0);
					Entity val3 = val2;
					Owner owner2 = default(Owner);
					while (targetSeeker.m_Owner.TryGetComponent(val3, ref owner2) && !targetSeeker.m_AreaNode.HasBuffer(val3))
					{
						val3 = owner2.m_Owner;
					}
					if (targetSeeker.m_AreaNode.HasBuffer(val3))
					{
						Random random = targetSeeker.m_RandomSeed.GetRandom(val3.Index);
						DynamicBuffer<Game.Areas.SubArea> subAreas = default(DynamicBuffer<Game.Areas.SubArea>);
						m_SubAreas.TryGetBuffer(val3, ref subAreas);
						targetSeeker.AddAreaTargets(ref random, entity, val3, val2, subAreas, 0f, addDistanceCost: true, EdgeFlags.DefaultMask);
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct SetupAccidentLocationJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public ComponentLookup<Creature> m_CreatureData;

		[ReadOnly]
		public ComponentLookup<Vehicle> m_VehicleData;

		[ReadOnly]
		public ComponentLookup<AccidentSite> m_AccidentSiteData;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		[ReadOnly]
		public BufferLookup<TargetElement> m_TargetElements;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(int index)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			m_SetupData.GetItem(index, out var entity, out var targetSeeker);
			if (!m_AccidentSiteData.HasComponent(entity))
			{
				return;
			}
			AccidentSite accidentSite = m_AccidentSiteData[entity];
			if (!m_TargetElements.HasBuffer(accidentSite.m_Event))
			{
				return;
			}
			DynamicBuffer<TargetElement> val = m_TargetElements[accidentSite.m_Event];
			EdgeFlags edgeFlags = EdgeFlags.DefaultMask;
			if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.SecondaryPath) != SetupTargetFlags.None)
			{
				edgeFlags |= EdgeFlags.Secondary;
			}
			bool allowAccessRestriction = true;
			CheckTarget(entity, accidentSite, edgeFlags, ref targetSeeker, ref allowAccessRestriction);
			for (int i = 0; i < val.Length; i++)
			{
				Entity entity2 = val[i].m_Entity;
				if (entity2 != entity)
				{
					CheckTarget(entity2, accidentSite, edgeFlags, ref targetSeeker, ref allowAccessRestriction);
				}
			}
		}

		private void CheckTarget(Entity target, AccidentSite accidentSite, EdgeFlags edgeFlags, ref PathfindTargetSeeker<PathfindSetupBuffer> targetSeeker, ref bool allowAccessRestriction)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0236: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_0185: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0288: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_029f: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			if ((accidentSite.m_Flags & AccidentSiteFlags.TrafficAccident) != 0 && !m_CreatureData.HasComponent(target) && !m_VehicleData.HasComponent(target))
			{
				return;
			}
			int num = targetSeeker.FindTargets(target, target, 0f, edgeFlags, allowAccessRestriction, navigationEnd: false);
			allowAccessRestriction &= num == 0;
			Entity val = target;
			if (targetSeeker.m_CurrentTransport.HasComponent(val))
			{
				val = targetSeeker.m_CurrentTransport[val].m_CurrentTransport;
			}
			else if (targetSeeker.m_CurrentBuilding.HasComponent(val))
			{
				val = targetSeeker.m_CurrentBuilding[val].m_CurrentBuilding;
			}
			if (!targetSeeker.m_Transform.HasComponent(val))
			{
				return;
			}
			float3 position = targetSeeker.m_Transform[val].m_Position;
			if ((targetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Flying) != 0)
			{
				if ((targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Helicopter) != RoadTypes.None)
				{
					Entity lane = Entity.Null;
					float curvePos = 0f;
					float distance = float.MaxValue;
					targetSeeker.m_AirwayData.helicopterMap.FindClosestLane(position, targetSeeker.m_Curve, ref lane, ref curvePos, ref distance);
					if (lane != Entity.Null)
					{
						targetSeeker.m_Buffer.Enqueue(new PathTarget(target, lane, curvePos, 0f));
					}
				}
				if ((targetSeeker.m_SetupQueueTarget.m_FlyingTypes & RoadTypes.Airplane) != RoadTypes.None)
				{
					Entity lane2 = Entity.Null;
					float curvePos2 = 0f;
					float distance2 = float.MaxValue;
					targetSeeker.m_AirwayData.airplaneMap.FindClosestLane(position, targetSeeker.m_Curve, ref lane2, ref curvePos2, ref distance2);
					if (lane2 != Entity.Null)
					{
						targetSeeker.m_Buffer.Enqueue(new PathTarget(target, lane2, curvePos2, 0f));
					}
				}
			}
			float value = targetSeeker.m_SetupQueueTarget.m_Value2;
			TargetIterator targetIterator = new TargetIterator
			{
				m_Entity = target,
				m_Bounds = new Bounds3(position - value, position + value),
				m_Position = position,
				m_MaxDistance = value,
				m_TargetSeeker = targetSeeker,
				m_Flags = edgeFlags,
				m_CompositionData = m_CompositionData,
				m_NetCompositionData = m_NetCompositionData
			};
			m_NetSearchTree.Iterate<TargetIterator>(ref targetIterator, 0);
			Entity val2 = val;
			Owner owner = default(Owner);
			while (targetSeeker.m_Owner.TryGetComponent(val2, ref owner) && !targetSeeker.m_AreaNode.HasBuffer(val2))
			{
				val2 = owner.m_Owner;
			}
			if (targetSeeker.m_AreaNode.HasBuffer(val2))
			{
				Random random = targetSeeker.m_RandomSeed.GetRandom(val2.Index);
				DynamicBuffer<Game.Areas.SubArea> subAreas = default(DynamicBuffer<Game.Areas.SubArea>);
				m_SubAreas.TryGetBuffer(val2, ref subAreas);
				targetSeeker.AddAreaTargets(ref random, target, val2, val, subAreas, 0f, addDistanceCost: true, EdgeFlags.DefaultMask);
			}
		}
	}

	[BurstCompile]
	private struct SetupSafetyJob : IJobParallelFor
	{
		[ReadOnly]
		public NativeQuadTree<Entity, QuadTreeBoundsXZ> m_NetSearchTree;

		[ReadOnly]
		public ComponentLookup<Composition> m_CompositionData;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(int index)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			m_SetupData.GetItem(index, out var entity, out var targetSeeker);
			Entity val = entity;
			if (targetSeeker.m_CurrentTransport.HasComponent(val))
			{
				val = targetSeeker.m_CurrentTransport[val].m_CurrentTransport;
			}
			else if (targetSeeker.m_CurrentBuilding.HasComponent(val))
			{
				val = targetSeeker.m_CurrentBuilding[val].m_CurrentBuilding;
			}
			if (!targetSeeker.m_Transform.HasComponent(val))
			{
				return;
			}
			float3 position = targetSeeker.m_Transform[val].m_Position;
			if (targetSeeker.m_Building.HasComponent(val))
			{
				Building building = targetSeeker.m_Building[val];
				if (targetSeeker.m_SubLane.HasBuffer(building.m_RoadEdge))
				{
					Random random = targetSeeker.m_RandomSeed.GetRandom(building.m_RoadEdge.Index);
					targetSeeker.AddEdgeTargets(ref random, entity, 0f, EdgeFlags.DefaultMask, building.m_RoadEdge, position, 0f, allowLaneGroupSwitch: true, allowAccessRestriction: true);
				}
			}
			float num = 100f;
			TargetIterator targetIterator = new TargetIterator
			{
				m_Entity = val,
				m_Bounds = new Bounds3(position - num, position + num),
				m_Position = position,
				m_MaxDistance = num,
				m_TargetSeeker = targetSeeker,
				m_Flags = EdgeFlags.DefaultMask,
				m_CompositionData = m_CompositionData,
				m_NetCompositionData = m_NetCompositionData
			};
			m_NetSearchTree.Iterate<TargetIterator>(ref targetIterator, 0);
		}
	}

	public struct TargetIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_Entity;

		public Bounds3 m_Bounds;

		public float3 m_Position;

		public float m_MaxDistance;

		public PathfindTargetSeeker<PathfindSetupBuffer> m_TargetSeeker;

		public EdgeFlags m_Flags;

		public ComponentLookup<Composition> m_CompositionData;

		public ComponentLookup<NetCompositionData> m_NetCompositionData;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return MathUtils.Intersect(bounds.m_Bounds, m_Bounds);
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity edgeEntity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			if (!MathUtils.Intersect(bounds.m_Bounds, m_Bounds) || !m_CompositionData.HasComponent(edgeEntity))
			{
				return;
			}
			Composition composition = m_CompositionData[edgeEntity];
			NetCompositionData netCompositionData = m_NetCompositionData[composition.m_Edge];
			bool flag = false;
			if ((m_TargetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Road) != 0)
			{
				flag |= (netCompositionData.m_State & (CompositionState.HasForwardRoadLanes | CompositionState.HasBackwardRoadLanes)) != 0;
			}
			if ((m_TargetSeeker.m_SetupQueueTarget.m_Methods & PathMethod.Pedestrian) != 0)
			{
				flag |= (netCompositionData.m_State & CompositionState.HasPedestrianLanes) != 0;
			}
			if (flag)
			{
				float num2 = default(float);
				float num = MathUtils.Distance(m_TargetSeeker.m_Curve[edgeEntity].m_Bezier, m_Position, ref num2) - netCompositionData.m_Width * 0.5f;
				if (num < m_MaxDistance)
				{
					Random random = m_TargetSeeker.m_RandomSeed.GetRandom(edgeEntity.Index);
					m_TargetSeeker.AddEdgeTargets(ref random, m_Entity, num, m_Flags, edgeEntity, m_Position, m_MaxDistance, allowLaneGroupSwitch: true, allowAccessRestriction: false);
				}
			}
		}
	}

	private Game.Net.SearchSystem m_NetSearchSystem;

	private ComponentLookup<PathOwner> m_PathOwnerData;

	private ComponentLookup<Vehicle> m_VehicleData;

	private ComponentLookup<Composition> m_CompositionData;

	private ComponentLookup<NetCompositionData> m_NetCompositionData;

	private ComponentLookup<Creature> m_CreatureData;

	private ComponentLookup<AccidentSite> m_AccidentSiteData;

	private BufferLookup<PathElement> m_PathElements;

	private BufferLookup<Game.Areas.SubArea> m_SubAreas;

	private BufferLookup<TargetElement> m_TargetElements;

	public CommonPathfindSetup(PathfindSetupSystem system)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		m_NetSearchSystem = ((ComponentSystemBase)system).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_PathOwnerData = ((SystemBase)system).GetComponentLookup<PathOwner>(true);
		m_VehicleData = ((SystemBase)system).GetComponentLookup<Vehicle>(true);
		m_CompositionData = ((SystemBase)system).GetComponentLookup<Composition>(true);
		m_NetCompositionData = ((SystemBase)system).GetComponentLookup<NetCompositionData>(true);
		m_CreatureData = ((SystemBase)system).GetComponentLookup<Creature>(true);
		m_AccidentSiteData = ((SystemBase)system).GetComponentLookup<AccidentSite>(true);
		m_PathElements = ((SystemBase)system).GetBufferLookup<PathElement>(true);
		m_SubAreas = ((SystemBase)system).GetBufferLookup<Game.Areas.SubArea>(true);
		m_TargetElements = ((SystemBase)system).GetBufferLookup<TargetElement>(true);
	}

	public JobHandle SetupCurrentLocation(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		m_PathOwnerData.Update((SystemBase)(object)system);
		m_VehicleData.Update((SystemBase)(object)system);
		m_CompositionData.Update((SystemBase)(object)system);
		m_NetCompositionData.Update((SystemBase)(object)system);
		m_PathElements.Update((SystemBase)(object)system);
		m_SubAreas.Update((SystemBase)(object)system);
		JobHandle dependencies;
		JobHandle val = IJobParallelForExtensions.Schedule<SetupCurrentLocationJob>(new SetupCurrentLocationJob
		{
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_PathOwnerData = m_PathOwnerData,
			m_VehicleData = m_VehicleData,
			m_CompositionData = m_CompositionData,
			m_NetCompositionData = m_NetCompositionData,
			m_PathElements = m_PathElements,
			m_SubAreas = m_SubAreas,
			m_SetupData = setupData
		}, setupData.Length, 1, JobHandle.CombineDependencies(inputDeps, dependencies));
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		return val;
	}

	public JobHandle SetupAccidentLocation(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		m_CreatureData.Update((SystemBase)(object)system);
		m_VehicleData.Update((SystemBase)(object)system);
		m_AccidentSiteData.Update((SystemBase)(object)system);
		m_CompositionData.Update((SystemBase)(object)system);
		m_NetCompositionData.Update((SystemBase)(object)system);
		m_TargetElements.Update((SystemBase)(object)system);
		m_SubAreas.Update((SystemBase)(object)system);
		JobHandle dependencies;
		JobHandle val = IJobParallelForExtensions.Schedule<SetupAccidentLocationJob>(new SetupAccidentLocationJob
		{
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_CreatureData = m_CreatureData,
			m_VehicleData = m_VehicleData,
			m_AccidentSiteData = m_AccidentSiteData,
			m_CompositionData = m_CompositionData,
			m_NetCompositionData = m_NetCompositionData,
			m_TargetElements = m_TargetElements,
			m_SubAreas = m_SubAreas,
			m_SetupData = setupData
		}, setupData.Length, 1, JobHandle.CombineDependencies(inputDeps, dependencies));
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		return val;
	}

	public JobHandle SetupSafety(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		m_CompositionData.Update((SystemBase)(object)system);
		m_NetCompositionData.Update((SystemBase)(object)system);
		JobHandle dependencies;
		JobHandle val = IJobParallelForExtensions.Schedule<SetupSafetyJob>(new SetupSafetyJob
		{
			m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies),
			m_CompositionData = m_CompositionData,
			m_NetCompositionData = m_NetCompositionData,
			m_SetupData = setupData
		}, setupData.Length, 1, JobHandle.CombineDependencies(inputDeps, dependencies));
		m_NetSearchSystem.AddNetSearchTreeReader(val);
		return val;
	}
}
