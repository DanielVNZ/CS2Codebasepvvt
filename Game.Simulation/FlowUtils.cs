using System;
using Game.Buildings;
using Game.Citizens;
using Game.Companies;
using Game.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Simulation;

public static class FlowUtils
{
	public static int ConsumeFromTotal(int demand, ref int totalSupply, ref int totalDemand)
	{
		int num = 0;
		if (demand > 0)
		{
			int num2 = totalSupply - (totalDemand - demand);
			int num3 = totalSupply;
			int num4 = totalDemand * 100 / demand;
			num = math.clamp(totalSupply * 100 / num4, num2, num3);
			totalSupply -= num;
			totalDemand -= demand;
		}
		return num;
	}

	public static float GetRenterConsumptionMultiplier(Entity prefab, DynamicBuffer<Renter> renterBuffer, ref BufferLookup<HouseholdCitizen> householdCitizens, ref BufferLookup<Employee> employees, ref ComponentLookup<Citizen> citizens, ref ComponentLookup<SpawnableBuildingData> spawnableDatas)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		float num2 = 0f;
		Enumerator<Renter> enumerator = renterBuffer.GetEnumerator();
		try
		{
			DynamicBuffer<HouseholdCitizen> val = default(DynamicBuffer<HouseholdCitizen>);
			Citizen citizen = default(Citizen);
			DynamicBuffer<Employee> val2 = default(DynamicBuffer<Employee>);
			Citizen citizen2 = default(Citizen);
			while (enumerator.MoveNext())
			{
				Renter current = enumerator.Current;
				if (householdCitizens.TryGetBuffer((Entity)current, ref val))
				{
					Enumerator<HouseholdCitizen> enumerator2 = val.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							if (citizens.TryGetComponent(enumerator2.Current.m_Citizen, ref citizen))
							{
								num2 += (float)citizen.GetEducationLevel();
								num++;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator2/*cast due to .constrained prefix*/).Dispose();
					}
				}
				else
				{
					if (!employees.TryGetBuffer((Entity)current, ref val2))
					{
						continue;
					}
					Enumerator<Employee> enumerator3 = val2.GetEnumerator();
					try
					{
						while (enumerator3.MoveNext())
						{
							if (citizens.TryGetComponent(enumerator3.Current.m_Worker, ref citizen2))
							{
								num2 += (float)citizen2.GetEducationLevel();
								num++;
							}
						}
					}
					finally
					{
						((IDisposable)enumerator3/*cast due to .constrained prefix*/).Dispose();
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to .constrained prefix*/).Dispose();
		}
		if (num != 0)
		{
			SpawnableBuildingData spawnableBuildingData = default(SpawnableBuildingData);
			float num3 = (spawnableDatas.TryGetComponent(prefab, ref spawnableBuildingData) ? ((float)(int)spawnableBuildingData.m_Level) : 5f);
			return 5f * (float)num / (num3 + 0.5f * (num2 / (float)num));
		}
		return 0f;
	}
}
