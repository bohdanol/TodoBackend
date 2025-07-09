using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host.Controllers;

[Route("api/todo-list/task")]
[ApiController]
public class TaskController
{
    [HttpGet("task")]
    public async async Task<IActionResult> ()
    {
        //var taskInfo = await 
    }
}
