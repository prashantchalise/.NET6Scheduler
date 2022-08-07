/*
 Service For Bill
Created by: Prashant
Created On: 30/07/2022
*/
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PROJBP.Model;
using System.Net;
using System.Text;

namespace PROJBP.Service
{
	public interface IBillService : IEntityService<Bill>
	{
		Task<Bill> GetBillByIdAsync(int billid);
		void ProcessBillsDuringOfficeHoursAsync(string _baseUrl, string _token);
	}

	public class BillService : EntityService<Bill>, IBillService
	{

		new IContext _context;
		public BillService(IContext context) : base(context)
		{
			_context = context;
			_dbset = _context.Set<Bill>();
		}

		/// <summary>
		/// Get Bill By Id :: Don't forget to add the DBSet to BabyAZContext
		/// </summary>

		public async Task<Bill> GetBillByIdAsync(int billid)
		{
			return await _dbset.FirstOrDefaultAsync(x => x.BillId == billid);
		}


		/// <summary>
		/// Process bills to _baseurl and by _token
		/// </summary>
		/// <param name="_baseUrl"></param>
		/// <param name="_token"></param>
		public void ProcessBillsDuringOfficeHoursAsync(string _baseUrl, string _token)
		{

			try
			{



				//Office starts at 9:00 AM and ends at 6:00 PM. 
				//Get bills that have status pending; push the information to server;

				//GET BILLS;
				var result = _dbset.Where(x => x.BillStatus == "PENDING").ToList();

				if (result.Count > 0)
				{
					//Convert to JSON; 
					string jsonstr = JsonConvert.SerializeObject(result);
					jsonstr = "{\"obj\": " + jsonstr + "}";

					//POST
					var _responseBody = BILL_POST(jsonstr: jsonstr, url: _baseUrl, token: _token);

					foreach (var _bill in result)
					{
						_bill.BillStatus = "APPROVED";
						int _updated = this.Update(_bill);
					}

				}
				// To get create a new bill with status set to Pending; 
				// This will be pushed during next scheduled time;
				var bill = new Bill() { BillNumber = "Bill# " + Guid.NewGuid().ToString().Replace("-", "").Substring(1, 10), BilledDateUTC = DateTime.UtcNow, CustomerName = "Prashant Chalise", CustomerAddress = "Nepal", VATNo = Guid.NewGuid().ToString().Replace("-", "").Substring(1, 10), BillStatus = "PENDING" };
				int _created = this.Create(bill);

			}
			catch (Exception e)
			{
				//logger.error("");
			}
		}


		//POST
		private string BILL_POST(string jsonstr, string url, string token)
		{

			var handler = new HttpClientHandler
			{
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
			};

			var client = new HttpClient(handler)
			{
				BaseAddress = new Uri(url)
			};

			var webRequest = new HttpRequestMessage(HttpMethod.Post, string.Empty)
			{
				Content = new StringContent(jsonstr, Encoding.UTF8, "application/json")
			};

			webRequest.Headers.Add("Authorization", "Bearer " + token);

			var response = client.Send(webRequest);
			var reader = new StreamReader(response.Content.ReadAsStream());
			var responseBody = reader.ReadToEnd();

			return responseBody;

		}
	}
}
