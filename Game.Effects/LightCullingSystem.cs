using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Collections;
using Colossal.Mathematics;
using Game.Prefabs;
using Game.Prefabs.Effects;
using Game.Rendering;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Entities.Internal;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Scripting;

namespace Game.Effects;

[CompilerGenerated]
public class LightCullingSystem : GameSystemBase
{
	private struct DefaultLightParams
	{
		public float shapeWidth;

		public float shapeHeight;

		public float spotIESCutoffPercent01;

		public float shapeRadius;
	}

	private struct LightEffectCullData
	{
		public float m_Range;

		public float m_SpotAngle;

		public float m_InvDistanceFactor;

		public int m_LightEffectPrefabDataIndex;

		public LightType m_lightType;
	}

	[BurstCompile]
	private struct LightCullingJob : IJobParallelForDefer
	{
		[DeallocateOnJobCompletion]
		[ReadOnly]
		public NativeArray<float4> m_Planes;

		[ReadOnly]
		public NativeParallelHashMap<Entity, LightEffectCullData> m_LightEffectCullData;

		[ReadOnly]
		public NativeList<EnabledEffectData> m_EnabledEffectData;

		[ReadOnly]
		public float4 m_LodParameters;

		[ReadOnly]
		public float3 m_CameraPosition;

		[ReadOnly]
		public float3 m_CameraDirection;

		[ReadOnly]
		public float m_AutoRejectDistance;

		public ParallelWriter<VisibleLightData> m_VisibleLights;

