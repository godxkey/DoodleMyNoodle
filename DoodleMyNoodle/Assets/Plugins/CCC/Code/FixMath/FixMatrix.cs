using System;
using UnityEngine;

/// <summary>
/// Provides XNA-like 4x4 matrix math.
/// </summary>
[NetSerializable]
[Serializable]
public struct FixMatrix
{
    public static Matrix4x4 a;
    public static FixMatrix b;

    /// <summary>
    /// Value at row 1, column 1 of the matrix.
    /// </summary>
    public Fix64 M11;

    /// <summary>
    /// Value at row 2, column 1 of the matrix.
    /// </summary>
    public Fix64 M21;

    /// <summary>
    /// Value at row 3, column 1 of the matrix.
    /// </summary>
    public Fix64 M31;

    /// <summary>
    /// Value at row 4, column 1 of the matrix.
    /// </summary>
    public Fix64 M41;

    /// <summary>
    /// Value at row 1, column 2 of the matrix.
    /// </summary>
    public Fix64 M12;

    /// <summary>
    /// Value at row 2, column 2 of the matrix.
    /// </summary>
    public Fix64 M22;

    /// <summary>
    /// Value at row 3, column 2 of the matrix.
    /// </summary>
    public Fix64 M32;

    /// <summary>
    /// Value at row 4, column 2 of the matrix.
    /// </summary>
    public Fix64 M42;

    /// <summary>
    /// Value at row 1, column 3 of the matrix.
    /// </summary>
    public Fix64 M13;

    /// <summary>
    /// Value at row 2, column 3 of the matrix.
    /// </summary>
    public Fix64 M23;

    /// <summary>
    /// Value at row 3, column 3 of the matrix.
    /// </summary>
    public Fix64 M33;

    /// <summary>
    /// Value at row 4, column 3 of the matrix.
    /// </summary>
    public Fix64 M43;

    /// <summary>
    /// Value at row 1, column 4 of the matrix.
    /// </summary>
    public Fix64 M14;

    /// <summary>
    /// Value at row 2, column 4 of the matrix.
    /// </summary>
    public Fix64 M24;

    /// <summary>
    /// Value at row 3, column 4 of the matrix.
    /// </summary>
    public Fix64 M34;

    /// <summary>
    /// Value at row 4, column 4 of the matrix.
    /// </summary>
    public Fix64 M44;

    /// <summary>
    /// Constructs a new 4 row, 4 column matrix.
    /// </summary>
    /// <param name="m11">Value at row 1, column 1 of the matrix.</param>
    /// <param name="m12">Value at row 1, column 2 of the matrix.</param>
    /// <param name="m13">Value at row 1, column 3 of the matrix.</param>
    /// <param name="m14">Value at row 1, column 4 of the matrix.</param>
    /// <param name="m21">Value at row 2, column 1 of the matrix.</param>
    /// <param name="m22">Value at row 2, column 2 of the matrix.</param>
    /// <param name="m23">Value at row 2, column 3 of the matrix.</param>
    /// <param name="m24">Value at row 2, column 4 of the matrix.</param>
    /// <param name="m31">Value at row 3, column 1 of the matrix.</param>
    /// <param name="m32">Value at row 3, column 2 of the matrix.</param>
    /// <param name="m33">Value at row 3, column 3 of the matrix.</param>
    /// <param name="m34">Value at row 3, column 4 of the matrix.</param>
    /// <param name="m41">Value at row 4, column 1 of the matrix.</param>
    /// <param name="m42">Value at row 4, column 2 of the matrix.</param>
    /// <param name="m43">Value at row 4, column 3 of the matrix.</param>
    /// <param name="m44">Value at row 4, column 4 of the matrix.</param>
    public FixMatrix(Fix64 m11, Fix64 m12, Fix64 m13, Fix64 m14,
                  Fix64 m21, Fix64 m22, Fix64 m23, Fix64 m24,
                  Fix64 m31, Fix64 m32, Fix64 m33, Fix64 m34,
                  Fix64 m41, Fix64 m42, Fix64 m43, Fix64 m44)
    {
        this.M11 = m11;
        this.M21 = m12;
        this.M31 = m13;
        this.M41 = m14;

        this.M12 = m21;
        this.M22 = m22;
        this.M32 = m23;
        this.M42 = m24;

        this.M13 = m31;
        this.M23 = m32;
        this.M33 = m33;
        this.M43 = m34;

        this.M14 = m41;
        this.M24 = m42;
        this.M34 = m43;
        this.M44 = m44;
    }

    /// <summary>
    /// Gets or sets the translation component of the transform.
    /// </summary>
    public FixVector3 Translation
    {
        get
        {
            return new FixVector3()
            {
                X = M14,
                Y = M24,
                Z = M34
            };
        }
        set
        {
            M14 = value.X;
            M24 = value.Y;
            M34 = value.Z;
        }
    }

    /// <summary>
    /// Gets or sets the backward vector of the matrix.
    /// </summary>
    public FixVector3 Backward
    {
        get
        {
#if !WINDOWS
            FixVector3 vector = new FixVector3();
#else
                FixVector3 vector;
#endif
            vector.X = M13;
            vector.Y = M23;
            vector.Z = M33;
            return vector;
        }
        set
        {
            M13 = value.X;
            M23 = value.Y;
            M33 = value.Z;
        }
    }

    /// <summary>
    /// Gets or sets the down vector of the matrix.
    /// </summary>
    public FixVector3 Down
    {
        get
        {
#if !WINDOWS
            FixVector3 vector = new FixVector3();
#else
                FixVector3 vector;
#endif
            vector.X = -M12;
            vector.Y = -M22;
            vector.Z = -M32;
            return vector;
        }
        set
        {
            M12 = -value.X;
            M22 = -value.Y;
            M32 = -value.Z;
        }
    }

