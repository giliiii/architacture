import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CardManage } from './card-manage';

describe('CardManage', () => {
  let component: CardManage;
  let fixture: ComponentFixture<CardManage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CardManage]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CardManage);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
