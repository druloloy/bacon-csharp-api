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
    public class HistoryController : ApiController
    {
        /** Database Instance **/
        BaconEntities  db = new BaconEntities();

        // fetch all history or only active ones
        [HttpGet]
        public IHttpActionResult GetHistories([FromUri] BankHistoryParameters param)
        {
            try
            {
                bool onlyActive = param.OnlyActive;
                var histories = !onlyActive ? 
                                db.histories
                                   .OrderByDescending(h=>h.createdAt)
                                   .ToList():
                                db.histories
                                   .Where(h => h.isArchived == false)
                                   .OrderByDescending(h=>h.createdAt)
                                   .ToList();
        
                return Ok(histories);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return InternalServerError();
            }
        }

        // get specific history
        [HttpGet]
        public IHttpActionResult GetHistory(string id)
        {
            try
            {
                // id from the url is encoded with base64 to ensure security
                string decodedId = ExtraBacon.DecodeBase64(id);

                var history = db.histories
                                   .FirstOrDefault(h => h.id.Equals(decodedId));
                if (history == null) return NotFound();

                return Ok(history);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return InternalServerError();
            }
        }

        // delete history according to its id
        [HttpDelete]
        public IHttpActionResult DeleteHistory(string id)
        {
            try
            {
                // id from the url is encoded with base64 to ensure security
                string decodedId = ExtraBacon.DecodeBase64(id);

                var history = db.histories
                                   .FirstOrDefault(h => h.id.Equals(decodedId));
                if (history == null) return NotFound();

                history.isArchived = true;
                db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return InternalServerError();
            }
        }
        [HttpDelete]
        public IHttpActionResult DeleteHistories()
        {
            try
            {

                var history = db.histories
                                 .Where(h=>h.isArchived==false)
                                 .ToList();
                if (history == null) return NotFound();

                history.ForEach(h => h.isArchived = true);
                db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                return InternalServerError();
            }
        }
    }
}
