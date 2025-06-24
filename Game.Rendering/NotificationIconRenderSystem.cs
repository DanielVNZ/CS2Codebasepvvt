using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace Game.Rendering;

[CompilerGenerated]
public class NotificationIconRenderSystem : GameSystemBase
{
	private struct TypeHandle
	{
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RW_ComponentTypeHandle;

		public ComponentTypeHandle<NotificationIconDisplayData> __Game_Prefabs_NotificationIconDisplayData_RW_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			__Game_Prefabs_PrefabData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(false);
			__Game_Prefabs_NotificationIconDisplayData_RW_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<NotificationIconDisplayData>(false);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private NotificationIconBufferSystem m_BufferSystem;

	private RenderingSystem m_RenderingSystem;

	private Mesh m_Mesh;

	private Material m_Material;

	private ComputeBuffer m_ArgsBuffer;

	private ComputeBuffer m_InstanceBuffer;

	private Texture2DArray m_TextureArray;

	private uint[] m_ArgsArray;

	private EntityQuery m_ConfigurationQuery;

	private EntityQuery m_PrefabQuery;

	private int m_InstanceBufferID;

	private bool m_UpdateBuffer;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_BufferSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NotificationIconBufferSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_ConfigurationQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<IconConfigurationData>() });
		m_PrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<NotificationIconData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_InstanceBufferID = Shader.PropertyToID("instanceBuffer");
		((ComponentSystemBase)this).RequireForUpdate(m_ConfigurationQuery);
		RenderPipelineManager.beginContextRendering += Render;
	}

	[Preserve]
	protected override void OnDestroy()
	{
		RenderPipelineManager.beginContextRendering -= Render;
		if ((Object)(object)m_Mesh != (Object)null)
		{
			Object.Destroy((Object)(object)m_Mesh);
		}
		if ((Object)(object)m_Material != (Object)null)
		{
			Object.Destroy((Object)(object)m_Material);
		}
		if (m_ArgsBuffer != null)
		{
			m_ArgsBuffer.Release();
		}
		if (m_InstanceBuffer != null)
		{
			m_InstanceBuffer.Release();
		}
		if ((Object)(object)m_TextureArray != (Object)null)
		{
			Object.Destroy((Object)(object)m_TextureArray);
		}
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		m_UpdateBuffer = true;
	}

	public void DisplayDataUpdated()
	{
		if ((Object)(object)m_Material != (Object)null)
		{
			Object.Destroy((Object)(object)m_Material);
			m_Material = null;
		}
		if ((Object)(object)m_TextureArray != (Object)null)
		{
			Object.Destroy((Object)(object)m_TextureArray);
			m_TextureArray = null;
		}
	}

	private void Render(ScriptableRenderContext context, List<Camera> cameras)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Invalid comparison between Unknown and I4
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Invalid comparison between Unknown and I4
		try
		{
			if (m_RenderingSystem.hideOverlay)
			{
				return;
			}
			NotificationIconBufferSystem.IconData iconData = m_BufferSystem.GetIconData();
			if (!iconData.m_InstanceData.IsCreated)
			{
				return;
			}
			int length = iconData.m_InstanceData.Length;
			if (length == 0)
			{
				return;
			}
			Bounds val = RenderingUtils.ToBounds(iconData.m_IconBounds.value);
			Mesh mesh = GetMesh();
			Material material = GetMaterial();
			ComputeBuffer argsBuffer = GetArgsBuffer();
			m_ArgsArray[0] = mesh.GetIndexCount(0);
			m_ArgsArray[1] = (uint)length;
			m_ArgsArray[2] = mesh.GetIndexStart(0);
			m_ArgsArray[3] = mesh.GetBaseVertex(0);
			m_ArgsArray[4] = 0u;
			argsBuffer.SetData((Array)m_ArgsArray);
			if (m_UpdateBuffer)
			{
				m_UpdateBuffer = false;
				ComputeBuffer instanceBuffer = GetInstanceBuffer(length);
				instanceBuffer.SetData<NotificationIconBufferSystem.InstanceData>(iconData.m_InstanceData, 0, 0, length);
				material.SetBuffer(m_InstanceBufferID, instanceBuffer);
			}
			foreach (Camera camera in cameras)
			{
				if ((int)camera.cameraType == 1 || (int)camera.cameraType == 2)
				{
					Graphics.DrawMeshInstancedIndirect(mesh, 0, material, val, argsBuffer, 0, (MaterialPropertyBlock)null, (ShadowCastingMode)0, false, 0, camera, (LightProbeUsage)1);
				}
			}
		}
		finally
		{
		}
	}

	private Mesh GetMesh()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_Mesh == (Object)null)
		{
			m_Mesh = new Mesh();
			((Object)m_Mesh).name = "Notification icon";
			m_Mesh.vertices = (Vector3[])(object)new Vector3[4]
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 1f, 0f),
				new Vector3(1f, 1f, 0f),
				new Vector3(1f, -1f, 0f)
			};
			m_Mesh.uv = (Vector2[])(object)new Vector2[4]
			{
				new Vector2(0f, 0f),
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(1f, 0f)
			};
			m_Mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		}
		return m_Mesh;
	}

	private Material GetMaterial()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_Material == (Object)null)
		{
			Entity singletonEntity = ((EntityQuery)(ref m_ConfigurationQuery)).GetSingletonEntity();
			IconConfigurationPrefab prefab = m_PrefabSystem.GetPrefab<IconConfigurationPrefab>(singletonEntity);
			m_Material = new Material(prefab.m_Material);
			((Object)m_Material).name = "Notification icons";
			NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_PrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)3));
			ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			ComponentTypeHandle<NotificationIconDisplayData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<NotificationIconDisplayData>(ref __TypeHandle.__Game_Prefabs_NotificationIconDisplayData_RW_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
			try
			{
				int num = 1;
				int2 val2 = default(int2);
				((int2)(ref val2))._002Ector(((Texture)prefab.m_MissingIcon).width, ((Texture)prefab.m_MissingIcon).height);
				TextureFormat format = prefab.m_MissingIcon.format;
				for (int i = 0; i < val.Length; i++)
				{
					ArchetypeChunk val3 = val[i];
					EnabledMask enabledMask = ((ArchetypeChunk)(ref val3)).GetEnabledMask<PrefabData>(ref componentTypeHandle);
					NativeArray<PrefabData> nativeArray = ((ArchetypeChunk)(ref val3)).GetNativeArray<PrefabData>(ref componentTypeHandle);
					NativeArray<NotificationIconDisplayData> nativeArray2 = ((ArchetypeChunk)(ref val3)).GetNativeArray<NotificationIconDisplayData>(ref componentTypeHandle2);
					for (int j = 0; j < nativeArray.Length; j++)
					{
						if (((EnabledMask)(ref enabledMask))[j])
						{
							PrefabData prefabData = nativeArray[j];
							NotificationIconPrefab prefab2 = m_PrefabSystem.GetPrefab<NotificationIconPrefab>(prefabData);
							num = math.max(num, nativeArray2[j].m_IconIndex + 1);
							val2 = math.max(val2, new int2(((Texture)prefab2.m_Icon).width, ((Texture)prefab2.m_Icon).height));
							format = prefab2.m_Icon.format;
						}
					}
				}
				m_TextureArray = new Texture2DArray(val2.x, val2.y, num, format, true)
				{
					name = "NotificationIcons"
				};
				Graphics.CopyTexture((Texture)(object)prefab.m_MissingIcon, 0, (Texture)(object)m_TextureArray, 0);
				for (int k = 0; k < val.Length; k++)
				{
					ArchetypeChunk val4 = val[k];
					EnabledMask enabledMask2 = ((ArchetypeChunk)(ref val4)).GetEnabledMask<PrefabData>(ref componentTypeHandle);
					NativeArray<PrefabData> nativeArray3 = ((ArchetypeChunk)(ref val4)).GetNativeArray<PrefabData>(ref componentTypeHandle);
					NativeArray<NotificationIconDisplayData> nativeArray4 = ((ArchetypeChunk)(ref val4)).GetNativeArray<NotificationIconDisplayData>(ref componentTypeHandle2);
					for (int l = 0; l < nativeArray3.Length; l++)
					{
						if (((EnabledMask)(ref enabledMask2))[l])
						{
							PrefabData prefabData2 = nativeArray3[l];
							NotificationIconPrefab prefab3 = m_PrefabSystem.GetPrefab<NotificationIconPrefab>(prefabData2);
							NotificationIconDisplayData notificationIconDisplayData = nativeArray4[l];
							Graphics.CopyTexture((Texture)(object)prefab3.m_Icon, 0, (Texture)(object)m_TextureArray, notificationIconDisplayData.m_IconIndex);
						}
					}
				}
				m_Material.mainTexture = (Texture)(object)m_TextureArray;
			}
			finally
			{
				val.Dispose();
			}
		}
		return m_Material;
	}

	private ComputeBuffer GetArgsBuffer()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		if (m_ArgsBuffer == null)
		{
			m_ArgsArray = new uint[5];
			m_ArgsBuffer = new ComputeBuffer(1, m_ArgsArray.Length * 4, (ComputeBufferType)256);
			m_ArgsBuffer.name = "Notification args buffer";
		}
		return m_ArgsBuffer;
	}

	private ComputeBuffer GetInstanceBuffer(int count)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		if (m_InstanceBuffer != null && m_InstanceBuffer.count < count)
		{
			count = math.max(m_InstanceBuffer.count * 2, count);
			m_InstanceBuffer.Release();
			m_InstanceBuffer = null;
		}
		if (m_InstanceBuffer == null)
		{
			m_InstanceBuffer = new ComputeBuffer(math.max(64, count), System.Runtime.CompilerServices.Unsafe.SizeOf<NotificationIconBufferSystem.InstanceData>());
			m_InstanceBuffer.name = "Notification instance buffer";
		}
		return m_InstanceBuffer;
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
	public NotificationIconRenderSystem()
	{
	}
}
