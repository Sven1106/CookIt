import { IngredientWithIsMatchDto } from './ingredientWithIsMatchDto';
import { Host } from './host';

export class RecipeForListDto {
    id: string;
    title: string;
    host: Host;
    url: string;
    imageUrl: string;
    imageSrc: string;
    isFavorite: boolean;
    ingredients: IngredientWithIsMatchDto[];
    matchedIngredientsCount: number;
    constructor(id: string, title: string, host: Host, url: string, imageUrl: string, imageSrc: string, isFavorite: boolean, ingredients: IngredientWithIsMatchDto[], matchedIngredientsCount: number) {
        this.id = id;
        this.title = title;
        this.host = host;
        this.url = url;
        this.imageUrl = imageUrl;
        this.imageSrc = imageSrc;
        this.isFavorite = isFavorite;
        this.ingredients = ingredients;
        this.matchedIngredientsCount = matchedIngredientsCount;
    }
};


