﻿//using System;
//using System.Runtime.InteropServices;
//using BEPUphysics.DataStructures;
//using BEPUphysics.Entities;
//using BEPUphysics.MathExtensions;
// 
//using BEPUphysics.CollisionShapes.ConvexShapes;
//using BEPUphysics.CollisionTests;
//using System.Diagnostics;

//namespace BEPUphysics
//{
//    /// <summary>
//    /// Stores basic data used by some collision systems.
//    /// </summary>
//    [StructLayout(LayoutKind.Sequential)]
//    public struct BoxContactData : IEquatable<BoxContactData>
//    {
//        /// <summary>
//        /// Position of the candidate contact.
//        /// </summary>
//        public Vector3 Position;

//        /// <summary>
//        /// Depth of the candidate contact.
//        /// </summary>
//        public float Depth;

//        /// <summary>
//        /// Id of the candidate contact.
//        /// </summary>
//        public int Id;

//        #region IEquatable<BoxContactData> Members

//        /// <summary>
//        /// Returns true if the other data has the same id.
//        /// </summary>
//        /// <param name="other">Data to compare.</param>
//        /// <returns>True if the other data has the same id, false otherwise.</returns>
//        public bool Equals(BoxContactData other)
//        {
//            return Id == other.Id;
//        }

//        #endregion
//    }

//    /// <summary>
//    /// Basic storage structure for contact data.
//    /// Designed for performance critical code and pointer access.
//    /// </summary>
//#if WINDOWS
//    [StructLayout(LayoutKind.Sequential, Pack = 1)]
//#else
//#if XBOX360
//    [StructLayout(LayoutKind.Sequential)]
//#endif
//#endif
//    public struct BoxContactDataCache
//    {
//        public BoxContactData D1;
//        public BoxContactData D2;
//        public BoxContactData D3;
//        public BoxContactData D4;

//        public BoxContactData D5;
//        public BoxContactData D6;
//        public BoxContactData D7;
//        public BoxContactData D8;

//        //internal BoxContactData d9;
//        //internal BoxContactData d10;
//        //internal BoxContactData d11;
//        //internal BoxContactData d12;

//        //internal BoxContactData d13;
//        //internal BoxContactData d14;
//        //internal BoxContactData d15;
//        //internal BoxContactData d16;

//        /// <summary>
//        /// Number of elements in the cache.
//        /// </summary>
//        public byte Count;

//#if ALLOWUNSAFE
//        /// <summary>
//        /// Removes an item at the given index.
//        /// </summary>
//        /// <param name="index">Index to remove.</param>
//        public unsafe void RemoveAt(int index)
//        {
//            BoxContactDataCache copy = this;
//            BoxContactData* pointer = &copy.D1;
//            pointer[index] = pointer[Count - 1];
//            this = copy;
//            Count--;
//        }
//#endif
//    }


//    /// <summary>
//    /// Contains helper methods for testing collisions between boxes.
//    /// </summary>
//    public static class BoxBoxCollider
//    {
//        /// <summary>
//        /// Determines if the two boxes are colliding.
//        /// </summary>
//        /// <param name="a">First box to collide.</param>
//        /// <param name="b">Second box to collide.</param>
//        /// <returns>Whether or not the boxes collide.</returns>
//        public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB)
//        {
//            float aX = a.HalfWidth;
//            float aY = a.HalfHeight;
//            float aZ = a.HalfLength;

//            float bX = b.HalfWidth;
//            float bY = b.HalfHeight;
//            float bZ = b.HalfLength;

//            //Relative rotation from A to B.
//            Matrix3X3 bR;

//            Matrix aO = transformA.OrientationMatrix, bO = transformB.OrientationMatrix;

//            //Relative translation rotated into A's configuration space.
//            Vector3 t;
//            Vector3.Subtract(ref transformB.Position, ref transformA.Position, out t);

//            bR.M11 = aO.M11 * bO.M11 + aO.M12 * bO.M12 + aO.M13 * bO.M13;
//            bR.M12 = aO.M11 * bO.M21 + aO.M12 * bO.M22 + aO.M13 * bO.M23;
//            bR.M13 = aO.M11 * bO.M31 + aO.M12 * bO.M32 + aO.M13 * bO.M33;
//            Matrix3X3 absBR;
//            //Epsilons are added to deal with near-parallel edges.
//            absBR.M11 = Math.Abs(bR.M11) + Toolbox.Epsilon;
//            absBR.M12 = Math.Abs(bR.M12) + Toolbox.Epsilon;
//            absBR.M13 = Math.Abs(bR.M13) + Toolbox.Epsilon;
//            float tX = t.X;
//            t.X = t.X * aO.M11 + t.Y * aO.M12 + t.Z * aO.M13;

//            //Test the axes defines by entity A's rotation matrix.
//            //A.X
//            float rb = bX * absBR.M11 + bY * absBR.M12 + bZ * absBR.M13;
//            if (Math.Abs(t.X) > aX + rb)
//                return false;
//            bR.M21 = aO.M21 * bO.M11 + aO.M22 * bO.M12 + aO.M23 * bO.M13;
//            bR.M22 = aO.M21 * bO.M21 + aO.M22 * bO.M22 + aO.M23 * bO.M23;
//            bR.M23 = aO.M21 * bO.M31 + aO.M22 * bO.M32 + aO.M23 * bO.M33;
//            absBR.M21 = Math.Abs(bR.M21) + Toolbox.Epsilon;
//            absBR.M22 = Math.Abs(bR.M22) + Toolbox.Epsilon;
//            absBR.M23 = Math.Abs(bR.M23) + Toolbox.Epsilon;
//            float tY = t.Y;
//            t.Y = tX * aO.M21 + t.Y * aO.M22 + t.Z * aO.M23;

//            //A.Y
//            rb = bX * absBR.M21 + bY * absBR.M22 + bZ * absBR.M23;
//            if (Math.Abs(t.Y) > aY + rb)
//                return false;

//            bR.M31 = aO.M31 * bO.M11 + aO.M32 * bO.M12 + aO.M33 * bO.M13;
//            bR.M32 = aO.M31 * bO.M21 + aO.M32 * bO.M22 + aO.M33 * bO.M23;
//            bR.M33 = aO.M31 * bO.M31 + aO.M32 * bO.M32 + aO.M33 * bO.M33;
//            absBR.M31 = Math.Abs(bR.M31) + Toolbox.Epsilon;
//            absBR.M32 = Math.Abs(bR.M32) + Toolbox.Epsilon;
//            absBR.M33 = Math.Abs(bR.M33) + Toolbox.Epsilon;
//            t.Z = tX * aO.M31 + tY * aO.M32 + t.Z * aO.M33;

//            //A.Z
//            rb = bX * absBR.M31 + bY * absBR.M32 + bZ * absBR.M33;
//            if (Math.Abs(t.Z) > aZ + rb)
//                return false;

//            //Test the axes defines by entity B's rotation matrix.
//            //B.X
//            float ra = aX * absBR.M11 + aY * absBR.M21 + aZ * absBR.M31;
//            if (Math.Abs(t.X * bR.M11 + t.Y * bR.M21 + t.Z * bR.M31) > ra + bX)
//                return false;

//            //B.Y
//            ra = aX * absBR.M12 + aY * absBR.M22 + aZ * absBR.M32;
//            if (Math.Abs(t.X * bR.M12 + t.Y * bR.M22 + t.Z * bR.M32) > ra + bY)
//                return false;

//            //B.Z
//            ra = aX * absBR.M13 + aY * absBR.M23 + aZ * absBR.M33;
//            if (Math.Abs(t.X * bR.M13 + t.Y * bR.M23 + t.Z * bR.M33) > ra + bZ)
//                return false;

//            //Now for the edge-edge cases.
//            //A.X x B.X
//            ra = aY * absBR.M31 + aZ * absBR.M21;
//            rb = bY * absBR.M13 + bZ * absBR.M12;
//            if (Math.Abs(t.Z * bR.M21 - t.Y * bR.M31) > ra + rb)
//                return false;

//            //A.X x B.Y
//            ra = aY * absBR.M32 + aZ * absBR.M22;
//            rb = bX * absBR.M13 + bZ * absBR.M11;
//            if (Math.Abs(t.Z * bR.M22 - t.Y * bR.M32) > ra + rb)
//                return false;

//            //A.X x B.Z
//            ra = aY * absBR.M33 + aZ * absBR.M23;
//            rb = bX * absBR.M12 + bY * absBR.M11;
//            if (Math.Abs(t.Z * bR.M23 - t.Y * bR.M33) > ra + rb)
//                return false;


//            //A.Y x B.X
//            ra = aX * absBR.M31 + aZ * absBR.M11;
//            rb = bY * absBR.M23 + bZ * absBR.M22;
//            if (Math.Abs(t.X * bR.M31 - t.Z * bR.M11) > ra + rb)
//                return false;

//            //A.Y x B.Y
//            ra = aX * absBR.M32 + aZ * absBR.M12;
//            rb = bX * absBR.M23 + bZ * absBR.M21;
//            if (Math.Abs(t.X * bR.M32 - t.Z * bR.M12) > ra + rb)
//                return false;

//            //A.Y x B.Z
//            ra = aX * absBR.M33 + aZ * absBR.M13;
//            rb = bX * absBR.M22 + bY * absBR.M21;
//            if (Math.Abs(t.X * bR.M33 - t.Z * bR.M13) > ra + rb)
//                return false;

//            //A.Z x B.X
//            ra = aX * absBR.M21 + aY * absBR.M11;
//            rb = bY * absBR.M33 + bZ * absBR.M32;
//            if (Math.Abs(t.Y * bR.M11 - t.X * bR.M21) > ra + rb)
//                return false;

//            //A.Z x B.Y
//            ra = aX * absBR.M22 + aY * absBR.M12;
//            rb = bX * absBR.M33 + bZ * absBR.M31;
//            if (Math.Abs(t.Y * bR.M12 - t.X * bR.M22) > ra + rb)
//                return false;

//            //A.Z x B.Z
//            ra = aX * absBR.M23 + aY * absBR.M13;
//            rb = bX * absBR.M32 + bY * absBR.M31;
//            if (Math.Abs(t.Y * bR.M13 - t.X * bR.M23) > ra + rb)
//                return false;

//            return true;
//        }

//        /// <summary>
//        /// Determines if the two boxes are colliding.
//        /// </summary>
//        /// <param name="a">First box to collide.</param>
//        /// <param name="b">Second box to collide.</param>
//        /// <param name="separationDistance">Distance of separation.</param>
//        /// <param name="separatingAxis">Axis of separation.</param>
//        /// <returns>Whether or not the boxes collide.</returns>
//        public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB, out float separationDistance, out Vector3 separatingAxis)
//        {
//            float aX = a.HalfWidth;
//            float aY = a.HalfHeight;
//            float aZ = a.HalfLength;

//            float bX = b.HalfWidth;
//            float bY = b.HalfHeight;
//            float bZ = b.HalfLength;

//            //Relative rotation from A to B.
//            Matrix3X3 bR;

//            Matrix aO = transformA.OrientationMatrix, bO = transformB.OrientationMatrix;

//            //Relative translation rotated into A's configuration space.
//            Vector3 t;
//            Vector3.Subtract(ref transformB.Position, ref transformA.Position, out t);

//            #region A Face Normals

//            bR.M11 = aO.M11 * bO.M11 + aO.M12 * bO.M12 + aO.M13 * bO.M13;
//            bR.M12 = aO.M11 * bO.M21 + aO.M12 * bO.M22 + aO.M13 * bO.M23;
//            bR.M13 = aO.M11 * bO.M31 + aO.M12 * bO.M32 + aO.M13 * bO.M33;
//            Matrix3X3 absBR;
//            //Epsilons are added to deal with near-parallel edges.
//            absBR.M11 = Math.Abs(bR.M11) + Toolbox.Epsilon;
//            absBR.M12 = Math.Abs(bR.M12) + Toolbox.Epsilon;
//            absBR.M13 = Math.Abs(bR.M13) + Toolbox.Epsilon;
//            float tX = t.X;
//            t.X = t.X * aO.M11 + t.Y * aO.M12 + t.Z * aO.M13;

//            //Test the axes defines by entity A's rotation matrix.
//            //A.X
//            float rarb = aX + bX * absBR.M11 + bY * absBR.M12 + bZ * absBR.M13;
//            if (t.X > rarb)
//            {
//                separationDistance = t.X - rarb;
//                separatingAxis = new Vector3(aO.M11, aO.M12, aO.M13);
//                return false;
//            }
//            if (t.X < -rarb)
//            {
//                separationDistance = -t.X - rarb;
//                separatingAxis = new Vector3(-aO.M11, -aO.M12, -aO.M13);
//                return false;
//            }


//            bR.M21 = aO.M21 * bO.M11 + aO.M22 * bO.M12 + aO.M23 * bO.M13;
//            bR.M22 = aO.M21 * bO.M21 + aO.M22 * bO.M22 + aO.M23 * bO.M23;
//            bR.M23 = aO.M21 * bO.M31 + aO.M22 * bO.M32 + aO.M23 * bO.M33;
//            absBR.M21 = Math.Abs(bR.M21) + Toolbox.Epsilon;
//            absBR.M22 = Math.Abs(bR.M22) + Toolbox.Epsilon;
//            absBR.M23 = Math.Abs(bR.M23) + Toolbox.Epsilon;
//            float tY = t.Y;
//            t.Y = tX * aO.M21 + t.Y * aO.M22 + t.Z * aO.M23;

//            //A.Y
//            rarb = aY + bX * absBR.M21 + bY * absBR.M22 + bZ * absBR.M23;
//            if (t.Y > rarb)
//            {
//                separationDistance = t.Y - rarb;
//                separatingAxis = new Vector3(aO.M21, aO.M22, aO.M23);
//                return false;
//            }
//            if (t.Y < -rarb)
//            {
//                separationDistance = -t.Y - rarb;
//                separatingAxis = new Vector3(-aO.M21, -aO.M22, -aO.M23);
//                return false;
//            }

//            bR.M31 = aO.M31 * bO.M11 + aO.M32 * bO.M12 + aO.M33 * bO.M13;
//            bR.M32 = aO.M31 * bO.M21 + aO.M32 * bO.M22 + aO.M33 * bO.M23;
//            bR.M33 = aO.M31 * bO.M31 + aO.M32 * bO.M32 + aO.M33 * bO.M33;
//            absBR.M31 = Math.Abs(bR.M31) + Toolbox.Epsilon;
//            absBR.M32 = Math.Abs(bR.M32) + Toolbox.Epsilon;
//            absBR.M33 = Math.Abs(bR.M33) + Toolbox.Epsilon;
//            t.Z = tX * aO.M31 + tY * aO.M32 + t.Z * aO.M33;

//            //A.Z
//            rarb = aZ + bX * absBR.M31 + bY * absBR.M32 + bZ * absBR.M33;
//            if (t.Z > rarb)
//            {
//                separationDistance = t.Z - rarb;
//                separatingAxis = new Vector3(aO.M31, aO.M32, aO.M33);
//                return false;
//            }
//            if (t.Z < -rarb)
//            {
//                separationDistance = -t.Z - rarb;
//                separatingAxis = new Vector3(-aO.M31, -aO.M32, -aO.M33);
//                return false;
//            }

//            #endregion

//            #region B Face Normals

//            //Test the axes defines by entity B's rotation matrix.
//            //B.X
//            rarb = bX + aX * absBR.M11 + aY * absBR.M21 + aZ * absBR.M31;
//            float tl = t.X * bR.M11 + t.Y * bR.M21 + t.Z * bR.M31;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(bO.M11, bO.M12, bO.M13);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(-bO.M11, -bO.M12, -bO.M13);
//                return false;
//            }

//            //B.Y
//            rarb = bY + aX * absBR.M12 + aY * absBR.M22 + aZ * absBR.M32;
//            tl = t.X * bR.M12 + t.Y * bR.M22 + t.Z * bR.M32;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(bO.M21, bO.M22, bO.M23);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(-bO.M21, -bO.M22, -bO.M23);
//                return false;
//            }


