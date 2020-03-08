export class IngredientWithIsMatchDto {
    id: string;
    name: string;
    isMatch: boolean;
    constructor(id: string, name: string, isMatch: boolean) {
        this.id = id;
        this.name = name;
        this.isMatch = isMatch;
    }
}
