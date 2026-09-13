using DatingAppApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[Controller]")]
    [ApiController]
    public class BaseApiController : Controller
    {

       
    }
}