//            //B.Z
//            rarb = bZ + aX * absBR.M13 + aY * absBR.M23 + aZ * absBR.M33;
//            tl = t.X * bR.M13 + t.Y * bR.M23 + t.Z * bR.M33;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(bO.M31, bO.M32, bO.M33);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(-bO.M31, -bO.M32, -bO.M33);
//                return false;
//            }

//            #endregion

//            #region A.X x B.()

//            //Now for the edge-edge cases.
//            //A.X x B.X
//            rarb = aY * absBR.M31 + aZ * absBR.M21 +
//                   bY * absBR.M13 + bZ * absBR.M12;
//            tl = t.Z * bR.M21 - t.Y * bR.M31;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M12 * bO.M13 - aO.M13 * bO.M12,
//                                             aO.M13 * bO.M11 - aO.M11 * bO.M13,
//                                             aO.M11 * bO.M12 - aO.M12 * bO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M12 * aO.M13 - bO.M13 * aO.M12,
//                                             bO.M13 * aO.M11 - bO.M11 * aO.M13,
//                                             bO.M11 * aO.M12 - bO.M12 * aO.M11);
//                return false;
//            }

//            //A.X x B.Y
//            rarb = aY * absBR.M32 + aZ * absBR.M22 +
//                   bX * absBR.M13 + bZ * absBR.M11;
//            tl = t.Z * bR.M22 - t.Y * bR.M32;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M12 * bO.M23 - aO.M13 * bO.M22,
//                                             aO.M13 * bO.M21 - aO.M11 * bO.M23,
//                                             aO.M11 * bO.M22 - aO.M12 * bO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M22 * aO.M13 - bO.M23 * aO.M12,
//                                             bO.M23 * aO.M11 - bO.M21 * aO.M13,
//                                             bO.M21 * aO.M12 - bO.M22 * aO.M11);
//                return false;
//            }

//            //A.X x B.Z
//            rarb = aY * absBR.M33 + aZ * absBR.M23 +
//                   bX * absBR.M12 + bY * absBR.M11;
//            tl = t.Z * bR.M23 - t.Y * bR.M33;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M12 * bO.M33 - aO.M13 * bO.M32,
//                                             aO.M13 * bO.M31 - aO.M11 * bO.M33,
//                                             aO.M11 * bO.M32 - aO.M12 * bO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M32 * aO.M13 - bO.M33 * aO.M12,
//                                             bO.M33 * aO.M11 - bO.M31 * aO.M13,
//                                             bO.M31 * aO.M12 - bO.M32 * aO.M11);
//                return false;
//            }

//            #endregion

//            #region A.Y x B.()

//            //A.Y x B.X
//            rarb = aX * absBR.M31 + aZ * absBR.M11 +
//                   bY * absBR.M23 + bZ * absBR.M22;
//            tl = t.X * bR.M31 - t.Z * bR.M11;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M22 * bO.M13 - aO.M23 * bO.M12,
//                                             aO.M23 * bO.M11 - aO.M21 * bO.M13,
//                                             aO.M21 * bO.M12 - aO.M22 * bO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M12 * aO.M23 - bO.M13 * aO.M22,
//                                             bO.M13 * aO.M21 - bO.M11 * aO.M23,
//                                             bO.M11 * aO.M22 - bO.M12 * aO.M21);
//                return false;
//            }

//            //A.Y x B.Y
//            rarb = aX * absBR.M32 + aZ * absBR.M12 +
//                   bX * absBR.M23 + bZ * absBR.M21;
//            tl = t.X * bR.M32 - t.Z * bR.M12;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M22 * bO.M23 - aO.M23 * bO.M22,
//                                             aO.M23 * bO.M21 - aO.M21 * bO.M23,
//                                             aO.M21 * bO.M22 - aO.M22 * bO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M22 * aO.M23 - bO.M23 * aO.M22,
//                                             bO.M23 * aO.M21 - bO.M21 * aO.M23,
//                                             bO.M21 * aO.M22 - bO.M22 * aO.M21);
//                return false;
//            }

//            //A.Y x B.Z
//            rarb = aX * absBR.M33 + aZ * absBR.M13 +
//                   bX * absBR.M22 + bY * absBR.M21;
//            tl = t.X * bR.M33 - t.Z * bR.M13;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M22 * bO.M33 - aO.M23 * bO.M32,
//                                             aO.M23 * bO.M31 - aO.M21 * bO.M33,
//                                             aO.M21 * bO.M32 - aO.M22 * bO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M32 * aO.M23 - bO.M33 * aO.M22,
//                                             bO.M33 * aO.M21 - bO.M31 * aO.M23,
//                                             bO.M31 * aO.M22 - bO.M32 * aO.M21);
//                return false;
//            }

//            #endregion

//            #region A.Z x B.()

//            //A.Z x B.X
//            rarb = aX * absBR.M21 + aY * absBR.M11 +
//                   bY * absBR.M33 + bZ * absBR.M32;
//            tl = t.Y * bR.M11 - t.X * bR.M21;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M32 * bO.M13 - aO.M33 * bO.M12,
//                                             aO.M33 * bO.M11 - aO.M31 * bO.M13,
//                                             aO.M31 * bO.M12 - aO.M32 * bO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M12 * aO.M33 - bO.M13 * aO.M32,
//                                             bO.M13 * aO.M31 - bO.M11 * aO.M33,
//                                             bO.M11 * aO.M32 - bO.M12 * aO.M31);
//                return false;
//            }

//            //A.Z x B.Y
//            rarb = aX * absBR.M22 + aY * absBR.M12 +
//                   bX * absBR.M33 + bZ * absBR.M31;
//            tl = t.Y * bR.M12 - t.X * bR.M22;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M32 * bO.M23 - aO.M33 * bO.M22,
//                                             aO.M33 * bO.M21 - aO.M31 * bO.M23,
//                                             aO.M31 * bO.M22 - aO.M32 * bO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M22 * aO.M33 - bO.M23 * aO.M32,
//                                             bO.M23 * aO.M31 - bO.M21 * aO.M33,
//                                             bO.M21 * aO.M32 - bO.M22 * aO.M31);
//                return false;
//            }

//            //A.Z x B.Z
//            rarb = aX * absBR.M23 + aY * absBR.M13 +
//                   bX * absBR.M32 + bY * absBR.M31;
//            tl = t.Y * bR.M13 - t.X * bR.M23;
//            if (tl > rarb)
//            {
//                separationDistance = tl - rarb;
//                separatingAxis = new Vector3(aO.M32 * bO.M33 - aO.M33 * bO.M32,
//                                             aO.M33 * bO.M31 - aO.M31 * bO.M33,
//                                             aO.M31 * bO.M32 - aO.M32 * bO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                separationDistance = -tl - rarb;
//                separatingAxis = new Vector3(bO.M32 * aO.M33 - bO.M33 * aO.M32,
//                                             bO.M33 * aO.M31 - bO.M31 * aO.M33,
//                                             bO.M31 * aO.M32 - bO.M32 * aO.M31);
//                return false;
//            }

//            #endregion

//            separationDistance = 0;
//            separatingAxis = Vector3.Zero;
//            return true;
//        }

//        /// <summary>
//        /// Determines if the two boxes are colliding, including penetration depth data.
//        /// </summary>
//        /// <param name="a">First box to collide.</param>
//        /// <param name="b">Second box to collide.</param>
//        /// <param name="distance">Distance of separation or penetration.</param>
//        /// <param name="axis">Axis of separation or penetration.</param>
//        /// <returns>Whether or not the boxes collide.</returns>
//        public static bool AreBoxesCollidingWithPenetration(Box a, Box b, ref RigidTransform transformA, ref RigidTransform transformB, out float distance, out Vector3 axis)
//        {
//            float aX = a.HalfWidth;
//            float aY = a.HalfHeight;
//            float aZ = a.HalfLength;

//            float bX = b.HalfWidth;
//            float bY = b.HalfHeight;
//            float bZ = b.HalfLength;

//            //Relative rotation from A to B.
//            Matrix3X3 bR;

//            Matrix aO = transformA.OrientationMatrix, bO = transformB.OrientationMatrix;

//            //Relative translation rotated into A's configuration space.
//            Vector3 t;
//            Vector3.Subtract(ref transformB.Position, ref transformA.Position, out t);

//            float tempDistance;
//            float minimumDistance = -float.MaxValue;
//            var minimumAxis = new Vector3();

//            #region A Face Normals

//            bR.M11 = aO.M11 * bO.M11 + aO.M12 * bO.M12 + aO.M13 * bO.M13;
//            bR.M12 = aO.M11 * bO.M21 + aO.M12 * bO.M22 + aO.M13 * bO.M23;
//            bR.M13 = aO.M11 * bO.M31 + aO.M12 * bO.M32 + aO.M13 * bO.M33;
//            Matrix3X3 absBR;
//            //Epsilons are added to deal with near-parallel edges.
//            absBR.M11 = Math.Abs(bR.M11) + Toolbox.Epsilon;
//            absBR.M12 = Math.Abs(bR.M12) + Toolbox.Epsilon;
//            absBR.M13 = Math.Abs(bR.M13) + Toolbox.Epsilon;
//            float tX = t.X;
//            t.X = t.X * aO.M11 + t.Y * aO.M12 + t.Z * aO.M13;

//            //Test the axes defines by entity A's rotation matrix.
//            //A.X
//            float rarb = aX + bX * absBR.M11 + bY * absBR.M12 + bZ * absBR.M13;
//            if (t.X > rarb)
//            {
//                distance = t.X - rarb;
//                axis = new Vector3(aO.M11, aO.M12, aO.M13);
//                return false;
//            }
//            if (t.X < -rarb)
//            {
//                distance = -t.X - rarb;
//                axis = new Vector3(-aO.M11, -aO.M12, -aO.M13);
//                return false;
//            }
//            //Inside
//            if (t.X > 0)
//            {
//                tempDistance = t.X - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(aO.M11, aO.M12, aO.M13);
//                }
//            }
//            else
//            {
//                tempDistance = -t.X - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-aO.M11, -aO.M12, -aO.M13);
//                }
//            }


//            bR.M21 = aO.M21 * bO.M11 + aO.M22 * bO.M12 + aO.M23 * bO.M13;
//            bR.M22 = aO.M21 * bO.M21 + aO.M22 * bO.M22 + aO.M23 * bO.M23;
//            bR.M23 = aO.M21 * bO.M31 + aO.M22 * bO.M32 + aO.M23 * bO.M33;
//            absBR.M21 = Math.Abs(bR.M21) + Toolbox.Epsilon;
//            absBR.M22 = Math.Abs(bR.M22) + Toolbox.Epsilon;
//            absBR.M23 = Math.Abs(bR.M23) + Toolbox.Epsilon;
//            float tY = t.Y;
//            t.Y = tX * aO.M21 + t.Y * aO.M22 + t.Z * aO.M23;

//            //A.Y
//            rarb = aY + bX * absBR.M21 + bY * absBR.M22 + bZ * absBR.M23;
//            if (t.Y > rarb)
//            {
//                distance = t.Y - rarb;
//                axis = new Vector3(aO.M21, aO.M22, aO.M23);
//                return false;
//            }
//            if (t.Y < -rarb)
//            {
//                distance = -t.Y - rarb;
//                axis = new Vector3(-aO.M21, -aO.M22, -aO.M23);
//                return false;
//            }
//            //Inside
//            if (t.Y > 0)
//            {
//                tempDistance = t.Y - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(aO.M21, aO.M22, aO.M23);
//                }
//            }
//            else
//            {
//                tempDistance = -t.Y - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-aO.M21, -aO.M22, -aO.M23);
//                }
//            }

//            bR.M31 = aO.M31 * bO.M11 + aO.M32 * bO.M12 + aO.M33 * bO.M13;
//            bR.M32 = aO.M31 * bO.M21 + aO.M32 * bO.M22 + aO.M33 * bO.M23;
//            bR.M33 = aO.M31 * bO.M31 + aO.M32 * bO.M32 + aO.M33 * bO.M33;
//            absBR.M31 = Math.Abs(bR.M31) + Toolbox.Epsilon;
//            absBR.M32 = Math.Abs(bR.M32) + Toolbox.Epsilon;
//            absBR.M33 = Math.Abs(bR.M33) + Toolbox.Epsilon;
//            t.Z = tX * aO.M31 + tY * aO.M32 + t.Z * aO.M33;

//            //A.Z
//            rarb = aZ + bX * absBR.M31 + bY * absBR.M32 + bZ * absBR.M33;
//            if (t.Z > rarb)
//            {
//                distance = t.Z - rarb;
//                axis = new Vector3(aO.M31, aO.M32, aO.M33);
//                return false;
//            }
//            if (t.Z < -rarb)
//            {
//                distance = -t.Z - rarb;
//                axis = new Vector3(-aO.M31, -aO.M32, -aO.M33);
//                return false;
//            }
//            //Inside
//            if (t.Z > 0)
//            {
//                tempDistance = t.Z - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(aO.M31, aO.M32, aO.M33);
//                }
//            }
//            else
//            {
//                tempDistance = -t.Z - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-aO.M31, -aO.M32, -aO.M33);
//                }
//            }

//            #endregion

//            #region B Face Normals

//            //Test the axes defines by entity B's rotation matrix.
//            //B.X
//            rarb = bX + aX * absBR.M11 + aY * absBR.M21 + aZ * absBR.M31;
//            float tl = t.X * bR.M11 + t.Y * bR.M21 + t.Z * bR.M31;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M11, bO.M12, bO.M13);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(-bO.M11, -bO.M12, -bO.M13);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempDistance = tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(bO.M11, bO.M12, bO.M13);
//                }
//            }
//            else
//            {
//                tempDistance = -tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-bO.M11, -bO.M12, -bO.M13);
//                }
//            }

//            //B.Y
//            rarb = bY + aX * absBR.M12 + aY * absBR.M22 + aZ * absBR.M32;
//            tl = t.X * bR.M12 + t.Y * bR.M22 + t.Z * bR.M32;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M21, bO.M22, bO.M23);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(-bO.M21, -bO.M22, -bO.M23);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempDistance = tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(bO.M21, bO.M22, bO.M23);
//                }
//            }
//            else
//            {
//                tempDistance = -tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-bO.M21, -bO.M22, -bO.M23);
//                }
//            }

//            //B.Z
//            rarb = bZ + aX * absBR.M13 + aY * absBR.M23 + aZ * absBR.M33;
//            tl = t.X * bR.M13 + t.Y * bR.M23 + t.Z * bR.M33;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M31, bO.M32, bO.M33);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(-bO.M31, -bO.M32, -bO.M33);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempDistance = tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(bO.M31, bO.M32, bO.M33);
//                }
//            }
//            else
//            {
//                tempDistance = -tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-bO.M31, -bO.M32, -bO.M33);
//                }
//            }

//            #endregion

//            float axisLengthInverse;
//            Vector3 tempAxis;

//            #region A.X x B.()

