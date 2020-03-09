import { IngredientWithIsMatchDto } from './ingredientWithIsMatchDto';
import { Host } from './host';

export class RecipeSearchResultDto {
    id: string;
    title: string;
    host: Host;
    url: string;
    imageUrl: string;
    imageSrc: string;
    ingredients: IngredientWithIsMatchDto[];
    matchedIngredientsCount: number;
    constructor(id: string, title: string, host: Host, url: string, imageUrl: string, imageSrc: string, ingredients: IngredientWithIsMatchDto[], matchedIngredientsCount: number) {
        this.id = id;
        this.title = title;
        this.host = host;
        this.url = url;
        this.imageUrl = imageUrl;
        this.imageSrc = imageSrc;
        this.ingredients = ingredients;
        this.matchedIngredientsCount = matchedIngredientsCount;
    }
};


