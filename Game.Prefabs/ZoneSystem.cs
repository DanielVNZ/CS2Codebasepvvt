using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Zones;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Prefabs;

[CompilerGenerated]
public class ZoneSystem : GameSystemBase
{
	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<Deleted> __Game_Common_Deleted_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		public ComponentTypeHandle<ZoneData> __Game_Prefabs_ZoneData_RW_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<ZoneData> __Game_Prefabs_ZoneData_RO_ComponentTypeHandle;

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
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Common_Deleted_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<Deleted>(true);
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_ZoneData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ZoneData>(false);
			__Game_Prefabs_ZoneData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<ZoneData>(true);
		}
	}

	private EntityQuery m_CreatedQuery;

	private EntityQuery m_PrefabQuery;

	private PrefabSystem m_PrefabSystem;

	private NativeList<Entity> m_ZonePrefabs;

	private int m_ZoneFillColors;

	private int m_ZoneEdgeColors;

	private bool m_IsEditorMode;

	private bool m_UpdateColors;

	private bool m_RemovedZones;

	private Vector4[] m_FillColorArray;

	private Vector4[] m_EdgeColorArray;

	private JobHandle m_PrefabsReaders;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadWrite<ZoneData>(),
			ComponentType.ReadOnly<PrefabData>()
		};
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Created>(),
			ComponentType.ReadOnly<Deleted>()
		};
		array[0] = val;
		m_CreatedQuery = ((ComponentSystemBase)this).GetEntityQuery((EntityQueryDesc[])(object)array);
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ZoneData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Deleted>()
		});
		m_ZonePrefabs = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
		m_FillColorArray = (Vector4[])(object)new Vector4[1023];
		m_EdgeColorArray = (Vector4[])(object)new Vector4[1023];
		m_ZoneFillColors = Shader.PropertyToID("colossal_ZoneFillColors");
		m_ZoneEdgeColors = Shader.PropertyToID("colossal_ZoneEdgeColors");
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_PrefabsReaders)).Complete();
		m_ZonePrefabs.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_PrefabsReaders)).Complete();
		m_PrefabsReaders = default(JobHandle);
		if (!((EntityQuery)(ref m_CreatedQuery)).IsEmptyIgnoreFilter)
		{
			InitializeZonePrefabs();
		}
		if (m_UpdateColors)
		{
			UpdateZoneColors();
		}
	}

	protected override void OnGamePreload(Purpose purpose, GameMode mode)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGamePreload(purpose, mode);
		if (mode.IsEditor() != m_IsEditorMode)
		{
			((JobHandle)(ref m_PrefabsReaders)).Complete();
			m_IsEditorMode = !m_IsEditorMode;
			m_UpdateColors = m_ZonePrefabs.Length != 0;
		}
	}

	private void InitializeZonePrefabs()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_CreatedQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<Deleted> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<Deleted>(ref __TypeHandle.__Game_Common_Deleted_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ZoneData> componentTypeHandle3 = InternalCompilerInterface.GetComponentTypeHandle<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle2);
			NativeArray<ZoneData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ZoneData>(ref componentTypeHandle3);
			if (((ArchetypeChunk)(ref val2)).Has<Deleted>(ref componentTypeHandle))
			{
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					Entity val3 = nativeArray[j];
					ZoneData zoneData = nativeArray3[j];
					if (zoneData.m_ZoneType.m_Index < m_ZonePrefabs.Length && m_ZonePrefabs[(int)zoneData.m_ZoneType.m_Index] == val3)
					{
						m_ZonePrefabs[(int)zoneData.m_ZoneType.m_Index] = Entity.Null;
						m_RemovedZones = true;
					}
				}
				continue;
			}
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Entity val4 = nativeArray[k];
				ZonePrefab prefab = m_PrefabSystem.GetPrefab<ZonePrefab>(nativeArray2[k]);
				ZoneData zoneData2 = nativeArray3[k];
				zoneData2.m_AreaType = prefab.m_AreaType;
				if (prefab.m_AreaType != AreaType.None)
				{
					zoneData2.m_ZoneType = new ZoneType
					{
						m_Index = (ushort)GetNextIndex()
					};
					zoneData2.m_MinOddHeight = ushort.MaxValue;
					zoneData2.m_MinEvenHeight = ushort.MaxValue;
					zoneData2.m_MaxHeight = 0;
				}
				else
				{
					zoneData2.m_ZoneFlags |= ZoneFlags.SupportNarrow;
					zoneData2.m_MinOddHeight = 1;
					zoneData2.m_MinEvenHeight = 1;
					zoneData2.m_MaxHeight = 1;
				}
				if (zoneData2.m_ZoneType.m_Index < m_ZonePrefabs.Length)
				{
					m_ZonePrefabs[(int)zoneData2.m_ZoneType.m_Index] = val4;
				}
				else
				{
					while (zoneData2.m_ZoneType.m_Index > m_ZonePrefabs.Length)
					{
						ref NativeList<Entity> reference = ref m_ZonePrefabs;
						Entity val5 = Entity.Null;
						reference.Add(ref val5);
					}
					m_ZonePrefabs.Add(ref val4);
				}
				nativeArray3[k] = zoneData2;
				UpdateZoneColors(prefab, zoneData2);
			}
		}
		val.Dispose();
		Shader.SetGlobalVectorArray(m_ZoneFillColors, m_FillColorArray);
		Shader.SetGlobalVectorArray(m_ZoneEdgeColors, m_EdgeColorArray);
	}

	private int GetNextIndex()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (m_RemovedZones)
		{
			for (int i = 1; i < m_ZonePrefabs.Length; i++)
			{
				if (m_ZonePrefabs[i] == Entity.Null)
				{
					return i;
				}
			}
			m_RemovedZones = false;
		}
		return math.max(1, m_ZonePrefabs.Length);
	}

	private void UpdateZoneColors()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		m_UpdateColors = false;
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
		ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<ZoneData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<ZoneData>(ref __TypeHandle.__Game_Prefabs_ZoneData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
			NativeArray<ZoneData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<ZoneData>(ref componentTypeHandle2);
			for (int j = 0; j < nativeArray2.Length; j++)
			{
				ZonePrefab prefab = m_PrefabSystem.GetPrefab<ZonePrefab>(nativeArray[j]);
				ZoneData zoneData = nativeArray2[j];
				UpdateZoneColors(prefab, zoneData);
			}
		}
		val.Dispose();
		Shader.SetGlobalVectorArray(m_ZoneFillColors, m_FillColorArray);
		Shader.SetGlobalVectorArray(m_ZoneEdgeColors, m_EdgeColorArray);
	}

	private void UpdateZoneColors(ZonePrefab zonePrefab, ZoneData zoneData)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		Color color = zonePrefab.m_Color;
		Color edge = zonePrefab.m_Edge;
		GetZoneColors(color, out var occupied, out var selected);
		GetZoneColors(edge, out var occupied2, out var selected2);
		int colorIndex = ZoneUtils.GetColorIndex(CellFlags.Visible, zoneData.m_ZoneType);
		int colorIndex2 = ZoneUtils.GetColorIndex(CellFlags.Visible | CellFlags.Occupied, zoneData.m_ZoneType);
		int colorIndex3 = ZoneUtils.GetColorIndex(CellFlags.Visible | CellFlags.Selected, zoneData.m_ZoneType);
		if (m_IsEditorMode)
		{
			color.a = 0f;
			edge.a *= 0.5f;
			occupied.a = 0f;
			occupied2.a = 0f;
			selected.a = 0f;
		}
		m_FillColorArray[colorIndex] = Color.op_Implicit(color);
		m_EdgeColorArray[colorIndex] = Color.op_Implicit(edge);
		m_FillColorArray[colorIndex2] = Color.op_Implicit(occupied);
		m_EdgeColorArray[colorIndex2] = Color.op_Implicit(occupied2);
		m_FillColorArray[colorIndex3] = Color.op_Implicit(selected);
		m_EdgeColorArray[colorIndex3] = Color.op_Implicit(selected2);
	}

	private void GetZoneColors(Color color, out Color occupied, out Color selected)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		float num = default(float);
		float num2 = default(float);
		float num3 = default(float);
		Color.RGBToHSV(color, ref num, ref num2, ref num3);
		occupied = Color.HSVToRGB(num, num2 * 0.75f, num3);
		occupied.a = color.a * 0.5f;
		selected = Color.HSVToRGB(num, math.min(1f, num2 * 1.25f), num3);
		selected.a = math.min(color.a * 1.5f, math.lerp(color.a, 1f, 0.5f));
	}

	public ZonePrefabs GetPrefabs()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return new ZonePrefabs(m_ZonePrefabs.AsArray());
	}

	public void AddPrefabsReader(JobHandle handle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_PrefabsReaders = JobHandle.CombineDependencies(m_PrefabsReaders, handle);
	}

	public Entity GetPrefab(ZoneType zoneType)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (zoneType.m_Index >= m_ZonePrefabs.Length)
		{
			return Entity.Null;
		}
		return m_ZonePrefabs[(int)zoneType.m_Index];
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
	public ZoneSystem()
	{
	}
}
