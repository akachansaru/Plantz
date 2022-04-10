using System;
using UnityEngine;

[Serializable]
public struct Vector4Serializable
{
    public float x, y, z, w;

    public Vector4Serializable(float x, float y, float z, float w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public Vector4Serializable(Vector4 vector4)
    {
        this.x = (float)vector4.x;
        this.y = (float)vector4.y;
        this.z = (float)vector4.z;
        this.w = (float)vector4.w;
    }

    public Vector4 ToVector4()
    {
        return new Vector4(x, y, z, w);
    }

    public override string ToString()
    {
        return ("(" + x + ", " + y + ", " + z + ", " + w + ")");
    }
}

[Serializable]
public struct Vector3Serializable
{
    public float x, y, z;

    public Vector3Serializable(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Serializable(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public Vector3Serializable(float xyz)
    {
        x = xyz;
        y = xyz;
        z = xyz;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public static Vector3Serializable operator +(Vector3Serializable a, Vector3Serializable b)
        => new Vector3Serializable(a.x + b.x, a.y + b.y, a.z + b.z);

    public static Vector3Serializable operator -(Vector3Serializable a, Vector3Serializable b)
    => new Vector3Serializable(a.x - b.x, a.y - b.y, a.z - b.z);

    public static Vector3Serializable operator *(int a, Vector3Serializable b)
        => new Vector3Serializable(a * b.x, a * b.y, a * b.z);

    public static Vector3Serializable operator *(float a, Vector3Serializable b)
        => new Vector3Serializable(a * b.x, a * b.y, a * b.z);

    public static Vector3Serializable operator *(Vector3Serializable a, float b)
     => new Vector3Serializable(a.x * b, a.y * b, a.z * b);

    public static Vector3Serializable operator /(Vector3Serializable a, float b)
        => new Vector3Serializable(a.x / b, a.y / b, a.z / b);

    public override string ToString()
    {
        return ("(" + x + ", " + y + ", " + z + ")");
    }
}

[Serializable]
public struct Vector2Serializable
{
    public float x, y;

    public static Vector2Serializable AverageValues(Vector2Serializable v1, Vector2Serializable v2)
    {
        return new Vector2Serializable((v1.x + v2.x) / 2, (v1.y + v2.y) / 2);
    }

    public Vector2Serializable(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2Serializable(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2Serializable(Vector2 vector2)
    {
        this.x = (float)vector2.x;
        this.y = (float)vector2.y;
    }

    public Vector3 ToVector2()
    {
        return new Vector3(x, y);
    }

    public override string ToString()
    {
        return ("(" + x + ", " + y + ")");
    }
}

