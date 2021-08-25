import { Box, Typography } from '@material-ui/core';
import TextField from '@material-ui/core/TextField';
import { Controller, SubmitHandler, useForm } from 'react-hook-form';
import { LinkButton } from '../common/LinkButton';
import { PrimaryButton } from '../common/PrimaryButton';
import { Logo } from './Logo';

import * as yup from 'yup';
import { yupResolver } from '@hookform/resolvers/yup';

type SignInFormProps = {
	signIn: Function;
};
type Inputs = {
	email: string;
	password: string;
};

const signInFormSchema = yup.object().shape({
	email: yup
		.string()
		.email('Dette er ikke en gyldig e-mail')
		.required('E-mail må ikke være tom'),
	password: yup
		.string()
		.min(8, 'Password skal være mindst 8 karakter langt')
		.required('Password må ikke være tomt'),
});

const SignInForm = (props: SignInFormProps) => {
	const { handleSubmit, control } = useForm<Inputs>({
		mode: 'onTouched',
		resolver: yupResolver(signInFormSchema),
	});
	const onSubmit: SubmitHandler<Inputs> = (data) =>
		props.signIn(data.email, data.password);
	return (
		<form onSubmit={handleSubmit(onSubmit)}>
			<Box padding={2}>
				<Logo />
				<Box>
					<Controller
						name="email"
						control={control}
						defaultValue=""
						render={({
							field: { onBlur, onChange, value },
							fieldState: { error },
						}) => (
							<TextField
								type="email"
								label="E-mail"
								variant="outlined"
								size="small"
								fullWidth
								value={value}
								onChange={onChange}
								onBlur={onBlur}
								error={!!error}
								helperText={error ? error.message : '*Påkrævet'}
							/>
						)}
					/>
				</Box>

				<Box mt={1}>
					<Controller
						name="password"
						control={control}
						defaultValue=""
						render={({
							field: { onBlur, onChange, value },
							fieldState: { error },
						}) => (
							<TextField
								type="password"
								label="Password"
								variant="outlined"
								size="small"
								fullWidth
								value={value}
								onChange={onChange}
								onBlur={onBlur}
								error={!!error}
								helperText={error ? error.message : '*Påkrævet'}
							/>
						)}
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
