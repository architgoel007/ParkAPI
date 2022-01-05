using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkAPI.Models;
using ParkAPI.Models.Dtos;
using ParkAPI.Repository.IRepository;
using System.Collections.Generic;

namespace ParkAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _nprepo;
        private readonly IMapper _mapper;
        public NationalParksController(INationalParkRepository nprepo, IMapper mapper)
        {
            _nprepo = nprepo;
            _mapper = mapper;
        }

        /// <summary>
        /// Get List of National Parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(List<NationalParkDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetNationalParks()
        {
            var objList = _nprepo.GetNationalParks();
            var objDto = new List<NationalParkDTO>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDTO>(obj));
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Get Individual National Parks
        /// </summary>
        /// <param name="nationalParkId"> The Id of National Parks </param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name= "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int nationalParkId) 
        {
            var obj = _nprepo.GetNationalPark(nationalParkId);
            if(obj == null)
            {
                return NotFound();
            }
            var objDto = _mapper.Map<NationalParkDTO>(obj);
            return Ok(objDto);
        }
       
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateNationalPark([FromBody] NationalParkDTO nationalParkDTO)
       {
            if (nationalParkDTO == null) 
            {
                return BadRequest(ModelState);
            }

            if (_nprepo.NationalParkExists(nationalParkDTO.Name))
            {
                ModelState.AddModelError("", "National Park Exists!");
                return StatusCode(404, ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);

            if (!_nprepo.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something Went Wrong When Saving the Record {nationalParkDTO.Name}");
                return StatusCode(500, ModelState);
            }
            //for 200 OK
            //return Ok();
            //for 201 created
            return CreatedAtRoute("GetNationalPark", new { nationalParkId = nationalParkObj.Id }, nationalParkObj);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDTO nationalParkDTO)
        {
            if (nationalParkDTO == null || nationalParkId != nationalParkDTO.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDTO);

            if (!_nprepo.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something Went Wrong When Updating the Record {nationalParkDTO.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_nprepo.NationalParkExists(nationalParkId))
            {
                return NotFound();
            }

            var nationalParkObj = _nprepo.GetNationalPark(nationalParkId);

            if (!_nprepo.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Something Went Wrong When Deleting the Record {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}