    /// <summary>
    /// Gets or sets the forward vector of the matrix.
    /// </summary>
    public FixVector3 Forward
    {
        get
        {
#if !WINDOWS
            FixVector3 vector = new FixVector3();
#else
                FixVector3 vector;
#endif
            vector.X = -M13;
            vector.Y = -M23;
            vector.Z = -M33;
            return vector;
        }
        set
        {
            M13 = -value.X;
            M23 = -value.Y;
            M33 = -value.Z;
        }
    }

    /// <summary>
    /// Gets or sets the left vector of the matrix.
    /// </summary>
    public FixVector3 Left
    {
        get
        {
#if !WINDOWS
            FixVector3 vector = new FixVector3();
#else
                FixVector3 vector;
#endif
            vector.X = -M11;
            vector.Y = -M21;
            vector.Z = -M31;
            return vector;
        }
        set
        {
            M11 = -value.X;
            M21 = -value.Y;
            M31 = -value.Z;
        }
    }

    /// <summary>
    /// Gets or sets the right vector of the matrix.
    /// </summary>
    public FixVector3 Right
    {
        get
        {
#if !WINDOWS
            FixVector3 vector = new FixVector3();
#else
                FixVector3 vector;
#endif
            vector.X = M11;
            vector.Y = M21;
            vector.Z = M31;
            return vector;
        }
        set
        {
            M11 = value.X;
            M21 = value.Y;
            M31 = value.Z;
        }
    }

    /// <summary>
    /// Gets or sets the up vector of the matrix.
    /// </summary>
    public FixVector3 Up
    {
        get
        {
#if !WINDOWS
            FixVector3 vector = new FixVector3();
#else
                FixVector3 vector;
#endif
            vector.X = M12;
            vector.Y = M22;
            vector.Z = M32;
            return vector;
        }
        set
        {
            M12 = value.X;
            M22 = value.Y;
            M32 = value.Z;
        }
    }


    /// <summary>
    /// Computes the determinant of the matrix.
    /// </summary>
    /// <returns></returns>
    public Fix64 Determinant()
    {
        //Compute the re-used 2x2 determinants.
        Fix64 det1 = M33 * M44 - M43 * M34;
        Fix64 det2 = M23 * M44 - M43 * M24;
        Fix64 det3 = M23 * M34 - M33 * M24;
        Fix64 det4 = M13 * M44 - M43 * M14;
        Fix64 det5 = M13 * M34 - M33 * M14;
        Fix64 det6 = M13 * M24 - M23 * M14;
        return
            (M11 * ((M22 * det1 - M32 * det2) + M42 * det3)) -
            (M21 * ((M12 * det1 - M32 * det4) + M42 * det5)) +
            (M31 * ((M12 * det2 - M22 * det4) + M42 * det6)) -
            (M41 * ((M12 * det3 - M22 * det5) + M32 * det6));
    }

    /// <summary>
    /// Transposes the matrix in-place.
    /// </summary>
    public void Transpose()
    {
        Fix64 intermediate = M21;
        M21 = M12;
        M12 = intermediate;

        intermediate = M31;
        M31 = M13;
        M13 = intermediate;

        intermediate = M41;
        M41 = M14;
        M14 = intermediate;

        intermediate = M32;
        M32 = M23;
        M23 = intermediate;

        intermediate = M42;
        M42 = M24;
        M24 = intermediate;

        intermediate = M43;
        M43 = M34;
        M34 = intermediate;
    }

    /// <summary>
    /// Creates a matrix representing the given axis and angle rotation.
    /// </summary>
    /// <param name="axis">Axis around which to rotate.</param>
    /// <param name="angle">Angle to rotate around the axis.</param>
    /// <returns>FixMatrix created from the axis and angle.</returns>
    public static FixMatrix CreateFromAxisAngle(in FixVector3 axis, in Fix64 angle)
    {
        FixMatrix toReturn;
        CreateFromAxisAngle(in axis, angle, out toReturn);
        return toReturn;
    }

    /// <summary>
    /// Creates a matrix representing the given axis and angle rotation.
    /// </summary>
    /// <param name="axis">Axis around which to rotate.</param>
    /// <param name="angle">Angle to rotate around the axis.</param>
    /// <param name="result">FixMatrix created from the axis and angle.</param>
    public static void CreateFromAxisAngle(in FixVector3 axis, in Fix64 angle, out FixMatrix result)
    {
        Fix64 xx = axis.X * axis.X;
        Fix64 yy = axis.Y * axis.Y;
        Fix64 zz = axis.Z * axis.Z;
        Fix64 xy = axis.X * axis.Y;
        Fix64 xz = axis.X * axis.Z;
        Fix64 yz = axis.Y * axis.Z;

        Fix64 sinAngle = Fix64.Sin(angle);
        Fix64 oneMinusCosAngle = F64.C1 - Fix64.Cos(angle);

        result.M11 = F64.C1 + oneMinusCosAngle * (xx - F64.C1);
        result.M12 = -axis.Z * sinAngle + oneMinusCosAngle * xy;
        result.M13 = axis.Y * sinAngle + oneMinusCosAngle * xz;
        result.M14 = F64.C0;

        result.M21 = axis.Z * sinAngle + oneMinusCosAngle * xy;
        result.M22 = F64.C1 + oneMinusCosAngle * (yy - F64.C1);
        result.M23 = -axis.X * sinAngle + oneMinusCosAngle * yz;
        result.M24 = F64.C0;

        result.M31 = -axis.Y * sinAngle + oneMinusCosAngle * xz;
        result.M32 = axis.X * sinAngle + oneMinusCosAngle * yz;
        result.M33 = F64.C1 + oneMinusCosAngle * (zz - F64.C1);
        result.M34 = F64.C0;

        result.M41 = F64.C0;
        result.M42 = F64.C0;
        result.M43 = F64.C0;
        result.M44 = F64.C1;
    }

