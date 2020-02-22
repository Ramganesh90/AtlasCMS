using Atlas.DAL;
using Atlas.Models.DBO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.DataAccess.Entity
{
    public static class ComboCollection
    {
        internal static List<Setup10_EquipmentCosts> EquipmentCostsCombos(DataTable table)
        {
            List<Setup10_EquipmentCosts> lstEquipmentCosts = new List<Setup10_EquipmentCosts>();
            foreach (var item in table.AsEnumerable())
            {
                var source = new Setup10_EquipmentCosts();
                source.EquipCostID = Convert.ToString(item["EquipCostID"]);
                source.EquipName = Convert.ToString(item["EquipName"]);
                lstEquipmentCosts.Add(source);
            }

            //ViewBag.lstEquipmentCosts = new SelectList(lstEquipmentCosts, "EquipCostID", "EquipName");
            return lstEquipmentCosts;
        }

        internal static List<Setup03_DefaultCrewSize> CrewSizeCombo(DataTable table)
        {
            List<Setup03_DefaultCrewSize> lstCrewSize = new List<Setup03_DefaultCrewSize>();
            foreach (var item in table.AsEnumerable())
            {
                var source = new Setup03_DefaultCrewSize();
                source.CrewPositionID = Convert.ToString(item["CrewPositionID"]);
                source.CrewPosition = Convert.ToString(item["CrewPosition"]);
                lstCrewSize.Add(source);
            }
            // ViewBag.lstCrewSize = new SelectList(lstCrewSize, "CrewPositionID", "CrewPosition");
            return lstCrewSize;

        }

        internal static List<Setup11_OtherCostTypes> OtherCostCombo(DataTable table)
        {
            List<Setup11_OtherCostTypes> lstOtherCostTypes = new List<Setup11_OtherCostTypes>();
            foreach (var item in table.AsEnumerable())
            {
                var source = new Setup11_OtherCostTypes();
                source.OtherCostTypeID = Convert.ToString(item["OtherCostTypeID"]);
                source.OtherCostType = Convert.ToString(item["OtherCostType"]);
                lstOtherCostTypes.Add(source);
            }

            //  ViewBag.OtherCostTypes = new SelectList(lstOtherCostTypes, "OtherCostTypeID", "OtherCostType");
            return lstOtherCostTypes;
        }
        internal static List<SelectListItem> RoundFactorCombo()
        {
            return (Enumerable.Range(0, 105).Where(i => i % 5 == 0)
                 .Select(p => new SelectListItem() { Text = p+" %".ToString(), Value = p.ToString() })).ToList();
            // ViewBag.lstConcreteTypes = new SelectList(lstConcreteTypes, "PartNum", "PartDescription");
        }
        internal static List<vFilterEstConcrete> ConcreteTypesCombo(DataTable table)
        {
            List<vFilterEstConcrete> lstConcreteTypes = new List<vFilterEstConcrete>();
            foreach (var item in table.AsEnumerable())
            {
                var source = new vFilterEstConcrete();
                source.PartNum = Convert.ToString(item["PartNum"]);
                source.PartDescription = Convert.ToString(item["PartDescription"]);
                lstConcreteTypes.Add(source);
            }
            return lstConcreteTypes;
            // ViewBag.lstConcreteTypes = new SelectList(lstConcreteTypes, "PartNum", "PartDescription");
        }

        internal static List<Setup28_FencePostTypes> PostTypesCombo(DataTable table)
        {
            List<Setup28_FencePostTypes> lstFencePostTypes = new List<Setup28_FencePostTypes>();
            foreach (var item in table.AsEnumerable())
            {
                var source = new Setup28_FencePostTypes();
                source.PstTypID = Convert.ToString(item["PstTypID"]);
                source.PostType = Convert.ToString(item["PostType"]);
                lstFencePostTypes.Add(source);
            }
            return lstFencePostTypes;
            // ViewBag.FencePostTypes = new SelectList(lstFencePostTypes, "PstTypID", "PostType");
        }

        internal static List<LabourDdls> AdditionalLabourCombo(DataTable table)
        {
            List<LabourDdls> additionalDropdown = new List<LabourDdls>();
            foreach (var item in table.AsEnumerable())
            {
                LabourDdls obj = new LabourDdls();
                obj.FieldLbrDtlsID = Convert.ToString(item["FieldLbrDtlsID"]);
                obj.FieldLbrDesc = Convert.ToString(item["FieldLbrDesc"]);
                additionalDropdown.Add(obj);
            }
            return additionalDropdown;
        }
    }
}