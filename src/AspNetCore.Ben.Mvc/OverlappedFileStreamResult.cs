﻿// Copyright(c) Ben Adams.All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Ben.Mvc
{
    /// <summary>
    /// Represents an <see cref="ActionResult"/> that when executed will
    /// write a file from a stream to the response.
    /// </summary>
    public class OverlappedFileStreamResult : FileResult
    {
        private Stream _fileStream;

        /// <summary>
        /// Creates a new <see cref="OverlappedFileStreamResult"/> instance with
        /// the provided <paramref name="fileStream"/> and the
        /// provided <paramref name="contentType"/>.
        /// </summary>
        /// <param name="fileStream">The stream with the file.</param>
        /// <param name="contentType">The Content-Type header of the response.</param>
        public OverlappedFileStreamResult(Stream fileStream, string contentType)
            : this(fileStream, MediaTypeHeaderValue.Parse(contentType))
        {
        }

        /// <summary>
        /// Creates a new <see cref="OverlappedFileStreamResult"/> instance with
        /// the provided <paramref name="fileStream"/> and the
        /// provided <paramref name="contentType"/>.
        /// </summary>
        /// <param name="fileStream">The stream with the file.</param>
        /// <param name="contentType">The Content-Type header of the response.</param>
        public OverlappedFileStreamResult(Stream fileStream, MediaTypeHeaderValue contentType)
            : base(contentType?.ToString())
        {
            if (fileStream == null)
            {
                throw new ArgumentNullException(nameof(fileStream));
            }

            FileStream = fileStream;
        }

        /// <summary>
        /// Gets or sets the stream with the file that will be sent back as the response.
        /// </summary>
        public Stream FileStream
        {
            get
            {
                return _fileStream;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _fileStream = value;
            }
        }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var executor = context.HttpContext.RequestServices.GetRequiredService<OverlappedFileStreamResultExecutor>();
            return executor.ExecuteAsync(context, this);
        }
    }
}