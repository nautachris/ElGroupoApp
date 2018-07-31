using ElGroupo.Domain;
using ElGroupo.Web.Models.Records;
using ElGroupo.Web.Models.Shared;
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
    [Route("Records")]
    public class RecordsController : ControllerBase
    {
        private SaveRecordDocumentsModel ParseDocumentRequest(HttpRequest request)
        {
            var model = new SaveRecordDocumentsModel();
            model.ItemId = Convert.ToInt64(request.Form["item-id"]);
            foreach (var file in request.Form.Files)
            {
                var doc = new RecordDocumentListModel();
                //i.e. file_0
                var id = file.Name.Substring(5, 1);
                if (request.Form.ContainsKey("description_" + id)) doc.Description = request.Form["description_" + id].ToString();
                doc.ContentType = file.ContentType;
                doc.FileName = file.FileName;
                var ms = new MemoryStream();
                file.CopyTo(ms);
                doc.Data = ms.ToArray();
                model.Documents.Add(doc);
            }
            return model;
        }

        [HttpDelete("DeleteDocument/{documentId}")]
        public async Task<IActionResult> DeleteDocument([FromRoute]long documentId)
        {
            var response = await _recordsService.DeleteDocument(documentId);
            if (response.Success)
            {
                var itemId = Convert.ToInt64(response.ResponseData);
                var docs = _recordsService.GetDocumentsByItemId(itemId);
                return PartialView("_ItemDocuments", docs);

            }
            else return BadRequest();
        }

        [HttpGet("items/user/{recordItemUserId}")]
        public async Task<IActionResult> GetItemUserListItem([FromRoute]long recordItemUserId)
        {
            var model = await _recordsService.GetUserItemById(recordItemUserId);
            return PartialView("_ItemUserListItem", model);
        }

        [HttpPost("SaveUserData")]
        public async Task<IActionResult> SaveUserData([FromBody]EditItemModel items)
        {
            var user = await CurrentUser();
            var response = await _recordsService.SaveUserData(user.Id, items);
            if (response.Success) return Json(new { displayValue = response.ResponseData.ToString() });
            else return BadRequest();
            //this needs to return the primary display value for the record

        }


        [HttpGet("GetDocument/{documentId}")]
        public async Task<IActionResult> GetDocument([FromRoute]long documentId)
        {
            var doc = await _recordsService.GetDocument(documentId);
            if (doc == null) return BadRequest();

            return File(doc.ImageData, doc.ContentType, doc.FileName);
        }


        [HttpPost("userrecord/savenewdocuments")]
        public async Task<IActionResult> UploadDocumentsForNewRecordItem()
        {
            try
            {
                var currentUser = await CurrentUser();
                if (Request.Form.Files.Count == 0) return BadRequest();
                if (!Request.Form.Keys.Contains("item-id")) return BadRequest("record-item-id was not included in the form data");
                var recordItemId = Convert.ToInt64(Request.Form["item-id"]);
                var response = await _recordsService.SaveDocument(ParseDocumentRequest(Request));
                if (response.Success)
                {
                    var newItem = _recordsService.GetRecordItem(recordItemId, false);
                    //return whatever is returned after saving new record
                    if (newItem.Category != null)
                    {
                        var catItems = _recordsService.GetUserItemsByCategoryId(newItem.Category.Id, currentUser.Id, false);
                        return PartialView("_ItemUserListItemContainer", catItems);
                    }
                    else
                    {
                        var subCatItems = _recordsService.GetUserItemsBySubCategoryId(newItem.SubCategory.Id, currentUser.Id, false);
                        return PartialView("_ItemUserListItemContainer", subCatItems);
                    }

                }
                else return BadRequest(new { error = response.ErrorMessage });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("UploadDocuments")]
        public async Task<IActionResult> UploadDocuments()
        {
            try
            {
                if (Request.Form.Files.Count == 0) return BadRequest();

                if (!Request.Form.Keys.Contains("item-id")) return BadRequest("itemUserId was not included in the form data");
                var itemId = Convert.ToInt64(Request.Form["item-id"]);
                var returnTable = Request.Form.Keys.Contains("returnTable");
                var response = await _recordsService.SaveDocument(ParseDocumentRequest(Request));
                if (response.Success)
                {
                    if (returnTable)
                    {
                        var docs = _recordsService.GetDocumentsByItemId(itemId);
                        return PartialView("_ItemDocuments", docs);
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
        #region views

        [HttpGet("Categories")]
        public IActionResult Categories()
        {
            var model = _recordsService.GetCategories();
            return View(model);
        }

        [HttpGet("GetCategories")]
        public IActionResult GetCategories()
        {
            var model = _recordsService.GetCategories();
            return Json(model);
        }

        [HttpGet("GetSubCategories/{categoryId}")]
        public IActionResult GetSubCategories(long categoryId)
        {
            var model = _recordsService.GetSubCategoriesByCategoryId(categoryId);
            return Json(model);
        }

        [HttpGet("GetInputTypesByDataTypeId/{dataTypeId}")]
        public IActionResult GetInputTypesByDataTypeId(long dataTypeId)
        {
            return Json(_recordsService.GetInputTypesByDataTypeId(dataTypeId));
        }

        [HttpGet("Elements")]
        public IActionResult Elements()
        {
            var model = _recordsService.GetEditElementsModel();
            return View(model);
        }

        [HttpGet("LookupTables")]
        public IActionResult LookupTables()
        {
            var model = _recordsService.GetLookupTables();
            return View(model);
        }

        [HttpGet("ItemTypes")]
        public IActionResult ItemTypes()
        {
            var model = _recordsService.GetItemTypes();
            return View(model);
        }

        [HttpGet("Admin")]
        public IActionResult Admin()
        {
            return View();
        }




        [HttpGet("EditCategory/{categoryId}")]
        public IActionResult EditCategory(long categoryId)
        {
            var model = _recordsService.GetCategoryModel(categoryId);
            return View("Category", model);
        }

        [HttpGet("EditSubCategory/{subCategoryId}")]
        public IActionResult EditSubCategory(long subCategoryId)
        {
            var model = _recordsService.GetSubCategoryModel(subCategoryId);
            return View("SubCategory", model);
        }


        [HttpGet("EditLookupTable/{lookupTableId}")]
        public IActionResult EditLookupTable(long lookupTableId)
        {
            var model = _recordsService.GetLookupTable(lookupTableId);
            return View("LookupTable", model);
        }

        [HttpGet("EditItem/{itemId}")]
        public IActionResult EditItem(long itemId)
        {
            var model = _recordsService.GetRecordItem(itemId, true);
            return View("Item", model);
        }


        [Authorize]
        [HttpPost]
        [Route("LookupAutocomplete")]
        public IActionResult SearchAutocomplete([FromBody]AutocompleteSearchModel search)
        {
            var list = _recordsService.LookupSearch(search.TableName, search.SearchText);
            return Json(list);
        }

        [HttpPost("ViewItemDetails")]
        public async Task<IActionResult> ViewItemDetails([FromBody]IdModel model)
        {
            var user = await CurrentUser();

            var outModel = _recordsService.GetItemUserDetails(model.Id);
            return View("_ItemDetails", outModel);
        }


        //[HttpGet("AddNewItem")]
        //public async Task<IActionResult> AddNewItem([FromBody]IdIdModel model)
        //{
        //    var user = await CurrentUser();

        //    var outModel = _recordsService.GetItemUserDetails(model.Id, user.Id);
        //    return View("_ItemDetails", outModel);
        //}

        #endregion


        #region data access
        private LookupTableService _lookupTableService = null;
        private RecordsService _recordsService = null;
        public RecordsController(UserManager<User> userMgr, LookupTableService lookupTableService, RecordsService recordsService) : base(userMgr)
        {
            _lookupTableService = lookupTableService;
            _recordsService = recordsService;
        }


        [HttpPost]
        [Route("datatype/create")]
        public async Task<IActionResult> CreateRecordElementDataType([FromBody]NameModel model)
        {
            var response = await _recordsService.CreateRecordElementDataType(model.Name);
            if (response.Success) return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            return BadRequest(new { message = response.ErrorMessage });

        }
        [HttpDelete]
        [Route("datatype/delete")]
        public async Task<IActionResult> DeleteRecordElementDataType([FromBody]IdModel model)
        {
            var response = await _recordsService.DeleteRecordElementDataType(model.Id);
            if (response.Success) return Ok();
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("lookuptable/create")]
        public async Task<IActionResult> CreateRecordElementLookupTable([FromBody]NameDescriptionModel model)
        {
            var response = await _recordsService.CreateRecordElementLookupTable(model.Name, model.Description);
            if (response.Success)
            {
                if (model.ReturnView) return View("_LookupTableList", _recordsService.GetLookupTables());
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }



        [HttpPost]
        [Route("itemtype/create")]
        public async Task<IActionResult> CreateRecordItemType([FromBody]ViewItemTypeModel model)
        {
            var response = await _recordsService.CreateRecordItemType(model);
            if (response.Success)
            {
                if (model.ReturnView) return View("_ItemTypeList", _recordsService.GetItemTypes());
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpDelete]
        [Route("lookuptable/delete")]
        public async Task<IActionResult> DeleteRecordElementLookupTable([FromBody]IdModel model)
        {
            var response = await _recordsService.DeleteRecordElementLookupTable(model.Id);
            if (response.Success)
            {
                if (model.ReturnView) return View("_LookupTableList", _recordsService.GetLookupTables());
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpDelete]
        [Route("itemtype/delete")]
        public async Task<IActionResult> DeleteRecordItemType([FromBody]IdModel model)
        {
            var response = await _recordsService.DeleteRecordItemType(model.Id);
            if (response.Success)
            {
                if (model.ReturnView) return View("_ItemTypeList", _recordsService.GetItemTypes());
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }



        [HttpPost]
        [Route("lookuptable/editadd")]
        public IActionResult AddItemToRecordElementLookupTable([FromBody]IdValueModel model)
        {
            var response = _recordsService.AddItemToRecordElementLookupTable(model.Id, model.Value);
            if (response.Success)
            {
                if (model.ReturnView) return View("_LookupTableValueList", _lookupTableService.GetValuesByLookupTableId(model.Id));
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("lookuptable/editremove")]
        public async Task<IActionResult> RemoveItemFromRecordElementLookupTable([FromBody]IdIdModel model)
        {
            var response = await _recordsService.RemoveItemFromRecordElementLookupTable(model.Id1, model.Id2);
            if (response.Success)
            {
                if (model.ReturnView) return View("_LookupTableValueList", _lookupTableService.GetValuesByLookupTableId(model.Id1));
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("category/create")]
        public async Task<IActionResult> CreateRecordCategory([FromBody]NameModel model)
        {
            var response = await _recordsService.CreateRecordCategory(model.Name);
            if (response.Success)
            {
                if (model.ReturnView) return View("_CategoryList", _recordsService.GetCategories());
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpDelete]
        [Route("category/delete")]
        public async Task<IActionResult> DeleteRecordCategory([FromBody]IdModel model)
        {
            var response = await _recordsService.DeleteRecordCategory(model.Id);
            if (response.Success)
            {
                if (model.ReturnView) return View("_CategoryList", _recordsService.GetCategories());
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("subcategory/create")]
        public async Task<IActionResult> CreateRecordSubCategory([FromBody]IdValueModel model)
        {
            var response = await _recordsService.CreateRecordSubCategory(model.Value, model.Id);
            if (response.Success)
            {
                if (model.ReturnView) return View("_SubCategoryList", _recordsService.GetSubCategoriesByCategoryId(model.Id));
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpDelete]
        [Route("subcategory/delete")]
        public async Task<IActionResult> DeleteRecordSubCategory([FromBody]IdModel model)
        {
            var response = await _recordsService.DeleteRecordSubCategory(model.Id);
            if (response.Success)
            {
                if (model.ReturnView) return View("_SubCategoryList", _recordsService.GetSubCategoriesByCategoryId(Convert.ToInt64(response.ResponseData)));
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }


        [HttpPost]
        [Route("userrecord/create")]
        public async Task<IActionResult> CreateUserRecord([FromBody]NewUserRecordModel model)
        {
            NewUserRecordModel response = null;
            var user = await CurrentUser();
            if (model.CategoryId.HasValue)
            {
                response = _recordsService.NewUserCategoryItem(model.CategoryId.Value);
                return PartialView("_CreateItemDetails", response);
            }
            else if (model.SubCategoryId.HasValue)
            {
                response = _recordsService.NewUserSubCategoryItem(model.SubCategoryId.Value);
                return PartialView("_CreateItemDetails", response);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("userrecord/new/category/{categoryId}")]
        public async Task<IActionResult> GetCategoryElements([FromRoute]long categoryId)
        {

            var user = await CurrentUser();
            NewUserRecordModel response = _recordsService.NewUserCategoryItem(categoryId);
            return PartialView("_CreateItemDetails", response);

        }
        [HttpGet]
        [Route("userrecord/new/subcategory/{subCategoryId}")]
        public async Task<IActionResult> GetSubCategoryElements([FromRoute]long subCategoryId)
        {

            var user = await CurrentUser();
            var response = _recordsService.NewUserSubCategoryItem(subCategoryId);
            return PartialView("_CreateItemDetails", response);

        }

        [HttpGet]
        [Route("userrecord/create/{categoryId}")]
        public async Task<IActionResult> CreateNewRecordItem([FromRoute]long categoryId)
        {
            NewUserRecordModel response = null;
            var user = await CurrentUser();
            if (_recordsService.CategoryHasSubCategories(categoryId))
            {
                return PartialView("_SelectSubCategory", _recordsService.GetSubCategoryListByCategoryId(categoryId));
            }
            else
            {
                response = _recordsService.NewUserCategoryItem(categoryId);
                return PartialView("_CreateItemDetails", response);
            }
            //if category w/ no subcategories, return teh
            //if (model.CategoryId.HasValue)
            //{
            //    response = _recordsService.NewUserCategoryItem(model.CategoryId.Value);
            //    return PartialView("_CreateItemDetails", response);
            //}
            //else if (model.SubCategoryId.HasValue)
            //{
            //    response = _recordsService.NewUserSubCategoryItem(model.SubCategoryId.Value);
            //    return PartialView("_CreateItemDetails", response);
            //}
            //else
            //{
            //    return BadRequest();
            //}
        }

        [HttpPost]
        [Route("userrecord/savenew")]
        public async Task<IActionResult> SaveNewUserRecord([FromBody]SaveNewItemUserDataModel model)
        {
            var user = await CurrentUser();
            var response = await _recordsService.SaveNewRecordItem(model, user.Id);
            if (response.Success)
            {
                if (model.ReturnView)
                {
                    if (model.CategoryId.HasValue)
                    {
                        var catItems = _recordsService.GetUserItemsByCategoryId(model.CategoryId.Value, user.Id, false);
                        return PartialView("_ItemUserListItemContainer", catItems);
                    }
                    else
                    {
                        var subCatItems = _recordsService.GetUserItemsBySubCategoryId(model.SubCategoryId.Value, user.Id, false);
                        return PartialView("_ItemUserListItemContainer", subCatItems);
                    }
                }
                else
                {
                    //documents come next - return the record item id
                    var newItemId = Convert.ToInt64(response.ResponseData);
                    return Json(new { recordItemId = newItemId });
                }

            }
            return Ok();
        }


        [HttpPost]
        [Route("item/create")]
        public async Task<IActionResult> CreateRecordItem([FromBody]CreateRecordItemModel model)
        {
            var response = await _recordsService.CreateRecordItem(model.Name, model.CategoryId, model.SubCategoryId);
            if (response.Success)
            {
                if (model.ReturnView)
                {
                    if (model.CategoryId.HasValue)
                    {
                        return View("_ItemList", _recordsService.GetRecordItemsByCategoryId(model.CategoryId.Value));
                    }
                    else
                    {
                        return View("_ItemList", _recordsService.GetRecordItemsBySubCategoryId(model.SubCategoryId.Value));
                    }
                }
                //return View("_ItemList",)}
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }


        [HttpPost]
        [Route("item/changeCategory")]
        public async Task<IActionResult> CreateRecordItem([FromBody]EditRecordItemModel model)
        {
            var response = await _recordsService.CreateRecordItem(model.Name, model.CategoryId, model.SubCategoryId);
            if (response.Success)
            {
                if (model.ReturnView)
                {
                    if (model.CategoryId.HasValue)
                    {
                        return View("_ItemList", _recordsService.GetRecordItemsByCategoryId(model.CategoryId.Value));
                    }
                    else
                    {
                        return View("_ItemList", _recordsService.GetRecordItemsBySubCategoryId(model.SubCategoryId.Value));
                    }
                }
                //return View("_ItemList",)}
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("item/edit")]
        public async Task<IActionResult> EditRecordItem([FromBody]EditRecordItemModel model)
        {
            var response = await _recordsService.EditRecordItem(model.Id, model.Name, model.CategoryId, model.SubCategoryId);
            if (response.Success)
            {

                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpDelete]
        [Route("item/delete")]
        public async Task<IActionResult> DeleteRecordItem([FromBody]IdModel model)
        {
            var user = await CurrentUser();
            var response = await _recordsService.DeleteRecordItem(model.Id);
            if (response.Success)
            {
                if (model.ReturnView)
                {
                    var kvp = (KeyValuePair<string, long>)response.ResponseData;
                    if (kvp.Key == "category") return View("_ItemUserListItemContainer", _recordsService.GetUserItemsByCategoryId(kvp.Value, user.Id, false));
                    return View("_ItemUserListItemContainer", _recordsService.GetUserItemsBySubCategoryId(kvp.Value, user.Id, false));
                }
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("item/hide")]
        public async Task<IActionResult> HideRecordItem([FromBody]IdModel model)
        {
            var user = await CurrentUser();
            var response = await _recordsService.ToggleRecordVisibility(model.Id, false);
            if (response.Success)
            {
                if (model.ReturnView)
                {
                    var kvp = (KeyValuePair<string, long>)response.ResponseData;
                    if (kvp.Key == "category") return View("_ItemUserListItemContainer", _recordsService.GetUserItemsByCategoryId(kvp.Value, user.Id, false));
                    return View("_ItemUserListItemContainer", _recordsService.GetUserItemsBySubCategoryId(kvp.Value, user.Id, false));
                }
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });
        }
        [HttpPost]
        [Route("item/show")]
        public async Task<IActionResult> ShowRecordItem([FromBody]IdModel model)
        {
            var user = await CurrentUser();
            var response = await _recordsService.ToggleRecordVisibility(model.Id, true);
            if (response.Success)
            {
                if (model.ReturnView)
                {
                    var kvp = (KeyValuePair<string, long>)response.ResponseData;
                    if (kvp.Key == "category") return View("_ItemUserListItemContainer", _recordsService.GetUserItemsByCategoryId(kvp.Value, user.Id, false));
                    return View("_ItemUserListItemContainer", _recordsService.GetUserItemsBySubCategoryId(kvp.Value, user.Id, false));
                }
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });
        }

        //[HttpPost]
        //[Route("export/subcategory/{subCategoryId}")]
        //public async Task<IActionResult> ExportSubCategory([FromRoute]long subCategoryId)
        //{
        //    var user = await CurrentUser();

        //}
        //[HttpPost]
        //[Route("export/subcategory/{categoryId}")]
        //public async Task<IActionResult> ExportCategory([FromRoute]long categoryId)
        //{
        //    var user = await CurrentUser();

        //}



        [HttpPost]
        [Route("element/create")]
        public async Task<IActionResult> CreateRecordElement([FromBody]CreateRecordElementModel model)
        {
            var response = await _recordsService.CreateRecordElement(model.Name, model.DisplayName, model.DataTypeId, model.LookupTableId, model.InputTypeId, model.SameRow);
            if (response.Success)
            {
                if (model.ReturnView) if (model.ReturnView) return View("_EditElementList", _recordsService.GetElements());
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("element/edit")]
        public async Task<IActionResult> EditRecordElement([FromBody]CreateRecordElementModel model)
        {
            var response = await _recordsService.EditRecordElement(model.Id, model.DisplayName, model.Name, model.DataTypeId, model.LookupTableId, model.InputTypeId, model.SameRow);
            if (response.Success)
            {
                //no use creating a view that consists of a single table row - do it client sid
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpDelete]
        [Route("element/delete")]
        public async Task<IActionResult> DeleteRecordElement([FromBody]IdModel model)
        {
            var response = await _recordsService.DeleteRecordElement(model.Id);
            if (response.Success)
            {
                if (model.ReturnView) return View("_EditElementList", _recordsService.GetElements());
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("itemelement/create")]
        public async Task<IActionResult> CreateRecordItemElement([FromBody]CreateRecordItemElementModel model)
        {
            var response = await _recordsService.CreateRecordItemElement(model.RecordItemId, model.ElementId, model.PrimaryDisplay);
            if (response.Success)
            {
                if (model.ReturnView) return View("_ElementList", _recordsService.GetElementsByRecordItemId(model.RecordItemId));
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("categorydefaultelement/create")]
        public async Task<IActionResult> CreateCategoryDefaultElement([FromBody]CreateCategoryDefaultElementModel model)
        {
            var response = await _recordsService.CreateCategoryDefaultElement(model.CategoryId, model.ElementId, model.PrimaryDisplay);
            if (response.Success)
            {
                if (model.ReturnView) return View("_ElementList", _recordsService.GetDefaultElementsByCategoryId(model.CategoryId));
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }


        //[HttpPost("AddNewItem")]
        //public async Task<IActionResult> ViewCategory([FromBody]IdIdModel model)
        //{
        //    var user = await CurrentUser();
        //    var model = _recordsService.GetUserCategoryModel(categoryId, user.Id);
        //    return View(model);
        //}


        [HttpGet("ViewCategory/{categoryId}")]
        public async Task<IActionResult> ViewCategory([FromRoute]long categoryId, [FromQuery]bool showHidden = false)
        {
            var user = await CurrentUser();
            var model = _recordsService.GetUserCategoryModel(categoryId, user.Id, showHidden);
            model.ShowHiddenRecords = showHidden;
            return View(model);
        }

        [HttpGet("Dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            var model = _recordsService.GetDashboardModel();
            var user = await CurrentUser();
            model.UserId = user.Id;
            model.FirstName = user.FirstName;
            return View(model);
        }

        [HttpPost]
        [Route("subcategorydefaultelement/create")]
        public async Task<IActionResult> CreateSubCategoryDefaultElement([FromBody]CreateSubCategoryDefaultElementModel model)
        {
            var response = await _recordsService.CreateSubCategoryDefaultElement(model.SubCategoryId, model.ElementId, model.PrimaryDisplay);
            if (response.Success)
            {
                if (model.ReturnView) return View("_ElementList", _recordsService.GetDefaultElementsBySubCategoryId(model.SubCategoryId));
                return Ok(new { id = Convert.ToInt32(response.ResponseData) });
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpDelete]
        [Route("categorydefaultelement/delete")]
        public async Task<IActionResult> DeleteCategoryDefaultElement([FromBody]IdModel model)
        {
            var response = await _recordsService.DeleteDefaultElement(model.Id);
            if (response.Success)
            {
                if (model.ReturnView)
                {
                    var responseData = (KeyValuePair<string, long>)response.ResponseData;
                    if (responseData.Key == "category") return View("_ElementList", _recordsService.GetDefaultElementsByCategoryId(responseData.Value));
                    return View("_ElementList", _recordsService.GetDefaultElementsBySubCategoryId(responseData.Value));
                }

                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }


        [HttpPost]
        [Route("itemelement/setPrimary")]
        public async Task<IActionResult> SetRecordItemElementPrimaryDisplay([FromBody]IdModel model)
        {
            var response = await _recordsService.SetRecordElementToPrimary(model.Id);
            if (response.Success)
            {
                if (model.ReturnView) return View("_ElementList", _recordsService.GetElementsByRecordItemId(Convert.ToInt64(response.ResponseData)));

                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }

        [HttpPost]
        [Route("category/updateDescriptionColumn")]
        public async Task<IActionResult> SetCategoryDescriptionColumn([FromBody]IdValueModel model)
        {
            var response = await _recordsService.UpdateCategoryDescriptionColumn(model.Id, model.Value);
            if (response.Success)
            {
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }
        [HttpPost]
        [Route("subcategory/updateValueColumn")]
        public async Task<IActionResult> SetSubCategoryDescriptionColumn([FromBody]IdValueModel model)
        {
            var response = await _recordsService.UpdateSubCategoryValueColumn(model.Id, model.Value);
            if (response.Success)
            {


                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }


        [HttpPost]
        [Route("category/updateValueColumn")]
        public async Task<IActionResult> SetCategoryValueColumn([FromBody]IdValueModel model)
        {
            var response = await _recordsService.UpdateCategoryValueColumn(model.Id, model.Value);
            if (response.Success)
            {
                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }
        [HttpPost]
        [Route("subcategory/updateDescriptionColumn")]
        public async Task<IActionResult> SetSubCategoryValueColumn([FromBody]IdValueModel model)
        {
            var response = await _recordsService.UpdateSubCategoryDescriptionColumn(model.Id, model.Value);
            if (response.Success)
            {


                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }



        [HttpPost]
        [Route("defaultElement/setPrimary")]
        public async Task<IActionResult> SetDefaultElementPrimaryDisplay([FromBody]IdModel model)
        {
            var response = await _recordsService.SetDefaultElementToPrimary(model.Id);
            if (response.Success)
            {
                if (model.ReturnView)
                {
                    var responseData = (KeyValuePair<string, long>)response.ResponseData;
                    if (responseData.Key == "category") return View("_ElementList", _recordsService.GetDefaultElementsByCategoryId(responseData.Value));
                    return View("_ElementList", _recordsService.GetDefaultElementsBySubCategoryId(responseData.Value));

                }

                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }


        [HttpDelete]
        [Route("itemelement/delete")]
        public async Task<IActionResult> DeleteRecordItemElement([FromBody]IdModel model)
        {
            var response = await _recordsService.DeleteRecordItemElement(model.Id);
            if (response.Success)
            {
                if (model.ReturnView) return View("_ElementList", _recordsService.GetElementsByRecordItemId(Convert.ToInt64(response.ResponseData)));

                return Ok();
            }
            return BadRequest(new { message = response.ErrorMessage });

        }


        //[HttpPost]
        //[Route("itemuserdata/create")]
        //public async Task<IActionResult> CreateRecordItemUserData([FromBody]CreateRecordItemUserDataModel model)
        //{
        //    var response = await _recordsService.CreateRecordItemUserData(model.RecordElementId, model.ItemUserId, model.Value.ToString());
        //    if (response.Success) return Ok(new { id = Convert.ToInt32(response.ResponseData) });
        //    return BadRequest(new { message = response.ErrorMessage });

        //}

        //[HttpDelete]
        //[Route("itemuserdata/delete")]
        //public async Task<IActionResult> DeleteRecordItemUserData([FromBody]IdModel model)
        //{
        //    var response = await _recordsService.DeleteRecordItemUserData(model.Id);
        //    if (response.Success) return Ok();
        //    return BadRequest(new { message = response.ErrorMessage });

        //}

        //[HttpPost]
        //[Route("itemuserdata/edit")]
        //public async Task<IActionResult> EditRecordItemUserData([FromBody]EditRecordItemUserDataModel model)
        //{
        //    var response = await _recordsService.EditRecordItemUserData(model.RecordItemUserDataId, model.Value.ToString());
        //    if (response.Success) return Ok();
        //    return BadRequest(new { message = response.ErrorMessage });

        //}


        //[HttpPost]
        //[Route("itemuser/create")]
        //public async Task<IActionResult> CreateRecordItemUser([FromBody]CreateRecordItemUserModel model)
        //{
        //    var response = await _recordsService.CreateRecordItemUser(model.RecordItemId, model.UserId);
        //    if (response.Success) return Ok(new { id = Convert.ToInt32(response.ResponseData) });
        //    return BadRequest(new { message = response.ErrorMessage });

        //}

        //[HttpDelete]
        //[Route("itemuser/delete")]
        //public async Task<IActionResult> DeleteRecordItemUser([FromBody]IdModel model)
        //{
        //    var response = await _recordsService.DeleteRecordItemUser(model.Id);
        //    if (response.Success) return Ok();
        //    return BadRequest(new { message = response.ErrorMessage });

        //}








        //[HttpDelete]
        //[Route("itemuserdocument/delete")]
        //public async Task<IActionResult> DeleteRecordItemUserDocument([FromBody]IdModel model)
        //{
        //    var response = await _recordsService.DeleteRecordItemUserDocument(model.Id);
        //    if (response.Success) return Ok();
        //    return BadRequest(new { message = response.ErrorMessage });

        //}

        #endregion
    }
}
