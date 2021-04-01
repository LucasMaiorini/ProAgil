import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/models/User';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss'],
})
export class RegistrationComponent implements OnInit {
  registerForm!: FormGroup;
  user!: User;

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private authService: AuthService,
    public router: Router
  ) {}

  ngOnInit(): void {
    this.validation();
  }

  validation(): void {
    this.registerForm = this.fb.group({
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      userName: ['', Validators.required],
      passwords: this.fb.group(
        {
          password: ['', [Validators.required, Validators.minLength(4)]],
          confirmPassword: ['', Validators.required],
        },
        { validator: this.comparePasswords }
      ),
    });
  }

  comparePasswords(fb: FormGroup): void {
    const confirmPasswordCtrl = fb.get('confirmPassword');
    if (
      confirmPasswordCtrl?.errors == null ||
      'mismatch' in confirmPasswordCtrl.errors
    ) {
      if (fb.get('password')?.value !== confirmPasswordCtrl?.value) {
        confirmPasswordCtrl?.setErrors({ mismatch: true });
      } else {
        confirmPasswordCtrl?.setErrors(null);
      }
    }
  }

  registerUser(): void {
    if (this.registerForm.valid) {
      this.user = Object.assign(
        {
          password: this.registerForm.get('passwords.password')?.value,
        },
        this.registerForm.value
      );

      this.authService.register(this.user).subscribe(
        () => {
          this.router.navigate(['/user/login']);
          this.toastr.success('Cadastrado realizado!');
        },
        (error) => {
          error.error.forEach((element: any) => {
            switch (element.code) {
              case 'DuplicateUserName':
                this.toastr.error('UserName já cadastrado!');
                break;

              default:
                this.toastr.error(`Erro no cadatro! Code: ${element.code}`);
                break;
            }
          });
        }
      );
    }
  }
}
