using BaconAPI.Models;
using BaconAPI.Functions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace BaconAPI.Controllers
{ 
    [EnableCors(origins:"*", headers:"*", methods:"*")]
    public class BankController : ApiController
    {
        /** Database Instamce **/
        private BaconEntities db = new BaconEntities();
        [HttpGet]
        public IHttpActionResult GetBanks()
        {
            try
            {
                var banks = db.banks
                              .ToList();

                return Ok(banks);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        [HttpGet]
        public IHttpActionResult GetBank(string id)
        {
            try
            {
                // id from the url is encoded with base64 to ensure security
                string decodedId = ExtraBacon.DecodeBase64(id); 

                var bank = db.banks
                              .FirstOrDefault(b => b.id.Equals(decodedId));
                    
                if (bank == null) return NotFound();

                return Ok(bank);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }
        [HttpPost]
       public IHttpActionResult AddBank(bank newBank)
       {
            try
            {
                // generate unique id for bank and history
                string bankId = ExtraBacon.GenId(BaconSize.MEDIUM);
                string historyId = ExtraBacon.GenId(BaconSize.MEDIUM);
                // assign id for the new bank
                newBank.id = bankId;


                bool doExist = db
                                .banks
                                .Where(bank => bank.id.Equals(newBank.id))
                                .FirstOrDefault() != null;

                if (doExist)
                    return BadRequest("Already Exists!");



                // create history
                var history = new history();
                history.id = historyId;
                history.bank_id = bankId;
                history.hist_label = newBank.name;
                history.hist_type = HistoryType.Created.Value;
                history.initialAmount = history.finalAmount = newBank.amount;
                history.createdAt = DateTime.Now;
                history.isArchived = false;

                db.banks.Add(newBank); // add new bank to db
                db.histories.Add(history); // add history of creation

                db.SaveChangesAsync();

                return StatusCode(HttpStatusCode.Created);
            }
            catch (DbEntityValidationException e)
            {
                var errs = e.EntityValidationErrors.ToList();
                string errorMessage = errs[0].ValidationErrors.ToList()[0].ErrorMessage;

                errs.ForEach(err =>
                {
                    var validationErrors = err.ValidationErrors.ToList();
                    validationErrors.ForEach(er =>
                    {
                        Debug.WriteLine($"property_name: {er.PropertyName}; errorMessage: {er.ErrorMessage}");
                    });
                });
                var errObj = new
                {
                    id = ExtraBacon.GenId(BaconSize.SMALL),
                    message = errorMessage,
                    code = HttpStatusCode.BadRequest,
                    stack = e.EntityValidationErrors.ToList()
                };
                return Content(HttpStatusCode.BadRequest, errObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        public async Task<IHttpActionResult> UpdateBank(UpdateBankParameter bank)
        {
            try
            {
                if (bank == null) throw new ArgumentNullException();
                bank existingBank = db.banks
                                        .SingleOrDefault(b => b.id.Equals(bank.id));

                if (existingBank == null)
                    return NotFound();

                // calculate new amount
                double? newAmount = existingBank.amount + bank.amount;

                // create history
                var hist = new history();
                hist.id = ExtraBacon.GenId(BaconSize.MEDIUM);
                hist.bank_id = existingBank.id;
                hist.hist_label = existingBank.name;
                hist.hist_type = HistoryType.Modified.Value;
                hist.reason = bank.reason;
                hist.initialAmount = existingBank.amount;
                hist.finalAmount = newAmount;
                hist.createdAt = DateTime.Now;
                hist.isArchived = false;

                db.histories.Add(hist);


                existingBank.name = bank.name;
                existingBank.amount = newAmount;
                existingBank.updatedAt = DateTime.Now;

                
                await db.SaveChangesAsync();
            
                return Ok("Bank updated");
            }
            catch (DbUpdateException e)
            {
               
                Debug.WriteLine(e.InnerException);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
            catch (DbEntityValidationException e)
            {
                var errs = e.EntityValidationErrors.ToList();
                string errorMessage = errs[0].ValidationErrors.ToList()[0].ErrorMessage;

                errs.ForEach(err =>
                {
                    var validationErrors = err.ValidationErrors.ToList();
                    validationErrors.ForEach(er =>
                    {
                        Debug.WriteLine($"property_name: {er.PropertyName}; errorMessage: {er.ErrorMessage}");
                    });
                });
                var errObj = new
                {
                    id = ExtraBacon.GenId(BaconSize.SMALL), 
                    message = errorMessage,
                    code = HttpStatusCode.BadRequest,
                    stack = e.EntityValidationErrors.ToList()
                };
                return Content(HttpStatusCode.BadRequest, errObj);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return StatusCode(HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        public IHttpActionResult DeleteBank(string id, [FromBody] string reason)
        {

            try
            {
                // id from the url is encoded with base64 to ensure security
                string decodedId = ExtraBacon.DecodeBase64(id);

                var existingBank = db.banks
                                      .Where(b => b.id.Equals(decodedId))
                                      .FirstOrDefault();

                if (existingBank == null)
                    return NotFound();

                // create history
                var hist = new history();
                hist.id = ExtraBacon.GenId(BaconSize.MEDIUM);
                hist.bank_id = existingBank.id;
                hist.hist_label = existingBank.name;
                hist.hist_type = HistoryType.Deleted.Value;
                hist.reason = reason;
                hist.initialAmount = existingBank.amount;
                hist.finalAmount = 0;
                hist.createdAt = DateTime.Now;
                hist.isArchived = false;

                db.histories.Add(hist);

                db.banks.Remove(existingBank);
                db.SaveChangesAsync();
                
                return Ok();
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return Content(HttpStatusCode.InternalServerError, "Server error.");
            }
        }
    }
}
