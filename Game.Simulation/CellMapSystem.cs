using Colossal.Mathematics;
using Colossal.Serialization.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Game.Simulation;

public abstract class CellMapSystem<T> : GameSystemBase where T : struct, ISerializable
{
	[BurstCompile]
	internal struct SerializeJob<TWriter> : IJob where TWriter : struct, IWriter
	{
		[ReadOnly]
		public int m_Stride;

		[ReadOnly]
		public NativeArray<T> m_Map;

		public EntityWriterData m_WriterData;

		public void Execute()
		{
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			TWriter writer = ((EntityWriterData)(ref m_WriterData)).GetWriter<TWriter>();
			if (m_Stride != 0 && m_Map.Length != 0)
			{
				NativeList<byte> val = default(NativeList<byte>);
				val._002Ector(1000, AllocatorHandle.op_Implicit((Allocator)2));
				((IWriter)((EntityWriterData)(ref m_WriterData)).GetWriter<TWriter>(val)/*cast due to .constrained prefix*/).Write<T>(m_Map);
				((IWriter)writer/*cast due to .constrained prefix*/).Write(-m_Map.Length);
				((IWriter)writer/*cast due to .constrained prefix*/).Write(val.Length);
				((IWriter)writer/*cast due to .constrained prefix*/).Write(val.AsArray(), m_Stride);
				val.Dispose();
			}
			else
			{
				((IWriter)writer/*cast due to .constrained prefix*/).Write(m_Map.Length);
				((IWriter)writer/*cast due to .constrained prefix*/).Write<T>(m_Map);
			}
		}
	}

	[BurstCompile]
	internal struct DeserializeJob<TReader> : IJob where TReader : struct, IReader
	{
		[ReadOnly]
		public int m_Stride;

		public NativeArray<T> m_Map;