    /// <summary>
    /// Creates a rotation matrix from a quaternion.
    /// </summary>
    /// <param name="quaternion">FixQuaternion to convert.</param>
    /// <param name="result">Rotation matrix created from the quaternion.</param>
    public static void CreateFromQuaternion(in FixQuaternion quaternion, out FixMatrix result)
    {
        Fix64 qX2 = quaternion.X + quaternion.X;
        Fix64 qY2 = quaternion.Y + quaternion.Y;
        Fix64 qZ2 = quaternion.Z + quaternion.Z;
        Fix64 XX = qX2 * quaternion.X;
        Fix64 YY = qY2 * quaternion.Y;
        Fix64 ZZ = qZ2 * quaternion.Z;
        Fix64 XY = qX2 * quaternion.Y;
        Fix64 XZ = qX2 * quaternion.Z;
        Fix64 XW = qX2 * quaternion.W;
        Fix64 YZ = qY2 * quaternion.Z;
        Fix64 YW = qY2 * quaternion.W;
        Fix64 ZW = qZ2 * quaternion.W;

        result.M11 = F64.C1 - YY - ZZ;
        result.M12 = XY - ZW;
        result.M13 = XZ + YW;
        result.M14 = F64.C0;

        result.M21 = XY + ZW;
        result.M22 = F64.C1 - XX - ZZ;
        result.M23 = YZ - XW;
        result.M24 = F64.C0;

        result.M31 = XZ - YW;
        result.M32 = YZ + XW;
        result.M33 = F64.C1 - XX - YY;
        result.M34 = F64.C0;

        result.M41 = F64.C0;
        result.M42 = F64.C0;
        result.M43 = F64.C0;
        result.M44 = F64.C1;
    }

    /// <summary>
    /// Creates a rotation matrix from a quaternion.
    /// </summary>
    /// <param name="quaternion">FixQuaternion to convert.</param>
    /// <returns>Rotation matrix created from the quaternion.</returns>
    public static FixMatrix CreateFromQuaternion(in FixQuaternion quaternion)
    {
        FixMatrix toReturn;
        CreateFromQuaternion(in quaternion, out toReturn);
        return toReturn;
    }

    /// <summary>
    /// Multiplies two matrices together.
    /// </summary>
    /// <param name="a">First matrix to multiply.</param>
    /// <param name="b">Second matrix to multiply.</param>
    /// <param name="result">Combined transformation.</param>
    public static void Multiply(in FixMatrix a, in FixMatrix b, out FixMatrix result)
    {
        Fix64 resultM11 = a.M11 * b.M11 + a.M21 * b.M12 + a.M31 * b.M13 + a.M41 * b.M14;
        Fix64 resultM12 = a.M11 * b.M21 + a.M21 * b.M22 + a.M31 * b.M23 + a.M41 * b.M24;
        Fix64 resultM13 = a.M11 * b.M31 + a.M21 * b.M32 + a.M31 * b.M33 + a.M41 * b.M34;
        Fix64 resultM14 = a.M11 * b.M41 + a.M21 * b.M42 + a.M31 * b.M43 + a.M41 * b.M44;

        Fix64 resultM21 = a.M12 * b.M11 + a.M22 * b.M12 + a.M32 * b.M13 + a.M42 * b.M14;
        Fix64 resultM22 = a.M12 * b.M21 + a.M22 * b.M22 + a.M32 * b.M23 + a.M42 * b.M24;
        Fix64 resultM23 = a.M12 * b.M31 + a.M22 * b.M32 + a.M32 * b.M33 + a.M42 * b.M34;
        Fix64 resultM24 = a.M12 * b.M41 + a.M22 * b.M42 + a.M32 * b.M43 + a.M42 * b.M44;

        Fix64 resultM31 = a.M13 * b.M11 + a.M23 * b.M12 + a.M33 * b.M13 + a.M43 * b.M14;
        Fix64 resultM32 = a.M13 * b.M21 + a.M23 * b.M22 + a.M33 * b.M23 + a.M43 * b.M24;
        Fix64 resultM33 = a.M13 * b.M31 + a.M23 * b.M32 + a.M33 * b.M33 + a.M43 * b.M34;
        Fix64 resultM34 = a.M13 * b.M41 + a.M23 * b.M42 + a.M33 * b.M43 + a.M43 * b.M44;

        Fix64 resultM41 = a.M14 * b.M11 + a.M24 * b.M12 + a.M34 * b.M13 + a.M44 * b.M14;
        Fix64 resultM42 = a.M14 * b.M21 + a.M24 * b.M22 + a.M34 * b.M23 + a.M44 * b.M24;
        Fix64 resultM43 = a.M14 * b.M31 + a.M24 * b.M32 + a.M34 * b.M33 + a.M44 * b.M34;
        Fix64 resultM44 = a.M14 * b.M41 + a.M24 * b.M42 + a.M34 * b.M43 + a.M44 * b.M44;

        result.M11 = resultM11;
        result.M21 = resultM12;
        result.M31 = resultM13;
        result.M41 = resultM14;

        result.M12 = resultM21;
        result.M22 = resultM22;
        result.M32 = resultM23;
        result.M42 = resultM24;

        result.M13 = resultM31;
        result.M23 = resultM32;
        result.M33 = resultM33;
        result.M43 = resultM34;

        result.M14 = resultM41;
        result.M24 = resultM42;
        result.M34 = resultM43;
        result.M44 = resultM44;
    }


    /// <summary>
    /// Multiplies two matrices together.
    /// </summary>
    /// <param name="a">First matrix to multiply.</param>
    /// <param name="b">Second matrix to multiply.</param>
    /// <returns>Combined transformation.</returns>
    public static FixMatrix Multiply(in FixMatrix a, in FixMatrix b)
    {
        FixMatrix result;
        Multiply(in a, in b, out result);
        return result;
    }


