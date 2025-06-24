using System.Runtime.CompilerServices;
using Game.Areas;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class WorkCarAISystem : GameSystemBase
{
	private struct WorkAction
	{
		public VehicleWorkType m_WorkType;

		public Entity m_Target;

		public Entity m_Owner;

		public float m_WorkAmount;
	}

	[BurstCompile]
	private struct WorkCarTickJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> m_UnspawnedType;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> m_PathInformationType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> m_LayoutElementType;

		public ComponentTypeHandle<Car> m_CarType;

		public ComponentTypeHandle<CarCurrentLane> m_CurrentLaneType;

		public ComponentTypeHandle<Target> m_TargetType;

		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public EntityStorageInfoLookup m_EntityLookup;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Attachment> m_AttachmentData;

		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public ComponentLookup<Plant> m_PlantData;

		[ReadOnly]
		public ComponentLookup<Damaged> m_DamagedData;

		[ReadOnly]
		public ComponentLookup<Area> m_AreaData;

		[ReadOnly]
		public ComponentLookup<CarData> m_PrefabCarData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> m_PrefabWorkVehicleData;

		[ReadOnly]
		public ComponentLookup<TreeData> m_PrefabTreeData;

		[ReadOnly]
		public ComponentLookup<SlaveLane> m_SlaveLaneData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Game.Vehicles.WorkVehicle> m_WorkVehicleData;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<SetupQueueItem> m_PathfindQueue;

		public ParallelWriter<WorkAction> m_WorkQueue;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Owner> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<PathInformation> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathInformation>(ref m_PathInformationType);
			NativeArray<PrefabRef> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<CarCurrentLane> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<CarCurrentLane>(ref m_CurrentLaneType);
			NativeArray<Car> nativeArray6 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Car>(ref m_CarType);
			NativeArray<Target> nativeArray7 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Target>(ref m_TargetType);
			NativeArray<PathOwner> nativeArray8 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			BufferAccessor<LayoutElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<LayoutElement>(ref m_LayoutElementType);
			BufferAccessor<PathElement> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			bool isUnspawned = ((ArchetypeChunk)(ref chunk)).Has<Unspawned>(ref m_UnspawnedType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Owner owner = nativeArray2[i];
				PathInformation pathInformation = nativeArray3[i];
				PrefabRef prefabRef = nativeArray4[i];
				Car car = nativeArray6[i];
				CarCurrentLane currentLane = nativeArray5[i];
				PathOwner pathOwner = nativeArray8[i];
				Target target = nativeArray7[i];
				DynamicBuffer<PathElement> path = bufferAccessor2[i];
				DynamicBuffer<LayoutElement> layout = default(DynamicBuffer<LayoutElement>);
				if (bufferAccessor.Length != 0)
				{
					layout = bufferAccessor[i];
				}
				Game.Vehicles.WorkVehicle workVehicle = m_WorkVehicleData[val];
				VehicleUtils.CheckUnspawned(unfilteredChunkIndex, val, currentLane, isUnspawned, m_CommandBuffer);
				Tick(unfilteredChunkIndex, val, owner, pathInformation, prefabRef, layout, path, ref workVehicle, ref car, ref currentLane, ref pathOwner, ref target);
				m_WorkVehicleData[val] = workVehicle;
				nativeArray6[i] = car;
				nativeArray5[i] = currentLane;
				nativeArray8[i] = pathOwner;
				nativeArray7[i] = target;
			}
		}

		private void Tick(int jobIndex, Entity vehicleEntity, Owner owner, PathInformation pathInformation, PrefabRef prefabRef, DynamicBuffer<LayoutElement> layout, DynamicBuffer<PathElement> path, ref Game.Vehicles.WorkVehicle workVehicle, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			if (VehicleUtils.ResetUpdatedPath(ref pathOwner) && !ResetPath(jobIndex, vehicleEntity, pathInformation, path, layout, ref workVehicle, ref car, ref currentLane, ref target))
			{
				ReturnToDepot(jobIndex, vehicleEntity, owner, ref workVehicle, ref car, ref pathOwner, ref target);
				FindPathIfNeeded(vehicleEntity, owner, prefabRef, layout, ref workVehicle, ref car, ref currentLane, ref pathOwner, ref target);
				return;
			}
			if (!((EntityStorageInfoLookup)(ref m_EntityLookup)).Exists(target.m_Target) || VehicleUtils.PathfindFailed(pathOwner))
			{
				if (VehicleUtils.IsStuck(pathOwner) || (workVehicle.m_State & WorkVehicleFlags.Returning) != 0)
				{
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
					return;
				}
				ReturnToDepot(jobIndex, vehicleEntity, owner, ref workVehicle, ref car, ref pathOwner, ref target);
			}
			else if (VehicleUtils.PathEndReached(currentLane))
			{
				if ((workVehicle.m_State & WorkVehicleFlags.Returning) != 0)
				{
					VehicleUtils.DeleteVehicle(m_CommandBuffer, jobIndex, vehicleEntity, layout);
					return;
				}
				if (PerformWork(jobIndex, vehicleEntity, owner, prefabRef, layout, ref workVehicle, ref target, ref pathOwner))
				{
					ReturnToDepot(jobIndex, vehicleEntity, owner, ref workVehicle, ref car, ref pathOwner, ref target);
				}
			}
			car.m_Flags |= CarFlags.Warning | CarFlags.Working;
			FindPathIfNeeded(vehicleEntity, owner, prefabRef, layout, ref workVehicle, ref car, ref currentLane, ref pathOwner, ref target);
		}

		private void FindPathIfNeeded(Entity vehicleEntity, Owner owner, PrefabRef prefabRef, DynamicBuffer<LayoutElement> layout, ref Game.Vehicles.WorkVehicle workVehicle, ref Car car, ref CarCurrentLane currentLane, ref PathOwner pathOwner, ref Target target)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0267: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0273: Unknown result type (might be due to invalid IL or missing references)
			//IL_0281: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0204: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			if (!VehicleUtils.RequireNewPath(pathOwner))
			{
				return;
			}
			CarData carData = m_PrefabCarData[prefabRef.m_Prefab];
			PathfindParameters parameters = new PathfindParameters
			{
				m_MaxSpeed = float2.op_Implicit(carData.m_MaxSpeed),
				m_WalkSpeed = float2.op_Implicit(5.555556f),
				m_Weights = new PathfindWeights(1f, 1f, 1f, 1f),
				m_Methods = (PathMethod.Road | PathMethod.Offroad),
				m_IgnoredRules = VehicleUtils.GetIgnoredPathfindRules(carData)
			};
			SetupQueueTarget origin = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (PathMethod.Road | PathMethod.Offroad),
				m_RoadTypes = RoadTypes.Car
			};
			SetupQueueTarget destination = new SetupQueueTarget
			{
				m_Type = SetupTargetType.CurrentLocation,
				m_Methods = (PathMethod.Road | PathMethod.Offroad),
				m_RoadTypes = RoadTypes.Car,
				m_Entity = target.m_Target
			};
			WorkVehicleData workVehicleData;
			if (layout.IsCreated && layout.Length != 0)
			{
				workVehicleData = default(WorkVehicleData);
				for (int i = 0; i < layout.Length; i++)
				{
					Entity vehicle = layout[i].m_Vehicle;
					PrefabRef prefabRef2 = m_PrefabRefData[vehicle];
					WorkVehicleData workVehicleData2 = m_PrefabWorkVehicleData[prefabRef2.m_Prefab];
					if (workVehicleData2.m_WorkType != VehicleWorkType.None)
					{
						workVehicleData.m_WorkType = workVehicleData2.m_WorkType;
					}
					if (workVehicleData2.m_MapFeature != MapFeature.None)
					{
						workVehicleData.m_MapFeature = workVehicleData2.m_MapFeature;
					}
					workVehicleData.m_Resources |= workVehicleData2.m_Resources;
				}
			}
			else
			{
				workVehicleData = m_PrefabWorkVehicleData[prefabRef.m_Prefab];
			}
			if (workVehicleData.m_WorkType == VehicleWorkType.Move)
			{
				origin.m_Methods |= PathMethod.CargoLoading;
				destination.m_Methods |= PathMethod.CargoLoading;
			}
			if ((workVehicle.m_State & (WorkVehicleFlags.Returning | WorkVehicleFlags.ExtractorVehicle)) == WorkVehicleFlags.ExtractorVehicle)
			{
				if (workVehicleData.m_MapFeature == MapFeature.Forest)
				{
					destination.m_Type = SetupTargetType.WoodResource;
				}
				else
				{
					destination.m_Type = SetupTargetType.AreaLocation;
				}
				destination.m_Entity = owner.m_Owner;
				destination.m_Value = (int)workVehicleData.m_WorkType;
				target.m_Target = owner.m_Owner;
			}
			else if ((workVehicle.m_State & (WorkVehicleFlags.Returning | WorkVehicleFlags.StorageVehicle)) == WorkVehicleFlags.StorageVehicle || (workVehicle.m_State & (WorkVehicleFlags.Returning | WorkVehicleFlags.CargoMoveVehicle)) == WorkVehicleFlags.CargoMoveVehicle)
			{
				destination.m_Type = SetupTargetType.AreaLocation;
				destination.m_Entity = owner.m_Owner;
				destination.m_Value = (int)workVehicleData.m_WorkType;
				target.m_Target = owner.m_Owner;
			}
			VehicleUtils.SetupPathfind(item: new SetupQueueItem(vehicleEntity, parameters, origin, destination), currentLane: ref currentLane, pathOwner: ref pathOwner, queue: m_PathfindQueue);
		}

		private bool ResetPath(int jobIndex, Entity vehicleEntity, PathInformation pathInformation, DynamicBuffer<PathElement> path, DynamicBuffer<LayoutElement> layout, ref Game.Vehicles.WorkVehicle workVehicle, ref Car car, ref CarCurrentLane currentLane, ref Target target)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			PathUtils.ResetPath(ref currentLane, path, m_SlaveLaneData, m_OwnerData, m_SubLanes);
			if (layout.IsCreated && layout.Length >= 2)
			{
				car.m_Flags |= CarFlags.CannotReverse;
			}
			else
			{
				car.m_Flags &= ~CarFlags.CannotReverse;
			}
			if ((workVehicle.m_State & (WorkVehicleFlags.Returning | WorkVehicleFlags.CargoMoveVehicle)) == WorkVehicleFlags.Returning)
			{
				car.m_Flags &= ~CarFlags.StayOnRoad;
			}
			else
			{
				car.m_Flags |= CarFlags.StayOnRoad;
			}
			if ((workVehicle.m_State & WorkVehicleFlags.Returning) == 0)
			{
				target.m_Target = pathInformation.m_Destination;
			}
			return true;
		}

		private void ReturnToDepot(int jobIndex, Entity vehicleEntity, Owner ownerData, ref Game.Vehicles.WorkVehicle workVehicle, ref Car car, ref PathOwner pathOwner, ref Target target)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			workVehicle.m_State |= WorkVehicleFlags.Returning;
			Entity newTarget = ownerData.m_Owner;
			Owner owner = default(Owner);
			if (m_AreaData.HasComponent(ownerData.m_Owner) && m_OwnerData.TryGetComponent(ownerData.m_Owner, ref owner))
			{
				Attachment attachment = default(Attachment);
				newTarget = ((!m_AttachmentData.TryGetComponent(owner.m_Owner, ref attachment)) ? owner.m_Owner : attachment.m_Attached);
			}
			VehicleUtils.SetTarget(ref pathOwner, ref target, newTarget);
		}

		private bool PerformWork(int jobIndex, Entity vehicleEntity, Owner owner, PrefabRef prefabRef, DynamicBuffer<LayoutElement> layout, ref Game.Vehicles.WorkVehicle workVehicle, ref Target target, ref PathOwner pathOwner)
		{
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_044a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0312: Unknown result type (might be due to invalid IL or missing references)
			//IL_0353: Unknown result type (might be due to invalid IL or missing references)
			//IL_035e: Unknown result type (might be due to invalid IL or missing references)
			//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_03be: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0429: Unknown result type (might be due to invalid IL or missing references)
			WorkVehicleData workVehicleData;
			if (layout.IsCreated && layout.Length != 0)
			{
				workVehicleData = default(WorkVehicleData);
				for (int i = 0; i < layout.Length; i++)
				{
					Entity vehicle = layout[i].m_Vehicle;
					PrefabRef prefabRef2 = m_PrefabRefData[vehicle];
					WorkVehicleData workVehicleData2 = m_PrefabWorkVehicleData[prefabRef2.m_Prefab];
					if (workVehicleData2.m_WorkType != VehicleWorkType.None)
					{
						workVehicleData.m_WorkType = workVehicleData2.m_WorkType;
						workVehicleData.m_MaxWorkAmount += workVehicleData2.m_MaxWorkAmount;
					}
					if (workVehicleData2.m_MapFeature != MapFeature.None)
					{
						workVehicleData.m_MapFeature = workVehicleData2.m_MapFeature;
					}
					workVehicleData.m_Resources |= workVehicleData2.m_Resources;
				}
			}
			else
			{
				workVehicleData = m_PrefabWorkVehicleData[prefabRef.m_Prefab];
			}
			float num = workVehicleData.m_MaxWorkAmount;
			if ((workVehicle.m_State & WorkVehicleFlags.ExtractorVehicle) != 0)
			{
				switch (workVehicleData.m_WorkType)
				{
				case VehicleWorkType.Harvest:
					num = 1000f;
					if (m_TreeData.HasComponent(target.m_Target))
					{
						Tree tree = m_TreeData[target.m_Target];
						Plant plant = m_PlantData[target.m_Target];
						PrefabRef prefabRef3 = m_PrefabRefData[target.m_Target];
						Damaged damaged = default(Damaged);
						m_DamagedData.TryGetComponent(target.m_Target, ref damaged);
						TreeData treeData = default(TreeData);
						if (m_PrefabTreeData.TryGetComponent(prefabRef3.m_Prefab, ref treeData))
						{
							num = ObjectUtils.CalculateWoodAmount(tree, plant, damaged, treeData);
						}
						((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, target.m_Target, default(BatchesUpdated));
					}
					m_WorkQueue.Enqueue(new WorkAction
					{
						m_WorkType = workVehicleData.m_WorkType,
						m_Target = target.m_Target,
						m_Owner = owner.m_Owner,
						m_WorkAmount = num
					});
					break;
				case VehicleWorkType.Collect:
					if (m_TreeData.HasComponent(target.m_Target))
					{
						m_WorkQueue.Enqueue(new WorkAction
						{
							m_WorkType = workVehicleData.m_WorkType,
							m_Target = target.m_Target
						});
					}
					num = workVehicleData.m_MaxWorkAmount * 0.25f;
					break;
				}
			}
			else if ((workVehicle.m_State & (WorkVehicleFlags.StorageVehicle | WorkVehicleFlags.CargoMoveVehicle)) != 0)
			{
				num = workVehicleData.m_MaxWorkAmount * 0.25f;
			}
			VehicleUtils.SetTarget(ref pathOwner, ref target, Entity.Null);
			if (layout.IsCreated && layout.Length != 0)
			{
				float num2 = 0f;
				float num3 = 0f;
				for (int j = 0; j < layout.Length; j++)
				{
					Entity vehicle2 = layout[j].m_Vehicle;
					if (vehicle2 == vehicleEntity)
					{
						float num4 = math.min(num, workVehicle.m_WorkAmount - workVehicle.m_DoneAmount);
						if (num4 > 0f)
						{
							workVehicle.m_DoneAmount += num4;
							num -= num4;
							QuantityUpdated(jobIndex, vehicle2);
						}
						num2 += workVehicle.m_DoneAmount;
						num3 += workVehicle.m_WorkAmount;
						continue;
					}
					Game.Vehicles.WorkVehicle workVehicle2 = m_WorkVehicleData[vehicle2];
					float num5 = math.min(num, workVehicle2.m_WorkAmount - workVehicle2.m_DoneAmount);
					if (num5 > 0f)
					{
						workVehicle2.m_DoneAmount += num5;
						num -= num5;
						m_WorkVehicleData[vehicle2] = workVehicle2;
						QuantityUpdated(jobIndex, vehicle2);
					}
					num2 += workVehicle2.m_DoneAmount;
					num3 += workVehicle2.m_WorkAmount;
				}
				if (num < 1f)
				{
					return num2 > num3 - 1f;
				}
				for (int k = 0; k < layout.Length; k++)
				{
					Entity vehicle3 = layout[k].m_Vehicle;
					if (vehicle3 == vehicleEntity)
					{
						if (workVehicle.m_WorkAmount >= 1f)
						{
							workVehicle.m_DoneAmount += num * workVehicle.m_WorkAmount / num3;
						}
						continue;
					}
					Game.Vehicles.WorkVehicle workVehicle3 = m_WorkVehicleData[vehicle3];
					if (workVehicle3.m_WorkAmount >= 1f)
					{
						workVehicle3.m_DoneAmount += num * workVehicle3.m_WorkAmount / num3;
						m_WorkVehicleData[vehicle3] = workVehicle3;
					}
				}
				return true;
			}
			QuantityUpdated(jobIndex, vehicleEntity);
			workVehicle.m_DoneAmount += num;
			return workVehicle.m_DoneAmount > workVehicle.m_WorkAmount - 1f;
		}

		private void QuantityUpdated(int jobIndex, Entity vehicleEntity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			if (m_SubObjects.HasBuffer(vehicleEntity))
			{
				DynamicBuffer<Game.Objects.SubObject> val = m_SubObjects[vehicleEntity];
				for (int i = 0; i < val.Length; i++)
				{
					Entity subObject = val[i].m_SubObject;
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(jobIndex, subObject, default(BatchesUpdated));
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct WorkCarWorkJob : IJob
	{
		public ComponentLookup<Tree> m_TreeData;

		public ComponentLookup<Extractor> m_ExtractorData;

		public NativeQueue<WorkAction> m_WorkQueue;

		public void Execute()
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			int count = m_WorkQueue.Count;
			for (int i = 0; i < count; i++)
			{
				WorkAction workAction = m_WorkQueue.Dequeue();
				switch (workAction.m_WorkType)
				{
				case VehicleWorkType.Harvest:
				{
					float num = 0f;
					if (m_TreeData.HasComponent(workAction.m_Target))
					{
						Tree tree2 = m_TreeData[workAction.m_Target];
						if ((tree2.m_State & TreeState.Stump) == 0)
						{
							tree2.m_State &= ~(TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Collected);
							tree2.m_State |= TreeState.Stump;
							tree2.m_Growth = 0;
							m_TreeData[workAction.m_Target] = tree2;
							num = workAction.m_WorkAmount;
						}
					}
					if (m_ExtractorData.HasComponent(workAction.m_Owner))
					{
						Extractor extractor = m_ExtractorData[workAction.m_Owner];
						extractor.m_ExtractedAmount -= num;
						extractor.m_HarvestedAmount += workAction.m_WorkAmount;
						m_ExtractorData[workAction.m_Owner] = extractor;
					}
					break;
				}
				case VehicleWorkType.Collect:
					if (m_TreeData.HasComponent(workAction.m_Target))
					{
						Tree tree = m_TreeData[workAction.m_Target];
						if ((tree.m_State & TreeState.Collected) == 0)
						{
							tree.m_State |= TreeState.Collected;
							m_TreeData[workAction.m_Target] = tree;
						}
					}
					break;
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Owner> __Game_Common_Owner_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Unspawned> __Game_Objects_Unspawned_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PathInformation> __Game_Pathfind_PathInformation_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<LayoutElement> __Game_Vehicles_LayoutElement_RO_BufferTypeHandle;

		public ComponentTypeHandle<Car> __Game_Vehicles_Car_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle;

		public ComponentTypeHandle<Target> __Game_Common_Target_RW_ComponentTypeHandle;

		public ComponentTypeHandle<PathOwner> __Game_Pathfind_PathOwner_RW_ComponentTypeHandle;

		public BufferTypeHandle<PathElement> __Game_Pathfind_PathElement_RW_BufferTypeHandle;

		[ReadOnly]
		public EntityStorageInfoLookup __EntityStorageInfoLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attachment> __Game_Objects_Attachment_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Tree> __Game_Objects_Tree_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Plant> __Game_Objects_Plant_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Area> __Game_Areas_Area_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CarData> __Game_Prefabs_CarData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkVehicleData> __Game_Prefabs_WorkVehicleData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TreeData> __Game_Prefabs_TreeData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SlaveLane> __Game_Net_SlaveLane_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		public ComponentLookup<Game.Vehicles.WorkVehicle> __Game_Vehicles_WorkVehicle_RW_ComponentLookup;

		public ComponentLookup<Tree> __Game_Objects_Tree_RW_ComponentLookup;

		public ComponentLookup<Extractor> __Game_Areas_Extractor_RW_ComponentLookup;

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
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Owner_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(true);
			__Game_Objects_Unspawned_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Unspawned>(true);
			__Game_Pathfind_PathInformation_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathInformation>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Vehicles_LayoutElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<LayoutElement>(true);
			__Game_Vehicles_Car_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Car>(false);
			__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CarCurrentLane>(false);
			__Game_Common_Target_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Target>(false);
			__Game_Pathfind_PathOwner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PathOwner>(false);
			__Game_Pathfind_PathElement_RW_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<PathElement>(false);
			__EntityStorageInfoLookup = ((SystemState)(ref state)).GetEntityStorageInfoLookup();
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Attachment_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attachment>(true);
			__Game_Objects_Tree_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(true);
			__Game_Objects_Plant_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Plant>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Areas_Area_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Area>(true);
			__Game_Prefabs_CarData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_WorkVehicleData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkVehicleData>(true);
			__Game_Prefabs_TreeData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TreeData>(true);
			__Game_Net_SlaveLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SlaveLane>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Vehicles_WorkVehicle_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Vehicles.WorkVehicle>(false);
			__Game_Objects_Tree_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Tree>(false);
			__Game_Areas_Extractor_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extractor>(false);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private PathfindSetupSystem m_PathfindSetupSystem;

	private EntityQuery m_VehicleQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	public override int GetUpdateOffset(SystemUpdatePhase phase)
	{
		return 12;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_PathfindSetupSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindSetupSystem>();
		m_VehicleQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[10]
		{
			ComponentType.ReadWrite<Game.Vehicles.WorkVehicle>(),
			ComponentType.ReadWrite<CarCurrentLane>(),
			ComponentType.ReadOnly<Owner>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadWrite<PathOwner>(),
			ComponentType.ReadWrite<Target>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<TripSource>(),
			ComponentType.Exclude<OutOfControl>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_VehicleQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<WorkAction> workQueue = default(NativeQueue<WorkAction>);
		workQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		WorkCarTickJob workCarTickJob = new WorkCarTickJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_UnspawnedType = InternalCompilerInterface.GetComponentTypeHandle<Unspawned>(ref __TypeHandle.__Game_Objects_Unspawned_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathInformationType = InternalCompilerInterface.GetComponentTypeHandle<PathInformation>(ref __TypeHandle.__Game_Pathfind_PathInformation_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_LayoutElementType = InternalCompilerInterface.GetBufferTypeHandle<LayoutElement>(ref __TypeHandle.__Game_Vehicles_LayoutElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarType = InternalCompilerInterface.GetComponentTypeHandle<Car>(ref __TypeHandle.__Game_Vehicles_Car_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CurrentLaneType = InternalCompilerInterface.GetComponentTypeHandle<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TargetType = InternalCompilerInterface.GetComponentTypeHandle<Target>(ref __TypeHandle.__Game_Common_Target_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathOwnerType = InternalCompilerInterface.GetComponentTypeHandle<PathOwner>(ref __TypeHandle.__Game_Pathfind_PathOwner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_PathElementType = InternalCompilerInterface.GetBufferTypeHandle<PathElement>(ref __TypeHandle.__Game_Pathfind_PathElement_RW_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_EntityLookup = InternalCompilerInterface.GetEntityStorageInfoLookup(ref __TypeHandle.__EntityStorageInfoLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachmentData = InternalCompilerInterface.GetComponentLookup<Attachment>(ref __TypeHandle.__Game_Objects_Attachment_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PlantData = InternalCompilerInterface.GetComponentLookup<Plant>(ref __TypeHandle.__Game_Objects_Plant_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AreaData = InternalCompilerInterface.GetComponentLookup<Area>(ref __TypeHandle.__Game_Areas_Area_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabCarData = InternalCompilerInterface.GetComponentLookup<CarData>(ref __TypeHandle.__Game_Prefabs_CarData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabWorkVehicleData = InternalCompilerInterface.GetComponentLookup<WorkVehicleData>(ref __TypeHandle.__Game_Prefabs_WorkVehicleData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabTreeData = InternalCompilerInterface.GetComponentLookup<TreeData>(ref __TypeHandle.__Game_Prefabs_TreeData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SlaveLaneData = InternalCompilerInterface.GetComponentLookup<SlaveLane>(ref __TypeHandle.__Game_Net_SlaveLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkVehicleData = InternalCompilerInterface.GetComponentLookup<Game.Vehicles.WorkVehicle>(ref __TypeHandle.__Game_Vehicles_WorkVehicle_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		workCarTickJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val)).AsParallelWriter();
		workCarTickJob.m_PathfindQueue = m_PathfindSetupSystem.GetQueue(this, 64).AsParallelWriter();
		workCarTickJob.m_WorkQueue = workQueue.AsParallelWriter();
		WorkCarTickJob workCarTickJob2 = workCarTickJob;
		WorkCarWorkJob obj = new WorkCarWorkJob
		{
			m_TreeData = InternalCompilerInterface.GetComponentLookup<Tree>(ref __TypeHandle.__Game_Objects_Tree_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorData = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkQueue = workQueue
		};
		JobHandle val2 = JobChunkExtensions.ScheduleParallel<WorkCarTickJob>(workCarTickJob2, m_VehicleQuery, ((SystemBase)this).Dependency);
		JobHandle val3 = IJobExtensions.Schedule<WorkCarWorkJob>(obj, val2);
		workQueue.Dispose(val3);
		m_PathfindSetupSystem.AddQueueWriter(val2);
		m_EndFrameBarrier.AddJobHandleForProducer(val2);
		((SystemBase)this).Dependency = val3;
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
	public WorkCarAISystem()
	{
	}
}
