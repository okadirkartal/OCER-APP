import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EquipmentService } from '../equipment.service';
import { IUserEquipmentViewModel } from '../models/equipment'; 

@Component({
  selector: 'app-equipment-card',
  templateUrl: './equipment-card.component.html',
  styleUrls: ['./equipment-card.component.css']
})
export class EquipmentCardComponent implements OnInit {

  pageTitle:string='Equipment Detail';
  equipment : IUserEquipmentViewModel[]=[];
  errorMessage:string;
  userRentalDay:number;

  constructor(private route : ActivatedRoute,
    private equipmentService : EquipmentService) {
   }

  ngOnInit() {
    let id:number=+this.route.snapshot.paramMap.get('id');
    this.equipmentService.getEquipment(id).subscribe(
      equipments=> {
        this.equipment=equipments
      },
      error=>this.errorMessage=<any>error
    );
  }

  addEquipment() {
  let model=this.equipment[0];
  model.userRentalDay=this.userRentalDay;
  if(this.userRentalDay<=0 || isNaN(this.userRentalDay)) { 
    console.log(this.userRentalDay);   
   alert('Please enter a number for rent day');
   return;
  }
   let  result ;
   this.equipmentService.addEquipment(model).subscribe(
    data=> {
      result=data
    }); 
    location.href='equipment-list'; 
  }

}
