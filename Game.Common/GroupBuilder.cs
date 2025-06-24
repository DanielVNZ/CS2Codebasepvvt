using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.Common;

public struct GroupBuilder<T> where T : unmanaged, IEquatable<T>
{
	public struct Result : IComparable<Result>
	{
		public T m_Item;

		public int m_Group;

		public Result(T item, int group)
		{
			m_Item = item;
			m_Group = group;
		}

		public int CompareTo(Result other)
		{
			return m_Group - other.m_Group;
		}
	}

	public struct Iterator
	{
		public int m_StartIndex;

		public int m_GroupIndex;

		public Iterator(int groupIndex)
		{
			m_StartIndex = 0;
			m_GroupIndex = groupIndex;
		}
	}

	private NativeList<int> m_Groups;

	private NativeParallelHashMap<T, int> m_GroupIndex;

	private NativeList<Result> m_Results;

	public GroupBuilder(Allocator allocator)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		m_Groups = new NativeList<int>(32, AllocatorHandle.op_Implicit(allocator));
		m_GroupIndex = new NativeParallelHashMap<T, int>(32, AllocatorHandle.op_Implicit(allocator));
		m_Results = (NativeList<Result>)new NativeList<GroupBuilder<Result>.Result>(32, AllocatorHandle.op_Implicit(allocator));
	}

	public void Dispose()
	{
		m_Groups.Dispose();
		m_GroupIndex.Dispose();
		System.Runtime.CompilerServices.Unsafe.As<NativeList<Result>, NativeList<GroupBuilder<Result>.Result>>(ref m_Results).Dispose();
	}

	public void AddSingle(T item)
	{
		int num = default(int);
		if (!m_GroupIndex.TryGetValue(item, ref num))
		{
			num = CreateGroup();
			AddToGroup(item, num);
		}
	}

	public void AddPair(T item1, T item2)
	{
		int num = default(int);
		int num2 = default(int);
		if (m_GroupIndex.TryGetValue(item1, ref num))
		{
			num = m_Groups[num];
			if (m_GroupIndex.TryGetValue(item2, ref num2))
			{
				num2 = m_Groups[num2];
				if (num != num2)
				{
					MergeGroups(num, num2);
				}
			}
			else
			{
				AddToGroup(item2, num);
			}
		}
		else if (m_GroupIndex.TryGetValue(item2, ref num2))
		{
			num2 = m_Groups[num2];
			AddToGroup(item1, num2);
		}
		else
		{
			int index = CreateGroup();
			AddToGroup(item1, index);
			AddToGroup(item2, index);
		}
	}

	public unsafe bool TryGetFirstGroup(out NativeArray<Result> group, out Iterator iterator)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<int> val = m_Groups.AsArray();
		NativeArray<Result> val2 = System.Runtime.CompilerServices.Unsafe.As<NativeList<Result>, NativeList<GroupBuilder<Result>.Result>>(ref m_Results).AsArray();
		if (((NativeArray<GroupBuilder<Result>.Result>*)(&val2))->Length == 0)
		{
			group = default(NativeArray<Result>);
			iterator = default(Iterator);
			return false;
		}
		for (int i = 0; i < val.Length; i++)
		{
			val[i] = val[val[i]];
		}
		for (int j = 0; j < ((NativeArray<GroupBuilder<Result>.Result>*)(&val2))->Length; j++)
		{
			Result result = (*(NativeArray<GroupBuilder<Result>.Result>*)(&val2))[j];
			result.m_Group = val[result.m_Group];
			(*(NativeArray<GroupBuilder<Result>.Result>*)(&val2))[j] = (GroupBuilder<Result>.Result)result;
		}
		NativeSortExtension.Sort<Result>(val2);
		iterator = new Iterator((*(NativeArray<GroupBuilder<Result>.Result>*)(&val2))[0].m_Group);
		return TryGetNextGroup(out group, ref iterator);
	}

	public unsafe bool TryGetNextGroup(out NativeArray<Result> group, ref Iterator iterator)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<Result> val = System.Runtime.CompilerServices.Unsafe.As<NativeList<Result>, NativeList<GroupBuilder<Result>.Result>>(ref m_Results).AsArray();
		for (int i = iterator.m_StartIndex + 1; i < ((NativeArray<GroupBuilder<Result>.Result>*)(&val))->Length; i++)
		{
			Result result = (*(NativeArray<GroupBuilder<Result>.Result>*)(&val))[i];
			if (result.m_Group != iterator.m_GroupIndex)
			{
				group = ((NativeArray<GroupBuilder<Result>.Result>*)(&val))->GetSubArray(iterator.m_StartIndex, i - iterator.m_StartIndex);
				iterator.m_StartIndex = i;
				iterator.m_GroupIndex = result.m_Group;
				return true;
			}
		}
		if (((NativeArray<GroupBuilder<Result>.Result>*)(&val))->Length > iterator.m_StartIndex)
		{
			group = ((NativeArray<GroupBuilder<Result>.Result>*)(&val))->GetSubArray(iterator.m_StartIndex, ((NativeArray<GroupBuilder<Result>.Result>*)(&val))->Length - iterator.m_StartIndex);
			iterator.m_StartIndex = ((NativeArray<GroupBuilder<Result>.Result>*)(&val))->Length;
			return true;
		}
		group = default(NativeArray<Result>);
		return false;
	}

	private int CreateGroup()
	{
		int length = m_Groups.Length;
		m_Groups.Add(ref length);
		return length;
	}

	private int MergeGroups(int index1, int index2)
	{
		int num = math.min(index1, index2);
		m_Groups[math.max(index1, index2)] = num;
		return num;
	}

	private unsafe void AddToGroup(T item, int index)
	{
		m_GroupIndex.TryAdd(item, index);
		ref NativeList<Result> results = ref m_Results;
		Result result = new Result(item, index);
		System.Runtime.CompilerServices.Unsafe.As<NativeList<Result>, NativeList<GroupBuilder<Result>.Result>>(ref results).Add(ref *(GroupBuilder<Result>.Result*)(&result));
	}
}
