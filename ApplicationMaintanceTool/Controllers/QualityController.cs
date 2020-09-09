using ApplicationMaintanceTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Routing;

/// <summary>
/// Application Maintaince Tool Controller
/// </summary>
namespace ApplicationMaintanceTool.Controllers
{
    /// <summary>
    /// Quality Information.
    ///  
    /// </summary>
    public class QualityController : ApiController
    {

        /// <summary>
        /// Test Web Api ,To Ensure Web Server is Fine.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Get")]
        public IHttpActionResult Get()
        {
            return Ok(new[] {"Everything is Good"});
        }

        /// <summary>
        /// Update Part Information 
        /// </summary>
        /// <param name="partName"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("api/UpdatePartInformation")]
        public IHttpActionResult UpdatePartInformation([FromUri]string partName)
        {
            PartViewModel model = new PartViewModel();
            model.id = Guid.NewGuid().ToString();
            model.name = partName;
            return Ok(model);
        }
         

        [HttpGet]
        [Route("api/GetAllSession")]
        public IHttpActionResult GetAllSession()
        {
            try
            {
                return Ok(AnalysisSession.GetAll());
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
        }

        [HttpPost]
        [Route("api/StartSession")]
        public IHttpActionResult InitQualityAnalysisSession([FromBody]AnalysisSession session)
        {
            AnalysisSession newQualitySession = null;
            try
            {
                newQualitySession = AnalysisSession.NewSession(session);
                bool isResult = AnalysisSession.SaveSessionToDB(newQualitySession);
                if (!isResult)
                    return InternalServerError(new Exception("Unable To Save Session Object"));
                return Ok(newQualitySession);
            }
            catch (Exception exception)
            {
                return InternalServerError(exception);
            }
            
        }

    }
}
