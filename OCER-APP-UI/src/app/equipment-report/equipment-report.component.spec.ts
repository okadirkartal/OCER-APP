import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentReportComponent } from './equipment-report.component';

describe('EquipmentFormComponent', () => {
  let component: EquipmentReportComponent;
  let fixture: ComponentFixture<EquipmentReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EquipmentReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
