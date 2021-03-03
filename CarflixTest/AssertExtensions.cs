using System;
using System.Collections.Generic;
using System.Text;

namespace CarflixTest
{
    class AssertExtensions : Xunit.Assert
    {
        public static void BadRequest(ActionResult result)
        {
            Assert.NotNull(result);
            var badRequest = Assert.IsType<HttpStatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequest.StatusCode);
        }

        public static void ExpctationFailed(ActionResult result)
        {
            Assert.NotNull(result);
            var badRequest = Assert.IsType<HttpStatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.ExpectationFailed, badRequest.StatusCode);
        }

        public static void JsonExpctationFailed(ActionResult result)
        {
            Assert.NotNull(result);
            var badRequest = Assert.IsType<JsonHttpStatusResult>(result);
            Assert.Equal((int)HttpStatusCode.ExpectationFailed, badRequest.StatusCode);
        }

        public static void NotFound(ActionResult result)
        {
            Assert.NotNull(result);
            var notFound = Assert.IsType<HttpNotFoundResult>(result);
            Assert.Equal((int)HttpStatusCode.NotFound, notFound.StatusCode);
        }
    }
}
