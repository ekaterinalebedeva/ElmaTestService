﻿using ElmaTestService.Models;
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
    /// <summary>
    /// Основной и единственный контроллер, предоставляющий api по работе с кэшем
    /// </summary>
    public class DataController : ApiController
    {
        /// <summary>
        /// Хранилище ключей значений сервера
        /// </summary>
        private IStoragable<string, string> _storage;
        /// <summary>
        /// Хранилище ключей, расположенных на других серверах
        /// </summary>
        private IDictionary<string, string> _otherStorage;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage">Хранилище ключей значений сервера</param>
        /// <param name="otherStorage">Хранилище ключей, расположенных на других серверах</param>
        public DataController(IStoragable<string, string> storage, IDictionary<string, string> otherStorage)
        {
            _storage = storage;
            _otherStorage = otherStorage;
        }
        /// <summary>
        /// Получить все ключи на этом сервере
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get()
        {
            var allCache = _storage.GetAllKeys();
            return Ok(string.Join("\r\n", allCache));
        }
        /// <summary>
        /// Получить значение кэша по ключу
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get(string key)
        {
            if (!_storage.TryGetByKey(key, out var value))
            {
                // Найти на других серверах и отдать.
                // TODO избежать зацикливания между 2 серверами (коллизии!).
                if (_otherStorage.ContainsKey(key))
                {
                    Console.WriteLine($"Запрос на получение перенаправлен на {_otherStorage[key]}.");
                    if (GetFromServer(_otherStorage[key], key, out value))
                    {
                        return Ok(value);
                    }
                }
                return NotFound();
            }
            return Ok(value);
        }
        /// <summary>
        /// Удалить значение по ключу
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpDelete]
        public IHttpActionResult Delete(string key)
        {
            if (!_storage.Remove(key))
            {   
                // Найти на других серверах и отдать.
                if (_otherStorage.ContainsKey(key))
                {
                    Console.WriteLine($"Запрос на удаление перенаправлен на {_otherStorage[key]}.");
                    if (DeleteFromServer(_otherStorage[key], key))
                    {
                        return Ok($"{key} deleted");
                    }
                }
                return NotFound();
            }
            return Ok($"{key} deleted");
        }
        /// <summary>
        /// Добавить кэш в виде ключ-значение
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Запрос значения с другого сервера
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key">Искомый ключ</param>
        /// <param name="value">Выходной параметр со значением, если удалось его получить</param>
        /// <returns>true, если удалось успешно получить значение</returns>
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
        /// <summary>
        /// Запрос на удаление ключа на другом сервере
        /// </summary>
        /// <param name="url"></param>
        /// <param name="key"></param>
        /// <returns>Ключ успешно удален по запросу</returns>
        private bool DeleteFromServer(string url, string key)
        {
            try
            {
                var request = WebRequest.Create($"{url}/data/{key}");
                request.Method = "DELETE";
                var response = (HttpWebResponse)request.GetResponse();
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch
            {
                Console.WriteLine($"Не удалось удалить ключ {key} с сервера {url}.");
            }
            return false;
        }
    }
}