		public void Execute(int index)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			EnabledEffectData enabledEffectData = m_EnabledEffectData[index];
			if ((enabledEffectData.m_Flags & (EnabledEffectFlags.IsEnabled | EnabledEffectFlags.IsLight)) != (EnabledEffectFlags.IsEnabled | EnabledEffectFlags.IsLight))
			{
				return;
			}
			float3 position = enabledEffectData.m_Position;
			LightEffectCullData lightEffectCullData = m_LightEffectCullData[enabledEffectData.m_Prefab];
			bool flag = enabledEffectData.m_Intensity != 0f;
			for (int i = 0; i < 6; i++)
			{
				float4 val = m_Planes[i];
				if (math.dot(((float4)(ref val)).xyz, position) + m_Planes[i].w < 0f - lightEffectCullData.m_Range)
				{
					flag = false;
				}
			}
			if (flag)
			{
				float num = RenderingUtils.CalculateMinDistance(new Bounds3(enabledEffectData.m_Position - lightEffectCullData.m_Range, enabledEffectData.m_Position + lightEffectCullData.m_Range), m_CameraPosition, m_CameraDirection, m_LodParameters) * lightEffectCullData.m_InvDistanceFactor;
				if (num < m_AutoRejectDistance)
				{
					m_VisibleLights.Enqueue(new VisibleLightData
					{
						m_Position = position,
						m_Rotation = enabledEffectData.m_Rotation,
						m_Prefab = enabledEffectData.m_Prefab,
						m_RelativeDistance = num,
						m_Color = enabledEffectData.m_Scale * enabledEffectData.m_Intensity
					});
				}
			}
		}
	}

	public struct VisibleLightData
	{
		public Entity m_Prefab;

		public float3 m_Position;

		public quaternion m_Rotation;

		public float3 m_Color;

		public float m_RelativeDistance;
	}

	[BurstCompile]
	private struct SortAndBuildPunctualLightsJob : IJob
	{
		private struct EffectInstanceDistanceComparer : IComparer<int>
		{
			private unsafe VisibleLightData* m_visibleLights;

			public unsafe EffectInstanceDistanceComparer(VisibleLightData* arrayPtr)
			{
				m_visibleLights = arrayPtr;
			}

			private unsafe ref VisibleLightData GetVisibleLightRef(int dataIndex)
			{
				return ref UnsafeUtility.AsRef<VisibleLightData>((void*)(m_visibleLights + dataIndex));
			}

			public int Compare(int x, int y)
			{
				return GetVisibleLightRef(x).m_RelativeDistance.CompareTo(GetVisibleLightRef(y).m_RelativeDistance);
			}
		}

		public NativeQueue<VisibleLightData> m_VisibleLights;

		[ReadOnly]
		public NativeParallelHashMap<Entity, LightEffectCullData> m_LightEffectCullData;

		[WriteOnly]
		public NativeList<PunctualLightData> m_PunctualLightsOut;

		[WriteOnly]
		public NativeList<LightEffectPrefabData> m_LightEffectPrefabData;

		[WriteOnly]
		public NativeArray<VisibleLight> m_VisibleLightsOut;

		public NativeReference<float> m_MaxDistance;

		public int m_maxLights;

		public float m_minDistanceScale;

		private unsafe ref VisibleLight GetVisibleLightRef(int dataIndex)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return ref UnsafeUtility.AsRef<VisibleLight>((void*)((byte*)NativeArrayUnsafeUtility.GetUnsafePtr<VisibleLight>(m_VisibleLightsOut) + (nint)dataIndex * (nint)System.Runtime.CompilerServices.Unsafe.SizeOf<VisibleLight>()));
		}

		private LightType GetLightType(LightType lightType)
		{
			return (LightType)(lightType switch
			{
				LightType.Spot => 0, 
				LightType.Point => 2, 
				LightType.Area => 3, 
				_ => 0, 
			});
		}

		public unsafe void Execute()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0219: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0270: Unknown result type (might be due to invalid IL or missing references)
			//IL_0283: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0297: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0301: Unknown result type (might be due to invalid IL or missing references)
			NativeList<int> val = default(NativeList<int>);
			val._002Ector(m_maxLights, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<int> val2 = default(NativeList<int>);
			val2._002Ector(m_maxLights, AllocatorHandle.op_Implicit((Allocator)2));
			NativeList<VisibleLightData> val3 = default(NativeList<VisibleLightData>);
			val3._002Ector(m_maxLights * 2, AllocatorHandle.op_Implicit((Allocator)2));
			float num = m_MaxDistance.Value * m_minDistanceScale;
			int num2 = 0;
			VisibleLightData visibleLightData = default(VisibleLightData);
			while (m_VisibleLights.TryDequeue(ref visibleLightData))
			{
				float num3 = visibleLightData.m_RelativeDistance;
				if (m_MaxDistance.Value > 0f)
				{
					val3.Add(ref visibleLightData);
					if (num3 < num)
					{
						val2.Add(ref num2);
					}
					else
					{
						val.Add(ref num2);
					}
					num2++;
				}
				else
				{
					val3.Add(ref visibleLightData);
					val.Add(ref num2);
					num2++;
				}
			}
			float num4 = -1f;
			if (val3.Length > 0)
			{
				NativeArray<VisibleLightData> val4 = val3.AsArray();
				NativeSortExtension.Sort<int, EffectInstanceDistanceComparer>(val, new EffectInstanceDistanceComparer((VisibleLightData*)NativeArrayUnsafeUtility.GetUnsafePtr<VisibleLightData>(val4)));
				int num5 = math.min(val.Length + val2.Length, m_maxLights);
				float num6 = 1f;
				if (num5 < val.Length + val2.Length)
				{
					num6 = 1f / math.clamp(((num5 >= val2.Length) ? val4[val[num5 - val2.Length]] : val4[val2[num5]]).m_RelativeDistance, 1E-05f, 1f);
				}
				num5 = math.min(num5, m_VisibleLightsOut.Length);
				for (int i = 0; i < num5; i++)
				{
					VisibleLightData visibleLightData2 = ((i >= val2.Length) ? val4[val[i - val2.Length]] : val4[val2[i]]);
					float3 val5 = visibleLightData2.m_Position;
					LightEffectCullData lightEffectCullData = m_LightEffectCullData[visibleLightData2.m_Prefab];
					float4x4 val6 = float4x4.TRS(val5, visibleLightData2.m_Rotation, float3.op_Implicit(Vector3.one));
					ref VisibleLight visibleLightRef = ref GetVisibleLightRef(i);
					((VisibleLight)(ref visibleLightRef)).spotAngle = lightEffectCullData.m_SpotAngle;
					((VisibleLight)(ref visibleLightRef)).localToWorldMatrix = float4x4.op_Implicit(val6);
					float num7 = visibleLightData2.m_RelativeDistance * num6;
					float num8 = math.saturate(1f - math.lengthsq(math.max(0f, 5f * num7 - 4f)));
					((VisibleLight)(ref visibleLightRef)).screenRect = new Rect(0f, 0f, 10f, 10f);
					((VisibleLight)(ref visibleLightRef)).lightType = GetLightType(lightEffectCullData.m_lightType);
					((VisibleLight)(ref visibleLightRef)).finalColor = Color.op_Implicit(float4.op_Implicit(new float4(visibleLightData2.m_Color * num8, 1f)));
					((VisibleLight)(ref visibleLightRef)).range = lightEffectCullData.m_Range;
					m_PunctualLightsOut.AddNoResize(new PunctualLightData
					{
						lightEffectPrefabDataIndex = lightEffectCullData.m_LightEffectPrefabDataIndex
					});
					m_LightEffectPrefabData[lightEffectCullData.m_LightEffectPrefabDataIndex] = new LightEffectPrefabData
					{
						cookieMode = (CookieMode)1
					};
					num4 = math.max(num4, visibleLightData2.m_RelativeDistance);
				}
			}
			m_MaxDistance.Value = num4;
			val.Dispose();
			val2.Dispose();
			val3.Dispose();
		}
	}

	private struct TypeHandle
	{
		[ReadOnly]
		public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<PrefabData> __Game_Prefabs_PrefabData_RO_ComponentTypeHandle;

		[ReadOnly]
		public ComponentTypeHandle<LightEffectData> __Game_Prefabs_LightEffectData_RO_ComponentTypeHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void __AssignHandles(ref SystemState state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			__Unity_Entities_Entity_TypeHandle = ((SystemState)(ref state)).GetEntityTypeHandle();
			__Game_Prefabs_PrefabData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<PrefabData>(true);
			__Game_Prefabs_LightEffectData_RO_ComponentTypeHandle = ((SystemState)(ref state)).GetComponentTypeHandle<LightEffectData>(true);
		}
	}

	private PrefabSystem m_PrefabSystem;

	private RenderingSystem m_RenderingSystem;

	private EffectControlSystem m_EffectControlSystem;

	private EntityQuery m_LightEffectPrefabQuery;

	private NativeParallelHashMap<Entity, LightEffectCullData> m_LightEffectCullData;

	private static DefaultLightParams s_DefaultLightParams;

	private NativeQueue<VisibleLightData> m_VisibleLights;

	private NativeReference<float> m_LastFrameMaxPunctualLightDistance;

	public static bool s_enableMinMaxLightCullingOptim = true;

	public static float s_maxLightDistanceScale = 1.5f;

	public static float s_minLightDistanceScale = 0.5f;

	private TypeHandle __TypeHandle;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_RenderingSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<RenderingSystem>();
		m_EffectControlSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<EffectControlSystem>();
		m_LightEffectPrefabQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<LightEffectData>(),
			ComponentType.ReadOnly<PrefabData>()
		});
		m_LightEffectCullData = new NativeParallelHashMap<Entity, LightEffectCullData>(128, AllocatorHandle.op_Implicit((Allocator)4));
		m_VisibleLights = new NativeQueue<VisibleLightData>(AllocatorHandle.op_Implicit((Allocator)4));
		m_LastFrameMaxPunctualLightDistance = new NativeReference<float>(AllocatorHandle.op_Implicit((Allocator)4), (NativeArrayOptions)1);
		m_LastFrameMaxPunctualLightDistance.Value = -1f;
		ReadDefaultLightParams();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		JobHandle punctualLightsJobHandle = HDRPDotsInputs.punctualLightsJobHandle;
		((JobHandle)(ref punctualLightsJobHandle)).Complete();
		m_LastFrameMaxPunctualLightDistance.Dispose();
		m_VisibleLights.Dispose();
		m_LightEffectCullData.Dispose();
		HDRPDotsInputs.ClearFrameLightData();
		base.OnDestroy();
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		JobHandle punctualLightsJobHandle = HDRPDotsInputs.punctualLightsJobHandle;
		((JobHandle)(ref punctualLightsJobHandle)).Complete();
		HDRPDotsInputs.MaxPunctualLights = m_RenderingSystem.maxLightCount;
		HDRPDotsInputs.ClearFrameLightData();
		Camera main = Camera.main;
		if (!((Object)(object)main == (Object)null))
		{
			m_EffectControlSystem.GetLodParameters(out var lodParameters, out var cameraPosition, out var cameraDirection);
			ComputeLightEffectCullData(lodParameters);
			Plane[] array = GeometryUtility.CalculateFrustumPlanes(main);
			NativeArray<float4> planes = default(NativeArray<float4>);
			planes._002Ector(6, (Allocator)3, (NativeArrayOptions)1);
			for (int i = 0; i < array.Length; i++)
			{
				planes[i] = new float4(float3.op_Implicit(((Plane)(ref array[i])).normal), ((Plane)(ref array[i])).distance);
			}
			if (!s_enableMinMaxLightCullingOptim)
			{
				m_LastFrameMaxPunctualLightDistance.Value = -1f;
			}
			float autoRejectDistance = 1f;
			if (m_LastFrameMaxPunctualLightDistance.Value > 0f)
			{
				autoRejectDistance = m_LastFrameMaxPunctualLightDistance.Value * s_maxLightDistanceScale;
			}
			JobHandle dependencies;
			LightCullingJob obj = new LightCullingJob
			{
				m_LightEffectCullData = m_LightEffectCullData,
				m_Planes = planes,
				m_EnabledEffectData = m_EffectControlSystem.GetEnabledData(readOnly: true, out dependencies),
				m_LodParameters = lodParameters,
				m_CameraPosition = cameraPosition,
				m_CameraDirection = cameraDirection,
				m_AutoRejectDistance = autoRejectDistance,
				m_VisibleLights = m_VisibleLights.AsParallelWriter()
			};
			JobHandle val = IJobParallelForDeferExtensions.Schedule<LightCullingJob, EnabledEffectData>(obj, obj.m_EnabledEffectData, 16, dependencies);
			m_EffectControlSystem.AddEnabledDataReader(val);
			HDRPDotsInputs.punctualLightsJobHandle = IJobExtensions.Schedule<SortAndBuildPunctualLightsJob>(new SortAndBuildPunctualLightsJob
			{
				m_maxLights = HDRPDotsInputs.MaxPunctualLights,
				m_minDistanceScale = s_minLightDistanceScale,
				m_PunctualLightsOut = HDRPDotsInputs.s_punctualLightdata,
				m_LightEffectPrefabData = HDRPDotsInputs.s_lightEffectPrefabData,
				m_VisibleLightsOut = HDRPDotsInputs.s_punctualVisibleLights,
				m_LightEffectCullData = m_LightEffectCullData,
				m_VisibleLights = m_VisibleLights,
				m_MaxDistance = m_LastFrameMaxPunctualLightDistance
			}, val);
		}
	}

	private void ReadDefaultLightParams()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0057: Expected O, but got Unknown
		GameObject val = new GameObject("Default LightSource");
		HDAdditionalLightData val2 = GameObjectExtension.AddHDLight(val, (HDLightTypeAndShape)3);
		s_DefaultLightParams.shapeWidth = val2.shapeWidth;
		s_DefaultLightParams.shapeHeight = val2.shapeHeight;
		s_DefaultLightParams.spotIESCutoffPercent01 = val2.spotIESCutoffPercent01;
		s_DefaultLightParams.shapeRadius = val2.shapeRadius;
		CoreUtils.Destroy((Object)val);
	}

	private SpotLightShape GetUnitySpotShape(SpotLightShape spotlightShape)
	{
		return (SpotLightShape)(spotlightShape switch
		{
			SpotLightShape.Pyramid => 1, 
			SpotLightShape.Box => 2, 
			SpotLightShape.Cone => 0, 
			_ => 0, 
		});
	}

	private AreaLightShape GetUnityAreaShape(AreaLightShape arealightShape)
	{
		return (AreaLightShape)(arealightShape switch
		{
			AreaLightShape.Tube => 1, 
			AreaLightShape.Rectangle => 0, 
			_ => 0, 
		});
	}

	private void GetRenderDataFromLigthEffet(ref HDLightRenderData hdLightRenderData, LightEffectData lightEffectData, LightEffect lightEffect, float4 lodParameters)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		hdLightRenderData.pointLightType = (PointLightHDType)(lightEffect.m_Type == LightType.Area);
		hdLightRenderData.spotLightShape = GetUnitySpotShape(lightEffect.m_SpotShape);
		hdLightRenderData.areaLightShape = GetUnityAreaShape(lightEffect.m_AreaShape);
		hdLightRenderData.lightLayer = (LightLayerEnum)255;
		hdLightRenderData.fadeDistance = 100000f;
		hdLightRenderData.distance = lightEffect.m_LuxAtDistance;
		hdLightRenderData.angularDiameter = lightEffect.m_SpotAngle;
		hdLightRenderData.volumetricFadeDistance = lightEffect.m_VolumetricFadeDistance;
		hdLightRenderData.includeForRayTracing = false;
		hdLightRenderData.useScreenSpaceShadows = false;
		hdLightRenderData.useRayTracedShadows = false;
		hdLightRenderData.colorShadow = false;
		hdLightRenderData.lightDimmer = lightEffect.m_LightDimmer;
		hdLightRenderData.volumetricDimmer = lightEffect.m_VolumetricDimmer;
		hdLightRenderData.shapeWidth = lightEffect.m_ShapeWidth;
		hdLightRenderData.shapeHeight = lightEffect.m_ShapeHeight;
		hdLightRenderData.aspectRatio = lightEffect.m_AspectRatio;
		hdLightRenderData.innerSpotPercent = lightEffect.m_InnerSpotPercentage;
		hdLightRenderData.spotIESCutoffPercent = 100f;
		hdLightRenderData.shadowDimmer = 1f;
		hdLightRenderData.volumetricShadowDimmer = 1f;
		hdLightRenderData.shadowFadeDistance = 0f;
		hdLightRenderData.shapeRadius = lightEffect.m_ShapeRadius;
		hdLightRenderData.barnDoorLength = lightEffect.m_BarnDoorLength;
		hdLightRenderData.barnDoorAngle = lightEffect.m_BarnDoorAngle;
		hdLightRenderData.flareSize = 0f;
		hdLightRenderData.flareFalloff = 0f;
		hdLightRenderData.affectVolumetric = lightEffect.m_UseVolumetric;
		hdLightRenderData.affectDiffuse = lightEffect.m_AffectDiffuse;
		hdLightRenderData.affectSpecular = lightEffect.m_AffectSpecular;
		hdLightRenderData.applyRangeAttenuation = lightEffect.m_ApplyRangeAttenuation;
		hdLightRenderData.penumbraTint = false;
		hdLightRenderData.interactsWithSky = false;
		hdLightRenderData.surfaceTint = Color.black;
		hdLightRenderData.shadowTint = Color.black;
		hdLightRenderData.flareTint = Color.black;
	}

	private void ComputeLightEffectCullData(float4 lodParameters)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		m_LightEffectCullData.Clear();
		NativeArray<ArchetypeChunk> val = ((EntityQuery)(ref m_LightEffectPrefabQuery)).ToArchetypeChunkArray(AllocatorHandle.op_Implicit((Allocator)2));
		EntityTypeHandle entityTypeHandle = InternalCompilerInterface.GetEntityTypeHandle(ref __TypeHandle.__Unity_Entities_Entity_TypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<PrefabData> componentTypeHandle = InternalCompilerInterface.GetComponentTypeHandle<PrefabData>(ref __TypeHandle.__Game_Prefabs_PrefabData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		ComponentTypeHandle<LightEffectData> componentTypeHandle2 = InternalCompilerInterface.GetComponentTypeHandle<LightEffectData>(ref __TypeHandle.__Game_Prefabs_LightEffectData_RO_ComponentTypeHandle, ref ((SystemBase)this).CheckedStateRef);
		((SystemBase)this).CompleteDependency();
		int num = ((EntityQuery)(ref m_LightEffectPrefabQuery)).CalculateEntityCount();
		float num2 = 1f / lodParameters.x;
		if (!HDRPDotsInputs.s_HdLightRenderData.IsCreated || num + 8 > HDRPDotsInputs.s_HdLightRenderData.Length)
		{
			ArrayExtensions.ResizeArray<HDLightRenderData>(ref HDRPDotsInputs.s_HdLightRenderData, num + 8);
		}
		LightEffectCullData lightEffectCullData = default(LightEffectCullData);
		for (int i = 0; i < val.Length; i++)
		{
			ArchetypeChunk val2 = val[i];
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref val2)).GetNativeArray(entityTypeHandle);
			NativeArray<PrefabData> nativeArray2 = ((ArchetypeChunk)(ref val2)).GetNativeArray<PrefabData>(ref componentTypeHandle);
			NativeArray<LightEffectData> nativeArray3 = ((ArchetypeChunk)(ref val2)).GetNativeArray<LightEffectData>(ref componentTypeHandle2);
			for (int j = 0; j < nativeArray.Length; j++)
			{
				Entity val3 = nativeArray[j];
				PrefabData prefabData = nativeArray2[j];
				LightEffectData lightEffectData = nativeArray3[j];
				LightEffect component = m_PrefabSystem.GetPrefab<EffectPrefab>(prefabData).GetComponent<LightEffect>();
				lightEffectCullData.m_LightEffectPrefabDataIndex = HDRPDotsInputs.s_lightEffectPrefabData.Length;
				lightEffectCullData.m_lightType = component.m_Type;
				lightEffectCullData.m_Range = component.m_Range;
				lightEffectCullData.m_SpotAngle = component.m_SpotAngle;
				lightEffectCullData.m_InvDistanceFactor = lightEffectData.m_InvDistanceFactor * num2;
				LightEffectPrefabData val4 = default(LightEffectPrefabData);
				HDRPDotsInputs.s_lightEffectPrefabData.Add(ref val4);
				HDRPDotsInputs.s_lightEffectPrefabCookies.Add(component.m_Cookie);
				GetRenderDataFromLigthEffet(ref CollectionUtils.ElementAt<HDLightRenderData>(HDRPDotsInputs.s_HdLightRenderData, lightEffectCullData.m_LightEffectPrefabDataIndex), lightEffectData, component, lodParameters);
				m_LightEffectCullData.Add(val3, lightEffectCullData);
			}
		}
	}

	private static GPULightType GetGPULightType(LightEffect lightEffect)
	{
		if (lightEffect.m_Type == LightType.Spot)
		{
			if (lightEffect.m_SpotShape == SpotLightShape.Cone)
			{
				return (GPULightType)2;
			}
			if (lightEffect.m_SpotShape == SpotLightShape.Pyramid)
			{
				return (GPULightType)3;
			}
			if (lightEffect.m_SpotShape == SpotLightShape.Box)
			{
				return (GPULightType)4;
			}
		}
		else
		{
			if (lightEffect.m_Type == LightType.Point)
			{
				return (GPULightType)1;
			}
			if (lightEffect.m_Type == LightType.Area)
			{
				if (lightEffect.m_AreaShape == AreaLightShape.Rectangle)
				{
					return (GPULightType)6;
				}
				if (lightEffect.m_AreaShape == AreaLightShape.Tube)
				{
					return (GPULightType)5;
				}
			}
		}
		throw new NotImplementedException($"Unsupported light type {lightEffect.m_Type}");
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
	public LightCullingSystem()
	{
	}
}
