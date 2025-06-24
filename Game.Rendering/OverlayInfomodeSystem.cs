using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.City;
using Game.Prefabs;
using Game.Simulation;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class OverlayInfomodeSystem : GameSystemBase
{
	[BurstCompile]
	private struct ClearJob : IJob
	{
		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			for (int i = 0; i < m_TextureData.Length; i++)
			{
				m_TextureData[i] = 0;
			}
		}
	}

	[BurstCompile]
	private struct GroundWaterJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<GroundWater> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					GroundWater groundWater = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(groundWater.m_Amount / 32, 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct GroundPollutionJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<GroundPollution> m_MapData;

		public NativeArray<byte> m_TextureData;

		public float m_Multiplier;

		public void Execute()
		{
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					GroundPollution groundPollution = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt((float)groundPollution.m_Pollution * m_Multiplier), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct NoisePollutionJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<NoisePollution> m_MapData;

		public NativeArray<byte> m_TextureData;

		public float m_Multiplier;

		public bool m_Water;

		public void Execute()
		{
			int num = (m_Water ? (m_ActiveData.m_SecondaryIndex - 5) : (m_ActiveData.m_Index - 1));
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					NoisePollution noisePollution = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt((float)noisePollution.m_Pollution * m_Multiplier), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct AirPollutionJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<AirPollution> m_MapData;

		public NativeArray<byte> m_TextureData;

		public float m_Multiplier;

		public bool m_Water;

		public void Execute()
		{
			int num = (m_Water ? (m_ActiveData.m_SecondaryIndex - 5) : (m_ActiveData.m_Index - 1));
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					AirPollution airPollution = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt((float)airPollution.m_Pollution * m_Multiplier), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct WindJob : IJob
	{
		[ReadOnly]
		public CellMapData<Wind> m_MapData;

		public NativeArray<half4> m_TextureData;

		public void Execute()
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num = j + i * m_MapData.m_TextureSize.x;
					Wind wind = m_MapData.m_Buffer[num];
					m_TextureData[num] = new half4((half)wind.m_Wind.x, (half)wind.m_Wind.y, (half)0f, (half)0f);
				}
			}
		}
	}

	[BurstCompile]
	private struct TelecomCoverageJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<TelecomCoverage> m_MapData;

		public NativeArray<byte> m_TextureData;

		public bool m_Water;

		public void Execute()
		{
			int num = (m_Water ? (m_ActiveData.m_SecondaryIndex - 5) : (m_ActiveData.m_Index - 1));
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					TelecomCoverage telecomCoverage = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(telecomCoverage.networkQuality, 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct FertilityJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<NaturalResourceCell> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					NaturalResourceCell naturalResourceCell = m_MapData.m_Buffer[num2];
					float num3 = (int)naturalResourceCell.m_Fertility.m_Base;
					num3 -= (float)(int)naturalResourceCell.m_Fertility.m_Used;
					num3 = math.saturate(num3 * 0.0001f);
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(num3 * 255f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct OreJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<NaturalResourceCell> m_MapData;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			int num = m_ActiveData.m_Index - 1;
			DynamicBuffer<CityModifier> modifiers = default(DynamicBuffer<CityModifier>);
			if (m_CityModifiers.HasBuffer(m_City))
			{
				modifiers = m_CityModifiers[m_City];
			}
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					NaturalResourceCell naturalResourceCell = m_MapData.m_Buffer[num2];
					float value = (int)naturalResourceCell.m_Ore.m_Base;
					if (modifiers.IsCreated)
					{
						CityUtils.ApplyModifier(ref value, modifiers, CityModifierType.OreResourceAmount);
					}
					value -= (float)(int)naturalResourceCell.m_Ore.m_Used;
					value = math.saturate(value * 0.0001f);
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(value * 255f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct OilJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<NaturalResourceCell> m_MapData;

		[ReadOnly]
		public Entity m_City;

		[ReadOnly]
		public BufferLookup<CityModifier> m_CityModifiers;

		public NativeArray<byte> m_TextureData;

		public bool m_Water;

		public void Execute()
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			int num = (m_Water ? (m_ActiveData.m_SecondaryIndex - 5) : (m_ActiveData.m_Index - 1));
			DynamicBuffer<CityModifier> modifiers = default(DynamicBuffer<CityModifier>);
			if (m_CityModifiers.HasBuffer(m_City))
			{
				modifiers = m_CityModifiers[m_City];
			}
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					NaturalResourceCell naturalResourceCell = m_MapData.m_Buffer[num2];
					float value = (int)naturalResourceCell.m_Oil.m_Base;
					if (modifiers.IsCreated)
					{
						CityUtils.ApplyModifier(ref value, modifiers, CityModifierType.OilResourceAmount);
					}
					value -= (float)(int)naturalResourceCell.m_Oil.m_Used;
					value = math.saturate(value * 0.0001f);
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(value * 255f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct FishJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<NaturalResourceCell> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			int num = m_ActiveData.m_Index - 5;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					NaturalResourceCell naturalResourceCell = m_MapData.m_Buffer[num2];
					float num3 = (int)naturalResourceCell.m_Fish.m_Base;
					num3 -= (float)(int)naturalResourceCell.m_Fish.m_Used;
					num3 = math.saturate(num3 * 0.0001f);
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(num3 * 255f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct LandValueJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<LandValueCell> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					LandValueCell landValueCell = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(landValueCell.m_LandValue * 0.51f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct PopulationJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<PopulationCell> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					PopulationCell populationCell = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(populationCell.Get() * 0.24902344f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct AttractionJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<AvailabilityInfoCell> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					AvailabilityInfoCell availabilityInfoCell = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(availabilityInfoCell.m_AvailabilityInfo.x * 15.9375f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct CustomerJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<AvailabilityInfoCell> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					AvailabilityInfoCell availabilityInfoCell = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(availabilityInfoCell.m_AvailabilityInfo.y * 15.9375f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct WorkplaceJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<AvailabilityInfoCell> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					AvailabilityInfoCell availabilityInfoCell = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(availabilityInfoCell.m_AvailabilityInfo.z * 15.9375f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct ServiceJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<AvailabilityInfoCell> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					AvailabilityInfoCell availabilityInfoCell = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(Mathf.RoundToInt(availabilityInfoCell.m_AvailabilityInfo.w * 15.9375f), 0, 255);
				}
			}
		}
	}

	[BurstCompile]
	private struct GroundWaterPollutionJob : IJob
	{
		[ReadOnly]
		public InfomodeActive m_ActiveData;

		[ReadOnly]
		public CellMapData<GroundWater> m_MapData;

		public NativeArray<byte> m_TextureData;

		public void Execute()
		{
			int num = m_ActiveData.m_Index - 1;
			for (int i = 0; i < m_MapData.m_TextureSize.y; i++)
			{
				for (int j = 0; j < m_MapData.m_TextureSize.x; j++)
				{
					int num2 = j + i * m_MapData.m_TextureSize.x;
					GroundWater groundWater = m_MapData.m_Buffer[num2];
					m_TextureData[num2 * 4 + num] = (byte)math.clamp(math.min(groundWater.m_Amount / 32, groundWater.m_Polluted * 256 / math.max(1, (int)groundWater.m_Amount)), 0, 255);
				}
			}
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentTypeHandle<InfoviewHeatmapData> __Game_Prefabs_InfoviewHeatmapData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<InfomodeActive> __Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle;

		[ReadOnly]
		public BufferLookup<CityModifier> __Game_City_CityModifier_RO_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_InfoviewHeatmapData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfoviewHeatmapData>(true);
			__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<InfomodeActive>(true);
			__Game_City_CityModifier_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<CityModifier>(true);
		}
	}

	private TerrainRenderSystem m_TerrainRenderSystem;

	private WaterRenderSystem m_WaterRenderSystem;

	private GroundWaterSystem m_GroundWaterSystem;

	private GroundPollutionSystem m_GroundPollutionSystem;

	private NoisePollutionSystem m_NoisePollutionSystem;

	private AirPollutionSystem m_AirPollutionSystem;

	private WindSystem m_WindSystem;

	private CitySystem m_CitySystem;

	private TelecomPreviewSystem m_TelecomCoverageSystem;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private LandValueSystem m_LandValueSystem;

	private PopulationToGridSystem m_PopulationToGridSystem;

	private AvailabilityInfoToGridSystem m_AvailabilityInfoToGridSystem;

	private ToolSystem m_ToolSystem;

	private EntityQuery m_InfomodeQuery;

	private EntityQuery m_HappinessParameterQuery;

	private Texture2D m_TerrainTexture;

	private Texture2D m_WaterTexture;

	private Texture2D m_WindTexture;

	private JobHandle m_Dependency;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Expected O, but got Unknown
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_TerrainRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainRenderSystem>();
		m_WaterRenderSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterRenderSystem>();
		m_GroundWaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundWaterSystem>();
		m_GroundPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<GroundPollutionSystem>();
		m_NoisePollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NoisePollutionSystem>();
		m_AirPollutionSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AirPollutionSystem>();
		m_WindSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_TelecomCoverageSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TelecomPreviewSystem>();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_LandValueSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LandValueSystem>();
		m_PopulationToGridSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PopulationToGridSystem>();
		m_AvailabilityInfoToGridSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<AvailabilityInfoToGridSystem>();
		m_TerrainTexture = new Texture2D(1, 1, (TextureFormat)4, false, true)
		{
			name = "TerrainInfoTexture",
			hideFlags = (HideFlags)61,
			wrapMode = (TextureWrapMode)1
		};
		m_WaterTexture = new Texture2D(1, 1, (TextureFormat)4, false, true)
		{
			name = "WaterInfoTexture",
			hideFlags = (HideFlags)61,
			wrapMode = (TextureWrapMode)1
		};
		m_WindTexture = new Texture2D(m_WindSystem.TextureSize.x, m_WindSystem.TextureSize.y, (GraphicsFormat)48, 1, (TextureCreationFlags)0)
		{
			name = "WindInfoTexture",
			hideFlags = (HideFlags)61,
			wrapMode = (TextureWrapMode)1
		};
		m_InfomodeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<InfomodeActive>(),
			ComponentType.ReadOnly<InfoviewHeatmapData>()
		});
		m_HappinessParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<CitizenHappinessParameterData>() });
	}

	[Preserve]
	protected override void OnDestroy()
	{
		CoreUtils.Destroy((Object)(object)m_TerrainTexture);
		CoreUtils.Destroy((Object)(object)m_WaterTexture);
		CoreUtils.Destroy((Object)(object)m_WindTexture);
		base.OnDestroy();
	}

	public void ApplyOverlay()
	{
		if ((Object)(object)m_TerrainRenderSystem.overrideOverlaymap == (Object)(object)m_TerrainTexture)
		{
			((JobHandle)(ref m_Dependency)).Complete();
			m_TerrainTexture.Apply();
		}
		if ((Object)(object)m_TerrainRenderSystem.overlayExtramap == (Object)(object)m_WindTexture)
		{
			((JobHandle)(ref m_Dependency)).Complete();
			m_WindTexture.Apply();
		}
		if ((Object)(object)m_WaterRenderSystem.overrideOverlaymap == (Object)(object)m_WaterTexture)
		{
			((JobHandle)(ref m_Dependency)).Complete();
			m_WaterTexture.Apply();
		}
	}

	private NativeArray<byte> GetTerrainTextureData<T>(CellMapData<T> cellMapData) where T : struct, ISerializable
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetTerrainTextureData(cellMapData.m_TextureSize);
	}

	private NativeArray<byte> GetTerrainTextureData(int2 size)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (((Texture)m_TerrainTexture).width != size.x || ((Texture)m_TerrainTexture).height != size.y)
		{
			m_TerrainTexture.Reinitialize(size.x, size.y);
			m_TerrainRenderSystem.overrideOverlaymap = null;
		}
		if ((Object)(object)m_TerrainRenderSystem.overrideOverlaymap != (Object)(object)m_TerrainTexture)
		{
			m_TerrainRenderSystem.overrideOverlaymap = (Texture)(object)m_TerrainTexture;
			ClearJob clearJob = new ClearJob
			{
				m_TextureData = m_TerrainTexture.GetRawTextureData<byte>()
			};
			m_Dependency = IJobExtensions.Schedule<ClearJob>(clearJob, ((SystemBase)this).Dependency);
			((SystemBase)this).Dependency = m_Dependency;
		}
		return m_TerrainTexture.GetRawTextureData<byte>();
	}

	private NativeArray<byte> GetWaterTextureData<T>(CellMapData<T> cellMapData) where T : struct, ISerializable
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetWaterTextureData(cellMapData.m_TextureSize);
	}

	private NativeArray<byte> GetWaterTextureData(int2 size)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (((Texture)m_WaterTexture).width != size.x || ((Texture)m_WaterTexture).height != size.y)
		{
			m_WaterTexture.Reinitialize(size.x, size.y);
			m_WaterRenderSystem.overrideOverlaymap = null;
		}
		if ((Object)(object)m_WaterRenderSystem.overrideOverlaymap != (Object)(object)m_WaterTexture)
		{
			m_WaterRenderSystem.overrideOverlaymap = (Texture)(object)m_WaterTexture;
			ClearJob clearJob = new ClearJob
			{
				m_TextureData = m_WaterTexture.GetRawTextureData<byte>()
			};
			m_Dependency = IJobExtensions.Schedule<ClearJob>(clearJob, ((SystemBase)this).Dependency);
			((SystemBase)this).Dependency = m_Dependency;
		}
		return m_WaterTexture.GetRawTextureData<byte>();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c18: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c33: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c38: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0522: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_0637: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_0799: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0894: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_097b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0980: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Unknown result type (might be due to invalid IL or missing references)
		//IL_098d: Unknown result type (might be due to invalid IL or missing references)
		//IL_098f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_0999: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a80: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0add: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ae2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0923: Unknown result type (might be due to invalid IL or missing references)
		//IL_092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0933: Unknown result type (might be due to invalid IL or missing references)
		//IL_0935: Unknown result type (might be due to invalid IL or missing references)
		//IL_093b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b89: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_082b: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		m_TerrainRenderSystem.overrideOverlaymap = null;
		m_TerrainRenderSystem.overlayExtramap = null;
		m_TerrainRenderSystem.overlayArrowMask = default(float4);
		m_WaterRenderSystem.overrideOverlaymap = null;
		m_WaterRenderSystem.overlayExtramap = null;
		m_WaterRenderSystem.overlayPollutionMask = default(float4);
		m_WaterRenderSystem.overlayArrowMask = default(float4);
		if (!((EntityQuery)(ref m_InfomodeQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_InfomodeQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			ComponentTypeHandle<InfoviewHeatmapData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<InfoviewHeatmapData>(ref __TypeHandle.__Game_Prefabs_InfoviewHeatmapData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<InfomodeActive> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<InfomodeActive>(ref __TypeHandle.__Game_Prefabs_InfomodeActive_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<InfoviewHeatmapData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfoviewHeatmapData>(ref componentTypeHandle);
				NativeArray<InfomodeActive> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<InfomodeActive>(ref componentTypeHandle2);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					InfoviewHeatmapData infoviewHeatmapData = nativeArray[j];
					InfomodeActive activeData = nativeArray2[j];
					switch (infoviewHeatmapData.m_Type)
					{
					case HeatmapData.GroundWater:
					{
						JobHandle dependencies17;
						GroundWaterJob groundWaterJob = new GroundWaterJob
						{
							m_ActiveData = activeData,
							m_MapData = m_GroundWaterSystem.GetData(readOnly: true, out dependencies17)
						};
						groundWaterJob.m_TextureData = GetTerrainTextureData<GroundWater>(groundWaterJob.m_MapData);
						JobHandle val19 = IJobExtensions.Schedule<GroundWaterJob>(groundWaterJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies17));
						m_GroundWaterSystem.AddReader(val19);
						m_Dependency = val19;
						((SystemBase)this).Dependency = val19;
						break;
					}
					case HeatmapData.GroundPollution:
					{
						CitizenHappinessParameterData singleton3 = ((EntityQuery)(ref m_HappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>();
						JobHandle dependencies16;
						GroundPollutionJob groundPollutionJob = new GroundPollutionJob
						{
							m_ActiveData = activeData,
							m_MapData = m_GroundPollutionSystem.GetData(readOnly: true, out dependencies16),
							m_Multiplier = 256f / ((float)singleton3.m_MaxAirAndGroundPollutionBonus * (float)singleton3.m_PollutionBonusDivisor)
						};
						groundPollutionJob.m_TextureData = GetTerrainTextureData<GroundPollution>(groundPollutionJob.m_MapData);
						JobHandle val18 = IJobExtensions.Schedule<GroundPollutionJob>(groundPollutionJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies16));
						m_GroundPollutionSystem.AddReader(val18);
						m_Dependency = val18;
						((SystemBase)this).Dependency = val18;
						break;
					}
					case HeatmapData.AirPollution:
					{
						CitizenHappinessParameterData singleton2 = ((EntityQuery)(ref m_HappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>();
						JobHandle dependencies15;
						AirPollutionJob airPollutionJob = new AirPollutionJob
						{
							m_ActiveData = activeData,
							m_MapData = m_AirPollutionSystem.GetData(readOnly: true, out dependencies15),
							m_Multiplier = 256f / ((float)singleton2.m_MaxAirAndGroundPollutionBonus * (float)singleton2.m_PollutionBonusDivisor)
						};
						airPollutionJob.m_TextureData = GetTerrainTextureData<AirPollution>(airPollutionJob.m_MapData);
						JobHandle val17 = IJobExtensions.Schedule<AirPollutionJob>(airPollutionJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies15));
						airPollutionJob.m_TextureData = GetWaterTextureData<AirPollution>(airPollutionJob.m_MapData);
						airPollutionJob.m_Water = true;
						val17 = JobHandle.CombineDependencies(val17, IJobExtensions.Schedule<AirPollutionJob>(airPollutionJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies15)));
						m_AirPollutionSystem.AddReader(val17);
						m_Dependency = val17;
						((SystemBase)this).Dependency = val17;
						break;
					}
					case HeatmapData.Noise:
					{
						CitizenHappinessParameterData singleton = ((EntityQuery)(ref m_HappinessParameterQuery)).GetSingleton<CitizenHappinessParameterData>();
						JobHandle dependencies14;
						NoisePollutionJob noisePollutionJob = new NoisePollutionJob
						{
							m_ActiveData = activeData,
							m_MapData = m_NoisePollutionSystem.GetData(readOnly: true, out dependencies14),
							m_Multiplier = 256f / ((float)singleton.m_MaxNoisePollutionBonus * (float)singleton.m_PollutionBonusDivisor)
						};
						noisePollutionJob.m_TextureData = GetTerrainTextureData<NoisePollution>(noisePollutionJob.m_MapData);
						JobHandle val16 = IJobExtensions.Schedule<NoisePollutionJob>(noisePollutionJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies14));
						noisePollutionJob.m_TextureData = GetWaterTextureData<NoisePollution>(noisePollutionJob.m_MapData);
						noisePollutionJob.m_Water = true;
						val16 = JobHandle.CombineDependencies(val16, IJobExtensions.Schedule<NoisePollutionJob>(noisePollutionJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies14)));
						m_NoisePollutionSystem.AddReader(val16);
						m_Dependency = val16;
						((SystemBase)this).Dependency = val16;
						break;
					}
					case HeatmapData.Wind:
					{
						m_TerrainRenderSystem.overlayExtramap = (Texture)(object)m_WindTexture;
						m_WaterRenderSystem.overlayExtramap = (Texture)(object)m_WindTexture;
						float4 overlayArrowMask2 = default(float4);
						float4 overlayArrowMask3 = default(float4);
						((float4)(ref overlayArrowMask2))[activeData.m_Index - 1] = 1f;
						((float4)(ref overlayArrowMask3))[activeData.m_SecondaryIndex - 5] = 1f;
						m_TerrainRenderSystem.overlayArrowMask = overlayArrowMask2;
						m_WaterRenderSystem.overlayArrowMask = overlayArrowMask3;
						JobHandle dependencies13;
						JobHandle val15 = IJobExtensions.Schedule<WindJob>(new WindJob
						{
							m_MapData = m_WindSystem.GetData(readOnly: true, out dependencies13),
							m_TextureData = m_WindTexture.GetRawTextureData<half4>()
						}, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies13));
						m_WindSystem.AddReader(val15);
						m_Dependency = val15;
						((SystemBase)this).Dependency = val15;
						break;
					}
					case HeatmapData.WaterFlow:
					{
						m_WaterRenderSystem.overlayExtramap = m_WaterRenderSystem.flowTexture;
						float4 overlayArrowMask = default(float4);
						((float4)(ref overlayArrowMask))[activeData.m_Index - 5] = 1f;
						m_WaterRenderSystem.overlayArrowMask = overlayArrowMask;
						break;
					}
					case HeatmapData.TelecomCoverage:
					{
						JobHandle dependencies12;
						TelecomCoverageJob telecomCoverageJob = new TelecomCoverageJob
						{
							m_ActiveData = activeData,
							m_MapData = m_TelecomCoverageSystem.GetData(readOnly: true, out dependencies12)
						};
						telecomCoverageJob.m_TextureData = GetTerrainTextureData<TelecomCoverage>(telecomCoverageJob.m_MapData);
						JobHandle val14 = IJobExtensions.Schedule<TelecomCoverageJob>(telecomCoverageJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies12));
						telecomCoverageJob.m_TextureData = GetWaterTextureData<TelecomCoverage>(telecomCoverageJob.m_MapData);
						telecomCoverageJob.m_Water = true;
						val14 = JobHandle.CombineDependencies(val14, IJobExtensions.Schedule<TelecomCoverageJob>(telecomCoverageJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies12)));
						m_TelecomCoverageSystem.AddReader(val14);
						m_Dependency = val14;
						((SystemBase)this).Dependency = val14;
						break;
					}
					case HeatmapData.Fertility:
					{
						JobHandle dependencies11;
						FertilityJob fertilityJob = new FertilityJob
						{
							m_ActiveData = activeData,
							m_MapData = m_NaturalResourceSystem.GetData(readOnly: true, out dependencies11)
						};
						fertilityJob.m_TextureData = GetTerrainTextureData<NaturalResourceCell>(fertilityJob.m_MapData);
						JobHandle val13 = IJobExtensions.Schedule<FertilityJob>(fertilityJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies11));
						m_NaturalResourceSystem.AddReader(val13);
						m_Dependency = val13;
						((SystemBase)this).Dependency = val13;
						break;
					}
					case HeatmapData.Ore:
					{
						JobHandle dependencies10;
						OreJob oreJob = new OreJob
						{
							m_ActiveData = activeData,
							m_MapData = m_NaturalResourceSystem.GetData(readOnly: true, out dependencies10),
							m_City = m_CitySystem.City,
							m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
						};
						oreJob.m_TextureData = GetTerrainTextureData<NaturalResourceCell>(oreJob.m_MapData);
						JobHandle val12 = IJobExtensions.Schedule<OreJob>(oreJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies10));
						m_NaturalResourceSystem.AddReader(val12);
						m_Dependency = val12;
						((SystemBase)this).Dependency = val12;
						break;
					}
					case HeatmapData.Oil:
					{
						JobHandle dependencies9;
						OilJob oilJob = new OilJob
						{
							m_ActiveData = activeData,
							m_MapData = m_NaturalResourceSystem.GetData(readOnly: true, out dependencies9),
							m_CityModifiers = InternalCompilerInterface.GetBufferLookup<CityModifier>(ref __TypeHandle.__Game_City_CityModifier_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
						};
						oilJob.m_TextureData = GetTerrainTextureData<NaturalResourceCell>(oilJob.m_MapData);
						JobHandle val11 = IJobExtensions.Schedule<OilJob>(oilJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies9));
						oilJob.m_TextureData = GetWaterTextureData<NaturalResourceCell>(oilJob.m_MapData);
						oilJob.m_Water = true;
						val11 = JobHandle.CombineDependencies(val11, IJobExtensions.Schedule<OilJob>(oilJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies9)));
						m_NaturalResourceSystem.AddReader(val11);
						m_Dependency = val11;
						((SystemBase)this).Dependency = val11;
						break;
					}
					case HeatmapData.Fish:
					{
						JobHandle dependencies8;
						FishJob fishJob = new FishJob
						{
							m_ActiveData = activeData,
							m_MapData = m_NaturalResourceSystem.GetData(readOnly: true, out dependencies8)
						};
						fishJob.m_TextureData = GetWaterTextureData<NaturalResourceCell>(fishJob.m_MapData);
						JobHandle val10 = IJobExtensions.Schedule<FishJob>(fishJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies8));
						m_NaturalResourceSystem.AddReader(val10);
						m_Dependency = val10;
						((SystemBase)this).Dependency = val10;
						break;
					}
					case HeatmapData.LandValue:
					{
						JobHandle dependencies7;
						LandValueJob landValueJob = new LandValueJob
						{
							m_ActiveData = activeData,
							m_MapData = m_LandValueSystem.GetData(readOnly: true, out dependencies7)
						};
						landValueJob.m_TextureData = GetTerrainTextureData<LandValueCell>(landValueJob.m_MapData);
						JobHandle val9 = IJobExtensions.Schedule<LandValueJob>(landValueJob, JobHandle.CombineDependencies(dependencies7, ((SystemBase)this).Dependency));
						m_LandValueSystem.AddReader(val9);
						m_Dependency = val9;
						((SystemBase)this).Dependency = val9;
						break;
					}
					case HeatmapData.Population:
					{
						JobHandle dependencies6;
						PopulationJob populationJob = new PopulationJob
						{
							m_ActiveData = activeData,
							m_MapData = m_PopulationToGridSystem.GetData(readOnly: true, out dependencies6)
						};
						populationJob.m_TextureData = GetTerrainTextureData<PopulationCell>(populationJob.m_MapData);
						JobHandle val8 = IJobExtensions.Schedule<PopulationJob>(populationJob, JobHandle.CombineDependencies(dependencies6, ((SystemBase)this).Dependency));
						m_PopulationToGridSystem.AddReader(val8);
						m_Dependency = val8;
						((SystemBase)this).Dependency = val8;
						break;
					}
					case HeatmapData.Attraction:
					{
						JobHandle dependencies5;
						AttractionJob attractionJob = new AttractionJob
						{
							m_ActiveData = activeData,
							m_MapData = m_AvailabilityInfoToGridSystem.GetData(readOnly: true, out dependencies5)
						};
						attractionJob.m_TextureData = GetTerrainTextureData<AvailabilityInfoCell>(attractionJob.m_MapData);
						JobHandle val7 = IJobExtensions.Schedule<AttractionJob>(attractionJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies5));
						m_AvailabilityInfoToGridSystem.AddReader(val7);
						m_Dependency = val7;
						((SystemBase)this).Dependency = val7;
						break;
					}
					case HeatmapData.Customers:
					{
						JobHandle dependencies4;
						CustomerJob customerJob = new CustomerJob
						{
							m_ActiveData = activeData,
							m_MapData = m_AvailabilityInfoToGridSystem.GetData(readOnly: true, out dependencies4)
						};
						customerJob.m_TextureData = GetTerrainTextureData<AvailabilityInfoCell>(customerJob.m_MapData);
						JobHandle val6 = IJobExtensions.Schedule<CustomerJob>(customerJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies4));
						m_AvailabilityInfoToGridSystem.AddReader(val6);
						m_Dependency = val6;
						((SystemBase)this).Dependency = val6;
						break;
					}
					case HeatmapData.Workplaces:
					{
						JobHandle dependencies3;
						WorkplaceJob workplaceJob = new WorkplaceJob
						{
							m_ActiveData = activeData,
							m_MapData = m_AvailabilityInfoToGridSystem.GetData(readOnly: true, out dependencies3)
						};
						workplaceJob.m_TextureData = GetTerrainTextureData<AvailabilityInfoCell>(workplaceJob.m_MapData);
						JobHandle val5 = IJobExtensions.Schedule<WorkplaceJob>(workplaceJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies3));
						m_AvailabilityInfoToGridSystem.AddReader(val5);
						m_Dependency = val5;
						((SystemBase)this).Dependency = val5;
						break;
					}
					case HeatmapData.Services:
					{
						JobHandle dependencies2;
						ServiceJob serviceJob = new ServiceJob
						{
							m_ActiveData = activeData,
							m_MapData = m_AvailabilityInfoToGridSystem.GetData(readOnly: true, out dependencies2)
						};
						serviceJob.m_TextureData = GetTerrainTextureData<AvailabilityInfoCell>(serviceJob.m_MapData);
						JobHandle val4 = IJobExtensions.Schedule<ServiceJob>(serviceJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies2));
						m_AvailabilityInfoToGridSystem.AddReader(val4);
						m_Dependency = val4;
						((SystemBase)this).Dependency = val4;
						break;
					}
					case HeatmapData.GroundWaterPollution:
					{
						JobHandle dependencies;
						GroundWaterPollutionJob groundWaterPollutionJob = new GroundWaterPollutionJob
						{
							m_ActiveData = activeData,
							m_MapData = m_GroundWaterSystem.GetData(readOnly: true, out dependencies)
						};
						groundWaterPollutionJob.m_TextureData = GetTerrainTextureData<GroundWater>(groundWaterPollutionJob.m_MapData);
						JobHandle val3 = IJobExtensions.Schedule<GroundWaterPollutionJob>(groundWaterPollutionJob, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies));
						m_GroundWaterSystem.AddReader(val3);
						m_Dependency = val3;
						((SystemBase)this).Dependency = val3;
						break;
					}
					case HeatmapData.WaterPollution:
					{
						float4 overlayPollutionMask = default(float4);
						((float4)(ref overlayPollutionMask))[activeData.m_Index - 5] = 1f;
						m_WaterRenderSystem.overlayPollutionMask = overlayPollutionMask;
						break;
					}
					}
				}
			}
			val.Dispose();
		}
		if ((Object)(object)m_ToolSystem.activeInfoview != (Object)null)
		{
			if ((Object)(object)m_TerrainRenderSystem.overrideOverlaymap == (Object)null)
			{
				GetTerrainTextureData(int2.op_Implicit(1));
			}
			if ((Object)(object)m_WaterRenderSystem.overrideOverlaymap == (Object)null)
			{
				GetWaterTextureData(int2.op_Implicit(1));
			}
		}
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
	public OverlayInfomodeSystem()
	{
	}
}
