using System;
using Colossal.Mathematics;
using Game.Net;
using Game.Rendering;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Game.Prefabs;

public static class NetCompositionHelpers
{
	private struct TempLaneData
	{
		public int m_GroupIndex;
	}

	private struct TempLaneGroup
	{
		public Entity m_Prefab;

		public float3 m_Position;

		public LaneFlags m_Flags;

		public int m_LaneCount;

		public int m_CarriagewayIndex;

		public bool IsCompatible(TempLaneGroup other)
		{
			LaneFlags laneFlags = LaneFlags.Invert | LaneFlags.Road;
			return ((m_Flags & laneFlags) == (other.m_Flags & laneFlags)) & ((m_Flags & LaneFlags.Road) != 0);
		}
	}

	public static void GetRequirementFlags(NetPieceRequirements[] requirements, out CompositionFlags compositionFlags, out NetSectionFlags sectionFlags)
	{
		compositionFlags = default(CompositionFlags);
		sectionFlags = (NetSectionFlags)0;
		if (requirements != null)
		{
			for (int i = 0; i < requirements.Length; i++)
			{
				GetRequirementFlags(requirements[i], ref compositionFlags, ref sectionFlags);
			}
		}
	}

	public static void GetRequirementFlags(NetPieceRequirements requirement, ref CompositionFlags compositionFlags, ref NetSectionFlags sectionFlags)
	{
		switch (requirement)
		{
		case NetPieceRequirements.Node:
			compositionFlags.m_General |= CompositionFlags.General.Node;
			break;
		case NetPieceRequirements.Intersection:
			compositionFlags.m_General |= CompositionFlags.General.Intersection;
			break;
		case NetPieceRequirements.DeadEnd:
			compositionFlags.m_General |= CompositionFlags.General.DeadEnd;
			break;
		case NetPieceRequirements.Crosswalk:
			compositionFlags.m_General |= CompositionFlags.General.Crosswalk;
			break;
		case NetPieceRequirements.BusStop:
			compositionFlags.m_Right |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.Median:
			sectionFlags |= NetSectionFlags.Median;
			break;
		case NetPieceRequirements.TrainStop:
			compositionFlags.m_Right |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.OppositeTrainStop:
			compositionFlags.m_Left |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.Inverted:
			sectionFlags |= NetSectionFlags.Invert;
			break;
		case NetPieceRequirements.TaxiStand:
			compositionFlags.m_Right |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.LevelCrossing:
			compositionFlags.m_General |= CompositionFlags.General.LevelCrossing;
			break;
		case NetPieceRequirements.Elevated:
			compositionFlags.m_General |= CompositionFlags.General.Elevated;
			break;
		case NetPieceRequirements.Tunnel:
			compositionFlags.m_General |= CompositionFlags.General.Tunnel;
			break;
		case NetPieceRequirements.Raised:
			compositionFlags.m_Right |= CompositionFlags.Side.Raised;
			break;
		case NetPieceRequirements.Lowered:
			compositionFlags.m_Right |= CompositionFlags.Side.Lowered;
			break;
		case NetPieceRequirements.LowTransition:
			compositionFlags.m_Right |= CompositionFlags.Side.LowTransition;
			break;
		case NetPieceRequirements.HighTransition:
			compositionFlags.m_Right |= CompositionFlags.Side.HighTransition;
			break;
		case NetPieceRequirements.WideMedian:
			compositionFlags.m_General |= CompositionFlags.General.WideMedian;
			break;
		case NetPieceRequirements.TramTrack:
			compositionFlags.m_Right |= CompositionFlags.Side.PrimaryTrack;
			break;
		case NetPieceRequirements.TramStop:
			compositionFlags.m_Right |= CompositionFlags.Side.SecondaryStop;
			break;
		case NetPieceRequirements.OppositeTramTrack:
			compositionFlags.m_Left |= CompositionFlags.Side.PrimaryTrack;
			break;
		case NetPieceRequirements.OppositeTramStop:
			compositionFlags.m_Left |= CompositionFlags.Side.SecondaryStop;
			break;
		case NetPieceRequirements.MedianBreak:
			compositionFlags.m_General |= CompositionFlags.General.MedianBreak;
			break;
		case NetPieceRequirements.ShipStop:
			compositionFlags.m_Right |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.Sidewalk:
			compositionFlags.m_Right |= CompositionFlags.Side.Sidewalk;
			break;
		case NetPieceRequirements.Edge:
			compositionFlags.m_General |= CompositionFlags.General.Edge;
			break;
		case NetPieceRequirements.SubwayStop:
			compositionFlags.m_Right |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.OppositeSubwayStop:
			compositionFlags.m_Left |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.MiddlePlatform:
			compositionFlags.m_General |= CompositionFlags.General.MiddlePlatform;
			break;
		case NetPieceRequirements.Underground:
			sectionFlags |= NetSectionFlags.Underground;
			break;
		case NetPieceRequirements.Roundabout:
			compositionFlags.m_General |= CompositionFlags.General.Roundabout;
			break;
		case NetPieceRequirements.OppositeSidewalk:
			compositionFlags.m_Left |= CompositionFlags.Side.Sidewalk;
			break;
		case NetPieceRequirements.SoundBarrier:
			compositionFlags.m_Right |= CompositionFlags.Side.SoundBarrier;
			break;
		case NetPieceRequirements.Overhead:
			sectionFlags |= NetSectionFlags.Overhead;
			break;
		case NetPieceRequirements.TrafficLights:
			compositionFlags.m_General |= CompositionFlags.General.TrafficLights;
			break;
		case NetPieceRequirements.PublicTransportLane:
			compositionFlags.m_Right |= CompositionFlags.Side.PrimaryLane;
			break;
		case NetPieceRequirements.OppositePublicTransportLane:
			compositionFlags.m_Left |= CompositionFlags.Side.PrimaryLane;
			break;
		case NetPieceRequirements.Spillway:
			compositionFlags.m_General |= CompositionFlags.General.Spillway;
			break;
		case NetPieceRequirements.MiddleGrass:
			compositionFlags.m_General |= CompositionFlags.General.PrimaryMiddleBeautification;
			break;
		case NetPieceRequirements.MiddleTrees:
			compositionFlags.m_General |= CompositionFlags.General.SecondaryMiddleBeautification;
			break;
		case NetPieceRequirements.WideSidewalk:
			compositionFlags.m_Right |= CompositionFlags.Side.WideSidewalk;
			break;
		case NetPieceRequirements.SideGrass:
			compositionFlags.m_Right |= CompositionFlags.Side.PrimaryBeautification;
			break;
		case NetPieceRequirements.SideTrees:
			compositionFlags.m_Right |= CompositionFlags.Side.SecondaryBeautification;
			break;
		case NetPieceRequirements.OppositeGrass:
			compositionFlags.m_Left |= CompositionFlags.Side.PrimaryBeautification;
			break;
		case NetPieceRequirements.OppositeTrees:
			compositionFlags.m_Left |= CompositionFlags.Side.SecondaryBeautification;
			break;
		case NetPieceRequirements.Opening:
			compositionFlags.m_General |= CompositionFlags.General.Opening;
			break;
		case NetPieceRequirements.Front:
			compositionFlags.m_General |= CompositionFlags.General.Front;
			break;
		case NetPieceRequirements.Back:
			compositionFlags.m_General |= CompositionFlags.General.Back;
			break;
		case NetPieceRequirements.Flipped:
			sectionFlags |= NetSectionFlags.FlipMesh;
			break;
		case NetPieceRequirements.RemoveTrafficLights:
			compositionFlags.m_General |= CompositionFlags.General.RemoveTrafficLights;
			break;
		case NetPieceRequirements.AllWayStop:
			compositionFlags.m_General |= CompositionFlags.General.AllWayStop;
			break;
		case NetPieceRequirements.Pavement:
			compositionFlags.m_General |= CompositionFlags.General.Pavement;
			break;
		case NetPieceRequirements.Gravel:
			compositionFlags.m_General |= CompositionFlags.General.Gravel;
			break;
		case NetPieceRequirements.Tiles:
			compositionFlags.m_General |= CompositionFlags.General.Tiles;
			break;
		case NetPieceRequirements.ForbidLeftTurn:
			compositionFlags.m_Right |= CompositionFlags.Side.ForbidLeftTurn;
			break;
		case NetPieceRequirements.ForbidRightTurn:
			compositionFlags.m_Right |= CompositionFlags.Side.ForbidRightTurn;
			break;
		case NetPieceRequirements.OppositeWideSidewalk:
			compositionFlags.m_Left |= CompositionFlags.Side.WideSidewalk;
			break;
		case NetPieceRequirements.OppositeForbidLeftTurn:
			compositionFlags.m_Left |= CompositionFlags.Side.ForbidLeftTurn;
			break;
		case NetPieceRequirements.OppositeForbidRightTurn:
			compositionFlags.m_Left |= CompositionFlags.Side.ForbidRightTurn;
			break;
		case NetPieceRequirements.OppositeSoundBarrier:
			compositionFlags.m_Left |= CompositionFlags.Side.SoundBarrier;
			break;
		case NetPieceRequirements.SidePlatform:
			compositionFlags.m_Right |= CompositionFlags.Side.Sidewalk;
			break;
		case NetPieceRequirements.AddCrosswalk:
			compositionFlags.m_Right |= CompositionFlags.Side.AddCrosswalk;
			break;
		case NetPieceRequirements.RemoveCrosswalk:
			compositionFlags.m_Right |= CompositionFlags.Side.RemoveCrosswalk;
			break;
		case NetPieceRequirements.Lighting:
			compositionFlags.m_General |= CompositionFlags.General.Lighting;
			break;
		case NetPieceRequirements.OppositeBusStop:
			compositionFlags.m_Left |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.OppositeTaxiStand:
			compositionFlags.m_Left |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.OppositeRaised:
			compositionFlags.m_Left |= CompositionFlags.Side.Raised;
			break;
		case NetPieceRequirements.OppositeLowered:
			compositionFlags.m_Left |= CompositionFlags.Side.Lowered;
			break;
		case NetPieceRequirements.OppositeLowTransition:
			compositionFlags.m_Left |= CompositionFlags.Side.LowTransition;
			break;
		case NetPieceRequirements.OppositeHighTransition:
			compositionFlags.m_Left |= CompositionFlags.Side.HighTransition;
			break;
		case NetPieceRequirements.OppositeShipStop:
			compositionFlags.m_Left |= CompositionFlags.Side.PrimaryStop;
			break;
		case NetPieceRequirements.OppositePlatform:
			compositionFlags.m_Left |= CompositionFlags.Side.Sidewalk;
			break;
		case NetPieceRequirements.OppositeAddCrosswalk:
			compositionFlags.m_Left |= CompositionFlags.Side.AddCrosswalk;
			break;
		case NetPieceRequirements.OppositeRemoveCrosswalk:
			compositionFlags.m_Left |= CompositionFlags.Side.RemoveCrosswalk;
			break;
		case NetPieceRequirements.Inside:
			compositionFlags.m_General |= CompositionFlags.General.Inside;
			break;
		case NetPieceRequirements.ForbidStraight:
			compositionFlags.m_Right |= CompositionFlags.Side.ForbidStraight;
			break;
		case NetPieceRequirements.OppositeForbidStraight:
			compositionFlags.m_Left |= CompositionFlags.Side.ForbidStraight;
			break;
		case NetPieceRequirements.Hidden:
			sectionFlags |= NetSectionFlags.Hidden;
			break;
		case NetPieceRequirements.ParkingSpaces:
			compositionFlags.m_Right |= CompositionFlags.Side.ParkingSpaces;
			break;
		case NetPieceRequirements.OppositeParkingSpaces:
			compositionFlags.m_Left |= CompositionFlags.Side.ParkingSpaces;
			break;
		case NetPieceRequirements.FixedNodeSize:
			compositionFlags.m_General |= CompositionFlags.General.FixedNodeSize;
			break;
		case NetPieceRequirements.HalfLength:
			sectionFlags |= NetSectionFlags.HalfLength;
			break;
		case NetPieceRequirements.AbruptEnd:
			compositionFlags.m_Right |= CompositionFlags.Side.AbruptEnd;
			break;
		case NetPieceRequirements.OppositeAbruptEnd:
			compositionFlags.m_Left |= CompositionFlags.Side.AbruptEnd;
			break;
		case NetPieceRequirements.AttachmentTrack:
			compositionFlags.m_Right |= CompositionFlags.Side.SecondaryTrack;
			break;
		case NetPieceRequirements.EnterGate:
			compositionFlags.m_Right |= CompositionFlags.Side.Gate;
			break;
		case NetPieceRequirements.ExitGate:
			compositionFlags.m_Left |= CompositionFlags.Side.Gate;
			break;
		case NetPieceRequirements.StyleBreak:
			compositionFlags.m_General |= CompositionFlags.General.StyleBreak;
			break;
		}
	}

