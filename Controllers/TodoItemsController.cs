using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackSheffield.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HackSheffield.Controllers
{
    [Route("api/test")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ILogger<TodoItemsController> _logger;
        public TodoItemsController(ILogger<TodoItemsController> logger)
        {
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<List<TodoItem>>> Get([FromQuery] String key)
        {
            if (Models.User.getByKey(key) == null)
            {
                return StatusCode(401, "Please provide api key");
            }
            return  TodoItem.getAll();
        }

        [HttpGet]
        [Route("add/{name}")]
        public async Task<ActionResult<bool>> Add([FromRoute] String name)
        {
            return TodoItem.insert(name);
        }
        
        [HttpGet]
        [Route("del/{id}")]
        public async Task<ActionResult<bool>> Del([FromRoute] String id)
        {
            return TodoItem.delete(id);
        }
    }
}
