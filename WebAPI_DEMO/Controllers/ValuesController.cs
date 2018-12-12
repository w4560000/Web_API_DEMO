using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI_DEMO.Model;
using WebAPI_DEMO.Model.Table;

namespace WebAPI_DEMO.Controllers
{
    [Authorize(Policy = "TrainedStaffOnly")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {


        public IAccountService _studentService;

        public ValuesController(IAccountService studentService)
        {
            this._studentService = studentService;
        }


        // GET api/values
        [HttpPost("GetAccountData")]
        public List<AccountData> Get()
        {
            return this._studentService.GetAccountData();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
