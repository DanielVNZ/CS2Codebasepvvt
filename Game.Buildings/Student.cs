using System;
using Colossal.Serialization.Entities;
using Unity.Entities;

namespace Game.Buildings;

public struct Student : IBufferElementData, IEmptySerializable, IEquatable<Student>
{
	public Entity m_Student;

	public Student(Entity student)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		m_Student = student;
	}

	public bool Equals(Student other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((Entity)(ref m_Student)).Equals(other.m_Student);
	}

	public static implicit operator Entity(Student student)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return student.m_Student;
	}
}
