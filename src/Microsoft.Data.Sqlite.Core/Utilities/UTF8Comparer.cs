// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#pragma warning disable IDE1006

namespace Microsoft.Data.Sqlite.Utilities
{
    // ReadOnlySpan?
    internal class UTF8Comparer : EqualityComparer<byte[]>
    {
        public static new UTF8Comparer Default = new();

        public override bool Equals(byte[]? x, byte[]? y)
        {
            if (x is null)
            {
                return y is null;
            }
            if (y is null)
            {
                return false;
            }

            var length = x.Length;
            if (length != y.Length)
            {
                return false;
            }
            if (x == y)
            {
                return true;
            }

            unsafe
            {
                fixed (byte* ap = x)
                fixed (byte* bp = y)
                {
                    var a = ap;
                    var b = bp;

                    while (length >= 4)
                    {
                        if (*(int*)a != *(int*)b)
                        {
                            return false;
                        }

                        a += 4;
                        b += 4;
                        length -= 4;
                    }

                    if (length >= 2)
                    {
                        if (*(short*)a != *(short*)b)
                        {
                            return false;
                        }

                        a += 2;
                        b += 2;
                        length -= 2;
                    }

                    if (length > 0)
                    {
                        if (*a != *b)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }

        public override unsafe int GetHashCode(byte[] obj)
        {
            var length = obj.Length;
            var hash = length;

            fixed (byte* ap = obj)
            {
                var a = ap;

                while (length >= 4)
                {
                    hash = (hash + _rotl(hash, 5)) ^ *(int*)a;
                    a += 4;
                    length -= 4;
                }

                if (length >= 2)
                {
                    hash = (hash + _rotl(hash, 5)) ^ *(short*)a;
                    a += 2;
                    length -= 2;
                }

                if (length > 0)
                {
                    hash = (hash + _rotl(hash, 5)) ^ *a;
                }

                hash += _rotl(hash, 7);
                hash += _rotl(hash, 15);

                return hash;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _rotl(int value, int shift)
        {
            // This is expected to be optimized into a single rotl instruction
            return (int)(((uint)value << shift) | ((uint)value >> (32 - shift)));
        }
    }
}
