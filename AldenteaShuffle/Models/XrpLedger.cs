using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace Aldentea.AldenteaShuffle.Models
{
	public class XrpLedger
	{
		Uri Endpoint { get; set; } = new Uri("https://s1.ripple.com:51234/");

		public async Task<(int, string, DateTime)> GetOldestBlock(DateTime after)
		{
			Dictionary<int, (string, DateTime)> blocks = [];

			using (HttpClient client = new() { BaseAddress = Endpoint })
			{

				// とりあえず最新のClosedなブロックを拾う。
				int index = await LedgerClosed(client);
				if (index < 0)
				{
					// これはだめです。
					return (-1, string.Empty, DateTime.UnixEpoch);
				}
				(index, string hash, DateTime time) = await Ledger(client, index);
				if (after > time)
				{
					// まあだだよ。
					return (-2, string.Empty, DateTime.UnixEpoch);
				}

				blocks[index] = (hash, time);
				// 取得したものが、after以降で最後のものかを確認する。

				var delta = (time - after).TotalSeconds;
				int skip = Convert.ToInt32(double.Floor(delta / 10));

				while (skip != 0)
				{
					if (skip < 0)
					{
						index += 1;
					}
					else  // i.e. skip > 0
					{
						index -= 2 * skip;
					}
					(index, hash, time) = await Ledger(client, index);
					blocks[index] = (hash, time);

					delta = (time - after).TotalSeconds;
					skip = Convert.ToInt32(double.Floor(delta / 10));
				}

				// indexに対するtimeはafter以上である。
				if (blocks.ContainsKey(index - 1))
				{
					return (index, blocks[index].Item1, blocks[index].Item2);
				}
				else
				{
					while (time >= after)
					{
						(index, hash, time) = await Ledger(client, index - 1);
						blocks[index] = (hash, time);
					}
					index += 1;
					return (index, blocks[index].Item1, blocks[index].Item2);
				}
			}

		}

		public async Task<(int, string, DateTime)> GetBlock(int index)
		{

			using HttpClient client = new() { BaseAddress = Endpoint };
			return await Ledger(client, index);

		}


		async Task<int> LedgerClosed(HttpClient client)
		{
			//"https://data.xrplf.org/v1/ledgers/ledger_index?date=2024-03-14T06%3A30%3A15.000Z"

			var data = $"{{ \"method\": \"ledger_closed\" }}";
			var response = await client.PostAsync("/", new StringContent(data, Encoding.UTF8, "application/json"));
			//var mojiretsu = await response.Content.ReadAsStringAsync();
			var result = await response.Content.ReadFromJsonAsync<LedgerClosedResponse>();
			if (result != null && result.Result != null)
			{
				return result.Result.LedgerIndex;
			}
			else
			{
				return -1;
			}
			//return mojiretsu;
		}

		async Task<(int, string, DateTime)> Ledger(HttpClient client, int ledger_index)
		{
			// POSTリクエストを送る。
			var data = $"{{ \"method\": \"ledger\", \"params\": [ {{ \"ledger_index\": {ledger_index}, \"full\": false, \"accounts\": false, \"transactions\": false, \"expand\": false, \"owner_funds\": false }} ] }}";
			var response = await client.PostAsync("/", new StringContent(data, Encoding.UTF8, "application/json"));
			//var mojiretsu = await response.Content.ReadAsStringAsync();
			var result = await response.Content.ReadFromJsonAsync<LedgerResponse>();
			if (result != null && result.Result != null && result.Result.Ledger != null)
			{
				return (result.Result.LedgerIndex, result.Result.LedgerHash ?? string.Empty, result.Result.Ledger.CloseTimeIso != null ? DateTime.Parse(result.Result.Ledger.CloseTimeIso) : DateTime.UnixEpoch);
			}
			else
			{
				return (-1, "NOT VALID", DateTime.UnixEpoch);
			}

		}

	}

	class LedgerClosedResponse
	{
		public ClosedResult? Result { get; set; }
	}


	class ClosedResult
	{
		[JsonPropertyName("ledger_index")]
		public int LedgerIndex { get; set; }

		[JsonPropertyName("ledger_hash")]
		public string? LedgerHash { get; set; }
	}

	class LedgerResponse
	{
		public LedgerResult? Result { get; set; }

	}

	class LedgerResult
	{
		[JsonPropertyName("ledger_index")]
		public int LedgerIndex { get; set; }

		[JsonPropertyName("ledger_hash")]
		public string? LedgerHash { get; set; }

		public bool Validated { get; set; }

		[JsonPropertyName("ledger")]
		public LedgerInfo? Ledger { get; set; }

	}

	class LedgerInfo
	{
		[JsonPropertyName("close_time_iso")]
		public string? CloseTimeIso { get; set; }

		[JsonPropertyName("close_time")]
		public long CloseTime { get; set; }

	}

}