    /// <summary>
    /// Scales all components of the matrix.
    /// </summary>
    /// <param name="matrix">FixMatrix to scale.</param>
    /// <param name="scale">Amount to scale.</param>
    /// <param name="result">Scaled matrix.</param>
    public static void Multiply(in FixMatrix matrix, in Fix64 scale, out FixMatrix result)
    {
        result.M11 = matrix.M11 * scale;
        result.M21 = matrix.M21 * scale;
        result.M31 = matrix.M31 * scale;
        result.M41 = matrix.M41 * scale;

        result.M12 = matrix.M12 * scale;
        result.M22 = matrix.M22 * scale;
        result.M32 = matrix.M32 * scale;
        result.M42 = matrix.M42 * scale;

        result.M13 = matrix.M13 * scale;
        result.M23 = matrix.M23 * scale;
        result.M33 = matrix.M33 * scale;
        result.M43 = matrix.M43 * scale;

        result.M14 = matrix.M14 * scale;
        result.M24 = matrix.M24 * scale;
        result.M34 = matrix.M34 * scale;
        result.M44 = matrix.M44 * scale;
    }

    /// <summary>
    /// Multiplies two matrices together.
    /// </summary>
    /// <param name="a">First matrix to multiply.</param>
    /// <param name="b">Second matrix to multiply.</param>
    /// <returns>Combined transformation.</returns>
    public static FixMatrix operator *(in FixMatrix a, in FixMatrix b)
    {
        FixMatrix toReturn;
        Multiply(in a, in b, out toReturn);
        return toReturn;
    }

    /// <summary>
    /// Scales all components of the matrix by the given value.
    /// </summary>
    /// <param name="m">First matrix to multiply.</param>
    /// <param name="f">Scaling value to apply to all components of the matrix.</param>
    /// <returns>Product of the multiplication.</returns>
    public static FixMatrix operator *(in FixMatrix m, in Fix64 f)
    {
        FixMatrix result;
        Multiply(in m, f, out result);
        return result;
    }

    /// <summary>
    /// Scales all components of the matrix by the given value.
    /// </summary>
    /// <param name="m">First matrix to multiply.</param>
    /// <param name="f">Scaling value to apply to all components of the matrix.</param>
    /// <returns>Product of the multiplication.</returns>
    public static FixMatrix operator *(in Fix64 f, in FixMatrix m)
    {
        FixMatrix result;
        Multiply(in m, f, out result);
        return result;
    }

    /// <summary>
    /// Transforms a vector using a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to apply to the vector.</param>
    /// <param name="result">Transformed vector.</param>
    public static void Transform(in FixVector4 v, in FixMatrix matrix, out FixVector4 result)
    {
        Fix64 vX = v.X;
        Fix64 vY = v.Y;
        Fix64 vZ = v.Z;
        Fix64 vW = v.W;
        result.X = vX * matrix.M11 + vY * matrix.M12 + vZ * matrix.M13 + vW * matrix.M14;
        result.Y = vX * matrix.M21 + vY * matrix.M22 + vZ * matrix.M23 + vW * matrix.M24;
        result.Z = vX * matrix.M31 + vY * matrix.M32 + vZ * matrix.M33 + vW * matrix.M34;
        result.W = vX * matrix.M41 + vY * matrix.M42 + vZ * matrix.M43 + vW * matrix.M44;
    }

    /// <summary>
    /// Transforms a vector using a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to apply to the vector.</param>
    /// <returns>Transformed vector.</returns>
    public static FixVector4 Transform(in FixVector4 v, in FixMatrix matrix)
    {
        FixVector4 toReturn;
        Transform(in v, in matrix, out toReturn);
        return toReturn;
    }

    /// <summary>
    /// Transforms a vector using the transpose of a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
    /// <param name="result">Transformed vector.</param>
    public static void TransformTranspose(in FixVector4 v, in FixMatrix matrix, out FixVector4 result)
    {
        Fix64 vX = v.X;
        Fix64 vY = v.Y;
        Fix64 vZ = v.Z;
        Fix64 vW = v.W;
        result.X = vX * matrix.M11 + vY * matrix.M21 + vZ * matrix.M31 + vW * matrix.M41;
        result.Y = vX * matrix.M12 + vY * matrix.M22 + vZ * matrix.M32 + vW * matrix.M42;
        result.Z = vX * matrix.M13 + vY * matrix.M23 + vZ * matrix.M33 + vW * matrix.M43;
        result.W = vX * matrix.M14 + vY * matrix.M24 + vZ * matrix.M34 + vW * matrix.M44;
    }

    /// <summary>
    /// Transforms a vector using the transpose of a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
    /// <returns>Transformed vector.</returns>
    public static FixVector4 TransformTranspose(in FixVector4 v, in FixMatrix matrix)
    {
        FixVector4 toReturn;
        TransformTranspose(in v, in matrix, out toReturn);
        return toReturn;
    }

    /// <summary>
    /// Transforms a vector using a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to apply to the vector.</param>
    /// <param name="result">Transformed vector.</param>
    public static void Transform(in FixVector3 v, in FixMatrix matrix, out FixVector4 result)
    {
        result.X = v.X * matrix.M11 + v.Y * matrix.M12 + v.Z * matrix.M13 + matrix.M14;
        result.Y = v.X * matrix.M21 + v.Y * matrix.M22 + v.Z * matrix.M23 + matrix.M24;
        result.Z = v.X * matrix.M31 + v.Y * matrix.M32 + v.Z * matrix.M33 + matrix.M34;
        result.W = v.X * matrix.M41 + v.Y * matrix.M42 + v.Z * matrix.M43 + matrix.M44;
    }

    /// <summary>
    /// Transforms a vector using a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to apply to the vector.</param>
    /// <returns>Transformed vector.</returns>
    public static FixVector4 Transform(in FixVector3 v, in FixMatrix matrix)
    {
        FixVector4 toReturn;
        Transform(in v, in matrix, out toReturn);
        return toReturn;
    }

    /// <summary>
    /// Transforms a postion using a matrix.
    /// </summary>
    /// <param name="v">Position to transform.</param>
    /// <param name="matrix">Transform to apply to the vector.</param>
    /// <returns>Transformed vector.</returns>
    public static FixVector3 TransformPoint(in FixVector3 v, in FixMatrix matrix)
    {
        FixVector4 result;
        Transform(in v, in matrix, out result);
        return new FixVector3(result.X / result.W, result.Y / result.W, result.Z / result.W);
    }

