using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class DestroyedBuildingSection : InfoSectionBase
{
	private enum Status
	{
		None,
		Waiting,
		NoService,
		Searching,
		Rebuild
	}

	private ToolSystem m_ToolSystem;

	private DefaultToolSystem m_DefaultToolSystem;

	private UpgradeToolSystem m_UpgradeToolSystem;

	private ValueBinding<bool> m_Rebuilding;

	private EntityQuery m_FireStationQuery;

	private EntityQuery m_ServiceDispatchQuery;

	protected override string group => "DestroyedBuildingSection";

	private Entity destroyer { get; set; }

	private bool cleared { get; set; }

	private float progress { get; set; }

	private Status status { get; set; }

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForUpgrades => true;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		base.OnCreate();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_DefaultToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_UpgradeToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpgradeToolSystem>();
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Combine(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
		m_FireStationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Building>(),
			ComponentType.ReadOnly<Game.Buildings.FireStation>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ServiceDispatchQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Vehicle>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		AddBinding((IBinding)new TriggerBinding(group, "toggleRebuild", (Action)OnToggleRebuild));
		AddBinding((IBinding)(object)(m_Rebuilding = new ValueBinding<bool>(group, "rebuilding", false, (IWriter<bool>)null, (EqualityComparer<bool>)null)));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		ToolSystem toolSystem = m_ToolSystem;
		toolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Remove(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
		base.OnDestroy();
	}

	private void OnToolChanged(ToolBaseSystem tool)
	{
		m_Rebuilding.Update(tool == m_UpgradeToolSystem);
	}

	private void OnToggleRebuild()
	{
		if (m_ToolSystem.activeTool == m_UpgradeToolSystem)
		{
			m_ToolSystem.activeTool = m_DefaultToolSystem;
			return;
		}
		m_UpgradeToolSystem.prefab = null;
		m_ToolSystem.activeTool = m_UpgradeToolSystem;
	}

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		destroyer = Entity.Null;
		status = Status.None;
		cleared = false;
		progress = 0f;
	}

	private bool Visible()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (base.Destroyed)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Building>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Owner>(selectedEntity))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(selectedEntity))
					{
						goto IL_0090;
					}
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(selectedPrefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<PlacedSignatureBuildingData>(selectedPrefab))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						return ((EntityManager)(ref entityManager)).HasComponent<Attached>(selectedEntity);
					}
				}
				return true;
			}
		}
		goto IL_0090;
		IL_0090:
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Destroyed componentData = ((EntityManager)(ref entityManager)).GetComponentData<Destroyed>(selectedEntity);
		PrefabRef prefabRef = default(PrefabRef);
		EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, componentData.m_Event, ref prefabRef);
		destroyer = prefabRef.m_Prefab;
		progress = math.max(0f, componentData.m_Cleared);
		cleared = progress >= 1f;
		if (!cleared)
		{
			NativeArray<PrefabRef> val = ((EntityQuery)(ref m_FireStationQuery)).ToComponentDataArray<PrefabRef>(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<Entity> val2 = ((EntityQuery)(ref m_ServiceDispatchQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			bool flag = false;
			FireStationData fireStationData = default(FireStationData);
			for (int i = 0; i < val.Length; i++)
			{
				if (EntitiesExtensions.TryGetComponent<FireStationData>(((ComponentSystemBase)this).EntityManager, val[i].m_Prefab, ref fireStationData) && fireStationData.m_DisasterResponseCapacity > 0)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				status = Status.NoService;
			}
			else
			{
				FireRescueRequest fireRescueRequest = default(FireRescueRequest);
				for (int j = 0; j < val2.Length; j++)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					DynamicBuffer<ServiceDispatch> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ServiceDispatch>(val2[j], true);
					for (int k = 0; k < buffer.Length; k++)
					{
						if (EntitiesExtensions.TryGetComponent<FireRescueRequest>(((ComponentSystemBase)this).EntityManager, buffer[k].m_Request, ref fireRescueRequest) && fireRescueRequest.m_Type == FireRescueRequestType.Disaster && fireRescueRequest.m_Target == selectedEntity && VehicleAtTarget(val2[j]))
						{
							status = Status.Searching;
							break;
						}
					}
				}
			}
			if (status == Status.None)
			{
				status = Status.Waiting;
			}
			val.Dispose();
			val2.Dispose();
		}
		else
		{
			status = Status.Rebuild;
		}
		if (status != Status.None)
		{
			base.tooltipKeys.Add(status.ToString());
		}
		m_InfoUISystem.tooltipTags.Add(TooltipTags.Destroyed);
	}

	private bool VehicleAtTarget(Entity vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Game.Vehicles.FireEngine fireEngine = default(Game.Vehicles.FireEngine);
		if (EntitiesExtensions.TryGetComponent<Game.Vehicles.FireEngine>(((ComponentSystemBase)this).EntityManager, vehicle, ref fireEngine))
		{
			return (fireEngine.m_State & FireEngineFlags.Rescueing) != 0;
		}
		return false;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("destroyer");
		if (destroyer != Entity.Null)
		{
			PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(destroyer);
			writer.Write(((Object)prefab).name);
		}
		else
		{
			writer.WriteNull();
		}
		writer.PropertyName("progress");
		writer.Write(progress * 100f);
		writer.PropertyName("cleared");
		writer.Write(cleared);
		writer.PropertyName("status");
		writer.Write(status.ToString());
	}

	[Preserve]
	public DestroyedBuildingSection()
	{
	}
}
