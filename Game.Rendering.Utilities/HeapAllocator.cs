using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Game.Rendering.Utilities;

public struct HeapAllocator : IDisposable
{
	[DebuggerDisplay("Size = {Size}, Alignment = {Alignment}")]
	private struct SizeBin : IComparable<SizeBin>, IEquatable<SizeBin>
	{
		public ulong sizeClass;

		public int blocksId;

		public ulong Size => sizeClass >> 6;

		public int AlignmentLog2 => (int)sizeClass & 0x3F;

		public uint Alignment => (uint)(1 << AlignmentLog2);

		public SizeBin(ulong size, uint alignment = 1u)
		{
			int num = math.tzcnt(alignment);
			num = math.min(63, num);
			sizeClass = (size << 6) | (uint)num;
			blocksId = -1;
		}

		public SizeBin(HeapBlock block)
		{
			int num = math.tzcnt(block.begin);
			num = math.min(63, num);
			sizeClass = (block.Length << 6) | (uint)num;
			blocksId = -1;
		}

		public int CompareTo(SizeBin other)
		{
			return sizeClass.CompareTo(other.sizeClass);
		}

		public bool Equals(SizeBin other)
		{
			return CompareTo(other) == 0;
		}

		public bool HasCompatibleAlignment(SizeBin requiredAlignment)
		{
			int alignmentLog = AlignmentLog2;
			int alignmentLog2 = requiredAlignment.AlignmentLog2;
			return alignmentLog >= alignmentLog2;
		}
	}

	private struct BlocksOfSize : IDisposable
	{
		private unsafe UnsafeList<HeapBlock>* m_Blocks;

		public unsafe bool Empty => m_Blocks->Length == 0;

		public unsafe int Length => m_Blocks->Length;

		public unsafe BlocksOfSize(int dummy)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			m_Blocks = (UnsafeList<HeapBlock>*)UnsafeUtility.Malloc((long)UnsafeUtility.SizeOf<UnsafeList<HeapBlock>>(), UnsafeUtility.AlignOf<UnsafeList<HeapBlock>>(), (Allocator)4);
			UnsafeUtility.MemClear((void*)m_Blocks, (long)UnsafeUtility.SizeOf<UnsafeList<HeapBlock>>());
			System.Runtime.CompilerServices.Unsafe.Write(&m_Blocks->Allocator, AllocatorHandle.op_Implicit((Allocator)4));
		}

		public unsafe void Push(HeapBlock block)
		{
			m_Blocks->Add(ref block);
		}

		public unsafe HeapBlock Pop()
		{
			int length = m_Blocks->Length;
			if (length == 0)
			{
				return default(HeapBlock);
			}
			HeapBlock result = Block(length - 1);
			m_Blocks->Resize(length - 1, (NativeArrayOptions)0);
			return result;
		}

		public unsafe bool Remove(HeapBlock block)
		{
			for (int i = 0; i < m_Blocks->Length; i++)
			{
				if (block.CompareTo(Block(i)) == 0)
				{
					m_Blocks->RemoveAtSwapBack(i);
					return true;
				}
			}
			return false;
		}

		public unsafe void Dispose()
		{
			m_Blocks->Dispose();
			UnsafeUtility.Free((void*)m_Blocks, (Allocator)4);
		}

