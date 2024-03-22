using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace Aldentea.AldenteaShuffle.Models
{
	internal class RandomOrg
	{
		const string URL_BASE = "https://www.random.org";
		public static async Task<(string, ImmutableArray<int>)> GetSequences(int length, string seed)
		{
			string path = $"/sequences/?min=0&max={length - 1}&col=1&format=plain&rnd=id.{seed}";
			string path_for_report = $"/sequences/?min=1&max={length}&col=1&format=html&rnd=id.{seed}";
			using (HttpClient client = new() { BaseAddress = new Uri(URL_BASE) })
			{
				var response = await client.GetStringAsync(path);
				return (URL_BASE + path_for_report, response.TrimEnd().Split('\n').Select(s => Convert.ToInt32(s)).ToImmutableArray());

			}
		}
	}

}
