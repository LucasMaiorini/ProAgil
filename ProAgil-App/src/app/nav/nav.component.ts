import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  constructor(
    private toastr: ToastrService,
    public authService: AuthService,
    public router: Router
  ) { }

  ngOnInit(): void { }

  loggedIn(): boolean {
    return this.authService.loggedIn();
  }

  entrar(): void {
    this.router.navigate(['/user/login']);
  }

  logout(): void {
    localStorage.removeItem('token');
    this.toastr.show('Log Out');
    this.router.navigate(['/user/login']);
  }

  userName(): string {
    return sessionStorage.getItem('username');
  }
}