//            //Now for the edge-edge cases.
//            //A.X x B.X
//            rarb = aY * absBR.M31 + aZ * absBR.M21 +
//                   bY * absBR.M13 + bZ * absBR.M12;
//            tl = t.Z * bR.M21 - t.Y * bR.M31;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M12 * bO.M13 - aO.M13 * bO.M12,
//                                   aO.M13 * bO.M11 - aO.M11 * bO.M13,
//                                   aO.M11 * bO.M12 - aO.M12 * bO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M12 * aO.M13 - bO.M13 * aO.M12,
//                                   bO.M13 * aO.M11 - bO.M11 * aO.M13,
//                                   bO.M11 * aO.M12 - bO.M12 * aO.M11);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M12 * bO.M13 - aO.M13 * bO.M12,
//                                       aO.M13 * bO.M11 - aO.M11 * bO.M13,
//                                       aO.M11 * bO.M12 - aO.M12 * bO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M12 * aO.M13 - bO.M13 * aO.M12,
//                                       bO.M13 * aO.M11 - bO.M11 * aO.M13,
//                                       bO.M11 * aO.M12 - bO.M12 * aO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.X x B.Y
//            rarb = aY * absBR.M32 + aZ * absBR.M22 +
//                   bX * absBR.M13 + bZ * absBR.M11;
//            tl = t.Z * bR.M22 - t.Y * bR.M32;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M12 * bO.M23 - aO.M13 * bO.M22,
//                                   aO.M13 * bO.M21 - aO.M11 * bO.M23,
//                                   aO.M11 * bO.M22 - aO.M12 * bO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M22 * aO.M13 - bO.M23 * aO.M12,
//                                   bO.M23 * aO.M11 - bO.M21 * aO.M13,
//                                   bO.M21 * aO.M12 - bO.M22 * aO.M11);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M12 * bO.M23 - aO.M13 * bO.M22,
//                                       aO.M13 * bO.M21 - aO.M11 * bO.M23,
//                                       aO.M11 * bO.M22 - aO.M12 * bO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M22 * aO.M13 - bO.M23 * aO.M12,
//                                       bO.M23 * aO.M11 - bO.M21 * aO.M13,
//                                       bO.M21 * aO.M12 - bO.M22 * aO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.X x B.Z
//            rarb = aY * absBR.M33 + aZ * absBR.M23 +
//                   bX * absBR.M12 + bY * absBR.M11;
//            tl = t.Z * bR.M23 - t.Y * bR.M33;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M12 * bO.M33 - aO.M13 * bO.M32,
//                                   aO.M13 * bO.M31 - aO.M11 * bO.M33,
//                                   aO.M11 * bO.M32 - aO.M12 * bO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M32 * aO.M13 - bO.M33 * aO.M12,
//                                   bO.M33 * aO.M11 - bO.M31 * aO.M13,
//                                   bO.M31 * aO.M12 - bO.M32 * aO.M11);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M12 * bO.M33 - aO.M13 * bO.M32,
//                                       aO.M13 * bO.M31 - aO.M11 * bO.M33,
//                                       aO.M11 * bO.M32 - aO.M12 * bO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M32 * aO.M13 - bO.M33 * aO.M12,
//                                       bO.M33 * aO.M11 - bO.M31 * aO.M13,
//                                       bO.M31 * aO.M12 - bO.M32 * aO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            #endregion

//            #region A.Y x B.()

//            //A.Y x B.X
//            rarb = aX * absBR.M31 + aZ * absBR.M11 +
//                   bY * absBR.M23 + bZ * absBR.M22;
//            tl = t.X * bR.M31 - t.Z * bR.M11;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M22 * bO.M13 - aO.M23 * bO.M12,
//                                   aO.M23 * bO.M11 - aO.M21 * bO.M13,
//                                   aO.M21 * bO.M12 - aO.M22 * bO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M12 * aO.M23 - bO.M13 * aO.M22,
//                                   bO.M13 * aO.M21 - bO.M11 * aO.M23,
//                                   bO.M11 * aO.M22 - bO.M12 * aO.M21);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M22 * bO.M13 - aO.M23 * bO.M12,
//                                       aO.M23 * bO.M11 - aO.M21 * bO.M13,
//                                       aO.M21 * bO.M12 - aO.M22 * bO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M12 * aO.M23 - bO.M13 * aO.M22,
//                                       bO.M13 * aO.M21 - bO.M11 * aO.M23,
//                                       bO.M11 * aO.M22 - bO.M12 * aO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.Y x B.Y
//            rarb = aX * absBR.M32 + aZ * absBR.M12 +
//                   bX * absBR.M23 + bZ * absBR.M21;
//            tl = t.X * bR.M32 - t.Z * bR.M12;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M22 * bO.M23 - aO.M23 * bO.M22,
//                                   aO.M23 * bO.M21 - aO.M21 * bO.M23,
//                                   aO.M21 * bO.M22 - aO.M22 * bO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M22 * aO.M23 - bO.M23 * aO.M22,
//                                   bO.M23 * aO.M21 - bO.M21 * aO.M23,
//                                   bO.M21 * aO.M22 - bO.M22 * aO.M21);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M22 * bO.M23 - aO.M23 * bO.M22,
//                                       aO.M23 * bO.M21 - aO.M21 * bO.M23,
//                                       aO.M21 * bO.M22 - aO.M22 * bO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M22 * aO.M23 - bO.M23 * aO.M22,
//                                       bO.M23 * aO.M21 - bO.M21 * aO.M23,
//                                       bO.M21 * aO.M22 - bO.M22 * aO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.Y x B.Z
//            rarb = aX * absBR.M33 + aZ * absBR.M13 +
//                   bX * absBR.M22 + bY * absBR.M21;
//            tl = t.X * bR.M33 - t.Z * bR.M13;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M22 * bO.M33 - aO.M23 * bO.M32,
//                                   aO.M23 * bO.M31 - aO.M21 * bO.M33,
//                                   aO.M21 * bO.M32 - aO.M22 * bO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M32 * aO.M23 - bO.M33 * aO.M22,
//                                   bO.M33 * aO.M21 - bO.M31 * aO.M23,
//                                   bO.M31 * aO.M22 - bO.M32 * aO.M21);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M22 * bO.M33 - aO.M23 * bO.M32,
//                                       aO.M23 * bO.M31 - aO.M21 * bO.M33,
//                                       aO.M21 * bO.M32 - aO.M22 * bO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M32 * aO.M23 - bO.M33 * aO.M22,
//                                       bO.M33 * aO.M21 - bO.M31 * aO.M23,
//                                       bO.M31 * aO.M22 - bO.M32 * aO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            #endregion

//            #region A.Z x B.()

//            //A.Z x B.X
//            rarb = aX * absBR.M21 + aY * absBR.M11 +
//                   bY * absBR.M33 + bZ * absBR.M32;
//            tl = t.Y * bR.M11 - t.X * bR.M21;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M32 * bO.M13 - aO.M33 * bO.M12,
//                                   aO.M33 * bO.M11 - aO.M31 * bO.M13,
//                                   aO.M31 * bO.M12 - aO.M32 * bO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M12 * aO.M33 - bO.M13 * aO.M32,
//                                   bO.M13 * aO.M31 - bO.M11 * aO.M33,
//                                   bO.M11 * aO.M32 - bO.M12 * aO.M31);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M32 * bO.M13 - aO.M33 * bO.M12,
//                                       aO.M33 * bO.M11 - aO.M31 * bO.M13,
//                                       aO.M31 * bO.M12 - aO.M32 * bO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M12 * aO.M33 - bO.M13 * aO.M32,
//                                       bO.M13 * aO.M31 - bO.M11 * aO.M33,
//                                       bO.M11 * aO.M32 - bO.M12 * aO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.Z x B.Y
//            rarb = aX * absBR.M22 + aY * absBR.M12 +
//                   bX * absBR.M33 + bZ * absBR.M31;
//            tl = t.Y * bR.M12 - t.X * bR.M22;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M32 * bO.M23 - aO.M33 * bO.M22,
//                                   aO.M33 * bO.M21 - aO.M31 * bO.M23,
//                                   aO.M31 * bO.M22 - aO.M32 * bO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M22 * aO.M33 - bO.M23 * aO.M32,
//                                   bO.M23 * aO.M31 - bO.M21 * aO.M33,
//                                   bO.M21 * aO.M32 - bO.M22 * aO.M31);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M32 * bO.M23 - aO.M33 * bO.M22,
//                                       aO.M33 * bO.M21 - aO.M31 * bO.M23,
//                                       aO.M31 * bO.M22 - aO.M32 * bO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M22 * aO.M33 - bO.M23 * aO.M32,
//                                       bO.M23 * aO.M31 - bO.M21 * aO.M33,
//                                       bO.M21 * aO.M32 - bO.M22 * aO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.Z x B.Z
//            rarb = aX * absBR.M23 + aY * absBR.M13 +
//                   bX * absBR.M32 + bY * absBR.M31;
//            tl = t.Y * bR.M13 - t.X * bR.M23;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(aO.M32 * bO.M33 - aO.M33 * bO.M32,
//                                   aO.M33 * bO.M31 - aO.M31 * bO.M33,
//                                   aO.M31 * bO.M32 - aO.M32 * bO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M32 * aO.M33 - bO.M33 * aO.M32,
//                                   bO.M33 * aO.M31 - bO.M31 * aO.M33,
//                                   bO.M31 * aO.M32 - bO.M32 * aO.M31);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(aO.M32 * bO.M33 - aO.M33 * bO.M32,
//                                       aO.M33 * bO.M31 - aO.M31 * bO.M33,
//                                       aO.M31 * bO.M32 - aO.M32 * bO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(bO.M32 * aO.M33 - bO.M33 * aO.M32,
//                                       bO.M33 * aO.M31 - bO.M31 * aO.M33,
//                                       bO.M31 * aO.M32 - bO.M32 * aO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            #endregion

//            distance = minimumDistance;
//            axis = minimumAxis;
//            return true;
//        }

//#if ALLOWUNSAFE
//        /// <summary>
//        /// Determines if the two boxes are colliding and computes contact data.
//        /// </summary>
//        /// <param name="a">First box to collide.</param>
//        /// <param name="b">Second box to collide.</param>
//        /// <param name="distance">Distance of separation or penetration.</param>
//        /// <param name="axis">Axis of separation or penetration.</param>
//        /// <param name="contactData">Data</param>
//        /// <returns>Whether or not the boxes collide.</returns>
//        public static unsafe bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB, out float distance, out Vector3 axis, out TinyStructList<BoxContactData> contactData)
//        {
//            BoxContactDataCache tempData;
//            bool toReturn = AreBoxesColliding(a, b, ref transformA, ref transformB, out distance, out axis, out tempData);
//            BoxContactData* dataPointer = &tempData.D1;
//            contactData = new TinyStructList<BoxContactData>();
//            for (int i = 0; i < tempData.Count; i++)
//            {
//                contactData.Add(ref dataPointer[i]);
//            }
//            return toReturn;
//        }
//#endif

//        /// <summary>
//        /// Determines if the two boxes are colliding and computes contact data.
//        /// </summary>
//        /// <param name="a">First box to collide.</param>
//        /// <param name="b">Second box to collide.</param>
//        /// <param name="distance">Distance of separation or penetration.</param>
//        /// <param name="axis">Axis of separation or penetration.</param>
//        /// <param name="contactData">Contact positions, depths, and ids.</param>
//        /// <returns>Whether or not the boxes collide.</returns>
//#if ALLOWUNSAFE
//        public static bool AreBoxesColliding(BoxShape a, BoxShape b, ref RigidTransform transformA, ref RigidTransform transformB, out float distance, out Vector3 axis, out BoxContactDataCache contactData)
//#else
//        public static bool AreBoxesColliding(Box a, Box b, out float distance, out Vector3 axis, out TinyStructList<BoxContactData> contactData)
//#endif
//        {
//            float aX = a.HalfWidth;
//            float aY = a.HalfHeight;
//            float aZ = a.HalfLength;

//            float bX = b.HalfWidth;
//            float bY = b.HalfHeight;
//            float bZ = b.HalfLength;

//#if ALLOWUNSAFE
//            contactData = new BoxContactDataCache();
//#else
//            contactData = new TinyStructList<BoxContactData>();
//#endif
//            //positions = new TinyStructList<Vector3>();
//            //depths = new TinyList<float>();
//            //ids = new TinyList<int>();

//            //Relative rotation from A to B.
//            Matrix3X3 bR;

//            Matrix3X3 aO;
//            Matrix3X3 bO;
//            Matrix3X3.CreateFromQuaternion(ref transformA.Orientation, out aO);
//            Matrix3X3.CreateFromQuaternion(ref transformB.Orientation, out bO);

//            //Relative translation rotated into A's configuration space.
//            Vector3 t;
//            Vector3.Subtract(ref transformB.Position, ref transformA.Position, out t);

//            float tempDistance;
//            float minimumDistance = -float.MaxValue;
//            var minimumAxis = new Vector3();
//            byte minimumFeature = 2; //2 means edge.  0-> A face, 1 -> B face.

//            #region A Face Normals

//            bR.M11 = aO.M11 * bO.M11 + aO.M12 * bO.M12 + aO.M13 * bO.M13;
//            bR.M12 = aO.M11 * bO.M21 + aO.M12 * bO.M22 + aO.M13 * bO.M23;
//            bR.M13 = aO.M11 * bO.M31 + aO.M12 * bO.M32 + aO.M13 * bO.M33;
//            Matrix3X3 absBR;
//            //Epsilons are added to deal with near-parallel edges.
//            absBR.M11 = Math.Abs(bR.M11) + Toolbox.Epsilon;
//            absBR.M12 = Math.Abs(bR.M12) + Toolbox.Epsilon;
//            absBR.M13 = Math.Abs(bR.M13) + Toolbox.Epsilon;
//            float tX = t.X;
//            t.X = t.X * aO.M11 + t.Y * aO.M12 + t.Z * aO.M13;

//            //Test the axes defines by entity A's rotation matrix.
//            //A.X
//            float rarb = aX + bX * absBR.M11 + bY * absBR.M12 + bZ * absBR.M13;
//            if (t.X > rarb)
//            {
//                distance = t.X - rarb;
//                axis = new Vector3(-aO.M11, -aO.M12, -aO.M13);
//                return false;
//            }
//            if (t.X < -rarb)
//            {
//                distance = -t.X - rarb;
//                axis = new Vector3(aO.M11, aO.M12, aO.M13);
//                return false;
//            }
//            //Inside
//            if (t.X > 0)
//            {
//                tempDistance = t.X - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-aO.M11, -aO.M12, -aO.M13);
//                    minimumFeature = 0;
//                }
//            }
//            else
//            {
//                tempDistance = -t.X - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(aO.M11, aO.M12, aO.M13);
//                    minimumFeature = 0;
//                }
//            }


//            bR.M21 = aO.M21 * bO.M11 + aO.M22 * bO.M12 + aO.M23 * bO.M13;
//            bR.M22 = aO.M21 * bO.M21 + aO.M22 * bO.M22 + aO.M23 * bO.M23;
//            bR.M23 = aO.M21 * bO.M31 + aO.M22 * bO.M32 + aO.M23 * bO.M33;
//            absBR.M21 = Math.Abs(bR.M21) + Toolbox.Epsilon;
//            absBR.M22 = Math.Abs(bR.M22) + Toolbox.Epsilon;
//            absBR.M23 = Math.Abs(bR.M23) + Toolbox.Epsilon;
//            float tY = t.Y;
//            t.Y = tX * aO.M21 + t.Y * aO.M22 + t.Z * aO.M23;

//            //A.Y
//            rarb = aY + bX * absBR.M21 + bY * absBR.M22 + bZ * absBR.M23;
//            if (t.Y > rarb)
//            {
//                distance = t.Y - rarb;
//                axis = new Vector3(-aO.M21, -aO.M22, -aO.M23);
//                return false;
//            }
//            if (t.Y < -rarb)
//            {
//                distance = -t.Y - rarb;
//                axis = new Vector3(aO.M21, aO.M22, aO.M23);
//                return false;
//            }
//            //Inside
//            if (t.Y > 0)
//            {
//                tempDistance = t.Y - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-aO.M21, -aO.M22, -aO.M23);
//                    minimumFeature = 0;
//                }
//            }
//            else
//            {
//                tempDistance = -t.Y - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(aO.M21, aO.M22, aO.M23);
//                    minimumFeature = 0;
//                }
//            }

//            bR.M31 = aO.M31 * bO.M11 + aO.M32 * bO.M12 + aO.M33 * bO.M13;
//            bR.M32 = aO.M31 * bO.M21 + aO.M32 * bO.M22 + aO.M33 * bO.M23;
//            bR.M33 = aO.M31 * bO.M31 + aO.M32 * bO.M32 + aO.M33 * bO.M33;
//            absBR.M31 = Math.Abs(bR.M31) + Toolbox.Epsilon;
//            absBR.M32 = Math.Abs(bR.M32) + Toolbox.Epsilon;
//            absBR.M33 = Math.Abs(bR.M33) + Toolbox.Epsilon;
//            t.Z = tX * aO.M31 + tY * aO.M32 + t.Z * aO.M33;

