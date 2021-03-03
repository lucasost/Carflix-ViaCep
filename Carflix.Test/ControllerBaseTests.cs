using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Carflix.Test
{
    class ControllerBaseTests
    {
        protected ControllerContext ControllerContext { get; }

        protected CarflixContext context { get; }

        public ControllerBaseTests()
        {
        }
    }
}
