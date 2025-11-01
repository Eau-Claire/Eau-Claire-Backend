using Microsoft.AspNetCore.Mvc;

namespace FishFarmAPI_v2.Controllers
{
    [ApiController]
    [Route("api/v1/minio")]
    public class MinioController : Controller
    {
        private readonly HttpClient _httpClient;

        public MinioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("{**path}")] //Bất cứ chuỗi nào (kể cả có / trong đó) đi sau /minio/ đều nhét hết vào biến path
        public async Task<IActionResult> GetMinio(string path)
        {
            try
            {
                string minioUrl = $"http://100.98.59.51:9000/{path}";

                var response = await _httpClient.GetAsync(minioUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var content = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType?.ToString();

                return File(content, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