//            //A.Z
//            rarb = aZ + bX * absBR.M31 + bY * absBR.M32 + bZ * absBR.M33;
//            if (t.Z > rarb)
//            {
//                distance = t.Z - rarb;
//                axis = new Vector3(-aO.M31, -aO.M32, -aO.M33);
//                return false;
//            }
//            if (t.Z < -rarb)
//            {
//                distance = -t.Z - rarb;
//                axis = new Vector3(aO.M31, aO.M32, aO.M33);
//                return false;
//            }
//            //Inside
//            if (t.Z > 0)
//            {
//                tempDistance = t.Z - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-aO.M31, -aO.M32, -aO.M33);
//                    minimumFeature = 0;
//                }
//            }
//            else
//            {
//                tempDistance = -t.Z - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(aO.M31, aO.M32, aO.M33);
//                    minimumFeature = 0;
//                }
//            }

//            #endregion

//            const float antiBBias = .01f;
//            minimumDistance += antiBBias;

//            #region B Face Normals

//            //Test the axes defines by entity B's rotation matrix.
//            //B.X
//            rarb = bX + aX * absBR.M11 + aY * absBR.M21 + aZ * absBR.M31;
//            float tl = t.X * bR.M11 + t.Y * bR.M21 + t.Z * bR.M31;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(-bO.M11, -bO.M12, -bO.M13);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M11, bO.M12, bO.M13);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempDistance = tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-bO.M11, -bO.M12, -bO.M13);
//                    minimumFeature = 1;
//                }
//            }
//            else
//            {
//                tempDistance = -tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(bO.M11, bO.M12, bO.M13);
//                    minimumFeature = 1;
//                }
//            }

//            //B.Y
//            rarb = bY + aX * absBR.M12 + aY * absBR.M22 + aZ * absBR.M32;
//            tl = t.X * bR.M12 + t.Y * bR.M22 + t.Z * bR.M32;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(-bO.M21, -bO.M22, -bO.M23);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M21, bO.M22, bO.M23);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempDistance = tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-bO.M21, -bO.M22, -bO.M23);
//                    minimumFeature = 1;
//                }
//            }
//            else
//            {
//                tempDistance = -tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(bO.M21, bO.M22, bO.M23);
//                    minimumFeature = 1;
//                }
//            }

//            //B.Z
//            rarb = bZ + aX * absBR.M13 + aY * absBR.M23 + aZ * absBR.M33;
//            tl = t.X * bR.M13 + t.Y * bR.M23 + t.Z * bR.M33;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(-bO.M31, -bO.M32, -bO.M33);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(bO.M31, bO.M32, bO.M33);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempDistance = tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(-bO.M31, -bO.M32, -bO.M33);
//                    minimumFeature = 1;
//                }
//            }
//            else
//            {
//                tempDistance = -tl - rarb;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumDistance = tempDistance;
//                    minimumAxis = new Vector3(bO.M31, bO.M32, bO.M33);
//                    minimumFeature = 1;
//                }
//            }

//            #endregion

//            if (minimumFeature != 1)
//                minimumDistance -= antiBBias;

//            const float antiEdgeBias = .01f;
//            minimumDistance += antiEdgeBias;
//            float axisLengthInverse;
//            Vector3 tempAxis;

//            #region A.X x B.()

//            //Now for the edge-edge cases.
//            //A.X x B.X
//            rarb = aY * absBR.M31 + aZ * absBR.M21 +
//                   bY * absBR.M13 + bZ * absBR.M12;
//            tl = t.Z * bR.M21 - t.Y * bR.M31;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M12 * aO.M13 - bO.M13 * aO.M12,
//                                   bO.M13 * aO.M11 - bO.M11 * aO.M13,
//                                   bO.M11 * aO.M12 - bO.M12 * aO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(aO.M12 * bO.M13 - aO.M13 * bO.M12,
//                                   aO.M13 * bO.M11 - aO.M11 * bO.M13,
//                                   aO.M11 * bO.M12 - aO.M12 * bO.M11);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M12 * aO.M13 - bO.M13 * aO.M12,
//                                       bO.M13 * aO.M11 - bO.M11 * aO.M13,
//                                       bO.M11 * aO.M12 - bO.M12 * aO.M11);

//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M12 * bO.M13 - aO.M13 * bO.M12,
//                                       aO.M13 * bO.M11 - aO.M11 * bO.M13,
//                                       aO.M11 * bO.M12 - aO.M12 * bO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.X x B.Y
//            rarb = aY * absBR.M32 + aZ * absBR.M22 +
//                   bX * absBR.M13 + bZ * absBR.M11;
//            tl = t.Z * bR.M22 - t.Y * bR.M32;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M22 * aO.M13 - bO.M23 * aO.M12,
//                                   bO.M23 * aO.M11 - bO.M21 * aO.M13,
//                                   bO.M21 * aO.M12 - bO.M22 * aO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(aO.M12 * bO.M23 - aO.M13 * bO.M22,
//                                   aO.M13 * bO.M21 - aO.M11 * bO.M23,
//                                   aO.M11 * bO.M22 - aO.M12 * bO.M21);

//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M22 * aO.M13 - bO.M23 * aO.M12,
//                                       bO.M23 * aO.M11 - bO.M21 * aO.M13,
//                                       bO.M21 * aO.M12 - bO.M22 * aO.M11);

//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M12 * bO.M23 - aO.M13 * bO.M22,
//                                       aO.M13 * bO.M21 - aO.M11 * bO.M23,
//                                       aO.M11 * bO.M22 - aO.M12 * bO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.X x B.Z
//            rarb = aY * absBR.M33 + aZ * absBR.M23 +
//                   bX * absBR.M12 + bY * absBR.M11;
//            tl = t.Z * bR.M23 - t.Y * bR.M33;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M32 * aO.M13 - bO.M33 * aO.M12,
//                                   bO.M33 * aO.M11 - bO.M31 * aO.M13,
//                                   bO.M31 * aO.M12 - bO.M32 * aO.M11);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;

//                axis = new Vector3(aO.M12 * bO.M33 - aO.M13 * bO.M32,
//                                   aO.M13 * bO.M31 - aO.M11 * bO.M33,
//                                   aO.M11 * bO.M32 - aO.M12 * bO.M31);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M32 * aO.M13 - bO.M33 * aO.M12,
//                                       bO.M33 * aO.M11 - bO.M31 * aO.M13,
//                                       bO.M31 * aO.M12 - bO.M32 * aO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M12 * bO.M33 - aO.M13 * bO.M32,
//                                       aO.M13 * bO.M31 - aO.M11 * bO.M33,
//                                       aO.M11 * bO.M32 - aO.M12 * bO.M31);

//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            #endregion

//            #region A.Y x B.()

//            //A.Y x B.X
//            rarb = aX * absBR.M31 + aZ * absBR.M11 +
//                   bY * absBR.M23 + bZ * absBR.M22;
//            tl = t.X * bR.M31 - t.Z * bR.M11;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M12 * aO.M23 - bO.M13 * aO.M22,
//                                   bO.M13 * aO.M21 - bO.M11 * aO.M23,
//                                   bO.M11 * aO.M22 - bO.M12 * aO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(aO.M22 * bO.M13 - aO.M23 * bO.M12,
//                                   aO.M23 * bO.M11 - aO.M21 * bO.M13,
//                                   aO.M21 * bO.M12 - aO.M22 * bO.M11);

//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M12 * aO.M23 - bO.M13 * aO.M22,
//                                       bO.M13 * aO.M21 - bO.M11 * aO.M23,
//                                       bO.M11 * aO.M22 - bO.M12 * aO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M22 * bO.M13 - aO.M23 * bO.M12,
//                                       aO.M23 * bO.M11 - aO.M21 * bO.M13,
//                                       aO.M21 * bO.M12 - aO.M22 * bO.M11);

//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.Y x B.Y
//            rarb = aX * absBR.M32 + aZ * absBR.M12 +
//                   bX * absBR.M23 + bZ * absBR.M21;
//            tl = t.X * bR.M32 - t.Z * bR.M12;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M22 * aO.M23 - bO.M23 * aO.M22,
//                                   bO.M23 * aO.M21 - bO.M21 * aO.M23,
//                                   bO.M21 * aO.M22 - bO.M22 * aO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;

//                axis = new Vector3(aO.M22 * bO.M23 - aO.M23 * bO.M22,
//                                   aO.M23 * bO.M21 - aO.M21 * bO.M23,
//                                   aO.M21 * bO.M22 - aO.M22 * bO.M21);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M22 * aO.M23 - bO.M23 * aO.M22,
//                                       bO.M23 * aO.M21 - bO.M21 * aO.M23,
//                                       bO.M21 * aO.M22 - bO.M22 * aO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M22 * bO.M23 - aO.M23 * bO.M22,
//                                       aO.M23 * bO.M21 - aO.M21 * bO.M23,
//                                       aO.M21 * bO.M22 - aO.M22 * bO.M21);

//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.Y x B.Z
//            rarb = aX * absBR.M33 + aZ * absBR.M13 +
//                   bX * absBR.M22 + bY * absBR.M21;
//            tl = t.X * bR.M33 - t.Z * bR.M13;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M32 * aO.M23 - bO.M33 * aO.M22,
//                                   bO.M33 * aO.M21 - bO.M31 * aO.M23,
//                                   bO.M31 * aO.M22 - bO.M32 * aO.M21);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;

//                axis = new Vector3(aO.M22 * bO.M33 - aO.M23 * bO.M32,
//                                   aO.M23 * bO.M31 - aO.M21 * bO.M33,
//                                   aO.M21 * bO.M32 - aO.M22 * bO.M31);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M32 * aO.M23 - bO.M33 * aO.M22,
//                                       bO.M33 * aO.M21 - bO.M31 * aO.M23,
//                                       bO.M31 * aO.M22 - bO.M32 * aO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M22 * bO.M33 - aO.M23 * bO.M32,
//                                       aO.M23 * bO.M31 - aO.M21 * bO.M33,
//                                       aO.M21 * bO.M32 - aO.M22 * bO.M31);

//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            #endregion

//            #region A.Z x B.()

//            //A.Z x B.X
//            rarb = aX * absBR.M21 + aY * absBR.M11 +
//                   bY * absBR.M33 + bZ * absBR.M32;
//            tl = t.Y * bR.M11 - t.X * bR.M21;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M12 * aO.M33 - bO.M13 * aO.M32,
//                                   bO.M13 * aO.M31 - bO.M11 * aO.M33,
//                                   bO.M11 * aO.M32 - bO.M12 * aO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;

//                axis = new Vector3(aO.M32 * bO.M13 - aO.M33 * bO.M12,
//                                   aO.M33 * bO.M11 - aO.M31 * bO.M13,
//                                   aO.M31 * bO.M12 - aO.M32 * bO.M11);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M12 * aO.M33 - bO.M13 * aO.M32,
//                                       bO.M13 * aO.M31 - bO.M11 * aO.M33,
//                                       bO.M11 * aO.M32 - bO.M12 * aO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M32 * bO.M13 - aO.M33 * bO.M12,
//                                       aO.M33 * bO.M11 - aO.M31 * bO.M13,
//                                       aO.M31 * bO.M12 - aO.M32 * bO.M11);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.Z x B.Y
//            rarb = aX * absBR.M22 + aY * absBR.M12 +
//                   bX * absBR.M33 + bZ * absBR.M31;
//            tl = t.Y * bR.M12 - t.X * bR.M22;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M22 * aO.M33 - bO.M23 * aO.M32,
//                                   bO.M23 * aO.M31 - bO.M21 * aO.M33,
//                                   bO.M21 * aO.M32 - bO.M22 * aO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;

//                axis = new Vector3(aO.M32 * bO.M23 - aO.M33 * bO.M22,
//                                   aO.M33 * bO.M21 - aO.M31 * bO.M23,
//                                   aO.M31 * bO.M22 - aO.M32 * bO.M21);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M22 * aO.M33 - bO.M23 * aO.M32,
//                                       bO.M23 * aO.M31 - bO.M21 * aO.M33,
//                                       bO.M21 * aO.M32 - bO.M22 * aO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M32 * bO.M23 - aO.M33 * bO.M22,
//                                       aO.M33 * bO.M21 - aO.M31 * bO.M23,
//                                       aO.M31 * bO.M22 - aO.M32 * bO.M21);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            //A.Z x B.Z
//            rarb = aX * absBR.M23 + aY * absBR.M13 +
//                   bX * absBR.M32 + bY * absBR.M31;
//            tl = t.Y * bR.M13 - t.X * bR.M23;
//            if (tl > rarb)
//            {
//                distance = tl - rarb;
//                axis = new Vector3(bO.M32 * aO.M33 - bO.M33 * aO.M32,
//                                   bO.M33 * aO.M31 - bO.M31 * aO.M33,
//                                   bO.M31 * aO.M32 - bO.M32 * aO.M31);
//                return false;
//            }
//            if (tl < -rarb)
//            {
//                distance = -tl - rarb;
//                axis = new Vector3(aO.M32 * bO.M33 - aO.M33 * bO.M32,
//                                   aO.M33 * bO.M31 - aO.M31 * bO.M33,
//                                   aO.M31 * bO.M32 - aO.M32 * bO.M31);
//                return false;
//            }
//            //Inside
//            if (tl > 0)
//            {
//                tempAxis = new Vector3(bO.M32 * aO.M33 - bO.M33 * aO.M32,
//                                       bO.M33 * aO.M31 - bO.M31 * aO.M33,
//                                       bO.M31 * aO.M32 - bO.M32 * aO.M31);
//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }
//            else
//            {
//                tempAxis = new Vector3(aO.M32 * bO.M33 - aO.M33 * bO.M32,
//                                       aO.M33 * bO.M31 - aO.M31 * bO.M33,
//                                       aO.M31 * bO.M32 - aO.M32 * bO.M31);

//                axisLengthInverse = 1 / tempAxis.Length();
//                tempDistance = (-tl - rarb) * axisLengthInverse;
//                if (tempDistance > minimumDistance)
//                {
//                    minimumFeature = 2;
//                    minimumDistance = tempDistance;
//                    tempAxis.X *= axisLengthInverse;
//                    tempAxis.Y *= axisLengthInverse;
//                    tempAxis.Z *= axisLengthInverse;
//                    minimumAxis = tempAxis;
//                }
//            }

//            #endregion

//            if (minimumFeature == 2)
//            {
//                Vector3 position;
//                float depth;
//                int id;
//                GetEdgeEdgeContact(a, b, transformA, transformB, ref minimumAxis, out position, out depth, out id);
//#if ALLOWUNSAFE
//                contactData.D1.Position = position;
//                contactData.D1.Depth = depth;
//                contactData.D1.Id = id;
//                contactData.Count = 1;
//#else
//                var toAdd = new BoxContactData();
//                toAdd.Position = position;
//                toAdd.Depth = depth;
//                toAdd.Id = id;
//                contactData.Add(ref toAdd);
//#endif
//            }
//            else
//            {
//                minimumDistance -= antiEdgeBias;
//                GetFaceContacts(a, b, ref aO, ref bO, ref transformA.Position, ref transformB.Position, minimumFeature == 0, ref minimumAxis, out contactData);
//            }

//            distance = minimumDistance;
//            axis = minimumAxis;
//            return true;
//        }


//        internal static void GetEdgeEdgeContact(BoxShape a, BoxShape b, RigidTransform transformA, RigidTransform transformB, ref Vector3 mtd, out Vector3 point, out float depth, out int id)
//        {
//            //Put the minimum translation direction into the local space of each object.
//            Vector3 mtdA, mtdB;
//            Vector3 negatedMtd;
//            Vector3.Negate(ref mtd, out negatedMtd);
//            Matrix3X3 aOrientation, bOrientation;
//            Matrix3X3.CreateFromQuaternion(ref transformA.Orientation, out aOrientation);
//            Matrix3X3.CreateFromQuaternion(ref transformB.Orientation, out bOrientation);
//            Matrix3X3.TransformTranspose(ref negatedMtd, ref aOrientation, out mtdA);
//            Matrix3X3.TransformTranspose(ref mtd, ref bOrientation, out mtdB);


//#if !WINDOWS
//            Vector3 edgeA1 = new Vector3(), edgeA2 = new Vector3();
//            Vector3 edgeB1 = new Vector3(), edgeB2 = new Vector3();
//#else
//            Vector3 edgeA1, edgeA2;
//            Vector3 edgeB1, edgeB2;
//#endif
//            float aHalfWidth = a.halfWidth;
//            float aHalfHeight = a.halfHeight;
//            float aHalfLength = a.halfLength;

