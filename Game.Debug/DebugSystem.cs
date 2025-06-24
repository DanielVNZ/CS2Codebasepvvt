using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Colossal;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.VirtualTexturing;
using Colossal.Logging;
using Colossal.Mathematics;
using Colossal.PSI.Common;
using Colossal.PSI.Environment;
using Colossal.PSI.PdxSdk;
using Colossal.Reflection;
using Colossal.Rendering;
using Colossal.Serialization.Entities;
using Game.Achievements;
using Game.Agents;
using Game.Areas;
using Game.Buildings;
using Game.Citizens;
using Game.City;
using Game.Common;
using Game.Companies;
using Game.Creatures;
using Game.Economy;
using Game.Effects;
using Game.Events;
using Game.Net;
using Game.Notifications;
using Game.Objects;
using Game.Pathfind;
using Game.Policies;
using Game.Prefabs;
using Game.Prefabs.Modes;
using Game.Rendering;
using Game.Rendering.Climate;
using Game.Rendering.Utilities;
using Game.SceneFlow;
using Game.Serialization;
using Game.Simulation;
using Game.Tools;
using Game.Triggers;
using Game.Tutorials;
using Game.UI;
using Game.UI.Debug;
using Game.UI.InGame;
using Game.UI.Widgets;
using Game.Vehicles;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Debug;

[DebugContainer]
[CompilerGenerated]
public class DebugSystem : GameSystemBase, IDebugData
{
	private enum ToggleEnum
	{
		Disabled,
		Enabled
	}

	private delegate void ToolUI(Container c);

	private struct ResetServiceData
	{
		public int count;

		public float meanPriority;

		public int serviceAvailable;

		public float efficiency;

		public int workers;

		public int maxWorkers;

		public int overworked;

		public int underworked;

		public int middle;

		public int level;
	}

	private struct NotificationInfo : IComparable<NotificationInfo>
	{
		public Entity m_Prefab;

		public int m_Instances;

		public int CompareTo(NotificationInfo other)
		{
			return other.m_Instances - m_Instances;
		}
	}

	private struct PathfindQueryItem : IComparable<PathfindQueryItem>
	{
		public PathfindResultSystem.ResultKey m_Key;

		public PathfindResultSystem.ResultValue m_Value;

		public int CompareTo(PathfindQueryItem other)
		{
			if (!((float)other.m_Value.m_QueryCount * other.m_Value.m_GraphTraversal >= (float)m_Value.m_QueryCount * m_Value.m_GraphTraversal))
			{
				return -1;
			}
			return 1;
		}
	}

	private struct SerializationItem : IComparable<SerializationItem>
	{
		public int m_TotalSize;

		public int m_OverheadSize;

		public Type m_Type;

		public int CompareTo(SerializationItem other)
		{
			return other.m_TotalSize - m_TotalSize;
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<HealthProblem> __Game_Citizens_HealthProblem_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Citizens.Student> __Game_Citizens_Student_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Worker> __Game_Citizens_Worker_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ServiceFee> __Game_City_ServiceFee_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Locked> __Game_Prefabs_Locked_RO_ComponentLookup;

		[ReadOnly]
		public ComponentTypeHandle<DistrictOptionData> __Game_Prefabs_DistrictOptionData_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<DistrictModifierData> __Game_Prefabs_DistrictModifierData_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<CityOptionData> __Game_Prefabs_CityOptionData_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferTypeHandle<CityModifierData> __Game_Prefabs_CityModifierData_RO_BufferTypeHandle;

		public ComponentLookup<Citizen> __Game_Citizens_Citizen_RW_ComponentLookup;

		public ComponentLookup<Game.Creatures.Resident> __Game_Creatures_Resident_RW_ComponentLookup;

		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RW_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Citizens_Citizen_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(true);
			__Game_Citizens_HealthProblem_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<HealthProblem>(true);
			__Game_Citizens_Student_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Citizens.Student>(true);
			__Game_Citizens_Worker_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Worker>(true);
			__Game_City_ServiceFee_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ServiceFee>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
			__Game_Prefabs_Locked_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Locked>(true);
			__Game_Prefabs_DistrictOptionData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<DistrictOptionData>(true);
			__Game_Prefabs_DistrictModifierData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<DistrictModifierData>(true);
			__Game_Prefabs_CityOptionData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CityOptionData>(true);
			__Game_Prefabs_CityModifierData_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<CityModifierData>(true);
			__Game_Citizens_Citizen_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Citizen>(false);
			__Game_Creatures_Resident_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Creatures.Resident>(false);
			__Game_Prefabs_PrefabRef_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(false);
		}
	}

	private string m_CommonComponentsFilter = "";

	private bool m_CommonComponentsUnused;

	private List<ComponentDebugUtils.ComponentInfo> m_CommonComponents;

	private static readonly GUIContent[] kDebugSimulationSpeedStrings = (GUIContent[])(object)new GUIContent[8]
	{
		new GUIContent("0x"),
		new GUIContent("1/8x"),
		new GUIContent("1/4x"),
		new GUIContent("1/2x"),
		new GUIContent("1x"),
		new GUIContent("2x"),
		new GUIContent("4x"),
		new GUIContent("8x")
	};

	private static readonly float[] kDebugSimulationSpeedValues = new float[8] { 0f, 0.125f, 0.25f, 0.5f, 1f, 2f, 4f, 8f };

	private static readonly GUIContent[] kDebugWaterSpeedStrings = (GUIContent[])(object)new GUIContent[7]
	{
		new GUIContent("0x"),
		new GUIContent("1x"),
		new GUIContent("8x"),
		new GUIContent("16x"),
		new GUIContent("32x"),
		new GUIContent("64x"),
		new GUIContent("Max")
	};

	private static int[] m_DebugWaterSpeedValues = new int[7] { 0, 1, 8, 16, 32, 64, -1 };

	private static ToolBaseSystem[] m_ToolSystems;

	private static GUIContent[] m_ToolSystemNames;

	private List<object> m_DebugClasses = new List<object>();

	private Dictionary<string, List<Widget>> m_Panels = new Dictionary<string, List<Widget>>();

	private PrefabBase m_LastToolPrefab;

	private int m_ArchetypeCount;

	private int m_FilteredArchetypeCount;

	private int m_ChunkCount;

	private int m_ChunkCapacity;

	private int m_EntityCount;

	private TerrainSystem m_TerrainSystem;

	private ToolSystem m_ToolSystem;

	private SimulationSystem m_SimulationSystem;

	private ZoneSpawnSystem m_ZoneSpawnSystem;

	private AreaSpawnSystem m_AreaSpawnSystem;

	private BuildingUpkeepSystem m_BuildingUpkeepSystem;

	private HouseholdFindPropertySystem m_HouseholdFindPropertySystem;

	private GraduationSystem m_GraduationSystem;

	private CrimeCheckSystem m_CrimeCheckSystem;

	private ApplyToSchoolSystem m_ApplyToSchoolSystem;

	private FindSchoolSystem m_FindSchoolSystem;

	private PrefabSystem m_PrefabSystem;

	private SelectedInfoUISystem m_SelectedInfoUISystem;

	private CitySystem m_CitySystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ResourceSystem m_ResourceSystem;

	private DebugUISystem m_DebugUISystem;

	private WaterSystem m_WaterSystem;

	private SnowSystem m_SnowSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private TelecomCoverageSystem m_TelecomCoverageSystem;

	private PlanetarySystem m_PlanetarySystem;

	private ClimateSystem m_ClimateSystem;

	private ClimateRenderSystem m_ClimateRenderSystem;

	private TimeSystem m_TimeSystem;

	private CityStatisticsSystem m_CityStatisticsSystem;

	private ElectricityFlowSystem m_ElectricityFlowSystem;

	private AdjustElectricityConsumptionSystem m_AdjustElectricityConsumptionSystem;

	private WaterPipeFlowSystem m_WaterPipeFlowSystem;

	private DispatchWaterSystem m_DispatchWaterSystem;

	private UnlockAllSystem m_UnlockAllSystem;

	private TripNeededSystem m_TripNeededSystem;

	private LifePathEventSystem m_LifePathEventSystem;

	private BirthSystem m_BirthSystem;

	private EndFrameBarrier m_EndFrameBarrier;

	private SignatureBuildingUISystem m_SignatureBuildingUISystem;

	private EntityQuery m_DebugQuery;

	private EntityQuery m_PolicyQuery;

	private EntityQuery m_NetQuery;

	private EntityQuery m_ObjectQuery;

	private EntityQuery m_ZoneQuery;

	private EntityQuery m_AreaQuery;

	private EntityQuery m_RouteQuery;

	private EntityQuery m_TerraformingQuery;

	private EntityQuery m_InfoviewQuery;

	private EntityQuery m_ThemeQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_DemandParameterQuery;

	private EntityQuery m_PollutionParameterQuery;

	private EntityQuery m_CitizenHappinessParameterQuery;

	private EntityQuery m_ExtractorParameterQuery;

	private EntityQuery m_HealthcareParameterQuery;

	private EntityQuery m_ParkParameterQuery;

	private EntityQuery m_EducationParameterQuery;

	private EntityQuery m_TelecomParameterQuery;

	private EntityQuery m_GarbageParameterQuery;

	private EntityQuery m_PoliceParameterQuery;

	private EntityQuery m_LandValueQuery;

	private EntityQuery m_RenterQuery;

	private EntityQuery m_EventQuery;

	private EntityQuery m_SelectableQuery;

	private EntityQuery m_ServiceQuery;

	private EntityQuery m_TradeCostQuery;

	private EntityQuery m_TransferQuery;

	private EntityQuery m_TripNeededQuery;

	private EntityQuery m_HouseholdGroup;

	private EntityQuery m_HouseholdMemberGroup;

	private EntityQuery m_AtmosphereQuery;

	private EntityQuery m_BiomeQuery;

	private EntityQuery m_IconQuery;

	private EntityQuery m_CompanyGroup;

	private EntityQuery m_SignatureBuildingQuery;

	private EntityArchetype m_PolicyEventArchetype;

	private float3 m_LastSelectionPosition;

	private bool m_FastForwardClimateTime;

	private const string kDebugSaveName = "DebugSave";

	private int selectedModeIndex;

	private bool m_RenderingDebugUIInitialized;

	private DebugCustomPass m_DebugBlitPass;

	private GameObject m_DebugBlitVolume;

	private PreCullingSystem m_PreCullingSystem;

	private EffectControlSystem m_EffectControlSystem;

	private ProceduralSkeletonSystem m_ProceduralSkeletonSystem;

	private ProceduralEmissiveSystem m_ProceduralEmissiveSystem;

	private BatchManagerSystem m_BatchManagerSystem;

	private AreaBatchSystem m_AreaBatchSystem;

	private BatchMeshSystem m_BatchMeshSystem;

	private AnimatedSystem m_AnimatedSystem;

	private ManagedBatchSystem m_ManagedBatchSystem;

	private VegetationRenderSystem m_VegetationRenderSystem;

	private TerrainMaterialSystem m_TerrainMaterialSystem;

	private RenderingSystem m_RenderingSystem;

	private List<NotificationInfo> m_Notifications;

	private List<PathfindQueryItem> m_PathfindQueryBuffer;

	private List<SerializationItem> m_SerializationBuffer;

	private TypeHandle __TypeHandle;

	private EntityQuery __query_1508003740_0;

