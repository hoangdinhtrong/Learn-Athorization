import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import { LoginResponse, OidcSecurityService } from 'angular-auth-oidc-client';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'app-angular';

  private accessToken?: string;

  constructor(
    public oidcSecurityService: OidcSecurityService,
    public http: HttpClient
  ) { }

  ngOnInit() {
    this.oidcSecurityService
      .checkAuth()
      .subscribe((loginResponse: LoginResponse) => {
        this.accessToken = loginResponse.accessToken;
        console.log(this.accessToken);
      });
  }

  login() {
    this.oidcSecurityService.authorize();
  }

  callApi() {
    // const token = this.oidcSecurityService
    //   .getAccessToken()
    //   .subscribe((token) => {
    //     const httpOptions = {
    //       headers: new HttpHeaders({
    //         Authorization: 'Bearer ' + token,
    //       }),
    //     };

    //     this.http
    //       .get('https://localhost:7174/secret', {
    //         headers: new HttpHeaders({
    //           Authorization: 'Bearer ' + token,
    //         }),
    //         responseType: 'text',
    //       })
    //       .subscribe((data: any) => {
    //         console.log('api result:', data);
    //       });
    //   });

    this.http
      .get('https://localhost:7174/secret', {
        headers: new HttpHeaders({
          Authorization: 'Bearer ' + this.accessToken,
        }),
        responseType: 'text',
      })
      .subscribe((data: any) => {
        console.log('api result:', data);
      });
  }
}
