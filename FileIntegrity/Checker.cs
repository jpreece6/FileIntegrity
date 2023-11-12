using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileIntegrity
{
    public static class Checker
    {
        public static async Task<List<HashedItem>> GetHashListAsync(string sumListFilePath, CancellationToken token = default)
        {
            if (File.Exists(sumListFilePath) == false)
                throw new ArgumentException($"'{sumListFilePath}' does not exist");

            var ext = Path.GetExtension(sumListFilePath);
            var algo = GetAlgorithmType(ext);

            List<HashedItem> checksums = new List<HashedItem>();
            await foreach (var line in File.ReadLinesAsync(sumListFilePath, token).ConfigureAwait(false))
            {
                var split = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var item = new HashedItem()
                {
                    Hash = ConvertHexStringToByteArray(split[0]),
                    FilePath = split[1],
                    AlgorithmType = algo,
                };

                checksums.Add(item);
            }

            return checksums;
        }

        public static async IAsyncEnumerable<CompareResultObject> VerifyAsync(string sumListFilePath, [EnumeratorCancellation] CancellationToken token = default)
        {
            var hashList = await GetHashListAsync(sumListFilePath, token).ConfigureAwait(false);

            await foreach (var item in VerifyAsync(hashList, token).ConfigureAwait(false))
                yield return item;
        }

        public static async IAsyncEnumerable<CompareResultObject> VerifyAsync(List<HashedItem> files,[EnumeratorCancellation] CancellationToken token = default)
        {
            foreach (var file in files)
            {
                var result = await VerifyAsync(file, token).ConfigureAwait(false);

                yield return new CompareResultObject(file.FilePath, result);
            }
        }

        public static async Task<CompareResult> VerifyAsync(HashedItem hashedItem, CancellationToken token = default)
        {
            if (File.Exists(hashedItem.FilePath))
            {
                var fileHash = await GetFileHashAsync(hashedItem.FilePath, hashedItem.AlgorithmType, token).ConfigureAwait(false);
                var comapreResult = hashedItem.Hash.SequenceEqual(fileHash);

                // Files are not the same
                if (comapreResult == false)
                {
                    return CompareResult.Changed;
                }
                else
                {
                    return CompareResult.BinarySame;
                }
            }
            else
            {
                // File missing
                return CompareResult.Missing;
            }
        }

        private static async Task<byte[]> GetFileHashAsync(string filePath, HashAlgorithmType algorithmType, CancellationToken token = default)
        {
            using (var algorithm = GetHashAlgorithm(algorithmType))
            {
                using (var stream = File.OpenRead(filePath))
                {

                    return await algorithm.ComputeHashAsync(stream, token)
                        .ConfigureAwait(false);
                }
            }
        }

        private static HashAlgorithm GetHashAlgorithm(HashAlgorithmType algorithmType) => algorithmType switch
        {
            HashAlgorithmType.Md5 => MD5.Create(),
            HashAlgorithmType.Sha1 => SHA1.Create(),
            HashAlgorithmType.Sha256 => SHA256.Create(),
            HashAlgorithmType.Sha384 => SHA384.Create(),
            HashAlgorithmType.Sha512 => SHA512.Create(),
            _ => throw new ArgumentException($"Algorithm type '{algorithmType}' is not supported")
        };

        private static HashAlgorithmType GetAlgorithmType(string ext) => ext switch
        {
            ".md5" => HashAlgorithmType.Md5,
            ".sha1" => HashAlgorithmType.Sha1,
            ".sha256" => HashAlgorithmType.Sha256,
            _ => throw new ArgumentException($"File extension '{ext}' is not a recognised hash algorithm")
        };

        // Source https://stackoverflow.com/a/321404/2883194
        internal static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
