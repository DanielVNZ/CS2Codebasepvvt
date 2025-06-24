using System;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.PSI.Common;
using Game.Achievements;
using Game.Areas;
using Game.City;
using Game.Common;
using Game.Prefabs;
using Game.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

[CompilerGenerated]
public class MapTilePurchaseSystem : GameSystemBase, IMapTilePurchaseSystem
{
	private struct TypeHandle
	{
		[ReadOnly]
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<TilePurchaseCostFactor> __Game_Prefabs_TilePurchaseCostFactor_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<Native> __Game_Common_Native_RO_ComponentLookup;

		[ReadOnly]
		public ComponentLookup<MapTile> __Game_Areas_MapTile_RO_ComponentLookup;

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
			__Game_Prefabs_PrefabRef_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(true);
			__Game_Prefabs_TilePurchaseCostFactor_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<TilePurchaseCostFactor>(true);
			__Game_Common_Native_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Native>(true);
			__Game_Areas_MapTile_RO_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<MapTile>(true);
		}
	}

	private static readonly double kMapTileSizeModifier = 1.0 / Math.Pow(623.304347826087, 2.0);

	private static readonly double kResourceModifier = 8.0718994140625E-07;

	private static readonly int kAutoUnlockedTiles = 9;

	private static readonly double[] kMapFeatureBaselineModifiers = new double[8] { kMapTileSizeModifier, kMapTileSizeModifier, kResourceModifier, kResourceModifier, kResourceModifier, kResourceModifier, 1.0, kResourceModifier };

	private SelectionToolSystem m_SelectionToolSystem;

	private DefaultToolSystem m_DefaultToolSystem;

	private ToolSystem m_ToolSystem;

	private CitySystem m_CitySystem;

	private NaturalResourceSystem m_NaturalResourceSystem;

	private MapTileSystem m_MapTileSystem;

	private CityConfigurationSystem m_CityConfigurationSystem;

	private EntityQuery m_SelectionQuery;

	private EntityQuery m_OwnedTileQuery;

	private EntityQuery m_LockedMapTilesQuery;

	private EntityQuery m_UnlockedMilestoneQuery;

	private EntityQuery m_LockedMilestoneQuery;

	private EntityQuery m_EconomyParameterQuery;

	private NativeArray<float> m_FeatureAmounts;

	private float m_Cost;

	private float m_Upkeep;

	private TypeHandle __TypeHandle;

	public TilePurchaseErrorFlags status { get; private set; }

	public bool selecting
	{
		get
		{
			if (m_ToolSystem.activeTool == m_SelectionToolSystem)
			{
				return m_SelectionToolSystem.selectionType == SelectionType.MapTiles;
			}
			return false;
		}
		set
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (value)
			{
				m_SelectionToolSystem.selectionType = SelectionType.MapTiles;
				m_SelectionToolSystem.selectionOwner = Entity.Null;
				m_ToolSystem.activeTool = m_SelectionToolSystem;
			}
			else if (m_ToolSystem.activeTool == m_SelectionToolSystem)
			{
				m_ToolSystem.activeTool = m_DefaultToolSystem;
			}
		}
	}

	public int cost => Mathf.RoundToInt(m_Cost);

	public int upkeep => Mathf.RoundToInt(m_Upkeep);

	public float GetMapTileUpkeepCostMultiplier(int tileCount)
	{
		if (tileCount <= kAutoUnlockedTiles)
		{
			return 0f;
		}
		EconomyParameterData singleton = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>();
		return ((AnimationCurve1)(ref singleton.m_MapTileUpkeepCostMultiplier)).Evaluate((float)tileCount);
	}

	public bool GetMapTileUpkeepEnabled()
	{
		if (m_CityConfigurationSystem.unlockMapTiles)
		{
			return false;
		}
		EconomyParameterData singleton = ((EntityQuery)(ref m_EconomyParameterQuery)).GetSingleton<EconomyParameterData>();
		float num = 0f;
		for (int i = 0; i <= 100; i += 10)
		{
			num = ((AnimationCurve1)(ref singleton.m_MapTileUpkeepCostMultiplier)).Evaluate((float)i);
			if (num > 0f)
			{
				return true;
			}
		}
		return num != 0f;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SelectionToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SelectionToolSystem>();
		m_DefaultToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<DefaultToolSystem>();
		m_ToolSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ToolSystem>();
		m_CitySystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CitySystem>();
		m_NaturalResourceSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NaturalResourceSystem>();
		m_MapTileSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<MapTileSystem>();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_SelectionQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<SelectionElement>() });
		m_OwnedTileQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.Exclude<Native>()
		});
		m_LockedMapTilesQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadWrite<MapTile>(),
			ComponentType.ReadOnly<Native>(),
			ComponentType.ReadOnly<Area>()
		});
		m_UnlockedMilestoneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<MilestoneData>(),
			ComponentType.Exclude<Locked>()
		});
		m_LockedMilestoneQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<MilestoneData>(),
			ComponentType.ReadOnly<Locked>()
		});
		m_EconomyParameterQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<EconomyParameterData>() });
		m_FeatureAmounts = new NativeArray<float>(9, (Allocator)4, (NativeArrayOptions)1);
		((ComponentSystemBase)this).RequireForUpdate(m_EconomyParameterQuery);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		base.OnDestroy();
		m_FeatureAmounts.Dispose();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		CollectionUtils.Fill<float>(m_FeatureAmounts, 0f);
		m_Cost = 0f;
		m_Upkeep = 0f;
		int availableTiles = GetAvailableTiles();
		if (availableTiles == 0)
		{
			status = (IsMilestonesLeft() ? TilePurchaseErrorFlags.NoCurrentlyAvailable : TilePurchaseErrorFlags.NoAvailable);
			return;
		}
		if (!TryGetSelections(isReadOnly: true, out var selections) || selections.Length == 0)
		{
			status = TilePurchaseErrorFlags.NoSelection;
			return;
		}
		status = TilePurchaseErrorFlags.None;
		ComponentLookup<PrefabRef> componentLookup = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<TilePurchaseCostFactor> componentLookup2 = InternalCompilerInterface.GetComponentLookup<TilePurchaseCostFactor>(ref __TypeHandle.__Game_Prefabs_TilePurchaseCostFactor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<Native> componentLookup3 = InternalCompilerInterface.GetComponentLookup<Native>(ref __TypeHandle.__Game_Common_Native_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<MapTile> componentLookup4 = InternalCompilerInterface.GetComponentLookup<MapTile>(ref __TypeHandle.__Game_Areas_MapTile_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		NativeList<float> val = default(NativeList<float>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)2));
		int num = 0;
		int num2 = CalculateOwnedTiles();
		float num3 = CalculateOwnedTilesCost();
		float num4 = num3 * GetMapTileUpkeepCostMultiplier(num2);
		float num5 = num3;
		DynamicBuffer<MapFeatureElement> val2 = default(DynamicBuffer<MapFeatureElement>);
		DynamicBuffer<MapFeatureData> val3 = default(DynamicBuffer<MapFeatureData>);
		for (int i = 0; i < selections.Length; i++)
		{
			Entity entity = selections[i].m_Entity;
			if (!componentLookup4.HasComponent(entity) || !componentLookup3.HasComponent(entity))
			{
				continue;
			}
			num++;
			if (!EntitiesExtensions.TryGetBuffer<MapFeatureElement>(((ComponentSystemBase)this).EntityManager, entity, true, ref val2))
			{
				continue;
			}
			Entity prefab = componentLookup[entity].m_Prefab;
			float amount = componentLookup2[prefab].m_Amount;
			if (EntitiesExtensions.TryGetBuffer<MapFeatureData>(((ComponentSystemBase)this).EntityManager, prefab, true, ref val3))
			{
				float num6 = 0f;
				for (int j = 0; j < val2.Length; j++)
				{
					float amount2 = val2[j].m_Amount;
					ref NativeArray<float> reference = ref m_FeatureAmounts;
					int num7 = j;
					reference[num7] += amount2;
					double baselineModifier = GetBaselineModifier(j);
					num6 += (float)((double)amount2 * baselineModifier * 10.0 * (double)val3[j].m_Cost * (double)amount);
					num5 += (float)((double)amount2 * baselineModifier * 10.0 * (double)val3[j].m_Cost * (double)amount);
				}
				val.Add(ref num6);
			}
		}
		NativeSortExtension.Sort<float>(val);
		for (int k = 0; k < val.Length; k++)
		{
			m_Cost += val[val.Length - k - 1] * (float)(num2 + k);
		}
		val.Dispose();
		m_Upkeep = num5 * GetMapTileUpkeepCostMultiplier(num2 + num) - num4;
		if (num > 0 && num > availableTiles)
		{
			status |= TilePurchaseErrorFlags.InsufficientPermits;
		}
		if (cost > m_CitySystem.moneyAmount)
		{
			status |= TilePurchaseErrorFlags.InsufficientFunds;
		}
	}

	public int GetAvailableTiles()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		int num = CalculateOwnedTiles();
		int num2 = kAutoUnlockedTiles;
		NativeArray<MilestoneData> val = ((EntityQuery)(ref m_UnlockedMilestoneQuery)).ToComponentDataArray<MilestoneData>(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			Enumerator<MilestoneData> enumerator = val.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					num2 += enumerator.Current.m_MapTiles;
				}
			}
			finally
			{
				((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
			}
		}
		finally
		{
			val.Dispose();
		}
		return Mathf.Max(num2 - num, 0);
	}

	public bool IsMilestonesLeft()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<MilestoneData> val = ((EntityQuery)(ref m_LockedMilestoneQuery)).ToComponentDataArray<MilestoneData>(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			return val.Length != 0;
		}
		finally
		{
			val.Dispose();
		}
	}

	private double GetBaselineModifier(int mapFeature)
	{
		if (mapFeature >= 0 && mapFeature < kMapFeatureBaselineModifiers.Length)
		{
			return kMapFeatureBaselineModifiers[mapFeature];
		}
		return 1.0;
	}

	public void UnlockMapTiles()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntityQuery)(ref m_LockedMapTilesQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val = ((EntityQuery)(ref m_LockedMapTilesQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
			for (int i = 0; i < val.Length; i++)
			{
				Entity area = val[i];
				UnlockTile(((ComponentSystemBase)this).EntityManager, area);
			}
			val.Dispose();
		}
	}

	public void PurchaseSelection()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		UpdateStatus();
		if (status != TilePurchaseErrorFlags.None)
		{
			return;
		}
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		PlayerMoney componentData = ((EntityManager)(ref entityManager)).GetComponentData<PlayerMoney>(m_CitySystem.City);
		componentData.Subtract(cost);
		entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).SetComponentData<PlayerMoney>(m_CitySystem.City, componentData);
		if (!TryGetSelections(isReadOnly: false, out var selections))
		{
			return;
		}
		NativeArray<SelectionElement> val = selections.ToNativeArray(AllocatorHandle.op_Implicit((Allocator)2));
		try
		{
			selections.Clear();
			for (int i = 0; i < val.Length; i++)
			{
				Entity entity = val[i].m_Entity;
				UnlockTile(((ComponentSystemBase)this).EntityManager, entity);
			}
			PlatformManager.instance.IndicateAchievementProgress((AchievementId[])(object)new AchievementId[2]
			{
				Game.Achievements.Achievements.TheExplorer,
				Game.Achievements.Achievements.EverythingTheLightTouches
			}, CalculateOwnedTiles(), (IndicateType)1);
		}
		finally
		{
			val.Dispose();
		}
	}

	public static void UnlockTile(EntityManager entityManager, Entity area)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityManager)(ref entityManager)).HasComponent<Native>(area))
		{
			((EntityManager)(ref entityManager)).RemoveComponent<Native>(area);
			((EntityManager)(ref entityManager)).AddComponentData<Updated>(area, default(Updated));
		}
	}

	private bool TryGetSelections(bool isReadOnly, out DynamicBuffer<SelectionElement> selections)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (selecting && !((EntityQuery)(ref m_SelectionQuery)).IsEmptyIgnoreFilter)
		{
			Entity singletonEntity = ((EntityQuery)(ref m_SelectionQuery)).GetSingletonEntity();
			if (EntitiesExtensions.TryGetBuffer<SelectionElement>(((ComponentSystemBase)this).EntityManager, singletonEntity, isReadOnly, ref selections))
			{
				return true;
			}
		}
		selections = default(DynamicBuffer<SelectionElement>);
		return false;
	}

	public int GetSelectedTileCount()
	{
		if (TryGetSelections(isReadOnly: true, out var selections))
		{
			return selections.Length;
		}
		return 0;
	}

	private int CalculateOwnedTiles()
	{
		return ((EntityQuery)(ref m_OwnedTileQuery)).CalculateEntityCountWithoutFiltering();
	}

	private float CalculateOwnedTilesCost()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		ComponentLookup<PrefabRef> componentLookup = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		ComponentLookup<TilePurchaseCostFactor> componentLookup2 = InternalCompilerInterface.GetComponentLookup<TilePurchaseCostFactor>(ref __TypeHandle.__Game_Prefabs_TilePurchaseCostFactor_RO_ComponentLookup, ref ((SystemBase)this).CheckedStateRef);
		NativeList<Entity> startTiles = m_MapTileSystem.GetStartTiles();
		NativeArray<Entity> val = default(NativeArray<Entity>);
		if (TryGetSelections(isReadOnly: true, out var selections) && selections.Length != 0)
		{
			val._002Ector(selections.Length, (Allocator)3, (NativeArrayOptions)1);
			for (int i = 0; i < selections.Length; i++)
			{
				val[i] = selections[i].m_Entity;
			}
		}
		else
		{
			val = ((EntityQuery)(ref m_OwnedTileQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		}
		float num = 0f;
		DynamicBuffer<MapFeatureElement> val2 = default(DynamicBuffer<MapFeatureElement>);
		DynamicBuffer<MapFeatureData> val3 = default(DynamicBuffer<MapFeatureData>);
		for (int j = 0; j < val.Length; j++)
		{
			if (NativeListExtensions.Contains<Entity, Entity>(startTiles, val[j]) || !EntitiesExtensions.TryGetBuffer<MapFeatureElement>(((ComponentSystemBase)this).EntityManager, val[j], true, ref val2))
			{
				continue;
			}
			Entity prefab = componentLookup[val[j]].m_Prefab;
			float amount = componentLookup2[prefab].m_Amount;
			if (EntitiesExtensions.TryGetBuffer<MapFeatureData>(((ComponentSystemBase)this).EntityManager, prefab, true, ref val3))
			{
				for (int k = 0; k < val2.Length; k++)
				{
					float amount2 = val2[k].m_Amount;
					ref NativeArray<float> reference = ref m_FeatureAmounts;
					int num2 = k;
					reference[num2] += amount2;
					double baselineModifier = GetBaselineModifier(k);
					num += (float)((double)amount2 * baselineModifier * 10.0 * (double)val3[k].m_Cost * (double)amount);
				}
			}
		}
		val.Dispose();
		return num;
	}

	public int CalculateOwnedTilesUpkeep()
	{
		return Mathf.RoundToInt(CalculateOwnedTilesCost() * GetMapTileUpkeepCostMultiplier(CalculateOwnedTiles()));
	}

	public float GetFeatureAmount(MapFeature feature)
	{
		float num = m_FeatureAmounts[(int)feature];
		if (feature != MapFeature.FertileLand)
		{
			return num;
		}
		return m_NaturalResourceSystem.ResourceAmountToArea(num);
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
	public MapTilePurchaseSystem()
	{
	}
}
