using System;
using System.Collections.Generic;

namespace Application.Infrastructure
{
    public class Invoice
    {
           public List<InvoiceOfEquipment> Equipments { get; set; }
        
           public decimal TotalPrice { get; set; }
        
           public int LoyaltPoint { get; set; }
    }

    public class InvoiceOfEquipment
    {
        public int InvoiceId { get; set; }

        public DateTime InvoiceDate { get; set; }
        
        public string EquipmentName { get; set; }
        
        public string EquipmentType { get; set; }
        
        public int EquipmentRentalDay { get; set; }
        
        public decimal EquipmentRentalFee { get; set; }

        public int EquipmentId { get; set; }
    }
}