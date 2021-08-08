import { Link } from '@material-ui/core';
type LinkButtonProps = {
	color: 'primary' | 'textSecondary';
	children: string;
};

export const LinkButton = (props: LinkButtonProps) => {
	return (
		<Link
			color={props.color}
			component="button"
			variant="button"
			style={{ textTransform: 'none' }}
		>
			{props.children}
		</Link>
	);
};
