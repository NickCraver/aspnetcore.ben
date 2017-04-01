// Copyright (c) Ben Adams. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OverlappedFileStream
{
    public class TwoGBFileStream : Stream
    {
        private int _bytesRemaing = int.MaxValue;
        private Task<int> _cachedTask;

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;

        public override int Read(byte[] buffer, int offset, int count)
        {
            var readBytes = Math.Min(_bytesRemaing, count);
            _bytesRemaing -= readBytes;
            return readBytes;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellation)
        {
            var readBytes = Read(buffer, offset, count);

            if (readBytes != _cachedTask?.Result)
            {
                _cachedTask = Task.FromResult(readBytes);
            }

            return _cachedTask;
        }

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public override void Flush() => throw new NotImplementedException();
        public override long Length => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}
