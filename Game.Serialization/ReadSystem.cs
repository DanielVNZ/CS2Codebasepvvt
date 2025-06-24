using System;
using Colossal;
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

public class ReadSystem : GameSystemBase, IReadBufferProvider<ReadBuffer>
{
	private struct BufferHeader
	{
		public int size;

		public int compressedSize;
	}

	private LoadGameSystem m_DeserializationSystem;

	private SerializerSystem m_SerializerSystem;

	private StreamBinaryReader m_Reader;

	[Preserve]
	protected override void OnCreate()
	{
		base.OnCreate();
		m_DeserializationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<LoadGameSystem>();
		m_SerializerSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SerializerSystem>();
	}

	[Preserve]
	protected override void OnDestroy()
	{
		Clear();
		base.OnDestroy();
	}

	public unsafe ReadBuffer GetBuffer(BufferFormat format)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		if (m_DeserializationSystem.dataDescriptor == AsyncReadDescriptor.Invalid)
		{
			return null;
		}
		if (m_Reader == null)
		{
			m_Reader = new StreamBinaryReader(m_DeserializationSystem.dataDescriptor, 65536L);
		}
		BufferHeader data = default(BufferHeader);
		if (format.IsCompressed())
		{
			ReadData(m_Reader, out data);
			m_SerializerSystem.totalSize += sizeof(BufferHeader);
		}
		else
		{
			ReadData(m_Reader, out data.size);
			m_SerializerSystem.totalSize += 4;
		}
		ReadBuffer readBuffer = new ReadBuffer(data.size);
		m_SerializerSystem.totalSize += data.size;
		if (format.IsCompressed())
		{
			NativeArray<byte> val = default(NativeArray<byte>);
			val._002Ector(data.compressedSize, (Allocator)4, (NativeArrayOptions)1);
			ReadData(m_Reader, val);
			PerformanceCounter val2 = PerformanceCounter.Start((Action<TimeSpan>)delegate(TimeSpan t)
			{
				COSystemBase.baseLog.VerboseFormat("Decompressed in {0}ms", (object)t.TotalMilliseconds);
			});
			try
			{
				JobHandle val3 = CompressionUtils.Decompress(SerializationUtils.BufferToCompressionFormat(format), NativeSlice<byte>.op_Implicit(val), NativeSlice<byte>.op_Implicit(readBuffer.buffer), default(JobHandle));
				((JobHandle)(ref val3)).Complete();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			val.Dispose();
		}
		else if ((int)format == 0)
		{
			ReadData(m_Reader, readBuffer.buffer);
		}
		else
		{
			COSystemBase.baseLog.WarnFormat("Unsupported BufferFormat {0}", (object)format);
		}
		return readBuffer;
	}

	public unsafe ReadBuffer GetBuffer(BufferFormat format, out JobHandle dependency)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		dependency = default(JobHandle);
		if (m_DeserializationSystem.dataDescriptor == AsyncReadDescriptor.Invalid)
		{
			return null;
		}
		if (m_Reader == null)
		{
			m_Reader = new StreamBinaryReader(m_DeserializationSystem.dataDescriptor, 65536L);
		}
		BufferHeader data = default(BufferHeader);
		if (format.IsCompressed())
		{
			ReadData(m_Reader, out data);
			m_SerializerSystem.totalSize += sizeof(BufferHeader);
		}
		else
		{
			ReadData(m_Reader, out data.size);
			m_SerializerSystem.totalSize += 4;
		}
		ReadBuffer readBuffer = new ReadBuffer(data.size);
		m_SerializerSystem.totalSize += data.size;
		if (format.IsCompressed())
		{
			NativeArray<byte> val = default(NativeArray<byte>);
			val._002Ector(data.compressedSize, (Allocator)4, (NativeArrayOptions)1);
			ReadData(m_Reader, val, out dependency);
			dependency = CompressionUtils.Decompress(SerializationUtils.BufferToCompressionFormat(format), NativeSlice<byte>.op_Implicit(val), NativeSlice<byte>.op_Implicit(readBuffer.buffer), dependency);
			val.Dispose(dependency);
		}
		else if ((int)format == 0)
		{
			ReadData(m_Reader, readBuffer.buffer, out dependency);
		}
		else
		{
			COSystemBase.baseLog.WarnFormat("Unsupported BufferFormat {0}", (object)format);
		}
		return readBuffer;
	}

	[Preserve]
	protected override void OnUpdate()
	{
		Clear();
	}

	private void Clear()
	{
		if (m_Reader != null)
		{
			m_Reader.Dispose();
			m_Reader = null;
		}
	}

	private unsafe static void ReadData<T>(StreamBinaryReader reader, out T data) where T : unmanaged
	{
		data = default(T);
		void* ptr = UnsafeUtility.AddressOf<T>(ref data);
		reader.ReadBytes(ptr, sizeof(T));
	}

	private unsafe static void ReadData(StreamBinaryReader reader, NativeArray<byte> data)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		void* unsafePtr = NativeArrayUnsafeUtility.GetUnsafePtr<byte>(data);
		reader.ReadBytes(unsafePtr, data.Length);
	}

	private unsafe static void ReadData(StreamBinaryReader reader, NativeArray<byte> data, out JobHandle dependency)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		void* unsafePtr = NativeArrayUnsafeUtility.GetUnsafePtr<byte>(data);
		reader.ReadBytes(unsafePtr, data.Length, ref dependency);
	}

	[Preserve]
	public ReadSystem()
	{
	}
}
