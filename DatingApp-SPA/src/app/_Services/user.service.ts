import { HttpClient, HttpHeaders, HttpParams} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import {map} from 'rxjs/operators';

// const httpOptions = {
//   headers: new HttpHeaders({
//     'Authorization': 'Bearer ' + localStorage.getItem('token')
//   })
// }

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) {
 }

 //getUsers(): Observable<User[]>{
getUsers(page?, itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>>{
  const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
  
  let params = new HttpParams();

  if(page != null && itemsPerPage !=null){//created params to send custom params in the header
    params = params.append('pageNumber',page);
    params = params.append('pageSize', itemsPerPage);
  }

if(userParams != null){
  params = params.append('minAge', userParams.minAge);
  params = params.append('maxAge', userParams.maxAge);
  params = params.append('gender', userParams.gender);
  params = params.append('orderby', userParams.orderBy);
}

  //return this.http.get<User[]>(this.baseUrl + 'users');
  //add observe response to get the pagination params response from here 
  return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;//set the user items from the body
          if(response.headers.get('Pagination') != null){//get the pagination params pagination from header pagination
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      )
 }

 getUser(id: number): Observable<User>{
   return this.http.get<User>(this.baseUrl +'users/' + id);
 }

updateUser(id: number, user: User){
  return this.http.put(this.baseUrl + 'users/' + id, user);
}

setMainPhoto(userId: number, photoId: number){
  return this.http.post(this.baseUrl + 'users/'+ userId + '/photos/' + photoId + '/setMain', {});
}

deletePhoto(userId: number, photoId: number) {
  return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + photoId);
} 

}
