import { Box, Typography } from '@material-ui/core';
import TextField from '@material-ui/core/TextField';
import React, { useReducer } from 'react';
import {
	onFocusOut,
	onInputChange,
	UPDATE_FORM,
	ValidationType,
} from '../../utils/FormUtils';
import { LinkButton } from '../common/LinkButton';
import { PrimaryButton } from '../common/PrimaryButton';
import { Logo } from './Logo';

const formsReducer = (state: validationFormState, action: validationAction) => {
	switch (action.type) {
		case UPDATE_FORM:
			const { name, value, hasError, error, touched, isFormValid } =
				action.data;
			return {
				...state,
				[name]: {
					...(state[name as keyof validationFormState] as object),
					value,
					hasError,
					error,
					touched,
				},
				isFormValid,
			};

		default:
			return state;
	}
};

type validationField = {
	value: string;
	touched: boolean;
	hasError: boolean;
	error: string;
};

type validationAction = {
	type: string;
	data: validationData;
};
type validationData = {
	name: string;
	value: any;
	touched: boolean;
	hasError: boolean;
	error: string;
	isFormValid: boolean;
};

type validationFormState = {
	email: validationField;
	password: validationField;
	isFormValid: boolean;
};

const SignInForm = () => {
	const initialState: validationFormState = {
		email: { value: '', touched: false, hasError: false, error: '' },
		password: { value: '', touched: false, hasError: false, error: '' },
		isFormValid: false,
	};
	const [state, dispatch] = useReducer(formsReducer, initialState);

	const formSubmitHandler = (e: React.FormEvent<HTMLFormElement>) => {
		e.preventDefault();

		// for (const propertyName in state) {

		// 	const item = state[propertyName];
		// 	const { value } = item;
		// 	const inputType: ValidationType =
		// 		ValidationType[propertyName as keyof typeof ValidationType];
		// 	const { hasError, error } = validateInput(inputType, value);

		// 	if (propertyName) {
		// 		dispatch({
		// 			type: UPDATE_FORM,
		// 			data: {
		// 				name: propertyName,
		// 				value,
		// 				hasError,
		// 				error,
		// 				touched: true,
		// 				isFormValid: state.isFormValid,
		// 			},
		// 		});
		// 	}
		// }
		if (state.isFormValid === false) {
			console.log('Form not valid');
		} else {
			//callback
		}
	};
	return (
		<form
			onSubmit={formSubmitHandler}
			onInvalidCapture={(e) => e.preventDefault()}
		>
			<Box padding={2}>
				<Logo />
				<Box>
					<TextField
						id="email"
						type="email"
						label="E-mail"
						variant="outlined"
						size="small"
						helperText={
							state.email.hasError && state.email.touched
								? state.email.error
								: '*Påkrævet'
						}
						error={state.email.hasError && state.email.touched}
						value={state.email.value}
						fullWidth
						onChange={(e) => {
							onInputChange(
								ValidationType.email,
								e.target.value,
								dispatch,
								state
							);
						}}
						onBlur={(e) => {
							onFocusOut(ValidationType.email, e.target.value, dispatch, state);
						}}
					/>
				</Box>

				<Box mt={1}>
					<TextField
						id="password"
						type="password"
						label="Password"
						variant="outlined"
						size="small"
						helperText={
							state.password.hasError && state.password.touched
								? state.password.error
								: '*Påkrævet'
						}
						error={state.password.hasError && state.password.touched}
						value={state.password.value}
						fullWidth
						onChange={(e) => {
							onInputChange(
								ValidationType.password,
								e.target.value,
								dispatch,
								state
							);
						}}
						onBlur={(e) => {
							onFocusOut(
								ValidationType.password,
								e.target.value,
								dispatch,
								state
							);
						}}
					/>
				</Box>

				<Box display="flex" justifyContent="flex-end" mb={4}>
					<LinkButton color="textSecondary">Glemt password?</LinkButton>
				</Box>

				<Box>
					<PrimaryButton type="submit">Log ind</PrimaryButton>
				</Box>

				<Box display="flex" flexDirection="column" alignItems="center" mt={3}>
					<Typography color="textSecondary" variant="body2">
						Har du ikke en bruger?
					</Typography>
					<LinkButton color="primary">Opret bruger</LinkButton>
				</Box>
			</Box>
		</form>
	);
};
export default SignInForm;
