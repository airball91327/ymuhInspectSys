﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic;

namespace InspectSystem.Models
{
    [Table("InspectDocDetail")]
    public class InspectDocDetail
    {
        [Key, Column(Order = 1)]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }    
        [Required]
        [Display(Name = "區域代碼")]
        public int AreaId { get; set; }
        [Required]
        [Display(Name = "區域名稱")]
        public string AreaName { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "班別代碼")]
        public int ShiftId { get; set; }
        [Required]
        [Display(Name = "班別")]
        public string ShiftName { get; set; }
        [Key, Column(Order = 3)]
        [Display(Name = "類別代碼")]
        public int ClassId { get; set; }
        [Required]
        [Display(Name = "類別名稱")]
        public string ClassName { get; set; }
        [Required]
        [Display(Name = "類別排列順序")]
        public int ClassOrder { get; set; }
        [Key, Column(Order = 4)]
        [Display(Name = "項目代碼")]
        public int ItemId { get; set; }
        [Required]
        [Display(Name = "項目名稱")]
        public string ItemName { get; set; }
        [Display(Name = "項目排列順序")]
        public int ItemOrder { get; set; }
        [Key, Column(Order = 5)]
        [Display(Name = "欄位代碼")]
        public int FieldId { get; set; }
        [Display(Name = "欄位名稱")]
        public string FieldName { get; set; }
        [Required]
        [Display(Name = "資料型態")]
        public string DataType { get; set; }    //"string", "float", "boolean", "checkbox", "dropdownlist"
        [Display(Name = "單位")]
        public string UnitOfData { get; set; }
        [Display(Name = "最小值")]
        public double? MinValue { get; set; }
        [Display(Name = "最大值")]
        public double? MaxValue { get; set; }
        [Required]
        [Display(Name = "是否必填")]
        public bool IsRequired { get; set; }
        [Display(Name = "數值")]
        public string Value { get; set; }
        [Display(Name = "是否正常")]
        public string IsFunctional { get; set; }
        [Display(Name = "備註說明")]
        public string ErrorDescription { get; set; }
        [Display(Name = "維修單號")]
        public string RepairDocId { get; set; }
        [Display(Name = "下拉選單元件")]
        public string DropDownItems { get; set; }

        public virtual InspectDoc InspectDocs { get; set; }

        /* Check that this field is needed to show past value or not. */
        public Boolean ToShowPastValue()
        {
            BMEDcontext db = new BMEDcontext();
            Boolean checkResult = false;
            checkResult = db.InspectField.Find(AreaId, ShiftId, ClassId, ItemId, FieldId).ShowPastValue;

            return checkResult;
        }

        /* Get the past value of the field. */
        public string PastValue()
        {
            BMEDcontext db = new BMEDcontext();
            var pastValue = "";
            var docidTable = db.InspectDocIdTable.Find(DocId);
            if (docidTable != null)
            {
                var applyDate = docidTable.ApplyDate.AddDays(-1);
                var targetDoc = db.InspectDocIdTable.Where(d => d.ApplyDate == applyDate).FirstOrDefault();
                if (targetDoc != null)
                {
                    var findDocDetails = db.InspectDocDetail.Find(targetDoc.DocId, ShiftId, ClassId, ItemId, FieldId);
                    // If has past value
                    if (findDocDetails != null)
                    {
                        pastValue = findDocDetails.Value;
                    }
                }
            }

            return pastValue;
        }
    }
}