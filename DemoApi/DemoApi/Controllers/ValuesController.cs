using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ITracer _tracer;

        public ValuesController(ITracer tracer)
        {
            _tracer = tracer;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            using (IScope scope = _tracer.BuildSpan("waitingForValues").StartActive(finishSpanOnDispose: true))
            {
                await Task.Delay(1000);

                return new string[] { "value1", "value2" };
            }
        }


        // GET api/values
        //[HttpGet]
        //public ActionResult<IEnumerable<string>> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
    }
}
