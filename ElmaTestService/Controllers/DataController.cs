using ElmaTestService.Models;
using System.Net;
using System.Web.Http;

namespace ElmaTestService.Controllers
{
    public class DataController : ApiController
    {
        private IStoragable<string, string> _storage = Program.MyStorage;

        //public DataController(IStoragable<string, string> storage)
        //{
        //    _storage = storage;
        //}

        [HttpGet]
        public IHttpActionResult Get()
        {
            var allCache = _storage.GetAllKeys();
            return Ok(string.Join("\r\n", allCache));
        }

        [HttpGet]
        public IHttpActionResult Get(string key)
        {
            if (!_storage.TryGetByKey(key, out var value))
            {
                return NotFound();
            }
            return Ok(value);
        }

        [HttpDelete]
        public IHttpActionResult Delete(string key)
        {
            if (!_storage.Remove(key))
            {
                return NotFound();
            }
            return Ok($"{key} deleted");
        }

        [HttpPost]
        public IHttpActionResult Add(string key, [FromBody] string value)
        {
            if (value == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            _storage.Add(key, value);
            return Ok($"{key} added");
        }
    }
}