    /// <summary>
    /// Transforms a vector using the transpose of a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
    /// <param name="result">Transformed vector.</param>
    public static void TransformTranspose(in FixVector3 v, in FixMatrix matrix, out FixVector4 result)
    {
        result.X = v.X * matrix.M11 + v.Y * matrix.M21 + v.Z * matrix.M31 + matrix.M41;
        result.Y = v.X * matrix.M12 + v.Y * matrix.M22 + v.Z * matrix.M32 + matrix.M42;
        result.Z = v.X * matrix.M13 + v.Y * matrix.M23 + v.Z * matrix.M33 + matrix.M43;
        result.W = v.X * matrix.M14 + v.Y * matrix.M24 + v.Z * matrix.M34 + matrix.M44;
    }

    /// <summary>
    /// Transforms a vector using the transpose of a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
    /// <returns>Transformed vector.</returns>
    public static FixVector4 TransformTranspose(in FixVector3 v, in FixMatrix matrix)
    {
        FixVector4 toReturn;
        TransformTranspose(in v, in matrix, out toReturn);
        return toReturn;
    }

    /// <summary>
    /// Transforms a vector using a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to apply to the vector.</param>
    /// <param name="result">Transformed vector.</param>
    public static void Transform(in FixVector3 v, in FixMatrix matrix, out FixVector3 result)
    {
        Fix64 vX = v.X;
        Fix64 vY = v.Y;
        Fix64 vZ = v.Z;
        result.X = vX * matrix.M11 + vY * matrix.M12 + vZ * matrix.M13 + matrix.M14;
        result.Y = vX * matrix.M21 + vY * matrix.M22 + vZ * matrix.M23 + matrix.M24;
        result.Z = vX * matrix.M31 + vY * matrix.M32 + vZ * matrix.M33 + matrix.M34;
    }

    /// <summary>
    /// Transforms a vector using the transpose of a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
    /// <param name="result">Transformed vector.</param>
    public static void TransformTranspose(in FixVector3 v, in FixMatrix matrix, out FixVector3 result)
    {
        Fix64 vX = v.X;
        Fix64 vY = v.Y;
        Fix64 vZ = v.Z;
        result.X = vX * matrix.M11 + vY * matrix.M21 + vZ * matrix.M31 + matrix.M41;
        result.Y = vX * matrix.M12 + vY * matrix.M22 + vZ * matrix.M32 + matrix.M42;
        result.Z = vX * matrix.M13 + vY * matrix.M23 + vZ * matrix.M33 + matrix.M43;
    }

    /// <summary>
    /// Transforms a vector using a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to apply to the vector.</param>
    /// <param name="result">Transformed vector.</param>
    public static void TransformNormal(in FixVector3 v, in FixMatrix matrix, out FixVector3 result)
    {
        Fix64 vX = v.X;
        Fix64 vY = v.Y;
        Fix64 vZ = v.Z;
        result.X = vX * matrix.M11 + vY * matrix.M12 + vZ * matrix.M13;
        result.Y = vX * matrix.M21 + vY * matrix.M22 + vZ * matrix.M23;
        result.Z = vX * matrix.M31 + vY * matrix.M32 + vZ * matrix.M33;
    }

    /// <summary>
    /// Transforms a vector using a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to apply to the vector.</param>
    /// <returns>Transformed vector.</returns>
    public static FixVector3 TransformNormal(in FixVector3 v, in FixMatrix matrix)
    {
        FixVector3 toReturn;
        TransformNormal(in v, in matrix, out toReturn);
        return toReturn;
    }

    /// <summary>
    /// Transforms a vector using the transpose of a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
    /// <param name="result">Transformed vector.</param>
    public static void TransformNormalTranspose(in FixVector3 v, in FixMatrix matrix, out FixVector3 result)
    {
        Fix64 vX = v.X;
        Fix64 vY = v.Y;
        Fix64 vZ = v.Z;
        result.X = vX * matrix.M11 + vY * matrix.M21 + vZ * matrix.M31;
        result.Y = vX * matrix.M12 + vY * matrix.M22 + vZ * matrix.M32;
        result.Z = vX * matrix.M13 + vY * matrix.M23 + vZ * matrix.M33;
    }

    /// <summary>
    /// Transforms a vector using the transpose of a matrix.
    /// </summary>
    /// <param name="v">Vector to transform.</param>
    /// <param name="matrix">Transform to tranpose and apply to the vector.</param>
    /// <returns>Transformed vector.</returns>
    public static FixVector3 TransformNormalTranspose(in FixVector3 v, in FixMatrix matrix)
    {
        FixVector3 toReturn;
        TransformNormalTranspose(in v, in matrix, out toReturn);
        return toReturn;
    }


    /// <summary>
    /// Transposes the matrix.
    /// </summary>
    /// <param name="m">FixMatrix to transpose.</param>
    /// <param name="transposed">FixMatrix to transpose.</param>
    public static void Transpose(in FixMatrix m, out FixMatrix transposed)
    {
        Fix64 intermediate = m.M21;
        transposed.M21 = m.M12;
        transposed.M12 = intermediate;

        intermediate = m.M31;
        transposed.M31 = m.M13;
        transposed.M13 = intermediate;

        intermediate = m.M41;
        transposed.M41 = m.M14;
        transposed.M14 = intermediate;

        intermediate = m.M32;
        transposed.M32 = m.M23;
        transposed.M23 = intermediate;

        intermediate = m.M42;
        transposed.M42 = m.M24;
        transposed.M24 = intermediate;

        intermediate = m.M43;
        transposed.M43 = m.M34;
        transposed.M34 = intermediate;

        transposed.M11 = m.M11;
        transposed.M22 = m.M22;
        transposed.M33 = m.M33;
        transposed.M44 = m.M44;
    }

