using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

using Xamarin.Forms;

namespace TextMood
{
	abstract class BaseHttpClientService
	{
		#region Constant Fields
		static readonly Lazy<JsonSerializer> _serializerHolder = new Lazy<JsonSerializer>();
		static readonly Lazy<HttpClient> _clientHolder = new Lazy<HttpClient>(() => CreateHttpClient(TimeSpan.FromSeconds(5)));
		#endregion

		#region Fields
		static int _networkIndicatorCount = 0;
		#endregion

		#region Properties
		static HttpClient Client => _clientHolder.Value;
		static JsonSerializer Serializer => _serializerHolder.Value;
		#endregion

		#region Methods
		protected static async Task<T> GetObjectFromAPI<T>(string apiUrl)
		{
			using (var responseMessage = await GetObjectFromAPI(apiUrl).ConfigureAwait(false))
			{
				try
				{
					UpdateActivityIndicatorStatus(true);

					using (var stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
						return await DeserializeContentStream<T>(stream).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return default;
				}
				finally
				{
					UpdateActivityIndicatorStatus(false);
				}
			}
		}

		protected static async Task<HttpResponseMessage> GetObjectFromAPI(string apiUrl)
		{
			try
			{
				UpdateActivityIndicatorStatus(true);

				return await Client.GetAsync(apiUrl).ConfigureAwait(false);
			}
			catch (Exception e)
			{
				Report(e);
				return null;
			}
			finally
			{
				UpdateActivityIndicatorStatus(false);
			}
		}

		protected static async Task<TResponseData> PostObjectToAPI<TResponseData, TRequestData>(string apiUrl, TRequestData requestData)
		{
			using (var responseMessage = await PostObjectToAPI(apiUrl, requestData).ConfigureAwait(false))
			{
				try
				{
					using (var stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
						return await DeserializeContentStream<TResponseData>(stream).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return default;
				}
			}
		}

		protected static async Task<HttpResponseMessage> PostObjectToAPI<T>(string apiUrl, T requestData)
		{
			var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(requestData)).ConfigureAwait(false);

			using (var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json"))
			{
				try
				{
					UpdateActivityIndicatorStatus(true);

					return await Client.PostAsync(apiUrl, httpContent).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return null;
				}
				finally
				{
					UpdateActivityIndicatorStatus(false);
				}
			}
		}

		protected static async Task<TResponseData> PutObjectToAPI<TResponseData, TRequestData>(string apiUrl, TRequestData requestData)
		{
			using (var responseMessage = await PutObjectToAPI(apiUrl, requestData).ConfigureAwait(false))
			{
				try
				{
					using (var stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
						return await DeserializeContentStream<TResponseData>(stream).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return default;
				}
			}
		}

		protected static async Task<HttpResponseMessage> PutObjectToAPI<T>(string apiUrl, T data)
		{
			var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

			using (var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json"))
			using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, apiUrl) { Content = httpContent })
			{
				try
				{
					UpdateActivityIndicatorStatus(true);

					return await Client.SendAsync(httpRequest).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return null;
				}
				finally
				{
					UpdateActivityIndicatorStatus(false);
				}
			}
		}

		protected static async Task<TResponseData> PatchObjectToAPI<TResponseData, TRequestData>(string apiUrl, TRequestData requestData)
		{
			using (var responseMessage = await PatchObjectToAPI(apiUrl, requestData).ConfigureAwait(false))
			{
				try
				{
					using (var stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
						return await DeserializeContentStream<TResponseData>(stream).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return default;
				}
			}
		}

		protected static async Task<HttpResponseMessage> PatchObjectToAPI<T>(string apiUrl, T data)
		{
			var stringPayload = await Task.Run(() => JsonConvert.SerializeObject(data)).ConfigureAwait(false);

			using (var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json"))
			using (var httpRequest = new HttpRequestMessage(new HttpMethod("PATCH"), apiUrl) { Content = httpContent })
			{
				try
				{
					UpdateActivityIndicatorStatus(true);

					return await Client.SendAsync(httpRequest).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return null;
				}
				finally
				{
					UpdateActivityIndicatorStatus(false);
				}
			}
		}

		protected static async Task<TResponseData> DeleteObjectFromAPI<TResponseData>(string apiUrl)
		{
			using (var responseMessage = await DeleteObjectFromAPI(apiUrl).ConfigureAwait(false))
			{
				try
				{
					using (var stream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
						return await DeserializeContentStream<TResponseData>(stream).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return default;
				}
			}
		}

		protected static async Task<HttpResponseMessage> DeleteObjectFromAPI(string apiUrl)
		{
			using (var httpRequest = new HttpRequestMessage(HttpMethod.Delete, new Uri(apiUrl)))
			{
				try
				{
					UpdateActivityIndicatorStatus(true);

					return await Client.SendAsync(httpRequest).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					Report(e);
					return null;
				}
				finally
				{
					UpdateActivityIndicatorStatus(false);
				}
			}
		}

		protected static void UpdateActivityIndicatorStatus(bool isActivityIndicatorDisplayed)
		{
			if (isActivityIndicatorDisplayed)
			{
				Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsBusy = true);
				_networkIndicatorCount++;
			}
			else if (--_networkIndicatorCount <= 0)
			{
				Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsBusy = false);
				_networkIndicatorCount = 0;
			}
		}

		static HttpClient CreateHttpClient(TimeSpan timeout)
		{
			HttpClient client;

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
				case Device.Android:
					client = new HttpClient();
					break;
				default:
					client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip });
					break;

			}

			client.Timeout = timeout;
			client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}

		static async Task<T> DeserializeContentStream<T>(Stream contentStream)
		{
			using (var reader = new StreamReader(contentStream))
			using (var json = new JsonTextReader(reader))
			{
				if (json == null)
					return default;

				return await Task.Run(() => Serializer.Deserialize<T>(json)).ConfigureAwait(false);
			}
		}

		static void Report(Exception e, [CallerMemberName]string callerMemberName = "") => DebugServices.Report(e, callerMemberName: callerMemberName);
		#endregion
	}
}
