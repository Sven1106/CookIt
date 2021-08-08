import { ThemeProvider } from '@material-ui/styles';
import './App.css';
import Home from './components/Home/Home';
import theme from './styles/Theme';

function App() {
	return (
		<ThemeProvider theme={theme}>
			<Home />
		</ThemeProvider>
	);
}

export default App;
