import { Component, OnInit } from '@angular/core';
import { IUserEquipmentViewModel } from '../models/equipment';
import { EquipmentService } from '../equipment.service';

@Component({
  selector: 'app-equipment-list',
  templateUrl: './equipment-list.component.html',
  styleUrls: ['./equipment-list.component.css']
})
export class EquipmentListComponent implements OnInit {
  pageTitle:string='Equipment List';
  errorMessage:string;
  equipments : IUserEquipmentViewModel[]=[];

  constructor(private equipmentService : EquipmentService) {

  }

  ngOnInit() {
    this.equipmentService.getEquipments().subscribe(
      equipments=> {
        this.equipments=equipments
      },
      error=>this.errorMessage=<any>error
    );
  }
}
