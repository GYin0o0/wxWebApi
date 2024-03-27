using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;

namespace wxWebApi.Utils
{
    public class QiniuUtils
    {
        private readonly Mac _mac;
        private readonly PutPolicy _putPolicy;
        private readonly Config _config;
        private readonly string _bucket;

        public QiniuUtils(string accessKey, string secretKey, string bucket)
        {
            _mac = new Mac(accessKey, secretKey);
            _bucket = bucket;
            _putPolicy = new PutPolicy()
            {
                Scope = _bucket
            };
            _putPolicy.SetExpires(3600);
            _putPolicy.DeleteAfterDays = 7;

            _config = new Config
            {
                // 设置区域，根据你的七牛云账户配置来设置  
                Zone = Zone.ZoneCnSouth,
                UseHttps = true,
                UseCdnDomains = true,
                ChunkSize = ChunkUnit.U512K
            };
        }

        public async Task<string> UploadFileAsync(string localFilePath, string key)
        {
            string token = Auth.CreateUploadToken(_mac, _putPolicy.ToJsonString());

            FormUploader target = new(_config);

            HttpResult result = await target.UploadFile(localFilePath, key, token, null);

            return result.ToString();
        }

        public string DownloadFile(string filePath)
        {
            string domain = "http://sawchsl81.hn-bkt.clouddn.com";
            string key = filePath;
            string privateUrl = DownloadManager.CreatePrivateUrl(_mac, domain, key);
            return privateUrl;
        }
    }
}