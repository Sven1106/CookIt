// https://www.codingdeft.com/posts/react-form-validation/

export type validationAction = {
	type: string;
	data: validationData;
};
export type validationData = {
	name: string;
	value: any;
	touched: boolean;
	error: string;
};

export type validationField = {
	value: string;
	touched: boolean;
	error: string;
};

export const UpdateField = 'UpdateField';

export const onInputChange = (
	name: string,
	validationFunction: Function,
	value: string,
	dispatch: React.Dispatch<validationAction>,
	isPropertyTouched: boolean
) => {
	const error = validationFunction();
	dispatch({
		type: UpdateField,
		data: { name: name, value, error, touched: isPropertyTouched },
	});
};

export const onFocusOut = (
	name: string,
	validationFunction: any,
	value: string,
	dispatch: React.Dispatch<validationAction>
) => {
	const error = validationFunction();
	dispatch({
		type: UpdateField,
		data: { name: name, value, error, touched: true },
	});
};

export enum ValidationType {
	name = 'name',
	email = 'email',
	password = 'password',
	repassword = 'repassword',
	phone = 'phone',
	terms = 'terms',
}

const emailValidationRegex =
	/^(([^<>()\]\\.,;:\s@"]+(\.[^<>()\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
const danishPhoneNumberValidationRegex = /(?:45\s)?(?:\d{2}\s){3}\d{2}/;

export const validateName = (value: string) => {
	let error = '';
	if (value.trim() === '') {
		error = 'Navn må ikke være tomt';
	}
	return error;
};
export const validateEmail = (value: string) => {
	let error = '';
	if (value.trim() === '') {
		error = 'E-mail må ikke være tom';
	} else if (emailValidationRegex.test(value) === false) {
		error = 'Dette er ikke en gyldig e-mail';
	}
	return error;
};
export const validatePassword = (value: string) => {
	let error = '';
	if (value.length < 8) {
		error = 'Password skal være mindst 8 karakter langt';
	}
	return error;
};
export const validateRepassword = (
	value: string,
	passwordToCompareTo: string
) => {
	let error = '';
	if (value !== passwordToCompareTo) {
		error = 'Passwords matcher ikke';
	}
	return error;
};
export const validatePhoneNumber = (value: string) => {
	let error = '';
	if (value.trim() === '') {
		error = 'Telefon må ikke være tom';
	} else if (danishPhoneNumberValidationRegex.test(value) === false) {
		error = 'Dette er ikke et gyldigt telefonnummer';
	}
	return error;
};
export const validateTerms = (value: boolean) => {
	let error = '';
	if (value !== true) {
		error = 'Bekræft at du har læst betingelserne';
	}
	return error;
};