    /// <summary>
    /// Inverts the matrix.
    /// </summary>
    /// <param name="m">FixMatrix to invert.</param>
    /// <param name="inverted">Inverted version of the matrix.</param>
    public static void Invert(in FixMatrix m, out FixMatrix inverted)
    {
        FixMatrix4x8.Invert(in m, out inverted);
    }

    /// <summary>
    /// Inverts the matrix.
    /// </summary>
    /// <param name="m">FixMatrix to invert.</param>
    /// <returns>Inverted version of the matrix.</returns>
    public static FixMatrix Invert(in FixMatrix m)
    {
        FixMatrix inverted;
        Invert(in m, out inverted);
        return inverted;
    }

    /// <summary>
    /// Inverts the matrix using a process that only works for rigid transforms.
    /// </summary>
    /// <param name="m">FixMatrix to invert.</param>
    /// <param name="inverted">Inverted version of the matrix.</param>
    public static void InvertRigid(in FixMatrix m, out FixMatrix inverted)
    {
        //Invert (transpose) the upper left 3x3 rotation.
        Fix64 intermediate = m.M21;
        inverted.M21 = m.M12;
        inverted.M12 = intermediate;

        intermediate = m.M31;
        inverted.M31 = m.M13;
        inverted.M13 = intermediate;

        intermediate = m.M32;
        inverted.M32 = m.M23;
        inverted.M23 = intermediate;

        inverted.M11 = m.M11;
        inverted.M22 = m.M22;
        inverted.M33 = m.M33;

        //Translation component
        var vX = m.M14;
        var vY = m.M24;
        var vZ = m.M34;
        inverted.M14 = -(vX * inverted.M11 + vY * inverted.M12 + vZ * inverted.M13);
        inverted.M24 = -(vX * inverted.M21 + vY * inverted.M22 + vZ * inverted.M23);
        inverted.M34 = -(vX * inverted.M31 + vY * inverted.M32 + vZ * inverted.M33);

        //Last chunk.
        inverted.M41 = F64.C0;
        inverted.M42 = F64.C0;
        inverted.M43 = F64.C0;
        inverted.M44 = F64.C1;
    }

    /// <summary>
    /// Inverts the matrix using a process that only works for rigid transforms.
    /// </summary>
    /// <param name="m">FixMatrix to invert.</param>
    /// <returns>Inverted version of the matrix.</returns>
    public static FixMatrix InvertRigid(FixMatrix m)
    {
        FixMatrix inverse;
        InvertRigid(in m, out inverse);
        return inverse;
    }

    /// <summary>
    /// Gets the 4x4 identity matrix.
    /// </summary>
    public static FixMatrix Identity
    {
        get
        {
            FixMatrix toReturn;
            toReturn.M11 = F64.C1;
            toReturn.M21 = F64.C0;
            toReturn.M31 = F64.C0;
            toReturn.M41 = F64.C0;

            toReturn.M12 = F64.C0;
            toReturn.M22 = F64.C1;
            toReturn.M32 = F64.C0;
            toReturn.M42 = F64.C0;

            toReturn.M13 = F64.C0;
            toReturn.M23 = F64.C0;
            toReturn.M33 = F64.C1;
            toReturn.M43 = F64.C0;

            toReturn.M14 = F64.C0;
            toReturn.M24 = F64.C0;
            toReturn.M34 = F64.C0;
            toReturn.M44 = F64.C1;
            return toReturn;
        }
    }

    /// <summary>
    /// Creates a right handed orthographic projection.
    /// </summary>
    /// <param name="left">Leftmost coordinate of the projected area.</param>
    /// <param name="right">Rightmost coordinate of the projected area.</param>
    /// <param name="bottom">Bottom coordinate of the projected area.</param>
    /// <param name="top">Top coordinate of the projected area.</param>
    /// <param name="zNear">Near plane of the projection.</param>
    /// <param name="zFar">Far plane of the projection.</param>
    /// <param name="projection">The resulting orthographic projection matrix.</param>
    public static void CreateOrthographicRH(in Fix64 left, in Fix64 right, in Fix64 bottom, in Fix64 top, in Fix64 zNear, in Fix64 zFar, out FixMatrix projection)
    {
        Fix64 width = right - left;
        Fix64 height = top - bottom;
        Fix64 depth = zFar - zNear;
        projection.M11 = F64.C2 / width;
        projection.M21 = F64.C0;
        projection.M31 = F64.C0;
        projection.M41 = F64.C0;

        projection.M12 = F64.C0;
        projection.M22 = F64.C2 / height;
        projection.M32 = F64.C0;
        projection.M42 = F64.C0;

        projection.M13 = F64.C0;
        projection.M23 = F64.C0;
        projection.M33 = new Fix64(-1) / depth;
        projection.M43 = F64.C0;

        projection.M14 = (left + right) / -width;
        projection.M24 = (top + bottom) / -height;
        projection.M34 = zNear / -depth;
        projection.M44 = F64.C1;

    }

    /// <summary>
    /// Creates a right-handed perspective matrix.
    /// </summary>
    /// <param name="fieldOfView">Field of view of the perspective in radians.</param>
    /// <param name="aspectRatio">Width of the viewport over the height of the viewport.</param>
    /// <param name="nearClip">Near clip plane of the perspective.</param>
    /// <param name="farClip">Far clip plane of the perspective.</param>
    /// <param name="perspective">Resulting perspective matrix.</param>
    public static void CreatePerspectiveFieldOfViewRH(in Fix64 fieldOfView, in Fix64 aspectRatio, in Fix64 nearClip, in Fix64 farClip, out FixMatrix perspective)
    {
        Fix64 h = F64.C1 / Fix64.Tan(fieldOfView / F64.C2);
        Fix64 w = h / aspectRatio;
        perspective.M11 = w;
        perspective.M21 = F64.C0;
        perspective.M31 = F64.C0;
        perspective.M41 = F64.C0;

        perspective.M12 = F64.C0;
        perspective.M22 = h;
        perspective.M32 = F64.C0;
        perspective.M42 = F64.C0;

        perspective.M13 = F64.C0;
        perspective.M23 = F64.C0;
        perspective.M33 = farClip / (nearClip - farClip);
        perspective.M43 = -1;

        perspective.M14 = F64.C0;
        perspective.M24 = F64.C0;
        perspective.M44 = F64.C0;
        perspective.M34 = nearClip * perspective.M33;

    }

