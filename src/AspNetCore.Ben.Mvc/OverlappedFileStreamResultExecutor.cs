// Copyright (c) Ben Adams. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Ben.Mvc
{
    public class OverlappedFileStreamResultExecutor : FileResultExecutorBase
    {
        // 32kB buffers
        private const int BufferSize = 0x10000;

        private static readonly Task<int> _readNotStarted = Task.FromResult(-1);
        private static readonly Task _writeNotStarted = Task.CompletedTask;

        public OverlappedFileStreamResultExecutor(ILoggerFactory loggerFactory)
            : base(CreateLogger<OverlappedFileStreamResultExecutor>(loggerFactory))
        {
        }

        public Task ExecuteAsync(ActionContext context, OverlappedFileStreamResult result)
        {
            SetHeadersAndLog(context, result);
            return WriteFileAsync(context, result);
        }

        private static async Task WriteFileAsync(ActionContext context, OverlappedFileStreamResult result)
        {
            var output = context.HttpContext.Response.Body;
            var input = result.FileStream;

            var readTask = _readNotStarted;
            var writeTask = _writeNotStarted;

            // Triple buffer so reads and writes never overlap data
            var buffers = new byte[3][];
            for (var i = 0; i < buffers.Length; i++)
            {
                buffers[i] = ArrayPool<byte>.Shared.Rent(BufferSize);
            }

            var cts = new CancellationTokenSource();
            var step = 1;
            try
            {
                var count = -1;
                do
                {
                    // Wait for last read to complete before reading again
                    count = await readTask;

                    // If last read wasn't the end, submit another read
                    if (count != 0)
                    {
                        var inputBuffer = buffers[(step % buffers.Length)];
                        readTask = input.ReadAsync(inputBuffer, 0, inputBuffer.Length, cts.Token);
                    }

                    // If previous read read something, write it out
                    if (count > 0)
                    {
                        // Wait for last write to complete before writing again
                        await writeTask;

                        var outputBuffer = buffers[((step - 1) % buffers.Length)];
                        writeTask = output.WriteAsync(outputBuffer, 0, count, cts.Token);
                    }

                    step++;

                } while (count != 0);

                // Wait for final write to complete
                await writeTask;
            }
            finally
            {
                // Cancel any pending operations
                cts.Cancel();

                // Return the buffers
                for (var i = 0; i < buffers.Length; i++)
                {
                    ArrayPool<byte>.Shared.Return(buffers[i]);
                }

                // Dispose the input stream
                input.Dispose();
            }
        }
    }
}
