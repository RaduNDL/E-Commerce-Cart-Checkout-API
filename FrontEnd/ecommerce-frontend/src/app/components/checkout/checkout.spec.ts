import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CheckoutComponent } from './checkout';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { CartService } from '../../services/cart';
import { AuthService } from '../../services/auth';

class MockCartService {
  items = [];
  totalPrice = 0;
  clearCart() {}
}

class MockAuthService {
  isLoggedIn = true;
  currentUser = { userId: '1', token: 'fake-token' };
}

describe('CheckoutComponent', () => {
  let component: CheckoutComponent;
  let fixture: ComponentFixture<CheckoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CheckoutComponent],
      providers: [
        provideHttpClient(),
        provideRouter([]),
        { provide: CartService, useClass: MockCartService },
        { provide: AuthService, useClass: MockAuthService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CheckoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});