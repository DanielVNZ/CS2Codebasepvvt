using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Areas;
using Game.Buildings;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class DescriptionSection : InfoSectionBase
{
	private PrefabUISystem m_PrefabUISystem;

	private NativeList<LeisureProviderData> m_LeisureDatas;

	private NativeList<LocalModifierData> m_LocalModifierDatas;

	private NativeList<CityModifierData> m_CityModifierDatas;

	protected override string group => "DescriptionSection";

	private string localeId { get; set; }

	protected override bool displayForOutsideConnections => true;

	protected override bool displayForUnderConstruction => true;

	protected override bool displayForUpgrades => true;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_LeisureDatas = new NativeList<LeisureProviderData>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_LocalModifierDatas = new NativeList<LocalModifierData>(10, AllocatorHandle.op_Implicit((Allocator)4));
		m_CityModifierDatas = new NativeList<CityModifierData>(10, AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_LeisureDatas.Dispose();
		m_LocalModifierDatas.Dispose();
		m_CityModifierDatas.Dispose();
		base.OnDestroy();
	}

	protected override void Reset()
	{
		m_LeisureDatas.Clear();
		m_LocalModifierDatas.Clear();
		m_CityModifierDatas.Clear();
		localeId = null;
	}

	private bool Visible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Route>(selectedEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<District>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Area>(selectedEntity))
				{
					goto IL_0058;
				}
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<ServiceObjectData>(selectedPrefab))
			{
				goto IL_0099;
			}
		}
		goto IL_0058;
		IL_0058:
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<SignatureBuildingData>(selectedPrefab))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(selectedEntity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				return ((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(selectedEntity);
			}
		}
		goto IL_0099;
		IL_0099:
		return true;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		base.visible = Visible();
	}

	protected override void OnProcess()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabUISystem.GetTitleAndDescription(selectedPrefab, out var _, out var descriptionId);
		localeId = descriptionId;
		DynamicBuffer<LocalModifierData> val = default(DynamicBuffer<LocalModifierData>);
		if (EntitiesExtensions.TryGetBuffer<LocalModifierData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, true, ref val))
		{
			m_LocalModifierDatas.AddRange(val.AsNativeArray());
		}
		DynamicBuffer<CityModifierData> val2 = default(DynamicBuffer<CityModifierData>);
		if (EntitiesExtensions.TryGetBuffer<CityModifierData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, true, ref val2))
		{
			m_CityModifierDatas.AddRange(val2.AsNativeArray());
		}
		LeisureProviderData leisureProviderData = default(LeisureProviderData);
		if (EntitiesExtensions.TryGetComponent<LeisureProviderData>(((ComponentSystemBase)this).EntityManager, selectedPrefab, ref leisureProviderData) && leisureProviderData.m_Efficiency > 0)
		{
			m_LeisureDatas.Add(ref leisureProviderData);
		}
		DynamicBuffer<InstalledUpgrade> val3 = default(DynamicBuffer<InstalledUpgrade>);
		if (!EntitiesExtensions.TryGetBuffer<InstalledUpgrade>(((ComponentSystemBase)this).EntityManager, selectedEntity, true, ref val3))
		{
			return;
		}
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<LocalModifierData> localModifiers = default(DynamicBuffer<LocalModifierData>);
		DynamicBuffer<CityModifierData> cityModifiers = default(DynamicBuffer<CityModifierData>);
		LeisureProviderData providerToAdd = default(LeisureProviderData);
		for (int i = 0; i < val3.Length; i++)
		{
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val3[i].m_Upgrade, ref prefabRef))
			{
				if (EntitiesExtensions.TryGetBuffer<LocalModifierData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref localModifiers))
				{
					LocalEffectSystem.AddToTempList(m_LocalModifierDatas, localModifiers, disabled: false);
				}
				if (EntitiesExtensions.TryGetBuffer<CityModifierData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, true, ref cityModifiers))
				{
					CityModifierUpdateSystem.AddToTempList(m_CityModifierDatas, cityModifiers);
				}
				if (EntitiesExtensions.TryGetComponent<LeisureProviderData>(((ComponentSystemBase)this).EntityManager, prefabRef.m_Prefab, ref providerToAdd) && providerToAdd.m_Efficiency > 0)
				{
					LeisureSystem.AddToTempList(m_LeisureDatas, providerToAdd);
				}
			}
		}
	}

	public override void OnWriteProperties(IJsonWriter writer)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		writer.PropertyName("localeId");
		writer.Write(localeId);
		writer.PropertyName("effects");
		int num = 0;
		if (m_CityModifierDatas.Length > 0)
		{
			num++;
		}
		if (m_LocalModifierDatas.Length > 0)
		{
			num++;
		}
		if (m_LeisureDatas.Length > 0)
		{
			num++;
		}
		JsonWriterExtensions.ArrayBegin(writer, num);
		if (m_CityModifierDatas.Length > 0)
		{
			PrefabUISystem.CityModifierBinder.Bind<NativeList<CityModifierData>>(writer, m_CityModifierDatas);
		}
		if (m_LocalModifierDatas.Length > 0)
		{
			PrefabUISystem.LocalModifierBinder.Bind<NativeList<LocalModifierData>>(writer, m_LocalModifierDatas);
		}
		if (m_LeisureDatas.Length > 0)
		{
			PrefabUISystem.LeisureProviderBinder.Bind<NativeList<LeisureProviderData>>(writer, m_LeisureDatas);
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public DescriptionSection()
	{
	}
}
