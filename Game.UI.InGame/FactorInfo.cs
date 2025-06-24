using System;
using Colossal.UI.Binding;
using Game.Buildings;
using Game.Simulation;
using Unity.Collections;
using Unity.Mathematics;

namespace Game.UI.InGame;

public readonly struct FactorInfo : IComparable<FactorInfo>
{
	public int factor { get; }

	public int weight { get; }

	public FactorInfo(int factor, int weight)
	{
		this.factor = factor;
		this.weight = weight;
	}

	public int CompareTo(FactorInfo other)
	{
		int num = math.abs(other.weight).CompareTo(math.abs(weight));
		if (num == 0)
		{
			return other.factor.CompareTo(factor);
		}
		return num;
	}

	public void WriteDemandFactor(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("factor");
		writer.Write(Enum.GetName(typeof(DemandFactor), factor));
		writer.PropertyName("weight");
		writer.Write(weight);
		writer.TypeEnd();
	}

	public void WriteHappinessFactor(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("factor");
		writer.Write(Enum.GetName(typeof(CitizenHappinessSystem.HappinessFactor), factor));
		writer.PropertyName("weight");
		writer.Write(weight);
		writer.TypeEnd();
	}

	public void WriteBuildingHappinessFactor(IJsonWriter writer)
	{
		writer.TypeBegin(GetType().FullName);
		writer.PropertyName("factor");
		writer.Write(Enum.GetName(typeof(BuildingHappinessFactor), factor));
		writer.PropertyName("weight");
		writer.Write(weight);
		writer.TypeEnd();
	}

	public static NativeList<FactorInfo> FromFactorArray(NativeArray<int> factors, Allocator allocator)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		NativeList<FactorInfo> val = default(NativeList<FactorInfo>);
		val._002Ector(factors.Length, AllocatorHandle.op_Implicit(allocator));
		for (int i = 0; i < factors.Length; i++)
		{
			if (factors[i] != 0)
			{
				FactorInfo factorInfo = new FactorInfo(i, factors[i]);
				val.Add(ref factorInfo);
			}
		}
		NativeSortExtension.Sort<FactorInfo>(val);
		return val;
	}
}
