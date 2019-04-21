import { Component } from '@angular/core';
import { EquipmentService } from './equipment.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Online Construction Equipment';
  equipmentCardCount:number=0;
  errorMessage:string;
  
  constructor(private equipmentService : EquipmentService){
    this.equipmentService.getUserEquipmentCount().subscribe(
      result=> {
        this.equipmentCardCount=result
      },
      error=>this.errorMessage=<any>error
    );
    console.log(this.equipmentCardCount);
  } 
}
