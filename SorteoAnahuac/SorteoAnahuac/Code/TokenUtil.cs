using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System.Configuration;
using System.Web;

namespace SorteoAnahuac.Code
{
    /// <summary>
    /// Clase estatica para inicializar variables de encriptación
    /// </summary>
    public static class ClavesTokenizador
    {
        /// <summary>
        /// Valor de clave IV para encriptar
        /// </summary>
        public static byte[] EncriptIV { get; set; }
        /// <summary>
        /// Valor de clave Key para encriptar
        /// </summary>
        public static byte[] EncriptKey { get; set; }

        /// <summary>
        /// Método que iniciliza los valores de las claves de encriptación
        /// </summary>
        public static void Init()
        {
            byte[] _salt = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Seguridad.Sal"]);

            System.Security.Cryptography.RijndaelManaged aesAlg = null;

            try
            {

                System.Security.Cryptography.Rfc2898DeriveBytes key = new System.Security.Cryptography.Rfc2898DeriveBytes(ConfigurationManager.AppSettings["Seguridad.Secreto"], _salt);

                aesAlg = new System.Security.Cryptography.RijndaelManaged();
                EncriptKey = key.GetBytes(aesAlg.KeySize / 8);
                EncriptIV = key.GetBytes(aesAlg.BlockSize / 8);
            }
            catch
            {

            };
        }
    }

    /// <summary>
    /// Clase que permite generar y desencriptar tokens en formato reducido.
    /// </summary>
    public class TokenUtil
    {
        private readonly Encoding encoding;

        private SicBlockCipher mode;

        private byte[] key;
        private byte[] iv;

        private void init()
        {
            this.mode = new SicBlockCipher(new AesFastEngine());

            // si las claves de encriptación no han sido inicializadas, las calculamos y las almacenamos en los valores estáticos.
            if (ClavesTokenizador.EncriptIV == null)
            {

                byte[] _salt = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["Seguridad.Sal"]);

                System.Security.Cryptography.RijndaelManaged aesAlg = null;

                try
                {
                    // Calculamos los valores de las claves de encriptación para ser usadas en el algoritmo
                    System.Security.Cryptography.Rfc2898DeriveBytes key = new System.Security.Cryptography.Rfc2898DeriveBytes(ConfigurationManager.AppSettings["Seguridad.Secreto"], _salt);

                    aesAlg = new System.Security.Cryptography.RijndaelManaged();
                    this.key = key.GetBytes(aesAlg.KeySize / 8);
                    this.iv = key.GetBytes(aesAlg.BlockSize / 8);

                    // Asignamos las variables de claves de ecnriptación a memoria
                    ClavesTokenizador.EncriptKey = this.key;
                    ClavesTokenizador.EncriptKey = this.iv;
                }
                catch
                {

                };
            } else
            {
                // Obtenemos las variables de las claves del tokenizador de memoria
                this.key = ClavesTokenizador.EncriptKey;
                this.iv = ClavesTokenizador.EncriptIV;
            }
        }

        public TokenUtil(Encoding codigo)
        {
            this.encoding = codigo;
            init();
        }

        public TokenUtil()
        {
            this.encoding = System.Text.Encoding.UTF8;
            init();
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        public static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];

            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }


        public string Encripta(string plain)
        {
            byte[] input = this.encoding.GetBytes(plain);

            byte[] bytes = this.BouncyCastleCrypto(true, input, key, iv);

            string result = ByteArrayToString(bytes);

            return result;
        }


        public string Desencripta(string cipher)
        {
            byte[] bytes = this.BouncyCastleCrypto(false, StringToByteArray(cipher), key, iv);

            string result = this.encoding.GetString(bytes);

            return result;
        }


        private byte[] BouncyCastleCrypto(bool forEncrypt, byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                this.mode.Init(forEncrypt, new ParametersWithIV(new KeyParameter(key), iv));

                BufferedBlockCipher cipher = new BufferedBlockCipher(this.mode);

                return cipher.DoFinal(input);
            }
            catch (CryptoException)
            {
                throw;
            }
        }
    }
}