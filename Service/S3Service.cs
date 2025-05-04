using Amazon.S3;
using Amazon.S3.Model;

namespace CvApi2.Service
{    
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucket;

        public S3Service(IConfiguration config)
        {
            _bucket = config["AWS:Bucket"];
            _s3Client = new AmazonS3Client(
                config["AWS:AccessKey"],
                config["AWS:SecretKey"],
                Amazon.RegionEndpoint.GetBySystemName(config["AWS:Region"])
            );
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