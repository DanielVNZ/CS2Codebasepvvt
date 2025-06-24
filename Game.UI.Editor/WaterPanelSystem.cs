using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Input;
using Game.Objects;
using Game.Prefabs;
using Game.Reflection;
using Game.Rendering;
using Game.Simulation;
using Game.Tools;
using Game.UI.Widgets;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.Editor;

[CompilerGenerated]
public class WaterPanelSystem : EditorPanelSystemBase
{
	private class WaterConfig
	{
		[Serializable]
		public class ConstantRateWaterSource : WaterSource
		{
			[InspectorName("Editor.WATER_RATE")]
			public float m_Rate;
		}

		[Serializable]
		public class ConstantLevelWaterSource : WaterSource
		{
			[InspectorName("Editor.HEIGHT")]
			public float m_Height;
		}

		[Serializable]
		public class BorderWaterSource : ConstantLevelWaterSource
		{
			[NonSerialized]
			[InspectorName("Editor.FLOOD_HEIGHT")]
			public float m_FloodHeight;
		}

		public abstract class WaterSource
		{
			[NonSerialized]
			public bool m_Initialized;

			[CustomField(typeof(WaterSourcePositionFactory))]
			[InspectorName("Editor.POSITION")]
			public float2 m_Position;

			[InspectorName("Editor.RADIUS")]
			public float m_Radius;

			[InspectorName("Editor.POLLUTION")]
			public float m_Pollution;
		}

		public class WaterSourcePositionFactory : IFieldBuilderFactory
		{
			public FieldBuilder TryCreate(Type memberType, object[] attributes)
			{
				return delegate(IValueAccessor accessor)
				{
					CastAccessor<float2> castAccessor = new CastAccessor<float2>(accessor);
					return new Column
					{
						children = new IWidget[2]
						{
							new Float2InputField
							{
								displayName = "Editor.POSITION",
								tooltip = "Editor.POSITION_TOOLTIP",
								accessor = castAccessor
							},
							new Button
							{
								displayName = "Editor.LOCATE",
								tooltip = "Editor.LOCATE_TOOLTIP",
								action = delegate
								{
									Locate(castAccessor);
								}
							}
						}
					};
				};
			}

			private void Locate(CastAccessor<float2> accessor)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				float2 typedValue = accessor.GetTypedValue();
				World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CameraUpdateSystem>().activeCameraController.pivot = new Vector3(typedValue.x, 0f, typedValue.y);
			}
		}

		[InspectorName("Editor.CONSTANT_RATE_WATER_SOURCES")]
		public List<ConstantRateWaterSource> m_ConstantRateWaterSources = new List<ConstantRateWaterSource>();

		[InspectorName("Editor.CONSTANT_LEVEL_WATER_SOURCES")]
		public List<ConstantLevelWaterSource> m_ConstantLevelWaterSources = new List<ConstantLevelWaterSource>();

		[InspectorName("Editor.BORDER_RIVER_WATER_SOURCES")]
		public List<BorderWaterSource> m_BorderRiverWaterSources = new List<BorderWaterSource>();

		[InspectorName("Editor.BORDER_SEA_WATER_SOURCES")]
		public List<BorderWaterSource> m_BorderSeaWaterSources = new List<BorderWaterSource>();
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private ToolSystem m_ToolSystem;

	private WaterToolSystem m_WaterToolSystem;

	private WaterSystem m_WaterSystem;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_WaterSourceQuery;

	private EntityQuery m_UpdatedSourceQuery;

	private EntityArchetype m_WaterSourceArchetype;

	private WaterConfig m_Config = new WaterConfig();

