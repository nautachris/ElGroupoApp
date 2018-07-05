using ElGroupo.Domain;
using ElGroupo.Web.Models.Activities;
using ElGroupo.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Controllers
{
    [Authorize]
    [Route("Activities")]
    public class ActivitiesController : ControllerBase
    {
        private LookupTableService _lookupTableService = null;
        private ActivitiesService _activitiesService = null;
        public ActivitiesController(UserManager<User> userMgr, ActivitiesService activitiesService, LookupTableService lts) : base(userMgr)
        {

            _activitiesService = activitiesService;
            _lookupTableService = lts;
        }
        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await CurrentUser();
            //await this.userManager.AddToRoleAsync(user, "admin");
            var model = _activitiesService.GetDashboardModel(user.Id, HttpContext.User.IsInRole("admin"));
            return View(model);
        }

        [HttpGet("ActivityGroup/Create/{groupId}")]
        public async Task<IActionResult> CreateActivityGroup([FromRoute]long groupId)
        {
            //when clicking "Other" within a user group listing
            var user = await CurrentUser();
            var model = _activitiesService.GetCreateActivityGroupModel(user.Id, groupId);
            return View(model);
        }

        [HttpGet("AddUserToActivity/{activityId}")]
        public async Task<IActionResult> AddUserToActivity([FromRoute]long activityId)
        {
            //this will just bring you to a page when you can log the number of credits received, and maybe the time?
            return Ok();
        }
        [HttpGet("ActivityGroup/Create")]
        public async Task<IActionResult> CreateActivityGroup()
        {
            //when clicking "Create New" (or something) outside of a group in the dashbiard
            var user = await CurrentUser();
            var model = _activitiesService.GetCreateActivityGroupModel(user.Id);
            return View(model);
        }

        [HttpGet("Activity/Create/{groupId}")]
        public async Task<IActionResult> CreateActivity(long groupId)
        {
            //creates a new activity within a group - i.e. tumor board wednesday
            //when clicking "Create New" (or something) outside of a group in the dashbiard
            var user = await CurrentUser();
            var model = _activitiesService.GetCreateActivityModel(user.Id, groupId);
            return View(model);
        }

        [HttpGet("Group/{activityGroupId}")]
        public async Task<IActionResult> Group([FromRoute]long activityGroupId)
        {
            //when clicking an existing group on the dashboard
            //will take you to the list of existing activities within said group
            var user = await CurrentUser();
            var model = _activitiesService.GetActivityGroupActivitiesModel(activityGroupId, user.Id, HttpContext.User.IsInRole("admin"));
            return View("ViewGroup", model);

        }

        [HttpGet("AddAttendenceLog/{activityId}")]
        public async Task<IActionResult> AddAttendenceLog([FromRoute]long activityId)
        {
            //when clicking an existing group on the dashboard
            //will take you to the list of existing activities within said group
            var user = await CurrentUser();
            var model = _activitiesService.GetCreateAttendenceLogModel(activityId, user.Id, user.TimeZoneId);
            return View("AddAttendenceLog", model);

        }
        [HttpGet("EditAttendenceLog/{userActivityId}")]
        public async Task<IActionResult> EditAttendenceLog([FromRoute]long userActivityId)
        {
            //when clicking an existing group on the dashboard
            //will take you to the list of existing activities within said group
            var user = await CurrentUser();
            var model = _activitiesService.GetEditAttendenceLogModel(userActivityId, user.Id, user.TimeZoneId);
            return View("EditAttendenceLog", model);

        }

        [HttpDelete("DeleteDocument/{userActivityDocumentId}")]
        public async Task<IActionResult> DeleteDocument([FromRoute]long userActivityDocumentId)
        {
            var response = await _activitiesService.DeleteDocument(userActivityDocumentId);
            if (response.Success)
            {
                var userActivityId = Convert.ToInt64(response.ResponseData);
                var docs = _activitiesService.GetDocumentsByUserActivityId(userActivityId);
                return PartialView("_ActivityDocumentTable", docs);

            }
            else return BadRequest();
        }


        [HttpGet("GetDocument/{documentId}")]
        public async Task<IActionResult> GetDocument([FromRoute]long documentId)
        {
            var doc = _activitiesService.GetDocument(documentId);
            if (doc == null) return BadRequest();

            return File(doc.ImageData, doc.ContentType);
        }
        
        [HttpGet("GetActivityDocument/{documentId}")]
        public async Task<IActionResult> GetActivityDocument([FromRoute]long documentId)
        {
            var doc = _activitiesService.GetActivityDocument(documentId);
            if (doc == null) return BadRequest();

            return File(doc.ImageData, doc.ContentType);
        }

        [HttpGet("DownloadRecords")]
        public async Task<IActionResult> DownloadRecords()
        {
            var user = await CurrentUser();
            var ms = await _activitiesService.GetMyRecordsDownloadStream(user.Id, user.TimeZoneId);
            return File(ms, "text/plain", "scatterbrainrecords.csv");
        }

        [HttpPost("UploadDocuments")]
        public async Task<IActionResult> UploadDocuments()
        {
            try
            {
                if (Request.Form.Files.Count == 0) return BadRequest();

                if (!Request.Form.Keys.Contains("userActivityId")) return BadRequest("userActivityId was not included in the form data");
                var userActivityId = Convert.ToInt64(Request.Form["userActivityId"]);
                var returnTable = Request.Form.Keys.Contains("returnTable");
                var response = await _activitiesService.SaveDocument(ParseDocumentRequest(Request));
                if (response.Success)
                {
                    if (returnTable)
                    {
                        var docs = _activitiesService.GetDocumentsByUserActivityId(userActivityId);
                        return PartialView("_ActivityDocumentTable", docs);
                    }
                    else
                    {
                        return Ok();
                    }

                }
                else return BadRequest(new { error = response.ErrorMessage });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

        }

        private SaveDocumentsModel ParseDocumentRequest(HttpRequest request)
        {
            var model = new SaveDocumentsModel();
            model.UserActivityId = Convert.ToInt64(request.Form["userActivityId"]);
            foreach (var file in request.Form.Files)
            {
                var doc = new DocumentListModel();
                //i.e. file_0
                var id = file.Name.Substring(5, 1);
                if (request.Form.ContainsKey("description_" + id)) doc.Description = request.Form["description_" + id].ToString();
                doc.ContentType = file.ContentType;
                doc.FileName = file.FileName;
                var ms = new MemoryStream();
                file.CopyTo(ms);
                doc.Data = ms.ToArray();
                if (request.Form["type_" + id].ToString() == "personal") doc.Shared = false;
                else doc.Shared = true;
                model.Documents.Add(doc);
            }
            return model;
        }

        [HttpPost("ActivityGroup/Create")]
        public async Task<IActionResult> CreateActivityGroup([FromBody]CreateActivityGroupModel model)
        {
            var user = await CurrentUser();
            var response = await _activitiesService.SaveNewActivityGroup(model, user.Id);
            if (response.Success) return Ok(new { userActivityId = Convert.ToInt64(response.ResponseData) });
            else return BadRequest(new { error = response.ErrorMessage });
        }

        [HttpPost("ActivityGroup/Update")]
        public async Task<IActionResult> UpdateActivityGroup([FromBody]CreateActivityGroupModel model)
        {
            var user = await CurrentUser();
            var response = await _activitiesService.SaveNewActivityGroup(model, user.Id);
            if (response.Success) return Ok(new { userActivityId = Convert.ToInt64(response.ResponseData) });
            else return BadRequest(new { error = response.ErrorMessage });
        }
        [HttpGet("ActivityGroup/Update/{activityGroupId}")]
        public async Task<IActionResult> UpdateActivityGroup([FromRoute]long activityGroupId)
        {
            return BadRequest();
            //var user = await CurrentUser();
            //var response = await _activitiesService.SaveNewActivityGroup(model, user.Id);
            //if (response.Success) return Ok(new { userActivityId = Convert.ToInt64(response.ResponseData) });
            //else return BadRequest(new { error = response.ErrorMessage });
        }
        [HttpDelete("ActivityGroup/Delete/{activityGroupId}")]
        public async Task<IActionResult> DeleteActivityGroup([FromRoute]long activityGroupId)
        {
            var user = await CurrentUser();
            var response = await _activitiesService.DeleteActivityGroup(activityGroupId, user.Id, HttpContext.User.IsInRole("admin"));
            if (response.Success) return Ok();
            else return BadRequest(new { error = response.ErrorMessage });
        }

        [HttpPost("Activity/Create")]
        public async Task<IActionResult> CreateActivity([FromBody]CreateActivityModel model)
        {
            var user = await CurrentUser();
            var response = await _activitiesService.SaveNewActivity(model, user.Id);
            if (response.Success) return Ok(new { userActivityId = Convert.ToInt64(response.ResponseData) });
            else return BadRequest(new { error = response.ErrorMessage });
        }

        [HttpGet("Records")]
        public async Task<IActionResult> Records()
        {
            var user = await CurrentUser();
            var records = await _activitiesService.GetUserRecords(user.Id, user.TimeZoneId);
            return View(records);
        }

        [HttpPost("Activity/Update")]
        public async Task<IActionResult> UpdateActivity([FromBody]CreateActivityModel model)
        {
            var user = await CurrentUser();
            var response = await _activitiesService.SaveNewActivity(model, user.Id);
            if (response.Success) return Ok(new { userActivityId = Convert.ToInt64(response.ResponseData) });
            else return BadRequest(new { error = response.ErrorMessage });
        }
        [HttpGet("UpdateActivity/{activityId}")]
        public async Task<IActionResult> UpdateActivity([FromRoute]long activityId)
        {
            return BadRequest();
            //var user = await CurrentUser();
            //var response = await _activitiesService.SaveNewActivity(model, user.Id);
            //if (response.Success) return Ok(new { userActivityId = Convert.ToInt64(response.ResponseData) });
            //else return BadRequest(new { error = response.ErrorMessage });
        }
        [HttpDelete("Activity/Delete/{activityId}")]
        public async Task<IActionResult> DeleteActivity([FromRoute]long activityId)
        {

            var user = await CurrentUser();
            var response = await _activitiesService.DeleteActivity(activityId, user.Id, HttpContext.User.IsInRole("admin"));
            if (response.Success) return Ok();
            else return BadRequest(new { error = response.ErrorMessage });
        }

        [HttpPost("AddAttendenceLog")]
        public async Task<IActionResult> AddAttendenceLog([FromBody]LogAttendenceModel model)
        {
            var user = await CurrentUser();
            var response = await _activitiesService.AddAttendenceLog(model, user.Id);
            if (response.Success) return Ok(new { userActivityId = Convert.ToInt64(response.ResponseData) });
            else return BadRequest(new { error = response.ErrorMessage });
        }
        [HttpDelete("DeleteAttendenceLog/{userActivityId}")]
        public async Task<IActionResult> DeleteAttendenceLog([FromRoute]long userActivityId)
        {
            return BadRequest();
            //var user = await CurrentUser();
            //var response = await _activitiesService.AddAttendenceLog(model, user.Id);
            //if (response.Success) return Ok(new { userActivityId = Convert.ToInt64(response.ResponseData) });
            //else return BadRequest(new { error = response.ErrorMessage });
        }
        [HttpPost("EditAttendenceLog")]
        public async Task<IActionResult> EditAttendenceLog([FromBody]EditLogAttendenceModel model)
        {
            var user = await CurrentUser();
            var response = await _activitiesService.EditAttendenceLog(model, user.Id);
            if (response.Success) return Ok(new { userActivityId = model.UserActivityId });
            else return BadRequest(new { error = response.ErrorMessage });
        }


    }


}