    /// <summary>
    /// Creates a right-handed perspective matrix.
    /// </summary>
    /// <param name="fieldOfView">Field of view of the perspective in radians.</param>
    /// <param name="aspectRatio">Width of the viewport over the height of the viewport.</param>
    /// <param name="nearClip">Near clip plane of the perspective.</param>
    /// <param name="farClip">Far clip plane of the perspective.</param>
    /// <returns>Resulting perspective matrix.</returns>
    public static FixMatrix CreatePerspectiveFieldOfViewRH(in Fix64 fieldOfView, in Fix64 aspectRatio, in Fix64 nearClip, in Fix64 farClip)
    {
        FixMatrix perspective;
        CreatePerspectiveFieldOfViewRH(fieldOfView, aspectRatio, nearClip, farClip, out perspective);
        return perspective;
    }

    /// <summary>
    /// Creates a view matrix pointing from a position to a target with the given up vector.
    /// </summary>
    /// <param name="position">Position of the camera.</param>
    /// <param name="target">Target of the camera.</param>
    /// <param name="upVector">Up vector of the camera.</param>
    /// <param name="viewMatrix">Look at matrix.</param>
    public static void CreateLookAtRH(in FixVector3 position, in FixVector3 target, in FixVector3 upVector, out FixMatrix viewMatrix)
    {
        FixVector3 forward;
        FixVector3.Subtract(in target, in position, out forward);
        CreateViewRH(in position, in forward, in upVector, out viewMatrix);
    }

    /// <summary>
    /// Creates a view matrix pointing from a position to a target with the given up vector.
    /// </summary>
    /// <param name="position">Position of the camera.</param>
    /// <param name="target">Target of the camera.</param>
    /// <param name="upVector">Up vector of the camera.</param>
    /// <returns>Look at matrix.</returns>
    public static FixMatrix CreateLookAtRH(in FixVector3 position, in FixVector3 target, in FixVector3 upVector)
    {
        FixMatrix lookAt;
        FixVector3 forward;
        FixVector3.Subtract(in target, in position, out forward);
        CreateViewRH(in position, in forward, in upVector, out lookAt);
        return lookAt;
    }


    /// <summary>
    /// Creates a view matrix pointing in a direction with a given up vector.
    /// </summary>
    /// <param name="position">Position of the camera.</param>
    /// <param name="forward">Forward direction of the camera.</param>
    /// <param name="upVector">Up vector of the camera.</param>
    /// <param name="viewMatrix">Look at matrix.</param>
    public static void CreateViewRH(in FixVector3 position, in FixVector3 forward, in FixVector3 upVector, out FixMatrix viewMatrix)
    {
        FixVector3 z;
        Fix64 length = forward.Length();
        FixVector3.Divide(in forward, -length, out z);
        FixVector3 x;
        FixVector3.Cross(in upVector, in z, out x);
        x.Normalize();
        FixVector3 y;
        FixVector3.Cross(in z, in x, out y);

        viewMatrix.M11 = x.X;
        viewMatrix.M21 = y.X;
        viewMatrix.M31 = z.X;
        viewMatrix.M41 = F64.C0;
        viewMatrix.M12 = x.Y;
        viewMatrix.M22 = y.Y;
        viewMatrix.M32 = z.Y;
        viewMatrix.M42 = F64.C0;
        viewMatrix.M13 = x.Z;
        viewMatrix.M23 = y.Z;
        viewMatrix.M33 = z.Z;
        viewMatrix.M43 = F64.C0;
        FixVector3.Dot(in x, in position, out viewMatrix.M14);
        FixVector3.Dot(in y, in position, out viewMatrix.M24);
        FixVector3.Dot(in z, in position, out viewMatrix.M34);
        viewMatrix.M14 = -viewMatrix.M14;
        viewMatrix.M24 = -viewMatrix.M24;
        viewMatrix.M34 = -viewMatrix.M34;
        viewMatrix.M44 = F64.C1;

    }

    /// <summary>
    /// Creates a view matrix pointing looking in a direction with a given up vector.
    /// </summary>
    /// <param name="position">Position of the camera.</param>
    /// <param name="forward">Forward direction of the camera.</param>
    /// <param name="upVector">Up vector of the camera.</param>
    /// <returns>Look at matrix.</returns>
    public static FixMatrix CreateViewRH(in FixVector3 position, in FixVector3 forward, in FixVector3 upVector)
    {
        FixMatrix lookat;
        CreateViewRH(in position, in forward, in upVector, out lookat);
        return lookat;
    }

    /// <summary>
    /// Creates a transform matrix with the given positon, rotation and scale
    /// </summary>
    public static FixMatrix CreateTRS(in FixVector3 position, in FixQuaternion rotation, in FixVector3 scale)
    {
        FixMatrix result;
        CreateTRS(position, rotation, scale, out result);
        return result;
    }
    /// <summary>
    /// Creates a transform matrix with the given positon, rotation and scale
    /// </summary>
    public static void CreateTRS(in FixVector3 position, in FixQuaternion rotation, in FixVector3 scale, out FixMatrix worldMatrix)
    {
        a = Matrix4x4.TRS(position.ToUnityVec(), rotation.ToUnityQuat(), scale.ToUnityVec());


        FixMatrix mat;


        // Scale
        CreateScale(scale, out worldMatrix);

        // Rotation
        CreateFromQuaternion(rotation, out mat);
        worldMatrix *= mat;

        // Translation
        CreateTranslation(in position, out mat);
        worldMatrix *= mat;








        b = worldMatrix;
    }


