import { Button } from '@material-ui/core';

type PrimaryButtonProps = {
	onClick?: (event: React.MouseEvent<HTMLButtonElement>) => void;
	children: string;
	type?: 'submit' | 'reset' | 'button' ;
};
export const PrimaryButton = (props: PrimaryButtonProps) => {
	return (
		<Button
			type={props.type}
			variant="contained"
			color="primary"
			size="medium"
			fullWidth
			onClick={props.onClick}
		>
			{props.children}
		</Button>
	);
};
