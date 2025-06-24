using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Mathematics;
using Game.Areas;
using Game.Buildings;
using Game.City;
using Game.Common;
using Game.Net;
using Game.Objects;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Tools;

[CompilerGenerated]
public class ApplyPrefabsSystem : GameSystemBase
{
	private CityConfigurationSystem m_CityConfigurationSystem;

	private PrefabSystem m_PrefabSystem;

	private EntityQuery m_SaveInstanceQuery;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_CityConfigurationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<CityConfigurationSystem>();
		m_PrefabSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<PrefabSystem>();
		m_SaveInstanceQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Objects.Object>(),
			ComponentType.ReadOnly<SaveInstance>()
		});
		((ComponentSystemBase)this).RequireForUpdate(m_SaveInstanceQuery);
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Entity> val = ((EntityQuery)(ref m_SaveInstanceQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		try
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent<SaveInstance>(m_SaveInstanceQuery);
			for (int i = 0; i < val.Length; i++)
			{
				Entity val2 = val[i];
				entityManager = ((ComponentSystemBase)this).EntityManager;
				((EntityManager)(ref entityManager)).RemoveComponent<SaveInstance>(val2);
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (!((EntityManager)(ref entityManager)).HasComponent<Game.Objects.SubObject>(val2))
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (!((EntityManager)(ref entityManager)).HasComponent<Game.Net.SubNet>(val2))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (!((EntityManager)(ref entityManager)).HasComponent<Game.Areas.SubArea>(val2))
						{
							continue;
						}
					}
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(val2);
				PrefabBase prefab = m_PrefabSystem.GetPrefab<PrefabBase>(componentData);
				uint constructionCost = 0u;
				uint upKeepCost = 0u;
				UpdateObjectSubObjects(val2, prefab, ref constructionCost, ref upKeepCost);
				UpdateObjectSubNets(val2, prefab, ref constructionCost, ref upKeepCost);
				UpdateObjectSubAreas(val2, prefab, ref constructionCost, ref upKeepCost);
				if (prefab is AssetStampPrefab)
				{
					AssetStampPrefab obj = prefab as AssetStampPrefab;
					obj.m_ConstructionCost = constructionCost;
					obj.m_UpKeepCost = upKeepCost;
				}
				m_PrefabSystem.UpdatePrefab(prefab, val2);
				PrefabAsset asset = prefab.asset;
				if (asset != null)
				{
					((AssetData)asset).MarkDirty();
				}
			}
		}
		finally
		{
			val.Dispose();
		}
	}

	private void UpdateObjectSubObjects(Entity instanceEntity, PrefabBase prefabBase, ref uint constructionCost, ref uint upKeepCost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ListObjectSubObjects(instanceEntity, out var subObjectList, out var subEffectList, out var subActivityList, ref constructionCost, ref upKeepCost);
		if (subObjectList != null && subObjectList.Count != 0)
		{
			ObjectSubObjects objectSubObjects = prefabBase.GetComponent<ObjectSubObjects>();
			if ((Object)(object)objectSubObjects == (Object)null)
			{
				objectSubObjects = AddComponent<ObjectSubObjects>(prefabBase);
			}
			objectSubObjects.m_SubObjects = subObjectList.ToArray();
		}
		else if ((Object)(object)prefabBase.GetComponent<ObjectSubObjects>() != (Object)null)
		{
			RemoveComponent<ObjectSubObjects>(prefabBase);
		}
		if (subEffectList != null && subEffectList.Count != 0)
		{
			EffectSource effectSource = prefabBase.GetComponent<EffectSource>();
			if ((Object)(object)effectSource == (Object)null)
			{
				effectSource = AddComponent<EffectSource>(prefabBase);
			}
			effectSource.m_Effects = new List<EffectSource.EffectSettings>();
			effectSource.m_Effects.AddRange(subEffectList);
		}
		else if ((Object)(object)prefabBase.GetComponent<EffectSource>() != (Object)null)
		{
			RemoveComponent<EffectSource>(prefabBase);
		}
		if (subActivityList != null && subActivityList.Count != 0)
		{
			Game.Prefabs.ActivityLocation activityLocation = prefabBase.GetComponent<Game.Prefabs.ActivityLocation>();
			if ((Object)(object)activityLocation == (Object)null)
			{
				activityLocation = AddComponent<Game.Prefabs.ActivityLocation>(prefabBase);
			}
			activityLocation.m_Locations = subActivityList.ToArray();
		}
		else if ((Object)(object)prefabBase.GetComponent<Game.Prefabs.ActivityLocation>() != (Object)null)
		{
			RemoveComponent<Game.Prefabs.ActivityLocation>(prefabBase);
		}
	}

	private void ListObjectSubObjects(Entity instanceEntity, out List<ObjectSubObjectInfo> subObjectList, out List<EffectSource.EffectSettings> subEffectList, out List<Game.Prefabs.ActivityLocation.LocationInfo> subActivityList, ref uint constructionCost, ref uint upKeepCost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Objects.SubObject>(instanceEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Game.Objects.SubObject> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Game.Objects.SubObject>(instanceEntity, true);
			if (buffer.Length != 0)
			{
				Transform inverseParentTransform = default(Transform);
				bool flag = false;
				Transform transform = default(Transform);
				if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, instanceEntity, ref transform))
				{
					inverseParentTransform.m_Position = -transform.m_Position;
					inverseParentTransform.m_Rotation = math.inverse(transform.m_Rotation);
					flag = true;
				}
				subObjectList = new List<ObjectSubObjectInfo>(buffer.Length);
				subEffectList = new List<EffectSource.EffectSettings>(buffer.Length);
				subActivityList = new List<Game.Prefabs.ActivityLocation.LocationInfo>(buffer.Length);
				Owner owner = default(Owner);
				Transform transform2 = default(Transform);
				LocalTransformCache localTransformCache = default(LocalTransformCache);
				Game.Objects.Elevation elevation = default(Game.Objects.Elevation);
				EditorContainer editorContainer = default(EditorContainer);
				EditorContainer editorContainer2 = default(EditorContainer);
				PlaceableObjectData placeableObjectData = default(PlaceableObjectData);
				ConsumptionData consumptionData = default(ConsumptionData);
				for (int i = 0; i < buffer.Length; i++)
				{
					Entity subObject = buffer[i].m_SubObject;
					if (!EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, subObject, ref owner))
					{
						continue;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Secondary>(subObject))
					{
						continue;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Game.Buildings.ServiceUpgrade>(subObject) || !(owner.m_Owner == instanceEntity))
					{
						continue;
					}
					int num = 0;
					int groupIndex = 0;
					int probability = 0;
					float3 value;
					quaternion value2;
					if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, subObject, ref transform2))
					{
						if (flag)
						{
							Transform transform3 = ObjectUtils.WorldToLocal(inverseParentTransform, transform2);
							value = transform3.m_Position;
							value2 = transform3.m_Rotation;
						}
						else
						{
							value = transform2.m_Position;
							value2 = transform2.m_Rotation;
						}
						if (!EntitiesExtensions.TryGetComponent<LocalTransformCache>(((ComponentSystemBase)this).EntityManager, subObject, ref localTransformCache))
						{
							num = ((!EntitiesExtensions.TryGetComponent<Game.Objects.Elevation>(((ComponentSystemBase)this).EntityManager, subObject, ref elevation)) ? (-1) : ObjectUtils.GetSubParentMesh(elevation.m_Flags));
						}
						else
						{
							CheckCachedValue(ref value, localTransformCache.m_Position);
							CheckCachedValue(ref value2, localTransformCache.m_Rotation);
							num = localTransformCache.m_ParentMesh;
							groupIndex = localTransformCache.m_GroupIndex;
							probability = localTransformCache.m_Probability;
							if (EntitiesExtensions.TryGetComponent<EditorContainer>(((ComponentSystemBase)this).EntityManager, subObject, ref editorContainer))
							{
								entityManager = ((ComponentSystemBase)this).EntityManager;
								if (((EntityManager)(ref entityManager)).HasComponent<EffectData>(editorContainer.m_Prefab))
								{
									localTransformCache.m_PrefabSubIndex = subEffectList.Count;
								}
								else
								{
									entityManager = ((ComponentSystemBase)this).EntityManager;
									if (((EntityManager)(ref entityManager)).HasComponent<ActivityLocationData>(editorContainer.m_Prefab))
									{
										localTransformCache.m_PrefabSubIndex = subActivityList.Count;
									}
								}
							}
							else
							{
								localTransformCache.m_PrefabSubIndex = subObjectList.Count;
							}
							entityManager = ((ComponentSystemBase)this).EntityManager;
							((EntityManager)(ref entityManager)).SetComponentData<LocalTransformCache>(subObject, localTransformCache);
						}
					}
					else
					{
						value = float3.zero;
						value2 = quaternion.identity;
						num = -1;
					}
					if (EntitiesExtensions.TryGetComponent<EditorContainer>(((ComponentSystemBase)this).EntityManager, subObject, ref editorContainer2))
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<EffectData>(editorContainer2.m_Prefab))
						{
							subEffectList.Add(new EffectSource.EffectSettings
							{
								m_Effect = m_PrefabSystem.GetPrefab<EffectPrefab>(editorContainer2.m_Prefab),
								m_PositionOffset = value,
								m_Rotation = value2,
								m_Scale = editorContainer2.m_Scale,
								m_Intensity = editorContainer2.m_Intensity,
								m_ParentMesh = num,
								m_AnimationIndex = editorContainer2.m_GroupIndex
							});
						}
						else
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							if (((EntityManager)(ref entityManager)).HasComponent<ActivityLocationData>(editorContainer2.m_Prefab))
							{
								subActivityList.Add(new Game.Prefabs.ActivityLocation.LocationInfo
								{
									m_Activity = m_PrefabSystem.GetPrefab<ActivityLocationPrefab>(editorContainer2.m_Prefab),
									m_Position = value,
									m_Rotation = value2
								});
							}
						}
					}
					else
					{
						ObjectSubObjectInfo objectSubObjectInfo = new ObjectSubObjectInfo();
						entityManager = ((ComponentSystemBase)this).EntityManager;
						PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(subObject);
						objectSubObjectInfo.m_Object = m_PrefabSystem.GetPrefab<ObjectPrefab>(componentData);
						if (EntitiesExtensions.TryGetComponent<PlaceableObjectData>(((ComponentSystemBase)this).EntityManager, componentData.m_Prefab, ref placeableObjectData))
						{
							constructionCost += placeableObjectData.m_ConstructionCost;
						}
						if (EntitiesExtensions.TryGetComponent<ConsumptionData>(((ComponentSystemBase)this).EntityManager, componentData.m_Prefab, ref consumptionData))
						{
							upKeepCost += (uint)consumptionData.m_Upkeep;
						}
						objectSubObjectInfo.m_Position = value;
						objectSubObjectInfo.m_Rotation = value2;
						objectSubObjectInfo.m_ParentMesh = num;
						objectSubObjectInfo.m_GroupIndex = groupIndex;
						objectSubObjectInfo.m_Probability = probability;
						subObjectList.Add(objectSubObjectInfo);
					}
				}
				return;
			}
		}
		subObjectList = null;
		subEffectList = null;
		subActivityList = null;
	}

	private void UpdateObjectSubNets(Entity instanceEntity, PrefabBase prefabBase, ref uint constructionCost, ref uint upKeepCost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		ListObjectSubNets(instanceEntity, out var subNetList, out var subLaneList, ref constructionCost, ref upKeepCost);
		if (subNetList != null && subNetList.Count != 0)
		{
			ObjectSubNets objectSubNets = prefabBase.GetComponent<ObjectSubNets>();
			if ((Object)(object)objectSubNets == (Object)null)
			{
				objectSubNets = AddComponent<ObjectSubNets>(prefabBase);
			}
			if (objectSubNets.m_InvertWhen == NetInvertMode.LefthandTraffic && m_CityConfigurationSystem.leftHandTraffic)
			{
				objectSubNets.m_InvertWhen = NetInvertMode.RighthandTraffic;
			}
			else if (objectSubNets.m_InvertWhen == NetInvertMode.RighthandTraffic && !m_CityConfigurationSystem.leftHandTraffic)
			{
				objectSubNets.m_InvertWhen = NetInvertMode.LefthandTraffic;
			}
			objectSubNets.m_SubNets = subNetList.ToArray();
		}
		else if ((Object)(object)prefabBase.GetComponent<ObjectSubNets>() != (Object)null)
		{
			RemoveComponent<ObjectSubNets>(prefabBase);
		}
		if (subLaneList != null && subLaneList.Count != 0)
		{
			ObjectSubLanes objectSubLanes = prefabBase.GetComponent<ObjectSubLanes>();
			if ((Object)(object)objectSubLanes == (Object)null)
			{
				objectSubLanes = AddComponent<ObjectSubLanes>(prefabBase);
			}
			objectSubLanes.m_SubLanes = subLaneList.ToArray();
		}
		else if ((Object)(object)prefabBase.GetComponent<ObjectSubLanes>() != (Object)null)
		{
			RemoveComponent<ObjectSubLanes>(prefabBase);
		}
	}

	private NetPieceRequirements[] CreateRequirementMap()
	{
		NetPieceRequirements[] array = new NetPieceRequirements[96];
		foreach (NetPieceRequirements value in Enum.GetValues(typeof(NetPieceRequirements)))
		{
			CompositionFlags compositionFlags = default(CompositionFlags);
			NetSectionFlags sectionFlags = (NetSectionFlags)0;
			NetCompositionHelpers.GetRequirementFlags(value, ref compositionFlags, ref sectionFlags);
			if (compositionFlags.m_Left != 0)
			{
				for (int i = 0; i < 32; i++)
				{
					if (((uint)compositionFlags.m_Left & (uint)(1 << i)) != 0)
					{
						array[i] = value;
					}
				}
			}
			if (compositionFlags.m_General != 0)
			{
				for (int j = 0; j < 32; j++)
				{
					if (((uint)compositionFlags.m_General & (uint)(1 << j)) != 0)
					{
						array[j + 32] = value;
					}
				}
			}
			if (compositionFlags.m_Right == (CompositionFlags.Side)0u)
			{
				continue;
			}
			for (int k = 0; k < 32; k++)
			{
				if (((uint)compositionFlags.m_Right & (uint)(1 << k)) != 0)
				{
					array[k + 64] = value;
				}
			}
		}
		return array;
	}

	private NetPieceRequirements[] CreateRequirementArray(NetPieceRequirements[] requirementMap, CompositionFlags flags)
	{
		List<NetPieceRequirements> list = new List<NetPieceRequirements>(10);
		if (flags.m_Left != 0)
		{
			for (int i = 0; i < 32; i++)
			{
				if (((uint)flags.m_Left & (uint)(1 << i)) != 0)
				{
					list.Add(requirementMap[i]);
				}
			}
		}
		if (flags.m_General != 0)
		{
			for (int j = 0; j < 32; j++)
			{
				if (((uint)flags.m_General & (uint)(1 << j)) != 0)
				{
					list.Add(requirementMap[j + 32]);
				}
			}
		}
		if (flags.m_Right != 0)
		{
			for (int k = 0; k < 32; k++)
			{
				if (((uint)flags.m_Right & (uint)(1 << k)) != 0)
				{
					list.Add(requirementMap[k + 64]);
				}
			}
		}
		if (list.Count != 0)
		{
			return list.ToArray();
		}
		return null;
	}

	private void ListObjectSubNets(Entity instanceEntity, out List<ObjectSubNetInfo> subNetList, out List<ObjectSubLaneInfo> subLaneList, ref uint constructionCost, ref uint upKeepCost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_053f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_056c: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_057e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Net.SubNet>(instanceEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Game.Net.SubNet> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Game.Net.SubNet>(instanceEntity, true);
			if (buffer.Length != 0)
			{
				Transform transform = default(Transform);
				bool flag = false;
				Transform transform2 = default(Transform);
				if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, instanceEntity, ref transform2))
				{
					transform.m_Position = -transform2.m_Position;
					transform.m_Rotation = math.inverse(transform2.m_Rotation);
					flag = true;
				}
				subNetList = new List<ObjectSubNetInfo>(buffer.Length);
				subLaneList = new List<ObjectSubLaneInfo>(buffer.Length);
				Dictionary<Entity, int> dictionary = new Dictionary<Entity, int>(buffer.Length * 2);
				Dictionary<Entity, int> dictionary2 = new Dictionary<Entity, int>(buffer.Length * 2);
				NetPieceRequirements[] array = null;
				Owner owner = default(Owner);
				Composition composition = default(Composition);
				NetCompositionData netCompositionData = default(NetCompositionData);
				Upgraded upgraded = default(Upgraded);
				Edge edge = default(Edge);
				Curve curve = default(Curve);
				LocalCurveCache localCurveCache = default(LocalCurveCache);
				Composition composition2 = default(Composition);
				PlaceableNetComposition placeableNetData = default(PlaceableNetComposition);
				Game.Net.Elevation startElevation = default(Game.Net.Elevation);
				Game.Net.Elevation endElevation = default(Game.Net.Elevation);
				EditorContainer editorContainer = default(EditorContainer);
				Game.Net.Node node = default(Game.Net.Node);
				DynamicBuffer<ConnectedEdge> connectedEdges = default(DynamicBuffer<ConnectedEdge>);
				LocalTransformCache localTransformCache = default(LocalTransformCache);
				EditorContainer editorContainer2 = default(EditorContainer);
				for (int i = 0; i < buffer.Length; i++)
				{
					Entity subNet = buffer[i].m_SubNet;
					if (!EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, subNet, ref owner) || !(owner.m_Owner == instanceEntity))
					{
						continue;
					}
					NetPieceRequirements[] array2 = null;
					CompositionFlags compositionFlags = default(CompositionFlags);
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Edge>(subNet) && EntitiesExtensions.TryGetComponent<Composition>(((ComponentSystemBase)this).EntityManager, subNet, ref composition) && EntitiesExtensions.TryGetComponent<NetCompositionData>(((ComponentSystemBase)this).EntityManager, composition.m_Edge, ref netCompositionData))
					{
						compositionFlags.m_General |= netCompositionData.m_Flags.m_General & CompositionFlags.General.Elevated;
					}
					if (EntitiesExtensions.TryGetComponent<Upgraded>(((ComponentSystemBase)this).EntityManager, subNet, ref upgraded) || compositionFlags != default(CompositionFlags))
					{
						if (array == null)
						{
							array = CreateRequirementMap();
						}
						array2 = CreateRequirementArray(array, upgraded.m_Flags | compositionFlags);
					}
					if (EntitiesExtensions.TryGetComponent<Edge>(((ComponentSystemBase)this).EntityManager, subNet, ref edge))
					{
						Bezier4x3 bezierCurve = default(Bezier4x3);
						if (EntitiesExtensions.TryGetComponent<Curve>(((ComponentSystemBase)this).EntityManager, subNet, ref curve))
						{
							if (flag)
							{
								bezierCurve.a = math.mul(transform.m_Rotation, curve.m_Bezier.a + transform.m_Position);
								bezierCurve.b = math.mul(transform.m_Rotation, curve.m_Bezier.b + transform.m_Position);
								bezierCurve.c = math.mul(transform.m_Rotation, curve.m_Bezier.c + transform.m_Position);
								bezierCurve.d = math.mul(transform.m_Rotation, curve.m_Bezier.d + transform.m_Position);
							}
							else
							{
								bezierCurve = curve.m_Bezier;
							}
							if (EntitiesExtensions.TryGetComponent<LocalCurveCache>(((ComponentSystemBase)this).EntityManager, subNet, ref localCurveCache))
							{
								CheckCachedValue(ref bezierCurve.a, localCurveCache.m_Curve.a);
								CheckCachedValue(ref bezierCurve.b, localCurveCache.m_Curve.b);
								CheckCachedValue(ref bezierCurve.c, localCurveCache.m_Curve.c);
								CheckCachedValue(ref bezierCurve.d, localCurveCache.m_Curve.d);
							}
							if (EntitiesExtensions.TryGetComponent<Composition>(((ComponentSystemBase)this).EntityManager, subNet, ref composition2) && EntitiesExtensions.TryGetComponent<PlaceableNetComposition>(((ComponentSystemBase)this).EntityManager, composition2.m_Edge, ref placeableNetData))
							{
								EntitiesExtensions.TryGetComponent<Game.Net.Elevation>(((ComponentSystemBase)this).EntityManager, edge.m_Start, ref startElevation);
								EntitiesExtensions.TryGetComponent<Game.Net.Elevation>(((ComponentSystemBase)this).EntityManager, edge.m_End, ref endElevation);
								constructionCost += (uint)NetUtils.GetConstructionCost(curve, startElevation, endElevation, placeableNetData);
								upKeepCost += (uint)NetUtils.GetUpkeepCost(curve, placeableNetData);
							}
						}
						if (EntitiesExtensions.TryGetComponent<EditorContainer>(((ComponentSystemBase)this).EntityManager, subNet, ref editorContainer))
						{
							subLaneList.Add(new ObjectSubLaneInfo
							{
								m_LanePrefab = m_PrefabSystem.GetPrefab<NetLanePrefab>(editorContainer.m_Prefab),
								m_BezierCurve = bezierCurve,
								m_NodeIndex = new int2(GetNodeIndex(edge.m_Start, dictionary2), GetNodeIndex(edge.m_End, dictionary2)),
								m_ParentMesh = new int2(GetParentMesh(edge.m_Start), GetParentMesh(edge.m_End))
							});
							continue;
						}
						List<ObjectSubNetInfo> obj = subNetList;
						ObjectSubNetInfo objectSubNetInfo = new ObjectSubNetInfo();
						PrefabSystem prefabSystem = m_PrefabSystem;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						objectSubNetInfo.m_NetPrefab = prefabSystem.GetPrefab<NetPrefab>(((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(subNet));
						objectSubNetInfo.m_BezierCurve = bezierCurve;
						objectSubNetInfo.m_NodeIndex = new int2(GetNodeIndex(edge.m_Start, dictionary), GetNodeIndex(edge.m_End, dictionary));
						objectSubNetInfo.m_ParentMesh = new int2(GetParentMesh(edge.m_Start), GetParentMesh(edge.m_End));
						objectSubNetInfo.m_Upgrades = array2;
						obj.Add(objectSubNetInfo);
					}
					else if (EntitiesExtensions.TryGetComponent<Game.Net.Node>(((ComponentSystemBase)this).EntityManager, subNet, ref node) && (array2 != null || !EntitiesExtensions.TryGetBuffer<ConnectedEdge>(((ComponentSystemBase)this).EntityManager, subNet, true, ref connectedEdges) || !HasEdgeStartOrEnd(connectedEdges, subNet, instanceEntity)))
					{
						Bezier4x3 val = default(Bezier4x3);
						if (flag)
						{
							val.a = math.mul(transform.m_Rotation, node.m_Position + transform.m_Position);
						}
						else
						{
							val.a = node.m_Position;
						}
						if (EntitiesExtensions.TryGetComponent<LocalTransformCache>(((ComponentSystemBase)this).EntityManager, subNet, ref localTransformCache))
						{
							CheckCachedValue(ref val.a, localTransformCache.m_Position);
						}
						val.b = val.a;
						val.c = val.a;
						val.d = val.a;
						if (EntitiesExtensions.TryGetComponent<EditorContainer>(((ComponentSystemBase)this).EntityManager, subNet, ref editorContainer2))
						{
							subLaneList.Add(new ObjectSubLaneInfo
							{
								m_LanePrefab = m_PrefabSystem.GetPrefab<NetLanePrefab>(editorContainer2.m_Prefab),
								m_BezierCurve = val,
								m_NodeIndex = new int2(GetNodeIndex(subNet, dictionary2)),
								m_ParentMesh = new int2(GetParentMesh(subNet))
							});
							continue;
						}
						List<ObjectSubNetInfo> obj2 = subNetList;
						ObjectSubNetInfo objectSubNetInfo2 = new ObjectSubNetInfo();
						PrefabSystem prefabSystem2 = m_PrefabSystem;
						entityManager = ((ComponentSystemBase)this).EntityManager;
						objectSubNetInfo2.m_NetPrefab = prefabSystem2.GetPrefab<NetPrefab>(((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(subNet));
						objectSubNetInfo2.m_BezierCurve = val;
						objectSubNetInfo2.m_NodeIndex = new int2(GetNodeIndex(subNet, dictionary));
						objectSubNetInfo2.m_ParentMesh = new int2(GetParentMesh(subNet));
						objectSubNetInfo2.m_Upgrades = array2;
						obj2.Add(objectSubNetInfo2);
					}
				}
				return;
			}
		}
		subNetList = null;
		subLaneList = null;
	}

	private int GetNodeIndex(Entity node, Dictionary<Entity, int> dictionary)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!dictionary.TryGetValue(node, out var value))
		{
			value = dictionary.Count;
			dictionary.Add(node, value);
		}
		return value;
	}

	private int GetParentMesh(Entity node)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		LocalTransformCache localTransformCache = default(LocalTransformCache);
		if (EntitiesExtensions.TryGetComponent<LocalTransformCache>(((ComponentSystemBase)this).EntityManager, node, ref localTransformCache))
		{
			return localTransformCache.m_ParentMesh;
		}
		return -1;
	}

	private bool HasEdgeStartOrEnd(DynamicBuffer<ConnectedEdge> connectedEdges, Entity node, Entity instanceEntity)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Edge edge2 = default(Edge);
		Owner owner = default(Owner);
		for (int i = 0; i < connectedEdges.Length; i++)
		{
			Entity edge = connectedEdges[i].m_Edge;
			if (EntitiesExtensions.TryGetComponent<Edge>(((ComponentSystemBase)this).EntityManager, edge, ref edge2) && (edge2.m_Start == node || edge2.m_End == node) && EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, edge, ref owner) && owner.m_Owner == instanceEntity)
			{
				return true;
			}
		}
		return false;
	}

	private void UpdateObjectSubAreas(Entity instanceEntity, PrefabBase prefabBase, ref uint constructionCost, ref uint upKeepCost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		List<ObjectSubAreaInfo> list = ListObjectSubAreas(instanceEntity, ref constructionCost, ref upKeepCost);
		if (list != null && list.Count != 0)
		{
			ObjectSubAreas objectSubAreas = prefabBase.GetComponent<ObjectSubAreas>();
			if ((Object)(object)objectSubAreas == (Object)null)
			{
				objectSubAreas = AddComponent<ObjectSubAreas>(prefabBase);
			}
			objectSubAreas.m_SubAreas = list.ToArray();
		}
		else if ((Object)(object)prefabBase.GetComponent<ObjectSubAreas>() != (Object)null)
		{
			RemoveComponent<ObjectSubAreas>(prefabBase);
		}
	}

	private List<ObjectSubAreaInfo> ListObjectSubAreas(Entity instanceEntity, ref uint constructionCost, ref uint upKeepCost)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
		if (((EntityManager)(ref entityManager)).HasComponent<Game.Areas.SubArea>(instanceEntity))
		{
			entityManager = ((ComponentSystemBase)this).EntityManager;
			DynamicBuffer<Game.Areas.SubArea> buffer = ((EntityManager)(ref entityManager)).GetBuffer<Game.Areas.SubArea>(instanceEntity, true);
			if (buffer.Length != 0)
			{
				Transform inverseParentTransform = default(Transform);
				bool flag = false;
				Transform transform = default(Transform);
				if (EntitiesExtensions.TryGetComponent<Transform>(((ComponentSystemBase)this).EntityManager, instanceEntity, ref transform))
				{
					inverseParentTransform.m_Position = -transform.m_Position;
					inverseParentTransform.m_Rotation = math.inverse(transform.m_Rotation);
					flag = true;
				}
				List<ObjectSubAreaInfo> list = new List<ObjectSubAreaInfo>(buffer.Length);
				Owner owner = default(Owner);
				DynamicBuffer<Game.Areas.Node> val = default(DynamicBuffer<Game.Areas.Node>);
				for (int i = 0; i < buffer.Length; i++)
				{
					Entity area = buffer[i].m_Area;
					if (!EntitiesExtensions.TryGetComponent<Owner>(((ComponentSystemBase)this).EntityManager, area, ref owner))
					{
						continue;
					}
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).HasComponent<Secondary>(area) || !(owner.m_Owner == instanceEntity))
					{
						continue;
					}
					ObjectSubAreaInfo objectSubAreaInfo = new ObjectSubAreaInfo();
					entityManager = ((ComponentSystemBase)this).EntityManager;
					PrefabRef componentData = ((EntityManager)(ref entityManager)).GetComponentData<PrefabRef>(area);
					objectSubAreaInfo.m_AreaPrefab = m_PrefabSystem.GetPrefab<AreaPrefab>(componentData);
					if (EntitiesExtensions.TryGetBuffer<Game.Areas.Node>(((ComponentSystemBase)this).EntityManager, area, true, ref val))
					{
						DynamicBuffer<LocalNodeCache> val2 = default(DynamicBuffer<LocalNodeCache>);
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).HasComponent<LocalNodeCache>(area))
						{
							entityManager = ((ComponentSystemBase)this).EntityManager;
							val2 = ((EntityManager)(ref entityManager)).GetBuffer<LocalNodeCache>(area, true);
						}
						objectSubAreaInfo.m_NodePositions = (float3[])(object)new float3[val.Length];
						objectSubAreaInfo.m_ParentMeshes = new int[val.Length];
						bool flag2 = false;
						for (int j = 0; j < val.Length; j++)
						{
							if (flag)
							{
								objectSubAreaInfo.m_NodePositions[j] = ObjectUtils.WorldToLocal(inverseParentTransform, val[j].m_Position);
							}
							else
							{
								objectSubAreaInfo.m_NodePositions[j] = val[j].m_Position;
							}
							if (val2.IsCreated)
							{
								CheckCachedValue(ref objectSubAreaInfo.m_NodePositions[j], val2[j].m_Position);
								objectSubAreaInfo.m_ParentMeshes[j] = val2[j].m_ParentMesh;
								flag2 |= val2[j].m_ParentMesh >= 0;
							}
							else
							{
								objectSubAreaInfo.m_ParentMeshes[j] = -1;
							}
						}
						if (!flag2)
						{
							objectSubAreaInfo.m_ParentMeshes = null;
						}
					}
					list.Add(objectSubAreaInfo);
				}
				return list;
			}
		}
		return null;
	}

	private static void CheckCachedValue(ref float3 value, float3 cached)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (math.distance(value, cached) < 0.01f)
		{
			value = cached;
		}
	}

	private static void CheckCachedValue(ref quaternion value, quaternion cached)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (MathUtils.RotationAngle(value, cached) < (float)Math.PI / 180f)
		{
			value = cached;
		}
	}

	public static T AddComponent<T>(PrefabBase asset) where T : ComponentBase
	{
		return asset.AddComponent<T>();
	}

	public static void RemoveComponent<T>(PrefabBase asset) where T : ComponentBase
	{
		RemoveComponent(asset, typeof(T));
	}

	public static void RemoveComponent(PrefabBase asset, Type componentType)
	{
		ComponentBase componentExactly = asset.GetComponentExactly(componentType);
		if (!((Object)(object)componentExactly == (Object)null))
		{
			asset.Remove(componentType);
			Object.DestroyImmediate((Object)(object)componentExactly, true);
		}
	}

	[Preserve]
	public ApplyPrefabsSystem()
	{
	}
}
