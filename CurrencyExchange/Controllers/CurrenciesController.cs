using System.Text;
using System.Text.Json;
using CurrencyRepo.Abstracts;
using Microsoft.AspNetCore.Mvc;
using CurrencyRepo.Models.BackModels;
using Microsoft.Extensions.Caching.Distributed;

namespace CurrencyExchange.Controllers
{    
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencies _currencies;
        private readonly IDistributedCache _cache;
        private readonly ICacheKeyStore _keyStore;
        private readonly ILogger<CurrenciesController> _logger;

        public CurrenciesController(ICurrencies currencies, IDistributedCache cache, 
            ILogger<CurrenciesController> logger, ICacheKeyStore keyStore) 
        {
            _cache = cache;
            _logger = logger;
            _keyStore = keyStore;
            _currencies = currencies;            
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetCurrencies(int page = 1, int count = 5) 
        public async Task<IActionResult> GetCurrencies()
        {
            //string key = $"currencies_{page.ToString()}_{count.ToString()}";
            string key = $"currencies";
            byte[]? bytes = await _cache.GetAsync(key);            
            if (bytes != null)            
            {
                IEnumerable<Currency>? cacheCurrencies = JsonSerializer.Deserialize<IEnumerable<Currency>>(bytes);                
                //HttpContext.Response.Headers["Cache-Control"] = "public, max-age=120";
                return Ok(new { Currencies = cacheCurrencies, Status = "Cache" });
            }
            //IEnumerable<Currency> currencies = _currencies.GetCurrencies(page, count);
            IEnumerable<Currency> currencies = _currencies.GetCurrencies();
            string currencies_cache = JsonSerializer.Serialize(currencies);
            byte[] result_bytes = Encoding.UTF8.GetBytes(currencies_cache);
            await _cache.SetAsync(key, result_bytes);
            _keyStore.AddKey(key);
            return Ok(new { Currencies = currencies, Status = "Database" });
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
                    //HttpContext.Response.Headers["Cache-Control"] = "public, max-age=120";                    
                    if (cacheCurrency != null) cacheCurrency.Source = "Cache";
                    return Ok(cacheCurrency);
                }
                Currency? currency = await _currencies.GetCurrency(id);                
                if (currency == null) return NotFound();
                
                string currency_cache = JsonSerializer.Serialize(currency);
                byte[] result_bytes = Encoding.UTF8.GetBytes(currency_cache);
                await _cache.SetAsync(key , result_bytes);
                _keyStore.AddKey(key);
                currency.Source = "Database";
                return Ok(currency);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
