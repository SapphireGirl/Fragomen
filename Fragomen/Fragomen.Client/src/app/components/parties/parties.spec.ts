import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Parties } from './parties';

describe('Parties', () => {
  let component: Parties;
  let fixture: ComponentFixture<Parties>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [Parties]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Parties);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
