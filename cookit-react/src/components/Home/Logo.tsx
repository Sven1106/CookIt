import { Box, Typography } from '@material-ui/core';
import logo from '../../assets/logo/logo.svg';

interface LogoProps {
	title?: string;
	subTitle?: string;
}

export const Logo = (props: LogoProps) => {
	const noneBreakingSpace = <>&nbsp;</>;
	return (
		<Box display="flex" alignItems="center" flexDirection="column">
			<Box mt={8}>
				<img style={{ width: '20vw' }} src={logo} alt="Logo" />
			</Box>
			<Box mb={10}>
				<Typography variant="h6" component="h2" color="textSecondary">
					{props.title ? props.title : noneBreakingSpace}
				</Typography>
			</Box>
			{props.subTitle && ( //TODO is this a code smell?
				<Box mb={6}>
					<Typography variant="subtitle2" color="textSecondary">
						{props.subTitle}
					</Typography>
				</Box>
			)}
		</Box>
	);
};
