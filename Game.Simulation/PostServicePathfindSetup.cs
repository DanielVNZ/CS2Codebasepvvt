using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Companies;
using Game.Economy;
using Game.Objects;
using Game.Pathfind;
using Game.Prefabs;
using Game.Routes;
using Game.Tools;
using Game.Vehicles;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Game.Simulation;

public struct PostServicePathfindSetup
{
	[BurstCompile]
	private struct SetupPostVansJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.PostFacility> m_PostFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Vehicles.PostVan> m_PostVanType;

		[ReadOnly]
		public ComponentTypeHandle<PathOwner> m_PathOwnerType;

		[ReadOnly]
		public ComponentTypeHandle<Owner> m_OwnerType;

		[ReadOnly]
		public BufferTypeHandle<PathElement> m_PathElementType;

		[ReadOnly]
		public BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

		[ReadOnly]
		public ComponentLookup<PathInformation> m_PathInformationData;

		[ReadOnly]
		public BufferLookup<PathElement> m_PathElements;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0209: Unknown result type (might be due to invalid IL or missing references)
			//IL_022a: Unknown result type (might be due to invalid IL or missing references)
			//IL_022f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_0266: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0306: Unknown result type (might be due to invalid IL or missing references)
			//IL_036b: Unknown result type (might be due to invalid IL or missing references)
			//IL_036d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0346: Unknown result type (might be due to invalid IL or missing references)
			//IL_034a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0351: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<Game.Buildings.PostFacility> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.PostFacility>(ref m_PostFacilityType);
			if (nativeArray2.Length != 0)
			{
				for (int i = 0; i < nativeArray2.Length; i++)
				{
					Game.Buildings.PostFacility postFacility = nativeArray2[i];
					for (int j = 0; j < m_SetupData.Length; j++)
					{
						m_SetupData.GetItem(j, out var entity, out var targetSeeker);
						if (((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Export) != SetupTargetFlags.None && (postFacility.m_Flags & PostFacilityFlags.CanDeliverMailWithVan) != 0) || ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Import) != SetupTargetFlags.None && (postFacility.m_Flags & PostFacilityFlags.CanCollectMailWithVan) != 0))
						{
							Entity val = nativeArray[i];
							if (AreaUtils.CheckServiceDistrict(entity, val, m_ServiceDistricts))
							{
								float cost = targetSeeker.m_PathfindParameters.m_Weights.time * 10f;
								targetSeeker.FindTargets(val, cost);
							}
						}
					}
				}
				return;
			}
			NativeArray<Game.Vehicles.PostVan> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Vehicles.PostVan>(ref m_PostVanType);
			if (nativeArray3.Length == 0)
			{
				return;
			}
			NativeArray<PathOwner> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PathOwner>(ref m_PathOwnerType);
			NativeArray<Owner> nativeArray5 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Owner>(ref m_OwnerType);
			BufferAccessor<PathElement> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<PathElement>(ref m_PathElementType);
			BufferAccessor<ServiceDispatch> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<ServiceDispatch>(ref m_ServiceDispatchType);
			PathInformation pathInformation = default(PathInformation);
			DynamicBuffer<PathElement> val5 = default(DynamicBuffer<PathElement>);
			for (int k = 0; k < nativeArray3.Length; k++)
			{
				Entity val2 = nativeArray[k];
				Game.Vehicles.PostVan postVan = nativeArray3[k];
				if ((postVan.m_State & PostVanFlags.Disabled) != 0)
				{
					continue;
				}
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out var entity2, out var targetSeeker2);
					if (((targetSeeker2.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Export) != SetupTargetFlags.None && (postVan.m_State & PostVanFlags.EstimatedEmpty) != 0) || ((targetSeeker2.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Import) != SetupTargetFlags.None && (postVan.m_State & PostVanFlags.EstimatedFull) != 0) || (nativeArray5.Length != 0 && !AreaUtils.CheckServiceDistrict(entity2, nativeArray5[k].m_Owner, m_ServiceDistricts)))
					{
						continue;
					}
					if ((postVan.m_State & PostVanFlags.Returning) != 0 || nativeArray4.Length == 0)
					{
						targetSeeker2.FindTargets(val2, 0f);
						continue;
					}
					PathOwner pathOwner = nativeArray4[k];
					DynamicBuffer<ServiceDispatch> val3 = bufferAccessor2[k];
					int num = math.min(postVan.m_RequestCount, val3.Length);
					PathElement pathElement = default(PathElement);
					float num2 = 0f;
					bool flag = false;
					if (num >= 1)
					{
						DynamicBuffer<PathElement> val4 = bufferAccessor[k];
						if (pathOwner.m_ElementIndex < val4.Length)
						{
							num2 += (float)(val4.Length - pathOwner.m_ElementIndex) * postVan.m_PathElementTime * targetSeeker2.m_PathfindParameters.m_Weights.time;
							pathElement = val4[val4.Length - 1];
							flag = true;
						}
					}
					for (int m = 1; m < num; m++)
					{
						Entity request = val3[m].m_Request;
						if (m_PathInformationData.TryGetComponent(request, ref pathInformation))
						{
							num2 += pathInformation.m_Duration * targetSeeker2.m_PathfindParameters.m_Weights.time;
						}
						if (m_PathElements.TryGetBuffer(request, ref val5) && val5.Length != 0)
						{
							pathElement = val5[val5.Length - 1];
							flag = true;
						}
					}
					if (flag)
					{
						targetSeeker2.m_Buffer.Enqueue(new PathTarget(val2, pathElement.m_Target, pathElement.m_TargetDelta.y, num2));
					}
					else
					{
						targetSeeker2.FindTargets(val2, val2, num2, EdgeFlags.DefaultMask, allowAccessRestriction: true, num >= 1);
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SetupMailTransferJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Buildings.PostFacility> m_PostFacilityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

		[ReadOnly]
		public BufferTypeHandle<Resources> m_ResourcesType;

		[ReadOnly]
		public BufferTypeHandle<TradeCost> m_TradeCostType;

		[ReadOnly]
		public BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

		[ReadOnly]
		public ComponentLookup<StorageLimitData> m_StorageLimitData;

		[ReadOnly]
		public ComponentLookup<StorageCompanyData> m_StorageCompanyData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_031d: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_032c: Unknown result type (might be due to invalid IL or missing references)
			//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0382: Unknown result type (might be due to invalid IL or missing references)
			//IL_0390: Unknown result type (might be due to invalid IL or missing references)
			//IL_0369: Unknown result type (might be due to invalid IL or missing references)
			//IL_0344: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			bool flag = ((ArchetypeChunk)(ref chunk)).Has<Game.Objects.OutsideConnection>(ref m_OutsideConnectionType);
			Entity entity;
			if (!flag)
			{
				NativeArray<Game.Buildings.PostFacility> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Buildings.PostFacility>(ref m_PostFacilityType);
				if (nativeArray2.Length != 0)
				{
					for (int i = 0; i < nativeArray2.Length; i++)
					{
						Game.Buildings.PostFacility postFacility = nativeArray2[i];
						for (int j = 0; j < m_SetupData.Length; j++)
						{
							m_SetupData.GetItem(j, out entity, out var targetSeeker);
							Resource resource = targetSeeker.m_SetupQueueTarget.m_Resource;
							if ((resource & (Resource)12288uL) == Resource.NoResource || ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.RequireTransport) != SetupTargetFlags.None && (postFacility.m_Flags & PostFacilityFlags.HasAvailableTrucks) == 0))
							{
								continue;
							}
							if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Import) != SetupTargetFlags.None)
							{
								if (((resource & Resource.UnsortedMail) != Resource.NoResource && (postFacility.m_Flags & PostFacilityFlags.AcceptsUnsortedMail) != 0) || ((resource & Resource.LocalMail) != Resource.NoResource && (postFacility.m_Flags & PostFacilityFlags.AcceptsLocalMail) != 0))
								{
									Entity entity2 = nativeArray[i];
									targetSeeker.FindTargets(entity2, 0f);
								}
							}
							else if ((targetSeeker.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Export) != SetupTargetFlags.None && (((resource & Resource.UnsortedMail) != Resource.NoResource && (postFacility.m_Flags & PostFacilityFlags.DeliversUnsortedMail) != 0) || ((resource & Resource.LocalMail) != Resource.NoResource && (postFacility.m_Flags & PostFacilityFlags.DeliversLocalMail) != 0)))
							{
								Entity entity3 = nativeArray[i];
								targetSeeker.FindTargets(entity3, 0f);
							}
						}
					}
					return;
				}
			}
			NativeArray<PrefabRef> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			BufferAccessor<Resources> bufferAccessor = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<Resources>(ref m_ResourcesType);
			BufferAccessor<TradeCost> bufferAccessor2 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<TradeCost>(ref m_TradeCostType);
			BufferAccessor<InstalledUpgrade> bufferAccessor3 = ((ArchetypeChunk)(ref chunk)).GetBufferAccessor<InstalledUpgrade>(ref m_InstalledUpgradeType);
			for (int k = 0; k < nativeArray.Length; k++)
			{
				Entity entity4 = nativeArray[k];
				Entity prefab = nativeArray3[k].m_Prefab;
				StorageCompanyData storageCompanyData = m_StorageCompanyData[prefab];
				for (int l = 0; l < m_SetupData.Length; l++)
				{
					m_SetupData.GetItem(l, out entity, out var targetSeeker2);
					Resource resource2 = targetSeeker2.m_SetupQueueTarget.m_Resource;
					int value = targetSeeker2.m_SetupQueueTarget.m_Value;
					switch (resource2)
					{
					case Resource.LocalMail:
						if ((targetSeeker2.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Import) != 0 && flag)
						{
							continue;
						}
						break;
					case Resource.UnsortedMail:
					case Resource.OutgoingMail:
						if ((targetSeeker2.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Export) != 0 && flag)
						{
							continue;
						}
						break;
					}
					if ((resource2 & storageCompanyData.m_StoredResources) == Resource.NoResource)
					{
						continue;
					}
					float num = EconomyUtils.GetResources(resource2, bufferAccessor[k]);
					if ((targetSeeker2.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Export) != SetupTargetFlags.None)
					{
						if (num >= (float)value)
						{
							num -= EconomyUtils.GetTradeCost(resource2, bufferAccessor2[k]).m_BuyCost * (float)value;
							if (num >= (float)value)
							{
								targetSeeker2.FindTargets(entity4, math.max(0f, 500f - num));
							}
						}
					}
					else
					{
						if ((targetSeeker2.m_SetupQueueTarget.m_Flags & SetupTargetFlags.Import) == 0)
						{
							continue;
						}
						int num2 = value;
						if (m_StorageLimitData.HasComponent(prefab))
						{
							StorageLimitData data = m_StorageLimitData[prefab];
							if (bufferAccessor3.Length != 0)
							{
								UpgradeUtils.CombineStats<StorageLimitData>(ref data, bufferAccessor3[k], ref targetSeeker2.m_PrefabRef, ref m_StorageLimitData);
							}
							num2 = data.m_Limit - EconomyUtils.GetResources(resource2, bufferAccessor[k]);
						}
						if (num2 >= value)
						{
							targetSeeker2.FindTargets(entity4, math.max(0f, -0.1f * (float)num2 + EconomyUtils.GetTradeCost(resource2, bufferAccessor2[k]).m_SellCost * (float)value));
						}
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct SetupMailBoxesJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<PrefabRef> m_PrefabRefType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.MailBox> m_MailBoxType;

		[ReadOnly]
		public ComponentTypeHandle<Game.Routes.TransportStop> m_TransportStopType;

		[ReadOnly]
		public ComponentLookup<MailBoxData> m_MailBoxData;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<PrefabRef> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PrefabRef>(ref m_PrefabRefType);
			NativeArray<Game.Routes.MailBox> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Routes.MailBox>(ref m_MailBoxType);
			NativeArray<Game.Routes.TransportStop> nativeArray4 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<Game.Routes.TransportStop>(ref m_TransportStopType);
			MailBoxData mailBoxData = default(MailBoxData);
			for (int i = 0; i < nativeArray.Length; i++)
			{
				Entity entity = nativeArray[i];
				Entity prefab = nativeArray2[i].m_Prefab;
				Game.Routes.MailBox mailBox = nativeArray3[i];
				if (!m_MailBoxData.TryGetComponent(prefab, ref mailBoxData) || mailBox.m_MailAmount >= mailBoxData.m_MailCapacity)
				{
					continue;
				}
				for (int j = 0; j < m_SetupData.Length; j++)
				{
					m_SetupData.GetItem(j, out var _, out var targetSeeker);
					float num = (float)mailBox.m_MailAmount * 100f / (float)mailBoxData.m_MailCapacity;
					if (nativeArray4.Length != 0)
					{
						num += 10f * (1f - nativeArray4[i].m_ComfortFactor) * targetSeeker.m_PathfindParameters.m_Weights.m_Value.w;
					}
					targetSeeker.FindTargets(entity, num);
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	[BurstCompile]
	private struct PostVanRequestsJob : IJobChunk
	{
		[ReadOnly]
		public EntityTypeHandle m_EntityType;

		[ReadOnly]
		public ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

		[ReadOnly]
		public ComponentTypeHandle<PostVanRequest> m_PostVanRequestType;

		[ReadOnly]
		public ComponentLookup<PostVanRequest> m_PostVanRequestData;

		[ReadOnly]
		public ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

		[ReadOnly]
		public ComponentLookup<Game.Buildings.PostFacility> m_PostFacilityData;

		[ReadOnly]
		public ComponentLookup<Game.Vehicles.PostVan> m_PostVanData;

		[ReadOnly]
		public BufferLookup<ServiceDistrict> m_ServiceDistricts;

		public PathfindSetupSystem.SetupData m_SetupData;

		public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_019c: Unknown result type (might be due to invalid IL or missing references)
			NativeArray<Entity> nativeArray = ((ArchetypeChunk)(ref chunk)).GetNativeArray(m_EntityType);
			NativeArray<ServiceRequest> nativeArray2 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<ServiceRequest>(ref m_ServiceRequestType);
			NativeArray<PostVanRequest> nativeArray3 = ((ArchetypeChunk)(ref chunk)).GetNativeArray<PostVanRequest>(ref m_PostVanRequestType);
			PostVanRequest postVanRequest = default(PostVanRequest);
			Game.Vehicles.PostVan postVan = default(Game.Vehicles.PostVan);
			Owner owner2 = default(Owner);
			Game.Buildings.PostFacility postFacility = default(Game.Buildings.PostFacility);
			CurrentDistrict currentDistrict = default(CurrentDistrict);
			for (int i = 0; i < m_SetupData.Length; i++)
			{
				m_SetupData.GetItem(i, out var _, out var owner, out var targetSeeker);
				if (!m_PostVanRequestData.TryGetComponent(owner, ref postVanRequest))
				{
					continue;
				}
				Random random = targetSeeker.m_RandomSeed.GetRandom(unfilteredChunkIndex);
				Entity service = Entity.Null;
				bool flag = false;
				bool flag2 = false;
				if (m_PostVanData.TryGetComponent(postVanRequest.m_Target, ref postVan))
				{
					flag = (postVan.m_State & PostVanFlags.EstimatedFull) == 0;
					flag2 = (postVan.m_State & PostVanFlags.EstimatedEmpty) == 0;
					if (targetSeeker.m_Owner.TryGetComponent(postVanRequest.m_Target, ref owner2))
					{
						service = owner2.m_Owner;
					}
				}
				else
				{
					if (!m_PostFacilityData.TryGetComponent(postVanRequest.m_Target, ref postFacility))
					{
						continue;
					}
					flag = (postFacility.m_Flags & PostFacilityFlags.CanCollectMailWithVan) != 0;
					flag2 = (postFacility.m_Flags & PostFacilityFlags.CanDeliverMailWithVan) != 0;
					service = postVanRequest.m_Target;
				}
				for (int j = 0; j < nativeArray3.Length; j++)
				{
					if ((nativeArray2[j].m_Flags & ServiceRequestFlags.Reversed) != 0)
					{
						continue;
					}
					PostVanRequest postVanRequest2 = nativeArray3[j];
					if (((postVanRequest2.m_Flags & PostVanRequestFlags.Collect) == 0 || flag) && ((postVanRequest2.m_Flags & PostVanRequestFlags.Deliver) == 0 || flag2))
					{
						Entity district = Entity.Null;
						if (m_CurrentDistrictData.TryGetComponent(postVanRequest2.m_Target, ref currentDistrict))
						{
							district = currentDistrict.m_District;
						}
						if (AreaUtils.CheckServiceDistrict(district, service, m_ServiceDistricts))
						{
							float cost = ((Random)(ref random)).NextFloat(30f);
							targetSeeker.FindTargets(nativeArray[j], postVanRequest2.m_Target, cost, EdgeFlags.DefaultMask, allowAccessRestriction: true, navigationEnd: false);
						}
					}
				}
			}
		}

		void IJobChunk.Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
		{
			Execute(in chunk, unfilteredChunkIndex, useEnabledMask, in chunkEnabledMask);
		}
	}

	private EntityQuery m_PostVanQuery;

	private EntityQuery m_MailTransferQuery;

	private EntityQuery m_MailBoxQuery;

	private EntityQuery m_PostVanRequestQuery;

	private EntityTypeHandle m_EntityType;

	private ComponentTypeHandle<PathOwner> m_PathOwnerType;

	private ComponentTypeHandle<Owner> m_OwnerType;

	private ComponentTypeHandle<Game.Objects.OutsideConnection> m_OutsideConnectionType;

	private ComponentTypeHandle<ServiceRequest> m_ServiceRequestType;

	private ComponentTypeHandle<PostVanRequest> m_PostVanRequestType;

	private ComponentTypeHandle<Game.Buildings.PostFacility> m_PostFacilityType;

	private ComponentTypeHandle<Game.Vehicles.PostVan> m_PostVanType;

	private ComponentTypeHandle<Game.Routes.MailBox> m_MailBoxType;

	private ComponentTypeHandle<Game.Routes.TransportStop> m_TransportStopType;

	private ComponentTypeHandle<PrefabRef> m_PrefabRefType;

	private BufferTypeHandle<PathElement> m_PathElementType;

	private BufferTypeHandle<ServiceDispatch> m_ServiceDispatchType;

	private BufferTypeHandle<Resources> m_ResourcesType;

	private BufferTypeHandle<TradeCost> m_TradeCostType;

	private BufferTypeHandle<InstalledUpgrade> m_InstalledUpgradeType;

	private ComponentLookup<PathInformation> m_PathInformationData;

	private ComponentLookup<PostVanRequest> m_PostVanRequestData;

	private ComponentLookup<Game.Buildings.PostFacility> m_PostFacilityData;

	private ComponentLookup<Game.Vehicles.PostVan> m_PostVanData;

	private ComponentLookup<CurrentDistrict> m_CurrentDistrictData;

	private ComponentLookup<StorageLimitData> m_StorageLimitData;

	private ComponentLookup<StorageCompanyData> m_StorageCompanyData;

	private ComponentLookup<MailBoxData> m_MailBoxData;

	private BufferLookup<PathElement> m_PathElements;

	private BufferLookup<ServiceDistrict> m_ServiceDistricts;

	public PostServicePathfindSetup(PathfindSetupSystem system)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		EntityQueryDesc[] array = new EntityQueryDesc[1];
		EntityQueryDesc val = new EntityQueryDesc();
		val.Any = (ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Game.Buildings.PostFacility>(),
			ComponentType.ReadOnly<Game.Vehicles.PostVan>()
		};
		val.None = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Game.Buildings.ServiceUpgrade>(),
			ComponentType.ReadOnly<Temp>()
		};
		array[0] = val;
		m_PostVanQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array);
		EntityQueryDesc[] array2 = new EntityQueryDesc[2];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Game.Buildings.PostFacility>(),
			ComponentType.ReadOnly<ServiceDispatch>(),
			ComponentType.ReadOnly<PrefabRef>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[0] = val;
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[4]
		{
			ComponentType.ReadOnly<Game.Companies.StorageCompany>(),
			ComponentType.ReadOnly<PrefabRef>(),
			ComponentType.ReadOnly<Resources>(),
			ComponentType.ReadOnly<TradeCost>()
		};
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array2[1] = val;
		m_MailTransferQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array2);
		EntityQueryDesc[] array3 = new EntityQueryDesc[1];
		val = new EntityQueryDesc();
		val.All = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<PrefabRef>() };
		val.Any = (ComponentType[])(object)new ComponentType[1] { ComponentType.ReadOnly<Game.Routes.MailBox>() };
		val.None = (ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Deleted>(),
			ComponentType.ReadOnly<Destroyed>(),
			ComponentType.ReadOnly<Temp>()
		};
		array3[0] = val;
		m_MailBoxQuery = system.GetSetupQuery((EntityQueryDesc[])(object)array3);
		m_PostVanRequestQuery = system.GetSetupQuery(ComponentType.ReadOnly<PostVanRequest>(), ComponentType.Exclude<Dispatched>(), ComponentType.Exclude<PathInformation>());
		m_EntityType = ((ComponentSystemBase)system).GetEntityTypeHandle();
		m_PathOwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<PathOwner>(true);
		m_OwnerType = ((ComponentSystemBase)system).GetComponentTypeHandle<Owner>(true);
		m_OutsideConnectionType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Objects.OutsideConnection>(true);
		m_ServiceRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<ServiceRequest>(true);
		m_PostVanRequestType = ((ComponentSystemBase)system).GetComponentTypeHandle<PostVanRequest>(true);
		m_PostFacilityType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Buildings.PostFacility>(true);
		m_PostVanType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Vehicles.PostVan>(true);
		m_MailBoxType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Routes.MailBox>(true);
		m_TransportStopType = ((ComponentSystemBase)system).GetComponentTypeHandle<Game.Routes.TransportStop>(true);
		m_PrefabRefType = ((ComponentSystemBase)system).GetComponentTypeHandle<PrefabRef>(true);
		m_PathElementType = ((ComponentSystemBase)system).GetBufferTypeHandle<PathElement>(true);
		m_ServiceDispatchType = ((ComponentSystemBase)system).GetBufferTypeHandle<ServiceDispatch>(true);
		m_ResourcesType = ((ComponentSystemBase)system).GetBufferTypeHandle<Resources>(true);
		m_TradeCostType = ((ComponentSystemBase)system).GetBufferTypeHandle<TradeCost>(true);
		m_InstalledUpgradeType = ((ComponentSystemBase)system).GetBufferTypeHandle<InstalledUpgrade>(true);
		m_PathInformationData = ((SystemBase)system).GetComponentLookup<PathInformation>(true);
		m_PostVanRequestData = ((SystemBase)system).GetComponentLookup<PostVanRequest>(true);
		m_PostFacilityData = ((SystemBase)system).GetComponentLookup<Game.Buildings.PostFacility>(true);
		m_PostVanData = ((SystemBase)system).GetComponentLookup<Game.Vehicles.PostVan>(true);
		m_CurrentDistrictData = ((SystemBase)system).GetComponentLookup<CurrentDistrict>(true);
		m_StorageLimitData = ((SystemBase)system).GetComponentLookup<StorageLimitData>(true);
		m_StorageCompanyData = ((SystemBase)system).GetComponentLookup<StorageCompanyData>(true);
		m_MailBoxData = ((SystemBase)system).GetComponentLookup<MailBoxData>(true);
		m_PathElements = ((SystemBase)system).GetBufferLookup<PathElement>(true);
		m_ServiceDistricts = ((SystemBase)system).GetBufferLookup<ServiceDistrict>(true);
	}

	public JobHandle SetupPostVans(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_PostFacilityType.Update((SystemBase)(object)system);
		m_PostVanType.Update((SystemBase)(object)system);
		m_PathOwnerType.Update((SystemBase)(object)system);
		m_OwnerType.Update((SystemBase)(object)system);
		m_PathElementType.Update((SystemBase)(object)system);
		m_ServiceDispatchType.Update((SystemBase)(object)system);
		m_PathInformationData.Update((SystemBase)(object)system);
		m_PathElements.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupPostVansJob>(new SetupPostVansJob
		{
			m_EntityType = m_EntityType,
			m_PostFacilityType = m_PostFacilityType,
			m_PostVanType = m_PostVanType,
			m_PathOwnerType = m_PathOwnerType,
			m_OwnerType = m_OwnerType,
			m_PathElementType = m_PathElementType,
			m_ServiceDispatchType = m_ServiceDispatchType,
			m_PathInformationData = m_PathInformationData,
			m_PathElements = m_PathElements,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_PostVanQuery, inputDeps);
	}

	public JobHandle SetupMailTransfer(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_PostFacilityType.Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_OutsideConnectionType.Update((SystemBase)(object)system);
		m_ResourcesType.Update((SystemBase)(object)system);
		m_TradeCostType.Update((SystemBase)(object)system);
		m_InstalledUpgradeType.Update((SystemBase)(object)system);
		m_StorageCompanyData.Update((SystemBase)(object)system);
		m_StorageLimitData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupMailTransferJob>(new SetupMailTransferJob
		{
			m_EntityType = m_EntityType,
			m_PostFacilityType = m_PostFacilityType,
			m_PrefabRefType = m_PrefabRefType,
			m_OutsideConnectionType = m_OutsideConnectionType,
			m_ResourcesType = m_ResourcesType,
			m_TradeCostType = m_TradeCostType,
			m_InstalledUpgradeType = m_InstalledUpgradeType,
			m_StorageCompanyData = m_StorageCompanyData,
			m_StorageLimitData = m_StorageLimitData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_MailTransferQuery, inputDeps);
	}

	public JobHandle SetupMailBoxes(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_PrefabRefType.Update((SystemBase)(object)system);
		m_MailBoxType.Update((SystemBase)(object)system);
		m_TransportStopType.Update((SystemBase)(object)system);
		m_MailBoxData.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<SetupMailBoxesJob>(new SetupMailBoxesJob
		{
			m_EntityType = m_EntityType,
			m_PrefabRefType = m_PrefabRefType,
			m_MailBoxType = m_MailBoxType,
			m_TransportStopType = m_TransportStopType,
			m_MailBoxData = m_MailBoxData,
			m_SetupData = setupData
		}, m_MailBoxQuery, inputDeps);
	}

	public JobHandle SetupPostVanRequest(PathfindSetupSystem system, PathfindSetupSystem.SetupData setupData, JobHandle inputDeps)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		((EntityTypeHandle)(ref m_EntityType)).Update((SystemBase)(object)system);
		m_ServiceRequestType.Update((SystemBase)(object)system);
		m_PostVanRequestType.Update((SystemBase)(object)system);
		m_PostVanRequestData.Update((SystemBase)(object)system);
		m_CurrentDistrictData.Update((SystemBase)(object)system);
		m_PostFacilityData.Update((SystemBase)(object)system);
		m_PostVanData.Update((SystemBase)(object)system);
		m_ServiceDistricts.Update((SystemBase)(object)system);
		return JobChunkExtensions.ScheduleParallel<PostVanRequestsJob>(new PostVanRequestsJob
		{
			m_EntityType = m_EntityType,
			m_ServiceRequestType = m_ServiceRequestType,
			m_PostVanRequestType = m_PostVanRequestType,
			m_PostVanRequestData = m_PostVanRequestData,
			m_CurrentDistrictData = m_CurrentDistrictData,
			m_PostFacilityData = m_PostFacilityData,
			m_PostVanData = m_PostVanData,
			m_ServiceDistricts = m_ServiceDistricts,
			m_SetupData = setupData
		}, m_PostVanRequestQuery, inputDeps);
	}
}
