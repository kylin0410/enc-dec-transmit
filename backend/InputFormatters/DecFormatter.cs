using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace DecBackEnd.InputFormatters
{
    /// <summary>
    /// 解析Content Type為"application/octet-stream"或者無Content Type的HTTP要求，將內容解壓縮並且使用Base64解碼，還原成原來的字串。
    /// Allows for a single input parameter in the form of:
    /// 
    /// public byte[] RawData([FromBody] byte[] data)
    /// </summary>
    public class DecFormatter : InputFormatter
    {
        private string pri_key = @"-----BEGIN RSA PRIVATE KEY-----
MIICXAIBAAKBgQDHlHdML1YgmUtXFEfyIOK+l8wHMHM16i2rFnRKSZQSSn1hndCe
zd/dVI/Z1ozFK1TL4q3jiqmAwYVb6VspgjeiA+2yn9JqgRYVhIH1Dl7xC1x1Bi/H
kbnFfjbMHv3GXGYetlvhLVTu8qke6Z4cGGhWqtw2MdkJTuaEbKPcYr8X4QIDAQAB
AoGBAMVnDdBnCbNYrHJG9xqAeIW3svBxdaMgK2eL4B4SRMkKsJU6+Tv5ubE7kMUQ
N1BWGZtIbGIhpqJZx0QvviGCUO7HReF68W0VSJe6RrVlpVbeitkFMOFu+R3KeKEt
XsKqHHCjmpAFbSwnR1Vq4TtYCOiEweLeabSneo7etUMuIVQhAkEAyoGjSYZWISBS
HGQjsDJlYbmBlP9SHjguCs2lcnm5a7LGl8K+dMN8GqjP8jNVAQspDAseYfa/DKVv
groqkUEMvQJBAPxM7cHRDTdIo4pe60QM8wYI8FR4daQuzcleeT7QJSNRaj0W+hM3
EzTPrKBtgnzBt3mi/Zp8yhmgjtq/Q5apc/UCQFzb/WhltXETRRPHx6WwNlUNn6IX
QiyhTlud2VQZBTGhlPdaUcNxMKN47YH+j+gemf/vyUravtips+yaOZLJ5XECQDkq
MQCmHil1guB6KzIrAPFQGyv4cyc1F5lVl4Ec5h0/eCPJTfYGl4pyt3lN9q/PsIOV
44IaXiw6TcPQbD75u/UCQBKEPU6OQAQYehH6ygAM6lr2AP4aNC0iur8mLpbxuglZ
kM4GQeNPwfuloLlO1yNnVuXYA9zVWjzbOuW3ACqtMnw=
-----END RSA PRIVATE KEY-----";
        //private IBufferedCipher dec_cipher;
        private AsymmetricKeyParameter priKey;

        public DecFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));
            // 載入私鑰.
            AsymmetricCipherKeyPair keyPair = ReadPem(this.pri_key);
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
            Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            this.priKey = PrivateKeyFactory.CreateKey(PrivateKeyInfo.GetInstance(asn1ObjectPrivate));
        }

        /// <summary>
        /// Allow application/octet-stream and no content type to be processed
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Boolean CanRead(InputFormatterContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var contentType = context.HttpContext.Request.ContentType;
            //if (string.IsNullOrEmpty(contentType) || contentType == "application/octet-stream")
            if (contentType == "application/octet-stream" || contentType == "application/gzip")
                return true;

            return false;
        }

        /// <summary>
        /// Handle application/octet-stream row data.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var contentType = context.HttpContext.Request.ContentType;
            if (contentType != "application/octet-stream" && contentType != "application/gzip")
            {
                return await InputFormatterResult.FailureAsync();
            }

            IServiceProvider serviceProvider = context.HttpContext.RequestServices;
            var logger = serviceProvider.GetService(typeof(ILogger<DecFormatter>)) as ILogger;
            var request = context.HttpContext.Request;


            var buffer = new MemoryStream();
            if (contentType == "application/octet-stream")
            {
                logger.LogDebug("解密資料");
                Thread.Sleep(100);

                // 載入私鑰.
                var dec_cipher = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
                dec_cipher.Init(false, this.priKey); // false表示解密

                // 分段解密.
                byte[] block = new byte[128];
                while (request.Body.Read(block, 0, block.Length) == block.Length)
                {
                    var decrypt = dec_cipher.DoFinal(block);
                    buffer.Write(decrypt, 0, decrypt.Length);
                }
            }
            else // "application/gzip" case
            {
                logger.LogDebug("不解密資料");
                await request.Body.CopyToAsync(buffer);
            }

            // 重置位置才能讓GZipStream讀取到資料
            buffer.Seek(0, SeekOrigin.Begin);
            //logger.LogDebug("stream length: " + buffer.ToArray().Length);

            // Decompress row data and decode it by base64, return origin string.
            var decompressor = (Stream)new GZipStream(buffer, CompressionMode.Decompress, true);
            var ms = new MemoryStream();
            await decompressor.CopyToAsync(ms);
            var base64Str = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            var originStr = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Str));
            if (string.IsNullOrEmpty(originStr))
                logger.LogError("Got empty result.");
            // 資源回收
            if (decompressor != null)
                decompressor.Dispose();
            if (ms != null)
                ms.Dispose();
            if (buffer != null)
                buffer.Dispose();
            return await InputFormatterResult.SuccessAsync(originStr);
        }

        static AsymmetricCipherKeyPair ReadPem(string pem)
        {
            using (TextReader reader = new StringReader(pem))
            {
                var obj = new Org.BouncyCastle.OpenSsl.PemReader(reader).ReadObject();
                return obj as AsymmetricCipherKeyPair;
            }
        }
    }
}