		public EntityReaderData m_ReaderData;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			TReader reader = ((EntityReaderData)(ref m_ReaderData)).GetReader<TReader>();
			Context context = ((IReader)reader).context;
			if (!(((Context)(ref context)).version > Version.stormWater))
			{
				return;
			}
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version > Version.cellMapLengths)
			{
				int num = default(int);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref num);
				if (m_Map.Length == num)
				{
					((IReader)reader/*cast due to .constrained prefix*/).Read<T>(m_Map);
				}
				else if (m_Map.Length == -num)
				{
					int num2 = default(int);
					((IReader)reader/*cast due to .constrained prefix*/).Read(ref num2);
					NativeArray<byte> val = default(NativeArray<byte>);
					val._002Ector(num2, (Allocator)2, (NativeArrayOptions)1);
					NativeReference<int> val2 = default(NativeReference<int>);
					val2._002Ector(0, AllocatorHandle.op_Implicit((Allocator)2));
					((IReader)reader/*cast due to .constrained prefix*/).Read(val, m_Stride);
					((IReader)((EntityReaderData)(ref m_ReaderData)).GetReader<TReader>(val, val2)/*cast due to .constrained prefix*/).Read<T>(m_Map);
					val.Dispose();
					val2.Dispose();
				}
			}
			else
			{
				((IReader)reader/*cast due to .constrained prefix*/).Read<T>(m_Map);
			}
		}
	}

	[BurstCompile]
	private struct SetDefaultsJob : IJob
	{
		public NativeArray<T> m_Map;

		public void Execute()
		{
			for (int i = 0; i < m_Map.Length; i++)
			{
				m_Map[i] = default(T);
			}
		}
	}

	public static readonly int kMapSize = 14336;

	protected JobHandle m_ReadDependencies;

	protected JobHandle m_WriteDependencies;

	protected NativeArray<T> m_Map;

	protected int2 m_TextureSize;

	public JobHandle Serialize<TWriter>(EntityWriterData writerData, JobHandle inputDeps) where TWriter : struct, IWriter
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		int stride = 0;
		object obj = default(T);
		IStrideSerializable val = (IStrideSerializable)((obj is IStrideSerializable) ? obj : null);
		if (val != null)
		{
			stride = val.GetStride(((IWriter)((EntityWriterData)(ref writerData)).GetWriter<TWriter>()).context);
		}
		JobHandle val2 = IJobExtensions.Schedule<SerializeJob<TWriter>>(new SerializeJob<TWriter>
		{
			m_Stride = stride,
			m_Map = m_Map,
			m_WriterData = writerData
		}, JobHandle.CombineDependencies(inputDeps, m_WriteDependencies));
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, val2);
		return val2;
	}

	public virtual JobHandle Deserialize<TReader>(EntityReaderData readerData, JobHandle inputDeps) where TReader : struct, IReader
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		int stride = 0;
		object obj = default(T);
		IStrideSerializable val = (IStrideSerializable)((obj is IStrideSerializable) ? obj : null);
		if (val != null)
		{
			stride = val.GetStride(((IReader)((EntityReaderData)(ref readerData)).GetReader<TReader>()).context);
		}
		DeserializeJob<TReader> deserializeJob = new DeserializeJob<TReader>
		{
			m_Stride = stride,
			m_Map = m_Map,
			m_ReaderData = readerData
		};
		m_WriteDependencies = IJobExtensions.Schedule<DeserializeJob<TReader>>(deserializeJob, JobHandle.CombineDependencies(inputDeps, m_ReadDependencies, m_WriteDependencies));
		return m_WriteDependencies;
	}

	public virtual JobHandle SetDefaults(Context context)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		SetDefaultsJob setDefaultsJob = new SetDefaultsJob
		{
			m_Map = m_Map
		};
		m_WriteDependencies = IJobExtensions.Schedule<SetDefaultsJob>(setDefaultsJob, JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies));
		return m_WriteDependencies;
	}

	public NativeArray<T> GetMap(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_WriteDependencies : JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies));
		return m_Map;
	}

	public CellMapData<T> GetData(bool readOnly, out JobHandle dependencies)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		dependencies = (readOnly ? m_WriteDependencies : JobHandle.CombineDependencies(m_ReadDependencies, m_WriteDependencies));
		return new CellMapData<T>
		{
			m_Buffer = m_Map,
			m_CellSize = float2.op_Implicit(kMapSize) / float2.op_Implicit(m_TextureSize),
			m_TextureSize = m_TextureSize
		};
	}

	public void AddReader(JobHandle jobHandle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_ReadDependencies = JobHandle.CombineDependencies(m_ReadDependencies, jobHandle);
	}

	public void AddWriter(JobHandle jobHandle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_WriteDependencies = jobHandle;
	}

	public static float3 GetCellCenter(int index, int textureSize)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		int num = index % textureSize;
		int num2 = index / textureSize;
		int num3 = kMapSize / textureSize;
		return new float3(-0.5f * (float)kMapSize + ((float)num + 0.5f) * (float)num3, 0f, -0.5f * (float)kMapSize + ((float)num2 + 0.5f) * (float)num3);
	}

	public static Bounds3 GetCellBounds(int index, int textureSize)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		int num = index % textureSize;
		int num2 = index / textureSize;
		int num3 = kMapSize / textureSize;
		return new Bounds3(new float3(-0.5f * (float)kMapSize + (float)(num * num3), -100000f, -0.5f * (float)kMapSize + (float)(num2 * num3)), new float3(-0.5f * (float)kMapSize + ((float)num + 1f) * (float)num3, 100000f, -0.5f * (float)kMapSize + ((float)num2 + 1f) * (float)num3));
	}

	public static float3 GetCellCenter(int2 cell, int textureSize)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		int num = kMapSize / textureSize;
		return new float3(-0.5f * (float)kMapSize + ((float)cell.x + 0.5f) * (float)num, 0f, -0.5f * (float)kMapSize + ((float)cell.y + 0.5f) * (float)num);
	}

	public static float2 GetCellCoords(float3 position, int mapSize, int textureSize)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return (0.5f + ((float3)(ref position)).xz / (float)mapSize) * (float)textureSize;
	}

	public static int2 GetCell(float3 position, int mapSize, int textureSize)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return (int2)math.floor(GetCellCoords(position, mapSize, textureSize));
	}

	protected void CreateTextures(int textureSize)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		m_Map = new NativeArray<T>(textureSize * textureSize, (Allocator)4, (NativeArrayOptions)1);
		m_TextureSize = new int2(textureSize, textureSize);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_ReadDependencies)).Complete();
		((JobHandle)(ref m_WriteDependencies)).Complete();
		m_Map.Dispose();
		base.OnDestroy();
	}

	[Preserve]
	protected CellMapSystem()
	{
	}
}
