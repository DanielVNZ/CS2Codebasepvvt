using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ParkingSection : InfoSectionBase
{
	protected override string group => "ParkingSection";

	private int parkingFee { get; set; }

	private int parkedCars { get; set; }

	private int parkingCapacity { get; set; }

	protected override void Reset()
	{
		parkingFee = 0;
		parkedCars = 0;
		parkingCapacity = 0;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ParkingFacility>(selectedEntity);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		int laneCount = 0;
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref subLanes))
		{
			CheckParkingLanes(subLanes, ref laneCount);
		}
		DynamicBuffer<Game.Net.SubNet> subNets = default(DynamicBuffer<Game.Net.SubNet>);
		if (EntitiesExtensions.TryGetBuffer<Game.Net.SubNet>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref subNets))
		{
			CheckParkingLanes(subNets, ref laneCount);
		}
		DynamicBuffer<Game.Objects.SubObject> subObjects = default(DynamicBuffer<Game.Objects.SubObject>);
		if (EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref subObjects))
		{
			CheckParkingLanes(subObjects, ref laneCount);
		}
		if (laneCount != 0)
		{
			parkingFee /= laneCount;
		}
		if (parkingCapacity < 0)
		{
			parkingCapacity = 0;
		}
	}

	private void CheckParkingLanes(DynamicBuffer<Game.Objects.SubObject> subObjects, ref int laneCount)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		DynamicBuffer<Game.Objects.SubObject> subObjects2 = default(DynamicBuffer<Game.Objects.SubObject>);
		for (int i = 0; i < subObjects.Length; i++)
		{
			Entity subObject = subObjects[i].m_SubObject;
			if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, subObject, true, ref subLanes))
			{
				CheckParkingLanes(subLanes, ref laneCount);
			}
			if (EntitiesExtensions.TryGetBuffer<Game.Objects.SubObject>(((ComponentSystemBase)this).EntityManager, subObject, true, ref subObjects2))
			{
				CheckParkingLanes(subObjects2, ref laneCount);
			}
		}
	}

	private void CheckParkingLanes(DynamicBuffer<Game.Net.SubNet> subNets, ref int laneCount)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<Game.Net.SubLane> subLanes = default(DynamicBuffer<Game.Net.SubLane>);
		for (int i = 0; i < subNets.Length; i++)
		{
			Entity subNet = subNets[i].m_SubNet;
			if (EntitiesExtensions.TryGetBuffer<Game.Net.SubLane>(((ComponentSystemBase)this).EntityManager, subNet, true, ref subLanes))
			{
				CheckParkingLanes(subLanes, ref laneCount);
			}
		}
	}

	private void CheckParkingLanes(DynamicBuffer<Game.Net.SubLane> subLanes, ref int laneCount)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Game.Net.ParkingLane parkingLane = default(Game.Net.ParkingLane);
		GarageLane garageLane = default(GarageLane);
		for (int i = 0; i < subLanes.Length; i++)
		{
			Entity subLane = subLanes[i].m_SubLane;
			if (EntitiesExtensions.TryGetComponent<Game.Net.ParkingLane>(((ComponentSystemBase)this).EntityManager, subLane, ref parkingLane))
			{
				if ((parkingLane.m_Flags & ParkingLaneFlags.VirtualLane) != 0)
				{
					continue;
				}
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(subLane).m_Prefab;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Curve componentData = ((EntityManager)(ref entityManager)).GetComponentData<Curve>(subLane);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<LaneObject> buffer = ((EntityManager)(ref entityManager)).GetBuffer<LaneObject>(subLane, true);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				ParkingLaneData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<ParkingLaneData>(prefab);
				if (componentData2.m_SlotInterval != 0f)
				{
					int parkingSlotCount = NetUtils.GetParkingSlotCount(componentData, parkingLane, componentData2);
					parkingCapacity += parkingSlotCount;
				}
				else
				{
					parkingCapacity = -1000000;
				}
				for (int j = 0; j < buffer.Length; j++)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<ParkedCar>(buffer[j].m_LaneObject))
					{
						parkedCars++;
					}
				}
				parkingFee += parkingLane.m_ParkingFee;
				laneCount++;
			}
			else if (EntitiesExtensions.TryGetComponent<GarageLane>(((ComponentSystemBase)this).EntityManager, subLane, ref garageLane))
			{
				parkingCapacity += garageLane.m_VehicleCapacity;
				parkedCars += garageLane.m_VehicleCount;
				parkingFee += garageLane.m_ParkingFee;
				laneCount++;
			}
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("parkedCars");
		writer.Write(parkedCars);
		writer.PropertyName("parkingCapacity");
		writer.Write(parkingCapacity);
	}

	[Preserve]
	public ParkingSection()
	{
	}
}
