namespace EntertainmentHub.Services.Data.DataAPI
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;

    using EntertainmentHub.Services.Data.DataAPI.DataModels;

    public class DataService : IDataService
    {
        // It is better to use HttpClient, because it supports asynchronous operations. WebClient is obsolete.
        private const string BaseUrl = "https://api.themoviedb.org/3";
        private const string ApiKey = "6d0b8982d8293f80a3cc09d6224deee8";
        private readonly HttpClient client = new HttpClient();

        public async Task<MovieDTO> GetMovieDataAsync(int movieId)
        {
            using HttpResponseMessage response = await this.client.GetAsync($"{BaseUrl}/movie/{movieId}?api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using HttpContent content = response.Content;

                    return await content.ReadFromJsonAsync<MovieDTO>();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("An error occurred.");
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine("The content type is not supported.");
                }
                catch (JsonException)
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }

            return null;
        }

        public async Task<TrailerDTO> GetMovieTrailersAsync(int movieId)
        {
            using HttpResponseMessage response = await this.client.GetAsync($"{BaseUrl}/movie/{movieId}/videos?api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using HttpContent content = response.Content;

                    return await content.ReadFromJsonAsync<TrailerDTO>();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("An error occurred.");
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine("The content type is not supported.");
                }
                catch (JsonException)
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }

            return null;
        }

        public async Task<CastAndCrewDTO> GetCastAndCrewAsync(int movieId)
        {
            using HttpResponseMessage response = await this.client.GetAsync($"{BaseUrl}/movie/{movieId}/credits?api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using HttpContent content = response.Content;

                    return await content.ReadFromJsonAsync<CastAndCrewDTO>();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("An error occurred.");
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine("The content type is not supported.");
                }
                catch (JsonException)
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }

            return null;
        }

        public async Task<ActorDTO> GetActorAsync(int actorId)
        {
            using HttpResponseMessage response = await this.client.GetAsync($"{BaseUrl}/person/{actorId}?api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using HttpContent content = response.Content;

                    return await content.ReadFromJsonAsync<ActorDTO>();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("An error occurred.");
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine("The content type is not supported.");
                }
                catch (JsonException)
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }

            return null;
        }

        public async Task<SlideshowDTO> GetMoviePhotoSlidesAsync(int movieId)
        {
            using HttpResponseMessage response = await this.client.GetAsync($"{BaseUrl}/movie/{movieId}/images?api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using HttpContent content = response.Content;

                    return await content.ReadFromJsonAsync<SlideshowDTO>();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("An error occurred.");
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine("The content type is not supported.");
                }
                catch (JsonException)
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }

            return null;
        }

        public async Task<MovieReviewDTO> GetMovieReviewAsync(int movieId)
        {
            using HttpResponseMessage response = await this.client.GetAsync($"{BaseUrl}/movie/{movieId}/reviews?api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using HttpContent content = response.Content;

                    return await content.ReadFromJsonAsync<MovieReviewDTO>();
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("An error occurred.");
                }
                catch (NotSupportedException)
                {
                    Console.WriteLine("The content type is not supported.");
                }
                catch (JsonException)
                {
                    Console.WriteLine("Invalid JSON.");
                }
            }

            return null;
        }
    }
}
