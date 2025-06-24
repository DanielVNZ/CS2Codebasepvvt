using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Modding;
using Game.Net;
using Game.Prefabs;
using Game.Rendering;
using Game.Serialization;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.City;

[CompilerGenerated]
public class CityConfigurationSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPostDeserialize
{
	private string m_LoadedCityName;

	private NativeList<Entity> m_RequiredContent;

	private bool m_LoadedLeftHandTraffic;

	private bool m_LoadedNaturalDisasters;

	private bool m_UnlockAll;

	private bool m_LoadedUnlockAll;

	private bool m_UnlimitedMoney;

	private bool m_LoadedUnlimitedMoney;

	private bool m_UnlockMapTiles;

	private bool m_LoadedUnlockMapTiles;

	private PrefabSystem m_PrefabSystem;

	private UnlockAllSystem m_UnlockAllSystem;

	private FlipTrafficHandednessSystem m_FlipTrafficHandednessSystem;

	private CameraUpdateSystem m_CameraUpdateSystem;

	private EntityQuery m_ThemeQuery;

	private EntityQuery m_SubLaneQuery;

	public float3 m_CameraPivot;

	public float2 m_CameraAngle;

	public float m_CameraZoom;

	public Entity m_CameraFollow;

	private static readonly float3 kDefaultCameraPivot = new float3(0f, 0f, 0f);

	private static readonly float2 kDefaultCameraAngle = new float2(0f, 45f);

	private static readonly float kDefaultCameraZoom = 250f;

	private static readonly Entity kDefaultCameraFollow = Entity.Null;

	public string cityName { get; set; }

	public string overrideCityName { get; set; }

	[CanBeNull]
	public string overrideThemeName { get; set; }

	public Entity defaultTheme { get; set; }

	public Entity loadedDefaultTheme { get; set; }

	public ref NativeList<Entity> requiredContent => ref m_RequiredContent;

	public bool leftHandTraffic { get; set; }

	public bool overrideLeftHandTraffic { get; set; }

	public bool naturalDisasters { get; set; }

	public bool overrideNaturalDisasters { get; set; }

	public bool unlockAll
	{
		get
		{
			if (!m_LoadedUnlockAll)
			{
				return m_UnlockAll;
			}
			return true;
		}
		set
		{
			m_UnlockAll = value;
		}
	}

	public bool overrideUnlockAll { get; set; }

	public bool unlimitedMoney
	{
		get
		{
			if (!m_LoadedUnlimitedMoney)
			{
				return m_UnlimitedMoney;
			}
			return true;
		}
		set
		{
			m_UnlimitedMoney = value;
		}
	}

	public bool overrideUnlimitedMoney { get; set; }

	public bool unlockMapTiles
	{
		get
		{
			if (!m_LoadedUnlockMapTiles)
			{
				return m_UnlockMapTiles;
			}
			return true;
		}
		set
		{
			m_UnlockMapTiles = value;
		}
	}

	public bool overrideUnlockMapTiles { get; set; }

	public bool overrideLoadedOptions { get; set; }

	public HashSet<string> usedMods { get; private set; } = new HashSet<string>();

