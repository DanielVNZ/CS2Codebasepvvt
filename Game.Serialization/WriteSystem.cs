using System.Collections.Generic;
using System.Runtime.InteropServices;
using Colossal.AssetPipeline.Native;
using Colossal.Compression;
using Colossal.Entities;
using Colossal.IO.AssetDatabase;
using Colossal.Serialization.Entities;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine.Scripting;

namespace Game.Serialization;

public class WriteSystem : GameSystemBase, IWriteBufferProvider<WriteBuffer>
{
	private struct WriteRawBufferJob : IJob
	{
		[ReadOnly]
		public NativeList<byte> m_Buffer;

		public GCHandle m_WriterHandle;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			StreamBinaryWriter val = (StreamBinaryWriter)m_WriterHandle.Target;
			WriteData(val, m_Buffer.Length);
			WriteData(val, m_Buffer.AsArray());
		}
	}

	private struct WriteCompressedBufferJob : IJob
	{
		[ReadOnly]
		public CompressedBytesStorage m_CompressedData;

		public int m_UncompressedSize;

		public GCHandle m_WriterHandle;

		public unsafe void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Expected O, but got Unknown
			StreamBinaryWriter val = (StreamBinaryWriter)m_WriterHandle.Target;
			BufferHeader data = default(BufferHeader);
			data.size = m_UncompressedSize;
			void* bytes = ((CompressedBytesStorage)(ref m_CompressedData)).GetBytes(ref data.compressedSize);
			WriteData(val, data);
			val.WriteBytes(bytes, data.compressedSize);
		}
	}

	private struct DisposeWriterJob : IJob
	{
		public GCHandle m_WriterHandle;

		public void Execute()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			((StreamBinaryWriter)m_WriterHandle.Target).Dispose();
			m_WriterHandle.Free();
		}
	}

	private struct BufferHeader
	{
		public int size;

		public int compressedSize;
	}

	private SaveGameSystem m_SerializationSystem;

	private SerializerSystem m_SerializerSystem;

	private List<(WriteBuffer, BufferFormat)> m_Buffers;

	private JobHandle m_WriteDependency;

	private GCHandle m_WriterHandle;

	public JobHandle writeDependency => m_WriteDependency;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_SerializationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SaveGameSystem>();
		m_SerializerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SerializerSystem>();
		m_Buffers = new List<(WriteBuffer, BufferFormat)>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		((JobHandle)(ref m_WriteDependency)).Complete();
		for (int i = 0; i < m_Buffers.Count; i++)
		{
			m_Buffers[i].Item1.Dispose();
		}
		base.OnDestroy();
	}

	public WriteBuffer AddBuffer(BufferFormat format)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < m_Buffers.Count; i++)
		{
			var (writeBuffer, format2) = m_Buffers[i];
			if (!writeBuffer.isCompleted)
			{
				break;
			}
			WriteBuffer(writeBuffer, format2);
			num++;
		}
		if (num != 0)
		{
			m_Buffers.RemoveRange(0, num);
		}
		WriteBuffer writeBuffer2 = new WriteBuffer();
		m_Buffers.Add((writeBuffer2, format));
		return writeBuffer2;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < m_Buffers.Count; i++)
		{
			var (buffer, format) = m_Buffers[i];
			WriteBuffer(buffer, format);
		}
		m_Buffers.Clear();
		if (m_WriterHandle.IsAllocated)
		{
			DisposeWriterJob disposeWriterJob = new DisposeWriterJob
			{
				m_WriterHandle = m_WriterHandle
			};
			m_WriterHandle = default(GCHandle);
			m_WriteDependency = IJobExtensions.Schedule<DisposeWriterJob>(disposeWriterJob, m_WriteDependency);
		}
	}

	private void WriteBuffer(WriteBuffer buffer, BufferFormat format)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		if (!m_WriterHandle.IsAllocated)
		{
			StreamBinaryWriter value = new StreamBinaryWriter(m_SerializationSystem.stream, 65536);
			m_WriterHandle = GCHandle.Alloc(value);
		}
		buffer.CompleteDependencies();
		if ((int)format == 0)
		{
			m_SerializerSystem.totalSize += 4 + buffer.buffer.Length;
			WriteRawBufferJob writeRawBufferJob = new WriteRawBufferJob
			{
				m_Buffer = buffer.buffer,
				m_WriterHandle = m_WriterHandle
			};
			m_WriteDependency = IJobExtensions.Schedule<WriteRawBufferJob>(writeRawBufferJob, m_WriteDependency);
			buffer.buffer.Dispose(m_WriteDependency);
		}
		else if (format.IsCompressed())
		{
			m_SerializerSystem.totalSize += 8 + buffer.buffer.Length;
			CompressionFormat val = SerializationUtils.BufferToCompressionFormat(format);
			CompressedBytesStorage val2 = default(CompressedBytesStorage);
			((CompressedBytesStorage)(ref val2))._002Ector(val, buffer.buffer.Length, (Allocator)4);
			int num = 3;
			JobHandle val3 = CompressionUtils.Compress(val, NativeSlice<byte>.op_Implicit(buffer.buffer.AsArray()), val2, default(JobHandle), num);
			WriteCompressedBufferJob writeCompressedBufferJob = new WriteCompressedBufferJob
			{
				m_CompressedData = val2,
				m_UncompressedSize = buffer.buffer.Length,
				m_WriterHandle = m_WriterHandle
			};
			buffer.buffer.Dispose(val3);
			m_WriteDependency = IJobExtensions.Schedule<WriteCompressedBufferJob>(writeCompressedBufferJob, JobHandle.CombineDependencies(m_WriteDependency, val3));
			((CompressedBytesStorage)(ref val2)).Dispose(m_WriteDependency);
		}
		else
		{
			COSystemBase.baseLog.WarnFormat("Unsupported BufferFormat {0}", (object)format);
		}
	}

	private unsafe static void WriteData<T>(StreamBinaryWriter writer, T data) where T : unmanaged
	{
		void* ptr = UnsafeUtility.AddressOf<T>(ref data);
		writer.WriteBytes(ptr, sizeof(T));
	}

	private unsafe static void WriteData(StreamBinaryWriter writer, NativeArray<byte> data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		void* unsafeReadOnlyPtr = NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(data);
		writer.WriteBytes(unsafeReadOnlyPtr, data.Length);
	}

	private unsafe static void WriteData(StreamBinaryWriter writer, NativeSlice<byte> data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		void* unsafeReadOnlyPtr = NativeSliceUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(data);
		writer.WriteBytes(unsafeReadOnlyPtr, data.Length);
	}

	[Preserve]
	public WriteSystem()
	{
	}
}
