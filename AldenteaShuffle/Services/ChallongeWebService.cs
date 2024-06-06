using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;  // for MediaTypeWithQualityHeaderValue.
using System.Text.Json.Serialization;
using System.Xml.Linq;
using System.Text.RegularExpressions;



namespace Aldentea.AldenteaShuffle.Services
{

	// このあたり、ChallongeSolkoffからコピペ。

	public class ChallongeWebService : IChallongeWebService
	{
		private static readonly HttpClient client = new HttpClient();


		#region *トーナメントから参加者を取得(GetParticipants)
		/// <summary>
		/// 指定したトーナメントのすべての参加者の情報を取得します。
		/// </summary>
		/// <param name="tournamentID">トーナメントID</param>
		/// <param name="userName">Challogeユーザ名</param>
		/// <param name="apiKey">ChallongeのAPIキー</param>
		/// <returns></returns>
		/* public async Task<IEnumerable<ParticipantItem>> GetParticipants(string tournamentID, string userName, string apiKey)
		{
			// コピペの巻、
			var base_uri = $@"https://api.challonge.com/v1/tournaments/{tournamentID}/participants.json";
			var request = new HttpRequestMessage(HttpMethod.Get, base_uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{apiKey}"))
			);
			var response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				using (var stream = new System.IO.MemoryStream(await response.Content.ReadAsByteArrayAsync()))
				{
					return await System.Text.Json.JsonSerializer.DeserializeAsync<List<ParticipantItem>>(stream);
				}
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}

		}
		*/
		#endregion



		#region *トーナメントに参加者を追加(AddParticipant)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="participantName"></param>
		/// <param name="tournamentID"></param>
		/// <param name="userName"></param>
		/// <param name="apiKey"></param>
		/// <param name="misc"></param>
		/// <returns></returns>
		/* public async Task<ParticipantItem> AddParticipant(string participantName, string tournamentID, string userName, string apiKey, string? misc = null)
		{
			var base_uri = $@"https://api.challonge.com/v1/tournaments/{tournamentID}/participants.json";
			var request = new HttpRequestMessage(HttpMethod.Post, base_uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{apiKey}"))
			);

			var misc_string = string.IsNullOrEmpty(misc) ? string.Empty : $@", ""misc"": ""{misc}""";
			var json_string = $@"{{ ""participant"": {{ ""name"": ""{participantName}""{misc_string} }} }}";
			request.Content = new StringContent(json_string, Encoding.UTF8, "application/json");
			var response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				using (var stream = new System.IO.MemoryStream(await response.Content.ReadAsByteArrayAsync()))
				{
					return await System.Text.Json.JsonSerializer.DeserializeAsync<ParticipantItem>(stream);
				}
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}

		}
		*/
		#endregion

		public async Task BulkEntryParticipants(IList<string> participantNames, string tournamentID, string userName, string apiKey)
		{
			var base_uri = $@"https://api.challonge.com/v1/tournaments/{tournamentID}/participants/bulk_add.json";
			var request = new HttpRequestMessage(HttpMethod.Post, base_uri);
			request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(Encoding.ASCII.GetBytes($"{userName}:{apiKey}"))
			);

			//var misc_string = string.IsNullOrEmpty(misc) ? string.Empty : $@", ""misc"": ""{misc}""";

			// [ { "name":"～～～"}, { "name":"～～～～"} ] みたいな文字列を渡す。
			var participants_list = string.Join(',', participantNames.Select(p => $@"{{ ""name"": ""{p}""}}"));
			var json_string = $@"{{""participants"": [ {participants_list} ]}}";
			request.Content = new StringContent(json_string, Encoding.UTF8, "application/json");
			var response = await client.SendAsync(request);
			if (response.StatusCode == System.Net.HttpStatusCode.OK)
			{
				// ※何が返ってくるの？

				//using (var stream = new System.IO.MemoryStream(await response.Content.ReadAsByteArrayAsync()))
				//{
				//	return await System.Text.Json.JsonSerializer.DeserializeAsync<ParticipantItem>(stream);
				//}
			}
			else
			{
				throw new Exception(response.StatusCode.ToString());
			}

		}

	}


	public interface IChallongeWebService
	{
		Task BulkEntryParticipants(IList<string> participantNames, string tournamentID, string userName, string apiKey);
		//Task<IEnumerable<ParticipantItem>> GetParticipants(string tournamentID, string userName, string apiKey);
		//Task<ParticipantItem> AddParticipant(string participantName, string tournamentID, string userName, string apiKey, string misc = null);
	}
}

