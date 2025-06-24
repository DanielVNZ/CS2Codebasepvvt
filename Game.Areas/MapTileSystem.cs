using System;
using System.Runtime.CompilerServices;
using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Prefabs;
using Game.Serialization;
using Game.Tools;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Areas;

[CompilerGenerated]
public class MapTileSystem : GameSystemBase, IDefaultSerializable, ISerializable, IPostDeserialize
{
	[BurstCompile]
	private struct GenerateMapTilesJob : IJobParallelFor
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<Entity> m_Entities;

		[ReadOnly]
		public Entity m_Prefab;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<PrefabRef> m_PrefabRefData;

		[NativeDisableParallelForRestriction]
		public ComponentLookup<Area> m_AreaData;

		[NativeDisableParallelForRestriction]
		public BufferLookup<Node> m_NodeData;

		public void Execute(int index)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entities[index];
			m_PrefabRefData[val] = new PrefabRef(m_Prefab);
			m_AreaData[val] = new Area(AreaFlags.Complete);
			DynamicBuffer<Node> val2 = m_NodeData[val];
			int2 val3 = default(int2);
			((int2)(ref val3))._002Ector(index % 23, index / 23);
			float2 val4 = new float2(23f, 23f) * 311.65216f;
			Bounds2 val5 = default(Bounds2);
			val5.min = float2.op_Implicit(val3) * 623.3043f - val4;
			val5.max = float2.op_Implicit(val3 + 1) * 623.3043f - val4;
			val2.ResizeUninitialized(4);
			val2[0] = new Node(new float3(val5.min.x, 0f, val5.min.y), float.MinValue);
			val2[1] = new Node(new float3(val5.min.x, 0f, val5.max.y), float.MinValue);
			val2[2] = new Node(new float3(val5.max.x, 0f, val5.max.y), float.MinValue);
			val2[3] = new Node(new float3(val5.max.x, 0f, val5.min.y), float.MinValue);
		}
	}

	private struct TypeHandle
	{
		public ComponentLookup<PrefabRef> __Game_Prefabs_PrefabRef_RW_ComponentLookup;

		public ComponentLookup<Area> __Game_Areas_Area_RW_ComponentLookup;

		public BufferLookup<Node> __Game_Areas_Node_RW_BufferLookup;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabRef_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<PrefabRef>(false);
			__Game_Areas_Area_RW_ComponentLookup = ((SystemState)(ref state)).GetComponentLookup<Area>(false);
			__Game_Areas_Node_RW_BufferLookup = ((SystemState)(ref state)).GetBufferLookup<Node>(false);
		}
	}

	private const int LEGACY_GRID_WIDTH = 23;

	private const int LEGACY_GRID_LENGTH = 23;

	private const float LEGACY_CELL_SIZE = 623.3043f;

	private EntityQuery m_PrefabQuery;

	private EntityQuery m_MapTileQuery;

	private EntityQuery m_DeletedMapTileQuery;

	private NativeList<Entity> m_StartTiles;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<MapTileData>(),
			ComponentType.ReadOnly<AreaData>(),
			ComponentType.ReadOnly<PrefabData>(),
			ComponentType.Exclude<Locked>()
		});
		m_MapTileQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.Exclude<Temp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_DeletedMapTileQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<MapTile>(),
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.Exclude<Temp>()
		});
		m_StartTiles = new NativeList<Entity>(AllocatorHandle.op_Implicit((Allocator)4));
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_StartTiles.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (((EntityQuery)(ref m_DeletedMapTileQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		JobHandle dependency = ((SystemBase)this).Dependency;
		((JobHandle)(ref dependency)).Complete();
		Enumerator<Entity> enumerator = ((EntityQuery)(ref m_DeletedMapTileQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)2)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Entity current = enumerator.Current;
				int num = NativeListExtensions.IndexOf<Entity, Entity>(m_StartTiles, current);
				if (num >= 0)
				{
					m_StartTiles.RemoveAtSwapBack(num);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
	}

	public void PostDeserialize(Context context)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((Context)(ref context)).purpose == 1)
		{
			if (((Context)(ref context)).version >= Version.editorMapTiles)
			{
				for (int i = 0; i < m_StartTiles.Length; i++)
				{
					if (m_StartTiles[i] == Entity.Null)
					{
						m_StartTiles.RemoveAtSwapBack(i);
					}
				}
				if (m_StartTiles.Length != 0)
				{
					EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
					((EntityManager)(ref entityManager)).RemoveComponent<Native>(m_StartTiles.AsArray());
				}
			}
			else
			{
				LegacyGenerateMapTiles(editorMode: false);
			}
		}
		else if ((int)((Context)(ref context)).purpose == 4)
		{
			LegacyGenerateMapTiles(editorMode: true);
		}
	}

	public NativeList<Entity> GetStartTiles()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return m_StartTiles;
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		int length = m_StartTiles.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		for (int i = 0; i < m_StartTiles.Length; i++)
		{
			Entity val = m_StartTiles[i];
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		m_StartTiles.ResizeUninitialized(num);
		Entity val = default(Entity);
		for (int i = 0; i < num; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
			m_StartTiles[i] = val;
		}
	}

	public void SetDefaults(Context context)
	{
		m_StartTiles.Clear();
	}

	private void LegacyGenerateMapTiles(bool editorMode)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager;
		if (!((EntityQuery)(ref m_MapTileQuery)).IsEmptyIgnoreFilter)
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).DestroyEntity(m_MapTileQuery);
		}
		m_StartTiles.Clear();
		NativeArray<Entity> val = ((EntityQuery)(ref m_PrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			Entity val2 = val[0];
			entityManager = ((ComponentSystemBase)this).EntityManager;
			AreaData componentData = ((EntityManager)(ref entityManager)).GetComponentData<AreaData>(val2);
			int num = 529;
			entityManager = ((ComponentSystemBase)this).EntityManager;
			NativeArray<Entity> val3 = ((EntityManager)(ref entityManager)).CreateEntity(componentData.m_Archetype, num, AllocatorHandle.op_Implicit((Allocator)3));
			if (!editorMode)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).AddComponent<Native>(val3);
			}
			AddOwner(new int2(10, 10), val3);
			AddOwner(new int2(11, 10), val3);
			AddOwner(new int2(12, 10), val3);
			AddOwner(new int2(10, 11), val3);
			AddOwner(new int2(11, 11), val3);
			AddOwner(new int2(12, 11), val3);
			AddOwner(new int2(10, 12), val3);
			AddOwner(new int2(11, 12), val3);
			AddOwner(new int2(12, 12), val3);
			GenerateMapTilesJob obj = new GenerateMapTilesJob
			{
				m_Entities = val3,
				m_Prefab = val2,
				m_PrefabRefData = InternalCompilerInterface.GetComponentLookup<PrefabRef>(ref __TypeHandle.__Game_Prefabs_PrefabRef_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_AreaData = InternalCompilerInterface.GetComponentLookup<Area>(ref __TypeHandle.__Game_Areas_Area_RW_ComponentLookup, ref ((SystemBase)this).CheckedStateRef),
				m_NodeData = InternalCompilerInterface.GetBufferLookup<Node>(ref __TypeHandle.__Game_Areas_Node_RW_BufferLookup, ref ((SystemBase)this).CheckedStateRef)
			};
			int length = val3.Length;
			JobHandle val4 = default(JobHandle);
			val4 = IJobParallelForExtensions.Schedule<GenerateMapTilesJob>(obj, length, 4, val4);
			((JobHandle)(ref val4)).Complete();
		}
		finally
		{
			val.Dispose();
		}
	}

	private void AddOwner(int2 tile, NativeArray<Entity> entities)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		int num = tile.y * 23 + tile.x;
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		((EntityManager)(ref entityManager)).RemoveComponent<Native>(entities[num]);
		ref NativeList<Entity> reference = ref m_StartTiles;
		Entity val = entities[num];
		reference.Add(ref val);
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
	public MapTileSystem()
	{
	}
}
