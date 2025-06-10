using AutoMapper;
using Infractructure.EF;
using KickstarterAPI.Dto.Kickstarter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KickstarterAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KickstarterController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public KickstarterController(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResponse<KickstarterProjectDto>>> GetProjects(int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            return BadRequest("Page number and size must be greater than 0.");

        var totalCount = await _context.Kickstarters.CountAsync();
        var projects = await _context.Kickstarters
            .OrderBy(p => p.ID)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new PaginatedResponse<KickstarterProjectDto>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Data = _mapper.Map<List<KickstarterProjectDto>>(projects)
        };

        return Ok(result);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<KickstarterProjectDto>> GetProject(long id)
    {
        var project = await _context.Kickstarters.FindAsync(id);
        if (project == null)
            return NotFound();
        
        var result = _mapper.Map<KickstarterProjectDto>(project);
        
        return result;
    }
    
    [HttpPost]
    [Authorize(Policy = "Bearer")]
    public async Task<ActionResult<KickstarterProjectDto>> CreateProject(KickstarterCreateDto dto)
    {
        var project = _mapper.Map<KickstarterEntity>(dto);

        _context.Kickstarters.Add(project);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<KickstarterProjectDto>(project);

        return CreatedAtAction(nameof(GetProject), new { id = project.ID }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Bearer")]
    public async Task<IActionResult> UpdateProject(long id, KickstarterCreateDto dto)
    {
        var existing = await _context.Kickstarters.FindAsync(id);
        if (existing == null)
            return NotFound();

        _mapper.Map(dto, existing);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id}")]
    [Authorize(Policy = "Bearer")]
    public async Task<IActionResult> DeleteProject(long id)
    {
        var project = await _context.Kickstarters.FindAsync(id);
        if (project == null)
            return NotFound();

        _context.Kickstarters.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}