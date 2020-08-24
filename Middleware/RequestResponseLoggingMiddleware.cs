using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace CRhodan.Api.Core.Middleware
{
// https://exceptionnotfound.net/using-middleware-to-log-requests-and-responses-in-asp-net-core/
// https://gist.github.com/elanderson/c50b2107de8ee2ed856353dfed9168a2
// https://stackoverflow.com/a/52328142/3563013
// https://stackoverflow.com/a/43404745/3563013
// https://gist.github.com/elanderson/c50b2107de8ee2ed856353dfed9168a2#gistcomment-2319007
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly Action<RequestProfilerModel> _requestResponseHandler;
        private const int ReadChunkBufferLength = 4096;

        public RequestResponseLoggingMiddleware(RequestDelegate next,
            Action<RequestProfilerModel> requestResponseHandler)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _requestResponseHandler = requestResponseHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            var model = new RequestProfilerModel
            {
                RequestTime = new DateTimeOffset(),
                Context = context,
                Request = await FormatRequest(context)
            };

            var originalBody = context.Response.Body;

            await using var newResponseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = newResponseBody;

            await _next(context);

            newResponseBody.Seek(0, SeekOrigin.Begin);
            await newResponseBody.CopyToAsync(originalBody);

            newResponseBody.Seek(0, SeekOrigin.Begin);
            model.Response = FormatResponse(context, newResponseBody);
            model.ResponseTime = new DateTimeOffset();
            _requestResponseHandler(model);
        }

        private static string FormatResponse(HttpContext context, MemoryStream newResponseBody)
        {
            var request = context.Request;
            var response = context.Response;

            return $"Http Response Information: \r" +
                   $"Schema:{request.Scheme} \r" +
                   $"Host: {request.Host} \r" +
                   $"Path: {request.Path} \r" +
                   $"QueryString: {request.QueryString} \r" +
                   $"StatusCode: {response.StatusCode} \r" +
                   $"Response Body: {ReadStreamInChunks(newResponseBody)}";
        }

        private async Task<string> FormatRequest(HttpContext context)
        {
            var request = context.Request;

            return $"Http Request Information: \r" +
                   $"Schema:{request.Scheme} \r" +
                   $"Host: {request.Host} \r" +
                   $"Path: {request.Path} \r" +
                   $"QueryString: {request.QueryString} \r" +
                   $"Request Body: {await GetRequestBody(request)}";
        }

        public async Task<string> GetRequestBody(HttpRequest request)
        {
            request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await request.Body.CopyToAsync(requestStream);
            request.Body.Seek(0, SeekOrigin.Begin);
            return ReadStreamInChunks(requestStream);
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            string result;
            using (var textWriter = new StringWriter())
            {
                using var reader = new StreamReader(stream);
                var readChunk = new char[ReadChunkBufferLength];
                int readChunkLength;
                //do while: is useful for the last iteration in case readChunkLength < chunkLength
                do
                {
                    readChunkLength = reader.ReadBlock(readChunk, 0, ReadChunkBufferLength);
                    textWriter.Write(readChunk, 0, readChunkLength);
                } while (readChunkLength > 0);

                result = textWriter.ToString();
            }

            return result;
        }

        public class RequestProfilerModel
        {
            public DateTimeOffset RequestTime { get; set; }
            public HttpContext Context { get; set; }
            public string Request { get; set; }
            public string Response { get; set; }
            public DateTimeOffset ResponseTime { get; set; }
        }
    }
}