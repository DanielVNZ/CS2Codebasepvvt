using System;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.Creatures;
using Game.Net;
using Game.Objects;
using Game.Routes;
using Game.Vehicles;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class TitleSection : InfoSectionBase
{
	private ImageSystem m_ImageSystem;

	protected override string group => "TitleSection";

	protected override bool displayForDestroyedObjects => true;

	protected override bool displayForOutsideConnections => true;

	protected override bool displayForUnderConstruction => true;

	protected override bool displayForUpgrades => true;

	[CanBeNull]
	private string icon { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_ImageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ImageSystem>();
		AddBinding((IBinding)(object)new TriggerBinding<string>(group, "renameEntity", (Action<string>)OnRename, (IReader<string>)null));
	}

	protected override void Reset()
	{
		icon = null;
	}

	private void OnRename(string newName)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		m_NameSystem.SetCustomName(selectedEntity, newName);
		m_InfoUISystem.RequestUpdate();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.visible = selectedEntity != Entity.Null;
	}

	protected override void OnProcess()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		icon = m_ImageSystem.GetInstanceIcon(selectedEntity, selectedPrefab);
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("name");
		m_NameSystem.BindName(writer, selectedEntity);
		writer.PropertyName("vkName");
		m_NameSystem.BindNameForVirtualKeyboard(writer, selectedEntity);
		writer.PropertyName("vkLocaleKey");
		writer.Write(GetVirtualKeyboardLocaleKey(((ComponentSystemBase)this).EntityManager, selectedEntity));
		writer.PropertyName("icon");
		if (icon == null)
		{
			writer.WriteNull();
		}
		else
		{
			writer.Write(icon);
		}
	}

	public static string GetVirtualKeyboardLocaleKey(EntityManager entityManager, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
		{
			return "BuildingName";
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Tree>(entity))
		{
			return "PlantName";
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(entity))
		{
			return "CitizenName";
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Vehicle>(entity))
		{
			return "VehicleName";
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Animal>(entity))
		{
			return "AnimalName";
		}
		if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(entity))
		{
			return "LineName";
		}
		if (((EntityManager)(ref entityManager)).HasComponent<Aggregate>(entity))
		{
			return "RoadName";
		}
		if (((EntityManager)(ref entityManager)).HasComponent<District>(entity))
		{
			return "DistrictName";
		}
		return "ObjectName";
	}

	[Preserve]
	public TitleSection()
	{
	}
}
