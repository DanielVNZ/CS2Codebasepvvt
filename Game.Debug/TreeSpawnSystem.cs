using System;
using Colossal.Serialization.Entities;
using Game.Objects;
using Game.Prefabs;
using Game.Serialization;
using Game.Simulation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Debug;

public class TreeSpawnSystem : GameSystemBase
{
	private LoadGameSystem m_LoadGameSystem;

	private TerrainSystem m_TerrainSystem;

	private EntityQuery m_Prefabs;

	private EntityQuery m_TreeQuery;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_LoadGameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_Prefabs = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<TreeData>(),
			ComponentType.Exclude<PlaceholderObjectElement>()
		});
		m_TreeQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Tree>() });
		((ComponentSystemBase)this).RequireForUpdate(m_Prefabs);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		Context context = m_LoadGameSystem.context;
		if ((int)((Context)(ref context)).purpose != 1 || !((EntityQuery)(ref m_TreeQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		Random val = default(Random);
		((Random)(ref val))._002Ector((uint)DateTime.Now.Ticks);
		TerrainHeightData data = m_TerrainSystem.GetHeightData(waitForPending: true);
		NativeArray<Entity> val2 = ((EntityQuery)(ref m_Prefabs)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			Transform transform = default(Transform);
			Tree tree = default(Tree);
			for (int i = 0; i < 5000; i++)
			{
				Entity val3 = val2[((Random)(ref val)).NextInt(val2.Length)];
				EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
				ObjectData componentData = ((EntityManager)(ref entityManager)).GetComponentData<ObjectData>(val3);
				float2 val4 = ((Random)(ref val)).NextFloat2(float2.op_Implicit(-1000f), float2.op_Implicit(1000f));
				transform.m_Rotation = quaternion.RotateY(((Random)(ref val)).NextFloat((float)Math.PI * 2f));
				transform.m_Position = new float3(val4.x, 0f, val4.y);
				transform.m_Position.y = TerrainUtils.SampleHeight(ref data, transform.m_Position);
				switch (((Random)(ref val)).NextInt(13))
				{
				case 2:
				case 3:
					tree.m_State = TreeState.Teen;
					break;
				case 4:
				case 5:
				case 6:
				case 7:
					tree.m_State = TreeState.Adult;
					break;
				case 8:
				case 9:
				case 10:
				case 11:
					tree.m_State = TreeState.Elderly;
					break;
				case 12:
					tree.m_State = TreeState.Dead;
					break;
				default:
					tree.m_State = (TreeState)0;
					break;
				}
				tree.m_Growth = (byte)((Random)(ref val)).NextInt(256);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				Entity val5 = ((EntityManager)(ref entityManager)).CreateEntity(componentData.m_Archetype);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<PrefabRef>(val5, new PrefabRef(val3));
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<Transform>(val5, transform);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<Tree>(val5, tree);
			}
		}
		finally
		{
			val2.Dispose();
		}
	}

	[Preserve]
	public TreeSpawnSystem()
	{
	}
}
