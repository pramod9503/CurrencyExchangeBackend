using System.Text;
using System.Text.Json;
using CurrencyRepo.Abstracts;
using Microsoft.AspNetCore.Mvc;
using CurrencyAdministrator.Hubs;
using CurrencyRepo.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using CurrencyAdministrator.Abstract;
using CurrencyRepo.Models.BackModels;
using CurrencyRepo.Models.FrontModels;
using Microsoft.Extensions.Caching.Distributed;

namespace CurrencyAdministrator.Controllers
{    
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AdministratorController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly ICacheKeyStore _keyStore;
        private readonly IAdminCurrencies _currencies;
        private readonly IDirectMessages _directMessages;        
        private readonly ILogger<AdministratorController> _logger;
        private readonly IHubContext<CurrencyHub, ICurrencyClient> _hubContext;

        public AdministratorController(IAdminCurrencies currencies, IDirectMessages directMessages, 
            IDistributedCache cache, ICacheKeyStore keyStore, ILogger<AdministratorController> logger,
            IHubContext<CurrencyHub, ICurrencyClient> hubContext) 
        {
            _cache = cache;
            _logger = logger;
            _keyStore = keyStore;            
            _currencies = currencies;
            _hubContext = hubContext;
            _directMessages = directMessages;            
        }

        //[HttpGet("TestRabbit")]
        //public async Task<IActionResult> TestRabbit(string text) 
        //{            
        //    _directMessages.Message = text;
        //    await _directMessages.SendMessageAsync();
        //    return Ok("Message sent.");
        //}

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrencies([FromQuery]int page = 1, [FromQuery]int count = 5)
        {
            string key = $"currencies_{page.ToString()}_{count.ToString()}";
            byte[]? bytes = await _cache.GetAsync(key);
            if (bytes != null)
            {
                IEnumerable<Currency>? cacheCurrencies = JsonSerializer.Deserialize<IEnumerable<Currency>>(bytes);
                //HttpContext.Response.Headers["Cache-Control"] = "public, max-age=120";
                return Ok(new { Currencies = cacheCurrencies, Status = "Cache" });
            }
            try
            {
                //IEnumerable<Currency> currencies = _currencies.GetCurrencies(page, count);
                IEnumerable<Currency> currencies = _currencies.GetCurrencies();
                string currencies_cache = JsonSerializer.Serialize(currencies);
                byte[] result_bytes = Encoding.UTF8.GetBytes(currencies_cache);
                await _cache.SetAsync(key, result_bytes);
                _keyStore.AddKey(key);
                return Ok(new { Currencies = currencies, Status = "Database" });
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest("Something went wrong on the server.");
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Currency([FromRoute]int id)
        {            
            try
            {
                string key = $"currency_{id.ToString()}";
                byte[]? bytes = await _cache.GetAsync(key);
                if (bytes != null)
                {
                    Currency? cacheCurrency = JsonSerializer.Deserialize<Currency>(bytes);                    
                    if (cacheCurrency != null) cacheCurrency.Source = "Cache";
                    return Ok(cacheCurrency);
                }
                Currency? currency = await _currencies.GetCurrency(id);
                if (currency == null) return NotFound();

                string currency_cache = JsonSerializer.Serialize(currency);
                byte[] result_bytes = Encoding.UTF8.GetBytes(currency_cache);
                await _cache.SetAsync(key, result_bytes);
                _keyStore.AddKey(key);
                currency.Source = "Database";
                return Ok(currency);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest("Something went wrong on the server.");
            }
        }

        [HttpPut("UpdateRate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateRate(UpdateRateModel rateModel) 
        {
            try
            {
                Currency currency = await _currencies.UpdateRate(rateModel);
                foreach (string key in _keyStore.CacheKeys)
                {
                    await _cache.RemoveAsync(key);
                }
                _keyStore.Clear();
                currency.Source = "Database";
                CurrencyMessageModel messageModel = new() { Currency = currency, Operation = CurrencyOperationEnum.RateUpdate};                
                _directMessages.CurrencyMessage = messageModel;
                await _hubContext.Clients.All.ReceiveUpdate(messageModel);
                await _directMessages.SendMessageAsync();                
                return Ok(currency);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest("Something went wrong on the server.");
            }
        }

        [HttpPut("UpdateCurrency")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCurrency([FromBody]Currency currency) 
        {
            try
            {
                Currency updatedCurrency = await _currencies.UpdateCurrency(currency);
                foreach (string key in _keyStore.CacheKeys)
                {
                    await _cache.RemoveAsync(key);
                }
                _keyStore.Clear();
                currency.Source = "Database";
                CurrencyMessageModel messageModel = new() { Currency = currency, Operation = CurrencyOperationEnum.CurrencyUpdate };                
                _directMessages.CurrencyMessage = messageModel;                
                await _directMessages.SendMessageAsync();                
                return Ok(updatedCurrency);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest("Something went wrong on the server.");
            }
        }

        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(AddCurrencyModel currencyModel) 
        {
            try
            {
                Currency currency = await _currencies.AddCurrency(currencyModel);
                foreach (string key in _keyStore.CacheKeys)
                {
                    await _cache.RemoveAsync(key);
                }
                _keyStore.Clear();
                currency.Source = "Database";
                CurrencyMessageModel messageModel = new() { Currency = currency, Operation = CurrencyOperationEnum.CurrencyAdded };                
                _directMessages.CurrencyMessage = messageModel;                
                await _directMessages.SendMessageAsync();                
                return CreatedAtAction(nameof(Currency), new { id = currency.Id }, currency);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest("Something went wrong on the server.");
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute]int id) 
        {
            try
            {
                Currency currency = await _currencies.DeleteCurrency(id);
                foreach (string key in _keyStore.CacheKeys)
                {
                    await _cache.RemoveAsync(key);
                }
                _keyStore.Clear();
                currency.Source = "Database";
                CurrencyMessageModel messageModel = new() { Currency = currency, Operation = CurrencyOperationEnum.CurrencySoftDelete };                
                await _directMessages.SendMessageAsync();
                return NoContent();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest("Something went wrong on the server");
            }
        }
    }
}
