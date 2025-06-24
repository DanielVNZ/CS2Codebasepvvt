using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Prefabs;
using Game.Rendering;
using Game.Rendering.Utilities;
using Game.Serialization;
using Game.Simulation;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;
using UnityEngine.VFX;

namespace Game.Effects;

[CompilerGenerated]
public class VFXSystem : GameSystemBase, IPreDeserialize
{
	private static class VFXIDs
	{
		public static readonly int WindTexture = Shader.PropertyToID("WindTexture");

		public static readonly int MapOffsetScale = Shader.PropertyToID("MapOffsetScale");

		public static readonly int Count = Shader.PropertyToID("Count");

		public static readonly int InstanceData = Shader.PropertyToID("InstanceData");
	}

	private struct EffectInfo
	{
		public VisualEffect m_VisualEffect;

		public Texture2D m_Texture;

		public NativeArray<int> m_Instances;

		public NativeParallelHashMap<int, int> m_Indices;

		public int m_LastCount;

		public bool m_NeedApply;
	}

	[BurstCompile]
	private struct VFXTextureUpdateJob : IJob
	{
		public NativeArray<float4> m_TextureData;

		[ReadOnly]
		public NativeArray<int> m_Instances;

		[ReadOnly]
		public NativeList<EnabledEffectData> m_EnabledData;

		public int m_Count;

		public int m_TextureWidth;

