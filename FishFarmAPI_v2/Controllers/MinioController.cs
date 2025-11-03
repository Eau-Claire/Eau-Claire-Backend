using Microsoft.AspNetCore.Mvc;
using Minio;

namespace FishFarmAPI_v2.Controllers
{
    [ApiController]
    [Route("api/v1/minio")]
    public class MinioController : Controller
    {
        private readonly HttpClient _httpClient;

        public MinioController(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient();
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        [HttpGet("{**path}")] //Bất cứ chuỗi nào (kể cả có / trong đó) đi sau /minio/ đều nhét hết vào biến path
        public async Task<IActionResult> GetMinio(string path)
        {
            try
            {
                var minioUrl = $"http://10.24.0.175:9000/{path}";

                // Chỉ đọc header, tai ve dan dan
                using var response = await _httpClient.GetAsync(minioUrl, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, $"Failed to fetch from MinIO: {response.StatusCode}");
                }

                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                var contentStream = await response.Content.ReadAsStreamAsync();

                return File(contentStream, contentType);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(502, $"Cannot reach MinIO: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return StatusCode(504, "Request to MinIO timed out.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}