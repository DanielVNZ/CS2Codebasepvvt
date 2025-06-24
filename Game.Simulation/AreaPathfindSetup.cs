using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Simulation;

public struct AreaPathfindSetup
{
	[BurstCompile]
	private struct SetupAreaLocationJob : IJobParallelFor
	{
		[ReadOnly]
		public ComponentLookup<Secondary> m_SecondaryData;

		[ReadOnly]
		public ComponentLookup<CargoTransportStationData> m_CargoTransportStationData;

		[ReadOnly]
		public BufferLookup<Game.Objects.SubObject> m_SubObjects;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(int index)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			m_SetupData.GetItem(index, out var entity, out var targetSeeker);
			Random random = targetSeeker.m_RandomSeed.GetRandom(0);
			if (targetSeeker.m_AreaNode.HasBuffer(entity))
			{
				DynamicBuffer<Game.Areas.SubArea> subAreas = default(DynamicBuffer<Game.Areas.SubArea>);
				m_SubAreas.TryGetBuffer(entity, ref subAreas);
				int num = 0;
				DynamicBuffer<Game.Objects.SubObject> val = default(DynamicBuffer<Game.Objects.SubObject>);
				if (m_SubObjects.TryGetBuffer(entity, ref val))
				{
					for (int i = 0; i < val.Length; i++)
					{
						Entity subObject = val[i].m_SubObject;
						if (!m_SecondaryData.HasComponent(subObject))
						{
							float cost = ((Random)(ref random)).NextFloat(600f);
							num += targetSeeker.AddAreaTargets(ref random, subObject, entity, subObject, subAreas, cost, addDistanceCost: false, EdgeFlags.DefaultMask);
						}
					}
				}
				if (num == 0)
				{
					targetSeeker.m_SetupQueueTarget.m_RandomCost = 600f;
					targetSeeker.AddAreaTargets(ref random, entity, entity, Entity.Null, subAreas, 0f, addDistanceCost: false, EdgeFlags.DefaultMask);
				}
			}
			else
			{
				PrefabRef prefabRef = default(PrefabRef);
				Owner owner = default(Owner);
				DynamicBuffer<InstalledUpgrade> val2 = default(DynamicBuffer<InstalledUpgrade>);
				if (!targetSeeker.m_PrefabRef.TryGetComponent(entity, ref prefabRef) || !targetSeeker.m_Owner.TryGetComponent(entity, ref owner) || !m_CargoTransportStationData.HasComponent(prefabRef.m_Prefab) || !m_InstalledUpgrades.TryGetBuffer(owner.m_Owner, ref val2))
				{
					return;
				}
				CargoTransportStationData cargoTransportStationData = default(CargoTransportStationData);
				for (int j = 0; j < val2.Length; j++)
				{
					InstalledUpgrade installedUpgrade = val2[j];
					if (!(installedUpgrade.m_Upgrade == entity) && !BuildingUtils.CheckOption(installedUpgrade, BuildingOption.Inactive))
					{
						PrefabRef prefabRef2 = targetSeeker.m_PrefabRef[installedUpgrade.m_Upgrade];
						if (m_CargoTransportStationData.TryGetComponent(prefabRef2.m_Prefab, ref cargoTransportStationData) && cargoTransportStationData.m_WorkMultiplier > 0f)
						{
							float cost2 = ((Random)(ref random)).NextFloat(10000f);
							targetSeeker.FindTargets(installedUpgrade.m_Upgrade, installedUpgrade.m_Upgrade, cost2, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
						}
					}
				}
			}
		}
	}

	[BurstCompile]
	private struct SetupWoodResourceJob : IJobParallelFor
	{
		[ReadOnly]
		public ComponentLookup<Tree> m_TreeData;

