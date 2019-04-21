import { Injectable } from "@angular/core";
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { throwError, Observable } from 'rxjs';
import { catchError,tap } from 'rxjs/operators';
import { IUserEquipmentViewModel } from './models/equipment';
import { Invoice } from './models/invoice';

@Injectable({
    providedIn:'root'
})

export class EquipmentService {

    userId : number=1;
     
    private serviceUrl='https://localhost:44354/api/UserEquipments/';

    constructor(private http: HttpClient){}


    getEquipments():Observable<IUserEquipmentViewModel[]>{
      return this.http.get<IUserEquipmentViewModel[]>(this.serviceUrl+'EquipmentList').pipe(
        tap(data=>console.log('All : '+JSON.stringify(data))),
        catchError(this.handleError)
      );
    }

    getEquipment(id:number):Observable<IUserEquipmentViewModel[]>{
        return this.http.get<IUserEquipmentViewModel[]>(this.serviceUrl+'EquipmentList',
        {
            headers: {'equipmentId':id.toString(),'UserId':this.userId.toString()}
         }).pipe(
          tap(data=>console.log('All : '+JSON.stringify(data))),
          catchError(this.handleError)
        );
      }

      getUserEquipmentCount():Observable<number>{
        return this.http.get<number>(this.serviceUrl+'UserEquipmentCount',
        {
            headers: {'UserId':this.userId.toString()}
         });
      }


      addEquipment(model:IUserEquipmentViewModel):Observable<boolean>{
       model.userId=this.userId;
        return this.http.post<boolean>(this.serviceUrl+'Add',model,
        {
            headers: {'UserId':this.userId.toString()}
         }).pipe(
          tap(data=>console.log('All : '+JSON.stringify(data))),
          catchError(this.handleError)
        );
      }


      removeEquipment(equipmentId:number):Observable<boolean>{
         return this.http.post<boolean>(this.serviceUrl+'RemoveEquipment',null,
         {
             headers: {'UserId':this.userId.toString(),"equipmentId":equipmentId.toString()}
          }).pipe(
           tap(data=>console.log('All : '+JSON.stringify(data))),
           catchError(this.handleError)
         );
       }

      getReport():Observable<Invoice>{
        return this.http.get<Invoice>(this.serviceUrl+'GenerateReport',
        {
            headers: {'userId':this.userId.toString()}
         }).pipe(
          tap(data=>console.log('All : '+JSON.stringify(data))),
          catchError(this.handleError)
        );
      }
    
    private handleError(err:HttpErrorResponse) {
        let errorMessage='';
        if(err.error instanceof ErrorEvent)
        errorMessage=`An error occured ${err.error.message}`;
        else 
        errorMessage=`Server returned  code ${err.status},error message is : ${err.message}`;
        console.log(errorMessage);
        return throwError(errorMessage);
    }
}
