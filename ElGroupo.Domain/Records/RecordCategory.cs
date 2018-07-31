using ElGroupo.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElGroupo.Domain.Records
{
    public class RecordCategory : ClassBase
    {
        //i.e. "License Type"
        public string ItemDescriptionColumnHeader { get; set; }
        //i.e. "#"
        public string ItemValueColumnHeader { get; set; }
        public string Name { get; set; }
        public virtual ICollection<RecordDefaultElement> DefaultElements { get; set; }
        public virtual ICollection<RecordItem> Items { get; set; }
        public virtual ICollection<RecordSubCategory> SubCategories { get; set; }

        public virtual ICollection<RecordItemType> ItemTypes { get; set; }
    }

    public class RecordSubCategory : ClassBase
    {
        public string ItemDescriptionColumnHeader { get; set; }
        //i.e. "#"
        public string ItemValueColumnHeader { get; set; }
        public string Name { get; set; }
        public RecordCategory ParentCategory { get; set; }
        public long ParentCategoryId { get; set; }
        public virtual ICollection<RecordItem> Items { get; set; }
        public virtual ICollection<RecordDefaultElement> DefaultElements { get; set; }

        public virtual ICollection<RecordItemType> ItemTypes { get; set; }
    }

    public class RecordDefaultElement : ClassBase
    {
        public long? CategoryId { get; set; }
        public RecordCategory Category { get; set; }
        public long? SubCategoryId { get; set; }
        public RecordSubCategory SubCategory { get; set; }
        public long ElementId { get; set; }
        public RecordElement Element { get; set; }
        public bool PrimaryDisplay { get; set; }
    }

    public class RecordItem : ClassBase
    {
        public RecordItemType ItemType { get; set; }
        public long? ItemTypeId { get; set; }
        public long? CategoryId { get; set; }
        public long? SubCategoryId { get; set; }
        public string Name { get; set; }
        public RecordCategory Category { get; set; }
        public RecordSubCategory SubCategory { get; set; }
        public virtual ICollection<RecordItemElement> Elements { get; set; }
        //public virtual ICollection<RecordItemUser> Users { get; set; }

        public User User { get; set; }
        public long UserId { get; set; }
        public bool Visible { get; set; } = true;

        public virtual ICollection<RecordItemDocument> Documents { get; set; }
        //public virtual ICollection<RecordItemUserData> UserData { get; set; }

    }

    public class RecordItemType : ClassBase
    {
        public RecordCategory Category { get; set; }
        public long? CategoryId { get; set; }
        public RecordSubCategory SubCategory { get; set; }
        public long? SubCategoryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<RecordItem> Items { get; set; }

    }
    //public class RecordItemUser : ClassBase
    //{



    //    public bool Visible { get; set; } = true;
    //    public RecordItem Item { get; set; }
    //    public long ItemId { get; set; }
    //    public User User { get; set; }
    //    public long UserId { get; set; }

    //}
    public class RecordItemDocument : ClassBase
    {
        public RecordItem Item { get; set; }
        public long ItemId { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string ContentType { get; set; }
        public byte[] ImageData { get; set; }
    }
    public class RecordElementInputType : ClassBase
    {
        public string Name { get; set; }
        public virtual ICollection<RecordElement> RecordElements { get; set; }
        public virtual ICollection<RecordElementDataTypeInputType> DataTypes { get; set; }
    }

    public class RecordElementDataTypeInputType : ClassBase
    {
        public long DataTypeId { get; set; }
        public RecordElementDataType DataType { get; set; }

        public long InputTypeId { get; set; }
        public RecordElementInputType InputType { get; set; }
    }
    public class RecordElementLookupTableFieldType : ClassBase
    {
        public string Name { get; set; }
        public virtual ICollection<RecordElement> RecordElements { get; set; }
    }


    public class RecordElement : ClassBase
    {
        public bool LabelOnSameRow { get; set; } = false;

        public string DisplayName { get; set; }
        //basic things like "date of issue", "license #"- a lookup table
        public string Name { get; set; }
        public RecordElementDataType DataType { get; set; }
        public RecordElementInputType InputType { get; set; }
        public long InputTypeId { get; set; }
        public long DataTypeId { get; set; }

        public RecordElementLookupTable LookupTable { get; set; }
        public long? LookupTableId { get; set; }

        //1 - id, 2- value
        public RecordElementLookupTableFieldType LookupTableFieldType { get; set; }
        public long? LookupTableFieldTypeId { get; set; }
        public virtual ICollection<RecordItemElement> Items { get; set; }

        public virtual ICollection<RecordDefaultElement> DefaultElements { get; set; }

    }
    public class RecordElementLookupTable : ClassBase
    {
        public string TableName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<RecordElement> RecordElements { get; set; }

    }

    public class RecordElementDataType : ClassBase
    {
        //simply string/int/double/
        public string Name { get; set; }
        public virtual ICollection<RecordElement> RecordElements { get; set; }
        public virtual ICollection<RecordElementDataTypeInputType> InputTypes { get; set; }
    }

    public class RecordItemElement : ClassBase
    {
        public RecordElement Element { get; set; }
        public RecordItem Item { get; set; }
        //public virtual ICollection<RecordItemUserData> UserData { get; set; }
        public long ItemId { get; set; }
        public long ElementId { get; set; }
        public bool PrimaryDisplay { get; set; }
        public string Value { get; set; }


    }

    //public class RecordItemUserData : ClassBase
    //{

    //    public long ItemUserId { get; set; }
    //    public RecordItemUser ItemUser { get; set; }
    //    public RecordItemElement Element { get; set; }
    //    public long ElementId { get; set; }
    //    public string Value { get; set; }
    //}
}
