using System.IO;
using System.Runtime.CompilerServices;
using Colossal.Serialization.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Game.Simulation;

public class WindSimulationSystem : GameSystemBase, IDefaultSerializable, ISerializable
{
	public struct WindCell : ISerializable
	{
		public float m_Pressure;

		public float3 m_Velocities;

		public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			float pressure = m_Pressure;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(pressure);
			float3 velocities = m_Velocities;
			((IWriter)writer/*cast due to .constrained prefix*/).Write(velocities);
		}

		public void Deserialize<TReader>(TReader reader) where TReader : IReader
		{
			ref float pressure = ref m_Pressure;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref pressure);
			ref float3 velocities = ref m_Velocities;
			((IReader)reader/*cast due to .constrained prefix*/).Read(ref velocities);
		}
	}

	[BurstCompile]
	private struct UpdateWindVelocityJob : IJobFor
	{
		public NativeArray<WindCell> m_Cells;

		[ReadOnly]
		public TerrainHeightData m_TerrainHeightData;

		[ReadOnly]
		public WaterSurfaceData m_WaterSurfaceData;

		public float2 m_TerrainRange;

		public void Execute(int index)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			int3 val = default(int3);
			((int3)(ref val))._002Ector(index % kResolution.x, index / kResolution.x % kResolution.y, index / (kResolution.x * kResolution.y));
			bool3 val2 = default(bool3);
			((bool3)(ref val2))._002Ector(val.x >= kResolution.x - 1, val.y >= kResolution.y - 1, val.z >= kResolution.z - 1);
			if (!val2.x && !val2.y && !val2.z)
			{
				int3 position = default(int3);
				((int3)(ref position))._002Ector(val.x, val.y + 1, val.z);
				int3 position2 = new int3(val.x + 1, val.y, val.z);
				float3 cellCenter = GetCellCenter(index);
				cellCenter.y = math.lerp(m_TerrainRange.x, m_TerrainRange.y, ((float)val.z + 0.5f) / (float)kResolution.z);
				float num = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, cellCenter);
				float num2 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, cellCenter);
				float num3 = WaterUtils.SampleHeight(ref m_WaterSurfaceData, ref m_TerrainHeightData, cellCenter);
				float num4 = 65535f / (m_TerrainHeightData.scale.y * (float)kResolution.z);
				float num5 = math.saturate((0.5f * (num4 + num + num2) - cellCenter.y) / num4);
				float num6 = math.saturate((0.5f * (num4 + num + num3) - cellCenter.y) / num4);
				WindCell windCell = m_Cells[index];
				WindCell cell = GetCell(new int3(val.x, val.y, val.z + 1), m_Cells);
				WindCell cell2 = GetCell(position, m_Cells);
				WindCell cell3 = GetCell(position2, m_Cells);
				windCell.m_Velocities.x *= math.lerp(kAirSlowdown, kTerrainSlowdown, num6);
				windCell.m_Velocities.y *= math.lerp(kAirSlowdown, kTerrainSlowdown, num5);
				windCell.m_Velocities.z *= kVerticalSlowdown;
				windCell.m_Velocities.x += kChangeFactor * (1f - num6) * (windCell.m_Pressure - cell3.m_Pressure);
				windCell.m_Velocities.y += kChangeFactor * (1f - num5) * (windCell.m_Pressure - cell2.m_Pressure);
				windCell.m_Velocities.z += kChangeFactor * (windCell.m_Pressure - cell.m_Pressure);
				m_Cells[index] = windCell;
			}
		}
	}

	[BurstCompile]
	private struct UpdatePressureJob : IJobFor
	{
		public NativeArray<WindCell> m_Cells;

		public float2 m_Wind;

		public void Execute(int index)
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0218: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0240: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0250: Unknown result type (might be due to invalid IL or missing references)
			//IL_0261: Unknown result type (might be due to invalid IL or missing references)
			//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			int3 val = default(int3);
			((int3)(ref val))._002Ector(index % kResolution.x, index / kResolution.x % kResolution.y, index / (kResolution.x * kResolution.y));
			bool3 val2 = default(bool3);
			((bool3)(ref val2))._002Ector(val.x == 0, val.y == 0, val.z == 0);
			bool3 val3 = default(bool3);
			((bool3)(ref val3))._002Ector(val.x >= kResolution.x - 1, val.y >= kResolution.y - 1, val.z >= kResolution.z - 1);
			if (!val3.x && !val3.y && !val3.z)
			{
				WindCell windCell = m_Cells[index];
				windCell.m_Pressure -= windCell.m_Velocities.x + windCell.m_Velocities.y + windCell.m_Velocities.z;
				if (!val2.x)
				{
					WindCell cell = GetCell(new int3(val.x - 1, val.y, val.z), m_Cells);
					windCell.m_Pressure += cell.m_Velocities.x;
				}
				if (!val2.y)
				{
					WindCell cell2 = GetCell(new int3(val.x, val.y - 1, val.z), m_Cells);
					windCell.m_Pressure += cell2.m_Velocities.y;
				}
				if (!val2.z)
				{
					WindCell cell3 = GetCell(new int3(val.x, val.y, val.z - 1), m_Cells);
					windCell.m_Pressure += cell3.m_Velocities.z;
				}
				m_Cells[index] = windCell;
			}
			if (val2.x || val2.y || val3.x || val3.y)
			{
				WindCell windCell2 = m_Cells[index];
				float num = math.dot(math.normalize(new float2((float)(val.x - kResolution.x / 2), (float)(val.y - kResolution.y / 2))), math.normalize(m_Wind));
				float num2 = math.pow((1f + (float)val.z) / (1f + (float)kResolution.z), 1f / 7f);
				float num3 = 0.1f * (2f - num);
				float num4 = (40f - 20f * (1f + num)) * math.length(m_Wind) * num2;
				windCell2.m_Pressure = ((num4 > windCell2.m_Pressure) ? math.min(num4, windCell2.m_Pressure + num3) : math.max(num4, windCell2.m_Pressure - num3));
				m_Cells[index] = windCell2;
			}
		}
	}

	public static readonly int kUpdateInterval = 512;

	public static readonly int3 kResolution = new int3(WindSystem.kTextureSize, WindSystem.kTextureSize, 16);

	public static readonly float kChangeFactor = 0.02f;

	public static readonly float kTerrainSlowdown = 0.99f;

	public static readonly float kAirSlowdown = 0.995f;

	public static readonly float kVerticalSlowdown = 0.9f;

	private SimulationSystem m_SimulationSystem;

	private TerrainSystem m_TerrainSystem;

	private WaterSystem m_WaterSystem;

	private ClimateSystem m_ClimateSystem;

	private bool m_Odd;

	private JobHandle m_Deps;

	private NativeArray<WindCell> m_Cells;

	public float2 constantWind { get; set; }

	private float m_ConstantPressure { get; set; }

	public override int GetUpdateInterval(SystemUpdatePhase phase)
	{
		if (phase != SystemUpdatePhase.GameSimulation)
		{
			return 1;
		}
		return kUpdateInterval;
	}

	public unsafe byte[] CreateByteArray<T>(NativeArray<T> src) where T : struct
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		int num = UnsafeUtility.SizeOf<T>() * src.Length;
		byte* unsafeReadOnlyPtr = (byte*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<T>(src);
		byte[] array = new byte[num];
		fixed (byte* ptr = array)
		{
			UnsafeUtility.MemCpy((void*)ptr, (void*)unsafeReadOnlyPtr, (long)num);
		}
		return array;
	}

	public void DebugSave()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Deps)).Complete();
		using BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(Application.streamingAssetsPath + "/wind_temp.dat"));
		binaryWriter.Write(kResolution.x);
		binaryWriter.Write(kResolution.y);
		binaryWriter.Write(kResolution.z);
		binaryWriter.Write(CreateByteArray<WindCell>(m_Cells));
	}

	public unsafe void DebugLoad()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Deps)).Complete();
		using BinaryReader binaryReader = new BinaryReader(File.OpenRead(Application.streamingAssetsPath + "/wind_temp.dat"));
		int num = binaryReader.ReadInt32();
		int num2 = binaryReader.ReadInt32();
		int num3 = binaryReader.ReadInt32();
		int num4 = num * num2 * num3 * UnsafeUtility.SizeOf<WindCell>();
		byte[] array = new byte[num4];
		binaryReader.Read(array, 0, num * num2 * num3 * System.Runtime.CompilerServices.Unsafe.SizeOf<WindCell>());
		byte* unsafePtr = (byte*)NativeArrayUnsafeUtility.GetUnsafePtr<WindCell>(m_Cells);
		for (int i = 0; i < num4; i++)
		{
			unsafePtr[i] = array[i];
		}
	}

	public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		int length = m_Cells.Length;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(length);
		NativeArray<WindCell> cells = m_Cells;
		((IWriter)writer/*cast due to .constrained prefix*/).Write<WindCell>(cells);
		float2 val = constantWind;
		((IWriter)writer/*cast due to .constrained prefix*/).Write(val);
	}

	public void Deserialize<TReader>(TReader reader) where TReader : IReader
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
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
			if (m_Cells.Length == num)
			{
				NativeArray<WindCell> cells = m_Cells;
				((IReader)reader/*cast due to .constrained prefix*/).Read<WindCell>(cells);
			}
			context = ((IReader)reader).context;
			if (((Context)(ref context)).version > Version.windDirection)
			{
				float2 val = default(float2);
				((IReader)reader/*cast due to .constrained prefix*/).Read(ref val);
				constantWind = val;
			}
			else
			{
				constantWind = new float2(0.275f, 0.275f);
			}
		}
		else
		{
			NativeArray<WindCell> cells2 = m_Cells;
			((IReader)reader/*cast due to .constrained prefix*/).Read<WindCell>(cells2);
		}
	}

	public void SetDefaults(Context context)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Deps)).Complete();
		for (int i = 0; i < m_Cells.Length; i++)
		{
			m_Cells[i] = new WindCell
			{
				m_Pressure = m_ConstantPressure,
				m_Velocities = new float3(constantWind, 0f)
			};
		}
	}

	public void SetWind(float2 direction, float pressure)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref m_Deps)).Complete();
		constantWind = direction;
		m_ConstantPressure = pressure;
		SetDefaults(default(Context));
	}

	public static float3 GetCenterVelocity(int3 cell, NativeArray<WindCell> cells)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		float3 velocities = GetCell(cell, cells).m_Velocities;
		float3 val = ((cell.x > 0) ? GetCell(cell + new int3(-1, 0, 0), cells).m_Velocities : velocities);
		float3 val2 = ((cell.y > 0) ? GetCell(cell + new int3(0, -1, 0), cells).m_Velocities : velocities);
		float3 val3 = ((cell.z > 0) ? GetCell(cell + new int3(0, 0, -1), cells).m_Velocities : velocities);
		return 0.5f * new float3(velocities.x + val.x, velocities.y + val2.y, velocities.z + val3.z);
	}

	public static float3 GetCellCenter(int index)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		int3 val = default(int3);
		((int3)(ref val))._002Ector(index % kResolution.x, index / kResolution.x % kResolution.y, index / (kResolution.x * kResolution.y));
		float3 result = (float)CellMapSystem<Wind>.kMapSize * new float3(((float)val.x + 0.5f) / (float)kResolution.x, 0f, ((float)val.y + 0.5f) / (float)kResolution.y) - (float)(CellMapSystem<Wind>.kMapSize / 2);
		result.y = 100f + 1024f * ((float)val.z + 0.5f) / (float)kResolution.z;
		return result;
	}

	public NativeArray<WindCell> GetCells(out JobHandle deps)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		deps = m_Deps;
		return m_Cells;
	}

	public void AddReader(JobHandle reader)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		m_Deps = JobHandle.CombineDependencies(m_Deps, reader);
	}

	[Preserve]
	protected override void OnCreate()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		m_SimulationSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<SimulationSystem>();
		m_TerrainSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<TerrainSystem>();
		m_WaterSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<WaterSystem>();
		m_ClimateSystem = ((ComponentSystemBase)this).World.GetOrCreateSystemManaged<ClimateSystem>();
		constantWind = new float2(0.275f, 0.275f);
		m_ConstantPressure = 40f;
		m_Cells = new NativeArray<WindCell>(kResolution.x * kResolution.y * kResolution.z, (Allocator)4, (NativeArrayOptions)1);
	}

	[Preserve]
	protected override void OnDestroy()
	{
		m_Cells.Dispose();
	}

	private WindCell GetCell(int3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return m_Cells[position.x + position.y * kResolution.x + position.z * kResolution.x * kResolution.y];
	}

	public static WindCell GetCell(int3 position, NativeArray<WindCell> cells)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		int num = position.x + position.y * kResolution.x + position.z * kResolution.x * kResolution.y;
		if (num < 0 || num >= cells.Length)
		{
			return default(WindCell);
		}
		return cells[num];
	}

	[Preserve]
	protected override void OnUpdate()
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_TerrainSystem.heightmap != (Object)null)
		{
			m_Odd = !m_Odd;
			if (!m_Odd)
			{
				TerrainHeightData data = m_TerrainSystem.GetHeightData();
				float num = TerrainUtils.ToWorldSpace(ref data, 0f);
				float num2 = TerrainUtils.ToWorldSpace(ref data, 65535f);
				float2 terrainRange = default(float2);
				((float2)(ref terrainRange))._002Ector(num, num2);
				JobHandle deps;
				UpdateWindVelocityJob updateWindVelocityJob = new UpdateWindVelocityJob
				{
					m_Cells = m_Cells,
					m_TerrainHeightData = data,
					m_WaterSurfaceData = m_WaterSystem.GetSurfaceData(out deps),
					m_TerrainRange = terrainRange
				};
				m_Deps = IJobForExtensions.Schedule<UpdateWindVelocityJob>(updateWindVelocityJob, kResolution.x * kResolution.y * kResolution.z, JobHandle.CombineDependencies(m_Deps, deps, ((SystemBase)this).Dependency));
				m_WaterSystem.AddSurfaceReader(m_Deps);
				m_TerrainSystem.AddCPUHeightReader(m_Deps);
			}
			else
			{
				UpdatePressureJob updatePressureJob = new UpdatePressureJob
				{
					m_Cells = m_Cells,
					m_Wind = constantWind / 10f
				};
				m_Deps = IJobForExtensions.Schedule<UpdatePressureJob>(updatePressureJob, kResolution.x * kResolution.y * kResolution.z, JobHandle.CombineDependencies(m_Deps, ((SystemBase)this).Dependency));
			}
			((SystemBase)this).Dependency = m_Deps;
		}
	}

	[Preserve]
	public WindSimulationSystem()
	{
	}
}