//            float bHalfWidth = b.halfWidth;
//            float bHalfHeight = b.halfHeight;
//            float bHalfLength = b.halfLength;

//            int edgeA1Id, edgeA2Id;
//            int edgeB1Id, edgeB2Id;

//            //This is an edge-edge collision, so one (AND ONLY ONE) of the components in the 
//            //local direction must be very close to zero.  We can use an arbitrary fixed 
//            //epsilon because the mtd is always unit length.

//            #region Edge A

//            if (Math.Abs(mtdA.X) < Toolbox.Epsilon)
//            {
//                //mtd is in the Y-Z plane.
//                if (mtdA.Y > 0)
//                {
//                    if (mtdA.Z > 0)
//                    {
//                        //++
//                        edgeA1.X = -aHalfWidth;
//                        edgeA1.Y = aHalfHeight;
//                        edgeA1.Z = aHalfLength;

//                        edgeA2.X = aHalfWidth;
//                        edgeA2.Y = aHalfHeight;
//                        edgeA2.Z = aHalfLength;

//                        edgeA1Id = 6;
//                        edgeA2Id = 7;
//                    }
//                    else
//                    {
//                        //+-
//                        edgeA1.X = -aHalfWidth;
//                        edgeA1.Y = aHalfHeight;
//                        edgeA1.Z = -aHalfLength;

//                        edgeA2.X = aHalfWidth;
//                        edgeA2.Y = aHalfHeight;
//                        edgeA2.Z = -aHalfLength;

//                        edgeA1Id = 2;
//                        edgeA2Id = 3;
//                    }
//                }
//                else
//                {
//                    if (mtdA.Z > 0)
//                    {
//                        //-+
//                        edgeA1.X = -aHalfWidth;
//                        edgeA1.Y = -aHalfHeight;
//                        edgeA1.Z = aHalfLength;

//                        edgeA2.X = aHalfWidth;
//                        edgeA2.Y = -aHalfHeight;
//                        edgeA2.Z = aHalfLength;

//                        edgeA1Id = 4;
//                        edgeA2Id = 5;
//                    }
//                    else
//                    {
//                        //--
//                        edgeA1.X = -aHalfWidth;
//                        edgeA1.Y = -aHalfHeight;
//                        edgeA1.Z = -aHalfLength;

//                        edgeA2.X = aHalfWidth;
//                        edgeA2.Y = -aHalfHeight;
//                        edgeA2.Z = -aHalfLength;

//                        edgeA1Id = 0;
//                        edgeA2Id = 1;
//                    }
//                }
//            }
//            else if (Math.Abs(mtdA.Y) < Toolbox.Epsilon)
//            {
//                //mtd is in the X-Z plane
//                if (mtdA.X > 0)
//                {
//                    if (mtdA.Z > 0)
//                    {
//                        //++
//                        edgeA1.X = aHalfWidth;
//                        edgeA1.Y = -aHalfHeight;
//                        edgeA1.Z = aHalfLength;

//                        edgeA2.X = aHalfWidth;
//                        edgeA2.Y = aHalfHeight;
//                        edgeA2.Z = aHalfLength;

//                        edgeA1Id = 5;
//                        edgeA2Id = 7;
//                    }
//                    else
//                    {
//                        //+-
//                        edgeA1.X = aHalfWidth;
//                        edgeA1.Y = -aHalfHeight;
//                        edgeA1.Z = -aHalfLength;

//                        edgeA2.X = aHalfWidth;
//                        edgeA2.Y = aHalfHeight;
//                        edgeA2.Z = -aHalfLength;

//                        edgeA1Id = 1;
//                        edgeA2Id = 3;
//                    }
//                }
//                else
//                {
//                    if (mtdA.Z > 0)
//                    {
//                        //-+
//                        edgeA1.X = -aHalfWidth;
//                        edgeA1.Y = -aHalfHeight;
//                        edgeA1.Z = aHalfLength;

//                        edgeA2.X = -aHalfWidth;
//                        edgeA2.Y = aHalfHeight;
//                        edgeA2.Z = aHalfLength;

//                        edgeA1Id = 4;
//                        edgeA2Id = 6;
//                    }
//                    else
//                    {
//                        //--
//                        edgeA1.X = -aHalfWidth;
//                        edgeA1.Y = -aHalfHeight;
//                        edgeA1.Z = -aHalfLength;

//                        edgeA2.X = -aHalfWidth;
//                        edgeA2.Y = aHalfHeight;
//                        edgeA2.Z = -aHalfLength;

//                        edgeA1Id = 0;
//                        edgeA2Id = 2;
//                    }
//                }
//            }
//            else
//            {
//                //mtd is in the X-Y plane
//                if (mtdA.X > 0)
//                {
//                    if (mtdA.Y > 0)
//                    {
//                        //++
//                        edgeA1.X = aHalfWidth;
//                        edgeA1.Y = aHalfHeight;
//                        edgeA1.Z = -aHalfLength;

//                        edgeA2.X = aHalfWidth;
//                        edgeA2.Y = aHalfHeight;
//                        edgeA2.Z = aHalfLength;

//                        edgeA1Id = 3;
//                        edgeA2Id = 7;
//                    }
//                    else
//                    {
//                        //+-
//                        edgeA1.X = aHalfWidth;
//                        edgeA1.Y = -aHalfHeight;
//                        edgeA1.Z = -aHalfLength;

//                        edgeA2.X = aHalfWidth;
//                        edgeA2.Y = -aHalfHeight;
//                        edgeA2.Z = aHalfLength;

//                        edgeA1Id = 1;
//                        edgeA2Id = 5;
//                    }
//                }
//                else
//                {
//                    if (mtdA.Y > 0)
//                    {
//                        //-+
//                        edgeA1.X = -aHalfWidth;
//                        edgeA1.Y = aHalfHeight;
//                        edgeA1.Z = -aHalfLength;

//                        edgeA2.X = -aHalfWidth;
//                        edgeA2.Y = aHalfHeight;
//                        edgeA2.Z = aHalfLength;

//                        edgeA1Id = 2;
//                        edgeA2Id = 6;
//                    }
//                    else
//                    {
//                        //--
//                        edgeA1.X = -aHalfWidth;
//                        edgeA1.Y = -aHalfHeight;
//                        edgeA1.Z = -aHalfLength;

//                        edgeA2.X = -aHalfWidth;
//                        edgeA2.Y = -aHalfHeight;
//                        edgeA2.Z = aHalfLength;

//                        edgeA1Id = 0;
//                        edgeA2Id = 4;
//                    }
//                }
//            }

//            #endregion

//            #region Edge B

//            if (Math.Abs(mtdB.X) < Toolbox.Epsilon)
//            {
//                //mtd is in the Y-Z plane.
//                if (mtdB.Y > 0)
//                {
//                    if (mtdB.Z > 0)
//                    {
//                        //++
//                        edgeB1.X = -bHalfWidth;
//                        edgeB1.Y = bHalfHeight;
//                        edgeB1.Z = bHalfLength;

//                        edgeB2.X = bHalfWidth;
//                        edgeB2.Y = bHalfHeight;
//                        edgeB2.Z = bHalfLength;

//                        edgeB1Id = 6;
//                        edgeB2Id = 7;
//                    }
//                    else
//                    {
//                        //+-
//                        edgeB1.X = -bHalfWidth;
//                        edgeB1.Y = bHalfHeight;
//                        edgeB1.Z = -bHalfLength;

//                        edgeB2.X = bHalfWidth;
//                        edgeB2.Y = bHalfHeight;
//                        edgeB2.Z = -bHalfLength;

//                        edgeB1Id = 2;
//                        edgeB2Id = 3;
//                    }
//                }
//                else
//                {
//                    if (mtdB.Z > 0)
//                    {
//                        //-+
//                        edgeB1.X = -bHalfWidth;
//                        edgeB1.Y = -bHalfHeight;
//                        edgeB1.Z = bHalfLength;

//                        edgeB2.X = bHalfWidth;
//                        edgeB2.Y = -bHalfHeight;
//                        edgeB2.Z = bHalfLength;

//                        edgeB1Id = 4;
//                        edgeB2Id = 5;
//                    }
//                    else
//                    {
//                        //--
//                        edgeB1.X = -bHalfWidth;
//                        edgeB1.Y = -bHalfHeight;
//                        edgeB1.Z = -bHalfLength;

//                        edgeB2.X = bHalfWidth;
//                        edgeB2.Y = -bHalfHeight;
//                        edgeB2.Z = -bHalfLength;

//                        edgeB1Id = 0;
//                        edgeB2Id = 1;
//                    }
//                }
//            }
//            else if (Math.Abs(mtdB.Y) < Toolbox.Epsilon)
//            {
//                //mtd is in the X-Z plane
//                if (mtdB.X > 0)
//                {
//                    if (mtdB.Z > 0)
//                    {
//                        //++
//                        edgeB1.X = bHalfWidth;
//                        edgeB1.Y = -bHalfHeight;
//                        edgeB1.Z = bHalfLength;

//                        edgeB2.X = bHalfWidth;
//                        edgeB2.Y = bHalfHeight;
//                        edgeB2.Z = bHalfLength;

//                        edgeB1Id = 5;
//                        edgeB2Id = 7;
//                    }
//                    else
//                    {
//                        //+-
//                        edgeB1.X = bHalfWidth;
//                        edgeB1.Y = -bHalfHeight;
//                        edgeB1.Z = -bHalfLength;

//                        edgeB2.X = bHalfWidth;
//                        edgeB2.Y = bHalfHeight;
//                        edgeB2.Z = -bHalfLength;

//                        edgeB1Id = 1;
//                        edgeB2Id = 3;
//                    }
//                }
//                else
//                {
//                    if (mtdB.Z > 0)
//                    {
//                        //-+
//                        edgeB1.X = -bHalfWidth;
//                        edgeB1.Y = -bHalfHeight;
//                        edgeB1.Z = bHalfLength;

//                        edgeB2.X = -bHalfWidth;
//                        edgeB2.Y = bHalfHeight;
//                        edgeB2.Z = bHalfLength;

//                        edgeB1Id = 4;
//                        edgeB2Id = 6;
//                    }
//                    else
//                    {
//                        //--
//                        edgeB1.X = -bHalfWidth;
//                        edgeB1.Y = -bHalfHeight;
//                        edgeB1.Z = -bHalfLength;

//                        edgeB2.X = -bHalfWidth;
//                        edgeB2.Y = bHalfHeight;
//                        edgeB2.Z = -bHalfLength;

//                        edgeB1Id = 0;
//                        edgeB2Id = 2;
//                    }
//                }
//            }
//            else
//            {
//                //mtd is in the X-Y plane
//                if (mtdB.X > 0)
//                {
//                    if (mtdB.Y > 0)
//                    {
//                        //++
//                        edgeB1.X = bHalfWidth;
//                        edgeB1.Y = bHalfHeight;
//                        edgeB1.Z = -bHalfLength;

//                        edgeB2.X = bHalfWidth;
//                        edgeB2.Y = bHalfHeight;
//                        edgeB2.Z = bHalfLength;

//                        edgeB1Id = 3;
//                        edgeB2Id = 7;
//                    }
//                    else
//                    {
//                        //+-
//                        edgeB1.X = bHalfWidth;
//                        edgeB1.Y = -bHalfHeight;
//                        edgeB1.Z = -bHalfLength;

//                        edgeB2.X = bHalfWidth;
//                        edgeB2.Y = -bHalfHeight;
//                        edgeB2.Z = bHalfLength;

//                        edgeB1Id = 1;
//                        edgeB2Id = 5;
//                    }
//                }
//                else
//                {
//                    if (mtdB.Y > 0)
//                    {
//                        //-+
//                        edgeB1.X = -bHalfWidth;
//                        edgeB1.Y = bHalfHeight;
//                        edgeB1.Z = -bHalfLength;

//                        edgeB2.X = -bHalfWidth;
//                        edgeB2.Y = bHalfHeight;
//                        edgeB2.Z = bHalfLength;

//                        edgeB1Id = 2;
//                        edgeB2Id = 6;
//                    }
//                    else
//                    {
//                        //--
//                        edgeB1.X = -bHalfWidth;
//                        edgeB1.Y = -bHalfHeight;
//                        edgeB1.Z = -bHalfLength;

//                        edgeB2.X = -bHalfWidth;
//                        edgeB2.Y = -bHalfHeight;
//                        edgeB2.Z = bHalfLength;

//                        edgeB1Id = 0;
//                        edgeB2Id = 4;
//                    }
//                }
//            }

//            #endregion

//            //TODO: Since the above uniquely identifies the edge from each box based on two vertices,
//            //get the edge feature id from vertexA id combined with vertexB id.
//            //Vertex id's are 3 bit binary 'numbers' because ---, --+, -+-, etc.


//            Matrix3X3.Transform(ref edgeA1, ref aOrientation, out edgeA1);
//            Matrix3X3.Transform(ref edgeA2, ref aOrientation, out edgeA2);
//            Matrix3X3.Transform(ref edgeB1, ref bOrientation, out edgeB1);
//            Matrix3X3.Transform(ref edgeB2, ref bOrientation, out edgeB2);
//            Vector3.Add(ref edgeA1, ref transformA.Position, out edgeA1);
//            Vector3.Add(ref edgeA2, ref transformA.Position, out edgeA2);
//            Vector3.Add(ref edgeB1, ref transformB.Position, out edgeB1);
//            Vector3.Add(ref edgeB2, ref transformB.Position, out edgeB2);

//            float s, t;
//            Vector3 onA, onB;
//            Toolbox.GetClosestPointsBetweenSegments(ref edgeA1, ref edgeA2, ref edgeB1, ref edgeB2, out s, out t, out onA, out onB);
//            //Vector3.Add(ref onA, ref onB, out point);
//            //Vector3.Multiply(ref point, .5f, out point);
//            point = onA;

//            depth = (onB.X - onA.X) * mtd.X + (onB.Y - onA.Y) * mtd.Y + (onB.Z - onA.Z) * mtd.Z;

//            id = GetContactId(edgeA1Id, edgeA2Id, edgeB1Id, edgeB2Id);
//        }

//#if ALLOWUNSAFE
//        internal static void GetFaceContacts(BoxShape a, BoxShape b, ref Matrix3X3 orientationMatrixA, ref Matrix3X3 orientationMatrixB, ref Vector3 positionA, ref Vector3 positionB, bool aIsFaceOwner, ref Vector3 mtd, out BoxContactDataCache contactData)
//#else
//        internal static void GetFaceContacts(Box a, Box b, bool aIsFaceOwner, ref Vector3 mtd, out TinyStructList<BoxContactData> contactData)
//#endif
//        {
//            float aHalfWidth = a.halfWidth;
//            float aHalfHeight = a.halfHeight;
//            float aHalfLength = a.halfLength;

//            float bHalfWidth = b.halfWidth;
//            float bHalfHeight = b.halfHeight;
//            float bHalfLength = b.halfLength;

//            BoxFace aBoxFace, bBoxFace;

//            Vector3 negatedMtd;
//            Vector3.Negate(ref mtd, out negatedMtd);
//            GetNearestFace(a, ref orientationMatrixA, ref positionA, ref negatedMtd, aHalfWidth, aHalfHeight, aHalfLength, out aBoxFace);


//            GetNearestFace(b, ref orientationMatrixB, ref positionB, ref mtd, bHalfWidth, bHalfHeight, bHalfLength, out bBoxFace);

//            if (aIsFaceOwner)
//                ClipFacesDirect(ref aBoxFace, ref bBoxFace, ref negatedMtd, out contactData);
//            else
//                ClipFacesDirect(ref bBoxFace, ref aBoxFace, ref mtd, out contactData);

//#if ALLOWUNSAFE
//            if (contactData.Count > 4)
//#else
//            if (contactData.Count > 4)
//#endif
//                PruneContactsMaxDistance(ref mtd, contactData, out contactData);
//        }