    /// <summary>
    /// Creates a world matrix pointing from a position to a target with the given up vector.
    /// </summary>
    /// <param name="position">Position of the transform.</param>
    /// <param name="forward">Forward direction of the transformation.</param>
    /// <param name="upVector">Up vector which is crossed against the forward vector to compute the transform's basis.</param>
    /// <param name="worldMatrix">World matrix.</param>
    public static void CreateWorldRH(in FixVector3 position, in FixVector3 forward, in FixVector3 upVector, out FixMatrix worldMatrix)
    {
        FixVector3 z;
        Fix64 length = forward.Length();
        FixVector3.Divide(in forward, -length, out z);
        FixVector3 x;
        FixVector3.Cross(in upVector, in z, out x);
        x.Normalize();
        FixVector3 y;
        FixVector3.Cross(in z, in x, out y);

        worldMatrix.M11 = x.X;
        worldMatrix.M21 = x.Y;
        worldMatrix.M31 = x.Z;
        worldMatrix.M41 = F64.C0;
        worldMatrix.M12 = y.X;
        worldMatrix.M22 = y.Y;
        worldMatrix.M32 = y.Z;
        worldMatrix.M42 = F64.C0;
        worldMatrix.M13 = z.X;
        worldMatrix.M23 = z.Y;
        worldMatrix.M33 = z.Z;
        worldMatrix.M43 = F64.C0;

        worldMatrix.M14 = position.X;
        worldMatrix.M24 = position.Y;
        worldMatrix.M34 = position.Z;
        worldMatrix.M44 = F64.C1;

    }


    /// <summary>
    /// Creates a world matrix pointing from a position to a target with the given up vector.
    /// </summary>
    /// <param name="position">Position of the transform.</param>
    /// <param name="forward">Forward direction of the transformation.</param>
    /// <param name="upVector">Up vector which is crossed against the forward vector to compute the transform's basis.</param>
    /// <returns>World matrix.</returns>
    public static FixMatrix CreateWorldRH(in FixVector3 position, in FixVector3 forward, in FixVector3 upVector)
    {
        FixMatrix lookat;
        CreateWorldRH(in position, in forward, in upVector, out lookat);
        return lookat;
    }



    /// <summary>
    /// Creates a matrix representing a translation.
    /// </summary>
    /// <param name="translation">Translation to be represented by the matrix.</param>
    /// <param name="translationMatrix">FixMatrix representing the given translation.</param>
    public static void CreateTranslation(in FixVector3 translation, out FixMatrix translationMatrix)
    {
        translationMatrix = new FixMatrix
        {
            M11 = F64.C1,
            M22 = F64.C1,
            M33 = F64.C1,
            M44 = F64.C1,
            M14 = translation.X,
            M24 = translation.Y,
            M34 = translation.Z
        };
    }

    /// <summary>
    /// Creates a matrix representing a translation.
    /// </summary>
    /// <param name="translation">Translation to be represented by the matrix.</param>
    /// <returns>FixMatrix representing the given translation.</returns>
    public static FixMatrix CreateTranslation(in FixVector3 translation)
    {
        FixMatrix translationMatrix;
        CreateTranslation(in translation, out translationMatrix);
        return translationMatrix;
    }

    /// <summary>
    /// Creates a matrix representing the given axis aligned scale.
    /// </summary>
    /// <param name="scale">Scale to be represented by the matrix.</param>
    /// <param name="scaleMatrix">FixMatrix representing the given scale.</param>
    public static void CreateScale(in FixVector3 scale, out FixMatrix scaleMatrix)
    {
        scaleMatrix = new FixMatrix
        {
            M11 = scale.X,
            M22 = scale.Y,
            M33 = scale.Z,
            M44 = F64.C1
        };
    }

    /// <summary>
    /// Creates a matrix representing the given axis aligned scale.
    /// </summary>
    /// <param name="scale">Scale to be represented by the matrix.</param>
    /// <returns>FixMatrix representing the given scale.</returns>
    public static FixMatrix CreateScale(in FixVector3 scale)
    {
        FixMatrix scaleMatrix;
        CreateScale(in scale, out scaleMatrix);
        return scaleMatrix;
    }

    /// <summary>
    /// Creates a matrix representing the given axis aligned scale.
    /// </summary>
    /// <param name="x">Scale along the x axis.</param>
    /// <param name="y">Scale along the y axis.</param>
    /// <param name="z">Scale along the z axis.</param>
    /// <param name="scaleMatrix">FixMatrix representing the given scale.</param>
    public static void CreateScale(in Fix64 x, in Fix64 y, in Fix64 z, out FixMatrix scaleMatrix)
    {
        scaleMatrix = new FixMatrix
        {
            M11 = x,
            M22 = y,
            M33 = z,
            M44 = F64.C1
        };
    }

    /// <summary>
    /// Creates a matrix representing the given axis aligned scale.
    /// </summary>
    /// <param name="x">Scale along the x axis.</param>
    /// <param name="y">Scale along the y axis.</param>
    /// <param name="z">Scale along the z axis.</param>
    /// <returns>FixMatrix representing the given scale.</returns>
    public static FixMatrix CreateScale(in Fix64 x, in Fix64 y, in Fix64 z)
    {
        FixMatrix scaleMatrix;
        CreateScale(x, y, z, out scaleMatrix);
        return scaleMatrix;
    }

    /// <summary>
    /// Creates a string representation of the matrix.
    /// </summary>
    /// <returns>A string representation of the matrix.</returns>
    public override string ToString()
    {
        return "{" + M11 + ", " + M21 + ", " + M31 + ", " + M41 + "} " +
               "{" + M12 + ", " + M22 + ", " + M32 + ", " + M42 + "} " +
               "{" + M13 + ", " + M23 + ", " + M33 + ", " + M43 + "} " +
               "{" + M14 + ", " + M24 + ", " + M34 + ", " + M44 + "}";
    }
}
