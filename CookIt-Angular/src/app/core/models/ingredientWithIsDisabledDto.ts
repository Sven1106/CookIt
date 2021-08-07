export class IngredientWithIsDisabledDto {
    id: string;
    name: string;
    isDisabled: boolean;

    constructor(id: string, name: string) {
        this.id = id;
        this.name = name;
        this.isDisabled = false;
    }
}