	public static CompositionFlags InvertCompositionFlags(CompositionFlags flags)
	{
		return new CompositionFlags(flags.m_General, flags.m_Right, flags.m_Left);
	}

	public static NetSectionFlags InvertSectionFlags(NetSectionFlags flags)
	{
		return flags;
	}

	public static bool TestSectionFlags(NetGeometrySection section, CompositionFlags compositionFlags)
	{
		if (((section.m_CompositionAll | section.m_CompositionNone) & compositionFlags) != section.m_CompositionAll)
		{
			return false;
		}
		if (section.m_CompositionAny == default(CompositionFlags))
		{
			return true;
		}
		return (section.m_CompositionAny & compositionFlags) != default(CompositionFlags);
	}

	public static bool TestSubSectionFlags(NetSubSection subSection, CompositionFlags compositionFlags, NetSectionFlags sectionFlags)
	{
		if ((sectionFlags & NetSectionFlags.Median) == 0)
		{
			compositionFlags.m_General &= ~CompositionFlags.General.MedianBreak;
		}
		if (((subSection.m_CompositionAll | subSection.m_CompositionNone) & compositionFlags) != subSection.m_CompositionAll)
		{
			return false;
		}
		if (((subSection.m_SectionAll | subSection.m_SectionNone) & sectionFlags) != subSection.m_SectionAll)
		{
			return false;
		}
		if (subSection.m_CompositionAny == default(CompositionFlags) && subSection.m_SectionAny == (NetSectionFlags)0)
		{
			return true;
		}
		if (!((subSection.m_CompositionAny & compositionFlags) != default(CompositionFlags)))
		{
			return (subSection.m_SectionAny & sectionFlags) != 0;
		}
		return true;
	}

	public static bool TestPieceFlags(NetSectionPiece piece, CompositionFlags compositionFlags, NetSectionFlags sectionFlags)
	{
		if ((sectionFlags & NetSectionFlags.Median) == 0)
		{
			compositionFlags.m_General &= ~CompositionFlags.General.MedianBreak;
		}
		if (((piece.m_CompositionAll | piece.m_CompositionNone) & compositionFlags) != piece.m_CompositionAll)
		{
			return false;
		}
		if (((piece.m_SectionAll | piece.m_SectionNone) & sectionFlags) != piece.m_SectionAll)
		{
			return false;
		}
		if (piece.m_CompositionAny == default(CompositionFlags) && piece.m_SectionAny == (NetSectionFlags)0)
		{
			return true;
		}
		if (!((piece.m_CompositionAny & compositionFlags) != default(CompositionFlags)))
		{
			return (piece.m_SectionAny & sectionFlags) != 0;
		}
		return true;
	}

