import { createTheme, responsiveFontSizes } from '@material-ui/core/styles';
let theme = createTheme({
	typography: {
		fontFamily: ['Roboto', 'Jost'].join(','),
		h1: {
			fontFamily: 'Jost',
			fontWeight: 400, // Normal (Regular)
		},
		h2: {
			fontFamily: 'Jost',
			fontWeight: 400, // Normal (Regular)
		},
		h3: {
			fontFamily: 'Jost',
			fontWeight: 500, // Medium
		},
		h4: {
			fontFamily: 'Jost',
			fontWeight: 500, // Medium
		},
		h5: {
			fontFamily: 'Jost',
			fontWeight: 500, // Medium
		},
		h6: {
			fontFamily: 'Jost',
			fontWeight: 600, // Medium
		},
		subtitle1: {
			fontFamily: 'Roboto',
			fontWeight: 500, // Medium
		},
		subtitle2: {
			fontFamily: 'Roboto',
			fontWeight: 600, // Semi Bold (Demi Bold)
		},
		body1: {
			fontFamily: 'Roboto',
			fontWeight: 500, // Medium
		},
		body2: {
			fontFamily: 'Roboto',
			fontWeight: 500, // Medium
		},
		button: {
			fontFamily: 'Roboto',
			fontWeight: 600, // Semi Bold (Demi Bold)
		},
		caption: {
			fontFamily: 'Roboto',
			fontWeight: 500, // Medium
		},
		overline: {
			fontFamily: 'Roboto',
			fontWeight: 500, // Medium
		},
	},
	palette: {
		primary: {
			main: '#f8b808',
			contrastText: '#fff',
		},
		secondary: {
			main: '#5191b7',
			contrastText: '#fff',
		},
	},
});
theme = responsiveFontSizes(theme, { factor: 2 });
// theme.typography.h3 = {
// 	fontSize: theme.typography.pxToRem(55)
// }
export default theme;