	private static readonly int[] kWaterSpeedValues = new int[7] { 0, 1, 8, 16, 32, 64, 128 };

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_WaterToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterToolSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSourceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[5]
		{
			ComponentType.ReadOnly<Game.Simulation.WaterSourceData>(),
			ComponentType.ReadOnly<Transform>(),
			ComponentType.Exclude<PrefabRef>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Simulation.WaterSourceData>() };
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Updated>(),
			ComponentType.ReadOnly<Deleted>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_UpdatedSourceQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		m_WaterSourceArchetype = ((EntityManager)(ref entityManager)).CreateArchetype((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<Game.Simulation.WaterSourceData>(),
			ComponentType.ReadWrite<Transform>()
		});
		EditorGenerator editorGenerator = new EditorGenerator();
		title = "Editor.WATER";
		children = new IWidget[1] { Scrollable.WithChildren(new IWidget[2]
		{
			new EditorSection
			{
				displayName = "Editor.WATER_SETTINGS",
				tooltip = "Editor.WATER_SETTINGS_TOOLTIP",
				expanded = true,
				children = editorGenerator.BuildMembers(new ObjectAccessor<WaterConfig>(m_Config), 0, "WaterSettings").ToArray()
			},
			new EditorSection
			{
				displayName = "Editor.WATER_SIMULATION_SPEED",
				tooltip = "Editor.WATER_SIMULATION_SPEED_TOOLTIP",
				children = BuildWaterSpeedToggles()
			}
		}) };
	}

	private IWidget[] BuildWaterSpeedToggles()
	{
		IWidget[] array = new IWidget[kWaterSpeedValues.Length];
		for (int i = 0; i < kWaterSpeedValues.Length; i++)
		{
			int speed = kWaterSpeedValues[i];
			array[i] = new ToggleField
			{
				displayName = $"{speed}x",
				accessor = new DelegateAccessor<bool>(() => m_WaterSystem.WaterSimSpeed == speed, delegate(bool val)
				{
					if (val)
					{
						m_WaterSystem.WaterSimSpeed = speed;
					}
				})
			};
		}
		return array;
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		FetchWaterSources();
	}

	[Preserve]
	protected override void OnStartRunning()
	{
		((COSystemBase)this).OnStartRunning();
		if (InputManager.instance.activeControlScheme == InputManager.ControlScheme.KeyboardAndMouse)
		{
			m_ToolSystem.activeTool = m_WaterToolSystem;
		}
	}

	[Preserve]
	protected override void OnUpdate()
	{
		if (!((EntityQuery)(ref m_UpdatedSourceQuery)).IsEmptyIgnoreFilter)
		{
			FetchWaterSources();
		}
	}

	[Preserve]
	protected override void OnStopRunning()
	{
		if (m_ToolSystem.activeTool == m_WaterToolSystem)
		{
			m_ToolSystem.ActivatePrefabTool(null);
		}
		m_WaterSystem.WaterSimSpeed = 1;
		((COSystemBase)this).OnStopRunning();
	}

	protected override bool OnCancel()
	{
		if (m_ToolSystem.activeTool == m_WaterToolSystem)
		{
			m_ToolSystem.ActivatePrefabTool(null);
			return false;
		}
		return base.OnCancel();
	}

	protected override void OnValueChanged(IWidget widget)
	{
		ApplyWaterSources();
	}

	public void FetchWaterSources()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		m_Config.m_ConstantRateWaterSources.Clear();
		m_Config.m_ConstantLevelWaterSources.Clear();
		m_Config.m_BorderRiverWaterSources.Clear();
		m_Config.m_BorderSeaWaterSources.Clear();
		NativeArray<Game.Simulation.WaterSourceData> val = ((EntityQuery)(ref m_WaterSourceQuery)).ToComponentDataArray<Game.Simulation.WaterSourceData>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Transform> val2 = ((EntityQuery)(ref m_WaterSourceQuery)).ToComponentDataArray<Transform>(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				Transform transform;
				switch (val[i].m_ConstantDepth)
				{
				case 0:
				{
					List<WaterConfig.ConstantRateWaterSource> list4 = m_Config.m_ConstantRateWaterSources;
					WaterConfig.ConstantRateWaterSource obj4 = new WaterConfig.ConstantRateWaterSource
					{
						m_Initialized = true,
						m_Rate = val[i].m_Amount
					};
					transform = val2[i];
					obj4.m_Position = ((float3)(ref transform.m_Position)).xz;
					obj4.m_Radius = val[i].m_Radius;
					obj4.m_Pollution = val[i].m_Polluted;
					list4.Add(obj4);
					break;
				}
				case 1:
				{
					List<WaterConfig.ConstantLevelWaterSource> list3 = m_Config.m_ConstantLevelWaterSources;
					WaterConfig.ConstantLevelWaterSource obj3 = new WaterConfig.ConstantLevelWaterSource
					{
						m_Initialized = true,
						m_Height = val[i].m_Amount
					};
					transform = val2[i];
					obj3.m_Position = ((float3)(ref transform.m_Position)).xz;
					obj3.m_Radius = val[i].m_Radius;
					obj3.m_Pollution = val[i].m_Polluted;
					list3.Add(obj3);
					break;
				}
				case 2:
				{
					List<WaterConfig.BorderWaterSource> list2 = m_Config.m_BorderRiverWaterSources;
					WaterConfig.BorderWaterSource obj2 = new WaterConfig.BorderWaterSource
					{
						m_Initialized = true,
						m_FloodHeight = val[i].m_Multiplier,
						m_Height = val[i].m_Amount
					};
					transform = val2[i];
					obj2.m_Position = ((float3)(ref transform.m_Position)).xz;
					obj2.m_Radius = val[i].m_Radius;
					obj2.m_Pollution = val[i].m_Polluted;
					list2.Add(obj2);
					break;
				}
				case 3:
				{
					List<WaterConfig.BorderWaterSource> list = m_Config.m_BorderSeaWaterSources;
					WaterConfig.BorderWaterSource obj = new WaterConfig.BorderWaterSource
					{
						m_Initialized = true,
						m_FloodHeight = val[i].m_Multiplier,
						m_Height = val[i].m_Amount
					};
					transform = val2[i];
					obj.m_Position = ((float3)(ref transform.m_Position)).xz;
					obj.m_Radius = val[i].m_Radius;
					obj.m_Pollution = val[i].m_Polluted;
					list.Add(obj);
					break;
				}
				}
			}
		}
		finally
		{
			val.Dispose();
			val2.Dispose();
		}
	}

	private void ApplyWaterSources()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		EntityCommandBuffer buffer = m_EndFrameBarrier.CreateCommandBuffer();
		NativeArray<Entity> sources = ((EntityQuery)(ref m_WaterSourceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		int sourceCount = 0;
		TerrainHeightData terrainHeightData = m_TerrainSystem.GetHeightData();
		foreach (WaterConfig.ConstantRateWaterSource item in m_Config.m_ConstantRateWaterSources)
		{
			AddSource(buffer, GetSource(buffer, sources, ref sourceCount), item, 0, ref item.m_Rate, ref terrainHeightData);
		}
		foreach (WaterConfig.ConstantLevelWaterSource item2 in m_Config.m_ConstantLevelWaterSources)
		{
			AddSource(buffer, GetSource(buffer, sources, ref sourceCount), item2, 1, ref item2.m_Height, ref terrainHeightData);
		}
		foreach (WaterConfig.BorderWaterSource item3 in m_Config.m_BorderRiverWaterSources)
		{
			AddBorderSource(buffer, GetSource(buffer, sources, ref sourceCount), item3, 2, ref terrainHeightData);
		}
		foreach (WaterConfig.BorderWaterSource item4 in m_Config.m_BorderSeaWaterSources)
		{
			AddBorderSource(buffer, GetSource(buffer, sources, ref sourceCount), item4, 3, ref terrainHeightData);
		}
		while (sourceCount < sources.Length)
		{
			((EntityCommandBuffer)(ref buffer)).AddComponent<Deleted>(sources[sourceCount++], default(Deleted));
		}
		sources.Dispose();
	}

	private Entity GetSource(EntityCommandBuffer buffer, NativeArray<Entity> sources, ref int sourceCount)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (sourceCount < sources.Length)
		{
			return sources[sourceCount++];
		}
		return ((EntityCommandBuffer)(ref buffer)).CreateEntity(m_WaterSourceArchetype);
	}

	private void AddSource(EntityCommandBuffer buffer, Entity entity, WaterConfig.WaterSource source, int constantDepth, ref float amount, ref TerrainHeightData terrainHeightData)
	{
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!source.m_Initialized)
		{
			CameraUpdateSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CameraUpdateSystem>();
			float3 val = float3.op_Implicit(existingSystemManaged.activeCameraController.pivot);
			source.m_Initialized = true;
			source.m_Position = ((float3)(ref val)).xz;
			Bounds3 val2 = MathUtils.Expand(TerrainUtils.GetBounds(ref terrainHeightData), float3.op_Implicit(0f - source.m_Radius));
			if (!MathUtils.Intersect(((Bounds3)(ref val2)).xz, source.m_Position))
			{
				source.m_Position = MathUtils.Clamp(source.m_Position, ((Bounds3)(ref val2)).xz);
				((float3)(ref val)).xz = source.m_Position;
				existingSystemManaged.activeCameraController.pivot = float3.op_Implicit(val);
			}
			if (constantDepth == 0)
			{
				source.m_Radius = 30f;
				amount = 20f;
			}
			else
			{
				source.m_Radius = 40f;
				amount = TerrainUtils.SampleHeight(ref terrainHeightData, new float3(source.m_Position.x, 0f, source.m_Position.y));
				amount += 25f - m_TerrainSystem.positionOffset.y;
			}
		}
		float3 val3 = default(float3);
		((float3)(ref val3))._002Ector(source.m_Position.x, 0f, source.m_Position.y);
		Game.Simulation.WaterSourceData waterSourceData = new Game.Simulation.WaterSourceData
		{
			m_Amount = amount,
			m_ConstantDepth = constantDepth,
			m_Radius = source.m_Radius,
			m_Polluted = source.m_Pollution
		};
		waterSourceData.m_Multiplier = WaterSystem.CalculateSourceMultiplier(waterSourceData, val3);
		((EntityCommandBuffer)(ref buffer)).SetComponent<Game.Simulation.WaterSourceData>(entity, waterSourceData);
		((EntityCommandBuffer)(ref buffer)).SetComponent<Transform>(entity, new Transform
		{
			m_Position = val3,
			m_Rotation = quaternion.identity
		});
	}

	private void AddBorderSource(EntityCommandBuffer buffer, Entity entity, WaterConfig.BorderWaterSource source, int constantDepth, ref TerrainHeightData terrainHeightData)
	{
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		if (!source.m_Initialized)
		{
			CameraUpdateSystem existingSystemManaged = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CameraUpdateSystem>();
			float3 val = float3.op_Implicit(existingSystemManaged.activeCameraController.pivot);
			source.m_Initialized = true;
			source.m_Position = ((float3)(ref val)).xz;
			Bounds3 bounds = TerrainUtils.GetBounds(ref terrainHeightData);
			Bounds3 val2 = MathUtils.Expand(bounds, float3.op_Implicit(0f - source.m_Radius));
			Bounds3 val3 = MathUtils.Expand(bounds, float3.op_Implicit(source.m_Radius));
			if (!MathUtils.Intersect(((Bounds3)(ref val3)).xz, source.m_Position))
			{
				source.m_Position = MathUtils.Clamp(source.m_Position, ((Bounds3)(ref bounds)).xz);
				((float3)(ref val)).xz = source.m_Position;
				existingSystemManaged.activeCameraController.pivot = float3.op_Implicit(val);
			}
			else if (MathUtils.Intersect(((Bounds3)(ref val2)).xz, source.m_Position))
			{
				float2 val4 = source.m_Position - ((float3)(ref val2.min)).xz;
				float2 val5 = ((float3)(ref val2.max)).xz - source.m_Position;
				float2 val6 = math.select(((float3)(ref bounds.min)).xz, ((float3)(ref bounds.max)).xz, val5 < val4);
				val4 = math.min(val4, val5);
				source.m_Position = math.select(source.m_Position, val6, ((float2)(ref val4)).xy < ((float2)(ref val4)).yx);
				((float3)(ref val)).xz = source.m_Position;
				existingSystemManaged.activeCameraController.pivot = float3.op_Implicit(val);
			}
			source.m_Height = TerrainUtils.SampleHeight(ref terrainHeightData, new float3(source.m_Position.x, 0f, source.m_Position.y));
			source.m_Height -= m_TerrainSystem.positionOffset.y;
			if (constantDepth == 2)
			{
				source.m_Radius = 50f;
				source.m_Height += 30f;
			}
			else
			{
				source.m_Radius = 5000f;
				source.m_Height += 100f;
			}
		}
		((EntityCommandBuffer)(ref buffer)).SetComponent<Game.Simulation.WaterSourceData>(entity, new Game.Simulation.WaterSourceData
		{
			m_Amount = source.m_Height,
			m_ConstantDepth = constantDepth,
			m_Radius = source.m_Radius,
			m_Multiplier = source.m_FloodHeight,
			m_Polluted = source.m_Pollution
		});
		((EntityCommandBuffer)(ref buffer)).SetComponent<Transform>(entity, new Transform
		{
			m_Position = new float3(source.m_Position.x, 0f, source.m_Position.y),
			m_Rotation = quaternion.identity
		});
	}

	[Preserve]
	public WaterPanelSystem()
	{
	}
}