	public static bool TestPieceFlags2(NetSectionPiece piece, CompositionFlags compositionFlags, NetSectionFlags sectionFlags)
	{
		if ((compositionFlags.m_General & CompositionFlags.General.Roundabout) != 0 && (piece.m_Flags & NetPieceFlags.Side) != 0)
		{
			CompositionFlags compositionFlags2 = compositionFlags;
			if ((compositionFlags2.m_General & CompositionFlags.General.Elevated) != 0)
			{
				if ((compositionFlags2.m_Left & CompositionFlags.Side.HighTransition) != 0)
				{
					compositionFlags2.m_General &= ~CompositionFlags.General.Elevated;
					compositionFlags2.m_Left &= ~CompositionFlags.Side.HighTransition;
				}
				else if ((compositionFlags2.m_Left & CompositionFlags.Side.LowTransition) != 0)
				{
					compositionFlags2.m_General &= ~CompositionFlags.General.Elevated;
					compositionFlags2.m_Left &= ~CompositionFlags.Side.LowTransition;
					compositionFlags2.m_Left |= CompositionFlags.Side.Raised;
				}
				if ((compositionFlags2.m_Right & CompositionFlags.Side.HighTransition) != 0)
				{
					compositionFlags2.m_General &= ~CompositionFlags.General.Elevated;
					compositionFlags2.m_Right &= ~CompositionFlags.Side.HighTransition;
				}
				else if ((compositionFlags2.m_Right & CompositionFlags.Side.LowTransition) != 0)
				{
					compositionFlags2.m_General &= ~CompositionFlags.General.Elevated;
					compositionFlags2.m_Right &= ~CompositionFlags.Side.LowTransition;
					compositionFlags2.m_Right |= CompositionFlags.Side.Raised;
				}
			}
			else if ((compositionFlags2.m_General & CompositionFlags.General.Tunnel) != 0)
			{
				if ((compositionFlags2.m_Left & CompositionFlags.Side.HighTransition) != 0)
				{
					compositionFlags2.m_General &= ~CompositionFlags.General.Tunnel;
					compositionFlags2.m_Left &= ~CompositionFlags.Side.HighTransition;
				}
				else if ((compositionFlags2.m_Left & CompositionFlags.Side.LowTransition) != 0)
				{
					compositionFlags2.m_General &= ~CompositionFlags.General.Tunnel;
					compositionFlags2.m_Left &= ~CompositionFlags.Side.LowTransition;
					compositionFlags2.m_Left |= CompositionFlags.Side.Lowered;
				}
				if ((compositionFlags2.m_Right & CompositionFlags.Side.HighTransition) != 0)
				{
					compositionFlags2.m_General &= ~CompositionFlags.General.Tunnel;
					compositionFlags2.m_Right &= ~CompositionFlags.Side.HighTransition;
				}
				else if ((compositionFlags2.m_Right & CompositionFlags.Side.LowTransition) != 0)
				{
					compositionFlags2.m_General &= ~CompositionFlags.General.Tunnel;
					compositionFlags2.m_Right &= ~CompositionFlags.Side.LowTransition;
					compositionFlags2.m_Right |= CompositionFlags.Side.Lowered;
				}
			}
			else
			{
				if ((compositionFlags2.m_Left & CompositionFlags.Side.LowTransition) != 0)
				{
					if ((compositionFlags2.m_Left & CompositionFlags.Side.Raised) != 0)
					{
						compositionFlags2.m_Left &= ~(CompositionFlags.Side.Raised | CompositionFlags.Side.LowTransition);
					}
					else if ((compositionFlags2.m_Left & CompositionFlags.Side.Lowered) != 0)
					{
						compositionFlags2.m_Left &= ~(CompositionFlags.Side.Lowered | CompositionFlags.Side.LowTransition);
					}
					else if ((compositionFlags2.m_Left & CompositionFlags.Side.SoundBarrier) != 0)
					{
						compositionFlags2.m_Left &= ~(CompositionFlags.Side.LowTransition | CompositionFlags.Side.SoundBarrier);
					}
				}
				if ((compositionFlags2.m_Right & CompositionFlags.Side.LowTransition) != 0)
				{
					if ((compositionFlags2.m_Right & CompositionFlags.Side.Raised) != 0)
					{
						compositionFlags2.m_Right &= ~(CompositionFlags.Side.Raised | CompositionFlags.Side.LowTransition);
					}
					else if ((compositionFlags2.m_Right & CompositionFlags.Side.Lowered) != 0)
					{
						compositionFlags2.m_Right &= ~(CompositionFlags.Side.Lowered | CompositionFlags.Side.LowTransition);
					}
					else if ((compositionFlags2.m_Right & CompositionFlags.Side.SoundBarrier) != 0)
					{
						compositionFlags2.m_Right &= ~(CompositionFlags.Side.LowTransition | CompositionFlags.Side.SoundBarrier);
					}
				}
			}
			if (compositionFlags != compositionFlags2)
			{
				return TestPieceFlags(piece, compositionFlags2, sectionFlags);
			}
		}
		return false;
	}

	public static bool TestObjectFlags(NetPieceObject _object, CompositionFlags compositionFlags, NetSectionFlags sectionFlags)
	{
		if ((sectionFlags & NetSectionFlags.Median) == 0)
		{
			compositionFlags.m_General &= ~CompositionFlags.General.MedianBreak;
		}
		if (((_object.m_CompositionAll | _object.m_CompositionNone) & compositionFlags) != _object.m_CompositionAll)
		{
			return false;
		}
		if (((_object.m_SectionAll | _object.m_SectionNone) & sectionFlags) != _object.m_SectionAll)
		{
			return false;
		}
		if (_object.m_CompositionAny == default(CompositionFlags) && _object.m_SectionAny == (NetSectionFlags)0)
		{
			return true;
		}
		if (!((_object.m_CompositionAny & compositionFlags) != default(CompositionFlags)))
		{
			return (_object.m_SectionAny & sectionFlags) != 0;
		}
		return true;
	}

	public static bool TestLaneFlags(AuxiliaryNetLane lane, CompositionFlags compositionFlags)
	{
		if (((lane.m_CompositionAll | lane.m_CompositionNone) & compositionFlags) != lane.m_CompositionAll)
		{
			return false;
		}
		if (lane.m_CompositionAny == default(CompositionFlags))
		{
			return true;
		}
		return (lane.m_CompositionAny & compositionFlags) != default(CompositionFlags);
	}

	public static bool TestEdgeFlags(NetGeometryEdgeState edgeState, CompositionFlags compositionFlags)
	{
		if (((edgeState.m_CompositionAll | edgeState.m_CompositionNone) & compositionFlags) != edgeState.m_CompositionAll)
		{
			return false;
		}
		if (edgeState.m_CompositionAny == default(CompositionFlags))
		{
			return true;
		}
		return (edgeState.m_CompositionAny & compositionFlags) != default(CompositionFlags);
	}

	public static bool TestEdgeFlags(NetGeometryNodeState nodeState, CompositionFlags compositionFlags)
	{
		if (((nodeState.m_CompositionAll | nodeState.m_CompositionNone) & compositionFlags) != nodeState.m_CompositionAll)
		{
			return false;
		}
		if (nodeState.m_CompositionAny == default(CompositionFlags))
		{
			return true;
		}
		return (nodeState.m_CompositionAny & compositionFlags) != default(CompositionFlags);
	}

	public static bool TestEdgeFlags(ElectricityConnectionData electricityConnectionData, CompositionFlags compositionFlags)
	{
		if (((electricityConnectionData.m_CompositionAll | electricityConnectionData.m_CompositionNone) & compositionFlags) != electricityConnectionData.m_CompositionAll)
		{
			return false;
		}
		if (electricityConnectionData.m_CompositionAny == default(CompositionFlags))
		{
			return true;
		}
		return (electricityConnectionData.m_CompositionAny & compositionFlags) != default(CompositionFlags);
	}

