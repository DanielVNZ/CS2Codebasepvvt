using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Citizens;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Events;
using Game.Objects;
using Game.Prefabs;
using Game.Routes;
using Game.SceneFlow;
using Game.UI.InGame;
using Game.UI.Localization;
using Game.Vehicles;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI;

[CompilerGenerated]
public class NameSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public enum NameType
	{
		Custom,
		Localized,
		Formatted
	}

	public struct Name : IJsonWritable, ISerializable
	{
		private NameType m_NameType;

		private string m_NameID;

		private string[] m_NameArgs;

		public static Name CustomName(string name)
		{
			return new Name
			{
				m_NameType = NameType.Custom,
				m_NameID = name
			};
		}

		public static Name LocalizedName(string nameID)
		{
			return new Name
			{
				m_NameType = NameType.Localized,
				m_NameID = nameID
			};
		}

		public static Name FormattedName(string nameID, params string[] args)
		{
			return new Name
			{
				m_NameType = NameType.Formatted,
				m_NameID = nameID,
				m_NameArgs = args
			};
		}

		public void Write(IJsonWriter writer)
		{
			if (m_NameType == NameType.Custom)
			{
				BindCustomName(writer);
			}
			else if (m_NameType == NameType.Formatted)
			{
				BindFormattedName(writer);
			}
			else if (m_NameType == NameType.Localized)
			{
				BindLocalizedName(writer);
			}
		}

		private void BindCustomName(IJsonWriter writer)
		{
			writer.TypeBegin("names.CustomName");
			writer.PropertyName("name");
			writer.Write(m_NameID);
			writer.TypeEnd();
		}

		private void BindFormattedName(IJsonWriter writer)
		{
			writer.TypeBegin("names.FormattedName");
			writer.PropertyName("nameId");
			writer.Write(m_NameID);
			writer.PropertyName("nameArgs");
			int num = ((m_NameArgs != null) ? (m_NameArgs.Length / 2) : 0);
			JsonWriterExtensions.MapBegin(writer, num);
			for (int i = 0; i < num; i++)
			{
				writer.Write(m_NameArgs[i * 2] ?? string.Empty);
				writer.Write(m_NameArgs[i * 2 + 1] ?? string.Empty);
			}
			writer.MapEnd();
			writer.TypeEnd();
		}

		private void BindLocalizedName(IJsonWriter writer)
		{
			writer.TypeBegin("names.LocalizedName");
			writer.PropertyName("nameId");
			writer.Write(m_NameID ?? string.Empty);
			writer.TypeEnd();
		}

		public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
		{
			NameType num = m_NameType;
			((IWriter)writer/*cast due to .constrained prefix*/).Write((int)num);
			string obj = m_NameID ?? string.Empty;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(obj);
			int num2 = ((m_NameArgs != null) ? m_NameArgs.Length : 0);
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
			for (int i = 0; i < num2; i++)
			{
				string obj2 = m_NameArgs[i] ?? string.Empty;
				((IWriter)writer/*cast due to .constrained prefix*/).Write(obj2);
			}
		}

		public void Deserialize<TReader>(TReader reader) where TReader : IReader
		{
			int nameType = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref nameType);
			m_NameType = (NameType)nameType;
			ref string reference = ref m_NameID;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			m_NameArgs = new string[num];
			string text = default(string);
			for (int i = 0; i < num; i++)
			{
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref text);
				m_NameArgs[i] = text;
			}
		}
	}

	private EntityQuery m_DeletedQuery;

	private PrefabSystem m_PrefabSystem;

	private PrefabUISystem m_PrefabUISystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private Dictionary<Entity, string> m_Names;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_DeletedQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CustomName>(),
			ComponentType.ReadOnly<Deleted>()
		});
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_Names = new Dictionary<Entity, string>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_DeletedQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<Entity> val = ((EntityQuery)(ref m_DeletedQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				if (m_Names.ContainsKey(val[i]))
				{
					m_Names.Remove(val[i]);
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	public string GetDebugName(Entity entity)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		string arg = null;
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref prefabRef))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			arg = ((!((EntityManager)(ref entityManager)).HasComponent<PrefabData>(prefabRef.m_Prefab)) ? "(invalid prefab)" : ((!m_PrefabSystem.TryGetPrefab<PrefabBase>(prefabRef.m_Prefab, out var prefab)) ? m_PrefabSystem.GetObsoleteID(prefabRef.m_Prefab).GetName() : ((Object)prefab).name));
		}
		return $"{arg} {entity.Index}";
	}

	public bool TryGetCustomName(Entity entity, out string customName)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return m_Names.TryGetValue(entity, out customName);
	}

	public void SetCustomName(Entity entity, string name)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (entity == Entity.Null)
		{
			return;
		}
		Controller controller = default(Controller);
		if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, entity, ref controller))
		{
			entity = controller.m_Controller;
		}
		if (name == string.Empty || string.IsNullOrWhiteSpace(name))
		{
			if (m_Names.ContainsKey(entity))
			{
				EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
				((EntityCommandBuffer)(ref val)).RemoveComponent<CustomName>(entity);
				((EntityCommandBuffer)(ref val)).AddComponent<BatchesUpdated>(entity);
				m_Names.Remove(entity);
			}
		}
		else
		{
			m_Names[entity] = name;
			EntityCommandBuffer val2 = m_EndFrameBarrier.CreateCommandBuffer();
			((EntityCommandBuffer)(ref val2)).AddComponent<CustomName>(entity);
			((EntityCommandBuffer)(ref val2)).AddComponent<BatchesUpdated>(entity);
		}
	}

	public string GetRenderedLabelName(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetCustomName(entity, out var customName))
		{
			return customName;
		}
		string id = GetId(entity);
		string result = default(string);
		if (!GameManager.instance.localizationManager.activeDictionary.TryGetValue(id, ref result))
		{
			return id;
		}
		return result;
	}

	public void BindNameForVirtualKeyboard(IJsonWriter writer, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		JsonWriterExtensions.Write<Name>(writer, GetNameForVirtualKeyboard(entity));
	}

	public Name GetNameForVirtualKeyboard(Entity entity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref prefabRef))
		{
			val = prefabRef.m_Prefab;
		}
		if (val != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(entity))
			{
				RoutePrefab prefab = m_PrefabSystem.GetPrefab<RoutePrefab>(val);
				return Name.LocalizedName(prefab.m_LocaleID + "[" + ((Object)prefab).name + "]");
			}
		}
		Controller controller = default(Controller);
		if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, entity, ref controller))
		{
			entity = controller.m_Controller;
		}
		return Name.LocalizedName(GetId(entity, useRandomLocalization: false));
	}

	public void BindName(IJsonWriter writer, Entity entity)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		JsonWriterExtensions.Write<Name>(writer, GetName(entity));
	}

	public Name GetName(Entity entity, bool omitBrand = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		Entity val = Entity.Null;
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref prefabRef))
		{
			val = prefabRef.m_Prefab;
		}
		Controller controller = default(Controller);
		if (EntitiesExtensions.TryGetComponent<Controller>(((ComponentSystemBase)this).EntityManager, entity, ref controller))
		{
			entity = controller.m_Controller;
		}
		if (TryGetCustomName(entity, out var customName))
		{
			return Name.CustomName(customName);
		}
		CompanyData companyData = default(CompanyData);
		if (EntitiesExtensions.TryGetComponent<CompanyData>(((ComponentSystemBase)this).EntityManager, entity, ref companyData))
		{
			string id = GetId(companyData.m_Brand);
			if (id == null)
			{
				return Name.CustomName(((Object)m_PrefabSystem.GetPrefab<PrefabBase>(val)).name + " brand is null!");
			}
			return Name.LocalizedName(id);
		}
		EntityManager entityManager;
		if (val != Entity.Null)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			if (!((EntityManager)(ref entityManager)).HasComponent<SignatureBuildingData>(val) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, val, ref spawnableBuildingData))
			{
				return GetSpawnableBuildingName(entity, spawnableBuildingData.m_ZonePrefab, omitBrand);
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Routes.TransportStop>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<Game.Objects.OutsideConnection>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<Marker>(entity))
				{
					return GetMarkerTransportStopName(entity);
				}
				return GetStaticTransportStopName(entity);
			}
		}
		if (val != Entity.Null)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<TransportLine>(entity))
			{
				return GetRouteName(entity, val);
			}
		}
		if (val != Entity.Null)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(entity))
			{
				return GetCitizenName(entity, val);
			}
		}
		if (val != Entity.Null)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Creatures.Resident>(entity))
			{
				return GetResidentName(entity, val);
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Events.TrafficAccident>(entity))
		{
			return Name.LocalizedName("SelectedInfoPanel.TRAFFIC_ACCIDENT");
		}
		return Name.LocalizedName(GetId(entity));
	}

	private string GetGenderedLastNameId(Entity household, bool male)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (household == Entity.Null)
		{
			return null;
		}
		PrefabRef refData = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, household, ref refData) && m_PrefabSystem.GetPrefab<PrefabBase>(refData).TryGet<RandomGenderedLocalization>(out var component))
		{
			string text = (male ? component.m_MaleID : component.m_FemaleID);
			DynamicBuffer<RandomLocalizationIndex> val = default(DynamicBuffer<RandomLocalizationIndex>);
			if (EntitiesExtensions.TryGetBuffer<RandomLocalizationIndex>(((ComponentSystemBase)this).EntityManager, household, true, ref val) && val.Length > 0)
			{
				return LocalizationUtils.AppendIndex(text, val[0]);
			}
			return text;
		}
		return GetId(household);
	}

	public void BindFamilyName(IJsonWriter writer, Entity household)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		JsonWriterExtensions.Write<Name>(writer, GetFamilyName(household));
	}

	private Name GetFamilyName(Entity household)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetCustomName(household, out var customName))
		{
			return Name.CustomName(customName);
		}
		int num = 0;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<HouseholdCitizen> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(household, false);
		Citizen citizen = default(Citizen);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity val = buffer[i];
			if (EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, val, ref citizen) && (citizen.m_State & CitizenFlags.Male) != CitizenFlags.None)
			{
				num++;
			}
		}
		return Name.LocalizedName(GetGenderedLastNameId(household, num > 1));
	}

	private string GetId(Entity entity, bool useRandomLocalization = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (entity == Entity.Null)
		{
			return null;
		}
		Entity val = Entity.Null;
		PrefabRef prefabRef = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, entity, ref prefabRef))
		{
			val = prefabRef.m_Prefab;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
		if (!((EntityManager)(ref entityManager)).HasComponent<SignatureBuildingData>(val) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(((ComponentSystemBase)this).EntityManager, val, ref spawnableBuildingData))
		{
			val = spawnableBuildingData.m_ZonePrefab;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<ChirperAccountData>(entity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (!((EntityManager)(ref entityManager)).HasComponent<BrandData>(entity))
			{
				goto IL_0078;
			}
		}
		val = entity;
		goto IL_0078;
		IL_0078:
		if (val != Entity.Null)
		{
			if (m_PrefabSystem.TryGetPrefab<PrefabBase>(val, out var prefab))
			{
				if (prefab.TryGet<Game.Prefabs.Localization>(out var component))
				{
					DynamicBuffer<RandomLocalizationIndex> val2 = default(DynamicBuffer<RandomLocalizationIndex>);
					if (useRandomLocalization && component is RandomLocalization && EntitiesExtensions.TryGetBuffer<RandomLocalizationIndex>(((ComponentSystemBase)this).EntityManager, entity, true, ref val2) && val2.Length > 0)
					{
						return LocalizationUtils.AppendIndex(component.m_LocalizationID, val2[0]);
					}
					return component.m_LocalizationID;
				}
				m_PrefabUISystem.GetTitleAndDescription(val, out var titleId, out var _);
				return titleId;
			}
			return m_PrefabSystem.GetObsoleteID(val).GetName();
		}
		return string.Empty;
	}

	private Name GetCitizenName(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		string id = GetId(entity);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		HouseholdMember componentData = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(entity);
		Citizen citizen = default(Citizen);
		bool male = EntitiesExtensions.TryGetComponent<Citizen>(((ComponentSystemBase)this).EntityManager, entity, ref citizen) && (citizen.m_State & CitizenFlags.Male) != 0;
		string genderedLastNameId = GetGenderedLastNameId(componentData.m_Household, male);
		return Name.FormattedName("Assets.CITIZEN_NAME_FORMAT", "FIRST_NAME", id, "LAST_NAME", genderedLastNameId);
	}

	private Name GetResidentName(Entity entity, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		PseudoRandomSeed pseudoRandomSeed = default(PseudoRandomSeed);
		EntitiesExtensions.TryGetComponent<PseudoRandomSeed>(((ComponentSystemBase)this).EntityManager, entity, ref pseudoRandomSeed);
		CreatureData creatureData = default(CreatureData);
		EntitiesExtensions.TryGetComponent<CreatureData>(((ComponentSystemBase)this).EntityManager, prefab, ref creatureData);
		Random random = pseudoRandomSeed.GetRandom(PseudoRandomSeed.kDummyName);
		bool flag = false;
		if (creatureData.m_Gender == GenderMask.Male)
		{
			flag = true;
		}
		else if (creatureData.m_Gender != GenderMask.Female)
		{
			flag = ((Random)(ref random)).NextBool();
		}
		string text = (flag ? "Assets.CITIZEN_NAME_MALE" : "Assets.CITIZEN_NAME_FEMALE");
		string text2 = (flag ? "Assets.CITIZEN_SURNAME_MALE" : "Assets.CITIZEN_SURNAME_FEMALE");
		PrefabBase prefab2 = m_PrefabSystem.GetPrefab<PrefabBase>(prefab);
		int localizationIndexCount = RandomLocalization.GetLocalizationIndexCount(prefab2, text);
		int localizationIndexCount2 = RandomLocalization.GetLocalizationIndexCount(prefab2, text2);
		string text3 = LocalizationUtils.AppendIndex(text, new RandomLocalizationIndex(((Random)(ref random)).NextInt(localizationIndexCount)));
		string text4 = LocalizationUtils.AppendIndex(text2, new RandomLocalizationIndex(((Random)(ref random)).NextInt(localizationIndexCount2)));
		return Name.FormattedName("Assets.CITIZEN_NAME_FORMAT", "FIRST_NAME", text3, "LAST_NAME", text4);
	}

	private Name GetSpawnableBuildingName(Entity building, Entity zone, bool omitBrand = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		BuildingUtils.GetAddress(((ComponentSystemBase)this).EntityManager, building, out var road, out var number);
		if (!TryGetCustomName(road, out var customName))
		{
			customName = GetId(road);
		}
		if (customName == null)
		{
			return Name.LocalizedName(GetId(building));
		}
		ZoneData zoneData = default(ZoneData);
		if (!omitBrand && EntitiesExtensions.TryGetComponent<ZoneData>(((ComponentSystemBase)this).EntityManager, zone, ref zoneData) && zoneData.m_AreaType != AreaType.Residential)
		{
			string brandId = GetBrandId(building);
			if (brandId != null)
			{
				return Name.FormattedName("Assets.NAMED_ADDRESS_NAME_FORMAT", "NAME", brandId, "ROAD", customName, "NUMBER", number.ToString());
			}
		}
		return Name.FormattedName("Assets.ADDRESS_NAME_FORMAT", "ROAD", customName, "NUMBER", number.ToString());
	}

	private string GetBrandId(Entity building)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Renter> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Renter>(building, true);
		CompanyData companyData = default(CompanyData);
		for (int i = 0; i < buffer.Length; i++)
		{
			if (EntitiesExtensions.TryGetComponent<CompanyData>(((ComponentSystemBase)this).EntityManager, buffer[i].m_Renter, ref companyData))
			{
				return GetId(companyData.m_Brand);
			}
		}
		return null;
	}

	private Name GetStaticTransportStopName(Entity stop)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		BuildingUtils.GetAddress(((ComponentSystemBase)this).EntityManager, stop, out var road, out var number);
		if (!TryGetCustomName(road, out var customName))
		{
			customName = GetId(road);
		}
		if (customName == null)
		{
			return Name.LocalizedName(GetId(stop));
		}
		return Name.FormattedName("Assets.ADDRESS_NAME_FORMAT", "ROAD", customName, "NUMBER", number.ToString());
	}

	private Name GetMarkerTransportStopName(Entity stop)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Entity val = stop;
		Owner owner = default(Owner);
		for (int i = 0; i < 8; i++)
		{
			if (!EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, val, ref owner))
			{
				break;
			}
			val = owner.m_Owner;
		}
		return Name.LocalizedName(GetId(val));
	}

	private Name GetRouteName(Entity route, Entity prefab)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		RoutePrefab prefab2 = m_PrefabSystem.GetPrefab<RoutePrefab>(prefab);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		RouteNumber componentData = ((EntityManager)(ref entityManager)).GetComponentData<RouteNumber>(route);
		return Name.FormattedName(prefab2.m_LocaleID + "[" + ((Object)prefab2).name + "]", "NUMBER", componentData.m_Number.ToString());
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		int count = m_Names.Count;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(count);
		foreach (KeyValuePair<Entity, string> item in m_Names)
		{
			Entity key = item.Key;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(key);
			string value = item.Value;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(value);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_Names.Clear();
		Entity val = default(Entity);
		string value = default(string);
		for (int i = 0; i < num; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref value);
			if (val != Entity.Null)
			{
				m_Names.Add(val, value);
			}
		}
	}

	public void SetDefaults(Context context)
	{
		m_Names.Clear();
	}

	[Preserve]
	public NameSystem()
	{
	}
}
