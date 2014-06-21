﻿using KanColle;
using KanColle.Member;
using KanColle.Request.Map;
using KanColle.Request.Sortie;
using KanColle.Request.Hokyu;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace Sparkler {

	class Sparkler {

		private const int ONE_SECOND = 1 * 1000;
		private const int FIVE_SECONDS = 5 * 1000;
		private const int TEN_SECONDS = 10 * 1000;

		private KanColleProxy kcp;
		private int fleet_id, run_times;
		private string member_id, ship_list;

		static void Main (string[] args) {
			Console.OutputEncoding = Encoding.Unicode;

			StreamReader reader = new StreamReader("api_token.txt");
			string full_api_token = reader.ReadLine();
			reader.Close();

			int fleet_id = Convert.ToInt32(args[0]),
				run_times = Convert.ToInt32(args[1]);

			Sparkler sparkler = new Sparkler(full_api_token, fleet_id, run_times);
			sparkler.run();
			Console.Read();
		}

		public Sparkler (string full_api_token, int fleet_id, int run_times) {
			this.kcp = new KanColleProxy(full_api_token);
			this.fleet_id = Math.Abs(fleet_id);
			this.run_times = Math.Abs(run_times);

			this.member_id = getMemberId();
			this.ship_list = getShipList(this.fleet_id);
			/*
			StreamReader reader = new StreamReader("DATA/dat.txt", Encoding.Unicode);
			string readin = reader.ReadLine();
			reader.Close();

			KanColleAPI<KanColle.Master.Start2> api_data = JsonConvert.DeserializeObject<KanColleAPI<KanColle.Master.Start2>>(readin);

			foreach (KanColle.Master.MapArea map in api_data.GetData().api_mst_maparea) {
				StreamWriter stream = new StreamWriter("DATA/MAPAREA.txt", true, Encoding.UTF8);
				stream.WriteLine(map);
				stream.Flush();
				stream.Close();
			}

			foreach (KanColle.Master.MapInfo map in api_data.GetData().api_mst_mapinfo) {
				StreamWriter stream = new StreamWriter("DATA/MAPINFO.txt", true, Encoding.UTF8);
				stream.WriteLine(map);
				stream.Flush();
				stream.Close();
			}*/
		}

		public void run () {
			
			int run_count = 0;
			while (this.run_times != run_count) {
				this.kcp.proxy(MapInfo.MAPINFO);
				Console.WriteLine("MapInfo done.");
				this.kcp.proxy(MapCell.MAPCELL, MapCell.Get(1, 1));
				Console.WriteLine("MapCell done.");

				this.kcp.proxy(Map.START, Map.Start(1, this.fleet_id, 1, 1));
				Console.WriteLine("STARTED.");
				Thread.Sleep(ONE_SECOND);

				this.kcp.proxy(Sortie.BATTLE, Sortie.Battle(1, 0));
				Console.WriteLine("First battle done.");
				Thread.Sleep(TEN_SECONDS);
				this.kcp.proxy(Sortie.BATTLERESULT);
				Console.WriteLine("First battle results get.");
				Thread.Sleep(FIVE_SECONDS);

				this.kcp.proxy(Ship3.SHIP2, Ship3.Ship2());
				Console.WriteLine("Ship2 get.");
				this.kcp.proxy(Map.NEXT, Map.Next());
				Console.WriteLine("NEXT.");
				Thread.Sleep(ONE_SECOND);

				this.kcp.proxy(Sortie.BATTLE, Sortie.Battle(1, 0));
				Console.WriteLine("Second battle done.");
				Thread.Sleep(TEN_SECONDS);
				this.kcp.proxy(Sortie.BATTLERESULT);
				Console.WriteLine("Second battle results get.");
				Thread.Sleep(FIVE_SECONDS);

				this.kcp.proxy(ApiPort.PORT, ApiPort.port(this.member_id));
				Console.WriteLine("Returned to port.");

				this.kcp.proxy(Hokyu.CHARGE, Hokyu.Charge(this.ship_list, ChargeKind.BOTH));
				Console.WriteLine("Refueled.");
				Thread.Sleep(ONE_SECOND);

				Console.WriteLine("ROUND {0} COMPLETE.", ++run_count);
			}
		}

		private string getShipList (int fleetNum) {
			String result = this.kcp.proxy(ApiPort.PORT, ApiPort.port(this.member_id));
			KanColleAPI<Port> api_data = JsonConvert.DeserializeObject<KanColleAPI<Port>>(result);
			try {
				return api_data.GetData().GetFleetList(fleetNum);
			} catch (Exception e) {
				Console.WriteLine(result);
				Console.WriteLine(e.Message);
				throw new Exception(e.Message, e);
			}
		}

		private string getMemberId () {
			String result = this.kcp.proxy(Basic.GET);
			KanColleAPI<Basic> api_data = JsonConvert.DeserializeObject<KanColleAPI<Basic>>(result);
			try {
				return api_data.GetData().api_member_id;
			} catch (Exception e) {
				Console.WriteLine(result);
				Console.WriteLine(e.Message);
				throw new Exception(e.Message, e);
			}
		}
	}
}