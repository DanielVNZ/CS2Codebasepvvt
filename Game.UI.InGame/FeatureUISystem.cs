using System;
using Colossal.Serialization.Entities;
using Colossal.UI.Binding;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

public class FeatureUISystem : UISystemBase
{
	private const string kGroup = "feature";

	private PrefabSystem m_PrefabSystem;

	private PrefabUISystem m_PrefabUISystem;

	private EntityQuery m_UnlockedFeatureQuery;

	private EntityQuery m_UnlocksQuery;

	private RawValueBinding m_FeaturesBinding;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_00a0: Expected O, but got Unknown
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_PrefabUISystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabUISystem>();
		m_UnlockedFeatureQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.ReadOnly<FeatureData>(),
			ComponentType.ReadOnly<Locked>()
		});
		m_UnlocksQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Unlock>() });
		RawValueBinding val = new RawValueBinding("feature", "lockedFeatures", (Action<IJsonWriter>)BindLockedFeatures);
		RawValueBinding binding = val;
		m_FeaturesBinding = val;
		AddBinding((IBinding)(object)binding);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (PrefabUtils.HasUnlockedPrefab<FeatureData>(((ComponentSystemBase)this).EntityManager, m_UnlocksQuery))
		{
			m_FeaturesBinding.Update();
		}
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		m_FeaturesBinding.Update();
	}

	private void BindLockedFeatures(IJsonWriter writer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_UnlockedFeatureQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2));
		NativeArray<PrefabData> val2 = ((EntityQuery)(ref m_UnlockedFeatureQuery)).ToComponentDataArray<PrefabData>(AllocatorHandle.op_Implicit((Allocator)2));
		JsonWriterExtensions.ArrayBegin(writer, val2.Length);
		for (int i = 0; i < val.Length; i++)
		{
			Entity prefabEntity = val[i];
			PrefabData prefabData = val2[i];
			FeaturePrefab prefab = m_PrefabSystem.GetPrefab<FeaturePrefab>(prefabData);
			writer.TypeBegin("Game.UI.InGame.LockedFeature");
			writer.PropertyName("name");
			writer.Write(((Object)prefab).name);
			writer.PropertyName("requirements");
			m_PrefabUISystem.BindPrefabRequirements(writer, prefabEntity);
			writer.TypeEnd();
		}
		writer.ArrayEnd();
	}

	[Preserve]
	public FeatureUISystem()
	{
	}
}
