import { Component, Input, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit {
user: User;
galleryOptions: NgxGalleryOptions[];
galleryImages: NgxGalleryImage[];

  constructor(private userService: UserService, 
    private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    //this.loadUser(); instead we should use route to avoid safe navigation in html
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });

    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview:false
      }]

      this.galleryImages = this.getImages();
  }

  getImages() {
    const imageUrls = [];
    for (const photo of this.user.photos) {
      imageUrls.push({
        small: photo.url,
        medium: photo.url,
        big: photo.url,
        description: photo.description
      });
    }
    return imageUrls;
  }

  //so this will be disabled and instead we use resolver
  // loadUser() {
  //   this.userService.getUser(+this.route.snapshot.params['id'])
  //   .subscribe((user: User) => {
  //     this.user= user
  //   }, error => {
  //     this.alertify.error(error);
  //   })
  // }

}
