using System;
using UnityEngine;

/// <summary>
/// Unity에서 직렬화가 가능한 글로벌 고유 식별자(GUID)를 나타내며, 게임 스크립트에서 사용할 수 있는 구조체입니다.
/// </summary>
[Serializable]
public struct SerializableGuid : IEquatable<SerializableGuid>
{
    // GUID를 저장하는 네 부분으로 구성된 데이터 (Unity 직렬화를 위해 설계됨)
    [SerializeField, HideInInspector] public uint Part1;
    [SerializeField, HideInInspector] public uint Part2;
    [SerializeField, HideInInspector] public uint Part3;
    [SerializeField, HideInInspector] public uint Part4;

    // 비어 있는 GUID를 나타내는 정적 속성
    public static SerializableGuid Empty => new(0, 0, 0, 0);

    // 네 개의 uint 값으로 GUID를 초기화하는 생성자
    public SerializableGuid(uint val1, uint val2, uint val3, uint val4)
    {
        Part1 = val1;
        Part2 = val2;
        Part3 = val3;
        Part4 = val4;
    }

    // System.Guid를 SerializableGuid로 변환하는 생성자
    public SerializableGuid(Guid guid)
    {
        byte[] bytes = guid.ToByteArray();
        Part1 = BitConverter.ToUInt32(bytes, 0);
        Part2 = BitConverter.ToUInt32(bytes, 4);
        Part3 = BitConverter.ToUInt32(bytes, 8);
        Part4 = BitConverter.ToUInt32(bytes, 12);
    }

    // 새 GUID를 생성하는 정적 메서드
    public static SerializableGuid NewGuid() => Guid.NewGuid().ToSerializableGuid();

    // 16진수 문자열을 SerializableGuid로 변환하는 정적 메서드
    public static SerializableGuid FromHexString(string hexString)
    {
        if (hexString.Length != 32)
        {
            return Empty;
        }

        return new SerializableGuid
        (
            Convert.ToUInt32(hexString.Substring(0, 8), 16),
            Convert.ToUInt32(hexString.Substring(8, 8), 16),
            Convert.ToUInt32(hexString.Substring(16, 8), 16),
            Convert.ToUInt32(hexString.Substring(24, 8), 16)
        );
    }

    // GUID를 16진수 문자열로 변환
    public string ToHexString()
    {
        return $"{Part1:X8}{Part2:X8}{Part3:X8}{Part4:X8}";
    }

    // SerializableGuid를 System.Guid로 변환
    public Guid ToGuid()
    {
        var bytes = new byte[16];
        BitConverter.GetBytes(Part1).CopyTo(bytes, 0);
        BitConverter.GetBytes(Part2).CopyTo(bytes, 4);
        BitConverter.GetBytes(Part3).CopyTo(bytes, 8);
        BitConverter.GetBytes(Part4).CopyTo(bytes, 12);
        return new Guid(bytes);
    }

    // SerializableGuid와 System.Guid 간의 암시적 변환 지원
    public static implicit operator Guid(SerializableGuid serializableGuid) => serializableGuid.ToGuid();
    public static implicit operator SerializableGuid(Guid guid) => new SerializableGuid(guid);

    // 두 GUID가 동일한지 비교
    public override bool Equals(object obj)
    {
        return obj is SerializableGuid guid && this.Equals(guid);
    }

    public bool Equals(SerializableGuid other)
    {
        return Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3 && Part4 == other.Part4;
    }

    // GUID의 해시코드 생성
    public override int GetHashCode()
    {
        return HashCode.Combine(Part1, Part2, Part3, Part4);
    }

    // 동등 연산자 구현
    public static bool operator ==(SerializableGuid left, SerializableGuid right) => left.Equals(right);
    public static bool operator !=(SerializableGuid left, SerializableGuid right) => !(left == right);
}