	public static bool TestEdgeMatch(NetGeometryNodeState nodeState, bool2 match)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		return nodeState.m_MatchType switch
		{
			NetEdgeMatchType.Both => math.all(match), 
			NetEdgeMatchType.Any => math.any(match), 
			NetEdgeMatchType.Exclusive => match.x != match.y, 
			_ => false, 
		};
	}

	public static void GetCompositionPieces(NativeList<NetCompositionPiece> resultBuffer, NativeArray<NetGeometrySection> geometrySections, CompositionFlags flags, BufferLookup<NetSubSection> subSectionData, BufferLookup<NetSectionPiece> sectionPieceData)
	{
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		CompositionFlags compositionFlags = InvertCompositionFlags(flags);
		compositionFlags.m_Left = (CompositionFlags.Side)(((uint)compositionFlags.m_Left & 0xFFFFBFFFu) | (uint)(flags.m_Left & CompositionFlags.Side.AbruptEnd));
		compositionFlags.m_Right = (CompositionFlags.Side)(((uint)compositionFlags.m_Right & 0xFFFFBFFFu) | (uint)(flags.m_Right & CompositionFlags.Side.AbruptEnd));
		for (int i = 0; i < geometrySections.Length; i++)
		{
			NetGeometrySection section;
			if ((flags.m_General & CompositionFlags.General.Invert) != 0)
			{
				section = geometrySections[geometrySections.Length - 1 - i];
				section.m_Flags ^= NetSectionFlags.Invert | NetSectionFlags.FlipLanes;
				if ((section.m_Flags & NetSectionFlags.Left) != 0)
				{
					section.m_Flags &= ~NetSectionFlags.Left;
					section.m_Flags |= NetSectionFlags.Right;
				}
				else if ((section.m_Flags & NetSectionFlags.Right) != 0)
				{
					section.m_Flags &= ~NetSectionFlags.Right;
					section.m_Flags |= NetSectionFlags.Left;
				}
			}
			else
			{
				section = geometrySections[i];
			}
			if ((flags.m_General & CompositionFlags.General.Flip) != 0)
			{
				section.m_Flags ^= NetSectionFlags.FlipLanes;
			}
			CompositionFlags compositionFlags2 = (((section.m_Flags & NetSectionFlags.Invert) != 0) ? compositionFlags : flags);
			NetPieceFlags netPieceFlags = (NetPieceFlags)0;
			if ((section.m_Flags & NetSectionFlags.HiddenSurface) != 0)
			{
				netPieceFlags |= NetPieceFlags.Surface;
			}
			if ((section.m_Flags & NetSectionFlags.HiddenBottom) != 0)
			{
				netPieceFlags |= NetPieceFlags.Bottom;
			}
			if ((section.m_Flags & NetSectionFlags.HiddenTop) != 0)
			{
				netPieceFlags |= NetPieceFlags.Top;
			}
			if ((section.m_Flags & NetSectionFlags.HiddenSide) != 0)
			{
				netPieceFlags |= NetPieceFlags.Side;
			}
			if (!TestSectionFlags(section, compositionFlags2))
			{
				continue;
			}
			NetSectionFlags sectionFlags = (((section.m_Flags & NetSectionFlags.Invert) != 0) ? InvertSectionFlags(section.m_Flags) : section.m_Flags);
			while (true)
			{
				DynamicBuffer<NetSubSection> val = subSectionData[section.m_Section];
				NetSubSection subSection;
				for (int j = 0; j < val.Length; j++)
				{
					subSection = val[j];
					if (TestSubSectionFlags(subSection, compositionFlags2, sectionFlags))
					{
						goto IL_01bb;
					}
				}
				break;
				IL_01bb:
				section.m_Section = subSection.m_SubSection;
			}
			DynamicBuffer<NetSectionPiece> val2 = sectionPieceData[section.m_Section];
			for (int k = 0; k < val2.Length; k++)
			{
				NetSectionPiece piece = val2[k];
				NetPieceFlags netPieceFlags2 = piece.m_Flags;
				if (!TestPieceFlags(piece, compositionFlags2, sectionFlags))
				{
					if (!TestPieceFlags2(piece, compositionFlags2, sectionFlags))
					{
						continue;
					}
					netPieceFlags2 |= NetPieceFlags.SkipBottomHalf;
				}
				NetCompositionPiece netCompositionPiece = new NetCompositionPiece
				{
					m_Piece = piece.m_Piece,
					m_SectionFlags = section.m_Flags,
					m_PieceFlags = netPieceFlags2,
					m_SectionIndex = num,
					m_Offset = section.m_Offset + piece.m_Offset
				};
				if ((netPieceFlags & netPieceFlags2) != 0)
				{
					netCompositionPiece.m_SectionFlags |= NetSectionFlags.Hidden;
				}
				resultBuffer.Add(ref netCompositionPiece);
				num2++;
			}
			if (num2 != 0)
			{
				num++;
				num2 = 0;
			}
		}
	}

	public static void CalculateCompositionData(ref NetCompositionData compositionData, NativeArray<NetCompositionPiece> pieces, ComponentLookup<NetPieceData> netPieceData, ComponentLookup<NetLaneData> netLaneData, ComponentLookup<NetVertexMatchData> netVertexMatchData, BufferLookup<NetPieceLane> netPieceLanes)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		CalculateCompositionPieceOffsets(ref compositionData, pieces, netPieceData);
		CalculateSyncVertexOffsets(ref compositionData, pieces, netVertexMatchData);
		CalculateRoundaboutSize(ref compositionData, pieces, netLaneData, netPieceLanes);
	}

	public static void CalculateMinLod(ref NetCompositionData compositionData, NativeArray<NetCompositionPiece> pieces, ComponentLookup<MeshData> meshDatas)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < pieces.Length; i++)
		{
			num += meshDatas[pieces[i].m_Piece].m_LodBias;
		}
		if (pieces.Length != 0)
		{
			num /= (float)pieces.Length;
		}
		float2 size = default(float2);
		((float2)(ref size))._002Ector(compositionData.m_Width, MathUtils.Size(compositionData.m_HeightRange));
		compositionData.m_MinLod = RenderingUtils.CalculateLodLimit(RenderingUtils.GetRenderingSize(size), num);
	}

	private static void CalculateCompositionPieceOffsets(ref NetCompositionData compositionData, NativeArray<NetCompositionPiece> pieces, ComponentLookup<NetPieceData> netPieceData)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		compositionData.m_Width = 0f;
		compositionData.m_MiddleOffset = 0f;
		compositionData.m_WidthOffset = 0f;
		compositionData.m_NodeOffset = 0f;
		compositionData.m_HeightRange = new Bounds1(float.MaxValue, float.MinValue);
		compositionData.m_SurfaceHeight = new Bounds1(float.MaxValue, float.MinValue);
		bool flag = (compositionData.m_Flags.m_General & CompositionFlags.General.Invert) != 0;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		int num8 = 0;
		bool2 val = default(bool2);
		while (num8 < pieces.Length)
		{
			NetCompositionPiece netCompositionPiece = pieces[num8];
			bool flag6 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Underground) != 0;
			bool flag7 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Overhead) != 0;
			bool flag8 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0;
			bool flag9 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.FlipMesh) != 0;
			((bool2)(ref val))._002Ector((netCompositionPiece.m_SectionFlags & NetSectionFlags.Left) != 0, (netCompositionPiece.m_SectionFlags & NetSectionFlags.Right) != 0);
			NetPieceData netPieceData2 = netPieceData[netCompositionPiece.m_Piece];
			float num9 = netPieceData2.m_Width;
			if (!flag6 || (compositionData.m_Flags.m_General & CompositionFlags.General.Elevated) == 0)
			{
				ref Bounds1 heightRange = ref compositionData.m_HeightRange;
				heightRange |= netCompositionPiece.m_Offset.y + netPieceData2.m_HeightRange;
			}
			if (!flag6 && !flag7 && (netCompositionPiece.m_PieceFlags & NetPieceFlags.PreserveShape) == 0)
			{
				if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.Side) != 0)
				{
					float4 val2 = math.select(netPieceData2.m_SurfaceHeights, ((float4)(ref netPieceData2.m_SurfaceHeights)).yxwz, flag8);
					val2 = math.select(val2, ((float4)(ref val2)).zwxy, flag9);
					compositionData.m_EdgeHeights = math.select(compositionData.m_EdgeHeights, val2, ((bool2)(ref val)).xyxy);
					compositionData.m_SideConnectionOffset = math.select(compositionData.m_SideConnectionOffset, float2.op_Implicit(netPieceData2.m_SideConnectionOffset), val);
				}
				if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.Surface) != 0)
				{
					compositionData.m_SurfaceHeight.min = math.min(compositionData.m_SurfaceHeight.min, netCompositionPiece.m_Offset.y + math.cmin(netPieceData2.m_SurfaceHeights));
					compositionData.m_SurfaceHeight.max = math.max(compositionData.m_SurfaceHeight.max, netCompositionPiece.m_Offset.y + math.cmax(netPieceData2.m_SurfaceHeights));
					flag2 = true;
				}
			}
			compositionData.m_WidthOffset = math.max(compositionData.m_WidthOffset, netPieceData2.m_WidthOffset);
			compositionData.m_NodeOffset = math.max(compositionData.m_NodeOffset, netPieceData2.m_NodeOffset);
			netCompositionPiece.m_Size.x = netPieceData2.m_Width;
			netCompositionPiece.m_Size.y = netPieceData2.m_HeightRange.max - netPieceData2.m_HeightRange.min;
			netCompositionPiece.m_Size.z = netPieceData2.m_Length;
			int i;
			for (i = num8 + 1; i < pieces.Length; i++)
			{
				NetCompositionPiece netCompositionPiece2 = pieces[i];
				if (netCompositionPiece2.m_SectionIndex != netCompositionPiece.m_SectionIndex)
				{
					break;
				}
				NetPieceData netPieceData3 = netPieceData[netCompositionPiece2.m_Piece];
				num9 = math.max(num9, netPieceData3.m_Width);
				if (!flag6 || (compositionData.m_Flags.m_General & CompositionFlags.General.Elevated) == 0)
				{
					ref Bounds1 heightRange2 = ref compositionData.m_HeightRange;
					heightRange2 |= netCompositionPiece2.m_Offset.y + netPieceData3.m_HeightRange;
				}
				if (!flag6 && !flag7 && (netCompositionPiece2.m_PieceFlags & NetPieceFlags.PreserveShape) == 0)
				{
					if ((netCompositionPiece2.m_PieceFlags & NetPieceFlags.Side) != 0)
					{
						float4 val3 = math.select(netPieceData3.m_SurfaceHeights, ((float4)(ref netPieceData3.m_SurfaceHeights)).yxwz, flag8);
						val3 = math.select(val3, ((float4)(ref val3)).zwxy, flag9);
						compositionData.m_EdgeHeights = math.select(compositionData.m_EdgeHeights, val3, ((bool2)(ref val)).xyxy);
					}
					if ((netCompositionPiece2.m_PieceFlags & NetPieceFlags.Surface) != 0)
					{
						compositionData.m_SurfaceHeight.min = math.min(compositionData.m_SurfaceHeight.min, netCompositionPiece2.m_Offset.y + math.cmin(netPieceData3.m_SurfaceHeights));
						compositionData.m_SurfaceHeight.max = math.max(compositionData.m_SurfaceHeight.max, netCompositionPiece2.m_Offset.y + math.cmax(netPieceData3.m_SurfaceHeights));
						flag2 = true;
					}
				}
				compositionData.m_WidthOffset = math.max(compositionData.m_WidthOffset, netPieceData3.m_WidthOffset);
				compositionData.m_NodeOffset = math.max(compositionData.m_NodeOffset, netPieceData3.m_NodeOffset);
				netCompositionPiece2.m_Size.x = netPieceData3.m_Width;
				netCompositionPiece2.m_Size.y = netPieceData3.m_HeightRange.max - netPieceData3.m_HeightRange.min;
				netCompositionPiece2.m_Size.z = netPieceData3.m_Length;
				pieces[i] = netCompositionPiece2;
			}
			float x = netCompositionPiece.m_Offset.x;
			if (flag6)
			{
				netCompositionPiece.m_Offset.x += num + num9 * 0.5f;
				num += num9;
				if ((netCompositionPiece.m_SectionFlags & (NetSectionFlags.Median | NetSectionFlags.AlignCenter)) == NetSectionFlags.Median)
				{
					num2 = netCompositionPiece.m_Offset.x - math.select(x * 2f, 0f, flag);
					num3 = x;
					flag4 = true;
				}
				else if ((netCompositionPiece.m_SectionFlags & (NetSectionFlags.Right | NetSectionFlags.AlignCenter)) == NetSectionFlags.Right && !flag4)
				{
					num2 = netCompositionPiece.m_Offset.x - netCompositionPiece.m_Size.x * 0.5f - math.select(x * 2f, 0f, flag);
					num3 = x;
					flag4 = true;
				}
			}
			else if (flag7)
			{
				netCompositionPiece.m_Offset.x += num4 + num9 * 0.5f;
				num4 += num9;
				if ((netCompositionPiece.m_SectionFlags & (NetSectionFlags.Median | NetSectionFlags.AlignCenter)) == NetSectionFlags.Median)
				{
					num5 = netCompositionPiece.m_Offset.x - math.select(x * 2f, 0f, flag);
					num6 = x;
					flag5 = true;
				}
				else if ((netCompositionPiece.m_SectionFlags & (NetSectionFlags.Right | NetSectionFlags.AlignCenter)) == NetSectionFlags.Right && !flag5)
				{
					num5 = netCompositionPiece.m_Offset.x - netCompositionPiece.m_Size.x * 0.5f - math.select(x * 2f, 0f, flag);
					num6 = x;
					flag5 = true;
				}
			}
			else
			{
				netCompositionPiece.m_Offset.x += compositionData.m_Width + num9 * 0.5f;
				compositionData.m_Width += num9;
				if ((netCompositionPiece.m_SectionFlags & (NetSectionFlags.Median | NetSectionFlags.AlignCenter)) == NetSectionFlags.Median)
				{
					compositionData.m_MiddleOffset = netCompositionPiece.m_Offset.x - math.select(x * 2f, 0f, flag);
					num7 = x;
					flag3 = true;
				}
				else if ((netCompositionPiece.m_SectionFlags & (NetSectionFlags.Right | NetSectionFlags.AlignCenter)) == NetSectionFlags.Right && !flag3)
				{
					compositionData.m_MiddleOffset = netCompositionPiece.m_Offset.x - netCompositionPiece.m_Size.x * 0.5f - math.select(x * 2f, 0f, flag);
					num7 = x;
					flag3 = true;
				}
			}
			pieces[num8] = netCompositionPiece;
			for (int j = num8 + 1; j < i; j++)
			{
				NetCompositionPiece netCompositionPiece3 = pieces[j];
				netCompositionPiece3.m_Offset.x = netCompositionPiece.m_Offset.x;
				pieces[j] = netCompositionPiece3;
			}
			if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.Side) != 0 && i > num8 + 1 && (netCompositionPiece.m_SectionFlags & (NetSectionFlags.Left | NetSectionFlags.Right)) != 0)
			{
				bool flag10 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Right) != 0;
				for (int k = num8; k < i; k++)
				{
					NetCompositionPiece netCompositionPiece4 = pieces[k];
					float num10 = (netPieceData[netCompositionPiece4.m_Piece].m_Width - num9) * 0.5f;
					netCompositionPiece4.m_Offset.x += math.select(0f - num10, num10, flag10);
					pieces[k] = netCompositionPiece4;
				}
			}
			num8 = i;
		}
		if (flag4)
		{
			num2 -= num * 0.5f;
		}
		if (flag5)
		{
			num5 -= num4 * 0.5f;
		}
		if (flag3)
		{
			compositionData.m_MiddleOffset -= compositionData.m_Width * 0.5f;
		}
		if ((compositionData.m_Flags.m_General & (CompositionFlags.General.DeadEnd | CompositionFlags.General.LevelCrossing)) == CompositionFlags.General.LevelCrossing || (compositionData.m_Flags.m_General & (CompositionFlags.General.DeadEnd | CompositionFlags.General.Intersection | CompositionFlags.General.Crosswalk)) == CompositionFlags.General.Crosswalk || (compositionData.m_Flags.m_Right & CompositionFlags.Side.AbruptEnd) != 0)
		{
			compositionData.m_State |= CompositionState.BlockUTurn;
		}
		for (int l = 0; l < pieces.Length; l++)
		{
			NetCompositionPiece netCompositionPiece5 = pieces[l];
			bool num11 = (netCompositionPiece5.m_SectionFlags & NetSectionFlags.Underground) != 0;
			bool flag11 = (netCompositionPiece5.m_SectionFlags & NetSectionFlags.Overhead) != 0;
			if ((netCompositionPiece5.m_PieceFlags & (NetPieceFlags.PreserveShape | NetPieceFlags.BlockTraffic)) == NetPieceFlags.BlockTraffic)
			{
				compositionData.m_State |= CompositionState.BlockUTurn;
			}
			if ((netCompositionPiece5.m_PieceFlags & NetPieceFlags.LowerBottomToTerrain) != 0)
			{
				compositionData.m_State |= CompositionState.LowerToTerrain;
			}
			if ((netCompositionPiece5.m_PieceFlags & NetPieceFlags.RaiseTopToTerrain) != 0)
			{
				compositionData.m_State |= CompositionState.RaiseToTerrain;
			}
			if ((netCompositionPiece5.m_SectionFlags & NetSectionFlags.HalfLength) != 0)
			{
				compositionData.m_State |= CompositionState.HalfLength;
			}
			if ((netCompositionPiece5.m_SectionFlags & NetSectionFlags.Hidden) != 0)
			{
				compositionData.m_State |= CompositionState.Hidden;
			}
			if (num11)
			{
				netCompositionPiece5.m_Offset.x -= num * 0.5f + num3;
				netCompositionPiece5.m_Offset.x += compositionData.m_MiddleOffset - num2;
			}
			else if (flag11)
			{
				netCompositionPiece5.m_Offset.x -= num4 * 0.5f + num6;
				netCompositionPiece5.m_Offset.x += compositionData.m_MiddleOffset - num5;
			}
			else
			{
				netCompositionPiece5.m_Offset.x -= compositionData.m_Width * 0.5f + num7;
			}
			pieces[l] = netCompositionPiece5;
		}
		compositionData.m_Width = math.max(compositionData.m_Width, math.max(num, num4));
		if (compositionData.m_HeightRange.min > compositionData.m_HeightRange.max)
		{
			compositionData.m_HeightRange = default(Bounds1);
		}
		if (flag2)
		{
			compositionData.m_State |= CompositionState.HasSurface;
			if ((compositionData.m_Flags.m_General & (CompositionFlags.General.Elevated | CompositionFlags.General.Tunnel)) == 0)
			{
				compositionData.m_State |= CompositionState.ExclusiveGround;
			}
		}
		else
		{
			float num12 = MathUtils.Center(compositionData.m_HeightRange);
			compositionData.m_SurfaceHeight = new Bounds1(num12, num12);
		}
	}

	private static void CalculateSyncVertexOffsets(ref NetCompositionData compositionData, NativeArray<NetCompositionPiece> pieces, ComponentLookup<NetVertexMatchData> netVertexMatchData)
	{
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		float4 val = default(float4);
		((float4)(ref val))._002Ector(0f, 0f, 0f, 1f);
		float4 val2 = default(float4);
		((float4)(ref val2))._002Ector(0f, 1f, 1f, 1f);
		float middleOffset = compositionData.m_MiddleOffset;
		float num = compositionData.m_Width * 0.5f + middleOffset;
		float num2 = compositionData.m_Width * 0.5f - middleOffset;
		bool flag = false;
		float2 val3 = default(float2);
		for (int i = 0; i < pieces.Length; i++)
		{
			NetCompositionPiece netCompositionPiece = pieces[i];
			if ((netCompositionPiece.m_SectionFlags & (NetSectionFlags.Underground | NetSectionFlags.Overhead)) != 0)
			{
				continue;
			}
			if ((netCompositionPiece.m_SectionFlags & NetSectionFlags.Median) != 0)
			{
				if (netVertexMatchData.HasComponent(netCompositionPiece.m_Piece))
				{
					NetVertexMatchData netVertexMatchData2 = netVertexMatchData[netCompositionPiece.m_Piece];
					if (!math.any(math.isnan(((float3)(ref netVertexMatchData2.m_Offsets)).xy)))
					{
						val3.x = netVertexMatchData2.m_Offsets.x;
						val3.y = math.select(netVertexMatchData2.m_Offsets.z, netVertexMatchData2.m_Offsets.y, math.isnan(netVertexMatchData2.m_Offsets.z));
						if ((netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0)
						{
							val3 = -((float2)(ref val3)).yx;
						}
						val3 += netCompositionPiece.m_Offset.x;
						if (num > 0f)
						{
							val.w = (val3.x - middleOffset) / num + 1f;
						}
						if (num2 > 0f)
						{
							val2.x = (val3.y - middleOffset) / num2;
						}
						flag = true;
					}
				}
				if (!flag)
				{
					float2 val4 = float2.op_Implicit(netCompositionPiece.m_Offset.x);
					val4.x -= netCompositionPiece.m_Size.x * 0.5f;
					val4.y += netCompositionPiece.m_Size.x * 0.5f;
					if (num > 0f)
					{
						val.w = (val4.x - middleOffset) / num + 1f;
					}
					if (num2 > 0f)
					{
						val2.x = (val4.y - middleOffset) / num2;
					}
				}
			}
			else
			{
				if (!netVertexMatchData.HasComponent(netCompositionPiece.m_Piece))
				{
					continue;
				}
				NetVertexMatchData netVertexMatchData3 = netVertexMatchData[netCompositionPiece.m_Piece];
				if (math.isnan(netVertexMatchData3.m_Offsets.x))
				{
					continue;
				}
				float num3 = netVertexMatchData3.m_Offsets.x;
				for (int j = 0; j < 3; j++)
				{
					if ((netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0)
					{
						num3 = 0f - num3;
					}
					num3 += netCompositionPiece.m_Offset.x;
					if ((netCompositionPiece.m_SectionFlags & NetSectionFlags.Right) != 0)
					{
						num3 = (num3 - middleOffset) / num2;
						if (val2.z != 1f)
						{
							val2.w = num3;
						}
						else if (val2.y != 1f)
						{
							val2.z = num3;
						}
						else
						{
							val2.y = num3;
						}
					}
					else
					{
						num3 = (num3 - middleOffset) / num + 1f;
						if (val.y != 0f)
						{
							val.x = num3;
						}
						else if (val.z != 0f)
						{
							val.y = num3;
						}
						else
						{
							val.z = num3;
						}
					}
					if (j == 0)
					{
						if (math.isnan(netVertexMatchData3.m_Offsets.y))
						{
							break;
						}
						num3 = netVertexMatchData3.m_Offsets.y;
					}
					else
					{
						if (math.isnan(netVertexMatchData3.m_Offsets.z))
						{
							break;
						}
						num3 = netVertexMatchData3.m_Offsets.z;
					}
				}
			}
		}
		if (val.x > val.y)
		{
			((float4)(ref val)).xy = ((float4)(ref val)).yx;
		}
		if (val2.z > val2.w)
		{
			((float4)(ref val2)).zw = ((float4)(ref val2)).wz;
		}
		if (val.y > val.z)
		{
			((float4)(ref val)).yz = ((float4)(ref val)).zy;
		}
		if (val2.y > val2.z)
		{
			((float4)(ref val2)).yz = ((float4)(ref val2)).zy;
		}
		if (val.x > val.y)
		{
			((float4)(ref val)).xy = ((float4)(ref val)).yx;
		}
		if (val2.z > val2.w)
		{
			((float4)(ref val2)).zw = ((float4)(ref val2)).wz;
		}
		if (val.z <= val.x)
		{
			val.z = math.lerp(val.x, val.w, 2f / 3f);
		}
		if (val2.y >= val2.w)
		{
			val2.y = math.lerp(val2.w, val2.x, 2f / 3f);
		}
		if (val.y <= val.x)
		{
			val.y = math.lerp(val.x, val.z, 0.5f);
		}
		if (val2.z >= val2.w)
		{
			val2.z = math.lerp(val2.w, val2.y, 0.5f);
		}
		if (val.y < val.x + 1E-05f)
		{
			val.y = val.x;
		}
		if (val.w < val.z + 1E-05f)
		{
			val.z = val.w;
		}
		if (val2.y < val2.x + 1E-05f)
		{
			val2.y = val2.x;
		}
		if (val2.w < val2.z + 1E-05f)
		{
			val2.z = val2.w;
		}
		compositionData.m_SyncVertexOffsetsLeft = val;
		compositionData.m_SyncVertexOffsetsRight = val2;
	}

	public static void CalculatePlaceableData(ref PlaceableNetComposition placeableData, NativeArray<NetCompositionPiece> pieces, ComponentLookup<PlaceableNetPieceData> placeableNetPieceData)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		placeableData.m_ConstructionCost = 0u;
		placeableData.m_UpkeepCost = 0f;
		for (int i = 0; i < pieces.Length; i++)
		{
			NetCompositionPiece netCompositionPiece = pieces[i];
			if (placeableNetPieceData.HasComponent(netCompositionPiece.m_Piece))
			{
				PlaceableNetPieceData placeableNetPieceData2 = placeableNetPieceData[netCompositionPiece.m_Piece];
				placeableData.m_ConstructionCost += placeableNetPieceData2.m_ConstructionCost;
				placeableData.m_ElevationCost += placeableNetPieceData2.m_ElevationCost;
				placeableData.m_UpkeepCost += placeableNetPieceData2.m_UpkeepCost;
			}
		}
	}

	public static void AddCompositionLanes<TNetCompositionPieceList>(Entity entity, ref NetCompositionData compositionData, TNetCompositionPieceList pieces, NativeList<NetCompositionLane> netLanes, DynamicBuffer<NetCompositionCarriageway> carriageways, ComponentLookup<NetLaneData> netLaneData, BufferLookup<NetPieceLane> netPieceLanes) where TNetCompositionPieceList : INativeList<NetCompositionPiece>
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a65: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a68: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0acd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b44: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = true;
		Bounds3 val = default(Bounds3);
		((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
		Bounds3 val2 = default(Bounds3);
		((Bounds3)(ref val2))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
		bool2 val3 = default(bool2);
		bool2 val4 = default(bool2);
		NetCompositionCarriageway netCompositionCarriageway = default(NetCompositionCarriageway);
		NativeList<TempLaneData> val5 = default(NativeList<TempLaneData>);
		val5._002Ector(256, AllocatorHandle.op_Implicit((Allocator)2));
		NativeList<TempLaneGroup> val6 = default(NativeList<TempLaneGroup>);
		val6._002Ector(32, AllocatorHandle.op_Implicit((Allocator)2));
		float3 val11 = default(float3);
		float3 val12 = default(float3);
		for (int i = 0; i < ((IIndexable<NetCompositionPiece>)pieces).Length; i++)
		{
			NetCompositionPiece netCompositionPiece = ((INativeList<NetCompositionPiece>)pieces)[i];
			if ((netCompositionPiece.m_PieceFlags & NetPieceFlags.BlockTraffic) != 0 && num3 != 0)
			{
				num2++;
				num3 = 0;
				if (math.all(val4))
				{
					flag = false;
				}
				if (math.any(val3 | val4) && carriageways.IsCreated)
				{
					Bounds3 val7 = val | val2;
					bool2 val8 = val3 | val4;
					if (math.any(val3 != val4))
					{
						if (math.all(val4) & math.any(val3))
						{
							val7 = val;
							val8 = val3;
						}
						else if (math.any(val4))
						{
							val7 = val2;
							val8 = val4;
						}
					}
					netCompositionCarriageway.m_Position = MathUtils.Center(val7);
					netCompositionCarriageway.m_Width = MathUtils.Size(val7).x;
					if (math.all(val8))
					{
						netCompositionCarriageway.m_Flags &= ~LaneFlags.Invert;
						netCompositionCarriageway.m_Flags |= LaneFlags.Twoway;
					}
					else if (val8.x)
					{
						netCompositionCarriageway.m_Flags &= ~(LaneFlags.Invert | LaneFlags.Twoway);
					}
					else
					{
						netCompositionCarriageway.m_Flags &= ~LaneFlags.Twoway;
						netCompositionCarriageway.m_Flags |= LaneFlags.Invert;
					}
					carriageways.Add(netCompositionCarriageway);
				}
				((Bounds3)(ref val))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
				((Bounds3)(ref val2))._002Ector(float3.op_Implicit(float.MaxValue), float3.op_Implicit(float.MinValue));
				val3 = default(bool2);
				val4 = default(bool2);
				netCompositionCarriageway = default(NetCompositionCarriageway);
			}
			if (!netPieceLanes.HasBuffer(netCompositionPiece.m_Piece))
			{
				continue;
			}
			DynamicBuffer<NetPieceLane> val9 = netPieceLanes[netCompositionPiece.m_Piece];
			bool flag2 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0;
			bool flag3 = (netCompositionPiece.m_SectionFlags & NetSectionFlags.FlipLanes) != 0;
			for (int j = 0; j < val9.Length; j++)
			{
				NetPieceLane netPieceLane = val9[math.select(j, val9.Length - j - 1, flag2)];
				NetLaneData netLaneData2 = netLaneData[netPieceLane.m_Lane];
				TempLaneData tempLaneData = default(TempLaneData);
				TempLaneGroup tempLaneGroup = default(TempLaneGroup);
				netLaneData2.m_Flags |= netPieceLane.m_ExtraFlags;
				if (flag2)
				{
					netPieceLane.m_Position.x = 0f - netPieceLane.m_Position.x;
				}
				if ((netLaneData2.m_Flags & LaneFlags.Twoway) != 0)
				{
					val3 |= (netLaneData2.m_Flags & LaneFlags.Track) != 0;
					val4 |= (netLaneData2.m_Flags & LaneFlags.Road) != 0;
				}
				else if (flag2 != flag3)
				{
					netLaneData2.m_Flags |= LaneFlags.Invert;
					val3.y |= (netLaneData2.m_Flags & LaneFlags.Track) != 0;
					val4.y |= (netLaneData2.m_Flags & LaneFlags.Road) != 0;
				}
				else
				{
					val3.x |= (netLaneData2.m_Flags & LaneFlags.Track) != 0;
					val4.x |= (netLaneData2.m_Flags & LaneFlags.Road) != 0;
				}
				float3 val10 = netCompositionPiece.m_Offset + netPieceLane.m_Position;
				tempLaneGroup.m_Flags = netLaneData2.m_Flags;
				if ((netLaneData2.m_Flags & LaneFlags.Road) != 0)
				{
					((float3)(ref val11))._002Ector(netLaneData2.m_Width * 0.5f, 0f, 0f);
					val2 |= new Bounds3(val10 - val11, val10 + val11);
					netCompositionCarriageway.m_Flags |= netLaneData2.m_Flags;
				}
				if ((netLaneData2.m_Flags & LaneFlags.Track) != 0)
				{
					((float3)(ref val12))._002Ector(netLaneData2.m_Width * 0.5f, 0f, 0f);
					val |= new Bounds3(val10 - val12, val10 + val12);
					netCompositionCarriageway.m_Flags |= netLaneData2.m_Flags;
				}
				if (num3 != 0)
				{
					TempLaneGroup tempLaneGroup2 = val6[val6.Length - 1];
					if (tempLaneGroup.IsCompatible(tempLaneGroup2))
					{
						tempLaneData.m_GroupIndex = val6.Length - 1;
						val5.Add(ref tempLaneData);
						if ((tempLaneGroup2.m_Flags & (LaneFlags.DisconnectedStart | LaneFlags.DisconnectedEnd)) != 0)
						{
							tempLaneGroup2.m_Prefab = netPieceLane.m_Lane;
							tempLaneGroup2.m_Flags = tempLaneGroup.m_Flags & (tempLaneGroup2.m_Flags | LaneFlags.Track);
						}
						else
						{
							tempLaneGroup2.m_Flags &= tempLaneGroup.m_Flags | LaneFlags.Track;
						}
						ref float3 position = ref tempLaneGroup2.m_Position;
						position += val10;
						tempLaneGroup2.m_LaneCount++;
						val6[val6.Length - 1] = tempLaneGroup2;
						continue;
					}
				}
				tempLaneData.m_GroupIndex = num++;
				val5.Add(ref tempLaneData);
				tempLaneGroup.m_Prefab = netPieceLane.m_Lane;
				tempLaneGroup.m_Position = val10;
				tempLaneGroup.m_LaneCount = 1;
				tempLaneGroup.m_CarriagewayIndex = num2;
				val6.Add(ref tempLaneGroup);
				num3++;
			}
		}
		if (num3 != 0)
		{
			if (math.all(val4))
			{
				flag = false;
			}
			if (math.any(val3 | val4) && carriageways.IsCreated)
			{
				Bounds3 val13 = val | val2;
				bool2 val14 = val3 | val4;
				if (math.any(val3 != val4))
				{
					if (math.all(val4) & math.any(val3))
					{
						val13 = val;
						val14 = val3;
					}
					else if (math.any(val4))
					{
						val13 = val2;
						val14 = val4;
					}
				}
				netCompositionCarriageway.m_Position = MathUtils.Center(val13);
				netCompositionCarriageway.m_Width = MathUtils.Size(val13).x;
				if (math.all(val14))
				{
					netCompositionCarriageway.m_Flags &= ~LaneFlags.Invert;
					netCompositionCarriageway.m_Flags |= LaneFlags.Twoway;
				}
				else if (val14.x)
				{
					netCompositionCarriageway.m_Flags &= ~(LaneFlags.Invert | LaneFlags.Twoway);
				}
				else
				{
					netCompositionCarriageway.m_Flags &= ~LaneFlags.Twoway;
					netCompositionCarriageway.m_Flags |= LaneFlags.Invert;
				}
				carriageways.Add(netCompositionCarriageway);
			}
		}
		if (flag)
		{
			compositionData.m_State |= CompositionState.SeparatedCarriageways;
		}
		int num4 = 0;
		int num5 = -1;
		int num6 = 0;
		for (int k = 0; k < ((IIndexable<NetCompositionPiece>)pieces).Length; k++)
		{
			NetCompositionPiece netCompositionPiece2 = ((INativeList<NetCompositionPiece>)pieces)[k];
			if (!netPieceLanes.HasBuffer(netCompositionPiece2.m_Piece))
			{
				continue;
			}
			DynamicBuffer<NetPieceLane> val15 = netPieceLanes[netCompositionPiece2.m_Piece];
			bool flag4 = (netCompositionPiece2.m_SectionFlags & NetSectionFlags.Invert) != 0;
			bool flag5 = (netCompositionPiece2.m_SectionFlags & NetSectionFlags.FlipLanes) != 0;
			for (int l = 0; l < val15.Length; l++)
			{
				NetPieceLane netPieceLane2 = val15[math.select(l, val15.Length - l - 1, flag4)];
				NetLaneData netLaneData3 = netLaneData[netPieceLane2.m_Lane];
				TempLaneData tempLaneData2 = val5[num4];
				TempLaneGroup tempLaneGroup3 = val6[tempLaneData2.m_GroupIndex];
				netLaneData3.m_Flags |= netPieceLane2.m_ExtraFlags;
				if (flag4)
				{
					netPieceLane2.m_Position.x = 0f - netPieceLane2.m_Position.x;
				}
				if (flag4 != flag5)
				{
					netLaneData3.m_Flags |= LaneFlags.Invert;
				}
				NetCompositionLane netCompositionLane = new NetCompositionLane
				{
					m_Lane = netPieceLane2.m_Lane,
					m_Position = netCompositionPiece2.m_Offset + netPieceLane2.m_Position,
					m_Carriageway = (byte)tempLaneGroup3.m_CarriagewayIndex,
					m_Group = (byte)tempLaneData2.m_GroupIndex,
					m_Index = (byte)num4,
					m_Flags = netLaneData3.m_Flags
				};
				if (tempLaneGroup3.m_LaneCount > 1)
				{
					netCompositionLane.m_Flags |= LaneFlags.Slave;
				}
				if (tempLaneData2.m_GroupIndex != num5)
				{
					num5 = tempLaneData2.m_GroupIndex;
					netCompositionLane.m_Flags |= (LaneFlags)((flag4 != flag5) ? 524288 : 262144);
					num6 = 0;
				}
				if (++num6 == tempLaneGroup3.m_LaneCount)
				{
					netCompositionLane.m_Flags |= (LaneFlags)((flag4 != flag5) ? 262144 : 524288);
				}
				netLanes.Add(ref netCompositionLane);
				num4++;
			}
		}
		int2 val16 = int2.op_Implicit(0);
		int2 val17 = int2.op_Implicit(0);
		for (int m = 0; m < netLanes.Length; m++)
		{
			NetCompositionLane netCompositionLane2 = netLanes[m];
			if ((netCompositionLane2.m_Flags & LaneFlags.Parking) != 0)
			{
				int num7 = FindClosestLane(netLanes, m, netCompositionLane2.m_Position, LaneFlags.Road);
				if (num7 != -1)
				{
					NetCompositionLane netCompositionLane3 = netLanes[num7];
					if (((netCompositionLane2.m_Flags ^ netCompositionLane3.m_Flags) & LaneFlags.Invert) != 0)
					{
						netCompositionLane2.m_Flags ^= LaneFlags.Invert;
					}
					LaneFlags laneFlags = ((num7 < m != ((netCompositionLane3.m_Flags & LaneFlags.Invert) != 0)) ? LaneFlags.ParkingRight : LaneFlags.ParkingLeft);
					netCompositionLane2.m_Flags |= laneFlags;
					netLanes[m] = netCompositionLane2;
					if ((netCompositionLane2.m_Flags & LaneFlags.Virtual) == 0)
					{
						netCompositionLane3.m_Flags |= laneFlags;
						netLanes[num7] = netCompositionLane3;
					}
				}
			}
			if ((netCompositionLane2.m_Flags & LaneFlags.Road) != 0)
			{
				if ((netCompositionLane2.m_Flags & LaneFlags.Invert) != 0)
				{
					compositionData.m_State |= CompositionState.HasBackwardRoadLanes;
					val17.x++;
				}
				else
				{
					compositionData.m_State |= CompositionState.HasForwardRoadLanes;
					val16.x++;
				}
			}
			if ((netCompositionLane2.m_Flags & LaneFlags.Track) != 0)
			{
				if ((netCompositionLane2.m_Flags & LaneFlags.Invert) != 0)
				{
					compositionData.m_State |= CompositionState.HasBackwardTrackLanes;
					val17.y++;
				}
				else
				{
					compositionData.m_State |= CompositionState.HasForwardTrackLanes;
					val16.y++;
				}
			}
			if ((netCompositionLane2.m_Flags & LaneFlags.Pedestrian) != 0)
			{
				compositionData.m_State |= CompositionState.HasPedestrianLanes;
			}
		}
		if (math.any(val16 != val17))
		{
			compositionData.m_State |= CompositionState.Asymmetric;
		}
		if (math.any(val16 > 1) | math.any(val17 > 1))
		{
			compositionData.m_State |= CompositionState.Multilane;
		}
		for (int n = 0; n < val6.Length; n++)
		{
			TempLaneGroup tempLaneGroup4 = val6[n];
			if (tempLaneGroup4.m_LaneCount > 1)
			{
				NetCompositionLane netCompositionLane4 = new NetCompositionLane
				{
					m_Lane = tempLaneGroup4.m_Prefab,
					m_Position = tempLaneGroup4.m_Position / (float)tempLaneGroup4.m_LaneCount,
					m_Carriageway = (byte)tempLaneGroup4.m_CarriagewayIndex,
					m_Group = (byte)n,
					m_Index = (byte)num4,
					m_Flags = (tempLaneGroup4.m_Flags | LaneFlags.Master)
				};
				netLanes.Add(ref netCompositionLane4);
				num4++;
			}
		}
		val5.Dispose();
		val6.Dispose();
		if (num4 >= 256)
		{
			throw new Exception($"Too many lanes: {entity.Index}");
		}
	}

	private static int FindClosestLane(NativeList<NetCompositionLane> lanes, int startIndex, float3 position, LaneFlags flags)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		int num = startIndex - 1;
		int num2 = startIndex + 1;
		while (true)
		{
			if (num >= 0 && num2 < lanes.Length)
			{
				NetCompositionLane netCompositionLane = lanes[num];
				NetCompositionLane netCompositionLane2 = lanes[num2];
				if (math.lengthsq(netCompositionLane.m_Position - position) <= math.lengthsq(netCompositionLane2.m_Position - position))
				{
					if ((netCompositionLane.m_Flags & flags) != 0)
					{
						return num;
					}
					num--;
				}
				else
				{
					if ((netCompositionLane2.m_Flags & flags) != 0)
					{
						return num2;
					}
					num2++;
				}
			}
			else if (num >= 0)
			{
				if ((lanes[num].m_Flags & flags) != 0)
				{
					return num;
				}
				num--;
			}
			else
			{
				if (num2 >= lanes.Length)
				{
					break;
				}
				if ((lanes[num2].m_Flags & flags) != 0)
				{
					return num2;
				}
				num2++;
			}
		}
		return -1;
	}

	public static void CalculateRoundaboutSize(ref NetCompositionData compositionData, NativeArray<NetCompositionPiece> pieces, ComponentLookup<NetLaneData> netLaneData, BufferLookup<NetPieceLane> netPieceLanes)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		float2 val = float2.op_Implicit(0f);
		float2 val2 = float2.op_Implicit(float.MaxValue);
		float4 val3 = float4.op_Implicit(0f);
		for (int i = 0; i < pieces.Length; i++)
		{
			NetCompositionPiece netCompositionPiece = pieces[i];
			if (HasRoad(netCompositionPiece.m_Piece, netLaneData, netPieceLanes))
			{
				float num = (((netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) == 0) ? ((compositionData.m_Width + netCompositionPiece.m_Size.x) * 0.5f - netCompositionPiece.m_Offset.x) : ((compositionData.m_Width + netCompositionPiece.m_Size.x) * 0.5f + netCompositionPiece.m_Offset.x));
				if ((netCompositionPiece.m_SectionFlags & NetSectionFlags.Invert) != 0 != ((netCompositionPiece.m_SectionFlags & NetSectionFlags.FlipLanes) != 0))
				{
					val.x = math.max(val.x, num);
					val2.y = math.min(val2.y, num);
					val3.x += netCompositionPiece.m_Size.x;
					val3.w = math.max(val3.w, netCompositionPiece.m_Size.x);
				}
				else
				{
					val.y = math.max(val.y, num);
					val2.x = math.min(val2.x, num);
					val3.y += netCompositionPiece.m_Size.x;
					val3.z = math.max(val3.z, netCompositionPiece.m_Size.x);
				}
			}
		}
		compositionData.m_RoundaboutSize = math.select(val, math.max(val2, val), val2 < float.MaxValue);
		compositionData.m_RoundaboutSize = math.select(compositionData.m_RoundaboutSize, float2.op_Implicit(compositionData.m_Width * 0.5f), compositionData.m_RoundaboutSize == 0f);
		ref float2 roundaboutSize = ref compositionData.m_RoundaboutSize;
		roundaboutSize += math.max(((float4)(ref val3)).xy, ((float4)(ref val3)).zw) / 3f;
	}

	private static bool HasRoad(Entity piece, ComponentLookup<NetLaneData> netLaneData, BufferLookup<NetPieceLane> netPieceLanes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!netPieceLanes.HasBuffer(piece))
		{
			return false;
		}
		DynamicBuffer<NetPieceLane> val = netPieceLanes[piece];
		for (int i = 0; i < val.Length; i++)
		{
			if ((netLaneData[val[i].m_Lane].m_Flags & LaneFlags.Road) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public static CompositionFlags GetElevationFlags(Elevation startElevation, Elevation middleElevation, Elevation endElevation, NetGeometryData prefabGeometryData)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		CompositionFlags result = default(CompositionFlags);
		float2 val = math.max(float2.op_Implicit(math.max(math.cmin(startElevation.m_Elevation), math.cmin(endElevation.m_Elevation))), middleElevation.m_Elevation);
		float3 val2 = default(float3);
		((float3)(ref val2))._002Ector(math.cmin(startElevation.m_Elevation), math.cmin(endElevation.m_Elevation), math.cmin(middleElevation.m_Elevation));
		float3 val3 = default(float3);
		((float3)(ref val3))._002Ector(math.cmax(startElevation.m_Elevation), math.cmax(endElevation.m_Elevation), math.cmax(middleElevation.m_Elevation));
		float2 val4 = math.min(float2.op_Implicit(math.min(math.cmax(startElevation.m_Elevation), math.cmax(endElevation.m_Elevation))), middleElevation.m_Elevation);
		if (math.all(val >= prefabGeometryData.m_ElevationLimit * 2f) || (prefabGeometryData.m_Flags & GeometryFlags.RequireElevated) != 0)
		{
			if ((prefabGeometryData.m_Flags & GeometryFlags.ElevatedIsRaised) != 0)
			{
				result.m_Left |= CompositionFlags.Side.Raised;
				result.m_Right |= CompositionFlags.Side.Raised;
			}
			else
			{
				result.m_General |= CompositionFlags.General.Elevated;
			}
		}
		else if (math.cmax(val2) <= prefabGeometryData.m_ElevationLimit * -2f && math.cmin(val3) <= prefabGeometryData.m_ElevationLimit * -3f)
		{
			result.m_General |= CompositionFlags.General.Tunnel;
		}
		else
		{
			if (val.x >= prefabGeometryData.m_ElevationLimit)
			{
				if ((prefabGeometryData.m_Flags & GeometryFlags.RaisedIsElevated) != 0)
				{
					result.m_General |= CompositionFlags.General.Elevated;
				}
				else
				{
					result.m_Left |= CompositionFlags.Side.Raised;
				}
			}
			else if (val4.x <= 0f - prefabGeometryData.m_ElevationLimit)
			{
				if ((prefabGeometryData.m_Flags & GeometryFlags.LoweredIsTunnel) != 0)
				{
					result.m_General |= CompositionFlags.General.Tunnel;
				}
				else
				{
					result.m_Left |= CompositionFlags.Side.Lowered;
				}
			}
			if (val.y >= prefabGeometryData.m_ElevationLimit)
			{
				if ((prefabGeometryData.m_Flags & GeometryFlags.RaisedIsElevated) != 0)
				{
					result.m_General |= CompositionFlags.General.Elevated;
				}
				else
				{
					result.m_Right |= CompositionFlags.Side.Raised;
				}
			}
			else if (val4.y <= 0f - prefabGeometryData.m_ElevationLimit)
			{
				if ((prefabGeometryData.m_Flags & GeometryFlags.LoweredIsTunnel) != 0)
				{
					result.m_General |= CompositionFlags.General.Tunnel;
				}
				else
				{
					result.m_Right |= CompositionFlags.Side.Lowered;
				}
			}
		}
		return result;
	}
}
