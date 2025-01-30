
//(c)Michael Thomas Lee

using System;
using UnityEngine;

/// <summary>
/// Extended Mutable Vector
/// Holds ability to round as well as a specified hash code generator.
/// Use when Unity Vector3 becomes too messy to code with.
/// Use Clone or CloneToLayer to avoid complications. Remember that stuff goes by reference.
/// Intended for 2D tile graphics.
/// </summary>
public class ExtVector
{
    public Vector3 Vector3 {
        get { return new Vector3(X,Y,Z); }
    }

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public ExtVector(int x = 0, int y = 0, int z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public ExtVector(float x = 0, float y = 0, float z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public ExtVector(Vector3 v) {
        X = v.x;
        Y = v.y;
        Z = v.z;
    }

    /// <summary>
    /// Retains layer (Z) of first parameter and adds X and Y coordinates
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>New ExtVector</returns>
    public static ExtVector operator +(ExtVector a, ExtVector b) {
        return new ExtVector(a.X+b.X, a.Y+b.Y, a.Y);
    }

    /// <summary>
    /// Retains layer (Z) of first parameter and subtracts X and Y coordinates
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns>New ExtVector</returns>
    public static ExtVector operator -(ExtVector a, ExtVector b) {
        return new ExtVector(a.X-b.X, a.Y-b.Y, a.Y);
    }

    /// <summary>
    /// Rounds the x,y,z (can ignore, one or more) components to whole numbers
    /// </summary>
    /// <param name="x">X parameter</param>
    /// <param name="y">Y parameter</param>
    /// <param name="z">Z parameter</param>
    /// <param name="decimals">Rounding decimals</param>
    public void Round(bool x = true, bool y = true, bool z = true, int decimals = 0)
    {
        if (x) X = (int)Math.Round(X,decimals);
        if (x) Y = (int)Math.Round(Y,decimals);
        if (x) Z = (int)Math.Round(Z,decimals);
    }

    /// <summary>
    /// Lerp from Vector3 position to this. Can ignore x,y or z components.
    /// </summary>
    /// <param name="position">The position to lerp from</param>
    /// <param name="t">The lerp time</param>
    /// <param name="lx">Lerp x?</param>
    /// <param name="ly">Lerp y?</param>
    /// <param name="lz">Lerp z?</param>
    /// <returns></returns>
    public Vector3 LerpTo(Vector3 position, float t, bool lx = true, bool ly = true, bool lz = true)
    {
        var x = lx ? Mathf.Lerp(position.x, X, t) : position.x;
        var y = ly ? Mathf.Lerp(position.y, Y, t) : position.y;
        var z = lz ? Mathf.Lerp(position.z, Z, t) : position.z;
        return new Vector3(x,y,z);
    }

    /// <summary>
    /// Clone this vector into a new one.
    /// Use this when you need to modify the vector, to affect
    /// all references.
    /// </summary>
    /// <returns>A new vector, cloned from the original</returns>
    public ExtVector Clone()
    {
        return new ExtVector(X,Y,Z);
    }

    /// <summary>
    /// Same as Clone()
    /// Allows you to specify tile layer
    /// </summary>
    /// <param name="z"></param>
    /// <returns>A new vector, cloned from the original, but with different layer(Z)</returns>
    public ExtVector CloneToLayer(int z)
    {
        return new ExtVector(X,Y,z);
    }

    public override string ToString()
    {
        return Vector3.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if(this == obj)
            return true;
        if (obj.GetType() != GetType())
            return false;
        var other = (ExtVector) obj;
        return (X == other.X) && (Y == other.Y) && (Z == other.Z);
    }

    public override int GetHashCode()
    {
        return X.GetHashCode() + Y.GetHashCode() + Z.GetHashCode();
    }
}
