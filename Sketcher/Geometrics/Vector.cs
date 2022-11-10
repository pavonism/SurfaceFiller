
using System.Runtime.CompilerServices;

namespace SketcherControl.Geometrics
{
    public struct Vector
    {
        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector(Vector source) : this(source.X, source.Y, source.Z) { }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float Length => (float)Math.Sqrt(X * X + Y * Y + Z * Z);


        #region Operators
        public static float operator&(Vector v1, Vector v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Vector operator *(float number, Vector vector)
        {
            return new Vector(vector.X * number, vector.Y * number, vector.Z * number);
        }

        public static Vector operator *(Vector vector, float number)
        {
            return new Vector(vector.X * number, vector.Y * number, vector.Z * number);
        }

        public static Vector operator -(Vector vector, float number)
        {
            return new Vector(vector.X - number, vector.Y * number, vector.Z - number);
        }

        public static Vector operator *(Vector v1, Vector v2)
        {
            return new Vector(v1.X*v2.X, v1.Y*v2.Y, v1.Z*v2.Z);
        }

        public static Vector operator ^(Vector vector, int power)
        {
            var result = new Vector(1,1,1);

            for (int i = 0; i < power; i++)
            {
                result *= vector;
            }

            return result;
        }

        public static Vector operator /(Vector vector, float number)
        {
            return new Vector(vector.X / number, vector.Y / number, vector.Z / number);
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector.X, -vector.Y, -vector.Z);
        }

        public static bool operator ==(Vector v1, Vector v2)
        {
            return !(v1 != v2);
        }

        public static bool operator !=(Vector v1, Vector v2)
        {
            return v1.X != v2.X || v1.Y != v2.Y || v1.Z == v2.Z;
        }

        public static Vector operator !(Vector v1)
        {
            if (v1.Length == 1f)
                return v1;

            return v1 / v1.Length;
        }

        public static Vector operator|(Vector v1, Vector v2)
        {
            return new Vector(v1.Y * v2.Z - v1.Z *v2.Y, v1.Z*v2.X - v1.X*v2.Z, v1.X*v2.Y - v1.Y*v2.X);
        }

        public override bool Equals(object? obj)
        {
            return GetHashCode() == obj?.GetHashCode();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    public static class VectorExtensions
    {
        public static Vector ToVector(this Color color)
        {
            return new Vector((float)color.R / 255, (float)color.G / 255, (float)color.B / 255);
        }

        public static Vector ToNormalMapVector(this Color color)
        {
            var normalMapVector = color.ToVector();
            normalMapVector.X = normalMapVector.X * 2 - 1;
            normalMapVector.Y = normalMapVector.Y * 2 - 1;
            return normalMapVector;
        }

        public static Color ToColor(this Vector vector)
        {
            return Color.FromArgb((int)Math.Min(Math.Max(0, vector.X) * 255, 255), (int)Math.Min(Math.Max(0, vector.Y) * 255, 255), (int)Math.Min(Math.Max(0, vector.Z) * 255, 255));
        }
    }
}
