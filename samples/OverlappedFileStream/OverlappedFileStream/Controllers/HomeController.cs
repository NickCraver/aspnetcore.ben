// Copyright (c) Ben Adams. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

using AspNetCore.Ben.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace OverlappedFileStream.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult FileStreamResult()
        {
            var result = new FileStreamResult(new TwoGBFileStream(), "application/octet-stream");
            result.FileDownloadName = "LargeEmptyFile.bin";
            return result;
        }

        public IActionResult OverlappedFileStreamResult()
        {
            var result = new OverlappedFileStreamResult(new TwoGBFileStream(), "application/octet-stream");
            result.FileDownloadName = "LargeEmptyFile.bin";
            return result;
        }
    }
}
