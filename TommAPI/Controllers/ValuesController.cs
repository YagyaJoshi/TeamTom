using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TommAPI.Providers;
using TommDAL.ViewModel;

namespace TommAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public IConfiguration _configuration { get; }

        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;

        }



        public static DateTime GetStartOfWeek(DateTime input)
        {
            int k = ((int)input.DayOfWeek);
            // Using +6 here leaves Monday as 0, Tuesday as 1 etc.
            int dayOfWeek = (((int)input.DayOfWeek) + 6) % 7;
            return input.Date.AddDays(-dayOfWeek);
        }


        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
           // List<UserTaskHisV3> objUserTaskHisV3 = PushNotification.GetUserDetailsforNotification(45, 2, _configuration);
            //DateTime periodStart = Convert.ToDateTime("2019-02-25"); 
            //DateTime periodEnd = new DateTime(2019, 12, 30);


            //periodStart = GetStartOfWeek(periodStart);
            //periodEnd = GetStartOfWeek(periodEnd);
            //int days = (int)(periodEnd - periodStart).TotalDays;
            //Int32 j = (days / 7);
            //int k = j % 8;
          
            //string h = $"He asked, \"{j}\", but didn't wait for a reply :-{{";

            return new string[] { "value1", "value2" };
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
