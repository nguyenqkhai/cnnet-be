using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ElearningBackend.DTOs;
using ElearningBackend.Services;

namespace ElearningBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModuleController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<List<ModuleDto>>> GetAllModules()
        {
            try
            {
                var modules = await _moduleService.GetAllModulesAsync();
                return Ok(modules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách module", details = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<ModuleDto>> CreateModule([FromBody] CreateModuleDto createModuleDto)
        {
            try
            {
                var module = await _moduleService.CreateModuleAsync(createModuleDto);
                return Ok(module);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tạo module", details = ex.Message });
            }
        }

        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<List<ModuleDto>>> GetModulesByCourseId(long courseId)
        {
            try
            {
                var modules = await _moduleService.GetModulesByCourseIdAsync(courseId);
                return Ok(modules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách module theo khóa học", details = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin,instructor")]
        public async Task<ActionResult<ModuleDto>> UpdateModule(long id, [FromBody] UpdateModuleDto updateModuleDto)
        {
            try
            {
                var module = await _moduleService.UpdateModuleAsync(id, updateModuleDto);
                return Ok(module);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật module", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteModule(long id)
        {
            try
            {
                var result = await _moduleService.DeleteModuleAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã xóa module thành công" });
                }
                return NotFound(new { message = "Không tìm thấy module" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa module", details = ex.Message });
            }
        }
    }
}
