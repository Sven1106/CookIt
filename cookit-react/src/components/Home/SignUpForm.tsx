 import { Box, Typography } from '@material-ui/core';
// import TextField from '@material-ui/core/TextField';
// import { useReducer } from 'react';
// import {
// 	onFocusOut,
// 	onInputChange,
// 	UpdateField,
// 	validateValue,
// 	validationAction,
// 	validationField,
// 	ValidationType,
// } from '../../utils/FormUtils';
// import { LinkButton } from '../common/LinkButton';
// import { PrimaryButton } from '../common/PrimaryButton';
// import { Logo } from './Logo';

// type signUpFormState = {
// 	name: validationField;
// 	email: validationField;
// 	password: validationField;
// 	repassword: validationField;
// };

// export const formReducer = (
// 	state: signUpFormState,
// 	action: validationAction
// ) => {
// 	switch (action.type) {
// 		case UpdateField:
// 			const { name, value, error, touched } = action.data;
// 			return {
// 				...state,
// 				[name]: {
// 					...(state[name as keyof signUpFormState] as validationField),
// 					value,
// 					error,
// 					touched,
// 				},
// 			};

// 		default:
// 			return state;
// 	}
// };
// const initialState: signUpFormState = {
// 	name: { value: '', touched: false, error: '' },
// 	email: { value: '', touched: false, error: '' },
// 	password: { value: '', touched: false, error: '' },
// 	repassword: { value: '', touched: false, error: '' },
// };

// type SignUpFormProps = {
// 	signUp: Function;
// };

// const SignUpForm = (props: SignUpFormProps) => {
// 	const [formState, dispatch] = useReducer(formReducer, initialState);
// 	const formSubmitHandler = (e: React.FormEvent<HTMLFormElement>) => {
// 		e.preventDefault();
// 		let isFormValid = true;
// 		for (const propertyName in formState) {
// 			const key = propertyName as keyof typeof formState;
// 			const { value } = formState[key] as validationField;
// 			const validationType =
// 				ValidationType[propertyName as keyof typeof ValidationType];
// 			const { error } = validateValue(validationType, value);

// 			if (propertyName) {
// 				dispatch({
// 					type: UpdateField,
// 					data: {
// 						name: propertyName,
// 						value,
// 						error,
// 						touched: true,
// 					},
// 				});
// 				if (error.length > 0) {
// 					isFormValid = false;
// 				}
// 			}
// 		}

// 		if (isFormValid === true) {
// 			props.signUp(formState.email.value, formState.password.value);
// 		} else {
// 			console.log('NOT VALID');
// 		}
// 	};
// 	return (
// 		<form
// 			onSubmit={formSubmitHandler}
// 			onInvalidCapture={(e) => e.preventDefault()}
// 		>
// 			<Box padding={2}>
// 				<Logo title="Opret bruger" />
// 				<Box>
// 					<TextField
// 						id="name"
// 						type="name"
// 						label="Navn"
// 						variant="outlined"
// 						size="small"
// 						helperText={
// 							formState.name.error.length > 0 && formState.name.touched
// 								? formState.name.error
// 								: '*Påkrævet'
// 						}
// 						error={formState.name.error.length > 0 && formState.name.touched}
// 						value={formState.name.value}
// 						fullWidth
// 						onChange={(e) => {
// 							onInputChange(
// 								ValidationType.name,
// 								e.target.value,
// 								dispatch,
// 								formState.name.touched
// 							);
// 						}}
// 						onBlur={(e) => {
// 							onFocusOut(ValidationType.name, e.target.value, dispatch);
// 						}}
// 					/>
// 				</Box>
// 				<Box>
// 					<TextField
// 						id="email"
// 						type="email"
// 						label="E-mail"
// 						variant="outlined"
// 						size="small"
// 						helperText={
// 							formState.email.error.length > 0 && formState.email.touched
// 								? formState.email.error
// 								: '*Påkrævet'
// 						}
// 						error={formState.email.error.length > 0 && formState.email.touched}
// 						value={formState.email.value}
// 						fullWidth
// 						onChange={(e) => {
// 							onInputChange(
// 								ValidationType.email,
// 								e.target.value,
// 								dispatch,
// 								formState.email.touched
// 							);
// 						}}
// 						onBlur={(e) => {
// 							onFocusOut(ValidationType.email, e.target.value, dispatch);
// 						}}
// 					/>
// 				</Box>

// 				<Box>
// 					<TextField
// 						id="password"
// 						type="password"
// 						label="Password"
// 						variant="outlined"
// 						size="small"
// 						helperText={
// 							formState.password.error.length > 0 && formState.password.touched
// 								? formState.password.error
// 								: '*Påkrævet'
// 						}
// 						error={
// 							formState.password.error.length > 0 && formState.password.touched
// 						}
// 						value={formState.password.value}
// 						fullWidth
// 						onChange={(e) => {
// 							onInputChange(
// 								ValidationType.password,
// 								e.target.value,
// 								dispatch,
// 								formState.password.touched
// 							);
// 						}}
// 						onBlur={(e) => {
// 							onFocusOut(ValidationType.password, e.target.value, dispatch);
// 						}}
// 					/>
// 				</Box>
// 				<Box>
// 					<TextField
// 						id="repassword"
// 						type="password"
// 						label="Bekræft password"
// 						variant="outlined"
// 						size="small"
// 						helperText={
// 							formState.repassword.error.length > 0 && formState.repassword.touched
// 								? formState.repassword.error
// 								: '*Påkrævet'
// 						}
// 						error={
// 							formState.repassword.error.length > 0 && formState.repassword.touched
// 						}
// 						value={formState.repassword.value}
// 						fullWidth
// 						onChange={(e) => {
// 							onInputChange(
// 								ValidationType.repassword,
// 								e.target.value,
// 								dispatch,
// 								formState.repassword.touched
// 							);
// 						}}
// 						onBlur={(e) => {
// 							onFocusOut(ValidationType.repassword, e.target.value, dispatch);
// 						}}
// 					/>
// 				</Box>
// 				<Box>
// 					<PrimaryButton type="submit">Opret bruger</PrimaryButton>
// 				</Box>
// 			</Box>
// 		</form>
// 	);
// };
// export default SignUpForm;
