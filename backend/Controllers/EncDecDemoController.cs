using DecBackEnd.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System.IO;

namespace DecBackEnd.Controllers
{
    /// <summary>
    /// 加解密測試Demo用API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LoggingFilter))]
    public class EncDecDemoController : ControllerBase
    {
        private readonly ILogger _logger;
        private string pub_key =
@"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDHlHdML1YgmUtXFEfyIOK+l8wH
MHM16i2rFnRKSZQSSn1hndCezd/dVI/Z1ozFK1TL4q3jiqmAwYVb6VspgjeiA+2y
n9JqgRYVhIH1Dl7xC1x1Bi/HkbnFfjbMHv3GXGYetlvhLVTu8qke6Z4cGGhWqtw2
MdkJTuaEbKPcYr8X4QIDAQAB";
         private string pri_key =
 @"-----BEGIN RSA PRIVATE KEY-----
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

        public EncDecDemoController(ILogger<EncDecDemoController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// C#後端加解密測試
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ServiceFilter(typeof(ExcpFilter))]
        public IActionResult Get()
        {
            string plain_text = "hello world";

            // 加密
            byte[] pubkey = System.Convert.FromBase64String(pub_key);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(pubkey);
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(SubjectPublicKeyInfo.GetInstance(pubKeyObj));
            IBufferedCipher cipher1 = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
            cipher1.Init(true, pubKey);//true表示加密  
            byte[] enc_utf8 = System.Text.Encoding.UTF8.GetBytes(plain_text);
            byte[] encoded_data = cipher1.DoFinal(enc_utf8);

            // 解密
            var keyPair = ReadPem(pri_key);
            _logger.LogError(keyPair.ToString());
            AsymmetricKeyParameter private_key = keyPair.Private;
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(private_key);
            Asn1Object asn1ObjectPrivate = privateKeyInfo.ToAsn1Object();
            AsymmetricKeyParameter priKey = PrivateKeyFactory.CreateKey(PrivateKeyInfo.GetInstance(asn1ObjectPrivate));
            IBufferedCipher cipher = CipherUtilities.GetCipher("RSA/ECB/PKCS1Padding");
            cipher.Init(false, priKey); //false表示解密  
            var dec_utf8 = cipher.DoFinal(encoded_data);
            var answer = System.Text.Encoding.UTF8.GetString(dec_utf8);

            var result = new
            {
                plain_text,
                enc_utf8 = (enc_utf8),
                encoded_data = (encoded_data),
                encoded_len = (encoded_data.Length),
                dec_utf8 = (dec_utf8),
                answer
            };
            return Ok(result);
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
