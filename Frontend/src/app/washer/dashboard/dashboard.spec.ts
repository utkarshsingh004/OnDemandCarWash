import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { Dashboard } from './dashboard';
import { AuthService } from '../../core/services/auth.service';
import { PlatformApiService } from '../../core/services/platform-api.service';

describe('Dashboard', () => {
  let component: Dashboard;
  let fixture: ComponentFixture<Dashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Dashboard],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: { getCurrentName: () => '', getCurrentUserId: () => '' } },
        { provide: PlatformApiService, useValue: {
          getPendingBookings: () => of([]),
          getMyAssignments: () => of([]),
          getWasherProfile: () => of({ washerId: '', name: '', email: '', phone: '', address: '', isAvailable: false })
        } }
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Dashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
