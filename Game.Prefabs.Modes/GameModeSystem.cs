using System.Collections.Generic;
using Colossal.Annotations;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs.Modes;

public class GameModeSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	private PrefabSystem m_PrefabSystem;

	private ModeSetting m_ModeSetting;

	private ModeSetting m_NextMode;

	private EntityQuery m_ModeSettingQuery;

	private EntityQuery m_ModeInfoQuery;

	[CanBeNull]
	public string overrideMode { get; set; }

	public ModeSetting modeSetting => m_ModeSetting;

	public string currentModeName { get; private set; }

	public List<GameModeInfo> GetGameModeInfo()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		List<GameModeInfo> list = new List<GameModeInfo>();
		NativeArray<Entity> val = ((EntityQuery)(ref m_ModeInfoQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		for (int i = 0; i < val.Length; i++)
		{
			Entity entity = val[i];
			GameModeInfo gameModeInfo = m_PrefabSystem.GetPrefab<GameModeInfoPrefab>(entity).GetGameModeInfo();
			list.Add(gameModeInfo);
		}
		val.Dispose();
		return list;
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_ModeSettingQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GameModeSettingData>() });
		m_ModeInfoQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<GameModeInfoData>() });
		currentModeName = string.Empty;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		if (overrideMode != null)
		{
			NativeArray<Entity> val = ((EntityQuery)(ref m_ModeSettingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int i = 0; i < val.Length; i++)
			{
				Entity entity = val[i];
				ModeSetting prefab = m_PrefabSystem.GetPrefab<ModeSetting>(entity);
				if (((Object)prefab.prefab).name == overrideMode)
				{
					m_NextMode = prefab;
					break;
				}
			}
			val.Dispose();
			overrideMode = null;
		}
		if ((Object)(object)m_ModeSetting != (Object)null)
		{
			COSystemBase.baseLog.Debug((object)("Clean up " + ((Object)m_ModeSetting.prefab).name));
			m_ModeSetting.RestoreDefaultData(((ComponentSystemBase)this).EntityManager, m_PrefabSystem);
			m_ModeSetting = null;
		}
		m_ModeSetting = m_NextMode;
		m_NextMode = null;
		if ((Object)(object)m_ModeSetting == (Object)null)
		{
			NativeArray<Entity> val2 = ((EntityQuery)(ref m_ModeSettingQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int j = 0; j < val2.Length; j++)
			{
				Entity entity2 = val2[j];
				ModeSetting prefab2 = m_PrefabSystem.GetPrefab<ModeSetting>(entity2);
				if (((Object)prefab2.prefab).name == "NormalMode")
				{
					m_ModeSetting = prefab2;
					break;
				}
			}
			val2.Dispose();
		}
		if ((Object)(object)m_ModeSetting != (Object)null)
		{
			COSystemBase.baseLog.Debug((object)("Apply " + ((Object)m_ModeSetting.prefab).name));
			m_ModeSetting.StoreDefaultData(((ComponentSystemBase)this).EntityManager, m_PrefabSystem);
			((SystemBase)this).Dependency = m_ModeSetting.ApplyMode(((ComponentSystemBase)this).EntityManager, m_PrefabSystem, ((SystemBase)this).Dependency);
			currentModeName = ((Object)m_ModeSetting.prefab).name;
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		if ((Object)(object)m_ModeSetting != (Object)null)
		{
			PrefabID prefabID = m_ModeSetting.prefab.GetPrefabID();
			((IWriter)writer/*cast due to .constrained prefix*/).Write<PrefabID>(prefabID);
		}
		else
		{
			((IWriter)writer/*cast due to .constrained prefix*/).Write<PrefabID>(default(PrefabID));
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		PrefabID id = default(PrefabID);
		((IReader)reader/*cast due to .constrained prefix*/).Read<PrefabID>(ref id);
		if (m_PrefabSystem.TryGetPrefab(id, out var prefab) && prefab.TryGetExactly<ModeSetting>(out var component))
		{
			m_NextMode = component;
		}
		else
		{
			m_NextMode = null;
		}
	}

	public void SetDefaults(Context context)
	{
	}

	[Preserve]
	public GameModeSystem()
	{
	}
}
