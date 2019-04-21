 
export interface Invoice {

    totalPrice:number;
    loyaltPoint:number;
    equipments : InvoiceOfEquipment[];
}

export interface InvoiceOfEquipment {
    invoiceId : number;
    invoiceDate : string;
    equipmentName : string;
    equipmentType:string;
    equipmentRentalDay:number;
    equipmentRentalFee:number;
    equipmentId:number;
}