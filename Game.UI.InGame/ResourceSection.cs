using System;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Objects;
using Game.Prefabs;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ResourceSection : InfoSectionBase
{
	private enum ResourceKey
	{
		Wood
	}

	protected override string group => "ResourceSection";

	private float resourceAmount { get; set; }

	private ResourceKey resourceKey { get; set; }

	protected override bool displayForDestroyedObjects => true;

	protected override void Reset()
	{
		resourceAmount = 0f;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		TreeData treeData = default(TreeData);
		base.visible = ((EntityManager)(ref entityManager)).HasComponent<Tree>(selectedEntity) && EntitiesExtensions.TryGetComponent<TreeData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref treeData) && treeData.m_WoodAmount > 0f;
	}

	protected override void OnProcess()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Tree componentData = ((EntityManager)(ref entityManager)).GetComponentData<Tree>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Plant componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<Plant>(selectedEntity);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		TreeData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<TreeData>(selectedPrefab);
		Damaged damaged = default(Damaged);
		EntitiesExtensions.TryGetComponent<Damaged>(((ComponentSystemBase)this).EntityManager, selectedEntity, ref damaged);
		resourceAmount = math.round(ObjectUtils.CalculateWoodAmount(componentData, componentData2, damaged, componentData3));
		resourceKey = ResourceKey.Wood;
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		writer.PropertyName("resourceAmount");
		writer.Write(resourceAmount);
		writer.PropertyName("resourceKey");
		writer.Write(Enum.GetName(typeof(ResourceKey), resourceKey));
	}

	[Preserve]
	public ResourceSection()
	{
	}
}
