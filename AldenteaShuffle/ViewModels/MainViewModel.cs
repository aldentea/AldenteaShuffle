﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Immutable;
using System.Reflection;
using System.Security.Policy;

namespace Aldentea.AldenteaShuffle.ViewModels
{

	public partial class MainViewModel : ObservableObject
	{
		#region フォントサイズ関連

		[ObservableProperty]
		public double _fontSize = 16;

		[RelayCommand]
		void FontSizeUp()
		{
			FontSize += 2;
		}

		[RelayCommand]
		void FontSizeDown()
		{
			FontSize = double.Max(FontSize - 2, 8.0);
		}

		#endregion

/*
		#region ユーザエントリー関連

		// これモデル？


		//public IRelayCommand EntryPlayerCommand { get; private set; }

		[NotifyCanExecuteChangedFor(nameof(ShufflePlayersCommand))]
		[ObservableProperty]
		List<string> _sortedPlayersList = [];

		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(PlayersList))]
		string _entryList = string.Empty;

		List<string> PlayersList
		{
			get
			{
				return EntryList.Split("\n").Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
			}
		}

		[RelayCommand]
		private void SortPlayers()
		{
			SortedPlayersList = [.. PlayersList.Order()];
			//SortedPlayersList = [.. PlayersList.Order()];
			// クリップボードへの積み込みを行う。
			string message = $"{Application.Current.FindResource("FinalizingPlayersMessage")}\n```\n{string.Join("\n", SortedPlayersList)}\n```";
			Clipboard.SetText(message);
		}

		#endregion


		public MainViewModel()
		{
			var now = DateTime.Now;
			this._promisedTime = new DateTime(now.Ticks - now.Ticks % (10000000L * 60 * 30) + (10000000L * 60 * 30));

			RetrieveHashCommand = new AsyncRelayCommand(RetrieveHash);
			//EntryPlayerCommand = new RelayCommand(EntryPlayer, CanEntryPlayer);
			ShufflePlayersCommand = new AsyncRelayCommand(ShufflePlayers, CanShufflePlayers);
		}

		#region ハッシュ値取得関連

		[ObservableProperty]
		DateTime _promisedTime;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(ShufflePlayersCommand))]
		public string _sourceHash = string.Empty;

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(RetrieveHashFromIDCommand))]
		public int _blockIndex = 80000000;


		private async Task RetrieveHash()
		{
			var model = new Models.XrpLedger();
			(int index, string hash, DateTime closed_time) = await model.GetOldestBlock(PromisedTime);
			BlockIndex = index;
			if (index > 0)
			{
				SourceHash = hash;
				var url = $"https://xrpscan.com/ledger/{index}";
				string message = string.Format((string)Application.Current.FindResource("RetrievedHashMessage"),index, hash, closed_time,url);
				Clipboard.SetText(message);
			}
			else
			{
				SourceHash = index switch
				{
					-2 => (string)Application.Current.FindResource("TooEarlyMessage"),
					_ => $"★Something is wrong. {index}",
				};
			}

		}

		public IAsyncRelayCommand RetrieveHashCommand { get; private set; }

		[RelayCommand(CanExecute = nameof(CanRetrieveHashFromID))]
		async Task RetrieveHashFromID()
		{
			if (CanRetrieveHashFromID())
			{
				var model = new Models.XrpLedger();
				(int index, string hash, DateTime closed_time) = await model.GetBlock(BlockIndex);
				BlockIndex = index;
				if (index > 0)
				{
					SourceHash = hash;
					var url = $"https://xrpscan.com/ledger/{index}";
					string message = string.Format((string)Application.Current.FindResource("RetrievedHashMessage"), index, hash, closed_time, url);
					Clipboard.SetText(message);
				}

			}
		}

		bool CanRetrieveHashFromID()
		{
			return BlockIndex > 0;
		}

		#endregion

		#region シャッフル関連


		async Task ShufflePlayers()
		{
			if (CanShufflePlayers())
			{
				// いわゆるB方式でプレイヤーを割り当てる。
				(var url, var random) = await Models.RandomOrg.GetSequences(SortedPlayersList.Count, SourceHash);
				ShuffledPlayersList = random.Select(i => SortedPlayersList[i]).ToImmutableList();

				string message = $"{Application.Current.FindResource("ShuffledPlayersMessage")}\n```\n{string.Join("\n", ShuffledPlayersList)}\n```\n{Application.Current.FindResource("SequencesUrlIsHereMessage")}\n{url}";
				Clipboard.SetText(message);
			}
		}
		bool CanShufflePlayers()
		{
			return SortedPlayersList.Count > 1 && !string.IsNullOrWhiteSpace(SourceHash);
		}


		public IAsyncRelayCommand ShufflePlayersCommand { get; set; }


		//IRelayCommand GetShuffledPlayersListTSVCommand { get; set; }

		[RelayCommand(CanExecute = nameof(CanGetShuffledPlayersListTSV))]
		void GetShuffledPlayersListTSV()
		{
			if (CanGetShuffledPlayersListTSV())
			{
				string message = string.Join("\n", ShuffledPlayersList).Replace('/', '\t');
				Clipboard.SetText(message);
			}
		}

		bool CanGetShuffledPlayersListTSV()
		{
			return ShuffledPlayersList.Count > 0;
		}

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(GetShuffledPlayersListTSVCommand))]
		System.Collections.Immutable.ImmutableList<string> _shuffledPlayersList = [];



		#endregion
*/
	}
}
