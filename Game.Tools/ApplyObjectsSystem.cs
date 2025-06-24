using System.Runtime.CompilerServices;
using Colossal.Collections;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Rendering;
using Game.Routes;
using Game.Simulation;
using Game.Triggers;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ApplyObjectsSystem : GameSystemBase
{
	[BurstCompile]
	private struct PatchTempReferencesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Attached> m_AttachedType;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> m_VehicleType;

		[ReadOnly]
		public ComponentTypeHandle<Creature> m_CreatureType;

		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Attached> m_AttachedData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		public BufferLookup<OwnedVehicle> m_OwnedVehicles;

		public BufferLookup<OwnedCreature> m_OwnedCreatures;

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
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_023f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_0238: Unknown result type (might be due to invalid IL or missing references)
			//IL_0303: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0286: Unknown result type (might be due to invalid IL or missing references)
			//IL_026f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_0328: Unknown result type (might be due to invalid IL or missing references)
			//IL_0311: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_0299: Unknown result type (might be due to invalid IL or missing references)
			//IL_0336: Unknown result type (might be due to invalid IL or missing references)
			//IL_0338: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Owner> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			NativeArray<Attached> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Attached>(ref m_AttachedType);
			Temp temp2 = default(Temp);
			Attached attached2 = default(Attached);
			DynamicBuffer<Game.Objects.SubObject> val2 = default(DynamicBuffer<Game.Objects.SubObject>);
			DynamicBuffer<Game.Objects.SubObject> val3 = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int i = 0; i < nativeArray4.Length; i++)
			{
				Entity subObject = nativeArray[i];
				Attached attached = nativeArray4[i];
				Temp temp = nativeArray2[i];
				if ((temp.m_Flags & (TempFlags.Delete | TempFlags.Cancel)) != 0)
				{
					continue;
				}
				Entity val = attached.m_Parent;
				bool flag = false;
				if (m_TempData.TryGetComponent(attached.m_Parent, ref temp2))
				{
					if (m_PrefabRefData.HasComponent(temp2.m_Original) && (temp2.m_Flags & (TempFlags.Replace | TempFlags.Combine)) == 0)
					{
						val = temp2.m_Original;
					}
					else
					{
						flag |= m_PrefabRefData.HasComponent(temp.m_Original);
					}
				}
				if (m_AttachedData.TryGetComponent(temp.m_Original, ref attached2))
				{
					if (attached2.m_Parent != val && m_SubObjects.TryGetBuffer(attached2.m_Parent, ref val2))
					{
						CollectionUtils.RemoveValue<Game.Objects.SubObject>(val2, new Game.Objects.SubObject(temp.m_Original));
					}
				}
				else
				{
					flag |= attached.m_Parent != val;
				}
				if (flag && m_SubObjects.TryGetBuffer(attached.m_Parent, ref val3))
				{
					CollectionUtils.RemoveValue<Game.Objects.SubObject>(val3, new Game.Objects.SubObject(subObject));
				}
			}
			bool flag2 = ((ArchetypeChunk)(ref chunk)).Has<Vehicle>(ref m_VehicleType);
			bool flag3 = ((ArchetypeChunk)(ref chunk)).Has<Creature>(ref m_CreatureType);
			DynamicBuffer<OwnedVehicle> val5 = default(DynamicBuffer<OwnedVehicle>);
			DynamicBuffer<OwnedCreature> val6 = default(DynamicBuffer<OwnedCreature>);
			DynamicBuffer<Game.Objects.SubObject> val7 = default(DynamicBuffer<Game.Objects.SubObject>);
			for (int j = 0; j < nativeArray3.Length; j++)
			{
				Entity val4 = nativeArray[j];
				Owner owner = nativeArray3[j];
				Temp temp3 = nativeArray2[j];
				if (!(temp3.m_Original == Entity.Null) || (temp3.m_Flags & TempFlags.Delete) != 0)
				{
					continue;
				}
				Owner owner2 = owner;
				if (m_TempData.HasComponent(owner.m_Owner))
				{
					Temp temp4 = m_TempData[owner.m_Owner];
					if (temp4.m_Original != Entity.Null && (temp4.m_Flags & TempFlags.Replace) == 0)
					{
						owner2.m_Owner = temp4.m_Original;
					}
				}
				if (!(owner2.m_Owner != owner.m_Owner))
				{
					continue;
				}
				if (flag2)
				{
					if (m_OwnedVehicles.TryGetBuffer(owner.m_Owner, ref val5))
					{
						CollectionUtils.RemoveValue<OwnedVehicle>(val5, new OwnedVehicle(val4));
					}
					if (m_OwnedVehicles.TryGetBuffer(owner2.m_Owner, ref val5))
					{
						CollectionUtils.TryAddUniqueValue<OwnedVehicle>(val5, new OwnedVehicle(val4));
					}
				}
				else if (flag3)
				{
					if (m_OwnedCreatures.TryGetBuffer(owner.m_Owner, ref val6))
					{
						CollectionUtils.RemoveValue<OwnedCreature>(val6, new OwnedCreature(val4));
					}
					if (m_OwnedCreatures.TryGetBuffer(owner2.m_Owner, ref val6))
					{
						CollectionUtils.TryAddUniqueValue<OwnedCreature>(val6, new OwnedCreature(val4));
					}
				}
				else
				{
					if (m_SubObjects.TryGetBuffer(owner.m_Owner, ref val7))
					{
						CollectionUtils.RemoveValue<Game.Objects.SubObject>(val7, new Game.Objects.SubObject(val4));
					}
					if (m_SubObjects.TryGetBuffer(owner2.m_Owner, ref val7))
					{
						CollectionUtils.TryAddUniqueValue<Game.Objects.SubObject>(val7, new Game.Objects.SubObject(val4));
					}
				}
				nativeArray3[j] = owner2;
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct HandleTempEntitiesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Temp> m_TempType;

		[ReadOnly]
		public ComponentTypeHandle<Transform> m_TransformType;

		[ReadOnly]
		public ComponentTypeHandle<Attached> m_AttachedType;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> m_CarCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> m_TrainCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> m_WatercraftCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> m_AircraftCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<ParkedCar> m_ParkedCarData;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> m_ParkedTrainData;

		[ReadOnly]
		public ComponentLookup<Controller> m_ControllerData;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> m_HumanCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<AnimalCurrentLane> m_AnimalCurrentLaneData;

		[ReadOnly]
		public ComponentLookup<Building> m_BuildingData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> m_ServiceUpgradeData;

		[ReadOnly]
		public ComponentLookup<RescueTarget> m_RescueTargetData;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> m_TransportStopData;

		[ReadOnly]
		public ComponentLookup<Hidden> m_HiddenData;

		[ReadOnly]
		public ComponentLookup<Temp> m_TempData;

		[ReadOnly]
		public ComponentLookup<Recent> m_RecentData;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> m_ElevationData;

		[ReadOnly]
		public ComponentLookup<Transform> m_TransformData;

		[ReadOnly]
		public ComponentLookup<Owner> m_OwnerData;

		[ReadOnly]
		public ComponentLookup<Destroyed> m_DestroyedData;

		[ReadOnly]
		public ComponentLookup<Damaged> m_DamagedData;

		[ReadOnly]
		public ComponentLookup<Static> m_StaticData;

		[ReadOnly]
		public ComponentLookup<Stopped> m_StoppedData;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> m_LocalTransformCacheData;

		[ReadOnly]
		public ComponentLookup<Swaying> m_SwayingData;

		[ReadOnly]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> m_SubNets;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> m_SubLanes;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		[ReadOnly]
		public BufferLookup<MeshColor> m_MeshColors;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public uint m_SimulationFrame;

		[ReadOnly]
		public EconomyParameterData m_EconomyParameterData;

		[ReadOnly]
		public bool m_EditorMode;

		[ReadOnly]
		public EntityArchetype m_PathTargetEventArchetype;

		[ReadOnly]
		public ComponentTypeSet m_AppliedTypes;

		[ReadOnly]
		public ComponentTypeSet m_TempAnimationTypes;

		[ReadOnly]
		public NativeParallelHashMap<Entity, int> m_InstanceCounts;

		public ParallelWriter m_CommandBuffer;

		public ParallelWriter<TriggerAction> m_TriggerBuffer;

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
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0330: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_033f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_03df: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0402: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0381: Unknown result type (might be due to invalid IL or missing references)
			//IL_0395: Unknown result type (might be due to invalid IL or missing references)
			//IL_025b: Unknown result type (might be due to invalid IL or missing references)
			//IL_025f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_0272: Unknown result type (might be due to invalid IL or missing references)
			//IL_0276: Unknown result type (might be due to invalid IL or missing references)
			//IL_027c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0289: Unknown result type (might be due to invalid IL or missing references)
			//IL_028d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_017e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0414: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_02da: Unknown result type (might be due to invalid IL or missing references)
			//IL_02df: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Unknown result type (might be due to invalid IL or missing references)
			//IL_042b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0432: Unknown result type (might be due to invalid IL or missing references)
			//IL_0437: Unknown result type (might be due to invalid IL or missing references)
			//IL_0439: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0313: Unknown result type (might be due to invalid IL or missing references)
			//IL_0305: Unknown result type (might be due to invalid IL or missing references)
			//IL_030a: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Temp> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Temp>(ref m_TempType);
			NativeArray<Transform> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Transform>(ref m_TransformType);
			NativeArray<Attached> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Attached>(ref m_AttachedType);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity val = nativeArray[i];
				Temp temp = nativeArray2[i];
				if ((temp.m_Flags & TempFlags.Cancel) != 0)
				{
					Cancel(unfilteredChunkIndex, val, temp);
					continue;
				}
				if ((temp.m_Flags & TempFlags.Delete) != 0)
				{
					if (m_ParkedCarData.HasComponent(val) && !m_ParkedCarData.HasComponent(temp.m_Original))
					{
						Cancel(unfilteredChunkIndex, val, temp);
					}
					else
					{
						Delete(unfilteredChunkIndex, val, temp);
					}
					continue;
				}
				if (m_PrefabRefData.HasComponent(temp.m_Original))
				{
					if (m_ParkedCarData.HasComponent(val) || m_ParkedTrainData.HasComponent(val))
					{
						if (!m_ParkedCarData.HasComponent(temp.m_Original) && !m_ParkedTrainData.HasComponent(temp.m_Original))
						{
							Cancel(unfilteredChunkIndex, val, temp);
							continue;
						}
						FixParkingLocation(unfilteredChunkIndex, temp.m_Original);
					}
					if (nativeArray4.Length != 0)
					{
						Attached data = nativeArray4[i];
						if (m_TempData.HasComponent(data.m_Parent))
						{
							Temp temp2 = m_TempData[data.m_Parent];
							if (m_PrefabRefData.HasComponent(temp2.m_Original) && (temp2.m_Flags & (TempFlags.Replace | TempFlags.Combine)) == 0)
							{
								data.m_Parent = temp2.m_Original;
							}
						}
						CopyToOriginal<Attached>(unfilteredChunkIndex, temp, data);
					}
					if ((temp.m_Flags & TempFlags.Upgrade) != 0)
					{
						UpdateComponent<PrefabRef>(unfilteredChunkIndex, val, temp.m_Original, m_PrefabRefData, updateValue: true);
						UpdateComponent<Destroyed>(unfilteredChunkIndex, val, temp.m_Original, m_DestroyedData, updateValue: false);
						UpdateComponent<Damaged>(unfilteredChunkIndex, val, temp.m_Original, m_DamagedData, updateValue: false);
						UpdateBuffer<MeshColor>(unfilteredChunkIndex, val, temp.m_Original, m_MeshColors, out DynamicBuffer<MeshColor> _, updateValue: false);
						if (!m_DestroyedData.HasComponent(val) && m_RescueTargetData.HasComponent(temp.m_Original))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<RescueTarget>(unfilteredChunkIndex, temp.m_Original);
						}
					}
					UpdateComponent<Game.Objects.Elevation>(unfilteredChunkIndex, val, temp.m_Original, m_ElevationData, updateValue: true);
					UpdateComponent<LocalTransformCache>(unfilteredChunkIndex, val, temp.m_Original, m_LocalTransformCacheData, updateValue: true);
					UpdateBuffer<Game.Net.SubNet>(unfilteredChunkIndex, val, temp.m_Original, m_SubNets, out DynamicBuffer<Game.Net.SubNet> _, updateValue: false);
					UpdateBuffer<Game.Areas.SubArea>(unfilteredChunkIndex, val, temp.m_Original, m_SubAreas, out DynamicBuffer<Game.Areas.SubArea> _, updateValue: false);
					if (UpdateBuffer<Game.Net.SubLane>(unfilteredChunkIndex, val, temp.m_Original, m_SubLanes, out DynamicBuffer<Game.Net.SubLane> oldBuffer4, updateValue: false))
					{
						RemoveOldSubItems(unfilteredChunkIndex, temp.m_Original, oldBuffer4);
					}
					if (UpdateBuffer<Game.Objects.SubObject>(unfilteredChunkIndex, val, temp.m_Original, m_SubObjects, out DynamicBuffer<Game.Objects.SubObject> oldBuffer5, updateValue: false))
					{
						RemoveOldSubItems(unfilteredChunkIndex, temp.m_Original, oldBuffer5);
					}
					Update(unfilteredChunkIndex, val, temp, nativeArray3[i]);
					continue;
				}
				if (m_ParkedCarData.HasComponent(val) || m_ParkedTrainData.HasComponent(val))
				{
					FixParkingLocation(unfilteredChunkIndex, val);
				}
				if (nativeArray4.Length != 0)
				{
					Attached attached = nativeArray4[i];
					if (m_TempData.HasComponent(attached.m_Parent))
					{
						Temp temp3 = m_TempData[attached.m_Parent];
						if (m_PrefabRefData.HasComponent(temp3.m_Original) && (temp3.m_Flags & (TempFlags.Replace | TempFlags.Combine)) == 0)
						{
							attached.m_Parent = temp3.m_Original;
						}
					}
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Attached>(unfilteredChunkIndex, val, attached);
				}
				Create(unfilteredChunkIndex, val, temp);
				if (m_PrefabRefData.HasComponent(val))
				{
					Entity prefab = m_PrefabRefData[val].m_Prefab;
					int num = (m_InstanceCounts.ContainsKey(prefab) ? m_InstanceCounts[prefab] : 0);
					m_TriggerBuffer.Enqueue(new TriggerAction(TriggerType.ObjectCreated, m_PrefabRefData[val].m_Prefab, val, val, num));
				}
			}
		}

		private void FixParkingLocation(int chunkIndex, Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Controller controller = default(Controller);
			if (!m_ControllerData.TryGetComponent(entity, ref controller) || !(controller.m_Controller != entity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<FixParkingLocation>(chunkIndex, entity, new FixParkingLocation
				{
					m_ResetLocation = entity
				});
			}
		}

		private void RemoveOldSubItems(int chunkIndex, Entity original, DynamicBuffer<Game.Net.SubLane> items)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < items.Length; i++)
			{
				Entity subLane = items[i].m_SubLane;
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, subLane, default(Deleted));
			}
		}

		private void RemoveOldSubItems(int chunkIndex, Entity original, DynamicBuffer<Game.Objects.SubObject> items)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			Owner owner = default(Owner);
			for (int i = 0; i < items.Length; i++)
			{
				Entity subObject = items[i].m_SubObject;
				if (m_OwnerData.TryGetComponent(subObject, ref owner) && owner.m_Owner == original)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, subObject, default(Deleted));
				}
			}
		}

		private void Cancel(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			if (m_HiddenData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<BatchesUpdated>(chunkIndex, temp.m_Original, default(BatchesUpdated));
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(chunkIndex, temp.m_Original);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void Delete(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			if (m_PrefabRefData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, temp.m_Original, default(Deleted));
				if (m_SubNets.HasBuffer(temp.m_Original))
				{
					DynamicBuffer<Game.Net.SubNet> val = m_SubNets[temp.m_Original];
					for (int i = 0; i < val.Length; i++)
					{
						Entity subNet = val[i].m_SubNet;
						if (!m_HiddenData.HasComponent(subNet))
						{
							((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Owner>(chunkIndex, subNet);
							((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(chunkIndex, subNet, default(Updated));
						}
					}
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
		}

		private void UpdateComponent<T>(int chunkIndex, Entity entity, Entity original, ComponentLookup<T> data, bool updateValue) where T : unmanaged, IComponentData
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (data.HasComponent(entity))
			{
				if (data.HasComponent(original))
				{
					if (updateValue)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<T>(chunkIndex, original, data[entity]);
					}
				}
				else if (updateValue)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<T>(chunkIndex, original, data[entity]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<T>(chunkIndex, original, default(T));
				}
			}
			else if (data.HasComponent(original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<T>(chunkIndex, original);
			}
		}

		private bool UpdateBuffer<T>(int chunkIndex, Entity entity, Entity original, BufferLookup<T> data, out DynamicBuffer<T> oldBuffer, bool updateValue) where T : unmanaged, IBufferElementData
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (data.HasBuffer(entity))
			{
				if (data.HasBuffer(original))
				{
					if (updateValue)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetBuffer<T>(chunkIndex, original).CopyFrom(data[entity]);
					}
				}
				else if (updateValue)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<T>(chunkIndex, original).CopyFrom(data[entity]);
				}
				else
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddBuffer<T>(chunkIndex, original);
				}
			}
			else if (data.TryGetBuffer(original, ref oldBuffer))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<T>(chunkIndex, original);
				return true;
			}
			oldBuffer = default(DynamicBuffer<T>);
			return false;
		}

		private void Update(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			if (m_HiddenData.HasComponent(temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Hidden>(chunkIndex, temp.m_Original);
			}
			if (temp.m_Cost != 0)
			{
				Recent recent = new Recent
				{
					m_ModificationFrame = m_SimulationFrame,
					m_ModificationCost = temp.m_Cost
				};
				Recent recent2 = default(Recent);
				if (m_RecentData.TryGetComponent(temp.m_Original, ref recent2))
				{
					recent.m_ModificationCost += recent2.m_ModificationCost;
					recent.m_ModificationCost += ObjectUtils.GetRefundAmount(recent2, m_SimulationFrame, m_EconomyParameterData);
					recent2.m_ModificationFrame = m_SimulationFrame;
					recent.m_ModificationCost -= ObjectUtils.GetRefundAmount(recent2, m_SimulationFrame, m_EconomyParameterData);
					recent.m_ModificationCost = math.min(recent.m_ModificationCost, temp.m_Value);
					if (recent.m_ModificationCost > 0)
					{
						((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Recent>(chunkIndex, temp.m_Original, recent);
					}
					else
					{
						((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<Recent>(chunkIndex, temp.m_Original);
					}
				}
				else if (recent.m_ModificationCost > 0)
				{
					((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Recent>(chunkIndex, temp.m_Original, recent);
				}
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Updated>(chunkIndex, temp.m_Original, default(Updated));
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Deleted>(chunkIndex, entity, default(Deleted));
			if (m_EditorMode && ShouldSaveInstance(entity, temp.m_Original))
			{
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<SaveInstance>(chunkIndex, temp.m_Original, default(SaveInstance));
			}
		}

		private bool ShouldSaveInstance(Entity temp, Entity original)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (m_OwnerData.HasComponent(original))
			{
				if (!m_ServiceUpgradeData.HasComponent(original))
				{
					return false;
				}
				return true;
			}
			DynamicBuffer<InstalledUpgrade> val = default(DynamicBuffer<InstalledUpgrade>);
			if (m_InstalledUpgrades.TryGetBuffer(original, ref val))
			{
				if (val.Length != 0)
				{
					return false;
				}
				if (m_InstalledUpgrades.TryGetBuffer(temp, ref val) && val.Length != 0)
				{
					return false;
				}
			}
			return true;
		}

		private void Update(int chunkIndex, Entity entity, Temp temp, Transform transform)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_029e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02db: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_020c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0220: Unknown result type (might be due to invalid IL or missing references)
			//IL_024f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0284: Unknown result type (might be due to invalid IL or missing references)
			Transform transform2 = m_TransformData[temp.m_Original];
			if (!transform2.Equals(transform))
			{
				((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Transform>(chunkIndex, temp.m_Original, transform);
				if (m_CarCurrentLaneData.HasComponent(temp.m_Original))
				{
					CarCurrentLane carCurrentLane = m_CarCurrentLaneData[temp.m_Original];
					carCurrentLane.m_LaneFlags |= Game.Vehicles.CarLaneFlags.Obsolete;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<CarCurrentLane>(chunkIndex, temp.m_Original, carCurrentLane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Moving>(chunkIndex, temp.m_Original, default(Moving));
				}
				else if (m_TrainCurrentLaneData.HasComponent(temp.m_Original))
				{
					TrainCurrentLane trainCurrentLane = m_TrainCurrentLaneData[temp.m_Original];
					trainCurrentLane.m_Front.m_LaneFlags |= TrainLaneFlags.Obsolete;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<TrainCurrentLane>(chunkIndex, temp.m_Original, trainCurrentLane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Moving>(chunkIndex, temp.m_Original, default(Moving));
				}
				else if (m_WatercraftCurrentLaneData.HasComponent(temp.m_Original))
				{
					WatercraftCurrentLane watercraftCurrentLane = m_WatercraftCurrentLaneData[temp.m_Original];
					watercraftCurrentLane.m_LaneFlags |= WatercraftLaneFlags.Obsolete;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<WatercraftCurrentLane>(chunkIndex, temp.m_Original, watercraftCurrentLane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Moving>(chunkIndex, temp.m_Original, default(Moving));
				}
				else if (m_AircraftCurrentLaneData.HasComponent(temp.m_Original))
				{
					AircraftCurrentLane aircraftCurrentLane = m_AircraftCurrentLaneData[temp.m_Original];
					aircraftCurrentLane.m_LaneFlags |= AircraftLaneFlags.Obsolete;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AircraftCurrentLane>(chunkIndex, temp.m_Original, aircraftCurrentLane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Moving>(chunkIndex, temp.m_Original, default(Moving));
				}
				else if (m_HumanCurrentLaneData.HasComponent(temp.m_Original))
				{
					HumanCurrentLane humanCurrentLane = m_HumanCurrentLaneData[temp.m_Original];
					humanCurrentLane.m_Flags |= CreatureLaneFlags.Obsolete;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<HumanCurrentLane>(chunkIndex, temp.m_Original, humanCurrentLane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Moving>(chunkIndex, temp.m_Original, default(Moving));
				}
				else if (m_AnimalCurrentLaneData.HasComponent(temp.m_Original))
				{
					AnimalCurrentLane animalCurrentLane = m_AnimalCurrentLaneData[temp.m_Original];
					animalCurrentLane.m_Flags |= CreatureLaneFlags.Obsolete;
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<AnimalCurrentLane>(chunkIndex, temp.m_Original, animalCurrentLane);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<Moving>(chunkIndex, temp.m_Original, default(Moving));
				}
				if (m_BuildingData.HasComponent(temp.m_Original) || m_TransportStopData.HasComponent(temp.m_Original))
				{
					Entity val = ((ParallelWriter)(ref m_CommandBuffer)).CreateEntity(chunkIndex, m_PathTargetEventArchetype);
					((ParallelWriter)(ref m_CommandBuffer)).SetComponent<PathTargetMoved>(chunkIndex, val, new PathTargetMoved(temp.m_Original, transform2.m_Position, transform.m_Position));
				}
			}
			Update(chunkIndex, entity, temp);
		}

		private void CopyToOriginal<T>(int chunkIndex, Temp temp, T data) where T : unmanaged, IComponentData
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).SetComponent<T>(chunkIndex, temp.m_Original, data);
		}

		private void Create(int chunkIndex, Entity entity, Temp temp)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent(chunkIndex, entity, ref m_TempAnimationTypes);
			if ((m_StaticData.HasComponent(entity) && !m_SwayingData.HasComponent(entity)) || m_StoppedData.HasComponent(entity))
			{
				((ParallelWriter)(ref m_CommandBuffer)).RemoveComponent<InterpolatedTransform>(chunkIndex, entity);
			}
			if (temp.m_Cost > 0)
			{
				Recent recent = new Recent
				{
					m_ModificationFrame = m_SimulationFrame,
					m_ModificationCost = temp.m_Cost
				};
				((ParallelWriter)(ref m_CommandBuffer)).AddComponent<Recent>(chunkIndex, entity, recent);
			}
			((ParallelWriter)(ref m_CommandBuffer)).AddComponent(chunkIndex, entity, ref m_AppliedTypes);
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
		public ComponentTypeHandle<Temp> __Game_Tools_Temp_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Attached> __Game_Objects_Attached_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Vehicle> __Game_Vehicles_Vehicle_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Creature> __Game_Creatures_Creature_RO_ComponentTypeHandle;

		public ComponentTypeHandle<Owner> __Game_Common_Owner_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Temp> __Game_Tools_Temp_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RW_BufferLookup;

		public BufferLookup<OwnedVehicle> __Game_Vehicles_OwnedVehicle_RW_BufferLookup;

		public BufferLookup<OwnedCreature> __Game_Creatures_OwnedCreature_RW_BufferLookup;

		[ReadOnly]
		public ComponentTypeHandle<Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<CarCurrentLane> __Game_Vehicles_CarCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TrainCurrentLane> __Game_Vehicles_TrainCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WatercraftCurrentLane> __Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AircraftCurrentLane> __Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedCar> __Game_Vehicles_ParkedCar_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ParkedTrain> __Game_Vehicles_ParkedTrain_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Controller> __Game_Vehicles_Controller_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HumanCurrentLane> __Game_Creatures_HumanCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AnimalCurrentLane> __Game_Creatures_AnimalCurrentLane_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.ServiceUpgrade> __Game_Buildings_ServiceUpgrade_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RescueTarget> __Game_Buildings_RescueTarget_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Routes.TransportStop> __Game_Routes_TransportStop_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Hidden> __Game_Tools_Hidden_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Recent> __Game_Tools_Recent_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Destroyed> __Game_Common_Destroyed_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Damaged> __Game_Objects_Damaged_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Static> __Game_Objects_Static_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Stopped> __Game_Objects_Stopped_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LocalTransformCache> __Game_Tools_LocalTransformCache_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Swaying> __Game_Rendering_Swaying_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubNet> __Game_Net_SubNet_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Net.SubLane> __Game_Net_SubLane_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<MeshColor> __Game_Rendering_MeshColor_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> __Game_Objects_SubObject_RO_BufferLookup;

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
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0202: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0224: Unknown result type (might be due to invalid IL or missing references)
			//IL_0229: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Tools_Temp_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Temp>(true);
			__Game_Objects_Attached_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Attached>(true);
			__Game_Vehicles_Vehicle_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Vehicle>(true);
			__Game_Creatures_Creature_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Creature>(true);
			__Game_Common_Owner_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Owner>(false);
			__Game_Tools_Temp_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Temp>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Objects_SubObject_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(false);
			__Game_Vehicles_OwnedVehicle_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedVehicle>(false);
			__Game_Creatures_OwnedCreature_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<OwnedCreature>(false);
			__Game_Objects_Transform_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Transform>(true);
			__Game_Vehicles_CarCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CarCurrentLane>(true);
			__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TrainCurrentLane>(true);
			__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WatercraftCurrentLane>(true);
			__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AircraftCurrentLane>(true);
			__Game_Vehicles_ParkedCar_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedCar>(true);
			__Game_Vehicles_ParkedTrain_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ParkedTrain>(true);
			__Game_Vehicles_Controller_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Controller>(true);
			__Game_Creatures_HumanCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HumanCurrentLane>(true);
			__Game_Creatures_AnimalCurrentLane_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AnimalCurrentLane>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_ServiceUpgrade_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Buildings.ServiceUpgrade>(true);
			__Game_Buildings_RescueTarget_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RescueTarget>(true);
			__Game_Routes_TransportStop_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Routes.TransportStop>(true);
			__Game_Tools_Hidden_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Hidden>(true);
			__Game_Tools_Recent_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Recent>(true);
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Common_Destroyed_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Destroyed>(true);
			__Game_Objects_Damaged_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Damaged>(true);
			__Game_Objects_Static_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Static>(true);
			__Game_Objects_Stopped_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Stopped>(true);
			__Game_Tools_LocalTransformCache_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LocalTransformCache>(true);
			__Game_Rendering_Swaying_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Swaying>(true);
			__Game_Net_SubNet_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubNet>(true);
			__Game_Net_SubLane_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Net.SubLane>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Rendering_MeshColor_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<MeshColor>(true);
			__Game_Objects_SubObject_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Objects.SubObject>(true);
		}
	}

	private ToolOutputBarrier m_ToolOutputBarrier;

	private ToolSystem m_ToolSystem;

	private SimulationSystem m_SimulationSystem;

	private TriggerSystem m_TriggerSystem;

	private InstanceCountSystem m_InstanceCountSystem;

	private EntityQuery m_TempQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityArchetype m_PathTargetEventArchetype;

	private ComponentTypeSet m_AppliedTypes;

	private ComponentTypeSet m_TempAnimationTypes;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ToolOutputBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolOutputBarrier>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TriggerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TriggerSystem>();
		m_InstanceCountSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<InstanceCountSystem>();
		m_TempQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Object>()
		});
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PathTargetEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Event>(),
			ComponentType.ReadWrite<PathTargetMoved>()
		});
		m_AppliedTypes = new ComponentTypeSet(ComponentType.ReadWrite<Applied>(), ComponentType.ReadWrite<Created>(), ComponentType.ReadWrite<Updated>());
		m_TempAnimationTypes = new ComponentTypeSet(ComponentType.ReadWrite<Temp>(), ComponentType.ReadWrite<Animation>(), ComponentType.ReadWrite<BackSide>());
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		((ComponentSystemBase)this).RequireForUpdate(m_TempQuery);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0606: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		PatchTempReferencesJob patchTempReferencesJob = new PatchTempReferencesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedType = InternalCompilerInterface.GetComponentTypeHandle<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_VehicleType = InternalCompilerInterface.GetComponentTypeHandle<Vehicle>(ref __TypeHandle.__Game_Vehicles_Vehicle_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CreatureType = InternalCompilerInterface.GetComponentTypeHandle<Creature>(ref __TypeHandle.__Game_Creatures_Creature_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerType = InternalCompilerInterface.GetComponentTypeHandle<Owner>(ref __TypeHandle.__Game_Common_Owner_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedData = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedVehicles = InternalCompilerInterface.GetBufferLookup<OwnedVehicle>(ref __TypeHandle.__Game_Vehicles_OwnedVehicle_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnedCreatures = InternalCompilerInterface.GetBufferLookup<OwnedCreature>(ref __TypeHandle.__Game_Creatures_OwnedCreature_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
		};
		NativeQueue<TriggerAction> val = (((ComponentSystemBase)m_TriggerSystem).Enabled ? m_TriggerSystem.CreateActionBuffer() : new NativeQueue<TriggerAction>(AllocatorHandle.op_Implicit((Allocator)3)));
		JobHandle dependencies;
		HandleTempEntitiesJob handleTempEntitiesJob = new HandleTempEntitiesJob
		{
			m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TempType = InternalCompilerInterface.GetComponentTypeHandle<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_TransformType = InternalCompilerInterface.GetComponentTypeHandle<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_AttachedType = InternalCompilerInterface.GetComponentTypeHandle<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
			m_CarCurrentLaneData = InternalCompilerInterface.GetComponentLookup<CarCurrentLane>(ref __TypeHandle.__Game_Vehicles_CarCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TrainCurrentLaneData = InternalCompilerInterface.GetComponentLookup<TrainCurrentLane>(ref __TypeHandle.__Game_Vehicles_TrainCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WatercraftCurrentLaneData = InternalCompilerInterface.GetComponentLookup<WatercraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_WatercraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AircraftCurrentLaneData = InternalCompilerInterface.GetComponentLookup<AircraftCurrentLane>(ref __TypeHandle.__Game_Vehicles_AircraftCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedCarData = InternalCompilerInterface.GetComponentLookup<ParkedCar>(ref __TypeHandle.__Game_Vehicles_ParkedCar_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ParkedTrainData = InternalCompilerInterface.GetComponentLookup<ParkedTrain>(ref __TypeHandle.__Game_Vehicles_ParkedTrain_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ControllerData = InternalCompilerInterface.GetComponentLookup<Controller>(ref __TypeHandle.__Game_Vehicles_Controller_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HumanCurrentLaneData = InternalCompilerInterface.GetComponentLookup<HumanCurrentLane>(ref __TypeHandle.__Game_Creatures_HumanCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_AnimalCurrentLaneData = InternalCompilerInterface.GetComponentLookup<AnimalCurrentLane>(ref __TypeHandle.__Game_Creatures_AnimalCurrentLane_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_BuildingData = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ServiceUpgradeData = InternalCompilerInterface.GetComponentLookup<Game.Buildings.ServiceUpgrade>(ref __TypeHandle.__Game_Buildings_ServiceUpgrade_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RescueTargetData = InternalCompilerInterface.GetComponentLookup<RescueTarget>(ref __TypeHandle.__Game_Buildings_RescueTarget_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransportStopData = InternalCompilerInterface.GetComponentLookup<Game.Routes.TransportStop>(ref __TypeHandle.__Game_Routes_TransportStop_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_HiddenData = InternalCompilerInterface.GetComponentLookup<Hidden>(ref __TypeHandle.__Game_Tools_Hidden_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TempData = InternalCompilerInterface.GetComponentLookup<Temp>(ref __TypeHandle.__Game_Tools_Temp_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_RecentData = InternalCompilerInterface.GetComponentLookup<Recent>(ref __TypeHandle.__Game_Tools_Recent_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DestroyedData = InternalCompilerInterface.GetComponentLookup<Destroyed>(ref __TypeHandle.__Game_Common_Destroyed_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_DamagedData = InternalCompilerInterface.GetComponentLookup<Damaged>(ref __TypeHandle.__Game_Objects_Damaged_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StaticData = InternalCompilerInterface.GetComponentLookup<Static>(ref __TypeHandle.__Game_Objects_Static_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_StoppedData = InternalCompilerInterface.GetComponentLookup<Stopped>(ref __TypeHandle.__Game_Objects_Stopped_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_LocalTransformCacheData = InternalCompilerInterface.GetComponentLookup<LocalTransformCache>(ref __TypeHandle.__Game_Tools_LocalTransformCache_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SwayingData = InternalCompilerInterface.GetComponentLookup<Swaying>(ref __TypeHandle.__Game_Rendering_Swaying_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubNets = InternalCompilerInterface.GetBufferLookup<Game.Net.SubNet>(ref __TypeHandle.__Game_Net_SubNet_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubLanes = InternalCompilerInterface.GetBufferLookup<Game.Net.SubLane>(ref __TypeHandle.__Game_Net_SubLane_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_MeshColors = InternalCompilerInterface.GetBufferLookup<MeshColor>(ref __TypeHandle.__Game_Rendering_MeshColor_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubObjects = InternalCompilerInterface.GetBufferLookup<Game.Objects.SubObject>(ref __TypeHandle.__Game_Objects_SubObject_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SimulationFrame = m_SimulationSystem.frameIndex,
			m_EconomyParameterData = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_EditorMode = m_ToolSystem.actionMode.IsEditor(),
			m_PathTargetEventArchetype = m_PathTargetEventArchetype,
			m_AppliedTypes = m_AppliedTypes,
			m_TempAnimationTypes = m_TempAnimationTypes,
			m_InstanceCounts = m_InstanceCountSystem.GetInstanceCounts(readOnly: true, out dependencies)
		};
		EntityCommandBuffer val2 = m_ToolOutputBarrier.CreateCommandBuffer();
		handleTempEntitiesJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val2)).AsParallelWriter();
		handleTempEntitiesJob.m_TriggerBuffer = val.AsParallelWriter();
		HandleTempEntitiesJob handleTempEntitiesJob2 = handleTempEntitiesJob;
		JobHandle val3 = JobChunkExtensions.Schedule<PatchTempReferencesJob>(patchTempReferencesJob, m_TempQuery, ((SystemBase)this).Dependency);
		JobHandle val4 = JobChunkExtensions.ScheduleParallel<HandleTempEntitiesJob>(handleTempEntitiesJob2, m_TempQuery, JobHandle.CombineDependencies(val3, dependencies));
		((EntityCommandBufferSystem)m_ToolOutputBarrier).AddJobHandleForProducer(val4);
		if (((ComponentSystemBase)m_TriggerSystem).Enabled)
		{
			m_TriggerSystem.AddActionBufferWriter(val4);
		}
		else
		{
			val.Dispose(val4);
		}
		m_InstanceCountSystem.AddCountReader(val4);
		((SystemBase)this).Dependency = val4;
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
	public ApplyObjectsSystem()
	{
	}
}