	private static Widget CreateVolumeParameterWidget(string name, VolumeParameter param, Func<bool> isHiddenCallback = null)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_0047: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Expected O, but got Unknown
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Expected O, but got Unknown
		if (param != null)
		{
			Type parameterType = ((object)param).GetType();
			if (parameterType == typeof(ColorParameter))
			{
				ColorParameter p = (ColorParameter)param;
				ColorField val = new ColorField
				{
					displayName = name,
					hdr = p.hdr,
					showAlpha = p.showAlpha
				};
				((Field<Color>)val).getter = () => ((VolumeParameter<Color>)(object)p).value;
				((Field<Color>)val).setter = delegate(Color value)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					((VolumeParameter<Color>)(object)p).value = value;
				};
				((Widget)val).isHiddenCallback = isHiddenCallback;
				return (Widget)val;
			}
			if (parameterType == typeof(BoolParameter))
			{
				BoolParameter p2 = (BoolParameter)param;
				BoolField val2 = new BoolField
				{
					displayName = name
				};
				((Field<bool>)val2).getter = () => ((VolumeParameter<bool>)(object)p2).value;
				((Field<bool>)val2).setter = delegate(bool value)
				{
					((VolumeParameter<bool>)(object)p2).value = value;
				};
				((Widget)val2).isHiddenCallback = isHiddenCallback;
				return (Widget)val2;
			}
			Type[] genericTypeArguments = parameterType.GetTypeInfo().BaseType.GenericTypeArguments;
			if (genericTypeArguments.Length != 0 && genericTypeArguments[0].IsArray)
			{
				ObjectListField val3 = new ObjectListField
				{
					displayName = name
				};
				((Field<Object[]>)val3).getter = () => (Object[])parameterType.GetProperty("value").GetValue(param, null);
				val3.type = parameterType;
				return (Widget)val3;
			}
			PropertyInfo property = ((object)param).GetType().GetProperty("value");
			MethodInfo method = property.PropertyType.GetMethod("ToString", Type.EmptyTypes);
			if (method == null || method.DeclaringType == typeof(object) || method.DeclaringType == typeof(Object))
			{
				PropertyInfo nameProp = property.PropertyType.GetProperty("name");
				if (!(nameProp == null))
				{
					return (Widget)new Value
					{
						displayName = name,
						getter = delegate
						{
							object value = property.GetValue(param);
							return (value == null || value.Equals(null)) ? "None" : (nameProp.GetValue(value) ?? "None");
						},
						isHiddenCallback = isHiddenCallback
					};
				}
				return (Widget)new Value
				{
					displayName = name,
					getter = () => "Debug view not supported"
				};
			}
			return (Widget)new Value
			{
				displayName = name,
				getter = delegate
				{
					object value = property.GetValue(param);
					return (value != null) ? value.ToString() : "None";
				},
				isHiddenCallback = isHiddenCallback
			};
		}
		return (Widget)new Value
		{
			displayName = name,
			getter = () => "-"
		};
	}

	[DebugTab("Climate", -994)]
	private List<Widget> BuildClimateUI()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Expected O, but got Unknown
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Expected O, but got Unknown
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Expected O, but got Unknown
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Expected O, but got Unknown
		//IL_053d: Expected O, but got Unknown
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Expected O, but got Unknown
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Expected O, but got Unknown
		//IL_05a3: Expected O, but got Unknown
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Expected O, but got Unknown
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_066a: Expected O, but got Unknown
		//IL_066f: Expected O, but got Unknown
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0675: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Expected O, but got Unknown
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Expected O, but got Unknown
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Expected O, but got Unknown
		if (!GameManager.instance.gameMode.IsGameOrEditor())
		{
			return null;
		}
		ClimateRenderSystem climateRenderSystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<ClimateRenderSystem>();
		ClimateSystem climateSystem = ((ComponentSystemBase)this).World.GetExistingSystemManaged<ClimateSystem>();
		Foldout val = new Foldout
		{
			displayName = "Overrideable Properties"
		};
		foreach (KeyValuePair<Type, WeatherPropertiesStack.InterpolatedProperties> component in climateRenderSystem.propertiesStack.components)
		{
			Foldout val2 = new Foldout
			{
				displayName = component.Key.Name,
				opened = true
			};
			Foldout val3 = new Foldout
			{
				displayName = "Current"
			};
			FieldInfo[] fieldsInfo = component.Value.current.GetFieldsInfo();
			for (int i = 0; i < component.Value.current.parameters.Count; i++)
			{
				((Container)val3).children.Add(CreateVolumeParameterWidget(StringUtils.Nicify(fieldsInfo[i].Name) ?? "", component.Value.current.parameters[i]));
			}
			((Container)val2).children.Add((Widget)(object)val3);
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "Interpolation mode",
				getter = () => component.Value.target.m_InterpolationMode
			});
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "Interpolation time",
				getter = () => component.Value.target.m_InterpolationTime
			});
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "Current lerp factor",
				getter = () => component.Value.time
			});
			Foldout val4 = new Foldout
			{
				displayName = "Previous"
			};
			for (int num = 0; num < component.Value.previous.parameters.Count; num++)
			{
				((Container)val4).children.Add(CreateVolumeParameterWidget(StringUtils.Nicify(fieldsInfo[num].Name) ?? "", component.Value.previous.parameters[num]));
			}
			((Container)val2).children.Add((Widget)(object)val4);
			Foldout val5 = new Foldout
			{
				displayName = "Target"
			};
			for (int num2 = 0; num2 < component.Value.target.parameters.Count; num2++)
			{
				((Container)val5).children.Add(CreateVolumeParameterWidget(StringUtils.Nicify(fieldsInfo[num2].Name) ?? "", component.Value.target.parameters[num2]));
			}
			((Container)val2).children.Add((Widget)(object)val5);
			Foldout val6 = new Foldout
			{
				displayName = "From"
			};
			for (int num3 = 0; num3 < component.Value.from.parameters.Count; num3++)
			{
				((Container)val6).children.Add(CreateVolumeParameterWidget(StringUtils.Nicify(fieldsInfo[num3].Name) ?? "", component.Value.from.parameters[num3]));
			}
			((Container)val2).children.Add((Widget)(object)val6);
			Foldout val7 = new Foldout
			{
				displayName = "To"
			};
			for (int num4 = 0; num4 < component.Value.to.parameters.Count; num4++)
			{
				((Container)val7).children.Add(CreateVolumeParameterWidget(StringUtils.Nicify(fieldsInfo[num4].Name) ?? "", component.Value.to.parameters[num4]));
			}
			((Container)val2).children.Add((Widget)(object)val7);
			((Container)val).children.Add((Widget)(object)val2);
		}
		Foldout val8 = new Foldout
		{
			displayName = "Active weathers"
		};
		for (int num5 = 0; num5 < 5; num5++)
		{
			int index = num5;
			((Container)val8).children.Add((Widget)new Value
			{
				displayName = $"From #{num5}",
				getter = () => (index >= climateRenderSystem.fromWeatherPrefabs.Count) ? "None" : ((Object)climateRenderSystem.fromWeatherPrefabs[index]).name
			});
			((Container)val8).children.Add((Widget)new Value
			{
				displayName = $"To #{num5}",
				getter = () => (index >= climateRenderSystem.toWeatherPrefabs.Count) ? "None" : ((Object)climateRenderSystem.toWeatherPrefabs[index]).name
			});
		}
		List<Widget> list = new List<Widget>();
		BoolField val9 = new BoolField
		{
			displayName = "Edit mode"
		};
		((Field<bool>)val9).getter = () => climateRenderSystem.editMode;
		((Field<bool>)val9).setter = delegate(bool value)
		{
			climateRenderSystem.editMode = value;
		};
		list.Add((Widget)val9);
		list.Add((Widget)(object)OverridableProperty("Climate time", () => climateSystem.currentDate));
		BoolField val10 = new BoolField
		{
			displayName = "Fast forward"
		};
		((Field<bool>)val10).getter = () => m_FastForwardClimateTime;
		((Field<bool>)val10).setter = delegate(bool value)
		{
			m_FastForwardClimateTime = value;
		};
		list.Add((Widget)val10);
		list.Add((Widget)(object)OverridableProperty("Cloudiness", () => climateSystem.cloudiness));
		list.Add((Widget)(object)OverridableProperty("Precipitation", () => climateSystem.precipitation));
		list.Add((Widget)(object)OverridableProperty("Aurora", () => climateSystem.aurora));
		list.Add((Widget)(object)val8);
		list.Add((Widget)(object)val);
		BoolField val11 = new BoolField
		{
			displayName = "Pause sim on lightning"
		};
		((Field<bool>)val11).getter = () => climateRenderSystem.pauseSimulationOnLightning;
		((Field<bool>)val11).setter = delegate(bool value)
		{
			climateRenderSystem.pauseSimulationOnLightning = value;
		};
		list.Add((Widget)val11);
		list.Add((Widget)new Button
		{
			displayName = "Lightning Strike",
			action = delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				climateRenderSystem.LightningStrike(m_LastSelectionPosition, m_LastSelectionPosition);
			}
		});
		return list;
	}

	[DebugTab("ECS Components", 0)]
	private List<Widget> BuildECSComponentsDebugUI()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_003f: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_0079: Expected O, but got Unknown
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		List<Widget> list = new List<Widget>();
		TextField val = new TextField
		{
			displayName = "Archetype Filter"
		};
		((Field<string>)val).getter = () => m_CommonComponentsFilter;
		((Field<string>)val).setter = delegate(string value)
		{
			m_CommonComponentsFilter = value.Trim();
		};
		list.Add((Widget)val);
		BoolField val2 = new BoolField
		{
			displayName = "Exclude used archetypes"
		};
		((Field<bool>)val2).getter = () => m_CommonComponentsUnused;
		((Field<bool>)val2).setter = delegate(bool value)
		{
			m_CommonComponentsUnused = value;
		};
		list.Add((Widget)val2);
		list.Add((Widget)new Button
		{
			displayName = "Refresh",
			action = delegate
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				m_CommonComponents = ComponentDebugUtils.GetCommonComponents(((ComponentSystemBase)this).EntityManager, m_CommonComponentsFilter, m_CommonComponentsUnused, out m_ArchetypeCount, out m_FilteredArchetypeCount, out m_ChunkCount, out m_ChunkCapacity, out m_EntityCount);
				Rebuild(BuildECSComponentsDebugUI);
			}
		});
		List<Widget> list2 = list;
		if (m_CommonComponents != null)
		{
			list2.Add((Widget)new Value
			{
				displayName = "Total Entity Count",
				getter = () => m_EntityCount
			});
			list2.Add((Widget)new Value
			{
				displayName = "Total Chunk Count",
				getter = () => m_ChunkCount
			});
			list2.Add((Widget)new Value
			{
				displayName = "Total Archetype Count",
				getter = () => m_ArchetypeCount
			});
			list2.Add((Widget)new Value
			{
				displayName = "Matching Archetype Count",
				getter = () => m_FilteredArchetypeCount
			});
			Container val3 = new Container
			{
				displayName = (string.IsNullOrEmpty(m_CommonComponentsFilter) ? "Common components" : "Common components on matching archetypes")
			};
			val3.children.Add((IEnumerable<Widget>)((IEnumerable<ComponentDebugUtils.ComponentInfo>)m_CommonComponents).Select((Func<ComponentDebugUtils.ComponentInfo, Value>)delegate(ComponentDebugUtils.ComponentInfo info)
			{
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0089: Expected O, but got Unknown
				string value = ((info.m_ChunkCapacity > 0) ? $"{info.m_ArchetypeCount}, {100f * (float)info.m_EntityCount / (float)info.m_ChunkCapacity:F1}%" : info.m_ArchetypeCount.ToString());
				return new Value
				{
					displayName = info.m_Type.FullName.Replace("Game.", ""),
					getter = () => value
				};
			}));
			list2.Add((Widget)val3);
		}
		return list2;
	}

	Action IDebugData.GetReset()
	{
		return delegate
		{
		};
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0501: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Unknown result type (might be due to invalid IL or missing references)
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Expected O, but got Unknown
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0711: Unknown result type (might be due to invalid IL or missing references)
		//IL_0716: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Expected O, but got Unknown
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0868: Expected O, but got Unknown
		base.OnCreate();
		((ComponentSystemBase)this).Enabled = false;
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomCoverageSystem>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_ZoneSpawnSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneSpawnSystem>();
		m_AreaSpawnSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaSpawnSystem>();
		m_BuildingUpkeepSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BuildingUpkeepSystem>();
		m_HouseholdFindPropertySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<HouseholdFindPropertySystem>();
		m_GraduationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GraduationSystem>();
		m_CrimeCheckSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CrimeCheckSystem>();
		m_ApplyToSchoolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ApplyToSchoolSystem>();
		m_FindSchoolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<FindSchoolSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_SelectedInfoUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectedInfoUISystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_DebugUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DebugUISystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_SnowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SnowSystem>();
		m_PlanetarySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PlanetarySystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_ClimateRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateRenderSystem>();
		m_TimeSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TimeSystem>();
		m_CityStatisticsSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityStatisticsSystem>();
		m_ElectricityFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ElectricityFlowSystem>();
		m_AdjustElectricityConsumptionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AdjustElectricityConsumptionSystem>();
		m_WaterPipeFlowSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterPipeFlowSystem>();
		m_DispatchWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DispatchWaterSystem>();
		m_TripNeededSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TripNeededSystem>();
		m_LifePathEventSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LifePathEventSystem>();
		m_BirthSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BirthSystem>();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_UnlockAllSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UnlockAllSystem>();
		m_SignatureBuildingUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SignatureBuildingUISystem>();
		m_DebugQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Tools.Debug>() });
		m_PolicyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PolicyData>() });
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_PolicyEventArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Common.Event>(),
			ComponentType.ReadWrite<Modify>()
		});
		m_NetQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<NetData>(),
			ComponentType.ReadOnly<NetGeometryData>()
		});
		m_ObjectQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ObjectData>(),
			ComponentType.ReadOnly<ObjectGeometryData>(),
			ComponentType.ReadOnly<PlaceableObjectData>()
		});
		m_ZoneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ZoneData>()
		});
		m_AreaQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<AreaData>()
		});
		m_RouteQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<RouteData>()
		});
		m_TerraformingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<TerraformingData>()
		});
		m_InfoviewQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<InfoviewData>()
		});
		m_ThemeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<ThemeData>()
		});
		m_ExtractorParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ExtractorParameterData>() });
		m_HealthcareParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HealthcareParameterData>() });
		m_ParkParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ParkParameterData>() });
		m_EducationParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EducationParameterData>() });
		m_TelecomParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TelecomParameterData>() });
		m_GarbageParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GarbageParameterData>() });
		m_PoliceParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PoliceConfigurationData>() });
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_DemandParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<DemandParameterData>() });
		m_PollutionParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PollutionParameterData>() });
		m_CitizenHappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
		m_LandValueQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<LandValue>() });
		m_RenterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<PropertyRenter>() });
		m_ServiceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<ServiceAvailable>(),
			ComponentType.ReadWrite<PropertyRenter>()
		});
		m_TradeCostQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<TradeCost>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>()
		});
		m_TransferQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<StorageTransferRequest>() });
		m_TripNeededQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadWrite<TripNeeded>() });
		m_EventQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EventData>() });
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabRef>() };
		array[0] = val;
		m_SelectableQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_HouseholdGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.Exclude<Deleted>()
		});
		m_CompanyGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<CompanyData>(),
			ComponentType.Exclude<Deleted>()
		});
		m_HouseholdMemberGroup = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<HouseholdMember>(),
			ComponentType.Exclude<Deleted>()
		});
		m_AtmosphereQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<AtmosphereData>() });
		m_BiomeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<BiomeData>() });
		m_IconQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Icon>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array2 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<SignatureBuildingData>(),
			ComponentType.ReadOnly<UIObjectData>()
		};
		array2[0] = val;
		m_SignatureBuildingQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array2);
		Type[] allTypesDerivedFromAsArray = ReflectionUtils.GetAllTypesDerivedFromAsArray<ToolBaseSystem>(true);
		m_ToolSystems = new ToolBaseSystem[allTypesDerivedFromAsArray.Length];
		m_ToolSystemNames = (GUIContent[])(object)new GUIContent[allTypesDerivedFromAsArray.Length];
		for (int i = 0; i < allTypesDerivedFromAsArray.Length; i++)
		{
			Type type = allTypesDerivedFromAsArray[i];
			m_ToolSystems[i] = (ToolBaseSystem)(object)((ComponentSystemBase)this).World.GetOrCreateSystemManaged(type);
			m_ToolSystemNames[i] = new GUIContent(m_ToolSystems[i].toolID);
		}
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		CoreUtils.Destroy((Object)(object)m_DebugBlitVolume);
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		SelectedInfoUISystem selectedInfoUISystem = m_SelectedInfoUISystem;
		selectedInfoUISystem.eventSelectionChanged = (Action<Entity, Entity, float3>)Delegate.Combine(selectedInfoUISystem.eventSelectionChanged, new Action<Entity, Entity, float3>(OnSelectionChanged));
		RegisterDebug();
		m_LastToolPrefab = m_ToolSystem.activePrefab;
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		UnregisterDebug();
		SelectedInfoUISystem selectedInfoUISystem = m_SelectedInfoUISystem;
		selectedInfoUISystem.eventSelectionChanged = (Action<Entity, Entity, float3>)Delegate.Remove(selectedInfoUISystem.eventSelectionChanged, new Action<Entity, Entity, float3>(OnSelectionChanged));
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (m_FastForwardClimateTime)
		{
			OverridableProperty<float> currentDate = m_ClimateSystem.currentDate;
			float overrideValue = currentDate.overrideValue;
			WorldUnmanaged worldUnmanaged = ((SystemState)(ref ((SystemBase)this).CheckedStateRef)).WorldUnmanaged;
			currentDate.overrideValue = overrideValue + ((WorldUnmanaged)(ref worldUnmanaged)).Time.DeltaTime * 0.001f;
			m_ClimateSystem.currentDate.overrideValue = Mathf.Repeat(m_ClimateSystem.currentDate.overrideValue, 1f);
		}
		if ((Object)(object)m_ToolSystem.activePrefab != (Object)(object)m_LastToolPrefab)
		{
			m_LastToolPrefab = m_ToolSystem.activePrefab;
			Rebuild(BuildSimulationDebugUI);
		}
	}

	public void Restart()
	{
		((ComponentSystemBase)this).OnStopRunning();
		((ComponentSystemBase)this).OnStartRunning();
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		if (((ComponentSystemBase)this).Enabled && mode == GameMode.MainMenu)
		{
			Restart();
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		if (((ComponentSystemBase)this).Enabled)
		{
			Restart();
		}
	}

	public static void Rebuild(Func<World, List<Widget>> method)
	{
		Rebuild((Delegate)method);
	}

	public static void Rebuild(Func<List<Widget>> method)
	{
		Rebuild((Delegate)method);
	}

	private static void Rebuild(Delegate method)
	{
		try
		{
			World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
			DebugSystem debugSystem = ((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetExistingSystemManaged<DebugSystem>() : null);
			if (debugSystem != null)
			{
				DebugTabAttribute attribute = default(DebugTabAttribute);
				if (!ReflectionUtils.TryGetAttribute<DebugTabAttribute>(method.Method, ref attribute, false))
				{
					throw new ArgumentException(method.Method.Name + " is missing the DebugTabAttribute");
				}
				debugSystem.ExecuteMethod(method.Method, method.Target, attribute);
			}
		}
		catch (Exception ex)
		{
			COSystemBase.baseLog.Error(ex, (object)("Failed to register '" + method.Method.Name + "' Debug UI"));
		}
	}

	private void ExecuteMethod(MethodInfo method, object target, DebugTabAttribute attribute)
	{
		ParameterInfo[] parameters = method.GetParameters();
		List<Widget> list = null;
		if (parameters.Length == 0)
		{
			if (target != null)
			{
				list = ReflectionUtils.Invoke<object, List<Widget>>(method, target, Array.Empty<object>());
			}
			else if (typeof(ComponentSystemBase).IsAssignableFrom(method.DeclaringType))
			{
				ComponentSystemBase orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged(method.DeclaringType);
				list = ReflectionUtils.Invoke<ComponentSystemBase, List<Widget>>(method, orCreateSystemManaged, Array.Empty<object>());
			}
			else
			{
				list = ReflectionUtils.Invoke<List<Widget>>(method, Array.Empty<object>());
			}
		}
		else if (parameters.Length == 1)
		{
			list = ((target == null) ? ReflectionUtils.Invoke<List<Widget>>(method, new object[1] { ((ComponentSystemBase)this).World }) : ReflectionUtils.Invoke<object, List<Widget>>(method, target, new object[1] { ((ComponentSystemBase)this).World }));
		}
		if (list != null)
		{
			AddPanel(attribute.name, list, attribute.priority);
		}
	}

	private void RegisterDebug()
	{
		foreach (Type item in ReflectionUtils.GetAllConcreteTypesWithAttribute<DebugContainerAttribute>(true))
		{
			object target = CreateDebugClass(item);
			foreach (var item2 in ReflectionUtils.GetAllMethodsWithAttribute<DebugTabAttribute>(item, false, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				try
				{
					ExecuteMethod(item2.Item1, target, item2.Item2);
				}
				catch (Exception ex)
				{
					COSystemBase.baseLog.Error(ex, (object)("Failed to register '" + item2.Item2.name + "' Debug UI"));
				}
			}
		}
		Rebuild(BuildSimulationDebugUI);
		DebugManager.instance.RegisterData((IDebugData)(object)this);
	}

	private void UnregisterDebug()
	{
		foreach (KeyValuePair<string, List<Widget>> item in m_Panels)
		{
			UnregisterDebugItems(item.Key, item.Value);
		}
		DisposeDebugClasses();
		DebugManager.instance.UnregisterData((IDebugData)(object)this);
	}

	private object CreateDebugClass(Type type)
	{
		if (!type.IsAbstract && !typeof(ComponentSystemBase).IsAssignableFrom(type))
		{
			object obj = Activator.CreateInstance(type);
			m_DebugClasses.Add(obj);
			return obj;
		}
		return null;
	}

	private void DisposeDebugClasses()
	{
		foreach (object item in m_DebugClasses)
		{
			if (item is IDisposable disposable)
			{
				disposable.Dispose();
			}
		}
		m_DebugClasses.Clear();
	}

	private void UnregisterDebugItems(string panelName, IEnumerable<Widget> items)
	{
		Panel panel = DebugManager.instance.GetPanel(panelName, false, 0, false);
		if (panel != null)
		{
			panel.children.Remove(items);
		}
	}

	private IContainer AddPanel(string name, List<Widget> widgets, int groupIndex = -1, bool overrideIfExists = false)
	{
		Panel panel = DebugManager.instance.GetPanel(name, true, groupIndex, overrideIfExists);
		if (m_Panels.TryGetValue(name, out var value) && panel != null)
		{
			panel.children.Remove(value);
		}
		m_Panels[name] = widgets;
		panel.children.Add(widgets);
		return (IContainer)(object)panel;
	}

	private EnumField ToggleSelection(string displayName, Func<bool> getter, Action<bool> setter, Action<Field<int>, int> onValueChanged = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		EnumField val = new EnumField
		{
			displayName = displayName
		};
		((Field<int>)val).getter = () => getter() ? 1 : 0;
		((Field<int>)val).setter = delegate(int value)
		{
			setter(value != 0);
		};
		val.autoEnum = typeof(ToggleEnum);
		((Field<int>)val).onValueChanged = onValueChanged;
		val.getIndex = () => getter() ? 1 : 0;
		val.setIndex = delegate
		{
		};
		return val;
	}

	private Widget RadioSelection<T>(string displayName, Func<T> getter, Action<T> setter, GUIContent[] names, T[] values, Action onValueChanged = null) where T : IEquatable<T>
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00cb: Expected O, but got Unknown
		int[] array = new int[values.Length];
		for (int i = 0; i < values.Length; i++)
		{
			array[i] = i;
		}
		EnumField val = new EnumField
		{
			displayName = displayName
		};
		((Field<int>)val).getter = () => GetIndex();
		((Field<int>)val).setter = delegate(int value)
		{
			SetValue(value);
		};
		((EnumField<int>)val).enumNames = names;
		((EnumField<int>)val).enumValues = array;
		val.getIndex = () => GetIndex();
		val.setIndex = delegate(int value)
		{
			SetValue(value);
		};
		((Field<int>)val).onValueChanged = delegate
		{
			onValueChanged?.Invoke();
		};
		return (Widget)val;
		int GetIndex()
		{
			for (int j = 0; j < values.Length; j++)
			{
				if (getter().Equals(values[j]))
				{
					return j;
				}
			}
			return names.Length - 1;
		}
		void SetValue(int index)
		{
			if (index >= 0 && index < values.Length)
			{
				setter(values[index]);
			}
		}
	}

	private int[] GetWaterSpeedValues()
	{
		int[] debugWaterSpeedValues = m_DebugWaterSpeedValues;
		debugWaterSpeedValues[debugWaterSpeedValues.Length - 1] = m_WaterSystem.MaxSpeed;
		return m_DebugWaterSpeedValues;
	}

	private T GetTool<T>() where T : ToolBaseSystem
	{
		return ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<T>();
	}

	private void OnSelectionChanged(Entity entity, Entity prefab, float3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_LastSelectionPosition = position;
		Rebuild(BuildSimulationDebugUI);
	}

	private string EditorDate()
	{
		return m_TimeSystem.GetCurrentDateTime().Hour + ":" + m_TimeSystem.GetCurrentDateTime().Minute.ToString("D2") + ", Month " + m_TimeSystem.GetCurrentDateTime().Day;
	}

	private Container OverridableProperty<T>(string displayName, Func<T> getter, float min = 0f, float max = 1f, float incStep = 0.001f) where T : OverridableProperty<float>
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00c4: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		ObservableList<Widget> obj = new ObservableList<Widget>();
		FloatField val = new FloatField
		{
			displayName = displayName
		};
		((Field<float>)val).getter = () => getter();
		((Field<float>)val).setter = delegate(float value)
		{
			getter().overrideValue = value;
		};
		val.min = () => min;
		val.max = () => max;
		val.incStep = incStep;
		obj.Add((Widget)val);
		BoolField val2 = new BoolField
		{
			displayName = "Override " + displayName
		};
		((Field<bool>)val2).getter = () => getter().overrideState;
		((Field<bool>)val2).setter = delegate(bool value)
		{
			getter().overrideState = value;
		};
		obj.Add((Widget)val2);
		return new Container(displayName, obj);
	}

	private void RebuildSimulationDebugUI<T>(Field<T> field, T value)
	{
		Rebuild(BuildSimulationDebugUI);
	}

	[DebugTab("Simulation", -1000)]
	private List<Widget> BuildSimulationDebugUI()
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Expected O, but got Unknown
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Expected O, but got Unknown
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Expected O, but got Unknown
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Expected O, but got Unknown
		//IL_02a3: Expected O, but got Unknown
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Expected O, but got Unknown
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Expected O, but got Unknown
		//IL_02dd: Expected O, but got Unknown
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Expected O, but got Unknown
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Expected O, but got Unknown
		//IL_0317: Expected O, but got Unknown
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Expected O, but got Unknown
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Expected O, but got Unknown
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Expected O, but got Unknown
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Expected O, but got Unknown
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Expected O, but got Unknown
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Expected O, but got Unknown
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Expected O, but got Unknown
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Expected O, but got Unknown
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Expected O, but got Unknown
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Expected O, but got Unknown
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Expected O, but got Unknown
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Expected O, but got Unknown
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Expected O, but got Unknown
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Expected O, but got Unknown
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0677: Expected O, but got Unknown
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_069f: Expected O, but got Unknown
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Expected O, but got Unknown
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ef: Expected O, but got Unknown
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Expected O, but got Unknown
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Expected O, but got Unknown
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Expected O, but got Unknown
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Expected O, but got Unknown
		//IL_07a6: Expected O, but got Unknown
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Expected O, but got Unknown
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07db: Expected O, but got Unknown
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Expected O, but got Unknown
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Expected O, but got Unknown
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_086a: Expected O, but got Unknown
		//IL_086f: Expected O, but got Unknown
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0892: Expected O, but got Unknown
		//IL_0892: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Expected O, but got Unknown
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f3: Expected O, but got Unknown
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Expected O, but got Unknown
		//IL_091c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_092c: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Expected O, but got Unknown
		//IL_093e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0950: Expected O, but got Unknown
		//IL_0950: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_099a: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ac: Expected O, but got Unknown
		//IL_09b1: Expected O, but got Unknown
		//IL_09b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d4: Expected O, but got Unknown
		//IL_09d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e6: Expected O, but got Unknown
		//IL_09eb: Expected O, but got Unknown
		//IL_09ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0e: Expected O, but got Unknown
		//IL_0a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a20: Expected O, but got Unknown
		//IL_0a25: Expected O, but got Unknown
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a48: Expected O, but got Unknown
		//IL_0a48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5a: Expected O, but got Unknown
		//IL_0a5f: Expected O, but got Unknown
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a70: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a82: Expected O, but got Unknown
		//IL_0a82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a94: Expected O, but got Unknown
		//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aee: Expected O, but got Unknown
		//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Expected O, but got Unknown
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b23: Expected O, but got Unknown
		//IL_0b23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7d: Expected O, but got Unknown
		//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba0: Expected O, but got Unknown
		//IL_0ba0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb2: Expected O, but got Unknown
		//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c0c: Expected O, but got Unknown
		//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c18: Expected O, but got Unknown
		//IL_0c50: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c77: Expected O, but got Unknown
		//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9f: Expected O, but got Unknown
		//IL_0cab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd2: Expected O, but got Unknown
		//IL_0cd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfa: Expected O, but got Unknown
		//IL_0cfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d00: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d22: Expected O, but got Unknown
		//IL_0d24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2e: Expected O, but got Unknown
		//IL_0d66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d8d: Expected O, but got Unknown
		//IL_0d8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db5: Expected O, but got Unknown
		//IL_0db6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddd: Expected O, but got Unknown
		//IL_0ddf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de9: Expected O, but got Unknown
		//IL_0ea3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eca: Expected O, but got Unknown
		//IL_0ecb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0edb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef2: Expected O, but got Unknown
		//IL_0ef3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1a: Expected O, but got Unknown
		//IL_0f1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f42: Expected O, but got Unknown
		//IL_0f43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f65: Expected O, but got Unknown
		//IL_0f65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f77: Expected O, but got Unknown
		//IL_0f77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd1: Expected O, but got Unknown
		//IL_0fd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff4: Expected O, but got Unknown
		//IL_0ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1006: Expected O, but got Unknown
		//IL_100b: Expected O, but got Unknown
		//IL_100c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1011: Unknown result type (might be due to invalid IL or missing references)
		//IL_101c: Unknown result type (might be due to invalid IL or missing references)
		//IL_102e: Expected O, but got Unknown
		//IL_102e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1040: Expected O, but got Unknown
		//IL_1045: Expected O, but got Unknown
		//IL_1046: Unknown result type (might be due to invalid IL or missing references)
		//IL_104b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1056: Unknown result type (might be due to invalid IL or missing references)
		//IL_106d: Expected O, but got Unknown
		//IL_106f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1079: Expected O, but got Unknown
		//IL_1085: Unknown result type (might be due to invalid IL or missing references)
		//IL_108a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1095: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ac: Expected O, but got Unknown
		//IL_10ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cf: Expected O, but got Unknown
		//IL_10cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e1: Expected O, but got Unknown
		//IL_10e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1106: Unknown result type (might be due to invalid IL or missing references)
		//IL_1130: Expected O, but got Unknown
		//IL_1131: Unknown result type (might be due to invalid IL or missing references)
		//IL_1136: Unknown result type (might be due to invalid IL or missing references)
		//IL_1141: Unknown result type (might be due to invalid IL or missing references)
		//IL_1153: Expected O, but got Unknown
		//IL_1153: Unknown result type (might be due to invalid IL or missing references)
		//IL_1165: Expected O, but got Unknown
		//IL_1165: Unknown result type (might be due to invalid IL or missing references)
		//IL_118a: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b4: Expected O, but got Unknown
		//IL_11b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d7: Expected O, but got Unknown
		//IL_11d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e9: Expected O, but got Unknown
		//IL_11e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_120e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1238: Expected O, but got Unknown
		//IL_1239: Unknown result type (might be due to invalid IL or missing references)
		//IL_123e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1249: Unknown result type (might be due to invalid IL or missing references)
		//IL_125b: Expected O, but got Unknown
		//IL_125b: Unknown result type (might be due to invalid IL or missing references)
		//IL_126d: Expected O, but got Unknown
		//IL_126d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1292: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bc: Expected O, but got Unknown
		//IL_12bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_12df: Expected O, but got Unknown
		//IL_12df: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f1: Expected O, but got Unknown
		//IL_12f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_131b: Expected O, but got Unknown
		//IL_131c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1321: Unknown result type (might be due to invalid IL or missing references)
		//IL_132c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1343: Expected O, but got Unknown
		//IL_1344: Unknown result type (might be due to invalid IL or missing references)
		//IL_1349: Unknown result type (might be due to invalid IL or missing references)
		//IL_1354: Unknown result type (might be due to invalid IL or missing references)
		//IL_1366: Expected O, but got Unknown
		//IL_1366: Unknown result type (might be due to invalid IL or missing references)
		//IL_1378: Expected O, but got Unknown
		//IL_137d: Expected O, but got Unknown
		//IL_137e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1383: Unknown result type (might be due to invalid IL or missing references)
		//IL_138e: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a0: Expected O, but got Unknown
		//IL_13a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b2: Expected O, but got Unknown
		//IL_13b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1401: Expected O, but got Unknown
		//IL_1402: Unknown result type (might be due to invalid IL or missing references)
		//IL_1407: Unknown result type (might be due to invalid IL or missing references)
		//IL_1412: Unknown result type (might be due to invalid IL or missing references)
		//IL_1429: Expected O, but got Unknown
		//IL_142a: Unknown result type (might be due to invalid IL or missing references)
		//IL_142f: Unknown result type (might be due to invalid IL or missing references)
		//IL_143a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1451: Expected O, but got Unknown
		//IL_1452: Unknown result type (might be due to invalid IL or missing references)
		//IL_1457: Unknown result type (might be due to invalid IL or missing references)
		//IL_1462: Unknown result type (might be due to invalid IL or missing references)
		//IL_1479: Expected O, but got Unknown
		//IL_147b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1485: Expected O, but got Unknown
		//IL_1491: Unknown result type (might be due to invalid IL or missing references)
		//IL_1496: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b8: Expected O, but got Unknown
		//IL_14b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_14be: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f3: Expected O, but got Unknown
		//IL_14f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1504: Unknown result type (might be due to invalid IL or missing references)
		//IL_152e: Expected O, but got Unknown
		//IL_152f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1534: Unknown result type (might be due to invalid IL or missing references)
		//IL_153f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1556: Expected O, but got Unknown
		//IL_1557: Unknown result type (might be due to invalid IL or missing references)
		//IL_155c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1567: Unknown result type (might be due to invalid IL or missing references)
		//IL_157e: Expected O, but got Unknown
		//IL_157f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1584: Unknown result type (might be due to invalid IL or missing references)
		//IL_158f: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b9: Expected O, but got Unknown
		//IL_15ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e1: Expected O, but got Unknown
		//IL_15e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1609: Expected O, but got Unknown
		//IL_160a: Unknown result type (might be due to invalid IL or missing references)
		//IL_160f: Unknown result type (might be due to invalid IL or missing references)
		//IL_161a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1631: Expected O, but got Unknown
		//IL_1632: Unknown result type (might be due to invalid IL or missing references)
		//IL_1637: Unknown result type (might be due to invalid IL or missing references)
		//IL_1642: Unknown result type (might be due to invalid IL or missing references)
		//IL_1659: Expected O, but got Unknown
		//IL_165a: Unknown result type (might be due to invalid IL or missing references)
		//IL_165f: Unknown result type (might be due to invalid IL or missing references)
		//IL_166a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1681: Expected O, but got Unknown
		//IL_1682: Unknown result type (might be due to invalid IL or missing references)
		//IL_1687: Unknown result type (might be due to invalid IL or missing references)
		//IL_1692: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a9: Expected O, but got Unknown
		//IL_16aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_16af: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d1: Expected O, but got Unknown
		//IL_16d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f9: Expected O, but got Unknown
		//IL_16fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_170a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1721: Expected O, but got Unknown
		//IL_1722: Unknown result type (might be due to invalid IL or missing references)
		//IL_1727: Unknown result type (might be due to invalid IL or missing references)
		//IL_1732: Unknown result type (might be due to invalid IL or missing references)
		//IL_1749: Expected O, but got Unknown
		//IL_174a: Unknown result type (might be due to invalid IL or missing references)
		//IL_174f: Unknown result type (might be due to invalid IL or missing references)
		//IL_175a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1771: Expected O, but got Unknown
		//IL_1772: Unknown result type (might be due to invalid IL or missing references)
		//IL_1777: Unknown result type (might be due to invalid IL or missing references)
		//IL_1782: Unknown result type (might be due to invalid IL or missing references)
		//IL_1799: Expected O, but got Unknown
		//IL_179a: Unknown result type (might be due to invalid IL or missing references)
		//IL_179f: Unknown result type (might be due to invalid IL or missing references)
		//IL_17aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c1: Expected O, but got Unknown
		//IL_17c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e9: Expected O, but got Unknown
		//IL_17ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1811: Expected O, but got Unknown
		//IL_185e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1863: Unknown result type (might be due to invalid IL or missing references)
		//IL_186e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1885: Expected O, but got Unknown
		//IL_1886: Unknown result type (might be due to invalid IL or missing references)
		//IL_188b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1896: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ad: Expected O, but got Unknown
		//IL_18ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18be: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d5: Expected O, but got Unknown
		//IL_18d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_18db: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fd: Expected O, but got Unknown
		//IL_18fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1903: Unknown result type (might be due to invalid IL or missing references)
		//IL_190e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1925: Expected O, but got Unknown
		//IL_1927: Unknown result type (might be due to invalid IL or missing references)
		//IL_1931: Expected O, but got Unknown
		//IL_193d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1942: Unknown result type (might be due to invalid IL or missing references)
		//IL_194d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1964: Expected O, but got Unknown
		//IL_1965: Unknown result type (might be due to invalid IL or missing references)
		//IL_196a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1975: Unknown result type (might be due to invalid IL or missing references)
		//IL_1987: Expected O, but got Unknown
		//IL_1987: Unknown result type (might be due to invalid IL or missing references)
		//IL_1999: Expected O, but got Unknown
		//IL_199e: Expected O, but got Unknown
		//IL_199f: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_19af: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c1: Expected O, but got Unknown
		//IL_19c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d3: Expected O, but got Unknown
		//IL_19d8: Expected O, but got Unknown
		//IL_19d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19de: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19fb: Expected O, but got Unknown
		//IL_19fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0d: Expected O, but got Unknown
		//IL_1a12: Expected O, but got Unknown
		//IL_1a14: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1e: Expected O, but got Unknown
		//IL_1a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a51: Expected O, but got Unknown
		//IL_1a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a79: Expected O, but got Unknown
		//IL_1a7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa7: Expected O, but got Unknown
		//IL_1aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab9: Expected O, but got Unknown
		//IL_1abe: Expected O, but got Unknown
		//IL_1abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ada: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aec: Expected O, but got Unknown
		//IL_1aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afe: Expected O, but got Unknown
		//IL_1b03: Expected O, but got Unknown
		//IL_1b04: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b14: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b31: Expected O, but got Unknown
		//IL_1b31: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b43: Expected O, but got Unknown
		//IL_1b48: Expected O, but got Unknown
		//IL_1b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b59: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b76: Expected O, but got Unknown
		//IL_1b76: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b88: Expected O, but got Unknown
		//IL_1b8d: Expected O, but got Unknown
		//IL_1b8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbb: Expected O, but got Unknown
		//IL_1bbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bcd: Expected O, but got Unknown
		//IL_1bd2: Expected O, but got Unknown
		//IL_1bd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1be3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c00: Expected O, but got Unknown
		//IL_1c00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c12: Expected O, but got Unknown
		//IL_1c17: Expected O, but got Unknown
		//IL_1c18: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c45: Expected O, but got Unknown
		//IL_1c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c57: Expected O, but got Unknown
		//IL_1c5c: Expected O, but got Unknown
		//IL_1c5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c8a: Expected O, but got Unknown
		//IL_1c8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c9c: Expected O, but got Unknown
		//IL_1ca1: Expected O, but got Unknown
		//IL_1ca2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ccf: Expected O, but got Unknown
		//IL_1ccf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce1: Expected O, but got Unknown
		//IL_1ce6: Expected O, but got Unknown
		//IL_1ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d14: Expected O, but got Unknown
		//IL_1d14: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d26: Expected O, but got Unknown
		//IL_1d2b: Expected O, but got Unknown
		//IL_1d2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d31: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d55: Expected O, but got Unknown
		//IL_1d55: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d67: Expected O, but got Unknown
		//IL_1d6c: Expected O, but got Unknown
		//IL_1d6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d72: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d97: Expected O, but got Unknown
		//IL_1d97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da9: Expected O, but got Unknown
		//IL_1dae: Expected O, but got Unknown
		//IL_1daf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd1: Expected O, but got Unknown
		//IL_1dd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de3: Expected O, but got Unknown
		//IL_1de8: Expected O, but got Unknown
		//IL_1de9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e0b: Expected O, but got Unknown
		//IL_1e0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1d: Expected O, but got Unknown
		//IL_1e22: Expected O, but got Unknown
		//IL_1e23: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e50: Expected O, but got Unknown
		//IL_1e50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e62: Expected O, but got Unknown
		//IL_1e67: Expected O, but got Unknown
		//IL_1e68: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e83: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e95: Expected O, but got Unknown
		//IL_1e95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ea7: Expected O, but got Unknown
		//IL_1eac: Expected O, but got Unknown
		//IL_1ead: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ebd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eda: Expected O, but got Unknown
		//IL_1eda: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eec: Expected O, but got Unknown
		//IL_1ef1: Expected O, but got Unknown
		//IL_1ef3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1efd: Expected O, but got Unknown
		//IL_1f15: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f25: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f3c: Expected O, but got Unknown
		//IL_1f3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f48: Expected O, but got Unknown
		//IL_1f54: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f59: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f7b: Expected O, but got Unknown
		//IL_1f7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f87: Expected O, but got Unknown
		//IL_1f93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f98: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fba: Expected O, but got Unknown
		//IL_1fbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fe2: Expected O, but got Unknown
		//IL_1fe3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fe8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff3: Unknown result type (might be due to invalid IL or missing references)
		//IL_200a: Expected O, but got Unknown
		//IL_200c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2016: Expected O, but got Unknown
		//IL_2022: Unknown result type (might be due to invalid IL or missing references)
		//IL_2027: Unknown result type (might be due to invalid IL or missing references)
		//IL_2032: Unknown result type (might be due to invalid IL or missing references)
		//IL_2049: Expected O, but got Unknown
		//IL_204a: Unknown result type (might be due to invalid IL or missing references)
		//IL_204f: Unknown result type (might be due to invalid IL or missing references)
		//IL_205a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2071: Expected O, but got Unknown
		//IL_2072: Unknown result type (might be due to invalid IL or missing references)
		//IL_2077: Unknown result type (might be due to invalid IL or missing references)
		//IL_2082: Unknown result type (might be due to invalid IL or missing references)
		//IL_2099: Expected O, but got Unknown
		//IL_209a: Unknown result type (might be due to invalid IL or missing references)
		//IL_209f: Unknown result type (might be due to invalid IL or missing references)
		//IL_20aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c1: Expected O, but got Unknown
		//IL_20c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_20cd: Expected O, but got Unknown
		//IL_20ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_20fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2110: Expected O, but got Unknown
		//IL_2110: Unknown result type (might be due to invalid IL or missing references)
		//IL_2122: Expected O, but got Unknown
		//IL_2122: Unknown result type (might be due to invalid IL or missing references)
		//IL_2147: Unknown result type (might be due to invalid IL or missing references)
		//IL_2171: Expected O, but got Unknown
		//IL_2172: Unknown result type (might be due to invalid IL or missing references)
		//IL_2177: Unknown result type (might be due to invalid IL or missing references)
		//IL_2182: Unknown result type (might be due to invalid IL or missing references)
		//IL_2194: Expected O, but got Unknown
		//IL_2194: Unknown result type (might be due to invalid IL or missing references)
		//IL_21a6: Expected O, but got Unknown
		//IL_21a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_21cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_21f5: Expected O, but got Unknown
		//IL_21f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_21ff: Expected O, but got Unknown
		//IL_2200: Unknown result type (might be due to invalid IL or missing references)
		//IL_2205: Unknown result type (might be due to invalid IL or missing references)
		//IL_2210: Unknown result type (might be due to invalid IL or missing references)
		//IL_2235: Expected O, but got Unknown
		//IL_2235: Unknown result type (might be due to invalid IL or missing references)
		//IL_225a: Expected O, but got Unknown
		//IL_225f: Expected O, but got Unknown
		if (GameManager.instance.gameMode.IsGameOrEditor())
		{
			string entitySearchQuery = string.Empty;
			AtmosphereData atmosphere = default(AtmosphereData);
			if (!((EntityQuery)(ref m_AtmosphereQuery)).IsEmptyIgnoreFilter)
			{
				atmosphere = ((EntityQuery)(ref m_AtmosphereQuery)).GetSingleton<AtmosphereData>();
			}
			BiomeData biome = default(BiomeData);
			if (!((EntityQuery)(ref m_BiomeQuery)).IsEmptyIgnoreFilter)
			{
				biome = ((EntityQuery)(ref m_BiomeQuery)).GetSingleton<BiomeData>();
			}
			PollutionParameterData pollution = default(PollutionParameterData);
			if (!((EntityQuery)(ref m_PollutionParameterQuery)).IsEmptyIgnoreFilter)
			{
				pollution = ((EntityQuery)(ref m_PollutionParameterQuery)).GetSingleton<PollutionParameterData>();
			}
			List<Widget> obj = new List<Widget>
			{
				RadioSelection("Active tool", () => m_ToolSystem.activeTool, delegate(ToolBaseSystem value)
				{
					m_ToolSystem.activeTool = value;
				}, m_ToolSystemNames, m_ToolSystems, delegate
				{
					Rebuild(BuildSimulationDebugUI);
				}),
				(Widget)(object)CreateToolUI(),
				InfoviewUI(),
				(Widget)new Button
				{
					displayName = "Save game",
					action = delegate
					{
						SaveGame();
					}
				},
				(Widget)new Button
				{
					displayName = "Load game",
					action = delegate
					{
						LoadGame();
					}
				},
				RadioSelection("Sim speed", () => m_SimulationSystem.selectedSpeed, delegate(float value)
				{
					m_SimulationSystem.selectedSpeed = value;
				}, kDebugSimulationSpeedStrings, kDebugSimulationSpeedValues),
				(Widget)new Value
				{
					displayName = "Smooth speed",
					getter = () => m_SimulationSystem.smoothSpeed
				}
			};
			FloatField val = new FloatField
			{
				displayName = "Interpolation offset"
			};
			((Field<float>)val).getter = () => m_RenderingSystem.frameOffset;
			((Field<float>)val).setter = delegate(float value)
			{
				if (m_RenderingSystem.frameOffset != value)
				{
					m_RenderingSystem.frameOffset = value;
				}
			};
			val.min = () => -1f;
			val.max = () => 1f;
			val.incStep = 0.01f;
			obj.Add((Widget)val);
			obj.Add((Widget)new Value
			{
				displayName = "Step Time (ms)",
				getter = () => m_SimulationSystem.frameDuration * 1000f
			});
			BoolField val2 = new BoolField
			{
				displayName = "Disable trips"
			};
			((Field<bool>)val2).getter = () => m_TripNeededSystem.debugDisableSpawning;
			((Field<bool>)val2).setter = delegate(bool value)
			{
				m_TripNeededSystem.debugDisableSpawning = value;
			};
			obj.Add((Widget)val2);
			BoolField val3 = new BoolField
			{
				displayName = "Disable homeless"
			};
			((Field<bool>)val3).getter = () => m_HouseholdFindPropertySystem.debugDisableHomeless;
			((Field<bool>)val3).setter = delegate(bool value)
			{
				RemoveAllHomeless();
				m_HouseholdFindPropertySystem.debugDisableHomeless = value;
			};
			obj.Add((Widget)val3);
			BoolField val4 = new BoolField
			{
				displayName = "Debug Lifepath Chirps "
			};
			((Field<bool>)val4).getter = () => m_LifePathEventSystem.m_DebugLifePathChirps;
			((Field<bool>)val4).setter = delegate(bool value)
			{
				m_LifePathEventSystem.m_DebugLifePathChirps = value;
			};
			obj.Add((Widget)val4);
			obj.Add((Widget)new Button
			{
				displayName = "Remove residents/vehicles",
				action = delegate
				{
					RemoveResidentsAndVehicles();
				}
			});
			obj.Add((Widget)new Button
			{
				displayName = "Cleanup obsolete entities",
				action = delegate
				{
					CleanupObsoleteEntities();
				}
			});
			ObservableList<Widget> obj2 = new ObservableList<Widget>();
			IntField val5 = new IntField
			{
				displayName = "Birth Chance"
			};
			((Field<int>)val5).getter = () => m_BirthSystem.m_BirthChance;
			((Field<int>)val5).setter = delegate(int value)
			{
				m_BirthSystem.m_BirthChance = value;
			};
			val5.min = () => 0;
			val5.max = () => 1000;
			obj2.Add((Widget)val5);
			IntField val6 = new IntField
			{
				displayName = "Speedup graduation"
			};
			((Field<int>)val6).getter = () => m_GraduationSystem.debugFastGraduationLevel;
			((Field<int>)val6).setter = delegate(int value)
			{
				m_GraduationSystem.debugFastGraduationLevel = value;
			};
			val6.min = () => 0;
			val6.max = () => 4;
			obj2.Add((Widget)val6);
			obj2.Add((Widget)(object)ToggleSelection("Full Crime test mode", () => m_CrimeCheckSystem.debugFullCrimeMode, delegate(bool value)
			{
				m_CrimeCheckSystem.debugFullCrimeMode = value;
			}));
			obj2.Add((Widget)(object)ToggleSelection("Superfast building spawning", () => m_ZoneSpawnSystem.debugFastSpawn, delegate(bool value)
			{
				m_ZoneSpawnSystem.debugFastSpawn = value;
			}));
			obj2.Add((Widget)(object)ToggleSelection("Superfast area-prop spawning", () => m_AreaSpawnSystem.debugFastSpawn, delegate(bool value)
			{
				m_AreaSpawnSystem.debugFastSpawn = value;
			}));
			obj2.Add((Widget)(object)ToggleSelection("Superfast leveling", () => m_BuildingUpkeepSystem.debugFastLeveling, delegate(bool value)
			{
				m_BuildingUpkeepSystem.debugFastLeveling = value;
			}));
			obj2.Add((Widget)(object)ToggleSelection("Superfast enter school", () => m_ApplyToSchoolSystem.debugFastApplySchool && m_FindSchoolSystem.debugFastFindSchool, delegate(bool value)
			{
				m_ApplyToSchoolSystem.debugFastApplySchool = value;
				m_FindSchoolSystem.debugFastFindSchool = value;
			}));
			obj2.Add((Widget)new Button
			{
				displayName = "Reset All Crime Accumulation to 0",
				action = delegate
				{
					ResetCrimeAccumulation();
				}
			});
			obj.Add((Widget)new Foldout("Test Mode", obj2, (string[])null, (string[])null));
			ObservableList<Widget> obj3 = new ObservableList<Widget>();
			obj3.Add((Widget)new Value
			{
				displayName = "Atmosphere",
				getter = () => (!m_PrefabSystem.TryGetPrefab<PrefabBase>(atmosphere.m_AtmospherePrefab, out var prefab)) ? "None" : ((Object)prefab).name
			});
			obj3.Add((Widget)new Value
			{
				displayName = "Biome",
				getter = () => (!m_PrefabSystem.TryGetPrefab<PrefabBase>(biome.m_BiomePrefab, out prefab)) ? "None" : ((Object)prefab).name
			});
			obj.Add((Widget)new Foldout("Diversity", obj3, (string[])null, (string[])null));
			ObservableList<Widget> obj4 = new ObservableList<Widget>();
			obj4.Add(RadioSelection("Water sim speed", () => m_WaterSystem.WaterSimSpeed, delegate(int value)
			{
				m_WaterSystem.WaterSimSpeed = value;
			}, kDebugWaterSpeedStrings, GetWaterSpeedValues()));
			obj4.Add((Widget)new Button
			{
				displayName = "Save water",
				action = delegate
				{
					m_WaterSystem.Save();
				}
			});
			obj4.Add((Widget)new Button
			{
				displayName = "Load water",
				action = delegate
				{
					m_WaterSystem.JobLoad();
				}
			});
			obj4.Add((Widget)new Button
			{
				displayName = "Restart water",
				action = delegate
				{
					m_WaterSystem.Restart();
				}
			});
			obj4.Add((Widget)new Button
			{
				displayName = "Water to sealevel",
				action = delegate
				{
					m_WaterSystem.ResetToSealevel();
				}
			});
			obj4.Add((Widget)new Button
			{
				displayName = "Reload Water Sources",
				action = delegate
				{
					ReloadWaterSources();
				}
			});
			FloatField val7 = new FloatField
			{
				displayName = "Time step override"
			};
			((Field<float>)val7).getter = () => m_WaterSystem.TimeStepOverride;
			((Field<float>)val7).setter = delegate(float value)
			{
				m_WaterSystem.TimeStepOverride = value;
			};
			val7.min = () => 0f;
			val7.max = () => 1f;
			val7.incStep = 0.01f;
			obj4.Add((Widget)val7);
			FloatField val8 = new FloatField
			{
				displayName = "Current Time Step"
			};
			((Field<float>)val8).getter = () => m_WaterSystem.GetTimeStep();
			obj4.Add((Widget)val8);
			FloatField val9 = new FloatField
			{
				displayName = "Max Velocity"
			};
			((Field<float>)val9).getter = () => m_WaterSystem.MaxVelocity;
			((Field<float>)val9).setter = delegate(float value)
			{
				m_WaterSystem.MaxVelocity = value;
			};
			val9.min = () => 0f;
			val9.max = () => 16f;
			val9.incStep = 0.025f;
			obj4.Add((Widget)val9);
			BoolField val10 = new BoolField
			{
				displayName = "Use Active Cells Culling"
			};
			((Field<bool>)val10).getter = () => m_WaterSystem.UseActiveCellsCulling;
			((Field<bool>)val10).setter = delegate(bool value)
			{
				m_WaterSystem.UseActiveCellsCulling = value;
			};
			obj4.Add((Widget)val10);
			IntField val11 = new IntField
			{
				displayName = "Water Grid Size Multiplier"
			};
			((Field<int>)val11).getter = () => m_WaterSystem.GridSizeMultiplier;
			((Field<int>)val11).setter = delegate(int value)
			{
				m_WaterSystem.GridSizeMultiplier = value;
			};
			val11.min = () => 0;
			val11.max = () => 6;
			obj4.Add((Widget)val11);
			obj4.Add((Widget)new Value
			{
				displayName = "Water grid size",
				getter = () => $"{m_WaterSystem.GridSize} {2048 / m_WaterSystem.GridSize}x{2048 / m_WaterSystem.GridSize}"
			});
			IntField val12 = new IntField
			{
				displayName = "Flow number of Downscale"
			};
			((Field<int>)val12).getter = () => m_WaterSystem.FlowMapNumDownscale;
			((Field<int>)val12).setter = delegate(int value)
			{
				m_WaterSystem.FlowMapNumDownscale = value;
			};
			val12.min = () => 0;
			val12.max = () => 3;
			((Field<int>)val12).onValueChanged = RebuildSimulationDebugUI;
			obj4.Add((Widget)val12);
			BoolField val13 = new BoolField
			{
				displayName = "Blur flow"
			};
			((Field<bool>)val13).getter = () => m_WaterSystem.BlurFlowMap;
			((Field<bool>)val13).setter = delegate(bool value)
			{
				m_WaterSystem.BlurFlowMap = value;
			};
			obj4.Add((Widget)val13);
			BoolField val14 = new BoolField
			{
				displayName = "Enable flow Downscale"
			};
			((Field<bool>)val14).getter = () => m_WaterSystem.EnableFlowDownscale;
			((Field<bool>)val14).setter = delegate(bool value)
			{
				m_WaterSystem.EnableFlowDownscale = value;
			};
			obj4.Add((Widget)val14);
			BoolField val15 = new BoolField
			{
				displayName = "Flow limiter for render"
			};
			((Field<bool>)val15).getter = () => m_WaterSystem.FlowPostProcess;
			((Field<bool>)val15).setter = delegate(bool value)
			{
				m_WaterSystem.FlowPostProcess = value;
			};
			obj4.Add((Widget)val15);
			FloatField val16 = new FloatField
			{
				displayName = "Max Water Flow Length for render"
			};
			((Field<float>)val16).getter = () => m_WaterSystem.MaxFlowlengthForRender;
			((Field<float>)val16).setter = delegate(float value)
			{
				m_WaterSystem.MaxFlowlengthForRender = value;
			};
			val16.min = () => 0f;
			val16.max = () => 5f;
			val16.incStep = 0.1f;
			obj4.Add((Widget)val16);
			FloatField val17 = new FloatField
			{
				displayName = "Water Flow Render Multiplier"
			};
			((Field<float>)val17).getter = () => m_WaterSystem.PostFlowspeedMultiplier;
			((Field<float>)val17).setter = delegate(float value)
			{
				m_WaterSystem.PostFlowspeedMultiplier = value;
			};
			val17.min = () => 0f;
			val17.max = () => 10f;
			val17.incStep = 0.1f;
			obj4.Add((Widget)val17);
			FloatField val18 = new FloatField
			{
				displayName = "Water Pollution Decay Rate"
			};
			((Field<float>)val18).getter = () => m_WaterSystem.m_PollutionDecayRate;
			((Field<float>)val18).setter = delegate(float value)
			{
				m_WaterSystem.m_PollutionDecayRate = value;
			};
			val18.min = () => 0f;
			val18.max = () => 0.1f;
			val18.incStep = 1E-05f;
			obj4.Add((Widget)val18);
			obj.Add((Widget)new Foldout("Water", obj4, (string[])null, (string[])null));
			ObservableList<Widget> obj5 = new ObservableList<Widget>();
			obj5.Add((Widget)(object)OverridableProperty("Climate time", () => m_ClimateSystem.currentDate));
			obj5.Add((Widget)new Value
			{
				displayName = "Current climate",
				getter = () => (!(m_ClimateSystem.currentClimate != Entity.Null)) ? "None" : ((Object)m_PrefabSystem.GetPrefab<PrefabBase>(m_ClimateSystem.currentClimate)).name
			});
			obj5.Add((Widget)new Value
			{
				displayName = "Current season",
				getter = () => (!(m_ClimateSystem.currentSeason != Entity.Null)) ? "None" : ((Object)m_PrefabSystem.GetPrefab<PrefabBase>(m_ClimateSystem.currentSeason)).name
			});
			ObservableList<Widget> obj6 = new ObservableList<Widget>();
			obj6.Add((Widget)new Value
			{
				displayName = "Average temperature",
				getter = () => m_ClimateSystem.seasonTemperature
			});
			obj6.Add((Widget)new Value
			{
				displayName = "Average precipitation",
				getter = () => m_ClimateSystem.seasonPrecipitation
			});
			obj6.Add((Widget)new Value
			{
				displayName = "Average cloudiness",
				getter = () => m_ClimateSystem.seasonCloudiness
			});
			obj5.Add((Widget)new Foldout("Season stats", obj6, (string[])null, (string[])null));
			obj5.Add((Widget)(object)OverridableProperty("Temperature", () => m_ClimateSystem.temperature, -50f, 50f));
			ObservableList<Widget> obj7 = new ObservableList<Widget>();
			obj7.Add((Widget)new Value
			{
				displayName = "Yearly average temperature",
				getter = () => m_ClimateSystem.averageTemperature
			});
			obj7.Add((Widget)new Value
			{
				displayName = "Freezing temperature",
				getter = () => m_ClimateSystem.freezingTemperature
			});
			obj7.Add((Widget)new Value
			{
				displayName = "Temperature base height",
				getter = () => m_ClimateSystem.temperatureBaseHeight
			});
			obj5.Add((Widget)new Foldout("Temperature stats", obj7, (string[])null, (string[])null));
			obj5.Add(RadioSelection("Snow sim speed", () => m_SnowSystem.SnowSimSpeed, delegate(int value)
			{
				m_SnowSystem.SnowSimSpeed = value;
			}, kDebugWaterSpeedStrings, GetWaterSpeedValues()));
			obj5.Add((Widget)(object)OverridableProperty("Precipitation", () => m_ClimateSystem.precipitation));
			obj5.Add((Widget)(object)OverridableProperty("Cloudiness", () => m_ClimateSystem.cloudiness));
			obj5.Add((Widget)(object)OverridableProperty("Aurora", () => m_ClimateSystem.aurora));
			obj5.Add((Widget)new Button
			{
				displayName = "Remove snow",
				action = delegate
				{
					((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SnowSystem>().DebugReset();
				}
			});
			obj5.Add((Widget)new Button
			{
				displayName = "Save Wind",
				action = delegate
				{
					((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSimulationSystem>().DebugSave();
				}
			});
			obj5.Add((Widget)new Button
			{
				displayName = "Load Wind",
				action = delegate
				{
					((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSimulationSystem>().DebugLoad();
				}
			});
			obj5.Add((Widget)new Button
			{
				displayName = "Reset Wind",
				action = delegate
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0013: Unknown result type (might be due to invalid IL or missing references)
					((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSimulationSystem>().SetDefaults(default(Context));
				}
			});
			FloatField val19 = new FloatField
			{
				displayName = "Precipitation volume scale"
			};
			((Field<float>)val19).getter = () => m_ClimateRenderSystem.precipitationVolumeScale;
			((Field<float>)val19).setter = delegate(float value)
			{
				m_ClimateRenderSystem.precipitationVolumeScale = value;
			};
			val19.min = () => 5f;
			val19.max = () => 300f;
			val19.incStep = 0.1f;
			obj5.Add((Widget)val19);
			BoolField val20 = new BoolField
			{
				displayName = "Global VFX time from Simulation time"
			};
			((Field<bool>)val20).getter = () => m_ClimateRenderSystem.globalEffectTimeStepFromSimulation;
			((Field<bool>)val20).setter = delegate(bool value)
			{
				m_ClimateRenderSystem.globalEffectTimeStepFromSimulation = value;
			};
			obj5.Add((Widget)val20);
			BoolField val21 = new BoolField
			{
				displayName = "Weather VFX time from Simulation time"
			};
			((Field<bool>)val21).getter = () => m_ClimateRenderSystem.weatherEffectTimeStepFromSimulation;
			((Field<bool>)val21).setter = delegate(bool value)
			{
				m_ClimateRenderSystem.weatherEffectTimeStepFromSimulation = value;
			};
			obj5.Add((Widget)val21);
			obj5.Add((Widget)new Value
			{
				displayName = "Temperature Electricity Consumption Multiplier",
				getter = () => m_AdjustElectricityConsumptionSystem.GetTemperatureMultiplier(m_ClimateSystem.temperature)
			});
			obj.Add((Widget)new Foldout("Weather & climate", obj5, (string[])null, (string[])null));
			ObservableList<Widget> obj8 = new ObservableList<Widget>();
			obj8.Add((Widget)new Value
			{
				displayName = "Time/Date",
				getter = () => EditorDate()
			});
			FloatField val22 = new FloatField
			{
				displayName = "Latitude"
			};
			((Field<float>)val22).getter = () => m_PlanetarySystem.latitude;
			((Field<float>)val22).setter = delegate(float value)
			{
				m_PlanetarySystem.latitude = value;
			};
			val22.min = () => -90f;
			val22.max = () => 90f;
			obj8.Add((Widget)val22);
			FloatField val23 = new FloatField
			{
				displayName = "Longitude"
			};
			((Field<float>)val23).getter = () => m_PlanetarySystem.longitude;
			((Field<float>)val23).setter = delegate(float value)
			{
				m_PlanetarySystem.longitude = value;
			};
			val23.min = () => -180f;
			val23.max = () => 180f;
			obj8.Add((Widget)val23);
			FloatField val24 = new FloatField
			{
				displayName = "Day of year"
			};
			((Field<float>)val24).getter = () => m_PlanetarySystem.day;
			((Field<float>)val24).setter = delegate(float value)
			{
				m_PlanetarySystem.day = (int)value;
			};
			val24.min = () => 1f;
			val24.max = () => 365f;
			obj8.Add((Widget)val24);
			FloatField val25 = new FloatField
			{
				displayName = "Time of day"
			};
			((Field<float>)val25).getter = () => m_PlanetarySystem.time;
			((Field<float>)val25).setter = delegate(float value)
			{
				m_PlanetarySystem.time = value;
			};
			val25.min = () => 0f;
			val25.max = () => 24f;
			obj8.Add((Widget)val25);
			IntField val26 = new IntField
			{
				displayName = "Number of lunar cycles per year"
			};
			((Field<int>)val26).getter = () => m_PlanetarySystem.numberOfLunarCyclesPerYear;
			((Field<int>)val26).setter = delegate(int value)
			{
				m_PlanetarySystem.numberOfLunarCyclesPerYear = value;
			};
			val26.min = () => 0;
			obj8.Add((Widget)val26);
			obj8.Add((Widget)new Value
			{
				displayName = "Day of year (Moon)",
				getter = () => m_PlanetarySystem.moonDay
			});
			BoolField val27 = new BoolField
			{
				displayName = "Override time for debug"
			};
			((Field<bool>)val27).getter = () => m_PlanetarySystem.overrideTime;
			((Field<bool>)val27).setter = delegate(bool value)
			{
				m_PlanetarySystem.overrideTime = value;
			};
			obj8.Add((Widget)val27);
			FloatField val28 = new FloatField
			{
				displayName = "Time of day multiplier"
			};
			((Field<float>)val28).getter = () => m_PlanetarySystem.debugTimeMultiplier;
			((Field<float>)val28).setter = delegate(float value)
			{
				m_PlanetarySystem.debugTimeMultiplier = value;
			};
			val28.min = () => 0f;
			val28.max = () => 100f;
			obj8.Add((Widget)val28);
			obj8.Add((Widget)new Button
			{
				displayName = "Advance time 1h",
				action = delegate
				{
					m_TimeSystem.DebugAdvanceTime(60);
				}
			});
			obj8.Add((Widget)new Button
			{
				displayName = "Advance time 12h",
				action = delegate
				{
					m_TimeSystem.DebugAdvanceTime(720);
				}
			});
			obj8.Add((Widget)new Button
			{
				displayName = "Advance time 6d",
				action = delegate
				{
					m_TimeSystem.DebugAdvanceTime(8640);
				}
			});
			obj.Add((Widget)new Foldout("Time", obj8, (string[])null, (string[])null));
			ObservableList<Widget> obj9 = new ObservableList<Widget>();
			obj9.Add((Widget)new Button
			{
				displayName = "Give max resource",
				action = delegate
				{
					GiveMaxResources();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Print age debug",
				action = delegate
				{
					EconomyDebugSystem.PrintAgeDebug();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Print school debug",
				action = delegate
				{
					EconomyDebugSystem.PrintSchoolDebug();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Print company debug",
				action = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					EconomyDebugSystem.PrintCompanyDebug(InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef));
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Print trade debug",
				action = delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0020: Unknown result type (might be due to invalid IL or missing references)
					TradeSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TradeSystem>();
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					EconomyDebugSystem.PrintTradeDebug(orCreateSystemManaged, ((EntityManager)(ref entityManager)).GetBuffer<CityModifier>(m_CitySystem.City, true));
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Remove extra companies",
				action = delegate
				{
					EconomyDebugSystem.RemoveExtraCompanies();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Print null households",
				action = delegate
				{
					PrintNullHouseholds();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Calc customers",
				action = delegate
				{
					CalculateCustomers();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Calc eligible",
				action = delegate
				{
					CalculateEligible();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Calc students from OC",
				action = delegate
				{
					CalculateStudentsFromOC();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Happiness factors",
				action = delegate
				{
					HappinessFactors();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset land value",
				action = delegate
				{
					ResetLandvalue();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset households wealth",
				action = delegate
				{
					ResetHouseholdsWealth();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Max households wealth",
				action = delegate
				{
					MaxHouseholdsWealth();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset company money",
				action = delegate
				{
					ResetCompanyMoney();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset rents",
				action = delegate
				{
					ResetRents();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset services",
				action = delegate
				{
					ResetServices();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset trade costs",
				action = delegate
				{
					ResetTradeCosts();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset transfers",
				action = delegate
				{
					ResetTransfers();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset trip neededs",
				action = delegate
				{
					ResetTripNeeded();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Reset storages",
				action = delegate
				{
					ResetStorages();
				}
			});
			IntInputField intInputField = new IntInputField();
			((Widget)intInputField).displayName = "Select entity";
			((Field<string>)(object)intInputField).getter = () => entitySearchQuery;
			((Field<string>)(object)intInputField).setter = delegate(string value)
			{
				entitySearchQuery = value;
			};
			((Field<string>)(object)intInputField).onValueChanged = delegate(Field<string> field, string value)
			{
				SelectEntity(value);
			};
			obj9.Add((Widget)(object)intInputField);
			obj9.Add((Widget)new Button
			{
				displayName = "Follow selected citizen",
				action = delegate
				{
					FollowSelectedCitizen();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Age selected citizen",
				action = delegate
				{
					AgeSelectedCitizen();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Trigger Test Life Event",
				action = delegate
				{
					TriggerTestLifeEvent();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Discard statistics",
				action = delegate
				{
					m_CityStatisticsSystem.DiscardStatistics();
				}
			});
			obj9.Add((Widget)new Button
			{
				displayName = "Print commuter distribution",
				action = delegate
				{
					PrintCommuterDistribute();
				}
			});
			obj.Add((Widget)new Foldout("Economy", obj9, (string[])null, (string[])null));
			ObservableList<Widget> obj10 = new ObservableList<Widget>();
			obj10.Add((Widget)new Button
			{
				displayName = "Reset Electricity",
				action = delegate
				{
					m_ElectricityFlowSystem.Reset();
					((ComponentSystemBase)m_ElectricityFlowSystem).Enabled = true;
				}
			});
			BoolField val29 = new BoolField
			{
				displayName = "Water Pipe Fluid Flow"
			};
			((Field<bool>)val29).getter = () => m_WaterPipeFlowSystem.fluidFlowEnabled;
			((Field<bool>)val29).setter = delegate(bool value)
			{
				m_WaterPipeFlowSystem.fluidFlowEnabled = value;
			};
			obj10.Add((Widget)val29);
			BoolField val30 = new BoolField
			{
				displayName = "Disable Water consumption"
			};
			((Field<bool>)val30).getter = () => m_DispatchWaterSystem.freshConsumptionDisabled;
			((Field<bool>)val30).setter = delegate(bool value)
			{
				m_DispatchWaterSystem.freshConsumptionDisabled = value;
			};
			obj10.Add((Widget)val30);
			BoolField val31 = new BoolField
			{
				displayName = "Disable Sewage generation"
			};
			((Field<bool>)val31).getter = () => m_DispatchWaterSystem.sewageConsumptionDisabled;
			((Field<bool>)val31).setter = delegate(bool value)
			{
				m_DispatchWaterSystem.sewageConsumptionDisabled = value;
			};
			obj10.Add((Widget)val31);
			obj.Add((Widget)new Foldout("Electricty & Water", obj10, (string[])null, (string[])null));
			ObservableList<Widget> obj11 = new ObservableList<Widget>();
			obj11.Add((Widget)new Button
			{
				displayName = "Full selected buidling with garbage",
				action = delegate
				{
					FullWithGarbage();
				}
			});
			obj11.Add((Widget)new Button
			{
				displayName = "Reset pollution",
				action = delegate
				{
					ResetPollution();
				}
			});
			FloatField val32 = new FloatField
			{
				displayName = "Ground Multiplier",
				incStep = 5f
			};
			((Field<float>)val32).getter = () => pollution.m_GroundMultiplier;
			((Field<float>)val32).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_GroundMultiplier = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val32);
			FloatField val33 = new FloatField
			{
				displayName = "Air Multiplier",
				incStep = 25f
			};
			((Field<float>)val33).getter = () => pollution.m_AirMultiplier;
			((Field<float>)val33).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_AirMultiplier = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val33);
			FloatField val34 = new FloatField
			{
				displayName = "Noise Multiplier",
				incStep = 50f
			};
			((Field<float>)val34).getter = () => pollution.m_NoiseMultiplier;
			((Field<float>)val34).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_NoiseMultiplier = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val34);
			FloatField val35 = new FloatField
			{
				displayName = "Net Noise Multiplier",
				incStep = 1f
			};
			((Field<float>)val35).getter = () => pollution.m_NetNoiseMultiplier;
			((Field<float>)val35).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_NetNoiseMultiplier = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val35);
			FloatField val36 = new FloatField
			{
				displayName = "Net Air Multiplier",
				incStep = 1f
			};
			((Field<float>)val36).getter = () => pollution.m_NetAirMultiplier;
			((Field<float>)val36).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_NetAirMultiplier = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val36);
			FloatField val37 = new FloatField
			{
				displayName = "Ground Radius",
				incStep = 5f
			};
			((Field<float>)val37).getter = () => pollution.m_GroundRadius;
			((Field<float>)val37).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_GroundRadius = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val37);
			FloatField val38 = new FloatField
			{
				displayName = "Air Radius",
				incStep = 5f
			};
			((Field<float>)val38).getter = () => pollution.m_AirRadius;
			((Field<float>)val38).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_AirRadius = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val38);
			FloatField val39 = new FloatField
			{
				displayName = "Noise Radius",
				incStep = 50f
			};
			((Field<float>)val39).getter = () => pollution.m_NoiseRadius;
			((Field<float>)val39).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_NoiseRadius = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val39);
			FloatField val40 = new FloatField
			{
				displayName = "Net Noise Radius",
				incStep = 5f
			};
			((Field<float>)val40).getter = () => pollution.m_NetNoiseRadius;
			((Field<float>)val40).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_NetNoiseRadius = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val40);
			FloatField val41 = new FloatField
			{
				displayName = "Wind Advection Speed",
				incStep = 5f
			};
			((Field<float>)val41).getter = () => pollution.m_WindAdvectionSpeed;
			((Field<float>)val41).setter = delegate(float value)
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_WindAdvectionSpeed = value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val41);
			IntField val42 = new IntField
			{
				displayName = "Air Fade",
				incStep = 1
			};
			((Field<int>)val42).getter = () => pollution.m_AirFade;
			((Field<int>)val42).setter = delegate(int value)
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_AirFade = (short)value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val42);
			IntField val43 = new IntField
			{
				displayName = "Ground Fade",
				incStep = 50
			};
			((Field<int>)val43).getter = () => pollution.m_GroundFade;
			((Field<int>)val43).setter = delegate(int value)
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_GroundFade = (short)value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val43);
			FloatField val44 = new FloatField
			{
				displayName = "Plant Air Multiplier"
			};
			((Field<float>)val44).getter = () => pollution.m_PlantAirMultiplier;
			((Field<float>)val44).setter = delegate(float value)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_PlantAirMultiplier = (short)value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val44);
			FloatField val45 = new FloatField
			{
				displayName = "Plant Ground Multiplier"
			};
			((Field<float>)val45).getter = () => pollution.m_PlantGroundMultiplier;
			((Field<float>)val45).setter = delegate(float value)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_PlantGroundMultiplier = (short)value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val45);
			FloatField val46 = new FloatField
			{
				displayName = "Plant Fade",
				incStep = 1f
			};
			((Field<float>)val46).getter = () => pollution.m_PlantFade;
			((Field<float>)val46).setter = delegate(float value)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_PlantFade = (short)value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val46);
			FloatField val47 = new FloatField
			{
				displayName = "Fertility Ground Multiplier",
				incStep = 1f
			};
			((Field<float>)val47).getter = () => pollution.m_FertilityGroundMultiplier;
			((Field<float>)val47).setter = delegate(float value)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_FertilityGroundMultiplier = (short)value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val47);
			FloatField val48 = new FloatField
			{
				displayName = "Distance Expotent",
				incStep = 1f
			};
			((Field<float>)val48).getter = () => pollution.m_DistanceExponent;
			((Field<float>)val48).setter = delegate(float value)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				pollution.m_DistanceExponent = (short)value;
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PollutionParameterData>(((EntityQuery)(ref m_PollutionParameterQuery)).GetSingletonEntity(), pollution);
			};
			obj11.Add((Widget)val48);
			obj.Add((Widget)new Foldout("Pollution", obj11, (string[])null, (string[])null));
			obj.Add((Widget)(object)CreateEventUI());
			ObservableList<Widget> obj12 = new ObservableList<Widget>();
			obj12.Add((Widget)new Button
			{
				displayName = "Export heightmap",
				action = ExportHeightMap
			});
			obj.Add((Widget)new Foldout("Terrain", obj12, (string[])null, (string[])null));
			ObservableList<Widget> obj13 = new ObservableList<Widget>();
			obj13.Add((Widget)new Button
			{
				displayName = "Create Chirps",
				action = delegate
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_002d: Unknown result type (might be due to invalid IL or missing references)
					NativeQueue<TriggerAction> val52 = ((ComponentSystemBase)this).World.GetExistingSystemManaged<TriggerSystem>().CreateActionBuffer();
					val52.Enqueue(new TriggerAction(TriggerType.NoOutsideConnection, Entity.Null, 0f));
					val52.Enqueue(new TriggerAction(TriggerType.UnpaidLoan, Entity.Null, 999f));
				}
			});
			obj.Add((Widget)new Foldout("Triggers", obj13, (string[])null, (string[])null));
			ObservableList<Widget> obj14 = new ObservableList<Widget>();
			obj14.Add((Widget)new Button
			{
				displayName = "Remove All TouristHousehold",
				action = delegate
				{
					RemoveAllTourist();
				}
			});
			obj14.Add((Widget)new Button
			{
				displayName = "Remove All HomelessHousehold",
				action = delegate
				{
					RemoveAllHomeless();
				}
			});
			obj14.Add((Widget)new Button
			{
				displayName = "Reset Commercial Storage",
				action = delegate
				{
					ResetCommercialStorage();
				}
			});
			obj.Add((Widget)new Foldout("Cleanup", obj14, (string[])null, (string[])null));
			ObservableList<Widget> obj15 = new ObservableList<Widget>();
			obj15.Add((Widget)new Button
			{
				displayName = "Check property rent errors",
				action = delegate
				{
					//IL_0009: Unknown result type (might be due to invalid IL or missing references)
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_004d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0052: Unknown result type (might be due to invalid IL or missing references)
					//IL_0059: Unknown result type (might be due to invalid IL or missing references)
					//IL_0066: Unknown result type (might be due to invalid IL or missing references)
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
					//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
					//IL_010c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0170: Unknown result type (might be due to invalid IL or missing references)
					//IL_017d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0124: Unknown result type (might be due to invalid IL or missing references)
					//IL_0136: Unknown result type (might be due to invalid IL or missing references)
					//IL_0148: Unknown result type (might be due to invalid IL or missing references)
					//IL_0152: Unknown result type (might be due to invalid IL or missing references)
					//IL_008d: Unknown result type (might be due to invalid IL or missing references)
					//IL_009a: Unknown result type (might be due to invalid IL or missing references)
					//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
					EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
					{
						ComponentType.ReadOnly<PropertyRenter>(),
						ComponentType.Exclude<Deleted>(),
						ComponentType.Exclude<Temp>()
					});
					NativeArray<Entity> val52 = ((EntityQuery)(ref entityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
					Dictionary<int, Entity> dictionary = new Dictionary<int, Entity>();
					DynamicBuffer<Renter> val53 = default(DynamicBuffer<Renter>);
					for (int i = 0; i < val52.Length; i++)
					{
						EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
						PropertyRenter componentData = ((EntityManager)(ref entityManager)).GetComponentData<PropertyRenter>(val52[i]);
						if (EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, componentData.m_Property, true, ref val53))
						{
							bool flag = false;
							for (int j = 0; j < val53.Length; j++)
							{
								if (val53[j].m_Renter.Index == val52[i].Index)
								{
									flag = true;
								}
							}
							if (!flag)
							{
								Debug.LogWarning((object)$"Found invalid renter :{val52[i].Index} of building:{componentData.m_Property.Index}");
							}
						}
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Household>(val52[i]))
						{
							if (dictionary.ContainsKey(componentData.m_Property.Index))
							{
								Debug.LogWarning((object)$"duplicate property:{componentData.m_Property.Index} found, renter:{val52[i].Index} & {dictionary[componentData.m_Property.Index].Index}");
							}
							else
							{
								dictionary[componentData.m_Property.Index] = val52[i];
							}
						}
					}
					val52.Dispose();
				}
			});
			obj15.Add((Widget)new Button
			{
				displayName = "Force Send Renter Update Event",
				action = delegate
				{
					//IL_0009: Unknown result type (might be due to invalid IL or missing references)
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0045: Unknown result type (might be due to invalid IL or missing references)
					//IL_004a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0050: Unknown result type (might be due to invalid IL or missing references)
					//IL_0055: Unknown result type (might be due to invalid IL or missing references)
					//IL_0061: Unknown result type (might be due to invalid IL or missing references)
					//IL_0066: Unknown result type (might be due to invalid IL or missing references)
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0072: Unknown result type (might be due to invalid IL or missing references)
					//IL_0077: Unknown result type (might be due to invalid IL or missing references)
					//IL_007c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0080: Unknown result type (might be due to invalid IL or missing references)
					//IL_0082: Unknown result type (might be due to invalid IL or missing references)
					//IL_0087: Unknown result type (might be due to invalid IL or missing references)
					//IL_008b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0090: Unknown result type (might be due to invalid IL or missing references)
					EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
					{
						ComponentType.ReadOnly<Renter>(),
						ComponentType.Exclude<Deleted>(),
						ComponentType.Exclude<Temp>()
					});
					NativeArray<Entity> val52 = ((EntityQuery)(ref entityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
					EntityCommandBuffer val53 = m_EndFrameBarrier.CreateCommandBuffer();
					for (int i = 0; i < val52.Length; i++)
					{
						EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
						EntityArchetype val54 = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
						{
							ComponentType.ReadWrite<Game.Common.Event>(),
							ComponentType.ReadWrite<RentersUpdated>()
						});
						Entity val55 = ((EntityCommandBuffer)(ref val53)).CreateEntity(val54);
						((EntityCommandBuffer)(ref val53)).SetComponent<RentersUpdated>(val55, new RentersUpdated(val52[i]));
					}
					val52.Dispose();
				}
			});
			obj15.Add((Widget)new Button
			{
				displayName = "Fix property rent errors",
				action = delegate
				{
					//IL_0009: Unknown result type (might be due to invalid IL or missing references)
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_004b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0050: Unknown result type (might be due to invalid IL or missing references)
					//IL_005a: Unknown result type (might be due to invalid IL or missing references)
					//IL_005f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0067: Unknown result type (might be due to invalid IL or missing references)
					//IL_0076: Unknown result type (might be due to invalid IL or missing references)
					//IL_0086: Unknown result type (might be due to invalid IL or missing references)
					//IL_008d: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00be: Unknown result type (might be due to invalid IL or missing references)
					//IL_0133: Unknown result type (might be due to invalid IL or missing references)
					//IL_0138: Unknown result type (might be due to invalid IL or missing references)
					//IL_013e: Unknown result type (might be due to invalid IL or missing references)
					//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
					//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
					//IL_0118: Unknown result type (might be due to invalid IL or missing references)
					//IL_0128: Unknown result type (might be due to invalid IL or missing references)
					//IL_014e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0153: Unknown result type (might be due to invalid IL or missing references)
					//IL_0159: Unknown result type (might be due to invalid IL or missing references)
					//IL_0187: Unknown result type (might be due to invalid IL or missing references)
					//IL_0198: Unknown result type (might be due to invalid IL or missing references)
					//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
					//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
					EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
					{
						ComponentType.ReadOnly<PropertyRenter>(),
						ComponentType.Exclude<Deleted>(),
						ComponentType.Exclude<Temp>()
					});
					NativeArray<Entity> val52 = ((EntityQuery)(ref entityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
					List<Entity> list2 = new List<Entity>();
					EntityCommandBuffer val53 = m_EndFrameBarrier.CreateCommandBuffer();
					DynamicBuffer<Renter> val54 = default(DynamicBuffer<Renter>);
					for (int i = 0; i < val52.Length; i++)
					{
						EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
						PropertyRenter componentData = ((EntityManager)(ref entityManager)).GetComponentData<PropertyRenter>(val52[i]);
						if (!list2.Contains(componentData.m_Property) && EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, componentData.m_Property, false, ref val54))
						{
							bool flag = false;
							for (int j = 0; j < val54.Length; j++)
							{
								if (val54[j].m_Renter.Index == val52[i].Index)
								{
									flag = true;
								}
							}
							if (!flag)
							{
								Debug.LogWarning((object)$"destroy invalid renter :{val52[i].Index} of building:{componentData.m_Property.Index}");
								list2.Add(componentData.m_Property);
								((EntityCommandBuffer)(ref val53)).AddComponent<Deleted>(val52[i]);
							}
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasComponent<ResidentialProperty>(componentData.m_Property))
							{
								entityManager = ((ComponentSystemBase)this).EntityManager;
								if (!((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.Park>(componentData.m_Property) && val54.Length > 1)
								{
									Debug.LogWarning((object)$"destroy one of the two company renter :{val54[val54.Length - 1].m_Renter.Index} of building:{componentData.m_Property.Index}");
									list2.Add(componentData.m_Property);
									((EntityCommandBuffer)(ref val53)).AddComponent<Deleted>(val54[val54.Length - 1].m_Renter);
									val54.RemoveAt(val54.Length - 1);
								}
							}
						}
					}
					val52.Dispose();
				}
			});
			obj15.Add((Widget)new Button
			{
				displayName = "Fix invalid Enabled Effects",
				action = delegate
				{
					//IL_0009: Unknown result type (might be due to invalid IL or missing references)
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0021: Unknown result type (might be due to invalid IL or missing references)
					//IL_0026: Unknown result type (might be due to invalid IL or missing references)
					//IL_002b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_0034: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0045: Unknown result type (might be due to invalid IL or missing references)
					//IL_004a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0053: Unknown result type (might be due to invalid IL or missing references)
					//IL_0058: Unknown result type (might be due to invalid IL or missing references)
					//IL_005f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0069: Unknown result type (might be due to invalid IL or missing references)
					//IL_006e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0071: Unknown result type (might be due to invalid IL or missing references)
					//IL_0076: Unknown result type (might be due to invalid IL or missing references)
					//IL_007d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0083: Unknown result type (might be due to invalid IL or missing references)
					//IL_0088: Unknown result type (might be due to invalid IL or missing references)
					//IL_0097: Unknown result type (might be due to invalid IL or missing references)
					//IL_009c: Unknown result type (might be due to invalid IL or missing references)
					//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
					//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
					//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
					//IL_00de: Unknown result type (might be due to invalid IL or missing references)
					//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
					//IL_0104: Unknown result type (might be due to invalid IL or missing references)
					//IL_0122: Unknown result type (might be due to invalid IL or missing references)
					EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
					{
						ComponentType.ReadOnly<EnabledEffect>(),
						ComponentType.Exclude<Deleted>(),
						ComponentType.Exclude<Temp>()
					});
					NativeArray<Entity> val52 = ((EntityQuery)(ref entityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
					EntityCommandBuffer val53 = m_EndFrameBarrier.CreateCommandBuffer();
					for (int i = 0; i < val52.Length; i++)
					{
						EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
						prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val52[i]).m_Prefab;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						DynamicBuffer<EnabledEffect> buffer = ((EntityManager)(ref entityManager)).GetBuffer<EnabledEffect>(val52[i], false);
						if (buffer.Length != 0)
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (!((EntityManager)(ref entityManager)).HasBuffer<Effect>(prefab))
							{
								Debug.LogWarning((object)$"Entity without effect but have EnabledEffect, entity:{val52[i].Index}");
							}
							else
							{
								entityManager = ((ComponentSystemBase)this).EntityManager;
								DynamicBuffer<Effect> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<Effect>(prefab, false);
								for (int j = 0; j < buffer.Length; j++)
								{
									if (buffer[j].m_EffectIndex >= buffer2.Length)
									{
										Debug.LogWarning((object)$"destroyed building with invalid effect, building:{val52[i].Index}");
										((EntityCommandBuffer)(ref val53)).AddComponent<Deleted>(val52[i]);
										break;
									}
								}
							}
						}
					}
					val52.Dispose();
				}
			});
			obj.Add((Widget)new Foldout("Error Check", obj15, (string[])null, (string[])null));
			List<Widget> list = obj;
			if (GameManager.instance.gameMode.IsEditor())
			{
				ObservableList<Widget> obj16 = new ObservableList<Widget>();
				FloatField val49 = new FloatField
				{
					displayName = "X"
				};
				((Field<float>)val49).getter = () => m_RenderingSystem.editorBuildingStateOverride.x;
				((Field<float>)val49).setter = delegate(float value)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					float4 editorBuildingStateOverride = m_RenderingSystem.editorBuildingStateOverride;
					editorBuildingStateOverride.x = value;
					m_RenderingSystem.editorBuildingStateOverride = editorBuildingStateOverride;
				};
				val49.min = () => 0f;
				val49.max = () => 1f;
				obj16.Add((Widget)val49);
				FloatField val50 = new FloatField
				{
					displayName = "Y"
				};
				((Field<float>)val50).getter = () => m_RenderingSystem.editorBuildingStateOverride.x;
				((Field<float>)val50).setter = delegate(float value)
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_001a: Unknown result type (might be due to invalid IL or missing references)
					float4 editorBuildingStateOverride = m_RenderingSystem.editorBuildingStateOverride;
					editorBuildingStateOverride.y = value;
					m_RenderingSystem.editorBuildingStateOverride = editorBuildingStateOverride;
				};
				val50.min = () => 0f;
				val50.max = () => 1f;
				obj16.Add((Widget)val50);
				list.Add((Widget)new Container("Objects lighting state", obj16));
				BoolField val51 = new BoolField
				{
					displayName = "Bypass editor value limits"
				};
				((Field<bool>)val51).getter = () => EditorGenerator.sBypassValueLimits;
				((Field<bool>)val51).setter = delegate(bool value)
				{
					EditorGenerator.sBypassValueLimits = value;
				};
				list.Add((Widget)val51);
			}
			return list;
		}
		return null;
	}

	private void ExportHeightMap()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		NativeSlice<byte> val = new NativeSlice<ushort>(m_TerrainSystem.GetHeightData(waitForPending: true).heights).SliceConvert<byte>();
		byte[] array = new byte[val.Length];
		val.CopyTo(array);
		File.WriteAllBytes(EnvPath.kUserDataPath + "/TerrainExport.raw", array);
	}

	private async void SaveGame()
	{
		SaveGameSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SaveGameSystem>();
		orCreateSystemManaged.stream = LongFile.OpenWrite(EnvPath.kUserDataPath + "/DebugSave.SaveGameData", (Action)null);
		orCreateSystemManaged.context = new Context((Purpose)0, Version.current, Hash128.Empty);
		await orCreateSystemManaged.RunOnce();
	}

	private async void LoadGame()
	{
		LoadGameSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		orCreateSystemManaged.dataDescriptor = new AsyncReadDescriptor("DebugSave", EnvPath.kUserDataPath + "/DebugSave.SaveGameData");
		orCreateSystemManaged.context = new Context((Purpose)2, Version.current, Hash128.Empty);
		await orCreateSystemManaged.RunOnce();
	}

	private void ResetServices()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_ServiceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<ResetServiceData> val2 = default(NativeArray<ResetServiceData>);
		val2._002Ector(EconomyUtils.GetResourceIndex(Resource.Last), (Allocator)3, (NativeArrayOptions)1);
		for (int i = 0; i < val.Length; i++)
		{
			Entity val3 = val[i];
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val3).m_Prefab;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity property = ((EntityManager)(ref entityManager)).GetComponentData<PropertyRenter>(val3).m_Property;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity prefab2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(property).m_Prefab;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			SpawnableBuildingData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SpawnableBuildingData>(prefab2);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			IndustrialProcessData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<IndustrialProcessData>(prefab);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			ServiceAvailable componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<ServiceAvailable>(val3);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Efficiency> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Efficiency>(property, true);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Employee> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<Employee>(val3, true);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			WorkProvider componentData4 = ((EntityManager)(ref entityManager)).GetComponentData<WorkProvider>(val3);
			ResetServiceData resetServiceData = val2[EconomyUtils.GetResourceIndex(componentData2.m_Output.m_Resource)];
			resetServiceData.count++;
			resetServiceData.meanPriority += componentData3.m_MeanPriority;
			resetServiceData.serviceAvailable += componentData3.m_ServiceAvailable;
			if (componentData3.m_ServiceAvailable <= 500)
			{
				resetServiceData.overworked++;
			}
			if (componentData3.m_ServiceAvailable >= 4500)
			{
				resetServiceData.underworked++;
			}
			if (componentData3.m_ServiceAvailable >= 2400 && componentData3.m_ServiceAvailable <= 2600)
			{
				resetServiceData.middle++;
			}
			resetServiceData.efficiency += BuildingUtils.GetEfficiency(buffer);
			resetServiceData.workers += buffer2.Length;
			resetServiceData.maxWorkers += componentData4.m_MaxWorkers;
			resetServiceData.level += componentData.m_Level;
			val2[EconomyUtils.GetResourceIndex(componentData2.m_Output.m_Resource)] = resetServiceData;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<ServiceAvailable>(val[i], new ServiceAvailable
			{
				m_MeanPriority = 0.5f,
				m_ServiceAvailable = 2500
			});
		}
		for (int j = 0; j < val2.Length; j++)
		{
			ResetServiceData resetServiceData2 = val2[j];
			if (resetServiceData2.count > 0)
			{
				Debug.Log((object)$"{EconomyUtils.GetName(EconomyUtils.GetResource(j))}: n = {resetServiceData2.count}, mean = {resetServiceData2.meanPriority / (float)resetServiceData2.count}, service = {resetServiceData2.serviceAvailable / resetServiceData2.count}, eff = {resetServiceData2.efficiency / (float)resetServiceData2.count}, wrkrs = {resetServiceData2.workers / resetServiceData2.count}/{resetServiceData2.maxWorkers / resetServiceData2.count} o/m/u: {(float)resetServiceData2.overworked / (float)resetServiceData2.count}|{(float)resetServiceData2.middle / (float)resetServiceData2.count}|{(float)resetServiceData2.underworked / (float)resetServiceData2.count} lvl {(float)resetServiceData2.level / (float)resetServiceData2.count}");
			}
		}
		val2.Dispose();
	}

	private void ResetTradeCosts()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_TradeCostQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).GetBuffer<TradeCost>(val[i], false).Clear();
		}
	}

	private void ResetCrimeAccumulation()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CrimeProducer>() });
		NativeArray<Entity> val = ((EntityQuery)(ref entityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			CrimeProducer componentData = ((EntityManager)(ref entityManager)).GetComponentData<CrimeProducer>(val[i]);
			componentData.m_Crime = 0f;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<CrimeProducer>(val[i], componentData);
		}
		val.Dispose();
	}

	private void ResetTransfers()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_TransferQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).GetBuffer<StorageTransferRequest>(val[i], false).Clear();
		}
	}

	private void ResetTripNeeded()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_TripNeededQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).GetBuffer<TripNeeded>(val[i], false).Clear();
		}
	}

	private void ResetStorages()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.Exclude<Game.Prefabs.ProcessingCompany>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Buildings.CargoTransportStation>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(val);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val2 = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<StorageProperty>(),
			ComponentType.Exclude<Game.Objects.OutsideConnection>(),
			ComponentType.Exclude<Game.Buildings.CargoTransportStation>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(val2);
	}

	private void GiveMaxResources()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		Entity val = m_ToolSystem.selected;
		DynamicBuffer<Renter> val2 = default(DynamicBuffer<Renter>);
		EntityManager entityManager;
		if (EntitiesExtensions.TryGetBuffer<Renter>(((ComponentSystemBase)this).EntityManager, m_ToolSystem.selected, true, ref val2))
		{
			for (int i = 0; i < val2.Length; i++)
			{
				Entity renter = val2[i].m_Renter;
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).HasComponent<CompanyData>(renter))
				{
					val = renter;
				}
			}
		}
		PrefabRef prefabRef = default(PrefabRef);
		DynamicBuffer<Resources> resources = default(DynamicBuffer<Resources>);
		if (!EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, val, ref prefabRef) || !EntitiesExtensions.TryGetBuffer<Resources>(((ComponentSystemBase)this).EntityManager, val, false, ref resources))
		{
			return;
		}
		Entity prefab = prefabRef.m_Prefab;
		int totalStorageUsed = EconomyUtils.GetTotalStorageUsed(resources);
		IndustrialProcessData industrialProcessData = default(IndustrialProcessData);
		StorageLimitData storageLimitData = default(StorageLimitData);
		PropertyRenter propertyRenter = default(PropertyRenter);
		PrefabRef prefabRef2 = default(PrefabRef);
		if (EntitiesExtensions.TryGetComponent<IndustrialProcessData>(((ComponentSystemBase)this).EntityManager, prefab, ref industrialProcessData) && EntitiesExtensions.TryGetComponent<StorageLimitData>(((ComponentSystemBase)this).EntityManager, prefab, ref storageLimitData) && EntitiesExtensions.TryGetComponent<PropertyRenter>(((ComponentSystemBase)this).EntityManager, val, ref propertyRenter) && EntitiesExtensions.TryGetComponent<PrefabRef>(((ComponentSystemBase)this).EntityManager, propertyRenter.m_Property, ref prefabRef2))
		{
			int num = storageLimitData.m_Limit;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Game.Companies.StorageCompany>(val))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				SpawnableBuildingData componentData = ((EntityManager)(ref entityManager)).GetComponentData<SpawnableBuildingData>(prefabRef2.m_Prefab);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				BuildingData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<BuildingData>(prefabRef2.m_Prefab);
				num = storageLimitData.GetAdjustedLimitForWarehouse(componentData, componentData2);
			}
			EconomyUtils.AddResources(industrialProcessData.m_Output.m_Resource, num - totalStorageUsed, resources);
		}
	}

	private void RemoveAllTourist()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TouristHousehold>() });
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(val);
	}

	private void RemoveAllHomeless()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HomelessHousehold>() });
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(val);
	}

	private void ResetCommercialStorage()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CommercialCompany>() });
		NativeArray<Entity> val2 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < val2.Length; i++)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent<Resources>(val2[i]);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponent<Resources>(val2[i]);
		}
		val2.Dispose();
	}

	private void RemoveResidentsAndVehicles()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Creatures.Resident>() });
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(val);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val2 = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Vehicle>() });
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(val2);
	}

	private void CleanupObsoleteEntities()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Deleted>()
		});
		NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref val)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeParallelHashMap<Entity, int> val3 = default(NativeParallelHashMap<Entity, int>);
		val3._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
		NativeList<Entity> val4 = default(NativeList<Entity>);
		val4._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
		NativeList<Entity> val5 = default(NativeList<Entity>);
		val5._002Ector(100, AllocatorHandle.op_Implicit((Allocator)2));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabRef> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		DynamicBuffer<ConnectedEdge> val8 = default(DynamicBuffer<ConnectedEdge>);
		int num2 = default(int);
		for (int i = 0; i < val2.Length; i++)
		{
			ArchetypeChunk val6 = val2[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val6)).GetNativeArray(entityTypeHandle);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref val6)).GetNativeArray<PrefabRef>(ref componentTypeHandle);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				Entity prefab = nativeArray2[j].m_Prefab;
				if (EntitiesExtensions.HasEnabledComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, prefab))
				{
					continue;
				}
				Entity val7 = nativeArray[j];
				if (EntitiesExtensions.TryGetBuffer<ConnectedEdge>(((ComponentSystemBase)this).EntityManager, val7, true, ref val8))
				{
					Entity val9 = Entity.Null;
					float num = float.MinValue;
					for (int k = 0; k < val8.Length; k++)
					{
						Entity edge = val8[k].m_Edge;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						Entity prefab2 = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(edge).m_Prefab;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						Game.Net.Edge componentData = ((EntityManager)(ref entityManager)).GetComponentData<Game.Net.Edge>(edge);
						if (!EntitiesExtensions.HasEnabledComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, prefab2))
						{
							continue;
						}
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Deleted>(prefab2) && (componentData.m_Start == val7 || componentData.m_End == val7))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							NetData componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<NetData>(prefab2);
							if (componentData2.m_NodePriority > num)
							{
								val9 = prefab2;
								num = componentData2.m_NodePriority;
							}
						}
					}
					if (val9 != Entity.Null)
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager)).SetComponentData<PrefabRef>(val7, new PrefabRef(val9));
						val5.Add(ref val7);
						continue;
					}
				}
				if (val3.TryGetValue(prefab, ref num2))
				{
					val3[prefab] = num2 + 1;
				}
				else
				{
					val3.Add(prefab, 1);
				}
				val4.Add(ref val7);
			}
		}
		val2.Dispose();
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Deleted>(val4.AsArray());
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).AddComponent<Updated>(val5.AsArray());
		val4.Dispose();
		Enumerator<Entity, int> enumerator = val3.GetEnumerator();
		while (enumerator.MoveNext())
		{
			PrefabID obsoleteID = m_PrefabSystem.GetObsoleteID(enumerator.Current.Key);
			Debug.Log((object)$"{obsoleteID}: Removed {enumerator.Current.Value} instances");
		}
		enumerator.Dispose();
		val3.Dispose();
	}

	private void ReloadWaterSources()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Simulation.WaterSourceData>() });
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).DestroyEntity(val);
		Entity singletonEntity = ((EntityQuery)(ref __query_1508003740_0)).GetSingletonEntity();
		WaterSystem.WaterSource[] waterSources = m_PrefabSystem.GetPrefab<TerrainPropertiesPrefab>(singletonEntity).m_WaterSources;
		float3 val2 = default(float3);
		for (int i = 0; i < waterSources.Length; i++)
		{
			WaterSystem.WaterSource waterSource = waterSources[i];
			((float3)(ref val2))._002Ector(waterSource.position.x, 0f, waterSource.position.y);
			Game.Simulation.WaterSourceData waterSourceData = new Game.Simulation.WaterSourceData
			{
				m_Amount = waterSource.amount,
				m_ConstantDepth = waterSource.constantDepth,
				m_Radius = waterSource.radius,
				m_Polluted = waterSource.pollution
			};
			if (waterSourceData.m_ConstantDepth != 2 && waterSourceData.m_ConstantDepth != 3)
			{
				waterSourceData.m_Multiplier = WaterSystem.CalculateSourceMultiplier(waterSourceData, val2);
			}
			else
			{
				waterSourceData.m_Multiplier = waterSource.floodheight;
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			Entity val3 = ((EntityManager)(ref entityManager)).CreateEntity();
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<Game.Simulation.WaterSourceData>(val3, waterSourceData);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).AddComponentData<Transform>(val3, new Transform
			{
				m_Position = val2,
				m_Rotation = quaternion.identity
			});
		}
	}

	private void FollowSelectedCitizen()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (m_ToolSystem.selected != Entity.Null)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Citizen>(m_ToolSystem.selected))
			{
				World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<LifePathEventSystem>().FollowCitizen(m_ToolSystem.selected);
			}
		}
	}

	private unsafe void PrintNullHouseholds()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_HouseholdMemberGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		for (int i = 0; i < val.Length; i++)
		{
			HouseholdMember componentData = ((EntityManager)(ref entityManager)).GetComponentData<HouseholdMember>(val[i]);
			if (componentData.m_Household == Entity.Null)
			{
				Debug.Log((object)("Null: " + ((object)val[i]/*cast due to .constrained prefix*/).ToString()));
			}
			else if (!((EntityManager)(ref entityManager)).Exists(componentData.m_Household))
			{
				string text = ((object)val[i]/*cast due to .constrained prefix*/).ToString();
				Entity household = componentData.m_Household;
				Debug.Log((object)("!Exists: " + text + " -> " + ((object)(*(Entity*)(&household))/*cast due to .constrained prefix*/).ToString()));
			}
		}
	}

	private void PrintCommuterDistribute()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		int[] array = new int[4];
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CommuterHousehold>() });
		NativeArray<Entity> val = ((EntityQuery)(ref entityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		PrefabRef prefabRef = default(PrefabRef);
		OutsideConnectionData outsideConnectionData = default(OutsideConnectionData);
		for (int i = 0; i < val.Length; i++)
		{
			CommuterHousehold componentData = ((EntityManager)(ref entityManager)).GetComponentData<CommuterHousehold>(val[i]);
			if (componentData.m_OriginalFrom != Entity.Null && EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, componentData.m_OriginalFrom, ref prefabRef) && EntitiesExtensions.TryGetComponent<OutsideConnectionData>(entityManager, prefabRef.m_Prefab, ref outsideConnectionData))
			{
				if ((outsideConnectionData.m_Type & OutsideConnectionTransferType.Road) != OutsideConnectionTransferType.None)
				{
					array[0]++;
				}
				if ((outsideConnectionData.m_Type & OutsideConnectionTransferType.Air) != OutsideConnectionTransferType.None)
				{
					array[1]++;
				}
				if ((outsideConnectionData.m_Type & OutsideConnectionTransferType.Train) != OutsideConnectionTransferType.None)
				{
					array[2]++;
				}
				if ((outsideConnectionData.m_Type & OutsideConnectionTransferType.Ship) != OutsideConnectionTransferType.None)
				{
					array[3]++;
				}
			}
		}
		Debug.Log((object)$"{val.Length} commuter households distribution:");
		Debug.Log((object)$"Road:{array[0]}");
		Debug.Log((object)$"Air:{array[1]}");
		Debug.Log((object)$"Train:{array[2]}");
		Debug.Log((object)$"Ship:{array[3]}");
	}

	private void CalculateStudentsFromOC()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Citizens.Student>(),
			ComponentType.ReadOnly<Citizen>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		ComponentLookup<Citizen> componentLookup = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<Entity> val2 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		int num = 0;
		for (int i = 0; i < val2.Length; i++)
		{
			Entity val3 = val2[i];
			if ((componentLookup[val3].m_State & CitizenFlags.Commuter) != CitizenFlags.None)
			{
				num++;
			}
		}
		Debug.Log((object)$"Students from OC: {num}");
	}

	private void CalculateEligible()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[6]
		{
			ComponentType.ReadOnly<Household>(),
			ComponentType.ReadOnly<PropertyRenter>(),
			ComponentType.ReadOnly<HouseholdCitizen>(),
			ComponentType.Exclude<TouristHousehold>(),
			ComponentType.Exclude<CommuterHousehold>(),
			ComponentType.Exclude<MovingAway>()
		});
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<TimeData>() });
		EntityQuery entityQuery2 = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		EntityQuery entityQuery3 = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EducationParameterData>() });
		TimeData singleton = ((EntityQuery)(ref entityQuery)).GetSingleton<TimeData>();
		EconomyParameterData economyParameters = ((EntityQuery)(ref entityQuery2)).GetSingleton<EconomyParameterData>();
		EducationParameterData educationParameterData = ((EntityQuery)(ref entityQuery3)).GetSingleton<EducationParameterData>();
		NativeArray<Entity> val2 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		ComponentLookup<HealthProblem> healthProblems = InternalCompilerInterface.GetComponentLookup<HealthProblem>(ref __TypeHandle.__Game_Citizens_HealthProblem_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Game.Citizens.Student> componentLookup = InternalCompilerInterface.GetComponentLookup<Game.Citizens.Student>(ref __TypeHandle.__Game_Citizens_Student_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Citizen> componentLookup2 = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Worker> componentLookup3 = InternalCompilerInterface.GetComponentLookup<Worker>(ref __TypeHandle.__Game_Citizens_Worker_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<ServiceFee> bufferLookup = InternalCompilerInterface.GetBufferLookup<ServiceFee>(ref __TypeHandle.__Game_City_ServiceFee_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		BufferLookup<CityModifier> bufferLookup2 = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef);
		Entity city = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<CitySystem>().City;
		uint frameIndex = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<SimulationSystem>().frameIndex;
		DynamicBuffer<ServiceFee> fees = bufferLookup[city];
		DynamicBuffer<CityModifier> val3 = bufferLookup2[city];
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		float num11 = 0f;
		float num12 = 0f;
		float num13 = 0f;
		float num14 = 0f;
		float num15 = 0f;
		float num16 = 0f;
		float num17 = 0f;
		float num18 = 0f;
		float num19 = 0f;
		float num20 = 0f;
		float num21 = 0f;
		float num22 = 0f;
		float num23 = 0f;
		for (int i = 0; i < val2.Length; i++)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<HouseholdCitizen> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HouseholdCitizen>(val2[i], true);
			for (int j = 0; j < buffer.Length; j++)
			{
				Entity citizen = buffer[j].m_Citizen;
				if (CitizenUtils.IsDead(citizen, ref healthProblems))
				{
					continue;
				}
				Citizen citizen2 = componentLookup2[citizen];
				CitizenAge age = citizen2.GetAge();
				if (componentLookup.HasComponent(citizen))
				{
					switch (componentLookup[citizen].m_Level)
					{
					case 1:
						num8 += 1f;
						num += 1f;
						break;
					case 2:
						num9 += 1f;
						num2 += 1f;
						break;
					case 3:
						num10 += 1f;
						num3 += 1f;
						break;
					case 4:
						num11 += 1f;
						num4 += 1f;
						break;
					}
					continue;
				}
				if (age == CitizenAge.Child)
				{
					num += 1f;
					num5 += 1f;
					continue;
				}
				bool flag = componentLookup3.HasComponent(citizen);
				if (citizen2.GetFailedEducationCount() >= 3)
				{
					continue;
				}
				SchoolData schoolData = default(SchoolData);
				Random pseudoRandom = citizen2.GetPseudoRandom(CitizenPseudoRandom.StudyWillingness);
				float num24 = ((Random)(ref pseudoRandom)).NextFloat();
				switch (citizen2.GetEducationLevel())
				{
				case 1:
				{
					float fee = ServiceFeeSystem.GetFee(PlayerResource.SecondaryEducation, fees);
					float enteringProbability = ApplyToSchoolSystem.GetEnteringProbability(age, flag, 2, citizen2.m_WellBeing, num24, val3, ref educationParameterData);
					float dropoutProbability = GraduationSystem.GetDropoutProbability(citizen2, 2, 500f, fee, 0, frameIndex, ref economyParameters, schoolData, val3, 1f, singleton);
					num2 += enteringProbability * (1f - dropoutProbability);
					num15 += dropoutProbability;
					num12 += enteringProbability;
					num6 += 1f;
					break;
				}
				case 2:
				{
					float fee = ServiceFeeSystem.GetFee(PlayerResource.HigherEducation, fees);
					float enteringProbability = ApplyToSchoolSystem.GetEnteringProbability(age, flag, 4, citizen2.m_WellBeing, num24, val3, ref educationParameterData);
					float dropoutProbability = GraduationSystem.GetDropoutProbability(citizen2, 4, 500f, fee, 0, frameIndex, ref economyParameters, schoolData, val3, 1f, singleton);
					num4 += enteringProbability * dropoutProbability;
					num17 += dropoutProbability;
					num14 += enteringProbability;
					enteringProbability = ApplyToSchoolSystem.GetEnteringProbability(age, flag, 3, citizen2.m_WellBeing, num24, val3, ref educationParameterData);
					dropoutProbability = GraduationSystem.GetDropoutProbability(citizen2, 3, 500f, fee, 0, frameIndex, ref economyParameters, schoolData, val3, 1f, singleton);
					num3 += enteringProbability * (1f - dropoutProbability);
					num16 += dropoutProbability;
					num13 += enteringProbability;
					num7 += 1f;
					switch (age)
					{
					case CitizenAge.Teen:
						num20 += 1f;
						break;
					case CitizenAge.Adult:
						num21 += 1f;
						break;
					default:
						num23 += 1f;
						break;
					}
					if (flag)
					{
						num22 += 1f;
					}
					num19 += (float)(int)citizen2.m_WellBeing;
					num18 += num24;
					break;
				}
				}
			}
		}
		num6 = math.max(num6, 0.1f);
		num7 = math.max(num7, 0.1f);
		Debug.Log((object)$"Elementary: eligible {num} students {num8} total {num5}");
		Debug.Log((object)$"High school: eligible {num2} students {num9} total {num6} enter {num12} drop {num15}");
		Debug.Log((object)$"College eligible {num3} students {num10} total {num7} enter {num13 / num7} drop {num16 / num7}");
		Debug.Log((object)$"University eligible {num4} students {num11} total {num7} enter {num14 / num7} drop {num17 / num7}");
		Debug.Log((object)$"Highest teens {num20} adults {num21} elders {num23} workers {num22} wellb {num19 / num7} willi {num18 / num7}");
	}

	private void CalculateCustomers()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		ILog logger = LogManager.GetLogger("customers");
		EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager2)).CreateEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ServiceAvailable>(),
			ComponentType.ReadOnly<PropertyRenter>()
		});
		NativeArray<Entity> val2 = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		for (int i = 0; i < val2.Length; i++)
		{
			Entity val3 = val2[i];
			Entity prefab = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val3).m_Prefab;
			Entity property = ((EntityManager)(ref entityManager)).GetComponentData<PropertyRenter>(val3).m_Property;
			((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(property);
			Building componentData = ((EntityManager)(ref entityManager)).GetComponentData<Building>(property);
			DynamicBuffer<ResourceAvailability> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ResourceAvailability>(componentData.m_RoadEdge, true);
			ServiceAvailable componentData2 = ((EntityManager)(ref entityManager)).GetComponentData<ServiceAvailable>(val3);
			ServiceCompanyData componentData3 = ((EntityManager)(ref entityManager)).GetComponentData<ServiceCompanyData>(prefab);
			float num = 1f / NetUtils.GetAvailability(buffer, AvailableResource.Workplaces, componentData.m_CurvePosition);
			float num2 = 1f / NetUtils.GetAvailability(buffer, AvailableResource.EducatedCitizens, componentData.m_CurvePosition);
			float num3 = 1f / NetUtils.GetAvailability(buffer, AvailableResource.UneducatedCitizens, componentData.m_CurvePosition);
			float num4 = 1f / NetUtils.GetAvailability(buffer, AvailableResource.Services, componentData.m_CurvePosition);
			float num5 = 1f / NetUtils.GetAvailability(buffer, AvailableResource.Attractiveness, componentData.m_CurvePosition);
			float num6 = math.saturate(1f - (float)componentData2.m_ServiceAvailable / (float)componentData3.m_MaxService);
			logger.InfoFormat("{0},{1},{2},{3},{4},{5}", new object[6] { num6, num, num2, num3, num4, num5 });
		}
		Debug.Log((object)"Done");
	}

	private void TriggerTestLifeEvent()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		EntityQuery val = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Followed>(),
			ComponentType.ReadOnly<Citizen>()
		});
		NativeQueue<TriggerAction> val2 = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<TriggerSystem>().CreateActionBuffer();
		Enumerator<Entity> enumerator = ((EntityQuery)(ref val)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				val2.Enqueue(new TriggerAction(TriggerType.CitizenFailedSchool, Entity.Null, current, current));
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	private void HappinessFactors()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		CitizenHappinessSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitizenHappinessSystem>();
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<HappinessFactorParameterData>() });
		Entity singletonEntity = ((EntityQuery)(ref entityQuery)).GetSingletonEntity();
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<HappinessFactorParameterData> buffer = ((EntityManager)(ref entityManager)).GetBuffer<HappinessFactorParameterData>(singletonEntity, true);
		ComponentLookup<Locked> locked = InternalCompilerInterface.GetComponentLookup<Locked>(ref __TypeHandle.__Game_Prefabs_Locked_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		for (int i = 0; i < 25; i++)
		{
			float3 happinessFactor = orCreateSystemManaged.GetHappinessFactor((CitizenHappinessSystem.HappinessFactor)i, buffer, ref locked);
			Debug.Log((object)$"{(CitizenHappinessSystem.HappinessFactor)i}: {happinessFactor.x} ({happinessFactor.y}+{happinessFactor.z})");
		}
	}

	private Container CreateToolUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		//IL_00eb: Expected O, but got Unknown
		Container val = new Container();
		ToolBaseSystem activeTool = m_ToolSystem.activeTool;
		if (activeTool is DefaultToolSystem)
		{
			DefaultToolSystemUI(val);
		}
		else if (activeTool is BulldozeToolSystem)
		{
			BulldozeToolSystemUI(val);
		}
		else if (activeTool is NetToolSystem)
		{
			NetToolSystemUI(val);
		}
		else if (activeTool is ObjectToolSystem)
		{
			ObjectToolSystemUI(val);
		}
		else if (activeTool is ZoneToolSystem)
		{
			ZoneToolSystemUI(val);
		}
		else if (activeTool is AreaToolSystem)
		{
			AreaToolSystemUI(val);
		}
		else if (activeTool is RouteToolSystem)
		{
			RouteToolSystemUI(val);
		}
		else if (activeTool is UpgradeToolSystem)
		{
			UpgradeToolSystemUI(val);
		}
		else if (activeTool is TerrainToolSystem)
		{
			TerrainToolSystemUI(val);
		}
		ObservableList<Widget> children = val.children;
		BoolField val2 = new BoolField
		{
			displayName = "Bypass validation results"
		};
		((Field<bool>)val2).getter = () => m_ToolSystem.ignoreErrors;
		((Field<bool>)val2).setter = delegate(bool value)
		{
			m_ToolSystem.ignoreErrors = value;
		};
		children.Add((Widget)val2);
		ToolBrushUI(val, m_ToolSystem.activeTool);
		ToolSnapUI(val, m_ToolSystem.activeTool);
		return val;
	}

	private void BuildingMoveUI(Container container, Entity entity, Entity prefab)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		EntityManager entityManager;
		if (m_ToolSystem.actionMode.IsEditor())
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Transform>(entity))
			{
				ObjectToolSystem objectToolSystem = GetTool<ObjectToolSystem>();
				container.children.Add((Widget)new Button
				{
					displayName = "Move",
					action = delegate
					{
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						objectToolSystem.StartMoving(m_ToolSystem.selected);
						m_ToolSystem.activeTool = objectToolSystem;
					}
				});
			}
		}
		else
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<Building>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(prefab))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Destroyed>(entity))
					{
						UpgradeToolSystem uts = GetTool<UpgradeToolSystem>();
						container.children.Add((Widget)new Button
						{
							displayName = "Rebuild",
							action = delegate
							{
								uts.prefab = null;
								m_ToolSystem.activeTool = uts;
							}
						});
					}
					else
					{
						ObjectToolSystem objectToolSystem2 = GetTool<ObjectToolSystem>();
						container.children.Add((Widget)new Button
						{
							displayName = "Relocate",
							action = delegate
							{
								//IL_0016: Unknown result type (might be due to invalid IL or missing references)
								objectToolSystem2.StartMoving(m_ToolSystem.selected);
								m_ToolSystem.activeTool = objectToolSystem2;
							}
						});
					}
				}
			}
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.Object>(entity))
		{
			container.children.Add((Widget)new Button
			{
				displayName = "Update",
				action = delegate
				{
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0010: Unknown result type (might be due to invalid IL or missing references)
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
					((EntityCommandBuffer)(ref val)).AddComponent<Updated>(entity, default(Updated));
				}
			});
		}
	}

	private void BuildingUpgradeUI(Container container, Entity prefab)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<BuildingUpgradeElement>(prefab))
		{
			return;
		}
		entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<BuildingUpgradeElement> buffer = ((EntityManager)(ref entityManager)).GetBuffer<BuildingUpgradeElement>(prefab, true);
		for (int i = 0; i < buffer.Length; i++)
		{
			Entity upgrade = buffer[i].m_Upgrade;
			PrefabBase prefab2 = m_PrefabSystem.GetPrefab<PrefabBase>(upgrade);
			container.children.Add((Widget)new Button
			{
				displayName = "Upgrade " + ((Object)prefab2).name,
				action = delegate
				{
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					Upgrade(upgrade);
				}
			});
		}
		void Upgrade(Entity val)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager2)).HasComponent<PlaceableObjectData>(val))
			{
				ObjectToolSystem tool = GetTool<ObjectToolSystem>();
				tool.mode = ObjectToolSystem.Mode.Upgrade;
				tool.prefab = m_PrefabSystem.GetPrefab<ObjectPrefab>(val);
				m_ToolSystem.activeTool = tool;
			}
			else
			{
				UpgradeToolSystem tool2 = GetTool<UpgradeToolSystem>();
				tool2.prefab = m_PrefabSystem.GetPrefab<ObjectPrefab>(val);
				m_ToolSystem.activeTool = tool2;
			}
		}
	}

	private void LevelUpUI(Container container, Entity entity, Entity prefab)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<SpawnableBuildingData>(prefab))
		{
			PrefabBase prefab2 = m_PrefabSystem.GetPrefab<PrefabBase>(prefab);
			container.children.Add((Widget)new Button
			{
				displayName = "Level up " + ((Object)prefab2).name,
				action = delegate
				{
					LevelUp();
				}
			});
			container.children.Add((Widget)new Button
			{
				displayName = "Abandon " + ((Object)prefab2).name,
				action = delegate
				{
					Abandon();
				}
			});
		}
		void Abandon()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			m_BuildingUpkeepSystem.DebugLevelDown(entity, ((SystemBase)this).GetComponentLookup<BuildingCondition>(false), ((SystemBase)this).GetComponentLookup<SpawnableBuildingData>(true), ((SystemBase)this).GetComponentLookup<PrefabRef>(true), ((SystemBase)this).GetComponentLookup<ZoneData>(true), ((SystemBase)this).GetComponentLookup<BuildingPropertyData>(true));
		}
		void LevelUp()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			m_BuildingUpkeepSystem.DebugLevelUp(entity, ((SystemBase)this).GetComponentLookup<BuildingCondition>(false), ((SystemBase)this).GetComponentLookup<SpawnableBuildingData>(true), ((SystemBase)this).GetComponentLookup<PrefabRef>(true), ((SystemBase)this).GetComponentLookup<ZoneData>(true), ((SystemBase)this).GetComponentLookup<BuildingPropertyData>(true));
		}
	}

	private void ServiceDistrictUI(Container container, Entity entity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<ServiceDistrict>(entity))
		{
			container.children.Add((Widget)new Button
			{
				displayName = "Select districts",
				action = delegate
				{
					SelectDistricts();
				}
			});
		}
		void SelectDistricts()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			SelectionToolSystem tool = GetTool<SelectionToolSystem>();
			tool.selectionType = SelectionType.ServiceDistrict;
			tool.selectionOwner = entity;
			m_ToolSystem.activeTool = tool;
		}
	}

	private void PolicyUI(Container container, Entity entity)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Expected O, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Expected O, but got Unknown
		//IL_01fe: Expected O, but got Unknown
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (!((EntityManager)(ref entityManager)).HasComponent<Policy>(entity))
		{
			return;
		}
		Container val = (Container)new Foldout();
		((Widget)val).displayName = "Policies";
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).GetBuffer<Policy>(entity, true);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		bool flag = ((EntityManager)(ref entityManager)).HasComponent<District>(entity);
		bool flag2 = m_CitySystem.City == entity;
		NativeArray<ArchetypeChunk> val2 = ((EntityQuery)(ref m_PolicyQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<DistrictOptionData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<DistrictOptionData>(ref __TypeHandle.__Game_Prefabs_DistrictOptionData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<DistrictModifierData> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<DistrictModifierData>(ref __TypeHandle.__Game_Prefabs_DistrictModifierData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<CityOptionData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<CityOptionData>(ref __TypeHandle.__Game_Prefabs_CityOptionData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			BufferTypeHandle<CityModifierData> bufferTypeHandle2 = InternalCompilerInterface.GetBufferTypeHandle<CityModifierData>(ref __TypeHandle.__Game_Prefabs_CityModifierData_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val2.Length; i++)
			{
				ArchetypeChunk val3 = val2[i];
				if ((flag && !((ArchetypeChunk)(ref val3)).Has<DistrictOptionData>(ref componentTypeHandle) && !((ArchetypeChunk)(ref val3)).Has<DistrictModifierData>(ref bufferTypeHandle)) || (flag2 && !((ArchetypeChunk)(ref val3)).Has<CityOptionData>(ref componentTypeHandle2) && !((ArchetypeChunk)(ref val3)).Has<CityModifierData>(ref bufferTypeHandle2)) || (!flag2 && (((ArchetypeChunk)(ref val3)).Has<CityOptionData>(ref componentTypeHandle2) || ((ArchetypeChunk)(ref val3)).Has<CityModifierData>(ref bufferTypeHandle2))))
				{
					continue;
				}
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray(entityTypeHandle);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					Entity policyEntity = nativeArray[j];
					PolicyPrefab prefab = m_PrefabSystem.GetPrefab<PolicyPrefab>(policyEntity);
					ObservableList<Widget> children = val.children;
					BoolField val4 = new BoolField
					{
						displayName = ((Object)prefab).name
					};
					((Field<bool>)val4).getter = () => TryGetPolicy(entity, policyEntity, out var policy) && (policy.m_Flags & PolicyFlags.Active) != 0;
					((Field<bool>)val4).setter = delegate
					{
						//IL_0013: Unknown result type (might be due to invalid IL or missing references)
						//IL_0019: Unknown result type (might be due to invalid IL or missing references)
						//IL_0042: Unknown result type (might be due to invalid IL or missing references)
						//IL_0047: Unknown result type (might be due to invalid IL or missing references)
						//IL_004c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0088: Unknown result type (might be due to invalid IL or missing references)
						//IL_008d: Unknown result type (might be due to invalid IL or missing references)
						//IL_009b: Unknown result type (might be due to invalid IL or missing references)
						//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
						//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
						//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
						//IL_00af: Unknown result type (might be due to invalid IL or missing references)
						//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
						//IL_0063: Unknown result type (might be due to invalid IL or missing references)
						//IL_0068: Unknown result type (might be due to invalid IL or missing references)
						//IL_006d: Unknown result type (might be due to invalid IL or missing references)
						bool flag3 = false;
						if (TryGetPolicy(entity, policyEntity, out policy))
						{
							flag3 = (policy.m_Flags & PolicyFlags.Active) != 0;
						}
						else
						{
							flag3 = false;
							EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager2)).HasComponent<PolicySliderData>(policyEntity))
							{
								entityManager2 = ((ComponentSystemBase)this).EntityManager;
								((EntityManager)(ref entityManager2)).GetComponentData<PolicySliderData>(policyEntity);
							}
						}
						EntityCommandBuffer val5 = m_EndFrameBarrier.CreateCommandBuffer();
						Entity val6 = ((EntityCommandBuffer)(ref val5)).CreateEntity(m_PolicyEventArchetype);
						((EntityCommandBuffer)(ref val5)).SetComponent<Modify>(val6, new Modify(entity, policyEntity, !flag3, policy.m_Adjustment));
					};
					children.Add((Widget)val4);
				}
			}
		}
		finally
		{
			val2.Dispose();
		}
		container.children.Add((Widget)(object)val);
	}

	private bool TryGetPolicy(Entity entity, Entity policyType, out Policy policy)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		DynamicBuffer<Policy> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Policy>(entity, true);
		for (int i = 0; i < buffer.Length; i++)
		{
			if (buffer[i].m_Policy == policyType)
			{
				policy = buffer[i];
				return true;
			}
		}
		policy = default(Policy);
		return false;
	}

	private T SelectNext<T>(EntityQuery group, T current) where T : PrefabBase
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Entity val = ((!((Object)(object)current != (Object)null)) ? SelectNext(group, Entity.Null) : SelectNext(group, m_PrefabSystem.GetEntity(current)));
		if (val != Entity.Null)
		{
			return m_PrefabSystem.GetPrefab<T>(val);
		}
		return null;
	}

	private Entity SelectNext(EntityQuery group, Entity current)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref group)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		bool flag = false;
		for (int i = 0; i < 2; i++)
		{
			for (int j = 0; j < val.Length; j++)
			{
				ArchetypeChunk val2 = val[j];
				NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
				for (int k = 0; k < nativeArray.Length; k++)
				{
					if (flag)
					{
						val.Dispose();
						return nativeArray[k];
					}
					flag = nativeArray[k] == current;
				}
			}
			flag = true;
		}
		val.Dispose();
		return current;
	}

	private void DefaultToolSystemUI(Container container)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Expected O, but got Unknown
		//IL_029a: Expected O, but got Unknown
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Expected O, but got Unknown
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Expected O, but got Unknown
		//IL_02d9: Expected O, but got Unknown
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Expected O, but got Unknown
		if (m_ToolSystem.actionMode.IsEditor())
		{
			container.children.Add((Widget)new Button
			{
				displayName = "Select start tiles",
				action = delegate
				{
					SelectMapTile();
				}
			});
		}
		if (m_ToolSystem.selected == Entity.Null)
		{
			container.children.Add((Widget)new Value
			{
				displayName = "Selected",
				getter = () => "None"
			});
			PolicyUI(container, m_CitySystem.City);
		}
		else
		{
			Entity entity = m_ToolSystem.selected;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).HasComponent<PrefabRef>(entity))
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity);
				string prefabName = m_PrefabSystem.GetPrefabName(componentData.m_Prefab);
				container.children.Add((Widget)new Value
				{
					displayName = "Selected",
					getter = () => prefabName
				});
				container.children.Add((Widget)new Value
				{
					displayName = "Entity Id",
					getter = () => $"({entity.Index})"
				});
				if (EntitiesExtensions.HasEnabledComponent<PrefabData>(((ComponentSystemBase)this).EntityManager, componentData.m_Prefab))
				{
					BuildingMoveUI(container, entity, componentData.m_Prefab);
					BuildingUpgradeUI(container, componentData.m_Prefab);
					LevelUpUI(container, entity, componentData.m_Prefab);
					ServiceDistrictUI(container, entity);
					PolicyUI(container, entity);
				}
			}
			else
			{
				container.children.Add((Widget)new Value
				{
					displayName = "Selected",
					getter = () => $"({entity.Index})"
				});
			}
		}
		if (!((EntityQuery)(ref m_DebugQuery)).IsEmptyIgnoreFilter)
		{
			container.children.Add((Widget)new Button
			{
				displayName = "Clear debug selection",
				action = delegate
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager2)).RemoveComponent<Game.Tools.Debug>(m_DebugQuery);
				}
			});
		}
		DefaultToolSystem dts = (DefaultToolSystem)m_ToolSystem.activeTool;
		ObservableList<Widget> children = container.children;
		BoolField val = new BoolField
		{
			displayName = "Allow gameplay manipulation"
		};
		((Field<bool>)val).getter = () => dts.allowManipulation;
		((Field<bool>)val).setter = delegate(bool value)
		{
			dts.allowManipulation = value;
		};
		children.Add((Widget)val);
		ObservableList<Widget> children2 = container.children;
		BoolField val2 = new BoolField
		{
			displayName = "Debug toggle"
		};
		((Field<bool>)val2).getter = () => dts.debugSelect;
		((Field<bool>)val2).setter = delegate(bool value)
		{
			dts.debugSelect = value;
		};
		children2.Add((Widget)val2);
		void SelectMapTile()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			SelectionToolSystem tool = GetTool<SelectionToolSystem>();
			tool.selectionType = SelectionType.MapTiles;
			tool.selectionOwner = Entity.Null;
			m_ToolSystem.activeTool = tool;
		}
	}

	private void BulldozeToolSystemUI(Container container)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0051: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0090: Expected O, but got Unknown
		BulldozeToolSystem bts = GetTool<BulldozeToolSystem>();
		ObservableList<Widget> children = container.children;
		BoolField val = new BoolField
		{
			displayName = "Allow gameplay manipulation"
		};
		((Field<bool>)val).getter = () => bts.allowManipulation;
		((Field<bool>)val).setter = delegate(bool value)
		{
			bts.allowManipulation = value;
		};
		children.Add((Widget)val);
		ObservableList<Widget> children2 = container.children;
		BoolField val2 = new BoolField
		{
			displayName = "Bypass confirmation"
		};
		((Field<bool>)val2).getter = () => bts.debugBypassBulldozeConfirmation;
		((Field<bool>)val2).setter = delegate(bool value)
		{
			bts.debugBypassBulldozeConfirmation = value;
		};
		children2.Add((Widget)val2);
	}

	private void NetToolSystemUI(Container container)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Expected O, but got Unknown
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Expected O, but got Unknown
		//IL_0303: Expected O, but got Unknown
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Expected O, but got Unknown
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Expected O, but got Unknown
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		NetToolSystem nts = GetTool<NetToolSystem>();
		if ((Object)(object)nts.GetPrefab() == (Object)null)
		{
			nts.prefab = SelectNext(m_NetQuery, nts.prefab);
		}
		container.children.Add((Widget)new Value
		{
			displayName = "Type",
			getter = () => ((Object)nts.GetPrefab()).name
		});
		container.children.Add((Widget)new Button
		{
			displayName = "Select next",
			action = delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				nts.prefab = SelectNext(m_NetQuery, nts.prefab);
			}
		});
		ObservableList<Widget> children = container.children;
		EnumField val = new EnumField
		{
			displayName = "Mode"
		};
		((Field<int>)val).getter = () => (int)nts.mode;
		((Field<int>)val).setter = delegate(int value)
		{
			nts.mode = (NetToolSystem.Mode)value;
		};
		((Field<int>)val).onValueChanged = RebuildSimulationDebugUI;
		val.autoEnum = typeof(NetToolSystem.Mode);
		val.getIndex = () => (int)nts.mode;
		val.setIndex = delegate(int value)
		{
			nts.mode = (NetToolSystem.Mode)value;
		};
		children.Add((Widget)val);
		Bounds1 elevationLimits = default(Bounds1);
		if ((Object)(object)nts.prefab != (Object)null && m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)nts.prefab, out PlaceableNetData component))
		{
			elevationLimits = component.m_ElevationRange;
			if (component.m_UndergroundPrefab != Entity.Null)
			{
				NetPrefab prefab = m_PrefabSystem.GetPrefab<NetPrefab>(component.m_UndergroundPrefab);
				if (m_PrefabSystem.TryGetComponentData<PlaceableNetData>((PrefabBase)prefab, out component))
				{
					elevationLimits |= component.m_ElevationRange;
				}
			}
		}
		if (elevationLimits.max > elevationLimits.min)
		{
			ObservableList<Widget> children2 = container.children;
			FloatField val2 = new FloatField
			{
				displayName = "Elevation Step"
			};
			((Field<float>)val2).getter = () => nts.elevationStep;
			((Field<float>)val2).setter = delegate(float value)
			{
				if (value > 7.5f)
				{
					nts.elevationStep = 10f;
				}
				else if (value > 3.75f)
				{
					nts.elevationStep = 5f;
				}
				else if (value > 1.875f)
				{
					nts.elevationStep = 2.5f;
				}
				else
				{
					nts.elevationStep = 1.25f;
				}
			};
			val2.min = () => 1.25f;
			val2.max = () => 10f;
			children2.Add((Widget)val2);
			ObservableList<Widget> children3 = container.children;
			FloatField val3 = new FloatField
			{
				displayName = "Elevation"
			};
			((Field<float>)val3).getter = () => nts.elevation;
			((Field<float>)val3).setter = delegate(float value)
			{
				nts.elevation = math.round(value / nts.elevationStep) * nts.elevationStep;
			};
			val3.min = () => elevationLimits.min;
			val3.max = () => elevationLimits.max;
			children3.Add((Widget)val3);
		}
		ObservableList<Widget> children4 = container.children;
		BoolField val4 = new BoolField
		{
			displayName = "Parallel Road"
		};
		((Field<bool>)val4).getter = () => nts.parallelCount > 0;
		((Field<bool>)val4).setter = delegate(bool value)
		{
			nts.parallelCount = (value ? 1 : 0);
		};
		children4.Add((Widget)val4);
	}

	private void ObjectToolSystemUI(Container container)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_012a: Expected O, but got Unknown
		ObjectToolSystem ots = GetTool<ObjectToolSystem>();
		if ((Object)(object)ots.GetPrefab() == (Object)null)
		{
			ots.mode = ObjectToolSystem.Mode.Create;
			ots.prefab = SelectNext(m_ObjectQuery, ots.prefab);
		}
		container.children.Add((Widget)new Value
		{
			displayName = "Type",
			getter = () => ((Object)ots.GetPrefab()).name
		});
		container.children.Add((Widget)new Button
		{
			displayName = "Select next",
			action = delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				ots.prefab = SelectNext(m_ObjectQuery, ots.prefab);
			}
		});
		if (!ots.allowAge)
		{
			return;
		}
		for (int num = 0; num < 4; num++)
		{
			Game.Tools.AgeMask age = (Game.Tools.AgeMask)(1 << num);
			ObservableList<Widget> children = container.children;
			BoolField val = new BoolField
			{
				displayName = age.ToString()
			};
			((Field<bool>)val).getter = () => (ots.ageMask & age) != 0;
			((Field<bool>)val).setter = delegate(bool value)
			{
				ots.ageMask = (Game.Tools.AgeMask)(((uint)ots.ageMask & (uint)(byte)(~(int)age)) | (uint)(value ? age : ((Game.Tools.AgeMask)0)));
			};
			children.Add((Widget)val);
		}
	}

	private void ZoneToolSystemUI(Container container)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		//IL_016c: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		ZoneToolSystem zts = GetTool<ZoneToolSystem>();
		if ((Object)(object)zts.GetPrefab() == (Object)null)
		{
			zts.prefab = SelectNext(m_ZoneQuery, zts.prefab);
		}
		container.children.Add((Widget)new Value
		{
			displayName = "Type",
			getter = () => ((Object)zts.GetPrefab()).name
		});
		container.children.Add((Widget)new Button
		{
			displayName = "Select next",
			action = delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				zts.prefab = SelectNext(m_ZoneQuery, zts.prefab);
			}
		});
		ObservableList<Widget> children = container.children;
		EnumField val = new EnumField
		{
			displayName = "Mode"
		};
		((Field<int>)val).getter = () => (int)zts.mode;
		((Field<int>)val).setter = delegate(int value)
		{
			zts.mode = (ZoneToolSystem.Mode)value;
		};
		((Field<int>)val).onValueChanged = RebuildSimulationDebugUI;
		val.autoEnum = typeof(ZoneToolSystem.Mode);
		val.getIndex = () => (int)zts.mode;
		val.setIndex = delegate(int value)
		{
			zts.mode = (ZoneToolSystem.Mode)value;
		};
		children.Add((Widget)val);
		ObservableList<Widget> children2 = container.children;
		BoolField val2 = new BoolField
		{
			displayName = "Overwrite existing zone"
		};
		((Field<bool>)val2).getter = () => zts.overwrite;
		((Field<bool>)val2).setter = delegate(bool value)
		{
			zts.overwrite = value;
		};
		children2.Add((Widget)val2);
	}

	private void AreaToolSystemUI(Container container)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		AreaToolSystem ats = GetTool<AreaToolSystem>();
		if ((Object)(object)ats.GetPrefab() == (Object)null)
		{
			ats.prefab = SelectNext(m_AreaQuery, ats.prefab);
		}
		container.children.Add((Widget)new Value
		{
			displayName = "Type",
			getter = () => ((Object)ats.prefab).name
		});
		container.children.Add((Widget)new Button
		{
			displayName = "Select next",
			action = delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				ats.prefab = SelectNext(m_AreaQuery, ats.prefab);
			}
		});
	}

	private void RouteToolSystemUI(Container container)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		RouteToolSystem rts = GetTool<RouteToolSystem>();
		if ((Object)(object)rts.GetPrefab() == (Object)null)
		{
			rts.prefab = SelectNext(m_RouteQuery, rts.prefab);
		}
		container.children.Add((Widget)new Value
		{
			displayName = "Type",
			getter = () => ((Object)rts.GetPrefab()).name
		});
		container.children.Add((Widget)new Button
		{
			displayName = "Select next",
			action = delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				rts.prefab = SelectNext(m_RouteQuery, rts.prefab);
			}
		});
	}

	private void UpgradeToolSystemUI(Container container)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		UpgradeToolSystem uts = GetTool<UpgradeToolSystem>();
		InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		if (m_ToolSystem.selected != Entity.Null)
		{
			Entity entity = m_ToolSystem.selected;
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(entity);
			PrefabBase prefabBase = m_PrefabSystem.GetPrefab<PrefabBase>(componentData);
			container.children.Add((Widget)new Value
			{
				displayName = "Selected",
				getter = () => ((Object)prefabBase).name
			});
			container.children.Add((Widget)new Value
			{
				displayName = "Entity Id",
				getter = () => $"({entity.Index})"
			});
		}
		if ((Object)(object)uts.prefab != (Object)null)
		{
			container.children.Add((Widget)new Value
			{
				displayName = "Upgrade",
				getter = () => ((Object)uts.prefab).name
			});
		}
	}

	private void TerrainToolSystemUI(Container container)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		TerrainToolSystem tts = GetTool<TerrainToolSystem>();
		if ((Object)(object)tts.GetPrefab() == (Object)null)
		{
			tts.SetPrefab(SelectNext(m_TerraformingQuery, tts.prefab));
		}
		container.children.Add((Widget)new Value
		{
			displayName = "Type",
			getter = () => ((Object)tts.GetPrefab()).name
		});
		container.children.Add((Widget)new Button
		{
			displayName = "Select next",
			action = delegate
			{
				//IL_0012: Unknown result type (might be due to invalid IL or missing references)
				tts.SetPrefab(SelectNext(m_TerraformingQuery, tts.prefab));
			}
		});
	}

	private Widget InfoviewUI()
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		//IL_0066: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_011b: Expected O, but got Unknown
		if (!((EntityQuery)(ref m_InfoviewQuery)).IsEmptyIgnoreFilter)
		{
			Container val = new Container
			{
				displayName = "Infoviews"
			};
			Button val2 = new Button
			{
				displayName = "Infoview: " + (((Object)(object)m_ToolSystem.infoview != (Object)null) ? ((Object)m_ToolSystem.infoview).name : "None") + " >>"
			};
			val2.action = delegate
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				m_ToolSystem.infoview = SelectNext(m_InfoviewQuery, m_ToolSystem.infoview);
				Rebuild(BuildSimulationDebugUI);
			};
			val.children.Add((Widget)(object)val2);
			Foldout val3 = new Foldout
			{
				displayName = "Infomodes"
			};
			List<InfomodeInfo> infoviewInfomodes = m_ToolSystem.GetInfoviewInfomodes();
			if (infoviewInfomodes != null)
			{
				foreach (InfomodeInfo infomode in infoviewInfomodes)
				{
					ObservableList<Widget> children = ((Container)val3).children;
					BoolField val4 = new BoolField
					{
						displayName = ((Object)infomode.m_Mode).name
					};
					((Field<bool>)val4).getter = () => m_ToolSystem.IsInfomodeActive(infomode.m_Mode);
					((Field<bool>)val4).setter = delegate(bool active)
					{
						m_ToolSystem.SetInfomodeActive(infomode.m_Mode, active, infomode.m_Priority);
					};
					children.Add((Widget)val4);
				}
				val.children.Add((Widget)(object)val3);
			}
			return (Widget)(object)val;
		}
		return (Widget)new Button
		{
			displayName = "No infoviews",
			action = delegate
			{
			}
		};
	}

	private Foldout CreateEventUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		Foldout val = new Foldout();
		((Widget)val).displayName = "Start event";
		if (!((EntityQuery)(ref m_EventQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val2 = ((EntityQuery)(ref m_EventQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			try
			{
				for (int i = 0; i < val2.Length; i++)
				{
					Entity entity = val2[i];
					EventPrefab prefab = m_PrefabSystem.GetPrefab<EventPrefab>(entity);
					((Container)val).children.Add((Widget)new Button
					{
						displayName = ((Object)prefab).name,
						action = delegate
						{
							//IL_0007: Unknown result type (might be due to invalid IL or missing references)
							StartEvent(entity);
						}
					});
				}
			}
			finally
			{
				val2.Dispose();
			}
		}
		Game.City.DangerLevel dangerLevel = default(Game.City.DangerLevel);
		((Container)val).children.Add((Widget)new Value
		{
			displayName = "Danger Level",
			getter = () => (m_CitySystem.City != Entity.Null && EntitiesExtensions.TryGetComponent<Game.City.DangerLevel>(((ComponentSystemBase)this).EntityManager, m_CitySystem.City, ref dangerLevel)) ? ((object)dangerLevel.m_DangerLevel) : ((object)0f)
		});
		return val;
	}

	private void StartEvent(Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		EventData componentData = ((EntityManager)(ref entityManager)).GetComponentData<EventData>(entity);
		EntityCommandBuffer val = m_EndFrameBarrier.CreateCommandBuffer();
		Entity val2 = ((EntityCommandBuffer)(ref val)).CreateEntity(componentData.m_Archetype);
		((EntityCommandBuffer)(ref val)).SetComponent<PrefabRef>(val2, new PrefabRef(entity));
		Entity selected = m_ToolSystem.selected;
		if (selected != Entity.Null)
		{
			((EntityCommandBuffer)(ref val)).SetBuffer<TargetElement>(val2).Add(new TargetElement(selected));
		}
	}

	private void BitToggle(Container container, string text, ToolBaseSystem tool, Snap bit)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0050: Expected O, but got Unknown
		ObservableList<Widget> children = container.children;
		BoolField val = new BoolField
		{
			displayName = text
		};
		((Field<bool>)val).getter = () => (tool.selectedSnap & bit) != 0;
		((Field<bool>)val).setter = delegate(bool value)
		{
			tool.selectedSnap = (value ? (tool.selectedSnap | bit) : (tool.selectedSnap & ~bit));
		};
		children.Add((Widget)val);
	}

	private void ToolSnapUI(Container container, ToolBaseSystem tool)
	{
		tool.GetAvailableSnapMask(out var onMask, out var offMask);
		Snap num = onMask & offMask;
		if ((num & Snap.ExistingGeometry) != Snap.None)
		{
			BitToggle(container, "Snap existing geometry", tool, Snap.ExistingGeometry);
		}
		if ((num & Snap.NearbyGeometry) != Snap.None)
		{
			BitToggle(container, "Snap nearby geometry", tool, Snap.NearbyGeometry);
		}
		if ((num & Snap.StraightDirection) != Snap.None)
		{
			BitToggle(container, "Snap 90 degree angles", tool, Snap.StraightDirection);
		}
		if ((num & Snap.CellLength) != Snap.None)
		{
			BitToggle(container, "Snap cell length", tool, Snap.CellLength);
		}
		if ((num & Snap.GuideLines) != Snap.None)
		{
			BitToggle(container, "Snap guide lines", tool, Snap.GuideLines);
		}
		if ((num & Snap.NetSide) != Snap.None)
		{
			BitToggle(container, "Snap road side", tool, Snap.NetSide);
		}
		if ((num & Snap.NetArea) != Snap.None)
		{
			BitToggle(container, "Snap on road", tool, Snap.NetArea);
		}
		if ((num & Snap.OwnerSide) != Snap.None)
		{
			BitToggle(container, "Snap main object", tool, Snap.OwnerSide);
		}
		if ((num & Snap.ObjectSide) != Snap.None)
		{
			BitToggle(container, "Snap building side", tool, Snap.ObjectSide);
		}
		if ((num & Snap.NetMiddle) != Snap.None)
		{
			BitToggle(container, "Snap to road", tool, Snap.NetMiddle);
		}
		if ((num & Snap.Shoreline) != Snap.None)
		{
			BitToggle(container, "Snap to shoreline", tool, Snap.Shoreline);
		}
		if ((num & Snap.NetNode) != Snap.None)
		{
			BitToggle(container, "Snap road intersection", tool, Snap.NetNode);
		}
		if ((num & Snap.ZoneGrid) != Snap.None)
		{
			BitToggle(container, "Snap zone grid", tool, Snap.ZoneGrid);
		}
		if ((num & Snap.ObjectSurface) != Snap.None)
		{
			BitToggle(container, "Snap object surface", tool, Snap.ObjectSurface);
		}
		if ((num & Snap.Upright) != Snap.None)
		{
			BitToggle(container, "Snap upright", tool, Snap.Upright);
		}
		if ((num & Snap.LotGrid) != Snap.None)
		{
			BitToggle(container, "Snap lot grid", tool, Snap.LotGrid);
		}
		if ((num & Snap.AutoParent) != Snap.None)
		{
			BitToggle(container, "Automatic parenting", tool, Snap.AutoParent);
		}
	}

	private void ToolBrushUI(Container container, ToolBaseSystem tool)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		if (tool.brushing)
		{
			ObservableList<Widget> children = container.children;
			FloatField val = new FloatField
			{
				displayName = "Brush Size"
			};
			((Field<float>)val).getter = () => tool.brushSize;
			((Field<float>)val).setter = delegate(float value)
			{
				tool.brushSize = value;
			};
			val.min = () => 10f;
			val.max = () => 1000f;
			children.Add((Widget)val);
			ObservableList<Widget> children2 = container.children;
			FloatField val2 = new FloatField
			{
				displayName = "Brush angle"
			};
			((Field<float>)val2).getter = () => tool.brushAngle;
			((Field<float>)val2).setter = delegate(float value)
			{
				tool.brushAngle = value;
			};
			val2.min = () => 0f;
			val2.max = () => 360f;
			children2.Add((Widget)val2);
			ObservableList<Widget> children3 = container.children;
			FloatField val3 = new FloatField
			{
				displayName = "Brush strength"
			};
			((Field<float>)val3).getter = () => tool.brushStrength;
			((Field<float>)val3).setter = delegate(float value)
			{
				tool.brushStrength = value;
			};
			val3.min = () => 0.01f;
			val3.max = () => 1f;
			children3.Add((Widget)val3);
		}
	}

	public void ResetLandvalue()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		LandValue landValue = default(LandValue);
		landValue.m_LandValue = 0f;
		landValue.m_Weight = 0f;
		NativeArray<Entity> val = ((EntityQuery)(ref m_LandValueQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			((EntityManager)(ref entityManager)).SetComponentData<LandValue>(val[i], landValue);
		}
		val.Dispose();
	}

	public void FullWithGarbage()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		GarbageParameterData singleton = ((EntityQuery)(ref m_GarbageParameterQuery)).GetSingleton<GarbageParameterData>();
		GarbageProducer garbageProducer = default(GarbageProducer);
		if (EntitiesExtensions.TryGetComponent<GarbageProducer>(entityManager, m_ToolSystem.selected, ref garbageProducer))
		{
			garbageProducer.m_Garbage = singleton.m_MaxGarbageAccumulation;
			((EntityManager)(ref entityManager)).SetComponentData<GarbageProducer>(m_ToolSystem.selected, garbageProducer);
		}
	}

	public void ResetPollution()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		GroundPollutionSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		AirPollutionSystem orCreateSystemManaged2 = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		JobHandle dependencies;
		CellMapData<GroundPollution> data = orCreateSystemManaged.GetData(readOnly: false, out dependencies);
		JobHandle dependencies2;
		CellMapData<AirPollution> data2 = orCreateSystemManaged2.GetData(readOnly: false, out dependencies2);
		((JobHandle)(ref dependencies)).Complete();
		for (int i = 0; i < data.m_TextureSize.x * data.m_TextureSize.y; i++)
		{
			data.m_Buffer[i] = default(GroundPollution);
		}
		((JobHandle)(ref dependencies2)).Complete();
		for (int j = 0; j < data2.m_TextureSize.x * data2.m_TextureSize.y; j++)
		{
			data2.m_Buffer[j] = default(AirPollution);
		}
	}

	public void MaxHouseholdsWealth()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_HouseholdGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		for (int i = 0; i < val.Length; i++)
		{
			DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(val[i], false);
			EconomyUtils.SetResources(Resource.Money, buffer, int.MaxValue);
			Household componentData = ((EntityManager)(ref entityManager)).GetComponentData<Household>(val[i]);
			componentData.m_Resources = int.MaxValue;
			componentData.m_ConsumptionPerDay = 0;
			((EntityManager)(ref entityManager)).SetComponentData<Household>(val[i], componentData);
		}
	}

	public void ResetHouseholdsWealth()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_HouseholdGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		for (int i = 0; i < val.Length; i++)
		{
			DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(val[i], false);
			EconomyUtils.SetResources(Resource.Money, buffer, 0);
			Household componentData = ((EntityManager)(ref entityManager)).GetComponentData<Household>(val[i]);
			componentData.m_Resources = 0;
			componentData.m_ConsumptionPerDay = 0;
			((EntityManager)(ref entityManager)).SetComponentData<Household>(val[i], componentData);
		}
	}

	public void ResetCompanyMoney()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_CompanyGroup)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
		for (int i = 0; i < val.Length; i++)
		{
			DynamicBuffer<Resources> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Resources>(val[i], false);
			EconomyUtils.SetResources(Resource.Money, buffer, 0);
		}
	}

	public void ResetRents()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0616: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_077d: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_0875: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		NativeArray<Entity> val = ((EntityQuery)(ref m_RenterQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		m_GroundPollutionSystem.GetMap(readOnly: true, out var dependencies);
		m_AirPollutionSystem.GetMap(readOnly: true, out var dependencies2);
		m_NoisePollutionSystem.GetMap(readOnly: true, out var dependencies3);
		m_TelecomCoverageSystem.GetData(readOnly: true, out var dependencies4);
		((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TaxSystem>().GetTaxRates();
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		((JobHandle)(ref dependencies3)).Complete();
		((JobHandle)(ref dependencies4)).Complete();
		float2 val2 = default(float2);
		((float2)(ref val2))._002Ector(0f, 0f);
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float2 val3 = default(float2);
		((float2)(ref val3))._002Ector(0f, 0f);
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		float num11 = 0f;
		float num12 = 0f;
		float2 val4 = default(float2);
		((float2)(ref val4))._002Ector(0f, 0f);
		float num13 = 0f;
		float num14 = 0f;
		float num15 = 0f;
		float num16 = 0f;
		EconomyParameterData economyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>();
		((EntityQuery)(ref m_DemandParameterQuery)).GetSingleton<DemandParameterData>();
		ResourcePrefabs prefabs = m_ResourceSystem.GetPrefabs();
		ComponentLookup<ResourceData> datas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		DynamicBuffer<Efficiency> buffer = default(DynamicBuffer<Efficiency>);
		DynamicBuffer<TradeCost> val6 = default(DynamicBuffer<TradeCost>);
		BuyingCompany buyingCompany = default(BuyingCompany);
		PrefabRef prefabRef = default(PrefabRef);
		BuildingPropertyData buildingPropertyData = default(BuildingPropertyData);
		SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
		for (int i = 0; i < val.Length; i++)
		{
			Entity val5 = val[i];
			EntityManager entityManager2 = ((ComponentSystemBase)this).EntityManager;
			Entity prefab = ((EntityManager)(ref entityManager2)).GetComponentData<PrefabRef>(val5).m_Prefab;
			entityManager2 = ((ComponentSystemBase)this).EntityManager;
			Entity property = ((EntityManager)(ref entityManager2)).GetComponentData<PropertyRenter>(val5).m_Property;
			entityManager2 = ((ComponentSystemBase)this).EntityManager;
			Entity prefab2 = ((EntityManager)(ref entityManager2)).GetComponentData<PrefabRef>(property).m_Prefab;
			entityManager2 = ((ComponentSystemBase)this).EntityManager;
			BuildingData componentData = ((EntityManager)(ref entityManager2)).GetComponentData<BuildingData>(prefab2);
			int num17 = componentData.m_LotSize.x * componentData.m_LotSize.y;
			entityManager2 = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager2)).HasComponent<IndustrialProcessData>(prefab))
			{
				entityManager2 = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager2)).HasComponent<WorkProvider>(val5))
				{
					entityManager2 = ((ComponentSystemBase)this).EntityManager;
					int maxWorkers = ((EntityManager)(ref entityManager2)).GetComponentData<WorkProvider>(val5).m_MaxWorkers;
					entityManager2 = ((ComponentSystemBase)this).EntityManager;
					IndustrialProcessData componentData2 = ((EntityManager)(ref entityManager2)).GetComponentData<IndustrialProcessData>(prefab);
					entityManager2 = ((ComponentSystemBase)this).EntityManager;
					WorkplaceData componentData3 = ((EntityManager)(ref entityManager2)).GetComponentData<WorkplaceData>(prefab);
					entityManager2 = ((ComponentSystemBase)this).EntityManager;
					SpawnableBuildingData componentData4 = ((EntityManager)(ref entityManager2)).GetComponentData<SpawnableBuildingData>(prefab2);
					entityManager2 = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager2)).GetBuffer<Employee>(val5, true);
					float num18 = 1f;
					if (EntitiesExtensions.TryGetBuffer<Efficiency>(((ComponentSystemBase)this).EntityManager, property, true, ref buffer))
					{
						num18 = BuildingUtils.GetEfficiency(buffer);
					}
					entityManager2 = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager2)).HasComponent<ServiceAvailable>(val5))
					{
						entityManager2 = ((ComponentSystemBase)this).EntityManager;
						ServiceAvailable componentData5 = ((EntityManager)(ref entityManager2)).GetComponentData<ServiceAvailable>(val5);
						entityManager2 = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager2)).GetComponentData<ServiceCompanyData>(prefab);
						entityManager2 = ((ComponentSystemBase)this).EntityManager;
						((EntityManager)(ref entityManager2)).GetBuffer<TradeCost>(val5, true);
						val2.x += (float)num17 * componentData5.m_MeanPriority;
						val2.y += num17;
						num3 += (float)EconomyUtils.GetCompanyProductionPerDay(num18, maxWorkers, componentData4.m_Level, isIndustrial: false, componentData3, componentData2, prefabs, ref datas, ref economyParameters);
						num4 += (float)EconomyUtils.CalculateTotalWage(maxWorkers, componentData3.m_Complexity, componentData4.m_Level, economyParameters);
						num6 += (float)num17 * num18;
					}
					else if (EconomyUtils.GetWeight(componentData2.m_Output.m_Resource, prefabs, ref datas) > 0f && EntitiesExtensions.TryGetBuffer<TradeCost>(entityManager, val5, true, ref val6) && EntitiesExtensions.TryGetComponent<BuyingCompany>(entityManager, val5, ref buyingCompany))
					{
						int companyProductionPerDay = EconomyUtils.GetCompanyProductionPerDay(num18, maxWorkers, componentData4.m_Level, isIndustrial: true, componentData3, componentData2, prefabs, ref datas, ref economyParameters);
						int num19 = EconomyUtils.CalculateTotalWage(maxWorkers, componentData3.m_Complexity, componentData4.m_Level, economyParameters);
						val3.y += num17;
						num7 += (float)companyProductionPerDay;
						num8 += (float)num19;
						num10 += (float)num17 * buyingCompany.m_MeanInputTripLength;
						num12 += (float)num17 * num18;
						float weight = EconomyUtils.GetWeight(componentData2.m_Input1.m_Resource, prefabs, ref datas);
						float weight2 = EconomyUtils.GetWeight(componentData2.m_Input2.m_Resource, prefabs, ref datas);
						num9 += (float)(num17 * (EconomyUtils.GetTransportCost(buyingCompany.m_MeanInputTripLength, componentData2.m_Input1.m_Resource, companyProductionPerDay * componentData2.m_Input1.m_Amount, weight) + EconomyUtils.GetTransportCost(buyingCompany.m_MeanInputTripLength, componentData2.m_Input2.m_Resource, companyProductionPerDay * componentData2.m_Input2.m_Amount, weight2)));
					}
					else if (EntitiesExtensions.TryGetBuffer<TradeCost>(entityManager, val5, true, ref val6) && EntitiesExtensions.TryGetComponent<BuyingCompany>(entityManager, val5, ref buyingCompany))
					{
						int companyProductionPerDay2 = EconomyUtils.GetCompanyProductionPerDay(num18, maxWorkers, componentData4.m_Level, isIndustrial: true, componentData3, componentData2, prefabs, ref datas, ref economyParameters);
						int num20 = EconomyUtils.CalculateTotalWage(maxWorkers, componentData3.m_Complexity, componentData4.m_Level, economyParameters);
						val4.y += num17;
						num13 += (float)companyProductionPerDay2;
						num14 += (float)num20;
						num16 += (float)num17 * num18;
					}
				}
			}
			Entity roadEdge = ((EntityManager)(ref entityManager)).GetComponentData<Building>(property).m_RoadEdge;
			float landValueBase = 0f;
			if (((EntityManager)(ref entityManager)).HasComponent<LandValue>(roadEdge))
			{
				landValueBase = ((EntityManager)(ref entityManager)).GetComponentData<LandValue>(roadEdge).m_LandValue;
			}
			PropertyRenter componentData6 = ((EntityManager)(ref entityManager)).GetComponentData<PropertyRenter>(val[i]);
			if (EntitiesExtensions.TryGetComponent<PrefabRef>(entityManager, componentData6.m_Property, ref prefabRef))
			{
				Entity prefab3 = prefabRef.m_Prefab;
				if (EntitiesExtensions.TryGetComponent<BuildingPropertyData>(entityManager, prefab3, ref buildingPropertyData) && EntitiesExtensions.TryGetComponent<SpawnableBuildingData>(entityManager, prefab3, ref spawnableBuildingData))
				{
					Game.Zones.AreaType areaType = ((EntityManager)(ref entityManager)).GetComponentData<ZoneData>(spawnableBuildingData.m_ZonePrefab).m_AreaType;
					int level = spawnableBuildingData.m_Level;
					int rentPricePerRenter = PropertyUtils.GetRentPricePerRenter(buildingPropertyData, level, num17, landValueBase, areaType, ref economyParameters);
					componentData6.m_Rent = rentPricePerRenter;
				}
				((EntityManager)(ref entityManager)).SetComponentData<PropertyRenter>(val[i], componentData6);
			}
		}
		val.Dispose();
		Debug.Log((object)(val2.y + " service averages:"));
		Debug.Log((object)("ServiceAvailable " + 100f * val2.x / val2.y));
		Debug.Log((object)("Profit per cell " + num / val2.y));
		Debug.Log((object)("Profit per unit " + num2 / val2.y));
		Debug.Log((object)("Productivity per cell " + num3 / val2.y));
		Debug.Log((object)("Wage per cell " + num4 / val2.y));
		Debug.Log((object)("Profitable " + 100f * num5 / val2.y));
		Debug.Log((object)("Efficiency " + 100f * num6 / val2.y));
		Debug.Log((object)(val3.y + " industrial averages:"));
		Debug.Log((object)("Profit per cell" + val3.x / val3.y));
		Debug.Log((object)("Transport cost " + num9 / val3.y));
		Debug.Log((object)("Productivity per cell " + num7 / val3.y));
		Debug.Log((object)("Wage per cell " + num8 / val3.y));
		Debug.Log((object)("Trip " + num10 / val3.y));
		Debug.Log((object)("Profitable " + 100f * num11 / val3.y));
		Debug.Log((object)("Efficiency " + 100f * num12 / val3.y));
		Debug.Log((object)(val4.y + " office averages:"));
		Debug.Log((object)("Profit per cell " + val4.x / val4.y));
		Debug.Log((object)("Productivity per cell " + num13 / val4.y));
		Debug.Log((object)("Wage per cell " + num14 / val4.y));
		Debug.Log((object)("Profitable " + 100f * num15 / val4.y));
		Debug.Log((object)("Efficiency " + 100f * num16 / val4.y));
	}

	public void AgeSelectedCitizen()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		ComponentLookup<Citizen> componentLookup = InternalCompilerInterface.GetComponentLookup<Citizen>(ref __TypeHandle.__Game_Citizens_Citizen_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Game.Creatures.Resident> componentLookup2 = InternalCompilerInterface.GetComponentLookup<Game.Creatures.Resident>(ref __TypeHandle.__Game_Creatures_Resident_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		Entity val = Entity.Null;
		if (componentLookup.HasComponent(m_ToolSystem.selected))
		{
			val = m_ToolSystem.selected;
		}
		if (componentLookup2.HasComponent(m_ToolSystem.selected))
		{
			val = componentLookup2[m_ToolSystem.selected].m_Citizen;
		}
		if (val != Entity.Null)
		{
			Citizen citizen = componentLookup[val];
			citizen.m_BirthDay -= 12;
			Debug.LogError((object)citizen.m_BirthDay);
			componentLookup[val] = citizen;
			AgingSystem existingSystemManaged = ((ComponentSystemBase)this).World.GetExistingSystemManaged<AgingSystem>();
			AgingSystem.s_DebugAgeAllCitizens = true;
			((ComponentSystemBase)existingSystemManaged).Update();
		}
	}

	public void SelectEntity(string searchTerm)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		ClearSelection();
		if (int.TryParse(searchTerm, out var result) && TryFindSelectableEntity(result, out var entity))
		{
			SelectEntity(entity);
		}
	}

	private bool TryFindSelectableEntity(int index, out Entity entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		entity = Entity.Null;
		NativeArray<Entity> val = ((EntityQuery)(ref m_SelectableQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			if (val[i].Index == index)
			{
				entity = val[i];
				break;
			}
		}
		val.Dispose();
		return entity != Entity.Null;
	}

	private void ClearSelection()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_ToolSystem.selected = Entity.Null;
	}

	private void SelectEntity(Entity entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		m_ToolSystem.selected = entity;
	}

	[DebugTab("Gameplay", -995)]
	private List<Widget> BuildGameplayDebugUI()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_0080: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		//IL_00ba: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Expected O, but got Unknown
		//IL_01bc: Expected O, but got Unknown
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Expected O, but got Unknown
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Expected O, but got Unknown
		//IL_01f6: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Expected O, but got Unknown
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Expected O, but got Unknown
		//IL_0230: Expected O, but got Unknown
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Expected O, but got Unknown
		//IL_026a: Expected O, but got Unknown
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Expected O, but got Unknown
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Expected O, but got Unknown
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Expected O, but got Unknown
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Expected O, but got Unknown
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Expected O, but got Unknown
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Expected O, but got Unknown
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Expected O, but got Unknown
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Expected O, but got Unknown
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Expected O, but got Unknown
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Expected O, but got Unknown
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Expected O, but got Unknown
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Expected O, but got Unknown
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Expected O, but got Unknown
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Expected O, but got Unknown
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Expected O, but got Unknown
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Expected O, but got Unknown
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Expected O, but got Unknown
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Expected O, but got Unknown
		//IL_058b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Expected O, but got Unknown
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Expected O, but got Unknown
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Expected O, but got Unknown
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Expected O, but got Unknown
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Expected O, but got Unknown
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a8: Expected O, but got Unknown
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Expected O, but got Unknown
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Expected O, but got Unknown
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Expected O, but got Unknown
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Expected O, but got Unknown
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Expected O, but got Unknown
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Expected O, but got Unknown
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Expected O, but got Unknown
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Expected O, but got Unknown
		//IL_0813: Unknown result type (might be due to invalid IL or missing references)
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_0823: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Expected O, but got Unknown
		TutorialSystem tutorialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TutorialSystem>();
		DynamicBuffer<ServiceFee> val = default(DynamicBuffer<ServiceFee>);
		if (EntitiesExtensions.TryGetBuffer<ServiceFee>(((ComponentSystemBase)this).World.EntityManager, m_CitySystem.City, true, ref val))
		{
			List<Widget> list = new List<Widget>();
			BoolField val2 = new BoolField
			{
				displayName = "Tutorials enabled"
			};
			((Field<bool>)val2).getter = () => tutorialSystem.tutorialEnabled;
			((Field<bool>)val2).setter = delegate(bool value)
			{
				tutorialSystem.tutorialEnabled = value;
			};
			list.Add((Widget)val2);
			BoolField val3 = new BoolField
			{
				displayName = "Freeze tutorials"
			};
			((Field<bool>)val3).getter = () => !((ComponentSystemBase)tutorialSystem).Enabled;
			((Field<bool>)val3).setter = delegate(bool value)
			{
				((ComponentSystemBase)tutorialSystem).Enabled = !value;
			};
			list.Add((Widget)val3);
			list.Add((Widget)new Button
			{
				displayName = "Skip tutorial phase",
				action = delegate
				{
					tutorialSystem.CompleteCurrentTutorialPhase();
				}
			});
			list.Add((Widget)new Button
			{
				displayName = "Show all tutorials in advisor",
				action = delegate
				{
					tutorialSystem.SetAllTutorialsShown();
				}
			});
			list.Add((Widget)new Button
			{
				displayName = "Skip active tutorial list",
				action = delegate
				{
					tutorialSystem.SkipActiveList();
				}
			});
			list.Add((Widget)new Value
			{
				displayName = "Active tutorial",
				getter = () => (!(tutorialSystem.activeTutorial != Entity.Null)) ? "None" : ((Object)m_PrefabSystem.GetPrefab<PrefabBase>(tutorialSystem.activeTutorial)).name
			});
			list.Add((Widget)new Value
			{
				displayName = "Active tutorial list",
				getter = () => (!(tutorialSystem.activeTutorialList != Entity.Null)) ? "None" : ((Object)m_PrefabSystem.GetPrefab<PrefabBase>(tutorialSystem.activeTutorialList)).name
			});
			BoolField val4 = new BoolField
			{
				displayName = "Show developer info"
			};
			((Field<bool>)val4).getter = () => m_DebugUISystem.developerInfoVisible;
			((Field<bool>)val4).setter = delegate(bool value)
			{
				m_DebugUISystem.developerInfoVisible = value;
			};
			list.Add((Widget)val4);
			BoolField val5 = new BoolField
			{
				displayName = "Show unspawned objects"
			};
			((Field<bool>)val5).getter = () => m_RenderingSystem.unspawnedVisible;
			((Field<bool>)val5).setter = delegate(bool value)
			{
				m_RenderingSystem.unspawnedVisible = value;
			};
			list.Add((Widget)val5);
			BoolField val6 = new BoolField
			{
				displayName = "Show markers"
			};
			((Field<bool>)val6).getter = () => m_RenderingSystem.markersVisible;
			((Field<bool>)val6).setter = delegate(bool value)
			{
				m_RenderingSystem.markersVisible = value;
			};
			list.Add((Widget)val6);
			BoolField val7 = new BoolField
			{
				displayName = "Lefthand traffic"
			};
			((Field<bool>)val7).getter = () => m_CityConfigurationSystem.leftHandTraffic;
			((Field<bool>)val7).setter = delegate(bool value)
			{
				m_CityConfigurationSystem.leftHandTraffic = value;
			};
			list.Add((Widget)val7);
			list.Add((Widget)new Value
			{
				displayName = "Default theme",
				getter = () => (!(m_CityConfigurationSystem.defaultTheme != Entity.Null)) ? "None" : ((Object)m_PrefabSystem.GetPrefab<PrefabBase>(m_CityConfigurationSystem.defaultTheme)).name
			});
			list.Add((Widget)new Button
			{
				displayName = "Select next theme",
				action = delegate
				{
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0027: Unknown result type (might be due to invalid IL or missing references)
					//IL_002c: Unknown result type (might be due to invalid IL or missing references)
					m_CityConfigurationSystem.defaultTheme = SelectNext(m_ThemeQuery, m_CityConfigurationSystem.defaultTheme);
				}
			});
			list.Add((Widget)new Button
			{
				displayName = "Unlock all",
				action = delegate
				{
					((ComponentSystemBase)m_UnlockAllSystem).Enabled = true;
				}
			});
			list.Add((Widget)new Button
			{
				displayName = "Get 200 XP",
				action = delegate
				{
					GetXP();
				}
			});
			list.Add((Widget)new Button
			{
				displayName = "Next MS",
				action = delegate
				{
					UnlockMilestone();
				}
			});
			list.Add((Widget)new Button
			{
				displayName = "Get 500k money",
				action = delegate
				{
					GetMoney(500000);
				}
			});
			IntField val8 = new IntField
			{
				displayName = "Hospital service fee"
			};
			((Field<int>)val8).getter = () => (int)GetFee(PlayerResource.Healthcare);
			((Field<int>)val8).setter = delegate(int value)
			{
				SetFee(PlayerResource.Healthcare, value);
			};
			val8.min = () => 0;
			val8.max = () => 1000;
			val8.incStep = 10;
			list.Add((Widget)val8);
			IntField val9 = new IntField
			{
				displayName = "Basic education service fee"
			};
			((Field<int>)val9).getter = () => (int)GetFee(PlayerResource.BasicEducation);
			((Field<int>)val9).setter = delegate(int value)
			{
				SetFee(PlayerResource.BasicEducation, value);
			};
			val9.min = () => 0;
			val9.max = () => 1000;
			val9.incStep = 10;
			list.Add((Widget)val9);
			IntField val10 = new IntField
			{
				displayName = "Secondary education service fee"
			};
			((Field<int>)val10).getter = () => (int)GetFee(PlayerResource.SecondaryEducation);
			((Field<int>)val10).setter = delegate(int value)
			{
				SetFee(PlayerResource.SecondaryEducation, value);
			};
			val10.min = () => 0;
			val10.max = () => 1000;
			val10.incStep = 10;
			list.Add((Widget)val10);
			IntField val11 = new IntField
			{
				displayName = "Higher education service fee"
			};
			((Field<int>)val11).getter = () => (int)GetFee(PlayerResource.HigherEducation);
			((Field<int>)val11).setter = delegate(int value)
			{
				SetFee(PlayerResource.HigherEducation, value);
			};
			val11.min = () => 0;
			val11.max = () => 1000;
			val11.incStep = 10;
			list.Add((Widget)val11);
			FloatField val12 = new FloatField
			{
				displayName = "Water usage fee"
			};
			((Field<float>)val12).getter = () => GetFee(PlayerResource.Water);
			((Field<float>)val12).setter = delegate(float value)
			{
				SetFee(PlayerResource.Water, value);
			};
			val12.min = () => 0f;
			val12.max = () => 5f;
			val12.incStep = 0.05f;
			list.Add((Widget)val12);
			FloatField val13 = new FloatField
			{
				displayName = "Garbage collection fee"
			};
			((Field<float>)val13).getter = () => GetFee(PlayerResource.Garbage);
			((Field<float>)val13).setter = delegate(float value)
			{
				SetFee(PlayerResource.Garbage, value);
			};
			val13.min = () => 0f;
			val13.max = () => 1f;
			val13.incStep = 0.01f;
			list.Add((Widget)val13);
			FloatField val14 = new FloatField
			{
				displayName = "Electricity fee"
			};
			((Field<float>)val14).getter = () => GetFee(PlayerResource.Electricity);
			((Field<float>)val14).setter = delegate(float value)
			{
				SetFee(PlayerResource.Electricity, value);
			};
			val14.min = () => 0.01f;
			val14.max = () => 1f;
			val14.incStep = 0.01f;
			list.Add((Widget)val14);
			IntField val15 = new IntField
			{
				displayName = "Public transport fee"
			};
			((Field<int>)val15).getter = () => (int)GetFee(PlayerResource.PublicTransport);
			((Field<int>)val15).setter = delegate(int value)
			{
				SetFee(PlayerResource.PublicTransport, value);
			};
			val15.min = () => 0;
			val15.max = () => 20;
			val15.incStep = 1;
			list.Add((Widget)val15);
			list.Add((Widget)new Button
			{
				displayName = "Display debug error",
				action = ErrorDialogManager.DisplayDebugErrorDialog
			});
			list.Add((Widget)new Button
			{
				displayName = "Show signature buildings unlock popup",
				action = ShowSignaturePopup
			});
			list.Add((Widget)new Button
			{
				displayName = "Clear signature buildings unlock popup",
				action = ClearSignaturePopup
			});
			return list;
		}
		return null;
		void ClearSignaturePopup()
		{
			m_SignatureBuildingUISystem.ClearUnlockedSignature();
		}
		float GetFee(PlayerResource resource)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ServiceFee> fees = default(DynamicBuffer<ServiceFee>);
			if (EntitiesExtensions.TryGetBuffer<ServiceFee>(((ComponentSystemBase)this).World.EntityManager, m_CitySystem.City, true, ref fees))
			{
				return ServiceFeeSystem.GetFee(resource, fees);
			}
			return 0f;
		}
		void GetMoney(int amount)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			PlayerMoney componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlayerMoney>(m_CitySystem.City);
			componentData.Add(500000);
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).SetComponentData<PlayerMoney>(m_CitySystem.City, componentData);
		}
		void GetXP()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			((ComponentSystemBase)this).World.GetOrCreateSystemManaged<XPSystem>().GetQueue(out var _).Enqueue(new XPGain
			{
				reason = XPReason.Unknown,
				amount = 200
			});
		}
		void SetFee(PlayerResource resource, float value)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			DynamicBuffer<ServiceFee> fees = default(DynamicBuffer<ServiceFee>);
			if (EntitiesExtensions.TryGetBuffer<ServiceFee>(((ComponentSystemBase)this).World.EntityManager, m_CitySystem.City, false, ref fees))
			{
				ServiceFeeSystem.SetFee(resource, fees, value);
			}
		}
		void ShowSignaturePopup()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (((EntityQuery)(ref m_SignatureBuildingQuery)).IsEmptyIgnoreFilter)
			{
				return;
			}
			Enumerator<Entity> enumerator = ((EntityQuery)(ref m_SignatureBuildingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Entity current = enumerator.Current;
					m_SignatureBuildingUISystem.AddUnlockedSignature(current);
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		void UnlockMilestone()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<XP>() });
			int xP = ((EntityQuery)(ref entityQuery)).GetSingleton<XP>().m_XP;
			EntityQuery entityQuery2 = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<MilestoneLevel>() });
			((EntityQuery)(ref entityQuery2)).GetSingleton<MilestoneLevel>();
			int amount = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MilestoneSystem>().nextRequiredXP - xP;
			((ComponentSystemBase)this).World.GetOrCreateSystemManaged<XPSystem>().GetQueue(out var _).Enqueue(new XPGain
			{
				reason = XPReason.Unknown,
				amount = amount
			});
		}
	}

	[DebugTab("GameMode", -1000)]
	private unsafe List<Widget> RefreshGameplayModeDebug()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected O, but got Unknown
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Expected O, but got Unknown
		GameModeSystem gms = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GameModeSystem>();
		PrefabSystem orCreateSystemManaged = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		List<(string name, ModeSetting mode)> modeSettings = new List<(string, ModeSetting)>();
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GameModeSettingData>() });
		NativeArray<Entity> val = ((EntityQuery)(ref entityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		Enumerator<Entity> enumerator = val.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				ModeSetting prefab = m_PrefabSystem.GetPrefab<ModeSetting>(current);
				modeSettings.Add((orCreateSystemManaged.GetPrefabName(current), prefab));
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		val.Dispose();
		List<Widget> list = new List<Widget>();
		EnumField val2 = new EnumField
		{
			displayName = "Select next mode"
		};
		((Field<int>)val2).getter = () => selectedModeIndex;
		((Field<int>)val2).setter = delegate(int num)
		{
			selectedModeIndex = num;
		};
		((EnumField<int>)val2).enumNames = ((IEnumerable<(string, ModeSetting)>)modeSettings).Select((Func<(string, ModeSetting), GUIContent>)(((string name, ModeSetting mode) x) => new GUIContent(x.name))).ToArray();
		((EnumField<int>)val2).enumValues = Enumerable.Range(0, modeSettings.Count).ToArray();
		((Field<int>)val2).onValueChanged = delegate(Field<int> w, int v)
		{
			ModeSetting item = modeSettings[v].mode;
			gms.overrideMode = ((Object)item.prefab).name;
		};
		val2.getIndex = () => selectedModeIndex;
		val2.setIndex = delegate
		{
		};
		list.Add((Widget)val2);
		List<Widget> list2 = list;
		if ((Object)(object)gms.modeSetting == (Object)null)
		{
			list2.Add((Widget)new Value
			{
				displayName = "Current mode: ",
				getter = () => "Normal"
			});
			return list2;
		}
		list2.Add((Widget)new Value
		{
			displayName = "Current mode: ",
			getter = () => ((Object)gms.modeSetting.prefab).name
		});
		EntityQuery val3 = ((EntityManager)(ref entityManager)).CreateEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ModeSettingData>() });
		Entity singletonEntity = ((EntityQuery)(ref val3)).GetSingletonEntity();
		Foldout val4 = new Foldout
		{
			displayName = $"Mode setting entity: {singletonEntity}"
		};
		ModeSettingData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ModeSettingData>(singletonEntity);
		Foldout fieldValueFoldout = GetFieldValueFoldout(componentData, null);
		((Container)val4).children.Add((Widget)(object)fieldValueFoldout);
		list2.Add((Widget)(object)val4);
		List<ModePrefab> modePrefabs = gms.modeSetting.m_ModePrefabs;
		if (modePrefabs == null)
		{
			return list2;
		}
		foreach (ModePrefab item2 in modePrefabs)
		{
			if ((Object)(object)item2 == (Object)null)
			{
				continue;
			}
			Dictionary<Entity, List<ModePrefab.ModeDebugUILogInfo>> modeDebugUILogs = item2.modeDebugUILogs;
			if (modeDebugUILogs == null)
			{
				continue;
			}
			Foldout val5 = new Foldout
			{
				displayName = $"{((Object)item2).name} : {modeDebugUILogs.Count} prefab(s) "
			};
			foreach (KeyValuePair<Entity, List<ModePrefab.ModeDebugUILogInfo>> item3 in modeDebugUILogs)
			{
				List<ModePrefab.ModeDebugUILogInfo> value = item3.Value;
				Entity key = item3.Key;
				Foldout val6 = new Foldout
				{
					displayName = orCreateSystemManaged.GetPrefabName(key) + " : " + ((object)(*(Entity*)(&key))/*cast due to .constrained prefix*/).ToString()
				};
				List<Foldout> list3 = new List<Foldout>();
				foreach (ModePrefab.ModeDebugUILogInfo item4 in value)
				{
					Foldout recordFoldout = GetRecordFoldout(item4);
					((Container)val6).children.Add((Widget)(object)recordFoldout);
				}
				list3.Add(val6);
				list3.Sort((Foldout a, Foldout b) => string.Compare(((Widget)a).displayName, ((Widget)b).displayName));
				foreach (Foldout item5 in list3)
				{
					((Container)val5).children.Add((Widget)(object)item5);
				}
			}
			list2.Add((Widget)(object)val5);
		}
		return list2;
	}

	private Foldout GetRecordFoldout(ModePrefab.ModeDebugUILogInfo record)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		Foldout val = new Foldout
		{
			displayName = $"{record}"
		};
		if (record == null || record.m_ValueBefore == null)
		{
			return val;
		}
		Type type = record.m_ValueBefore.GetType();
		if (record.m_ValueBefore is Array array)
		{
			if (record.m_ValueAfter != null && record.m_ValueAfter is Array array2 && array.Length == array2.Length)
			{
				val = GetArrayFoldout(array, array2);
			}
		}
		else
		{
			string name = type.Name;
			val = GetFieldValueFoldout(record.m_ValueBefore, record.m_ValueAfter);
			((Widget)val).displayName = name ?? "";
		}
		return val;
	}

	private Foldout GetArrayFoldout(Array record1, Array record2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		Foldout val = new Foldout
		{
			displayName = "Change Buffers Data"
		};
		for (int i = 0; i < record1.Length; i++)
		{
			object value = record1.GetValue(i);
			object value2 = record2.GetValue(i);
			Foldout fieldValueFoldout = GetFieldValueFoldout(value, value2);
			((Widget)fieldValueFoldout).displayName = "Element " + i;
			((Container)val).children.Add((Widget)(object)fieldValueFoldout);
		}
		return val;
	}

	private Foldout GetFieldValueFoldout(object object1, object object2)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		Type type = object1.GetType();
		Foldout val = new Foldout
		{
			displayName = "Value:"
		};
		if (type.IsPrimitive || type == typeof(string))
		{
			if (object1.Equals(object2))
			{
				return val;
			}
			Value primitiveUIValueObject = GetPrimitiveUIValueObject(object1, object2);
			((Widget)primitiveUIValueObject).displayName = $"{object1}";
			((Container)val).children.Add((Widget)(object)primitiveUIValueObject);
		}
		else
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				string name = fieldInfo.Name;
				object value = fieldInfo.GetValue(object1);
				object obj = null;
				if (object2 != null)
				{
					obj = fieldInfo.GetValue(object2);
					if (value.GetHashCode().Equals(obj.GetHashCode()))
					{
						continue;
					}
				}
				Foldout fieldValueFoldout = GetFieldValueFoldout(value, obj);
				((Widget)fieldValueFoldout).displayName = name ?? "";
				((Container)val).children.Add((Widget)(object)fieldValueFoldout);
			}
		}
		return val;
	}

	private Value GetPrimitiveUIValueObject(object object1, object object2)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		return new Value
		{
			displayName = $"{GetValue(object1)}",
			getter = () => GetValue(object2)
		};
		static object GetValue(object obj)
		{
			if (obj == null)
			{
				return string.Empty;
			}
			if (!obj.GetType().IsEnum)
			{
				return obj;
			}
			return Enum.GetName(obj.GetType(), obj);
		}
	}

	private void InitializeRenderingDebugUI()
	{
		if (!m_RenderingDebugUIInitialized)
		{
			CreateDebugBlitPass();
			CacheGameRenderingDebugUISystems();
		}
		m_RenderingDebugUIInitialized = true;
	}

	private void CacheGameRenderingDebugUISystems()
	{
		m_PreCullingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PreCullingSystem>();
		m_EffectControlSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectControlSystem>();
		m_ProceduralSkeletonSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralSkeletonSystem>();
		m_ProceduralEmissiveSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ProceduralEmissiveSystem>();
		m_BatchManagerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchManagerSystem>();
		m_AreaBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AreaBatchSystem>();
		m_BatchMeshSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<BatchMeshSystem>();
		m_AnimatedSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AnimatedSystem>();
		m_ManagedBatchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ManagedBatchSystem>();
		m_VegetationRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<VegetationRenderSystem>();
		m_TerrainMaterialSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainMaterialSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
	}

	private void CreateDebugBlitPass()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_DebugBlitVolume == (Object)null && m_DebugBlitPass == null)
		{
			m_DebugBlitVolume = new GameObject("DebugBlitVolume");
			Object.DontDestroyOnLoad((Object)(object)m_DebugBlitVolume);
			CustomPassVolume val = m_DebugBlitVolume.AddComponent<CustomPassVolume>();
			val.injectionPoint = (CustomPassInjectionPoint)3;
			val.priority = -100f;
			m_DebugBlitPass = (DebugCustomPass)(object)val.AddPassOfType<DebugCustomPass>();
			((CustomPass)m_DebugBlitPass).name = "DebugBlit";
		}
	}

	private string GetEntityCulling()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeList<PreCullingData> cullingData = m_PreCullingSystem.GetCullingData(readOnly: true, out dependencies);
		EntityQuery entityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CullingInfo>() });
		int num = ((EntityQuery)(ref entityQuery)).CalculateEntityCount();
		((JobHandle)(ref dependencies)).Complete();
		return $"{cullingData.Length} / {num}";
	}

	private string GetEffectCulling()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeList<EnabledEffectData> enabledData = m_EffectControlSystem.GetEnabledData(readOnly: true, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		return $"{enabledData.Length}";
	}

	private string GetBatchAllocation()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: true, out dependencies);
		((JobHandle)(ref dependencies)).Complete();
		uint num = default(uint);
		uint num2 = default(uint);
		nativeBatchInstances.GetMemoryStats(ref num, ref num2);
		return FormatUtils.FormatBytes((long)num) + " / " + FormatUtils.FormatBytes((long)num2);
	}

	private string GetBatchUpload()
	{
		uint num = default(uint);
		uint num2 = default(uint);
		m_BatchManagerSystem.GetManagedBatches().GetMemoryStats(ref num, ref num2);
		return FormatUtils.FormatBytes((long)num) + " / " + FormatUtils.FormatBytes((long)num2);
	}

	private string GetProceduralSkeletonBuffer()
	{
		m_ProceduralSkeletonSystem.GetMemoryStats(out var allocatedSize, out var bufferSize, out var _, out var _, out var allocationCount);
		return $"{FormatUtils.FormatBytes((long)allocatedSize)} / {FormatUtils.FormatBytes((long)bufferSize)} ({allocationCount})";
	}

	private string GetProceduralSkeletonUpload()
	{
		m_ProceduralSkeletonSystem.GetMemoryStats(out var _, out var _, out var currentUpload, out var uploadSize, out var _);
		return FormatUtils.FormatBytes((long)currentUpload) + " / " + FormatUtils.FormatBytes((long)uploadSize);
	}

	private string GetProceduralEmissiveBuffer()
	{
		m_ProceduralEmissiveSystem.GetMemoryStats(out var allocatedSize, out var bufferSize, out var _, out var _, out var allocationCount);
		return $"{FormatUtils.FormatBytes((long)allocatedSize)} / {FormatUtils.FormatBytes((long)bufferSize)} ({allocationCount})";
	}

	private string GetProceduralEmissiveUpload()
	{
		m_ProceduralEmissiveSystem.GetMemoryStats(out var _, out var _, out var currentUpload, out var uploadSize, out var _);
		return FormatUtils.FormatBytes((long)currentUpload) + " / " + FormatUtils.FormatBytes((long)uploadSize);
	}

	private string GetAreaBuffer()
	{
		m_AreaBatchSystem.GetAreaStats(out var allocatedSize, out var bufferSize, out var count);
		return $"{FormatUtils.FormatBytes((long)allocatedSize)} / {FormatUtils.FormatBytes((long)bufferSize)} ({count})";
	}

	private string GetShapeBuffer()
	{
		m_BatchMeshSystem.GetShapeStats(out var allocatedSize, out var bufferSize, out var count);
		return $"{FormatUtils.FormatBytes((long)allocatedSize)} / {FormatUtils.FormatBytes((long)bufferSize)} ({count})";
	}

	private string GetBoneBuffer()
	{
		m_AnimatedSystem.GetBoneStats(out var allocatedSize, out var bufferSize, out var count);
		return $"{FormatUtils.FormatBytes((long)allocatedSize)} / {FormatUtils.FormatBytes((long)bufferSize)} ({count})";
	}

	private string GetAnimBuffer()
	{
		m_AnimatedSystem.GetAnimStats(out var allocatedSize, out var bufferSize, out var count);
		return $"{FormatUtils.FormatBytes((long)allocatedSize)} / {FormatUtils.FormatBytes((long)bufferSize)} ({count})";
	}

	private string GetIndexBuffer()
	{
		m_AnimatedSystem.GetIndexStats(out var allocatedSize, out var bufferSize, out var count);
		return $"{FormatUtils.FormatBytes((long)allocatedSize)} / {FormatUtils.FormatBytes((long)bufferSize)} ({count})";
	}

	private string GetMetaBuffer()
	{
		m_AnimatedSystem.GetMetaStats(out var allocatedSize, out var bufferSize, out var count);
		return $"{FormatUtils.FormatBytes((long)allocatedSize)} / {FormatUtils.FormatBytes((long)bufferSize)} ({count})";
	}

	private string GetBatchGroups()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		JobHandle dependencies;
		NativeBatchGroups<CullingData, GroupData, BatchData, InstanceData> nativeBatchGroups = m_BatchManagerSystem.GetNativeBatchGroups(readOnly: true, out dependencies);
		JobHandle dependencies2;
		NativeBatchInstances<CullingData, GroupData, BatchData, InstanceData> nativeBatchInstances = m_BatchManagerSystem.GetNativeBatchInstances(readOnly: true, out dependencies2);
		((JobHandle)(ref dependencies)).Complete();
		((JobHandle)(ref dependencies2)).Complete();
		return $"{nativeBatchInstances.GetActiveGroupCount()} / {m_ManagedBatchSystem.groupCount} ({nativeBatchGroups.GetGroupCount()})";
	}

	private string GetBatchRenderers()
	{
		ManagedBatches<OptionalProperties> managedBatches = m_BatchManagerSystem.GetManagedBatches();
		return $"{managedBatches.RendererCount} / {m_ManagedBatchSystem.batchCount} ({managedBatches.BatchCount})";
	}

	private string GetBatchMaterials()
	{
		return m_ManagedBatchSystem.materialCount.ToString();
	}

	private string GetBatchMeshes()
	{
		long totalSizeInMemory = (long)m_BatchMeshSystem.totalSizeInMemory;
		long memoryBudget = (long)m_BatchMeshSystem.memoryBudget;
		return $"{FormatUtils.FormatBytes(totalSizeInMemory)} / {FormatUtils.FormatBytes(memoryBudget)} ({m_BatchMeshSystem.loadedMeshCount})";
	}

	[DebugTab("Game Rendering", -6)]
	private List<Widget> BuildRenderingDebugUI()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Expected O, but got Unknown
		//IL_01e5: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Expected O, but got Unknown
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a3: Expected O, but got Unknown
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Expected O, but got Unknown
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Expected O, but got Unknown
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Expected O, but got Unknown
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Expected O, but got Unknown
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0831: Unknown result type (might be due to invalid IL or missing references)
		//IL_0836: Unknown result type (might be due to invalid IL or missing references)
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Expected O, but got Unknown
		//IL_0859: Unknown result type (might be due to invalid IL or missing references)
		//IL_085e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Expected O, but got Unknown
		//IL_0881: Unknown result type (might be due to invalid IL or missing references)
		//IL_0886: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Expected O, but got Unknown
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Expected O, but got Unknown
		//IL_08d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Expected O, but got Unknown
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0909: Unknown result type (might be due to invalid IL or missing references)
		//IL_0920: Expected O, but got Unknown
		//IL_0921: Unknown result type (might be due to invalid IL or missing references)
		//IL_0926: Unknown result type (might be due to invalid IL or missing references)
		//IL_0931: Unknown result type (might be due to invalid IL or missing references)
		//IL_0948: Expected O, but got Unknown
		//IL_0949: Unknown result type (might be due to invalid IL or missing references)
		//IL_094e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0959: Unknown result type (might be due to invalid IL or missing references)
		//IL_0970: Expected O, but got Unknown
		//IL_0971: Unknown result type (might be due to invalid IL or missing references)
		//IL_0976: Unknown result type (might be due to invalid IL or missing references)
		//IL_0981: Unknown result type (might be due to invalid IL or missing references)
		//IL_0998: Expected O, but got Unknown
		//IL_0999: Unknown result type (might be due to invalid IL or missing references)
		//IL_099e: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Expected O, but got Unknown
		//IL_09c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e8: Expected O, but got Unknown
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a10: Expected O, but got Unknown
		//IL_0a11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a38: Expected O, but got Unknown
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a60: Expected O, but got Unknown
		//IL_0a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a88: Expected O, but got Unknown
		//IL_0a89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab0: Expected O, but got Unknown
		//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad8: Expected O, but got Unknown
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b00: Expected O, but got Unknown
		//IL_0b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b11: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b23: Expected O, but got Unknown
		//IL_0b23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b35: Expected O, but got Unknown
		//IL_0b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Expected O, but got Unknown
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Expected O, but got Unknown
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Expected O, but got Unknown
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Expected O, but got Unknown
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Expected O, but got Unknown
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Expected O, but got Unknown
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Expected O, but got Unknown
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Expected O, but got Unknown
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Expected O, but got Unknown
		//IL_082a: Expected O, but got Unknown
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Expected O, but got Unknown
		//IL_0b7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8f: Expected O, but got Unknown
		//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb2: Expected O, but got Unknown
		//IL_0bb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc4: Expected O, but got Unknown
		//IL_0bc9: Expected O, but got Unknown
		//IL_0bca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Expected O, but got Unknown
		//IL_0bec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfe: Expected O, but got Unknown
		//IL_0c03: Expected O, but got Unknown
		//IL_0c04: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c14: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c26: Expected O, but got Unknown
		//IL_0c26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c38: Expected O, but got Unknown
		//IL_0c3d: Expected O, but got Unknown
		//IL_0c3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c60: Expected O, but got Unknown
		//IL_0c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c72: Expected O, but got Unknown
		//IL_0c77: Expected O, but got Unknown
		//IL_0c78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9a: Expected O, but got Unknown
		//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cac: Expected O, but got Unknown
		//IL_0cb1: Expected O, but got Unknown
		//IL_0cd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf7: Expected O, but got Unknown
		//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d09: Expected O, but got Unknown
		//IL_0d0e: Expected O, but got Unknown
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Expected O, but got Unknown
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Expected O, but got Unknown
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Expected O, but got Unknown
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Expected O, but got Unknown
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Expected O, but got Unknown
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Expected O, but got Unknown
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Expected O, but got Unknown
		//IL_0694: Expected O, but got Unknown
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Expected O, but got Unknown
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Expected O, but got Unknown
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Expected O, but got Unknown
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Expected O, but got Unknown
		//IL_06e9: Expected O, but got Unknown
		//IL_06ee: Expected O, but got Unknown
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Expected O, but got Unknown
		//IL_0d54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d59: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7e: Expected O, but got Unknown
		//IL_0d7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d91: Expected O, but got Unknown
		//IL_0d96: Expected O, but got Unknown
		//IL_0dbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc5: Expected O, but got Unknown
		//IL_0dd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df3: Expected O, but got Unknown
		//IL_0df3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e05: Expected O, but got Unknown
		//IL_0e05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e17: Expected O, but got Unknown
		//IL_0e1c: Expected O, but got Unknown
		//IL_0ff7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ffc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e48: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5a: Expected O, but got Unknown
		//IL_0e5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e6c: Expected O, but got Unknown
		//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e7e: Expected O, but got Unknown
		//IL_0e83: Expected O, but got Unknown
		//IL_0e83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_102c: Expected O, but got Unknown
		//IL_102c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1051: Expected O, but got Unknown
		//IL_1051: Unknown result type (might be due to invalid IL or missing references)
		//IL_1063: Expected O, but got Unknown
		//IL_1068: Expected O, but got Unknown
		//IL_0ead: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebf: Expected O, but got Unknown
		//IL_0ebf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed1: Expected O, but got Unknown
		//IL_0ed1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1358: Unknown result type (might be due to invalid IL or missing references)
		//IL_135d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1368: Unknown result type (might be due to invalid IL or missing references)
		//IL_1073: Unknown result type (might be due to invalid IL or missing references)
		//IL_1078: Unknown result type (might be due to invalid IL or missing references)
		//IL_1083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1392: Expected O, but got Unknown
		//IL_10a8: Expected O, but got Unknown
		//IL_10a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2b: Expected O, but got Unknown
		//IL_0f2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f36: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f41: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f53: Expected O, but got Unknown
		//IL_0f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f65: Expected O, but got Unknown
		//IL_0f65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f75: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cd: Expected O, but got Unknown
		//IL_10cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10df: Expected O, but got Unknown
		//IL_10e4: Expected O, but got Unknown
		//IL_10e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1107: Expected O, but got Unknown
		//IL_1107: Unknown result type (might be due to invalid IL or missing references)
		//IL_1119: Expected O, but got Unknown
		//IL_1119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_113e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc4: Expected O, but got Unknown
		//IL_0fc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff1: Expected O, but got Unknown
		//IL_0ff6: Expected O, but got Unknown
		//IL_1163: Unknown result type (might be due to invalid IL or missing references)
		//IL_116e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1176: Unknown result type (might be due to invalid IL or missing references)
		//IL_1188: Expected O, but got Unknown
		//IL_118d: Expected O, but got Unknown
		//IL_118e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1193: Unknown result type (might be due to invalid IL or missing references)
		//IL_119e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c3: Expected O, but got Unknown
		//IL_11c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e8: Expected O, but got Unknown
		//IL_11ed: Expected O, but got Unknown
		//IL_11ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1223: Expected O, but got Unknown
		//IL_1223: Unknown result type (might be due to invalid IL or missing references)
		//IL_1248: Expected O, but got Unknown
		//IL_1248: Unknown result type (might be due to invalid IL or missing references)
		//IL_126d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1292: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a2: Expected O, but got Unknown
		//IL_12a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d8: Expected O, but got Unknown
		//IL_12d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fd: Expected O, but got Unknown
		//IL_12fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1322: Unknown result type (might be due to invalid IL or missing references)
		//IL_1347: Unknown result type (might be due to invalid IL or missing references)
		//IL_1357: Expected O, but got Unknown
		InitializeRenderingDebugUI();
		Foldout val = new Foldout
		{
			displayName = "Custom passes"
		};
		CustomPassVolume[] array = Object.FindObjectsOfType<CustomPassVolume>();
		for (int i = 0; i < array.Length; i++)
		{
			foreach (CustomPass cp in array[i].customPasses)
			{
				ObservableList<Widget> children = ((Container)val).children;
				BoolField val2 = new BoolField
				{
					displayName = cp.name
				};
				((Field<bool>)val2).getter = () => cp.enabled;
				((Field<bool>)val2).setter = delegate(bool value)
				{
					cp.enabled = value;
				};
				children.Add((Widget)val2);
			}
		}
		TerrainSurface terrainSurface = TerrainSurface.GetValidSurface();
		Foldout val3 = new Foldout
		{
			displayName = "Terrain"
		};
		ObservableList<Widget> children2 = ((Container)val3).children;
		IntField val4 = new IntField
		{
			displayName = "CBT Max Depth"
		};
		((Field<int>)val4).getter = () => (!((Object)(object)terrainSurface != (Object)null)) ? 25 : terrainSurface.MaxTreeDepth;
		((Field<int>)val4).setter = delegate(int value)
		{
			if ((Object)(object)terrainSurface != (Object)null)
			{
				terrainSurface.MaxTreeDepth = value;
			}
		};
		val4.min = () => 8;
		val4.max = () => 28;
		val4.incStep = 1;
		children2.Add((Widget)val4);
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Tri count",
			getter = () => ((Object)(object)terrainSurface != (Object)null) ? terrainSurface.GetCameraTriangleCount(Camera.main) : (-1)
		});
		ObservableList<Widget> children3 = ((Container)val3).children;
		BoolField val5 = new BoolField
		{
			displayName = "Foliage"
		};
		((Field<bool>)val5).getter = () => ((ComponentSystemBase)m_VegetationRenderSystem).Enabled;
		((Field<bool>)val5).setter = delegate(bool value)
		{
			((ComponentSystemBase)m_VegetationRenderSystem).Enabled = value;
		};
		children3.Add((Widget)val5);
		((Container)val3).children.Add((Widget)new Button
		{
			displayName = "Refresh splatmap",
			action = delegate
			{
				m_TerrainMaterialSystem.ForceUpdateWholeSplatmap();
			}
		});
		List<Widget> list = new List<Widget>();
		EnumField val6 = new EnumField
		{
			displayName = "Texture Debug Mode"
		};
		((Field<int>)val6).getter = () => (int)m_DebugBlitPass.textureDebugMode;
		((Field<int>)val6).setter = delegate(int value)
		{
			m_DebugBlitPass.textureDebugMode = (DebugCustomPass.TextureDebugMode)value;
		};
		val6.autoEnum = typeof(DebugCustomPass.TextureDebugMode);
		((Field<int>)val6).onValueChanged = RebuildRenderingDebugUI<int>;
		val6.getIndex = () => (int)m_DebugBlitPass.textureDebugMode;
		val6.setIndex = delegate(int value)
		{
			m_DebugBlitPass.textureDebugMode = (DebugCustomPass.TextureDebugMode)value;
		};
		list.Add((Widget)val6);
		List<Widget> list2 = list;
		if (m_DebugBlitPass.textureDebugMode != DebugCustomPass.TextureDebugMode.None)
		{
			m_DebugBlitPass.sliceIndex = 0;
			Container val7 = new Container();
			m_DebugBlitPass.minValue = m_DebugBlitPass.GetDefaultMinValue();
			m_DebugBlitPass.maxValue = m_DebugBlitPass.GetDefaultMaxValue();
			m_DebugBlitPass.showExtra = m_DebugBlitPass.HasExtra();
			if (m_DebugBlitPass.SetupTexture(out var tex, out var sliceCount))
			{
				val7.children.Add((Widget)new Value
				{
					displayName = "Format",
					getter = () => (object)(GraphicsFormat)(((Object)(object)tex != (Object)null) ? ((int)tex.graphicsFormat) : 0)
				});
				ObservableList<Widget> children4 = val7.children;
				FloatField val8 = new FloatField
				{
					displayName = "Overlay Ratio"
				};
				((Field<float>)val8).getter = () => m_DebugBlitPass.debugOverlayRatio;
				((Field<float>)val8).setter = delegate(float value)
				{
					m_DebugBlitPass.debugOverlayRatio = value;
				};
				val8.min = () => 0f;
				val8.max = () => 1f;
				val8.incStep = 0.001f;
				children4.Add((Widget)val8);
				if (sliceCount > 0)
				{
					ObservableList<Widget> children5 = val7.children;
					IntField val9 = new IntField
					{
						displayName = "Slice"
					};
					((Field<int>)val9).getter = () => m_DebugBlitPass.sliceIndex;
					((Field<int>)val9).setter = delegate(int value)
					{
						m_DebugBlitPass.sliceIndex = value;
					};
					val9.min = () => 0;
					val9.max = () => sliceCount;
					val9.incStep = 1;
					children5.Add((Widget)val9);
				}
			}
			if (m_DebugBlitPass.textureDebugMode == DebugCustomPass.TextureDebugMode.TerrainTesselation)
			{
				val7.children.Add((Widget)new Value
				{
					displayName = "Terrain triangle count",
					getter = () => ((Object)(object)terrainSurface != (Object)null) ? terrainSurface.GetCameraTriangleCount(Camera.main) : (-1)
				});
			}
			list2.Add((Widget)(object)val7);
		}
		if (m_DebugBlitPass.textureDebugMode >= DebugCustomPass.TextureDebugMode.HeightMap && m_DebugBlitPass.textureDebugMode <= DebugCustomPass.TextureDebugMode.Wind)
		{
			m_DebugBlitPass.minValue = m_DebugBlitPass.GetDefaultMinValue();
			m_DebugBlitPass.maxValue = m_DebugBlitPass.GetDefaultMaxValue();
			m_DebugBlitPass.showExtra = m_DebugBlitPass.HasExtra();
			Container val10 = new Container();
			ObservableList<Widget> children6 = val10.children;
			FloatField val11 = new FloatField
			{
				displayName = "Zoom Level"
			};
			((Field<float>)val11).getter = () => m_DebugBlitPass.zoom;
			((Field<float>)val11).setter = delegate(float value)
			{
				m_DebugBlitPass.zoom = value;
			};
			val11.min = () => 0f;
			val11.max = () => 1f;
			val11.incStep = 0.001f;
			children6.Add((Widget)val11);
			ObservableList<Widget> children7 = val10.children;
			FloatField val12 = new FloatField
			{
				displayName = "Min Range"
			};
			((Field<float>)val12).getter = () => m_DebugBlitPass.minValue;
			((Field<float>)val12).setter = delegate(float value)
			{
				m_DebugBlitPass.minValue = value;
			};
			val12.min = () => m_DebugBlitPass.GetMinValue();
			val12.max = () => m_DebugBlitPass.GetMaxValue();
			val12.incStep = 0.001f;
			children7.Add((Widget)val12);
			ObservableList<Widget> children8 = val10.children;
			FloatField val13 = new FloatField
			{
				displayName = "Max Range"
			};
			((Field<float>)val13).getter = () => m_DebugBlitPass.maxValue;
			((Field<float>)val13).setter = delegate(float value)
			{
				m_DebugBlitPass.maxValue = value;
			};
			val13.min = () => m_DebugBlitPass.GetMinValue();
			val13.max = () => m_DebugBlitPass.GetMaxValue();
			val13.incStep = 0.001f;
			children8.Add((Widget)val13);
			list2.Add((Widget)val10);
			if (m_DebugBlitPass.HasExtra())
			{
				Container val14 = new Container();
				ObservableList<Widget> children9 = val14.children;
				BoolField val15 = new BoolField
				{
					displayName = "Show Extra"
				};
				((Field<bool>)val15).getter = () => m_DebugBlitPass.showExtra;
				((Field<bool>)val15).setter = delegate(bool value)
				{
					m_DebugBlitPass.showExtra = value;
				};
				children9.Add((Widget)val15);
				list2.Add((Widget)val14);
			}
		}
		else if (m_DebugBlitPass.textureDebugMode >= DebugCustomPass.TextureDebugMode.WaterSurfaceSpectrum && m_DebugBlitPass.textureDebugMode <= DebugCustomPass.TextureDebugMode.WaterSurfaceCaustics)
		{
			m_DebugBlitPass.minValue = m_DebugBlitPass.GetDefaultMinValue();
			m_DebugBlitPass.maxValue = m_DebugBlitPass.GetDefaultMaxValue();
			Container val16 = new Container();
			ObservableList<Widget> children10 = val16.children;
			FloatField val17 = new FloatField
			{
				displayName = "Min Range"
			};
			((Field<float>)val17).getter = () => m_DebugBlitPass.minValue;
			((Field<float>)val17).setter = delegate(float value)
			{
				m_DebugBlitPass.minValue = value;
			};
			val17.min = () => m_DebugBlitPass.GetMinValue();
			val17.max = () => m_DebugBlitPass.GetMaxValue();
			val17.incStep = 0.001f;
			children10.Add((Widget)val17);
			ObservableList<Widget> children11 = val16.children;
			FloatField val18 = new FloatField
			{
				displayName = "Max Range"
			};
			((Field<float>)val18).getter = () => m_DebugBlitPass.maxValue;
			((Field<float>)val18).setter = delegate(float value)
			{
				m_DebugBlitPass.maxValue = value;
			};
			val18.min = () => m_DebugBlitPass.GetMinValue();
			val18.max = () => m_DebugBlitPass.GetMaxValue();
			val18.incStep = 0.001f;
			children11.Add((Widget)val18);
			list2.Add((Widget)val16);
		}
		List<Widget> obj = new List<Widget>
		{
			(Widget)new Value
			{
				displayName = "Entity culling",
				getter = GetEntityCulling
			},
			(Widget)new Value
			{
				displayName = "Effect culling",
				getter = GetEffectCulling
			},
			(Widget)new Value
			{
				displayName = "Batch allocation",
				getter = GetBatchAllocation
			},
			(Widget)new Value
			{
				displayName = "Batch upload",
				getter = GetBatchUpload
			},
			(Widget)new Value
			{
				displayName = "Batch groups",
				getter = GetBatchGroups
			},
			(Widget)new Value
			{
				displayName = "Batch renderers",
				getter = GetBatchRenderers
			},
			(Widget)new Value
			{
				displayName = "Batch materials",
				getter = GetBatchMaterials
			},
			(Widget)new Value
			{
				displayName = "Batch meshes",
				getter = GetBatchMeshes
			},
			(Widget)new Value
			{
				displayName = "Area triangle buffer",
				getter = GetAreaBuffer
			},
			(Widget)new Value
			{
				displayName = "Procedural skeleton buffer",
				getter = GetProceduralSkeletonBuffer
			},
			(Widget)new Value
			{
				displayName = "Procedural skeleton upload",
				getter = GetProceduralSkeletonUpload
			},
			(Widget)new Value
			{
				displayName = "Procedural emissive buffer",
				getter = GetProceduralEmissiveBuffer
			},
			(Widget)new Value
			{
				displayName = "Procedural emissive upload",
				getter = GetProceduralEmissiveUpload
			},
			(Widget)new Value
			{
				displayName = "Animation shape buffer",
				getter = GetShapeBuffer
			},
			(Widget)new Value
			{
				displayName = "Animation bone buffer",
				getter = GetBoneBuffer
			},
			(Widget)new Value
			{
				displayName = "Animation frame buffer",
				getter = GetAnimBuffer
			},
			(Widget)new Value
			{
				displayName = "Animation index buffer",
				getter = GetIndexBuffer
			},
			(Widget)new Value
			{
				displayName = "Animation meta buffer",
				getter = GetMetaBuffer
			}
		};
		FloatField val19 = new FloatField
		{
			displayName = "Level of detail"
		};
		((Field<float>)val19).getter = () => m_RenderingSystem.levelOfDetail;
		((Field<float>)val19).setter = delegate(float value)
		{
			if (m_RenderingSystem.levelOfDetail != value)
			{
				m_RenderingSystem.levelOfDetail = value;
			}
		};
		val19.min = () => 0.01f;
		val19.max = () => 1f;
		val19.incStep = 0.01f;
		obj.Add((Widget)val19);
		BoolField val20 = new BoolField
		{
			displayName = "Disable lod models"
		};
		((Field<bool>)val20).getter = () => m_RenderingSystem.disableLodModels;
		((Field<bool>)val20).setter = delegate(bool value)
		{
			m_RenderingSystem.disableLodModels = value;
		};
		obj.Add((Widget)val20);
		BoolField val21 = new BoolField
		{
			displayName = "Disable mesh loading"
		};
		((Field<bool>)val21).getter = () => !m_BatchMeshSystem.enableMeshLoading;
		((Field<bool>)val21).setter = delegate(bool value)
		{
			m_BatchMeshSystem.enableMeshLoading = !value;
		};
		obj.Add((Widget)val21);
		BoolField val22 = new BoolField
		{
			displayName = "Force mesh unloading"
		};
		((Field<bool>)val22).getter = () => m_BatchMeshSystem.forceMeshUnloading;
		((Field<bool>)val22).setter = delegate(bool value)
		{
			m_BatchMeshSystem.forceMeshUnloading = value;
		};
		obj.Add((Widget)val22);
		BoolField val23 = new BoolField
		{
			displayName = "Strict mesh memory budget"
		};
		((Field<bool>)val23).getter = () => m_BatchMeshSystem.strictMemoryBudget;
		((Field<bool>)val23).setter = delegate(bool value)
		{
			m_BatchMeshSystem.strictMemoryBudget = value;
		};
		obj.Add((Widget)val23);
		BoolField val24 = new BoolField
		{
			displayName = "Long cross fade"
		};
		((Field<bool>)val24).getter = () => m_RenderingSystem.debugCrossFade;
		((Field<bool>)val24).setter = delegate(bool value)
		{
			m_RenderingSystem.debugCrossFade = value;
		};
		obj.Add((Widget)val24);
		obj.Add((Widget)(object)val);
		obj.Add((Widget)(object)val3);
		list2.AddRange(obj);
		ObservableList<Widget> shaders = new ObservableList<Widget>();
		ObservableList<Widget> obj2 = shaders;
		BoolField val25 = new BoolField
		{
			displayName = "All"
		};
		((Field<bool>)val25).getter = delegate
		{
			if (m_RenderingSystem.enabledShaders.Count + 1 != shaders.Count)
			{
				Rebuild(BuildRenderingDebugUI);
			}
			foreach (KeyValuePair<Shader, bool> enabledShader in m_RenderingSystem.enabledShaders)
			{
				if (!enabledShader.Value)
				{
					return false;
				}
			}
			return true;
		};
		((Field<bool>)val25).setter = delegate(bool value)
		{
			foreach (KeyValuePair<Shader, bool> item in new List<KeyValuePair<Shader, bool>>(m_RenderingSystem.enabledShaders))
			{
				if (item.Value != value)
				{
					m_RenderingSystem.SetShaderEnabled(item.Key, value);
				}
			}
		};
		obj2.Add((Widget)val25);
		foreach (KeyValuePair<Shader, bool> enabledShader2 in m_RenderingSystem.enabledShaders)
		{
			Shader shader = enabledShader2.Key;
			ObservableList<Widget> obj3 = shaders;
			BoolField val26 = new BoolField
			{
				displayName = ((Object)shader).name
			};
			((Field<bool>)val26).getter = () => m_RenderingSystem.IsShaderEnabled(shader);
			((Field<bool>)val26).setter = delegate(bool value)
			{
				m_RenderingSystem.SetShaderEnabled(shader, value);
			};
			obj3.Add((Widget)val26);
		}
		list2.Add((Widget)new Foldout("Shaders", shaders, (string[])null, (string[])null));
		AdaptiveDynamicResolutionScale adrs = AdaptiveDynamicResolutionScale.instance;
		BoolField val27 = new BoolField
		{
			displayName = "Dynamic Resolution"
		};
		((Field<bool>)val27).getter = () => adrs.isEnabled;
		((Field<bool>)val27).setter = delegate(bool value)
		{
			adrs.SetParams(value, adrs.isAdaptive, adrs.minScale, adrs.upscaleFilter, Camera.main);
		};
		((Field<bool>)val27).onValueChanged = RebuildRenderingDebugUI<bool>;
		list2.Add((Widget)val27);
		if (adrs.isEnabled)
		{
			Container val28 = new Container();
			ObservableList<Widget> children12 = val28.children;
			BoolField val29 = new BoolField
			{
				displayName = "Auto Adaptive"
			};
			((Field<bool>)val29).getter = () => adrs.isAdaptive;
			((Field<bool>)val29).setter = delegate(bool value)
			{
				adrs.SetParams(adrs.isEnabled, value, adrs.minScale, adrs.upscaleFilter, Camera.main);
			};
			((Field<bool>)val29).onValueChanged = RebuildRenderingDebugUI<bool>;
			children12.Add((Widget)val29);
			ObservableList<Widget> children13 = val28.children;
			FloatField val30 = new FloatField
			{
				displayName = (adrs.isAdaptive ? "Min Scale" : "Scale")
			};
			((Field<float>)val30).getter = () => adrs.minScale;
			((Field<float>)val30).setter = delegate(float value)
			{
				adrs.SetParams(adrs.isEnabled, adrs.isAdaptive, value, adrs.upscaleFilter, Camera.main);
			};
			val30.min = () => 0.5f;
			val30.max = () => 1f;
			val30.incStep = 0.01f;
			children13.Add((Widget)val30);
			ObservableList<Widget> children14 = val28.children;
			EnumField val31 = new EnumField
			{
				displayName = "Upscale Filter"
			};
			((Field<int>)val31).getter = () => (int)adrs.upscaleFilter;
			((Field<int>)val31).setter = delegate(int value)
			{
				adrs.SetParams(adrs.isEnabled, adrs.isAdaptive, adrs.minScale, (AdaptiveDynamicResolutionScale.DynResUpscaleFilter)value, Camera.main);
			};
			val31.autoEnum = typeof(AdaptiveDynamicResolutionScale.DynResUpscaleFilter);
			val31.getIndex = () => (int)AdaptiveDynamicResolutionScale.instance.upscaleFilter;
			val31.setIndex = delegate
			{
			};
			children14.Add((Widget)val31);
			val28.children.Add((Widget)new Value
			{
				displayName = "Debug",
				getter = () => adrs.debugState
			});
			list2.Add((Widget)val28);
		}
		BoolField val32 = new BoolField
		{
			displayName = "Punctual Lights"
		};
		((Field<bool>)val32).getter = () => HDRPDotsInputs.PunctualLightsEnable;
		((Field<bool>)val32).setter = delegate(bool value)
		{
			HDRPDotsInputs.PunctualLightsEnable = value;
		};
		((Field<bool>)val32).onValueChanged = RebuildRenderingDebugUI<bool>;
		list2.Add((Widget)val32);
		if (HDRPDotsInputs.PunctualLightsEnable)
		{
			BoolField val33 = new BoolField
			{
				displayName = "Punctual Lights Cookies"
			};
			((Field<bool>)val33).getter = () => HDRPDotsInputs.PunctualLightsCookies;
			((Field<bool>)val33).setter = delegate(bool value)
			{
				HDRPDotsInputs.PunctualLightsCookies = value;
			};
			((Field<bool>)val33).onValueChanged = RebuildRenderingDebugUI<bool>;
			list2.Add((Widget)val33);
			IntField val34 = new IntField
			{
				displayName = "Max Punctual Lights"
			};
			((Field<int>)val34).getter = () => m_RenderingSystem.maxLightCount;
			((Field<int>)val34).setter = delegate(int value)
			{
				m_RenderingSystem.maxLightCount = value;
			};
			val34.min = () => 512;
			val34.max = () => 16384;
			val34.incStep = 256;
			val34.intStepMult = 16;
			((Field<int>)val34).onValueChanged = RebuildRenderingDebugUI<int>;
			list2.Add((Widget)val34);
			BoolField val35 = new BoolField
			{
				displayName = "Enable Min-Max light culling optim"
			};
			((Field<bool>)val35).getter = () => LightCullingSystem.s_enableMinMaxLightCullingOptim;
			((Field<bool>)val35).setter = delegate(bool value)
			{
				LightCullingSystem.s_enableMinMaxLightCullingOptim = value;
			};
			list2.Add((Widget)val35);
			FloatField val36 = new FloatField
			{
				displayName = "Max Distance Culling Scale"
			};
			((Field<float>)val36).getter = () => LightCullingSystem.s_maxLightDistanceScale;
			((Field<float>)val36).setter = delegate(float value)
			{
				LightCullingSystem.s_maxLightDistanceScale = value;
			};
			val36.min = () => 1f;
			val36.max = () => 3f;
			val36.incStep = 0.1f;
			list2.Add((Widget)val36);
			FloatField val37 = new FloatField
			{
				displayName = "Min Distance Culling Scale"
			};
			((Field<float>)val37).getter = () => LightCullingSystem.s_minLightDistanceScale;
			((Field<float>)val37).setter = delegate(float value)
			{
				LightCullingSystem.s_minLightDistanceScale = value;
			};
			val37.min = () => 0.1f;
			val37.max = () => 0.9f;
			val37.incStep = 0.1f;
			list2.Add((Widget)val37);
		}
		list2.Add((Widget)new Value
		{
			displayName = "Number of punctual lights",
			getter = () => HDRPDotsInputs.NumPunctualLights
		});
		return list2;
		void RebuildRenderingDebugUI<TValue>(Field<TValue> field, TValue value)
		{
			Rebuild(BuildRenderingDebugUI);
		}
	}

	private static void AddSystemGizmoField<T>(ref Container container, World world, params Widget[] additionalIfEnabled) where T : ComponentSystemBase
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Expected O, but got Unknown
		T system = world.GetOrCreateSystemManaged<T>();
		ObservableList<Widget> children = container.children;
		EnumField val = new EnumField
		{
			displayName = StringUtils.Nicify(typeof(T).Name)
		};
		((Field<int>)val).getter = () => ((ComponentSystemBase)system).Enabled ? 1 : 0;
		((Field<int>)val).setter = delegate(int value)
		{
			((ComponentSystemBase)system).Enabled = value != 0;
		};
		val.autoEnum = typeof(ToggleEnum);
		((Field<int>)val).onValueChanged = RebuildGizmosDebugUI<int>;
		val.getIndex = () => ((ComponentSystemBase)system).Enabled ? 1 : 0;
		val.setIndex = delegate
		{
		};
		children.Add((Widget)val);
		if (!(system is BaseDebugSystem baseDebugSystem))
		{
			return;
		}
		if (((ComponentSystemBase)system).Enabled)
		{
			List<BaseDebugSystem.Option> options = baseDebugSystem.options;
			if (options.Count != 0)
			{
				Container val2 = new Container();
				for (int num = 0; num < options.Count; num++)
				{
					BaseDebugSystem.Option option = options[num];
					EnumField val3 = new EnumField
					{
						displayName = option.name
					};
					((Field<int>)val3).getter = () => option.enabled ? 1 : 0;
					((Field<int>)val3).setter = delegate(int value)
					{
						option.enabled = value != 0;
					};
					val3.autoEnum = typeof(ToggleEnum);
					val3.getIndex = () => option.enabled ? 1 : 0;
					val3.setIndex = delegate
					{
					};
					EnumField val4 = val3;
					val2.children.Add((Widget)(object)val4);
				}
				for (int num2 = 0; num2 < additionalIfEnabled.Length; num2++)
				{
					val2.children.Add(additionalIfEnabled[num2]);
				}
				container.children.Add((Widget)(object)val2);
			}
			baseDebugSystem.OnEnabled(container);
		}
		else
		{
			baseDebugSystem.OnDisabled(container);
		}
		static void RebuildGizmosDebugUI<TValue>(Field<TValue> field, TValue value)
		{
			Rebuild(BuildGizmosDebugUI);
		}
	}

	[DebugTab("Gizmos", -990)]
	private static List<Widget> BuildGizmosDebugUI(World world)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		Value val = new Value
		{
			displayName = "Buildable Area",
			getter = () => (int)math.round(100f * world.GetOrCreateSystemManaged<BuildableAreaDebugSystem>().buildableArea)
		};
		Container container = new Container();
		AddSystemGizmoField<ObjectDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<NetDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<LaneDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<LightDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<WaterCullingDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<ZoneDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<AreaDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<CollapseSFXDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<ZoneAmbienceValueDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<RouteDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<NavigationDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<AvailabilityDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<DensityDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<CoverageDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<PathDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<PathfindDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<SearchTreeDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<TerrainAttractivenessDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<LandValueDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<EconomyDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<PollutionDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<GroundWaterDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<SoilWaterDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<GarbageDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<WaterDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<WindDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<EventDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<PropertyDebugSystem>(ref container, world, Array.Empty<Widget>());
		AddSystemGizmoField<BuildableAreaDebugSystem>(ref container, world, (Widget[])(object)new Widget[1] { (Widget)val });
		return new List<Widget> { (Widget)(object)container };
	}

	private void RefreshNotifications()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<Entity, int> dictionary = new Dictionary<Entity, int>();
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_IconQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentTypeHandle<PrefabRef> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref componentTypeHandle);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Entity prefab = nativeArray[j].m_Prefab;
				if (dictionary.TryGetValue(prefab, out var value))
				{
					dictionary[prefab] = value + 1;
				}
				else
				{
					dictionary.Add(prefab, 1);
				}
			}
		}
		val.Dispose();
		if (m_Notifications == null)
		{
			m_Notifications = new List<NotificationInfo>(dictionary.Count);
		}
		else
		{
			m_Notifications.Clear();
		}
		foreach (KeyValuePair<Entity, int> item in dictionary)
		{
			m_Notifications.Add(new NotificationInfo
			{
				m_Prefab = item.Key,
				m_Instances = item.Value
			});
		}
		m_Notifications.Sort();
	}

	private void EnableNotification(Entity entity, bool enabled)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentEnabled<NotificationIconDisplayData>(entity, enabled);
		((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IconClusterSystem>().RecalculateClusters();
	}

	[DebugTab("Notifications", -12)]
	private List<Widget> BuildNotificationsDebugUI()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		List<Widget> list = new List<Widget> { (Widget)new Button
		{
			displayName = "Refresh",
			action = delegate
			{
				RefreshNotifications();
				Rebuild(BuildNotificationsDebugUI);
			}
		} };
		if (m_Notifications != null)
		{
			Container val = new Container
			{
				displayName = "Notifications"
			};
			val.children.Add((IEnumerable<Widget>)((IEnumerable<NotificationInfo>)m_Notifications).Select((Func<NotificationInfo, BoolField>)delegate(NotificationInfo info)
			{
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Expected O, but got Unknown
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Expected O, but got Unknown
				//IL_0079: Expected O, but got Unknown
				BoolField val2 = new BoolField
				{
					displayName = $"{((Object)m_PrefabSystem.GetPrefab<NotificationIconPrefab>(info.m_Prefab)).name} ({info.m_Instances})"
				};
				((Field<bool>)val2).getter = delegate
				{
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000b: Unknown result type (might be due to invalid IL or missing references)
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					return ((EntityManager)(ref entityManager)).IsComponentEnabled<NotificationIconDisplayData>(info.m_Prefab);
				};
				((Field<bool>)val2).setter = delegate(bool value)
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					EnableNotification(info.m_Prefab, value);
				};
				return val2;
			}));
			list.Add((Widget)val);
		}
		return list;
	}

	[DebugTab("Pathfind", -985)]
	private List<Widget> BuildPathfindDebugUI()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Expected O, but got Unknown
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected O, but got Unknown
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Expected O, but got Unknown
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Expected O, but got Unknown
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Expected O, but got Unknown
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Expected O, but got Unknown
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Expected O, but got Unknown
		PathfindResultSystem pathfindResultSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindResultSystem>();
		PathfindQueueSystem pathfindQueueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PathfindQueueSystem>();
		Table val = new Table
		{
			displayName = "Top 20 queries",
			isReadOnly = true
		};
		for (int i = 0; i < 20; i++)
		{
			int index = i;
			Row val2 = new Row
			{
				displayName = "#" + (i + 1),
				opened = true
			};
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "System type",
				getter = () => GetPathfindSystem(index)
			});
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "Query type",
				getter = () => GetPathfindType(index)
			});
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "Query count",
				getter = () => GetPathfindCount(index)
			});
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "Success rate",
				getter = () => GetSuccessRate(index)
			});
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "Average graph traversal",
				getter = () => GetPathfindTraversal(index)
			});
			((Container)val2).children.Add((Widget)new Value
			{
				displayName = "Average efficiency",
				getter = () => GetPathfindEfficiency(index)
			});
			((Container)val).children.Add((Widget)(object)val2);
		}
		return new List<Widget>
		{
			(Widget)new Value
			{
				displayName = "Graph edge count",
				getter = () => pathfindQueueSystem.GetGraphSize()
			},
			(Widget)new Value
			{
				displayName = "Graph memory",
				getter = GetGraphMemory
			},
			(Widget)new Value
			{
				displayName = "Query memory",
				getter = GetQueryMemory
			},
			(Widget)new Value
			{
				displayName = "Pending queries",
				getter = () => pathfindResultSystem.pendingRequestCount
			},
			(Widget)new Value
			{
				displayName = "Simulation frame margin",
				getter = GetSimulationFrameMargin
			},
			(Widget)new Value
			{
				displayName = "Total queries",
				getter = () => GetPathfindCount(-1)
			},
			(Widget)new Value
			{
				displayName = "Success rate",
				getter = () => GetSuccessRate(-1)
			},
			(Widget)new Value
			{
				displayName = "Average graph traversal",
				getter = () => GetPathfindTraversal(-1)
			},
			(Widget)new Value
			{
				displayName = "Average efficiency",
				getter = () => GetPathfindEfficiency(-1)
			},
			(Widget)(object)val
		};
		string GetGraphMemory()
		{
			pathfindQueueSystem.GetGraphMemory(out var usedMemory, out var allocatedMemory);
			return FormatUtils.FormatBytes((long)usedMemory) + " / " + FormatUtils.FormatBytes((long)allocatedMemory);
		}
		object GetPathfindCount(int num)
		{
			if (num < 0)
			{
				Dictionary<PathfindResultSystem.ResultKey, PathfindResultSystem.ResultValue> queryStats = pathfindResultSystem.queryStats;
				if (m_PathfindQueryBuffer != null)
				{
					m_PathfindQueryBuffer.Clear();
				}
				else
				{
					m_PathfindQueryBuffer = new List<PathfindQueryItem>(queryStats.Count);
				}
				foreach (KeyValuePair<PathfindResultSystem.ResultKey, PathfindResultSystem.ResultValue> item in queryStats)
				{
					m_PathfindQueryBuffer.Add(new PathfindQueryItem
					{
						m_Key = item.Key,
						m_Value = item.Value
					});
				}
				m_PathfindQueryBuffer.Sort();
				int num2 = 0;
				for (int j = 0; j < m_PathfindQueryBuffer.Count; j++)
				{
					num2 += m_PathfindQueryBuffer[j].m_Value.m_QueryCount;
				}
				return num2;
			}
			if (m_PathfindQueryBuffer != null && num < m_PathfindQueryBuffer.Count)
			{
				return m_PathfindQueryBuffer[num].m_Value.m_QueryCount;
			}
			return "-";
		}
		object GetPathfindEfficiency(int num)
		{
			if (m_PathfindQueryBuffer == null)
			{
				return "-";
			}
			if (num < 0)
			{
				int num2 = 0;
				float num3 = 0f;
				for (int j = 0; j < m_PathfindQueryBuffer.Count; j++)
				{
					PathfindResultSystem.ResultValue resultValue = m_PathfindQueryBuffer[j].m_Value;
					num2 += resultValue.m_QueryCount;
					num3 += resultValue.m_Efficiency;
				}
				return num3 / math.max(1f, (float)num2) * 100f + " %";
			}
			if (num < m_PathfindQueryBuffer.Count)
			{
				PathfindResultSystem.ResultValue resultValue2 = m_PathfindQueryBuffer[num].m_Value;
				return resultValue2.m_Efficiency / math.max(1f, (float)resultValue2.m_QueryCount) * 100f + " %";
			}
			return "-";
		}
		object GetPathfindSystem(int num)
		{
			if (m_PathfindQueryBuffer != null && num < m_PathfindQueryBuffer.Count)
			{
				PathfindResultSystem.ResultKey resultKey = m_PathfindQueryBuffer[num].m_Key;
				if (resultKey.m_System != null)
				{
					return resultKey.m_System.GetType().Name;
				}
				return "-";
			}
			return "-";
		}
		object GetPathfindTraversal(int num)
		{
			if (m_PathfindQueryBuffer == null)
			{
				return "-";
			}
			if (num < 0)
			{
				int num2 = 0;
				float num3 = 0f;
				for (int j = 0; j < m_PathfindQueryBuffer.Count; j++)
				{
					PathfindResultSystem.ResultValue resultValue = m_PathfindQueryBuffer[j].m_Value;
					num2 += resultValue.m_QueryCount;
					num3 += resultValue.m_GraphTraversal;
				}
				return num3 / math.max(1f, (float)num2) * 100f + " %";
			}
			if (num < m_PathfindQueryBuffer.Count)
			{
				PathfindResultSystem.ResultValue resultValue2 = m_PathfindQueryBuffer[num].m_Value;
				return resultValue2.m_GraphTraversal / math.max(1f, (float)resultValue2.m_QueryCount) * 100f + " %";
			}
			return "-";
		}
		object GetPathfindType(int num)
		{
			if (m_PathfindQueryBuffer != null && num < m_PathfindQueryBuffer.Count)
			{
				PathfindResultSystem.ResultKey resultKey = m_PathfindQueryBuffer[num].m_Key;
				switch (resultKey.m_QueryType)
				{
				case PathfindResultSystem.QueryType.Pathfind:
					if (resultKey.m_OriginType == SetupTargetType.None && resultKey.m_DestinationType == SetupTargetType.None)
					{
						return "Pathfind";
					}
					return resultKey.m_OriginType.ToString() + " -> " + resultKey.m_DestinationType;
				case PathfindResultSystem.QueryType.Coverage:
					return "Coverage";
				case PathfindResultSystem.QueryType.Availability:
					return "Availability";
				default:
					return "-";
				}
			}
			return "-";
		}
		string GetQueryMemory()
		{
			pathfindQueueSystem.GetQueryMemory(out var usedMemory, out var allocatedMemory);
			return FormatUtils.FormatBytes((long)usedMemory) + " / " + FormatUtils.FormatBytes((long)allocatedMemory);
		}
		object GetSimulationFrameMargin()
		{
			if (pathfindResultSystem.pendingSimulationFrame < uint.MaxValue)
			{
				int num = (int)math.max(0u, pathfindResultSystem.pendingSimulationFrame - m_SimulationSystem.frameIndex - 1);
				if (num > 0)
				{
					float num2 = (float)num * (1f / 48f);
					if (num2 < 1f)
					{
						return $"{num}: Slowing down ({num2}x)";
					}
					return num;
				}
				return "None: Blocking simulation!";
			}
			return "Infinite";
		}
		object GetSuccessRate(int num)
		{
			if (m_PathfindQueryBuffer == null)
			{
				return "-";
			}
			if (num < 0)
			{
				int num2 = 0;
				int num3 = 0;
				for (int j = 0; j < m_PathfindQueryBuffer.Count; j++)
				{
					PathfindResultSystem.ResultValue resultValue = m_PathfindQueryBuffer[j].m_Value;
					num2 += resultValue.m_QueryCount;
					num3 += resultValue.m_SuccessCount;
				}
				return (float)num3 / math.max(1f, (float)num2) * 100f + " %";
			}
			if (num < m_PathfindQueryBuffer.Count)
			{
				PathfindResultSystem.ResultValue resultValue2 = m_PathfindQueryBuffer[num].m_Value;
				return (float)resultValue2.m_SuccessCount / math.max(1f, (float)resultValue2.m_QueryCount) * 100f + " %";
			}
			return "-";
		}
	}

	[DebugTab("Platforms", -10)]
	private static List<Widget> BuildPlatformsDebugUI(World world)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a6: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Expected O, but got Unknown
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Expected O, but got Unknown
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Expected O, but got Unknown
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Expected O, but got Unknown
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Expected O, but got Unknown
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Expected O, but got Unknown
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Expected O, but got Unknown
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Expected O, but got Unknown
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Expected O, but got Unknown
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Expected O, but got Unknown
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Expected O, but got Unknown
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Expected O, but got Unknown
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Expected O, but got Unknown
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Expected O, but got Unknown
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Expected O, but got Unknown
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Expected O, but got Unknown
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Expected O, but got Unknown
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Expected O, but got Unknown
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Expected O, but got Unknown
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Expected O, but got Unknown
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Expected O, but got Unknown
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0761: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Expected O, but got Unknown
		//IL_0790: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a2: Expected O, but got Unknown
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Expected O, but got Unknown
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Expected O, but got Unknown
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Unknown result type (might be due to invalid IL or missing references)
		//IL_086f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Expected O, but got Unknown
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Expected O, but got Unknown
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Expected O, but got Unknown
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Expected O, but got Unknown
		//IL_096c: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0980: Unknown result type (might be due to invalid IL or missing references)
		//IL_098f: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Expected O, but got Unknown
		//IL_0a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a22: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a4c: Expected O, but got Unknown
		//IL_0a4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8f: Expected O, but got Unknown
		//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b65: Expected O, but got Unknown
		//IL_0ac9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b06: Expected O, but got Unknown
		//IL_0b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc3: Expected O, but got Unknown
		//IL_0bda: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bec: Expected O, but got Unknown
		//IL_0c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c20: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c4a: Expected O, but got Unknown
		//IL_0c61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c66: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c73: Expected O, but got Unknown
		//IL_0d78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d88: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9c: Expected O, but got Unknown
		//IL_0c9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ca7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cd1: Expected O, but got Unknown
		if (!GameManager.instance.configuration.qaDeveloperMode)
		{
			return null;
		}
		AchievementTriggerSystem achievementTriggerSystem = world.GetExistingSystemManaged<AchievementTriggerSystem>();
		Foldout val = new Foldout
		{
			displayName = "Platform managers"
		};
		foreach (IPlatformSupport platformManager in PlatformManager.instance.platformManagers)
		{
			ObservableList<Widget> children = ((Container)val).children;
			BoolField val2 = new BoolField
			{
				displayName = ((IPlatformServiceIntegration)platformManager).name
			};
			((Field<bool>)val2).getter = () => ((IPlatformServiceIntegration)platformManager).isInitialized;
			((Field<bool>)val2).setter = delegate(bool value)
			{
				if (value)
				{
					((IPlatformServiceIntegration)platformManager).Initialize(CancellationToken.None);
				}
				else
				{
					((IDisposableAsync)platformManager).Dispose(false, CancellationToken.None);
				}
			};
			children.Add((Widget)val2);
		}
		Foldout val3 = new Foldout
		{
			displayName = "Remote storages"
		};
		foreach (IRemoteStorageSupport remoteStorage in PlatformManager.instance.remoteStorages)
		{
			((Container)val3).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)remoteStorage).name,
				getter = () => (!((IPlatformServiceIntegration)remoteStorage).isInitialized) ? "Not Initialized" : "Initialized"
			});
			((Container)val3).children.Add((Widget)new Button
			{
				displayName = "Wipe data",
				action = delegate
				{
					GameManager.instance.userInterface.appBindings.ShowConfirmationDialog(new ConfirmationDialog(null, "Common.DIALOG_MESSAGE[ConfirmRemoteStorageWipe]", "Common.DIALOG_ACTION[Yes]", "Common.DIALOG_ACTION[No]"), delegate(int ret)
					{
						if (ret == 0)
						{
							remoteStorage.Wipe();
						}
					});
				}
			});
		}
		Foldout val4 = new Foldout
		{
			displayName = "User backends"
		};
		foreach (IUserSupport userBackend in PlatformManager.instance.userBackends)
		{
			((Container)val4).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)userBackend).name,
				getter = () => (!((IPlatformServiceIntegration)userBackend).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val5 = new Foldout
		{
			displayName = "Mod Backends"
		};
		foreach (IModSupport modBackend in PlatformManager.instance.modsBackends)
		{
			((Container)val5).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)modBackend).name,
				getter = () => (!((IPlatformServiceIntegration)modBackend).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val6 = new Foldout
		{
			displayName = "Telemetry Backends"
		};
		foreach (ITelemetrySupport telemetryBackend in PlatformManager.instance.telemetryBackends)
		{
			((Container)val6).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)telemetryBackend).name,
				getter = () => (!((IPlatformServiceIntegration)telemetryBackend).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val7 = new Foldout
		{
			displayName = "Achievements Backends"
		};
		((Container)val7).children.Add((Widget)new Value
		{
			displayName = "Achievements enabled",
			getter = () => PlatformManager.instance.achievementsEnabled
		});
		foreach (IAchievementsSupport achievementsBackend in PlatformManager.instance.achievementsBackends)
		{
			((Container)val7).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)achievementsBackend).name,
				getter = () => (!((IPlatformServiceIntegration)achievementsBackend).isInitialized) ? "Not Initialized" : "Initialized"
			});
			Foldout val8 = new Foldout
			{
				displayName = $"{((IPlatformServiceIntegration)achievementsBackend).name} entries ({achievementsBackend.CountAchievements(true)}/{achievementsBackend.CountAchievements(false)})"
			};
			foreach (IAchievement achievement in achievementsBackend.EnumerateAchievements())
			{
				Foldout val9 = new Foldout
				{
					displayName = $"{achievement.internalName} ({achievement.id})"
				};
				((Container)val9).children.Add((Widget)new Value
				{
					displayName = achievement.backendId,
					getter = () => string.Empty
				});
				((Container)val9).children.Add((Widget)new Value
				{
					displayName = "Achieved",
					getter = () => achievement.achieved
				});
				if (achievement.isIncremental)
				{
					((Container)val9).children.Add((Widget)new Value
					{
						displayName = "Progress",
						getter = () => $"{(achievement.achieved ? achievement.maxProgress : achievement.progress)}/{achievement.maxProgress}"
					});
				}
				if (achievementTriggerSystem.GetDebugData(achievement.id, out var _))
				{
					((Container)val9).children.Add((Widget)new Value
					{
						displayName = "Additional data",
						getter = () => (!achievementTriggerSystem.GetDebugData(achievement.id, out var data2)) ? string.Empty : data2
					});
				}
				((Container)val8).children.Add((Widget)(object)val9);
			}
			((Container)val8).children.Add((Widget)new Button
			{
				displayName = "Reset",
				action = delegate
				{
					achievementsBackend.ResetAchievements();
				}
			});
			((Container)val7).children.Add((Widget)(object)val8);
		}
		Foldout val10 = new Foldout
		{
			displayName = "Rich presence handlers"
		};
		foreach (IRichPresenceSupport richPresenceHandler in PlatformManager.instance.richPresenceHandlers)
		{
			((Container)val10).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)richPresenceHandler).name,
				getter = () => (!((IPlatformServiceIntegration)richPresenceHandler).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val11 = new Foldout
		{
			displayName = "Virtual keyboard support"
		};
		foreach (IVirtualKeyboardSupport virtualKeyboardHandler in PlatformManager.instance.virtualKeyboardHandlers)
		{
			((Container)val11).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)virtualKeyboardHandler).name,
				getter = () => (!((IPlatformServiceIntegration)virtualKeyboardHandler).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val12 = new Foldout
		{
			displayName = "Overlay support"
		};
		foreach (IOverlaySupport overlayHandler in PlatformManager.instance.overlayHandlers)
		{
			((Container)val12).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)overlayHandler).name,
				getter = () => overlayHandler.isOverlaySupported
			});
		}
		Foldout val13 = new Foldout
		{
			displayName = "Transfer managers"
		};
		foreach (ITransferSupport transferManager in PlatformManager.instance.transferManagers)
		{
			((Container)val13).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)transferManager).name,
				getter = () => (!((IPlatformServiceIntegration)transferManager).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val14 = new Foldout
		{
			displayName = "Device association support"
		};
		foreach (IDeviceAssociationSupport deviceAssociationHandler in PlatformManager.instance.deviceAssociationHandlers)
		{
			((Container)val14).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)deviceAssociationHandler).name,
				getter = () => (!((IPlatformServiceIntegration)deviceAssociationHandler).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val15 = new Foldout
		{
			displayName = "Dlc backends"
		};
		PlatformManager localBackend = PlatformManager.instance;
		((Container)val15).children.Add((Widget)new Value
		{
			displayName = localBackend.name,
			getter = () => (!localBackend.isInitialized) ? "Not Initialized" : "Initialized"
		});
		Foldout val16 = new Foldout
		{
			displayName = $"{localBackend.name} entries ({localBackend.dlcCount}/{PlatformManager.instance.dlcCount})"
		};
		foreach (IDlc item2 in localBackend.EnumerateLocalDLCs())
		{
			bool isOwned = localBackend.IsDlcOwned(item2.id);
			Value val17 = new Value
			{
				displayName = $"{item2.internalName} ({item2.id})",
				getter = () => (!isOwned) ? "Not Owned" : "Owned"
			};
			((Container)val16).children.Add((Widget)(object)val17);
		}
		((Container)val15).children.Add((Widget)(object)val16);
		foreach (IDlcSupport dlcBackend in PlatformManager.instance.dlcBackends)
		{
			((Container)val15).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)dlcBackend).name,
				getter = () => (!((IPlatformServiceIntegration)dlcBackend).isInitialized) ? "Not Initialized" : "Initialized"
			});
			Foldout val18 = new Foldout
			{
				displayName = $"{((IPlatformServiceIntegration)dlcBackend).name} entries ({dlcBackend.dlcCount}/{PlatformManager.instance.dlcCount})"
			};
			foreach (IDlc item3 in dlcBackend.EnumerateDLCs())
			{
				bool isOwned2 = dlcBackend.IsDlcOwned(item3);
				Value val19 = new Value
				{
					displayName = $"{item3.internalName} ({item3.id})",
					getter = () => (!isOwned2) ? "Not Owned" : "Owned"
				};
				((Container)val18).children.Add((Widget)(object)val19);
			}
			((Container)val15).children.Add((Widget)(object)val18);
		}
		Foldout val20 = new Foldout
		{
			displayName = "App state handlers"
		};
		foreach (IAppStateSupport appStateHandler in PlatformManager.instance.appStateHandlers)
		{
			((Container)val20).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)appStateHandler).name,
				getter = () => (!((IPlatformServiceIntegration)appStateHandler).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val21 = new Foldout
		{
			displayName = "Screen capture handlers"
		};
		foreach (IScreenCaptureSupport screenCaptureHandler in PlatformManager.instance.screenCaptureHandlers)
		{
			((Container)val21).children.Add((Widget)new Value
			{
				displayName = ((IPlatformServiceIntegration)screenCaptureHandler).name,
				getter = () => (!((IPlatformServiceIntegration)screenCaptureHandler).isInitialized) ? "Not Initialized" : "Initialized"
			});
		}
		Foldout val22 = new Foldout
		{
			displayName = "Content Prefabs"
		};
		string[] availablePrerequisitesNames = GameManager.instance.GetAvailablePrerequisitesNames();
		if (availablePrerequisitesNames != null)
		{
			string[] array = availablePrerequisitesNames;
			foreach (string displayName in array)
			{
				((Container)val22).children.Add((Widget)new Value
				{
					displayName = displayName,
					getter = () => string.Empty
				});
			}
		}
		List<Widget> list = new List<Widget>
		{
			(Widget)(object)val,
			(Widget)(object)val3,
			(Widget)(object)val4,
			(Widget)(object)val5,
			(Widget)(object)val6,
			(Widget)(object)val7,
			(Widget)(object)val10,
			(Widget)(object)val11,
			(Widget)(object)val12,
			(Widget)(object)val13,
			(Widget)(object)val14,
			(Widget)(object)val15,
			(Widget)(object)val20,
			(Widget)(object)val21,
			(Widget)(object)val22
		};
		PdxSdkPlatform pdxSdkPlatform = PlatformManager.instance.GetPSI<PdxSdkPlatform>("PdxSdk");
		if (pdxSdkPlatform != null)
		{
			Button item = new Button
			{
				displayName = "Reset pdx content unlock",
				action = delegate
				{
					foreach (IDlc item4 in pdxSdkPlatform.EnumerateDLCs())
					{
						PlatformManager.instance.UserDataDelete(item4.internalName);
					}
					PlatformManager.instance.UserDataDelete("hasEverLoggedIn");
				}
			};
			list.Add((Widget)(object)item);
		}
		return list;
	}

	[DebugTab("Serialization", -980)]
	private List<Widget> BuildSerializationDebugUI()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		SerializerSystem serializerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SerializerSystem>();
		List<Widget> list = new List<Widget>();
		if (serializerSystem.componentLibrary != null && serializerSystem.systemLibrary != null)
		{
			list.Add((Widget)new Value
			{
				displayName = "Uncompressed size (Overhead)",
				getter = () => GetItem(-1)
			});
			Table val = new Table
			{
				displayName = "Top 100 types",
				isReadOnly = true
			};
			for (int num = 0; num < 100; num++)
			{
				int index = num;
				((Container)val).children.Add((Widget)new Value
				{
					displayName = "#" + (num + 1),
					getter = () => GetItem(index)
				});
			}
			list.Add((Widget)(object)val);
		}
		return list;
		object GetItem(int num4)
		{
			string text = null;
			int num2 = -1;
			int num3 = -1;
			if (num4 == -1)
			{
				ComponentSerializerLibrary componentLibrary = serializerSystem.componentLibrary;
				SystemSerializerLibrary systemLibrary = serializerSystem.systemLibrary;
				int serializerCount = componentLibrary.GetSerializerCount();
				int serializerCount2 = systemLibrary.GetSerializerCount();
				num2 = serializerSystem.totalSize;
				num3 = serializerSystem.totalSize;
				if (m_SerializationBuffer != null)
				{
					m_SerializationBuffer.Clear();
				}
				else
				{
					m_SerializationBuffer = new List<SerializationItem>(serializerCount + serializerCount2 + 1);
				}
				int num5 = default(int);
				for (int i = 0; i < serializerCount; i++)
				{
					int serializedSize = componentLibrary.GetSerializedSize(i, ref num5);
					m_SerializationBuffer.Add(new SerializationItem
					{
						m_Type = componentLibrary.GetSerializer(i).GetComponentType(),
						m_TotalSize = serializedSize + num5,
						m_OverheadSize = num5
					});
					num3 -= serializedSize;
				}
				int num6 = default(int);
				for (int j = 0; j < serializerCount2; j++)
				{
					int serializedSize2 = systemLibrary.GetSerializedSize(j, ref num6);
					m_SerializationBuffer.Add(new SerializationItem
					{
						m_Type = systemLibrary.GetSerializer(j).GetSystemType(),
						m_TotalSize = serializedSize2 + num6,
						m_OverheadSize = num6
					});
					num3 -= serializedSize2;
				}
				int num7 = default(int);
				int obsoleteSerializedSize = componentLibrary.GetObsoleteSerializedSize(ref num7);
				int num8 = default(int);
				obsoleteSerializedSize += systemLibrary.GetObsoleteSerializedSize(ref num8);
				m_SerializationBuffer.Add(new SerializationItem
				{
					m_Type = null,
					m_TotalSize = obsoleteSerializedSize + num7 + num8,
					m_OverheadSize = num7 + num8
				});
				num3 -= obsoleteSerializedSize;
				m_SerializationBuffer.Sort();
			}
			else if (m_SerializationBuffer != null && num4 < m_SerializationBuffer.Count)
			{
				SerializationItem serializationItem = m_SerializationBuffer[num4];
				text = ((!(serializationItem.m_Type == null)) ? serializationItem.m_Type.Name : "Unknown");
				num2 = serializationItem.m_TotalSize;
				num3 = serializationItem.m_OverheadSize;
			}
			if (num2 >= 0)
			{
				string text2 = ((num2 < 1024) ? (num2 + " B (") : ((num2 >= 1048576) ? ((num2 >> 20) + " MB (") : ((num2 >> 10) + " kB (")));
				text2 = ((num3 < 1024) ? (text2 + num3 + " B)") : ((num3 >= 1048576) ? (text2 + (num3 >> 20) + " MB)") : (text2 + (num3 >> 10) + " kB)")));
				if (text != null)
				{
					return text + "\t" + text2;
				}
				return text2;
			}
			return "-";
		}
	}

	[DebugTab("Virtual Texturing", -19)]
	private static List<Widget> BuildVirtualTexturingDebugUI(World world)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Expected O, but got Unknown
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Expected O, but got Unknown
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Expected O, but got Unknown
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Expected O, but got Unknown
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Expected O, but got Unknown
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Expected O, but got Unknown
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Expected O, but got Unknown
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Expected O, but got Unknown
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Expected O, but got Unknown
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Expected O, but got Unknown
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Expected O, but got Unknown
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Expected O, but got Unknown
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Expected O, but got Unknown
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Expected O, but got Unknown
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Expected O, but got Unknown
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Expected O, but got Unknown
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Expected O, but got Unknown
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Expected O, but got Unknown
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0592: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Expected O, but got Unknown
		TextureStreamingSystem tss = world.GetExistingSystemManaged<TextureStreamingSystem>();
		ManagedBatchSystem mbs = world.GetExistingSystemManaged<ManagedBatchSystem>();
		int num = tss.tileSize + 16;
		long tileSizeInBytes = num * num;
		tileSizeInBytes += tileSizeInBytes / 4;
		Foldout val = new Foldout
		{
			displayName = "Memory"
		};
		((Container)val).children.Add((Widget)new Value
		{
			displayName = "Atlas CPU Cache Size",
			getter = () => FormatUtils.FormatBytes((long)tss.atlasDataSize)
		});
		((Container)val).children.Add((Widget)new Value
		{
			displayName = "Data Size",
			getter = () => FormatUtils.FormatBytes((long)tss.dataSize)
		});
		Foldout val2 = new Foldout
		{
			displayName = "Atlas"
		};
		((Container)val2).children.Add((Widget)new Value
		{
			displayName = "Atlas Blocks Stack 0",
			getter = delegate
			{
				int totalNbBlocks = tss.GetTotalNbBlocks(0);
				int nbReservedBlocks = tss.GetNbReservedBlocks(0);
				float num2 = (float)nbReservedBlocks * 100f / (float)totalNbBlocks;
				return nbReservedBlocks + " / " + totalNbBlocks + " (" + num2 + " %)";
			}
		});
		((Container)val2).children.Add((Widget)new Value
		{
			displayName = "Atlas Blocks Stack 1",
			getter = delegate
			{
				int totalNbBlocks = tss.GetTotalNbBlocks(1);
				int nbReservedBlocks = tss.GetNbReservedBlocks(1);
				float num2 = (float)nbReservedBlocks * 100f / (float)totalNbBlocks;
				return nbReservedBlocks + " / " + totalNbBlocks + " (" + num2 + " %)";
			}
		});
		Foldout val3 = new Foldout
		{
			displayName = "Material Loading"
		};
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Nb VT Material Assets",
			getter = () => tss.VTMaterialsCountAssetsCount
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Nb VT Material Duplicates",
			getter = () => tss.VTMaterialsAssetsDuplicatesCount
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Nb VT Materials Left To Load",
			getter = () => tss.VTMaterialsLeftToLoadCount
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Nb VT Materials Loaded per Frame (config)",
			getter = () => tss.virtualTexturingConfig.nbVTMaterialsToStartAsyncLoadingPerFrame
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Load progression",
			getter = () => tss.VTMaterialAssetsProgression * 100f + " %"
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Nb VT Materials Duplicates To Init",
			getter = () => tss.VTMaterialsDuplicatesToProcessCount
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Nb VT Materials Duplicates Inited per Frame (config)",
			getter = () => tss.virtualTexturingConfig.nbVTMaterialsToStartAsyncDuplicateInitPerFrame
		});
		((Container)val3).children.Add((Widget)new Value
		{
			displayName = "Duplicate progression",
			getter = () => tss.VTMaterialDuplicatesProgression * 100f + " %"
		});
		Foldout val4 = new Foldout
		{
			displayName = "Tile Loading Stats"
		};
		((Container)val4).children.Add((Widget)new Value
		{
			displayName = "Nb High Mip CPU Cache Tiles",
			getter = delegate
			{
				long num2 = tss.tilesFilledFromCPUCacheCount * tileSizeInBytes;
				return tss.tilesFilledFromCPUCacheCount + "(" + FormatUtils.FormatBytes(num2) + ")";
			}
		});
		((Container)val4).children.Add((Widget)new Value
		{
			displayName = "Nb Per Surface Async Tile Read",
			getter = delegate
			{
				long num2 = tss.perSurfaceAsyncTileReadCount * tileSizeInBytes;
				return tss.perSurfaceAsyncTileReadCount + " (" + FormatUtils.FormatBytes(num2) + ")";
			}
		});
		((Container)val4).children.Add((Widget)new Value
		{
			displayName = "Nb Per Texture Async Tile Read",
			getter = delegate
			{
				long num2 = tss.perTextureAsyncTileReadCount * tileSizeInBytes;
				return tss.perTextureAsyncTileReadCount + " (" + FormatUtils.FormatBytes(num2) + ")";
			}
		});
		((Container)val4).children.Add((Widget)new Value
		{
			displayName = "Nb Mid Mip Async Tile Read",
			getter = delegate
			{
				long num2 = tss.midMipAsyncTextureReadCount * tileSizeInBytes;
				return tss.midMipAsyncTextureReadCount + " (" + FormatUtils.FormatBytes(num2) + ")";
			}
		});
		Foldout val5 = new Foldout
		{
			displayName = "Textures Requester"
		};
		((Container)val5).children.Add((Widget)new Value
		{
			displayName = "Stack count",
			getter = () => mbs.VTTextureRequester.stacksCount
		});
		((Container)val5).children.Add((Widget)new Value
		{
			displayName = "Registered count",
			getter = () => mbs.VTTextureRequester.registeredCount
		});
		((Container)val5).children.Add((Widget)new Value
		{
			displayName = "Requested this frame",
			getter = () => mbs.VTTextureRequester.requestCount
		});
		return new List<Widget>
		{
			(Widget)new Value
			{
				displayName = "Mip Bias",
				getter = () => tss.mipBias
			},
			(Widget)new Value
			{
				displayName = "Tile Size",
				getter = () => tss.tileSize
			},
			(Widget)new Value
			{
				displayName = "Working Set Bias",
				getter = () => tss.workingSetLodBias,
				formatString = "{0:F1}"
			},
			(Widget)new Value
			{
				displayName = "Nd Mid Mip Levels",
				getter = () => tss.midMipLevelsCount
			},
			(Widget)new Value
			{
				displayName = "Nb BC7 SRGB Entries",
				getter = () => tss.bc7SrgbEntriesCount
			},
			(Widget)new Value
			{
				displayName = "Nb BC7 UNorm Entries",
				getter = () => tss.bc7UNormEntriesCount
			},
			(Widget)new Value
			{
				displayName = "Nb other Entries",
				getter = () => tss.otherEntriesCount
			},
			(Widget)new Value
			{
				displayName = "Nb requests busy/available",
				getter = () => tss.busyRequestsCount + "/" + tss.availableRequestsCount
			},
			(Widget)(object)val3,
			(Widget)(object)val,
			(Widget)(object)val2,
			(Widget)(object)val4,
			(Widget)(object)val5,
			(Widget)new Button
			{
				displayName = "Reload",
				action = delegate
				{
					World defaultGameObjectInjectionWorld = World.DefaultGameObjectInjectionWorld;
					((defaultGameObjectInjectionWorld != null) ? defaultGameObjectInjectionWorld.GetExistingSystemManaged<ManagedBatchSystem>() : null)?.ReloadVT();
				}
			}
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void __AssignQueries(ref SystemState state)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryBuilder val = default(EntityQueryBuilder);
		((EntityQueryBuilder)(ref val))._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		EntityQueryBuilder val2 = ((EntityQueryBuilder)(ref val)).WithAll<TerrainPropertiesData>();
		val2 = ((EntityQueryBuilder)(ref val2)).WithOptions((EntityQueryOptions)16);
		__query_1508003740_0 = ((EntityQueryBuilder)(ref val2)).Build(ref state);
		((EntityQueryBuilder)(ref val)).Reset();
		((EntityQueryBuilder)(ref val)).Dispose();
	}

	protected override void OnCreateForCompiler()
	{
		((ComponentSystemBase)this).OnCreateForCompiler();
		__AssignQueries(ref ((SystemBase)this).CheckedStateRef);
		__TypeHandle.__AssignHandles(ref ((SystemBase)this).CheckedStateRef);
	}

	[Preserve]
	public DebugSystem()
	{
	}
}
