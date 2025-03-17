using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Data;
using FilmesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]

public class CinemaController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public CinemaController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    /// <summary>
    /// Adiciona um novo cinema.
    /// </summary>
    /// <param name="cinemaDto">Objeto com os campos necessários para criação de um cinema.</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso a inserção seja feita com sucesso.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionaCinema([FromBody] CreateCinemaDto cinemaDto)
    {
        Cinema cinema = _mapper.Map<Cinema>(cinemaDto);
        
        _context.Cinemas.Add(cinema);
        _context.SaveChanges();

        return CreatedAtAction(nameof(RecuperaCinemasPorId), new { Id = cinema.Id }, cinemaDto);
    }

    /// <summary>
    /// Lista todos os cinemas.
    /// </summary>
    /// <param name="enderecoId">Opcional: Filtra cinemas por ID de endereço.</param>
    /// <returns>Lista de cinemas.</returns>
    /// <response code="200">Caso a listagem seja feita com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ReadCinemaDto), StatusCodes.Status200OK)]
    public IEnumerable<ReadCinemaDto> RecuperaCinemas([FromQuery] int? enderecoId = null)
    {
        if(enderecoId == null)
        {
        return _mapper.Map<List<ReadCinemaDto>>(_context.Cinemas.ToList());
        }

        return _mapper.Map<List<ReadCinemaDto>>(_context.Cinemas.FromSqlRaw($"SELECT Id, Nome, EnderecoId FROM cinemas where cinemas.EnderecoId = {enderecoId}").ToList());
    }

    /// <summary>
    /// Busca um cinema pelo ID.
    /// </summary>
    /// <param name="id">Identificador do cinema.</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a busca seja feita com sucesso.</response>
    /// <response code="404">Caso o cinema não seja encontrado.</response>
    [HttpGet("{id}")]
    public IActionResult RecuperaCinemasPorId(int id)
    {
        Cinema cinema = _context.Cinemas.FirstOrDefault(cinema => cinema.Id == id);
       
        if (cinema != null)
        {
            ReadCinemaDto cinemaDto = _mapper.Map<ReadCinemaDto>(cinema);
            return Ok(cinemaDto);
        }

        return NotFound();
    }

    /// <summary>
    /// Atualiza um cinema pelo ID
    /// </summary>
    /// <param name="id">Identificador do cinema.</param>
    /// <param name="cinemaDto">Objeto com os campos que serão alterados.</param>
    /// <response code="204">Sem conteúdo.</response>
    /// <response code="404">Caso o cinema não seja encontrado.</response>
    [HttpPut("{id}")]
    public IActionResult AtualizaCinema(int id, [FromBody] UpdateCinemaDto cinemaDto)
    {
        Cinema cinema = _context.Cinemas.FirstOrDefault(cinema => cinema.Id == id);
        
        if (cinema == null)
        {
            return NotFound();
        }

        _mapper.Map(cinemaDto, cinema);
        _context.SaveChanges();

        return NoContent();
    }

    /// <summary>
    /// Deleta um cinema pelo ID.
    /// </summary>
    /// <param name="id">Identificador do cinema.</param>
    /// <response code="204">Sem conteúdo.</response>
    /// <response code="404">Caso o cinema não seja encontrado.</response>
    [HttpDelete("{id}")]
    public IActionResult DeletaCinema(int id)
    {
        Cinema cinema = _context.Cinemas.FirstOrDefault(cinema => cinema.Id == id);
        
        if (cinema == null)
        {
            return NotFound();
        }

        _context.Remove(cinema);
        _context.SaveChanges();

        return NoContent();
    }

}