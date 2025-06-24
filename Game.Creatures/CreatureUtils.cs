using System;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Creatures;

public static class CreatureUtils
{
	private struct ActivityLocationIterator : INativeQuadTreeIterator<Entity, QuadTreeBoundsXZ>, IUnsafeQuadTreeIterator<Entity, QuadTreeBoundsXZ>
	{
		public Entity m_Ignore;

		public Segment m_Line;

		public ComponentLookup<Transform> m_TransformData;

		public bool m_Found;

		public bool Intersect(QuadTreeBoundsXZ bounds)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			if (!m_Found)
			{
				return MathUtils.Intersect(MathUtils.Expand(bounds.m_Bounds, float3.op_Implicit(0.5f)), m_Line, ref val);
			}
			return false;
		}

		public void Iterate(QuadTreeBoundsXZ bounds, Entity entity)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			float2 val = default(float2);
			if (!m_Found && MathUtils.Intersect(MathUtils.Expand(bounds.m_Bounds, float3.op_Implicit(0.5f)), m_Line, ref val) && !(entity == m_Ignore) && m_TransformData.HasComponent(entity))
			{
				Transform transform = m_TransformData[entity];
				float num = default(float);
				m_Found |= MathUtils.Distance(m_Line, transform.m_Position, ref num) < 0.5f;
			}
		}
	}

	public const float MAX_HUMAN_WALK_SPEED = 5.555556f;

	public const float AVG_HUMAN_WALK_SPEED = 1.6666667f;

	public const float MIN_MOVE_SPEED = 0.1f;

	public const float RESIDENT_PATHFIND_RANDOM_COST = 30f;

	public const int MAX_TRANSPORT_WAIT_TICKS = 5000;

	public const int MAX_ENTER_VEHICLE_TICKS = 250;

	public const float QUEUE_TICKS_TO_SECONDS = 2f / 15f;

	public static bool PathfindFailed(PathOwner pathOwner)
	{
		return (pathOwner.m_State & (PathFlags.Failed | PathFlags.Stuck)) != 0;
	}

	public static bool EndReached(HumanCurrentLane currentLane)
	{
		return (currentLane.m_Flags & CreatureLaneFlags.EndReached) != 0;
	}

	public static bool PathEndReached(HumanCurrentLane currentLane)
	{
		return (currentLane.m_Flags & (CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached)) == (CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached);
	}

	public static bool PathEndReached(AnimalCurrentLane currentLane)
	{
		return (currentLane.m_Flags & (CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached)) == (CreatureLaneFlags.EndOfPath | CreatureLaneFlags.EndReached);
	}

	public static bool ParkingSpaceReached(HumanCurrentLane currentLane)
	{
		return (currentLane.m_Flags & (CreatureLaneFlags.EndReached | CreatureLaneFlags.ParkingSpace)) == (CreatureLaneFlags.EndReached | CreatureLaneFlags.ParkingSpace);
	}

	public static bool ActionLocationReached(HumanCurrentLane currentLane)
	{
		return (currentLane.m_Flags & (CreatureLaneFlags.EndReached | CreatureLaneFlags.Action)) == (CreatureLaneFlags.EndReached | CreatureLaneFlags.Action);
	}

	public static bool TransportStopReached(HumanCurrentLane currentLane)
	{
		return (currentLane.m_Flags & (CreatureLaneFlags.EndReached | CreatureLaneFlags.Transport)) == (CreatureLaneFlags.EndReached | CreatureLaneFlags.Transport);
	}

	public static bool RequireNewPath(PathOwner pathOwner)
	{
		if ((pathOwner.m_State & (PathFlags.Obsolete | PathFlags.DivertObsolete)) != 0)
		{
			return (pathOwner.m_State & (PathFlags.Pending | PathFlags.Failed | PathFlags.Stuck)) == 0;
		}
		return false;
	}

	public static bool IsStuck(PathOwner pathOwner)
	{
		return (pathOwner.m_State & PathFlags.Stuck) != 0;
	}

	public static bool IsStuck(AnimalCurrentLane currentLane)
	{
		return (currentLane.m_Flags & CreatureLaneFlags.Stuck) != 0;
	}

	public static bool ResetUncheckedLane(ref HumanCurrentLane currentLane)
	{
		bool result = (currentLane.m_Flags & CreatureLaneFlags.Checked) == 0;
		currentLane.m_Flags |= CreatureLaneFlags.Checked;
		return result;
	}

	public static void SetupPathfind(ref HumanCurrentLane currentLane, ref PathOwner pathOwner, ParallelWriter<SetupQueueItem> queue, SetupQueueItem item)
	{
		if ((pathOwner.m_State & (PathFlags.Obsolete | PathFlags.Divert)) == (PathFlags.Obsolete | PathFlags.Divert))
		{
			pathOwner.m_State |= PathFlags.CachedObsolete;
		}
		pathOwner.m_State &= ~(PathFlags.Failed | PathFlags.Obsolete | PathFlags.DivertObsolete);
		pathOwner.m_State |= PathFlags.Pending;
		currentLane.m_Flags &= ~(CreatureLaneFlags.EndOfPath | CreatureLaneFlags.ParkingSpace | CreatureLaneFlags.Transport | CreatureLaneFlags.Taxi | CreatureLaneFlags.Action);
		queue.Enqueue(item);
	}

	public static bool DivertDestination(ref SetupQueueTarget destination, ref PathOwner pathOwner, Divert divert)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (divert.m_Purpose == Purpose.None)
		{
			return true;
		}
		if (divert.m_Target != Entity.Null)
		{
			destination.m_Entity = divert.m_Target;
			pathOwner.m_State |= PathFlags.Divert;
			return true;
		}
		switch (divert.m_Purpose)
		{
		case Purpose.SendMail:
			destination.m_Type = SetupTargetType.MailBox;
			pathOwner.m_State |= PathFlags.AddDestination | PathFlags.Divert;
			return true;
		case Purpose.Safety:
		case Purpose.Escape:
			destination.m_Type = SetupTargetType.Safety;
			pathOwner.m_State |= PathFlags.Divert;
			return true;
		case Purpose.Disappear:
			destination.m_Type = SetupTargetType.OutsideConnection;
			pathOwner.m_State |= PathFlags.AddDestination | PathFlags.Divert;
			return true;
		case Purpose.WaitingHome:
		case Purpose.PathFailed:
			return false;
		default:
			return true;
		}
	}

	public static bool ResetUpdatedPath(ref PathOwner pathOwner)
	{
		bool result = (pathOwner.m_State & PathFlags.Updated) != 0;
		pathOwner.m_State &= ~PathFlags.Updated;
		return result;
	}

	public static Transform GetVehicleDoorPosition(ref Random random, ActivityType activityType, ActivityCondition conditions, Transform vehicleTransform, float3 targetPosition, bool isDriver, bool lefthandTraffic, Entity creaturePrefab, Entity vehicle, DynamicBuffer<MeshGroup> meshGroups, ref ComponentLookup<Game.Vehicles.PublicTransport> publicTransports, ref ComponentLookup<Train> trains, ref ComponentLookup<Controller> controllers, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<CarData> prefabCarDatas, ref BufferLookup<ActivityLocationElement> prefabActivityLocations, ref BufferLookup<SubMeshGroup> subMeshGroupBuffers, ref BufferLookup<CharacterElement> characterElementBuffers, ref BufferLookup<SubMesh> subMeshBuffers, ref BufferLookup<AnimationClip> animationClipBuffers, ref BufferLookup<AnimationMotion> animationMotionBuffers, out ActivityMask activityMask, out AnimatedPropID propID)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		PrefabRef prefabRef = prefabRefs[vehicle];
		Transform result = vehicleTransform;
		activityMask = default(ActivityMask);
		propID = new AnimatedPropID(-1);
		DynamicBuffer<ActivityLocationElement> val = default(DynamicBuffer<ActivityLocationElement>);
		if (prefabActivityLocations.TryGetBuffer(prefabRef.m_Prefab, ref val))
		{
			ActivityMask activityMask2 = new ActivityMask(ActivityType.Enter);
			ActivityMask activityMask3 = new ActivityMask(ActivityType.Driving);
			activityMask2.m_Mask |= new ActivityMask(ActivityType.Exit).m_Mask;
			int num = 0;
			int num2 = -1;
			bool a = true;
			bool b = true;
			Game.Vehicles.PublicTransport publicTransport = default(Game.Vehicles.PublicTransport);
			if (publicTransports.TryGetComponent(vehicle, ref publicTransport))
			{
				bool flag = false;
				Controller controller = default(Controller);
				Game.Vehicles.PublicTransport publicTransport2 = default(Game.Vehicles.PublicTransport);
				if (controllers.TryGetComponent(vehicle, ref controller) && publicTransports.TryGetComponent(controller.m_Controller, ref publicTransport2))
				{
					publicTransport = publicTransport2;
					Train train = default(Train);
					Train train2 = default(Train);
					if (trains.TryGetComponent(vehicle, ref train) && trains.TryGetComponent(controller.m_Controller, ref train2))
					{
						flag = ((train.m_Flags ^ train2.m_Flags) & Game.Vehicles.TrainFlags.Reversed) != 0;
					}
				}
				a = (publicTransport.m_State & PublicTransportFlags.StopRight) == 0;
				b = (publicTransport.m_State & PublicTransportFlags.StopLeft) == 0;
				if (flag)
				{
					CommonUtils.Swap(ref a, ref b);
				}
			}
			else if (prefabCarDatas.HasComponent(prefabRef.m_Prefab))
			{
				float num3 = float.MinValue;
				for (int i = 0; i < val.Length; i++)
				{
					ActivityLocationElement activityLocationElement = val[i];
					if ((activityLocationElement.m_ActivityMask.m_Mask & activityMask2.m_Mask) == activityMask2.m_Mask)
					{
						bool flag2 = ((activityLocationElement.m_ActivityFlags & ActivityFlags.InvertLefthandTraffic) != 0 && lefthandTraffic) || ((activityLocationElement.m_ActivityFlags & ActivityFlags.InvertRighthandTraffic) != 0 && !lefthandTraffic);
						activityLocationElement.m_Position.x = math.select(activityLocationElement.m_Position.x, 0f - activityLocationElement.m_Position.x, flag2);
						if ((!(math.abs(activityLocationElement.m_Position.x) >= 0.5f) || activityLocationElement.m_Position.x >= 0f == lefthandTraffic) && activityLocationElement.m_Position.z > num3)
						{
							num2 = i;
							num3 = activityLocationElement.m_Position.z;
						}
					}
				}
			}
			isDriver = isDriver && num2 != -1;
			ObjectUtils.ActivityStartPositionCache cache = default(ObjectUtils.ActivityStartPositionCache);
			for (int j = 0; j < val.Length; j++)
			{
				ActivityLocationElement activityLocationElement2 = val[j];
				ActivityMask activityMask4 = new ActivityMask(activityType);
				activityMask4.m_Mask &= activityLocationElement2.m_ActivityMask.m_Mask;
				if (activityMask4.m_Mask == 0 || isDriver != (j == num2))
				{
					continue;
				}
				bool flag3 = ((activityLocationElement2.m_ActivityFlags & ActivityFlags.InvertLefthandTraffic) != 0 && lefthandTraffic) || ((activityLocationElement2.m_ActivityFlags & ActivityFlags.InvertRighthandTraffic) != 0 && !lefthandTraffic);
				activityLocationElement2.m_Position.x = math.select(activityLocationElement2.m_Position.x, 0f - activityLocationElement2.m_Position.x, flag3);
				if (!(math.abs(activityLocationElement2.m_Position.x) >= 0.5f) || ((activityLocationElement2.m_Position.x >= 0f) ? b : a))
				{
					if (activityType == ActivityType.Exit && (activityLocationElement2.m_ActivityMask.m_Mask & activityMask2.m_Mask) == activityMask2.m_Mask && (activityLocationElement2.m_ActivityMask.m_Mask & activityMask3.m_Mask) == 0)
					{
						activityLocationElement2.m_Rotation = math.mul(quaternion.RotateY((float)Math.PI), activityLocationElement2.m_Rotation);
					}
					Transform transform = ObjectUtils.LocalToWorld(vehicleTransform, activityLocationElement2.m_Position, activityLocationElement2.m_Rotation);
					if (activityType == ActivityType.Enter && (activityLocationElement2.m_ActivityMask.m_Mask & activityMask3.m_Mask) != 0)
					{
						transform = ObjectUtils.GetActivityStartPosition(creaturePrefab, meshGroups, transform, TransformState.Action, activityType, activityLocationElement2.m_PropID, conditions, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, ref animationMotionBuffers, ref cache);
					}
					if (math.distancesq(transform.m_Position, targetPosition) < 0.01f)
					{
						activityMask = activityLocationElement2.m_ActivityMask;
						propID = activityLocationElement2.m_PropID;
						return transform;
					}
					if (((Random)(ref random)).NextInt(++num) == 0)
					{
						result = transform;
						activityMask = activityLocationElement2.m_ActivityMask;
						propID = activityLocationElement2.m_PropID;
					}
				}
			}
		}
		return result;
	}

	public static ActivityCondition GetConditions(Human human)
	{
		ActivityCondition activityCondition = (ActivityCondition)0u;
		if ((human.m_Flags & HumanFlags.Homeless) != 0)
		{
			activityCondition |= ActivityCondition.Homeless;
		}
		if ((human.m_Flags & HumanFlags.Angry) != 0)
		{
			activityCondition |= ActivityCondition.Angry;
		}
		else if ((human.m_Flags & HumanFlags.Waiting) != 0)
		{
			activityCondition |= ActivityCondition.Waiting;
		}
		else if ((human.m_Flags & HumanFlags.Sad) != 0)
		{
			activityCondition |= ActivityCondition.Sad;
		}
		else if ((human.m_Flags & HumanFlags.Happy) != 0)
		{
			activityCondition |= ActivityCondition.Happy;
		}
		return activityCondition;
	}

	public static bool CalculateTransformPosition(Entity creature, Entity creaturePrefab, DynamicBuffer<MeshGroup> meshGroups, ref Random random, ref Transform result, ref ActivityType activity, CurrentVehicle currentVehicle, Entity entity, bool leftHandTraffic, ActivityMask activityMask, ActivityCondition conditions, NativeQuadTree<Entity, QuadTreeBoundsXZ> movingObjectSearchTree, ref ComponentLookup<Transform> transforms, ref ComponentLookup<Position> positions, ref ComponentLookup<Game.Vehicles.PublicTransport> publicTransports, ref ComponentLookup<Train> trains, ref ComponentLookup<Controller> controllers, ref ComponentLookup<PrefabRef> prefabRefs, ref ComponentLookup<BuildingData> prefabBuildingDatas, ref ComponentLookup<CarData> prefabCarDatas, ref BufferLookup<ActivityLocationElement> prefabActivityLocations, ref BufferLookup<SubMeshGroup> subMeshGroupBuffers, ref BufferLookup<CharacterElement> characterElementBuffers, ref BufferLookup<SubMesh> subMeshBuffers, ref BufferLookup<AnimationClip> animationClipBuffers, ref BufferLookup<AnimationMotion> animationMotionBuffers)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		if (transforms.HasComponent(entity))
		{
			Transform transform = transforms[entity];
			PrefabRef prefabRef = prefabRefs[entity];
			if (entity == currentVehicle.m_Vehicle)
			{
				float3 position = result.m_Position;
				bool isDriver = (currentVehicle.m_Flags & CreatureVehicleFlags.Driver) != 0;
				result = GetVehicleDoorPosition(ref random, ActivityType.Enter, conditions, transform, position, isDriver, leftHandTraffic, creaturePrefab, entity, meshGroups, ref publicTransports, ref trains, ref controllers, ref prefabRefs, ref prefabCarDatas, ref prefabActivityLocations, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, ref animationMotionBuffers, out var activityMask2, out var _);
				if ((activityMask2.m_Mask & new ActivityMask(ActivityType.Driving).m_Mask) != 0)
				{
					activity = ActivityType.Enter;
				}
				return true;
			}
			if (prefabBuildingDatas.HasComponent(prefabRef.m_Prefab))
			{
				BuildingData buildingData = prefabBuildingDatas[prefabRef.m_Prefab];
				result.m_Position = BuildingUtils.CalculateFrontPosition(transform, buildingData.m_LotSize.y);
				return true;
			}
			if (prefabActivityLocations.HasBuffer(prefabRef.m_Prefab))
			{
				DynamicBuffer<ActivityLocationElement> val = prefabActivityLocations[prefabRef.m_Prefab];
				float num = float.MaxValue;
				float3 position2 = result.m_Position;
				ActivityLocationIterator activityLocationIterator = new ActivityLocationIterator
				{
					m_Ignore = creature,
					m_TransformData = transforms
				};
				ObjectUtils.ActivityStartPositionCache cache = default(ObjectUtils.ActivityStartPositionCache);
				for (int i = 0; i < val.Length; i++)
				{
					ActivityLocationElement activityLocationElement = val[i];
					ActivityMask activityMask3 = activityMask;
					activityMask3.m_Mask &= activityLocationElement.m_ActivityMask.m_Mask;
					if (activityMask3.m_Mask == 0)
					{
						continue;
					}
					Transform activityTransform = ObjectUtils.LocalToWorld(transform, activityLocationElement.m_Position, activityLocationElement.m_Rotation);
					float3 val2 = math.forward(activityTransform.m_Rotation);
					activityLocationIterator.m_Line = new Segment(activityTransform.m_Position, activityTransform.m_Position + val2);
					activityLocationIterator.m_Found = false;
					movingObjectSearchTree.Iterate<ActivityLocationIterator>(ref activityLocationIterator, 0);
					if (activityLocationIterator.m_Found)
					{
						continue;
					}
					int num2 = ((Random)(ref random)).NextInt(math.countbits(activityMask3.m_Mask));
					for (int j = 1; j <= 64; j++)
					{
						ActivityType activityType = (ActivityType)j;
						if ((activityMask3.m_Mask & new ActivityMask(activityType).m_Mask) != 0 && num2-- == 0)
						{
							activity = activityType;
							break;
						}
					}
					activityTransform = ObjectUtils.GetActivityStartPosition(creaturePrefab, meshGroups, activityTransform, TransformState.Start, activity, activityLocationElement.m_PropID, conditions, ref subMeshGroupBuffers, ref characterElementBuffers, ref subMeshBuffers, ref animationClipBuffers, ref animationMotionBuffers, ref cache);
					float num3 = math.distance(activityTransform.m_Position, position2);
					num3 *= ((Random)(ref random)).NextFloat(0.5f, 1.5f);
					if (!(num3 >= num))
					{
						num = num3;
						result = activityTransform;
					}
				}
				return num != float.MaxValue;
			}
			result.m_Position = transform.m_Position;
			return true;
		}
		if (positions.HasComponent(entity))
		{
			result.m_Position = positions[entity].m_Position;
			return true;
		}
		return false;
	}

	public static void GetAreaActivity(ref Random random, ref ActivityType activity, Entity laneEntity, ActivityMask activityMask, ComponentLookup<Owner> owners, ComponentLookup<PrefabRef> prefabRefs, ComponentLookup<SpawnLocationData> prefabSpawnLocationDatas)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!owners.HasComponent(laneEntity))
		{
			return;
		}
		Entity owner = owners[laneEntity].m_Owner;
		PrefabRef prefabRef = prefabRefs[owner];
		if (!prefabSpawnLocationDatas.HasComponent(prefabRef.m_Prefab))
		{
			return;
		}
		SpawnLocationData spawnLocationData = prefabSpawnLocationDatas[prefabRef.m_Prefab];
		activityMask.m_Mask &= spawnLocationData.m_ActivityMask.m_Mask;
		if (activityMask.m_Mask == 0)
		{
			return;
		}
		int num = ((Random)(ref random)).NextInt(math.countbits(activityMask.m_Mask));
		for (int i = 1; i <= 64; i++)
		{
			ActivityType activityType = (ActivityType)i;
			if ((activityMask.m_Mask & new ActivityMask(activityType).m_Mask) != 0 && num-- == 0)
			{
				activity = activityType;
				break;
			}
		}
	}

	public static bool SetTriangleTarget(float3 left, float3 right, float3 next, float3 comparePosition, PathElement nextElement, int elementIndex, DynamicBuffer<PathElement> pathElements, ref float3 targetPosition, float minDistance, float lanePosition, float curveDelta, float navigationSize, bool isSingle, ComponentLookup<Transform> transforms, ComponentLookup<TaxiStand> taxiStands, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		targetPosition = CalculateTriangleTarget(left, right, next, targetPosition, nextElement, elementIndex, pathElements, lanePosition, curveDelta, navigationSize, isSingle, transforms, taxiStands, areaLanes, curves);
		return math.distance(comparePosition, targetPosition) >= minDistance;
	}

	private static float3 CalculateTriangleTarget(float3 left, float3 right, float3 next, float3 lastTarget, PathElement nextElement, int elementIndex, DynamicBuffer<PathElement> pathElements, float lanePosition, float curveDelta, float navigationSize, bool isSingle, ComponentLookup<Transform> transforms, ComponentLookup<TaxiStand> taxiStands, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (nextElement.m_Target == Entity.Null && pathElements.IsCreated && elementIndex < pathElements.Length)
		{
			nextElement = pathElements[elementIndex];
		}
		if (nextElement.m_Target != Entity.Null)
		{
			Transform transform = default(Transform);
			if (transforms.TryGetComponent(nextElement.m_Target, ref transform) && !taxiStands.HasComponent(nextElement.m_Target))
			{
				return CalculateTriangleTarget(left, right, next, transform.m_Position, navigationSize, isSingle);
			}
			if (areaLanes.HasComponent(nextElement.m_Target))
			{
				return CalculateTriangleTarget(left, right, next, lastTarget, navigationSize, isSingle);
			}
			Curve curve = default(Curve);
			if (curves.TryGetComponent(nextElement.m_Target, ref curve))
			{
				float3 target = MathUtils.Position(curve.m_Bezier, nextElement.m_TargetDelta.x);
				return CalculateTriangleTarget(left, right, next, target, navigationSize, isSingle);
			}
		}
		return CalculateTriangleTarget(left, right, next, lanePosition, curveDelta, navigationSize, isSingle);
	}

	private static float3 CalculateTriangleTarget(float3 left, float3 right, float3 next, float3 target, float navigationSize, bool isSingle)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Triangle3 val = default(Triangle3);
		((Triangle3)(ref val))._002Ector(next, left, right);
		if (isSingle)
		{
			float num2 = default(float);
			float3 val2 = MathUtils.Incenter(val, ref num2);
			float num3 = default(float);
			MathUtils.Incenter(((Triangle3)(ref val)).xz, ref num3);
			float num4 = math.saturate(num / num3);
			ref float3 a = ref val.a;
			a += (val2 - val.a) * num4;
			ref float3 b = ref val.b;
			b += (val2 - val.b) * num4;
			ref float3 c = ref val.c;
			c += (val2 - val.c) * num4;
			float2 val3 = default(float2);
			if (MathUtils.Distance(((Triangle3)(ref val)).xz, ((float3)(ref target)).xz, ref val3) != 0f)
			{
				target = MathUtils.Position(val, val3);
			}
		}
		else
		{
			Segment val4 = ((Triangle3)(ref val)).ba;
			float2 val6 = default(float2);
			float2 val5 = default(float2);
			val5.x = MathUtils.Distance(((Segment)(ref val4)).xz, ((float3)(ref target)).xz, ref val6.x);
			val4 = ((Triangle3)(ref val)).ca;
			val5.y = MathUtils.Distance(((Segment)(ref val4)).xz, ((float3)(ref target)).xz, ref val6.y);
			val5 = ((!MathUtils.Intersect(((Triangle3)(ref val)).xz, ((float3)(ref target)).xz)) ? math.select(new float2(val5.x, 0f - val5.y), new float2(0f - val5.x, val5.y), val5.x > val5.y) : (-val5));
			if (math.any(val5 > 0f - num))
			{
				if (val5.y <= 0f - num)
				{
					float2 val7 = math.normalizesafe(MathUtils.Right(((float3)(ref left)).xz - ((float3)(ref next)).xz), default(float2)) * num;
					target = MathUtils.Position(((Triangle3)(ref val)).ba, val6.x);
					((float3)(ref target)).xz = ((float3)(ref target)).xz + math.select(val7, -val7, math.dot(val7, ((float3)(ref right)).xz - ((float3)(ref next)).xz) < 0f);
				}
				else if (val5.x <= 0f - num)
				{
					float2 val8 = math.normalizesafe(MathUtils.Left(((float3)(ref right)).xz - ((float3)(ref next)).xz), default(float2)) * num;
					target = MathUtils.Position(((Triangle3)(ref val)).ca, val6.y);
					((float3)(ref target)).xz = ((float3)(ref target)).xz + math.select(val8, -val8, math.dot(val8, ((float3)(ref left)).xz - ((float3)(ref next)).xz) < 0f);
				}
				else
				{
					target = math.lerp(MathUtils.Position(((Triangle3)(ref val)).ba, val6.x), MathUtils.Position(((Triangle3)(ref val)).ca, val6.y), 0.5f);
				}
			}
		}
		return target;
	}

	private static float3 CalculateTriangleTarget(float3 left, float3 right, float3 next, float lanePosition, float curveDelta, float navigationSize, bool isSingle)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Segment val = default(Segment);
		((Segment)(ref val))._002Ector(left, right);
		float num2 = lanePosition * math.saturate(1f - navigationSize / MathUtils.Length(((Segment)(ref val)).xz));
		val.a = MathUtils.Position(val, num2 + 0.5f);
		val.b = next;
		float num3;
		if (isSingle)
		{
			num3 = (math.sqrt(math.saturate(1f - curveDelta)) - 0.5f) * math.saturate(1f - navigationSize / MathUtils.Length(((Segment)(ref val)).xz)) + 0.5f;
		}
		else
		{
			float num4 = curveDelta * 2f;
			num4 = math.select(1f - num4, num4 - 1f, curveDelta > 0.5f);
			num3 = math.sqrt(math.saturate(1f - num4)) * math.saturate(1f - num / MathUtils.Length(((Segment)(ref val)).xz));
		}
		return MathUtils.Position(val, num3);
	}

	public static bool SetAreaTarget(float3 prev2, float3 prev, float3 left, float3 right, float3 next, Entity areaEntity, DynamicBuffer<Game.Areas.Node> nodes, float3 comparePosition, PathElement nextElement, int elementIndex, DynamicBuffer<PathElement> pathElements, ref float3 targetPosition, float minDistance, float lanePosition, float curveDelta, float navigationSize, bool isBackward, ComponentLookup<Transform> transforms, ComponentLookup<TaxiStand> taxiStands, ComponentLookup<AreaLane> areaLanes, ComponentLookup<Curve> curves, ComponentLookup<Owner> owners)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0413: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Segment val = default(Segment);
		((Segment)(ref val))._002Ector(left, right);
		float num2 = 1f / MathUtils.Length(((Segment)(ref val)).xz);
		Bounds1 val2 = default(Bounds1);
		((Bounds1)(ref val2))._002Ector(math.min(0.5f, num * num2), math.max(0.5f, 1f - num * num2));
		int num3 = 0;
		int num4 = elementIndex;
		if (pathElements.IsCreated)
		{
			num3 = pathElements.Length;
			elementIndex = math.min(elementIndex, num3);
		}
		elementIndex -= math.select(0, 1, nextElement.m_Target != Entity.Null);
		int num5 = elementIndex;
		Owner owner = default(Owner);
		bool4 val3 = default(bool4);
		Segment val4 = default(Segment);
		Segment val5 = default(Segment);
		float2 val8 = default(float2);
		while (elementIndex < num3)
		{
			PathElement pathElement = ((elementIndex >= num4) ? pathElements[elementIndex] : nextElement);
			if (!owners.TryGetComponent(pathElement.m_Target, ref owner) || !(owner.m_Owner == areaEntity))
			{
				break;
			}
			AreaLane areaLane = areaLanes[pathElement.m_Target];
			((bool4)(ref val3))._002Ector(pathElement.m_TargetDelta < 0.5f, pathElement.m_TargetDelta > 0.5f);
			if (math.any(((bool4)(ref val3)).xy & ((bool4)(ref val3)).wz))
			{
				((Segment)(ref val4))._002Ector(comparePosition, nodes[areaLane.m_Nodes.y].m_Position);
				((Segment)(ref val5))._002Ector(comparePosition, nodes[areaLane.m_Nodes.z].m_Position);
				Bounds1 val6 = val2;
				Bounds1 val7 = val2;
				if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val4)).xz), ref val8))
				{
					float num6 = math.max(math.max(0f, 0.4f * math.min(val8.y, 1f - val8.y) * MathUtils.Length(((Segment)(ref val4)).xz) * num2), math.max(val8.x - val2.max, val2.min - val8.x));
					if (num6 < val2.max - val2.min)
					{
						((Bounds1)(ref val6))._002Ector(math.max(val2.min, math.min(val2.max, val8.x) - num6), math.min(val2.max, math.max(val2.min, val8.x) + num6));
					}
				}
				if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val5)).xz), ref val8))
				{
					float num7 = math.max(math.max(0f, 0.4f * math.min(val8.y, 1f - val8.y) * MathUtils.Length(((Segment)(ref val4)).xz) * num2), math.max(val8.x - val2.max, val2.min - val8.x));
					if (num7 < val2.max - val2.min)
					{
						((Bounds1)(ref val7))._002Ector(math.max(val2.min, math.min(val2.max, val8.x) - num7), math.min(val2.max, math.max(val2.min, val8.x) + num7));
					}
				}
				if (!(((Bounds1)(ref val6)).Equals(val2) & ((Bounds1)(ref val7)).Equals(val2)))
				{
					val2 = val6 | val7;
					elementIndex++;
					continue;
				}
				elementIndex = num3;
			}
			elementIndex++;
			break;
		}
		if (elementIndex - 1 < num3)
		{
			float3 val9;
			if (elementIndex > num5)
			{
				PathElement pathElement2 = ((elementIndex - 1 >= num4) ? pathElements[elementIndex - 1] : nextElement);
				AreaLane areaLane2 = areaLanes[pathElement2.m_Target];
				bool flag = pathElement2.m_TargetDelta.y > 0.5f;
				val9 = CalculateTriangleTarget(nodes[areaLane2.m_Nodes.y].m_Position, nodes[areaLane2.m_Nodes.z].m_Position, nodes[math.select(areaLane2.m_Nodes.x, areaLane2.m_Nodes.w, flag)].m_Position, lanePosition: math.select(lanePosition, 0f - lanePosition, pathElement2.m_TargetDelta.y < pathElement2.m_TargetDelta.x != isBackward), lastTarget: targetPosition, nextElement: default(PathElement), elementIndex: elementIndex, pathElements: pathElements, curveDelta: pathElement2.m_TargetDelta.y, navigationSize: navigationSize, isSingle: false, transforms: transforms, taxiStands: taxiStands, areaLanes: areaLanes, curves: curves);
			}
			else
			{
				val9 = CalculateTriangleTarget(left, right, next, targetPosition, nextElement, elementIndex, pathElements, lanePosition, curveDelta, navigationSize, isSingle: false, transforms, taxiStands, areaLanes, curves);
			}
			Segment val10 = default(Segment);
			((Segment)(ref val10))._002Ector(comparePosition, val9);
			float2 val11 = default(float2);
			if (MathUtils.Intersect(Line2.op_Implicit(((Segment)(ref val)).xz), Line2.op_Implicit(((Segment)(ref val10)).xz), ref val11))
			{
				float num8 = math.max(math.max(0f, 0.4f * math.min(val11.y, 1f - val11.y) * MathUtils.Length(((Segment)(ref val10)).xz) * num2), math.max(val11.x - val2.max, val2.min - val11.x));
				if (num8 < val2.max - val2.min)
				{
					((Bounds1)(ref val2))._002Ector(math.max(val2.min, math.min(val2.max, val11.x) - num8), math.min(val2.max, math.max(val2.min, val11.x) + num8));
				}
			}
		}
		float lanePosition2 = math.lerp(val2.min, val2.max, lanePosition + 0.5f);
		targetPosition = CalculateAreaTarget(prev2, prev, left, right, comparePosition, minDistance, lanePosition2, navigationSize, out var farEnough);
		if (!farEnough)
		{
			return math.distance(comparePosition, targetPosition) >= minDistance;
		}
		return true;
	}

	private static float3 CalculateAreaTarget(float3 prev2, float3 prev, float3 left, float3 right, float3 comparePosition, float minDistance, float lanePosition, float navigationSize, out bool farEnough)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		float num = navigationSize * 0.5f;
		Segment val = default(Segment);
		((Segment)(ref val))._002Ector(left, right);
		val.a = MathUtils.Position(val, lanePosition);
		if (!((float3)(ref prev2)).Equals(prev))
		{
			Segment val2 = default(Segment);
			((Segment)(ref val2))._002Ector(prev2, prev);
			val.b = comparePosition;
			float2 val3 = default(float2);
			if (MathUtils.Intersect(((Segment)(ref val)).xz, ((Segment)(ref val2)).xz, ref val3) && math.min(val3.y, 1f - val3.y) >= num / MathUtils.Length(((Segment)(ref val2)).xz))
			{
				farEnough = false;
				return val.a;
			}
		}
		Triangle3 val4 = default(Triangle3);
		((Triangle3)(ref val4))._002Ector(prev, left, right);
		Segment val5 = ((Triangle3)(ref val4)).ba;
		float2 val7 = default(float2);
		float2 val6 = default(float2);
		val6.x = MathUtils.Distance(((Segment)(ref val5)).xz, ((float3)(ref comparePosition)).xz, ref val7.x);
		val5 = ((Triangle3)(ref val4)).ca;
		val6.y = MathUtils.Distance(((Segment)(ref val5)).xz, ((float3)(ref comparePosition)).xz, ref val7.y);
		val6 = ((!MathUtils.Intersect(((Triangle3)(ref val4)).xz, ((float3)(ref comparePosition)).xz)) ? math.select(new float2(val6.x, 0f - val6.y), new float2(0f - val6.x, val6.y), val6.x > val6.y) : (-val6));
		if (math.all(val6 <= 0f - num))
		{
			farEnough = false;
			return val.a;
		}
		if (val6.y <= 0f - num)
		{
			float2 val8 = math.normalizesafe(MathUtils.Right(((float3)(ref left)).xz - ((float3)(ref prev)).xz), default(float2)) * num;
			val.b = MathUtils.Position(((Triangle3)(ref val4)).ba, val7.x);
			ref float3 b = ref val.b;
			((float3)(ref b)).xz = ((float3)(ref b)).xz + math.select(val8, -val8, math.dot(val8, ((float3)(ref right)).xz - ((float3)(ref prev)).xz) < 0f);
		}
		else if (val6.x <= 0f - num)
		{
			float2 val9 = math.normalizesafe(MathUtils.Left(((float3)(ref right)).xz - ((float3)(ref prev)).xz), default(float2)) * num;
			val.b = MathUtils.Position(((Triangle3)(ref val4)).ca, val7.y);
			ref float3 b2 = ref val.b;
			((float3)(ref b2)).xz = ((float3)(ref b2)).xz + math.select(val9, -val9, math.dot(val9, ((float3)(ref left)).xz - ((float3)(ref prev)).xz) < 0f);
		}
		else
		{
			val.b = prev;
		}
		float num3 = default(float);
		float num2 = MathUtils.Distance(val, comparePosition, ref num3);
		num3 -= math.sqrt(math.max(0f, minDistance * minDistance - num2 * num2) / MathUtils.LengthSquared(val));
		if (num3 >= 0f)
		{
			farEnough = true;
			return MathUtils.Position(val, num3);
		}
		farEnough = false;
		return val.a;
	}

	public static float GetNavigationSize(ObjectGeometryData prefabObjectGeometryData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return prefabObjectGeometryData.m_Bounds.max.x - prefabObjectGeometryData.m_Bounds.min.x;
	}

	public static float GetLaneOffset(ObjectGeometryData prefabObjectGeometryData, NetLaneData prefabLaneData, float lanePosition)
	{
		float navigationSize = GetNavigationSize(prefabObjectGeometryData);
		float num = math.max(0f, prefabLaneData.m_Width - navigationSize);
		return lanePosition * num;
	}

	public static float3 GetLanePosition(Bezier4x3 curve, float curvePosition, float laneOffset)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float3 result = MathUtils.Position(curve, curvePosition);
		float3 val = MathUtils.Tangent(curve, curvePosition);
		float2 val2 = math.normalizesafe(((float3)(ref val)).xz, default(float2));
		((float3)(ref result)).xz = ((float3)(ref result)).xz + MathUtils.Right(val2) * laneOffset;
		return result;
	}

	public static float GetMaxBrakingSpeed(HumanData prefabHumanData, float distance, float timeStep)
	{
		float num = timeStep * prefabHumanData.m_Acceleration;
		return math.sqrt(num * num + 2f * prefabHumanData.m_Acceleration * distance) - num;
	}

	public static float GetMaxBrakingSpeed(HumanData prefabHumanData, float distance, float maxResultSpeed, float timeStep)
	{
		float num = timeStep * prefabHumanData.m_Acceleration;
		return math.sqrt(num * num + 2f * prefabHumanData.m_Acceleration * distance + maxResultSpeed * maxResultSpeed) - num;
	}

	public static float GetBrakingDistance(HumanData prefabHumanData, float speed, float timeStep)
	{
		return 0.5f * speed * speed / prefabHumanData.m_Acceleration + speed * timeStep;
	}

	public static float GetMaxBrakingSpeed(AnimalData prefabAnimalData, float distance, float timeStep)
	{
		float num = timeStep * prefabAnimalData.m_Acceleration;
		return math.sqrt(num * num + 2f * prefabAnimalData.m_Acceleration * distance) - num;
	}

	public static float GetMaxBrakingSpeed(AnimalData prefabAnimalData, float distance, float maxResultSpeed, float timeStep)
	{
		float num = timeStep * prefabAnimalData.m_Acceleration;
		return math.sqrt(num * num + 2f * prefabAnimalData.m_Acceleration * distance + maxResultSpeed * maxResultSpeed) - num;
	}

	public static float GetBrakingDistance(AnimalData prefabAnimalData, float speed, float timeStep)
	{
		return 0.5f * speed * speed / prefabAnimalData.m_Acceleration + speed * timeStep;
	}

	public static Sphere3 GetQueueArea(ObjectGeometryData prefabObjectGeometryData, float3 position)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Sphere3 result = default(Sphere3);
		result.radius = (prefabObjectGeometryData.m_Bounds.max.x - prefabObjectGeometryData.m_Bounds.min.x) * 0.5f + 0.25f;
		result.position = position;
		return result;
	}

	public static Sphere3 GetQueueArea(ObjectGeometryData prefabObjectGeometryData, float3 position1, float3 position2)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Sphere3 result = default(Sphere3);
		result.radius = (prefabObjectGeometryData.m_Bounds.max.x - prefabObjectGeometryData.m_Bounds.min.x + math.distance(position1, position2)) * 0.5f + 0.25f;
		result.position = math.lerp(position1, position2, 0.5f);
		return result;
	}

	public static void SetQueue(ref Entity queueEntity, ref Sphere3 queueArea, Entity setEntity, Sphere3 setArea)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (queueArea.radius > 0f && setArea.radius > 0f && queueEntity == setEntity)
		{
			queueArea = MathUtils.Sphere(queueArea, setArea);
			return;
		}
		queueEntity = setEntity;
		queueArea = setArea;
	}

	public static void FixPathStart(ref Random random, float3 position, int elementIndex, DynamicBuffer<PathElement> path, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Lane> laneData, ref ComponentLookup<EdgeLane> edgeLaneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref ComponentLookup<Curve> curveData, ref BufferLookup<Game.Net.SubLane> subLanes, ref BufferLookup<Game.Areas.Node> areaNodes, ref BufferLookup<Triangle> areaTriangles)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (path.Length <= elementIndex)
		{
			return;
		}
		PathElement pathElement = path[elementIndex];
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		if (connectionLaneData.TryGetComponent(pathElement.m_Target, ref connectionLane))
		{
			if ((connectionLane.m_Flags & ConnectionLaneFlags.Area) != 0)
			{
				FixPathStart_AreaLane(ref random, position, elementIndex, path, ref ownerData, ref curveData, ref laneData, ref connectionLaneData, ref subLanes, ref areaNodes, ref areaTriangles);
			}
		}
		else if (curveData.HasComponent(pathElement.m_Target))
		{
			FixPathStart_EdgeLane(ref random, position, elementIndex, path, ref ownerData, ref laneData, ref edgeLaneData, ref curveData, ref subLanes);
		}
	}

	private static void FixPathStart_AreaLane(ref Random random, float3 position, int elementIndex, DynamicBuffer<PathElement> path, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Lane> laneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref BufferLookup<Game.Net.SubLane> subLanes, ref BufferLookup<Game.Areas.Node> areaNodes, ref BufferLookup<Triangle> areaTriangles)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		Entity owner = ownerData[path[elementIndex].m_Target].m_Owner;
		DynamicBuffer<Game.Areas.Node> nodes = areaNodes[owner];
		DynamicBuffer<Triangle> val = areaTriangles[owner];
		int num = -1;
		float num2 = float.MaxValue;
		float2 val2 = float2.op_Implicit(0f);
		float2 val3 = default(float2);
		for (int i = 0; i < val.Length; i++)
		{
			float num3 = MathUtils.Distance(AreaUtils.GetTriangle3(nodes, val[i]), position, ref val3) + ((Random)(ref random)).NextFloat(0.5f);
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
				val2 = val3;
			}
		}
		if (num == -1)
		{
			return;
		}
		DynamicBuffer<Game.Net.SubLane> lanes = subLanes[owner];
		Triangle3 triangle = AreaUtils.GetTriangle3(nodes, val[num]);
		float3 val4 = MathUtils.Position(triangle, val2);
		num2 = float.MaxValue;
		Entity val5 = Entity.Null;
		float startCurvePos = 0f;
		bool2 val6 = default(bool2);
		float2 val7 = default(float2);
		float num5 = default(float);
		for (int j = 0; j < lanes.Length; j++)
		{
			Entity subLane = lanes[j].m_SubLane;
			if (!connectionLaneData.HasComponent(subLane) || (connectionLaneData[subLane].m_Flags & ConnectionLaneFlags.Pedestrian) == 0)
			{
				continue;
			}
			Curve curve = curveData[subLane];
			((bool2)(ref val6))._002Ector(MathUtils.Intersect(((Triangle3)(ref triangle)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val7), MathUtils.Intersect(((Triangle3)(ref triangle)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val7));
			if (math.any(val6))
			{
				float num4 = MathUtils.Distance(curve.m_Bezier, val4, ref num5);
				if (num4 < num2)
				{
					float2 val8 = math.select(new float2(0f, 0.49f), math.select(new float2(0.51f, 1f), new float2(0f, 1f), val6.x), val6.y);
					num2 = num4;
					val5 = subLane;
					startCurvePos = math.clamp(num5, val8.x, val8.y);
				}
			}
		}
		if (val5 == Entity.Null)
		{
			Debug.Log((object)$"Start path lane not found ({position.x}, {position.y}, {position.z})");
			return;
		}
		int k;
		Owner owner2 = default(Owner);
		for (k = elementIndex; k < path.Length - 1 && ownerData.TryGetComponent(path[k + 1].m_Target, ref owner2); k++)
		{
			if (owner2.m_Owner != owner)
			{
				break;
			}
		}
		NativeList<PathElement> path2 = default(NativeList<PathElement>);
		path2._002Ector(lanes.Length, AllocatorHandle.op_Implicit((Allocator)2));
		PathElement pathElement = path[k];
		AreaUtils.FindAreaPath(ref random, path2, lanes, val5, startCurvePos, pathElement.m_Target, pathElement.m_TargetDelta.y, laneData, curveData);
		if (path2.Length != 0)
		{
			int num6 = k - elementIndex + 1;
			int num7 = math.min(num6, path2.Length);
			for (int l = 0; l < num7; l++)
			{
				path[elementIndex + l] = path2[l];
			}
			if (path2.Length < num6)
			{
				path.RemoveRange(elementIndex + path2.Length, num6 - path2.Length);
			}
			else
			{
				for (int m = num6; m < path2.Length; m++)
				{
					path.Insert(elementIndex + m, path2[m]);
				}
			}
		}
		path2.Dispose();
	}

	private static void FixPathStart_EdgeLane(ref Random random, float3 position, int elementIndex, DynamicBuffer<PathElement> path, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Lane> laneData, ref ComponentLookup<EdgeLane> edgeLaneData, ref ComponentLookup<Curve> curveData, ref BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		PathElement pathElement = path[elementIndex];
		if (!edgeLaneData.HasComponent(pathElement.m_Target))
		{
			Lane lane = laneData[pathElement.m_Target];
			bool flag = pathElement.m_TargetDelta.x < 0.5f;
			if (!NetUtils.FindEdgeLane(ref pathElement.m_Target, ref ownerData, ref laneData, ref subLanes, flag))
			{
				return;
			}
			pathElement.m_TargetDelta = float2.op_Implicit(flag ? lane.m_StartNode.GetCurvePos() : lane.m_EndNode.GetCurvePos());
			path.Insert(elementIndex, pathElement);
		}
		Curve curve = curveData[pathElement.m_Target];
		Entity val = pathElement.m_Target;
		float num2 = default(float);
		float num = MathUtils.Distance(curve.m_Bezier, position, ref num2) + ((Random)(ref random)).NextFloat(0.5f);
		Entity entity = pathElement.m_Target;
		if (NetUtils.FindPrevLane(ref entity, ref ownerData, ref laneData, ref subLanes))
		{
			curve = curveData[entity];
			float num4 = default(float);
			float num3 = MathUtils.Distance(curve.m_Bezier, position, ref num4) + ((Random)(ref random)).NextFloat(0.5f);
			if (num3 < num)
			{
				val = entity;
				num = num3;
				num2 = num4;
			}
		}
		Entity entity2 = pathElement.m_Target;
		if (NetUtils.FindNextLane(ref entity2, ref ownerData, ref laneData, ref subLanes))
		{
			curve = curveData[entity2];
			float num6 = default(float);
			float num5 = MathUtils.Distance(curve.m_Bezier, position, ref num6) + ((Random)(ref random)).NextFloat(0.5f);
			if (num5 < num)
			{
				val = entity2;
				num = num5;
				num2 = num6;
			}
		}
		curve = curveData[val];
		float num7 = ((Random)(ref random)).NextFloat(-0.5f, 0.5f);
		if (num7 >= 0f)
		{
			Bounds1 val2 = default(Bounds1);
			((Bounds1)(ref val2))._002Ector(num2, 1f);
			num2 = ((!MathUtils.ClampLength(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val2, num7)) ? math.saturate(num2 + (1f - num2) * num7 / 0.5f) : val2.max);
		}
		else
		{
			num7 = 0f - num7;
			Bounds1 val3 = default(Bounds1);
			((Bounds1)(ref val3))._002Ector(0f, num2);
			num2 = ((!MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val3, num7)) ? math.saturate(num2 - num2 * num7 / 0.5f) : val3.min);
		}
		if (val == pathElement.m_Target)
		{
			pathElement.m_TargetDelta.x = num2;
			path[elementIndex] = pathElement;
		}
		else if (val == entity2)
		{
			if (elementIndex < path.Length - 1 && path[elementIndex + 1].m_Target == entity2)
			{
				path.RemoveAt(elementIndex);
				pathElement = path[elementIndex];
				pathElement.m_TargetDelta.x = num2;
				path[elementIndex] = pathElement;
			}
			else
			{
				path.Insert(elementIndex + 1, new PathElement
				{
					m_Target = pathElement.m_Target,
					m_TargetDelta = new float2(1f, pathElement.m_TargetDelta.y)
				});
				pathElement.m_Target = entity2;
				pathElement.m_TargetDelta = new float2(num2, 0f);
				path[elementIndex] = pathElement;
			}
		}
		else if (val == entity)
		{
			if (elementIndex < path.Length - 1 && path[elementIndex + 1].m_Target == entity)
			{
				path.RemoveAt(elementIndex);
				pathElement = path[elementIndex];
				pathElement.m_TargetDelta.x = num2;
				path[elementIndex] = pathElement;
			}
			else
			{
				path.Insert(elementIndex + 1, new PathElement
				{
					m_Target = pathElement.m_Target,
					m_TargetDelta = new float2(0f, pathElement.m_TargetDelta.y)
				});
				pathElement.m_Target = entity;
				pathElement.m_TargetDelta = new float2(num2, 1f);
				path[elementIndex] = pathElement;
			}
		}
	}

	public static void FixEnterPath(ref Random random, float3 position, int elementIndex, DynamicBuffer<PathElement> path, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Lane> laneData, ref ComponentLookup<EdgeLane> edgeLaneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref ComponentLookup<Curve> curveData, ref BufferLookup<Game.Net.SubLane> subLanes, ref BufferLookup<Game.Areas.Node> areaNodes, ref BufferLookup<Triangle> areaTriangles)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (path.Length <= elementIndex)
		{
			return;
		}
		PathElement pathElement = path[elementIndex];
		Game.Net.ConnectionLane connectionLane = default(Game.Net.ConnectionLane);
		if (connectionLaneData.TryGetComponent(pathElement.m_Target, ref connectionLane))
		{
			if ((connectionLane.m_Flags & ConnectionLaneFlags.Area) != 0)
			{
				FixEnterPath_AreaLane(ref random, position, elementIndex, path, ref ownerData, ref curveData, ref laneData, ref connectionLaneData, ref subLanes, ref areaNodes, ref areaTriangles);
			}
		}
		else if (curveData.HasComponent(pathElement.m_Target))
		{
			FixEnterPath_EdgeLane(ref random, position, elementIndex, path, ref ownerData, ref laneData, ref edgeLaneData, ref curveData, ref subLanes);
		}
	}

	private static void FixEnterPath_AreaLane(ref Random random, float3 position, int elementIndex, DynamicBuffer<PathElement> path, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Curve> curveData, ref ComponentLookup<Lane> laneData, ref ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, ref BufferLookup<Game.Net.SubLane> subLanes, ref BufferLookup<Game.Areas.Node> areaNodes, ref BufferLookup<Triangle> areaTriangles)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		Entity owner = ownerData[path[elementIndex].m_Target].m_Owner;
		DynamicBuffer<Game.Areas.Node> nodes = areaNodes[owner];
		DynamicBuffer<Triangle> val = areaTriangles[owner];
		int num = -1;
		float num2 = float.MaxValue;
		float2 val2 = float2.op_Implicit(0f);
		float2 val3 = default(float2);
		for (int i = 0; i < val.Length; i++)
		{
			float num3 = MathUtils.Distance(AreaUtils.GetTriangle3(nodes, val[i]), position, ref val3) + ((Random)(ref random)).NextFloat(0.5f);
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
				val2 = val3;
			}
		}
		if (num == -1)
		{
			return;
		}
		DynamicBuffer<Game.Net.SubLane> lanes = subLanes[owner];
		Triangle3 triangle = AreaUtils.GetTriangle3(nodes, val[num]);
		float3 val4 = MathUtils.Position(triangle, val2);
		num2 = float.MaxValue;
		Entity val5 = Entity.Null;
		float endCurvePos = 0f;
		bool2 val6 = default(bool2);
		float2 val7 = default(float2);
		float num5 = default(float);
		for (int j = 0; j < lanes.Length; j++)
		{
			Entity subLane = lanes[j].m_SubLane;
			if (!connectionLaneData.HasComponent(subLane) || (connectionLaneData[subLane].m_Flags & ConnectionLaneFlags.Pedestrian) == 0)
			{
				continue;
			}
			Curve curve = curveData[subLane];
			((bool2)(ref val6))._002Ector(MathUtils.Intersect(((Triangle3)(ref triangle)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val7), MathUtils.Intersect(((Triangle3)(ref triangle)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val7));
			if (math.any(val6))
			{
				float num4 = MathUtils.Distance(curve.m_Bezier, val4, ref num5);
				if (num4 < num2)
				{
					float2 val8 = math.select(new float2(0f, 0.49f), math.select(new float2(0.51f, 1f), new float2(0f, 1f), val6.x), val6.y);
					num2 = num4;
					val5 = subLane;
					endCurvePos = math.clamp(num5, val8.x, val8.y);
				}
			}
		}
		if (val5 == Entity.Null)
		{
			Debug.Log((object)$"Enter path lane not found ({position.x}, {position.y}, {position.z})");
			return;
		}
		NativeList<PathElement> path2 = default(NativeList<PathElement>);
		path2._002Ector(lanes.Length, AllocatorHandle.op_Implicit((Allocator)2));
		PathElement pathElement = path[elementIndex];
		AreaUtils.FindAreaPath(ref random, path2, lanes, pathElement.m_Target, pathElement.m_TargetDelta.x, val5, endCurvePos, laneData, curveData);
		if (path2.Length != 0)
		{
			path[elementIndex] = path2[0];
			for (int k = 1; k < path2.Length; k++)
			{
				path.Insert(elementIndex + k, path2[k]);
			}
		}
		path2.Dispose();
	}

	private static void FixEnterPath_EdgeLane(ref Random random, float3 position, int elementIndex, DynamicBuffer<PathElement> path, ref ComponentLookup<Owner> ownerData, ref ComponentLookup<Lane> laneData, ref ComponentLookup<EdgeLane> edgeLaneData, ref ComponentLookup<Curve> curveData, ref BufferLookup<Game.Net.SubLane> subLanes)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		PathElement pathElement = path[elementIndex];
		if (!edgeLaneData.HasComponent(pathElement.m_Target))
		{
			Lane lane = laneData[pathElement.m_Target];
			bool flag = pathElement.m_TargetDelta.y < 0.5f;
			if (!NetUtils.FindEdgeLane(ref pathElement.m_Target, ref ownerData, ref laneData, ref subLanes, flag))
			{
				return;
			}
			pathElement.m_TargetDelta = float2.op_Implicit(flag ? lane.m_StartNode.GetCurvePos() : lane.m_EndNode.GetCurvePos());
			path.Insert(elementIndex + 1, pathElement);
		}
		Curve curve = curveData[pathElement.m_Target];
		Entity val = pathElement.m_Target;
		float num2 = default(float);
		float num = MathUtils.Distance(curve.m_Bezier, position, ref num2) + ((Random)(ref random)).NextFloat(0.5f);
		Entity entity = pathElement.m_Target;
		if (NetUtils.FindPrevLane(ref entity, ref ownerData, ref laneData, ref subLanes))
		{
			curve = curveData[entity];
			float num4 = default(float);
			float num3 = MathUtils.Distance(curve.m_Bezier, position, ref num4) + ((Random)(ref random)).NextFloat(0.5f);
			if (num3 < num)
			{
				val = entity;
				num = num3;
				num2 = num4;
			}
		}
		Entity entity2 = pathElement.m_Target;
		if (NetUtils.FindNextLane(ref entity2, ref ownerData, ref laneData, ref subLanes))
		{
			curve = curveData[entity2];
			float num6 = default(float);
			float num5 = MathUtils.Distance(curve.m_Bezier, position, ref num6) + ((Random)(ref random)).NextFloat(0.5f);
			if (num5 < num)
			{
				val = entity2;
				num = num5;
				num2 = num6;
			}
		}
		curve = curveData[val];
		float num7 = ((Random)(ref random)).NextFloat(-0.5f, 0.5f);
		if (num7 >= 0f)
		{
			Bounds1 val2 = default(Bounds1);
			((Bounds1)(ref val2))._002Ector(num2, 1f);
			num2 = ((!MathUtils.ClampLength(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val2, num7)) ? math.saturate(num2 + (1f - num2) * num7 / 0.5f) : val2.max);
		}
		else
		{
			num7 = 0f - num7;
			Bounds1 val3 = default(Bounds1);
			((Bounds1)(ref val3))._002Ector(0f, num2);
			num2 = ((!MathUtils.ClampLengthInverse(((Bezier4x3)(ref curve.m_Bezier)).xz, ref val3, num7)) ? math.saturate(num2 - num2 * num7 / 0.5f) : val3.min);
		}
		if (val == pathElement.m_Target)
		{
			pathElement.m_TargetDelta.y = num2;
			path[elementIndex] = pathElement;
		}
		else if (val == entity2)
		{
			path.Insert(elementIndex + 1, new PathElement
			{
				m_Target = entity2,
				m_TargetDelta = new float2(0f, num2)
			});
			pathElement.m_TargetDelta.y = 1f;
			path[elementIndex] = pathElement;
		}
		else if (val == entity)
		{
			path.Insert(elementIndex + 1, new PathElement
			{
				m_Target = entity,
				m_TargetDelta = new float2(1f, num2)
			});
			pathElement.m_TargetDelta.y = 0f;
			path[elementIndex] = pathElement;
		}
	}

	public static void SetRandomAreaTarget(ref Random random, int elementIndex, DynamicBuffer<PathElement> path, ComponentLookup<Owner> ownerData, ComponentLookup<Curve> curveData, ComponentLookup<Lane> laneData, ComponentLookup<Game.Net.ConnectionLane> connectionLaneData, BufferLookup<Game.Net.SubLane> subLanes, BufferLookup<Game.Areas.Node> areaNodes, BufferLookup<Triangle> areaTriangles)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		Entity owner = ownerData[path[elementIndex].m_Target].m_Owner;
		DynamicBuffer<Game.Areas.Node> nodes = areaNodes[owner];
		DynamicBuffer<Triangle> val = areaTriangles[owner];
		int num = -1;
		float num2 = 0f;
		for (int i = 0; i < val.Length; i++)
		{
			Triangle3 triangle = AreaUtils.GetTriangle3(nodes, val[i]);
			float num3 = MathUtils.Area(((Triangle3)(ref triangle)).xz);
			num2 += num3;
			if (((Random)(ref random)).NextFloat(num2) < num3)
			{
				num = i;
			}
		}
		if (num == -1)
		{
			return;
		}
		DynamicBuffer<Game.Net.SubLane> lanes = subLanes[owner];
		float2 val2 = ((Random)(ref random)).NextFloat2(float2.op_Implicit(1f));
		val2 = math.select(val2, 1f - val2, math.csum(val2) > 1f);
		Triangle3 triangle2 = AreaUtils.GetTriangle3(nodes, val[num]);
		float3 val3 = MathUtils.Position(triangle2, val2);
		float num4 = float.MaxValue;
		Entity val4 = Entity.Null;
		float endCurvePos = 0f;
		bool2 val5 = default(bool2);
		float2 val6 = default(float2);
		float num6 = default(float);
		for (int j = 0; j < lanes.Length; j++)
		{
			Entity subLane = lanes[j].m_SubLane;
			if (!connectionLaneData.HasComponent(subLane) || (connectionLaneData[subLane].m_Flags & ConnectionLaneFlags.Pedestrian) == 0)
			{
				continue;
			}
			Curve curve = curveData[subLane];
			((bool2)(ref val5))._002Ector(MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.a)).xz, ref val6), MathUtils.Intersect(((Triangle3)(ref triangle2)).xz, ((float3)(ref curve.m_Bezier.d)).xz, ref val6));
			if (math.any(val5))
			{
				float num5 = MathUtils.Distance(curve.m_Bezier, val3, ref num6);
				if (num5 < num4)
				{
					float2 val7 = math.select(new float2(0f, 0.49f), math.select(new float2(0.51f, 1f), new float2(0f, 1f), val5.x), val5.y);
					num4 = num5;
					val4 = subLane;
					endCurvePos = ((Random)(ref random)).NextFloat(val7.x, val7.y);
				}
			}
		}
		if (val4 == Entity.Null)
		{
			return;
		}
		int num7 = elementIndex;
		Owner owner2 = default(Owner);
		while (num7 > 0 && ownerData.TryGetComponent(path[num7 - 1].m_Target, ref owner2) && !(owner2.m_Owner != owner))
		{
			num7--;
		}
		NativeList<PathElement> path2 = default(NativeList<PathElement>);
		path2._002Ector(lanes.Length, AllocatorHandle.op_Implicit((Allocator)2));
		PathElement pathElement = path[num7];
		AreaUtils.FindAreaPath(ref random, path2, lanes, pathElement.m_Target, pathElement.m_TargetDelta.x, val4, endCurvePos, laneData, curveData);
		if (path2.Length != 0)
		{
			int num8 = elementIndex - num7 + 1;
			int num9 = math.min(num8, path2.Length);
			for (int k = 0; k < num9; k++)
			{
				path[num7 + k] = path2[k];
			}
			if (path2.Length < num8)
			{
				path.RemoveRange(num7 + path2.Length, num8 - path2.Length);
			}
			else
			{
				for (int l = num8; l < path2.Length; l++)
				{
					path.Insert(num7 + l, path2[l]);
				}
			}
		}
		path2.Dispose();
	}

	public static void CheckUnspawned(int jobIndex, Entity entity, HumanCurrentLane currentLane, Human human, bool isUnspawned, ParallelWriter commandBuffer)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0 || (human.m_Flags & (HumanFlags.Dead | HumanFlags.Carried)) != 0)
		{
			if (!isUnspawned)
			{
				((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, entity, default(Unspawned));
				((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
			}
		}
		else if ((currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0 && currentLane.m_Lane != Entity.Null && isUnspawned)
		{
			((ParallelWriter)(ref commandBuffer)).RemoveComponent<Unspawned>(jobIndex, entity);
			((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
		}
	}

	public static void CheckUnspawned(int jobIndex, Entity entity, AnimalCurrentLane currentLane, Animal animal, bool isUnspawned, ParallelWriter commandBuffer)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if ((currentLane.m_Flags & CreatureLaneFlags.Connection) != 0)
		{
			if (!isUnspawned)
			{
				((ParallelWriter)(ref commandBuffer)).AddComponent<Unspawned>(jobIndex, entity, default(Unspawned));
				((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
			}
		}
		else if ((currentLane.m_Flags & CreatureLaneFlags.TransformTarget) == 0 && (currentLane.m_Lane != Entity.Null || (animal.m_Flags & AnimalFlags.Roaming) != 0) && isUnspawned)
		{
			((ParallelWriter)(ref commandBuffer)).RemoveComponent<Unspawned>(jobIndex, entity);
			((ParallelWriter)(ref commandBuffer)).AddComponent<BatchesUpdated>(jobIndex, entity, default(BatchesUpdated));
		}
	}
}
