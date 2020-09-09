using ApplicationMaintanceTool.DAL;
using ApplicationMaintanceTool.Models;
using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Web.Http;

/**
 * Application Maintaince Tool
 */
namespace ApplicationMaintanceTool.Controllers
{


    public class ImpactAnalysisController : ApiController
    {

        private Item ExecuteServerMethodCodeWhereUsed(Innovator innovator, string methodName , InputRequest request)
        {
            Item qItem = innovator.newItem("Method","get");
            qItem.setAttribute("select","keyed_name,method_type");
            qItem.setAttribute("where",string.Format
                (@"[Method].method_code like '%{0}%' and keyed_name != '{0}'",methodName));
            Item methodItems = qItem.apply();

            if (methodItems.isError())
                return null;

            Item resultItem = innovator.newItem();

            int count = methodItems.getItemCount();
            for (int index = 0; index < count; index++)
            {
                string name = methodItems.getItemByIndex(index).getProperty("keyed_name");
                request.iServerMethod = name;
                Item rItem = GetAnalysis(innovator,request);
                if (!resultItem.isError())
                    resultItem.appendItem(rItem);
            }

            if (!methodItems.isError())
                resultItem.appendItem(methodItems); 

            return resultItem;
        }


        private string GetMethodId(Innovator innovator, string methodName)
        {
            string sql = string.Format
                (@"select id from innovator.[Method] where is_current = '1' and keyed_name = '{0}'", methodName);
            string methodId = innovator.applySQL(sql).getProperty("id");
            return methodId;
        }



        private Item ExecuteLifeCycleWhereUsed(Innovator innovator, string methodId)
        {
            Item lcEvents = innovator.newItem("Life Cycle Transition","get");
            lcEvents.setAttribute("select","source_id,from_state,to_state");
            lcEvents.setAttribute("where",
                string.Format("[life_cycle_transition].pre_action='{0}' or post_action='{0}'",methodId));
            Item rItem = lcEvents.apply();
            if (rItem.isError())
                return null;
            int count = rItem.getItemCount();
            for (int index = 0; index < count; index++)
            {
                Item indexItem = rItem.getItemByIndex(index);
                Item qItem = innovator.newItem("ItemType Life Cycle", "get");
                qItem.setProperty("related_id",indexItem.getProperty("source_id"));
                qItem.setAttribute("select","source_id,related_id");
                Item lcMaps = qItem.apply();
                if (lcMaps.isError())
                    break;
                indexItem.setProperty("itemtype",lcMaps.getPropertyAttribute("source_id", "keyed_name"));
            }

            return rItem;
        }

        private Item GetAnalysis(Innovator innovator,InputRequest request)
        {
            string methodId = GetMethodId(innovator, request.iServerMethod);
            Item apmItem = innovator.newItem();
            Item resultItem = ExecuteServerMethodWhereUsed(innovator, methodId);
            if (!resultItem.isError())
                apmItem.appendItem(resultItem);

            Item lcResultItem = ExecuteLifeCycleWhereUsed(innovator, methodId);
            if (lcResultItem != null && !lcResultItem.isError())
                apmItem.appendItem(lcResultItem);

            Item methodItems = ExecuteServerMethodCodeWhereUsed(innovator, request.iServerMethod, request);
            if (methodItems != null && !methodItems.isError())
                apmItem.appendItem(methodItems);

            return apmItem;
        }


        private Item ExecuteServerMethodWhereUsed(Innovator innovator, string methodId)
        {
            Item serverEvents = innovator.newItem("Server Event","get");
            serverEvents.setProperty("related_id",methodId);
            serverEvents.setAttribute("select","source_id,server_event");
            return serverEvents.apply();
        }

        [HttpGet]
        [Route("api/GetImpactAnalysis")]
        public IHttpActionResult GetImpactAnalysis([FromBody]InputRequest request)
        {
            ArasConnectionManager manager = null;
            try
            {
                manager =
                    new ArasConnectionManager
                        (request.iArasUrl, request.iArasDatabase, request.iUserId, request.iPassword);

                if (request.iServerMethod == null || request.iServerMethod.Length <= 0)
                    throw new InvalidOperationException("Invalid Server Method ,Please Send Correct Server Method");

                string methodId = GetMethodId(manager.innovator, request.iServerMethod);
                Item apmItem = manager.innovator.newItem();
                Item resultItem = ExecuteServerMethodWhereUsed(manager.innovator, methodId);
                if (!resultItem.isError())
                    apmItem.appendItem(resultItem);

                Item lcResultItem = ExecuteLifeCycleWhereUsed(manager.innovator, methodId);
                if ( lcResultItem != null && !lcResultItem.isError())
                    apmItem.appendItem(lcResultItem);

                Item methodItems = ExecuteServerMethodCodeWhereUsed(manager.innovator, request.iServerMethod, request);
                if (methodItems != null && !methodItems.isError())
                    apmItem.appendItem(methodItems);

                if (apmItem.nodeList == null)
                    return Ok(apmItem.node);
                else
                    return Ok(apmItem.nodeList);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            finally
            {
                if (manager != null) manager.LogoutArasConnection();
            }
        }

    }
}