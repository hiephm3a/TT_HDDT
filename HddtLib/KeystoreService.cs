using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace HddtLib
{
    public class KeystoreService
    {
        private string Password { set; get; }

        public X509Certificate2 GetCertificateBySerial(string serial)
        {
            try
            {
                X509Store x509Store = new X509Store(StoreLocation.CurrentUser);
                int num = 4;
                x509Store.Open((OpenFlags)num);
                foreach (X509Certificate2 certificate in x509Store.Certificates)
                {
                    if (string.Compare(certificate.GetSerialNumberString().ToUpper(), serial.ToUpper().Trim(), StringComparison.InvariantCultureIgnoreCase) == 0)
                    {
                        if (string.IsNullOrEmpty(this.Password))
                            return certificate;
                        try
                        {
                            CspKeyContainerInfo keyContainerInfo = ((RSACryptoServiceProvider)certificate.PrivateKey).CspKeyContainerInfo;
                            CspParameters parameters = new CspParameters
                            {
                                ProviderName = keyContainerInfo.ProviderName,
                                ProviderType = keyContainerInfo.ProviderType,
                                KeyContainerName = keyContainerInfo.KeyContainerName,
                                KeyNumber = (int)keyContainerInfo.KeyNumber,
                                Flags = CspProviderFlags.UseExistingKey | CspProviderFlags.NoPrompt,
                                KeyPassword = new SecureString()
                            };
                            foreach (char c in this.Password)
                                parameters.KeyPassword.AppendChar(c);
                            RSACryptoServiceProvider cryptoServiceProvider = new RSACryptoServiceProvider(parameters);
                            return new X509Certificate2(certificate.GetRawCertData())
                            {
                                PrivateKey = (AsymmetricAlgorithm)cryptoServiceProvider
                            };
                        }
                        catch
                        {
                            throw new Exception("Không lấy được private key, chọn chứng thư khác.");
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public X509Certificate2 SelectCertificate()
        {
            X509Certificate2 result = null;
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection collection = X509Certificate2UI.SelectFromCollection(
                store.Certificates,
                "Select Certificate",
                "Select Certificate",
                X509SelectionFlag.SingleSelection);
            foreach (X509Certificate2 cert in collection)
                result = cert;
            return result;
        }

        public static string SignHash(X509Certificate2 cert, string hash)
        {
            byte[] rgbHash = Convert.FromBase64String(hash);
            return Convert.ToBase64String(((RSACryptoServiceProvider)cert.PrivateKey).SignHash(rgbHash, "SHA1"));
        }
    }
}
