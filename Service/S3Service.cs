using Amazon.S3;
using Amazon.S3.Model;
using Amazon;
using Microsoft.Extensions.Options;
using CvAPI2.Models;

namespace CvApi2.Service
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucket;

        public S3Service(IOptions<AwsSettings> options)
        {
            var settings = options.Value;
            _bucket = settings.BucketName;

            var regionEndpoint = settings.Region == "us-east-1"
                ? RegionEndpoint.USEast1
                : RegionEndpoint.GetBySystemName(settings.Region);

            var config = new AmazonS3Config
            {
                RegionEndpoint = regionEndpoint
            };

            _s3Client = new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
        }

        public async Task<string> UploadAsync(IFormFile file, string key)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = key,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(request);
            return $"https://{_bucket}.s3.amazonaws.com/{key}";
        }
    }
}
