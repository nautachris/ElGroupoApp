using ElGroupo.Domain.Data;
using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ElGroupo.Domain.Records;
using ElGroupo.Web.Models.Records;

namespace ElGroupo.Web.Services
{

    public class RecordsService
    {
        public List<ViewSubCategoryModel> GetSubCategoriesByCategoryId(long categoryId)
        {
            return _ctx.RecordSubCategories.Include(x => x.ParentCategory).Where(x => x.ParentCategory.Id == categoryId).Select(x => new ViewSubCategoryModel { Id = x.Id, Name = x.Name }).ToList();
        }

        public RecordsDashboardModel GetDashboardModel()
        {
            var model = new RecordsDashboardModel { Categories = this.GetCategories() };
            return model;
        }
        public List<IdValueModel> GetCategories()
        {
            return _ctx.RecordCategories.OrderBy(x => x.Name).Select(y => new IdValueModel { Id = y.Id, Value = y.Name }).ToList();
        }
        public EditElementsModel GetEditElementsModel()
        {
            return new EditElementsModel
            {
                Elements = this.GetElements(),
                DataTypes = this.GetDataTypes(),
                LookupTables = this.GetLookupTables()
            };
        }
        public List<ViewDataTypeModel> GetDataTypes()
        {
            return _ctx.RecordElementDataTypes.Select(x => new ViewDataTypeModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
        public List<ViewLookupTableModel> GetLookupTables()
        {
            return _ctx.RecordElementLookupTables.Select(x => new ViewLookupTableModel
            {
                Id = x.Id,
                TableName = x.TableName,
                Description = x.Description
            }).ToList();
        }

        public List<ViewInputTypeModel> GetInputTypesByDataTypeId(long dataTypeId)
        {
            return _ctx.RecordElementDataTypeInputTypes.Include(x => x.DataType).Include(x => x.InputType).Where(x => x.DataType.Id == dataTypeId).Select(x => new ViewInputTypeModel
            {
                Id = x.InputType.Id,
                Name = x.InputType.Name
            }).ToList();
        }

        public List<ViewElementModel> GetElements()
        {
            return _ctx.RecordElements.Include(x => x.DataType).Include(x => x.LookupTable).Include(x => x.InputType).OrderBy(x => x.Name).Select(y =>
                  new ViewElementModel
                  {
                      Id = y.Id,
                      Name = y.Name,
                      DisplayName = y.DisplayName,
                      DataType = new ViewDataTypeModel { Id = y.DataType.Id, Name = y.DataType.Name },
                      InputType = new ViewInputTypeModel { Id = y.InputType.Id, Name = y.InputType.Name },
                      LookupTable = y.LookupTable != null ? new ViewLookupTableModel { Id = y.LookupTable.Id, TableName = y.LookupTable.TableName, Description = y.LookupTable.Description } : null

                  }).ToList();
        }

        public List<ViewElementModel> GetElementsByRecordItemId(long itemId)
        {
            return _ctx.RecordItemElements.Include(x => x.Item).Include(x => x.Element).Where(x => x.Item.Id == itemId).OrderBy(x => x.Element.Name).Select(y =>
                  new ViewElementModel
                  {
                      PrimaryDisplay = y.PrimaryDisplay,
                      Id = y.Id,
                      Name = y.Element.Name
                  }).ToList();
        }

        public List<ViewElementModel> GetDefaultElementsByCategoryId(long id)
        {
            return _ctx.RecordDefaultElements.Include(x => x.Category).Include(x => x.Element).Where(x => x.Category != null && x.Category.Id == id).Select(y => new ViewElementModel { Id = y.Id, Name = y.Element.Name, PrimaryDisplay = y.PrimaryDisplay }).ToList();
        }
        public List<ViewElementModel> GetDefaultElementsBySubCategoryId(long id)
        {
            return _ctx.RecordDefaultElements.Include(x => x.SubCategory).Include(x => x.Element).Where(x => x.SubCategory != null && x.SubCategory.Id == id).Select(y => new ViewElementModel { Id = y.Id, Name = y.Element.Name, PrimaryDisplay = y.PrimaryDisplay }).ToList();
        }
        public ViewCategoryModel GetCategoryModel(long id)
        {
            var cat = _ctx.RecordCategories.Include(x => x.SubCategories).Include(x => x.Items).First(x => x.Id == id);
            return new ViewCategoryModel
            {
                Id = id,
                Name = cat.Name,
                RecordItems = cat.Items.Select(x => new ViewItemModel
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList(),
                SubCategories = cat.SubCategories.Select(y => new ViewSubCategoryModel { Id = y.Id, Name = y.Name }).ToList(),
                AllElements = _ctx.RecordElements.OrderBy(y => y.Name).Select(y => new ViewElementModel { Id = y.Id, Name = y.Name }).ToList(),
                DefaultElements = _ctx.RecordDefaultElements.Include(x => x.Category).Include(x => x.Element).Where(x => x.Category != null && x.Category.Id == id).Select(y => new ViewElementModel { PrimaryDisplay = y.PrimaryDisplay, Id = y.Id, Name = y.Element.Name }).ToList()

            };

        }
        public EditLookupTableModel GetLookupTable(long id)
        {
            var item = _ctx.RecordElementLookupTables.First(x => x.Id == id);
            return new EditLookupTableModel
            {
                Id = item.Id,
                TableName = item.TableName,
                Description = item.Description,
                Values = _lookupTableService.Tables.ContainsKey(item.TableName) ? _lookupTableService.Tables[item.TableName] : new List<IdValueModel>()
            };
        }

        public ViewSubCategoryModel GetSubCategoryModel(long id)
        {
            var cat = _ctx.RecordSubCategories.Include(x => x.ParentCategory).Include(x => x.Items).First(x => x.Id == id);
            return new ViewSubCategoryModel
            {
                Id = id,
                Name = cat.Name,
                RecordItems = cat.Items.Select(x => new ViewItemModel
                {
                    Id = x.Id,
                    Name = x.Name,
                }).ToList(),
                ParentCategory = new ViewCategoryModel
                {
                    Id = cat.ParentCategory.Id,
                    Name = cat.ParentCategory.Name
                },
                AllElements = _ctx.RecordElements.OrderBy(y => y.Name).Select(y => new ViewElementModel { Id = y.Id, Name = y.Name }).ToList(),
                DefaultElements = _ctx.RecordDefaultElements.Include(x => x.SubCategory).Include(x => x.Element).Where(x => x.SubCategory != null && x.SubCategory.Id == id).Select(y => new ViewElementModel { PrimaryDisplay = y.PrimaryDisplay, Id = y.Id, Name = y.Element.Name }).ToList()
            };

        }
        public List<ViewItemModel> GetRecordItemsByCategoryId(long categoryId)
        {
            return _ctx.RecordItems.Include(x => x.Category).Where(x => x.Category != null && x.Category.Id == categoryId).Select(x => new ViewItemModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
        public List<ViewItemModel> GetRecordItemsBySubCategoryId(long subCategoryId)
        {
            return _ctx.RecordItems.Include(x => x.SubCategory).Where(x => x.SubCategory != null && x.SubCategory.Id == subCategoryId).Select(x => new ViewItemModel
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
        public ViewItemModel GetRecordItem(long id, bool includeElementList)
        {
            var item = _ctx.RecordItems.Include(x => x.Elements).ThenInclude(x => x.Element).ThenInclude(x => x.DataType).Include(x => x.Category).Include(x => x.SubCategory).ThenInclude(x => x.ParentCategory).Include(x => x.Elements).First(x => x.Id == id);
            var model = new ViewItemModel
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category != null ? new ViewCategoryModel { Id = item.Category.Id, Name = item.Category.Name } : null,
                SubCategory = item.SubCategory != null ? new ViewSubCategoryModel
                {
                    Id = item.SubCategory.Id,
                    Name = item.SubCategory.Name,
                    ParentCategory = new ViewCategoryModel
                    {
                        Id = item.SubCategory.ParentCategory.Id,
                        Name = item.SubCategory.ParentCategory.Name
                    }
                } : null,
                Elements = item.Elements.Select(y => new ViewElementModel { Id = y.Id, Name = y.Element.Name, DataType = new ViewDataTypeModel { Id = y.Element.DataType.Id, Name = y.Element.DataType.Name } }).ToList()
            };

            if (includeElementList)
            {
                model.ElementList = _ctx.RecordElements.Include(x => x.DataType).Include(x => x.LookupTable).OrderBy(x => x.Name).Select(x => new ViewElementModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    DataType = new ViewDataTypeModel { Id = x.DataType.Id, Name = x.DataType.Name },
                    LookupTable = x.LookupTable == null ? null : new ViewLookupTableModel { Id = x.LookupTable.Id, Description = x.LookupTable.Description, TableName = x.LookupTable.TableName }
                }).ToList();
            }
            return model;
        }



        private readonly ElGroupoDbContext _ctx;
        private readonly LookupTableService _lookupTableService;
        public RecordsService(ElGroupoDbContext ctx, LookupTableService lookupTableService)
        {
            _ctx = ctx;
            _lookupTableService = lookupTableService;
        }


        public async Task<SaveDataResponse> CreateRecordElementDataType(string name)
        {
            try
            {
                var item = new RecordElementDataType { Name = name };
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteRecordElementDataType(long dataTypeId)
        {
            try
            {
                var item = _ctx.RecordElementDataTypes.First(x => x.Id == dataTypeId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateRecordElementLookupTable(string name, string description)
        {
            try
            {
                var tableResponse = _lookupTableService.CreateTable(name);
                if (!tableResponse.Success) return tableResponse;
                var item = new RecordElementLookupTable { TableName = name, Description = description };
                _ctx.Add(item);
                //need to run the add table script
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteRecordElementLookupTable(long lookupTableId)
        {
            try
            {

                var item = _ctx.RecordElementLookupTables.Include(x => x.RecordElements).First(x => x.Id == lookupTableId);
                var tableResponse = _lookupTableService.DropTable(item.TableName);
                if (!tableResponse.Success) return tableResponse;
                //also need to drop the table
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public SaveDataResponse AddItemToRecordElementLookupTable(long lookupTableId, string value)
        {
            try
            {
                var response = _lookupTableService.AddValue(_ctx.RecordElementLookupTables.First(x => x.Id == lookupTableId).TableName, value);
                return response;
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> RemoveItemFromRecordElementLookupTable(long lookupTableId, long idToRemove)
        {
            try
            {
                var response = _lookupTableService.RemoveValue(_ctx.RecordElementLookupTables.First(x => x.Id == lookupTableId).TableName, idToRemove);
                if (response.Success)
                {
                    //need to remove from all userdata
                    foreach (var userData in _ctx.RecordItemUserData.Include(x => x.Element).ThenInclude(x => x.Element).Where(x => x.Element.Element.LookupTable != null && x.Element.Element.LookupTable.Id == lookupTableId))
                    {
                        if (userData.Value == idToRemove.ToString())
                        {
                            userData.Value = null;
                            _ctx.Update(userData);
                        }
                    }
                    await _ctx.SaveChangesAsync();
                }
                return response;
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> CreateRecordCategory(string name)
        {
            try
            {
                var item = new RecordCategory { Name = name };
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteRecordCategory(long categoryId)
        {
            try
            {
                var item = _ctx.RecordCategories.Include(x => x.Items).Include(x => x.SubCategories).ThenInclude(x => x.Items).First(x => x.Id == categoryId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateRecordSubCategory(string name, long categoryId)
        {
            try
            {
                var item = new RecordSubCategory { Name = name, ParentCategory = _ctx.RecordCategories.First(x => x.Id == categoryId) };
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteRecordSubCategory(long subCategoryId)
        {
            try
            {
                var item = _ctx.RecordSubCategories.Include(x => x.ParentCategory).Include(x => x.Items).First(x => x.Id == subCategoryId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.ParentCategory.Id);

            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateRecordItem(string name, long? recordCategoryId, long? recordSubCategoryId)
        {
            try
            {
                var item = new RecordItem { Name = name };
                if (recordCategoryId.HasValue) item.Category = _ctx.RecordCategories.First(x => x.Id == recordCategoryId.Value);
                if (recordSubCategoryId.HasValue) item.SubCategory = _ctx.RecordSubCategories.First(x => x.Id == recordSubCategoryId.Value);
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> EditRecordItem(long recordItemid, string name, long? categoryId, long? subCategoryId)
        {
            try
            {
                var item = _ctx.RecordItems.First(x => x.Id == recordItemid);
                item.Name = name;
                if (categoryId.HasValue)
                {
                    item.Category = _ctx.RecordCategories.First(x => x.Id == categoryId.Value);
                    item.SubCategory = null;
                    item.SubCategoryId = null;
                }
                if (subCategoryId.HasValue)
                {
                    item.SubCategory = _ctx.RecordSubCategories.First(x => x.Id == subCategoryId.Value);
                    item.Category = null;
                }
                else
                {
                    item.SubCategory = null;
                }
                _ctx.Update(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteRecordItem(long recordItemId)
        {
            try
            {
                var item = _ctx.RecordItems.Include(x => x.SubCategory).Include(x => x.Category).Include(x => x.Elements).ThenInclude(x => x.UserData).First(x => x.Id == recordItemId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                if (item.SubCategory != null) return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("subcategory", item.SubCategory.Id));
                return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("category", item.Category.Id));
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }


        public async Task<SaveDataResponse> CreateRecordElement(string name, string displayName, long dataTypeId, long? lookupTableId, long inputTypeId)
        {
            try
            {
                var item = new RecordElement { Name = name, DisplayName = displayName, DataType = _ctx.RecordElementDataTypes.First(x => x.Id == dataTypeId), InputType = _ctx.RecordElementInputTypes.First(x => x.Id == inputTypeId) };
                if (lookupTableId.HasValue) item.LookupTable = _ctx.RecordElementLookupTables.First(x => x.Id == lookupTableId.Value);
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> EditRecordElement(long id, string name, string displayName, long dataTypeId, long? lookupTableId, long inputTypeId)
        {
            try
            {
                var item = _ctx.RecordElements.First(x => x.Id == id);
                item.Name = name;
                item.DisplayName = displayName;
                item.DataType = _ctx.RecordElementDataTypes.First(x => x.Id == dataTypeId);
                item.InputType = _ctx.RecordElementInputTypes.First(x => x.Id == inputTypeId);
                if (lookupTableId.HasValue) item.LookupTable = _ctx.RecordElementLookupTables.First(x => x.Id == lookupTableId.Value);
                else item.LookupTable = null;
                _ctx.Update(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> DeleteRecordElement(long recordElementId)
        {
            try
            {
                var item = _ctx.RecordElements.Include(x => x.Items).First(x => x.Id == recordElementId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> SetRecordElementToPrimary(long recordItemElementId)
        {
            //adds a question to an item like "Undergrad Education"
            try
            {
                var el = _ctx.RecordItemElements.Include(x => x.Item).ThenInclude(x => x.Elements).First(x => x.Id == recordItemElementId);

                foreach (var otherEl in el.Item.Elements)
                {
                    if (otherEl.Id != el.Id)
                    {
                        otherEl.PrimaryDisplay = false;

                    }
                    else
                    {
                        otherEl.PrimaryDisplay = true;
                    }
                    _ctx.Update(otherEl);

                }

                await _ctx.SaveChangesAsync();



                return SaveDataResponse.IncludeData(el.Item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> SetDefaultElementToPrimary(long defaultElementId)
        {
            //adds a question to an item like "Undergrad Education"
            try
            {
                var el = _ctx.RecordDefaultElements.Include(x => x.Category).ThenInclude(x => x.DefaultElements).Include(x => x.SubCategory).ThenInclude(x => x.DefaultElements).First(x => x.Id == defaultElementId);
                if (el.Category != null)
                {
                    foreach (var otherEl in el.Category.DefaultElements)
                    {
                        if (otherEl.Id != el.Id)
                        {
                            otherEl.PrimaryDisplay = false;

                        }
                        else
                        {
                            otherEl.PrimaryDisplay = true;
                        }
                        _ctx.Update(otherEl);

                    }
                }
                else if (el.SubCategory != null)
                {
                    foreach (var otherEl in el.SubCategory.DefaultElements)
                    {
                        if (otherEl.Id != el.Id)
                        {
                            otherEl.PrimaryDisplay = false;

                        }
                        else
                        {
                            otherEl.PrimaryDisplay = true;
                        }
                        _ctx.Update(otherEl);

                    }
                }


                await _ctx.SaveChangesAsync();



                if (el.SubCategory != null) return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("subcategory", el.SubCategory.Id));
                return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("category", el.Category.Id));
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateCategoryDefaultElement(long categoryId, long elementId, bool primaryDisplay)
        {
            //adds a question to an item like "Undergrad Education"
            try
            {
                var item = new RecordDefaultElement { Category = _ctx.RecordCategories.First(x => x.Id == categoryId), Element = _ctx.RecordElements.First(x => x.Id == elementId), PrimaryDisplay = primaryDisplay };
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                if (primaryDisplay)
                {
                    foreach (var rie in _ctx.RecordDefaultElements.Include(x => x.Category).Where(x => x.Category != null && x.Category.Id == categoryId))
                    {
                        if (rie.Id != item.Id)
                        {
                            rie.PrimaryDisplay = false;
                            _ctx.Update(rie);
                        }

                    }
                    await _ctx.SaveChangesAsync();

                }


                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateSubCategoryDefaultElement(long subCategoryId, long elementId, bool primaryDisplay)
        {
            //adds a question to an item like "Undergrad Education"
            try
            {
                var item = new RecordDefaultElement { SubCategory = _ctx.RecordSubCategories.First(x => x.Id == subCategoryId), Element = _ctx.RecordElements.First(x => x.Id == elementId), PrimaryDisplay = primaryDisplay };
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                if (primaryDisplay)
                {
                    foreach (var rie in _ctx.RecordDefaultElements.Include(x => x.Category).Where(x => x.SubCategory != null && x.SubCategory.Id == subCategoryId && x.Id != item.Id))
                    {
                        rie.PrimaryDisplay = false;
                        _ctx.Update(rie);
                    }
                    await _ctx.SaveChangesAsync();

                }


                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateRecordItemElement(long recordItemId, long elementId, bool primaryDisplay)
        {
            //adds a question to an item like "Undergrad Education"
            try
            {
                var item = new RecordItemElement { Item = _ctx.RecordItems.First(x => x.Id == recordItemId), Element = _ctx.RecordElements.First(x => x.Id == elementId), PrimaryDisplay = primaryDisplay };
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                if (primaryDisplay)
                {
                    foreach (var rie in _ctx.RecordItemElements.Include(x => x.Item).Where(x => x.Id != item.Id))
                    {
                        rie.PrimaryDisplay = false;
                        _ctx.Update(rie);
                    }
                    await _ctx.SaveChangesAsync();

                }


                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteRecordItemElement(long recordItemElementId)
        {
            try
            {
                var item = _ctx.RecordItemElements.Include(x => x.Item).Include(x => x.UserData).First(x => x.Id == recordItemElementId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }


        public async Task<SaveDataResponse> DeleteDefaultElement(long id)
        {
            try
            {
                var item = _ctx.RecordDefaultElements.Include(x => x.Category).Include(x => x.SubCategory).First(x => x.Id == id);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                if (item.SubCategory != null) return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("subcategory", item.SubCategory.Id));
                return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("category", item.Category.Id));
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> CreateRecordItemUserData(long recordElementId, long itemUserId, string value)
        {

            try
            {
                var item = new RecordItemUserData
                {
                    Element = _ctx.RecordItemElements.First(x => x.Id == recordElementId),
                    ItemUser = _ctx.RecordItemUsers.First(x => x.Id == itemUserId),
                    Value = value
                };
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteRecordItemUserData(long recordItemUserDataId)
        {

            try
            {
                var item = _ctx.RecordItemUserData.First(x => x.Id == recordItemUserDataId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> EditRecordItemUserData(long recordItemUserDataId, string value)
        {

            try
            {

                var item = _ctx.RecordItemUserData.First(x => x.Id == recordItemUserDataId);
                item.Value = value;
                _ctx.Update(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> CreateRecordItemUser(long recordItemId, long userId)
        {

            try
            {
                var item = new RecordItemUser { Item = _ctx.RecordItems.First(x => x.Id == recordItemId), User = _ctx.Users.First(x => x.Id == userId) };
                _ctx.Add(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(item.Id);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> DeleteRecordItemUser(long recordItemUserId)
        {

            try
            {
                var item = _ctx.RecordItemUsers.Include(x => x.UserData).Include(x => x.Documents).First(x => x.Id == recordItemUserId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> CreateRecordItemUserDocument(List<CreateItemUserDocumentModel> models)
        {

            try
            {
                var idList = new List<long>();
                foreach (var model in models)
                {
                    var item = new RecordItemUserDocument
                    {
                        ContentType = model.ContentType,
                        Description = model.Description,
                        FileName = model.FileName,
                        ImageData = model.Data,
                        ItemUser = _ctx.RecordItemUsers.First(x => x.Id == model.ItemUserId)
                    };
                    _ctx.Add(item);
                    idList.Add(item.Id);
                }

                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(idList);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<SaveDataResponse> DeleteRecordItemUserDocument(long recordItemUserDocumentId)
        {

            try
            {
                var item = _ctx.RecordItemUserDocuments.First(x => x.Id == recordItemUserDocumentId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

    }
}
