using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation.Demo.Controllers
{
    public enum IntEnum
    {
        One,
        Two,
        Three,
        Four
    }

    /// <summary>
    /// Gets values and that
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// // GET api/values
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// // GET api/values/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<string> Get(IntEnum id)
        {
            return "value";
        }

        /// <summary>
        /// // POST api/values
        /// </summary>
        /// <param name="value"></param>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// // PUT api/values/5
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="value">value</param>
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] string value)
        {
        }

        /// <summary>
        /// // DELETE api/values/5
        /// </summary>
        /// <param name="id">di</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
