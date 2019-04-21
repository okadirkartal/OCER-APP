import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule} from '@angular/forms'
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RouterModule,Routes } from '@angular/router';



import { EquipmentListComponent } from './equipment-list/equipment-list.component';
import { EquipmentReportComponent } from './equipment-report/equipment-report.component';
import { EquipmentCardComponent } from './equipment-card/equipment-card.component';
import { EquipmentService } from './equipment.service';

const routes : Routes =[
  { path:'equipment-list',  component:EquipmentListComponent }, 
  { path:'add-to-card/:id', component:EquipmentCardComponent },
  { path:'report', component:EquipmentReportComponent },    
  { path:'',redirectTo:'equipment-list',pathMatch:'full'},
  { path:'**',redirectTo:'equipment-list',pathMatch:'full'}
];

@NgModule({
  declarations: [
    AppComponent,
    EquipmentListComponent,
    EquipmentReportComponent,
    EquipmentCardComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,FormsModule,HttpClientModule,
    RouterModule.forRoot(routes)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { 
  pageTitle :string="Online Construction Equipment Rental";
 
}
