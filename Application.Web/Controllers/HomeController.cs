using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Application.Infrastructure;
using Application.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using ViewModels = Application.Core.Models.ViewModels;

namespace Application.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMemoryCache _cache;

        private readonly HttpClient _client = new HttpClient();

        private readonly IConfiguration _configuration;

        private readonly IHttpContextAccessor _accessor;

        public HomeController(IConfiguration configuration, IMemoryCache cache, IHttpContextAccessor accessor)
        {
            _accessor = accessor;
            _configuration = configuration;

            _client.BaseAddress = new Uri(_configuration.GetSection("ApplicationSettings:BaseApiUrl").Value);
            _client.DefaultRequestHeaders.Accept.Clear();

            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            _client.DefaultRequestHeaders.Add("UserId", GetUserId().ToString());

            _cache = cache;
        }

        [HttpGet]
        private async Task<Equipments> GetEquipment(int equipmentId)
        {
            Equipments result = null;

            HttpResponseMessage response =
                await _client.GetAsync($"UserEquipments/EquipmentList/{GetUserId()}/{equipmentId}");

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<Equipments>();
            }

            return result;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Equipment List";

            var cachedStock = _cache.Get<IEnumerable<ViewModels.UserEquipmentViewModel>>(GetUserId());

            HttpResponseMessage response = null;

            if (cachedStock == null)
            {
                response = await _client.GetAsync($"UserEquipments/EquipmentList");

                List<ViewModels.UserEquipmentViewModel> result = null;
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<List<ViewModels.UserEquipmentViewModel>>();
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(60));

                _cache.Set(GetUserId(), result, cacheEntryOptions);
                cachedStock = _cache.Get<IEnumerable<ViewModels.UserEquipmentViewModel>>(GetUserId());
            }

            var equipmentCount = 0;

            response = await _client.GetAsync($"UserEquipments/UserEquipmentCount");

            if (response.IsSuccessStatusCode)
            {
                equipmentCount = await response.Content.ReadAsAsync<int>();
            }

            ViewBag.EquipmentCount = equipmentCount;
            return View(cachedStock);
        }


        [HttpGet]
        public async Task<IActionResult> Add(int equipmentId)
        {
            ViewBag.Title = "Add To Cart";
            AddHeader("equipmentId", equipmentId.ToString());
            HttpResponseMessage response = await _client.GetAsync($"UserEquipments/EquipmentList/");

            List<ViewModels.UserEquipmentViewModel> result = null;
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<List<ViewModels.UserEquipmentViewModel>>();
            }

            return View(result.Single());
        }


        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ViewModels.UserEquipmentViewModel model)
        {
            if (model.UserRentalDay == 0)
            {
                ModelState.AddModelError("rentalDayError", "Please enter a rental day");
                return View(model);
            }

            if (model.MinimumRentalDay > 0 && model.UserRentalDay < model.MinimumRentalDay)
            {
                ModelState.AddModelError("rentalDayError", "Rental day is less or greater than Minimum Rental Day");
                return View(model);
            }

            model.UserId = GetUserId();

            HttpResponseMessage response = await _client.PostAsJsonAsync("UserEquipments/Add/", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Report()
        {
            return View("Report", await GetReport());
        }

        public async Task<FileResult> DownloadReport()
        {
            var report = await GetReport();

            StringBuilder sb = new StringBuilder();

            foreach (var item in report.Equipments)
            {
                sb.Append($"Invoice Number : {item.InvoiceId}\tInvoice Date : {item.InvoiceDate}\t");
                sb.Append($"Equipment Type : {item.EquipmentType}\tEquipment Name : {item.EquipmentName}\t");
                sb.Append($"Rental Days : {item.EquipmentRentalDay}\tRental Fee : {item.EquipmentRentalFee}\n"+Environment.NewLine);
            }
            sb.Append(Environment.NewLine);
            sb.Append($"{Environment.NewLine}Loyalty Point : {report.LoyaltPoint}\tTotal Fee : {report.TotalPrice}");

            var byteArray = Encoding.ASCII.GetBytes(sb.ToString());
            var stream = new MemoryStream(byteArray);

            return File(stream, "text/plain", $"{DateTime.Now.ToShortDateString()}_report.txt");
        }

        [HttpGet]
        public async Task<IActionResult> RemoveEquipment(string equipmentId)
        {
            ViewBag.Title = "Remove Item";

            AddHeader("equipmentId", equipmentId.ToString());
            HttpResponseMessage response =
                await _client.PostAsJsonAsync($"UserEquipments/RemoveEquipment/", new { equipmentId });

            return RedirectToAction("Report");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Private Methods
        private async Task<Invoice> GetReport()
        {
            Invoice result = null;

            HttpResponseMessage response = await _client.GetAsync($"UserEquipments/GenerateReport/");

            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<Invoice>();
            }
            return result;
        }

        private void AddHeader(string name, string value)
        {
            _client.DefaultRequestHeaders.Add(name, value);
        }

        private int GetUserId()
        {
            //created random user id from request ip for unique records
            try
            {
                return Convert.ToInt32(_accessor.HttpContext.Connection.RemoteIpAddress.ToString().Replace(".", "")
                    .Replace(":", ""));
            }
            catch
            {
                return 1;
            }
        }

        #endregion
    }
}