		public unsafe HeapBlock Block(int i)
		{
			return UnsafeUtility.ReadArrayElement<HeapBlock>((void*)m_Blocks->Ptr, i);
		}
	}

	public const int MaxAlignmentLog2 = 63;

	public const int AlignmentBits = 6;

	private NativeList<SizeBin> m_SizeBins;

	private NativeList<BlocksOfSize> m_Blocks;

	private NativeList<int> m_BlocksFreelist;

	private NativeParallelHashMap<ulong, ulong> m_FreeEndpoints;

	private ulong m_Size;

	private ulong m_Free;

	private readonly int m_MinimumAlignmentLog2;

	private bool m_IsCreated;

	public uint MinimumAlignment => (uint)(1 << m_MinimumAlignmentLog2);

	public ulong FreeSpace => m_Free;

	public ulong UsedSpace => m_Size - m_Free;

	public ulong OnePastHighestUsedAddress
	{
		get
		{
			ulong result = default(ulong);
			if (!m_FreeEndpoints.TryGetValue(m_Size, ref result))
			{
				return m_Size;
			}
			return result;
		}
	}

	public ulong Size => m_Size;

	public bool Empty => m_Free == m_Size;

	public bool Full => m_Free == 0;

	public bool IsCreated => m_IsCreated;

	public HeapAllocator(ulong size = 0uL, uint minimumAlignment = 1u)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		m_SizeBins = new NativeList<SizeBin>(AllocatorHandle.op_Implicit((Allocator)4));
		m_Blocks = new NativeList<BlocksOfSize>(AllocatorHandle.op_Implicit((Allocator)4));
		m_BlocksFreelist = new NativeList<int>(AllocatorHandle.op_Implicit((Allocator)4));
		m_FreeEndpoints = new NativeParallelHashMap<ulong, ulong>(0, AllocatorHandle.op_Implicit((Allocator)4));
		m_Size = 0uL;
		m_Free = 0uL;
		m_MinimumAlignmentLog2 = math.tzcnt(minimumAlignment);
		m_IsCreated = true;
		Resize(size);
	}

	public void Clear()
	{
		ulong size = m_Size;
		m_SizeBins.Clear();
		m_Blocks.Clear();
		m_BlocksFreelist.Clear();
		m_FreeEndpoints.Clear();
		m_Size = 0uL;
		m_Free = 0uL;
		Resize(size);
	}

	public void Dispose()
	{
		if (IsCreated)
		{
			for (int i = 0; i < m_Blocks.Length; i++)
			{
				m_Blocks[i].Dispose();
			}
			m_FreeEndpoints.Dispose();
			m_Blocks.Dispose();
			m_BlocksFreelist.Dispose();
			m_SizeBins.Dispose();
			m_IsCreated = false;
		}
	}

	public bool Resize(ulong newSize)
	{
		if (newSize == m_Size)
		{
			return true;
		}
		if (newSize > m_Size)
		{
			ulong size = newSize - m_Size;
			HeapBlock block = HeapBlock.OfSize(m_Size, size);
			Release(block);
			m_Size = newSize;
			return true;
		}
		return false;
	}

	public HeapBlock Allocate(ulong size, uint alignment = 1u)
	{
		size = NextAligned(size, m_MinimumAlignmentLog2);
		alignment = math.max(alignment, MinimumAlignment);
		SizeBin sizeBin = new SizeBin(size, alignment);
		for (int i = FindSmallestSufficientBin(sizeBin); i < m_SizeBins.Length; i++)
		{
			SizeBin bin = m_SizeBins[i];
			if (CanFitAllocation(sizeBin, bin))
			{
				HeapBlock block = PopBlockFromBin(bin, i);
				return CutAllocationFromBlock(sizeBin, block);
			}
		}
		return default(HeapBlock);
	}

	public void Release(HeapBlock block)
	{
		block = Coalesce(block);
		SizeBin bin = new SizeBin(block);
		int num = FindSmallestSufficientBin(bin);
		if (num >= m_SizeBins.Length || bin.CompareTo(m_SizeBins[num]) != 0)
		{
			num = AddNewBin(ref bin, num);
		}
		m_Blocks[m_SizeBins[num].blocksId].Push(block);
		m_Free += block.Length;
		m_FreeEndpoints[block.begin] = block.end;
		m_FreeEndpoints[block.end] = block.begin;
	}

	public void DebugValidateInternalState()
	{
		int length = m_SizeBins.Length;
		int length2 = m_BlocksFreelist.Length;
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < m_Blocks.Length; i++)
		{
			if (m_Blocks[i].Empty)
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		Assert.AreEqual(length, num2, "There should be exactly one non-empty block list per size bin");
		Assert.AreEqual(num, length2, "All empty block lists should be in the free list");
		for (int j = 0; j < m_BlocksFreelist.Length; j++)
		{
			int num3 = m_BlocksFreelist[j];
			Assert.IsTrue(m_Blocks[num3].Empty, "There should be only empty block lists in the free list");
		}
		ulong num4 = 0uL;
		int num5 = 0;
		ulong num6 = default(ulong);
		ulong num7 = default(ulong);
		for (int k = 0; k < m_SizeBins.Length; k++)
		{
			SizeBin sizeBin = m_SizeBins[k];
			ulong size = sizeBin.Size;
			uint alignment = sizeBin.Alignment;
			BlocksOfSize blocksOfSize = m_Blocks[sizeBin.blocksId];
			Assert.IsFalse(blocksOfSize.Empty, "All block lists should be non-empty, empty lists should be removed");
			int length3 = blocksOfSize.Length;
			for (int l = 0; l < length3; l++)
			{
				HeapBlock block = blocksOfSize.Block(l);
				SizeBin sizeBin2 = new SizeBin(block);
				Assert.AreEqual(size, sizeBin2.Size, "Block size should match its bin");
				Assert.AreEqual(alignment, sizeBin2.Alignment, "Block alignment should match its bin");
				num4 += block.Length;
				if (m_FreeEndpoints.TryGetValue(block.begin, ref num6))
				{
					Assert.AreEqual(block.end, num6, "Free block end does not match stored endpoint");
				}
				else
				{
					Assert.IsTrue(false, "No end endpoint found for free block");
				}
				if (m_FreeEndpoints.TryGetValue(block.end, ref num7))
				{
					Assert.AreEqual(block.begin, num7, "Free block begin does not match stored endpoint");
				}
				else
				{
					Assert.IsTrue(false, "No begin endpoint found for free block");
				}
				num5++;
			}
		}
		Assert.AreEqual(num4, FreeSpace, "Free size reported incorrectly");
		Assert.IsTrue(num4 <= Size, "Amount of free size larger than maximum");
		Assert.AreEqual(2 * num5, m_FreeEndpoints.Count(), "Each free block should have exactly 2 stored endpoints");
	}

	private int FindSmallestSufficientBin(SizeBin needle)
	{
		if (m_SizeBins.Length == 0)
		{
			return 0;
		}
		int num = 0;
		int num2 = m_SizeBins.Length;
		int num4;
		while (true)
		{
			int num3 = (num2 - num) / 2;
			if (num3 == 0)
			{
				if (needle.CompareTo(m_SizeBins[num]) <= 0)
				{
					return num;
				}
				return num + 1;
			}
			num4 = num + num3;
			int num5 = needle.CompareTo(m_SizeBins[num4]);
			if (num5 < 0)
			{
				num2 = num4;
				continue;
			}
			if (num5 <= 0)
			{
				break;
			}
			num = num4;
		}
		return num4;
	}

	private unsafe int AddNewBin(ref SizeBin bin, int index)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (m_BlocksFreelist.IsEmpty)
		{
			bin.blocksId = m_Blocks.Length;
			ref NativeList<BlocksOfSize> blocks = ref m_Blocks;
			BlocksOfSize blocksOfSize = new BlocksOfSize(0);
			blocks.Add(ref blocksOfSize);
		}
		else
		{
			int num = m_BlocksFreelist.Length - 1;
			bin.blocksId = m_BlocksFreelist[num];
			m_BlocksFreelist.ResizeUninitialized(num);
		}
		int num2 = m_SizeBins.Length - index;
		m_SizeBins.ResizeUninitialized(m_SizeBins.Length + 1);
		SizeBin* unsafePtr = NativeListUnsafeUtility.GetUnsafePtr<SizeBin>(m_SizeBins);
		UnsafeUtility.MemMove((void*)(unsafePtr + (index + 1)), (void*)(unsafePtr + index), (long)(num2 * UnsafeUtility.SizeOf<SizeBin>()));
		unsafePtr[index] = bin;
		return index;
	}

	private unsafe void RemoveBinIfEmpty(SizeBin bin, int index)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (m_Blocks[bin.blocksId].Empty)
		{
			int num = m_SizeBins.Length - (index + 1);
			SizeBin* unsafePtr = NativeListUnsafeUtility.GetUnsafePtr<SizeBin>(m_SizeBins);
			UnsafeUtility.MemMove((void*)(unsafePtr + index), (void*)(unsafePtr + (index + 1)), (long)(num * UnsafeUtility.SizeOf<SizeBin>()));
			m_SizeBins.ResizeUninitialized(m_SizeBins.Length - 1);
			m_BlocksFreelist.Add(ref bin.blocksId);
		}
	}

	private HeapBlock PopBlockFromBin(SizeBin bin, int index)
	{
		HeapBlock heapBlock = m_Blocks[bin.blocksId].Pop();
		RemoveEndpoints(heapBlock);
		m_Free -= heapBlock.Length;
		RemoveBinIfEmpty(bin, index);
		return heapBlock;
	}

	private void RemoveEndpoints(HeapBlock block)
	{
		m_FreeEndpoints.Remove(block.begin);
		m_FreeEndpoints.Remove(block.end);
	}

	private void RemoveFreeBlock(HeapBlock block)
	{
		RemoveEndpoints(block);
		SizeBin needle = new SizeBin(block);
		int num = FindSmallestSufficientBin(needle);
		m_Blocks[m_SizeBins[num].blocksId].Remove(block);
		RemoveBinIfEmpty(m_SizeBins[num], num);
		m_Free -= block.Length;
	}

	private HeapBlock Coalesce(HeapBlock block, ulong endpoint)
	{
		ulong num = default(ulong);
		if (m_FreeEndpoints.TryGetValue(endpoint, ref num))
		{
			if (endpoint == block.begin)
			{
				HeapBlock block2 = new HeapBlock(num, block.begin);
				RemoveFreeBlock(block2);
				return new HeapBlock(block2.begin, block.end);
			}
			HeapBlock block3 = new HeapBlock(block.end, num);
			RemoveFreeBlock(block3);
			return new HeapBlock(block.begin, block3.end);
		}
		return block;
	}

	private HeapBlock Coalesce(HeapBlock block)
	{
		block = Coalesce(block, block.begin);
		block = Coalesce(block, block.end);
		return block;
	}

	private bool CanFitAllocation(SizeBin allocation, SizeBin bin)
	{
		if (m_Blocks[bin.blocksId].Empty)
		{
			return false;
		}
		if (bin.HasCompatibleAlignment(allocation))
		{
			return true;
		}
		return bin.Size >= allocation.Size + allocation.Alignment;
	}

	private static ulong NextAligned(ulong offset, int alignmentLog2)
	{
		int num = (1 << alignmentLog2) - 1;
		return (ulong)((long)offset + (long)num >>> alignmentLog2 << alignmentLog2);
	}

	private HeapBlock CutAllocationFromBlock(SizeBin allocation, HeapBlock block)
	{
		if (allocation.Size == block.Length)
		{
			return block;
		}
		ulong num = NextAligned(block.begin, allocation.AlignmentLog2);
		ulong num2 = num + allocation.Size;
		if (num > block.begin)
		{
			Release(new HeapBlock(block.begin, num));
		}
		if (num2 < block.end)
		{
			Release(new HeapBlock(num2, block.end));
		}
		return new HeapBlock(num, num2);
	}
}
