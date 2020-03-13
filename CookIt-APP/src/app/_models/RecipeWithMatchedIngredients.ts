import { Ingredient } from 'src/app/_models/ingredient';
import { Host } from './host';
export class RecipeWithMatchedIngredients {
    id: string;
    title: string;
    host: Host;
    url: string;
    imageUrl: string;
    isFavorite: boolean;
    ingredients: Ingredient[];
    matchedIngredients: Ingredient[];
    constructor() {

    }
}