//#if ALLOWUNSAFE
//        private static unsafe void PruneContactsMaxDistance(ref Vector3 mtd, BoxContactDataCache input, out BoxContactDataCache output)
//        {
//            BoxContactData* data = &input.D1;
//            int count = input.Count;
//            Vector3 v;
//            var maximumOffset = new Vector3();
//            int maxIndexA = -1, maxIndexB = -1;
//            float temp;
//            float maximumDistanceSquared = -float.MaxValue;
//            for (int i = 0; i < count; i++)
//            {
//                for (int j = i + 1; j < count; j++)
//                {
//                    Vector3.Subtract(ref data[j].Position, ref data[i].Position, out v);
//                    temp = v.LengthSquared();
//                    if (temp > maximumDistanceSquared)
//                    {
//                        maximumDistanceSquared = temp;
//                        maxIndexA = i;
//                        maxIndexB = j;
//                        maximumOffset = v;
//                    }
//                }
//            }

//            Vector3 otherDirection;
//            Vector3.Cross(ref mtd, ref maximumOffset, out otherDirection);
//            int minimumIndex = -1, maximumIndex = -1;
//            float minimumDistance = float.MaxValue, maximumDistance = -float.MaxValue;

//            for (int i = 0; i < count; i++)
//            {
//                if (i != maxIndexA && i != maxIndexB)
//                {
//                    Vector3.Dot(ref data[i].Position, ref otherDirection, out temp);
//                    if (temp > maximumDistance)
//                    {
//                        maximumDistance = temp;
//                        maximumIndex = i;
//                    }
//                    if (temp < minimumDistance)
//                    {
//                        minimumDistance = temp;
//                        minimumIndex = i;
//                    }
//                }
//            }

//            output = new BoxContactDataCache();
//            output.Count = 4;
//            output.D1 = data[maxIndexA];
//            output.D2 = data[maxIndexB];
//            output.D3 = data[minimumIndex];
//            output.D4 = data[maximumIndex];
//        }
//#else
//        private static void PruneContactsMaxDistance(ref Vector3 mtd, TinyStructList<BoxContactData> input, out TinyStructList<BoxContactData> output)
//        {
//            int count = input.Count;
//            Vector3 v;
//            var maximumOffset = new Vector3();
//            int maxIndexA = -1, maxIndexB = -1;
//            float temp;
//            float maximumDistanceSquared = -float.MaxValue;
//            BoxContactData itemA, itemB;
//            for (int i = 0; i < count; i++)
//            {
//                for (int j = i + 1; j < count; j++)
//                {
//                    input.Get(j, out itemB);
//                    input.Get(i, out itemA);
//                    Vector3.Subtract(ref itemB.Position, ref itemA.Position, out v);
//                    temp = v.LengthSquared();
//                    if (temp > maximumDistanceSquared)
//                    {
//                        maximumDistanceSquared = temp;
//                        maxIndexA = i;
//                        maxIndexB = j;
//                        maximumOffset = v;
//                    }
//                }
//            }

//            Vector3 otherDirection;
//            Vector3.Cross(ref mtd, ref maximumOffset, out otherDirection);
//            int minimumIndex = -1, maximumIndex = -1;
//            float minimumDistance = float.MaxValue, maximumDistance = -float.MaxValue;

//            for (int i = 0; i < count; i++)
//            {
//                if (i != maxIndexA && i != maxIndexB)
//                {
//                    input.Get(i, out itemA);
//                    Vector3.Dot(ref itemA.Position, ref otherDirection, out temp);
//                    if (temp > maximumDistance)
//                    {
//                        maximumDistance = temp;
//                        maximumIndex = i;
//                    }
//                    if (temp < minimumDistance)
//                    {
//                        minimumDistance = temp;
//                        minimumIndex = i;
//                    }
//                }
//            }

//            output = new TinyStructList<BoxContactData>();
//            input.Get(maxIndexA, out itemA);
//            output.Add(ref itemA);
//            input.Get(maxIndexB, out itemA);
//            output.Add(ref itemA);
//            input.Get(minimumIndex, out itemA);
//            output.Add(ref itemA);
//            input.Get(maximumIndex, out itemA);
//            output.Add(ref itemA);
//        }
//#endif
//#if EXCLUDED
//        private static unsafe void clipFacesSH(ref BoxFace clipFace, ref BoxFace face, ref Vector3 mtd, out ContactDataCache outputData)
//        {
//            ContactDataCache contactDataCache = new ContactDataCache();
//            BoxContactData* data = &contactDataCache.d1;

//            //Set up the initial face list.
//            data[0].position = face.v1;
//            data[0].id = face.id1;
//            data[1].position = face.v2;
//            data[1].id = face.id2;
//            data[2].position = face.v3;
//            data[2].id = face.id3;
//            data[3].position = face.v4;
//            data[3].id = face.id4;
//            contactDataCache.count = 4;

//            ContactDataCache temporaryCache;
//            BoxContactData* temp = &temporaryCache.d1;
//            FaceEdge clippingEdge;
//            Vector3 intersection;
//            for (int i = 0; i < 4; i++)
//            {//For each clipping edge (edges of face a)

//                clipFace.GetEdge(i, ref mtd, out clippingEdge);

//                temporaryCache = contactDataCache;

//                contactDataCache.count = 0;

//                Vector3 start = temp[temporaryCache.count - 1].position;
//                int startId = temp[temporaryCache.count - 1].id;


//                for (int j = 0; j < temporaryCache.count; j++)
//                {//For each point in the input list
//                    Vector3 end = temp[j].position;
//                    int endId = temp[j].id;
//                    if (clippingEdge.isPointInside(ref end))
//                    {
//                        if (!clippingEdge.isPointInside(ref start))
//                        {
//                            ComputeIntersection(ref start, ref end, ref mtd, ref clippingEdge, out intersection);
//                            if (contactDataCache.count < 8)
//                            {
//                                data[contactDataCache.count].position = intersection;
//                                data[contactDataCache.count].id = GetContactId(startId, endId, ref clippingEdge);
//                                contactDataCache.count++;
//                            }
//                            else
//                            {
//                                data[contactDataCache.count - 1].position = intersection;
//                                data[contactDataCache.count - 1].id = GetContactId(startId, endId, ref clippingEdge);
//                            }
//                        }
//                        if (contactDataCache.count < 8)
//                        {
//                            data[contactDataCache.count].position = end;
//                            data[contactDataCache.count].id = endId;
//                            contactDataCache.count++;
//                        }
//                        else
//                        {
//                            data[contactDataCache.count - 1].position = end;
//                            data[contactDataCache.count - 1].id = endId;
//                        }
//                    }
//                    else if (clippingEdge.isPointInside(ref start))
//                    {
//                        ComputeIntersection(ref start, ref end, ref mtd, ref clippingEdge, out intersection);
//                        if (contactDataCache.count < 8)
//                        {
//                            data[contactDataCache.count].position = intersection;
//                            data[contactDataCache.count].id = GetContactId(startId, endId, ref clippingEdge);
//                            contactDataCache.count++;
//                        }
//                        else
//                        {
//                            data[contactDataCache.count - 1].position = intersection;
//                            data[contactDataCache.count - 1].id = GetContactId(startId, endId, ref clippingEdge);
//                        }
//                    }
//                    start = end;
//                    startId = endId;
//                }
//            }
//            temporaryCache = contactDataCache;
//            contactDataCache.count = 0;

//            float depth;
//            float a, b;
//            Vector3.Dot(ref clipFace.v1, ref mtd, out a);
//            for (int i = 0; i < temporaryCache.count; i++)
//            {
//                Vector3.Dot(ref temp[i].position, ref mtd, out b);
//                depth = b - a;
//                if (depth < 0)
//                {
//                    data[contactDataCache.count].position = temp[i].position;
//                    data[contactDataCache.count].id = temp[i].id;
//                    contactDataCache.count++;
//                }
//            }

//            outputData = contactDataCache;

//            /*
//             * 
//  List outputList = subjectPolygon;
//  for (Edge clipEdge in clipPolygon) do
//     List inputList = outputList;
//     outputList.clear();
//     Point S = inputList.last;
//     for (Point E in inputList) do
//        if (E inside clipEdge) then
//           if (S not inside clipEdge) then
//              outputList.add(ComputeIntersection(S,E,clipEdge));
//           end if
//           outputList.add(E);
//        else if (S inside clipEdge) then
//           outputList.add(ComputeIntersection(S,E,clipEdge));
//        end if
//        S = E;
//     done
//  done
//             */

//        }
//#endif

//#if ALLOWUNSAFE
//        private static unsafe void ClipFacesDirect(ref BoxFace clipFace, ref BoxFace face, ref Vector3 mtd, out BoxContactDataCache outputData)
//        {
//            var contactData = new BoxContactDataCache();
//            BoxContactDataCache tempData; //Local version.
//            BoxContactData* data = &contactData.D1;
//            BoxContactData* temp = &tempData.D1;

//            //Local directions on the clip face.  Their length is equal to the length of an edge.
//            Vector3 clipX, clipY;
//            Vector3.Subtract(ref clipFace.V4, ref clipFace.V3, out clipX);
//            Vector3.Subtract(ref clipFace.V2, ref clipFace.V3, out clipY);
//            float inverse = 1 / clipX.LengthSquared();
//            clipX.X *= inverse;
//            clipX.Y *= inverse;
//            clipX.Z *= inverse;
//            inverse = 1 / clipY.LengthSquared();
//            clipY.X *= inverse;
//            clipY.Y *= inverse;
//            clipY.Z *= inverse;

//            //Local directions on the opposing face.  Their length is equal to the length of an edge.
//            Vector3 faceX, faceY;
//            Vector3.Subtract(ref face.V4, ref face.V3, out faceX);
//            Vector3.Subtract(ref face.V2, ref face.V3, out faceY);
//            inverse = 1 / faceX.LengthSquared();
//            faceX.X *= inverse;
//            faceX.Y *= inverse;
//            faceX.Z *= inverse;
//            inverse = 1 / faceY.LengthSquared();
//            faceY.X *= inverse;
//            faceY.Y *= inverse;
//            faceY.Z *= inverse;

//            Vector3 clipCenter;
//            Vector3.Add(ref clipFace.V1, ref clipFace.V3, out clipCenter);
//            //Defer division until after dot product (2 multiplies instead of 3)
//            float clipCenterX, clipCenterY;
//            Vector3.Dot(ref clipCenter, ref clipX, out clipCenterX);
//            Vector3.Dot(ref clipCenter, ref clipY, out clipCenterY);
//            clipCenterX *= .5f;
//            clipCenterY *= .5f;

//            Vector3 faceCenter;
//            Vector3.Add(ref face.V1, ref face.V3, out faceCenter);
//            //Defer division until after dot product (2 multiplies instead of 3)
//            float faceCenterX, faceCenterY;
//            Vector3.Dot(ref faceCenter, ref faceX, out faceCenterX);
//            Vector3.Dot(ref faceCenter, ref faceY, out faceCenterY);
//            faceCenterX *= .5f;
//            faceCenterY *= .5f;

//            //To test bounds, recall that clipX is the length of the X edge.
//            //Going from the center to the max or min goes half of the length of X edge, or +/- 0.5.
//            //Bias could be added here.
//            const float extent = .51f; //.5f is the default, the extra is for robustness.
//            float clipCenterMaxX = clipCenterX + extent;
//            float clipCenterMaxY = clipCenterY + extent;
//            float clipCenterMinX = clipCenterX - extent;
//            float clipCenterMinY = clipCenterY - extent;

//            float faceCenterMaxX = faceCenterX + extent;
//            float faceCenterMaxY = faceCenterY + extent;
//            float faceCenterMinX = faceCenterX - extent;
//            float faceCenterMinY = faceCenterY - extent;

//            //Find out where the opposing face is.
//            float dotX, dotY;

//            //The four edges can be thought of as minX, maxX, minY and maxY.

//            //Face v1
//            Vector3.Dot(ref clipX, ref face.V1, out dotX);
//            bool v1MaxXInside = dotX < clipCenterMaxX;
//            bool v1MinXInside = dotX > clipCenterMinX;
//            Vector3.Dot(ref clipY, ref face.V1, out dotY);
//            bool v1MaxYInside = dotY < clipCenterMaxY;
//            bool v1MinYInside = dotY > clipCenterMinY;

//            //Face v2
//            Vector3.Dot(ref clipX, ref face.V2, out dotX);
//            bool v2MaxXInside = dotX < clipCenterMaxX;
//            bool v2MinXInside = dotX > clipCenterMinX;
//            Vector3.Dot(ref clipY, ref face.V2, out dotY);
//            bool v2MaxYInside = dotY < clipCenterMaxY;
//            bool v2MinYInside = dotY > clipCenterMinY;

//            //Face v3
//            Vector3.Dot(ref clipX, ref face.V3, out dotX);
//            bool v3MaxXInside = dotX < clipCenterMaxX;
//            bool v3MinXInside = dotX > clipCenterMinX;
//            Vector3.Dot(ref clipY, ref face.V3, out dotY);
//            bool v3MaxYInside = dotY < clipCenterMaxY;
//            bool v3MinYInside = dotY > clipCenterMinY;

//            //Face v4
//            Vector3.Dot(ref clipX, ref face.V4, out dotX);
//            bool v4MaxXInside = dotX < clipCenterMaxX;
//            bool v4MinXInside = dotX > clipCenterMinX;
//            Vector3.Dot(ref clipY, ref face.V4, out dotY);
//            bool v4MaxYInside = dotY < clipCenterMaxY;
//            bool v4MinYInside = dotY > clipCenterMinY;

//            //Find out where the clip face is.
//            //Clip v1
//            Vector3.Dot(ref faceX, ref clipFace.V1, out dotX);
//            bool clipv1MaxXInside = dotX < faceCenterMaxX;
//            bool clipv1MinXInside = dotX > faceCenterMinX;
//            Vector3.Dot(ref faceY, ref clipFace.V1, out dotY);
//            bool clipv1MaxYInside = dotY < faceCenterMaxY;
//            bool clipv1MinYInside = dotY > faceCenterMinY;

//            //Clip v2
//            Vector3.Dot(ref faceX, ref clipFace.V2, out dotX);
//            bool clipv2MaxXInside = dotX < faceCenterMaxX;
//            bool clipv2MinXInside = dotX > faceCenterMinX;
//            Vector3.Dot(ref faceY, ref clipFace.V2, out dotY);
//            bool clipv2MaxYInside = dotY < faceCenterMaxY;
//            bool clipv2MinYInside = dotY > faceCenterMinY;

//            //Clip v3
//            Vector3.Dot(ref faceX, ref clipFace.V3, out dotX);
//            bool clipv3MaxXInside = dotX < faceCenterMaxX;
//            bool clipv3MinXInside = dotX > faceCenterMinX;
//            Vector3.Dot(ref faceY, ref clipFace.V3, out dotY);
//            bool clipv3MaxYInside = dotY < faceCenterMaxY;
//            bool clipv3MinYInside = dotY > faceCenterMinY;

//            //Clip v4
//            Vector3.Dot(ref faceX, ref clipFace.V4, out dotX);
//            bool clipv4MaxXInside = dotX < faceCenterMaxX;
//            bool clipv4MinXInside = dotX > faceCenterMinX;
//            Vector3.Dot(ref faceY, ref clipFace.V4, out dotY);
//            bool clipv4MaxYInside = dotY < faceCenterMaxY;
//            bool clipv4MinYInside = dotY > faceCenterMinY;

//            #region Face Vertices

//            if (v1MinXInside && v1MaxXInside && v1MinYInside && v1MaxYInside)
//            {
//                data[contactData.Count].Position = face.V1;
//                data[contactData.Count].Id = face.Id1;
//                contactData.Count++;
//            }

//            if (v2MinXInside && v2MaxXInside && v2MinYInside && v2MaxYInside)
//            {
//                data[contactData.Count].Position = face.V2;
//                data[contactData.Count].Id = face.Id2;
//                contactData.Count++;
//            }

//            if (v3MinXInside && v3MaxXInside && v3MinYInside && v3MaxYInside)
//            {
//                data[contactData.Count].Position = face.V3;
//                data[contactData.Count].Id = face.Id3;
//                contactData.Count++;
//            }

