using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Colossal.Entities;
using Colossal.Serialization.Entities;
using Game.Common;
using Game.Triggers;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Game.UI.InGame;

[CompilerGenerated]
public class ChirpLinkSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public struct CachedChirpData : ISerializable
	{
		public CachedEntityName m_Sender;

		public CachedEntityName[] m_Links;

		public CachedChirpData(NameSystem nameSystem, Chirp chirpData)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			m_Sender = new CachedEntityName(nameSystem, chirpData.m_Sender);
			m_Links = null;
		}

		public CachedChirpData(NameSystem nameSystem, Chirp chirpData, DynamicBuffer<ChirpEntity> chirpEntities)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			m_Sender = new CachedEntityName(nameSystem, chirpData.m_Sender);
			m_Links = new CachedEntityName[chirpEntities.Length];
			for (int i = 0; i < chirpEntities.Length; i++)
			{
				m_Links[i] = new CachedEntityName(nameSystem, chirpEntities[i].m_Entity);
			}
		}

		public CachedChirpData Update(NameSystem nameSystem, Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (m_Sender.m_Entity == entity)
			{
				m_Sender = new CachedEntityName(nameSystem, entity);
			}
			if (m_Links != null)
			{
				for (int i = 0; i < m_Links.Length; i++)
				{
					if (m_Links[i].m_Entity == entity)
					{
						m_Links[i] = new CachedEntityName(nameSystem, entity);
					}
				}
			}
			return this;
		}

		public CachedChirpData Remove(Entity entity)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			if (m_Sender.m_Entity == entity)
			{
				m_Sender.m_Entity = Entity.Null;
			}
			if (m_Links != null)
			{
				for (int i = 0; i < m_Links.Length; i++)
				{
					if (m_Links[i].m_Entity == entity)
					{
						m_Links[i] = new CachedEntityName
						{
							m_Entity = Entity.Null,
							m_Name = m_Links[i].m_Name
						};
					}
				}
			}
			return this;
		}

		public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
		{
			CachedEntityName cachedEntityName = m_Sender;
			((IWriter)writer/*cast due to .constrained prefix*/).Write<CachedEntityName>(cachedEntityName);
			int num = ((m_Links != null) ? m_Links.Length : 0);
			((IWriter)writer/*cast due to .constrained prefix*/).Write(num);
			for (int i = 0; i < num; i++)
			{
				CachedEntityName cachedEntityName2 = m_Links[i];
				((IWriter)writer/*cast due to .constrained prefix*/).Write<CachedEntityName>(cachedEntityName2);
			}
		}

		public void Deserialize<TReader>(TReader reader) where TReader : IReader
		{
			ref CachedEntityName reference = ref m_Sender;
			((IReader)reader/*cast due to .constrained prefix*/).Read<CachedEntityName>(ref reference);
			int num = default(int);
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
			if (num > 0)
			{
				m_Links = new CachedEntityName[num];
				for (int i = 0; i < num; i++)
				{
					ref CachedEntityName reference2 = ref m_Links[i];
					((IReader)reader/*cast due to .constrained prefix*/).Read<CachedEntityName>(ref reference2);
				}
			}
			else
			{
				m_Links = null;
			}
		}
	}

	public struct CachedEntityName : ISerializable
	{
		public Entity m_Entity;

		public NameSystem.Name m_Name;

		public CachedEntityName(NameSystem nameSystem, Entity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			m_Entity = entity;
			m_Name = nameSystem.GetName(entity);
		}

		public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			Entity val = m_Entity;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
			NameSystem.Name name = m_Name;
			((IWriter)writer/*cast due to .constrained prefix*/).Write<NameSystem.Name>(name);
		}

		public void Deserialize<TReader>(TReader reader) where TReader : IReader
		{
			ref Entity reference = ref m_Entity;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref reference);
			ref NameSystem.Name reference2 = ref m_Name;
			((IReader)reader/*cast due to .constrained prefix*/).Read<NameSystem.Name>(ref reference2);
		}
	}

	private NameSystem m_NameSystem;

	private EntityQuery m_CreatedChirpQuery;

	private EntityQuery m_AllChirpsQuery;

	private EntityQuery m_DeletedChirpQuery;

	private EntityQuery m_UpdatedLinkEntityQuery;

	private EntityQuery m_DeletedLinkEntityQuery;

	private Dictionary<Entity, CachedChirpData> m_CachedChirpData;

	[Preserve]
	protected override void OnCreate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		base.OnCreate();
		m_NameSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<NameSystem>();
		m_CreatedChirpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<Chirp>(),
			ComponentType.ReadOnly<Created>(),
			ComponentType.Exclude<Deleted>()
		});
		m_AllChirpsQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Chirp>(),
			ComponentType.Exclude<Deleted>()
		});
		m_DeletedChirpQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<Chirp>(),
			ComponentType.ReadOnly<Deleted>()
		});
		m_UpdatedLinkEntityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[3]
		{
			ComponentType.ReadOnly<ChirpLink>(),
			ComponentType.ReadOnly<Updated>(),
			ComponentType.Exclude<Deleted>()
		});
		m_DeletedLinkEntityQuery = ((ComponentSystemBase)this).GetEntityQuery((ComponentType[])(object)new ComponentType[2]
		{
			ComponentType.ReadOnly<ChirpLink>(),
			ComponentType.ReadOnly<Deleted>()
		});
		m_CachedChirpData = new Dictionary<Entity, CachedChirpData>();
		((ComponentSystemBase)this).RequireAnyForUpdate((EntityQuery[])(object)new EntityQuery[4] { m_CreatedChirpQuery, m_UpdatedLinkEntityQuery, m_DeletedLinkEntityQuery, m_DeletedChirpQuery });
	}

	public bool TryGetData(Entity chirp, out CachedChirpData data)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (m_CachedChirpData.TryGetValue(chirp, out data))
		{
			return true;
		}
		return false;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		EntityManager entityManager;
		if (!((EntityQuery)(ref m_CreatedChirpQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val = ((EntityQuery)(ref m_CreatedChirpQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			NativeArray<Chirp> val2 = ((EntityQuery)(ref m_CreatedChirpQuery)).ToComponentDataArray<Chirp>(AllocatorHandle.op_Implicit((Allocator)3));
			DynamicBuffer<ChirpEntity> chirpEntities = default(DynamicBuffer<ChirpEntity>);
			NativeArray<ChirpEntity> val3 = default(NativeArray<ChirpEntity>);
			for (int i = 0; i < val.Length; i++)
			{
				if (EntitiesExtensions.TryGetBuffer<ChirpEntity>(((ComponentSystemBase)this).EntityManager, val[i], true, ref chirpEntities))
				{
					m_CachedChirpData[val[i]] = new CachedChirpData(m_NameSystem, val2[i], chirpEntities);
					val3._002Ector(chirpEntities.AsNativeArray(), (Allocator)2);
					for (int j = 0; j < val3.Length; j++)
					{
						entityManager = ((ComponentSystemBase)this).EntityManager;
						if (((EntityManager)(ref entityManager)).Exists(val3[j].m_Entity))
						{
							RegisterLink(val3[j].m_Entity, val[i]);
						}
					}
					val3.Dispose();
				}
				else
				{
					m_CachedChirpData[val[i]] = new CachedChirpData(m_NameSystem, val2[i]);
				}
				entityManager = ((ComponentSystemBase)this).EntityManager;
				if (((EntityManager)(ref entityManager)).Exists(val2[i].m_Sender))
				{
					RegisterLink(val2[i].m_Sender, val[i]);
				}
			}
			val.Dispose();
			val2.Dispose();
		}
		if (!((EntityQuery)(ref m_UpdatedLinkEntityQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val4 = ((EntityQuery)(ref m_UpdatedLinkEntityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int k = 0; k < val4.Length; k++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<ChirpLink> buffer = ((EntityManager)(ref entityManager)).GetBuffer<ChirpLink>(val4[k], true);
				for (int l = 0; l < buffer.Length; l++)
				{
					if (m_CachedChirpData.TryGetValue(buffer[l].m_Chirp, out var value))
					{
						m_CachedChirpData[buffer[l].m_Chirp] = value.Update(m_NameSystem, val4[k]);
					}
				}
			}
			val4.Dispose();
		}
		if (!((EntityQuery)(ref m_DeletedLinkEntityQuery)).IsEmptyIgnoreFilter)
		{
			NativeArray<Entity> val5 = ((EntityQuery)(ref m_DeletedLinkEntityQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
			for (int m = 0; m < val5.Length; m++)
			{
				entityManager = ((ComponentSystemBase)this).EntityManager;
				DynamicBuffer<ChirpLink> buffer2 = ((EntityManager)(ref entityManager)).GetBuffer<ChirpLink>(val5[m], true);
				for (int n = 0; n < buffer2.Length; n++)
				{
					if (m_CachedChirpData.TryGetValue(buffer2[n].m_Chirp, out var value2))
					{
						m_CachedChirpData[buffer2[n].m_Chirp] = value2.Remove(val5[m]);
					}
				}
			}
			val5.Dispose();
		}
		if (((EntityQuery)(ref m_DeletedChirpQuery)).IsEmptyIgnoreFilter)
		{
			return;
		}
		NativeArray<Entity> val6 = ((EntityQuery)(ref m_DeletedChirpQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Chirp> val7 = ((EntityQuery)(ref m_DeletedChirpQuery)).ToComponentDataArray<Chirp>(AllocatorHandle.op_Implicit((Allocator)3));
		DynamicBuffer<ChirpEntity> val8 = default(DynamicBuffer<ChirpEntity>);
		NativeArray<ChirpEntity> val9 = default(NativeArray<ChirpEntity>);
		for (int num = 0; num < val6.Length; num++)
		{
			if (EntitiesExtensions.TryGetBuffer<ChirpEntity>(((ComponentSystemBase)this).EntityManager, val6[num], true, ref val8))
			{
				val9._002Ector(val8.AsNativeArray(), (Allocator)2);
				for (int num2 = 0; num2 < val9.Length; num2++)
				{
					UnregisterLink(val9[num2].m_Entity, val6[num]);
				}
				val9.Dispose();
			}
			UnregisterLink(val7[num].m_Sender, val6[num]);
			m_CachedChirpData.Remove(val6[num]);
		}
		val7.Dispose();
		val6.Dispose();
	}

	private void RegisterLink(Entity linkEntity, Entity chirpEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<ChirpLink> links = default(DynamicBuffer<ChirpLink>);
		if (!EntitiesExtensions.TryGetBuffer<ChirpLink>(((ComponentSystemBase)this).EntityManager, linkEntity, false, ref links))
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			links = ((EntityManager)(ref entityManager)).AddBuffer<ChirpLink>(linkEntity);
		}
		if (!LinkExists(links, chirpEntity))
		{
			links.Add(new ChirpLink
			{
				m_Chirp = chirpEntity
			});
		}
	}

	private void UnregisterLink(Entity linkEntity, Entity chirpEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		DynamicBuffer<ChirpLink> val = default(DynamicBuffer<ChirpLink>);
		if (!EntitiesExtensions.TryGetBuffer<ChirpLink>(((ComponentSystemBase)this).EntityManager, linkEntity, false, ref val))
		{
			return;
		}
		for (int i = 0; i < val.Length; i++)
		{
			if (val[i].m_Chirp == chirpEntity)
			{
				val.RemoveAt(i);
				break;
			}
		}
		if (val.Length == 0)
		{
			EntityManager entityManager = ((ComponentSystemBase)this).EntityManager;
			((EntityManager)(ref entityManager)).RemoveComponent<ChirpLink>(linkEntity);
		}
	}

	private bool LinkExists(DynamicBuffer<ChirpLink> links, Entity link)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < links.Length; i++)
		{
			if (links[i].m_Chirp == link)
			{
				return true;
			}
		}
		return false;
	}

	private void Initialize()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		m_CachedChirpData.Clear();
		NativeArray<Chirp> val = ((EntityQuery)(ref m_AllChirpsQuery)).ToComponentDataArray<Chirp>(AllocatorHandle.op_Implicit((Allocator)3));
		NativeArray<Entity> val2 = ((EntityQuery)(ref m_AllChirpsQuery)).ToEntityArray(AllocatorHandle.op_Implicit((Allocator)3));
		DynamicBuffer<ChirpEntity> chirpEntities = default(DynamicBuffer<ChirpEntity>);
		NativeArray<ChirpEntity> val3 = default(NativeArray<ChirpEntity>);
		for (int i = 0; i < val2.Length; i++)
		{
			EntityManager entityManager;
			if (EntitiesExtensions.TryGetBuffer<ChirpEntity>(((ComponentSystemBase)this).EntityManager, val2[i], true, ref chirpEntities))
			{
				m_CachedChirpData[val2[i]] = new CachedChirpData(m_NameSystem, val[i], chirpEntities);
				val3._002Ector(chirpEntities.AsNativeArray(), (Allocator)2);
				for (int j = 0; j < val3.Length; j++)
				{
					entityManager = ((ComponentSystemBase)this).EntityManager;
					if (((EntityManager)(ref entityManager)).Exists(val3[j].m_Entity))
					{
						RegisterLink(val3[j].m_Entity, val2[i]);
					}
				}
				val3.Dispose();
			}
			else
			{
				m_CachedChirpData[val2[i]] = new CachedChirpData(m_NameSystem, val[i]);
			}
			entityManager = ((ComponentSystemBase)this).EntityManager;
			if (((EntityManager)(ref entityManager)).Exists(val[i].m_Sender))
			{
				RegisterLink(val[i].m_Sender, val2[i]);
			}
		}
		val2.Dispose();
		val.Dispose();
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		int count = m_CachedChirpData.Count;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(count);
		foreach (KeyValuePair<Entity, CachedChirpData> item in m_CachedChirpData)
		{
			Entity key = item.Key;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(key);
			CachedChirpData value = item.Value;
			((IWriter)writer/*cast due to .constrained prefix*/).Write<CachedChirpData>(value);
		}
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		m_CachedChirpData.Clear();
		int num = default(int);
		((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
		Entity key = default(Entity);
		CachedChirpData value = default(CachedChirpData);
		for (int i = 0; i < num; i++)
		{
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref key);
			((IReader)reader/*cast due to .constrained prefix*/).Read<CachedChirpData>(ref value);
			m_CachedChirpData[key] = value;
		}
	}

	public void SetDefaults(Context context)
	{
		m_CachedChirpData.Clear();
	}

	protected override void OnGameLoaded(Context serializationContext)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.OnGameLoaded(serializationContext);
		if (m_CachedChirpData.Count == 0)
		{
			Initialize();
		}
	}

	[Preserve]
	public ChirpLinkSystem()
	{
	}
}
