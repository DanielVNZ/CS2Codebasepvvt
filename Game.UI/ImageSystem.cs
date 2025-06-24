using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Entities;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Creatures;
using Game.Net;
using Game.Prefabs;
using Game.SceneFlow;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI;

[CompilerGenerated]
public class ImageSystem : GameSystemBase
{
	private const string kPlaceholderIcon = "Media/Placeholder.svg";

	private const string kCitizenIcon = "Media/Game/Icons/Citizen.svg";

	private const string kTouristIcon = "Media/Game/Icons/Tourist.svg";

	private const string kCommuterIcon = "Media/Game/Icons/Commuter.svg";

	private const string kAnimalIcon = "Media/Game/Icons/Animal.svg";

	private const string kPetIcon = "Media/Game/Icons/Pet.svg";

	private const string kHealthcareIcon = "Media/Game/Icons/Healthcare.svg";

	private const string kDeathcareIcon = "Media/Game/Icons/Deathcare.svg";

	private const string kPoliceIcon = "Media/Game/Icons/Police.svg";

	private const string kGarbageIcon = "Media/Game/Icons/Garbage.svg";

	private const string kFireIcon = "Media/Game/Icons/FireSafety.svg";

	private const string kPostIcon = "Media/Game/Icons/PostService.svg";

	private const string kDeliveryIcon = "Media/Game/Icons/DeliveryVan.svg";

	private PrefabSystem m_PrefabSystem;

	public string placeholderIcon => "Media/Placeholder.svg";

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	[CanBeNull]
	public static string GetIcon(PrefabBase prefab)
	{
		if (prefab.TryGet<UIObject>(out var component) && !string.IsNullOrEmpty(component.m_Icon))
		{
			return component.m_Icon;
		}
		return null;
	}

	[CanBeNull]
	public string GetGroupIcon(Entity prefabEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		UIObjectData uIObjectData = default(UIObjectData);
		if (EntitiesExtensions.TryGetComponent<UIObjectData>(((ComponentSystemBase)this).EntityManager, prefabEntity, ref uIObjectData) && uIObjectData.m_Group != Entity.Null && m_PrefabSystem.TryGetPrefab<PrefabBase>(uIObjectData.m_Group, out var prefab))
		{
			return GetIcon(prefab);
		}
		return null;
	}

	[CanBeNull]
	public string GetIconOrGroupIcon(Entity prefabEntity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (m_PrefabSystem.TryGetPrefab<PrefabBase>(prefabEntity, out var prefab))
		{
			return GetIcon(prefab) ?? GetGroupIcon(prefabEntity);
		}
		return null;
	}

	[CanBeNull]
	public string GetThumbnail(Entity prefabEntity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (m_PrefabSystem.TryGetPrefab<PrefabBase>(prefabEntity, out var prefab))
		{
			return GetThumbnail(prefab);
		}
		return null;
	}

	[CanBeNull]
	public static string GetThumbnail(PrefabBase prefab)
	{
		string icon = GetIcon(prefab);
		if (icon != null)
		{
			return icon;
		}
		if (GameManager.instance.configuration.noThumbnails)
		{
			return "Media/Placeholder.svg";
		}
		string text = $"{prefab.thumbnailUrl}?width={128}&height={128}";
		COSystemBase.baseLog.VerboseFormat("GetThumbnail - {0}", (object)text);
		return text;
	}

	[CanBeNull]
	public string GetInstanceIcon(Entity instanceEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		return GetInstanceIcon(instanceEntity, ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(instanceEntity).m_Prefab);
	}

