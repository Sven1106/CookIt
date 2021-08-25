import { useState } from 'react';
import SignInForm from './SignInForm';
// import SignUpForm from './SignUpForm';
const Home = () => {
	const [isLoggedIn] = useState(false);
	const SignInHandler = (email: string, password: string) => {
		console.log(email, password);
	};
	// const SignUpHandler = (email: string, password: string) => {
	// 	console.log(email, password);
	// };
	if (isLoggedIn === false) {
		// return <SignUpForm signUp={SignUpHandler} />;
		return <SignInForm signIn={SignInHandler} />;
	}
	return <></>;
};
export default Home;
