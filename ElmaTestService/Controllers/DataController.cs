using ElmaTestService.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;

namespace ElmaTestService.Controllers
{
    public class DataController : ApiController
    {
        private IStoragable<string, string> _storage;
        private IDictionary<string, string> _otherStorage;
        public DataController(IStoragable<string, string> storage, IDictionary<string, string> otherStorage)
        {
            _storage = storage;
            _otherStorage = otherStorage;
        }

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
                // Найти на других серверах и отдать.
                // TODO избежать зацикливания между 2 серверами (коллизии!).
                if (_otherStorage.ContainsKey(key) && GetFromServer(_otherStorage[key], key, out value))
                {
                    return Ok(value);
                }
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
            // TODO как вариант, можно реализовать проверку, добавлен ли ключ на других серверах и обновить его там.
            return Ok($"{key} added");
        }

        private bool GetFromServer(string url, string key, out string value)
        {
            value = null;
            try
            {
                var request = WebRequest.Create($"{url}/data/{key}");
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        value = stream.ReadToEnd();
                        return true;
                    }
                }
            }
            catch
            {
                Console.WriteLine($"Не удалось получить значение ключа {key} с сервера {url}.");
            }
            return false;
        }
    }
}
