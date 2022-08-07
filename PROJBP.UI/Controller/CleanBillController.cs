using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROJBP.Model;
using PROJBP.Service;
using System.Data.Entity.Infrastructure;

namespace PROJBP.UI.Controllers
{
    [Authorize]
    public class CleanBillController : Controller
    {

        IBillService _billService;
        public CleanBillController(IBillService billService)
        {
            _billService = billService;
        }

        // GET: Bills
        public async Task<IActionResult> Index()
        {
            return View(await _billService.GetAllAsync());
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _billService.GetBillByIdAsync(billid: id ?? 0);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Bills/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Bill bill)
        {
            if (ModelState.IsValid)
            {
                _billService.Create(bill);
                 return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }


        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _billService.GetBillByIdAsync(billid: id ?? 0);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Bills/Edit/5 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillId,BillNumber,BilledDateUTC,CustomerName,CustomerAddress,VATNo,BillStatus")] Bill bill)
        {
            if (id != bill.BillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _billService.Update(bill);

                }
                catch (DbUpdateConcurrencyException)
                {
                    var _bill = await _billService.GetBillByIdAsync(billid: bill.BillId);

                    if (_bill == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }

        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _billService.GetBillByIdAsync(billid: id ?? 0);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var bill = await _billService.GetBillByIdAsync(billid: id);
            if (bill != null)
            {
                _billService.Delete(bill);

            }
             return RedirectToAction(nameof(Index));
        }


    }
}
