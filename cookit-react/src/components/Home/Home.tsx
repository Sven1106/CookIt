import { useState } from 'react';
import SignInForm from './SignInForm';
const Home = () => {
	const [isLoggedIn] = useState(false);

	if (isLoggedIn === false) {
		return <SignInForm></SignInForm>;
	}
	return <></>;
};
export default Home;