	[CanBeNull]
	public string GetInstanceIcon(Entity instanceEntity, Entity prefabEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
		if (EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefabEntity, ref spawnableBuildingData))
		{
			string iconOrGroupIcon = GetIconOrGroupIcon(spawnableBuildingData.m_ZonePrefab);
			if (iconOrGroupIcon != null)
			{
				return iconOrGroupIcon;
			}
		}
		string iconOrGroupIcon2 = GetIconOrGroupIcon(prefabEntity);
		if (iconOrGroupIcon2 != null)
		{
			return iconOrGroupIcon2;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		HouseholdMember householdMember = default(HouseholdMember);
		if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(instanceEntity) && EntitiesExtensions.TryGetComponent<HouseholdMember>(((ComponentSystemBase)this).EntityManager, instanceEntity, ref householdMember))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasChunkComponent<CommuterHousehold>(householdMember.m_Household))
			{
				return "Media/Game/Icons/Commuter.svg";
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TouristHousehold>(householdMember.m_Household))
			{
				return "Media/Game/Icons/Tourist.svg";
			}
			return "Media/Game/Icons/Citizen.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Animal>(instanceEntity))
		{
			return "Media/Game/Icons/Animal.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<HouseholdPet>(instanceEntity))
		{
			return "Media/Game/Icons/Pet.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<AmbulanceData>(prefabEntity))
		{
			return "Media/Game/Icons/Healthcare.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<PoliceCarData>(prefabEntity))
		{
			return "Media/Game/Icons/Police.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<FireEngineData>(prefabEntity))
		{
			return "Media/Game/Icons/FireSafety.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<DeliveryTruckData>(prefabEntity))
		{
			return "Media/Game/Icons/DeliveryVan.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<PostVanData>(prefabEntity))
		{
			return "Media/Game/Icons/PostService.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<HearseData>(prefabEntity))
		{
			return "Media/Game/Icons/Deathcare.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<GarbageTruckData>(prefabEntity))
		{
			return "Media/Game/Icons/Garbage.svg";
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		Owner owner = default(Owner);
		PrefabRef prefabRef = default(PrefabRef);
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(instanceEntity) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, instanceEntity, ref owner) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, owner.m_Owner, ref prefabRef))
		{
			instanceEntity = owner.m_Owner;
			prefabEntity = prefabRef.m_Prefab;
		}
		ServiceObjectData serviceObjectData = default(ServiceObjectData);
		if (EntitiesExtensions.TryGetComponent<ServiceObjectData>(((ComponentSystemBase)this).EntityManager, prefabEntity, ref serviceObjectData))
		{
			string iconOrGroupIcon3 = GetIconOrGroupIcon(serviceObjectData.m_Service);
			if (iconOrGroupIcon3 != null)
			{
				return iconOrGroupIcon3;
			}
		}
		DynamicBuffer<AggregateElement> val = default(DynamicBuffer<AggregateElement>);
		PrefabRef prefabRef2 = default(PrefabRef);
		if (EntitiesExtensions.TryGetBuffer<AggregateElement>(((ComponentSystemBase)this).EntityManager, instanceEntity, true, ref val) && val.Length != 0 && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val[0].m_Edge, ref prefabRef2))
		{
			string iconOrGroupIcon4 = GetIconOrGroupIcon(prefabRef2.m_Prefab);
			if (iconOrGroupIcon4 != null)
			{
				return iconOrGroupIcon4;
			}
		}
		Owner owner2 = default(Owner);
		PropertyRenter propertyRenter = default(PropertyRenter);
		PrefabRef prefabRef3 = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, instanceEntity, ref owner2) && EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, owner2.m_Owner, ref propertyRenter) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref prefabRef3))
		{
			SpawnableBuildingData spawnableBuildingData2 = default(SpawnableBuildingData);
			Entity prefabEntity2 = (EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, prefabRef3.m_Prefab, ref spawnableBuildingData2) ? spawnableBuildingData2.m_ZonePrefab : prefabRef3.m_Prefab);
			string iconOrGroupIcon5 = GetIconOrGroupIcon(prefabEntity2);
			if (iconOrGroupIcon5 != null)
			{
				return iconOrGroupIcon5;
			}
		}
		return null;
	}

	[Preserve]
	public ImageSystem()
	{
	}
}
