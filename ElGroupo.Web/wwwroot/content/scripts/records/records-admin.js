


$(document).ready(function () {

});

RecordsAdmin = {
    Init: function () {

    },
    CreateRecordElementDataType: function (name, cb) {
        var _cb = cb;
        Ajax.Post("/Records/datatype/create", { name: name }).done(function (results) {
            _cb(results);
        });
    },
    GetInputTypesByDataTypeId: function (id,cb) {
        var _cb = cb;
        Ajax.Get({ url: "/Records/GetInputTypesByDataTypeId/" + id, dataType: "application/json" }).done(function (results) {
            _cb(results);
        });
    },
    GetCategories: function (cb) {
        var _cb = cb;
        Ajax.Get({ url: "/Records/GetCategories", dataType: "application/json" }).done(function (results) {
            _cb(results);
        });
    },
    ChangeItemCategory: function (id, categoryId, subCategoryId, cb) {
        var _cb = cb;
        Ajax.Post("/Records/item/changeCategory", { id: id, categoryId: categoryId, subCategoryId: subCategoryId, returnView: true }).done(function (results) {
            _cb(results);
        });
    },
    GetSubCategories: function (id, cb) {
        var _cb = cb;
        Ajax.Get({ url: "/Records/GetSubCategories/" + id, dataType: "application/json" }).done(function (results) {
            _cb(results);
        });
    },
    DeleteRecordElementDataType: function(id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/datatype/delete", { id: id }).done(function (results) {
            _cb(results);
        });
    },
    CreateRecordElementLookupTable: function (name, description, cb) {
        var _cb = cb;
        Ajax.Post("/Records/lookuptable/create", { name: name, description: description, returnView:true }).done(function (results) {
            _cb(results);
        });
    },

    DeleteRecordElementLookupTable: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/lookuptable/delete", { id: id, returnView:true }).done(function (results) {
            _cb(results);
        });
    },

    CreateItemType: function (name, categoryId, subCategoryId, cb) {
        var _cb = cb;
        Ajax.Post("/Records/itemtype/create", { name: name, categoryId: categoryId, subCategoryId: subCategoryId, returnView: true }).done(function (results) {
            _cb(results);
        });
    },

    DeleteItemType: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/itemtype/delete", { id: id, returnView: true }).done(function (results) {
            _cb(results);
        });
    },

    AddItemToRecordElementLookupTable: function (id, value, cb) {
        var _cb = cb;
        Ajax.Post("/Records/lookuptable/editadd", { id: id, value: value, returnView:true }).done(function (results) {
            _cb(results);
        });
    },
    RemoveItemFromRecordElementLookupTable: function (recordElementLookupTableId, lookupIdToRemove, cb) {
        var _cb = cb;
        Ajax.Post("/Records/lookuptable/editremove", { id1: recordElementLookupTableId, id2: lookupIdToRemove, returnView:true }).done(function (results) {
            _cb(results);
        });
    },

    CreateRecordCategory: function (name, cb) {
        var _cb = cb;
        Ajax.Post("/Records/category/create", { name: name, returnView: true }).done(function (results) {
            _cb(results);
        });
    },

    DeleteRecordCategory: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/category/delete", { id: id, returnView:true }).done(function (results) {
            _cb(results);
        });
    },

    CreateRecordSubCategory: function (id, name, cb) {
        var _cb = cb;


        Ajax.Post("/Records/subcategory/create", { id: id, value: name, returnView: true }).done(function (results) {
            console.log('records admin - CreateRecordSubCategory');
            console.log(results);
            _cb(results);
        });
    },

    DeleteRecordSubCategory: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/subcategory/delete", { id: id, returnView:true }).done(function (results) {
            _cb(results);
        });
    },

    CreateRecordItem: function (name, categoryId, subCategoryId, cb) {
        var _cb = cb;
        console.log(name);
        console.log(categoryId);
        console.log(subCategoryId);
        Ajax.Post("/Records/item/create", { name: name, categoryId: categoryId, subCategoryId: subCategoryId, returnView: true }).done(function (results) {
            _cb(results);
        });
    },

    EditRecordItem: function (id, name, categoryId, subCategoryId, cb) {
        var _cb = cb;
        Ajax.Post("/Records/item/edit", { id: id, name: name, categoryId: categoryId === '-1' ? null : categoryId, subCategoryId: subCategoryId === '-1' ? null : subCategoryId, returnView:false }).done(function (results) {
            _cb(results);
        });
    },

    DeleteRecordItem: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/item/delete", { id: id, returnView:true }).done(function (results) {
            _cb(results);
        });
    },


    CreateRecordElement: function (name, displayName, dataTypeId, lookupTableId, inputTypeId, sameRow,cb) {
        var _cb = cb;
        Ajax.Post("/Records/element/create", { name: name, displayName: displayName, dataTypeId: dataTypeId, lookupTableId: lookupTableId, inputTypeId: inputTypeId, sameRow: sameRow, returnView:true }).done(function (results) {
            _cb(results);
        });
    },
    EditRecordElement: function (id, name, displayName,dataTypeId, lookupTableId, inputTypeId, sameRow, cb) {
        var _cb = cb;
        Ajax.Post("/Records/element/edit", { id: id, name: name, displayName: displayName, dataTypeId: dataTypeId, lookupTableId: lookupTableId, inputTypeId: inputTypeId, sameRow:sameRow, returnView: true }).done(function (results) {
            _cb(results);
        });
    },

    DeleteRecordElement: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/element/delete", { id: id, returnView: true }).done(function (results) {
            _cb(results);
        });
    },
    CreateCategoryDefaultElement: function (categoryId, elementId, primaryDisplay, cb) {
        var _cb = cb;
        Ajax.Post("/Records/categorydefaultelement/create", { categoryId: categoryId, elementId: elementId, primaryDisplay: primaryDisplay, returnView: true }).done(function (results) {
            _cb(results);
        });
    },
    DeleteDefaultElement: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/categorydefaultelement/delete", { id: id, returnView: true }).done(function (results) {
            _cb(results);
        });
    },
    CreateSubCategoryDefaultElement: function (subCategoryId, elementId, primaryDisplay, cb) {
        var _cb = cb;
        Ajax.Post("/Records/subcategorydefaultelement/create", { subCategoryId: subCategoryId, elementId: elementId, primaryDisplay: primaryDisplay, returnView: true }).done(function (results) {
            _cb(results);
        });
    },
    CreateRecordItemElement: function (recordItemId, elementId, primaryDisplay, cb) {
        var _cb = cb;
        Ajax.Post("/Records/itemelement/create", { recordItemId: recordItemId, elementId: elementId, primaryDisplay: primaryDisplay, returnView: true }).done(function (results) {
            _cb(results);
        });
    },

    DeleteRecordItemElement: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/itemelement/delete", { id: id, returnView: true }).done(function (results) {
            _cb(results);
        });
    },


    SetRecordItemElementPrimary: function (id, cb) {
        var _cb = cb;
        Ajax.Post("/Records/itemelement/setPrimary", { id: id, returnView: true }).done(function (results) {
            _cb(results);
        });
    },
    SetDefaultElementPrimary: function (id, cb) {
        var _cb = cb;
        Ajax.Post("/Records/defaultelement/setPrimary", { id: id, returnView: true }).done(function (results) {
            _cb(results);
        });
    },

    CreateRecordItemUserData: function (recordElementId, itemUserId, value, cb) {
        var _cb = cb;
        Ajax.Post("/Records/itemuserdata/create", { recordElementId: recordElementId, itemUserId: itemUserId, value: value }).done(function (results) {
            _cb(results);
        });
    },

    DeleteRecordItemUserData: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/itemuserdata/delete", { id: id }).done(function (results) {
            _cb(results);
        });
    },

    EditRecordItemUserData: function (recordItemUserDataId, value, cb) {
        var _cb = cb;
        Ajax.Post("/Records/itemuserdata/edit", { recordItemUserDataId: recordItemUserDataId, value: value }).done(function (results) {
            _cb(results);
        });
    },


    CreateRecordItemUser: function (recordItemId, userId, cb) {
        var _cb = cb;
        Ajax.Post("/Records/itemuser/create", { recordItemId: recordItemId, userId: userId}).done(function (results) {
            _cb(results);
        });
    },


    DeleteRecordItemUser: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/itemuser/delete", { id: id }).done(function (results) {
            _cb(results);
        });
    },

    CreateRecordItemUserDocument: function (formData, cb) {
        var _cb = cb;
        Ajax.Post("/Records/itemuserdocument/create", formData).done(function (results) {
            _cb(results);
        });
    },

    DeleteRecordItemUserDocument: function (id, cb) {
        var _cb = cb;
        Ajax.Delete("/Records/itemuserdocument/delete", { id: id }).done(function (results) {
            _cb(results);
        });
    }

}