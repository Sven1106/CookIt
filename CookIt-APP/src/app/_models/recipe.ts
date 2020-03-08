import { Ingredient } from 'src/app/_models/ingredient';
import { Host } from './host';
export class Recipe {
    id: string;
    title: string;
    host: Host;
    url: string;
    imageUrl: string;
    ingredients: Ingredient[];
    matchedIngredients: Ingredient[];
}