		[ReadOnly]
		public BufferLookup<WoodResource> m_WoodResources;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> m_SubAreas;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(int index)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0134: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			m_SetupData.GetItem(index, out var entity, out var targetSeeker);
			VehicleWorkType value = (VehicleWorkType)targetSeeker.m_SetupQueueTarget.m_Value;
			if (!m_WoodResources.HasBuffer(entity))
			{
				return;
			}
			DynamicBuffer<WoodResource> val = m_WoodResources[entity];
			Random random = targetSeeker.m_RandomSeed.GetRandom(0);
			DynamicBuffer<Game.Areas.SubArea> subAreas = default(DynamicBuffer<Game.Areas.SubArea>);
			m_SubAreas.TryGetBuffer(entity, ref subAreas);
			for (int i = 0; i < val.Length; i++)
			{
				Entity tree = val[i].m_Tree;
				Tree tree2 = m_TreeData[tree];
				float num = ((Random)(ref random)).NextFloat(15f);
				switch (value)
				{
				case VehicleWorkType.Harvest:
					if ((tree2.m_State & TreeState.Adult) != 0)
					{
						num += (float)(511 - tree2.m_Growth) * (15f / 128f);
						break;
					}
					if ((tree2.m_State & TreeState.Elderly) == 0)
					{
						continue;
					}
					num += (float)(255 - tree2.m_Growth) * (15f / 128f);
					break;
				case VehicleWorkType.Collect:
					if ((tree2.m_State & (TreeState.Stump | TreeState.Collected)) == TreeState.Stump)
					{
						num += (float)(int)tree2.m_Growth * (15f / 128f);
						break;
					}
					if ((tree2.m_State & (TreeState.Teen | TreeState.Adult | TreeState.Elderly | TreeState.Dead | TreeState.Collected)) != 0)
					{
						continue;
					}
					num += (float)(256 + tree2.m_Growth) * (15f / 128f);
					break;
				}
				targetSeeker.AddAreaTargets(ref random, tree, entity, tree, subAreas, num, addDistanceCost: false, EdgeFlags.DefaultMask);
			}
		}
	}

	private ComponentLookup<Tree> m_TreeData;

	private ComponentLookup<Secondary> m_SecondaryData;

	private ComponentLookup<CargoTransportStationData> m_CargoTransportStationData;

	private BufferLookup<Game.Objects.SubObject> m_SubObjects;

	private BufferLookup<WoodResource> m_WoodResources;

	private BufferLookup<Game.Areas.SubArea> m_SubAreas;

	private BufferLookup<InstalledUpgrade> m_InstalledUpgrades;

	public AreaPathfindSetup(PathfindSetupSystem system)
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
		m_TreeData = ((SystemBase)system).GetComponentLookup<Tree>(true);
		m_SecondaryData = ((SystemBase)system).GetComponentLookup<Secondary>(true);
		m_CargoTransportStationData = ((SystemBase)system).GetComponentLookup<CargoTransportStationData>(true);
		m_SubObjects = ((SystemBase)system).GetBufferLookup<Game.Objects.SubObject>(true);
		m_WoodResources = ((SystemBase)system).GetBufferLookup<WoodResource>(true);
		m_SubAreas = ((SystemBase)system).GetBufferLookup<Game.Areas.SubArea>(true);
		m_InstalledUpgrades = ((SystemBase)system).GetBufferLookup<InstalledUpgrade>(true);
	}

	public JobHandle SetupAreaLocation(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		m_SecondaryData.Update((SystemBase)(object)system);
		m_CargoTransportStationData.Update((SystemBase)(object)system);
		m_SubObjects.Update((SystemBase)(object)system);
		m_SubAreas.Update((SystemBase)(object)system);
		m_InstalledUpgrades.Update((SystemBase)(object)system);
		return IJobParallelForExtensions.Schedule<SetupAreaLocationJob>(new SetupAreaLocationJob
		{
			m_SecondaryData = m_SecondaryData,
			m_CargoTransportStationData = m_CargoTransportStationData,
			m_SubObjects = m_SubObjects,
			m_SubAreas = m_SubAreas,
			m_InstalledUpgrades = m_InstalledUpgrades,
			m_SetupData = setupData
		}, setupData.Length, 1, inputDeps);
	}

	public JobHandle SetupWoodResource(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		m_TreeData.Update((SystemBase)(object)system);
		m_WoodResources.Update((SystemBase)(object)system);
		m_SubAreas.Update((SystemBase)(object)system);
		return IJobParallelForExtensions.Schedule<SetupWoodResourceJob>(new SetupWoodResourceJob
		{
			m_TreeData = m_TreeData,
			m_WoodResources = m_WoodResources,
			m_SubAreas = m_SubAreas,
			m_SetupData = setupData
		}, setupData.Length, 1, inputDeps);
	}
}
