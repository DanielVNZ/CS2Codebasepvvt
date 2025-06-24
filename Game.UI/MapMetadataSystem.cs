using System;
using System.Runtime.CompilerServices;
using Colossal.Annotations;
using Colossal.Entities;
using Colossal.Json;
using Colossal.Mathematics;
using Colossal.UI.Binding;
using Game.Areas;
using Game.City;
using Game.Common;
using Game.Objects;
using Game.Prefabs;
using Game.Prefabs.Climate;
using Game.Simulation;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI;

[CompilerGenerated]
public class MapMetadataSystem : GameSystemBase
{
	public struct Resources : IJsonWritable
	{
		public float fertile;

		public float forest;

		public float oil;

		public float ore;

		public float fish;

		public ProxyObject ToVariant()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Expected O, but got Unknown
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Expected O, but got Unknown
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Expected O, but got Unknown
			//IL_008d: Expected O, but got Unknown
			ProxyObject val = new ProxyObject();
			val.Add("fertile", (Variant)new ProxyNumber((IConvertible)fertile));
			val.Add("forest", (Variant)new ProxyNumber((IConvertible)forest));
			val.Add("oil", (Variant)new ProxyNumber((IConvertible)oil));
			val.Add("ore", (Variant)new ProxyNumber((IConvertible)ore));
			val.Add("fish", (Variant)new ProxyNumber((IConvertible)fish));
			return val;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("fertile");
			writer.Write(fertile);
			writer.PropertyName("forest");
			writer.Write(forest);
			writer.PropertyName("oil");
			writer.Write(oil);
			writer.PropertyName("ore");
			writer.Write(ore);
			writer.PropertyName("fish");
			writer.Write(fish);
			writer.TypeEnd();
		}
	}

	public struct Connections : IJsonWritable
	{
		public bool road;

		public bool train;

		public bool air;

		public bool ship;

		public bool electricity;

		public bool water;

		public ProxyObject ToVariant()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Expected O, but got Unknown
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Expected O, but got Unknown
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Expected O, but got Unknown
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Expected O, but got Unknown
			//IL_008a: Expected O, but got Unknown
			ProxyObject val = new ProxyObject();
			val.Add("road", (Variant)new ProxyBoolean(road));
			val.Add("train", (Variant)new ProxyBoolean(train));
			val.Add("air", (Variant)new ProxyBoolean(air));
			val.Add("ship", (Variant)new ProxyBoolean(ship));
			val.Add("electricity", (Variant)new ProxyBoolean(electricity));
			val.Add("water", (Variant)new ProxyBoolean(water));
			return val;
		}

		public void Write(IJsonWriter writer)
		{
			writer.TypeBegin(GetType().FullName);
			writer.PropertyName("road");
			writer.Write(road);
			writer.PropertyName("train");
			writer.Write(train);
			writer.PropertyName("air");
			writer.Write(air);
			writer.PropertyName("ship");
			writer.Write(ship);
			writer.PropertyName("electricity");
			writer.Write(electricity);
			writer.PropertyName("water");
			writer.Write(water);
			writer.TypeEnd();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public BufferTypeHandle<MapFeatureElement> __Game_Areas_MapFeatureElement_RO_BufferTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.ElectricityOutsideConnection> __Game_Objects_ElectricityOutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.WaterPipeOutsideConnection> __Game_Objects_WaterPipeOutsideConnection_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			__Game_Areas_MapFeatureElement_RO_BufferTypeHandle = ((SystemState)(ref state)).GetBufferTypeHandle<MapFeatureElement>(true);
			__Game_Objects_ElectricityOutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.ElectricityOutsideConnection>(true);
			__Game_Objects_WaterPipeOutsideConnection_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Objects.WaterPipeOutsideConnection>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
		}
	}

	private PlanetarySystem m_PlanetarySystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private ClimateSystem m_ClimateSystem;

	private PrefabSystem m_PrefabSystem;

	private float m_Area;

	private float m_BuildableLand;

	private float m_SurfaceWaterAvailability;

	private float m_GroundWaterAvailability;

	private Resources m_Resources;

	private Connections m_Connections;

	private EntityQuery m_MapTileQuery;

	private EntityQuery m_OutsideConnectionQuery;

	private TypeHandle __TypeHandle;

	[CanBeNull]
	public string mapName { get; set; }

	[CanBeNull]
	public string theme
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (m_CityConfigurationSystem.defaultTheme != Entity.Null)
			{
				return ((Object)m_PrefabSystem.GetPrefab<ThemePrefab>(m_CityConfigurationSystem.defaultTheme)).name;
			}
			return null;
		}
	}

	public Bounds1 temperatureRange
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			if (m_ClimateSystem.currentClimate != Entity.Null)
			{
				return m_PrefabSystem.GetPrefab<ClimatePrefab>(m_ClimateSystem.currentClimate).temperatureRange;
			}
			return default(Bounds1);
		}
	}

	public float cloudiness
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (m_ClimateSystem.currentClimate != Entity.Null)
			{
				return m_PrefabSystem.GetPrefab<ClimatePrefab>(m_ClimateSystem.currentClimate).averageCloudiness;
			}
			return 0f;
		}
	}

	public float precipitation
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (m_ClimateSystem.currentClimate != Entity.Null)
			{
				return m_PrefabSystem.GetPrefab<ClimatePrefab>(m_ClimateSystem.currentClimate).averagePrecipitation;
			}
			return 0f;
		}
	}

	public float latitude => m_PlanetarySystem.latitude;

	public float longitude => m_PlanetarySystem.longitude;

	public float area => m_Area;

	public float buildableLand => m_BuildableLand;

	public float surfaceWaterAvailability => m_SurfaceWaterAvailability;

	public float groundWaterAvailability => m_GroundWaterAvailability;

	public Resources resources => m_Resources;

	public Connections connections => m_Connections;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PlanetarySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PlanetarySystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_MapTileQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Objects.OutsideConnection>(),
			ComponentType.ReadOnly<Game.Objects.ElectricityOutsideConnection>(),
			ComponentType.ReadOnly<Game.Objects.WaterPipeOutsideConnection>()
		};
		val.None = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Temp>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_OutsideConnectionQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		((SystemBase)this).CompleteDependency();
		UpdateResources();
		UpdateConnections();
	}

	private void UpdateResources()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		m_Area = 0f;
		m_BuildableLand = 0f;
		m_SurfaceWaterAvailability = 0f;
		m_GroundWaterAvailability = 0f;
		m_Resources = default(Resources);
		BufferTypeHandle<MapFeatureElement> bufferTypeHandle = InternalCompilerInterface.GetBufferTypeHandle<MapFeatureElement>(ref __TypeHandle.__Game_Areas_MapFeatureElement_RO_BufferTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_MapTileQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				BufferAccessor<MapFeatureElement> bufferAccessor = ((ArchetypeChunk)(ref val2)).GetBufferAccessor<MapFeatureElement>(ref bufferTypeHandle);
				for (int j = 0; j < bufferAccessor.Length; j++)
				{
					DynamicBuffer<MapFeatureElement> val3 = bufferAccessor[j];
					m_Area += val3[0].m_Amount;
					m_BuildableLand += val3[1].m_Amount;
					m_SurfaceWaterAvailability += val3[6].m_Amount;
					m_GroundWaterAvailability += val3[7].m_Amount;
					m_Resources.fertile += val3[2].m_Amount;
					m_Resources.forest += val3[3].m_Amount;
					m_Resources.oil += val3[4].m_Amount;
					m_Resources.ore += val3[5].m_Amount;
					m_Resources.fish += val3[8].m_Amount;
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	private void UpdateConnections()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		m_Connections = default(Connections);
		ComponentTypeHandle<Game.Objects.ElectricityOutsideConnection> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.ElectricityOutsideConnection>(ref __TypeHandle.__Game_Objects_ElectricityOutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Game.Objects.WaterPipeOutsideConnection> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<Game.Objects.WaterPipeOutsideConnection>(ref __TypeHandle.__Game_Objects_WaterPipeOutsideConnection_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabRef> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_OutsideConnectionQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			OutsideConnectionData outsideConnectionData = default(OutsideConnectionData);
			for (int i = 0; i < val.Length; i++)
			{
				ArchetypeChunk val2 = val[i];
				NativeArray<PrefabRef> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabRef>(ref componentTypeHandle3);
				m_Connections.electricity |= ((ArchetypeChunk)(ref val2)).Has<Game.Objects.ElectricityOutsideConnection>(ref componentTypeHandle);
				m_Connections.water |= ((ArchetypeChunk)(ref val2)).Has<Game.Objects.WaterPipeOutsideConnection>(ref componentTypeHandle2);
				for (int j = 0; j < nativeArray.Length; j++)
				{
					if (EntitiesExtensions.TryGetComponent<OutsideConnectionData>(((ComponentSystemBase)this).EntityManager, nativeArray[j].m_Prefab, ref outsideConnectionData))
					{
						m_Connections.road |= (outsideConnectionData.m_Type & OutsideConnectionTransferType.Road) != 0;
						m_Connections.train |= (outsideConnectionData.m_Type & OutsideConnectionTransferType.Train) != 0;
						m_Connections.air |= (outsideConnectionData.m_Type & OutsideConnectionTransferType.Air) != 0;
						m_Connections.ship |= (outsideConnectionData.m_Type & OutsideConnectionTransferType.Ship) != 0;
					}
				}
			}
		}
		finally
		{
			val.Dispose();
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
	public MapMetadataSystem()
	{
	}
}
