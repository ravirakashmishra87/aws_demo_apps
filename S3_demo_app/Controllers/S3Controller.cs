using Amazon.S3;
using Amazon.S3.Util;
using Microsoft.AspNetCore.Mvc;

namespace S3_demo_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class S3Controller : ControllerBase
    {
        private readonly IAmazonS3 _amazonS3;

        public S3Controller(IAmazonS3 smazonS3)
        {
            _amazonS3 = smazonS3;
        }
        [HttpPost]
        public async Task<IActionResult> CreateBucketAsync(string bucketName)
        {
            var IsBucketExist = await AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, bucketName);
            if (IsBucketExist)
            {
                return BadRequest($"Bucket with name {bucketName} already exist.");
            }
            await _amazonS3.PutBucketAsync(bucketName);
            return Created("Buckets", $"bucket {bucketName} created. ");
        }
        [HttpGet]
        public async Task<IActionResult> GetAllBuckets()
        {
            var data = await _amazonS3.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => { return b.BucketName; });
            return Ok(buckets);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBucket(string bucketName)
        {
            await _amazonS3.DeleteBucketAsync(bucketName);
            return Ok();
        }

        
    }

   
}
