using Asp.Versioning;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace MyBGList_ApiVersion.Controllers.v3
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]/[action]")]
    [ApiVersion("3.0")]
    public class CodeOnDemandController : ControllerBase
    {
        [HttpGet(Name = "Test2")]
        [EnableCors("AnyOrigin")]
        [ResponseCache(NoStore = true)]
        public ContentResult Test2(int? minutesToAdd = null)
        {
            var datetime = DateTime.UtcNow;
            if (minutesToAdd.HasValue)
            {
                datetime = datetime.AddMinutes(minutesToAdd.Value);
            }

            return Content("<script>" +
                            "window.alert('Your client supports JavaScript!" +
                            "\\r\\n\\r\\n" +
                            $"Server time (UTC): {datetime.ToString("o")}" +
                            "\\r\\n" +
                            "Client time (UTC): ' + new Date().toISOString());" +
                            "</script>" +
                            "<noscript>Your client does not support JavaScript</noscript>",
                            "text/html");
        }
    }
}