	public void PatchReferences(ref PrefabReferences references)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		defaultTheme = references.Check(((ComponentSystemBase)this).EntityManager, defaultTheme);
		loadedDefaultTheme = references.Check(((ComponentSystemBase)this).EntityManager, loadedDefaultTheme);
		for (int i = 0; i < m_RequiredContent.Length; i++)
		{
			m_RequiredContent[i] = references.Check(((ComponentSystemBase)this).EntityManager, m_RequiredContent[i]);
		}
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_UnlockAllSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UnlockAllSystem>();
		m_FlipTrafficHandednessSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<FlipTrafficHandednessSystem>();
		m_CameraUpdateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CameraUpdateSystem>();
		m_RequiredContent = new NativeList<Entity>(0, AllocatorHandle.op_Implicit((Allocator)4));
		m_ThemeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ThemeData>(),
			ComponentType.Exclude<Locked>()
		});
		m_SubLaneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Net.SubLane>() });
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_RequiredContent.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		cityName = overrideCityName;
		leftHandTraffic = overrideLeftHandTraffic;
		naturalDisasters = overrideNaturalDisasters;
		unlockAll = overrideUnlockAll;
		unlimitedMoney = overrideUnlimitedMoney;
		unlockMapTiles = overrideUnlockMapTiles;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		((ComponentSystemBase)m_UnlockAllSystem).Enabled = unlockAll;
	}

	public void PostDeserialize(Context context)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		if (defaultTheme == Entity.Null || !string.IsNullOrEmpty(overrideThemeName))
		{
			NativeArray<Entity> val = ((EntityQuery)(ref m_ThemeQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				if (defaultTheme == Entity.Null && val.Length > 0)
				{
					defaultTheme = val[0];
				}
				if (!string.IsNullOrEmpty(overrideThemeName))
				{
					for (int i = 0; i < val.Length; i++)
					{
						if (((Object)m_PrefabSystem.GetPrefab<ThemePrefab>(val[i])).name == overrideThemeName)
						{
							defaultTheme = val[i];
							break;
						}
					}
				}
			}
			finally
			{
				val.Dispose();
				overrideThemeName = null;
			}
		}
		EntityManager entityManager;
		if (leftHandTraffic != m_LoadedLeftHandTraffic || defaultTheme != loadedDefaultTheme)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Updated>(m_SubLaneQuery);
		}
		if (leftHandTraffic != m_LoadedLeftHandTraffic)
		{
			((ComponentSystemBase)m_FlipTrafficHandednessSystem).Update();
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).Exists(m_CameraFollow))
		{
			if ((Object)(object)m_CameraUpdateSystem.orbitCameraController != (Object)null)
			{
				m_CameraUpdateSystem.orbitCameraController.pivot = float3.op_Implicit(m_CameraPivot);
				m_CameraUpdateSystem.orbitCameraController.rotation = new Vector3(m_CameraAngle.y, m_CameraAngle.x, 0f);
				m_CameraUpdateSystem.orbitCameraController.zoom = m_CameraZoom;
				m_CameraUpdateSystem.orbitCameraController.followedEntity = m_CameraFollow;
				m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.orbitCameraController;
			}
		}
		else if ((Object)(object)m_CameraUpdateSystem.gamePlayController != (Object)null)
		{
			m_CameraUpdateSystem.gamePlayController.pivot = float3.op_Implicit(m_CameraPivot);
			m_CameraUpdateSystem.gamePlayController.rotation = new Vector3(m_CameraAngle.y, m_CameraAngle.x, 0f);
			m_CameraUpdateSystem.gamePlayController.zoom = m_CameraZoom;
			m_CameraUpdateSystem.activeCameraController = m_CameraUpdateSystem.gamePlayController;
		}
		string[] modsEnabled = ModManager.GetModsEnabled();
		if (modsEnabled != null)
		{
			string[] array = modsEnabled;
			foreach (string item in array)
			{
				usedMods.Add(item);
			}
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Invalid comparison between Unknown and I4
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		string text = cityName;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(text);
		Entity val = defaultTheme;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		NativeList<Entity> val2 = m_RequiredContent;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val2);
		bool num = leftHandTraffic;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
		bool num2 = naturalDisasters;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num2);
		Context context = ((IWriter)writer).context;
		if ((int)((Context)(ref context)).purpose == 3)
		{
			float3 val3 = m_CameraPivot;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val3);
			float2 val4 = m_CameraAngle;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val4);
			float num3 = m_CameraZoom;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num3);
			Entity val5 = kDefaultCameraFollow;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val5);
		}
		else if (m_CameraUpdateSystem.activeCameraController != null)
		{
			float3 val6 = float3.op_Implicit(m_CameraUpdateSystem.activeCameraController.pivot);
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val6);
			Vector3 rotation = m_CameraUpdateSystem.activeCameraController.rotation;
			float2 val7 = new float2(rotation.y, rotation.x);
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val7);
			float zoom = m_CameraUpdateSystem.activeCameraController.zoom;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(zoom);
			if (m_CameraUpdateSystem.activeCameraController == m_CameraUpdateSystem.orbitCameraController)
			{
				Entity followedEntity = m_CameraUpdateSystem.orbitCameraController.followedEntity;
				((IWriter)writer/*cast due to .constrained prefix*/).Write(followedEntity);
			}
			else
			{
				Entity val8 = kDefaultCameraFollow;
				((IWriter)writer/*cast due to .constrained prefix*/).Write(val8);
			}
		}
		else
		{
			float3 val9 = kDefaultCameraPivot;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val9);
			float2 val10 = kDefaultCameraAngle;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val10);
			float num4 = kDefaultCameraZoom;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num4);
			Entity val11 = kDefaultCameraFollow;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val11);
		}
		bool num5 = m_UnlimitedMoney;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num5);
		bool num6 = m_UnlockAll;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num6);
		bool num7 = m_UnlockMapTiles;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(num7);
		context = ((IWriter)writer).context;
		if ((int)((Context)(ref context)).purpose == 0)
		{
			int count = usedMods.Count;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(count);
			{
				foreach (string usedMod in usedMods)
				{
					((IWriter)writer/*cast due to .constrained prefix*/).Write(usedMod);
				}
				return;
			}
		}
		((IWriter)writer/*cast due to .constrained prefix*/).Write(0);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Invalid comparison between Unknown and I4
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		Context context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.cityNameInConfig)
		{
			ref string reference = ref m_LoadedCityName;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
		}
		else
		{
			m_LoadedCityName = "";
		}
		Entity val = default(Entity);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
		loadedDefaultTheme = val;
		context = ((IReader)reader).context;
		ContextFormat format = ((Context)(ref context)).format;
		if (((ContextFormat)(ref format)).Has<FormatTags>(FormatTags.ContentPrefabInCityConfiguration))
		{
			NativeList<Entity> val2 = m_RequiredContent;
			((IReader)reader/*cast due to .constrained prefix*/).Read(val2);
		}
		else
		{
			m_RequiredContent.ResizeUninitialized(0);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.leftHandTrafficOption)
		{
			ref bool reference2 = ref m_LoadedLeftHandTraffic;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference2);
		}
		else
		{
			m_LoadedLeftHandTraffic = false;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.naturalDisasterOption)
		{
			ref bool reference3 = ref m_LoadedNaturalDisasters;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference3);
		}
		else
		{
			m_LoadedNaturalDisasters = false;
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.cameraPosition)
		{
			ref float3 reference4 = ref m_CameraPivot;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference4);
			ref float2 reference5 = ref m_CameraAngle;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference5);
			ref float reference6 = ref m_CameraZoom;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference6);
			ref Entity reference7 = ref m_CameraFollow;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference7);
		}
		else
		{
			ResetCameraProperties();
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.unlimitedMoneyAndUnlockAllOptions)
		{
			ref bool reference8 = ref m_LoadedUnlimitedMoney;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference8);
			ref bool reference9 = ref m_LoadedUnlockAll;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference9);
		}
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.unlockMapTilesOption)
		{
			ref bool reference10 = ref m_LoadedUnlockMapTiles;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference10);
		}
		else
		{
			m_LoadedUnlockMapTiles = false;
		}
		usedMods.Clear();
		context = ((IReader)reader).context;
		if (((Context)(ref context)).version >= Version.saveGameUsedMods)
		{
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			usedMods.EnsureCapacity(num);
			string item = default(string);
			for (int i = 0; i < num; i++)
			{
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref item);
				usedMods.Add(item);
			}
		}
		if (!overrideLoadedOptions)
		{
			cityName = m_LoadedCityName;
			naturalDisasters = m_LoadedNaturalDisasters;
			defaultTheme = loadedDefaultTheme;
			leftHandTraffic = m_LoadedLeftHandTraffic;
			unlimitedMoney = m_LoadedUnlimitedMoney;
			unlockAll = m_LoadedUnlockAll;
			unlockMapTiles = m_LoadedUnlockMapTiles;
		}
		else
		{
			context = ((IReader)reader).context;
			if ((int)((Context)(ref context)).purpose == 2)
			{
				defaultTheme = loadedDefaultTheme;
				leftHandTraffic = m_LoadedLeftHandTraffic;
			}
		}
		overrideLoadedOptions = false;
	}

	private void ResetCameraProperties()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		m_CameraPivot = kDefaultCameraPivot;
		m_CameraAngle = kDefaultCameraAngle;
		m_CameraZoom = kDefaultCameraZoom;
		m_CameraFollow = kDefaultCameraFollow;
	}

	public void SetDefaults(Context context)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		m_LoadedCityName = "";
		loadedDefaultTheme = Entity.Null;
		m_LoadedLeftHandTraffic = false;
		m_LoadedNaturalDisasters = false;
		m_LoadedUnlimitedMoney = false;
		m_LoadedUnlockAll = false;
		m_LoadedUnlockMapTiles = false;
		m_RequiredContent.ResizeUninitialized(0);
		if (!overrideLoadedOptions)
		{
			cityName = m_LoadedCityName;
			defaultTheme = loadedDefaultTheme;
			leftHandTraffic = m_LoadedLeftHandTraffic;
			naturalDisasters = m_LoadedNaturalDisasters;
			unlimitedMoney = m_LoadedUnlimitedMoney;
			unlockAll = m_LoadedUnlockAll;
			unlockMapTiles = m_LoadedUnlockMapTiles;
		}
		overrideLoadedOptions = false;
		ResetCameraProperties();
		usedMods.Clear();
	}

	[Preserve]
	public CityConfigurationSystem()
	{
	}
}
