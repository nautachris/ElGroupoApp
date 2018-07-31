using ElGroupo.Domain.Data;
using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ElGroupo.Domain.Records;
using ElGroupo.Web.Models.Records;
using System.IO;
using System.IO.Compression;

namespace ElGroupo.Web.Services
{

    public class RecordsService
    {

        public async Task<SaveDataResponse> SaveDocument(SaveRecordDocumentsModel model)
        {
            try
            {
                var item = _ctx.RecordItems.First(x => x.Id == model.ItemId);
                foreach (var doc in model.Documents)
                {
                    RecordItemDocument newDoc = new RecordItemDocument
                    {
                        ContentType = doc.ContentType,
                        Description = doc.Description,
                        FileName = doc.FileName,
                        Item = item,
                        ImageData = doc.Data
                    };
                    _ctx.Add(newDoc);
                }
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public List<UserItemListModel> GetUserItemsByCategoryId(long categoryId, long userId, bool showHidden)
        {
            var list = new List<UserItemListModel>();
            var items = _ctx.RecordItems.
                Include(x => x.User).
                Include(x => x.Documents).
                Include(x => x.Elements).
                    ThenInclude(x => x.Element).
                    ThenInclude(x => x.DataType).
                Include(x => x.Elements).
                    ThenInclude(x => x.Element).
                    ThenInclude(x => x.LookupTable).
                Include(x => x.Category).
                Where(x => (x.Category != null && x.Category.Id == categoryId) && (x.User != null && x.User.Id == userId));
            if (!showHidden) items = items.Where(x => x.Visible);
            foreach (var recordItem in items)
            {
                var itemListModel = new UserItemListModel();
                itemListModel.RecordItemId = recordItem.Id;
                itemListModel.Name = recordItem.Name;

                itemListModel.RecordItemId = recordItem.Id;
                if (recordItem.Documents.Any()) itemListModel.DocumentId = recordItem.Documents.First().Id;


                var primaryElement = recordItem.Elements.Any(x => x.PrimaryDisplay) ? recordItem.Elements.First(x => x.PrimaryDisplay) : recordItem.Elements.FirstOrDefault();
                //itemListModel.Value = userItem.us
                itemListModel.Value = GetUserDataDisplayValueByElement(primaryElement);


                list.Add(itemListModel);
            }
            return list;
        }
        public async Task<UserItemListModel> GetUserItemById(long itemId)
        {
            var recordItem = await _ctx.RecordItems.Include(x => x.Elements).ThenInclude(x => x.Element).FirstAsync(x => x.Id == itemId);
            var itemListModel = new UserItemListModel();
            itemListModel.Name = recordItem.Name;
            itemListModel.RecordItemId = recordItem.Id;


            if (recordItem.Documents.Any()) itemListModel.DocumentId = recordItem.Documents.First().Id;

            if (recordItem.Elements != null)
            {
                var primaryDisplayElement = recordItem.Elements.Any(x => x.PrimaryDisplay) ? recordItem.Elements.First(x => x.PrimaryDisplay) : recordItem.Elements.First();
                //itemListModel.Value = userItem.us
                itemListModel.Value = GetUserDataDisplayValueByElement(primaryDisplayElement);
            }

            return itemListModel;
        }
        public List<UserItemListModel> GetUserItemsBySubCategoryId(long subCategoryId, long userId, bool showHidden)
        {
            var list = new List<UserItemListModel>();
            var items = _ctx.RecordItems.
                Include(x => x.User).
                Include(x => x.Documents).
                Include(x => x.Elements).
                    ThenInclude(x => x.Element).
                    ThenInclude(x => x.DataType).
                Include(x => x.Elements).
                    ThenInclude(x => x.Element).
                    ThenInclude(x => x.LookupTable).
                Include(x => x.SubCategory).
                Where(x => (x.SubCategory != null && x.SubCategory.Id == subCategoryId) && x.User.Id == userId);
            if (!showHidden) items = items.Where(x => x.Visible);
            foreach (var recordItem in items)
            {
                var itemListModel = new UserItemListModel();
                itemListModel.Name = recordItem.Name;
                itemListModel.RecordItemId = recordItem.Id;
                //var userItem = recordItem.Users.FirstOrDefault(x => x.UserId == userId);


                if (recordItem.Documents.Any()) itemListModel.DocumentId = recordItem.Documents.First().Id;

                if (recordItem.Elements.Any())
                {
                    var primaryDisplayElement = recordItem.Elements.Any(x => x.PrimaryDisplay) ? recordItem.Elements.First(x => x.PrimaryDisplay) : recordItem.Elements.FirstOrDefault();
                    //itemListModel.Value = userItem.us
                    itemListModel.Value = GetUserDataDisplayValueByElement(primaryDisplayElement);

                }

                list.Add(itemListModel);
            }
            return list;
        }
        public UserSubCategoryModel GetUserSubCategoryModel(long subCategoryId, long userId, bool showHidden)
        {
            //do we show all record items all the time?
            //or only if they've been associated with a user already
            //why don't we just populate the recorduseritem after the user has accessed it for the first time?
            var cat = _ctx.RecordSubCategories.First(x => x.Id == subCategoryId);
            var model = new UserSubCategoryModel
            {
                Id = cat.Id,
                Name = cat.Name,
                NameColumnHeader = cat.ItemDescriptionColumnHeader,
                ValueColumnHeader = cat.ItemValueColumnHeader,
                Items = this.GetUserItemsBySubCategoryId(subCategoryId, userId, showHidden)
            };
            return model;
        }


        public EditItemModel GetAddNewItemModel(long? categoryId, long? subCategoryId, long userId)
        {
            return null;
        }

        public UserCategoryModel GetUserCategoryModel(long categoryId, long userId, bool showHidden)
        {
            //do we show all record items all the time?
            //or only if they've been associated with a user already
            //why don't we just populate the recorduseritem after the user has accessed it for the first time?
            var cat = _ctx.RecordCategories.Include(x => x.SubCategories).First(x => x.Id == categoryId);
            var model = new UserCategoryModel
            {
                Id = cat.Id,
                Name = cat.Name,
                NameColumnHeader = cat.ItemDescriptionColumnHeader,
                ValueColumnHeader = cat.ItemValueColumnHeader,
                Items = this.GetUserItemsByCategoryId(categoryId, userId, showHidden),
                SubCategories = cat.SubCategories.Select(x => this.GetUserSubCategoryModel(x.Id, userId, showHidden)).ToList()
            };
            return model;
        }

        private string GetUserDataDisplayValueByElement(RecordItemElement itemElement)
        {

            if (itemElement == null) return null;
            if (string.IsNullOrEmpty(itemElement.Value)) return null;
            if (itemElement.Element == null || itemElement.Element == null || itemElement.Element.DataType == null) return null;
            if (itemElement.Element.LookupTable != null)
            {
                var lookupVal = _lookupTableService.GetValue(itemElement.Element.LookupTable.TableName, Convert.ToInt32(itemElement.Value));
                if (lookupVal == null) return itemElement.Value;
                return lookupVal;
            }
            else
            {
                switch (itemElement.Element.DataType.Name)
                {
                    case "Date":
                        return Convert.ToDateTime(itemElement.Value).ToShortDateString();
                    case "DateTime":
                        return Convert.ToDateTime(itemElement.Value).ToShortDateString() + " " + Convert.ToDateTime(itemElement.Value).ToShortTimeString();
                    case "Text":
                    case "Integer":
                    case "Double":
                        return itemElement.Value;
                    case "Boolean":
                        if (Convert.ToBoolean(itemElement.Value) == true) return "Yes";
                        return "No";
                    default:
                        return null;
                }
            }

        }
        public async Task<SaveDataResponse> DeleteDocument(long documentId)
        {
            try
            {
                var doc = await _ctx.RecordItemDocuments.FirstAsync(x => x.Id == documentId);
                _ctx.Remove(doc);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(doc.ItemId);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }


        }

        public async Task<SaveDataResponse> SaveUserData(long userId, EditItemModel model)
        {
            try
            {



                var recordItem = _ctx.RecordItems.
                    Include(x => x.Elements).
                        ThenInclude(x => x.Element).
                        ThenInclude(x => x.DataType).
                    Include(x => x.Elements).
                        ThenInclude(x => x.Element).
                        ThenInclude(x => x.LookupTable).First(x => x.Id == model.ItemId);
                if (model.ItemTypeId.HasValue)
                {
                    recordItem.ItemType = _ctx.RecordItemTypes.First(x => x.Id == model.ItemTypeId.Value);
                    recordItem.Name = recordItem.ItemType.Name;
                }
                else
                {
                    recordItem.Name = model.Name;
                    recordItem.ItemType = null;
                }
                _ctx.Update(recordItem);
                foreach (var updatedElement in model.UserData)
                {
                    var recordElement = recordItem.Elements.First(x => x.Id == updatedElement.Id);
                    if (updatedElement.Value != null && recordElement.Value != null && updatedElement.Value.ToString() == recordElement.Value) continue;
                    switch (recordElement.Element.DataType.Name)
                    {
                        case "String":
                            recordElement.Value = updatedElement.Value.ToString();
                            break;
                        case "DateTime":
                        case "Date":
                            recordElement.Value = Convert.ToDateTime(updatedElement.Value).ToString("O");
                            break;
                        case "Integer":
                            recordElement.Value = Convert.ToInt32(updatedElement.Value).ToString();
                            break;
                        case "Double":
                            recordElement.Value = Convert.ToDouble(updatedElement.Value).ToString();
                            break;
                        case "Boolean":
                            recordElement.Value = Convert.ToBoolean(updatedElement.Value).ToString();
                            break;
                        case "Text":
                            recordElement.Value = updatedElement.Value.ToString();
                            break;
                    }
                    _ctx.Update(recordElement);
                }


                await _ctx.SaveChangesAsync();


                var primaryElement = recordItem.Elements.Any(x => x.PrimaryDisplay) ? recordItem.Elements.First(x => x.PrimaryDisplay) : recordItem.Elements.First();
                var newPrimaryDisplay = GetUserDataDisplayValueByElement(primaryElement);
                return SaveDataResponse.IncludeData(newPrimaryDisplay);
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<RecordItemDocument> GetDocument(long id)
        {
            var doc = await _ctx.RecordItemDocuments.FirstAsync(x => x.Id == id);
            return doc;
        }

        public List<RecordDocumentListModel> GetDocumentsByItemId(long id)
        {
            return _ctx.RecordItemDocuments.Where(x => x.ItemId == id).Select(x => new RecordDocumentListModel
            {
                Id = x.Id,
                FileName = x.FileName,
                ContentType = x.ContentType,
                Description = x.Description

            }).ToList();
        }
        public List<ViewSubCategoryModel> GetSubCategoriesByCategoryId(long categoryId)
        {
            return _ctx.RecordSubCategories.Include(x => x.ParentCategory).Where(x => x.ParentCategory.Id == categoryId).Select(x => new ViewSubCategoryModel { Id = x.Id, Name = x.Name }).ToList();

        }
        public SelectSubcategoryModel GetSubCategoryListByCategoryId(long categoryId)
        {
            var model = new SelectSubcategoryModel { ParentCategoryId = categoryId };
            model.SubCategories = _ctx.RecordCategories.Include(x => x.SubCategories).First(x => x.Id == categoryId).SubCategories.Select(x => new IdValueModel { Id = x.Id, Value = x.Name }).OrderBy(x => x.Value).ToList();
            model.SubCategories.Add(new IdValueModel { Id = -1, Value = "Other" });
            model.SubCategories.Insert(0, new IdValueModel { Id = 0, Value = "Select a Sub Category..." });
            return model;

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

        public List<ViewItemTypeModel> GetItemTypes()
        {
            var list = new List<ViewItemTypeModel>();
            foreach (var itemType in _ctx.RecordItemTypes.Include(x => x.Category).Include(x => x.SubCategory).ThenInclude(x => x.ParentCategory))
            {
                var item = new ViewItemTypeModel
                {
                    Id = itemType.Id,
                    Name = itemType.Name,
                    CategoryId = itemType.CategoryId,
                    SubCategoryId = itemType.SubCategoryId
                };

                if (itemType.SubCategory != null)
                {
                    item.SubCategoryName = itemType.SubCategory.Name;
                    item.CategoryName = itemType.SubCategory.ParentCategory.Name;
                }
                else
                {
                    item.CategoryName = itemType.Category.Name;
                }

                list.Add(item);

            }

            return list.OrderBy(x => x.CategoryName).ThenBy(x => x.SubCategoryName).ThenBy(x => x.Name).ToList();
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
                      LabelOnSameRow = y.LabelOnSameRow,
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
                DescriptionColumnHeader = cat.ItemDescriptionColumnHeader,
                ValueColumnHeader = cat.ItemValueColumnHeader,
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
                DescriptionColumnHeader = cat.ItemDescriptionColumnHeader,
                ValueColumnHeader = cat.ItemValueColumnHeader,
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

        private InputControlTypes ConvertTextToInputControl(string text)
        {
            switch (text)
            {


                case "Auto Complete":
                    return InputControlTypes.AutoComplete;
                case "Text Box":
                    return InputControlTypes.TextBox;
                case "Numeric Text Box":
                    return InputControlTypes.NumericTextBox;
                case "Date Picker":
                    return InputControlTypes.DatePicker;
                case "Date Time Picker":
                    return InputControlTypes.DateTimePicker;
                case "Checkbox":
                    return InputControlTypes.CheckBox;
                case "Dropdown List":
                    return InputControlTypes.DropdownList;
                case "Radio Button List":
                    return InputControlTypes.RadioButtonList;
                default:
                    return InputControlTypes.TextBox;

            }
        }

        private Type ConvertTextToType(string typeText)
        {
            switch (typeText)
            {
                case "Integer": return typeof(int);
                case "Double": return typeof(double);
                case "Boolean": return typeof(bool);
                case "Text": return typeof(string);
                case "Date":
                case "DateTime":
                    return typeof(DateTime?);
                default: return typeof(string);
            }
        }


        private object ConvertTextToNativeObject(string typeText, string value)
        {
            if (value == null || string.IsNullOrEmpty(value)) return null;
            switch (typeText)
            {
                case "Integer": return Convert.ToInt32(value);
                case "Double": return Convert.ToDouble(value);
                case "Boolean": return Convert.ToBoolean(value);
                case "Text": return value;
                case "Date":
                case "DateTime":
                    return Convert.ToDateTime(value);
                default: return value;
            }
        }
        private object GetDefault(Type t)
        {
            if (Nullable.GetUnderlyingType(t) != null) return null;
            if (t.Name == "String") return null;
            return Activator.CreateInstance(t);
        }
        public EditItemModel GetItemUserDetails(long recordItemId)
        {

            //called when they click on the row to expand the details for a record
            //we could get rid of item user and just search based on await CurrentUser from the controller
            //this occurs the first time the user clicks on item details.
            //if (recordItemId == 0)
            //{
            //    var newUser = new RecordItem
            //    {
            //        User = _ctx.Users.First(x => x.Id == userId),
            //        Visible = true,
            //        Re = _ctx.RecordItems.First(x => x.Id == recordItemId)
            //    };
            //    _ctx.Add(newUser);
            //    _ctx.SaveChanges();
            //    itemUserId = newUser.Id;
            //}
            //what if admin went in and added a new element to the question?
            //i.e. added "graduated" to undergraduate education
            var recordItem = _ctx.RecordItems.Include(x => x.User).
                Include(x => x.Category).
                Include(x => x.SubCategory).
                Include(x => x.ItemType).
                Include(x => x.Elements).
                    ThenInclude(x => x.Element).
                    ThenInclude(x => x.DataType).
                Include(x => x.Elements).
                    ThenInclude(x => x.Element).
                    ThenInclude(x => x.LookupTable).
                Include(x => x.Elements).
                    ThenInclude(x => x.Element).
                    ThenInclude(x => x.InputType).
                Include(x => x.Documents).
                FirstOrDefault(x => x.Id == recordItemId);
            if (recordItem == null) return null;

            //var missingElements = itemUser.Item.Elements.Select(x => x.Id).Except(itemUser.UserData.Select(x => x.Element.Id));


            //foreach (var itemElement in itemUser.Item.Elements) { }

            var model = new EditItemModel { ItemId = recordItemId, Name = recordItem.Name };
            if (recordItem.Category != null)
            {
                model.CategoryId = recordItem.Category.Id;
                model.ItemTypes = _ctx.RecordItemTypes.Where(x => x.CategoryId.HasValue && x.CategoryId.Value == model.CategoryId).OrderBy(x => x.Name).Select(x => new Domain.Lookups.IdValueModel { Id = x.Id, Value = x.Name }).ToList();
            }
            if (recordItem.SubCategory != null)
            {
                model.SubCategoryId = recordItem.SubCategory.Id;
                model.ItemTypes = _ctx.RecordItemTypes.Where(x => x.SubCategoryId.HasValue && x.SubCategoryId.Value == model.SubCategoryId).OrderBy(x => x.Name).Select(x => new Domain.Lookups.IdValueModel { Id = x.Id, Value = x.Name }).ToList();
            }
            if (recordItem.ItemType != null) model.ItemTypeId = recordItem.ItemType.Id;

            foreach (var itemElement in recordItem.Elements)
            {
                var item = new EditItemUserDataModel
                {
                    Id = itemElement.Id,
                    DataType = itemElement.Element.DataType.Name,
                    Description = itemElement.Element.DisplayName,
                    LabelOnSameRow = itemElement.Element.LabelOnSameRow,
                    ControlType = ConvertTextToInputControl(itemElement.Element.InputType.Name),
                    LookupTable = itemElement.Element.LookupTable != null ? itemElement.Element.LookupTable.TableName : null

                };

                if (itemElement.Value != null)
                {

                    switch (item.ControlType)
                    {
                        case InputControlTypes.AutoComplete:
                            item.LookupText = _lookupTableService.GetValue(itemElement.Element.LookupTable.TableName, itemElement.Value);
                            break;
                        case InputControlTypes.DropdownList:
                        case InputControlTypes.RadioButtonList:
                            if (item.DataType != "Boolean")
                            {
                                item.Options = _lookupTableService.GetValuesByLookupTableName(item.LookupTable);
                            }

                            break;
                    }

                    item.Value = ConvertTextToNativeObject(itemElement.Element.DataType.Name, itemElement.Value);

                }
                else
                {
                    if (item.ControlType == InputControlTypes.DropdownList || item.ControlType == InputControlTypes.RadioButtonList && item.LookupTable != null)
                    {
                        //this may be for boolean
                        item.Options = _lookupTableService.GetValuesByLookupTableName(item.LookupTable);
                    }
                    Type tt = ConvertTextToType(itemElement.Element.DataType.Name);
                    item.Value = GetDefault(ConvertTextToType(itemElement.Element.DataType.Name));
                }

                model.UserData.Add(item);
            }

            foreach (var doc in recordItem.Documents)
            {
                model.Documents.Add(new RecordDocumentListModel
                {
                    Id = doc.Id,
                    Description = doc.Description,
                    FileName = doc.FileName
                });
            }
            return model;
        }

        public List<IdValueModel> LookupSearch(string tableName, string searchText)
        {
            return _lookupTableService.LookupSearch(tableName, searchText);
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

        public async Task<SaveDataResponse> CreateRecordItemType(ViewItemTypeModel model)
        {
            try
            {
                var item = new RecordItemType
                {
                    Name = model.Name,
                    Category = model.CategoryId.HasValue ? _ctx.RecordCategories.First(x => x.Id == model.CategoryId.Value) : null,
                    SubCategory = model.SubCategoryId.HasValue ? _ctx.RecordSubCategories.First(x => x.Id == model.SubCategoryId.Value) : null
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

        public async Task<SaveDataResponse> DeleteRecordItemType(long itemTypeId)
        {
            try
            {

                var itemType = _ctx.RecordItemTypes.Include(x => x.Items).First(x => x.Id == itemTypeId);
                foreach (var item in itemType.Items)
                {
                    item.ItemType = null;
                    _ctx.Update(item);
                }
                _ctx.Remove(itemType);
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
                    foreach (var userData in _ctx.RecordItemElements.Include(x => x.Element).ThenInclude(x => x.LookupTable).Where(x => x.Element.LookupTable != null && x.Element.LookupTable.Id == lookupTableId))
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
        public bool CategoryHasSubCategories(long categoryId)
        {
            return _ctx.RecordSubCategories.Any(x => x.ParentCategoryId == categoryId);
        }
        public NewUserRecordModel NewUserCategoryItem(long categoryId)
        {
            try
            {
                var model = new NewUserRecordModel
                {
                    CategoryId = categoryId,
                    CategoryName = _ctx.RecordCategories.First(x => x.Id == categoryId).Name,
                    ItemTypes = _ctx.RecordItemTypes.Where(x => x.CategoryId.HasValue && x.CategoryId.Value == categoryId && x.SubCategoryId == null).OrderBy(x => x.Name).Select(x => new IdValueModel { Id = x.Id, Value = x.Name }).ToList(),
                    UserData = new List<CreateItemUserDataModel>()
                };

                foreach (var el in _ctx.RecordDefaultElements.
                    Include(x => x.Element).
                        ThenInclude(x => x.DataType).
                    Include(x => x.Element).
                        ThenInclude(x => x.InputType).
                    Include(x => x.Element).ThenInclude(x => x.LookupTable)
                        .Where(x => x.CategoryId.HasValue && x.CategoryId.Value == categoryId))
                {

                    var dataModel = new CreateItemUserDataModel
                    {
                        ElementId = el.Element.Id,
                        ControlType = ConvertTextToInputControl(el.Element.InputType.Name),
                        DataType = el.Element.DataType.Name,
                        Description = el.Element.DisplayName,
                        LabelOnSameRow = el.Element.LabelOnSameRow,
                        LookupTable = el.Element.LookupTable?.TableName

                    };

                    switch (dataModel.ControlType)
                    {
                        case InputControlTypes.DropdownList:
                        case InputControlTypes.RadioButtonList:
                            if (dataModel.DataType != "Boolean")
                            {
                                dataModel.Options = _lookupTableService.GetValuesByLookupTableName(dataModel.LookupTable);
                            }

                            break;
                    }
                    model.UserData.Add(dataModel);

                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public NewUserRecordModel NewUserSubCategoryItem(long subCategoryId)
        {
            var model = new NewUserRecordModel
            {
                SubCategoryId = subCategoryId,
                SubCategoryName = _ctx.RecordSubCategories.First(x => x.Id == subCategoryId).Name,
                ItemTypes = _ctx.RecordItemTypes.Where(x => x.SubCategoryId.HasValue && x.SubCategoryId.Value == subCategoryId).OrderBy(x => x.Name).Select(x => new IdValueModel { Id = x.Id, Value = x.Name }).ToList(),
                UserData = new List<CreateItemUserDataModel>()
            };

            foreach (var el in _ctx.RecordDefaultElements.
                Include(x => x.Element).
                    ThenInclude(x => x.DataType).
                Include(x => x.Element).
                    ThenInclude(x => x.InputType).
                Include(x => x.Element).ThenInclude(x => x.LookupTable)
                    .Where(x => x.SubCategoryId.HasValue && x.SubCategoryId.Value == subCategoryId))
            {

                var dataModel = new CreateItemUserDataModel
                {
                    ElementId = el.Element.Id,
                    ControlType = ConvertTextToInputControl(el.Element.InputType.Name),
                    DataType = el.Element.DataType.Name,
                    Description = el.Element.DisplayName,
                    LabelOnSameRow = el.Element.LabelOnSameRow,
                    LookupTable = el.Element.LookupTable?.TableName

                };

                switch (dataModel.ControlType)
                {
                    case InputControlTypes.DropdownList:
                    case InputControlTypes.RadioButtonList:
                        if (dataModel.DataType != "Boolean")
                        {
                            dataModel.Options = _lookupTableService.GetValuesByLookupTableName(dataModel.LookupTable);
                        }

                        break;
                }
                model.UserData.Add(dataModel);

            }
            return model;
        }

        public async Task<SaveDataResponse> SaveNewRecordItem(SaveNewItemUserDataModel model, long userId)
        {
            try
            {
                var recordItem = new RecordItem { User = _ctx.Users.First(x => x.Id == userId) };
                if (model.RecordItemTypeId.HasValue)
                {
                    recordItem.ItemType = _ctx.RecordItemTypes.First(x => x.Id == model.RecordItemTypeId.Value);
                    recordItem.Name = recordItem.ItemType.Name;
                }
                else
                {
                    recordItem.Name = model.Name;
                }

                if (model.CategoryId.HasValue) recordItem.Category = _ctx.RecordCategories.First(x => x.Id == model.CategoryId.Value);
                else recordItem.SubCategory = _ctx.RecordSubCategories.First(x => x.Id == model.SubCategoryId.Value);

                _ctx.Add(recordItem);
                foreach (var el in model.Elements)
                {
                    var rie = new RecordItemElement
                    {
                        Item = recordItem,
                        Element = _ctx.RecordElements.First(x => x.Id == el.Id),
                        Value = el.Value
                    };
                    _ctx.Add(rie);
                }

                await _ctx.SaveChangesAsync();
                return SaveDataResponse.IncludeData(recordItem.Id);

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
                var item = _ctx.RecordItems.Include(x => x.Documents).Include(x => x.Elements).First(x => x.Id == recordItemId);
                _ctx.Remove(item);
                await _ctx.SaveChangesAsync();
                if (item.SubCategoryId != null) return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("subcategory", item.SubCategoryId.Value));
                return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("category", item.CategoryId.Value));
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> ToggleRecordVisibility(long id, bool visible)
        {
            try
            {
                var item = _ctx.RecordItems.First(x => x.Id == id);
                item.Visible = visible;
                _ctx.Update(item);
                await _ctx.SaveChangesAsync();
                if (item.SubCategoryId != null) return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("subcategory", item.SubCategoryId.Value));
                return SaveDataResponse.IncludeData(new KeyValuePair<string, long>("category", item.CategoryId.Value));
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }

        public async Task<MemoryStream> ExportSubCategory(long subCategoryId, long userId)
        {
            var documents = _ctx.RecordItems.Include(x => x.User).Include(x => x.SubCategory).Include(x => x.Documents).Where(x => x.SubCategory != null && x.SubCategory.Id == subCategoryId && x.User.Id == userId).SelectMany(x => x.Documents).ToList();
            if (documents.Count == 0) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (var doc in documents)
                    {
                        var entry = archive.CreateEntry(doc.FileName, CompressionLevel.Fastest);
                        using (var zipStream = entry.Open()) zipStream.Write(doc.ImageData, 0, doc.ImageData.Length);
                    }

                }
                return ms;
            }
            
        }

        public async Task<SaveDataResponse> CreateRecordElement(string name, string displayName, long dataTypeId, long? lookupTableId, long inputTypeId, bool labelOnSameRow)
        {
            try
            {
                var item = new RecordElement { Name = name, LabelOnSameRow = labelOnSameRow, DisplayName = displayName, DataType = _ctx.RecordElementDataTypes.First(x => x.Id == dataTypeId), InputType = _ctx.RecordElementInputTypes.First(x => x.Id == inputTypeId) };
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
        public async Task<SaveDataResponse> EditRecordElement(long id, string name, string displayName, long dataTypeId, long? lookupTableId, long inputTypeId, bool sameRow)
        {
            try
            {
                var item = _ctx.RecordElements.First(x => x.Id == id);
                item.Name = name;
                item.DisplayName = displayName;
                item.LabelOnSameRow = sameRow;
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
        public async Task<SaveDataResponse> UpdateCategoryDescriptionColumn(long catId, string columnName)
        {
            try
            {
                var cat = _ctx.RecordCategories.First(x => x.Id == catId);
                cat.ItemDescriptionColumnHeader = columnName;
                _ctx.Update(cat);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> UpdateSubCategoryDescriptionColumn(long catId, string columnName)
        {
            try
            {
                var cat = _ctx.RecordSubCategories.First(x => x.Id == catId);
                cat.ItemDescriptionColumnHeader = columnName;
                _ctx.Update(cat);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }



        public async Task<SaveDataResponse> UpdateCategoryValueColumn(long catId, string columnName)
        {
            try
            {
                var cat = _ctx.RecordCategories.First(x => x.Id == catId);
                cat.ItemValueColumnHeader = columnName;
                _ctx.Update(cat);
                await _ctx.SaveChangesAsync();
                return SaveDataResponse.Ok();
            }
            catch (Exception ex)
            {
                return SaveDataResponse.FromException(ex);
            }
        }
        public async Task<SaveDataResponse> UpdateSubCategoryValueColumn(long catId, string columnName)
        {
            try
            {
                var cat = _ctx.RecordSubCategories.First(x => x.Id == catId);
                cat.ItemValueColumnHeader = columnName;
                _ctx.Update(cat);
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
                var defaultEl = _ctx.RecordDefaultElements.Include(x => x.Category).ThenInclude(x => x.DefaultElements).Include(x => x.SubCategory).ThenInclude(x => x.DefaultElements).First(x => x.Id == recordItemElementId);
                if (defaultEl.Category != null)
                {
                    foreach (var otherEl in defaultEl.Category.DefaultElements)
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
                else
                {
                    foreach (var otherEl in defaultEl.SubCategory.DefaultElements)
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
                var item = _ctx.RecordItemElements.Include(x => x.Item).First(x => x.Id == recordItemElementId);
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
        //public async Task<SaveDataResponse> CreateRecordItemUserData(long recordElementId, long itemUserId, string value)
        //{

        //    try
        //    {
        //        var item = new RecordItemUserData
        //        {
        //            Element = _ctx.RecordItemElements.First(x => x.Id == recordElementId),
        //            ItemUser = _ctx.RecordItemUsers.First(x => x.Id == itemUserId),
        //            Value = value
        //        };
        //        _ctx.Add(item);
        //        await _ctx.SaveChangesAsync();
        //        return SaveDataResponse.IncludeData(item.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        return SaveDataResponse.FromException(ex);
        //    }
        //}
        //public async Task<SaveDataResponse> DeleteRecordItemUserData(long recordItemUserDataId)
        //{

        //    try
        //    {
        //        var item = _ctx.RecordItemUserData.First(x => x.Id == recordItemUserDataId);
        //        _ctx.Remove(item);
        //        await _ctx.SaveChangesAsync();
        //        return SaveDataResponse.Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return SaveDataResponse.FromException(ex);
        //    }
        //}
        //public async Task<SaveDataResponse> EditRecordItemUserData(long recordItemUserDataId, string value)
        //{

        //    try
        //    {

        //        var item = _ctx.RecordItemUserData.First(x => x.Id == recordItemUserDataId);
        //        item.Value = value;
        //        _ctx.Update(item);
        //        await _ctx.SaveChangesAsync();
        //        return SaveDataResponse.Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return SaveDataResponse.FromException(ex);
        //    }
        //}

        //public async Task<SaveDataResponse> CreateRecordItemUser(long recordItemId, long userId)
        //{

        //    try
        //    {
        //        var item = new RecordItemUser { Item = _ctx.RecordItems.First(x => x.Id == recordItemId), User = _ctx.Users.First(x => x.Id == userId) };
        //        _ctx.Add(item);
        //        await _ctx.SaveChangesAsync();
        //        return SaveDataResponse.IncludeData(item.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        return SaveDataResponse.FromException(ex);
        //    }
        //}
        //public async Task<SaveDataResponse> DeleteRecordItemUser(long recordItemUserId)
        //{

        //    try
        //    {
        //        var item = _ctx.RecordItemUsers.Include(x => x.UserData).Include(x => x.Documents).First(x => x.Id == recordItemUserId);
        //        _ctx.Remove(item);
        //        await _ctx.SaveChangesAsync();
        //        return SaveDataResponse.Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return SaveDataResponse.FromException(ex);
        //    }
        //}

        //public async Task<SaveDataResponse> CreateRecordItemUserDocument(SaveRecordDocumentsModel model)
        //{

        //    try
        //    {
        //        var idList = new List<long>();
        //        foreach (var doc in model.Documents)
        //        {
        //            var item = new RecordItemUserDocument
        //            {
        //                ContentType = doc.ContentType,
        //                Description = doc.Description,
        //                FileName = doc.FileName,
        //                ImageData = doc.Data,
        //                ItemUser = _ctx.RecordItemUsers.First(x => x.Id == model.ItemUserId)
        //            };
        //            _ctx.Add(item);
        //            idList.Add(item.Id);
        //        }

        //        await _ctx.SaveChangesAsync();
        //        return SaveDataResponse.IncludeData(idList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return SaveDataResponse.FromException(ex);
        //    }
        //}

        //public async Task<SaveDataResponse> DeleteRecordItemUserDocument(long recordItemUserDocumentId)
        //{

        //    try
        //    {
        //        var item = _ctx.RecordItemUserDocuments.First(x => x.Id == recordItemUserDocumentId);
        //        _ctx.Remove(item);
        //        await _ctx.SaveChangesAsync();
        //        return SaveDataResponse.Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return SaveDataResponse.FromException(ex);
        //    }
        //}

    }
}
