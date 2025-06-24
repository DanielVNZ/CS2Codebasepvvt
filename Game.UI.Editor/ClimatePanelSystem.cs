using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Prefabs.Climate;
using Game.Reflection;
using Game.SceneFlow;
using Game.Simulation;
using Game.Tools;
using Game.UI.Widgets;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class ClimatePanelSystem : EditorPanelSystemBase, SeasonsField.IAdapter
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private ClimateSystem m_ClimateSystem;

	private WindSimulationSystem m_WindSimulationSystem;

	private PlanetarySystem m_PlanetarySystem;

	private EntityQuery m_ClimateQuery;

	private EntityQuery m_ClimateSeasonQuery;

	private EntityQuery m_UpdatedQuery;

	private SeasonsField.SeasonCurves m_SeasonsCurves;

	private EntityQuery m_RenderQuery;

	private ToolSystem m_ToolSystem;

	private InfoviewPrefab m_WindInfoview;

	private double m_LastWindDirection;

	private int m_LastClimateHash;

	private int m_InfoviewCooldown;

	private EditorSection m_InspectorSection;

	private EditorGenerator m_Generator;

	private Coroutine m_DelayedInfomodeReset;

	private InfoviewPrefab m_PreviousInfoview;

	private EntityQuery m_AllInfoviewQuery;

	private TypeHandle __TypeHandle;

	private ClimatePrefab currentClimate
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
			if (((EntityManager)(ref entityManager)).Exists(m_ClimateSystem.currentClimate))
			{
				return m_PrefabSystem.GetPrefab<ClimatePrefab>(m_ClimateSystem.currentClimate);
			}
			return null;
		}
		set
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			m_ClimateSystem.currentClimate = m_PrefabSystem.GetEntity(value);
			RebuildInspector();
		}
	}

	private double windDirection
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			float2 constantWind = m_WindSimulationSystem.constantWind;
			float num = Mathf.Atan2(constantWind.y, constantWind.x) * 57.29578f;
			if (num < 0f)
			{
				num += 360f;
			}
			return num;
		}
		set
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			float num = math.radians((float)value);
			float2 direction = default(float2);
			((float2)(ref direction))._002Ector((float)Math.Cos(num), (float)Math.Sin(num));
			m_WindSimulationSystem.SetWind(direction, 40f);
			if (m_DelayedInfomodeReset == null)
			{
				m_PreviousInfoview = m_ToolSystem.activeInfoview;
			}
			m_ToolSystem.infoview = m_WindInfoview;
			foreach (InfomodeInfo infomode in m_ToolSystem.GetInfomodes(m_WindInfoview))
			{
				m_ToolSystem.SetInfomodeActive(infomode.m_Mode, ((Object)infomode.m_Mode).name == "WindInfomode", infomode.m_Priority);
			}
			if (m_DelayedInfomodeReset != null)
			{
				((MonoBehaviour)GameManager.instance).StopCoroutine(m_DelayedInfomodeReset);
			}
			m_DelayedInfomodeReset = ((MonoBehaviour)GameManager.instance).StartCoroutine(DisableInfomode());
		}
	}

	IEnumerable<ClimateSystem.SeasonInfo> SeasonsField.IAdapter.seasons
	{
		get
		{
			return currentClimate.m_Seasons;
		}
		set
		{
			currentClimate.m_Seasons = value.ToArray();
		}
	}

	SeasonsField.SeasonCurves SeasonsField.IAdapter.curves
	{
		get
		{
			return m_SeasonsCurves;
		}
		set
		{
			m_SeasonsCurves = value;
		}
	}

	public Entity selectedSeason { get; set; }

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_WindSimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSimulationSystem>();
		m_PlanetarySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PlanetarySystem>();
		m_AllInfoviewQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<InfoviewData>()
		});
		GetWindInfoView();
		title = "Editor.CLIMATE_SETTINGS";
		m_Generator = new EditorGenerator();
		IWidget[] array = new IWidget[1];
		IWidget[] array2 = new IWidget[1];
		EditorSection obj = new EditorSection
		{
			displayName = "Editor.CLIMATE_SETTINGS",
			tooltip = "Editor.CLIMATE_SETTINGS_TOOLTIP",
			expanded = true
		};
		EditorSection editorSection = obj;
		m_InspectorSection = obj;
		array2[0] = editorSection;
		array[0] = Scrollable.WithChildren(array2);
		children = array;
	}

	protected override void OnValueChanged(IWidget widget)
	{
		base.OnValueChanged(widget);
		ClimatePrefab climatePrefab = currentClimate;
		if (!climatePrefab.builtin)
		{
			climatePrefab.RebuildCurves();
		}
		m_PlanetarySystem.latitude = climatePrefab.m_Latitude;
		m_PlanetarySystem.longitude = climatePrefab.m_Longitude;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Invalid comparison between Unknown and I4
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		base.OnGameLoaded(serializationContext);
		if ((int)((Context)(ref serializationContext)).purpose == 5 || (int)((Context)(ref serializationContext)).purpose == 4)
		{
			RebuildInspector();
		}
	}

	private void RebuildInspector()
	{
		List<IWidget> list = new List<IWidget>
		{
			new PopupValueField<PrefabBase>
			{
				displayName = "Editor.CLIMATE_LOAD_PREFAB",
				tooltip = "Editor.CLIMATE_LOAD_PREFAB_TOOLTIP",
				accessor = new DelegateAccessor<PrefabBase>(() => currentClimate, delegate(PrefabBase prefab)
				{
					currentClimate = (ClimatePrefab)prefab;
				}),
				popup = new PrefabPickerPopup(typeof(ClimatePrefab))
			}
		};
		ClimatePrefab climatePrefab = currentClimate;
		bool builtin = climatePrefab.builtin;
		if (builtin)
		{
			list.Add(new Label
			{
				displayName = "Editor.CREATE_CUSTOM_CLIMATE_PROMPT"
			});
			list.Add(new Button
			{
				displayName = "Editor.CREATE_CUSTOM_CLIMATE",
				action = Duplicate
			});
		}
		IWidget[] array = m_Generator.BuildMembers(new ObjectAccessor<PrefabBase>(climatePrefab), 0, "Climate Settings").ToArray();
		if (builtin)
		{
			IWidget[] array2 = array;
			for (int num = 0; num < array2.Length; num++)
			{
				InspectorPanelSystem.DisableAllFields(array2[num]);
			}
		}
		list.AddRange(array);
		m_InspectorSection.children = list;
	}

	private void Duplicate()
	{
		PrefabBase prefabBase = currentClimate.Clone();
		m_PrefabSystem.AddPrefab(prefabBase);
		currentClimate = (ClimatePrefab)prefabBase;
	}

	private IEnumerator DisableInfomode()
	{
		yield return (object)new WaitForSeconds(1f);
		m_ToolSystem.infoview = m_PreviousInfoview;
		m_DelayedInfomodeReset = null;
	}

	private void GetWindInfoView()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_AllInfoviewQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				InfoviewPrefab prefab = m_PrefabSystem.GetPrefab<InfoviewPrefab>(nativeArray[j]);
				if (((Object)prefab).name == "AirPollution")
				{
					m_WindInfoview = prefab;
				}
			}
		}
		val.Dispose();
	}

	public void RebuildCurves()
	{
		ClimatePrefab climatePrefab = currentClimate;
		climatePrefab.RebuildCurves();
		m_SeasonsCurves = default(SeasonsField.SeasonCurves);
		m_SeasonsCurves.m_Temperature = climatePrefab.m_Temperature;
		m_SeasonsCurves.m_Precipitation = climatePrefab.m_Precipitation;
		m_SeasonsCurves.m_Cloudiness = climatePrefab.m_Cloudiness;
		m_SeasonsCurves.m_Aurora = climatePrefab.m_Aurora;
		m_SeasonsCurves.m_Fog = climatePrefab.m_Fog;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public ClimatePanelSystem()
	{
	}
}
