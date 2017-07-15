// <copyright file="TestController.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace App.Metrics.Graphite.Sandbox.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
