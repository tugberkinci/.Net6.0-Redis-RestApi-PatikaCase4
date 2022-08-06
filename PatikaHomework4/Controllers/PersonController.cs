using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using PatikaHomework4.Data.Model;
using PatikaHomework4.Dto.Dto;
using PatikaHomework4.Dto.Response;
using PatikaHomework4.Service.IServices;
using System.Text;

namespace PatikaHomework4.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService _personService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _distributedCache;



        public PersonController(IPersonService personService, IMapper mapper,IDistributedCache distributedCache)
        {
            _personService = personService;
            _mapper = mapper;
            _distributedCache = distributedCache;
          

        }


        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retuns data </response>
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<Person>>), StatusCodes.Status200OK)]
        [HttpGet("Redis/{id}")]
        public async Task<IActionResult> GetRedis(int id)
        {
            string cacheKey = id.ToString();
            GenericResponse<IEnumerable<Person>> response = new GenericResponse<IEnumerable<Person>>();
            string json;


            var personsFromCache = await _distributedCache.GetAsync(cacheKey);
            if (personsFromCache != null)
            {
                json = Encoding.UTF8.GetString(personsFromCache);
                var person = JsonConvert.DeserializeObject<List<Person>>(json);
                response.Success = true;
                response.Message = "Cache contains data.";
                response.Data = person;

                return Ok(response);
            }
            else
            {
                var person = await Task.Run(() => _personService.GetAll());
                
                response.Success = true;
                response.Message = "Cache does not contains data.Cache updated.";
                response.Data = person;

                json = JsonConvert.SerializeObject(person);
                personsFromCache = Encoding.UTF8.GetBytes(json);
                var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromDays(1)) // belirli bir süre erişilmemiş ise expire eder
                        .SetAbsoluteExpiration(DateTime.Now.AddMonths(1)); // belirli bir süre sonra expire eder.
                await _distributedCache.SetAsync(cacheKey, personsFromCache, options);
                return Ok(response);
            }
        }




        /// <summary>
        /// Get all
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Retuns data </response>
        [HttpGet]
        [ProducesResponseType(typeof(GenericResponse<IEnumerable<Person>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var person = await Task.Run(() => _personService.GetAll());
            GenericResponse<IEnumerable<Person>> response = new GenericResponse<IEnumerable<Person>>();
            response.Success = true;
            response.Message = null;
            response.Data = person;

            return Ok(response);

        }


        /// <summary>
        /// Get by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Retuns data </response>
        /// <response code="404">Returns error</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
           
            var person = await Task.Run(() => _personService.GetById(id));
            GenericResponse<Person> response = new GenericResponse<Person>();

            if (person == null)
            {
                response.Success = false;
                response.Message = "Does not exist.";
                response.Data = null;
                return NotFound(response);
            }
            response.Success = true;
            response.Message = null;
            response.Data = person;
            return Ok(response);

        }


        /// <summary>
        /// post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Retuns data </response>
        /// <response code="404">Returns error</response>
        /// <response code="400">Returns error</response>
        [HttpPost]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Post(PersonDto model)
        {

            GenericResponse<Person> response = new GenericResponse<Person>();
            var entity = _mapper.Map<PersonDto, Person>(model);

    

            var result = await Task.Run(() => _personService.Add(entity));

            if (result == null)
            {
                response.Success = false;
                response.Message = "An error ocurred.";
                response.Data = null;
                return BadRequest(response);
            }
            response.Success = true;
            response.Message = null;
            response.Data = result;

            return Created("", response);


        }



        /// <summary>
        /// update
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Retuns data </response>
        /// <response code="404">Returns error</response>
        /// <response code="400">Returns error</response>
        [HttpPatch]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Patch(int id,PersonDto model)
        {
            
            GenericResponse<Person> response = new GenericResponse<Person>();
            var person = await Task.Run(() => _personService.GetById(id));
            if (person == null)
            {
                response.Success = false;
                response.Message = "Does not exist. Please check id.";
                response.Data = null;
                return NotFound(response);
            }

            var entity = _mapper.Map<PersonDto, Person>(model);

  
            var result = await Task.Run(() => _personService.Add(entity));

            if (result == null)
            {
                response.Success = false;
                response.Message = "An error occured.";
                response.Data = null;
                return BadRequest(response);
            }

            response.Success = true;
            response.Message = null;
            response.Data = result;
            return Ok(response);

        }


        /// <summary>
        /// post
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="201">Retuns data </response>
        /// <response code="404">Returns error</response>
        /// <response code="400">Returns error</response>
        [HttpPut]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(PersonDto model)
        {
            
            GenericResponse<Person> response = new GenericResponse<Person>();
            var entity = _mapper.Map<PersonDto, Person>(model);
      

            var result = await Task.Run(() => _personService.Add(entity));

            if (result == null)
            {
                response.Success = false;
                response.Message = "An error ocurred.";
                response.Data = null;
                return BadRequest(response);
            }
            response.Success = true;
            response.Message = null;
            response.Data = result;

            return Created("", response);

        }


        /// <summary>
        /// delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Retuns data </response>
        /// <response code="404">Returns error</response>
        [HttpDelete]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResponse<Person>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            
            GenericResponse<String> response = new GenericResponse<String>();
            var person = await Task.Run(() => _personService.GetById(id));
            if (person == null)
            {
                response.Success = false;
                response.Message = "Does not exist.";
                response.Data = null; ;
                return NotFound(response);
            }

            var result = await Task.Run(() => _personService.Delete(person.Id));

            if (result == null)
            {
                response.Success = false;
                response.Message = "Does not exist.";
                response.Data = null; ;
                return NotFound(response);
            }

            response.Success = true;
            response.Message = result;
            response.Data = null;
            return Ok(response);


        }
    }
}
