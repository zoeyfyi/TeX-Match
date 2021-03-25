/*
 * Derived from https://github.com/google/google-authenticator-android/blob/master/AuthenticatorApp/src/main/java/com/google/android/apps/authenticator/Base32String.java
 * 
 * Copyright (C) 2016 BravoTango86
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Text;

namespace TeX_Match.Core.Helpers
{
    public static class Base32
    {
        private static readonly char[] _digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567".ToCharArray();
        private const int _mask = 31;
        private const int _shift = 5;

        private static int CharToInt(char c)
        {
            return Array.IndexOf(_digits, c);
        }

        public static byte[] FromBase32String(string encoded)
        {
            if (encoded == null)
                throw new ArgumentNullException(nameof(encoded));

            // Remove whitespace and padding. Note: the padding is used as hint 
            // to determine how many bits to decode from the last incomplete chunk
            // Also, canonicalize to all upper case
            encoded = encoded.Trim().TrimEnd('=').ToUpper();
            if (encoded.Length == 0)
                return new byte[0];

            var outLength = encoded.Length * _shift / 8;
            var result = new byte[outLength];
            var buffer = 0;
            var next = 0;
            var bitsLeft = 0;
            var charValue = 0;
            foreach (var c in encoded)
            {
                charValue = CharToInt(c);
                if (charValue < 0)
                    throw new FormatException("Illegal character: `" + c + "`");

                buffer <<= _shift;
                buffer |= charValue & _mask;
                bitsLeft += _shift;
                if (bitsLeft >= 8)
                {
                    result[next++] = (byte)(buffer >> bitsLeft - 8);
                    bitsLeft -= 8;
                }
            }

            return result;
        }

        public static string ToBase32String(byte[] data, bool padOutput = false)
        {
            return ToBase32String(data, 0, data.Length, padOutput);
        }

        public static string ToBase32String(byte[] data, int offset, int length, bool padOutput = false)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (offset + length > data.Length)
                throw new ArgumentOutOfRangeException();

            if (length == 0)
                return "";

            // SHIFT is the number of bits per output character, so the length of the
            // output is the length of the input multiplied by 8/SHIFT, rounded up.
            // The computation below will fail, so don't do it.
            if (length >= 1 << 28)
                throw new ArgumentOutOfRangeException(nameof(data));

            var outputLength = (length * 8 + _shift - 1) / _shift;
            var result = new StringBuilder(outputLength);

            var last = offset + length;
            int buffer = data[offset++];
            var bitsLeft = 8;
            while (bitsLeft > 0 || offset < last)
            {
                if (bitsLeft < _shift)
                {
                    if (offset < last)
                    {
                        buffer <<= 8;
                        buffer |= data[offset++] & 0xff;
                        bitsLeft += 8;
                    }
                    else
                    {
                        int pad = _shift - bitsLeft;
                        buffer <<= pad;
                        bitsLeft += pad;
                    }
                }
                int index = _mask & buffer >> bitsLeft - _shift;
                bitsLeft -= _shift;
                result.Append(_digits[index]);
            }
            if (padOutput)
            {
                int padding = 8 - result.Length % 8;
                if (padding > 0) result.Append('=', padding == 8 ? 0 : padding);
            }
            return result.ToString();
        }
    }
}