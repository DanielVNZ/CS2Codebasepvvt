using System;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Common;
using Game.Companies;
using Game.Prefabs;
using JetBrains.Annotations;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class AvatarsUISystem : UISystemBase
{
	private const string kGroup = "avatars";

	private const int kIconSize = 32;

	private PrefabSystem m_PrefabSystem;

	private NameSystem m_NameSystem;

	private EntityQuery m_ColorsQuery;

	[UsedImplicitly]
	private RawMapBinding<Entity> m_AvatarsBinding;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_ColorsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<UIAvatarColorData>() });
		AddBinding((IBinding)(object)(m_AvatarsBinding = new RawMapBinding<Entity>("avatars", "avatarsMap", (Action<IJsonWriter, Entity>)BindAvatar, (IReader<Entity>)null, (IWriter<Entity>)null)));
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	private void BindAvatar(IJsonWriter writer, Entity entity)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		writer.TypeBegin("avatars.AvatarData");
		writer.PropertyName("picture");
		writer.Write(GetPicture(entity));
		writer.PropertyName("name");
		m_NameSystem.BindName(writer, entity);
		Color32 color = GetColor(entity);
		writer.PropertyName("color");
		UnityWriters.Write(writer, color);
		writer.TypeEnd();
	}

	private Color32 GetColor(Entity entity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<UIAvatarColorData> singletonBuffer = ((EntityQuery)(ref m_ColorsQuery)).GetSingletonBuffer<UIAvatarColorData>(false);
		int randomIndex = GetRandomIndex(entity);
		if (randomIndex < 0)
		{
			return singletonBuffer[0].m_Color;
		}
		return singletonBuffer[randomIndex % singletonBuffer.Length].m_Color;
	}

	[CanBeNull]
	private string GetPicture(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		CompanyData companyData = default(CompanyData);
		if (EntitiesExtensions.TryGetComponent<CompanyData>(((ComponentSystemBase)this).EntityManager, entity, ref companyData))
		{
			entity = companyData.m_Brand;
		}
		PrefabData prefabData = default(PrefabData);
		if (EntitiesExtensions.TryGetComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, entity, ref prefabData) && m_PrefabSystem.TryGetPrefab<PrefabBase>(prefabData, out var prefab))
		{
			string icon = ImageSystem.GetIcon(prefab);
			if (icon != null)
			{
				return icon;
			}
			if (prefab is ChirperAccount chirperAccount && (Object)(object)chirperAccount.m_InfoView != (Object)null && chirperAccount.m_InfoView.m_IconPath != null)
			{
				return chirperAccount.m_InfoView.m_IconPath;
			}
			if (prefab is BrandPrefab brandPrefab)
			{
				return $"{brandPrefab.thumbnailUrl}?width={32}&height={32}";
			}
		}
		return null;
	}

	private int GetRandomIndex(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<RandomLocalizationIndex> val = default(DynamicBuffer<RandomLocalizationIndex>);
		if (EntitiesExtensions.TryGetBuffer<RandomLocalizationIndex>(((ComponentSystemBase)this).EntityManager, entity, true, ref val) && val.Length > 0)
		{
			return val[0].m_Index;
		}
		return 0;
	}

	[Preserve]
	public AvatarsUISystem()
	{
	}
}
