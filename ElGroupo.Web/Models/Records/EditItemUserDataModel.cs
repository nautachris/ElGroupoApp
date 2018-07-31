
using ElGroupo.Web.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public enum InputControlTypes
    {
        TextBox,
        NumericTextBox,
        DatePicker,
        DateTimePicker,
        CheckBox,
        DropdownList,
        RadioButtonList,
        AutoComplete
    }
    public class EditItemUserDataModel
    {
        public List<IdValueModel> Options { get; set; } = new List<IdValueModel>();
        public bool LabelOnSameRow { get; set; }
        public long Id { get; set; }
        //public long ItemElementId { get; set; }
        public string DataType { get; set; }
        public InputControlTypes ControlType { get; set; }
        public object Value { get; set; }
        //required for autocomplete
        public string LookupText { get; set; }
        public string Description { get; set; }

        public string LookupTable { get; set; }
    }

    public class CreateItemUserDataModel : EditItemUserDataModel
    {

        public long ElementId { get; set; }


    }

    public class SaveNewItemUserDataModel:ReturnViewModel
    {
        public long? CategoryId { get; set; }
        public long? SubCategoryId { get; set; }
        public long? RecordItemTypeId { get; set; }
        public string Name { get; set; }
        public List<IdValueModel> Elements { get; set; }
    }
}
