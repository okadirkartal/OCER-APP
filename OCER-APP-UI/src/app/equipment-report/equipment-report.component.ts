import { Component, OnInit } from '@angular/core';
import { EquipmentService } from '../equipment.service';
import { Invoice, InvoiceOfEquipment } from '../models/invoice';
import { of } from 'rxjs';

@Component({
  selector: 'app-equipment-report',
  templateUrl: './equipment-report.component.html',
  styleUrls: ['./equipment-report.component.css']
})
export class EquipmentReportComponent implements OnInit {

  pageTitle:string="Your Cards";
  errorMessage:string;
  invoice : Invoice;
  equipments : InvoiceOfEquipment[];

  constructor(private equipmentService : EquipmentService) { }

  ngOnInit() {
   this.equipmentService.getReport().subscribe(
      invoices=> {
        this.invoice=invoices
        this.equipments=invoices.equipments
      },
      error=>this.errorMessage=<any>error
    );
  }

  removeEquipment(equipmentId:number):void {
        let result;
        this.equipmentService.removeEquipment(equipmentId).subscribe(data=>result=data);
        console.log(result);
        location.href='report'; 
  }

  getInvoiceData() {
        let tempStr="";
        for(let i=0;i<this.equipments.length;i++) {
          tempStr+=`Invoice Number : ${this.equipments[i].invoiceId}\tInvoice Date : ${this.equipments[i].invoiceDate}\t`;
          tempStr+=`Equipment Type : ${this.equipments[i].equipmentType}\tEquipment Name : ${this.equipments[i].equipmentName}\t`;
          tempStr+=`Rental Days : ${this.equipments[i].equipmentRentalDay}\tRental Fee : ${this.equipments[i].equipmentRentalFee}\n\n`;
        }  
        tempStr+="\u000A";
        tempStr+=`\u000ALoyalty Point : ${this.invoice.loyaltPoint}\u000ATotal Fee ${this.invoice.totalPrice}`;

        return of({
         data: tempStr
        });    
  }

  dynamicDownloadTxt() {
   var date=new Date();
    this.getInvoiceData().subscribe((res) => {
      this.dyanmicDownloadByHtmlTag({
        fileName: `${date.getDay()}-${date.getMonth()}-${date.getFullYear()}_report.txt`,
        text: JSON.stringify(res.data)
      });
    });

  }


  private setting = { element : { dynamicDownload: null as HTMLElement}}

  private dyanmicDownloadByHtmlTag(arg: {
    fileName: string,
    text: string
    }) {
      if (!this.setting.element.dynamicDownload) {
        this.setting.element.dynamicDownload = document.createElement('a');
      }
    const element = this.setting.element.dynamicDownload;
    const fileType =  'text/plain';
    element.setAttribute('href', `data:${fileType};charset=utf-8,${encodeURIComponent(arg.text)}`);
    element.setAttribute('download', arg.fileName);

    var event = new MouseEvent("click");
    element.dispatchEvent(event);
  }
}
