﻿// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Buffers;
using Nerdbank.Streams;
using Xunit;

namespace MessagePack.Tests
{
    public class MessagePackWriterTests
    {
        /// <summary>
        /// Verifies that <see cref="MessagePackWriter.WriteRaw(ReadOnlySpan{byte})"/>
        /// accepts a span that came from stackalloc.
        /// </summary>
        [Fact]
        public unsafe void WriteRaw_StackAllocatedSpan()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);

            Span<byte> bytes = stackalloc byte[8];
            bytes[0] = 1;
            bytes[7] = 2;
            fixed (byte* pBytes = bytes)
            {
                var flexSpan = new Span<byte>(pBytes, bytes.Length);
                writer.WriteRaw(flexSpan);
            }

            writer.Flush();
            var written = sequence.AsReadOnlySequence.ToArray();
            Assert.Equal(1, written[0]);
            Assert.Equal(2, written[7]);
        }

        [Fact]
        public void Write_ByteArray_null()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);
            writer.Write((byte[])null);
            writer.Flush();
            var reader = new MessagePackReader(sequence.AsReadOnlySequence);
            Assert.True(reader.TryReadNil());
        }

        [Fact]
        public void Write_ByteArray()
        {
            var sequence = new Sequence<byte>();
            var writer = new MessagePackWriter(sequence);
            var buffer = new byte[] { 1, 2, 3 };
            writer.Write(buffer);
            writer.Flush();
            var reader = new MessagePackReader(sequence.AsReadOnlySequence);
            Assert.Equal(buffer, reader.ReadBytes().ToArray());
        }
    }
}
