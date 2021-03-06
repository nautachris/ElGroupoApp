﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElGroupo.Web.Models.Records
{
    public class UserCategoryModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string NameColumnHeader { get; set; }
        public string ValueColumnHeader { get; set; }
        public List<UserItemListModel> Items { get; set; }
        public List<UserSubCategoryModel> SubCategories { get; set; }

        public bool ShowHiddenRecords { get; set; }
    }
}
