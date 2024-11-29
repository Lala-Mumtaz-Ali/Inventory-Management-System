import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { TOKEN_KEY } from './constants';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http:HttpClient) { }

  login(formData:any){
    return this.http.post(environment.apiBaseUrl+'/login/authenticate', formData);
  }

  isLoggedIn(){
    console.log('LoggedIn\n');
    return this.getToken() != null ? true : false;
  }

  saveToken(token: string) {
    console.log('Saving token:', token); // Debugging token saving
    localStorage.setItem(TOKEN_KEY, token);
  }

  getToken() {
    const token = localStorage.getItem(TOKEN_KEY);
    console.log('Retrieved token:', token); // Debugging token retrieval
    return token;
  }

  removeToken(){
    localStorage.removeItem(TOKEN_KEY);
  }
}
