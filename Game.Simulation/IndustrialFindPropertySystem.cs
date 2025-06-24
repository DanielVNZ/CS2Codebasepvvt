using System.Runtime.CompilerServices;
using Colossal.Entities;
using Game.Agents;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class IndustrialFindPropertySystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentTypeHandle;

		public ComponentTypeHandle<PropertySeeker> __Game_Agents_PropertySeeker_RW_ComponentTypeHandle;

		public ComponentTypeHandle<CompanyData> __Game_Companies_CompanyData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Game.Companies.StorageCompany> __Game_Companies_StorageCompany_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentLookup<BuildingPropertyData> __Game_Prefabs_BuildingPropertyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<IndustrialProcessData> __Game_Prefabs_IndustrialProcessData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<PropertyOnMarket> __Game_Buildings_PropertyOnMarket_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<ResourceAvailability> __Game_Net_ResourceAvailability_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<BuildingData> __Game_Prefabs_BuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Building> __Game_Buildings_Building_RO_ComponentLookup;

		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RW_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ResourceData> __Game_Prefabs_ResourceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<LandValue> __Game_Net_LandValue_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ServiceCompanyData> __Game_Companies_ServiceCompanyData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<SpawnableBuildingData> __Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<WorkplaceData> __Game_Prefabs_WorkplaceData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<CommercialCompany> __Game_Companies_CommercialCompany_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Signature> __Game_Buildings_Signature_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Renter> __Game_Buildings_Renter_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<Attached> __Game_Objects_Attached_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Extractor> __Game_Areas_Extractor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<ExtractorAreaData> __Game_Prefabs_ExtractorAreaData_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Geometry> __Game_Areas_Geometry_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Game.Areas.Lot> __Game_Areas_Lot_RO_ComponentLookup;

		[ReadOnly]
		public BufferLookup<Game.Areas.SubArea> __Game_Areas_SubArea_RO_BufferLookup;

		[ReadOnly]
		public BufferLookup<InstalledUpgrade> __Game_Buildings_InstalledUpgrade_RO_BufferLookup;

		[ReadOnly]
		public ComponentLookup<PropertyRenter> __Game_Buildings_PropertyRenter_RO_ComponentLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabRef>(true);
			__Game_Agents_PropertySeeker_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PropertySeeker>(false);
			__Game_Companies_CompanyData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<CompanyData>(false);
			__Game_Companies_StorageCompany_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Game.Companies.StorageCompany>(true);
			__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingPropertyData>(true);
			__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<IndustrialProcessData>(true);
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Buildings_PropertyOnMarket_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyOnMarket>(true);
			__Game_Net_ResourceAvailability_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<ResourceAvailability>(true);
			__Game_Prefabs_BuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<BuildingData>(true);
			__Game_Buildings_Building_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Building>(true);
			__Game_Buildings_PropertyRenter_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(false);
			__Game_Prefabs_ResourceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ResourceData>(true);
			__Game_Net_LandValue_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<LandValue>(true);
			__Game_Companies_ServiceCompanyData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ServiceCompanyData>(true);
			__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<SpawnableBuildingData>(true);
			__Game_Prefabs_WorkplaceData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<WorkplaceData>(true);
			__Game_Companies_CommercialCompany_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<CommercialCompany>(true);
			__Game_Buildings_Signature_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Signature>(true);
			__Game_Buildings_Renter_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Renter>(true);
			__Game_Objects_Attached_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Attached>(true);
			__Game_Areas_Extractor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Extractor>(true);
			__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<ExtractorAreaData>(true);
			__Game_Areas_Geometry_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Geometry>(true);
			__Game_Areas_Lot_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Game.Areas.Lot>(true);
			__Game_Areas_SubArea_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Game.Areas.SubArea>(true);
			__Game_Buildings_InstalledUpgrade_RO_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<InstalledUpgrade>(true);
			__Game_Buildings_PropertyRenter_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PropertyRenter>(true);
		}
	}

	private EndFrameBarrier m_EndFrameBarrier;

	private ResourceSystem m_ResourceSystem;

	private PropertyProcessingSystem m_PropertyProcessingSystem;

	private IndustrialDemandSystem m_IndustrialDemandSystem;

	private CountCompanyDataSystem m_CountCompanyDataSystem;

	private ClimateSystem m_ClimateSystem;

	private EntityQuery m_IndustryQuery;

	private EntityQuery m_ExtractorQuery;

	private EntityQuery m_FreePropertyQuery;

	private EntityQuery m_EconomyParameterQuery;

	private EntityQuery m_ZonePreferenceQuery;

	private EntityQuery m_FreeExtractorQuery;

	private EntityQuery m_CompanyPrefabQuery;

	private EntityQuery m_ExtractorParameterQuery;

	private TypeHandle __TypeHandle;

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		return 16;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_EndFrameBarrier = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EndFrameBarrier>();
		m_ResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ResourceSystem>();
		m_PropertyProcessingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PropertyProcessingSystem>();
		m_IndustrialDemandSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<IndustrialDemandSystem>();
		m_CountCompanyDataSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CountCompanyDataSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		m_CompanyPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IndustrialCompanyData>() });
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_ExtractorParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ExtractorParameterData>() });
		m_IndustryQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadWrite<IndustrialCompany>(),
			ComponentType.ReadWrite<CompanyData>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<ServiceAvailable>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Created>(),
			ComponentType.Exclude<Game.Companies.ExtractorCompany>()
		});
		m_ExtractorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadWrite<IndustrialCompany>(),
			ComponentType.ReadWrite<CompanyData>(),
			ComponentType.ReadOnly<Game.Companies.ExtractorCompany>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<ServiceAvailable>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Created>()
		});
		m_FreeExtractorQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[8]
		{
			ComponentType.ReadWrite<PropertyOnMarket>(),
			ComponentType.ReadWrite<ExtractorProperty>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<Abandoned>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Condemned>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_FreePropertyQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[9]
		{
			ComponentType.ReadWrite<PropertyOnMarket>(),
			ComponentType.ReadWrite<IndustrialProperty>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.Exclude<ExtractorProperty>(),
			ComponentType.Exclude<Abandoned>(),
			ComponentType.Exclude<Condemned>(),
			ComponentType.Exclude<Destroyed>(),
			ComponentType.Exclude<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_ZonePreferenceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<ZonePreferenceData>() });
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[2] { m_IndustryQuery, m_ExtractorQuery });
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0640: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		JobHandle deps = default(JobHandle);
		if (!((EntityQuery)(ref m_IndustryQuery)).IsEmptyIgnoreFilter)
		{
			JobHandle val = default(JobHandle);
			JobHandle val2 = default(JobHandle);
			PropertyUtils.CompanyFindPropertyJob companyFindPropertyJob = new PropertyUtils.CompanyFindPropertyJob
			{
				m_EntityType = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabType = InternalCompilerInterface.GetComponentTypeHandle<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_PropertySeekerType = InternalCompilerInterface.GetComponentTypeHandle<PropertySeeker>(ref __TypeHandle.__Game_Agents_PropertySeeker_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_CompanyDataType = InternalCompilerInterface.GetComponentTypeHandle<CompanyData>(ref __TypeHandle.__Game_Companies_CompanyData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_StorageCompanyType = InternalCompilerInterface.GetComponentTypeHandle<Game.Companies.StorageCompany>(ref __TypeHandle.__Game_Companies_StorageCompany_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef),
				m_FreePropertyEntities = ((EntityQuery)(ref m_FreePropertyQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val),
				m_PropertyPrefabs = ((EntityQuery)(ref m_FreePropertyQuery)).ToComponentDataListAsync<PrefabRef>(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val2),
				m_BuildingPropertyDatas = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_IndustrialProcessDatas = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PrefabFromEntity = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertiesOnMarket = InternalCompilerInterface.GetComponentLookup<PropertyOnMarket>(ref __TypeHandle.__Game_Buildings_PropertyOnMarket_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Availabilities = InternalCompilerInterface.GetBufferLookup<ResourceAvailability>(ref __TypeHandle.__Game_Net_ResourceAvailability_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_BuildingDatas = InternalCompilerInterface.GetComponentLookup<BuildingData>(ref __TypeHandle.__Game_Prefabs_BuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Buildings = InternalCompilerInterface.GetComponentLookup<Building>(ref __TypeHandle.__Game_Buildings_Building_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_LandValues = InternalCompilerInterface.GetComponentLookup<LandValue>(ref __TypeHandle.__Game_Net_LandValue_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_ServiceCompanies = InternalCompilerInterface.GetComponentLookup<ServiceCompanyData>(ref __TypeHandle.__Game_Companies_ServiceCompanyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_SpawnableDatas = InternalCompilerInterface.GetComponentLookup<SpawnableBuildingData>(ref __TypeHandle.__Game_Prefabs_SpawnableBuildingData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_WorkplaceDatas = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_CommercialCompanies = InternalCompilerInterface.GetComponentLookup<CommercialCompany>(ref __TypeHandle.__Game_Companies_CommercialCompany_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Signatures = InternalCompilerInterface.GetComponentLookup<Signature>(ref __TypeHandle.__Game_Buildings_Signature_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_Renters = InternalCompilerInterface.GetBufferLookup<Renter>(ref __TypeHandle.__Game_Buildings_Renter_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
				m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
				m_ZonePreferences = ((EntityQuery)(ref m_ZonePreferenceQuery)).GetSingleton<ZonePreferenceData>(),
				m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
				m_Commercial = false,
				m_RentActionQueue = m_PropertyProcessingSystem.GetRentActionQueue(out deps).AsParallelWriter()
			};
			EntityCommandBuffer val3 = m_EndFrameBarrier.CreateCommandBuffer();
			companyFindPropertyJob.m_CommandBuffer = ((EntityCommandBuffer)(ref val3)).AsParallelWriter();
			PropertyUtils.CompanyFindPropertyJob companyFindPropertyJob2 = companyFindPropertyJob;
			((SystemBase)this).Dependency = JobChunkExtensions.ScheduleParallel<PropertyUtils.CompanyFindPropertyJob>(companyFindPropertyJob2, m_IndustryQuery, JobUtils.CombineDependencies(val, val2, deps, ((SystemBase)this).Dependency));
			m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
			m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
			m_PropertyProcessingSystem.AddWriter(((SystemBase)this).Dependency);
		}
		JobHandle val4 = default(JobHandle);
		JobHandle val5 = default(JobHandle);
		JobHandle val6 = default(JobHandle);
		JobHandle deps2;
		JobHandle deps3;
		PropertyUtils.ExtractorFindCompanyJob extractorFindCompanyJob = new PropertyUtils.ExtractorFindCompanyJob
		{
			m_Entities = ((EntityQuery)(ref m_FreeExtractorQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val4),
			m_ExtractorCompanyEntities = ((EntityQuery)(ref m_ExtractorQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val5),
			m_CompanyPrefabs = ((EntityQuery)(ref m_CompanyPrefabQuery)).ToEntityListAsync(AllocatorHandle.op_Implicit(((RewindableAllocator)(ref ((ComponentSystemBase)this).World.UpdateAllocator)).ToAllocator), ref val6),
			m_Attached = InternalCompilerInterface.GetComponentLookup<Attached>(ref __TypeHandle.__Game_Objects_Attached_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorAreas = InternalCompilerInterface.GetComponentLookup<Extractor>(ref __TypeHandle.__Game_Areas_Extractor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ExtractorDatas = InternalCompilerInterface.GetComponentLookup<ExtractorAreaData>(ref __TypeHandle.__Game_Prefabs_ExtractorAreaData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Geometries = InternalCompilerInterface.GetComponentLookup<Geometry>(ref __TypeHandle.__Game_Areas_Geometry_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Lots = InternalCompilerInterface.GetComponentLookup<Game.Areas.Lot>(ref __TypeHandle.__Game_Areas_Lot_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Prefabs = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Processes = InternalCompilerInterface.GetComponentLookup<IndustrialProcessData>(ref __TypeHandle.__Game_Prefabs_IndustrialProcessData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_Properties = InternalCompilerInterface.GetComponentLookup<BuildingPropertyData>(ref __TypeHandle.__Game_Prefabs_BuildingPropertyData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourceDatas = InternalCompilerInterface.GetComponentLookup<ResourceData>(ref __TypeHandle.__Game_Prefabs_ResourceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_SubAreas = InternalCompilerInterface.GetBufferLookup<Game.Areas.SubArea>(ref __TypeHandle.__Game_Areas_SubArea_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_InstalledUpgrades = InternalCompilerInterface.GetBufferLookup<InstalledUpgrade>(ref __TypeHandle.__Game_Buildings_InstalledUpgrade_RO_BufferLookup, ref ((SystemBase)this).CheckedStateRef),
			m_WorkplaceDatas = InternalCompilerInterface.GetComponentLookup<WorkplaceData>(ref __TypeHandle.__Game_Prefabs_WorkplaceData_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_PropertyRenters = InternalCompilerInterface.GetComponentLookup<PropertyRenter>(ref __TypeHandle.__Game_Buildings_PropertyRenter_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
			m_ResourcePrefabs = m_ResourceSystem.GetPrefabs(),
			m_Productions = m_CountCompanyDataSystem.GetProduction(out deps2),
			m_Consumptions = m_IndustrialDemandSystem.GetConsumption(out deps3),
			m_ExtractorParameters = ((EntityQuery)(ref m_ExtractorParameterQuery)).GetSingleton<ExtractorParameterData>(),
			m_EconomyParameters = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>(),
			m_RentActionQueue = m_PropertyProcessingSystem.GetRentActionQueue(out deps),
			m_CommandBuffer = m_EndFrameBarrier.CreateCommandBuffer(),
			m_AverageTemperature = m_ClimateSystem.averageTemperature
		};
		((SystemBase)this).Dependency = IJobExtensions.Schedule<PropertyUtils.ExtractorFindCompanyJob>(extractorFindCompanyJob, JobUtils.CombineDependencies(((SystemBase)this).Dependency, val4, val5, val6, deps2, deps3, deps));
		m_ResourceSystem.AddPrefabsReader(((SystemBase)this).Dependency);
		m_IndustrialDemandSystem.AddReader(((SystemBase)this).Dependency);
		m_CountCompanyDataSystem.AddReader(((SystemBase)this).Dependency);
		m_PropertyProcessingSystem.AddWriter(((SystemBase)this).Dependency);
		m_EndFrameBarrier.AddJobHandleForProducer(((SystemBase)this).Dependency);
	}

	public static float Evaluate(Entity company, Entity property, ref IndustrialProcessData process, ref PropertySeeker propertySeeker, ComponentLookup<Building> buildings, ComponentLookup<PropertyOnMarket> propertiesOnMarket, ComponentLookup<PrefabRef> prefabFromEntity, ComponentLookup<BuildingData> buildingDatas, ComponentLookup<SpawnableBuildingData> spawnableDatas, ComponentLookup<WorkplaceData> workplaceDatas, ComponentLookup<LandValue> landValues, BufferLookup<ResourceAvailability> availabilities, EconomyParameterData economyParameters, ResourcePrefabs resourcePrefabs, ComponentLookup<ResourceData> resourceDatas, ComponentLookup<BuildingPropertyData> propertyDatas, bool storage)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		if (buildings.HasComponent(property) && availabilities.HasBuffer(buildings[property].m_RoadEdge))
		{
			Building building = buildings[property];
			Entity prefab = prefabFromEntity[property].m_Prefab;
			Entity prefab2 = prefabFromEntity[company].m_Prefab;
			float num = 0f;
			if (storage)
			{
				DynamicBuffer<ResourceAvailability> availabilities2 = availabilities[building.m_RoadEdge];
				float weight = EconomyUtils.GetWeight(process.m_Output.m_Resource, resourcePrefabs, ref resourceDatas);
				num += 50f * weight * (float)process.m_Output.m_Amount * NetUtils.GetAvailability(availabilities2, EconomyUtils.GetAvailableResourceSupply(process.m_Output.m_Resource), building.m_CurvePosition);
			}
			else
			{
				if (!workplaceDatas.HasComponent(prefab2))
				{
					return -1f;
				}
				num += 500f;
				DynamicBuffer<ResourceAvailability> availabilities3 = availabilities[building.m_RoadEdge];
				num += 10f * NetUtils.GetAvailability(availabilities3, AvailableResource.UneducatedCitizens, building.m_CurvePosition);
				if (process.m_Input1.m_Resource != Resource.NoResource)
				{
					float weight2 = EconomyUtils.GetWeight(process.m_Input1.m_Resource, resourcePrefabs, ref resourceDatas);
					num += 50f * weight2 * (float)process.m_Input1.m_Amount * NetUtils.GetAvailability(availabilities3, EconomyUtils.GetAvailableResourceSupply(process.m_Input1.m_Resource), building.m_CurvePosition);
				}
				if (process.m_Input2.m_Resource != Resource.NoResource)
				{
					float weight3 = EconomyUtils.GetWeight(process.m_Input2.m_Resource, resourcePrefabs, ref resourceDatas);
					num += 50f * weight3 * (float)process.m_Input2.m_Amount * NetUtils.GetAvailability(availabilities3, EconomyUtils.GetAvailableResourceSupply(process.m_Input2.m_Resource), building.m_CurvePosition);
				}
			}
			float landValue = landValues[building.m_RoadEdge].m_LandValue;
			float num2 = 1f;
			int num3 = 1;
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			if (spawnableDatas.TryGetComponent(property, ref spawnableBuildingData))
			{
				num3 = spawnableBuildingData.m_Level;
			}
			float num4 = propertyDatas[prefab].m_SpaceMultiplier * (1f + 0.5f * (float)num3);
			if (!storage)
			{
				num2 = num4 * process.m_MaxWorkersPerCell;
				if (EconomyUtils.GetWeight(process.m_Output.m_Resource, resourcePrefabs, ref resourceDatas) == 0f)
				{
					num2 *= 3f;
				}
			}
			num -= landValue / num2;
			return 250f + num;
		}
		return 0f;
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
	public IndustrialFindPropertySystem()
	{
	}
}