//            if (v4MinXInside && v4MaxXInside && v4MinYInside && v4MaxYInside)
//            {
//                data[contactData.Count].Position = face.V4;
//                data[contactData.Count].Id = face.Id4;
//                contactData.Count++;
//            }

//            #endregion

//            //Compute depths.
//            tempData = contactData;
//            contactData.Count = 0;
//            float depth;
//            float clipFaceDot, faceDot;
//            Vector3.Dot(ref clipFace.V1, ref mtd, out clipFaceDot);
//            for (int i = 0; i < tempData.Count; i++)
//            {
//                Vector3.Dot(ref temp[i].Position, ref mtd, out faceDot);
//                depth = faceDot - clipFaceDot;
//                if (depth < 0)
//                {
//                    data[contactData.Count].Position = temp[i].Position;
//                    data[contactData.Count].Depth = depth;
//                    data[contactData.Count].Id = temp[i].Id;
//                    contactData.Count++;
//                }
//            }

//            byte previousCount = contactData.Count;
//            if (previousCount >= 4) //Early finish :)
//            {
//                outputData = contactData;
//                return;
//            }

//            #region Clip face vertices

//            Vector3 faceNormal;
//            Vector3.Cross(ref faceY, ref faceX, out faceNormal);
//            //inverse = 1 / faceNormal.LengthSquared();
//            //faceNormal.X *= inverse;
//            //faceNormal.Y *= inverse;
//            //faceNormal.Z *= inverse;
//            faceNormal.Normalize();
//            Vector3 v;
//            float a, b;
//            Vector3.Dot(ref face.V1, ref faceNormal, out b);
//            //CLIP FACE
//            if (clipv1MinXInside && clipv1MaxXInside && clipv1MinYInside && clipv1MaxYInside)
//            {
//                Vector3.Dot(ref clipFace.V1, ref faceNormal, out a);
//                Vector3.Multiply(ref faceNormal, a - b, out v);
//                Vector3.Subtract(ref clipFace.V1, ref v, out v);
//                data[contactData.Count].Position = v;
//                data[contactData.Count].Id = clipFace.Id1 + 8;
//                contactData.Count++;
//            }

//            if (clipv2MinXInside && clipv2MaxXInside && clipv2MinYInside && clipv2MaxYInside)
//            {
//                Vector3.Dot(ref clipFace.V2, ref faceNormal, out a);
//                Vector3.Multiply(ref faceNormal, a - b, out v);
//                Vector3.Subtract(ref clipFace.V2, ref v, out v);
//                data[contactData.Count].Position = v;
//                data[contactData.Count].Id = clipFace.Id2 + 8;
//                contactData.Count++;
//            }

//            if (clipv3MinXInside && clipv3MaxXInside && clipv3MinYInside && clipv3MaxYInside)
//            {
//                Vector3.Dot(ref clipFace.V3, ref faceNormal, out a);
//                Vector3.Multiply(ref faceNormal, a - b, out v);
//                Vector3.Subtract(ref clipFace.V3, ref v, out v);
//                data[contactData.Count].Position = v;
//                data[contactData.Count].Id = clipFace.Id3 + 8;
//                contactData.Count++;
//            }

//            if (clipv4MinXInside && clipv4MaxXInside && clipv4MinYInside && clipv4MaxYInside)
//            {
//                Vector3.Dot(ref clipFace.V4, ref faceNormal, out a);
//                Vector3.Multiply(ref faceNormal, a - b, out v);
//                Vector3.Subtract(ref clipFace.V4, ref v, out v);
//                data[contactData.Count].Position = v;
//                data[contactData.Count].Id = clipFace.Id4 + 8;
//                contactData.Count++;
//            }

//            #endregion

//            //Compute depths.
//            byte postClipCount = contactData.Count;
//            tempData = contactData;
//            contactData.Count = previousCount;

//            for (int i = previousCount; i < tempData.Count; i++)
//            {
//                Vector3.Dot(ref temp[i].Position, ref mtd, out faceDot);
//                depth = faceDot - clipFaceDot;
//                if (depth < 0)
//                {
//                    data[contactData.Count].Position = temp[i].Position;
//                    data[contactData.Count].Depth = depth;
//                    data[contactData.Count].Id = temp[i].Id;
//                    contactData.Count++;
//                }
//            }

//            previousCount = contactData.Count;
//            if (previousCount >= 4) //Early finish :)
//            {
//                outputData = contactData;
//                return;
//            }

//            //Intersect edges.

//            //maxX maxY -> v1
//            //minX maxY -> v2
//            //minX minY -> v3
//            //maxX minY -> v4

//            //Once we get here there can only be 3 contacts or less.
//            //Once 4 possible contacts have been added, switch to using safe increments.
//            float dot;

//            #region CLIP EDGE: v1 v2

//            FaceEdge clipEdge;
//            clipFace.GetEdge(0, ref mtd, out clipEdge);
//            if (!v1MaxYInside)
//            {
//                if (v2MaxYInside)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v4MaxYInside)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v2MaxYInside)
//            {
//                if (v1MaxYInside)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v3MaxYInside)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v3MaxYInside)
//            {
//                if (v2MaxYInside)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v4MaxYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v4MaxYInside)
//            {
//                if (v1MaxYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v3MaxYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }

//            #endregion

//            #region CLIP EDGE: v2 v3

//            clipFace.GetEdge(1, ref mtd, out clipEdge);
//            if (!v1MinXInside)
//            {
//                if (v2MinXInside && contactData.Count < 8)
//                {
//                    //test v1-v2 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v4MinXInside && contactData.Count < 8)
//                {
//                    //test v1-v3 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v2MinXInside)
//            {
//                if (v1MinXInside && contactData.Count < 8)
//                {
//                    //test v1-v2 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v3MinXInside && contactData.Count < 8)
//                {
//                    //test v2-v4 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v3MinXInside)
//            {
//                if (v2MinXInside && contactData.Count < 8)
//                {
//                    //test v1-v3 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v4MinXInside && contactData.Count < 8)
//                {
//                    //test v3-v4 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v4MinXInside)
//            {
//                if (v1MinXInside && contactData.Count < 8)
//                {
//                    //test v2-v4 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v3MinXInside && contactData.Count < 8)
//                {
//                    //test v3-v4 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }

//            #endregion

//            #region CLIP EDGE: v3 v4

//            clipFace.GetEdge(2, ref mtd, out clipEdge);
//            if (!v1MinYInside)
//            {
//                if (v2MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v4MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v2MinYInside)
//            {
//                if (v1MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v3MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v3MinYInside)
//            {
//                if (v2MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v4MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v4MinYInside)
//            {
//                if (v3MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v1MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }

//            #endregion

//            #region CLIP EDGE: v4 v1

//            clipFace.GetEdge(3, ref mtd, out clipEdge);
//            if (!v1MaxXInside)
//            {
//                if (v2MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v4MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v2MaxXInside)
//            {
//                if (v1MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v3MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v3MaxXInside)
//            {
//                if (v2MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v4MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }
//            if (!v4MaxXInside)
//            {
//                if (v1MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//                if (v3MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        data[contactData.Count].Position = v;
//                        data[contactData.Count].Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Count++;
//                    }
//                }
//            }

//            #endregion

//            //Compute depths.
//            postClipCount = contactData.Count;
//            tempData = contactData;
//            contactData.Count = previousCount;

//            for (int i = previousCount; i < tempData.Count; i++)
//            {
//                Vector3.Dot(ref temp[i].Position, ref mtd, out faceDot);
//                depth = faceDot - clipFaceDot;
//                if (depth < 0)
//                {
//                    data[contactData.Count].Position = temp[i].Position;
//                    data[contactData.Count].Depth = depth;
//                    data[contactData.Count].Id = temp[i].Id;
//                    contactData.Count++;
//                }
//            }
//            outputData = contactData;
//        }
//#else
//        private static void ClipFacesDirect(ref BoxFace clipFace, ref BoxFace face, ref Vector3 mtd, out TinyStructList<BoxContactData> contactData)
//        {
//            contactData = new TinyStructList<BoxContactData>();
//            //BoxContactData* data = &contactData.d1;
//            //BoxContactData* temp = &tempData.d1;

//            //Local directions on the clip face.  Their length is equal to the length of an edge.
//            Vector3 clipX, clipY;
//            Vector3.Subtract(ref clipFace.V4, ref clipFace.V3, out clipX);
//            Vector3.Subtract(ref clipFace.V2, ref clipFace.V3, out clipY);
//            float inverse = 1/clipX.LengthSquared();
//            clipX.X *= inverse;
//            clipX.Y *= inverse;
//            clipX.Z *= inverse;
//            inverse = 1/clipY.LengthSquared();
//            clipY.X *= inverse;
//            clipY.Y *= inverse;
//            clipY.Z *= inverse;

//            //Local directions on the opposing face.  Their length is equal to the length of an edge.
//            Vector3 faceX, faceY;
//            Vector3.Subtract(ref face.V4, ref face.V3, out faceX);
//            Vector3.Subtract(ref face.V2, ref face.V3, out faceY);
//            inverse = 1/faceX.LengthSquared();
//            faceX.X *= inverse;
//            faceX.Y *= inverse;
//            faceX.Z *= inverse;
//            inverse = 1/faceY.LengthSquared();
//            faceY.X *= inverse;
//            faceY.Y *= inverse;
//            faceY.Z *= inverse;

//            Vector3 clipCenter;
//            Vector3.Add(ref clipFace.V1, ref clipFace.V3, out clipCenter);
//            //Defer division until after dot product (2 multiplies instead of 3)
//            float clipCenterX, clipCenterY;
//            Vector3.Dot(ref clipCenter, ref clipX, out clipCenterX);
//            Vector3.Dot(ref clipCenter, ref clipY, out clipCenterY);
//            clipCenterX *= .5f;
//            clipCenterY *= .5f;

//            Vector3 faceCenter;
//            Vector3.Add(ref face.V1, ref face.V3, out faceCenter);
//            //Defer division until after dot product (2 multiplies instead of 3)
//            float faceCenterX, faceCenterY;
//            Vector3.Dot(ref faceCenter, ref faceX, out faceCenterX);
//            Vector3.Dot(ref faceCenter, ref faceY, out faceCenterY);
//            faceCenterX *= .5f;
//            faceCenterY *= .5f;

//            //To test bounds, recall that clipX is the length of the X edge.
//            //Going from the center to the max or min goes half of the length of X edge, or +/- 0.5.
//            //Bias could be added here.
//            const float extent = .51f; //.5f is the default, the extra is for robustness.
//            float clipCenterMaxX = clipCenterX + extent;
//            float clipCenterMaxY = clipCenterY + extent;
//            float clipCenterMinX = clipCenterX - extent;
//            float clipCenterMinY = clipCenterY - extent;

//            float faceCenterMaxX = faceCenterX + extent;
//            float faceCenterMaxY = faceCenterY + extent;
//            float faceCenterMinX = faceCenterX - extent;
//            float faceCenterMinY = faceCenterY - extent;

//            //Find out where the opposing face is.
//            float dotX, dotY;

//            //The four edges can be thought of as minX, maxX, minY and maxY.

//            //Face v1
//            Vector3.Dot(ref clipX, ref face.V1, out dotX);
//            bool v1MaxXInside = dotX < clipCenterMaxX;
//            bool v1MinXInside = dotX > clipCenterMinX;
//            Vector3.Dot(ref clipY, ref face.V1, out dotY);
//            bool v1MaxYInside = dotY < clipCenterMaxY;
//            bool v1MinYInside = dotY > clipCenterMinY;

//            //Face v2
//            Vector3.Dot(ref clipX, ref face.V2, out dotX);
//            bool v2MaxXInside = dotX < clipCenterMaxX;
//            bool v2MinXInside = dotX > clipCenterMinX;
//            Vector3.Dot(ref clipY, ref face.V2, out dotY);
//            bool v2MaxYInside = dotY < clipCenterMaxY;
//            bool v2MinYInside = dotY > clipCenterMinY;

//            //Face v3
//            Vector3.Dot(ref clipX, ref face.V3, out dotX);
//            bool v3MaxXInside = dotX < clipCenterMaxX;
//            bool v3MinXInside = dotX > clipCenterMinX;
//            Vector3.Dot(ref clipY, ref face.V3, out dotY);
//            bool v3MaxYInside = dotY < clipCenterMaxY;
//            bool v3MinYInside = dotY > clipCenterMinY;

//            //Face v4
//            Vector3.Dot(ref clipX, ref face.V4, out dotX);
//            bool v4MaxXInside = dotX < clipCenterMaxX;
//            bool v4MinXInside = dotX > clipCenterMinX;
//            Vector3.Dot(ref clipY, ref face.V4, out dotY);
//            bool v4MaxYInside = dotY < clipCenterMaxY;
//            bool v4MinYInside = dotY > clipCenterMinY;

//            //Find out where the clip face is.
//            //Clip v1
//            Vector3.Dot(ref faceX, ref clipFace.V1, out dotX);
//            bool clipv1MaxXInside = dotX < faceCenterMaxX;
//            bool clipv1MinXInside = dotX > faceCenterMinX;
//            Vector3.Dot(ref faceY, ref clipFace.V1, out dotY);
//            bool clipv1MaxYInside = dotY < faceCenterMaxY;
//            bool clipv1MinYInside = dotY > faceCenterMinY;

//            //Clip v2
//            Vector3.Dot(ref faceX, ref clipFace.V2, out dotX);
//            bool clipv2MaxXInside = dotX < faceCenterMaxX;
//            bool clipv2MinXInside = dotX > faceCenterMinX;
//            Vector3.Dot(ref faceY, ref clipFace.V2, out dotY);
//            bool clipv2MaxYInside = dotY < faceCenterMaxY;
//            bool clipv2MinYInside = dotY > faceCenterMinY;

//            //Clip v3
//            Vector3.Dot(ref faceX, ref clipFace.V3, out dotX);
//            bool clipv3MaxXInside = dotX < faceCenterMaxX;
//            bool clipv3MinXInside = dotX > faceCenterMinX;
//            Vector3.Dot(ref faceY, ref clipFace.V3, out dotY);
//            bool clipv3MaxYInside = dotY < faceCenterMaxY;
//            bool clipv3MinYInside = dotY > faceCenterMinY;

//            //Clip v4
//            Vector3.Dot(ref faceX, ref clipFace.V4, out dotX);
//            bool clipv4MaxXInside = dotX < faceCenterMaxX;
//            bool clipv4MinXInside = dotX > faceCenterMinX;
//            Vector3.Dot(ref faceY, ref clipFace.V4, out dotY);
//            bool clipv4MaxYInside = dotY < faceCenterMaxY;
//            bool clipv4MinYInside = dotY > faceCenterMinY;

//            var item = new BoxContactData();

//        #region Face Vertices

//            if (v1MinXInside && v1MaxXInside && v1MinYInside && v1MaxYInside)
//            {
//                item.Position = face.V1;
//                item.Id = face.Id1;
//                contactData.Add(ref item);
//            }

//            if (v2MinXInside && v2MaxXInside && v2MinYInside && v2MaxYInside)
//            {
//                item.Position = face.V2;
//                item.Id = face.Id2;
//                contactData.Add(ref item);
//            }

//            if (v3MinXInside && v3MaxXInside && v3MinYInside && v3MaxYInside)
//            {
//                item.Position = face.V3;
//                item.Id = face.Id3;
//                contactData.Add(ref item);
//            }

//            if (v4MinXInside && v4MaxXInside && v4MinYInside && v4MaxYInside)
//            {
//                item.Position = face.V4;
//                item.Id = face.Id4;
//                contactData.Add(ref item);
//            }

//        #endregion

//            //Compute depths.
//            TinyStructList<BoxContactData> tempData = contactData;
//            contactData.Clear();
//            float clipFaceDot, faceDot;
//            Vector3.Dot(ref clipFace.V1, ref mtd, out clipFaceDot);
//            for (int i = 0; i < tempData.Count; i++)
//            {
//                tempData.Get(i, out item);
//                Vector3.Dot(ref item.Position, ref mtd, out faceDot);
//                item.Depth = faceDot - clipFaceDot;
//                if (item.Depth < 0)
//                {
//                    contactData.Add(ref item);
//                }
//            }

