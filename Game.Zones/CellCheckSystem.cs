using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Areas;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Zones;

[CompilerGenerated]
public class CellCheckSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<Block> __Game_Zones_Block_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Owner> __Game_Common_Owner_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Transform> __Game_Objects_Transform_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EdgeGeometry> __Game_Net_EdgeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<StartNodeGeometry> __Game_Net_StartNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<EndNodeGeometry> __Game_Net_EndNodeGeometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Composition> __Game_Net_Composition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<NetCompositionData> __Game_Prefabs_NetCompositionData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<RoadComposition> __Game_Prefabs_RoadComposition_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<AreaGeometryData> __Game_Prefabs_AreaGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ObjectGeometryData> __Game_Prefabs_ObjectGeometryData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.Node> __Game_Areas_Node_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<Triangle> __Game_Areas_Triangle_RO_BufferLookup;

		public BufferLookup<Cell> __Game_Zones_Cell_RW_BufferLookup;

		public ComponentLookup<ValidArea> __Game_Zones_ValidArea_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ValidArea> __Game_Zones_ValidArea_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<BuildOrder> __Game_Zones_BuildOrder_RO_ComponentLookup;

		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentLookup<Game.Objects.Elevation> __Game_Objects_Elevation_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SignatureBuildingData> __Game_Prefabs_SignatureBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PlaceholderBuildingData> __Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Updated> __Game_Common_Updated_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Cell> __Game_Zones_Cell_RO_BufferLookup;

		public BufferLookup<VacantLot> __Game_Zones_VacantLot_RW_BufferLookup;

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
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			__Game_Zones_Block_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Block>(true);
			__Game_Common_Owner_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Owner>(true);
			__Game_Objects_Transform_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Transform>(true);
			__Game_Net_EdgeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EdgeGeometry>(true);
			__Game_Net_StartNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<StartNodeGeometry>(true);
			__Game_Net_EndNodeGeometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<EndNodeGeometry>(true);
			__Game_Net_Composition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Composition>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_NetCompositionData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<NetCompositionData>(true);
			__Game_Prefabs_RoadComposition_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<RoadComposition>(true);
			__Game_Prefabs_AreaGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<AreaGeometryData>(true);
			__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ObjectGeometryData>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Areas_Node_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.Node>(true);
			__Game_Areas_Triangle_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Triangle>(true);
			__Game_Zones_Cell_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Cell>(false);
			__Game_Zones_ValidArea_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ValidArea>(false);
			__Game_Zones_ValidArea_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ValidArea>(true);
			__Game_Zones_BuildOrder_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildOrder>(true);
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Objects_Elevation_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Objects.Elevation>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_SignatureBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SignatureBuildingData>(true);
			__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PlaceholderBuildingData>(true);
			__Game_Prefabs_ZoneData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ZoneData>(true);
			__Game_Prefabs_PrefabData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabData>(true);
			__Game_Common_Updated_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Updated>(true);
			__Game_Zones_Cell_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Cell>(true);
			__Game_Zones_VacantLot_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<VacantLot>(false);
		}
	}

	private UpdateCollectSystem m_ZoneUpdateCollectSystem;

	private Game.Objects.UpdateCollectSystem m_ObjectUpdateCollectSystem;

	private Game.Net.UpdateCollectSystem m_NetUpdateCollectSystem;

	private Game.Areas.UpdateCollectSystem m_AreaUpdateCollectSystem;

	private SearchSystem m_ZoneSearchSystem;

	private Game.Objects.SearchSystem m_ObjectSearchSystem;

	private Game.Net.SearchSystem m_NetSearchSystem;

	private Game.Areas.SearchSystem m_AreaSearchSystem;

	private ZoneSystem m_ZonePrefabSystem;

	private ModificationBarrier5 m_ModificationBarrier;

	private EntityQuery m_DeletedBlocksQuery;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_ZoneUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<UpdateCollectSystem>();
		m_ObjectUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.UpdateCollectSystem>();
		m_NetUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.UpdateCollectSystem>();
		m_AreaUpdateCollectSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.UpdateCollectSystem>();
		m_ZoneSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SearchSystem>();
		m_ObjectSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Objects.SearchSystem>();
		m_NetSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Net.SearchSystem>();
		m_AreaSearchSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<Game.Areas.SearchSystem>();
		m_ZonePrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ZoneSystem>();
		m_ModificationBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ModificationBarrier5>();
		m_DeletedBlocksQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Block>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_051b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0749: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0781: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0800: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Unknown result type (might be due to invalid IL or missing references)
		//IL_080a: Unknown result type (might be due to invalid IL or missing references)
		//IL_080c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0811: Unknown result type (might be due to invalid IL or missing references)
		//IL_0816: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0820: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_082e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0830: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0837: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0844: Unknown result type (might be due to invalid IL or missing references)
		//IL_0846: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_085a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0862: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_0880: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0898: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		if (m_ZoneUpdateCollectSystem.isUpdated || m_ObjectUpdateCollectSystem.isUpdated || m_NetUpdateCollectSystem.netsUpdated || m_AreaUpdateCollectSystem.lotsUpdated || m_AreaUpdateCollectSystem.mapTilesUpdated)
		{
			NativeList<CellCheckHelpers.SortedEntity> val = default(NativeList<CellCheckHelpers.SortedEntity>);
			val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<CellCheckHelpers.BlockOverlap> overlapQueue = default(NativeQueue<CellCheckHelpers.BlockOverlap>);
			overlapQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<CellCheckHelpers.BlockOverlap> blockOverlaps = default(NativeList<CellCheckHelpers.BlockOverlap>);
			blockOverlaps._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeList<CellCheckHelpers.OverlapGroup> val2 = default(NativeList<CellCheckHelpers.OverlapGroup>);
			val2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeQueue<Bounds2> boundsQueue = default(NativeQueue<Bounds2>);
			boundsQueue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<CellCheckHelpers.SortedEntity> blocks = val.AsDeferredJobArray();
			((SystemBase)this).Dependency = JobHandle.CombineDependencies(((SystemBase)this).Dependency, CollectUpdatedBlocks(val));
			JobHandle dependencies;
			NativeQuadTree<Entity, Bounds2> searchTree = m_ZoneSearchSystem.GetSearchTree(readOnly: true, out dependencies);
			JobHandle val3 = default(JobHandle);
			NativeList<ArchetypeChunk> deletedBlockChunks = ((EntityQuery)(ref m_DeletedBlocksQuery)).ToArchetypeChunkListAsync(AllocatorHandle.op_Implicit((Allocator)3), ref val3);
			JobHandle dependencies2;
			JobHandle dependencies3;
			CellBlockJobs.BlockCellsJob blockCellsJob = new CellBlockJobs.BlockCellsJob
			{
				m_Blocks = blocks,
				m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NetSearchTree = m_NetSearchSystem.GetNetSearchTree(readOnly: true, out dependencies2),
				m_AreaSearchTree = m_AreaSearchSystem.GetSearchTree(readOnly: true, out dependencies3),
				m_OwnerData = InternalCompilerInterface.GetComponentLookup<Owner>(ref __TypeHandle.__Game_Common_Owner_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EdgeGeometryData = InternalCompilerInterface.GetComponentLookup<EdgeGeometry>(ref __TypeHandle.__Game_Net_EdgeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_StartNodeGeometryData = InternalCompilerInterface.GetComponentLookup<StartNodeGeometry>(ref __TypeHandle.__Game_Net_StartNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EndNodeGeometryData = InternalCompilerInterface.GetComponentLookup<EndNodeGeometry>(ref __TypeHandle.__Game_Net_EndNodeGeometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CompositionData = InternalCompilerInterface.GetComponentLookup<Composition>(ref __TypeHandle.__Game_Net_Composition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabCompositionData = InternalCompilerInterface.GetComponentLookup<NetCompositionData>(ref __TypeHandle.__Game_Prefabs_NetCompositionData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRoadCompositionData = InternalCompilerInterface.GetComponentLookup<RoadComposition>(ref __TypeHandle.__Game_Prefabs_RoadComposition_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabAreaGeometryData = InternalCompilerInterface.GetComponentLookup<AreaGeometryData>(ref __TypeHandle.__Game_Prefabs_AreaGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NativeData = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaNodes = InternalCompilerInterface.GetBufferLookup<Game.Areas.Node>(ref __TypeHandle.__Game_Areas_Node_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaTriangles = InternalCompilerInterface.GetBufferLookup<Triangle>(ref __TypeHandle.__Game_Areas_Triangle_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ValidAreaData = InternalCompilerInterface.GetComponentLookup<ValidArea>(ref __TypeHandle.__Game_Zones_ValidArea_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			CellCheckHelpers.FindOverlappingBlocksJob findOverlappingBlocksJob = new CellCheckHelpers.FindOverlappingBlocksJob
			{
				m_Blocks = blocks,
				m_SearchTree = searchTree,
				m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ValidAreaData = InternalCompilerInterface.GetComponentLookup<ValidArea>(ref __TypeHandle.__Game_Zones_ValidArea_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildOrderData = InternalCompilerInterface.GetComponentLookup<BuildOrder>(ref __TypeHandle.__Game_Zones_BuildOrder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResultQueue = overlapQueue.AsParallelWriter()
			};
			CellCheckHelpers.GroupOverlappingBlocksJob groupOverlappingBlocksJob = new CellCheckHelpers.GroupOverlappingBlocksJob
			{
				m_Blocks = blocks,
				m_OverlapQueue = overlapQueue,
				m_BlockOverlaps = blockOverlaps,
				m_OverlapGroups = val2
			};
			JobHandle dependencies4;
			CellOccupyJobs.ZoneAndOccupyCellsJob zoneAndOccupyCellsJob = new CellOccupyJobs.ZoneAndOccupyCellsJob
			{
				m_Blocks = blocks,
				m_DeletedBlockChunks = deletedBlockChunks,
				m_ZonePrefabs = m_ZonePrefabSystem.GetPrefabs(),
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ValidAreaData = InternalCompilerInterface.GetComponentLookup<ValidArea>(ref __TypeHandle.__Game_Zones_ValidArea_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ObjectSearchTree = m_ObjectSearchSystem.GetStaticSearchTree(readOnly: true, out dependencies4),
				m_TransformData = InternalCompilerInterface.GetComponentLookup<Transform>(ref __TypeHandle.__Game_Objects_Transform_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ElevationData = InternalCompilerInterface.GetComponentLookup<Game.Objects.Elevation>(ref __TypeHandle.__Game_Objects_Elevation_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabObjectGeometryData = InternalCompilerInterface.GetComponentLookup<ObjectGeometryData>(ref __TypeHandle.__Game_Prefabs_ObjectGeometryData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSpawnableBuildingData = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabSignatureBuildingData = InternalCompilerInterface.GetComponentLookup<SignatureBuildingData>(ref __TypeHandle.__Game_Prefabs_SignatureBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabPlaceholderBuildingData = InternalCompilerInterface.GetComponentLookup<PlaceholderBuildingData>(ref __TypeHandle.__Game_Prefabs_PlaceholderBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabData = InternalCompilerInterface.GetComponentLookup<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			CellOverlapJobs.CheckBlockOverlapJob checkBlockOverlapJob = new CellOverlapJobs.CheckBlockOverlapJob
			{
				m_BlockOverlaps = blockOverlaps.AsDeferredJobArray(),
				m_OverlapGroups = val2.AsDeferredJobArray(),
				m_ZonePrefabs = m_ZonePrefabSystem.GetPrefabs(),
				m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildOrderData = InternalCompilerInterface.GetComponentLookup<BuildOrder>(ref __TypeHandle.__Game_Zones_BuildOrder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ValidAreaData = InternalCompilerInterface.GetComponentLookup<ValidArea>(ref __TypeHandle.__Game_Zones_ValidArea_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			CellCheckHelpers.UpdateBlocksJob updateBlocksJob = new CellCheckHelpers.UpdateBlocksJob
			{
				m_Blocks = blocks,
				m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			LotSizeJobs.UpdateLotSizeJob updateLotSizeJob = new LotSizeJobs.UpdateLotSizeJob
			{
				m_Blocks = blocks,
				m_ZonePrefabs = m_ZonePrefabSystem.GetPrefabs(),
				m_BlockData = InternalCompilerInterface.GetComponentLookup<Block>(ref __TypeHandle.__Game_Zones_Block_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ValidAreaData = InternalCompilerInterface.GetComponentLookup<ValidArea>(ref __TypeHandle.__Game_Zones_ValidArea_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildOrderData = InternalCompilerInterface.GetComponentLookup<BuildOrder>(ref __TypeHandle.__Game_Zones_BuildOrder_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_UpdatedData = InternalCompilerInterface.GetComponentLookup<Updated>(ref __TypeHandle.__Game_Common_Updated_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ZoneData = InternalCompilerInterface.GetComponentLookup<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Cells = InternalCompilerInterface.GetBufferLookup<Cell>(ref __TypeHandle.__Game_Zones_Cell_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SearchTree = searchTree,
				m_VacantLots = InternalCompilerInterface.GetBufferLookup<VacantLot>(ref __TypeHandle.__Game_Zones_VacantLot_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			EntityCommandBuffer val4 = m_ModificationBarrier.CreateCommandBuffer();
			updateLotSizeJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val4)).AsParallelWriter();
			updateLotSizeJob.m_BoundsQueue = boundsQueue.AsParallelWriter();
			LotSizeJobs.UpdateLotSizeJob updateLotSizeJob2 = updateLotSizeJob;
			JobHandle dependencies5;
			LotSizeJobs.UpdateBoundsJob obj = new LotSizeJobs.UpdateBoundsJob
			{
				m_BoundsList = m_ZoneUpdateCollectSystem.GetUpdatedBounds(readOnly: false, out dependencies5),
				m_BoundsQueue = boundsQueue
			};
			JobHandle val5 = IJobParallelForDeferExtensions.Schedule<CellBlockJobs.BlockCellsJob, CellCheckHelpers.SortedEntity>(blockCellsJob, val, 1, JobHandle.CombineDependencies(((SystemBase)this).Dependency, dependencies2, dependencies3));
			JobHandle val6 = IJobParallelForDeferExtensions.Schedule<CellCheckHelpers.FindOverlappingBlocksJob, CellCheckHelpers.SortedEntity>(findOverlappingBlocksJob, val, 1, JobHandle.CombineDependencies(val5, dependencies));
			JobHandle val7 = IJobExtensions.Schedule<CellCheckHelpers.GroupOverlappingBlocksJob>(groupOverlappingBlocksJob, val6);
			JobHandle val8 = IJobParallelForDeferExtensions.Schedule<CellOccupyJobs.ZoneAndOccupyCellsJob, CellCheckHelpers.SortedEntity>(zoneAndOccupyCellsJob, val, 1, JobHandle.CombineDependencies(val5, dependencies4, val3));
			JobHandle val9 = IJobParallelForDeferExtensions.Schedule<CellOverlapJobs.CheckBlockOverlapJob, CellCheckHelpers.OverlapGroup>(checkBlockOverlapJob, val2, 1, JobHandle.CombineDependencies(val7, val8));
			JobHandle val10 = IJobParallelForDeferExtensions.Schedule<CellCheckHelpers.UpdateBlocksJob, CellCheckHelpers.SortedEntity>(updateBlocksJob, val, 1, val9);
			JobHandle val11 = IJobParallelForDeferExtensions.Schedule<LotSizeJobs.UpdateLotSizeJob, CellCheckHelpers.SortedEntity>(updateLotSizeJob2, val, 1, val10);
			JobHandle val12 = IJobExtensions.Schedule<LotSizeJobs.UpdateBoundsJob>(obj, JobHandle.CombineDependencies(val11, dependencies5));
			val.Dispose(val11);
			overlapQueue.Dispose(val7);
			blockOverlaps.Dispose(val9);
			val2.Dispose(val9);
			boundsQueue.Dispose(val12);
			deletedBlockChunks.Dispose(val8);
			m_NetSearchSystem.AddNetSearchTreeReader(val5);
			m_AreaSearchSystem.AddSearchTreeReader(val5);
			m_ZoneSearchSystem.AddSearchTreeReader(val11);
			m_ObjectSearchSystem.AddStaticSearchTreeReader(val8);
			m_ZonePrefabSystem.AddPrefabsReader(val11);
			((EntityCommandBufferSystem)m_ModificationBarrier).AddJobHandleForProducer(val11);
			m_ZoneUpdateCollectSystem.AddBoundsWriter(val12);
			((SystemBase)this).Dependency = val11;
		}
	}

	private JobHandle CollectUpdatedBlocks(NativeList<CellCheckHelpers.SortedEntity> updateBlocksList)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<Entity> queue = default(NativeQueue<Entity>);
		queue._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<Entity> queue2 = default(NativeQueue<Entity>);
		queue2._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<Entity> queue3 = default(NativeQueue<Entity>);
		queue3._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		NativeQueue<Entity> queue4 = default(NativeQueue<Entity>);
		queue4._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		JobHandle dependencies;
		NativeQuadTree<Entity, Bounds2> searchTree = m_ZoneSearchSystem.GetSearchTree(readOnly: true, out dependencies);
		JobHandle val = default(JobHandle);
		if (m_ZoneUpdateCollectSystem.isUpdated)
		{
			JobHandle dependencies2;
			NativeList<Bounds2> updatedBounds = m_ZoneUpdateCollectSystem.GetUpdatedBounds(readOnly: true, out dependencies2);
			JobHandle val2 = IJobParallelForDeferExtensions.Schedule<CellCheckHelpers.FindUpdatedBlocksSingleIterationJob, Bounds2>(new CellCheckHelpers.FindUpdatedBlocksSingleIterationJob
			{
				m_Bounds = updatedBounds.AsDeferredJobArray(),
				m_SearchTree = searchTree,
				m_ResultQueue = queue.AsParallelWriter()
			}, updatedBounds, 1, JobHandle.CombineDependencies(dependencies2, dependencies));
			m_ZoneUpdateCollectSystem.AddBoundsReader(val2);
			val = JobHandle.CombineDependencies(val, val2);
		}
		if (m_ObjectUpdateCollectSystem.isUpdated)
		{
			JobHandle dependencies3;
			NativeList<Bounds2> updatedBounds2 = m_ObjectUpdateCollectSystem.GetUpdatedBounds(out dependencies3);
			JobHandle val3 = IJobParallelForDeferExtensions.Schedule<CellCheckHelpers.FindUpdatedBlocksDoubleIterationJob, Bounds2>(new CellCheckHelpers.FindUpdatedBlocksDoubleIterationJob
			{
				m_Bounds = updatedBounds2.AsDeferredJobArray(),
				m_SearchTree = searchTree,
				m_ResultQueue = queue2.AsParallelWriter()
			}, updatedBounds2, 1, JobHandle.CombineDependencies(dependencies3, dependencies));
			m_ObjectUpdateCollectSystem.AddBoundsReader(val3);
			val = JobHandle.CombineDependencies(val, val3);
		}
		if (m_NetUpdateCollectSystem.netsUpdated)
		{
			JobHandle dependencies4;
			NativeList<Bounds2> updatedNetBounds = m_NetUpdateCollectSystem.GetUpdatedNetBounds(out dependencies4);
			JobHandle val4 = IJobParallelForDeferExtensions.Schedule<CellCheckHelpers.FindUpdatedBlocksDoubleIterationJob, Bounds2>(new CellCheckHelpers.FindUpdatedBlocksDoubleIterationJob
			{
				m_Bounds = updatedNetBounds.AsDeferredJobArray(),
				m_SearchTree = searchTree,
				m_ResultQueue = queue3.AsParallelWriter()
			}, updatedNetBounds, 1, JobHandle.CombineDependencies(dependencies4, dependencies));
			m_NetUpdateCollectSystem.AddNetBoundsReader(val4);
			val = JobHandle.CombineDependencies(val, val4);
		}
		JobHandle val5 = dependencies;
		if (m_AreaUpdateCollectSystem.lotsUpdated)
		{
			JobHandle dependencies5;
			NativeList<Bounds2> updatedLotBounds = m_AreaUpdateCollectSystem.GetUpdatedLotBounds(out dependencies5);
			JobHandle val6 = IJobParallelForDeferExtensions.Schedule<CellCheckHelpers.FindUpdatedBlocksDoubleIterationJob, Bounds2>(new CellCheckHelpers.FindUpdatedBlocksDoubleIterationJob
			{
				m_Bounds = updatedLotBounds.AsDeferredJobArray(),
				m_SearchTree = searchTree,
				m_ResultQueue = queue4.AsParallelWriter()
			}, updatedLotBounds, 1, JobHandle.CombineDependencies(dependencies5, dependencies));
			m_AreaUpdateCollectSystem.AddLotBoundsReader(val6);
			val = JobHandle.CombineDependencies(val, val6);
			val5 = val6;
		}
		if (m_AreaUpdateCollectSystem.mapTilesUpdated)
		{
			JobHandle dependencies6;
			NativeList<Bounds2> updatedMapTileBounds = m_AreaUpdateCollectSystem.GetUpdatedMapTileBounds(out dependencies6);
			JobHandle val7 = IJobParallelForDeferExtensions.Schedule<CellCheckHelpers.FindUpdatedBlocksDoubleIterationJob, Bounds2>(new CellCheckHelpers.FindUpdatedBlocksDoubleIterationJob
			{
				m_Bounds = updatedMapTileBounds.AsDeferredJobArray(),
				m_SearchTree = searchTree,
				m_ResultQueue = queue4.AsParallelWriter()
			}, updatedMapTileBounds, 1, JobHandle.CombineDependencies(dependencies6, val5));
			m_AreaUpdateCollectSystem.AddMapTileBoundsReader(val7);
			val = JobHandle.CombineDependencies(val, val7);
		}
		JobHandle val8 = IJobExtensions.Schedule<CellCheckHelpers.CollectBlocksJob>(new CellCheckHelpers.CollectBlocksJob
		{
			m_Queue1 = queue,
			m_Queue2 = queue2,
			m_Queue3 = queue3,
			m_Queue4 = queue4,
			m_ResultList = updateBlocksList
		}, val);
		queue.Dispose(val8);
		queue2.Dispose(val8);
		queue3.Dispose(val8);
		queue4.Dispose(val8);
		m_ZoneSearchSystem.AddSearchTreeReader(val);
		return val8;
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
	public CellCheckSystem()
	{
	}
}
