using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Data;
using FilmesAPI.Models;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class EnderecoController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public EnderecoController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um novo endereço.
    /// </summary>
    /// <param name="enderecoDto">Objeto contendo os dados do endereço.</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso a inserção seja feita com sucesso.</response>
    [HttpPost]
    public IActionResult AdicionaEndereco([FromBody] CreateEnderecoDto enderecoDto)
    {
        Endereco endereco = _mapper.Map<Endereco>(enderecoDto);
       
        _context.Enderecos.Add(endereco);
        _context.SaveChanges();

        return CreatedAtAction(nameof(RecuperaEnderecosPorId), new { Id = endereco.Id }, endereco);
    }

    /// <summary>
    /// Lista todos os endereços.
    /// </summary>
    /// <response code="200">Caso a listagem seja feita com sucesso.</response>
    [HttpGet]
    public IEnumerable<ReadEnderecoDto> RecuperaEnderecos()
    {
        return _mapper.Map<List<ReadEnderecoDto>>(_context.Enderecos);
    }

    /// <summary>
    /// Busca um endereço pelo seu ID.
    /// </summary>
    /// <param name="id">ID do endereço.</param>
    /// <response code="200">Caso a busca seja feita com sucesso.</response>
    /// <response code="404">Caso o endereco não seja encontrado.</response>
    /// <returns>IActionResult</returns>
    [HttpGet("{id}")]
    public IActionResult RecuperaEnderecosPorId(int id)
    {
        Endereco endereco = _context.Enderecos.FirstOrDefault(endereco => endereco.Id == id);
       
        if (endereco != null)
        {
            ReadEnderecoDto enderecoDto = _mapper.Map<ReadEnderecoDto>(endereco);

            return Ok(enderecoDto);
        }

        return NotFound();
    }

    /// <summary>
    /// Atualiza um endereço pelo ID.
    /// </summary>
    /// <param name="id">ID do endereço.</param>
    /// <param name="enderecoDto">Objeto contendo os novos dados do endereço.</param>
    /// <response code="204">Sem conteúdo.</response>
    /// <response code="404">Caso o endereco não seja encontrado.</response>
    [HttpPut("{id}")]
    public IActionResult AtualizaEndereco(int id, [FromBody] UpdateEnderecoDto enderecoDto)
    {
        Endereco endereco = _context.Enderecos.FirstOrDefault(endereco => endereco.Id == id);
       
        if (endereco == null)
        {
            return NotFound();
        }

        _mapper.Map(enderecoDto, endereco);
        _context.SaveChanges();
        
        return NoContent();
    }

    /// <summary>
    /// Deleta um endereço pelo ID.
    /// </summary>
    /// <param name="id">ID do endereço.</param>
    /// <response code="204">Sem conteúdo.</response>
    /// <response code="404">Caso o endereço não seja encontrado.</response>
    [HttpDelete("{id}")]
    public IActionResult DeletaEndereco(int id)
    {
        Endereco endereco = _context.Enderecos.FirstOrDefault(endereco => endereco.Id == id);
       
        if (endereco == null)
        {
            return NotFound();
        }

        _context.Remove(endereco);
        _context.SaveChanges();

        return NoContent();
    }

}