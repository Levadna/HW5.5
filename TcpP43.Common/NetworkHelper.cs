using System.Net.Sockets;

namespace TcpP43.Common
{
    public static class NetworkHelper
    {
        /// <summary>
        /// Sends an integer value over a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task SendIntAsync(NetworkStream stream, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Receives an integer value from a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<int> ReceiveIntAsync(NetworkStream stream)
        {
            byte[] bytes = new byte[4];
            int bytesRead = await stream.ReadAsync(bytes, 0, bytes.Length);
            if (bytesRead != 4)
            {
                throw new InvalidOperationException("Failed to read an integer from the stream.");
            }
            return BitConverter.ToInt32(bytes, 0);
        }
        /// <summary>
        /// Sends a decimal value over a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task SendDecimalAsync(NetworkStream stream, decimal value)
        {
            int[] bits = decimal.GetBits(value);
            byte[] bytes = new byte[16];
            Buffer.BlockCopy(bits, 0, bytes, 0, 16);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// Receives a decimal value from a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<decimal> ReceiveDecimalAsync(NetworkStream stream)
        {
            byte[] bytes = new byte[16];
            int bytesRead = await stream.ReadAsync(bytes, 0, bytes.Length);
            if (bytesRead != 16)
            {
                throw new InvalidOperationException("Failed to read a decimal from the stream.");
            }
            int[] bits = new int[4];
            Buffer.BlockCopy(bytes, 0, bits, 0, 16);
            return new decimal(bits);
        }
        /// <summary>
        /// Sends a double value over a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task SendDoubleAsync(NetworkStream stream, double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// Receives a double value from a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<double> ReceiveDoubleAsync(NetworkStream stream)
        {
            byte[] bytes = new byte[8];
            int bytesRead = await stream.ReadAsync(bytes, 0, bytes.Length);
            if (bytesRead != 8)
            {
                throw new InvalidOperationException("Failed to read a double from the stream.");
            }
            return BitConverter.ToDouble(bytes, 0);
        }

        /// <summary>
        /// Sends an array of integers over a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static async Task SendIntArrayAsync(NetworkStream stream, int[] values)
        {
            await SendIntAsync(stream, values.Length);

            foreach (int value in values)
            {
                await SendIntAsync(stream, value);
            }
        }

        /// <summary>
        /// Receives an array of integers from a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<int[]> ReceiveIntArrayAsync(NetworkStream stream)
        {
            int length = await ReceiveIntAsync(stream);
            int[] values = new int[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = await ReceiveIntAsync(stream);
            }
            return values;
        }


        /// <summary>
        /// Sends a long (Int64) value over a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task SendInt64Async(NetworkStream stream, long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
        /// <summary>
        /// Receives a long (Int64) value from a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<long> ReceiveInt64Async(NetworkStream stream)
        {
            byte[] bytes = new byte[8];
            int bytesRead = await stream.ReadAsync(bytes, 0, bytes.Length);
            if (bytesRead != 8)
            {
                throw new InvalidOperationException("Failed to read a long from the stream.");
            }
            return BitConverter.ToInt64(bytes, 0);
        }


        /// <summary>
        /// Sends a string value over a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task SendStringAsync(NetworkStream stream, string value)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(value);
            await SendIntAsync(stream, bytes.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Receives a string value from a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task<string> ReceiveStringAsync(NetworkStream stream)
        {
            int length = await ReceiveIntAsync(stream);

            int bufferSize = 8192; // 8 KB buffer size
            byte[] buffer = new byte[bufferSize];

            using var memoryStream = new System.IO.MemoryStream();
            int totalBytesRead = 0;

            while (totalBytesRead < length)
            {
                int bytesToRead = Math.Min(bufferSize, length - totalBytesRead);
                int bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead);
                if (bytesRead == 0)
                {
                    throw new InvalidOperationException("Failed to read the complete string from the stream.");
                }
                memoryStream.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
            }
            return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        /// <summary>
        /// Sends a file over a NetworkStream asynchronously.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public static async Task SendFileAsync(NetworkStream stream, string filePath)
        {
            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException($"File not found: {filePath}");
            }

            // Send the file name
            var fileName = System.IO.Path.GetFileName(filePath);
            await SendStringAsync(stream, fileName);

            // Send the file size
            var fileInfo = new System.IO.FileInfo(filePath);
            await SendInt64Async(stream, fileInfo.Length);

            // Send the file content in chunks
            using var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await stream.WriteAsync(buffer, 0, bytesRead);
            }
        }
        /// <summary>
        /// Receives a file from a NetworkStream asynchronously and saves it to the specified destination directory.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static async Task ReceiveFileAsync(NetworkStream stream, string destinationDirectory, string? fileName = null)
        {
            // Receive the file name
            var fileNameFromSender = await ReceiveStringAsync(stream);
            var destinationFilePath = System.IO.Path.Combine(destinationDirectory, fileName ?? fileNameFromSender);
            // Receive the file size
            var fileSize = await ReceiveInt64Async(stream);
            // Receive the file content in chunks
            using var fileStream = new System.IO.FileStream(destinationFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            byte[] buffer = new byte[8192];
            long totalBytesReceived = 0;
            while (totalBytesReceived < fileSize)
            {
                int bytesToRead = (int)Math.Min(buffer.Length, fileSize - totalBytesReceived);
                int bytesRead = await stream.ReadAsync(buffer, 0, bytesToRead);
                if (bytesRead == 0)
                {
                    throw new InvalidOperationException("Failed to read the complete file from the stream.");
                }
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                totalBytesReceived += bytesRead;
            }
        }

        /// <summary>
        /// Sends an object of type T over a NetworkStream asynchronously using JSON serialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static async Task SendObjectAsync<T>(NetworkStream stream, T obj)
        {
            string jsonString = System.Text.Json.JsonSerializer.Serialize(obj);
            await SendStringAsync(stream, jsonString);
        }

        /// <summary>
        /// Receives an object of type T from a NetworkStream asynchronously using JSON deserialization.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<T> ReceiveObjectAsync<T>(NetworkStream stream)
        {
            string jsonString = await ReceiveStringAsync(stream);
            return System.Text.Json.JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
