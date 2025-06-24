using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class DestroyedTreeSection : InfoSectionBase
{
	protected override string group => "DestroyedTreeSection";

	private Entity destroyer { get; set; }

	protected override bool displayForDestroyedObjects => true;

	protected override void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		destroyer = Entity.Null;
	}

	private bool Visible()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (base.Destroyed)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			return ((EntityManager)(ref entityManager)).HasComponent<Tree>(selectedEntity);
		}
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
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		Destroyed componentData = ((EntityManager)(ref entityManager)).GetComponentData<Destroyed>(selectedEntity);
		PrefabRef prefabRef = default(PrefabRef);
		EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, componentData.m_Event, ref prefabRef);
		destroyer = prefabRef.m_Prefab;
		m_InfoUISystem.tooltipTags.Add(TooltipTags.Destroyed);
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
	}

	[Preserve]
	public DestroyedTreeSection()
	{
	}
}
