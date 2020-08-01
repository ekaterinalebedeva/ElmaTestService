using ElmaTestService.Broadcast;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Results;

namespace ElmaTestService.Controllers
{
    public class DataController : ApiController
    {
        private readonly Storage<string> _storage = Startup.MyStorage;

        [HttpGet]
        public IHttpActionResult Get()
        {
            var allCache = _storage.Values.ToList();
            return Ok(string.Join("\r\n", allCache.Select(p => $"{p.Key}    {p.Value}").ToList()));
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
            SendMessage("delete", key);
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
            SendMessage("add", key);
            return Ok($"{key} added");
        }

        private void SendMessage(string method, string key)
        {
            // Получаем контекст хаба
            var context =
                Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            // отправляем сообщение
            context.Clients.All.displayMessage(method, key);
        }
    }
}
