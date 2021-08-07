import { Ingredient } from '@core/models/ingredient';
import { Host } from '@core/models/host';

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
