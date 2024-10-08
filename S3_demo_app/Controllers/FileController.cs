using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Mvc;
using S3_demo_app.Models.Dtos;

namespace S3_demo_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public readonly IAmazonS3 _amazonS3;
        public FileController(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(IFormFile file, string bucketName, string prefix)
        {
            var IsBucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!IsBucketExist)
            {
                return NotFound($"{bucketName} not found to upload file.");
            }

            var request = new PutObjectRequest()
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix?.TrimEnd('/')}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            await _amazonS3.PutObjectAsync(request);
            return Ok($"{prefix}/{file.FileName} uploaded successuly to S3 bucket.");
        }


        [HttpGet]
        public async Task<IActionResult> GetAllFilesAsync(string bucketName, string prefix)
        {
            var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExist)
                return NotFound($"{bucketName} does not exist.");
            var request = new ListObjectsV2Request()
            {
                BucketName = bucketName,
                Prefix = prefix
            };

            var result = await _amazonS3.ListObjectsV2Async(request);

            var s3Objects = result.S3Objects.Select(x =>
            {
                var urlRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = x.BucketName,
                    Key = x.Key,
                    Expires = DateTime.Now.AddMinutes(2)
                };
                return new S3DTO()
                {
                    Name = x.Key,
                    PresignedUrl = _amazonS3.GetPreSignedURL(urlRequest)
                };
            });
            return Ok(s3Objects);
        }


        [HttpGet("preview")]
        public async Task<IActionResult> GetFileByKeyAsync(string bucketName, string key)
        {
            var bucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (!bucketExist)
                return NotFound($"{bucketName} does not exist.");
            var s3Object = await _amazonS3.GetObjectAsync(bucketName, key);
            return File(s3Object.ResponseStream, s3Object.Headers.ContentType);
        }

        [HttpDelete]
        public async Task<IActionResult>DeleteFileAsync(string bucketName,string key)
        {
            var bucketExist =await  AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3,bucketName);
            if(!bucketExist) return NotFound($"{bucketExist} does not exist");

            await _amazonS3.DeleteObjectAsync(bucketName,key);
            return NoContent();

        }
    }
}