		public void Execute()
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < m_Count; i++)
			{
				int num = m_Instances[i];
				if (num == -1)
				{
					CollectionUtils.ElementAt<float4>(m_TextureData, i).w = 0f;
					continue;
				}
				EnabledEffectData enabledEffectData = m_EnabledData[num];
				Quaternion val = quaternion.op_Implicit(enabledEffectData.m_Rotation);
				m_TextureData[i] = new float4(enabledEffectData.m_Position, enabledEffectData.m_Intensity);
				m_TextureData[i + m_TextureWidth] = new float4(float3.op_Implicit((float)Math.PI / 180f * ((Quaternion)(ref val)).eulerAngles), 0f);
				m_TextureData[i + 2 * m_TextureWidth] = new float4(enabledEffectData.m_Scale, 0f);
			}
		}
	}

	private Queue<NativeQueue<VFXUpdateInfo>> m_SourceUpdateQueue;

	private JobHandle m_SourceUpdateWriter;

	private EntityQuery m_VFXPrefabQuery;

	private PrefabSystem m_PrefabSystem;

	private bool m_Initialized;

	private EffectInfo[] m_Effects;

	private JobHandle m_TextureUpdate;

	private WindTextureSystem m_WindTextureSystem;

	private TerrainSystem m_TerrainSystem;

	private EffectControlSystem m_EffectControlSystem;

	private RenderingSystem m_RenderingSystem;

	public NativeQueue<VFXUpdateInfo> GetSourceUpdateData()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		NativeQueue<VFXUpdateInfo> val = default(NativeQueue<VFXUpdateInfo>);
		val._002Ector(AllocatorHandle.op_Implicit((Allocator)3));
		m_SourceUpdateQueue.Enqueue(val);
		return val;
	}

	public void AddSourceUpdateWriter(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_SourceUpdateWriter = JobHandle.CombineDependencies(m_SourceUpdateWriter, jobHandle);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_SourceUpdateQueue = new Queue<NativeQueue<VFXUpdateInfo>>();
		m_VFXPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<VFXData>() });
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_WindTextureSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WindTextureSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_EffectControlSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectControlSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
	}

	private bool Initialize()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Initialized && !((EntityQuery)(ref m_VFXPrefabQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val = ((EntityQuery)(ref m_VFXPrefabQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<VFXData> val2 = ((EntityQuery)(ref m_VFXPrefabQuery)).ToComponentDataArray<VFXData>(AllocatorHandle.op_Implicit((Allocator)3));
			m_Effects = new EffectInfo[val.Length];
			for (int i = 0; i < val.Length; i++)
			{
				EntityManager entityManager = ((ComponentSystemBase)this).World.EntityManager;
				((EntityManager)(ref entityManager)).GetComponentData<VFXData>(val[i]);
				EffectPrefab prefab = m_PrefabSystem.GetPrefab<EffectPrefab>(val[i]);
				VFX component = prefab.GetComponent<VFX>();
				VisualEffect val3 = new GameObject("VFX " + ((Object)prefab).name).AddComponent<VisualEffect>();
				val3.visualEffectAsset = component.m_Effect;
				val3.SetCheckedInt(VFXIDs.Count, 0);
				m_Effects[i].m_VisualEffect = val3;
				VFXData vFXData = val2[i];
				vFXData.m_MaxCount = component.m_MaxCount;
				vFXData.m_Index = i;
				entityManager = ((ComponentSystemBase)this).World.EntityManager;
				((EntityManager)(ref entityManager)).SetComponentData<VFXData>(val[i], vFXData);
				Texture2D val4 = new Texture2D(component.m_MaxCount, 3, (GraphicsFormat)52, 1, (TextureCreationFlags)0)
				{
					name = "VFXTexture " + ((Object)prefab).name,
					hideFlags = (HideFlags)61
				};
				m_Effects[i].m_Texture = val4;
				val3.SetCheckedTexture(VFXIDs.InstanceData, (Texture)(object)val4);
				m_Effects[i].m_Instances = new NativeArray<int>(vFXData.m_MaxCount, (Allocator)4, (NativeArrayOptions)1);
				m_Effects[i].m_Indices = new NativeParallelHashMap<int, int>(vFXData.m_MaxCount, AllocatorHandle.op_Implicit((Allocator)4));
			}
			val2.Dispose();
			val.Dispose();
			m_Initialized = true;
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		if (m_Initialized && m_Effects != null)
		{
			for (int i = 0; i < m_Effects.Length; i++)
			{
				Object.Destroy((Object)(object)m_Effects[i].m_Texture);
				if (m_Effects[i].m_Instances.IsCreated)
				{
					m_Effects[i].m_Instances.Dispose();
				}
				if (m_Effects[i].m_Indices.IsCreated)
				{
					m_Effects[i].m_Indices.Dispose();
				}
			}
		}
		ClearQueue();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_060a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		if (!m_Initialized && !Initialize())
		{
			ClearQueue();
			return;
		}
		((JobHandle)(ref m_TextureUpdate)).Complete();
		((JobHandle)(ref m_SourceUpdateWriter)).Complete();
		JobHandle dependencies;
		NativeList<EnabledEffectData> enabledData = m_EffectControlSystem.GetEnabledData(readOnly: true, out dependencies);
		NativeQueue<VFXUpdateInfo> val = default(NativeQueue<VFXUpdateInfo>);
		VFXUpdateInfo vFXUpdateInfo = default(VFXUpdateInfo);
		VFXData vFXData = default(VFXData);
		int num = default(int);
		while (m_SourceUpdateQueue.TryDequeue(ref val))
		{
			if (!val.IsEmpty())
			{
				((JobHandle)(ref dependencies)).Complete();
				while (val.TryDequeue(ref vFXUpdateInfo))
				{
					EnabledEffectData enabledEffectData = enabledData[vFXUpdateInfo.m_EnabledIndex.x];
					if (!EntitiesExtensions.TryGetComponent<VFXData>(((ComponentSystemBase)this).EntityManager, enabledEffectData.m_Prefab, ref vFXData))
					{
						continue;
					}
					int index = vFXData.m_Index;
					switch (vFXUpdateInfo.m_Type)
					{
					case VFXUpdateType.Add:
						if (!m_Effects[index].m_Indices.ContainsKey(vFXUpdateInfo.m_EnabledIndex.x) && index >= 0 && m_Effects[index].m_Indices.Count() < m_Effects[index].m_Instances.Length)
						{
							int num2 = m_Effects[index].m_Indices.Count();
							m_Effects[index].m_Instances[num2] = vFXUpdateInfo.m_EnabledIndex.x;
							m_Effects[index].m_Indices[vFXUpdateInfo.m_EnabledIndex.x] = num2;
							if ((Object)(object)m_Effects[index].m_VisualEffect != (Object)null)
							{
								m_Effects[index].m_VisualEffect.SetCheckedInt(VFXIDs.Count, m_Effects[index].m_Indices.Count());
							}
						}
						break;
					case VFXUpdateType.Remove:
						if (m_Effects[index].m_Indices.ContainsKey(vFXUpdateInfo.m_EnabledIndex.x))
						{
							int num3 = m_Effects[index].m_Instances[m_Effects[index].m_Indices.Count() - 1];
							int num4 = m_Effects[index].m_Indices[vFXUpdateInfo.m_EnabledIndex.x];
							if (vFXUpdateInfo.m_EnabledIndex.x != num3)
							{
								m_Effects[index].m_Instances[num4] = num3;
								m_Effects[index].m_Indices[num3] = num4;
							}
							m_Effects[index].m_Instances[m_Effects[index].m_Indices.Count() - 1] = -1;
							m_Effects[index].m_Indices.Remove(vFXUpdateInfo.m_EnabledIndex.x);
							if ((Object)(object)m_Effects[index].m_VisualEffect != (Object)null)
							{
								m_Effects[index].m_VisualEffect.SetCheckedInt(VFXIDs.Count, m_Effects[index].m_Indices.Count());
							}
						}
						break;
					case VFXUpdateType.MoveIndex:
						if (m_Effects[index].m_Indices.TryGetValue(vFXUpdateInfo.m_EnabledIndex.y, ref num))
						{
							m_Effects[index].m_Indices.Remove(vFXUpdateInfo.m_EnabledIndex.y);
							m_Effects[index].m_Indices[vFXUpdateInfo.m_EnabledIndex.x] = num;
							m_Effects[index].m_Instances[num] = vFXUpdateInfo.m_EnabledIndex.x;
						}
						break;
					}
				}
			}
			val.Dispose();
		}
		float frameDelta = m_RenderingSystem.frameDelta;
		WorldUnmanaged worldUnmanaged = ((SystemState)(ref ((SystemBase)this).CheckedStateRef)).WorldUnmanaged;
		float playRate = frameDelta / math.max(1E-06f, ((WorldUnmanaged)(ref worldUnmanaged)).Time.DeltaTime * 60f);
		for (int i = 0; i < m_Effects.Length; i++)
		{
			if ((Object)(object)m_Effects[i].m_VisualEffect != (Object)null)
			{
				m_Effects[i].m_VisualEffect.playRate = playRate;
				m_Effects[i].m_VisualEffect.SetCheckedTexture(VFXIDs.WindTexture, (Texture)(object)m_WindTextureSystem.WindTexture);
				m_Effects[i].m_VisualEffect.SetCheckedVector4(VFXIDs.MapOffsetScale, m_TerrainSystem.mapOffsetScale);
			}
			int num5 = math.max(m_Effects[i].m_Indices.Count(), m_Effects[i].m_LastCount);
			if (m_Effects[i].m_NeedApply)
			{
				m_Effects[i].m_Texture.Apply();
				m_Effects[i].m_NeedApply = false;
			}
			m_Effects[i].m_VisualEffect.SetCheckedInt(VFXIDs.Count, m_Effects[i].m_LastCount);
			m_Effects[i].m_LastCount = m_Effects[i].m_Indices.Count();
			if (num5 != 0)
			{
				VFXTextureUpdateJob vFXTextureUpdateJob = new VFXTextureUpdateJob
				{
					m_TextureData = m_Effects[i].m_Texture.GetRawTextureData<float4>(),
					m_Instances = m_Effects[i].m_Instances,
					m_EnabledData = enabledData,
					m_Count = num5,
					m_TextureWidth = ((Texture)m_Effects[i].m_Texture).width
				};
				m_Effects[i].m_NeedApply = true;
				m_TextureUpdate = JobHandle.CombineDependencies(m_TextureUpdate, IJobExtensions.Schedule<VFXTextureUpdateJob>(vFXTextureUpdateJob, dependencies));
			}
		}
		m_EffectControlSystem.AddEnabledDataReader(m_TextureUpdate);
	}

	public void PreDeserialize(Context context)
	{
		if (m_Initialized && m_Effects != null)
		{
			((JobHandle)(ref m_TextureUpdate)).Complete();
			for (int i = 0; i < m_Effects.Length; i++)
			{
				if ((Object)(object)m_Effects[i].m_VisualEffect != (Object)null)
				{
					m_Effects[i].m_VisualEffect.SetCheckedInt(VFXIDs.Count, 0);
					Object.Destroy((Object)(object)((Component)m_Effects[i].m_VisualEffect).gameObject);
				}
				if (m_Effects[i].m_Instances.IsCreated)
				{
					m_Effects[i].m_Instances.Dispose();
				}
				if (m_Effects[i].m_Indices.IsCreated)
				{
					m_Effects[i].m_Indices.Dispose();
				}
			}
			m_Initialized = false;
		}
		ClearQueue();
	}

	private void ClearQueue()
	{
		((JobHandle)(ref m_SourceUpdateWriter)).Complete();
		NativeQueue<VFXUpdateInfo> val = default(NativeQueue<VFXUpdateInfo>);
		while (m_SourceUpdateQueue.TryDequeue(ref val))
		{
			val.Dispose();
		}
	}

	[Preserve]
	public VFXSystem()
	{
	}
}
