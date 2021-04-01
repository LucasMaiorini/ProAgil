import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  titulo = 'Login';
  model: any = {};
  constructor(
    private toastr: ToastrService,
    private authService: AuthService,
    public router: Router
  ) {}

  ngOnInit(): void {
    if (localStorage.getItem('token') !== null) {
      this.router.navigate(['/dashboard']);
    }
  }

  login(): void {
    this.authService.login(this.model).subscribe(
      () => {
        this.router.navigate(['/dashboard']);
      },
      (error) => {
        this.toastr.error(`Falha ao tentar logar.`);
      }
    );
  }

}
