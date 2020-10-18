import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit {

  loginForm: FormGroup;   // <--- form group
  returnUrl: string;

  constructor(private accountService: AccountService, private router: Router, 
              private activatedRoute: ActivatedRoute) { }

  ngOnInit() {
    this.returnUrl = this.activatedRoute.snapshot.queryParams.returnUrl || '/shop';

    this.createLoginForm();   // create the form
  }

  createLoginForm() {

    this.loginForm = new FormGroup({

      // Create FormControls for email and for password
      //   (see login.component.html  -> formControlName="email")
      // Note: FormControls come with built-in validators
      //     (for reg expression see  www.regexlib.com and search for email)
      email: new FormControl('', [Validators.required, 
        Validators.pattern('^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$')]),

      password: new FormControl('', Validators.required)
    });
  }

  onSubmit() {

    // call the login method in account.service.ts sending the 'loginForm' FormGroup value
    this.accountService.login(this.loginForm.value).subscribe(() => {
      //console.log('user logged in');
      this.router.navigateByUrl(this.returnUrl);
    }, error => {
      console.log(error);
    });
  }

}