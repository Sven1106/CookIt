// https://www.codingdeft.com/posts/react-form-validation/

const emailValidationRegex =
	/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
const danishPhoneNumberValidationRegex = /(?:45\s)?(?:\d{2}\s){3}\d{2}/;

export enum ValidationType {
	name = 'name',
	email = 'email',
	password = 'password',
	phone = 'phone',
	terms = 'terms',
}
export type validationField = {
	value: string;
	touched: boolean;
	hasError: boolean;
	error: string;
};

export const UPDATE_FORM = 'UPDATE_FORM';

export const onInputChange = (
	name: ValidationType,
	value: any,
	dispatch: any,
	formState: any
) => {
	const { hasError, error } = validateInput(name, value);

	let isFormValid = true;
	let property = formState[name];
	for (const key in formState) {
		const item = formState[key];
		if (key === name && hasError) {
			isFormValid = false;
			break;
		} else if (key !== name && item.hasError) {
			isFormValid = false;
			break;
		}
	}
	let isTouched = property.touched ?? false; // a property can never be untouched after it has been touched
	dispatch({
		type: UPDATE_FORM,
		data: { name, value, hasError, error, touched: isTouched, isFormValid },
	});
};
export const onFocusOut = (
	name: ValidationType,
	value: any,
	dispatch: any,
	formState: any
) => {
	const { hasError, error } = validateInput(name, value);
	let isFormValid = true;
	for (const key in formState) {
		let propertyName: string = ValidationType[name];
		const item = formState[key];
		if (key === propertyName && hasError) {
			isFormValid = false;
			break;
		} else if (key !== propertyName && item.hasError) {
			isFormValid = false;
			break;
		}
	}
	dispatch({
		type: UPDATE_FORM,
		data: { name, value, hasError, error, touched: true, isFormValid },
	});
};
export const validateInput = (
	inputType: ValidationType,
	value: string | number | boolean
) => {
	let hasError = false;
	let error = '';
	switch (inputType) {
		case ValidationType.name:
			if ((value as string).trim() === '') {
				hasError = true;
				error = 'Navn må ikke være tomt';
			}
			break;
		case ValidationType.email:
			if ((value as string).trim() === '') {
				hasError = true;
				error = 'E-mail kan ikke være tom';
			} else if (emailValidationRegex.test(value as string) === false) {
				hasError = true;
				error = 'Dette er ikke en gyldig e-mail';
			}
			break;
		case ValidationType.password:
			if ((value as string).length < 8) {
				hasError = true;
				error = 'Password skal være mindst 8 karakter langt';
			}
			break;
		case ValidationType.phone:
			if ((value as string).trim() === '') {
				hasError = true;
				error = 'Telefon må ikke være tomt';
			} else if (
				danishPhoneNumberValidationRegex.test(value as string) === false
			) {
				hasError = true;
				error = 'Dette er ikke et gyldigt telefonnummer';
			} else {
				hasError = false;
				error = '';
			}
			break;
		case ValidationType.terms:
			if ((value as boolean) === false) {
				hasError = true;
				error = 'Bekræft at du har læst betingelserne';
			} else {
				hasError = false;
				error = '';
			}
			break;
		default:
			break;
	}
	return { hasError, error };
};
