using BaconAPI.Functions;
using BaconAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BaconAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BankHistoryController : ApiController
    {
        BaconEntities db = new BaconEntities();

        // fetches history of a bank
        [HttpGet]
        public IHttpActionResult GetBankHistory(string id, [FromUri] BankHistoryParameters param)
        {
            try
            {
                // id from the url is encoded with base64 to ensure security
                string decodedId = ExtraBacon.DecodeBase64(id);
                uint limit, offset;
                bool onlyActive;

                // get parameters

                limit = param.Limit;
                offset = param.Offset;
                onlyActive = param.OnlyActive;

                var history = db.histories
                                   .Where(h => (onlyActive == true ?
                                                (h.bank_id.Equals(decodedId) && h.isArchived == false) :
                                                h.bank_id.Equals(decodedId)))
                                   .OrderByDescending(h => h.createdAt)
                                   .Skip((int)offset)
                                   .Take((int)limit)
                                   .ToList();

                if (history == null) return NotFound();

                return Ok(history);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return InternalServerError();
            }
        }

        // archives an individual or mass history from a bank's history
        [HttpDelete]
        public IHttpActionResult DeleteHistory(string id, [FromUri] BankHistoryParameters param)
        {
            try
            {
                // id from the url is encoded with base64 to ensure security
                string decodedId = ExtraBacon.DecodeBase64(id);
                uint limit, offset;
                bool onlyActive;

                // get parameters

                limit = param.Limit;
                offset = param.Offset;
                onlyActive = param.OnlyActive;

                var history = db.histories
                                   .Where(h =>(onlyActive==true ? 
                                                (h.bank_id.Equals(decodedId) && h.isArchived == false):
                                                h.bank_id.Equals(decodedId)))
                                   .OrderByDescending(h => h.createdAt)
                                   .Skip((int)offset)
                                   .Take((int)limit)
                                   .ToList();

                if (history == null) return NotFound();

                history.ForEach(h => h.isArchived = true);

                db.SaveChangesAsync();
                return Ok(history);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return InternalServerError();
            }
        }
    }
}
