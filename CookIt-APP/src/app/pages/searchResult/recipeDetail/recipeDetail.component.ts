import { ImageRequest } from 'src/app/_models/imageRequest';
import { ImageService } from './../../../_services/image.service';
import { RecipeSearchResultDto } from './../../../_models/recipeSearchResultDto';
import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { map } from 'rxjs/operators';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-recipeDetail',
  templateUrl: './recipeDetail.component.html',
  styleUrls: ['./recipeDetail.component.scss']
})

export class RecipeDetailComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<RecipeDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public recipe: RecipeSearchResultDto,
    public imageService: ImageService
  ) {


  }
  ngOnInit() {
    this.dialogRef.updatePosition({
      top: '25vh'
    });


  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  goToUrl(event: string) {
    window.open(event, '_system');
  }


}