//            int previousCount = contactData.Count;
//            if (previousCount >= 4) //Early finish :)
//            {
//                return;
//            }

//        #region Clip face vertices

//            Vector3 faceNormal;
//            Vector3.Cross(ref faceY, ref faceX, out faceNormal);
//            //inverse = 1 / faceNormal.LengthSquared();
//            //faceNormal.X *= inverse;
//            //faceNormal.Y *= inverse;
//            //faceNormal.Z *= inverse;
//            faceNormal.Normalize();
//            Vector3 v;
//            float a, b;
//            Vector3.Dot(ref face.V1, ref faceNormal, out b);
//            //CLIP FACE
//            if (clipv1MinXInside && clipv1MaxXInside && clipv1MinYInside && clipv1MaxYInside)
//            {
//                Vector3.Dot(ref clipFace.V1, ref faceNormal, out a);
//                Vector3.Multiply(ref faceNormal, a - b, out v);
//                Vector3.Subtract(ref clipFace.V1, ref v, out v);
//                item.Position = v;
//                item.Id = clipFace.Id1 + 8;
//                contactData.Add(ref item);
//            }

//            if (clipv2MinXInside && clipv2MaxXInside && clipv2MinYInside && clipv2MaxYInside)
//            {
//                Vector3.Dot(ref clipFace.V2, ref faceNormal, out a);
//                Vector3.Multiply(ref faceNormal, a - b, out v);
//                Vector3.Subtract(ref clipFace.V2, ref v, out v);
//                item.Position = v;
//                item.Id = clipFace.Id2 + 8;
//                contactData.Add(ref item);
//            }

//            if (clipv3MinXInside && clipv3MaxXInside && clipv3MinYInside && clipv3MaxYInside)
//            {
//                Vector3.Dot(ref clipFace.V3, ref faceNormal, out a);
//                Vector3.Multiply(ref faceNormal, a - b, out v);
//                Vector3.Subtract(ref clipFace.V3, ref v, out v);
//                item.Position = v;
//                item.Id = clipFace.Id3 + 8;
//                contactData.Add(ref item);
//            }

//            if (clipv4MinXInside && clipv4MaxXInside && clipv4MinYInside && clipv4MaxYInside)
//            {
//                Vector3.Dot(ref clipFace.V4, ref faceNormal, out a);
//                Vector3.Multiply(ref faceNormal, a - b, out v);
//                Vector3.Subtract(ref clipFace.V4, ref v, out v);
//                item.Position = v;
//                item.Id = clipFace.Id4 + 8;
//                contactData.Add(ref item);
//            }

//        #endregion

//            //Compute depths.
//            int postClipCount = contactData.Count;
//            tempData = contactData;
//            for (int i = postClipCount - 1; i >= previousCount; i--) //TODO: >=?
//                contactData.RemoveAt(i);


//            for (int i = previousCount; i < tempData.Count; i++)
//            {
//                tempData.Get(i, out item);
//                Vector3.Dot(ref item.Position, ref mtd, out faceDot);
//                item.Depth = faceDot - clipFaceDot;
//                if (item.Depth < 0)
//                {
//                    contactData.Add(ref item);
//                }
//            }

//            previousCount = contactData.Count;
//            if (previousCount >= 4) //Early finish :)
//            {
//                return;
//            }

//            //Intersect edges.

//            //maxX maxY -> v1
//            //minX maxY -> v2
//            //minX minY -> v3
//            //maxX minY -> v4

//            //Once we get here there can only be 3 contacts or less.
//            //Once 4 possible contacts have been added, switch to using safe increments.
//            float dot;

//        #region CLIP EDGE: v1 v2

//            FaceEdge clipEdge;
//            clipFace.GetEdge(0, ref mtd, out clipEdge);
//            if (!v1MaxYInside)
//            {
//                if (v2MaxYInside)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v4MaxYInside)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v2MaxYInside)
//            {
//                if (v1MaxYInside)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v3MaxYInside)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v3MaxYInside)
//            {
//                if (v2MaxYInside)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v4MaxYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v4MaxYInside)
//            {
//                if (v1MaxYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v3MaxYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }

//        #endregion

//        #region CLIP EDGE: v2 v3

//            clipFace.GetEdge(1, ref mtd, out clipEdge);
//            if (!v1MinXInside)
//            {
//                if (v2MinXInside && contactData.Count < 8)
//                {
//                    //test v1-v2 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v4MinXInside && contactData.Count < 8)
//                {
//                    //test v1-v3 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v2MinXInside)
//            {
//                if (v1MinXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v3MinXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v3MinXInside)
//            {
//                if (v2MinXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v4MinXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v4MinXInside)
//            {
//                if (v1MinXInside && contactData.Count < 8)
//                {
//                    //test v2-v4 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v3MinXInside && contactData.Count < 8)
//                {
//                    //test v3-v4 against minXminY-minXmaxY
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }

//        #endregion

//        #region CLIP EDGE: v3 v4

//            clipFace.GetEdge(2, ref mtd, out clipEdge);
//            if (!v1MinYInside)
//            {
//                if (v2MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v4MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v2MinYInside)
//            {
//                if (v1MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v3MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v3MinYInside)
//            {
//                if (v2MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v4MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v4MinYInside)
//            {
//                if (v3MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v1MinYInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipX, ref v, out dot);
//                    if (dot > clipCenterMinX && dot < clipCenterMaxX)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }

//        #endregion

//        #region CLIP EDGE: v4 v1

//            clipFace.GetEdge(3, ref mtd, out clipEdge);
//            if (!v1MaxXInside)
//            {
//                if (v2MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v4MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v2MaxXInside)
//            {
//                if (v1MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V1, ref face.V2, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id1, face.Id2, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v3MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v3MaxXInside)
//            {
//                if (v2MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V2, ref face.V3, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id2, face.Id3, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v4MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }
//            if (!v4MaxXInside)
//            {
//                if (v1MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V4, ref face.V1, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id4, face.Id1, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//                if (v3MaxXInside && contactData.Count < 8)
//                {
//                    ComputeIntersection(ref face.V3, ref face.V4, ref clipEdge, out v);
//                    Vector3.Dot(ref clipY, ref v, out dot);
//                    if (dot > clipCenterMinY && dot < clipCenterMaxY)
//                    {
//                        item.Position = v;
//                        item.Id = GetContactId(face.Id3, face.Id4, ref clipEdge);
//                        contactData.Add(ref item);
//                    }
//                }
//            }

//        #endregion

//            //Compute depths.
//            postClipCount = contactData.Count;
//            tempData = contactData;
//            for (int i = postClipCount - 1; i >= previousCount; i--)
//                contactData.RemoveAt(i);

//            for (int i = previousCount; i < tempData.Count; i++)
//            {
//                tempData.Get(i, out item);
//                Vector3.Dot(ref item.Position, ref mtd, out faceDot);
//                item.Depth = faceDot - clipFaceDot;
//                if (item.Depth < 0)
//                {
//                    contactData.Add(ref item);
//                }
//            }
//        }
//#endif

//        private static void ComputeIntersection(ref Vector3 edgeA1, ref Vector3 edgeA2, ref FaceEdge clippingEdge, out Vector3 intersection)
//        {
//            //Intersect the incoming edge (edgeA1, edgeA2) with the clipping edge's PLANE.  Nicely given by one of its positions and its 'perpendicular,'
//            //which is its normal.

//            Vector3 offset;
//            Vector3.Subtract(ref clippingEdge.A, ref edgeA1, out offset);

//            Vector3 edgeDirection;
//            Vector3.Subtract(ref edgeA2, ref edgeA1, out edgeDirection);
//            float distanceToPlane;
//            Vector3.Dot(ref offset, ref clippingEdge.Perpendicular, out distanceToPlane);
//            float edgeDirectionLength;
//            Vector3.Dot(ref edgeDirection, ref clippingEdge.Perpendicular, out edgeDirectionLength);
//            Vector3.Multiply(ref edgeDirection, distanceToPlane / edgeDirectionLength, out offset);
//            Vector3.Add(ref offset, ref edgeA1, out intersection);
//        }

//        private static void GetNearestFace(BoxShape box, ref Matrix3X3 orientationMatrix, ref Vector3 position, ref Vector3 mtd, float halfWidth, float halfHeight, float halfLength, out BoxFace boxFace)
//        {
//            boxFace = new BoxFace();

//            float xDot = orientationMatrix.M11 * mtd.X +
//                         orientationMatrix.M12 * mtd.Y +
//                         orientationMatrix.M13 * mtd.Z;
//            float yDot = orientationMatrix.M21 * mtd.X +
//                         orientationMatrix.M22 * mtd.Y +
//                         orientationMatrix.M23 * mtd.Z;
//            float zDot = orientationMatrix.M31 * mtd.X +
//                         orientationMatrix.M32 * mtd.Y +
//                         orientationMatrix.M33 * mtd.Z;

//            float absX = Math.Abs(xDot);
//            float absY = Math.Abs(yDot);
//            float absZ = Math.Abs(zDot);

//            Matrix worldTransform;
//            Matrix3X3.ToMatrix4X4(ref orientationMatrix, out worldTransform);
//            worldTransform.M41 = position.X;
//            worldTransform.M42 = position.Y;
//            worldTransform.M43 = position.Z;

//            Vector3 candidate;
//            int bit;
//            if (absX > absY && absX > absZ)
//            {
//                //"X" faces are candidates
//                if (xDot < 0)
//                {
//                    halfWidth = -halfWidth;
//                    bit = 0;
//                }
//                else
//                    bit = 1;
//                candidate = new Vector3(halfWidth, halfHeight, halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V1 = candidate;
//                candidate = new Vector3(halfWidth, -halfHeight, halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V2 = candidate;
//                candidate = new Vector3(halfWidth, -halfHeight, -halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V3 = candidate;
//                candidate = new Vector3(halfWidth, halfHeight, -halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V4 = candidate;

//                boxFace.Id1 = bit + 2 + 4;
//                boxFace.Id2 = bit + 4;
//                boxFace.Id3 = bit + 2;
//                boxFace.Id4 = bit;
//            }
//            else if (absY > absX && absY > absZ)
//            {
//                //"Y" faces are candidates
//                if (yDot < 0)
//                {
//                    halfHeight = -halfHeight;
//                    bit = 0;
//                }
//                else
//                    bit = 2;
//                candidate = new Vector3(halfWidth, halfHeight, halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V1 = candidate;
//                candidate = new Vector3(-halfWidth, halfHeight, halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V2 = candidate;
//                candidate = new Vector3(-halfWidth, halfHeight, -halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V3 = candidate;
//                candidate = new Vector3(halfWidth, halfHeight, -halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V4 = candidate;

//                boxFace.Id1 = 1 + bit + 4;
//                boxFace.Id2 = bit + 4;
//                boxFace.Id3 = 1 + bit;
//                boxFace.Id4 = bit;
//            }
//            else if (absZ > absX && absZ > absY)
//            {
//                //"Z" faces are candidates
//                if (zDot < 0)
//                {
//                    halfLength = -halfLength;
//                    bit = 0;
//                }
//                else
//                    bit = 4;
//                candidate = new Vector3(halfWidth, halfHeight, halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V1 = candidate;
//                candidate = new Vector3(-halfWidth, halfHeight, halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V2 = candidate;
//                candidate = new Vector3(-halfWidth, -halfHeight, halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V3 = candidate;
//                candidate = new Vector3(halfWidth, -halfHeight, halfLength);
//                Vector3.Transform(ref candidate, ref worldTransform, out candidate);
//                boxFace.V4 = candidate;

//                boxFace.Id1 = 1 + 2 + bit;
//                boxFace.Id2 = 2 + bit;
//                boxFace.Id3 = 1 + bit;
//                boxFace.Id4 = bit;
//            }
//        }


//        private struct BoxFace
//        {
//            public int Id1, Id2, Id3, Id4;
//            public Vector3 V1, V2, V3, V4;

//            public int GetId(int i)
//            {
//                switch (i)
//                {
//                    case 0:
//                        return Id1;
//                    case 1:
//                        return Id2;
//                    case 2:
//                        return Id3;
//                    case 3:
//                        return Id4;
//                }
//                return -1;
//            }

//            public void GetVertex(int i, out Vector3 v)
//            {
//                switch (i)
//                {
//                    case 0:
//                        v = V1;
//                        return;
//                    case 1:
//                        v = V2;
//                        return;
//                    case 2:
//                        v = V3;
//                        return;
//                    case 3:
//                        v = V4;
//                        return;
//                }
//                v = Toolbox.NoVector;
//            }

//            internal void GetEdge(int i, ref Vector3 mtd, out FaceEdge clippingEdge)
//            {
//                Vector3 insidePoint;
//                switch (i)
//                {
//                    case 0:
//                        clippingEdge.A = V1;
//                        clippingEdge.B = V2;
//                        insidePoint = V3;
//                        clippingEdge.Id = GetEdgeId(Id1, Id2);
//                        break;
//                    case 1:
//                        clippingEdge.A = V2;
//                        clippingEdge.B = V3;
//                        insidePoint = V4;
//                        clippingEdge.Id = GetEdgeId(Id2, Id3);
//                        break;
//                    case 2:
//                        clippingEdge.A = V3;
//                        clippingEdge.B = V4;
//                        insidePoint = V1;
//                        clippingEdge.Id = GetEdgeId(Id3, Id4);
//                        break;
//                    case 3:
//                        clippingEdge.A = V4;
//                        clippingEdge.B = V1;
//                        insidePoint = V2;
//                        clippingEdge.Id = GetEdgeId(Id4, Id1);
//                        break;
//                    default:
//                        clippingEdge.A = Toolbox.NoVector;
//                        clippingEdge.B = Toolbox.NoVector;
//                        insidePoint = Toolbox.NoVector;
//                        clippingEdge.Id = -1;
//                        break;
//                }
//                Vector3.Subtract(ref clippingEdge.B, ref clippingEdge.A, out clippingEdge.EdgeDirection);
//                Vector3.Cross(ref clippingEdge.EdgeDirection, ref mtd, out clippingEdge.Perpendicular);
//                float dot;
//                Vector3 offset;
//                Vector3.Subtract(ref insidePoint, ref clippingEdge.A, out offset);
//                Vector3.Dot(ref clippingEdge.Perpendicular, ref offset, out dot);
//                if (dot > 0)
//                {
//                    clippingEdge.Perpendicular.X = -clippingEdge.Perpendicular.X;
//                    clippingEdge.Perpendicular.Y = -clippingEdge.Perpendicular.Y;
//                    clippingEdge.Perpendicular.Z = -clippingEdge.Perpendicular.Z;
//                }
//                Vector3.Dot(ref clippingEdge.A, ref clippingEdge.Perpendicular, out clippingEdge.EdgeDistance);
//            }
//        }

//        private static int GetContactId(int vertexAEdgeA, int vertexBEdgeA, int vertexAEdgeB, int vertexBEdgeB)
//        {
//            return GetEdgeId(vertexAEdgeA, vertexBEdgeA) * 2549 + GetEdgeId(vertexAEdgeB, vertexBEdgeB) * 2857;
//        }

//        private static int GetContactId(int vertexAEdgeA, int vertexBEdgeA, ref FaceEdge clippingEdge)
//        {
//            return GetEdgeId(vertexAEdgeA, vertexBEdgeA) * 2549 + clippingEdge.Id * 2857;
//        }

//        private static int GetEdgeId(int id1, int id2)
//        {
//            return (id1 + 1) * 571 + (id2 + 1) * 577;
//        }

//        private struct FaceEdge : IEquatable<FaceEdge>
//        {
//            public Vector3 A, B;
//            public Vector3 EdgeDirection;
//            public float EdgeDistance;
//            public int Id;
//            public Vector3 Perpendicular;

//            #region IEquatable<FaceEdge> Members

//            public bool Equals(FaceEdge other)
//            {
//                return other.Id == Id;
//            }

//            #endregion

//            public bool IsPointInside(ref Vector3 point)
//            {
//                float distance;
//                Vector3.Dot(ref point, ref Perpendicular, out distance);
//                return distance < EdgeDistance; // +1; //TODO: Bias this a little?
//            }
//        }
//    }
//}